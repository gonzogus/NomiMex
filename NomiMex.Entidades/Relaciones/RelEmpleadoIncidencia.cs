namespace NomiMex.Entidades.Relaciones
{
    /// <summary>
    /// Tabla dbo.RelEmpleadoIncidencia: conceptos SAT asignados de forma individual a un trabajador. Al abrir un periodo
    /// limpio, las filas con <see cref="EsPermanente"/> y <see cref="Activo"/> en verdadero se precargan en
    /// dbo.ProcIncidencias. Se personaliza por empleado agregando o quitando conceptos del catálogo SAT.
    /// </summary>
    public class RelEmpleadoIncidencia
    {
        public int IdEmpleadoIncidencia { get; set; }
        public int IdEmpleado { get; set; }
        public int IdConceptoSAT { get; set; }

        /// <summary>Descripción del concepto SAT (solo lectura, para listados).</summary>
        public string Concepto { get; set; }

        /// <summary>Concepto base calculado por el motor de nómina: no se puede quitar de la ficha del trabajador.</summary>
        public bool EsBase { get; set; }

        public decimal CantidadOValor { get; set; }

        /// <summary>Verdadero si el concepto se precarga automáticamente en los periodos siguientes.</summary>
        public bool EsPermanente { get; set; }

        public string Observaciones { get; set; }
        public bool Activo { get; set; }

        /// <summary>Indicador para listados: "Base (automático)", "Permanente" o "Temporal".</summary>
        public string EsPermanenteTexto
        {
            get { return EsBase ? "Base (automático)" : EsPermanente ? "Permanente" : "Temporal"; }
        }

        public string CantidadTexto
        {
            get { return EsBase ? "Automático" : CantidadOValor.ToString("0.####"); }
        }
    }
}
