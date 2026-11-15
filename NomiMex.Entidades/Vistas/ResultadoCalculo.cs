using System.Collections.Generic;


namespace NomiMex.Entidades.Vistas
{
    /// <summary>Resultado de ejecutar el cálculo de un periodo, ya verificado por la capa de negocio.</summary>
    public class ResultadoCalculo
    {
        public ResultadoCalculo()
        {
            Advertencias = new List<string>();
        }

        public int PeriodoId { get; set; }
        public int TotalRecibos { get; set; }
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalNeto { get; set; }
        public List<string> Advertencias { get; set; }
    }
}
