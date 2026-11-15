using System;
using System.Data;
using System.Data.SqlClient;
using NomiMex.Entidades.Catalogos;

namespace NomiMex.Datos
{
    /// <summary>Acceso a datos de la configuración SMTP (dbo.CatConfiguracionCorreo).</summary>
    public class ConfiguracionCorreoDAL
    {
        /// <summary>Devuelve la configuración SMTP guardada, o null si aún no existe.</summary>
        public CatConfiguracionCorreo Obtener()
        {
            DataTable tabla = AccesoDatos.Consultar("EXEC dbo.sp_Config_ObtenerCorreo");
            if (tabla.Rows.Count == 0)
            {
                return null;
            }

            DataRow fila = tabla.Rows[0];
            return new CatConfiguracionCorreo
            {
                IdConfigCorreo = Fila.Obtener<int>(fila, "IdConfigCorreo"),
                ServidorSmtp = Fila.Obtener<string>(fila, "ServidorSmtp"),
                Puerto = Fila.Obtener<int>(fila, "Puerto"),
                UsaSsl = Fila.Obtener<bool>(fila, "UsaSsl"),
                CorreoRemitente = Fila.Obtener<string>(fila, "CorreoRemitente"),
                NombreMostrar = Fila.Obtener<string>(fila, "NombreMostrar"),
                UsuarioSmtp = Fila.Obtener<string>(fila, "UsuarioSmtp"),
                PasswordCifrado = Fila.Obtener<string>(fila, "PasswordCifrado"),
                AsuntoDefaultRecibo = Fila.Obtener<string>(fila, "AsuntoDefaultRecibo"),
                CuerpoDefaultRecibo = Fila.Obtener<string>(fila, "CuerpoDefaultRecibo"),
                Activo = Fila.Obtener<bool>(fila, "Activo"),
                FechaUltimaModificacion = Fila.Obtener<DateTime>(fila, "FechaUltimaModificacion")
            };
        }

        /// <summary>
        /// Inserta o actualiza la configuración SMTP. Si <paramref name="passwordCifrado"/> es null,
        /// el procedimiento conserva la contraseña previamente guardada. Devuelve el Id del registro.
        /// </summary>
        public int Guardar(CatConfiguracionCorreo config, string passwordCifrado)
        {
            SqlParameter idResultado = AccesoDatos.ParametroSalida("@IdResultado", SqlDbType.Int, 0);

            AccesoDatos.EjecutarSP("dbo.sp_Config_GuardarCorreo",
                AccesoDatos.ParametroEntero("@IdConfigCorreo", config.IdConfigCorreo == 0 ? (int?)null : config.IdConfigCorreo),
                AccesoDatos.Parametro("@ServidorSmtp", config.ServidorSmtp),
                AccesoDatos.Parametro("@Puerto", config.Puerto),
                AccesoDatos.Parametro("@UsaSsl", config.UsaSsl),
                AccesoDatos.Parametro("@CorreoRemitente", config.CorreoRemitente),
                AccesoDatos.Parametro("@NombreMostrar", config.NombreMostrar),
                AccesoDatos.Parametro("@UsuarioSmtp", config.UsuarioSmtp),
                AccesoDatos.Parametro("@PasswordCifrado", passwordCifrado),
                AccesoDatos.Parametro("@AsuntoDefaultRecibo", config.AsuntoDefaultRecibo),
                AccesoDatos.Parametro("@CuerpoDefaultRecibo", config.CuerpoDefaultRecibo),
                AccesoDatos.Parametro("@Activo", config.Activo),
                idResultado);

            return idResultado.Value == DBNull.Value ? config.IdConfigCorreo : (int)idResultado.Value;
        }
    }
}
