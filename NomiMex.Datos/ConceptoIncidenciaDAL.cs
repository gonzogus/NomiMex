using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Procesos;
using NomiMex.Entidades.Relaciones;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>
    /// Acceso a datos de las incidencias por concepto SAT. Única fuente de conceptos: dbo.CatConcepto (catálogo oficial
    /// de ingresos y deducciones). Las asignaciones individuales viven en dbo.RelEmpleadoIncidencia y los movimientos
    /// del periodo en dbo.ProcIncidencias.
    /// </summary>
    public class ConceptoIncidenciaDAL
    {
        internal const string ColumnasConcepto =
            "c.ConceptoId, c.ClaveSAT, c.TipoConcepto, c.Descripcion, c.EsGravado, c.FormaCalculo, c.FactorMultiplicador, c.EsIncidencia, c.EsBase, c.Activo";

        internal static CatConcepto LeerConcepto(DataRow f)
        {
            return new CatConcepto
            {
                ConceptoId = Fila.Obtener<int>(f, "ConceptoId"),
                ClaveSAT = Fila.Obtener<string>(f, "ClaveSAT"),
                TipoConcepto = Fila.Obtener<string>(f, "TipoConcepto"),
                Descripcion = Fila.Obtener<string>(f, "Descripcion"),
                EsGravado = Fila.Obtener<bool>(f, "EsGravado"),
                FormaCalculo = Fila.Obtener<string>(f, "FormaCalculo"),
                FactorMultiplicador = Fila.Obtener<decimal>(f, "FactorMultiplicador"),
                EsIncidencia = Fila.Obtener<bool>(f, "EsIncidencia"),
                EsBase = Fila.Obtener<bool>(f, "EsBase"),
                Activo = Fila.Obtener<bool>(f, "Activo")
            };
        }

        internal const string SelectConcepto = "SELECT " + ColumnasConcepto + " FROM dbo.CatConcepto c ";

        /// <summary>Conceptos SAT capturables como incidencia (percepción o deducción).</summary>
        public List<CatConcepto> ListarConceptos()
        {
            DataTable t = AccesoDatos.Consultar(
                SelectConcepto + "WHERE c.EsIncidencia = 1 AND c.TipoConcepto IN ('P','D') ORDER BY c.TipoConcepto, c.ClaveSAT");
            var lista = new List<CatConcepto>();
            foreach (DataRow f in t.Rows) lista.Add(LeerConcepto(f));
            return lista;
        }

        /// <summary>Catálogo SAT completo de percepciones y deducciones activas (sin los conceptos base del motor).</summary>
        public List<CatConcepto> ListarCatalogo()
        {
            DataTable t = AccesoDatos.Consultar(
                SelectConcepto + "WHERE c.Activo = 1 AND c.EsBase = 0 AND c.TipoConcepto IN ('P','D') ORDER BY c.TipoConcepto, c.ClaveSAT");
            var lista = new List<CatConcepto>();
            foreach (DataRow f in t.Rows) lista.Add(LeerConcepto(f));
            return lista;
        }

        /// <summary>Define la regla de cálculo de un concepto SAT y lo habilita para capturarse como incidencia.</summary>
        public void HabilitarConcepto(int conceptoId, string formaCalculo, decimal factor, bool esGravado)
        {
            int filas = AccesoDatos.EjecutarComando(
                "UPDATE dbo.CatConcepto SET FormaCalculo = @Forma, FactorMultiplicador = @Factor, EsGravado = @Gravado, EsIncidencia = 1 " +
                "WHERE ConceptoId = @Id AND Activo = 1 AND EsBase = 0 AND TipoConcepto IN ('P','D')",
                AccesoDatos.Parametro("@Forma", formaCalculo), AccesoDatos.ParametroDecimal("@Factor", factor, 10, 4),
                AccesoDatos.Parametro("@Gravado", esGravado), AccesoDatos.Parametro("@Id", conceptoId));
            if (filas != 1) throw new ExcepcionDatos("El concepto no existe, está inactivo o lo calcula el motor de nómina.", 50000, true, null);
        }

        public List<RelEmpleadoIncidencia> ListarAsignadas(int empleadoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT r.IdEmpleadoIncidencia, r.IdEmpleado, r.IdConceptoSAT, r.CantidadOValor, r.EsPermanente, r.Observaciones, r.Activo, " +
                "c.EsBase, c.ClaveSAT + ' - ' + c.Descripcion AS Concepto " +
                "FROM dbo.RelEmpleadoIncidencia r JOIN dbo.CatConcepto c ON c.ConceptoId = r.IdConceptoSAT " +
                "WHERE r.IdEmpleado = @Empleado ORDER BY c.EsBase DESC, r.IdEmpleadoIncidencia",
                AccesoDatos.Parametro("@Empleado", empleadoId));
            var lista = new List<RelEmpleadoIncidencia>();
            foreach (DataRow f in t.Rows)
                lista.Add(new RelEmpleadoIncidencia
                {
                    IdEmpleadoIncidencia = Fila.Obtener<int>(f, "IdEmpleadoIncidencia"),
                    IdEmpleado = empleadoId,
                    IdConceptoSAT = Fila.Obtener<int>(f, "IdConceptoSAT"),
                    Concepto = Fila.Obtener<string>(f, "Concepto"),
                    EsBase = Fila.Obtener<bool>(f, "EsBase"),
                    CantidadOValor = Fila.Obtener<decimal>(f, "CantidadOValor"),
                    EsPermanente = Fila.Obtener<bool>(f, "EsPermanente"),
                    Observaciones = Fila.Obtener<string>(f, "Observaciones"),
                    Activo = Fila.Obtener<bool>(f, "Activo")
                });
            return lista;
        }

        public List<ProcIncidenciaConcepto> ListarPeriodo(int periodoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT i.IdIncidencia, i.IdPeriodo, i.IdEmpleado, i.IdConceptoSAT, i.CantidadOValor, i.ImporteCalculado, i.EsPermanente, i.Observaciones, " +
                ColumnasConcepto + " FROM dbo.ProcIncidencias i JOIN dbo.CatConcepto c ON c.ConceptoId = i.IdConceptoSAT AND c.TipoConcepto <> 'I' " +
                "WHERE i.IdPeriodo = @Periodo AND NOT (c.TipoConcepto = 'P' AND c.ClaveSAT IN ('019','020','002','021','022','025')) ORDER BY i.IdIncidencia",
                AccesoDatos.Parametro("@Periodo", periodoId));
            var lista = new List<ProcIncidenciaConcepto>();
            foreach (DataRow f in t.Rows)
                lista.Add(new ProcIncidenciaConcepto
                {
                    IdIncidencia = Fila.Obtener<int>(f, "IdIncidencia"),
                    IdPeriodo = periodoId,
                    IdEmpleado = Fila.Obtener<int>(f, "IdEmpleado"),
                    IdConceptoSAT = Fila.Obtener<int>(f, "IdConceptoSAT"),
                    CantidadOValor = Fila.Obtener<decimal>(f, "CantidadOValor"),
                    ImporteCalculado = Fila.Obtener<decimal>(f, "ImporteCalculado"),
                    EsPermanente = Fila.Obtener<bool>(f, "EsPermanente"),
                    Observaciones = Fila.Obtener<string>(f, "Observaciones"),
                    Concepto = LeerConcepto(f)
                });
            return lista;
        }

        // El bloqueo del periodo serializa precarga, captura, eliminación y cierre.
        private static void ExigirPeriodoAbierto(SqlConnection cn, SqlTransaction tx, int periodoId)
        {
            object estatus = AccesoDatos.Escalar(cn, tx,
                "SELECT Estatus FROM dbo.ProcPeriodoNomina WITH (UPDLOCK,HOLDLOCK) WHERE PeriodoId = @Periodo",
                AccesoDatos.Parametro("@Periodo", periodoId));
            if (!Equals(estatus, "CREADO") && !Equals(estatus, "PRENOMINA"))
                throw new ExcepcionDatos("El periodo ya no admite cambios.", 50000, true, null);
        }

        public void Inicializar(int periodoId, Func<CatConcepto, decimal, decimal, decimal, decimal> calcular)
        {
            AccesoDatos.EjecutarEnTransaccion((cn, tx) =>
            {
                ExigirPeriodoAbierto(cn, tx, periodoId);
                bool inicializado = Convert.ToBoolean(AccesoDatos.Escalar(cn, tx,
                    "SELECT IncidenciasInicializadas FROM dbo.ProcPeriodoNomina WHERE PeriodoId = @Periodo",
                    AccesoDatos.Parametro("@Periodo", periodoId)));
                if (inicializado) return;

                DataTable existentes = AccesoDatos.Consultar(cn, tx,
                    "SELECT 1 FROM dbo.ProcIncidencias WHERE IdPeriodo = @Periodo AND FechaIncidencia IS NULL",
                    AccesoDatos.Parametro("@Periodo", periodoId));
                if (existentes.Rows.Count == 0)
                {
                    DataTable fijas = AccesoDatos.Consultar(cn, tx,
                        "EXEC dbo.sp_ListarIncidenciasFijasAplicables @PeriodoId = @Periodo",
                        AccesoDatos.Parametro("@Periodo", periodoId));
                    var horasPorEmpleado = new Dictionary<int, decimal>();
                    foreach (DataRow f in fijas.Rows)
                    {
                        CatConcepto c = LeerConcepto(f);
                        decimal cantidad = Fila.Obtener<decimal>(f, "CantidadOValor");
                        int empleadoId = Fila.Obtener<int>(f, "IdEmpleado");
                        decimal horas;
                        if (!horasPorEmpleado.TryGetValue(empleadoId, out horas))
                            horasPorEmpleado[empleadoId] = horas = HorasJornada(cn, tx, empleadoId);
                        Insertar(cn, tx, new ProcIncidenciaConcepto
                        {
                            IdPeriodo = periodoId, IdEmpleado = empleadoId,
                            IdConceptoSAT = c.ConceptoId, CantidadOValor = cantidad,
                            ImporteCalculado = calcular(c, cantidad, Fila.Obtener<decimal>(f, "SalarioDiario"), horas),
                            EsPermanente = true, Observaciones = Fila.Obtener<string>(f, "Observaciones")
                        });
                    }
                }
                AsignacionPaqueteDAL.AplicarPaquetes(cn, tx, periodoId, calcular);
                AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.ProcPeriodoNomina SET IncidenciasInicializadas = 1, " +
                    "Estatus = CASE WHEN EXISTS(SELECT 1 FROM dbo.ProcIncidencias WHERE IdPeriodo=@Periodo) THEN 'PRENOMINA' ELSE Estatus END " +
                    "WHERE PeriodoId = @Periodo",
                    AccesoDatos.Parametro("@Periodo", periodoId));
            });
        }

        private static void Insertar(SqlConnection cn, SqlTransaction tx, ProcIncidenciaConcepto i)
        {
            AccesoDatos.EjecutarComando(cn, tx,
                "INSERT dbo.ProcIncidencias(IdPeriodo,IdEmpleado,IdConceptoSAT,CantidadOValor,ImporteCalculado,EsPermanente,Observaciones) " +
                "VALUES(@Periodo,@Empleado,@Concepto,@Cantidad,@Importe,@Permanente,@Obs)",
                AccesoDatos.Parametro("@Periodo", i.IdPeriodo), AccesoDatos.Parametro("@Empleado", i.IdEmpleado),
                AccesoDatos.Parametro("@Concepto", i.IdConceptoSAT),
                AccesoDatos.ParametroDecimal("@Cantidad", i.CantidadOValor, 18, 4),
                AccesoDatos.ParametroDecimal("@Importe", i.ImporteCalculado, 18, 2),
                AccesoDatos.Parametro("@Permanente", i.EsPermanente), AccesoDatos.Parametro("@Obs", i.Observaciones));
        }

        private static decimal HorasJornada(SqlConnection cn, SqlTransaction tx, int empleadoId)
        {
            object horas = AccesoDatos.Escalar(cn, tx,
                "SELECT " + ConsultasJornada.HorasJornada("e") + " FROM dbo.CatEmpleado e WHERE e.EmpleadoId = @Empleado",
                AccesoDatos.Parametro("@Empleado", empleadoId));
            return horas == null ? 0m : Convert.ToDecimal(horas);
        }

        /// <summary>Sueldo diario base y horas de la jornada del trabajador; horas en 0 si no tiene turno ni jornada con horas.</summary>
        public BaseCalculoEmpleado ObtenerBaseCalculo(int empleadoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT d.SalarioDiario, " + ConsultasJornada.HorasJornada("e") + " AS HorasJornada " +
                "FROM dbo.CatEmpleado e JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId WHERE e.EmpleadoId = @Empleado",
                AccesoDatos.Parametro("@Empleado", empleadoId));
            if (t.Rows.Count != 1) return null;
            return new BaseCalculoEmpleado
            {
                SalarioDiario = Fila.Obtener<decimal>(t.Rows[0], "SalarioDiario"),
                HorasJornada = Fila.Obtener<decimal>(t.Rows[0], "HorasJornada")
            };
        }

        public void Registrar(ProcIncidenciaConcepto i, RelEmpleadoIncidencia permanente,
            Func<CatConcepto, decimal, decimal, decimal, decimal> calcular)
        {
            AccesoDatos.EjecutarEnTransaccion((cn, tx) =>
            {
                ExigirPeriodoAbierto(cn, tx, i.IdPeriodo);
                DataTable entrada = AccesoDatos.Consultar(cn, tx,
                    "SELECT d.SalarioDiario, " + ColumnasConcepto +
                    " FROM dbo.CatConcepto c " +
                    "JOIN dbo.CatEmpleado e ON e.EmpleadoId = @Empleado " +
                    "JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId " +
                    "JOIN dbo.ProcPeriodoNomina p ON p.PeriodoId = @Periodo " +
                    "WHERE c.ConceptoId = @Concepto AND c.Activo = 1 AND c.EsIncidencia = 1 AND e.Estatus = 'ACTIVO' " +
                    "AND e.EmpresaId = p.EmpresaId AND e.FechaIngreso <= p.FechaFin " +
                    "AND EXISTS(SELECT 1 FROM dbo.DocContratoEmpleado ec WHERE ec.EmpleadoId = e.EmpleadoId " +
                    "AND ec.Activo = 1 AND ec.PeriodicidadPagoId = p.PeriodicidadPagoId)",
                    AccesoDatos.Parametro("@Empleado", i.IdEmpleado), AccesoDatos.Parametro("@Periodo", i.IdPeriodo),
                    AccesoDatos.Parametro("@Concepto", i.IdConceptoSAT));
                if (entrada.Rows.Count != 1)
                    throw new ExcepcionDatos("El concepto o empleado ya no está activo para este periodo.", 50000, true, null);
                CatConcepto concepto = LeerConcepto(entrada.Rows[0]);
                i.ImporteCalculado = calcular(concepto, i.CantidadOValor,
                    Fila.Obtener<decimal>(entrada.Rows[0], "SalarioDiario"), HorasJornada(cn, tx, i.IdEmpleado));
                Insertar(cn, tx, i);
                if (permanente != null) GuardarAsignada(cn, tx, permanente);
                AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.ProcPeriodoNomina SET Estatus = 'PRENOMINA', IncidenciasInicializadas = 1 WHERE PeriodoId = @Periodo",
                    AccesoDatos.Parametro("@Periodo", i.IdPeriodo));
            });
        }

        /// <summary>Quita la incidencia del periodo y, si se indica, también de la configuración permanente del empleado.</summary>
        public void Quitar(int periodoId, int empleadoId, int incidenciaId, bool tambienPermanente)
        {
            AccesoDatos.EjecutarEnTransaccion((cn, tx) =>
            {
                ExigirPeriodoAbierto(cn, tx, periodoId);
                object conceptoId = AccesoDatos.Escalar(cn, tx,
                    "SELECT IdConceptoSAT FROM dbo.ProcIncidencias WHERE IdIncidencia=@Id AND IdPeriodo=@Periodo AND IdEmpleado=@Empleado AND FechaIncidencia IS NULL",
                    AccesoDatos.Parametro("@Id", incidenciaId), AccesoDatos.Parametro("@Periodo", periodoId),
                    AccesoDatos.Parametro("@Empleado", empleadoId));
                int filas = AccesoDatos.EjecutarComando(cn, tx,
                    "DELETE dbo.ProcIncidencias WHERE IdIncidencia = @Id AND IdPeriodo = @Periodo AND IdEmpleado = @Empleado AND FechaIncidencia IS NULL",
                    AccesoDatos.Parametro("@Id", incidenciaId), AccesoDatos.Parametro("@Periodo", periodoId),
                    AccesoDatos.Parametro("@Empleado", empleadoId));
                if (filas != 1) throw new ExcepcionDatos("La incidencia ya no pertenece a este trabajador y periodo.", 50000, true, null);
                if (tambienPermanente && conceptoId != null && conceptoId != DBNull.Value)
                    AccesoDatos.EjecutarComando(cn, tx,
                        "UPDATE dbo.RelEmpleadoIncidencia SET Activo = 0 WHERE IdEmpleado = @Empleado AND IdConceptoSAT = @Concepto",
                        AccesoDatos.Parametro("@Empleado", empleadoId), AccesoDatos.Parametro("@Concepto", conceptoId));
            });
        }

        public void GuardarAsignada(RelEmpleadoIncidencia r)
        {
            AccesoDatos.EjecutarEnTransaccion((cn, tx) => GuardarAsignada(cn, tx, r));
        }

        // Upsert por (empleado, concepto): una asignación individual por concepto SAT.
        private static void GuardarAsignada(SqlConnection cn, SqlTransaction tx, RelEmpleadoIncidencia r)
        {
            AccesoDatos.EjecutarComando(cn, tx,
                "MERGE dbo.RelEmpleadoIncidencia WITH (HOLDLOCK) AS t " +
                "USING (SELECT @Empleado AS IdEmpleado, @Concepto AS IdConceptoSAT) AS s " +
                "ON t.IdEmpleado = s.IdEmpleado AND t.IdConceptoSAT = s.IdConceptoSAT " +
                "WHEN MATCHED THEN UPDATE SET CantidadOValor=@Cantidad, EsPermanente=@Permanente, Observaciones=@Obs, Activo=@Activo " +
                "WHEN NOT MATCHED THEN INSERT(IdEmpleado,IdConceptoSAT,CantidadOValor,EsPermanente,Observaciones,Activo) " +
                "VALUES(@Empleado,@Concepto,@Cantidad,@Permanente,@Obs,@Activo);",
                AccesoDatos.Parametro("@Empleado", r.IdEmpleado), AccesoDatos.Parametro("@Concepto", r.IdConceptoSAT),
                AccesoDatos.ParametroDecimal("@Cantidad", r.CantidadOValor, 18, 4),
                AccesoDatos.Parametro("@Permanente", r.EsPermanente),
                AccesoDatos.Parametro("@Obs", r.Observaciones), AccesoDatos.Parametro("@Activo", r.Activo));
        }

        /// <summary>
        /// Precarga el paquete por defecto de la periodicidad del empleado al darlo de alta. Si coinciden varios paquetes
        /// (general, de la periodicidad, de la empresa) prevalece el más específico para cada concepto.
        /// </summary>
        public void AsignarPaqueteDefault(int empleadoId, int empresaId, int periodicidadPagoId)
        {
            AccesoDatos.EjecutarComando(
                "WITH candidatos AS (" +
                "SELECT rp.IdConceptoSAT, rp.CantidadOValor, rp.EsPermanente, " +
                "ROW_NUMBER() OVER (PARTITION BY rp.IdConceptoSAT ORDER BY " +
                "CASE WHEN p.EmpresaId IS NOT NULL THEN 0 ELSE 1 END, CASE WHEN p.PeriodicidadPagoId IS NOT NULL THEN 0 ELSE 1 END, p.IdPaquete) AS Orden " +
                "FROM dbo.CatPaqueteIncidencia p " +
                "JOIN dbo.RelPaqueteConcepto rp ON rp.IdPaquete = p.IdPaquete AND rp.Activo = 1 " +
                "JOIN dbo.CatConcepto c ON c.ConceptoId = rp.IdConceptoSAT AND c.Activo = 1 AND (c.EsIncidencia = 1 OR c.EsBase = 1) " +
                "WHERE p.EsDefault = 1 AND p.Activo = 1 " +
                "AND (p.PeriodicidadPagoId = @Periodicidad OR p.PeriodicidadPagoId IS NULL) " +
                "AND (p.EmpresaId = @Empresa OR p.EmpresaId IS NULL)) " +
                "INSERT dbo.RelEmpleadoIncidencia(IdEmpleado,IdConceptoSAT,CantidadOValor,EsPermanente,Observaciones,Activo) " +
                "SELECT @Empleado, k.IdConceptoSAT, k.CantidadOValor, k.EsPermanente, NULL, 1 FROM candidatos k WHERE k.Orden = 1 " +
                "AND NOT EXISTS (SELECT 1 FROM dbo.RelEmpleadoIncidencia r WHERE r.IdEmpleado = @Empleado AND r.IdConceptoSAT = k.IdConceptoSAT)",
                AccesoDatos.Parametro("@Empleado", empleadoId),
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Periodicidad", periodicidadPagoId));
        }
    }
}
