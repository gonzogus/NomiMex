using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using NomiMex.Entidades.Configuracion;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Envío de correo electrónico mediante System.Net.Mail.SmtpClient. La configuración del envío
    /// de códigos de recuperación se lee de App.config (ConfigurationManager.AppSettings); el envío de
    /// prueba del módulo de configuración usa los parámetros capturados por el usuario (sin guardar).
    /// </summary>
    internal static class ServicioCorreo
    {
        private const string KeyHost = "Smtp.Host";
        private const string KeyPuerto = "Smtp.Puerto";
        private const string KeySsl = "Smtp.Ssl";
        private const string KeyUsuario = "Smtp.Usuario";
        private const string KeyPassword = "Smtp.Password";
        private const string KeyRemitente = "Smtp.Remitente";
        private const string KeyRemitenteNombre = "Smtp.RemitenteNombre";
        private const string KeyTimeout = "Smtp.TimeoutMs";

        /// <summary>Envía el código de recuperación de contraseña al correo del usuario.</summary>
        public static void EnviarCodigoRecuperacion(string destinatario, string nombreMostrar, string codigo,
            DateTime expiraUtc, int minutosVigencia)
        {
            string host = Leer(KeyHost);
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ExcepcionNegocio(
                    "El servicio de correo no está configurado. Capture los datos del servidor SMTP en el módulo de " +
                    "configuración (o en App.config: Smtp.Host, Smtp.Puerto, Smtp.Ssl, Smtp.Usuario, Smtp.Password y Smtp.Remitente).");
            }

            string remitente = Leer(KeyRemitente);
            if (string.IsNullOrWhiteSpace(remitente))
            {
                remitente = Leer(KeyUsuario);
            }
            if (string.IsNullOrWhiteSpace(remitente))
            {
                throw new ExcepcionNegocio("Falta configurar el correo remitente (Smtp.Remitente) en App.config.");
            }

            string asunto = "NomiMex - Código de recuperación de contraseña";
            DateTime expiraLocal = expiraUtc.ToLocalTime();
            string cuerpo =
                "Hola " + (string.IsNullOrWhiteSpace(nombreMostrar) ? "usuario" : nombreMostrar) + ":" + Environment.NewLine + Environment.NewLine +
                "Recibimos una solicitud para restablecer la contraseña de su cuenta en NomiMex." + Environment.NewLine + Environment.NewLine +
                "Su código de verificación es: " + codigo + Environment.NewLine + Environment.NewLine +
                "Este código es válido durante " + minutosVigencia + " minutos (hasta las " +
                expiraLocal.ToString("HH:mm", CultureInfo.CurrentCulture) + " del " +
                expiraLocal.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture) + ")." + Environment.NewLine + Environment.NewLine +
                "Si usted no solicitó este cambio, ignore este mensaje; su contraseña actual seguirá siendo válida." + Environment.NewLine + Environment.NewLine +
                "NomiMex - Nómina y Recursos Humanos";

            EnviarMensaje(host, LeerEntero(KeyPuerto, 587), LeerBooleano(KeySsl, true), Leer(KeyUsuario), Leer(KeyPassword),
                remitente, Leer(KeyRemitenteNombre), LeerEntero(KeyTimeout, 30000), destinatario, asunto, cuerpo);
        }

        /// <summary>Envía un correo de prueba con los parámetros SMTP capturados en el módulo de configuración.</summary>
        public static void EnviarPrueba(AppConfiguracion config, string destinatario)
        {
            string remitente = (config.CorreoRemitente ?? string.Empty).Trim();
            string asunto = "NomiMex - Correo de prueba";
            string cuerpo =
                "Este es un correo de prueba enviado desde el módulo de configuración de NomiMex." + Environment.NewLine + Environment.NewLine +
                "Si lo recibió, la configuración del servidor SMTP es correcta." + Environment.NewLine + Environment.NewLine +
                "NomiMex - Nómina y Recursos Humanos";

            EnviarMensaje(config.ServidorSmtp, config.PuertoSmtp, config.UsarSsl, remitente, config.PasswordSmtp,
                remitente, config.NombreMostrar, config.TiempoEsperaSmtpMs, destinatario, asunto, cuerpo);
        }

        private static void EnviarMensaje(string host, int puerto, bool ssl, string usuario, string password,
            string remitente, string remitenteNombre, int timeout, string destinatario, string asunto, string cuerpo)
        {
            try
            {
                using (MailMessage mensaje = new MailMessage())
                using (SmtpClient cliente = new SmtpClient(host, puerto))
                {
                    mensaje.From = string.IsNullOrWhiteSpace(remitenteNombre)
                        ? new MailAddress(remitente)
                        : new MailAddress(remitente, remitenteNombre);
                    mensaje.To.Add(destinatario);
                    mensaje.Subject = asunto;
                    mensaje.Body = cuerpo;
                    mensaje.IsBodyHtml = false;

                    cliente.EnableSsl = ssl;
                    cliente.Timeout = timeout > 0 ? timeout : 30000;
                    cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
                    if (!string.IsNullOrWhiteSpace(usuario))
                    {
                        cliente.UseDefaultCredentials = false;
                        cliente.Credentials = new NetworkCredential(usuario, password ?? string.Empty);
                    }

                    cliente.Send(mensaje);
                }
            }
            catch (SmtpException ex)
            {
                throw new ExcepcionNegocio(
                    "No fue posible enviar el correo. Verifique la configuración del servidor SMTP. (" + ex.Message + ")", ex, true);
            }
            catch (FormatException ex)
            {
                throw new ExcepcionNegocio("La dirección de correo del remitente o del destinatario no es válida.", ex, true);
            }
        }

        private static string Leer(string clave)
        {
            string valor = ConfigurationManager.AppSettings[clave];
            return valor == null ? null : valor.Trim();
        }

        private static int LeerEntero(string clave, int predeterminado)
        {
            int valor;
            return int.TryParse(Leer(clave), NumberStyles.Integer, CultureInfo.InvariantCulture, out valor) ? valor : predeterminado;
        }

        private static bool LeerBooleano(string clave, bool predeterminado)
        {
            bool valor;
            return bool.TryParse(Leer(clave), out valor) ? valor : predeterminado;
        }
    }
}
