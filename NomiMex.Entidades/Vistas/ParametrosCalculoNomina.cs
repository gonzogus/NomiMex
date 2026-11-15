using System.Collections.Generic;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Renglón de la tarifa mensual de ISR (dbo.CatTablaISR).</summary>
    public class TramoISR
    {
        public int Renglon { get; set; }
        public decimal LimiteInferior { get; set; }
        public decimal LimiteSuperior { get; set; }
        public decimal CuotaFija { get; set; }
        public decimal TasaExcedente { get; set; }
    }

    /// <summary>Renglón de la tabla de subsidio al empleo (dbo.CatTablaSubsidioEmpleo).</summary>
    public class TramoSubsidio
    {
        public decimal HastaIngreso { get; set; }
        public decimal MontoSubsidio { get; set; }
    }

    /// <summary>Concepto de cuota obrero-patronal (dbo.CatCuotaIMSS) con la parte que paga el trabajador.</summary>
    public class CuotaIMSSObrera
    {
        public string ClaveConcepto { get; set; }
        public decimal PorcentajeObrero { get; set; }

        /// <summary>SBC, SBC_EXCEDENTE_3UMA o UMA_DIARIA_POR_TRAB.</summary>
        public string BaseCalculo { get; set; }
    }

    /// <summary>Tablas fiscales, UMA y política de pagos vigentes para calcular los pre-recibos de un periodo.</summary>
    public class ParametrosCalculoNomina
    {
        public ParametrosCalculoNomina()
        {
            TablaISR = new List<TramoISR>();
            TablaSubsidio = new List<TramoSubsidio>();
            CuotasIMSS = new List<CuotaIMSSObrera>();
            FactorHoraExtraDoble = 2m;
            FactorHoraExtraTriple = 3m;
            LimiteHorasExtraSinGravar = 9m;
            ExencionHorasExtraUMAsSemanal = 5m;
            PorcentajePrimaDominical = 25m;
            ExencionPrimaDominicalUMAs = 1m;
            FactorDiaFestivoLaborado = 2m;
            JornadaDiariaEstandar = 8m;
        }

        public List<TramoISR> TablaISR { get; set; }
        public List<TramoSubsidio> TablaSubsidio { get; set; }
        public List<CuotaIMSSObrera> CuotasIMSS { get; set; }

        public decimal UMADiaria { get; set; }
        public decimal SalarioMinimoGeneral { get; set; }

        public decimal FactorHoraExtraDoble { get; set; }
        public decimal FactorHoraExtraTriple { get; set; }
        public decimal LimiteHorasExtraSinGravar { get; set; }
        public decimal ExencionHorasExtraUMAsSemanal { get; set; }
        public decimal PorcentajePrimaDominical { get; set; }
        public decimal ExencionPrimaDominicalUMAs { get; set; }
        public decimal FactorDiaFestivoLaborado { get; set; }

        /// <summary>
        /// Jornada diaria estándar del ejercicio = jornada semanal de ley / 6 (8 h hasta 2026, 7.6667 h desde 2027 con la
        /// semana de 46 h). Es el divisor del salario por hora cuando el trabajador no tiene un turno con horas propias.
        /// </summary>
        public decimal JornadaDiariaEstandar { get; set; }
    }
}
