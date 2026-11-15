namespace NomiMex.Entidades.Vistas
{
    /// <summary>Paquete de incidencias que puede asignarse a una empresa en un periodo, indicando si ya está asignado.</summary>
    public class PaqueteAsignacion
    {
        public int IdPaquete { get; set; }
        public string Nombre { get; set; }
        public int? EmpresaId { get; set; }
        public int? PeriodicidadPagoId { get; set; }
        public string Periodicidad { get; set; }
        public bool EsDefault { get; set; }
        public bool Activo { get; set; }
        public int TotalConceptos { get; set; }

        /// <summary>Verdadero si el paquete está asignado a la empresa en el periodo.</summary>
        public bool Asignado { get; set; }

        /// <summary>Paquete propio de la empresa o general (sin empresa), para la columna del grid.</summary>
        public string Alcance
        {
            get { return EmpresaId.HasValue ? "Empresa" : "General"; }
        }

        public string EsDefaultTexto
        {
            get { return EsDefault ? "Sí" : "No"; }
        }
    }
}
