using System.Collections.Generic;


namespace NomiMex.Entidades.Seguridad
{
    /// <summary>Datos de la sesión autenticada (resultado de sp_Auth_IniciarSesion + roles).</summary>
    public class SesionUsuario
    {
        public SesionUsuario()
        {
            Roles = new List<string>();
        }

        public string Token { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreMostrar { get; set; }
        public string Email { get; set; }
        public bool RequiereCambioPwd { get; set; }
        public List<string> Roles { get; set; }

        public string RolesTexto
        {
            get { return string.Join(", ", Roles); }
        }
    }
}
