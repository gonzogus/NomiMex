using System;

namespace NomiMex.Entidades.Procesos
{
    /// <summary>Movimiento de una incidencia legal en dbo.ProcIncidencias (TipoIncidenciaId = IdConceptoSAT, Cantidad = CantidadOValor).</summary>
    public class ProcIncidencia
    {
        public int IncidenciaId { get; set; }
        public int EmpleadoId { get; set; }
        public int? PeriodoId { get; set; }
        public int TipoIncidenciaId { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public DateTime FechaIncidencia { get; set; }
        public string Estatus { get; set; }
        public string Observaciones { get; set; }
    }
}
