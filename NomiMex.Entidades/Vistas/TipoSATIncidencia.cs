namespace NomiMex.Entidades.Vistas
{
    public class TipoSATIncidencia
    {
        public string Clave { get; set; }
        public string TipoMovimiento { get; set; }
        public string Descripcion { get; set; }
        public string Texto { get { return Clave + " - " + Descripcion; } }
    }
}
