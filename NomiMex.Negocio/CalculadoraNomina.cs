using System;
using System.Collections.Generic;
using System.Linq;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Documentos;
using NomiMex.Entidades.Vistas;
using NomiMex.Entidades.Procesos;

namespace NomiMex.Negocio
{
    /// <summary>Días pagados de un periodo desglosados en laborables y séptimo día proporcional.</summary>
    public class DesgloseDiasPagados
    {
        public decimal DiasLaborables { get; set; }
        public decimal DiasLaborablesPagados { get; set; }
        public decimal DiasSeptimo { get; set; }
        public decimal DiasPagados { get; set; }
    }

    /// <summary>Datos del periodo y del empleado con los que se recalcula un recibo para conciliarlo.</summary>
    public class ContextoConciliacion
    {
        public PeriodoResumen Periodo { get; set; }
        public IncidenciasEmpleado Incidencias { get; set; }

        /// <summary>Verdadero si la empresa omitió el concepto de sueldo en el periodo (no se compara contra salario × días).</summary>
        public bool SueldoOmitido { get; set; }

        /// <summary>Días laborados por semana vigentes del empleado en el periodo; nulo = jornada completa.</summary>
        public int? DiasLaboradosSemana { get; set; }
        public decimal FactorFestivo { get; set; }
        public decimal PorcentajePrimaDominical { get; set; }
        public IList<ProcIncidenciaConcepto> ConceptosIncidencia { get; set; }
    }

    /// <summary>
    /// Reglas de cálculo de percepciones, deducciones y neto a pagar. Son funciones puras (sin acceso a datos):
    /// se usan para integrar el SDI/SBC y para conciliar los recibos que genera sp_CalcularNomina.
    /// </summary>
    public static class CalculadoraNomina
    {
        public const decimal TopeSBCEnUMA = 25m;
        public const decimal Tolerancia = 0.05m;
        public const decimal ToleranciaDias = 0.01m;

        public static decimal Redondear(decimal valor)
        {
            return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
        }

        public static decimal CalcularSalarioBase(decimal salarioDiario, decimal diasPagados)
        {
            return salarioDiario * diasPagados;
        }

        /// <summary>Factor de integración: 1 + aguinaldo/365 + (prima vacacional % × días de vacaciones)/365.</summary>
        public static decimal CalcularFactorIntegracion(decimal diasAguinaldo, decimal primaVacacionalPorcentaje, decimal diasVacaciones)
        {
            decimal factor = 1m + (diasAguinaldo / 365m) + ((primaVacacionalPorcentaje / 100m) * diasVacaciones / 365m);
            return Math.Round(factor, 6, MidpointRounding.AwayFromZero);
        }

        public static decimal CalcularSDI(decimal salarioDiario, decimal factorIntegracion)
        {
            return Math.Round(salarioDiario * factorIntegracion, 4, MidpointRounding.AwayFromZero);
        }

        /// <summary>Factor de integración implícito en un SDI ya calculado (SDI / salario diario); 0 si no hay salario.</summary>
        public static decimal ObtenerFactorIntegracion(decimal salarioDiario, decimal sdi)
        {
            return salarioDiario > 0m ? Math.Round(sdi / salarioDiario, 6, MidpointRounding.AwayFromZero) : 0m;
        }

        /// <summary>SBC = SDI limitado a 25 veces la UMA diaria vigente.</summary>
        public static decimal CalcularSBC(decimal sdi, decimal umaDiaria)
        {
            if (umaDiaria <= 0m)
            {
                return sdi;
            }
            decimal tope = TopeSBCEnUMA * umaDiaria;
            return sdi > tope ? Math.Round(tope, 4, MidpointRounding.AwayFromZero) : sdi;
        }

        public static decimal CalcularNeto(decimal totalPercepciones, decimal totalDeducciones)
        {
            return Redondear(totalPercepciones - totalDeducciones);
        }

        /// <summary>Días de descanso (p. ej. domingos) que contiene el periodo cuando se calcula el séptimo día; 0 si no aplica.</summary>
        public static int ContarDescansos(PeriodoResumen periodo)
        {
            if (!periodo.CalculaSeptimoDia || !periodo.DiaDescanso.HasValue)
            {
                return 0;
            }
            return DiaSemanaISO.Contar(periodo.FechaInicio, periodo.FechaFin, periodo.DiaDescanso.Value);
        }

