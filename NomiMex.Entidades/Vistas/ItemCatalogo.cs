namespace NomiMex.Entidades.Vistas
{
    /// <summary>
    /// Elemento genérico de lista desplegable para catálogos de solo lectura
    /// (SAT, bancos, tipos de contrato, etc.). Id numérico o Clave textual según el catálogo.
    /// </summary>
    public class ItemCatalogo
    {
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }

        /// <summary>Texto visible en los ComboBox (Clave - Descripción).</summary>
        public string Texto
        {
            get { return string.IsNullOrEmpty(Clave) ? Descripcion : Clave + " - " + Descripcion; }
        }

        public override string ToString()
        {
            return Texto;
        }
    }
}
