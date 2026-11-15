using NomiMex.Entidades.Catalogos;

namespace NomiMex.Entidades.Procesos
{
    /// <summary>Tabla dbo.ProcIncidencias: movimiento de un concepto SAT para un empleado en un periodo.</summary>
    public class ProcIncidenciaConcepto
    {
        public int IdIncidencia { get; set; }
        public int IdPeriodo { get; set; }
        public int IdEmpleado { get; set; }
        public int IdConceptoSAT { get; set; }
        public decimal CantidadOValor { get; set; }
        public decimal ImporteCalculado { get; set; }

        /// <summary>Verdadero si el concepto queda permanente y se sincroniza con dbo.RelEmpleadoIncidencia.</summary>
        public bool EsPermanente { get; set; }

        public string Observaciones { get; set; }
        public CatConcepto Concepto { get; set; }
    }
}
