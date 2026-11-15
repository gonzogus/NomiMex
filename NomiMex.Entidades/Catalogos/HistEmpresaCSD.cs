using System;

namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.HistEmpresaCSD: certificado de sello digital con su llave y contraseña cifrada.</summary>
    public class HistEmpresaCSD
    {
        public int EmpresaId { get; set; }
        public string NoCertificado { get; set; }
        public byte[] CertificadoCSD { get; set; }
        public byte[] LlaveCSD { get; set; }
        public byte[] PasswordCSDCifrada { get; set; }
        public byte[] PasswordCSDIV { get; set; }
        public DateTime VigenciaDesde { get; set; }
        public DateTime VigenciaHasta { get; set; }
    }
}
