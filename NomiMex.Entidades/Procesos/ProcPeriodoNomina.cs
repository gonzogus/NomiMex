using System;

namespace NomiMex.Entidades.Procesos
{
    /// <summary>Tabla dbo.ProcPeriodoNomina.</summary>
    public class ProcPeriodoNomina
    {
        public int PeriodoId { get; set; }
        public int EmpresaId { get; set; }
        public int EjercicioId { get; set; }
        public int PeriodicidadPagoId { get; set; }
        public int NumeroPeriodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime? FechaFinalPago { get; set; }
        public decimal DiasPeriodo { get; set; }
        public bool EsInicioMes { get; set; }
        public bool EsFinMes { get; set; }
        public byte? DiaDescanso { get; set; }
        public bool CalculaSeptimoDia { get; set; }
        public DateTime? CalendarioIMSS { get; set; }
        public string Estatus { get; set; }
        public string UsuarioCalculando { get; set; }
        public DateTime? FechaInicioCalculo { get; set; }
    }
}
