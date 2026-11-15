using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace NomiMex.Datos
{
    /// <summary>
    /// Utilidades ADO.NET: consultas y comandos parametrizados, procedimientos almacenados y
    /// transacciones. Toda conexión, comando y lector se libera con bloques using.
    /// </summary>
    public static class AccesoDatos
    {
        private static readonly Dictionary<string, string> MensajesPorRestriccion = new Dictionary<string, string>
        {
            { "UQ_CatUsuario_NombreUsuario", "El nombre de usuario ya existe." },
            { "UQ_CatUsuario_Email", "El correo electrónico ya está registrado en otro usuario." },
            { "UQ_CatPuesto_Clave", "Ya existe un puesto con esa clave." },
            { "UQ_CatDepartamento_Clave", "Ya existe un departamento con esa clave." },
            { "UQ_CatEmpresa_RFC", "Ya existe una empresa con ese RFC." },
            { "UQ_HistEmpresaCSD_NoCert", "Ese certificado (número de serie) ya está registrado." },
            { "UQ_CatEmpleado_NumEmp", "Ya existe un empleado con ese número en la empresa." },
            { "UQ_CatEDF_RFC", "El RFC ya está registrado en otro empleado." },
            { "UQ_CatEDF_CURP", "La CURP ya está registrada en otro empleado." },
            { "UQ_CatEDI_NSS", "El NSS ya está registrado en otro empleado." },
            { "UQ_ProcPeriodoNomina", "Ya existe un periodo con ese número para la empresa, ejercicio y tipo de periodo." },
            { "UQ_RelEmpPer_EmpresaPeriodicidad", "La periodicidad ya está asignada a la empresa." },
            { "UX_CatDiaFestivo_Ley", "Ya existe un día festivo de ley en esa fecha." },
            { "UX_CatDiaFestivo_Empresa", "La empresa ya tiene registrado un día festivo en esa fecha." }
        };

        #region Parámetros

        public static SqlParameter Parametro(string nombre, object valor)
        {
            return new SqlParameter(nombre, valor ?? DBNull.Value);
        }

        public static SqlParameter ParametroDecimal(string nombre, decimal? valor, byte precision, byte escala)
        {
            SqlParameter p = new SqlParameter(nombre, SqlDbType.Decimal);
            p.Precision = precision;
            p.Scale = escala;
            p.Value = valor.HasValue ? (object)valor.Value : DBNull.Value;
            return p;
        }

        public static SqlParameter ParametroEntero(string nombre, int? valor)
        {
            SqlParameter p = new SqlParameter(nombre, SqlDbType.Int);
            p.Value = valor.HasValue ? (object)valor.Value : DBNull.Value;
            return p;
        }

        public static SqlParameter ParametroSalida(string nombre, SqlDbType tipo, int tamano)
        {
            SqlParameter p = new SqlParameter(nombre, tipo, tamano);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        #endregion

        #region Operaciones con conexión propia

        public static DataTable Consultar(string sql, params SqlParameter[] parametros)
        {
            return Ejecutar(delegate(SqlConnection cn) { return Consultar(cn, null, sql, parametros); });
        }

        public static int EjecutarComando(string sql, params SqlParameter[] parametros)
        {
            return Ejecutar(delegate(SqlConnection cn) { return EjecutarComando(cn, null, sql, parametros); });
        }

        public static int EjecutarSP(string procedimiento, params SqlParameter[] parametros)
        {
            return Ejecutar(delegate(SqlConnection cn) { return EjecutarNoQuery(cn, null, procedimiento, CommandType.StoredProcedure, parametros); });
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado con un tiempo de espera específico (en segundos),
        /// pensado para operaciones largas como BACKUP DATABASE.
        /// </summary>
        public static int EjecutarSPLargo(string procedimiento, int segundosTimeout, params SqlParameter[] parametros)
        {
            using (SqlConnection cn = ConexionBD.CrearConexion())
            {
                AbrirConexion(cn);
                try
                {
                    using (SqlCommand cmd = new SqlCommand(procedimiento, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = segundosTimeout;
                        if (parametros != null)
                        {
                            foreach (SqlParameter p in parametros)
                            {
                                cmd.Parameters.Add(p);
                            }
                        }
                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    throw Traducir(ex);
                }
            }
        }

        public static object Escalar(string sql, params SqlParameter[] parametros)
        {
            return Ejecutar(delegate(SqlConnection cn) { return Escalar(cn, null, sql, parametros); });
        }

        #endregion

        #region Operaciones dentro de una transacción existente

        public static DataTable Consultar(SqlConnection cn, SqlTransaction tx, string sql, params SqlParameter[] parametros)
        {
            return LeerTabla(cn, tx, sql, CommandType.Text, parametros);
        }

        public static int EjecutarComando(SqlConnection cn, SqlTransaction tx, string sql, params SqlParameter[] parametros)
        {
            return EjecutarNoQuery(cn, tx, sql, CommandType.Text, parametros);
        }

        public static int EjecutarSP(SqlConnection cn, SqlTransaction tx, string procedimiento, params SqlParameter[] parametros)
        {
            return EjecutarNoQuery(cn, tx, procedimiento, CommandType.StoredProcedure, parametros);
        }

        public static object Escalar(SqlConnection cn, SqlTransaction tx, string sql, params SqlParameter[] parametros)
        {
            try
            {
                using (SqlCommand cmd = CrearComando(cn, tx, sql, CommandType.Text, parametros))
                {
                    object resultado = cmd.ExecuteScalar();
                    return resultado == DBNull.Value ? null : resultado;
                }
            }
            catch (SqlException ex)
            {
                throw Traducir(ex);
            }
        }

        #endregion

        #region Transacciones

        /// <summary>
        /// Ejecuta el trabajo dentro de una SqlTransaction: confirma si termina bien y revierte ante cualquier excepción.
        /// </summary>
        public static T EjecutarEnTransaccion<T>(Func<SqlConnection, SqlTransaction, T> trabajo)
        {
            using (SqlConnection cn = ConexionBD.CrearConexion())
            {
                AbrirConexion(cn);
                using (SqlTransaction tx = cn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        T resultado = trabajo(cn, tx);
                        tx.Commit();
                        return resultado;
                    }
                    catch
                    {
                        RevertirSeguro(tx);
                        throw;
                    }
                }
            }
        }

        public static void EjecutarEnTransaccion(Action<SqlConnection, SqlTransaction> trabajo)
        {
            EjecutarEnTransaccion<bool>(delegate(SqlConnection cn, SqlTransaction tx)
            {
                trabajo(cn, tx);
                return true;
            });
        }

        #endregion

        #region Internos

        private static T Ejecutar<T>(Func<SqlConnection, T> operacion)
        {
            using (SqlConnection cn = ConexionBD.CrearConexion())
            {
                AbrirConexion(cn);
                return operacion(cn);
            }
        }

        private static void AbrirConexion(SqlConnection cn)
        {
            try
            {
                cn.Open();
            }
            catch (SqlException ex)
            {
                throw Traducir(ex);
            }
        }

        private static DataTable LeerTabla(SqlConnection cn, SqlTransaction tx, string comando, CommandType tipo, SqlParameter[] parametros)
        {
            try
            {
                using (SqlCommand cmd = CrearComando(cn, tx, comando, tipo, parametros))
                using (SqlDataAdapter adaptador = new SqlDataAdapter(cmd))
                {
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    return tabla;
                }
            }
            catch (SqlException ex)
            {
                throw Traducir(ex);
            }
        }

        private static int EjecutarNoQuery(SqlConnection cn, SqlTransaction tx, string comando, CommandType tipo, SqlParameter[] parametros)
        {
            try
            {
                using (SqlCommand cmd = CrearComando(cn, tx, comando, tipo, parametros))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw Traducir(ex);
            }
        }

        private static SqlCommand CrearComando(SqlConnection cn, SqlTransaction tx, string comando, CommandType tipo, SqlParameter[] parametros)
        {
            SqlCommand cmd = new SqlCommand(comando, cn, tx);
            cmd.CommandType = tipo;
            cmd.CommandTimeout = 120;
            if (parametros != null)
            {
                foreach (SqlParameter p in parametros)
                {
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }

        private static void RevertirSeguro(SqlTransaction tx)
        {
            try
            {
                if (tx.Connection != null)
                {
                    tx.Rollback();
                }
            }
            catch (InvalidOperationException)
            {
                // La transacción ya fue revertida por el servidor (por ejemplo, XACT_ABORT).
            }
            catch (SqlException)
            {
                // Se conserva la excepción original que provocó la reversión.
            }
        }

        private static ExcepcionDatos Traducir(SqlException ex)
        {
            int numero = ex.Number;
            string mensaje;
            bool esNegocio = false;

            if (numero >= 50000)
            {
                // SqlException.Message concatena todos los errores; el mensaje del RAISERROR es el primero.
                mensaje = ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message;
                esNegocio = true;
            }
            else if (numero == 2627 || numero == 2601)
            {
                mensaje = MensajeRestriccion(ex.Message,
                    "Ya existe un registro con los mismos datos únicos (clave, RFC, CURP, NSS o número).");
                esNegocio = true;
            }
            else if (numero == 547)
            {
                if (ex.Message.Contains("FK_"))
                {
                    mensaje = "El registro está relacionado con otros datos y no puede modificarse o eliminarse, " +
                              "o hace referencia a un dato que no existe.";
                }
                else
                {
                    mensaje = "Algún valor no cumple las reglas de validación de la base de datos" +
                              ExtraerRestriccion(ex.Message) + ".";
                }
                esNegocio = true;
            }
            else if (numero == 2628 || numero == 8152)
            {
                mensaje = "Alguno de los textos excede la longitud permitida.";
                esNegocio = true;
            }
            else if (numero == 515)
            {
                mensaje = "Falta capturar un dato obligatorio.";
                esNegocio = true;
            }
            else if (numero == 1205)
            {
                mensaje = "La base de datos está ocupada con otra operación. Intente nuevamente.";
            }
            else if (numero == -2)
            {
                mensaje = "Se agotó el tiempo de espera de la base de datos.";
            }
            else if (numero == 2 || numero == 53 || numero == 233 || numero == 4060 || numero == 18456 ||
                     numero == 10060 || numero == 40 || numero == 10061)
            {
                mensaje = "No fue posible conectar con el servidor de base de datos. Verifique la conexión y las credenciales. (" +
                          ex.Message + ")";
            }
            else
            {
                mensaje = "Error de base de datos (" + numero + "): " + ex.Message;
            }

            return new ExcepcionDatos(mensaje, numero, esNegocio, ex);
        }

        private static string MensajeRestriccion(string mensajeSql, string predeterminado)
        {
            foreach (KeyValuePair<string, string> par in MensajesPorRestriccion)
            {
                if (mensajeSql.Contains(par.Key))
                {
                    return par.Value;
                }
            }
            return predeterminado;
        }

        private static string ExtraerRestriccion(string mensajeSql)
        {
            Match m = Regex.Match(mensajeSql, "(CK_[A-Za-z0-9_]+)");
            return m.Success ? " (" + m.Groups[1].Value + ")" : string.Empty;
        }

        #endregion
    }
}
