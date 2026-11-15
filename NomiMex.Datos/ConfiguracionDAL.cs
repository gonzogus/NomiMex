using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using NomiMex.Entidades.Configuracion;

namespace NomiMex.Datos
{
    /// <summary>
    /// Acceso a la configuración técnica local de la aplicación. Lee y escribe el archivo de
    /// configuración del ejecutable (App.config / NomiMex.exe.config): la cadena de conexión en
    /// la sección &lt;connectionStrings&gt; y el resto de parámetros en &lt;appSettings&gt;.
    /// Las contraseñas se protegen en disco cifrando ambas secciones con DPAPI de Windows
    /// (DataProtectionConfigurationProvider), de forma transparente para el resto de la aplicación.
    /// También realiza la prueba real de conexión a SQL Server y la escritura física de bitácoras.
    /// </summary>
    public class ConfiguracionDAL
    {
        private const string NombreCadena = "NomiMex";
        private const string ProveedorSql = "System.Data.SqlClient";
        private const string ProveedorCifrado = "DataProtectionConfigurationProvider";

        private const string KeySmtpHost = "Smtp.Host";
        private const string KeySmtpPuerto = "Smtp.Puerto";
        private const string KeySmtpSsl = "Smtp.Ssl";
        private const string KeySmtpUsuario = "Smtp.Usuario";
        private const string KeySmtpPassword = "Smtp.Password";
        private const string KeySmtpRemitente = "Smtp.Remitente";
        private const string KeySmtpRemitenteNombre = "Smtp.RemitenteNombre";
        private const string KeySmtpTimeout = "Smtp.TimeoutMs";
        private const string KeySmtpAsuntoRecibo = "Smtp.AsuntoRecibo";
        private const string KeySmtpCuerpoRecibo = "Smtp.CuerpoRecibo";

        private const string KeyLogCarpeta = "Log.Carpeta";
        private const string KeyLogNivel = "Log.Nivel";
        private const string KeyLogRetencion = "Log.DiasRetencion";
        private const string KeyRutaExportRecibos = "Recibos.RutaExportacion";

        private const string KeySesionInactividad = "Sesion.MinutosInactividad";
        private const string KeyInicioVerificar = "Inicio.VerificarConexion";
        private const string KeyRespaldosRuta = "Respaldos.RutaLocal";

        /// <summary>Lee la configuración técnica actual del archivo de configuración local.</summary>
        public AppConfiguracion Cargar()
        {
            AppConfiguracion config = new AppConfiguracion();

            ConnectionStringSettings cadena = ConfigurationManager.ConnectionStrings[NombreCadena];
            if (cadena != null && !string.IsNullOrWhiteSpace(cadena.ConnectionString))
            {
                LeerCadenaConexion(cadena.ConnectionString, config);
            }

            config.ServidorSmtp = LeerTexto(KeySmtpHost, config.ServidorSmtp);
            config.PuertoSmtp = LeerEntero(KeySmtpPuerto, config.PuertoSmtp);
            config.UsarSsl = LeerBooleano(KeySmtpSsl, config.UsarSsl);
            config.CorreoRemitente = LeerTexto(KeySmtpRemitente, LeerTexto(KeySmtpUsuario, config.CorreoRemitente));
            config.NombreMostrar = LeerTexto(KeySmtpRemitenteNombre, config.NombreMostrar);
            config.PasswordSmtp = LeerTexto(KeySmtpPassword, config.PasswordSmtp);
            config.TiempoEsperaSmtpMs = LeerEntero(KeySmtpTimeout, config.TiempoEsperaSmtpMs);
            config.AsuntoRecibo = LeerTexto(KeySmtpAsuntoRecibo, config.AsuntoRecibo);
            config.CuerpoRecibo = LeerTexto(KeySmtpCuerpoRecibo, config.CuerpoRecibo);

            config.CarpetaLogs = LeerTexto(KeyLogCarpeta, CarpetaLogsPredeterminada());
            config.Nivel = (NivelBitacora)LeerEntero(KeyLogNivel, (int)config.Nivel);
            config.DiasRetencionLogs = LeerEntero(KeyLogRetencion, config.DiasRetencionLogs);
            config.RutaExportacionRecibos = LeerTexto(KeyRutaExportRecibos, config.RutaExportacionRecibos);

            config.MinutosInactividad = LeerEntero(KeySesionInactividad, config.MinutosInactividad);
            config.VerificarConexionAlInicio = LeerBooleano(KeyInicioVerificar, config.VerificarConexionAlInicio);
            config.RutaRespaldosLocal = LeerTexto(KeyRespaldosRuta, config.RutaRespaldosLocal);

            return config;
        }

