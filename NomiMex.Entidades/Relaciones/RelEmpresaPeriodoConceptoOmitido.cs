using System;

namespace NomiMex.Entidades.Relaciones
{
    /// <summary>
    /// Tabla dbo.RelEmpresaPeriodoConceptoOmitido: concepto que una empresa omite en un periodo exacto. Si el concepto es legal,
    /// BloqueaTimbrado queda activo y los recibos del periodo no se pueden timbrar.
    /// </summary>
    public class RelEmpresaPeriodoConceptoOmitido
    {
        public int EmpresaId { get; set; }
        public int PeriodoId { get; set; }
        public int ConceptoId { get; set; }

        /// <summary>Concepto base del motor (sueldo, IMSS, ISR, subsidio) o incidencia legal.</summary>
        public bool EsLegal { get; set; }

        /// <summary>Bandera: el recibo no cumple los requisitos del CFDI de nómina y no se puede timbrar.</summary>
        public bool BloqueaTimbrado { get; set; }

        public string Motivo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
    }
}
