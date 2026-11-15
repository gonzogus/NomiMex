using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Documentos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>
    /// Acceso a la información del empleado. El alta/edición/baja afecta varias tablas
    /// (CatEmpleado, DatosFiscales, DatosIMSS, contrato, cuenta bancaria, histórico y auditoría salarial)
    /// y por eso se ejecuta siempre dentro de una SqlTransaction.
    /// </summary>
    public class EmpleadoDAL
    {
        private const string SelectListado =
            "SELECT e.EmpleadoId, e.EmpresaId, emp.RazonSocial AS Empresa, e.NumeroEmpleado, " +
            "CONCAT(e.ApellidoPaterno, ' ', ISNULL(e.ApellidoMaterno + ' ', ''), e.Nombres) AS NombreCompleto, " +
            "e.FechaIngreso, e.Estatus, f.RFC, f.CURP, i.NSS, i.SalarioDiario AS SD, " +
            "dep.Descripcion AS Departamento, pu.Descripcion AS Puesto, tc.Descripcion AS TipoContrato, tp.Descripcion AS TipoPeriodo " +
            "FROM dbo.CatEmpleado e " +
            "INNER JOIN dbo.CatEmpresa emp ON emp.EmpresaId = e.EmpresaId " +
            "LEFT JOIN dbo.CatEmpleadoDatosFiscales f ON f.EmpleadoId = e.EmpleadoId " +
            "LEFT JOIN dbo.CatEmpleadoDatosIMSS i ON i.EmpleadoId = e.EmpleadoId " +
            "LEFT JOIN dbo.DocContratoEmpleado ec ON ec.EmpleadoId = e.EmpleadoId AND ec.Activo = 1 " +
            "LEFT JOIN dbo.CatDepartamento dep ON dep.DepartamentoId = ec.DepartamentoId " +
            "LEFT JOIN dbo.CatPuesto pu ON pu.PuestoId = ec.PuestoId " +
            "LEFT JOIN dbo.CatTipoContrato tc ON tc.TipoContratoId = ec.TipoContratoId " +
            "LEFT JOIN dbo.CatPeriodicidadPago tp ON tp.PeriodicidadPagoId = ec.PeriodicidadPagoId ";

        public List<EmpleadoListado> Listar(int? empresaId, bool soloActivos)
        {
            DataTable tabla = AccesoDatos.Consultar(
                SelectListado +
                "WHERE (@EmpresaId IS NULL OR e.EmpresaId = @EmpresaId) AND (@SoloActivos = 0 OR e.Estatus = 'ACTIVO') " +
                "ORDER BY e.ApellidoPaterno, e.ApellidoMaterno, e.Nombres",
                AccesoDatos.ParametroEntero("@EmpresaId", empresaId),
                AccesoDatos.Parametro("@SoloActivos", soloActivos));
            return MapearListado(tabla);
        }

        private static List<EmpleadoListado> MapearListado(DataTable tabla)
        {
            List<EmpleadoListado> lista = new List<EmpleadoListado>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new EmpleadoListado
                {
                    EmpleadoId = Fila.Obtener<int>(fila, "EmpleadoId"),
                    EmpresaId = Fila.Obtener<int>(fila, "EmpresaId"),
                    Empresa = Fila.Obtener<string>(fila, "Empresa"),
                    NumeroEmpleado = Fila.Obtener<string>(fila, "NumeroEmpleado"),
                    NombreCompleto = Fila.Obtener<string>(fila, "NombreCompleto"),
                    FechaIngreso = Fila.Obtener<DateTime>(fila, "FechaIngreso"),
                    Estatus = Fila.Obtener<string>(fila, "Estatus"),
                    RFC = Fila.Obtener<string>(fila, "RFC"),
                    CURP = Fila.Obtener<string>(fila, "CURP"),
                    NSS = Fila.Obtener<string>(fila, "NSS"),
                    SD = Fila.Obtener<decimal?>(fila, "SD"),
                    Departamento = Fila.Obtener<string>(fila, "Departamento"),
                    Puesto = Fila.Obtener<string>(fila, "Puesto"),
                    TipoContrato = Fila.Obtener<string>(fila, "TipoContrato"),
                    TipoPeriodo = Fila.Obtener<string>(fila, "TipoPeriodo")
                });
            }
            return lista;
        }

        /// <summary>
        /// Empleados que procesará la nómina de un periodo: activos, con datos IMSS, contrato vigente de la periodicidad
        /// y que ya habían ingresado a la fecha final del periodo. Es el mismo criterio de sp_CalcularNomina.
        /// </summary>
        public List<EmpleadoListado> ListarElegibles(int empresaId, int tipoPeriodoId, DateTime fechaFinPeriodo)
        {
            DataTable tabla = AccesoDatos.Consultar(
                SelectListado +
                "WHERE e.EmpresaId = @Empresa AND e.Estatus = 'ACTIVO' AND ec.PeriodicidadPagoId = @Tipo " +
                "AND i.EmpleadoId IS NOT NULL AND e.FechaIngreso <= @Fin " +
                "ORDER BY e.ApellidoPaterno, e.ApellidoMaterno, e.Nombres",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Tipo", tipoPeriodoId),
                AccesoDatos.Parametro("@Fin", fechaFinPeriodo.Date));
            return MapearListado(tabla);
        }

        /// <summary>Empleados activos asignados a la combinación Empresa + Periodicidad (sin considerar fechas de un periodo).</summary>
        public int ContarActivosPorPeriodicidad(int empresaId, int tipoPeriodoId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.CatEmpleado e " +
                "INNER JOIN dbo.DocContratoEmpleado c ON c.EmpleadoId = e.EmpleadoId AND c.Activo = 1 " +
                "WHERE e.EmpresaId = @Empresa AND e.Estatus = 'ACTIVO' AND c.PeriodicidadPagoId = @Tipo",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Tipo", tipoPeriodoId));
            return Convert.ToInt32(r);
        }

        public EmpleadoExpediente ObtenerExpediente(int empleadoId)
        {
            EmpleadoExpediente x = new EmpleadoExpediente();

            DataTable t = AccesoDatos.Consultar(
                "SELECT EmpleadoId, EmpresaId, NumeroEmpleado, Nombres, ApellidoPaterno, ApellidoMaterno, FechaIngreso, " +
                "FechaNacimiento, Sexo, EstadoCivil, Nacionalidad, Estatus, TurnoId, FechaAlta, FechaModificacion " +
                "FROM dbo.CatEmpleado WHERE EmpleadoId = @Id",
                AccesoDatos.Parametro("@Id", empleadoId));
            if (t.Rows.Count == 0)
            {
                return null;
            }

            DataRow f = t.Rows[0];
            x.Empleado = new CatEmpleado
            {
                EmpleadoId = Fila.Obtener<int>(f, "EmpleadoId"),
                EmpresaId = Fila.Obtener<int>(f, "EmpresaId"),
                NumeroEmpleado = Fila.Obtener<string>(f, "NumeroEmpleado"),
                Nombres = Fila.Obtener<string>(f, "Nombres"),
                ApellidoPaterno = Fila.Obtener<string>(f, "ApellidoPaterno"),
                ApellidoMaterno = Fila.Obtener<string>(f, "ApellidoMaterno"),
                FechaIngreso = Fila.Obtener<DateTime>(f, "FechaIngreso"),
                FechaNacimiento = Fila.Obtener<DateTime?>(f, "FechaNacimiento"),
                Sexo = Fila.Obtener<string>(f, "Sexo"),
                EstadoCivil = Fila.Obtener<string>(f, "EstadoCivil"),
                Nacionalidad = Fila.Obtener<string>(f, "Nacionalidad"),
                Estatus = Fila.Obtener<string>(f, "Estatus"),
                TurnoId = Fila.Obtener<int?>(f, "TurnoId"),
                FechaAlta = Fila.Obtener<DateTime>(f, "FechaAlta"),
                FechaModificacion = Fila.Obtener<DateTime?>(f, "FechaModificacion")
            };

            t = AccesoDatos.Consultar(
                "SELECT EmpleadoId, RFC, CURP, RegimenFiscalSAT, CodigoPostal, ClaveEntidadFederativa, Email, Telefono " +
                "FROM dbo.CatEmpleadoDatosFiscales WHERE EmpleadoId = @Id", AccesoDatos.Parametro("@Id", empleadoId));
            if (t.Rows.Count > 0)
            {
                f = t.Rows[0];
                x.Fiscales = new CatEmpleadoDatosFiscales
                {
                    EmpleadoId = empleadoId,
                    RFC = Fila.Obtener<string>(f, "RFC"),
                    CURP = Fila.Obtener<string>(f, "CURP"),
                    RegimenFiscalSAT = Fila.Obtener<int>(f, "RegimenFiscalSAT"),
                    CodigoPostal = Fila.Obtener<string>(f, "CodigoPostal"),
                    ClaveEntidadFederativa = Fila.Obtener<string>(f, "ClaveEntidadFederativa"),
                    Email = Fila.Obtener<string>(f, "Email"),
                    Telefono = Fila.Obtener<string>(f, "Telefono")
                };
            }

            t = AccesoDatos.Consultar(
                "SELECT EmpleadoId, NSS, SalarioDiario, SalarioDiarioIntegrado, SBC, TipoRiesgo " +
                "FROM dbo.CatEmpleadoDatosIMSS WHERE EmpleadoId = @Id", AccesoDatos.Parametro("@Id", empleadoId));
            if (t.Rows.Count > 0)
            {
                f = t.Rows[0];
                x.IMSS = new CatEmpleadoDatosIMSS
                {
                    EmpleadoId = empleadoId,
                    NSS = Fila.Obtener<string>(f, "NSS"),
                    SalarioDiario = Fila.Obtener<decimal>(f, "SalarioDiario"),
                    SalarioDiarioIntegrado = Fila.Obtener<decimal>(f, "SalarioDiarioIntegrado"),
                    SBC = Fila.Obtener<decimal>(f, "SBC"),
                    TipoRiesgo = Fila.Obtener<int>(f, "TipoRiesgo")
                };
            }

            t = AccesoDatos.Consultar(
                "SELECT TOP 1 EmpleadoContratoId, EmpleadoId, TipoContratoId, DepartamentoId, PuestoId, CentroCostoId, " +
                "TipoJornadaId, PeriodicidadPagoId, TipoRegimenId, SalarioBaseCotizacion, SalarioDiarioIntegrado, ZonaSMG, DiasLaboradosSemana, " +
                "FechaInicio, FechaFin, Activo FROM dbo.DocContratoEmpleado WHERE EmpleadoId = @Id " +
                "ORDER BY Activo DESC, FechaInicio DESC, EmpleadoContratoId DESC",
                AccesoDatos.Parametro("@Id", empleadoId));
            if (t.Rows.Count > 0)
            {
                f = t.Rows[0];
                x.Contrato = new DocContratoEmpleado
                {
                    EmpleadoContratoId = Fila.Obtener<int>(f, "EmpleadoContratoId"),
                    EmpleadoId = empleadoId,
                    TipoContratoId = Fila.Obtener<int>(f, "TipoContratoId"),
                    DepartamentoId = Fila.Obtener<int?>(f, "DepartamentoId"),
                    PuestoId = Fila.Obtener<int?>(f, "PuestoId"),
                    CentroCostoId = Fila.Obtener<int?>(f, "CentroCostoId"),
                    TipoJornadaId = Fila.Obtener<int>(f, "TipoJornadaId"),
                    PeriodicidadPagoId = Fila.Obtener<int>(f, "PeriodicidadPagoId"),
                    TipoRegimenId = Fila.Obtener<int>(f, "TipoRegimenId"),
                    SalarioBaseCotizacion = Fila.Obtener<decimal?>(f, "SalarioBaseCotizacion"),
                    SalarioDiarioIntegrado = Fila.Obtener<decimal?>(f, "SalarioDiarioIntegrado"),
                    ZonaSMG = Fila.Obtener<string>(f, "ZonaSMG"),
                    DiasLaboradosSemana = Fila.Obtener<int?>(f, "DiasLaboradosSemana"),
                    FechaInicio = Fila.Obtener<DateTime>(f, "FechaInicio"),
                    FechaFin = Fila.Obtener<DateTime?>(f, "FechaFin"),
                    Activo = Fila.Obtener<bool>(f, "Activo")
                };
            }

            t = AccesoDatos.Consultar(
                "SELECT TOP 1 BancoCuentaId, EmpleadoId, BancoId, MetodoPagoId, CLABE, Activa FROM dbo.CatEmpleadoBancoCuenta " +
                "WHERE EmpleadoId = @Id ORDER BY Activa DESC, BancoCuentaId DESC", AccesoDatos.Parametro("@Id", empleadoId));
            if (t.Rows.Count > 0)
            {
                f = t.Rows[0];
                x.Cuenta = new CatEmpleadoBancoCuenta
                {
                    BancoCuentaId = Fila.Obtener<int>(f, "BancoCuentaId"),
                    EmpleadoId = empleadoId,
                    BancoId = Fila.Obtener<int?>(f, "BancoId"),
                    MetodoPagoId = Fila.Obtener<int?>(f, "MetodoPagoId"),
                    CLABE = (Fila.Obtener<string>(f, "CLABE") ?? string.Empty).Trim(),
                    Activa = Fila.Obtener<bool>(f, "Activa")
                };
            }

            return x;
        }

        public bool ExisteNumeroEmpleado(int empresaId, string numeroEmpleado, int excluirEmpleadoId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.CatEmpleado WHERE EmpresaId = @Empresa AND NumeroEmpleado = @Numero AND EmpleadoId <> @Id",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Numero", numeroEmpleado),
                AccesoDatos.Parametro("@Id", excluirEmpleadoId));
            return Convert.ToInt32(r) > 0;
        }

        /// <summary>
        /// Guarda el expediente completo en una transacción. Registra histórico y auditoría salarial cuando cambian
        /// el salario diario, el SDI o el SBC. Devuelve el EmpleadoId.
        /// </summary>
        public int Guardar(EmpleadoExpediente x, int? usuarioId, decimal factorIntegracion)
        {
            return AccesoDatos.EjecutarEnTransaccion<int>(delegate(SqlConnection cn, SqlTransaction tx)
            {
                CatEmpleado e = x.Empleado;
                bool esNuevo = e.EmpleadoId == 0;
                decimal? sdPrevio = null, sdiPrevio = null, sbcPrevio = null;

                if (esNuevo)
                {
                    object id = AccesoDatos.Escalar(cn, tx,
                        "INSERT INTO dbo.CatEmpleado (EmpresaId, NumeroEmpleado, Nombres, ApellidoPaterno, ApellidoMaterno, FechaIngreso, " +
                        "FechaNacimiento, Sexo, EstadoCivil, Nacionalidad, Estatus, TurnoId) " +
                        "VALUES (@Empresa, @Numero, @Nombres, @Paterno, @Materno, @Ingreso, @Nacimiento, @Sexo, @EstadoCivil, @Nacionalidad, @Estatus, @Turno); " +
                        "SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        ParametrosEmpleado(e));
                    e.EmpleadoId = Convert.ToInt32(id);
                }
                else
                {
                    DataTable previo = AccesoDatos.Consultar(cn, tx,
                        "SELECT SalarioDiario, SalarioDiarioIntegrado, SBC FROM dbo.CatEmpleadoDatosIMSS WHERE EmpleadoId = @Id",
                        AccesoDatos.Parametro("@Id", e.EmpleadoId));
                    if (previo.Rows.Count > 0)
                    {
                        sdPrevio = Fila.Obtener<decimal>(previo.Rows[0], "SalarioDiario");
                        sdiPrevio = Fila.Obtener<decimal>(previo.Rows[0], "SalarioDiarioIntegrado");
                        sbcPrevio = Fila.Obtener<decimal>(previo.Rows[0], "SBC");
                    }

                    int afectadas = AccesoDatos.EjecutarComando(cn, tx,
                        "UPDATE dbo.CatEmpleado SET EmpresaId = @Empresa, NumeroEmpleado = @Numero, Nombres = @Nombres, " +
                        "ApellidoPaterno = @Paterno, ApellidoMaterno = @Materno, FechaIngreso = @Ingreso, FechaNacimiento = @Nacimiento, " +
                        "Sexo = @Sexo, EstadoCivil = @EstadoCivil, Nacionalidad = @Nacionalidad, Estatus = @Estatus, TurnoId = @Turno " +
                        "WHERE EmpleadoId = @Id",
                        ParametrosEmpleado(e));
                    if (afectadas == 0)
                    {
                        throw new ExcepcionDatos("El empleado ya no existe en la base de datos.");
                    }
                }

                int empleadoId = e.EmpleadoId;
                GuardarFiscales(cn, tx, empleadoId, x.Fiscales);
                GuardarIMSS(cn, tx, empleadoId, x.IMSS);
                GuardarContrato(cn, tx, empleadoId, x.Contrato, x.IMSS, e.Estatus);
                GuardarCuenta(cn, tx, empleadoId, x.Cuenta);

                bool cambioSalario = esNuevo || sdPrevio != x.IMSS.SalarioDiario ||
                                     sdiPrevio != x.IMSS.SalarioDiarioIntegrado || sbcPrevio != x.IMSS.SBC;
                if (cambioSalario)
                {
                    RegistrarHistorialSalario(cn, tx, empleadoId, x.IMSS, factorIntegracion, usuarioId,
                        sdPrevio, sdiPrevio, sbcPrevio, esNuevo ? "Alta de empleado" : "Actualización desde el catálogo de empleados");
                }

                return empleadoId;
            });
        }

        /// <summary>Baja lógica: marca al empleado como BAJA y cierra su contrato vigente en una transacción.</summary>
        public void DarDeBajaLogica(int empleadoId)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                int afectadas = AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.CatEmpleado SET Estatus = 'BAJA' WHERE EmpleadoId = @Id",
                    AccesoDatos.Parametro("@Id", empleadoId));
                if (afectadas == 0)
                {
                    throw new ExcepcionDatos("El empleado ya no existe en la base de datos.");
                }
                CerrarContratoVigente(cn, tx, empleadoId);
            });
        }

        private static SqlParameter[] ParametrosEmpleado(CatEmpleado e)
        {
            return new[]
            {
                AccesoDatos.Parametro("@Id", e.EmpleadoId),
                AccesoDatos.Parametro("@Empresa", e.EmpresaId),
                AccesoDatos.Parametro("@Numero", e.NumeroEmpleado),
                AccesoDatos.Parametro("@Nombres", e.Nombres),
                AccesoDatos.Parametro("@Paterno", e.ApellidoPaterno),
                AccesoDatos.Parametro("@Materno", e.ApellidoMaterno),
                AccesoDatos.Parametro("@Ingreso", e.FechaIngreso.Date),
                AccesoDatos.Parametro("@Nacimiento", e.FechaNacimiento.HasValue ? (object)e.FechaNacimiento.Value.Date : null),
                AccesoDatos.Parametro("@Sexo", e.Sexo),
                AccesoDatos.Parametro("@EstadoCivil", e.EstadoCivil),
                AccesoDatos.Parametro("@Nacionalidad", e.Nacionalidad),
                AccesoDatos.Parametro("@Estatus", e.Estatus),
                AccesoDatos.ParametroEntero("@Turno", e.TurnoId)
            };
        }

        private static void GuardarFiscales(SqlConnection cn, SqlTransaction tx, int empleadoId, CatEmpleadoDatosFiscales d)
        {
            AccesoDatos.EjecutarComando(cn, tx,
                "IF EXISTS (SELECT 1 FROM dbo.CatEmpleadoDatosFiscales WHERE EmpleadoId = @Id) " +
                "UPDATE dbo.CatEmpleadoDatosFiscales SET RFC = @RFC, CURP = @CURP, RegimenFiscalSAT = @Regimen, CodigoPostal = @CP, " +
                "ClaveEntidadFederativa = @Entidad, Email = @Email, Telefono = @Telefono WHERE EmpleadoId = @Id " +
                "ELSE INSERT INTO dbo.CatEmpleadoDatosFiscales (EmpleadoId, RFC, CURP, RegimenFiscalSAT, CodigoPostal, ClaveEntidadFederativa, Email, Telefono) " +
                "VALUES (@Id, @RFC, @CURP, @Regimen, @CP, @Entidad, @Email, @Telefono)",
                AccesoDatos.Parametro("@Id", empleadoId),
                AccesoDatos.Parametro("@RFC", d.RFC),
                AccesoDatos.Parametro("@CURP", d.CURP),
                AccesoDatos.Parametro("@Regimen", d.RegimenFiscalSAT),
                AccesoDatos.Parametro("@CP", d.CodigoPostal),
                AccesoDatos.Parametro("@Entidad", d.ClaveEntidadFederativa),
                AccesoDatos.Parametro("@Email", d.Email),
                AccesoDatos.Parametro("@Telefono", d.Telefono));
        }

        private static void GuardarIMSS(SqlConnection cn, SqlTransaction tx, int empleadoId, CatEmpleadoDatosIMSS d)
        {
            AccesoDatos.EjecutarComando(cn, tx,
                "IF EXISTS (SELECT 1 FROM dbo.CatEmpleadoDatosIMSS WHERE EmpleadoId = @Id) " +
                "UPDATE dbo.CatEmpleadoDatosIMSS SET NSS = @NSS, SalarioDiario = @SD, SalarioDiarioIntegrado = @SDI, SBC = @SBC, TipoRiesgo = @Riesgo WHERE EmpleadoId = @Id " +
                "ELSE INSERT INTO dbo.CatEmpleadoDatosIMSS (EmpleadoId, NSS, SalarioDiario, SalarioDiarioIntegrado, SBC, TipoRiesgo) " +
                "VALUES (@Id, @NSS, @SD, @SDI, @SBC, @Riesgo)",
                AccesoDatos.Parametro("@Id", empleadoId),
                AccesoDatos.Parametro("@NSS", d.NSS),
                AccesoDatos.ParametroDecimal("@SD", d.SalarioDiario, 18, 4),
                AccesoDatos.ParametroDecimal("@SDI", d.SalarioDiarioIntegrado, 18, 4),
                AccesoDatos.ParametroDecimal("@SBC", d.SBC, 18, 4),
                AccesoDatos.Parametro("@Riesgo", d.TipoRiesgo));
        }

        private static void GuardarContrato(SqlConnection cn, SqlTransaction tx, int empleadoId, DocContratoEmpleado c,
                                            CatEmpleadoDatosIMSS imss, string estatusEmpleado)
        {
            AccesoDatos.EjecutarComando(cn, tx,
                "IF EXISTS (SELECT 1 FROM dbo.DocContratoEmpleado WHERE EmpleadoId = @Id AND Activo = 1) " +
                "UPDATE dbo.DocContratoEmpleado SET TipoContratoId = @TipoContrato, DepartamentoId = @Depto, PuestoId = @Puesto, " +
                "TipoJornadaId = @Jornada, PeriodicidadPagoId = @Periodicidad, " +
                "TipoRegimenId = @Regimen, SalarioBaseCotizacion = @SBC, SalarioDiarioIntegrado = @SDI, ZonaSMG = @Zona, " +
                "DiasLaboradosSemana = @DiasSemana, " +
                "FechaInicio = @Inicio, FechaFin = NULL WHERE EmpleadoId = @Id AND Activo = 1 " +
                "ELSE INSERT INTO dbo.DocContratoEmpleado (EmpleadoId, TipoContratoId, DepartamentoId, PuestoId, TipoJornadaId, " +
                "PeriodicidadPagoId, TipoRegimenId, SalarioBaseCotizacion, SalarioDiarioIntegrado, ZonaSMG, DiasLaboradosSemana, FechaInicio, Activo) " +
                "VALUES (@Id, @TipoContrato, @Depto, @Puesto, @Jornada, @Periodicidad, " +
                "@Regimen, @SBC, @SDI, @Zona, @DiasSemana, @Inicio, 1)",
                AccesoDatos.Parametro("@Id", empleadoId),
                AccesoDatos.Parametro("@TipoContrato", c.TipoContratoId),
                AccesoDatos.ParametroEntero("@Depto", c.DepartamentoId),
                AccesoDatos.ParametroEntero("@Puesto", c.PuestoId),
                AccesoDatos.Parametro("@Periodicidad", c.PeriodicidadPagoId),
                AccesoDatos.Parametro("@Jornada", c.TipoJornadaId),
                AccesoDatos.Parametro("@Regimen", c.TipoRegimenId),
                AccesoDatos.ParametroDecimal("@SBC", imss.SBC, 18, 4),
                AccesoDatos.ParametroDecimal("@SDI", imss.SalarioDiarioIntegrado, 18, 4),
                AccesoDatos.Parametro("@Zona", string.IsNullOrEmpty(c.ZonaSMG) ? "GENERAL" : c.ZonaSMG),
                AccesoDatos.ParametroEntero("@DiasSemana", c.DiasLaboradosSemana),
                AccesoDatos.Parametro("@Inicio", c.FechaInicio.Date));

            if (estatusEmpleado == EstatusEmpleado.Baja)
            {
                CerrarContratoVigente(cn, tx, empleadoId);
            }
        }

        private static void CerrarContratoVigente(SqlConnection cn, SqlTransaction tx, int empleadoId)
        {
            AccesoDatos.EjecutarComando(cn, tx,
                "UPDATE dbo.DocContratoEmpleado SET Activo = 0, " +
                "FechaFin = CASE WHEN FechaInicio > CAST(GETDATE() AS DATE) THEN FechaInicio ELSE CAST(GETDATE() AS DATE) END " +
                "WHERE EmpleadoId = @Id AND Activo = 1",
                AccesoDatos.Parametro("@Id", empleadoId));
        }

        private static void GuardarCuenta(SqlConnection cn, SqlTransaction tx, int empleadoId, CatEmpleadoBancoCuenta c)
        {
            if (c == null || string.IsNullOrWhiteSpace(c.CLABE))
            {
                return;
            }

            AccesoDatos.EjecutarComando(cn, tx,
                "IF EXISTS (SELECT 1 FROM dbo.CatEmpleadoBancoCuenta WHERE EmpleadoId = @Id AND Activa = 1) " +
                "UPDATE dbo.CatEmpleadoBancoCuenta SET BancoId = @Banco, CLABE = @CLABE WHERE EmpleadoId = @Id AND Activa = 1 " +
                "ELSE INSERT INTO dbo.CatEmpleadoBancoCuenta (EmpleadoId, BancoId, MetodoPagoId, CLABE, Activa) " +
                "VALUES (@Id, @Banco, @Metodo, @CLABE, 1)",
                AccesoDatos.Parametro("@Id", empleadoId),
                AccesoDatos.ParametroEntero("@Banco", c.BancoId),
                AccesoDatos.ParametroEntero("@Metodo", c.MetodoPagoId),
                AccesoDatos.Parametro("@CLABE", c.CLABE));
        }

        private static void RegistrarHistorialSalario(SqlConnection cn, SqlTransaction tx, int empleadoId, CatEmpleadoDatosIMSS imss,
                                                      decimal factor, int? usuarioId, decimal? sdPrevio, decimal? sdiPrevio,
                                                      decimal? sbcPrevio, string motivo)
        {
            AccesoDatos.EjecutarComando(cn, tx,
                "INSERT INTO dbo.HistEmpleadoSalario (EmpleadoId, FechaEfectiva, SalarioDiario, SDI, SBC, FactorIntegracion, MotivoCambio) " +
                "VALUES (@Id, CAST(GETDATE() AS DATE), @SD, @SDI, @SBC, @Factor, @Motivo)",
                AccesoDatos.Parametro("@Id", empleadoId),
                AccesoDatos.ParametroDecimal("@SD", imss.SalarioDiario, 18, 4),
                AccesoDatos.ParametroDecimal("@SDI", imss.SalarioDiarioIntegrado, 18, 4),
                AccesoDatos.ParametroDecimal("@SBC", imss.SBC, 18, 4),
                AccesoDatos.ParametroDecimal("@Factor", factor, 10, 6),
                AccesoDatos.Parametro("@Motivo", motivo));

            RegistrarAuditoria(cn, tx, empleadoId, usuarioId, "SalarioDiario", sdPrevio, imss.SalarioDiario, motivo);
            RegistrarAuditoria(cn, tx, empleadoId, usuarioId, "SDI", sdiPrevio, imss.SalarioDiarioIntegrado, motivo);
            RegistrarAuditoria(cn, tx, empleadoId, usuarioId, "SBC", sbcPrevio, imss.SBC, motivo);
        }

        private static void RegistrarAuditoria(SqlConnection cn, SqlTransaction tx, int empleadoId, int? usuarioId,
                                               string campo, decimal? anterior, decimal nuevo, string motivo)
        {
            if (anterior.HasValue && anterior.Value == nuevo)
            {
                return;
            }

            AccesoDatos.EjecutarComando(cn, tx,
                "INSERT INTO dbo.LogAuditoriaSalario (EmpleadoId, UsuarioId, CampoModificado, ValorAnterior, ValorNuevo, MotivoCambio) " +
                "VALUES (@Id, @Usuario, @Campo, @Anterior, @Nuevo, @Motivo)",
                AccesoDatos.Parametro("@Id", empleadoId),
                AccesoDatos.ParametroEntero("@Usuario", usuarioId),
                AccesoDatos.Parametro("@Campo", campo),
                AccesoDatos.ParametroDecimal("@Anterior", anterior, 18, 4),
                AccesoDatos.ParametroDecimal("@Nuevo", nuevo, 18, 4),
                AccesoDatos.Parametro("@Motivo", motivo));
        }
    }
}
