namespace NomiMex.Entidades.Vistas
{
    /// <summary>Periodo que corresponde procesar según la fecha actual y el aviso que debe mostrarse al usuario.</summary>
    public class DeteccionPeriodo
    {
        /// <summary>Periodo detectado; nulo si no hay periodos generados que correspondan a la fecha.</summary>
        public PeriodoResumen Periodo { get; set; }

        /// <summary>Verdadero si la fecha de proceso cae dentro del periodo detectado.</summary>
        public bool EsVigente { get; set; }

        /// <summary>Periodos anteriores todavía sin procesar (abiertos) que el usuario puede seleccionar.</summary>
        public int AnterioresPendientes { get; set; }

        public string Mensaje { get; set; }
    }
}
