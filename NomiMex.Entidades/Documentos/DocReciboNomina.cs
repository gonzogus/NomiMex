using System;

namespace NomiMex.Entidades.Documentos
{
    /// <summary>Tabla dbo.DocReciboNomina (cabecera del recibo calculado).</summary>
    public class DocReciboNomina
    {
        public int ReciboId { get; set; }
        public int PeriodoId { get; set; }
        public int EmpleadoId { get; set; }
        public string NumFolioRecibo { get; set; }
        public string TipoNomina { get; set; }
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal NetoPagar { get; set; }
        public decimal? TotalGravado { get; set; }
        public decimal? TotalExento { get; set; }
        public int? AntiguedadSemanas { get; set; }
        public DateTime? SnapshotFechaInicialPago { get; set; }
        public DateTime? SnapshotFechaFinalPago { get; set; }
        public DateTime? SnapshotFechaPago { get; set; }
        public decimal? SnapshotDiasPagados { get; set; }
        public string SnapshotTipoJornada { get; set; }
        public string SnapshotPeriodicidadPago { get; set; }
        public string SnapshotTipoRegimen { get; set; }
        public string SnapshotTipoContrato { get; set; }
        public string SnapshotRegimenFiscalReceptor { get; set; }
        public string SnapshotBanco { get; set; }
        public string SnapshotCuentaBancaria { get; set; }
        public decimal? TotalSueldos { get; set; }
        public decimal? TotalSeparacionIndemnizacion { get; set; }
        public decimal? TotalJubilacionPensionRetiro { get; set; }
        public decimal TotalOtrosPagos { get; set; }
        public decimal? SnapshotSalarioDiario { get; set; }
        public decimal? SnapshotSDI { get; set; }
        public decimal? SnapshotSBC { get; set; }
        public string SnapshotPuesto { get; set; }
        public string SnapshotDepartamento { get; set; }
        public string SnapshotTipoContratoDesc { get; set; }
        public string SnapshotRegimenFiscalDesc { get; set; }
        public DateTime FechaCalculo { get; set; }
        public string Estatus { get; set; }

        /// <summary>Notas que se imprimen en el recibo únicamente cuando contienen texto.</summary>
        public string Notas { get; set; }
    }
}
