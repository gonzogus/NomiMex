using System;
using System.Collections.Generic;
using System.Globalization;

namespace NomiMex.Entidades.Constantes
{
    /// <summary>Días de la semana con numeración ISO 8601 (1 = lunes ... 7 = domingo), la usada en la base de datos.</summary>
    public static class DiaSemanaISO
    {
        private static readonly string[] Nombres = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

        public static int De(DateTime fecha)
        {
            return ((int)fecha.DayOfWeek + 6) % 7 + 1;
        }

        public static string Nombre(byte? dia)
        {
            return dia.HasValue && dia.Value >= 1 && dia.Value <= 7 ? Nombres[dia.Value - 1] : "-";
        }

        public static List<KeyValuePair<int, string>> Todos()
        {
            List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();
            for (int i = 1; i <= 7; i++)
            {
                lista.Add(new KeyValuePair<int, string>(i, Nombres[i - 1]));
            }
            return lista;
        }

        /// <summary>Cantidad de veces que el día ISO indicado ocurre entre dos fechas, ambas incluidas.</summary>
        public static int Contar(DateTime desde, DateTime hasta, int diaISO)
        {
            int dias = (hasta.Date - desde.Date).Days + 1;
            if (dias <= 0)
            {
                return 0;
            }

            int salto = (diaISO - De(desde) + 7) % 7;
            return salto >= dias ? 0 : (dias - salto - 1) / 7 + 1;
        }

        public static string Texto(DateTime fecha)
        {
            return fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
