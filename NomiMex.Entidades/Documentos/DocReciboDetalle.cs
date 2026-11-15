namespace NomiMex.Entidades.Documentos
{
    /// <summary>Tabla dbo.DocReciboDetalle (P = percepción, D = deducción, O = otro pago).</summary>
    public class DocReciboDetalle
    {
        public int DetalleId { get; set; }
        public int ReciboId { get; set; }
        public string TipoConcepto { get; set; }
        public string ClaveConceptoSAT { get; set; }
        public string Descripcion { get; set; }
        public decimal ImporteGravado { get; set; }
        public decimal ImporteExento { get; set; }

        /// <summary>Días u horas que originan el importe (nulo en conceptos sin cantidad, como ISR o bonos).</summary>
        public decimal? Cantidad { get; set; }

        public decimal ImporteTotal
        {
            get { return ImporteGravado + ImporteExento; }
        }

        public string TipoConceptoTexto
        {
            get
            {
                switch (TipoConcepto)
                {
                    case "P": return "Percepción";
                    case "D": return "Deducción";
                    case "O": return "Otro pago";
                    default: return TipoConcepto;
                }
            }
        }
    }
}
