namespace NomiMex.Entidades.Vistas
{
    /// <summary>
    /// Concepto de los paquetes que una empresa tiene asignados en un periodo, con su estado de omisión. Sirve para el grid de
    /// conceptos de la asignación de paquetes y para aplicar las omisiones al cálculo del pre-recibo.
    /// </summary>
    public class ConceptoPeriodoPaquete
    {
        public int ConceptoId { get; set; }
        public string ClaveSAT { get; set; }
        public string TipoConcepto { get; set; }
        public string Descripcion { get; set; }

        /// <summary>Concepto base que calcula el motor de nómina (sueldo, IMSS, ISR, subsidio).</summary>
        public bool EsBase { get; set; }

        /// <summary>Base o incidencia legal: omitirlo impide timbrar el periodo.</summary>
        public bool EsLegal { get; set; }

        /// <summary>Paquetes asignados en el periodo que contienen el concepto.</summary>
        public string Paquetes { get; set; }

        /// <summary>Verdadero si la empresa omite el concepto en el periodo.</summary>
        public bool Omitido { get; set; }

        public string Motivo { get; set; }

        /// <summary>Identifica un concepto base del motor: tipo y clave SAT (por ejemplo D|002 es el ISR).</summary>
        public string Llave
        {
            get { return TipoConcepto + "|" + ClaveSAT; }
        }

        public string TipoTexto
        {
            get
            {
                switch (TipoConcepto)
                {
                    case "P": return "Percepción";
                    case "D": return "Deducción";
                    case "O": return "Otro pago";
                    default: return "Incidencia";
                }
            }
        }

        public string LegalTexto
        {
            get { return EsLegal ? "Legal" : string.Empty; }
        }

        public string Concepto
        {
            get { return ClaveSAT + " - " + Descripcion; }
        }
    }
}
