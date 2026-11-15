namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Incidencia legal del motor de nómina: concepto TipoConcepto 'I' de dbo.CatConcepto (Codigo = ClaveSAT, TipoIncidenciaId = ConceptoId).</summary>
    public class CatTipoIncidencia
    {
        public int TipoIncidenciaId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool AfectaSueldo { get; set; }
        public bool DisminuyeDias { get; set; }
        public bool GeneraHorasExtra { get; set; }
        public bool Activo { get; set; }
    }
}
