using System;
using System.Data;
using System.Data.SqlClient;
using NomiMex.Entidades.Seguridad;

namespace NomiMex.Datos
{
    /// <summary>
    /// Acceso a datos de la recuperación de acceso por correo y del cambio de contraseña.
    /// Toda la criptografía (hash SHA2_512 con sal y hash del código) se resuelve en el
    /// servidor mediante los procedimientos sp_Auth_*; el código en claro nunca se persiste.
    /// </summary>
    public class RecuperacionDAL
    {
        /// <summary>Confirma que el correo pertenece a un usuario activo. Lanza ExcepcionDatos si no.</summary>
        public SolicitudRecuperacion ValidarCorreo(string email)
        {
            SqlParameter usuarioId = AccesoDatos.ParametroSalida("@UsuarioId", SqlDbType.Int, 0);
            SqlParameter nombre = AccesoDatos.ParametroSalida("@NombreMostrar", SqlDbType.NVarChar, 150);

            AccesoDatos.EjecutarSP("dbo.sp_Auth_ValidarCorreoRecuperacion",
                AccesoDatos.Parametro("@Email", email),
                usuarioId, nombre);

            return new SolicitudRecuperacion
            {
                UsuarioId = usuarioId.Value == DBNull.Value ? 0 : (int)usuarioId.Value,
                NombreMostrar = nombre.Value as string,
                Email = email
            };
        }

        /// <summary>
        /// Registra un nuevo código temporal (invalidando los previos no utilizados) y devuelve
        /// el usuario destino y la vigencia. El código en claro lo genera la capa de negocio.
        /// </summary>
        public SolicitudRecuperacion GenerarCodigo(string email, string codigo, int minutosVigencia, string ipSolicitud)
        {
            SqlParameter usuarioId = AccesoDatos.ParametroSalida("@UsuarioId", SqlDbType.Int, 0);
            SqlParameter nombre = AccesoDatos.ParametroSalida("@NombreMostrar", SqlDbType.NVarChar, 150);
            SqlParameter expira = AccesoDatos.ParametroSalida("@ExpiraUtc", SqlDbType.DateTime2, 0);

            AccesoDatos.EjecutarSP("dbo.sp_Auth_GenerarCodigoRecuperacion",
                AccesoDatos.Parametro("@Email", email),
                AccesoDatos.Parametro("@Codigo", codigo),
                AccesoDatos.Parametro("@MinutosVigencia", minutosVigencia),
                AccesoDatos.Parametro("@IpSolicitud", ipSolicitud),
                usuarioId, nombre, expira);

            return new SolicitudRecuperacion
            {
                UsuarioId = usuarioId.Value == DBNull.Value ? 0 : (int)usuarioId.Value,
                NombreMostrar = nombre.Value as string,
                Email = email,
                ExpiraUtc = expira.Value == DBNull.Value ? DateTime.MinValue : (DateTime)expira.Value
            };
        }

        /// <summary>
        /// Valida el código y restablece la contraseña dentro de una transacción (actualiza la
        /// contraseña cifrada y quema el token en la misma SqlTransaction).
        /// </summary>
        public void Restablecer(string email, string codigo, string nuevaContrasena, string ipSolicitud)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_Auth_RestablecerContrasena",
                    AccesoDatos.Parametro("@Email", email),
                    AccesoDatos.Parametro("@Codigo", codigo),
                    AccesoDatos.Parametro("@NuevaContrasena", nuevaContrasena),
                    AccesoDatos.Parametro("@IpSolicitud", ipSolicitud));
            });
        }

        /// <summary>
        /// Cambia la contraseña desde una sesión iniciada validando la contraseña actual,
        /// dentro de una transacción.
        /// </summary>
        public void CambiarContrasena(int usuarioId, string contrasenaActual, string nuevaContrasena)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_Auth_CambiarContrasena",
                    AccesoDatos.Parametro("@UsuarioId", usuarioId),
                    AccesoDatos.Parametro("@ContrasenaActual", contrasenaActual),
                    AccesoDatos.Parametro("@NuevaContrasena", nuevaContrasena));
            });
        }
    }
}
