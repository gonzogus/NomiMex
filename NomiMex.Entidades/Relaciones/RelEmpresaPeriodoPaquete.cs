using System;

namespace NomiMex.Entidades.Relaciones
{
    /// <summary>Tabla dbo.RelEmpresaPeriodoPaquete: paquete de incidencias asignado a una empresa en un periodo exacto.</summary>
    public class RelEmpresaPeriodoPaquete
    {
        public int EmpresaId { get; set; }
        public int PeriodoId { get; set; }
        public int IdPaquete { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string UsuarioAsignacion { get; set; }
    }
}
