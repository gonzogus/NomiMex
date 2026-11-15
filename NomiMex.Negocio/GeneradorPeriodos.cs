using System;
using System.Collections.Generic;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Procesos;
using NomiMex.Entidades.Relaciones;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Reglas puras (sin acceso a datos) para generar el calendario de periodos de un ejercicio fiscal.
    /// Semanal/catorcenal: los periodos pertenecen al ejercicio en que TERMINAN (52 o 53 semanas); el primero contiene el 1 de enero.
    /// Quincenal: 24 periodos (1-15 y 16-fin de mes real). Mensual: 12. Decenal: 36 (1-10, 11-20, 21-fin de mes).
    /// </summary>
    public static class GeneradorPeriodos
    {
        /// <summary>Configuración inicial de una periodicidad recién asignada a una empresa.</summary>
        public static RelEmpresaPeriodicidad ConfiguracionPredeterminada(string claveTipoPeriodo, int tipoPeriodoId, string descripcion)
        {
            bool porSemana = EsPorSemana(claveTipoPeriodo);
            return new RelEmpresaPeriodicidad
            {
                PeriodicidadPagoId = tipoPeriodoId,
                Descripcion = descripcion,
                CalculaSeptimoDia = porSemana,
                DiaInicio = porSemana ? (byte?)1 : null,
                DiaFin = porSemana ? (byte?)7 : null,
                DiaPago = porSemana ? (byte?)5 : null,
                DiaDescanso = 7,
                TrabajaDomingo = false,
                PorcentajePrimaDominical = 25m,
                Activo = true
            };
        }

        /// <summary>Periodicidades cuyos periodos se definen por días de la semana (inicio, fin y pago).</summary>
        public static bool EsPorSemana(string claveTipoPeriodo)
        {
            return claveTipoPeriodo == ClavePeriodicidad.Semanal || claveTipoPeriodo == ClavePeriodicidad.Catorcenal;
        }

        public static bool EsSoportada(string claveTipoPeriodo)
        {
            return EsPorSemana(claveTipoPeriodo) || claveTipoPeriodo == ClavePeriodicidad.Quincenal ||
                   claveTipoPeriodo == ClavePeriodicidad.Mensual || claveTipoPeriodo == ClavePeriodicidad.Decenal;
        }

        /// <summary>Día de fin de una semana de pago: el día anterior al de inicio.</summary>
        public static byte DiaFinSemana(byte diaInicio)
        {
            return diaInicio == 1 ? (byte)7 : (byte)(diaInicio - 1);
        }

        /// <summary>
        /// Genera todos los periodos del ejercicio. fechaAncla es el primer día del primer periodo catorcenal del ejercicio
        /// (día siguiente al último periodo del ejercicio anterior); si es nula se usa el día de inicio configurado más cercano.
        /// </summary>
        public static List<ProcPeriodoNomina> Generar(string claveTipoPeriodo, int anio, RelEmpresaPeriodicidad config, DateTime? fechaAncla)
        {
            if (config == null)
            {
                throw new ExcepcionNegocio("La periodicidad no tiene configuración de periodos.");
            }
            if (anio < 2000 || anio > 2100)
            {
                throw new ExcepcionNegocio("El ejercicio fiscal debe estar entre 2000 y 2100.");
            }

            switch (claveTipoPeriodo)
            {
                case ClavePeriodicidad.Semanal: return GenerarPorDias(anio, config, 7, fechaAncla);
                case ClavePeriodicidad.Catorcenal: return GenerarPorDias(anio, config, 14, fechaAncla);
                case ClavePeriodicidad.Quincenal: return GenerarPorMes(anio, config, new[] { 15 }, 15m);
                case ClavePeriodicidad.Decenal: return GenerarPorMes(anio, config, new[] { 10, 20 }, 10m);
                case ClavePeriodicidad.Mensual: return GenerarPorMes(anio, config, new int[0], 30m);
                default:
                    throw new ExcepcionNegocio("La generación automática de periodos no está disponible para la periodicidad '" +
                                               claveTipoPeriodo + "'.");
            }
        }

        private static List<ProcPeriodoNomina> GenerarPorDias(int anio, RelEmpresaPeriodicidad config, int dias, DateTime? fechaAncla)
        {
            if (!config.DiaInicio.HasValue || !config.DiaPago.HasValue)
            {
                throw new ExcepcionNegocio("Configure el día de inicio y el día de pago de la semana antes de generar los periodos.");
            }

            DateTime primero = new DateTime(anio, 1, 1);
            DateTime inicio = fechaAncla.HasValue
                ? fechaAncla.Value.Date
                : primero.AddDays(-((DiaSemanaISO.De(primero) - config.DiaInicio.Value + 7) % 7));

            // Se ajusta el ancla para que el primer periodo sea el que contiene el 1 de enero (el anterior termina el año previo)
            while (inicio.AddDays(dias - 1) < primero)
            {
                inicio = inicio.AddDays(dias);
            }
            while (inicio.AddDays(-1) >= primero)
            {
                inicio = inicio.AddDays(-dias);
            }

            DateTime limite = new DateTime(anio, 12, 31);
            List<ProcPeriodoNomina> lista = new List<ProcPeriodoNomina>();
            int numero = 1;
            while (inicio.AddDays(dias - 1) <= limite)
            {
                DateTime fin = inicio.AddDays(dias - 1);
                lista.Add(Crear(config, numero++, inicio, fin, FechaPagoSemanal(fin, config.DiaPago.Value), dias));
                inicio = inicio.AddDays(dias);
            }
            return lista;
        }

        private static List<ProcPeriodoNomina> GenerarPorMes(int anio, RelEmpresaPeriodicidad config, int[] cortes, decimal diasPagados)
        {
            List<ProcPeriodoNomina> lista = new List<ProcPeriodoNomina>();
            int numero = 1;
            for (int mes = 1; mes <= 12; mes++)
            {
                int ultimo = DateTime.DaysInMonth(anio, mes);
                int diaInicio = 1;
                List<int> finales = new List<int>(cortes);
                finales.Add(ultimo);
                foreach (int diaFin in finales)
                {
                    DateTime inicio = new DateTime(anio, mes, diaInicio);
                    DateTime fin = new DateTime(anio, mes, diaFin);
                    lista.Add(Crear(config, numero++, inicio, fin, fin, diasPagados));
                    diaInicio = diaFin + 1;
                }
            }
            return lista;
        }

        private static ProcPeriodoNomina Crear(RelEmpresaPeriodicidad config, int numero, DateTime inicio, DateTime fin, DateTime pago, decimal dias)
        {
            return new ProcPeriodoNomina
            {
                NumeroPeriodo = numero,
                FechaInicio = inicio,
                FechaFin = fin,
                FechaPago = pago,
                DiasPeriodo = dias,
                EsInicioMes = inicio.Day == 1 || inicio.Month != fin.Month,
                EsFinMes = fin.Day == DateTime.DaysInMonth(fin.Year, fin.Month) || inicio.Month != fin.Month,
                DiaDescanso = config.DiaDescanso,
                CalculaSeptimoDia = config.CalculaSeptimoDia && config.DiaDescanso.HasValue,
                Estatus = EstatusPeriodo.Creado
            };
        }

        /// <summary>Primer día igual o posterior al fin del periodo que cae en el día de pago configurado.</summary>
        public static DateTime FechaPagoSemanal(DateTime finPeriodo, byte diaPago)
        {
            return finPeriodo.Date.AddDays((diaPago - DiaSemanaISO.De(finPeriodo) + 7) % 7);
        }
    }
}
