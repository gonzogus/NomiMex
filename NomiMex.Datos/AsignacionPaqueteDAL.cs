using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Relaciones;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>
    /// Acceso a dbo.RelEmpresaPeriodoPaquete: paquetes de incidencias asignados a una empresa en un periodo exacto. Los conceptos
    /// de cada paquete asignado se aplican a los trabajadores del periodo (dbo.ProcIncidencias.IdPaquete guarda su origen).
    /// </summary>
    public class AsignacionPaqueteDAL
    {
        // Un paquete cumple la política de la empresa en el periodo si está activo, es general o de esa empresa y
        // es de cualquier periodicidad o de la del periodo. @Periodo identifica al periodo (alias pn).
        private const string CondicionPolitica =
            "p.Activo = 1 AND (p.EmpresaId IS NULL OR p.EmpresaId = pn.EmpresaId) " +
            "AND (p.PeriodicidadPagoId IS NULL OR p.PeriodicidadPagoId = pn.PeriodicidadPagoId)";

        /// <summary>
        /// Paquetes que la política de la empresa permite en el periodo, más los ya asignados aunque hayan dejado de cumplirla
        /// (para poder retirarlos). Devuelve vacío si el periodo no pertenece a la empresa.
        /// </summary>
        public List<PaqueteAsignacion> ListarPaquetes(int empresaId, int periodoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT p.IdPaquete, p.Nombre, p.EmpresaId, p.PeriodicidadPagoId, ISNULL(pp.Descripcion, 'Todas') AS Periodicidad, " +
                "p.EsDefault, p.Activo, " +
                "(SELECT COUNT(1) FROM dbo.RelPaqueteConcepto rp INNER JOIN dbo.CatConcepto c ON c.ConceptoId = rp.IdConceptoSAT " +
                " WHERE rp.IdPaquete = p.IdPaquete AND rp.Activo = 1 AND c.EsBase = 0) AS Total, " +
                "CASE WHEN rep.IdPaquete IS NULL THEN 0 ELSE 1 END AS Asignado " +
                "FROM dbo.ProcPeriodoNomina pn " +
                "CROSS JOIN dbo.CatPaqueteIncidencia p " +
                "LEFT JOIN dbo.RelEmpresaPeriodoPaquete rep ON rep.PeriodoId = pn.PeriodoId AND rep.EmpresaId = pn.EmpresaId AND rep.IdPaquete = p.IdPaquete " +
                "LEFT JOIN dbo.CatPeriodicidadPago pp ON pp.PeriodicidadPagoId = p.PeriodicidadPagoId " +
                "WHERE pn.PeriodoId = @Periodo AND pn.EmpresaId = @Empresa AND ((" + CondicionPolitica + ") OR rep.IdPaquete IS NOT NULL) " +
                "ORDER BY p.Nombre",
                AccesoDatos.Parametro("@Periodo", periodoId),
                AccesoDatos.Parametro("@Empresa", empresaId));

            List<PaqueteAsignacion> lista = new List<PaqueteAsignacion>();
            foreach (DataRow f in t.Rows)
            {
                lista.Add(new PaqueteAsignacion
                {
                    IdPaquete = Fila.Obtener<int>(f, "IdPaquete"),
                    Nombre = Fila.Obtener<string>(f, "Nombre"),
                    EmpresaId = Fila.Obtener<int?>(f, "EmpresaId"),
                    PeriodicidadPagoId = Fila.Obtener<int?>(f, "PeriodicidadPagoId"),
                    Periodicidad = Fila.Obtener<string>(f, "Periodicidad"),
                    EsDefault = Fila.Obtener<bool>(f, "EsDefault"),
                    Activo = Fila.Obtener<bool>(f, "Activo"),
                    TotalConceptos = Fila.Obtener<int>(f, "Total"),
                    Asignado = Fila.Obtener<int>(f, "Asignado") == 1
                });
            }
            return lista;
        }

        /// <summary>Asignaciones de paquetes de la empresa en el periodo.</summary>
        public List<RelEmpresaPeriodoPaquete> ListarAsignaciones(int empresaId, int periodoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT EmpresaId, PeriodoId, IdPaquete, FechaAsignacion, UsuarioAsignacion FROM dbo.RelEmpresaPeriodoPaquete " +
                "WHERE EmpresaId = @Empresa AND PeriodoId = @Periodo ORDER BY IdPaquete",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Periodo", periodoId));

            List<RelEmpresaPeriodoPaquete> lista = new List<RelEmpresaPeriodoPaquete>();
            foreach (DataRow f in t.Rows)
            {
                lista.Add(new RelEmpresaPeriodoPaquete
                {
                    EmpresaId = Fila.Obtener<int>(f, "EmpresaId"),
                    PeriodoId = Fila.Obtener<int>(f, "PeriodoId"),
                    IdPaquete = Fila.Obtener<int>(f, "IdPaquete"),
                    FechaAsignacion = Fila.Obtener<DateTime>(f, "FechaAsignacion"),
                    UsuarioAsignacion = Fila.Obtener<string>(f, "UsuarioAsignacion")
                });
            }
            return lista;
        }

        /// <summary>
        /// Conceptos de los paquetes asignados a la empresa en el periodo (incluidos los base y legales), más los que la empresa
        /// ya omite aunque su paquete se haya quitado después. Cada uno indica si está omitido y por qué.
        /// </summary>
        public List<ConceptoPeriodoPaquete> ListarConceptosPeriodo(int empresaId, int periodoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT c.ConceptoId, c.ClaveSAT, c.TipoConcepto, c.Descripcion, c.EsBase, " +
                "CAST(CASE WHEN c.EsBase = 1 OR c.TipoConcepto = 'I' THEN 1 ELSE 0 END AS BIT) AS EsLegal, " +
                "ISNULL(STUFF((SELECT DISTINCT ', ' + pq.Nombre FROM dbo.RelEmpresaPeriodoPaquete r2 " +
                "  INNER JOIN dbo.RelPaqueteConcepto pc2 ON pc2.IdPaquete = r2.IdPaquete AND pc2.Activo = 1 " +
                "  INNER JOIN dbo.CatPaqueteIncidencia pq ON pq.IdPaquete = r2.IdPaquete " +
                "  WHERE r2.EmpresaId = @Empresa AND r2.PeriodoId = @Periodo AND pc2.IdConceptoSAT = c.ConceptoId " +
                "  FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), '') AS Paquetes, " +
                "CASE WHEN o.ConceptoId IS NULL THEN 0 ELSE 1 END AS Omitido, o.Motivo " +
                "FROM dbo.CatConcepto c " +
                "LEFT JOIN dbo.RelEmpresaPeriodoConceptoOmitido o ON o.EmpresaId = @Empresa AND o.PeriodoId = @Periodo AND o.ConceptoId = c.ConceptoId " +
                "WHERE o.ConceptoId IS NOT NULL OR EXISTS (SELECT 1 FROM dbo.RelEmpresaPeriodoPaquete r " +
                "  INNER JOIN dbo.RelPaqueteConcepto pc ON pc.IdPaquete = r.IdPaquete AND pc.Activo = 1 " +
                "  WHERE r.EmpresaId = @Empresa AND r.PeriodoId = @Periodo AND pc.IdConceptoSAT = c.ConceptoId) " +
                "ORDER BY CASE WHEN c.EsBase = 1 OR c.TipoConcepto = 'I' THEN 0 ELSE 1 END, c.TipoConcepto, c.ClaveSAT",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Periodo", periodoId));
            return LeerConceptosPeriodo(t);
        }

        /// <summary>Conceptos que la empresa omite en el periodo (para aplicar las omisiones al cálculo y al cierre).</summary>
        public List<ConceptoPeriodoPaquete> ListarOmitidos(int periodoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT c.ConceptoId, c.ClaveSAT, c.TipoConcepto, c.Descripcion, c.EsBase, CAST(o.EsLegal AS BIT) AS EsLegal, " +
                "CAST('' AS NVARCHAR(10)) AS Paquetes, 1 AS Omitido, o.Motivo " +
                "FROM dbo.RelEmpresaPeriodoConceptoOmitido o INNER JOIN dbo.CatConcepto c ON c.ConceptoId = o.ConceptoId " +
                "WHERE o.PeriodoId = @Periodo ORDER BY c.TipoConcepto, c.ClaveSAT",
                AccesoDatos.Parametro("@Periodo", periodoId));
            return LeerConceptosPeriodo(t);
        }

        private static List<ConceptoPeriodoPaquete> LeerConceptosPeriodo(DataTable t)
        {
            List<ConceptoPeriodoPaquete> lista = new List<ConceptoPeriodoPaquete>();
            foreach (DataRow f in t.Rows)
            {
                lista.Add(new ConceptoPeriodoPaquete
                {
                    ConceptoId = Fila.Obtener<int>(f, "ConceptoId"),
                    ClaveSAT = Fila.Obtener<string>(f, "ClaveSAT"),
                    TipoConcepto = Fila.Obtener<string>(f, "TipoConcepto"),
                    Descripcion = Fila.Obtener<string>(f, "Descripcion"),
                    EsBase = Fila.Obtener<bool>(f, "EsBase"),
                    EsLegal = Fila.Obtener<bool>(f, "EsLegal"),
                    Paquetes = Fila.Obtener<string>(f, "Paquetes"),
                    Omitido = Fila.Obtener<int>(f, "Omitido") == 1,
                    Motivo = Fila.Obtener<string>(f, "Motivo")
                });
            }
            return lista;
        }

        /// <summary>Conceptos del catálogo con los identificadores indicados (para validar las omisiones).</summary>
        public List<CatConcepto> ListarConceptos(IEnumerable<int> conceptosIds)
        {
            List<CatConcepto> lista = new List<CatConcepto>();
            foreach (int id in conceptosIds.Distinct())
            {
                DataTable t = AccesoDatos.Consultar(ConceptoIncidenciaDAL.SelectConcepto + "WHERE c.ConceptoId = @Id",
                    AccesoDatos.Parametro("@Id", id));
                foreach (DataRow f in t.Rows)
                {
                    lista.Add(ConceptoIncidenciaDAL.LeerConcepto(f));
                }
            }
            return lista;
        }

        /// <summary>
        /// Reemplaza en una sola transacción los paquetes de la empresa en el periodo y los conceptos que omite: toma el periodo
        /// (debe pertenecer a la empresa y estar abierto), revalida cada paquete contra la política, agrega y retira asignaciones
        /// y omisiones (un concepto legal omitido activa la bandera que impide timbrar) y, si el periodo ya inició la captura de
        /// incidencias, aplica los conceptos de los paquetes nuevos y retira los de los paquetes quitados o los omitidos.
        /// </summary>
        public void Guardar(int empresaId, int periodoId, IList<int> paquetesIds, IList<RelEmpresaPeriodoConceptoOmitido> omisiones,
                            Func<CatConcepto, decimal, decimal, decimal, decimal> calcular)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                DataTable periodo = AccesoDatos.Consultar(cn, tx,
                    "SELECT Estatus, IncidenciasInicializadas FROM dbo.ProcPeriodoNomina WITH (UPDLOCK, HOLDLOCK) " +
                    "WHERE PeriodoId = @Periodo AND EmpresaId = @Empresa",
                    AccesoDatos.Parametro("@Periodo", periodoId),
                    AccesoDatos.Parametro("@Empresa", empresaId));
                if (periodo.Rows.Count == 0)
                {
                    throw new ExcepcionDatos("El periodo no pertenece a la empresa seleccionada.", 50000, true, null);
                }
                string estatus = Fila.Obtener<string>(periodo.Rows[0], "Estatus");
                if (estatus != "CREADO" && estatus != "PRENOMINA")
                {
                    throw new ExcepcionDatos("El periodo está " + estatus + " y ya no admite cambios de paquetes.", 50000, true, null);
                }
                bool inicializado = Fila.Obtener<bool>(periodo.Rows[0], "IncidenciasInicializadas");

                foreach (int id in paquetesIds)
                {
                    object cumple = AccesoDatos.Escalar(cn, tx,
                        "SELECT COUNT(1) FROM dbo.ProcPeriodoNomina pn INNER JOIN dbo.CatPaqueteIncidencia p ON p.IdPaquete = @Paquete " +
                        "WHERE pn.PeriodoId = @Periodo AND " + CondicionPolitica,
                        AccesoDatos.Parametro("@Paquete", id),
                        AccesoDatos.Parametro("@Periodo", periodoId));
                    if (Convert.ToInt32(cumple) == 0)
                    {
                        throw new ExcepcionDatos("El paquete " + id + " no cumple la política de la empresa para este periodo " +
                            "(debe estar activo, ser general o de la empresa y de la periodicidad del periodo).", 50000, true, null);
                    }
                }

                DataTable actuales = AccesoDatos.Consultar(cn, tx,
                    "SELECT IdPaquete FROM dbo.RelEmpresaPeriodoPaquete WHERE EmpresaId = @Empresa AND PeriodoId = @Periodo",
                    AccesoDatos.Parametro("@Empresa", empresaId),
                    AccesoDatos.Parametro("@Periodo", periodoId));
                HashSet<int> anteriores = new HashSet<int>(actuales.Rows.Cast<DataRow>().Select(f => Fila.Obtener<int>(f, "IdPaquete")));
                HashSet<int> nuevos = new HashSet<int>(paquetesIds);

                foreach (int quitado in anteriores.Where(a => !nuevos.Contains(a)))
                {
                    AccesoDatos.EjecutarComando(cn, tx,
                        "DELETE FROM dbo.ProcIncidencias WHERE IdPeriodo = @Periodo AND IdPaquete = @Paquete AND FechaIncidencia IS NULL",
                        AccesoDatos.Parametro("@Periodo", periodoId),
                        AccesoDatos.Parametro("@Paquete", quitado));
                    AccesoDatos.EjecutarComando(cn, tx,
                        "DELETE FROM dbo.RelEmpresaPeriodoPaquete WHERE EmpresaId = @Empresa AND PeriodoId = @Periodo AND IdPaquete = @Paquete",
                        AccesoDatos.Parametro("@Empresa", empresaId),
                        AccesoDatos.Parametro("@Periodo", periodoId),
                        AccesoDatos.Parametro("@Paquete", quitado));
                }

                foreach (int agregado in nuevos.Where(n => !anteriores.Contains(n)))
                {
                    AccesoDatos.EjecutarComando(cn, tx,
                        "INSERT INTO dbo.RelEmpresaPeriodoPaquete (EmpresaId, PeriodoId, IdPaquete) VALUES (@Empresa, @Periodo, @Paquete)",
                        AccesoDatos.Parametro("@Empresa", empresaId),
                        AccesoDatos.Parametro("@Periodo", periodoId),
                        AccesoDatos.Parametro("@Paquete", agregado));
                }

                // Omisiones: se reemplazan; la de un concepto que ya no está en los paquetes asignados se descarta. La bandera
                // BloqueaTimbrado queda activa en los conceptos legales (base o incidencias legales).
                AccesoDatos.EjecutarComando(cn, tx,
                    "DELETE FROM dbo.RelEmpresaPeriodoConceptoOmitido WHERE EmpresaId = @Empresa AND PeriodoId = @Periodo",
                    AccesoDatos.Parametro("@Empresa", empresaId),
                    AccesoDatos.Parametro("@Periodo", periodoId));
                foreach (RelEmpresaPeriodoConceptoOmitido omision in omisiones)
                {
                    AccesoDatos.EjecutarComando(cn, tx,
                        "INSERT INTO dbo.RelEmpresaPeriodoConceptoOmitido (EmpresaId, PeriodoId, ConceptoId, EsLegal, BloqueaTimbrado, Motivo) " +
                        "SELECT @Empresa, @Periodo, c.ConceptoId, x.Legal, x.Legal, @Motivo FROM dbo.CatConcepto c " +
                        "CROSS APPLY (SELECT CAST(CASE WHEN c.EsBase = 1 OR c.TipoConcepto = 'I' THEN 1 ELSE 0 END AS BIT) AS Legal) x " +
                        "WHERE c.ConceptoId = @Concepto AND c.Activo = 1 AND EXISTS (SELECT 1 FROM dbo.RelEmpresaPeriodoPaquete r " +
                        "  INNER JOIN dbo.RelPaqueteConcepto pc ON pc.IdPaquete = r.IdPaquete AND pc.Activo = 1 " +
                        "  WHERE r.EmpresaId = @Empresa AND r.PeriodoId = @Periodo AND pc.IdConceptoSAT = c.ConceptoId)",
                        AccesoDatos.Parametro("@Empresa", empresaId),
                        AccesoDatos.Parametro("@Periodo", periodoId),
                        AccesoDatos.Parametro("@Concepto", omision.ConceptoId),
                        AccesoDatos.Parametro("@Motivo", string.IsNullOrWhiteSpace(omision.Motivo) ? null : omision.Motivo.Trim()));
                }
                AccesoDatos.EjecutarComando(cn, tx,
                    "DELETE i FROM dbo.ProcIncidencias i INNER JOIN dbo.RelEmpresaPeriodoConceptoOmitido o " +
                    "ON o.PeriodoId = i.IdPeriodo AND o.ConceptoId = i.IdConceptoSAT " +
                    "WHERE i.IdPeriodo = @Periodo AND o.EmpresaId = @Empresa AND i.IdPaquete IS NOT NULL AND i.FechaIncidencia IS NULL",
                    AccesoDatos.Parametro("@Empresa", empresaId),
                    AccesoDatos.Parametro("@Periodo", periodoId));

                if (inicializado)
                {
                    AplicarPaquetes(cn, tx, periodoId, calcular);
                }
            });
        }

        /// <summary>
        /// Aplica a los trabajadores elegibles del periodo los conceptos (no base) de los paquetes asignados. Es idempotente:
        /// omite el concepto que el trabajador ya tiene activo en el periodo, sea cual sea su origen.
        /// </summary>
        internal static void AplicarPaquetes(SqlConnection cn, SqlTransaction tx, int periodoId,
                                             Func<CatConcepto, decimal, decimal, decimal, decimal> calcular)
        {
            DataTable filas = AccesoDatos.Consultar(cn, tx,
                "SELECT e.EmpleadoId, d.SalarioDiario, " + ConsultasJornada.HorasJornada("e") + " AS HorasJornada, " +
                "pc.IdPaquete, pq.Nombre AS Paquete, pc.CantidadOValor, " + ConceptoIncidenciaDAL.ColumnasConcepto + " " +
                "FROM dbo.RelEmpresaPeriodoPaquete rep " +
                "INNER JOIN dbo.ProcPeriodoNomina pn ON pn.PeriodoId = rep.PeriodoId AND pn.EmpresaId = rep.EmpresaId " +
                "INNER JOIN dbo.CatPaqueteIncidencia pq ON pq.IdPaquete = rep.IdPaquete " +
                "INNER JOIN dbo.RelPaqueteConcepto pc ON pc.IdPaquete = rep.IdPaquete AND pc.Activo = 1 AND pc.CantidadOValor > 0 " +
                "INNER JOIN dbo.CatConcepto c ON c.ConceptoId = pc.IdConceptoSAT AND c.Activo = 1 AND c.EsIncidencia = 1 AND c.EsBase = 0 " +
                "INNER JOIN dbo.CatEmpleado e ON e.EmpresaId = pn.EmpresaId AND e.Estatus = 'ACTIVO' AND e.FechaIngreso <= pn.FechaFin " +
                "INNER JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId " +
                "WHERE rep.PeriodoId = @Periodo " +
                "AND EXISTS (SELECT 1 FROM dbo.DocContratoEmpleado ec WHERE ec.EmpleadoId = e.EmpleadoId AND ec.Activo = 1 " +
                "            AND ec.PeriodicidadPagoId = pn.PeriodicidadPagoId) " +
                "AND NOT EXISTS (SELECT 1 FROM dbo.RelEmpresaPeriodoConceptoOmitido o WHERE o.PeriodoId = pn.PeriodoId " +
                "                AND o.EmpresaId = pn.EmpresaId AND o.ConceptoId = c.ConceptoId) " +
                "AND NOT EXISTS (SELECT 1 FROM dbo.ProcIncidencias i WHERE i.IdPeriodo = pn.PeriodoId AND i.IdEmpleado = e.EmpleadoId " +
                "                AND i.IdConceptoSAT = c.ConceptoId AND i.Estatus = 'ACTIVA') " +
                "ORDER BY e.EmpleadoId, pc.IdPaquete",
                AccesoDatos.Parametro("@Periodo", periodoId));

            HashSet<string> aplicados = new HashSet<string>();
            foreach (DataRow f in filas.Rows)
            {
                CatConcepto concepto = ConceptoIncidenciaDAL.LeerConcepto(f);
                int empleadoId = Fila.Obtener<int>(f, "EmpleadoId");
                if (!aplicados.Add(empleadoId + "|" + concepto.ConceptoId))
                {
                    continue;
                }

                decimal cantidad = Fila.Obtener<decimal>(f, "CantidadOValor");
                decimal importe = calcular(concepto, cantidad, Fila.Obtener<decimal>(f, "SalarioDiario"), Fila.Obtener<decimal>(f, "HorasJornada"));
                if (importe <= 0m)
                {
                    continue;
                }

                AccesoDatos.EjecutarComando(cn, tx,
                    "INSERT dbo.ProcIncidencias(IdPeriodo, IdEmpleado, IdConceptoSAT, CantidadOValor, ImporteCalculado, EsPermanente, Observaciones, IdPaquete) " +
                    "VALUES (@Periodo, @Empleado, @Concepto, @Cantidad, @Importe, 0, @Obs, @Paquete)",
                    AccesoDatos.Parametro("@Periodo", periodoId),
                    AccesoDatos.Parametro("@Empleado", empleadoId),
                    AccesoDatos.Parametro("@Concepto", concepto.ConceptoId),
                    AccesoDatos.ParametroDecimal("@Cantidad", cantidad, 18, 4),
                    AccesoDatos.ParametroDecimal("@Importe", importe, 18, 2),
                    AccesoDatos.Parametro("@Obs", Truncar("Paquete: " + Fila.Obtener<string>(f, "Paquete"), 300)),
                    AccesoDatos.Parametro("@Paquete", Fila.Obtener<int>(f, "IdPaquete")));
            }
        }

        private static string Truncar(string texto, int maximo)
        {
            return texto.Length <= maximo ? texto : texto.Substring(0, maximo);
        }
    }
}
