namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatDepartamento.</summary>
    public class CatDepartamento
    {
        public int DepartamentoId { get; set; }
        public int EmpresaId { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
