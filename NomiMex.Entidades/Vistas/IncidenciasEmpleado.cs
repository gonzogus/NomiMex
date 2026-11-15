namespace NomiMex.Entidades.Vistas
{
    /// <summary>Totales de incidencias activas de un empleado en un periodo (base para conciliar el cálculo).</summary>
    public class IncidenciasEmpleado
    {
        public int EmpleadoId { get; set; }

        /// <summary>Días que disminuyen el pago (faltas, permisos sin goce, suspensiones).</summary>
        public decimal DiasDescontados { get; set; }

        public decimal DiasFestivosLaborados { get; set; }
        public decimal DiasPrimaDominical { get; set; }
        public decimal DiasAguinaldo { get; set; }
        public decimal DiasPrimaVacacional { get; set; }
        public decimal DiasAntiguedad { get; set; }
        public decimal DiasIndemnizacion { get; set; }
        public decimal Bonos { get; set; }
        public decimal Descuentos { get; set; }

        public decimal HorasExtraDobles { get; set; }
        public decimal HorasExtraTriples { get; set; }

        /// <summary>Cantidad de incidencias activas (incluye las informativas que no modifican el cálculo).</summary>
        public int TotalActivas { get; set; }
    }
}
