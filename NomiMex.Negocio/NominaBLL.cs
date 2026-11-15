using System.Collections.Generic;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Documentos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Orquesta la prenómina, el cálculo de recibos, la autorización y el cierre de un periodo. Valida las precondiciones del
    /// flujo (empresa activa, periodicidad asignada, empleados activos) antes de invocar los procedimientos transaccionales y
    /// concilia cada recibo generado con las reglas de CalculadoraNomina.
    /// </summary>
    public class NominaBLL : BllBase
    {
        private readonly NominaDAL _dal = new NominaDAL();
        private readonly PeriodoDAL _periodoDal = new PeriodoDAL();
        private readonly ReciboDAL _reciboDal = new ReciboDAL();
        private readonly PrerrequisitosNomina _prerrequisitos = new PrerrequisitosNomina();

        /// <summary>Condiciones que no impiden calcular pero que el usuario debe conocer antes de continuar.</summary>
        public List<string> ObtenerAdvertenciasPrevias(int periodoId)
        {
            List<string> advertencias = new List<string>();
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);

            int isr = 0, subsidio = 0, imss = 0;
            Ejecutar(() => { _dal.ContarTablasFiscales(periodoId, out isr, out subsidio, out imss); });
            if (isr == 0)
            {
                advertencias.Add("No hay tarifa de ISR cargada para el ejercicio " + periodo.Anio + ": el ISR se calculará en cero.");
            }
            if (subsidio == 0)
            {
                advertencias.Add("No hay tabla de subsidio al empleo para el ejercicio " + periodo.Anio + ".");
            }
            if (imss == 0)
            {
                advertencias.Add("No hay cuotas IMSS cargadas para el ejercicio " + periodo.Anio + ": la cuota obrera se calculará en cero.");
            }
            if (isr == 0 || subsidio == 0 || imss == 0)
            {
                advertencias.Add("Cargue las tablas con: EXEC dbo.sp_CargarDatosFiscales @Anio = " + periodo.Anio + ";");
            }

            int incidenciasHuerfanas = Ejecutar(() => _dal.ContarIncidenciasSinEmpleadoActivo(periodoId));
            if (incidenciasHuerfanas > 0)
            {
                advertencias.Add(incidenciasHuerfanas + " incidencia(s) activa(s) pertenecen a empleados que ya no están activos y se ignorarán.");
            }

            List<ConceptoPeriodoPaquete> legalesOmitidos = new AsignacionPaqueteBLL().ListarOmisionesQueBloqueanTimbrado(periodoId);
            if (legalesOmitidos.Count > 0)
            {
                advertencias.Add("La empresa omite conceptos legales en este periodo (" + string.Join(", ", legalesOmitidos.Select(c => c.Concepto)) +
                                 "): el periodo puede cerrarse, pero sus recibos NO se podrán timbrar porque no cumplen los requisitos del CFDI de nómina.");
            }
            return advertencias;
        }

        public ResultadoCalculo EjecutarCalculo(int periodoId)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Calcular);
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);

            // Pasos 1, 2 y 4: empresa activa, periodicidad asignada y empleados activos para Empresa + Periodicidad
            _prerrequisitos.ExigirEmpleadosActivos(periodo.EmpresaId, periodo.PeriodicidadPagoId);

            if (periodo.Estatus == EstatusPeriodo.Calculado)
            {
                throw new ExcepcionNegocio("El periodo ya fue procesado: sus recibos están emitidos y no se recalculan. " +
                                           "Si debe corregir incidencias, reabra la prenómina antes de volver a procesarlo.");
            }
            if (!PeriodoBLL.PermiteCalcular(periodo.Estatus))
            {
                throw new ExcepcionNegocio("El periodo tiene estatus " + periodo.Estatus + " y no puede calcularse.");
            }

            int elegibles = Ejecutar(() => _dal.ContarEmpleadosElegibles(periodoId));
            if (elegibles == 0)
            {
                throw new ExcepcionNegocio("No hay empleados activos con datos IMSS y contrato vigente de la periodicidad '" +
                                           periodo.TipoPeriodo + "' en la empresa del periodo.");
            }

            new ConceptoIncidenciaBLL().Inicializar(periodoId);
            Ejecutar(() => _dal.CalcularNomina(periodoId));
            ResultadoCalculo resultado = ObtenerResultado(periodoId);
            if (resultado.TotalRecibos != elegibles)
            {
                resultado.Advertencias.Insert(0, "Se esperaban " + elegibles + " recibos y se generaron " + resultado.TotalRecibos + ".");
            }
            return resultado;
        }

        /// <summary>Regresa un periodo calculado a prenómina eliminando sus recibos, para corregir incidencias y reprocesarlo.</summary>
        public void Reabrir(int periodoId, string motivo)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Autorizar);
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            if (!PeriodoBLL.PermiteReabrir(periodo.Estatus))
            {
                throw new ExcepcionNegocio("Solo se puede reabrir un periodo CALCULADO que aún no se autoriza (actual: " + periodo.Estatus + ").");
            }

            int? usuarioId = SesionActual.HaySesion ? SesionActual.Usuario.UsuarioId : (int?)null;
            string nota = Validador.Opcional(motivo, "Motivo", 300);
            Ejecutar(() => _dal.ReversarCalculo(periodoId, usuarioId, nota));
        }

        /// <summary>Totales del periodo y conciliación de todos sus recibos (una sola consulta de cabeceras y otra de detalles).</summary>
        public ResultadoCalculo ObtenerResultado(int periodoId)
        {
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            List<ReciboListado> recibos = Ejecutar(() => _dal.ListarRecibos(periodoId));
            Dictionary<int, List<DocReciboDetalle>> detalles = Ejecutar(() => _reciboDal.ListarDetallePorPeriodo(periodoId));

            ResultadoCalculo resultado = new ResultadoCalculo { PeriodoId = periodoId, TotalRecibos = recibos.Count };
            if (recibos.Count == 0)
            {
                return resultado;
            }

            Dictionary<int, IncidenciasEmpleado> incidencias = Ejecutar(() => _dal.ObtenerIncidenciasPorEmpleado(periodoId));
            Dictionary<int, int> diasSemana = Ejecutar(() => _dal.ObtenerDiasSemanaPorEmpleado(periodoId));
            var conceptos = Ejecutar(() => new ConceptoIncidenciaDAL().ListarPeriodo(periodoId));
            decimal factorFestivo = 2m, porcentajePrima = 25m;
            Ejecutar(() => _dal.ObtenerParametrosPago(periodo.EmpresaId, periodo.FechaFin, out factorFestivo, out porcentajePrima));
            bool sueldoOmitido = new AsignacionPaqueteBLL().ListarOmitidos(periodoId)
                .Any(c => c.EsBase && c.Llave == CalculadoraPreRecibo.LlaveSueldo);
            foreach (ReciboListado r in recibos)
            {
                resultado.TotalPercepciones += r.TotalPercepciones;
                resultado.TotalDeducciones += r.TotalDeducciones;
                resultado.TotalNeto += r.NetoPagar;

                DocReciboNomina cabecera = new DocReciboNomina
                {
                    ReciboId = r.ReciboId,
                    TotalPercepciones = r.TotalPercepciones,
                    TotalDeducciones = r.TotalDeducciones,
                    NetoPagar = r.NetoPagar,
                    SnapshotSalarioDiario = r.SnapshotSalarioDiario,
                    SnapshotDiasPagados = r.SnapshotDiasPagados
                };
                List<DocReciboDetalle> detalleRecibo;
                if (!detalles.TryGetValue(r.ReciboId, out detalleRecibo))
                {
                    detalleRecibo = new List<DocReciboDetalle>();
                }

                IncidenciasEmpleado incidenciasEmpleado;
                incidencias.TryGetValue(r.EmpleadoId, out incidenciasEmpleado);
                int diasLaborados;
                ContextoConciliacion contexto = new ContextoConciliacion
                {
                    Periodo = periodo,
                    Incidencias = incidenciasEmpleado,
                    DiasLaboradosSemana = diasSemana.TryGetValue(r.EmpleadoId, out diasLaborados) ? diasLaborados : (int?)null,
                    SueldoOmitido = sueldoOmitido,
                    FactorFestivo = factorFestivo,
                    PorcentajePrimaDominical = _dal.ResolverPorcentajePrimaDominical(periodo.EmpresaId, periodo.PeriodoId, r.EmpleadoId) ?? porcentajePrima,
                    ConceptosIncidencia = conceptos.Where(i => i.IdEmpleado == r.EmpleadoId).ToList()
                };

                foreach (string problema in CalculadoraNomina.ConciliarRecibo(cabecera, detalleRecibo, contexto))
                {
                    resultado.Advertencias.Add(r.NumeroEmpleado + " " + r.NombreEmpleado + ": " + problema);
                }
            }
            return resultado;
        }

        public void Autorizar(int periodoId, string comentario)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Autorizar);
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            if (!PeriodoBLL.PermiteAutorizar(periodo.Estatus))
            {
                throw new ExcepcionNegocio("Solo se puede autorizar un periodo en estatus CALCULADO (actual: " + periodo.Estatus + ").");
            }

            ResultadoCalculo resultado = ObtenerResultado(periodoId);
            if (resultado.TotalRecibos == 0)
            {
                throw new ExcepcionNegocio("El periodo no tiene recibos calculados.");
            }
            if (resultado.TotalNeto <= 0m)
            {
                throw new ExcepcionNegocio("El neto total del periodo debe ser mayor a cero para autorizarlo.");
            }

            int? usuarioId = SesionActual.HaySesion ? SesionActual.Usuario.UsuarioId : (int?)null;
            string nota = Validador.Opcional(comentario, "Comentario", 300);
            Ejecutar(() => _dal.AutorizarPeriodo(periodoId, usuarioId, nota));
        }

        public void Cerrar(int periodoId)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Autorizar);
            PeriodoResumen periodo = ObtenerPeriodo(periodoId);
            if (!PeriodoBLL.PermiteCerrar(periodo.Estatus))
            {
                throw new ExcepcionNegocio("Solo se puede cerrar un periodo AUTORIZADO (actual: " + periodo.Estatus + ").");
            }
            Ejecutar(() => _dal.CerrarPeriodo(periodoId));
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
    }
}
