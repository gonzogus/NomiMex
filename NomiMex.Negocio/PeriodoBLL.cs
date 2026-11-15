using System;
using System.Collections.Generic;
using NomiMex.Datos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Procesos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Reglas del calendario de periodos (ProcPeriodoNomina): configuración por empresa y periodicidad, generación anual
    /// y detección del periodo vigente. Es independiente del cálculo de nómina (NominaBLL).
    /// </summary>
    public class PeriodoBLL : BllBase
    {
        private readonly PeriodoDAL _dal = new PeriodoDAL();
        private readonly PeriodicidadDAL _periodicidadDal = new PeriodicidadDAL();
        private readonly PrerrequisitosNomina _prerrequisitos = new PrerrequisitosNomina();

        public List<PeriodoResumen> ListarPorEmpresa(int empresaId)
        {
            if (empresaId <= 0)
            {
                return new List<PeriodoResumen>();
            }
            return Ejecutar(() => _dal.ListarPorEmpresa(empresaId));
        }

        public List<PeriodoResumen> ListarPorEmpresaTipo(int empresaId, int tipoPeriodoId, int anio)
        {
            if (empresaId <= 0 || tipoPeriodoId <= 0)
            {
                return new List<PeriodoResumen>();
            }
            return Ejecutar(() => _dal.ListarPorEmpresaTipo(empresaId, tipoPeriodoId, anio));
        }

        public PeriodoResumen ObtenerPorId(int periodoId)
        {
            PeriodoResumen periodo = Ejecutar(() => _dal.ObtenerPorId(periodoId));
            if (periodo == null)
            {
                throw new ExcepcionNegocio("El periodo seleccionado ya no existe.");
            }
            return periodo;
        }

        public EmpresaPeriodicidad ObtenerConfiguracion(int empresaId, int tipoPeriodoId)
        {
            return _prerrequisitos.ExigirPeriodicidadDeEmpresa(empresaId, tipoPeriodoId);
        }

        /// <summary>Guarda los días de inicio, fin, pago y descanso y la regla del séptimo día de una periodicidad de la empresa.</summary>
        public void GuardarConfiguracion(EmpresaPeriodicidad config)
        {
            ControlAcceso.Exigir(ModuloSistema.Periodos, AccionSistema.Escribir);
            EmpresaPeriodicidad vigente = _prerrequisitos.ExigirPeriodicidadDeEmpresa(config.EmpresaId, config.PeriodicidadPagoId);

            bool porSemana = GeneradorPeriodos.EsPorSemana(vigente.PeriodicidadClave);
            if (porSemana)
            {
                if (!config.DiaInicio.HasValue || config.DiaInicio < 1 || config.DiaInicio > 7)
                {
                    throw new ExcepcionNegocio("Seleccione el día de inicio de la semana.");
                }
                if (!config.DiaPago.HasValue || config.DiaPago < 1 || config.DiaPago > 7)
                {
                    throw new ExcepcionNegocio("Seleccione el día de pago.");
                }
                config.DiaFin = GeneradorPeriodos.DiaFinSemana(config.DiaInicio.Value);
            }
            else
            {
                config.DiaInicio = null;
                config.DiaFin = null;
                config.DiaPago = null;
            }

            if (config.DiaDescanso.HasValue && (config.DiaDescanso < 1 || config.DiaDescanso > 7))
            {
                throw new ExcepcionNegocio("El día de descanso debe estar entre lunes y domingo.");
            }
            if (config.CalculaSeptimoDia && !config.DiaDescanso.HasValue)
            {
                throw new ExcepcionNegocio("Para calcular el séptimo día seleccione el día de descanso.");
            }
            Ejecutar(() => _periodicidadDal.GuardarConfiguracion(config));
        }

        /// <summary>
        /// Genera (o actualiza los periodos aún abiertos de) todo el ejercicio fiscal para la empresa y periodicidad indicadas,
        /// con las reglas guardadas en su configuración, y vincula los días festivos que caen en cada periodo.
        /// </summary>
        public ResultadoGeneracionPeriodos GenerarEjercicio(int empresaId, int tipoPeriodoId, int anio)
        {
            ControlAcceso.Exigir(ModuloSistema.Periodos, AccionSistema.Escribir);
            EmpresaPeriodicidad config = _prerrequisitos.ExigirPeriodicidadDeEmpresa(empresaId, tipoPeriodoId);

            DateTime? ancla = null;
            if (config.PeriodicidadClave == ClavePeriodicidad.Catorcenal)
            {
                DateTime? ultimoFin = Ejecutar(() => _dal.ObtenerUltimaFechaFinEjercicio(empresaId, tipoPeriodoId, anio - 1));
                ancla = ultimoFin.HasValue ? ultimoFin.Value.AddDays(1) : (DateTime?)null;
            }

            List<ProcPeriodoNomina> periodos = GeneradorPeriodos.Generar(config.PeriodicidadClave, anio, config, ancla);
            return Ejecutar(() => _dal.GuardarCalendario(empresaId, tipoPeriodoId, anio, periodos));
        }

        /// <summary>
        /// Periodo que corresponde procesar en la fecha indicada: el que la contiene si sigue abierto; si ya se procesó, el
        /// siguiente abierto; si no hay periodo vigente, el último abierto que quedó pendiente.
        /// </summary>
        public DeteccionPeriodo DetectarPeriodo(int empresaId, int tipoPeriodoId, DateTime fecha)
        {
            _prerrequisitos.ExigirPeriodicidadDeEmpresa(empresaId, tipoPeriodoId);
            DeteccionPeriodo deteccion = new DeteccionPeriodo();
            deteccion.AnterioresPendientes = Ejecutar(() => _dal.ContarAbiertosAnteriores(empresaId, tipoPeriodoId, fecha));

            int? id = Ejecutar(() => _dal.ObtenerIdVigente(empresaId, tipoPeriodoId, fecha));
            if (id.HasValue)
            {
                PeriodoResumen vigente = ObtenerPorId(id.Value);
                if (vigente.EstaAbierto)
                {
                    deteccion.Periodo = vigente;
                    deteccion.EsVigente = true;
                    deteccion.Mensaje = "Periodo vigente a la fecha " + fecha.ToString("dd/MM/yyyy") + ".";
                    return deteccion;
                }

                int? siguiente = Ejecutar(() => _dal.ObtenerIdSiguienteAbierto(empresaId, tipoPeriodoId, fecha));
                if (siguiente.HasValue)
                {
                    deteccion.Periodo = ObtenerPorId(siguiente.Value);
                    deteccion.Mensaje = "El periodo vigente (#" + vigente.NumeroPeriodo + ") ya fue procesado. Se muestra el siguiente periodo abierto.";
                    return deteccion;
                }

                deteccion.Periodo = vigente;
                deteccion.EsVigente = true;
                deteccion.Mensaje = "El periodo vigente ya fue procesado y no hay un siguiente periodo abierto. Genere el ejercicio siguiente.";
                return deteccion;
            }

            int? pendiente = Ejecutar(() => _dal.ObtenerIdUltimoAbiertoAnterior(empresaId, tipoPeriodoId, fecha));
            if (pendiente.HasValue)
            {
                deteccion.Periodo = ObtenerPorId(pendiente.Value);
                deteccion.Mensaje = "No hay un periodo vigente a la fecha. Se muestra el último periodo abierto pendiente de pago.";
                return deteccion;
            }

            int? futuro = Ejecutar(() => _dal.ObtenerIdSiguienteAbierto(empresaId, tipoPeriodoId, fecha));
            if (futuro.HasValue)
            {
                deteccion.Periodo = ObtenerPorId(futuro.Value);
                deteccion.Mensaje = "No hay un periodo vigente a la fecha. Se muestra el siguiente periodo abierto.";
                return deteccion;
            }

            deteccion.Mensaje = "No hay periodos generados para esta empresa y periodicidad en la fecha " + fecha.ToString("dd/MM/yyyy") +
                                ". Genérelos en Configuración de periodos.";
            return deteccion;
        }

        /// <summary>Un periodo admite incidencias y cálculo solo mientras no se haya procesado (CREADO o PRENOMINA).</summary>
        public static bool PermiteCapturarIncidencias(string estatus)
        {
            return estatus == EstatusPeriodo.Creado || estatus == EstatusPeriodo.Prenomina;
        }

        public static bool PermiteCalcular(string estatus)
        {
            return PermiteCapturarIncidencias(estatus);
        }

        public static bool PermiteReabrir(string estatus)
        {
            return estatus == EstatusPeriodo.Calculado;
        }

        public static bool PermiteAutorizar(string estatus)
        {
            return estatus == EstatusPeriodo.Calculado;
        }

        public static bool PermiteCerrar(string estatus)
        {
            return estatus == EstatusPeriodo.Autorizado;
        }
    }
}
