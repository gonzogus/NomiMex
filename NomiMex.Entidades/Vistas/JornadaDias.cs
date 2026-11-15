namespace NomiMex.Entidades.Vistas
{
    /// <summary>Tipo de jornada SAT con sus horas máximas y los días que se laboran por semana (nulo = jornada completa).</summary>
    public class JornadaDias
    {
        public int TipoJornadaId { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public decimal? HorasMaximasLegal { get; set; }

        /// <summary>Días por semana de la jornada (1 a 7); 0 = sin definir (jornada completa).</summary>
        public int DiasSemana { get; set; }

        public string Texto
        {
            get { return Clave + " - " + Descripcion; }
        }
    }
}
