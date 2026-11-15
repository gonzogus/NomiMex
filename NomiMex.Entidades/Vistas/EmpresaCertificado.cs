using System;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Certificado de sello digital (CSD) de una empresa, sin el contenido binario ni la contraseña (dbo.HistEmpresaCSD).</summary>
    public class EmpresaCertificado
    {
        public int CSDId { get; set; }
        public int EmpresaId { get; set; }
        public string NoCertificado { get; set; }
        public DateTime VigenciaDesde { get; set; }
        public DateTime VigenciaHasta { get; set; }
        public bool Activo { get; set; }
    }
}
