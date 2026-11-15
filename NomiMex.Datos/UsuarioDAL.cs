using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Seguridad;

namespace NomiMex.Datos
{
    /// <summary>
    /// Acceso a seguridad: dbo.CatUsuario, dbo.CatRol, dbo.RelUsuarioRol y los procedimientos sp_Auth_*.
    /// Las contraseñas se procesan siempre en el servidor (SHA2_512 con sal) y nunca viajan de regreso.
    /// </summary>
    public class UsuarioDAL
    {
        private const string SelectUsuarios =
            "SELECT u.UsuarioId, u.NombreUsuario, u.Email, u.NombreMostrar, u.Activo, u.Bloqueado, u.IntentosFallidos, " +
            "u.UltimoAccesoUtc, u.RequiereCambioPwd, u.FechaAltaUtc, u.FechaUpdateUtc, " +
            "(SELECT STRING_AGG(r.Codigo, N', ') WITHIN GROUP (ORDER BY r.Codigo) " +
            "   FROM dbo.RelUsuarioRol ur INNER JOIN dbo.CatRol r ON r.RolId = ur.RolId " +
            "  WHERE ur.UsuarioId = u.UsuarioId) AS RolesTexto " +
            "FROM dbo.CatUsuario u";

        /// <summary>Valida credenciales con sp_Auth_IniciarSesion. Lanza ExcepcionDatos si son inválidas.</summary>
        public SesionUsuario IniciarSesion(string nombreUsuario, string contrasena, string ipOrigen, string userAgent)
        {
            SqlParameter token = AccesoDatos.ParametroSalida("@TokenSesion", SqlDbType.NVarChar, 128);
            SqlParameter usuarioId = AccesoDatos.ParametroSalida("@UsuarioId", SqlDbType.Int, 0);
            SqlParameter requiereCambio = AccesoDatos.ParametroSalida("@RequiereCambio", SqlDbType.Bit, 0);

            AccesoDatos.EjecutarSP("dbo.sp_Auth_IniciarSesion",
                AccesoDatos.Parametro("@NombreUsuario", nombreUsuario),
                AccesoDatos.Parametro("@Contrasena", contrasena),
                AccesoDatos.Parametro("@IpOrigen", ipOrigen),
                AccesoDatos.Parametro("@UserAgent", userAgent),
                token, usuarioId, requiereCambio);

            SesionUsuario sesion = new SesionUsuario();
            sesion.Token = token.Value as string;
            sesion.UsuarioId = (usuarioId.Value == System.DBNull.Value) ? 0 : (int)usuarioId.Value;
            sesion.RequiereCambioPwd = requiereCambio.Value != System.DBNull.Value && (bool)requiereCambio.Value;

            CatUsuario usuario = ObtenerPorId(sesion.UsuarioId);
            if (usuario != null)
            {
                sesion.NombreUsuario = usuario.NombreUsuario;
                sesion.NombreMostrar = usuario.NombreMostrar;
                sesion.Email = usuario.Email;
            }
            sesion.Roles = ObtenerCodigosRol(sesion.UsuarioId);
            return sesion;
        }

        public void CerrarSesion(string token)
        {
            AccesoDatos.EjecutarSP("dbo.sp_Auth_CerrarSesion", AccesoDatos.Parametro("@TokenSesion", token));
        }

        public List<string> ObtenerCodigosRol(int usuarioId)
        {
            DataTable tabla = AccesoDatos.Consultar(
                "SELECT r.Codigo FROM dbo.RelUsuarioRol ur INNER JOIN dbo.CatRol r ON r.RolId = ur.RolId " +
                "WHERE ur.UsuarioId = @Id AND r.Activo = 1 ORDER BY r.Codigo",
                AccesoDatos.Parametro("@Id", usuarioId));
            List<string> roles = new List<string>();
            foreach (DataRow fila in tabla.Rows)
            {
                roles.Add(Fila.Obtener<string>(fila, "Codigo"));
            }
            return roles;
        }

