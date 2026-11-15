namespace NomiMex.Entidades.Vistas
{
    /// <summary>Parámetros de política/legales necesarios para calcular el SDI y el SBC de un empleado.</summary>
    public class ParametrosIntegracion
    {
        public decimal DiasAguinaldo { get; set; }
        public decimal PrimaVacacionalPorcentaje { get; set; }
        public decimal DiasVacaciones { get; set; }
        public decimal UMADiaria { get; set; }
    }
}
