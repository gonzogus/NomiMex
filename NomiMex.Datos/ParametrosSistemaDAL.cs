using System;
using System.Data;
using System.Data.SqlClient;
using NomiMex.Entidades.Catalogos;

namespace NomiMex.Datos
{
    /// <summary>Acceso a datos de los parámetros generales del sistema (dbo.CatParametrosSistema).</summary>
    public class ParametrosSistemaDAL
    {
        /// <summary>Devuelve los parámetros generales guardados, o null si aún no existen.</summary>
        public CatParametrosSistema Obtener()
        {
            DataTable tabla = AccesoDatos.Consultar("EXEC dbo.sp_Config_ObtenerParametros");
            if (tabla.Rows.Count == 0)
            {
                return null;
            }

            DataRow fila = tabla.Rows[0];
            return new CatParametrosSistema
            {
                IdParametro = Fila.Obtener<int>(fila, "IdParametro"),
                RutaRespaldosDefault = Fila.Obtener<string>(fila, "RutaRespaldosDefault"),
                RutaExportacionRecibos = Fila.Obtener<string>(fila, "RutaExportacionRecibos"),
                RutaArchivosSat = Fila.Obtener<string>(fila, "RutaArchivosSat"),
                DiasRetencionLogs = Fila.Obtener<int>(fila, "DiasRetencionLogs"),
                ModoTimbradoPruebas = Fila.Obtener<bool>(fila, "ModoTimbradoPruebas"),
                FechaUltimaModificacion = Fila.Obtener<DateTime>(fila, "FechaUltimaModificacion")
            };
        }

        /// <summary>Inserta o actualiza los parámetros generales. Devuelve el Id del registro.</summary>
        public int Guardar(CatParametrosSistema parametros)
        {
            SqlParameter idResultado = AccesoDatos.ParametroSalida("@IdResultado", SqlDbType.Int, 0);

            AccesoDatos.EjecutarSP("dbo.sp_Config_GuardarParametros",
                AccesoDatos.ParametroEntero("@IdParametro", parametros.IdParametro == 0 ? (int?)null : parametros.IdParametro),
                AccesoDatos.Parametro("@RutaRespaldosDefault", parametros.RutaRespaldosDefault),
                AccesoDatos.Parametro("@RutaExportacionRecibos", parametros.RutaExportacionRecibos),
                AccesoDatos.Parametro("@RutaArchivosSat", parametros.RutaArchivosSat),
                AccesoDatos.Parametro("@DiasRetencionLogs", parametros.DiasRetencionLogs),
                AccesoDatos.Parametro("@ModoTimbradoPruebas", parametros.ModoTimbradoPruebas),
                idResultado);

            return idResultado.Value == DBNull.Value ? parametros.IdParametro : (int)idResultado.Value;
        }
    }
}
