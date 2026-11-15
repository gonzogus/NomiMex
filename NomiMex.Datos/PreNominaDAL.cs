using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>
    /// Datos de entrada del cálculo de pre-recibos (tablas fiscales, política y trabajadores del periodo) y persistencia
    /// de los recibos: guardado del borrador de un trabajador y cierre transaccional del periodo.
    /// </summary>
    public class PreNominaDAL
    {
        private const string InsertarCabecera =
            "INSERT INTO dbo.DocReciboNomina (PeriodoId, EmpleadoId, TipoNomina, TotalPercepciones, TotalDeducciones, NetoPagar, " +
            "TotalGravado, TotalExento, AntiguedadSemanas, SnapshotFechaInicialPago, SnapshotFechaFinalPago, SnapshotFechaPago, " +
            "SnapshotDiasPagados, SnapshotTipoJornada, SnapshotPeriodicidadPago, SnapshotTipoRegimen, SnapshotTipoContrato, " +
            "SnapshotRegimenFiscalReceptor, SnapshotSalarioDiario, SnapshotSDI, SnapshotSBC, SnapshotPuesto, SnapshotDepartamento, " +
            "SnapshotTipoContratoDesc, Estatus, Notas) " +
            "SELECT pn.PeriodoId, e.EmpleadoId, 'O', @Percepciones, @Deducciones, @Neto, @Gravado, @Exento, " +
            "DATEDIFF(WEEK, e.FechaIngreso, pn.FechaFin), pn.FechaInicio, pn.FechaFin, pn.FechaPago, @Dias, " +
            "j.Clave, p.Clave, tr.Clave, tc.Clave, CAST(f.RegimenFiscalSAT AS NVARCHAR(3)), " +
            "@SD, @SDI, @SBC, pu.Descripcion, dep.Descripcion, tc.Descripcion, @Estatus, @Notas " +
            "FROM dbo.ProcPeriodoNomina pn " +
            "INNER JOIN dbo.CatEmpleado e ON e.EmpleadoId = @Empleado " +
            "OUTER APPLY (SELECT TOP 1 ec.TipoJornadaId, ec.PeriodicidadPagoId, ec.TipoRegimenId, ec.TipoContratoId, ec.PuestoId, ec.DepartamentoId " +
            "             FROM dbo.DocContratoEmpleado ec WHERE ec.EmpleadoId = e.EmpleadoId AND ec.Activo = 1 " +
            "             ORDER BY ec.FechaInicio DESC) c " +
            "LEFT JOIN dbo.CatTipoJornada j ON j.TipoJornadaId = c.TipoJornadaId " +
            "LEFT JOIN dbo.CatPeriodicidadPago p ON p.PeriodicidadPagoId = c.PeriodicidadPagoId " +
            "LEFT JOIN dbo.CatTipoRegimen tr ON tr.TipoRegimenId = c.TipoRegimenId " +
            "LEFT JOIN dbo.CatTipoContrato tc ON tc.TipoContratoId = c.TipoContratoId " +
            "LEFT JOIN dbo.CatPuesto pu ON pu.PuestoId = c.PuestoId " +
            "LEFT JOIN dbo.CatDepartamento dep ON dep.DepartamentoId = c.DepartamentoId " +
                        "LEFT JOIN dbo.CatEmpleadoDatosFiscales f ON f.EmpleadoId = e.EmpleadoId " +
            "WHERE pn.PeriodoId = @Periodo; " +
            "SELECT CAST(SCOPE_IDENTITY() AS INT);";

        #region Datos de entrada del cálculo

        /// <summary>Tablas fiscales del ejercicio, UMA, salario mínimo y política de pagos vigente a la fecha fin del periodo.</summary>
        public ParametrosCalculoNomina ObtenerParametros(int empresaId, int ejercicioId, int anio, DateTime fechaFin)
        {
            ParametrosCalculoNomina p = new ParametrosCalculoNomina();

            DataTable isr = AccesoDatos.Consultar(
                "SELECT Renglon, LimiteInferior, LimiteSuperior, CuotaFija, TasaExcedente FROM dbo.CatTablaISR " +
                "WHERE EjercicioId = @Ejercicio ORDER BY Renglon",
                AccesoDatos.Parametro("@Ejercicio", ejercicioId));
            foreach (DataRow f in isr.Rows)
            {
                p.TablaISR.Add(new TramoISR
                {
                    Renglon = Fila.Obtener<int>(f, "Renglon"),
                    LimiteInferior = Fila.Obtener<decimal>(f, "LimiteInferior"),
                    LimiteSuperior = Fila.Obtener<decimal>(f, "LimiteSuperior"),
                    CuotaFija = Fila.Obtener<decimal>(f, "CuotaFija"),
                    TasaExcedente = Fila.Obtener<decimal>(f, "TasaExcedente")
                });
            }

            DataTable subsidio = AccesoDatos.Consultar(
                "SELECT HastaIngreso, MontoSubsidio FROM dbo.CatTablaSubsidioEmpleo WHERE EjercicioId = @Ejercicio ORDER BY HastaIngreso",
                AccesoDatos.Parametro("@Ejercicio", ejercicioId));
            foreach (DataRow f in subsidio.Rows)
            {
                p.TablaSubsidio.Add(new TramoSubsidio
                {
                    HastaIngreso = Fila.Obtener<decimal>(f, "HastaIngreso"),
                    MontoSubsidio = Fila.Obtener<decimal>(f, "MontoSubsidio")
                });
            }

            DataTable imss = AccesoDatos.Consultar(
                "SELECT ClaveConcepto, PorcentajeObrero, BaseCalculo FROM dbo.CatCuotaIMSS WHERE EjercicioId = @Ejercicio",
                AccesoDatos.Parametro("@Ejercicio", ejercicioId));
            foreach (DataRow f in imss.Rows)
            {
                p.CuotasIMSS.Add(new CuotaIMSSObrera
                {
                    ClaveConcepto = Fila.Obtener<string>(f, "ClaveConcepto"),
                    PorcentajeObrero = Fila.Obtener<decimal>(f, "PorcentajeObrero"),
                    BaseCalculo = Fila.Obtener<string>(f, "BaseCalculo")
                });
            }

            DataTable valores = AccesoDatos.Consultar(
                "SELECT dbo.fn_ObtenerUMADiariaFecha(@FechaFin) AS UMA, dbo.fn_ObtenerSMGDiarioFecha(@FechaFin, 'GENERAL') AS SMG",
                AccesoDatos.Parametro("@FechaFin", fechaFin));
            p.UMADiaria = Fila.Obtener<decimal>(valores.Rows[0], "UMA");
            p.SalarioMinimoGeneral = Fila.Obtener<decimal>(valores.Rows[0], "SMG");

            // Jornada diaria estándar del ejercicio = jornada semanal de ley / 6 (8 h hasta 2026, 7.6667 h desde 2027).
            object jornadaSemanal = AccesoDatos.Escalar(
                "SELECT JornadaSemanalHoras FROM dbo.CatEjercicioFiscal WHERE EjercicioId = @Ejercicio",
                AccesoDatos.Parametro("@Ejercicio", ejercicioId));
            if (jornadaSemanal != null && jornadaSemanal != System.DBNull.Value)
            {
                decimal semanal = System.Convert.ToDecimal(jornadaSemanal);
                if (semanal > 0m) p.JornadaDiariaEstandar = System.Math.Round(semanal / 6m, 4, System.MidpointRounding.AwayFromZero);
            }

            DataTable politica = AccesoDatos.Consultar(
                "SELECT FactorHoraExtraDoble, FactorHoraExtraTriple, LimiteHorasExtraSinGravar, ExencionHorasExtraUMAsSemanal, " +
                "PorcentajePrimaDominical, ExencionPrimaDominicalUMAs, FactorDiaFestivoLaborado FROM dbo.CatPoliticaEmpresarial " +
                "WHERE PoliticaId = dbo.fn_ObtenerPoliticaActiva(@Empresa, @Fecha)",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Fecha", fechaFin.Date));
            if (politica.Rows.Count > 0)
            {
                DataRow f = politica.Rows[0];
                p.FactorHoraExtraDoble = Fila.Obtener<decimal>(f, "FactorHoraExtraDoble");
                p.FactorHoraExtraTriple = Fila.Obtener<decimal>(f, "FactorHoraExtraTriple");
                p.LimiteHorasExtraSinGravar = Fila.Obtener<decimal>(f, "LimiteHorasExtraSinGravar");
                p.ExencionHorasExtraUMAsSemanal = Fila.Obtener<decimal>(f, "ExencionHorasExtraUMAsSemanal");
                p.PorcentajePrimaDominical = Fila.Obtener<decimal>(f, "PorcentajePrimaDominical");
                p.ExencionPrimaDominicalUMAs = Fila.Obtener<decimal>(f, "ExencionPrimaDominicalUMAs");
                p.FactorDiaFestivoLaborado = Fila.Obtener<decimal>(f, "FactorDiaFestivoLaborado");
            }
            return p;
        }

        /// <summary>
        /// Trabajadores que calcula la nómina (mismo criterio que sp_CalcularNomina): activos de la empresa, con datos IMSS y
        /// contrato vigente de la periodicidad. Si se indica el empleado, solo se consulta ese trabajador.
        /// </summary>
        public List<DatosEmpleadoCalculo> ListarEmpleadosCalculo(int empresaId, int tipoPeriodoId, int periodoId, DateTime fechaFin, int? empleadoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT e.EmpleadoId, e.NumeroEmpleado, " +
                "CONCAT(e.ApellidoPaterno, ' ', ISNULL(e.ApellidoMaterno + ' ', ''), e.Nombres) AS NombreCompleto, e.FechaIngreso, " +
                "d.NSS, d.SalarioDiario, d.SalarioDiarioIntegrado AS SDI, d.SBC, f.RFC, f.CURP, " +
                ConsultasJornada.HorasJornada("e") + " AS HorasJornada, " +
                "COALESCE(ov.DiasSemana, ec.DiasLaboradosSemana, rj.DiasSemana) AS DiasSemana, " +
                "CASE WHEN ov.DiasSemana IS NULL THEN 0 ELSE 1 END AS DiasSemanaSoloPeriodo, " +
                "pu.Descripcion AS Puesto, dep.Descripcion AS Departamento, " +
                "inf.TipoDescuento, inf.MontoDescuento, ISNULL(pr.Descuento, 0) AS DescuentoPrestamos, " +
                "ISNULL(dbo.fn_ResolverParametro('PORCENTAJE_PRIMA_DOMINICAL', e.EmpresaId, @Periodo, e.EmpleadoId), 0) AS PctPrimaDom, " +
                "ISNULL(dbo.fn_ResolverParametro('FACTOR_HORAS_EXTRA_DOBLE', e.EmpresaId, @Periodo, e.EmpleadoId), 0) AS FactorHED, " +
                "ISNULL(dbo.fn_ResolverParametro('TOPE_UMA_PRIMA_DOMINICAL', e.EmpresaId, @Periodo, e.EmpleadoId), 0) AS TopePrimaDom, " +
                "ISNULL(dbo.fn_ResolverParametro('PORCENTAJE_PRIMA_VACACIONAL', e.EmpresaId, @Periodo, e.EmpleadoId), 0) AS PctPrimaVac, " +
                "ISNULL(dbo.fn_ResolverParametro('TOPE_UMA_AGUINALDO', e.EmpresaId, @Periodo, e.EmpleadoId), 0) AS TopeAgui, " +
                "ISNULL(dbo.fn_ResolverParametro('TOPE_UMA_PRIMA_VACACIONAL', e.EmpresaId, @Periodo, e.EmpleadoId), 0) AS TopePrimaVac " +
                "FROM dbo.CatEmpleado e " +
                "INNER JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId " +
                "CROSS APPLY (SELECT TOP 1 c.PuestoId, c.DepartamentoId, c.DiasLaboradosSemana, c.TipoJornadaId FROM dbo.DocContratoEmpleado c " +
                "             WHERE c.EmpleadoId = e.EmpleadoId AND c.Activo = 1 AND c.PeriodicidadPagoId = @Tipo " +
                "             ORDER BY c.FechaInicio DESC) ec " +
                "LEFT JOIN dbo.CatReglaTipoJornada rj ON rj.TipoJornadaId = ec.TipoJornadaId " +
                "LEFT JOIN dbo.ProcDiasSemanaPeriodo ov ON ov.PeriodoId = @Periodo AND ov.EmpleadoId = e.EmpleadoId " +
                                "LEFT JOIN dbo.CatEmpleadoDatosFiscales f ON f.EmpleadoId = e.EmpleadoId " +
                "LEFT JOIN dbo.CatPuesto pu ON pu.PuestoId = ec.PuestoId " +
                "LEFT JOIN dbo.CatDepartamento dep ON dep.DepartamentoId = ec.DepartamentoId " +
                "OUTER APPLY (SELECT TOP 1 TipoDescuento, MontoDescuento FROM dbo.ProcCreditoInfonavit " +
                "             WHERE EmpleadoId = e.EmpleadoId AND Activo = 1 AND Estatus = 'ACTIVO') inf " +
                "OUTER APPLY (SELECT SUM(CASE WHEN SaldoRestante >= DescuentoPorPeriodo THEN DescuentoPorPeriodo ELSE SaldoRestante END) AS Descuento " +
                "             FROM dbo.ProcPrestamo WHERE EmpleadoId = e.EmpleadoId AND Estatus = 'ACTIVO') pr " +
                "WHERE e.EmpresaId = @Empresa AND e.Estatus = 'ACTIVO' AND e.FechaIngreso <= @Fin " +
                "AND (@Empleado IS NULL OR e.EmpleadoId = @Empleado) " +
                "ORDER BY e.ApellidoPaterno, e.ApellidoMaterno, e.Nombres",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Tipo", tipoPeriodoId),
                AccesoDatos.Parametro("@Periodo", periodoId),
                AccesoDatos.Parametro("@Fin", fechaFin.Date),
                AccesoDatos.ParametroEntero("@Empleado", empleadoId));

            List<DatosEmpleadoCalculo> lista = new List<DatosEmpleadoCalculo>();
            foreach (DataRow f in t.Rows)
            {
                lista.Add(new DatosEmpleadoCalculo
                {
                    EmpleadoId = Fila.Obtener<int>(f, "EmpleadoId"),
                    NumeroEmpleado = Fila.Obtener<string>(f, "NumeroEmpleado"),
                    NombreCompleto = Fila.Obtener<string>(f, "NombreCompleto"),
                    FechaIngreso = Fila.Obtener<DateTime>(f, "FechaIngreso"),
                    NSS = Fila.Obtener<string>(f, "NSS"),
                    RFC = Fila.Obtener<string>(f, "RFC"),
                    CURP = Fila.Obtener<string>(f, "CURP"),
                    Puesto = Fila.Obtener<string>(f, "Puesto"),
                    Departamento = Fila.Obtener<string>(f, "Departamento"),
                    SalarioDiario = Fila.Obtener<decimal>(f, "SalarioDiario"),
                    SDI = Fila.Obtener<decimal>(f, "SDI"),
                    SBC = Fila.Obtener<decimal>(f, "SBC"),
                    HorasJornada = Fila.Obtener<decimal>(f, "HorasJornada"),
                    DiasLaboradosSemana = Fila.Obtener<int?>(f, "DiasSemana"),
                    DiasSemanaSoloPeriodo = Fila.Obtener<int>(f, "DiasSemanaSoloPeriodo") == 1,
                    TipoDescuentoInfonavit = Fila.Obtener<string>(f, "TipoDescuento"),
                    MontoDescuentoInfonavit = Fila.Obtener<decimal>(f, "MontoDescuento"),
                    DescuentoPrestamos = Fila.Obtener<decimal>(f, "DescuentoPrestamos"),
                    PorcentajePrimaDominical = Fila.Obtener<decimal>(f, "PctPrimaDom"),
                    FactorHoraExtraDoble = Fila.Obtener<decimal>(f, "FactorHED"),
                    ExencionPrimaDominicalUMAs = Fila.Obtener<decimal>(f, "TopePrimaDom"),
                    PorcentajePrimaVacacional = Fila.Obtener<decimal>(f, "PctPrimaVac"),
                    TopeUMAAguinaldo = Fila.Obtener<decimal>(f, "TopeAgui"),
                    TopeUMAPrimaVacacional = Fila.Obtener<decimal>(f, "TopePrimaVac")
                });
            }
            return lista;
        }

        /// <summary>Identidad (número, nombre, RFC, CURP y NSS) de los trabajadores con recibo en el periodo, estén o no activos.</summary>
        public Dictionary<int, DatosEmpleadoCalculo> ListarIdentidadConRecibo(int periodoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT e.EmpleadoId, e.NumeroEmpleado, " +
                "CONCAT(e.ApellidoPaterno, ' ', ISNULL(e.ApellidoMaterno + ' ', ''), e.Nombres) AS NombreCompleto, " +
                "f.RFC, f.CURP, d.NSS " +
                "FROM dbo.DocReciboNomina r                 INNER JOIN dbo.CatEmpleado e ON e.EmpleadoId = r.EmpleadoId " +
                                "LEFT JOIN dbo.CatEmpleadoDatosFiscales f ON f.EmpleadoId = e.EmpleadoId " +
                                "LEFT JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId " +
                "WHERE r.PeriodoId = @Periodo AND r.Estatus <> 'PRERECIBO' " +
                "ORDER BY e.ApellidoPaterno, e.ApellidoMaterno, e.Nombres",
                AccesoDatos.Parametro("@Periodo", periodoId));

            Dictionary<int, DatosEmpleadoCalculo> resultado = new Dictionary<int, DatosEmpleadoCalculo>();
            foreach (DataRow f in t.Rows)
            {
                int id = Fila.Obtener<int>(f, "EmpleadoId");
                resultado[id] = new DatosEmpleadoCalculo
                {
                    EmpleadoId = id,
                    NumeroEmpleado = Fila.Obtener<string>(f, "NumeroEmpleado"),
                    NombreCompleto = Fila.Obtener<string>(f, "NombreCompleto"),
                    RFC = Fila.Obtener<string>(f, "RFC"),
                    CURP = Fila.Obtener<string>(f, "CURP"),
                    NSS = Fila.Obtener<string>(f, "NSS")
                };
            }
            return resultado;
        }

        #endregion

        #region Días laborados por semana

        /// <summary>Ajusta los días por semana solo para el periodo (1 a 7); null quita el ajuste y vuelve al valor del contrato o de la jornada.</summary>
        public void GuardarDiasSemanaPeriodo(int periodoId, int empleadoId, int? dias)
        {
            if (dias.HasValue)
            {
                AccesoDatos.EjecutarComando(
                    "MERGE dbo.ProcDiasSemanaPeriodo AS d USING (SELECT @Periodo AS PeriodoId, @Empleado AS EmpleadoId) AS o " +
                    "ON d.PeriodoId = o.PeriodoId AND d.EmpleadoId = o.EmpleadoId " +
                    "WHEN MATCHED THEN UPDATE SET DiasSemana = @Dias " +
                    "WHEN NOT MATCHED THEN INSERT (PeriodoId, EmpleadoId, DiasSemana) VALUES (@Periodo, @Empleado, @Dias);",
                    AccesoDatos.Parametro("@Periodo", periodoId),
                    AccesoDatos.Parametro("@Empleado", empleadoId),
                    AccesoDatos.Parametro("@Dias", (byte)dias.Value));
                return;
            }

            AccesoDatos.EjecutarComando(
                "DELETE dbo.ProcDiasSemanaPeriodo WHERE PeriodoId = @Periodo AND EmpleadoId = @Empleado",
                AccesoDatos.Parametro("@Periodo", periodoId),
                AccesoDatos.Parametro("@Empleado", empleadoId));
        }

        /// <summary>
        /// Deja fijos los días por semana en el contrato vigente (null = según la jornada) y elimina el ajuste del periodo
        /// para que el nuevo valor sea el que se aplique. Devuelve false si el empleado no tiene contrato vigente.
        /// </summary>
        public bool GuardarDiasSemanaContrato(int periodoId, int empleadoId, int? dias)
        {
            return AccesoDatos.EjecutarEnTransaccion<bool>(delegate(SqlConnection cn, SqlTransaction tx)
            {
                int filas = AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.DocContratoEmpleado SET DiasLaboradosSemana = @Dias WHERE EmpleadoId = @Empleado AND Activo = 1",
                    AccesoDatos.Parametro("@Empleado", empleadoId),
                    AccesoDatos.ParametroEntero("@Dias", dias));
                AccesoDatos.EjecutarComando(cn, tx,
                    "DELETE dbo.ProcDiasSemanaPeriodo WHERE PeriodoId = @Periodo AND EmpleadoId = @Empleado",
                    AccesoDatos.Parametro("@Periodo", periodoId),
                    AccesoDatos.Parametro("@Empleado", empleadoId));
                return filas > 0;
            });
        }

        #endregion

        #region Persistencia

        /// <summary>
        /// Guarda el pre-recibo de un trabajador como borrador (estatus PRERECIBO) en una sola transacción: reemplaza la
        /// cabecera y el detalle anteriores del borrador. Devuelve el ReciboId.
        /// </summary>
        public int GuardarBorrador(PreReciboDTO pre)
        {
            return AccesoDatos.EjecutarEnTransaccion<int>(delegate(SqlConnection cn, SqlTransaction tx)
            {
                ExigirConceptosVigentes(cn, tx, pre.PeriodoId, new[] { pre }, pre.EmpleadoId);
                DataTable existente = AccesoDatos.Consultar(cn, tx,
                    "SELECT ReciboId, Estatus FROM dbo.DocReciboNomina WITH (UPDLOCK, HOLDLOCK) WHERE PeriodoId = @Periodo AND EmpleadoId = @Empleado",
                    AccesoDatos.Parametro("@Periodo", pre.PeriodoId),
                    AccesoDatos.Parametro("@Empleado", pre.EmpleadoId));

                if (existente.Rows.Count > 0)
                {
                    if (Fila.Obtener<string>(existente.Rows[0], "Estatus") != EstatusRecibo.Prerecibo)
                    {
                        throw new ExcepcionDatos("El trabajador ya tiene un recibo definitivo en el periodo y no puede modificarse.", 50000, true, null);
                    }
                    EliminarRecibo(cn, tx, Fila.Obtener<int>(existente.Rows[0], "ReciboId"));
                }
                return InsertarRecibo(cn, tx, pre, EstatusRecibo.Prerecibo);
            });
        }

        /// <summary>
        /// Persiste los recibos definitivos del periodo y lo cierra en una sola transacción: toma el periodo, reemplaza borradores,
        /// emite los recibos CALCULADOS, autoriza el periodo y ejecuta sp_CerrarPeriodo (amortiza préstamos, consolida acumulados
        /// y crea los CFDI pendientes de timbrado). Ante cualquier error se revierte todo.
        /// </summary>
        public void CerrarNomina(int periodoId, IList<PreReciboDTO> recibos, int? usuarioId, string comentario)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                int tomado = AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.ProcPeriodoNomina SET Estatus = 'EN_CALCULO', UsuarioCalculando = SYSTEM_USER, FechaInicioCalculo = GETDATE() " +
                    "WHERE PeriodoId = @Periodo AND Estatus IN ('CREADO','PRENOMINA')",
                    AccesoDatos.Parametro("@Periodo", periodoId));
                if (tomado == 0)
                {
                    throw new ExcepcionDatos("El periodo ya no está abierto: otro usuario lo procesó o lo está procesando.", 50000, true, null);
                }
                ExigirConceptosVigentes(cn, tx, periodoId, recibos, null, true);

                AccesoDatos.EjecutarComando(cn, tx,
                    "DELETE rd FROM dbo.DocReciboDetalle rd INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = rd.ReciboId " +
                    "WHERE rn.PeriodoId = @Periodo",
                    AccesoDatos.Parametro("@Periodo", periodoId));
                AccesoDatos.EjecutarComando(cn, tx,
                    "DELETE FROM dbo.DocReciboNomina WHERE PeriodoId = @Periodo",
                    AccesoDatos.Parametro("@Periodo", periodoId));

                foreach (PreReciboDTO pre in recibos)
                {
                    InsertarRecibo(cn, tx, pre, EstatusRecibo.Calculado);
                }

                AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.ProcPeriodoNomina SET Estatus = 'CALCULADO', UsuarioCalculando = NULL, FechaInicioCalculo = NULL " +
                    "WHERE PeriodoId = @Periodo",
                    AccesoDatos.Parametro("@Periodo", periodoId));
                AccesoDatos.EjecutarComando(cn, tx,
                    "INSERT INTO dbo.LogCalculo (PeriodoId, Tipo, Mensaje) VALUES (@Periodo, 'NOMINA', CONCAT('Nómina calculada desde la prenómina por ', SYSTEM_USER))",
                    AccesoDatos.Parametro("@Periodo", periodoId));

                AutorizarYCerrar(cn, tx, periodoId, true, usuarioId, comentario);
            });
        }

        /// <summary>Autoriza (si aún está CALCULADO) y cierra un periodo cuyos recibos ya existen, en una sola transacción.</summary>
        public void CerrarPeriodoProcesado(int periodoId, bool autorizar, int? usuarioId, string comentario)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                AutorizarYCerrar(cn, tx, periodoId, autorizar, usuarioId, comentario);
            });
        }

        private static void AutorizarYCerrar(SqlConnection cn, SqlTransaction tx, int periodoId, bool autorizar, int? usuarioId, string comentario)
        {
            if (autorizar)
            {
                AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_AutorizarPeriodo",
                    AccesoDatos.Parametro("@PeriodoId", periodoId),
                    AccesoDatos.ParametroEntero("@UsuarioId", usuarioId),
                    AccesoDatos.Parametro("@Comentario", comentario));
            }
            AccesoDatos.EjecutarSP(cn, tx, "dbo.sp_CerrarPeriodo", AccesoDatos.Parametro("@PeriodoId", periodoId));
        }

        private static void EliminarRecibo(SqlConnection cn, SqlTransaction tx, int reciboId)
        {
            AccesoDatos.EjecutarComando(cn, tx,
                "DELETE FROM dbo.DocReciboDetalle WHERE ReciboId = @Id", AccesoDatos.Parametro("@Id", reciboId));
            AccesoDatos.EjecutarComando(cn, tx,
                "DELETE FROM dbo.DocReciboNomina WHERE ReciboId = @Id", AccesoDatos.Parametro("@Id", reciboId));
        }

        private static void ExigirConceptosVigentes(SqlConnection cn, SqlTransaction tx, int periodoId,
            IList<PreReciboDTO> recibos, int? empleadoId, bool tomadoParaCalculo = false)
        {
            object estatus = AccesoDatos.Escalar(cn, tx,
                "SELECT Estatus FROM dbo.ProcPeriodoNomina WITH(UPDLOCK,HOLDLOCK) WHERE PeriodoId=@Periodo",
                AccesoDatos.Parametro("@Periodo", periodoId));
            if (!Equals(estatus, "CREADO") && !Equals(estatus, "PRENOMINA") &&
                !(tomadoParaCalculo && Equals(estatus, "EN_CALCULO")))
                throw new ExcepcionDatos("El periodo ya no admite guardar recibos.", 50000, true, null);
            DataTable invalidos = AccesoDatos.Consultar(cn, tx,
                "SELECT c.Descripcion AS Nombre FROM dbo.ProcIncidencias i JOIN dbo.CatConcepto c WITH (HOLDLOCK) ON c.ConceptoId=i.IdConceptoSAT " +
                "WHERE i.IdPeriodo=@Periodo AND (@Empleado IS NULL OR i.IdEmpleado=@Empleado) AND c.TipoConcepto<>'I' AND (c.Activo=0 OR c.EsIncidencia=0)",
                AccesoDatos.Parametro("@Periodo", periodoId), AccesoDatos.ParametroEntero("@Empleado", empleadoId));
            if (invalidos.Rows.Count > 0)
                throw new ExcepcionDatos("Una incidencia usa un concepto SAT inactivo. Recargue y revise la clasificación.", 50000, true, null);
            DataTable actuales = AccesoDatos.Consultar(cn, tx,
                "SELECT i.IdIncidencia,i.IdEmpleado,i.CantidadOValor,i.ImporteCalculado,c.Descripcion AS Nombre,c.TipoConcepto AS TipoMovimiento,c.EsGravado,c.ClaveSAT " +
                "FROM dbo.ProcIncidencias i JOIN dbo.CatConcepto c WITH (HOLDLOCK) ON c.ConceptoId=i.IdConceptoSAT AND c.TipoConcepto<>'I' " +
                "WHERE i.IdPeriodo=@Periodo AND (@Empleado IS NULL OR i.IdEmpleado=@Empleado)",
                AccesoDatos.Parametro("@Periodo", periodoId), AccesoDatos.ParametroEntero("@Empleado", empleadoId));
            foreach (PreReciboDTO pre in recibos)
            {
                var lineas = pre.Percepciones.Concat(pre.Deducciones).Where(c => c.IdIncidenciaConcepto.HasValue).ToList();
                var filas = actuales.Rows.Cast<DataRow>().Where(f => Fila.Obtener<int>(f, "IdEmpleado") == pre.EmpleadoId).ToList();
                bool coincide = filas.Count == lineas.Count && filas.All(f => lineas.Any(c =>
                    c.IdIncidenciaConcepto == Fila.Obtener<int>(f, "IdIncidencia") &&
                    c.Cantidad == Fila.Obtener<decimal>(f, "CantidadOValor") &&
                    c.Total == Fila.Obtener<decimal>(f, "ImporteCalculado") &&
                    c.Descripcion == Fila.Obtener<string>(f, "Nombre") &&
                    c.Clave == Fila.Obtener<string>(f, "ClaveSAT") &&
                    c.TipoConcepto == Fila.Obtener<string>(f, "TipoMovimiento") &&
                    (c.TipoConcepto != "P" || (c.ImporteGravado > 0m) == Fila.Obtener<bool>(f, "EsGravado"))));
                if (!coincide)
                    throw new ExcepcionDatos("Las incidencias cambiaron mientras se calculaba el recibo. Recargue el periodo e intente nuevamente.", 50000, true, null);
            }
        }

        private static int InsertarRecibo(SqlConnection cn, SqlTransaction tx, PreReciboDTO pre, string estatus)
        {
            object id = AccesoDatos.Escalar(cn, tx, InsertarCabecera,
                AccesoDatos.Parametro("@Periodo", pre.PeriodoId),
                AccesoDatos.Parametro("@Empleado", pre.EmpleadoId),
                AccesoDatos.ParametroDecimal("@Percepciones", pre.TotalPercepciones, 18, 2),
                AccesoDatos.ParametroDecimal("@Deducciones", pre.TotalDeducciones, 18, 2),
                AccesoDatos.ParametroDecimal("@Neto", pre.NetoPagar, 18, 2),
                AccesoDatos.ParametroDecimal("@Gravado", pre.TotalGravado, 18, 4),
                AccesoDatos.ParametroDecimal("@Exento", pre.TotalExento, 18, 4),
                AccesoDatos.ParametroDecimal("@Dias", pre.DiasPagados, 10, 4),
                AccesoDatos.ParametroDecimal("@SD", pre.SalarioDiario, 18, 4),
                AccesoDatos.ParametroDecimal("@SDI", pre.SDI, 18, 4),
                AccesoDatos.ParametroDecimal("@SBC", pre.SBC, 18, 4),
                AccesoDatos.Parametro("@Estatus", estatus),
                AccesoDatos.Parametro("@Notas", pre.Notas));
            if (id == null)
            {
                throw new ExcepcionDatos("No se pudo crear el recibo del trabajador " + pre.NumeroEmpleado + ": el periodo o el empleado ya no existen.");
            }
            int reciboId = Convert.ToInt32(id);

            foreach (PreReciboConceptoDTO c in pre.Percepciones)
            {
                InsertarDetalle(cn, tx, reciboId, c);
            }
            foreach (PreReciboConceptoDTO c in pre.Deducciones)
            {
                InsertarDetalle(cn, tx, reciboId, c);
            }
            return reciboId;
        }

        private static void InsertarDetalle(SqlConnection cn, SqlTransaction tx, int reciboId, PreReciboConceptoDTO c)
        {
            AccesoDatos.EjecutarComando(cn, tx,
                "INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento, Cantidad, EsIncidencia) " +
                "VALUES (@Recibo, @Tipo, @Clave, @Descripcion, @Gravado, @Exento, @Cantidad, @EsIncidencia)",
                AccesoDatos.Parametro("@Recibo", reciboId),
                AccesoDatos.Parametro("@Tipo", c.TipoConcepto),
                AccesoDatos.Parametro("@Clave", c.Clave),
                AccesoDatos.Parametro("@Descripcion", c.Descripcion),
                AccesoDatos.ParametroDecimal("@Gravado", c.ImporteGravado, 18, 2),
                AccesoDatos.ParametroDecimal("@Exento", c.ImporteExento, 18, 2),
                AccesoDatos.ParametroDecimal("@Cantidad", c.Cantidad, 18, 4),
                AccesoDatos.Parametro("@EsIncidencia", c.IdIncidenciaConcepto.HasValue));
        }

        #endregion
    }
}
