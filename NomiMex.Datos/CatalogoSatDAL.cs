using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>
    /// Acceso a los catálogos oficiales del SAT. Lee el contenido de un catálogo para consultarlo y aplica,
    /// dentro de una sola transacción, los INSERT/UPDATE masivos a partir de los renglones de un archivo del SAT.
    /// Los nombres de tabla y columna provienen de descriptores de confianza definidos en la capa de negocio.
    /// </summary>
    public class CatalogoSatDAL
    {
        private static readonly Regex IdentificadorValido = new Regex(@"^[A-Za-z0-9_\.]+$", RegexOptions.Compiled);

        /// <summary>Devuelve el contenido completo del catálogo para mostrarlo en la rejilla de solo lectura.</summary>
        public DataTable Consultar(CatalogoSatDescriptor catalogo)
        {
            string tabla = Identificador(catalogo.Tabla);
            string orden = Identificador(catalogo.ColumnaId);
            return AccesoDatos.Consultar("SELECT * FROM " + tabla + " ORDER BY " + orden);
        }

        /// <summary>
        /// Aplica los renglones del archivo del SAT: actualiza los que ya existen por su clave e inserta los nuevos.
        /// Todo ocurre en una transacción, de modo que un error deja el catálogo sin cambios.
        /// Devuelve un arreglo con [insertados, actualizados].
        /// </summary>
        public int[] ActualizarMasivo(CatalogoSatDescriptor catalogo, List<CatalogoSatFila> filas)
        {
            string tabla = Identificador(catalogo.Tabla);
            string colId = Identificador(catalogo.ColumnaId);
            string colClave = Identificador(catalogo.ColumnaClave);
            string colDescripcion = Identificador(catalogo.ColumnaDescripcion);

            int nuevos = 0;
            int modificados = 0;

            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                foreach (CatalogoSatFila fila in filas)
                {
                    bool existe = ExisteClave(cn, tx, tabla, colClave, fila.Clave);
                    if (existe)
                    {
                        AccesoDatos.EjecutarComando(cn, tx, ConstruirUpdate(catalogo, tabla, colClave, colDescripcion),
                            ParametrosFila(catalogo, fila));
                        modificados++;
                    }
                    else
                    {
                        int nuevoId = SiguienteId(cn, tx, tabla, colId);
                        AccesoDatos.EjecutarComando(cn, tx, ConstruirInsert(catalogo, tabla, colId, colClave, colDescripcion),
                            ParametrosFila(catalogo, fila, nuevoId));
                        nuevos++;
                    }
                }
            });

            return new[] { nuevos, modificados };
        }

        private static bool ExisteClave(SqlConnection cn, SqlTransaction tx, string tabla, string colClave, string clave)
        {
            object resultado = AccesoDatos.Escalar(cn, tx,
                "SELECT 1 FROM " + tabla + " WHERE " + colClave + " = @Clave",
                AccesoDatos.Parametro("@Clave", clave));
            return resultado != null;
        }

        private static int SiguienteId(SqlConnection cn, SqlTransaction tx, string tabla, string colId)
        {
            object resultado = AccesoDatos.Escalar(cn, tx,
                "SELECT ISNULL(MAX(" + colId + "), 0) + 1 FROM " + tabla);
            return Convert.ToInt32(resultado);
        }

        private static string ConstruirUpdate(CatalogoSatDescriptor catalogo, string tabla, string colClave, string colDescripcion)
        {
            List<string> asignaciones = new List<string> { colDescripcion + " = @Descripcion" };
            if (!string.IsNullOrEmpty(catalogo.ColumnaTextoExtra))
            {
                asignaciones.Add(Identificador(catalogo.ColumnaTextoExtra) + " = @TextoExtra");
            }
            if (catalogo.TieneVigencia)
            {
                asignaciones.Add("FechaInicioVigencia = @Inicio");
                asignaciones.Add("FechaFinVigencia = @Fin");
            }
            if (catalogo.TieneActivo)
            {
                asignaciones.Add("Activo = @Activo");
            }
            return "UPDATE " + tabla + " SET " + string.Join(", ", asignaciones) + " WHERE " + colClave + " = @Clave";
        }

        private static string ConstruirInsert(CatalogoSatDescriptor catalogo, string tabla, string colId, string colClave, string colDescripcion)
        {
            List<string> columnas = new List<string> { colId, colClave, colDescripcion };
            List<string> valores = new List<string> { "@Id", "@Clave", "@Descripcion" };
            if (!string.IsNullOrEmpty(catalogo.ColumnaTextoExtra))
            {
                columnas.Add(Identificador(catalogo.ColumnaTextoExtra));
                valores.Add("@TextoExtra");
            }
            if (catalogo.TieneVigencia)
            {
                columnas.Add("FechaInicioVigencia");
                columnas.Add("FechaFinVigencia");
                valores.Add("@Inicio");
                valores.Add("@Fin");
            }
            if (catalogo.TieneActivo)
            {
                columnas.Add("Activo");
                valores.Add("@Activo");
            }
            return "INSERT INTO " + tabla + " (" + string.Join(", ", columnas) + ") VALUES (" + string.Join(", ", valores) + ")";
        }

        private static SqlParameter[] ParametrosFila(CatalogoSatDescriptor catalogo, CatalogoSatFila fila)
        {
            return ParametrosFila(catalogo, fila, null);
        }

        private static SqlParameter[] ParametrosFila(CatalogoSatDescriptor catalogo, CatalogoSatFila fila, int? nuevoId)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                AccesoDatos.Parametro("@Clave", fila.Clave),
                AccesoDatos.Parametro("@Descripcion", fila.Descripcion)
            };
            if (nuevoId.HasValue)
            {
                parametros.Add(AccesoDatos.ParametroEntero("@Id", nuevoId.Value));
            }
            if (!string.IsNullOrEmpty(catalogo.ColumnaTextoExtra))
            {
                parametros.Add(AccesoDatos.Parametro("@TextoExtra",
                    string.IsNullOrEmpty(fila.TextoExtra) ? fila.Descripcion : fila.TextoExtra));
            }
            if (catalogo.TieneVigencia)
            {
                parametros.Add(ParametroFecha("@Inicio", fila.InicioVigencia));
                parametros.Add(ParametroFecha("@Fin", fila.FinVigencia));
            }
            if (catalogo.TieneActivo)
            {
                parametros.Add(AccesoDatos.Parametro("@Activo", fila.Activo));
            }
            return parametros.ToArray();
        }

        private static SqlParameter ParametroFecha(string nombre, DateTime? valor)
        {
            SqlParameter p = new SqlParameter(nombre, SqlDbType.Date);
            p.Value = valor.HasValue ? (object)valor.Value : DBNull.Value;
            return p;
        }

        private static string Identificador(string nombre)
        {
            if (string.IsNullOrEmpty(nombre) || !IdentificadorValido.IsMatch(nombre))
            {
                throw new ExcepcionDatos("Identificador de catálogo no válido: " + nombre, 0, false, null);
            }
            return nombre;
        }
    }
}