        /// <summary>
        /// Días pagados de un periodo: días laborables (limitados a los contratados por semana) menos los descontados, más el
        /// proporcional del séptimo día (el descanso se paga en la misma proporción de días laborables efectivamente trabajados).
        /// </summary>
        public static DesgloseDiasPagados CalcularDiasPagados(decimal diasPeriodo, int diasDescanso, decimal diasDescontados, int? diasSemana = null)
        {
            decimal laborables = Math.Max(0m, diasPeriodo - diasDescanso);
            decimal contratados = CalcularDiasContratados(diasPeriodo, laborables, diasSemana);
            decimal laborablesPagados = Math.Max(0m, contratados - diasDescontados);
            decimal septimo = diasDescanso > 0 && laborables > 0m ? diasDescanso * laborablesPagados / laborables : 0m;
            return new DesgloseDiasPagados
            {
                DiasLaborables = laborables,
                DiasLaborablesPagados = laborablesPagados,
                DiasSeptimo = septimo,
                DiasPagados = laborablesPagados + septimo
            };
        }

        /// <summary>Días laborables que el contrato cubre en el periodo: los días del periodo × días por semana / 7, sin rebasar los laborables.</summary>
        public static decimal CalcularDiasContratados(decimal diasPeriodo, decimal diasLaborables, int? diasSemana)
        {
            if (!diasSemana.HasValue)
            {
                return diasLaborables;
            }
            return Math.Min(diasLaborables, Math.Round(diasPeriodo * diasSemana.Value / 7m, 4, MidpointRounding.AwayFromZero));
        }

        /// <summary>Pago adicional por días festivos laborados (LFT Art. 75), limitado a los festivos del periodo.</summary>
        public static decimal CalcularPagoFestivo(decimal diasFestivosLaborados, int festivosDelPeriodo, decimal salarioDiario, decimal factor)
        {
            decimal dias = Math.Min(diasFestivosLaborados, festivosDelPeriodo);
            return dias * salarioDiario * factor;
        }

        public static decimal CalcularPrimaDominical(decimal diasLaborados, decimal salarioDiario, decimal porcentaje)
        {
            return diasLaborados * salarioDiario * porcentaje / 100m;
        }

        /// <summary>
        /// Concilia el recibo contra sus propios conceptos y, además, contra las reglas de la prenómina recalculadas con las
        /// incidencias del empleado: días pagados (séptimo día), festivos laborados, prima dominical, bonos y descuentos.
        /// </summary>
        public static List<string> ConciliarRecibo(DocReciboNomina recibo, IList<DocReciboDetalle> detalles, ContextoConciliacion contexto)
        {
            var conceptos = contexto == null ? new List<ProcIncidenciaConcepto>() :
                (contexto.ConceptosIncidencia ?? new List<ProcIncidenciaConcepto>());
            decimal sueldoAdicional = conceptos.Where(i => i.Concepto.TipoConcepto == "P" && i.Concepto.ClaveSAT == "001")
                .Sum(i => i.ImporteCalculado);
            List<string> problemas = ConciliarReciboInterno(recibo, detalles, sueldoAdicional, contexto != null && contexto.SueldoOmitido);
            if (contexto == null || contexto.Periodo == null)
            {
                return problemas;
            }

            IncidenciasEmpleado inc = contexto.Incidencias ?? new IncidenciasEmpleado();
            DesgloseDiasPagados dias = CalcularDiasPagados(contexto.Periodo.DiasPeriodo, ContarDescansos(contexto.Periodo), inc.DiasDescontados,
                contexto.DiasLaboradosSemana);
            if (recibo.SnapshotDiasPagados.HasValue && Math.Abs(dias.DiasPagados - recibo.SnapshotDiasPagados.Value) > ToleranciaDias)
            {
                problemas.Add("Los días pagados (" + recibo.SnapshotDiasPagados.Value.ToString("0.####") +
                              ") no coinciden con los esperados por periodo, séptimo día e incidencias (" + dias.DiasPagados.ToString("0.####") + ").");
            }

            decimal salarioDiario = recibo.SnapshotSalarioDiario ?? 0m;
            decimal esperadoOtrosIngresos = CalcularPagoFestivo(inc.DiasFestivosLaborados, contexto.Periodo.TotalFestivos, salarioDiario, contexto.FactorFestivo)
                                            + inc.Bonos + conceptos
                                                .Where(i => i.Concepto.TipoConcepto == "P" && i.Concepto.ClaveSAT == "038").Sum(i => i.ImporteCalculado);
            ComparaConcepto(problemas, detalles, "P", "038", esperadoOtrosIngresos, "días festivos laborados y bonos");
            ComparaConcepto(problemas, detalles, "P", "020",
                CalcularPrimaDominical(inc.DiasPrimaDominical, salarioDiario, contexto.PorcentajePrimaDominical) +
                conceptos.Where(i => i.Concepto.TipoConcepto == "P" && i.Concepto.ClaveSAT == "020").Sum(i => i.ImporteCalculado), "prima dominical");
            ComparaConcepto(problemas, detalles, "D", "004", inc.Descuentos +
                conceptos.Where(i => i.Concepto.TipoConcepto == "D" && i.Concepto.ClaveSAT == "004")
                    .Sum(i => i.ImporteCalculado), "descuentos");
            return problemas;
        }

