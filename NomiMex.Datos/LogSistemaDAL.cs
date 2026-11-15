using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NomiMex.Entidades.Bitacoras;

namespace NomiMex.Datos
{
    /// <summary>Acceso a datos de la bitácora de auditoría y errores (dbo.LogSistema).</summary>
    public class LogSistemaDAL
    {
        /// <summary>Inserta un evento en la bitácora.</summary>
        public void Insertar(LogSistema log)
        {
            AccesoDatos.EjecutarSP("dbo.sp_Log_Insertar",
                AccesoDatos.Parametro("@Nivel", log.Nivel),
                AccesoDatos.Parametro("@Modulo", log.Modulo),
                AccesoDatos.Parametro("@Mensaje", log.Mensaje),
                AccesoDatos.Parametro("@DetalleTecnico", log.DetalleTecnico),
                AccesoDatos.ParametroEntero("@IdUsuario", log.IdUsuario));
        }

        /// <summary>
        /// Consulta la bitácora con filtros opcionales. <paramref name="nivel"/> null o "TODOS"
        /// no restringe por nivel; <paramref name="modulo"/> null no restringe por módulo.
        /// </summary>
        public List<LogSistema> Consultar(DateTime? fechaInicio, DateTime? fechaFin, string nivel, string modulo)
        {
            DataTable tabla = AccesoDatos.Consultar("EXEC dbo.sp_Log_Consultar @FechaInicio, @FechaFin, @Nivel, @Modulo",
                ParametroFecha("@FechaInicio", fechaInicio),
                ParametroFecha("@FechaFin", fechaFin),
                AccesoDatos.Parametro("@Nivel", string.IsNullOrEmpty(nivel) ? null : nivel),
                AccesoDatos.Parametro("@Modulo", string.IsNullOrEmpty(modulo) ? null : modulo));

            List<LogSistema> lista = new List<LogSistema>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new LogSistema
                {
                    IdLog = Fila.Obtener<long>(fila, "IdLog"),
                    FechaHora = Fila.Obtener<DateTime>(fila, "FechaHora"),
                    Nivel = Fila.Obtener<string>(fila, "Nivel"),
                    Modulo = Fila.Obtener<string>(fila, "Modulo"),
                    Mensaje = Fila.Obtener<string>(fila, "Mensaje"),
                    DetalleTecnico = Fila.Obtener<string>(fila, "DetalleTecnico"),
                    IdUsuario = fila["IdUsuario"] == DBNull.Value ? (int?)null : Fila.Obtener<int>(fila, "IdUsuario"),
                    NombreUsuario = Fila.Obtener<string>(fila, "NombreUsuario")
                });
            }
            return lista;
        }

        /// <summary>Elimina los registros anteriores a <paramref name="diasRetencion"/> días. Devuelve cuántos se eliminaron.</summary>
        public int Depurar(int diasRetencion)
        {
            SqlParameter eliminados = AccesoDatos.ParametroSalida("@Eliminados", SqlDbType.Int, 0);

            AccesoDatos.EjecutarSP("dbo.sp_Log_Depurar",
                AccesoDatos.Parametro("@DiasRetencion", diasRetencion),
                eliminados);

            return eliminados.Value == DBNull.Value ? 0 : (int)eliminados.Value;
        }

        private static SqlParameter ParametroFecha(string nombre, DateTime? valor)
        {
            SqlParameter p = new SqlParameter(nombre, SqlDbType.DateTime);
            p.Value = valor.HasValue ? (object)valor.Value : DBNull.Value;
            return p;
        }
    }
}
