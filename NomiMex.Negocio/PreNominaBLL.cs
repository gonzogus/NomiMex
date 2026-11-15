using System;
using System.Collections.Generic;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Documentos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Pre-nómina por trabajador: calcula el pre-recibo con las incidencias activas, aplica los movimientos (agregar o quitar
    /// incidencias), guarda el borrador con sus notas y, al cerrar, persiste los recibos definitivos y cierra el periodo.
    /// En periodos ya procesados presenta los recibos guardados en modo de solo lectura.
    /// </summary>
    public class PreNominaBLL : BllBase
    {
        public const int LongitudMaximaNotas = 500;
        private const decimal ToleranciaTotales = 0.005m;

        private readonly PreNominaDAL _dal = new PreNominaDAL();
        private readonly NominaDAL _nominaDal = new NominaDAL();
        private readonly ReciboDAL _reciboDal = new ReciboDAL();
        private readonly PeriodoDAL _periodoDal = new PeriodoDAL();
        private readonly IncidenciaDAL _incidenciaDal = new IncidenciaDAL();
        private readonly EmpresaDAL _empresaDal = new EmpresaDAL();
        private readonly IncidenciaBLL _incidenciaBll = new IncidenciaBLL();
        private readonly ConceptoIncidenciaBLL _conceptoBll = new ConceptoIncidenciaBLL();
        private readonly ConceptoIncidenciaDAL _conceptoDal = new ConceptoIncidenciaDAL();
        private readonly PrerrequisitosNomina _prerrequisitos = new PrerrequisitosNomina();
        private readonly AsignacionPaqueteBLL _asignacionBll = new AsignacionPaqueteBLL();

        #region Consulta

        /// <summary>Unidad (DIA, HORA o MONTO) en que se captura el tipo de incidencia.</summary>
        public string UnidadDe(CatTipoIncidencia tipo)
        {
            return IncidenciaBLL.UnidadSugerida(tipo);
        }

        /// <summary>
        /// Pre-recibos de todos los trabajadores del periodo. En un periodo abierto se calculan con las incidencias activas;
        /// en uno ya procesado se leen los recibos guardados.
        /// </summary>
        public List<PreReciboDTO> ListarPreRecibos(int periodoId)
        {
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            if (!periodo.EstaAbierto)
            {
                return LeerRecibosProcesados(periodo, null);
            }

            _prerrequisitos.ExigirEmpleadosActivos(periodo.EmpresaId, periodo.PeriodicidadPagoId);
            return CalcularPeriodoAbierto(periodo, null, null);
        }

        /// <summary>
        /// Pre-recibo de un trabajador con sus incidencias activas. Las notas en edición (aún sin guardar) tienen prioridad
        /// sobre las guardadas; si es null se usan las del borrador.
        /// </summary>
        public PreReciboDTO ObtenerPreRecibo(int periodoId, int empleadoId, string notasEnEdicion)
        {
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            PreReciboDTO pre = periodo.EstaAbierto
                ? CalcularPeriodoAbierto(periodo, empleadoId, notasEnEdicion).FirstOrDefault()
                : LeerRecibosProcesados(periodo, empleadoId).FirstOrDefault();

            if (pre == null)
            {
                throw new ExcepcionNegocio("El trabajador ya no pertenece a la nómina de este periodo.");
            }
            pre.Incidencias = Ejecutar(() => _incidenciaDal.ListarActivasPorEmpleado(periodoId, empleadoId));
            if (periodo.EstaAbierto)
                foreach (PreReciboConceptoDTO c in pre.Percepciones.Concat(pre.Deducciones))
                    c.IdsIncidenciasLegales = pre.Incidencias
                        .Where(i => c.CodigosOrigen != null && c.CodigosOrigen.Contains(i.Codigo))
                        .Select(i => i.IncidenciaId).ToList();
            return pre;
        }

        #endregion

        #region Incidencias

        /// <summary>
        /// Cambia los días que el trabajador labora por semana (1 a 7; 7 = jornada completa). Con <paramref name="fijo"/> el valor
        /// queda en el contrato (null = según la jornada); si no, aplica solo al periodo (null quita el ajuste del periodo).
        /// </summary>
        public PreReciboDTO CambiarDiasSemana(int periodoId, int empleadoId, int? dias, bool fijo, string notas)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Escribir);
            if (fijo) ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            if (dias.HasValue && (dias.Value < 1 || dias.Value > 7))
                throw new ExcepcionNegocio("Los días laborados por semana deben estar entre 1 y 7.");

            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            if (!periodo.EstaAbierto)
                throw new ExcepcionNegocio("El periodo ya no admite cambios.");

            if (fijo)
            {
                bool actualizado = Ejecutar(() => _dal.GuardarDiasSemanaContrato(periodoId, empleadoId, dias));
                if (!actualizado) throw new ExcepcionNegocio("El trabajador no tiene un contrato vigente.");
            }
            else
            {
                Ejecutar(() => _dal.GuardarDiasSemanaPeriodo(periodoId, empleadoId, dias));
            }
            return ObtenerPreRecibo(periodoId, empleadoId, notas);
        }

        public PreReciboDTO QuitarConcepto(int periodoId, int empleadoId, PreReciboConceptoDTO concepto, string notas, bool tambienPermanente = false)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Escribir);
            if (concepto == null) throw new ExcepcionNegocio("Seleccione un concepto.");
            if (concepto.IdIncidenciaConcepto.HasValue)
                _conceptoBll.Quitar(periodoId, empleadoId, concepto.IdIncidenciaConcepto.Value, tambienPermanente);
            else if (concepto.IdsIncidenciasLegales != null && concepto.IdsIncidenciasLegales.Count > 0)
                Ejecutar(() => _incidenciaDal.CancelarGrupo(periodoId, empleadoId, concepto.IdsIncidenciasLegales));
            else
                throw new ExcepcionNegocio("No se puede quitar un concepto automático base.");
            return ObtenerPreRecibo(periodoId, empleadoId, notas);
        }

        /// <summary>Registra una incidencia del trabajador y devuelve su pre-recibo recalculado. El aviso (o null) es informativo.</summary>
        public PreReciboDTO AgregarIncidencia(int periodoId, int empleadoId, CatTipoIncidencia tipo, decimal cantidad, DateTime fecha,
                                              string observaciones, string notasEnEdicion, out string aviso)
        {
            aviso = _incidenciaBll.Registrar(periodoId, empleadoId, tipo, cantidad, UnidadDe(tipo), fecha, observaciones);
            return ObtenerPreRecibo(periodoId, empleadoId, notasEnEdicion);
        }

        /// <summary>Cancela una incidencia activa del trabajador y devuelve su pre-recibo recalculado.</summary>
        public PreReciboDTO QuitarIncidencia(int periodoId, int empleadoId, int incidenciaId, string notasEnEdicion)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Escribir);
            Validador.SeleccionRequerida(incidenciaId, "Concepto");

            bool pertenece = Ejecutar(() => _incidenciaDal.ListarActivasPorEmpleado(periodoId, empleadoId))
                .Any(i => i.IncidenciaId == incidenciaId);
            if (!pertenece)
            {
                throw new ExcepcionNegocio("El concepto ya no está activo para este trabajador.");
            }

            Ejecutar(() => _incidenciaDal.CancelarGrupo(periodoId, empleadoId, new[] { incidenciaId }));
            return ObtenerPreRecibo(periodoId, empleadoId, notasEnEdicion);
        }

        #endregion

        #region Guardado y vista previa

        /// <summary>Texto de notas listo para guardar: sin espacios sobrantes, máximo 500 caracteres y null si está vacío.</summary>
        public static string NormalizarNotas(string notas)
        {
            return Validador.Opcional(notas, "Notas del recibo", LongitudMaximaNotas);
        }

        /// <summary>Guarda el pre-recibo del trabajador (conceptos, totales y notas) como borrador del periodo abierto.</summary>
        public PreReciboDTO GuardarPreRecibo(int periodoId, int empleadoId, string notas)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Escribir);
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            if (!periodo.EstaAbierto)
            {
                throw new ExcepcionNegocio("El periodo está en estatus " + periodo.Estatus + " y sus recibos ya no pueden modificarse.");
            }

            string notasValidas = NormalizarNotas(notas);
            PreReciboDTO pre = CalcularPeriodoAbierto(periodo, empleadoId, notasValidas ?? string.Empty).FirstOrDefault();
            if (pre == null)
            {
                throw new ExcepcionNegocio("El trabajador ya no pertenece a la nómina de este periodo.");
            }

            pre.Notas = notasValidas;
            Ejecutar(() => _dal.GuardarBorrador(pre));
            Ejecutar(() => _periodoDal.MarcarPrenomina(periodoId));
            return ObtenerPreRecibo(periodoId, empleadoId, null);
        }

        /// <summary>Recibo del trabajador listo para vista previa e impresión, con las notas indicadas (se imprimen solo si traen texto).</summary>
        public ReciboImpresion ObtenerVistaPrevia(int periodoId, int empleadoId, string notasEnEdicion)
        {
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            PreReciboDTO pre = ObtenerPreRecibo(periodoId, empleadoId, notasEnEdicion);
            CatEmpresa empresa = Ejecutar(() => _empresaDal.Listar(false)).FirstOrDefault(e => e.EmpresaId == periodo.EmpresaId);

            ReciboImpresion impresion = new ReciboImpresion
            {
                Empresa = empresa != null ? empresa.RazonSocial : periodo.Empresa,
                EmpresaRFC = empresa != null ? empresa.RFC : null,
                RegistroPatronal = empresa != null ? empresa.RegistroPatronal : null,
                EmpresaLogo = empresa != null ? Ejecutar(() => _empresaDal.ObtenerLogo(empresa.EmpresaId)) : null,
                NumeroEmpleado = pre.NumeroEmpleado,
                NombreEmpleado = pre.NombreCompleto,
                EmpleadoRFC = pre.RFC,
                EmpleadoCURP = pre.CURP,
                EmpleadoNSS = pre.NSS,
                NumeroPeriodo = periodo.NumeroPeriodo,
                TipoPeriodo = periodo.TipoPeriodo,
                FechaInicioPeriodo = periodo.FechaInicio,
                FechaFinPeriodo = periodo.FechaFin,
                FechaPagoPeriodo = periodo.FechaPago,
                EstatusPeriodo = periodo.Estatus,
                Recibo = ConstruirCabecera(pre, pre.EsEditable ? EstatusRecibo.Prerecibo : EstatusRecibo.Calculado)
            };
            impresion.Recibo.FechaCalculo = DateTime.Now;
            impresion.Detalles = ConstruirDetalles(pre);
            return impresion;
        }

        #endregion

        #region Cierre del periodo

        /// <summary>
        /// Persiste los recibos definitivos de todos los trabajadores del periodo, lo autoriza y lo cierra en una sola transacción.
        /// Las notas en edición (por empleado) se guardan junto con cada recibo. Si el periodo ya está CALCULADO o AUTORIZADO,
        /// solo se autoriza (si hace falta) y se cierra.
        /// </summary>
        public ResultadoCierreNomina CerrarNomina(int periodoId, IDictionary<int, string> notasPorEmpleado)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Autorizar);
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            int? usuarioId = SesionActual.HaySesion ? SesionActual.Usuario.UsuarioId : (int?)null;
            string comentario = SesionActual.HaySesion ? "Cierre desde NomiMex por " + SesionActual.Usuario.NombreUsuario : null;

            if (periodo.Estatus == EstatusPeriodo.Calculado || periodo.Estatus == EstatusPeriodo.Autorizado)
            {
                if (periodo.SumaNetoPagar <= 0m)
                {
                    throw new ExcepcionNegocio("El neto total del periodo debe ser mayor a cero para cerrarlo.");
                }
                bool autorizar = periodo.Estatus == EstatusPeriodo.Calculado;
                Ejecutar(() => _dal.CerrarPeriodoProcesado(periodoId, autorizar, usuarioId, comentario));
                return ResultadoDe(periodoId, ObtenerPeriodo(periodoId).Estatus, LeerRecibosProcesados(periodo, null));
            }

            if (!periodo.EstaAbierto)
            {
                throw new ExcepcionNegocio("El periodo tiene estatus " + periodo.Estatus + " y no puede cerrarse.");
            }

            _prerrequisitos.ExigirEmpleadosActivos(periodo.EmpresaId, periodo.PeriodicidadPagoId);
            List<PreReciboDTO> recibos = CalcularPeriodoAbierto(periodo, null, null);
            if (recibos.Count == 0)
            {
                throw new ExcepcionNegocio("No hay empleados activos con datos IMSS y contrato vigente de la periodicidad '" +
                                           periodo.TipoPeriodo + "' en la empresa del periodo.");
            }

            foreach (PreReciboDTO pre in recibos)
            {
                string notas;
                if (notasPorEmpleado != null && notasPorEmpleado.TryGetValue(pre.EmpleadoId, out notas))
                {
                    pre.Notas = Validador.Opcional(notas, "Notas del recibo de " + pre.NumeroEmpleado, LongitudMaximaNotas);
                }
                else
                {
                    pre.Notas = Validador.Opcional(pre.Notas, "Notas del recibo de " + pre.NumeroEmpleado, LongitudMaximaNotas);
                }
            }

            ValidarRecibosParaCierre(periodo, recibos);
            Ejecutar(() => _dal.CerrarNomina(periodoId, recibos, usuarioId, comentario));
            return ResultadoDe(periodoId, ObtenerPeriodo(periodoId).Estatus, recibos);
        }

        private void ValidarRecibosParaCierre(PeriodoResumen periodo, List<PreReciboDTO> recibos)
        {
            List<string> problemas = new List<string>();

            foreach (PreReciboDTO negativo in recibos.Where(r => r.NetoPagar < 0m))
            {
                problemas.Add(negativo.NumeroEmpleado + " " + negativo.NombreCompleto + ": las deducciones superan a las percepciones (neto " +
                              negativo.NetoPagar.ToString("N2") + ").");
            }

            if (recibos.Sum(r => r.NetoPagar) <= 0m)
            {
                problemas.Add("El neto total del periodo debe ser mayor a cero.");
            }

            if (problemas.Count == 0)
            {
                ParametrosCalculoNomina par = Ejecutar(() => _dal.ObtenerParametros(periodo.EmpresaId, periodo.EjercicioId, periodo.Anio, periodo.FechaFin));
                Dictionary<int, IncidenciasEmpleado> incidencias = Ejecutar(() => _nominaDal.ObtenerIncidenciasPorEmpleado(periodo.PeriodoId));
                var conceptos = Ejecutar(() => _conceptoDal.ListarPeriodo(periodo.PeriodoId));
                bool sueldoOmitido = _asignacionBll.ListarOmitidos(periodo.PeriodoId)
                    .Any(c => c.EsBase && c.Llave == CalculadoraPreRecibo.LlaveSueldo);
                foreach (PreReciboDTO pre in recibos)
                {
                    IncidenciasEmpleado inc;
                    incidencias.TryGetValue(pre.EmpleadoId, out inc);
                    ContextoConciliacion contexto = new ContextoConciliacion
                    {
                        Periodo = periodo,
                        Incidencias = inc,
                        DiasLaboradosSemana = pre.DiasSemana,
                        SueldoOmitido = sueldoOmitido,
                        FactorFestivo = par.FactorDiaFestivoLaborado,
                        PorcentajePrimaDominical = _nominaDal.ResolverPorcentajePrimaDominical(periodo.EmpresaId, periodo.PeriodoId, pre.EmpleadoId) ?? par.PorcentajePrimaDominical,
                        ConceptosIncidencia = conceptos.Where(i => i.IdEmpleado == pre.EmpleadoId).ToList()
                    };
                    foreach (string problema in CalculadoraNomina.ConciliarRecibo(ConstruirCabecera(pre, EstatusRecibo.Calculado), ConstruirDetalles(pre), contexto))
                    {
                        problemas.Add(pre.NumeroEmpleado + " " + pre.NombreCompleto + ": " + problema);
                    }
                }
            }

            if (problemas.Count > 0)
            {
                throw new ExcepcionNegocio("No se puede cerrar la nómina:\n - " + string.Join("\n - ", problemas.Take(8)) +
                                           (problemas.Count > 8 ? "\n - ... y " + (problemas.Count - 8) + " más." : string.Empty));
            }
        }

        private static ResultadoCierreNomina ResultadoDe(int periodoId, string estatus, IList<PreReciboDTO> recibos)
        {
            return new ResultadoCierreNomina
            {
                PeriodoId = periodoId,
                EstatusPeriodo = estatus,
                TotalRecibos = recibos.Count,
                TotalPercepciones = recibos.Sum(r => r.TotalPercepciones),
                TotalDeducciones = recibos.Sum(r => r.TotalDeducciones),
                TotalNeto = recibos.Sum(r => r.NetoPagar)
            };
        }

        #endregion

        #region Cálculo y lectura

        private List<PreReciboDTO> CalcularPeriodoAbierto(PeriodoResumen periodo, int? empleadoId, string notasEnEdicion)
        {
            _conceptoBll.Inicializar(periodo.PeriodoId);
            var conceptos = Ejecutar(() => _conceptoDal.ListarPeriodo(periodo.PeriodoId));
            ParametrosCalculoNomina par = Ejecutar(() => _dal.ObtenerParametros(periodo.EmpresaId, periodo.EjercicioId, periodo.Anio, periodo.FechaFin));
            List<DatosEmpleadoCalculo> empleados = Ejecutar(() => _dal.ListarEmpleadosCalculo(periodo.EmpresaId, periodo.PeriodicidadPagoId, periodo.PeriodoId, periodo.FechaFin, empleadoId));
            Dictionary<int, IncidenciasEmpleado> incidencias = Ejecutar(() => _nominaDal.ObtenerIncidenciasPorEmpleado(periodo.PeriodoId, empleadoId));
            List<ConceptoPeriodoPaquete> omitidos = _asignacionBll.ListarOmitidos(periodo.PeriodoId);
            Dictionary<int, DocReciboNomina> borradores = Ejecutar(() => _reciboDal.ListarCabecerasPorPeriodo(periodo.PeriodoId))
                .ToDictionary(r => r.EmpleadoId);

            bool puedeEditar = ControlAcceso.Puede(ModuloSistema.Nomina, AccionSistema.Escribir);
            List<PreReciboDTO> resultado = new List<PreReciboDTO>();
            foreach (DatosEmpleadoCalculo empleado in empleados)
            {
                IncidenciasEmpleado inc;
                incidencias.TryGetValue(empleado.EmpleadoId, out inc);
                PreReciboDTO pre = CalculadoraPreRecibo.Calcular(periodo, par, empleado, inc,
                    conceptos.Where(i => i.IdEmpleado == empleado.EmpleadoId), omitidos);

                DocReciboNomina guardado;
                borradores.TryGetValue(empleado.EmpleadoId, out guardado);
                pre.EsEditable = puedeEditar;
                pre.ReciboId = guardado != null ? guardado.ReciboId : (int?)null;
                pre.Notas = notasEnEdicion ?? (guardado != null ? guardado.Notas : null);
                pre.Estatus = DeterminarEstatus(pre, guardado);
                resultado.Add(pre);
            }
            return resultado;
        }

        // Calculado = el borrador guardado coincide con lo que arrojan hoy las incidencias; si no, hay cambios pendientes de guardar
        private static string DeterminarEstatus(PreReciboDTO pre, DocReciboNomina guardado)
        {
            bool vigente = guardado != null &&
                           Math.Abs(guardado.TotalPercepciones - pre.TotalPercepciones) < ToleranciaTotales &&
                           Math.Abs(guardado.TotalDeducciones - pre.TotalDeducciones) < ToleranciaTotales &&
                           Math.Abs(guardado.NetoPagar - pre.NetoPagar) < ToleranciaTotales;
            if (vigente)
            {
                return EstatusTrabajadorNomina.Calculado;
            }
            return pre.TotalIncidencias > 0 ? EstatusTrabajadorNomina.ConIncidencias : EstatusTrabajadorNomina.Pendiente;
        }

        private List<PreReciboDTO> LeerRecibosProcesados(PeriodoResumen periodo, int? empleadoId)
        {
            List<DocReciboNomina> cabeceras = Ejecutar(() => _reciboDal.ListarCabecerasPorPeriodo(periodo.PeriodoId))
                .Where(r => r.Estatus != EstatusRecibo.Prerecibo && (!empleadoId.HasValue || r.EmpleadoId == empleadoId.Value))
                .ToList();
            Dictionary<int, List<DocReciboDetalle>> detalles = Ejecutar(() => _reciboDal.ListarDetallePorPeriodo(periodo.PeriodoId));
            Dictionary<int, DatosEmpleadoCalculo> identidad = Ejecutar(() => _dal.ListarIdentidadConRecibo(periodo.PeriodoId));
            Dictionary<int, IncidenciasEmpleado> incidencias = Ejecutar(() => _nominaDal.ObtenerIncidenciasPorEmpleado(periodo.PeriodoId, empleadoId));
            var conceptos = Ejecutar(() => _conceptoDal.ListarPeriodo(periodo.PeriodoId));

            List<PreReciboDTO> resultado = new List<PreReciboDTO>();
            foreach (DocReciboNomina cabecera in cabeceras)
            {
                DatosEmpleadoCalculo persona;
                identidad.TryGetValue(cabecera.EmpleadoId, out persona);
                persona = persona ?? new DatosEmpleadoCalculo { EmpleadoId = cabecera.EmpleadoId };
                IncidenciasEmpleado inc;
                incidencias.TryGetValue(cabecera.EmpleadoId, out inc);
                List<DocReciboDetalle> lineas;
                if (!detalles.TryGetValue(cabecera.ReciboId, out lineas))
                {
                    lineas = new List<DocReciboDetalle>();
                }

                PreReciboDTO pre = new PreReciboDTO
                {
                    PeriodoId = periodo.PeriodoId,
                    EmpleadoId = cabecera.EmpleadoId,
                    ReciboId = cabecera.ReciboId,
                    NumeroEmpleado = persona.NumeroEmpleado,
                    NombreCompleto = persona.NombreCompleto,
                    RFC = persona.RFC,
                    CURP = persona.CURP,
                    NSS = persona.NSS,
                    Puesto = cabecera.SnapshotPuesto,
                    Departamento = cabecera.SnapshotDepartamento,
                    SalarioDiario = cabecera.SnapshotSalarioDiario ?? 0m,
                    SDI = cabecera.SnapshotSDI ?? 0m,
                    SBC = cabecera.SnapshotSBC ?? 0m,
                    DiasPagados = cabecera.SnapshotDiasPagados ?? 0m,
                    TotalPercepciones = cabecera.TotalPercepciones,
                    TotalDeducciones = cabecera.TotalDeducciones,
                    NetoPagar = cabecera.NetoPagar,
                    TotalGravado = cabecera.TotalGravado ?? 0m,
                    TotalExento = cabecera.TotalExento ?? 0m,
                    TotalIncidencias = (inc != null ? inc.TotalActivas : 0) + conceptos.Count(i => i.IdEmpleado == cabecera.EmpleadoId),
                    Notas = cabecera.Notas,
                    Estatus = EstatusTrabajadorNomina.Calculado,
                    EsEditable = false
                };
                pre.Percepciones = lineas.Where(d => d.TipoConcepto != "D").Select(ConvertirDetalle).ToList();
                pre.Deducciones = lineas.Where(d => d.TipoConcepto == "D").Select(ConvertirDetalle).ToList();
                resultado.Add(pre);
            }
            return resultado.OrderBy(r => r.NombreCompleto, StringComparer.CurrentCultureIgnoreCase).ToList();
        }

        private static PreReciboConceptoDTO ConvertirDetalle(DocReciboDetalle d)
        {
            return new PreReciboConceptoDTO
            {
                TipoConcepto = d.TipoConcepto,
                Clave = d.ClaveConceptoSAT,
                Descripcion = d.Descripcion,
                Cantidad = d.Cantidad,
                ImporteGravado = d.ImporteGravado,
                ImporteExento = d.ImporteExento
            };
        }

        private static DocReciboNomina ConstruirCabecera(PreReciboDTO pre, string estatus)
        {
            return new DocReciboNomina
            {
                ReciboId = pre.ReciboId ?? 0,
                PeriodoId = pre.PeriodoId,
                EmpleadoId = pre.EmpleadoId,
                TotalPercepciones = pre.TotalPercepciones,
                TotalDeducciones = pre.TotalDeducciones,
                NetoPagar = pre.NetoPagar,
                TotalGravado = pre.TotalGravado,
                TotalExento = pre.TotalExento,
                SnapshotSalarioDiario = pre.SalarioDiario,
                SnapshotSDI = pre.SDI,
                SnapshotSBC = pre.SBC,
                SnapshotDiasPagados = pre.DiasPagados,
                SnapshotPuesto = pre.Puesto,
                SnapshotDepartamento = pre.Departamento,
                Notas = pre.Notas,
                Estatus = estatus
            };
        }

        private static List<DocReciboDetalle> ConstruirDetalles(PreReciboDTO pre)
        {
            return pre.Percepciones.Concat(pre.Deducciones)
                .Select(c => new DocReciboDetalle
                {
                    TipoConcepto = c.TipoConcepto,
                    ClaveConceptoSAT = c.Clave,
                    Descripcion = c.Descripcion,
                    ImporteGravado = c.ImporteGravado,
                    ImporteExento = c.ImporteExento,
                    Cantidad = c.Cantidad
                })
                .ToList();
        }

        private PeriodoResumen ObtenerPeriodo(int periodoId)
        {
            if (periodoId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione un periodo de nómina.");
            }
            PeriodoResumen periodo = Ejecutar(() => _periodoDal.ObtenerPorId(periodoId));
            if (periodo == null)
            {
                throw new ExcepcionNegocio("El periodo seleccionado ya no existe.");
            }
            return periodo;
        }

        #endregion
    }
}
