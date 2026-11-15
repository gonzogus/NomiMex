using System;
using System.Net;
using System.Net.Sockets;
using NomiMex.Datos;
using NomiMex.Entidades.Seguridad;

namespace NomiMex.Negocio
{
    /// <summary>Inicio y cierre de sesión contra CatUsuario con control de acceso por rol.</summary>
    public class AutenticacionBLL : BllBase
    {
        private readonly UsuarioDAL _usuarioDal = new UsuarioDAL();

        public SesionUsuario IniciarSesion(string nombreUsuario, string contrasena)
        {
            string usuario = (nombreUsuario ?? string.Empty).Trim();
            if (usuario.Length == 0)
            {
                throw new ExcepcionNegocio("Capture el nombre de usuario.");
            }
            if (string.IsNullOrEmpty(contrasena))
            {
                throw new ExcepcionNegocio("Capture la contraseña.");
            }

            SesionUsuario sesion = Ejecutar(() => _usuarioDal.IniciarSesion(
                usuario, contrasena, ObtenerIPLocal(), "NomiMex Desktop (" + Environment.OSVersion.VersionString + ")"));

            if (sesion.Roles.Count == 0)
            {
                Ejecutar(() => _usuarioDal.CerrarSesion(sesion.Token));
                throw new ExcepcionNegocio("El usuario no tiene ningún rol activo asignado. Solicite acceso al administrador.");
            }

            SesionActual.Iniciar(sesion);
            return sesion;
        }

        public void CerrarSesion()
        {
            SesionUsuario sesion = SesionActual.Usuario;
            SesionActual.Terminar();
            if (sesion != null && !string.IsNullOrEmpty(sesion.Token))
            {
                Ejecutar(() => _usuarioDal.CerrarSesion(sesion.Token));
            }
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
