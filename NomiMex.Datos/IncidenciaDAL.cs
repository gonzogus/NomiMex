using System;
using System.Collections.Generic;
using System.Data;
using NomiMex.Entidades.Procesos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>
    /// Acceso a las incidencias legales (faltas, retardos, horas extra...) del periodo. Viven en dbo.ProcIncidencias como
    /// movimientos de un concepto TipoConcepto 'I' de dbo.CatConcepto; la unidad de captura sale de FormaCalculo del concepto.
    /// </summary>
    public class IncidenciaDAL
    {
        private const string ColumnasListado =
            "SELECT i.IdIncidencia AS IncidenciaId, i.IdEmpleado AS EmpleadoId, e.NumeroEmpleado, " +
            "CONCAT(e.ApellidoPaterno, ' ', ISNULL(e.ApellidoMaterno + ' ', ''), e.Nombres) AS NombreEmpleado, " +
            "t.ClaveSAT AS Codigo, t.Descripcion AS TipoIncidencia, i.CantidadOValor AS Cantidad, " +
            "CASE t.FormaCalculo WHEN 'HORAS' THEN 'HORA' WHEN 'MONTO_FIJO' THEN 'MONTO' ELSE 'DIA' END AS Unidad, " +
            "i.FechaIncidencia, i.Estatus, i.Observaciones " +
            "FROM dbo.ProcIncidencias i " +
            "INNER JOIN dbo.CatEmpleado e ON e.EmpleadoId = i.IdEmpleado " +
            "INNER JOIN dbo.CatConcepto t ON t.ConceptoId = i.IdConceptoSAT AND t.TipoConcepto = 'I' ";

        public List<IncidenciaListado> ListarPorPeriodo(int periodoId)
        {
            DataTable tabla = AccesoDatos.Consultar(
                ColumnasListado +
                "WHERE i.IdPeriodo = @Periodo ORDER BY i.FechaIncidencia DESC, i.IdIncidencia DESC",
                AccesoDatos.Parametro("@Periodo", periodoId));

            List<IncidenciaListado> lista = new List<IncidenciaListado>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(Leer(fila));
            }
            return lista;
        }

        /// <summary>Incidencias activas de un trabajador en el periodo (las que intervienen en su pre-recibo).</summary>
        public List<IncidenciaListado> ListarActivasPorEmpleado(int periodoId, int empleadoId)
        {
            DataTable tabla = AccesoDatos.Consultar(
                ColumnasListado +
                "WHERE i.IdPeriodo = @Periodo AND i.IdEmpleado = @Empleado AND i.Estatus = 'ACTIVA' " +
                "ORDER BY i.FechaIncidencia, i.IdIncidencia",
                AccesoDatos.Parametro("@Periodo", periodoId),
                AccesoDatos.Parametro("@Empleado", empleadoId));

            List<IncidenciaListado> lista = new List<IncidenciaListado>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(Leer(fila));
            }
            return lista;
        }

        private static IncidenciaListado Leer(DataRow fila)
        {
            return new IncidenciaListado
            {
                IncidenciaId = Fila.Obtener<int>(fila, "IncidenciaId"),
                EmpleadoId = Fila.Obtener<int>(fila, "EmpleadoId"),
                NumeroEmpleado = Fila.Obtener<string>(fila, "NumeroEmpleado"),
                NombreEmpleado = Fila.Obtener<string>(fila, "NombreEmpleado"),
                TipoIncidencia = Fila.Obtener<string>(fila, "TipoIncidencia"),
                Codigo = Fila.Obtener<string>(fila, "Codigo"),
                Cantidad = Fila.Obtener<decimal>(fila, "Cantidad"),
                Unidad = Fila.Obtener<string>(fila, "Unidad"),
                FechaIncidencia = Fila.Obtener<DateTime>(fila, "FechaIncidencia"),
                Estatus = Fila.Obtener<string>(fila, "Estatus"),
                Observaciones = Fila.Obtener<string>(fila, "Observaciones")
            };
        }

        public int Insertar(ProcIncidencia i)
        {
            // El importe de una incidencia legal lo calcula el motor de nómina: se guarda en cero.
            object id = AccesoDatos.Escalar(
                "INSERT INTO dbo.ProcIncidencias (IdEmpleado, IdPeriodo, IdConceptoSAT, CantidadOValor, ImporteCalculado, FechaIncidencia, Estatus, Observaciones) " +
                "VALUES (@Empleado, @Periodo, @Tipo, @Cantidad, 0, @Fecha, 'ACTIVA', @Obs); SELECT CAST(SCOPE_IDENTITY() AS INT);",
                AccesoDatos.Parametro("@Empleado", i.EmpleadoId),
                AccesoDatos.ParametroEntero("@Periodo", i.PeriodoId),
                AccesoDatos.Parametro("@Tipo", i.TipoIncidenciaId),
                AccesoDatos.ParametroDecimal("@Cantidad", i.Cantidad, 18, 4),
                AccesoDatos.Parametro("@Fecha", i.FechaIncidencia.Date),
                AccesoDatos.Parametro("@Obs", i.Observaciones));
            return Convert.ToInt32(id);
        }

        /// <summary>Cancela (baja lógica) una incidencia activa. Devuelve false si no estaba activa.</summary>
        public bool Cancelar(int incidenciaId)
        {
            int filas = AccesoDatos.EjecutarComando(
                "UPDATE dbo.ProcIncidencias SET Estatus = 'CANCELADA' WHERE IdIncidencia = @Id AND Estatus = 'ACTIVA' AND FechaIncidencia IS NOT NULL",
                AccesoDatos.Parametro("@Id", incidenciaId));
            return filas > 0;
        }

        public void CancelarGrupo(int periodoId, int empleadoId, IList<int> ids)
        {
            AccesoDatos.EjecutarEnTransaccion((cn, tx) =>
            {
                object estatus = AccesoDatos.Escalar(cn, tx,
                    "SELECT Estatus FROM dbo.ProcPeriodoNomina WITH(UPDLOCK,HOLDLOCK) WHERE PeriodoId=@Periodo",
                    AccesoDatos.Parametro("@Periodo", periodoId));
                if (!Equals(estatus, "CREADO") && !Equals(estatus, "PRENOMINA"))
                    throw new ExcepcionDatos("El periodo ya no admite cambios.", 50000, true, null);
                foreach (int id in ids)
                {
                    int filas = AccesoDatos.EjecutarComando(cn, tx,
                        "UPDATE dbo.ProcIncidencias SET Estatus='CANCELADA' WHERE IdIncidencia=@Id AND IdPeriodo=@Periodo " +
                        "AND IdEmpleado=@Empleado AND Estatus='ACTIVA' AND FechaIncidencia IS NOT NULL",
                        AccesoDatos.Parametro("@Id", id), AccesoDatos.Parametro("@Periodo", periodoId),
                        AccesoDatos.Parametro("@Empleado", empleadoId));
                    if (filas != 1) throw new ExcepcionDatos("Una incidencia ya no pertenece al trabajador y periodo.", 50000, true, null);
                }
            });
        }
    }
}
