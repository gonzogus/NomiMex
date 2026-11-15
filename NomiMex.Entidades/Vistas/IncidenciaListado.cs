using System;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Incidencia de un periodo con datos descriptivos para el listado.</summary>
    public class IncidenciaListado
    {
        public string Codigo { get; set; }
        public int IncidenciaId { get; set; }
        public int EmpleadoId { get; set; }
        public string NumeroEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string TipoIncidencia { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public DateTime FechaIncidencia { get; set; }
        public string Estatus { get; set; }
        public string Observaciones { get; set; }
    }
}
