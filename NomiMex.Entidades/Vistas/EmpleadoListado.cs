using System;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Fila del listado de empleados (basada en dbo.vw_EmpleadoCompleto).</summary>
    public class EmpleadoListado
    {
        public int EmpleadoId { get; set; }
        public int EmpresaId { get; set; }
        public string Empresa { get; set; }
        public string NumeroEmpleado { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Estatus { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public decimal? SD { get; set; }
        public string Departamento { get; set; }
        public string Puesto { get; set; }
        public string TipoContrato { get; set; }
        public string TipoPeriodo { get; set; }

        /// <summary>Texto visible en listas desplegables (No. - Nombre).</summary>
        public string Texto
        {
            get { return NumeroEmpleado + " - " + NombreCompleto; }
        }

        public override string ToString()
        {
            return NumeroEmpleado + " - " + NombreCompleto;
        }
    }
}
