using System;
using System.Collections.Generic;
using System.Data;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>
    /// Ejecuta los procesos críticos de nómina. Los procedimientos sp_CalcularNomina, sp_AutorizarPeriodo y
    /// sp_CerrarPeriodo manejan su propia transacción en el servidor, por eso aquí se invocan sin una transacción
    /// externa (un ROLLBACK interno revertiría también la transacción del cliente).
    /// </summary>
    public class NominaDAL
    {
        public void CalcularNomina(int periodoId)
        {
            AccesoDatos.EjecutarSP("dbo.sp_CalcularNomina", AccesoDatos.Parametro("@PeriodoId", periodoId));
        }

        public void AutorizarPeriodo(int periodoId, int? usuarioId, string comentario)
        {
            AccesoDatos.EjecutarSP("dbo.sp_AutorizarPeriodo",
                AccesoDatos.Parametro("@PeriodoId", periodoId),
                AccesoDatos.ParametroEntero("@UsuarioId", usuarioId),
                AccesoDatos.Parametro("@Comentario", comentario));
        }

        public void CerrarPeriodo(int periodoId)
        {
            AccesoDatos.EjecutarSP("dbo.sp_CerrarPeriodo", AccesoDatos.Parametro("@PeriodoId", periodoId));
        }

        public void ReversarCalculo(int periodoId, int? usuarioId, string motivo)
        {
            AccesoDatos.EjecutarSP("dbo.sp_ReversarCalculo",
                AccesoDatos.Parametro("@PeriodoId", periodoId),
                AccesoDatos.ParametroEntero("@UsuarioId", usuarioId),
                AccesoDatos.Parametro("@Motivo", motivo));
        }

        /// <summary>Empleados activos con datos IMSS y contrato vigente de la periodicidad (los que calculará la nómina).</summary>
        public int ContarEmpleadosElegibles(int periodoId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.ProcPeriodoNomina pn " +
                "INNER JOIN dbo.CatEmpleado e ON e.EmpresaId = pn.EmpresaId AND e.Estatus = 'ACTIVO' AND e.FechaIngreso <= pn.FechaFin " +
                "INNER JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId " +
                "INNER JOIN dbo.DocContratoEmpleado c ON c.EmpleadoId = e.EmpleadoId AND c.Activo = 1 AND c.PeriodicidadPagoId = pn.PeriodicidadPagoId " +
                "WHERE pn.PeriodoId = @Id",
                AccesoDatos.Parametro("@Id", periodoId));
            return Convert.ToInt32(r);
        }

        /// <summary>Totales de incidencias activas por empleado, con el mismo criterio que usa sp_CalcularNomina.</summary>
        public Dictionary<int, IncidenciasEmpleado> ObtenerIncidenciasPorEmpleado(int periodoId)
        {
            return ObtenerIncidenciasPorEmpleado(periodoId, null);
        }

        /// <summary>Totales de incidencias activas del periodo; si se indica el empleado solo se consulta ese trabajador.</summary>
        public Dictionary<int, IncidenciasEmpleado> ObtenerIncidenciasPorEmpleado(int periodoId, int? empleadoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT i.IdEmpleado AS EmpleadoId, COUNT(1) AS TotalActivas, " +
                "ISNULL(SUM(CASE WHEN c.DisminuyeDias = 1 AND c.FormaCalculo = 'DIAS' THEN i.CantidadOValor ELSE 0 END), 0) AS DiasDescontados, " +
                "ISNULL(SUM(CASE WHEN c.ClaveSAT = '019' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0) AS HorasExtraDobles, " +
                "CAST(0 AS DECIMAL(18,4)) AS HorasExtraTriples, " +
                "CAST(0 AS DECIMAL(18,4)) AS DiasFestivos, " +
                "ISNULL(SUM(CASE WHEN c.ClaveSAT = '020' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0) AS DiasPrimaDominical, " +
                "ISNULL(SUM(CASE WHEN c.ClaveSAT = '002' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0) AS DiasAguinaldo, " +
                "ISNULL(SUM(CASE WHEN c.ClaveSAT = '021' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0) AS DiasPrimaVac, " +
                "ISNULL(SUM(CASE WHEN c.ClaveSAT = '022' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0) AS DiasAntiguedad, " +
                "ISNULL(SUM(CASE WHEN c.ClaveSAT = '025' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0) AS DiasIndemniz, " +
                "CAST(0 AS DECIMAL(18,4)) AS Bonos, " +
                "CAST(0 AS DECIMAL(18,4)) AS Descuentos " +
                "FROM dbo.ProcIncidencias i INNER JOIN dbo.CatConcepto c ON c.ConceptoId = i.IdConceptoSAT " +
                "AND EXISTS (SELECT 1 FROM dbo.RelConceptoFormula rf WHERE rf.ConceptoId = c.ConceptoId AND rf.Activa = 1) " +
                "WHERE i.IdPeriodo = @Id AND i.Estatus = 'ACTIVA' AND (@Empleado IS NULL OR i.IdEmpleado = @Empleado) GROUP BY i.IdEmpleado",
                AccesoDatos.Parametro("@Id", periodoId),
                AccesoDatos.ParametroEntero("@Empleado", empleadoId));

            Dictionary<int, IncidenciasEmpleado> resultado = new Dictionary<int, IncidenciasEmpleado>();
            foreach (DataRow fila in t.Rows)
            {
                int id = Fila.Obtener<int>(fila, "EmpleadoId");
                resultado[id] = new IncidenciasEmpleado
                {
                    EmpleadoId = id,
                    TotalActivas = Fila.Obtener<int>(fila, "TotalActivas"),
                    DiasDescontados = Fila.Obtener<decimal>(fila, "DiasDescontados"),
                    HorasExtraDobles = Fila.Obtener<decimal>(fila, "HorasExtraDobles"),
                    HorasExtraTriples = Fila.Obtener<decimal>(fila, "HorasExtraTriples"),
                    DiasFestivosLaborados = Fila.Obtener<decimal>(fila, "DiasFestivos"),
                    DiasPrimaDominical = Fila.Obtener<decimal>(fila, "DiasPrimaDominical"),
                    DiasAguinaldo = Fila.Obtener<decimal>(fila, "DiasAguinaldo"),
                    DiasPrimaVacacional = Fila.Obtener<decimal>(fila, "DiasPrimaVac"),
                    DiasAntiguedad = Fila.Obtener<decimal>(fila, "DiasAntiguedad"),
                    DiasIndemnizacion = Fila.Obtener<decimal>(fila, "DiasIndemniz"),
                    Bonos = Fila.Obtener<decimal>(fila, "Bonos"),
                    Descuentos = Fila.Obtener<decimal>(fila, "Descuentos")
                };
            }

            // Reparto legal de horas extra (LFT Art. 66-68): las primeras 9 h/semana (prorrateadas al periodo) van al
            // doble; el excedente al triple. Todos los trabajadores del periodo comparten la empresa y su política.
            if (resultado.Count > 0)
            {
                DataTable pt = AccesoDatos.Consultar(
                    "SELECT DATEDIFF(DAY, p.FechaInicio, p.FechaFin) + 1 AS Dias, " +
                    "ISNULL((SELECT LimiteHorasExtraSinGravar FROM dbo.CatPoliticaEmpresarial WHERE PoliticaId = dbo.fn_ObtenerPoliticaActiva(p.EmpresaId, p.FechaFin)), 9) AS Limite " +
                    "FROM dbo.ProcPeriodoNomina p WHERE p.PeriodoId = @Id",
                    AccesoDatos.Parametro("@Id", periodoId));
                if (pt.Rows.Count == 1)
                {
                    decimal limiteHEDoble = Math.Round(Fila.Obtener<decimal>(pt.Rows[0], "Limite") * (Fila.Obtener<int>(pt.Rows[0], "Dias") / 7m), 2, MidpointRounding.AwayFromZero);
                    foreach (IncidenciasEmpleado inc in resultado.Values)
                    {
                        decimal total = inc.HorasExtraDobles;
                        inc.HorasExtraTriples = total > limiteHEDoble ? total - limiteHEDoble : 0m;
                        inc.HorasExtraDobles = total > limiteHEDoble ? limiteHEDoble : total;
                    }
                }
            }
            return resultado;
        }
        public Dictionary<int, int> ObtenerDiasSemanaPorEmpleado(int periodoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT r.EmpleadoId, dbo.fn_ObtenerDiasSemanaEmpleado(r.EmpleadoId, r.PeriodoId) AS DiasSemana " +
                "FROM dbo.DocReciboNomina r WHERE r.PeriodoId = @Id",
                AccesoDatos.Parametro("@Id", periodoId));

            Dictionary<int, int> resultado = new Dictionary<int, int>();
            foreach (DataRow fila in t.Rows)
            {
                int? dias = Fila.Obtener<int?>(fila, "DiasSemana");
                if (dias.HasValue)
                {
                    resultado[Fila.Obtener<int>(fila, "EmpleadoId")] = dias.Value;
                }
            }
            return resultado;
        }

        /// <summary>% de prima dominical resuelto (override empresa/periodo/empleado) para un trabajador; NULL si no hay parámetro.</summary>
        public decimal? ResolverPorcentajePrimaDominical(int empresaId, int periodoId, int empleadoId)
        {
            object v = AccesoDatos.Escalar(
                "SELECT dbo.fn_ResolverParametro('PORCENTAJE_PRIMA_DOMINICAL', @Empresa, @Periodo, @Empleado)",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Periodo", periodoId),
                AccesoDatos.Parametro("@Empleado", empleadoId));
            return (v == null || v == System.DBNull.Value) ? (decimal?)null : System.Convert.ToDecimal(v);
        }

        /// <summary>Factor de pago por festivo laborado y porcentaje de prima dominical de la política vigente (valores de ley si no hay).</summary>
        public void ObtenerParametrosPago(int empresaId, DateTime fecha, out decimal factorFestivo, out decimal porcentajePrimaDominical)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT FactorDiaFestivoLaborado, PorcentajePrimaDominical FROM dbo.CatPoliticaEmpresarial " +
                "WHERE PoliticaId = dbo.fn_ObtenerPoliticaActiva(@Empresa, @Fecha)",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Fecha", fecha.Date));

            factorFestivo = 2m;
            porcentajePrimaDominical = 25m;
            if (t.Rows.Count > 0)
            {
                factorFestivo = Fila.Obtener<decimal>(t.Rows[0], "FactorDiaFestivoLaborado");
                porcentajePrimaDominical = Fila.Obtener<decimal>(t.Rows[0], "PorcentajePrimaDominical");
            }
        }

        /// <summary>Cantidad de renglones cargados de ISR, subsidio e IMSS para el ejercicio del periodo.</summary>
        public void ContarTablasFiscales(int periodoId, out int renglonesISR, out int renglonesSubsidio, out int conceptosIMSS)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT (SELECT COUNT(1) FROM                 dbo.CatTablaISR x WHERE x.EjercicioId = pn.EjercicioId) AS ISR, " +
                                "       (SELECT COUNT(1) FROM dbo.CatTablaSubsidioEmpleo x WHERE x.EjercicioId = pn.EjercicioId) AS Subsidio, " +
                                "       (SELECT COUNT(1) FROM dbo.CatCuotaIMSS x WHERE x.EjercicioId = pn.EjercicioId) AS IMSS " +
                "FROM dbo.ProcPeriodoNomina pn WHERE pn.PeriodoId = @Id",
                AccesoDatos.Parametro("@Id", periodoId));

            renglonesISR = 0;
            renglonesSubsidio = 0;
            conceptosIMSS = 0;
            if (t.Rows.Count > 0)
            {
                renglonesISR = Fila.Obtener<int>(t.Rows[0], "ISR");
                renglonesSubsidio = Fila.Obtener<int>(t.Rows[0], "Subsidio");
                conceptosIMSS = Fila.Obtener<int>(t.Rows[0], "IMSS");
            }
        }

        /// <summary>Incidencias activas del periodo cuyo empleado ya no está activo (no se calcularán).</summary>
        public int ContarIncidenciasSinEmpleadoActivo(int periodoId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.ProcIncidencias i INNER JOIN dbo.CatEmpleado e ON e.EmpleadoId = i.IdEmpleado " +
                "INNER JOIN dbo.CatConcepto c ON c.ConceptoId = i.IdConceptoSAT AND c.TipoConcepto = 'I' " +
                "WHERE i.IdPeriodo = @Id AND i.Estatus = 'ACTIVA' AND e.Estatus <> 'ACTIVO'",
                AccesoDatos.Parametro("@Id", periodoId));
            return Convert.ToInt32(r);
        }

        public List<ReciboListado> ListarRecibos(int periodoId)
        {
            DataTable tabla = AccesoDatos.Consultar(
                "SELECT r.ReciboId, r.PeriodoId, r.EmpleadoId, e.NumeroEmpleado, " +
                "CONCAT(e.ApellidoPaterno, ' ', ISNULL(e.ApellidoMaterno + ' ', ''), e.Nombres) AS NombreEmpleado, " +
                "r.SnapshotPuesto AS Puesto, r.SnapshotDepartamento AS Departamento, ISNULL(r.SnapshotDiasPagados, 0) AS Dias, " +
                "r.SnapshotSalarioDiario AS SalarioDiario, " +
                "r.TotalPercepciones, r.TotalDeducciones, r.NetoPagar, r.Estatus, r.FechaCalculo " +
                "FROM dbo.DocReciboNomina r INNER JOIN dbo.CatEmpleado e ON e.EmpleadoId = r.EmpleadoId " +
                "WHERE r.PeriodoId = @Id AND r.Estatus <> 'PRERECIBO' ORDER BY e.ApellidoPaterno, e.ApellidoMaterno, e.Nombres",
                AccesoDatos.Parametro("@Id", periodoId));

            List<ReciboListado> lista = new List<ReciboListado>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new ReciboListado
                {
                    ReciboId = Fila.Obtener<int>(fila, "ReciboId"),
                    PeriodoId = Fila.Obtener<int>(fila, "PeriodoId"),
                    EmpleadoId = Fila.Obtener<int>(fila, "EmpleadoId"),
                    NumeroEmpleado = Fila.Obtener<string>(fila, "NumeroEmpleado"),
                    NombreEmpleado = Fila.Obtener<string>(fila, "NombreEmpleado"),
                    Puesto = Fila.Obtener<string>(fila, "Puesto"),
                    Departamento = Fila.Obtener<string>(fila, "Departamento"),
                    SnapshotDiasPagados = Fila.Obtener<decimal>(fila, "Dias"),
                    SnapshotSalarioDiario = Fila.Obtener<decimal?>(fila, "SalarioDiario"),
                    TotalPercepciones = Fila.Obtener<decimal>(fila, "TotalPercepciones"),
                    TotalDeducciones = Fila.Obtener<decimal>(fila, "TotalDeducciones"),
                    NetoPagar = Fila.Obtener<decimal>(fila, "NetoPagar"),
                    Estatus = Fila.Obtener<string>(fila, "Estatus"),
                    FechaCalculo = Fila.Obtener<DateTime>(fila, "FechaCalculo")
                });
            }
            return lista;
        }
    }
}
