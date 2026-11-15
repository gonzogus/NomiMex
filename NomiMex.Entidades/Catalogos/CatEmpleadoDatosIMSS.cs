namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatEmpleadoDatosIMSS (seguridad social y salarios).</summary>
    public class CatEmpleadoDatosIMSS
    {
        public int EmpleadoId { get; set; }
        public string NSS { get; set; }
        public decimal SalarioDiario { get; set; }
        public decimal SalarioDiarioIntegrado { get; set; }
        public decimal SBC { get; set; }
        public int TipoRiesgo { get; set; }
    }
}
