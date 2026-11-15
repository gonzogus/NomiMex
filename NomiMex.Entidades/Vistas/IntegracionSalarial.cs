namespace NomiMex.Entidades.Vistas
{
    /// <summary>Resultado de integrar el salario diario de un empleado (factor, SDI y SBC con tope de 25 UMA).</summary>
    public class IntegracionSalarial
    {
        public decimal SalarioDiario { get; set; }
        public decimal FactorIntegracion { get; set; }
        public decimal SDI { get; set; }
        public decimal SBC { get; set; }
        public decimal UMADiaria { get; set; }
    }
}
