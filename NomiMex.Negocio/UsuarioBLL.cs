using System.Collections.Generic;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;

namespace NomiMex.Negocio
{
    /// <summary>Administración de usuarios y asignación de roles (solo SUPERADMIN).</summary>
    public class UsuarioBLL : BllBase
    {
        private readonly UsuarioDAL _dal = new UsuarioDAL();

        public List<CatUsuario> Listar()
        {
            ControlAcceso.Exigir(ModuloSistema.Usuarios, AccionSistema.Consultar);
            return Ejecutar(() => _dal.Listar());
        }

        public List<CatRol> ListarRoles()
        {
            return Ejecutar(() => _dal.ListarRoles());
        }

        public List<CatUsuario> Filtrar(IEnumerable<CatUsuario> usuarios, string texto)
        {
            return FiltroBusqueda.Filtrar(usuarios, texto, u => u.NombreUsuario, u => u.NombreMostrar, u => u.Email, u => u.RolesTexto);
        }

        public int Guardar(CatUsuario usuario, string contrasena, string confirmacion, IList<string> codigosRol)
        {
            ControlAcceso.Exigir(ModuloSistema.Usuarios, AccionSistema.Escribir);

            usuario.NombreUsuario = Validador.Requerido(usuario.NombreUsuario, "Usuario", 50);
            if (usuario.NombreUsuario.Length < 4 || usuario.NombreUsuario.Contains(" "))
            {
                throw new ExcepcionNegocio("El usuario debe tener al menos 4 caracteres y no contener espacios.");
            }
            usuario.NombreMostrar = Validador.Requerido(usuario.NombreMostrar, "Nombre", 150);
            usuario.Email = Validador.Correo(usuario.Email, "Correo electrónico", true, 150);

            if (codigosRol == null || codigosRol.Count == 0)
            {
                throw new ExcepcionNegocio("Seleccione al menos un rol para el usuario.");
            }

            bool esNuevo = usuario.UsuarioId == 0;
            bool cambiaContrasena = !string.IsNullOrEmpty(contrasena) || !string.IsNullOrEmpty(confirmacion);
            if (esNuevo || cambiaContrasena)
            {
                ValidarContrasena(usuario.NombreUsuario, contrasena, confirmacion);
            }

            if (!esNuevo && SesionActual.HaySesion && SesionActual.Usuario.UsuarioId == usuario.UsuarioId)
            {
                if (!usuario.Activo || usuario.Bloqueado)
                {
                    throw new ExcepcionNegocio("No puede desactivar ni bloquear su propio usuario.");
                }
                if (SesionActual.Usuario.Roles.Contains(RolesSistema.SuperAdmin) && !codigosRol.Contains(RolesSistema.SuperAdmin))
                {
                    throw new ExcepcionNegocio("No puede quitarse el rol de administrador a sí mismo.");
                }
            }

            if (esNuevo)
            {
                return Ejecutar(() => _dal.Crear(usuario, contrasena, codigosRol));
            }

            Ejecutar(() => _dal.Actualizar(usuario, cambiaContrasena ? contrasena : null, codigosRol));
            return usuario.UsuarioId;
        }

        public void DesactivarLogico(int usuarioId)
        {
            ControlAcceso.Exigir(ModuloSistema.Usuarios, AccionSistema.Escribir);
            if (usuarioId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione un usuario.");
            }
            if (SesionActual.HaySesion && SesionActual.Usuario.UsuarioId == usuarioId)
            {
                throw new ExcepcionNegocio("No puede eliminar su propio usuario.");
            }
            Ejecutar(() => _dal.DesactivarLogico(usuarioId));
        }

        private static void ValidarContrasena(string usuario, string contrasena, string confirmacion)
        {
            if (string.IsNullOrEmpty(contrasena))
            {
                throw new ExcepcionNegocio("Capture la contraseña.");
            }
            if (contrasena.Length < 10)
            {
                throw new ExcepcionNegocio("La contraseña debe tener al menos 10 caracteres.");
            }
            if (!contrasena.Any(char.IsLetter) || !contrasena.Any(char.IsDigit))
            {
                throw new ExcepcionNegocio("La contraseña debe combinar letras y números.");
            }
            if (string.Equals(contrasena, usuario, System.StringComparison.OrdinalIgnoreCase))
            {
                throw new ExcepcionNegocio("La contraseña no puede ser igual al usuario.");
            }
            if (contrasena != confirmacion)
            {
                throw new ExcepcionNegocio("La confirmación de la contraseña no coincide.");
            }
        }
    }
}