        public CatUsuario ObtenerPorId(int usuarioId)
        {
            DataTable tabla = AccesoDatos.Consultar(SelectUsuarios + " WHERE u.UsuarioId = @Id",
                AccesoDatos.Parametro("@Id", usuarioId));
            return tabla.Rows.Count == 0 ? null : Mapear(tabla.Rows[0]);
        }

        public List<CatUsuario> Listar()
        {
            DataTable tabla = AccesoDatos.Consultar(SelectUsuarios + " ORDER BY u.NombreUsuario");
            List<CatUsuario> lista = new List<CatUsuario>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(Mapear(fila));
            }
            return lista;
        }

        public List<CatRol> ListarRoles()
        {
            DataTable tabla = AccesoDatos.Consultar(
                "SELECT RolId, Codigo, Descripcion, Activo FROM dbo.CatRol WHERE Activo = 1 ORDER BY RolId");
            List<CatRol> lista = new List<CatRol>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new CatRol
                {
                    RolId = Fila.Obtener<int>(fila, "RolId"),
                    Codigo = Fila.Obtener<string>(fila, "Codigo"),
                    Descripcion = Fila.Obtener<string>(fila, "Descripcion"),
                    Activo = Fila.Obtener<bool>(fila, "Activo")
                });
            }
            return lista;
        }

        /// <summary>Crea el usuario y asigna sus roles en una sola transacción.</summary>
        public int Crear(CatUsuario usuario, string contrasena, IList<string> codigosRol)
        {
            return AccesoDatos.EjecutarEnTransaccion<int>(delegate(SqlConnection cn, SqlTransaction tx)
            {
                SqlParameter nuevoId = AccesoDatos.ParametroSalida("@UsuarioId", SqlDbType.Int, 0);
                AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_Auth_CrearUsuario",
                    AccesoDatos.Parametro("@NombreUsuario", usuario.NombreUsuario),
                    AccesoDatos.Parametro("@Email", usuario.Email),
                    AccesoDatos.Parametro("@NombreMostrar", usuario.NombreMostrar),
                    AccesoDatos.Parametro("@Contrasena", contrasena),
                    AccesoDatos.Parametro("@RequiereCambioPwd", usuario.RequiereCambioPwd),
                    nuevoId);

                int id = (int)nuevoId.Value;
                foreach (string codigo in codigosRol)
                {
                    AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_AsignarRolUsuario",
                        AccesoDatos.Parametro("@UsuarioId", id),
                        AccesoDatos.Parametro("@CodigoRol", codigo));
                }

                if (!usuario.Activo)
                {
                    AccesoDatos.EjecutarComando(cn, tx, "UPDATE dbo.CatUsuario SET Activo = 0 WHERE UsuarioId = @Id",
                        AccesoDatos.Parametro("@Id", id));
                }
                return id;
            });
        }

        /// <summary>Actualiza datos, estado, contraseña opcional y roles en una sola transacción.</summary>
        public void Actualizar(CatUsuario usuario, string nuevaContrasena, IList<string> codigosRol)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.CatUsuario SET Email = @Email, NombreMostrar = @Nombre, Activo = @Activo, " +
                    "Bloqueado = @Bloqueado, RequiereCambioPwd = @Cambio, " +
                    "IntentosFallidos = CASE WHEN @Bloqueado = 0 THEN 0 ELSE IntentosFallidos END, " +
                    "FechaUpdateUtc = SYSUTCDATETIME() WHERE UsuarioId = @Id",
                    AccesoDatos.Parametro("@Id", usuario.UsuarioId),
                    AccesoDatos.Parametro("@Email", usuario.Email),
                    AccesoDatos.Parametro("@Nombre", usuario.NombreMostrar),
                    AccesoDatos.Parametro("@Activo", usuario.Activo),
                    AccesoDatos.Parametro("@Bloqueado", usuario.Bloqueado),
                    AccesoDatos.Parametro("@Cambio", usuario.RequiereCambioPwd));

                if (!string.IsNullOrEmpty(nuevaContrasena))
                {
                    AccesoDatos.EjecutarComando(cn, tx,
                        "DECLARE @Sal VARBINARY(32) = CRYPT_GEN_RANDOM(32); " +
                        "UPDATE dbo.CatUsuario SET ContrasenaSalt = @Sal, " +
                        "ContrasenaHash = dbo.fn_Auth_HashContrasena(@Pwd, @Sal) WHERE UsuarioId = @Id",
                        AccesoDatos.Parametro("@Id", usuario.UsuarioId),
                        AccesoDatos.Parametro("@Pwd", nuevaContrasena));
                }

                DataTable actuales = AccesoDatos.Consultar(cn, tx,
                    "SELECT r.Codigo FROM dbo.RelUsuarioRol ur INNER JOIN dbo.CatRol r ON r.RolId = ur.RolId WHERE ur.UsuarioId = @Id",
                    AccesoDatos.Parametro("@Id", usuario.UsuarioId));

                List<string> codigosActuales = new List<string>();
                foreach (DataRow fila in actuales.Rows)
                {
                    codigosActuales.Add(Fila.Obtener<string>(fila, "Codigo"));
                }

                foreach (string codigo in codigosActuales)
                {
                    if (!codigosRol.Contains(codigo))
                    {
                        AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_QuitarRolUsuario",
                            AccesoDatos.Parametro("@UsuarioId", usuario.UsuarioId),
                            AccesoDatos.Parametro("@CodigoRol", codigo));
                    }
                }

                foreach (string codigo in codigosRol)
                {
                    if (!codigosActuales.Contains(codigo))
                    {
                        AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_AsignarRolUsuario",
                            AccesoDatos.Parametro("@UsuarioId", usuario.UsuarioId),
                            AccesoDatos.Parametro("@CodigoRol", codigo));
                    }
                }
            });
        }

        /// <summary>Baja lógica: desactiva al usuario y revoca sus sesiones vigentes.</summary>
        public void DesactivarLogico(int usuarioId)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.CatUsuario SET Activo = 0, FechaUpdateUtc = SYSUTCDATETIME() WHERE UsuarioId = @Id",
                    AccesoDatos.Parametro("@Id", usuarioId));
                AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.LogSesionUsuario SET RevocadaUtc = SYSUTCDATETIME() WHERE UsuarioId = @Id AND RevocadaUtc IS NULL",
                    AccesoDatos.Parametro("@Id", usuarioId));
            });
        }

        private static CatUsuario Mapear(DataRow fila)
        {
            return new CatUsuario
            {
                UsuarioId = Fila.Obtener<int>(fila, "UsuarioId"),
                NombreUsuario = Fila.Obtener<string>(fila, "NombreUsuario"),
                Email = Fila.Obtener<string>(fila, "Email"),
                NombreMostrar = Fila.Obtener<string>(fila, "NombreMostrar"),
                Activo = Fila.Obtener<bool>(fila, "Activo"),
                Bloqueado = Fila.Obtener<bool>(fila, "Bloqueado"),
                IntentosFallidos = Fila.Obtener<short>(fila, "IntentosFallidos"),
                UltimoAccesoUtc = Fila.Obtener<System.DateTime?>(fila, "UltimoAccesoUtc"),
                RequiereCambioPwd = Fila.Obtener<bool>(fila, "RequiereCambioPwd"),
                FechaAltaUtc = Fila.Obtener<System.DateTime>(fila, "FechaAltaUtc"),
                FechaUpdateUtc = Fila.Obtener<System.DateTime>(fila, "FechaUpdateUtc"),
                RolesTexto = Fila.Obtener<string>(fila, "RolesTexto")
            };
        }
    }
}
