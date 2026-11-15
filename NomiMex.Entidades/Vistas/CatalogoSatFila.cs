using System;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Renglón de catálogo SAT ya extraído del archivo de Excel y listo para insertarse o actualizarse.</summary>
    public class CatalogoSatFila
    {
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public string TextoExtra { get; set; }
        public DateTime? InicioVigencia { get; set; }
        public DateTime? FinVigencia { get; set; }
        public bool Activo { get; set; }
    }
}
