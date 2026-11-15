namespace NomiMex.Entidades.Vistas
{
    /// <summary>Resultado de generar los periodos de un ejercicio fiscal para una empresa y periodicidad.</summary>
    public class ResultadoGeneracionPeriodos
    {
        public int Anio { get; set; }
        public int TotalPeriodos { get; set; }
        public int Creados { get; set; }
        public int Actualizados { get; set; }

        /// <summary>Periodos que ya estaban en cálculo o procesados y por eso no se modificaron.</summary>
        public int Omitidos { get; set; }

        public int FestivosVinculados { get; set; }
    }
}
