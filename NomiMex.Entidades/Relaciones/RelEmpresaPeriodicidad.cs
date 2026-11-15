namespace NomiMex.Entidades.Relaciones
{
    /// <summary>
    /// Tabla dbo.RelEmpresaPeriodicidad: periodicidades de pago habilitadas para una empresa (1:N) y las reglas con las
    /// que se generan sus periodos. Los días de la semana usan numeración ISO: 1 = lunes ... 7 = domingo.
    /// </summary>
    public class RelEmpresaPeriodicidad
    {
        public int EmpresaPeriodicidadId { get; set; }
        public int EmpresaId { get; set; }
        public int PeriodicidadPagoId { get; set; }
        public string Descripcion { get; set; }
        public bool CalculaSeptimoDia { get; set; }
        public byte? DiaInicio { get; set; }
        public byte? DiaFin { get; set; }
        public byte? DiaPago { get; set; }
        public byte? DiaDescanso { get; set; }
        public bool TrabajaDomingo { get; set; }
        public decimal PorcentajePrimaDominical { get; set; }
        public bool Activo { get; set; }
    }
}
