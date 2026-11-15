namespace NomiMex.Entidades.Configuracion
{
    /// <summary>Tipo de autenticación de la conexión a SQL Server.</summary>
    public enum TipoAutenticacion
    {
        /// <summary>Autenticación integrada de Windows (Integrated Security).</summary>
        Windows = 0,

        /// <summary>Autenticación de SQL Server (usuario y contraseña).</summary>
        SqlServer = 1
    }

    /// <summary>
    /// DTO con los parámetros de conexión a la base de datos que se leen y escriben en la
    /// sección &lt;connectionStrings&gt; de App.config. No contiene lógica de acceso a datos.
    /// </summary>
    public class ConexionDbConfig
    {
        public string Servidor { get; set; }
        public string BaseDatos { get; set; }
        public TipoAutenticacion Autenticacion { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }

        public bool EsAutenticacionWindows
        {
            get { return Autenticacion == TipoAutenticacion.Windows; }
        }

        public ConexionDbConfig()
        {
            Autenticacion = TipoAutenticacion.SqlServer;
        }
    }
}
