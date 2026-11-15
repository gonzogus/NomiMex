namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatEmpleadoDatosFiscales (receptor CFDI).</summary>
    public class CatEmpleadoDatosFiscales
    {
        public int EmpleadoId { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public int RegimenFiscalSAT { get; set; }
        public string CodigoPostal { get; set; }
        public string ClaveEntidadFederativa { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }
}
