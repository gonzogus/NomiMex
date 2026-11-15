using NomiMex.Entidades.Seguridad;

namespace NomiMex.Negocio
{
    /// <summary>Sesión del usuario autenticado en la aplicación de escritorio.</summary>
    public static class SesionActual
    {
        public static SesionUsuario Usuario { get; private set; }

        public static bool HaySesion
        {
            get { return Usuario != null; }
        }

        internal static void Iniciar(SesionUsuario sesion)
        {
            Usuario = sesion;
        }

        internal static void Terminar()
        {
            Usuario = null;
        }
    }
}
