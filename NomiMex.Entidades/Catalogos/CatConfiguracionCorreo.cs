using System;

namespace NomiMex.Entidades.Catalogos
{
    /// <summary>
    /// Configuración SMTP (dbo.CatConfiguracionCorreo) para el envío de recibos (DocRecibos)
    /// y de códigos de recuperación de contraseña. La contraseña viaja cifrada en
    /// <see cref="PasswordCifrado"/>; nunca se persiste en texto plano.
    /// </summary>
    public class CatConfiguracionCorreo
    {
        public int IdConfigCorreo { get; set; }
        public string ServidorSmtp { get; set; }
        public int Puerto { get; set; }
        public bool UsaSsl { get; set; }
        public string CorreoRemitente { get; set; }
        public string NombreMostrar { get; set; }
        public string UsuarioSmtp { get; set; }

        /// <summary>Contraseña SMTP cifrada con AES-256 (Base64 de IV + texto cifrado).</summary>
        public string PasswordCifrado { get; set; }

        public string AsuntoDefaultRecibo { get; set; }
        public string CuerpoDefaultRecibo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }

        public CatConfiguracionCorreo()
        {
            Puerto = 587;
            UsaSsl = true;
            Activo = true;
        }
    }
}
