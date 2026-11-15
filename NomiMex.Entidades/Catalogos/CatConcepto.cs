namespace NomiMex.Entidades.Catalogos
{
    /// <summary>
    /// Tabla dbo.CatConcepto: catálogo oficial de Ingresos y Deducciones (SAT). Es la única fuente de conceptos de
    /// incidencia. Los conceptos marcados con <see cref="EsIncidencia"/> se pueden capturar manualmente como percepción
    /// o deducción del periodo y llevan la regla de cálculo (<see cref="FormaCalculo"/> y <see cref="FactorMultiplicador"/>).
    /// </summary>
    public class CatConcepto
    {
        public int ConceptoId { get; set; }
        public string ClaveSAT { get; set; }

        /// <summary>P = Percepción, D = Deducción, O = Otro pago.</summary>
        public string TipoConcepto { get; set; }

        public string Descripcion { get; set; }
        public bool EsGravado { get; set; }

        /// <summary>MONTO_FIJO, DIAS, HORAS o PORCENTAJE_SD. Nulo cuando el concepto no se captura como incidencia.</summary>
        public string FormaCalculo { get; set; }

        public decimal FactorMultiplicador { get; set; }

        /// <summary>Verdadero cuando el concepto SAT se ofrece para capturar incidencias de percepción o deducción.</summary>
        public bool EsIncidencia { get; set; }

        /// <summary>Concepto que calcula el motor de nómina en cada recibo (sueldo, IMSS, ISR, subsidio): no se captura ni se quita.</summary>
        public bool EsBase { get; set; }

        public bool Activo { get; set; }

        /// <summary>Texto de presentación en combos: "ClaveSAT - Descripción".</summary>
        public string Texto
        {
            get { return ClaveSAT + " - " + Descripcion; }
        }

        /// <summary>Estado para listados del catálogo: habilitado para capturarse o pendiente de definir su regla.</summary>
        public string EstadoTexto
        {
            get { return EsIncidencia ? "Habilitado" : "Sin regla de cálculo"; }
        }
    }
}
