using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Procesos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>Acceso a dbo.ProcPeriodoNomina, dbo.CatEjercicioFiscal y los festivos ligados a cada periodo (dbo.RelPeriodoDiaFestivo).</summary>
    public class PeriodoDAL
    {
        private const string SelectResumen =
            "SELECT pn.PeriodoId, pn.EmpresaId, emp.RazonSocial AS Empresa, pn.EjercicioId, ef.Anio, pn.PeriodicidadPagoId, " +
            "tp.Clave AS PeriodicidadClave, tp.Descripcion AS TipoPeriodo, " +
            "pn.NumeroPeriodo, pn.FechaInicio, pn.FechaFin, pn.FechaPago, pn.DiasPeriodo, pn.EsInicioMes, pn.EsFinMes, " +
            "pn.DiaDescanso, pn.CalculaSeptimoDia, pn.Estatus, " +
            "tot.TotalEmpleados, tot.SumaPercepciones, tot.SumaDeducciones, tot.SumaNeto, " +
            "(SELECT COUNT(1) FROM dbo.RelPeriodoDiaFestivo rf WHERE rf.PeriodoId = pn.PeriodoId) AS TotalFestivos " +
            "FROM dbo.ProcPeriodoNomina pn " +
            "INNER JOIN dbo.CatEmpresa emp ON emp.EmpresaId = pn.EmpresaId " +
            "INNER JOIN dbo.CatEjercicioFiscal ef ON ef.EjercicioId = pn.EjercicioId " +
            "INNER JOIN dbo.CatPeriodicidadPago tp ON tp.PeriodicidadPagoId = pn.PeriodicidadPagoId " +
            "OUTER APPLY (SELECT COUNT(*) AS TotalEmpleados, ISNULL(SUM(r.TotalPercepciones), 0) AS SumaPercepciones, " +
            "             ISNULL(SUM(r.TotalDeducciones), 0) AS SumaDeducciones, ISNULL(SUM(r.NetoPagar), 0) AS SumaNeto " +
            "             FROM dbo.DocReciboNomina r WHERE r.PeriodoId = pn.PeriodoId AND r.Estatus <> 'PRERECIBO') tot ";

        private const string EstatusAbiertos = "pn.Estatus IN ('CREADO','PRENOMINA') ";

        public List<PeriodoResumen> ListarPorEmpresa(int empresaId)
        {
            DataTable tabla = AccesoDatos.Consultar(
                SelectResumen + "WHERE pn.EmpresaId = @Empresa ORDER BY pn.FechaInicio DESC, pn.NumeroPeriodo DESC",
                AccesoDatos.Parametro("@Empresa", empresaId));
            return Mapear(tabla);
        }

        /// <summary>Periodos de un ejercicio fiscal para una empresa y periodicidad, con sus días festivos.</summary>
        public List<PeriodoResumen> ListarPorEmpresaTipo(int empresaId, int tipoPeriodoId, int anio)
        {
            DataTable tabla = AccesoDatos.Consultar(
                SelectResumen + "WHERE pn.EmpresaId = @Empresa AND pn.PeriodicidadPagoId = @Tipo AND ef.Anio = @Anio ORDER BY pn.NumeroPeriodo",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Tipo", tipoPeriodoId),
                AccesoDatos.Parametro("@Anio", anio));
            List<PeriodoResumen> lista = Mapear(tabla);

            DataTable festivos = AccesoDatos.Consultar(
                "SELECT rf.PeriodoId, f.DiaFestivoId, f.EmpresaId, f.Fecha, f.Anio, f.Descripcion, f.Tipo, f.Activo " +
                "FROM dbo.RelPeriodoDiaFestivo rf " +
                "INNER JOIN dbo.CatDiaFestivo f ON f.DiaFestivoId = rf.DiaFestivoId " +
                "INNER JOIN dbo.ProcPeriodoNomina pn ON pn.PeriodoId = rf.PeriodoId " +
                "                INNER JOIN dbo.CatEjercicioFiscal ef ON ef.EjercicioId = pn.EjercicioId " +
                "WHERE pn.EmpresaId = @Empresa AND pn.PeriodicidadPagoId = @Tipo AND ef.Anio = @Anio ORDER BY f.Fecha",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Tipo", tipoPeriodoId),
                AccesoDatos.Parametro("@Anio", anio));
            AsignarFestivos(lista, festivos);
            return lista;
        }

        public PeriodoResumen ObtenerPorId(int periodoId)
        {
            DataTable tabla = AccesoDatos.Consultar(SelectResumen + "WHERE pn.PeriodoId = @Id",
                AccesoDatos.Parametro("@Id", periodoId));
            if (tabla.Rows.Count == 0)
            {
                return null;
            }

            List<PeriodoResumen> lista = Mapear(tabla);
            DataTable festivos = AccesoDatos.Consultar(
                "SELECT rf.PeriodoId, f.DiaFestivoId, f.EmpresaId, f.Fecha, f.Anio, f.Descripcion, f.Tipo, f.Activo " +
                "FROM dbo.RelPeriodoDiaFestivo rf INNER JOIN dbo.CatDiaFestivo f ON f.DiaFestivoId = rf.DiaFestivoId " +
                "WHERE rf.PeriodoId = @Id ORDER BY f.Fecha",
                AccesoDatos.Parametro("@Id", periodoId));
            AsignarFestivos(lista, festivos);
            return lista[0];
        }

        /// <summary>Periodo cuyo rango de fechas contiene la fecha indicada (cualquier estatus).</summary>
        public int? ObtenerIdVigente(int empresaId, int tipoPeriodoId, DateTime fecha)
        {
            return IdPeriodo(
                "SELECT TOP 1 pn.PeriodoId FROM dbo.ProcPeriodoNomina pn " +
                "WHERE pn.EmpresaId = @Empresa AND pn.PeriodicidadPagoId = @Tipo AND @Fecha BETWEEN pn.FechaInicio AND pn.FechaFin " +
                "ORDER BY pn.FechaInicio DESC",
                empresaId, tipoPeriodoId, fecha);
        }

        /// <summary>Primer periodo abierto que inicia después de la fecha.</summary>
        public int? ObtenerIdSiguienteAbierto(int empresaId, int tipoPeriodoId, DateTime fecha)
        {
            return IdPeriodo(
                "SELECT TOP 1 pn.PeriodoId FROM dbo.ProcPeriodoNomina pn " +
                "WHERE pn.EmpresaId = @Empresa AND pn.PeriodicidadPagoId = @Tipo AND " + EstatusAbiertos + "AND pn.FechaInicio > @Fecha " +
                "ORDER BY pn.FechaInicio",
                empresaId, tipoPeriodoId, fecha);
        }

        /// <summary>Último periodo abierto (sin procesar) que terminó antes de la fecha.</summary>
        public int? ObtenerIdUltimoAbiertoAnterior(int empresaId, int tipoPeriodoId, DateTime fecha)
        {
            return IdPeriodo(
                "SELECT TOP 1 pn.PeriodoId FROM dbo.ProcPeriodoNomina pn " +
                "WHERE pn.EmpresaId = @Empresa AND pn.PeriodicidadPagoId = @Tipo AND " + EstatusAbiertos + "AND pn.FechaFin < @Fecha " +
                "ORDER BY pn.FechaFin DESC",
                empresaId, tipoPeriodoId, fecha);
        }

        /// <summary>
        /// Periodos abiertos anteriores a la fecha que quedaron sin procesar: los que preceden a un periodo ya procesado
        /// (en el primer uso del sistema, cuando aún no hay ninguno procesado, no se consideran pendientes).
        /// </summary>
        public int ContarAbiertosAnteriores(int empresaId, int tipoPeriodoId, DateTime fecha)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.ProcPeriodoNomina pn " +
                "WHERE pn.EmpresaId = @Empresa AND pn.PeriodicidadPagoId = @Tipo AND " + EstatusAbiertos + "AND pn.FechaFin < @Fecha " +
                "AND EXISTS (SELECT 1 FROM dbo.ProcPeriodoNomina x WHERE x.EmpresaId = pn.EmpresaId AND x.PeriodicidadPagoId = pn.PeriodicidadPagoId " +
                "            AND x.Estatus NOT IN ('CREADO','PRENOMINA','CANCELADO') AND x.FechaFin > pn.FechaFin)",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Tipo", tipoPeriodoId),
                AccesoDatos.Parametro("@Fecha", fecha.Date));
            return Convert.ToInt32(r);
        }

        /// <summary>Fecha final del último periodo del ejercicio anterior; sirve de ancla para periodicidades catorcenales.</summary>
        public DateTime? ObtenerUltimaFechaFinEjercicio(int empresaId, int tipoPeriodoId, int anio)
        {
            object r = AccesoDatos.Escalar(
                "SELECT MAX(pn.FechaFin) FROM dbo.ProcPeriodoNomina pn " +
                "                INNER JOIN dbo.CatEjercicioFiscal ef ON ef.EjercicioId = pn.EjercicioId " +
                "WHERE pn.EmpresaId = @Empresa AND pn.PeriodicidadPagoId = @Tipo AND ef.Anio = @Anio",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Tipo", tipoPeriodoId),
                AccesoDatos.Parametro("@Anio", anio));
            return r == null ? (DateTime?)null : Convert.ToDateTime(r);
        }

        /// <summary>Pasa el periodo de CREADO a PRENOMINA cuando comienza la captura de incidencias.</summary>
        public void MarcarPrenomina(int periodoId)
        {
            AccesoDatos.EjecutarComando(
                "UPDATE dbo.ProcPeriodoNomina SET Estatus = 'PRENOMINA' WHERE PeriodoId = @Id AND Estatus = 'CREADO'",
                AccesoDatos.Parametro("@Id", periodoId));
        }

        /// <summary>
        /// Guarda el calendario anual generado en una sola transacción: crea el ejercicio si no existe, inserta los periodos
        /// nuevos, actualiza los que siguen abiertos y vincula los días festivos. Los periodos ya calculados no se modifican.
        /// </summary>
        public ResultadoGeneracionPeriodos GuardarCalendario(int empresaId, int tipoPeriodoId, int anio, IList<ProcPeriodoNomina> periodos)
        {
            return AccesoDatos.EjecutarEnTransaccion<ResultadoGeneracionPeriodos>(delegate(SqlConnection cn, SqlTransaction tx)
            {
                DataTable ejercicio = AccesoDatos.Consultar(cn, tx,
                    "SELECT EjercicioId, Estatus FROM dbo.CatEjercicioFiscal WITH (UPDLOCK, HOLDLOCK) WHERE Anio = @Anio",
                    AccesoDatos.Parametro("@Anio", anio));

                int ejercicioId;
                if (ejercicio.Rows.Count == 0)
                {
                    ejercicioId = Convert.ToInt32(AccesoDatos.Escalar(cn, tx,
                        "INSERT INTO dbo.CatEjercicioFiscal (Anio, Estatus) VALUES (@Anio, 'ABIERTO'); SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        AccesoDatos.Parametro("@Anio", anio)));
                }
                else
                {
                    if (Fila.Obtener<string>(ejercicio.Rows[0], "Estatus") != "ABIERTO")
                    {
                        throw new ExcepcionDatos("El ejercicio fiscal " + anio + " está cerrado.", 0, true, null);
                    }
                    ejercicioId = Fila.Obtener<int>(ejercicio.Rows[0], "EjercicioId");
                }

                ResultadoGeneracionPeriodos resultado = new ResultadoGeneracionPeriodos { Anio = anio, TotalPeriodos = periodos.Count };
                foreach (ProcPeriodoNomina p in periodos)
                {
                    DataTable existente = AccesoDatos.Consultar(cn, tx,
                        "SELECT PeriodoId, Estatus FROM dbo.ProcPeriodoNomina WITH (UPDLOCK, HOLDLOCK) " +
                        "WHERE EmpresaId = @Empresa AND EjercicioId = @Ejercicio AND PeriodicidadPagoId = @Tipo AND NumeroPeriodo = @Numero",
                        AccesoDatos.Parametro("@Empresa", empresaId),
                        AccesoDatos.Parametro("@Ejercicio", ejercicioId),
                        AccesoDatos.Parametro("@Tipo", tipoPeriodoId),
                        AccesoDatos.Parametro("@Numero", p.NumeroPeriodo));

                    if (existente.Rows.Count == 0)
                    {
                        AccesoDatos.EjecutarComando(cn, tx,
                            "INSERT INTO dbo.ProcPeriodoNomina (EmpresaId, EjercicioId, PeriodicidadPagoId, NumeroPeriodo, " +
                            "FechaInicio, FechaFin, FechaPago, DiasPeriodo, EsInicioMes, EsFinMes, DiaDescanso, CalculaSeptimoDia, Estatus) " +
                            "VALUES (@Empresa, @Ejercicio, @Tipo, @Numero, @Inicio, @Fin, @Pago, @Dias, @IniciaMes, @FinMes, @Descanso, @Septimo, 'CREADO')",
                            ParametrosPeriodo(empresaId, ejercicioId, tipoPeriodoId, p));
                        resultado.Creados++;
                    }
                    else if (Fila.Obtener<string>(existente.Rows[0], "Estatus") == EstatusPeriodo.Creado ||
                             Fila.Obtener<string>(existente.Rows[0], "Estatus") == EstatusPeriodo.Prenomina)
                    {
                        AccesoDatos.EjecutarComando(cn, tx,
                            "UPDATE dbo.ProcPeriodoNomina SET FechaInicio = @Inicio, FechaFin = @Fin, FechaPago = @Pago, DiasPeriodo = @Dias, " +
                            "EsInicioMes = @IniciaMes, EsFinMes = @FinMes, DiaDescanso = @Descanso, CalculaSeptimoDia = @Septimo " +
                            "WHERE EmpresaId = @Empresa AND EjercicioId = @Ejercicio AND PeriodicidadPagoId = @Tipo AND NumeroPeriodo = @Numero",
                            ParametrosPeriodo(empresaId, ejercicioId, tipoPeriodoId, p));
                        resultado.Actualizados++;
                    }
                    else
                    {
                        resultado.Omitidos++;
                    }
                }

                AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_VincularFestivosPeriodos",
                    AccesoDatos.Parametro("@EmpresaId", empresaId),
                    AccesoDatos.Parametro("@EjercicioId", ejercicioId),
                    AccesoDatos.Parametro("@PeriodicidadPagoId", tipoPeriodoId));

                resultado.FestivosVinculados = Convert.ToInt32(AccesoDatos.Escalar(cn, tx,
                    "SELECT COUNT(1) FROM dbo.RelPeriodoDiaFestivo rf INNER JOIN dbo.ProcPeriodoNomina pn ON pn.PeriodoId = rf.PeriodoId " +
                    "WHERE pn.EmpresaId = @Empresa AND pn.EjercicioId = @Ejercicio AND pn.PeriodicidadPagoId = @Tipo",
                    AccesoDatos.Parametro("@Empresa", empresaId),
                    AccesoDatos.Parametro("@Ejercicio", ejercicioId),
                    AccesoDatos.Parametro("@Tipo", tipoPeriodoId)));
                return resultado;
            });
        }

        private static SqlParameter[] ParametrosPeriodo(int empresaId, int ejercicioId, int tipoPeriodoId, ProcPeriodoNomina p)
        {
            return new[]
            {
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Ejercicio", ejercicioId),
                AccesoDatos.Parametro("@Tipo", tipoPeriodoId),
                AccesoDatos.Parametro("@Numero", p.NumeroPeriodo),
                AccesoDatos.Parametro("@Inicio", p.FechaInicio.Date),
                AccesoDatos.Parametro("@Fin", p.FechaFin.Date),
                AccesoDatos.Parametro("@Pago", p.FechaPago.Date),
                AccesoDatos.ParametroDecimal("@Dias", p.DiasPeriodo, 5, 2),
                AccesoDatos.Parametro("@IniciaMes", p.EsInicioMes),
                AccesoDatos.Parametro("@FinMes", p.EsFinMes),
                AccesoDatos.Parametro("@Descanso", p.DiaDescanso.HasValue ? (object)p.DiaDescanso.Value : null),
                AccesoDatos.Parametro("@Septimo", p.CalculaSeptimoDia)
            };
        }

        private static int? IdPeriodo(string sql, int empresaId, int tipoPeriodoId, DateTime fecha)
        {
            object r = AccesoDatos.Escalar(sql,
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Tipo", tipoPeriodoId),
                AccesoDatos.Parametro("@Fecha", fecha.Date));
            return r == null ? (int?)null : Convert.ToInt32(r);
        }

        private static List<PeriodoResumen> Mapear(DataTable tabla)
        {
            List<PeriodoResumen> lista = new List<PeriodoResumen>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new PeriodoResumen
                {
                    PeriodoId = Fila.Obtener<int>(fila, "PeriodoId"),
                    EmpresaId = Fila.Obtener<int>(fila, "EmpresaId"),
                    Empresa = Fila.Obtener<string>(fila, "Empresa"),
                    EjercicioId = Fila.Obtener<int>(fila, "EjercicioId"),
                    Anio = Fila.Obtener<int>(fila, "Anio"),
                    PeriodicidadPagoId = Fila.Obtener<int>(fila, "PeriodicidadPagoId"),
                    PeriodicidadClave = Fila.Obtener<string>(fila, "PeriodicidadClave"),
                    TipoPeriodo = Fila.Obtener<string>(fila, "TipoPeriodo"),
                    NumeroPeriodo = Fila.Obtener<int>(fila, "NumeroPeriodo"),
                    FechaInicio = Fila.Obtener<DateTime>(fila, "FechaInicio"),
                    FechaFin = Fila.Obtener<DateTime>(fila, "FechaFin"),
                    FechaPago = Fila.Obtener<DateTime>(fila, "FechaPago"),
                    DiasPeriodo = Fila.Obtener<decimal>(fila, "DiasPeriodo"),
                    EsInicioMes = Fila.Obtener<bool>(fila, "EsInicioMes"),
                    EsFinMes = Fila.Obtener<bool>(fila, "EsFinMes"),
                    DiaDescanso = Fila.Obtener<byte?>(fila, "DiaDescanso"),
                    CalculaSeptimoDia = Fila.Obtener<bool>(fila, "CalculaSeptimoDia"),
                    Estatus = Fila.Obtener<string>(fila, "Estatus"),
                    TotalEmpleados = Fila.Obtener<int>(fila, "TotalEmpleados"),
                    SumaPercepciones = Fila.Obtener<decimal>(fila, "SumaPercepciones"),
                    SumaDeducciones = Fila.Obtener<decimal>(fila, "SumaDeducciones"),
                    SumaNetoPagar = Fila.Obtener<decimal>(fila, "SumaNeto"),
                    TotalFestivos = Fila.Obtener<int>(fila, "TotalFestivos")
                });
            }
            return lista;
        }

        private static void AsignarFestivos(List<PeriodoResumen> periodos, DataTable festivos)
        {
            Dictionary<int, PeriodoResumen> porId = new Dictionary<int, PeriodoResumen>();
            foreach (PeriodoResumen p in periodos)
            {
                porId[p.PeriodoId] = p;
            }

            foreach (DataRow fila in festivos.Rows)
            {
                PeriodoResumen periodo;
                if (porId.TryGetValue(Fila.Obtener<int>(fila, "PeriodoId"), out periodo))
                {
                    periodo.Festivos.Add(new CatDiaFestivo
                    {
                        DiaFestivoId = Fila.Obtener<int>(fila, "DiaFestivoId"),
                        EmpresaId = Fila.Obtener<int?>(fila, "EmpresaId"),
                        Fecha = Fila.Obtener<DateTime>(fila, "Fecha"),
                        Anio = Fila.Obtener<int>(fila, "Anio"),
                        Descripcion = Fila.Obtener<string>(fila, "Descripcion"),
                        Tipo = Fila.Obtener<string>(fila, "Tipo"),
                        Activo = Fila.Obtener<bool>(fila, "Activo")
                    });
                }
            }
        }
    }
}
