using System;
using System.Collections.Generic;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Procesos;
using NomiMex.Entidades.Relaciones;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Reglas de las incidencias por concepto SAT. Única fuente de conceptos: dbo.CatConcepto (catálogo oficial de
    /// ingresos y deducciones). Administra la configuración individual permanente del trabajador
    /// (dbo.RelEmpleadoIncidencia) y la captura de incidencias del periodo (dbo.ProcIncidencias).
    /// </summary>
    public class ConceptoIncidenciaBLL : BllBase
    {
        private readonly ConceptoIncidenciaDAL _dal = new ConceptoIncidenciaDAL();

        /// <summary>Conceptos SAT capturables como incidencia (percepción o deducción).</summary>
        public List<CatConcepto> ListarConceptos()
        {
            return Ejecutar(() => _dal.ListarConceptos());
        }

        /// <summary>Catálogo SAT completo de percepciones y deducciones, habilitadas o no para capturarse como incidencia.</summary>
        public List<CatConcepto> ListarCatalogo()
        {
            return Ejecutar(() => _dal.ListarCatalogo());
        }

        /// <summary>Formas de cálculo disponibles para un concepto: código y texto descriptivo.</summary>
        public static List<KeyValuePair<string, string>> ListarFormasCalculo()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("MONTO_FIJO", "Monto fijo ($)"),
                new KeyValuePair<string, string>("DIAS", "Días (días x salario diario)"),
                new KeyValuePair<string, string>("HORAS", "Horas (horas x salario por hora)"),
                new KeyValuePair<string, string>("PORCENTAJE_SD", "Porcentaje del salario diario (%)")
            };
        }

        /// <summary>
        /// Define la regla de cálculo de un concepto del catálogo SAT y lo habilita para capturarse como incidencia
        /// (en un periodo, en un paquete o en la ficha de un trabajador).
        /// </summary>
        public void HabilitarConcepto(int conceptoId, string formaCalculo, decimal factor, bool esGravado)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            Validador.SeleccionRequerida(conceptoId, "Concepto");
            if (!ListarFormasCalculo().Any(f => f.Key == formaCalculo))
                throw new ExcepcionNegocio("Seleccione la forma de cálculo del concepto.");
            if (factor <= 0m || factor > 999999m || Math.Round(factor, 4) != factor)
                throw new ExcepcionNegocio("El factor debe ser mayor a cero, con máximo cuatro decimales.");
            Ejecutar(() => _dal.HabilitarConcepto(conceptoId, formaCalculo, factor, esGravado));
        }

        /// <summary>Conceptos SAT del tipo indicado ('P' o 'D') activos, para el selector de incidencias.</summary>
        public List<CatConcepto> ListarConceptos(string tipo)
        {
            if (tipo != "P" && tipo != "D") throw new ExcepcionNegocio("El tipo de movimiento no es válido.");
            return ListarConceptos().Where(c => c.Activo && c.TipoConcepto == tipo).ToList();
        }

        /// <summary>Concepto de nómina (percepción o deducción) en que cae una incidencia legal.</summary>
        public static string TipoMovimientoLegal(CatTipoIncidencia t)
        {
            if (t == null) return null;
            bool percepcion = t.GeneraHorasExtra || t.Codigo == CodigoIncidencia.PrimaDominical ||
                              t.Codigo == CodigoIncidencia.DiaFestivoLaborado || t.Codigo == CodigoIncidencia.Bono;
            return percepcion ? "P" : "D";
        }

        /// <summary>Clave SAT con que el cálculo timbra la incidencia legal; null si solo reduce días pagados.</summary>
        public static string ClaveSATLegal(CatTipoIncidencia t)
        {
            if (t == null) return null;
            if (t.GeneraHorasExtra) return "019";
            switch (t.Codigo)
            {
                case CodigoIncidencia.PrimaDominical: return "020";
                case CodigoIncidencia.DiaFestivoLaborado:
                case CodigoIncidencia.Bono: return "038";
                case CodigoIncidencia.Descuento: return "004";
                default: return null;
            }
        }

        public List<CatTipoIncidencia> ListarTiposLegales(string tipo, bool soloActivos)
        {
            if (tipo != "P" && tipo != "D") throw new ExcepcionNegocio("El tipo de movimiento no es válido.");
            return new CatalogoBLL().ListarTiposIncidencia()
                .Where(t => (!soloActivos || t.Activo) && TipoMovimientoLegal(t) == tipo).ToList();
        }

        /// <summary>Reparto gravado/exento del importe, con la misma regla con que se persiste el recibo.</summary>
        public static void DesgloseFiscal(CatConcepto c, decimal importe, out decimal gravado, out decimal exento)
        {
            bool exenta = c != null && c.TipoConcepto == "P" && !c.EsGravado;
            gravado = exenta ? 0m : importe;
            exento = exenta ? importe : 0m;
        }

        /// <summary>Valida que el concepto SAT sea utilizable como incidencia.</summary>
        public static void ExigirConcepto(CatConcepto c)
        {
            if (c == null || string.IsNullOrWhiteSpace(c.ClaveSAT) || !c.EsIncidencia)
                throw new ExcepcionNegocio("El concepto '" + (c == null ? "" : c.Descripcion) +
                    "' no es un concepto SAT válido para incidencias.");
        }

        /// <summary>Sueldo diario base y horas de la jornada (turno o tipo de jornada) del trabajador, para calcular conceptos.</summary>
        public BaseCalculoEmpleado ObtenerBaseCalculo(int empleadoId)
        {
            Validador.SeleccionRequerida(empleadoId, "Empleado");
            BaseCalculoEmpleado b = Ejecutar(() => _dal.ObtenerBaseCalculo(empleadoId));
            if (b == null) throw new ExcepcionNegocio("El trabajador no tiene datos de salario capturados.");
            b.HorasPredeterminadas = b.HorasJornada <= 0m;
            if (b.HorasPredeterminadas) b.HorasJornada = HorasJornadaPredeterminada;
            return b;
        }

        public List<RelEmpleadoIncidencia> ListarAsignadas(int empleadoId)
        {
            Validador.SeleccionRequerida(empleadoId, "Empleado");
            return Ejecutar(() => _dal.ListarAsignadas(empleadoId));
        }

        public void Inicializar(int periodoId)
        {
            PeriodoResumen p = Ejecutar(() => new PeriodoDAL().ObtenerPorId(periodoId));
            if (p == null) throw new ExcepcionNegocio("El periodo no existe.");
            if (!p.EstaAbierto) return;
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Consultar);
            Ejecutar(() => _dal.Inicializar(periodoId, CalcularImporte));
        }

        /// <summary>Jornada legal diurna: horas con que se calcula el salario por hora si el trabajador no tiene turno ni jornada con horas.</summary>
        public const decimal HorasJornadaPredeterminada = 8m;

        public static decimal CalcularImporte(CatConcepto c, decimal cantidad, decimal salarioDiario)
        {
            return CalcularImporte(c, cantidad, salarioDiario, HorasJornadaPredeterminada);
        }

        /// <summary>
        /// Importe de un concepto a partir del sueldo base del trabajador: MONTO_FIJO (cantidad), DIAS (sueldo diario x días),
        /// HORAS (sueldo diario ÷ horas de la jornada x horas) o PORCENTAJE_SD; en todos se multiplica por el factor del concepto
        /// (por ejemplo 2 para horas dobles o descanso laborado, 3 para horas triples).
        /// </summary>
        public static decimal CalcularImporte(CatConcepto c, decimal cantidad, decimal salarioDiario, decimal horasJornada)
        {
            try { return CalcularImporteValidado(c, cantidad, salarioDiario, HorasEfectivas(horasJornada)); }
            catch (OverflowException) { throw new ExcepcionNegocio("El importe excede la capacidad numérica permitida."); }
        }

        private static decimal HorasEfectivas(decimal horasJornada)
        {
            return horasJornada > 0m ? horasJornada : HorasJornadaPredeterminada;
        }

        /// <summary>Operación del cálculo con las cifras reales, para que el usuario la verifique antes de guardar.</summary>
        public static string DescribirCalculo(CatConcepto c, decimal cantidad, decimal salarioDiario, decimal horasJornada)
        {
            if (c == null) return string.Empty;
            decimal importe;
            try { importe = CalcularImporte(c, cantidad, salarioDiario, horasJornada); }
            catch (ExcepcionNegocio) { return string.Empty; }

            decimal horas = HorasEfectivas(horasJornada);
            string factor = c.FactorMultiplicador.ToString("0.####");
            string cantidadTexto = cantidad.ToString("0.####");
            switch (c.FormaCalculo)
            {
                case "DIAS":
                    return salarioDiario.ToString("C2") + " (sueldo diario) × " + cantidadTexto + " d × " + factor + " = " + importe.ToString("C2");
                case "HORAS":
                    return salarioDiario.ToString("C2") + " ÷ " + horas.ToString("0.####") + " h = " +
                           (salarioDiario / horas).ToString("C2") + " por hora × " + cantidadTexto + " h × " + factor + " = " + importe.ToString("C2");
                case "PORCENTAJE_SD":
                    return salarioDiario.ToString("C2") + " (sueldo diario) × " + cantidadTexto + "% × " + factor + " = " + importe.ToString("C2");
                default:
                    return cantidad.ToString("C2") + " × " + factor + " = " + importe.ToString("C2");
            }
        }

        private static decimal CalcularImporteValidado(CatConcepto c, decimal cantidad, decimal salarioDiario, decimal horasJornada)
        {
            if (c == null) throw new ExcepcionNegocio("Seleccione un concepto.");
            if (cantidad <= 0m || cantidad > 99999999999999.9999m)
                throw new ExcepcionNegocio("La cantidad debe ser positiva y caber en DECIMAL(18,4).");
            if (Math.Round(cantidad, 4) != cantidad)
                throw new ExcepcionNegocio("La cantidad admite como máximo cuatro decimales.");
            if (c.FactorMultiplicador <= 0m) throw new ExcepcionNegocio("El factor del concepto debe ser positivo.");
            decimal baseImporte;
            switch (c.FormaCalculo)
            {
                case "MONTO_FIJO": baseImporte = cantidad; break;
                case "DIAS": baseImporte = cantidad * salarioDiario; break;
                case "HORAS": baseImporte = cantidad * salarioDiario / horasJornada; break;
                case "PORCENTAJE_SD": baseImporte = cantidad * salarioDiario / 100m; break;
                default: throw new ExcepcionNegocio("La forma de cálculo del concepto no es válida.");
            }
            if (c.FormaCalculo != "MONTO_FIJO" && salarioDiario <= 0m)
                throw new ExcepcionNegocio("El trabajador debe tener un salario diario positivo.");
            decimal importe = Math.Round(baseImporte * c.FactorMultiplicador, 2, MidpointRounding.AwayFromZero);
            if (importe <= 0m || importe > 9999999999999999.99m)
                throw new ExcepcionNegocio("El importe calculado debe ser positivo y caber en DECIMAL(18,2).");
            return importe;
        }

        public static string EtiquetaCantidad(CatConcepto c)
        {
            if (c == null) return "Cantidad";
            switch (c.FormaCalculo)
            {
                case "MONTO_FIJO": return "Monto ($)";
                case "DIAS": return "Días";
                case "HORAS": return "Horas";
                case "PORCENTAJE_SD": return "Porcentaje del salario diario (%)";
                default: throw new ExcepcionNegocio("Forma de cálculo desconocida.");
            }
        }

        private CatConcepto ExigirConceptoActivo(int id)
        {
            CatConcepto c = ListarConceptos().FirstOrDefault(x => x.ConceptoId == id && x.Activo);
            if (c == null) throw new ExcepcionNegocio("Seleccione un concepto activo.");
            ExigirConcepto(c);
            return c;
        }

        /// <summary>
        /// Captura una incidencia del periodo. Si <paramref name="permanente"/> es verdadero, el concepto se guarda
        /// además en la configuración permanente del empleado para precargarse en periodos siguientes.
        /// </summary>
        public void Registrar(int periodoId, int empleadoId, int conceptoId, decimal cantidad, string observaciones, bool permanente)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Escribir);
            if (permanente) ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            PeriodoResumen p = Ejecutar(() => new PeriodoDAL().ObtenerPorId(periodoId));
            if (p == null || !p.EstaAbierto) throw new ExcepcionNegocio("El periodo ya no admite incidencias.");
            DatosEmpleadoCalculo empleado = Ejecutar(() => new PreNominaDAL()
                .ListarEmpleadosCalculo(p.EmpresaId, p.PeriodicidadPagoId, periodoId, p.FechaFin, empleadoId)).SingleOrDefault();
            if (empleado == null) throw new ExcepcionNegocio("El empleado no pertenece a la empresa y periodicidad del periodo.");
            CatConcepto c = ExigirConceptoActivo(conceptoId);
            string obs = Validador.Opcional(observaciones, "Observaciones", 250);
            Inicializar(periodoId);
            var i = new ProcIncidenciaConcepto
            {
                IdPeriodo = periodoId, IdEmpleado = empleadoId, IdConceptoSAT = conceptoId,
                CantidadOValor = cantidad, ImporteCalculado = CalcularImporte(c, cantidad, empleado.SalarioDiario, empleado.HorasJornada),
                Observaciones = obs, EsPermanente = permanente
            };
            RelEmpleadoIncidencia asignada = permanente ? new RelEmpleadoIncidencia
            {
                IdEmpleado = empleadoId, IdConceptoSAT = conceptoId, CantidadOValor = cantidad,
                EsPermanente = true, Observaciones = obs, Activo = true
            } : null;
            Ejecutar(() => _dal.Registrar(i, asignada, CalcularImporte));
        }

        public void Quitar(int periodoId, int empleadoId, int incidenciaId, bool tambienPermanente)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Escribir);
            if (tambienPermanente) ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            Validador.SeleccionRequerida(incidenciaId, "Incidencia");
            Ejecutar(() => _dal.Quitar(periodoId, empleadoId, incidenciaId, tambienPermanente));
        }

        public void GuardarAsignada(RelEmpleadoIncidencia r)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            if (r == null) throw new ExcepcionNegocio("Falta la configuración del concepto.");
            Validador.SeleccionRequerida(r.IdEmpleado, "Empleado");
            if (Ejecutar(() => new EmpleadoDAL().ObtenerExpediente(r.IdEmpleado)) == null)
                throw new ExcepcionNegocio("El empleado ya no existe.");
            if (r.Activo) ExigirConceptoActivo(r.IdConceptoSAT);
            if (r.CantidadOValor <= 0m || r.CantidadOValor > 99999999999999.9999m)
                throw new ExcepcionNegocio("La cantidad debe ser positiva y caber en DECIMAL(18,4).");
            if (Math.Round(r.CantidadOValor, 4) != r.CantidadOValor)
                throw new ExcepcionNegocio("La cantidad admite como máximo cuatro decimales.");
            r.Observaciones = Validador.Opcional(r.Observaciones, "Observaciones", 250);
            Ejecutar(() => _dal.GuardarAsignada(r));
        }

        public void DesactivarAsignada(int empleadoId, int id)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            RelEmpleadoIncidencia r = ListarAsignadas(empleadoId).SingleOrDefault(i => i.IdEmpleadoIncidencia == id);
            if (r == null) throw new ExcepcionNegocio("El concepto asignado ya no pertenece al empleado.");
            if (r.EsBase) throw new ExcepcionNegocio("Los conceptos base (sueldo, IMSS, ISR y subsidio) los calcula el sistema y no se pueden quitar.");
            r.Activo = false;
            Ejecutar(() => _dal.GuardarAsignada(r));
        }

        /// <summary>Asigna el paquete de incidencias por defecto de la periodicidad del empleado (alta del trabajador).</summary>
        public void AsignarPaqueteDefault(int empleadoId, int empresaId, int tipoPeriodoId)
        {
            if (empleadoId <= 0 || empresaId <= 0 || tipoPeriodoId <= 0) return;
            Ejecutar(() => _dal.AsignarPaqueteDefault(empleadoId, empresaId, tipoPeriodoId));
        }
    }
}
