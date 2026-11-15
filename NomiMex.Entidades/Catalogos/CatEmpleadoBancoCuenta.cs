namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatEmpleadoBancoCuenta (dispersión bancaria).</summary>
    public class CatEmpleadoBancoCuenta
    {
        public int BancoCuentaId { get; set; }
        public int EmpleadoId { get; set; }
        public int? BancoId { get; set; }
        public int? MetodoPagoId { get; set; }
        public string CLABE { get; set; }
        public bool Activa { get; set; }
    }
}
