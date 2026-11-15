using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Día festivo con el nombre de la empresa a la que pertenece (lectura).</summary>
    public class DiaFestivoListado : CatDiaFestivo
    {
        public string Empresa { get; set; }

        public string Alcance
        {
            get { return Tipo == TipoDiaFestivo.Ley ? "De ley (todas las empresas)" : Empresa; }
        }

        public string DiaSemana
        {
            get { return Fecha.ToString("dddd", new System.Globalization.CultureInfo("es-MX")); }
        }
    }
}
