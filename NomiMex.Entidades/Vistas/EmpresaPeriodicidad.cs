using NomiMex.Entidades.Relaciones;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Periodicidad asignada a una empresa, con los datos descriptivos de CatPeriodicidadPago.</summary>
    public class EmpresaPeriodicidad : RelEmpresaPeriodicidad
    {
        public string PeriodicidadClave { get; set; }
        public string PeriodicidadDescripcion { get; set; }
        public decimal DiasBase { get; set; }

        public override string ToString()
        {
            return PeriodicidadDescripcion;
        }
    }
}
