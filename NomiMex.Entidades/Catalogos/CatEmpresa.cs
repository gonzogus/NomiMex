using System;

namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatEmpresa.</summary>
    public class CatEmpresa
    {
        public int EmpresaId { get; set; }
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string RegistroPatronal { get; set; }
        public int RegimenFiscalId { get; set; }
        public string CodigoPostal { get; set; }
        public string LugarExpedicion { get; set; }
        public string ClaveEntidadFederativa { get; set; }
        public string ClaveOrigenRecurso { get; set; }
        public string NombreComercial { get; set; }
        public string RfcPatronOrigen { get; set; }
        public decimal? MontoRecursoPropio { get; set; }
        public int? RiesgoPuestoId { get; set; }
        /// <summary>Prima de riesgo de trabajo del registro patronal, en porcentaje (0.5 a 15).</summary>
        public decimal? PrimaRiesgoTrabajoPct { get; set; }
        /// <summary>Logo (imagen PNG) del encabezado del recibo. No se carga en los listados; use EmpresaBLL.ObtenerLogo.</summary>
        public byte[] Logo { get; set; }
        /// <summary>Indica que Logo debe guardarse (incluso vacío, para quitarlo); si es falso se conserva el logo actual.</summary>
        public bool LogoModificado { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
