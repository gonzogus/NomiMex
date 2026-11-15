namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatPuesto.</summary>
    public class CatPuesto
    {
        public int PuestoId { get; set; }
        public int EmpresaId { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public decimal? SalarioBandaMin { get; set; }
        public decimal? SalarioBandaMax { get; set; }
        public bool Activo { get; set; }
    }
}
