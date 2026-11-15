using System.IO;
using NomiMex.Datos;
using NomiMex.Entidades.Configuracion;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Reglas de negocio de la configuración técnica de la aplicación: validación de campos
    /// obligatorios, formato de correo, rangos de puertos y existencia de directorios; prueba de
    /// conexión a SQL Server (vía ConfiguracionDAL) y prueba de envío SMTP (vía ServicioCorreo).
    /// </summary>
    public class ConfiguracionBLL : BllBase
    {
        private readonly ConfiguracionDAL _dal = new ConfiguracionDAL();

        /// <summary>Lee la configuración técnica actual.</summary>
        public AppConfiguracion Cargar()
        {
            return Ejecutar(() => _dal.Cargar());
        }

        /// <summary>Valida y guarda la configuración técnica completa.</summary>
        public void Guardar(AppConfiguracion config)
        {
            ValidarConexion(config);
            ValidarCorreoParaGuardar(config);
            ValidarBitacoras(config);
            ValidarOperacion(config);

            Ejecutar(() => _dal.Guardar(config));
        }

        /// <summary>Valida los datos de conexión y abre una conexión real; devuelve la versión del servidor.</summary>
        public string ProbarConexion(AppConfiguracion config)
        {
            ValidarConexion(config);
            return Ejecutar(() => _dal.ProbarConexion(config));
        }

        /// <summary>Valida la configuración SMTP y envía un correo de prueba al destinatario indicado.</summary>
        public void EnviarCorreoPrueba(AppConfiguracion config, string destinatario)
        {
            ValidarCorreoParaEnvio(config);

            if (!Validador.EsCorreoValido(destinatario))
            {
                throw new ExcepcionNegocio("Capture un correo de destino válido para la prueba.");
            }

            ServicioCorreo.EnviarPrueba(config, destinatario.Trim());
        }

        #region Validaciones

        private static void ValidarConexion(AppConfiguracion config)
        {
            if (config == null)
            {
                throw new ExcepcionNegocio("No se recibió la configuración.");
            }

            Validador.Requerido(config.ServidorBaseDatos, "Servidor / Instancia", 128);
            Validador.Requerido(config.NombreBaseDatos, "Base de datos", 128);

            if (!config.EsAutenticacionWindows)
            {
                Validador.Requerido(config.UsuarioBaseDatos, "Usuario de SQL Server", 128);
            }

            ValidarRango(config.TiempoEsperaConexion, 1, 600, "Tiempo de espera de conexión");
        }

        private static void ValidarCorreoParaGuardar(AppConfiguracion config)
        {
            // El correo es opcional al guardar: solo se valida cuando ya se capturó un servidor SMTP.
            if (string.IsNullOrWhiteSpace(config.ServidorSmtp))
            {
                return;
            }
            ValidarCorreoComun(config);
        }

        private static void ValidarCorreoParaEnvio(AppConfiguracion config)
        {
            if (config == null)
            {
                throw new ExcepcionNegocio("No se recibió la configuración.");
            }
            Validador.Requerido(config.ServidorSmtp, "Servidor SMTP", 200);
            ValidarCorreoComun(config);
        }

        private static void ValidarCorreoComun(AppConfiguracion config)
        {
            ValidarRango(config.PuertoSmtp, 1, 65535, "Puerto SMTP");

            if (!Validador.EsCorreoValido(config.CorreoRemitente))
            {
                throw new ExcepcionNegocio("El correo remitente no tiene un formato válido.");
            }
        }

        private static void ValidarBitacoras(AppConfiguracion config)
        {
            Validador.Requerido(config.CarpetaLogs, "Carpeta de bitácoras", 260);
            ValidarRango(config.DiasRetencionLogs, 0, 3650, "Días de retención de bitácoras");
            ValidarDirectorioOpcional(config.RutaExportacionRecibos, "Ruta de exportación de recibos");
        }

        private static void ValidarOperacion(AppConfiguracion config)
        {
            ValidarRango(config.MinutosInactividad, 0, 480, "Minutos de inactividad");
            ValidarDirectorioOpcional(config.RutaRespaldosLocal, "Ruta de respaldos local");
        }

        private static void ValidarRango(int valor, int minimo, int maximo, string campo)
        {
            if (valor < minimo || valor > maximo)
            {
                throw new ExcepcionNegocio(
                    "El campo '" + campo + "' debe estar entre " + minimo + " y " + maximo + ".");
            }
        }

        private static void ValidarDirectorioOpcional(string ruta, string campo)
        {
            if (string.IsNullOrWhiteSpace(ruta))
            {
                return;
            }
            if (!Directory.Exists(ruta.Trim()))
            {
                throw new ExcepcionNegocio("La carpeta indicada en '" + campo + "' no existe: " + ruta.Trim());
            }
        }

        #endregion
    }
}
