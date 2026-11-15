using System;

namespace NomiMex.Entidades.Procesos
{
    /// <summary>
    /// Solicitud de recuperación de contraseña en dbo.ProcRecuperacionContrasena.
    /// Sólo se almacena el hash del código de verificación (TokenHash), nunca el valor en claro.
    /// </summary>
    public class ProcRecuperacionContrasena
    {
        public Guid RecuperacionId { get; set; }
        public int UsuarioId { get; set; }
        public byte[] TokenHash { get; set; }
        public DateTime CreadoUtc { get; set; }
        public DateTime ExpiraUtc { get; set; }
        public bool Usado { get; set; }
        public DateTime? UsadoUtc { get; set; }
        public string IpSolicitud { get; set; }
    }
}
