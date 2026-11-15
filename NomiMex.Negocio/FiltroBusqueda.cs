using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NomiMex.Negocio
{
    /// <summary>Filtro de búsqueda en memoria, sin distinguir mayúsculas ni acentos, para los filtros en tiempo real.</summary>
    public static class FiltroBusqueda
    {
        public static List<T> Filtrar<T>(IEnumerable<T> elementos, string texto, params Func<T, string>[] campos)
        {
            if (elementos == null)
            {
                return new List<T>();
            }

            string criterio = Normalizar(texto);
            if (criterio.Length == 0)
            {
                return elementos.ToList();
            }

            return elementos.Where(e => campos.Any(c => Normalizar(c(e)).Contains(criterio))).ToList();
        }

        public static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            string descompuesto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(descompuesto.Length);
            foreach (char c in descompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
