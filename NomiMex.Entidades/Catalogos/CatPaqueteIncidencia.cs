namespace NomiMex.Entidades.Catalogos
{
    /// <summary>
    /// Tabla dbo.CatPaqueteIncidencia: paquete de conceptos SAT que se asigna por defecto a los empleados según la
    /// periodicidad de pago (y opcionalmente la empresa). Al dar de alta un empleado se precargan los conceptos del
    /// paquete por defecto de su periodicidad en dbo.RelEmpleadoIncidencia.
    /// </summary>
    public class CatPaqueteIncidencia
    {
        public int IdPaquete { get; set; }
        public string Nombre { get; set; }

        /// <summary>Periodicidad (dbo.CatPeriodicidadPago) a la que aplica el paquete; nulo si aplica a cualquiera.</summary>
        public int? PeriodicidadPagoId { get; set; }

        /// <summary>Empresa a la que pertenece el paquete; nulo si es un paquete general.</summary>
        public int? EmpresaId { get; set; }

        /// <summary>Paquete por defecto que se asigna automáticamente en el alta del empleado.</summary>
        public bool EsDefault { get; set; }

        public bool Activo { get; set; }

        /// <summary>Descripción de la periodicidad (solo lectura); "Todas" cuando el paquete aplica a cualquiera.</summary>
        public string PeriodicidadDescripcion { get; set; }

        public int TotalConceptos { get; set; }

        public string EsDefaultTexto
        {
            get { return EsDefault ? "Sí" : "No"; }
        }
    }
}
