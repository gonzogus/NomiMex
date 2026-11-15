using System;

namespace NomiMex.Entidades.Documentos
{
    /// <summary>Tabla dbo.DocContratoEmpleado.</summary>
    public class DocContratoEmpleado
    {
        public int EmpleadoContratoId { get; set; }
        public int EmpleadoId { get; set; }
        public int TipoContratoId { get; set; }
        public int? DepartamentoId { get; set; }
        public int? PuestoId { get; set; }
        public int? CentroCostoId { get; set; }
        public int TipoJornadaId { get; set; }
        public int PeriodicidadPagoId { get; set; }
        public int TipoRegimenId { get; set; }
        public decimal? SalarioBaseCotizacion { get; set; }
        public decimal? SalarioDiarioIntegrado { get; set; }
        public string ZonaSMG { get; set; }

        /// <summary>Días que el empleado labora por semana (1 a 7) fijos en su contrato; nulo = los de su tipo de jornada.</summary>
        public int? DiasLaboradosSemana { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activo { get; set; }
    }
}
