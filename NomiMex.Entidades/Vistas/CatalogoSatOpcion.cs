namespace NomiMex.Entidades.Vistas
{
    /// <summary>
    /// Opción de la lista desplegable de catálogos oficiales del SAT disponibles para consultar y actualizar
    /// (Régimen Fiscal, Tipo de Contrato, etc.). Clave identifica el catálogo; Nombre es el texto visible.
    /// </summary>
    public class CatalogoSatOpcion
    {
        public string Clave { get; set; }
        public string Nombre { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
