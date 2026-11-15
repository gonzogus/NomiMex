using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using NomiMex.Datos;
using NomiMex.Entidades.Seguridad;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Recuperación de acceso por correo electrónico y cambio de contraseña (voluntario o forzado).
    /// Genera códigos de verificación seguros con RandomNumberGenerator, aplica la política de
    /// contraseñas y delega el cifrado al servidor a través de los procedimientos sp_Auth_*.
    /// </summary>
    public class RecuperacionBLL : BllBase
    {
        public const int MinutosVigencia = 15;
        private const int DigitosCodigo = 8;

        private readonly RecuperacionDAL _dal = new RecuperacionDAL();

        /// <summary>
        /// Genera y envía por correo un código temporal de recuperación. Devuelve el momento de
        /// expiración (UTC) para informar al usuario cuánto tiempo tiene para usarlo.
        /// </summary>
        public DateTime SolicitarCodigo(string correo)
        {
            string email = Validador.Correo(correo, "Correo electrónico", true, 150);
            string codigo = GenerarCodigoNumerico(DigitosCodigo);

            SolicitudRecuperacion solicitud = Ejecutar(() => _dal.GenerarCodigo(email, codigo, MinutosVigencia, ObtenerIPLocal()));

            ServicioCorreo.EnviarCodigoRecuperacion(solicitud.Email, solicitud.NombreMostrar, codigo,
                solicitud.ExpiraUtc, MinutosVigencia);

            return solicitud.ExpiraUtc;
        }

        /// <summary>Valida el código recibido y restablece la contraseña del usuario.</summary>
        public void Restablecer(string correo, string codigo, string nuevaContrasena, string confirmacion)
        {
            string email = Validador.Correo(correo, "Correo electrónico", true, 150);

            string codigoLimpio = (codigo ?? string.Empty).Trim();
            if (codigoLimpio.Length == 0)
            {
                throw new ExcepcionNegocio("Capture el código de verificación que recibió por correo.");
            }

            PoliticaContrasena.Validar(email, nuevaContrasena, confirmacion);

            Ejecutar(() => _dal.Restablecer(email, codigoLimpio, nuevaContrasena, ObtenerIPLocal()));
        }

        /// <summary>Cambia la contraseña del usuario de la sesión actual validando la contraseña vigente.</summary>
        public void CambiarPropia(string contrasenaActual, string nuevaContrasena, string confirmacion)
        {
            if (!SesionActual.HaySesion)
            {
                throw new ExcepcionNegocio("No hay una sesión activa.");
            }
            if (string.IsNullOrEmpty(contrasenaActual))
            {
                throw new ExcepcionNegocio("Capture su contraseña actual.");
            }

            SesionUsuario usuario = SesionActual.Usuario;
            PoliticaContrasena.Validar(usuario.NombreUsuario, nuevaContrasena, confirmacion);

            if (string.Equals(nuevaContrasena, contrasenaActual, StringComparison.Ordinal))
            {
                throw new ExcepcionNegocio("La nueva contraseña debe ser diferente de la actual.");
            }

            Ejecutar(() => _dal.CambiarContrasena(usuario.UsuarioId, contrasenaActual, nuevaContrasena));
            usuario.RequiereCambioPwd = false;
        }

        /// <summary>Genera un código numérico seguro sin sesgo de módulo usando RandomNumberGenerator.</summary>
        private static string GenerarCodigoNumerico(int digitos)
        {
            StringBuilder codigo = new StringBuilder(digitos);
            byte[] buffer = new byte[1];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                while (codigo.Length < digitos)
                {
                    rng.GetBytes(buffer);
                    // 250..255 se descartan para que los 256 valores no favorezcan dígitos bajos.
                    if (buffer[0] >= 250)
                    {
                        continue;
                    }
                    codigo.Append((char)('0' + (buffer[0] % 10)));
                }
            }
            return codigo.ToString();
        }

        private static string ObtenerIPLocal()
        {
            try
            {
                foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (SocketException)
            {
                // Sin resolución de nombre disponible: se registra la dirección local.
            }
            return "127.0.0.1";
        }
    }
}