        /// <summary>
        /// Guarda la configuración técnica en el archivo de configuración local, protege las secciones
        /// sensibles con DPAPI y actualiza la cadena de conexión en uso por la aplicación.
        /// </summary>
        public void Guardar(AppConfiguracion config)
        {
            if (config == null)
            {
                throw new ExcepcionDatos("No se recibió la configuración a guardar.");
            }

            string cadena = ConstruirCadenaConexion(config);

            try
            {
                System.Configuration.Configuration archivo =
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                ConnectionStringSettings cs = archivo.ConnectionStrings.ConnectionStrings[NombreCadena];
                if (cs == null)
                {
                    archivo.ConnectionStrings.ConnectionStrings.Add(
                        new ConnectionStringSettings(NombreCadena, cadena, ProveedorSql));
                }
                else
                {
                    cs.ConnectionString = cadena;
                    cs.ProviderName = ProveedorSql;
                }

                EscribirAjuste(archivo, KeySmtpHost, Texto(config.ServidorSmtp));
                EscribirAjuste(archivo, KeySmtpPuerto, config.PuertoSmtp.ToString(CultureInfo.InvariantCulture));
                EscribirAjuste(archivo, KeySmtpSsl, config.UsarSsl ? "true" : "false");
                EscribirAjuste(archivo, KeySmtpUsuario, Texto(config.CorreoRemitente));
                EscribirAjuste(archivo, KeySmtpPassword, Texto(config.PasswordSmtp));
                EscribirAjuste(archivo, KeySmtpRemitente, Texto(config.CorreoRemitente));
                EscribirAjuste(archivo, KeySmtpRemitenteNombre, Texto(config.NombreMostrar));
                EscribirAjuste(archivo, KeySmtpTimeout, config.TiempoEsperaSmtpMs.ToString(CultureInfo.InvariantCulture));
                EscribirAjuste(archivo, KeySmtpAsuntoRecibo, Texto(config.AsuntoRecibo));
                EscribirAjuste(archivo, KeySmtpCuerpoRecibo, Texto(config.CuerpoRecibo));

                EscribirAjuste(archivo, KeyLogCarpeta, Texto(config.CarpetaLogs));
                EscribirAjuste(archivo, KeyLogNivel, ((int)config.Nivel).ToString(CultureInfo.InvariantCulture));
                EscribirAjuste(archivo, KeyLogRetencion, config.DiasRetencionLogs.ToString(CultureInfo.InvariantCulture));
                EscribirAjuste(archivo, KeyRutaExportRecibos, Texto(config.RutaExportacionRecibos));

                EscribirAjuste(archivo, KeySesionInactividad, config.MinutosInactividad.ToString(CultureInfo.InvariantCulture));
                EscribirAjuste(archivo, KeyInicioVerificar, config.VerificarConexionAlInicio ? "true" : "false");
                EscribirAjuste(archivo, KeyRespaldosRuta, Texto(config.RutaRespaldosLocal));

                Proteger(archivo.ConnectionStrings.SectionInformation);
                Proteger(archivo.AppSettings.SectionInformation);

                archivo.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("connectionStrings");
                ConfigurationManager.RefreshSection("appSettings");

                // La aplicación en ejecución usará de inmediato la nueva cadena de conexión.
                ConexionBD.CadenaConexion = cadena;
            }
            catch (ConfigurationErrorsException ex)
            {
                throw new ExcepcionDatos(
                    "No fue posible guardar la configuración en el archivo local. (" + ex.Message + ")", 0, false, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ExcepcionDatos(
                    "No hay permisos para escribir el archivo de configuración. Ejecute la aplicación con los permisos " +
                    "adecuados o revise la carpeta de instalación. (" + ex.Message + ")", 0, false, ex);
            }
            catch (IOException ex)
            {
                throw new ExcepcionDatos(
                    "No fue posible escribir el archivo de configuración local. (" + ex.Message + ")", 0, false, ex);
            }
        }

        /// <summary>
        /// Abre una conexión real a SQL Server con los datos indicados y devuelve la versión del servidor.
        /// No guarda la configuración: sirve para validar antes de confirmar los cambios.
        /// </summary>
        public string ProbarConexion(AppConfiguracion config)
        {
            if (config == null)
            {
                throw new ExcepcionDatos("No se recibieron los datos de conexión a probar.");
            }

            string cadena = ConstruirCadenaConexion(config);
            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    cn.Open();
                    return cn.ServerVersion;
                }
            }
            catch (SqlException ex)
            {
                throw new ExcepcionDatos(
                    "No fue posible conectar con el servidor de base de datos. Verifique el servidor, la base de datos y " +
                    "las credenciales. (" + ex.Message + ")", ex.Number, false, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ExcepcionDatos(
                    "Los datos de conexión no son válidos. (" + ex.Message + ")", 0, false, ex);
            }
        }

