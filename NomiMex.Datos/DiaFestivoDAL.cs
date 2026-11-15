using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>Acceso a dbo.CatDiaFestivo. Toda modificación vuelve a vincular los festivos con los periodos aún abiertos.</summary>
    public class DiaFestivoDAL
    {
        public List<DiaFestivoListado> Listar(int anio, int? empresaId)
        {
            DataTable tabla = AccesoDatos.Consultar(
                "SELECT f.DiaFestivoId, f.EmpresaId, e.RazonSocial AS Empresa, f.Fecha, f.Anio, f.Descripcion, f.Tipo, f.Activo " +
                "FROM dbo.CatDiaFestivo f LEFT JOIN dbo.CatEmpresa e ON e.EmpresaId = f.EmpresaId " +
                "WHERE f.Anio = @Anio AND (f.EmpresaId IS NULL OR @Empresa IS NULL OR f.EmpresaId = @Empresa) " +
                "ORDER BY f.Fecha, f.Tipo",
                AccesoDatos.Parametro("@Anio", anio),
                AccesoDatos.ParametroEntero("@Empresa", empresaId));

            List<DiaFestivoListado> lista = new List<DiaFestivoListado>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new DiaFestivoListado
                {
                    DiaFestivoId = Fila.Obtener<int>(fila, "DiaFestivoId"),
                    EmpresaId = Fila.Obtener<int?>(fila, "EmpresaId"),
                    Empresa = Fila.Obtener<string>(fila, "Empresa"),
                    Fecha = Fila.Obtener<DateTime>(fila, "Fecha"),
                    Anio = Fila.Obtener<int>(fila, "Anio"),
                    Descripcion = Fila.Obtener<string>(fila, "Descripcion"),
                    Tipo = Fila.Obtener<string>(fila, "Tipo"),
                    Activo = Fila.Obtener<bool>(fila, "Activo")
                });
            }
            return lista;
        }

        public int Insertar(CatDiaFestivo f)
        {
            return AccesoDatos.EjecutarEnTransaccion<int>(delegate(SqlConnection cn, SqlTransaction tx)
            {
                object id = AccesoDatos.Escalar(cn, tx,
                    "INSERT INTO dbo.CatDiaFestivo (EmpresaId, Fecha, Descripcion, Tipo, Activo) " +
                    "VALUES (@Empresa, @Fecha, @Descripcion, @Tipo, 1); SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    AccesoDatos.ParametroEntero("@Empresa", f.EmpresaId),
                    AccesoDatos.Parametro("@Fecha", f.Fecha.Date),
                    AccesoDatos.Parametro("@Descripcion", f.Descripcion),
                    AccesoDatos.Parametro("@Tipo", f.Tipo));
                Revincular(cn, tx);
                return Convert.ToInt32(id);
            });
        }

        public void CambiarEstado(int diaFestivoId, bool activo)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                int filas = AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.CatDiaFestivo SET Activo = @Activo WHERE DiaFestivoId = @Id",
                    AccesoDatos.Parametro("@Activo", activo),
                    AccesoDatos.Parametro("@Id", diaFestivoId));
                if (filas == 0)
                {
                    throw new ExcepcionDatos("El día festivo ya no existe.", 0, true, null);
                }
                Revincular(cn, tx);
            });
        }

        /// <summary>Genera los festivos del Art. 74 LFT del año (idempotente) y devuelve cuántos festivos de ley tiene el año.</summary>
        public int GenerarFestivosLey(int anio)
        {
            return AccesoDatos.EjecutarEnTransaccion<int>(delegate(SqlConnection cn, SqlTransaction tx)
            {
                AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_GenerarDiasFestivosLey", AccesoDatos.Parametro("@Anio", anio));
                Revincular(cn, tx);
                return Convert.ToInt32(AccesoDatos.Escalar(cn, tx,
                    "SELECT COUNT(1) FROM dbo.CatDiaFestivo WHERE Anio = @Anio AND Tipo = 'LEY'",
                    AccesoDatos.Parametro("@Anio", anio)));
            });
        }

        private static void Revincular(SqlConnection cn, SqlTransaction tx)
        {
            AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_VincularFestivosPeriodos");
        }
    }
}
