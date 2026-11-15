namespace NomiMex.Entidades.Configuracion
{
    /// <summary>Nivel de detalle que se registra en los archivos de bitácora locales (.log).</summary>
    public enum NivelBitacora
    {
        /// <summary>Solo se registran errores.</summary>
        SoloErrores = 0,

        /// <summary>Se registran advertencias y errores.</summary>
        AdvertenciasYErrores = 1,

        /// <summary>Se registra todo, incluyendo mensajes informativos y de depuración.</summary>
        Informativo = 2
    }

    /// <summary>
    /// Configuración técnica y de infraestructura local de la aplicación: conexión a la base de datos,
    /// correo SMTP para el envío de recibos, bitácoras (logs) y parámetros generales de operación.
    /// Es un POCO sin lógica: se lee y se escribe en el archivo de configuración local (App.config)
    /// a través de <c>ConfiguracionDAL</c>.
    /// </summary>
    public class AppConfiguracion
    {
        public AppConfiguracion()
        {
            // Valores predeterminados razonables para una instalación nueva.
            Autenticacion = TipoAutenticacion.SqlServer;
            TiempoEsperaConexion = 15;

            PuertoSmtp = 587;
            UsarSsl = true;
            TiempoEsperaSmtpMs = 30000;
            AsuntoRecibo = "NomiMex - Recibo de nómina";
            CuerpoRecibo = "Adjunto encontrará su recibo de nómina.";

            Nivel = NivelBitacora.AdvertenciasYErrores;
            DiasRetencionLogs = 30;

            MinutosInactividad = 15;
            VerificarConexionAlInicio = true;
        }

        #region Conexión a la base de datos (SQL Server)

        public string ServidorBaseDatos { get; set; }
        public string NombreBaseDatos { get; set; }
        public TipoAutenticacion Autenticacion { get; set; }
        public string UsuarioBaseDatos { get; set; }
        public string PasswordBaseDatos { get; set; }

        /// <summary>Tiempo de espera de conexión (Connect Timeout) en segundos.</summary>
        public int TiempoEsperaConexion { get; set; }

        public bool EsAutenticacionWindows
        {
            get { return Autenticacion == TipoAutenticacion.Windows; }
        }

        #endregion

        #region Correo electrónico (SMTP) para el envío de recibos

        public string ServidorSmtp { get; set; }
        public int PuertoSmtp { get; set; }
        public bool UsarSsl { get; set; }
        public string CorreoRemitente { get; set; }
        public string NombreMostrar { get; set; }
        public string PasswordSmtp { get; set; }
        public string AsuntoRecibo { get; set; }
        public string CuerpoRecibo { get; set; }

        /// <summary>Tiempo de espera del cliente SMTP en milisegundos.</summary>
        public int TiempoEsperaSmtpMs { get; set; }

        #endregion

        #region Bitácoras (logs) y rutas del sistema

        public string CarpetaLogs { get; set; }
        public NivelBitacora Nivel { get; set; }
        public int DiasRetencionLogs { get; set; }
        public string RutaExportacionRecibos { get; set; }

        #endregion

        #region Parámetros generales de operación

        /// <summary>Minutos de inactividad para el bloqueo o cierre automático de la sesión. 0 desactiva el bloqueo.</summary>
        public int MinutosInactividad { get; set; }

        public bool VerificarConexionAlInicio { get; set; }
        public string RutaRespaldosLocal { get; set; }

        #endregion
    }
}
