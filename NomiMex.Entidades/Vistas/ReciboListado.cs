using System;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Fila del listado de recibos de un periodo (cabecera + datos del empleado).</summary>
    public class ReciboListado
    {
        public int ReciboId { get; set; }
        public int PeriodoId { get; set; }
        public int EmpleadoId { get; set; }
        public string NumeroEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }
        public decimal SnapshotDiasPagados { get; set; }
        public decimal? SnapshotSalarioDiario { get; set; }
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal NetoPagar { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaCalculo { get; set; }
    }
}