        private static void ComparaConcepto(List<string> problemas, IList<DocReciboDetalle> detalles, string tipo, string clave,
                                            decimal esperado, string nombre)
        {
            decimal actual = detalles.Where(d => d.TipoConcepto == tipo && d.ClaveConceptoSAT == clave).Sum(d => d.ImporteTotal);
            if (Math.Abs(actual - esperado) > Tolerancia)
            {
                problemas.Add("El importe de " + nombre + " (" + actual.ToString("N2") + ") no coincide con el calculado desde las incidencias (" +
                              esperado.ToString("N2") + ").");
            }
        }

        /// <summary>
        /// Concilia un recibo contra sus propios conceptos y devuelve las inconsistencias encontradas
        /// (lista vacía si el recibo es coherente).
        /// </summary>
        public static List<string> ConciliarRecibo(DocReciboNomina recibo, IList<DocReciboDetalle> detalles)
        {
            return ConciliarReciboInterno(recibo, detalles, 0m);
        }

        private static List<string> ConciliarReciboInterno(DocReciboNomina recibo, IList<DocReciboDetalle> detalles, decimal sueldoAdicional,
                                                           bool sueldoOmitido = false)
        {
            List<string> problemas = new List<string>();
            decimal percepciones = 0m, otrosPagos = 0m, deducciones = 0m, sueldo = 0m;

            foreach (DocReciboDetalle d in detalles)
            {
                decimal importe = d.ImporteGravado + d.ImporteExento;
                if (d.TipoConcepto == "P")
                {
                    percepciones += importe;
                    if (d.ClaveConceptoSAT == "001")
                    {
                        sueldo += importe;
                    }
                }
                else if (d.TipoConcepto == "O")
                {
                    otrosPagos += importe;
                }
                else if (d.TipoConcepto == "D")
                {
                    deducciones += importe;
                }
            }

            if (!sueldoOmitido && recibo.SnapshotSalarioDiario.HasValue && recibo.SnapshotDiasPagados.HasValue)
            {
                decimal esperado = CalcularSalarioBase(recibo.SnapshotSalarioDiario.Value, recibo.SnapshotDiasPagados.Value) + sueldoAdicional;
                if (Math.Abs(esperado - sueldo) > Tolerancia)
                {
                    problemas.Add("El sueldo (" + sueldo.ToString("N2") + ") no coincide con salario diario × días pagados (" + esperado.ToString("N2") + ").");
                }
            }

            // sp_CalcularNomina incluye el subsidio (otro pago) dentro de TotalPercepciones; se aceptan ambas formas.
            if (Math.Abs((percepciones + otrosPagos) - recibo.TotalPercepciones) > Tolerancia &&
                Math.Abs(percepciones - recibo.TotalPercepciones) > Tolerancia)
            {
                problemas.Add("El total de percepciones (" + recibo.TotalPercepciones.ToString("N2") + ") no coincide con la suma de sus conceptos (" +
                              (percepciones + otrosPagos).ToString("N2") + ").");
            }

            if (Math.Abs(deducciones - recibo.TotalDeducciones) > Tolerancia)
            {
                problemas.Add("El total de deducciones (" + recibo.TotalDeducciones.ToString("N2") + ") no coincide con la suma de sus conceptos (" +
                              deducciones.ToString("N2") + ").");
            }

            if (Math.Abs(CalcularNeto(recibo.TotalPercepciones, recibo.TotalDeducciones) - recibo.NetoPagar) > Tolerancia)
            {
                problemas.Add("El neto a pagar (" + recibo.NetoPagar.ToString("N2") + ") no es percepciones menos deducciones.");
            }

            if (recibo.NetoPagar < 0m)
            {
                problemas.Add("El neto a pagar es negativo (" + recibo.NetoPagar.ToString("N2") + "): las deducciones superan las percepciones.");
            }

            return problemas;
        }
    }
}

