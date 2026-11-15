namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatEntidadFederativa (catálogo de Estados, SAT c_CveEntidad).</summary>
    public class CatEntidadFederativa
    {
        public string CveEntidad { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public bool Activo { get; set; }
    }
}
