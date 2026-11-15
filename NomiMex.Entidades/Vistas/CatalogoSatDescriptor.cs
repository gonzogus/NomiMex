namespace NomiMex.Entidades.Vistas
{
    /// <summary>
    /// Describe cómo leer y actualizar un catálogo oficial del SAT: la tabla física, su columna llave
    /// (clave SAT) y su columna de descripción, además de si maneja vigencia y estado activo.
    /// La capa de negocio mantiene la lista blanca de estos descriptores; la de datos solo opera sobre ellos,
    /// de modo que los nombres de tabla y columna nunca provienen de la interfaz.
    /// </summary>
    public class CatalogoSatDescriptor
    {
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Tabla { get; set; }
        public string ColumnaId { get; set; }
        public string ColumnaClave { get; set; }
        public string ColumnaDescripcion { get; set; }
        public bool TieneVigencia { get; set; }
        public bool TieneActivo { get; set; }

        /// <summary>Columna de texto adicional obligatoria (p. ej. RazonSocial en bancos); null si el catálogo no la tiene.</summary>
        public string ColumnaTextoExtra { get; set; }
    }
}
