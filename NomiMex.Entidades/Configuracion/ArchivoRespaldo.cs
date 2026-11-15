using System;
using System.Globalization;

namespace NomiMex.Entidades.Configuracion
{
    /// <summary>
    /// Metadatos de un archivo de respaldo (.bak) existente en la carpeta de respaldos,
    /// para su presentación en el historial del módulo de configuración.
    /// </summary>
    public class ArchivoRespaldo
    {
        public string NombreArchivo { get; set; }
        public string RutaCompleta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public long TamanoBytes { get; set; }

        /// <summary>Tamaño en una unidad legible (KB, MB o GB).</summary>
        public string TamanoLegible
        {
            get
            {
                double tamano = TamanoBytes;
                string[] unidades = { "bytes", "KB", "MB", "GB", "TB" };
                int i = 0;
                while (tamano >= 1024 && i < unidades.Length - 1)
                {
                    tamano /= 1024;
                    i++;
                }
                return tamano.ToString(i == 0 ? "0" : "0.0", CultureInfo.CurrentCulture) + " " + unidades[i];
            }
        }
    }
}
