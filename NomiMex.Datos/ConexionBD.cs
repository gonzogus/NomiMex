using System.Configuration;
using System.Data.SqlClient;

namespace NomiMex.Datos
{
    /// <summary>
    /// Punto único de acceso a la cadena de conexión. Lee la cadena "NomiMex" del App.config
    /// del ejecutable (proyecto de presentación).
    /// </summary>
    public static class ConexionBD
    {
        private const string NombreCadena = "NomiMex";
        private static string _cadenaConexion;

        public static string CadenaConexion
        {
            get
            {
                if (_cadenaConexion == null)
                {
                    ConnectionStringSettings config = ConfigurationManager.ConnectionStrings[NombreCadena];
                    if (config == null || string.IsNullOrWhiteSpace(config.ConnectionString))
                    {
                        throw new ExcepcionDatos("No se encontró la cadena de conexión '" + NombreCadena +
                                                 "' en el archivo App.config.");
                    }
                    _cadenaConexion = config.ConnectionString;
                }
                return _cadenaConexion;
            }
            set { _cadenaConexion = value; }
        }

        /// <summary>Crea una conexión nueva (cerrada). El llamador debe liberarla con using.</summary>
        public static SqlConnection CrearConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}
