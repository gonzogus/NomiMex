namespace NomiMex.Entidades.Vistas
{
    /// <summary>Totales del periodo tras persistir los recibos definitivos y cerrarlo.</summary>
    public class ResultadoCierreNomina
    {
        public int PeriodoId { get; set; }
        public int TotalRecibos { get; set; }
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalNeto { get; set; }
        public string EstatusPeriodo { get; set; }
    }
}
