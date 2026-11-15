using NomiMex.Entidades.Catalogos;

namespace NomiMex.Entidades.Relaciones
{
    /// <summary>
    /// Tabla dbo.RelPaqueteConcepto: conceptos SAT que integran un paquete de incidencias por defecto, con el valor
    /// base y la bandera de permanencia con la que se asignan al empleado.
    /// </summary>
    public class RelPaqueteConcepto
    {
        public int IdPaqueteConcepto { get; set; }
        public int IdPaquete { get; set; }
        public int IdConceptoSAT { get; set; }
        public decimal CantidadOValor { get; set; }
        public bool EsPermanente { get; set; }
        public bool Activo { get; set; }
        public CatConcepto Concepto { get; set; }

        public string ConceptoTexto
        {
            get { return Concepto == null ? string.Empty : Concepto.Texto; }
        }

        public string TipoTexto
        {
            get { return Concepto == null ? string.Empty : Concepto.TipoConcepto == "P" ? "Percepción" : Concepto.TipoConcepto == "D" ? "Deducción" : "Otro pago"; }
        }

        public string CantidadTexto
        {
            get { return Concepto != null && Concepto.EsBase ? "Automático" : CantidadOValor.ToString("0.####"); }
        }

        public string AlcanceTexto
        {
            get { return Concepto != null && Concepto.EsBase ? "Base (automático)" : EsPermanente ? "Permanente" : "Temporal"; }
        }
    }
}
