using System.Collections.Generic;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>
    /// Resumen del resultado de importar un archivo de Excel del SAT sobre un catálogo:
    /// cuántos renglones se leyeron, cuántos se insertaron y cuántos se actualizaron, más los
    /// renglones que se omitieron por venir incompletos o con datos inválidos.
    /// </summary>
    public class ResultadoImportacionSat
    {
        public ResultadoImportacionSat()
        {
            Omitidos = new List<string>();
        }

        public string Catalogo { get; set; }
        public int RenglonesLeidos { get; set; }
        public int Insertados { get; set; }
        public int Actualizados { get; set; }
        public List<string> Omitidos { get; private set; }

        public int Procesados
        {
            get { return Insertados + Actualizados; }
        }

        /// <summary>Mensaje legible para mostrar al usuario al terminar la importación.</summary>
        public string Resumen()
        {
            string texto = "Catálogo: " + Catalogo + "\n" +
                           "Renglones leídos del archivo: " + RenglonesLeidos + "\n" +
                           "Registros nuevos (INSERT): " + Insertados + "\n" +
                           "Registros actualizados (UPDATE): " + Actualizados;
            if (Omitidos.Count > 0)
            {
                texto += "\nRenglones omitidos: " + Omitidos.Count;
            }
            return texto;
        }
    }
}
