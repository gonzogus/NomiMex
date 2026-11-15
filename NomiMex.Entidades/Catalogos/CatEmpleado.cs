using System;

namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatEmpleado.</summary>
    public class CatEmpleado
    {
        public int EmpleadoId { get; set; }
        public int EmpresaId { get; set; }
        public string NumeroEmpleado { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string EstadoCivil { get; set; }
        public string Nacionalidad { get; set; }
        public string Estatus { get; set; }
        public int? TurnoId { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