        /// <summary>
        /// Escribe físicamente una línea en el archivo de bitácora del día dentro de la carpeta indicada,
        /// creándola si no existe. Respeta el nivel configurado.
        /// </summary>
        public void EscribirLog(AppConfiguracion config, NivelBitacora nivel, string modulo, string mensaje)
        {
            if (config == null || string.IsNullOrWhiteSpace(config.CarpetaLogs))
            {
                return;
            }
            if ((int)nivel > (int)config.Nivel)
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(config.CarpetaLogs);
                string archivo = Path.Combine(config.CarpetaLogs,
                    "NomiMex_" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".log");
                string linea = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) +
                    " [" + nivel.ToString().ToUpperInvariant() + "] " + (modulo ?? "General") + " - " + mensaje +
                    Environment.NewLine;
                File.AppendAllText(archivo, linea);
            }
            catch (IOException ex)
            {
                throw new ExcepcionDatos(
                    "No fue posible escribir en la carpeta de bitácoras. (" + ex.Message + ")", 0, false, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ExcepcionDatos(
                    "No hay permisos para escribir en la carpeta de bitácoras. (" + ex.Message + ")", 0, false, ex);
            }
        }

        /// <summary>Carpeta de bitácoras predeterminada: subcarpeta "Logs" junto al ejecutable.</summary>
        public static string CarpetaLogsPredeterminada()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }

        #region Internos

        private static string ConstruirCadenaConexion(AppConfiguracion config)
        {
            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder
            {
                DataSource = Texto(config.ServidorBaseDatos),
                InitialCatalog = Texto(config.NombreBaseDatos),
                ConnectTimeout = config.TiempoEsperaConexion > 0 ? config.TiempoEsperaConexion : 15,
                ApplicationName = "NomiMex",
                Encrypt = false,
                TrustServerCertificate = true
            };

            if (config.EsAutenticacionWindows)
            {
                b.IntegratedSecurity = true;
            }
            else
            {
                b.IntegratedSecurity = false;
                b.UserID = Texto(config.UsuarioBaseDatos);
                b.Password = Texto(config.PasswordBaseDatos);
            }

            return b.ConnectionString;
        }

        private static void LeerCadenaConexion(string cadena, AppConfiguracion config)
        {
            try
            {
                SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(cadena);
                config.ServidorBaseDatos = b.DataSource;
                config.NombreBaseDatos = b.InitialCatalog;
                config.TiempoEsperaConexion = b.ConnectTimeout;
                if (b.IntegratedSecurity)
                {
                    config.Autenticacion = TipoAutenticacion.Windows;
                }
                else
                {
                    config.Autenticacion = TipoAutenticacion.SqlServer;
                    config.UsuarioBaseDatos = b.UserID;
                    config.PasswordBaseDatos = b.Password;
                }
            }
            catch (ArgumentException)
            {
                // Cadena de conexión con formato no reconocido: se conservan los valores predeterminados.
            }
        }

        private static void Proteger(SectionInformation seccion)
        {
            if (!seccion.IsProtected)
            {
                seccion.ProtectSection(ProveedorCifrado);
            }
            seccion.ForceSave = true;
        }

        private static void EscribirAjuste(System.Configuration.Configuration archivo, string clave, string valor)
        {
            if (archivo.AppSettings.Settings[clave] == null)
            {
                archivo.AppSettings.Settings.Add(clave, valor);
            }
            else
            {
                archivo.AppSettings.Settings[clave].Value = valor;
            }
        }

        private static string LeerTexto(string clave, string predeterminado)
        {
            string valor = ConfigurationManager.AppSettings[clave];
            return string.IsNullOrWhiteSpace(valor) ? predeterminado : valor.Trim();
        }

        private static int LeerEntero(string clave, int predeterminado)
        {
            int valor;
            string texto = ConfigurationManager.AppSettings[clave];
            return int.TryParse(texto, NumberStyles.Integer, CultureInfo.InvariantCulture, out valor) ? valor : predeterminado;
        }

        private static bool LeerBooleano(string clave, bool predeterminado)
        {
            bool valor;
            return bool.TryParse(ConfigurationManager.AppSettings[clave], out valor) ? valor : predeterminado;
        }

        private static string Texto(string valor)
        {
            return valor == null ? string.Empty : valor.Trim();
        }

        #endregion
    }
}
