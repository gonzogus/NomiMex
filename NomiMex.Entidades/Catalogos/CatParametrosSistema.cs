using System;

namespace NomiMex.Entidades.Catalogos
{
    /// <summary>
    /// Parámetros generales de operación (dbo.CatParametrosSistema): rutas por defecto,
    /// retención de bitácora y modo de timbrado.
    /// </summary>
    public class CatParametrosSistema
    {
        public int IdParametro { get; set; }
        public string RutaRespaldosDefault { get; set; }
        public string RutaExportacionRecibos { get; set; }
        public string RutaArchivosSat { get; set; }
        public int DiasRetencionLogs { get; set; }
        public bool ModoTimbradoPruebas { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }

        public CatParametrosSistema()
        {
            DiasRetencionLogs = 90;
            ModoTimbradoPruebas = true;
        }
    }
}
