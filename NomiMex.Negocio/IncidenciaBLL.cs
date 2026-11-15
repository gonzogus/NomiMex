using System;
using System.Collections.Generic;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Procesos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Captura de incidencias del periodo por empleado: faltas, retardos, horas extra, días festivos laborados,
    /// primas dominicales, bonos y descuentos. Es lo único que el usuario captura en la prenómina.
    /// </summary>
    public class IncidenciaBLL : BllBase
    {
        // Códigos de incidencias legales (CatConcepto 'I') que, además de los que disminuyen días, consume sp_CalcularNomina.
        private static readonly string[] CodigosQueAfectanCalculo =
        {
            CodigoIncidencia.HoraExtraDoble, CodigoIncidencia.HoraExtraTriple, CodigoIncidencia.PrimaDominical,
            CodigoIncidencia.DiaFestivoLaborado, CodigoIncidencia.Bono, CodigoIncidencia.Descuento
        };

        private readonly IncidenciaDAL _dal = new IncidenciaDAL();
        private readonly PeriodoDAL _periodoDal = new PeriodoDAL();
        private readonly EmpleadoDAL _empleadoDal = new EmpleadoDAL();

        public List<IncidenciaListado> ListarPorPeriodo(int periodoId)
        {
            if (periodoId <= 0)
            {
                return new List<IncidenciaListado>();
            }
            return Ejecutar(() => _dal.ListarPorPeriodo(periodoId));
        }

        public List<IncidenciaListado> Filtrar(IEnumerable<IncidenciaListado> incidencias, string texto)
        {
            return FiltroBusqueda.Filtrar(incidencias, texto, i => i.NumeroEmpleado, i => i.NombreEmpleado, i => i.TipoIncidencia, i => i.Estatus);
        }

        /// <summary>Unidad de captura recomendada según el tipo de incidencia.</summary>
        public static string UnidadSugerida(CatTipoIncidencia tipo)
        {
            if (tipo == null)
            {
                return UnidadIncidencia.Dia;
            }
            if (tipo.Codigo == CodigoIncidencia.Bono || tipo.Codigo == CodigoIncidencia.Descuento)
            {
                return UnidadIncidencia.Monto;
            }
            return tipo.GeneraHorasExtra || tipo.Codigo == "RETARDO" ? UnidadIncidencia.Hora : UnidadIncidencia.Dia;
        }

        /// <summary>Registra la incidencia. Devuelve un aviso informativo (o null) para mostrar al usuario.</summary>
        public string Registrar(int periodoId, int empleadoId, CatTipoIncidencia tipo, decimal cantidad, string unidad,
                                DateTime fecha, string observaciones)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Escribir);

            Validador.SeleccionRequerida(periodoId, "Periodo");
            Validador.SeleccionRequerida(empleadoId, "Empleado");
            if (tipo == null)
            {
                throw new ExcepcionNegocio("Seleccione el tipo de incidencia.");
            }
            if (cantidad <= 0m)
            {
                throw new ExcepcionNegocio("La cantidad debe ser mayor a cero.");
            }
            if (unidad != UnidadIncidencia.Dia && unidad != UnidadIncidencia.Hora && unidad != UnidadIncidencia.Monto)
            {
                throw new ExcepcionNegocio("Seleccione la unidad de la incidencia (DIA, HORA o MONTO).");
            }

            string unidadRequerida = UnidadSugerida(tipo);
            bool unidadFija = tipo.Codigo == CodigoIncidencia.Bono || tipo.Codigo == CodigoIncidencia.Descuento ||
                              tipo.Codigo == CodigoIncidencia.DiaFestivoLaborado || tipo.Codigo == CodigoIncidencia.PrimaDominical ||
                              tipo.GeneraHorasExtra;
            if (unidadFija && unidad != unidadRequerida)
            {
                throw new ExcepcionNegocio("La incidencia '" + tipo.Descripcion + "' se captura en la unidad " + unidadRequerida + ".");
            }

            PeriodoResumen periodo = Ejecutar(() => _periodoDal.ObtenerPorId(periodoId));
            if (periodo == null)
            {
                throw new ExcepcionNegocio("El periodo seleccionado ya no existe.");
            }
            if (!PeriodoBLL.PermiteCapturarIncidencias(periodo.Estatus))
            {
                throw new ExcepcionNegocio("El periodo está en estatus " + periodo.Estatus +
                                           " y ya no admite incidencias (un periodo calculado debe reabrirse para modificarlo).");
            }
            if (fecha.Date < periodo.FechaInicio.Date || fecha.Date > periodo.FechaFin.Date)
            {
                throw new ExcepcionNegocio(string.Format("La fecha de la incidencia debe estar dentro del periodo ({0:dd/MM/yyyy} al {1:dd/MM/yyyy}).",
                    periodo.FechaInicio, periodo.FechaFin));
            }
            if (unidad == UnidadIncidencia.Hora && cantidad > 24m)
            {
                throw new ExcepcionNegocio("Una incidencia por horas no puede exceder 24 horas.");
            }
            if (unidad == UnidadIncidencia.Dia && cantidad > periodo.DiasPeriodo)
            {
                throw new ExcepcionNegocio("Los días de la incidencia no pueden exceder los días del periodo (" + periodo.DiasPeriodo.ToString("0.##") + ").");
            }

            if (tipo.Codigo == CodigoIncidencia.DiaFestivoLaborado)
            {
                ValidarDiaFestivoLaborado(periodo, fecha, cantidad);
            }

            bool empleadoValido = Ejecutar(() => _empleadoDal.ListarElegibles(periodo.EmpresaId, periodo.PeriodicidadPagoId, periodo.FechaFin))
                .Any(e => e.EmpleadoId == empleadoId);
            if (!empleadoValido)
            {
                throw new ExcepcionNegocio("El empleado no está activo o no pertenece a la empresa y periodicidad del periodo.");
            }

            ProcIncidencia incidencia = new ProcIncidencia
            {
                EmpleadoId = empleadoId,
                PeriodoId = periodoId,
                TipoIncidenciaId = tipo.TipoIncidenciaId,
                Cantidad = cantidad,
                Unidad = unidad,
                FechaIncidencia = fecha.Date,
                Observaciones = Validador.Opcional(observaciones, "Observaciones", 300)
            };
            Ejecutar(() => _dal.Insertar(incidencia));
            Ejecutar(() => _periodoDal.MarcarPrenomina(periodoId));

            if (!tipo.DisminuyeDias && !CodigosQueAfectanCalculo.Contains(tipo.Codigo))
            {
                return "La incidencia '" + tipo.Descripcion + "' se guardó como registro informativo: no modifica el cálculo de nómina.";
            }
            return null;
        }

        public void Cancelar(int periodoId, int incidenciaId)
        {
            ControlAcceso.Exigir(ModuloSistema.Nomina, AccionSistema.Escribir);
            Validador.SeleccionRequerida(incidenciaId, "Incidencia");

            PeriodoResumen periodo = Ejecutar(() => _periodoDal.ObtenerPorId(periodoId));
            if (periodo == null || !PeriodoBLL.PermiteCapturarIncidencias(periodo.Estatus))
            {
                throw new ExcepcionNegocio("El periodo ya no admite cambios en sus incidencias.");
            }
            if (!Ejecutar(() => _dal.Cancelar(incidenciaId)))
            {
                throw new ExcepcionNegocio("La incidencia ya no está activa.");
            }
        }

        // Un festivo laborado solo procede en una fecha que el catálogo marque como festiva dentro del periodo
        private static void ValidarDiaFestivoLaborado(PeriodoResumen periodo, DateTime fecha, decimal cantidad)
        {
            if (periodo.Festivos.Count == 0)
            {
                throw new ExcepcionNegocio("El periodo no tiene días festivos: no se puede registrar un día festivo laborado.");
            }
            if (!periodo.Festivos.Any(f => f.Fecha.Date == fecha.Date))
            {
                throw new ExcepcionNegocio("La fecha debe coincidir con un día festivo del periodo: " + periodo.FestivosTexto + ".");
            }
            if (cantidad > 1m)
            {
                throw new ExcepcionNegocio("Registre un día festivo laborado por fecha (cantidad 1).");
            }
        }
    }
}
