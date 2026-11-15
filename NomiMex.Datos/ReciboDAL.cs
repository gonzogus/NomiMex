using System;
using System.Data;
using NomiMex.Entidades.Documentos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>Consulta de documentos dbo.DocReciboNomina y dbo.DocReciboDetalle.</summary>
    public class ReciboDAL
    {
        public ReciboImpresion ObtenerRecibo(int reciboId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT r.*, emp.RazonSocial AS EmpresaNombre, emp.RFC AS EmpresaRFC, emp.RegistroPatronal, emp.Logo AS EmpresaLogo, e.NumeroEmpleado, " +
                "CONCAT(e.Nombres, ' ', e.ApellidoPaterno, ISNULL(' ' + e.ApellidoMaterno, '')) AS NombreEmpleado, " +
                "f.RFC AS EmpleadoRFC, f.CURP AS EmpleadoCURP, i.NSS AS EmpleadoNSS, pn.NumeroPeriodo, tp.Descripcion AS TipoPeriodo, " +
                "pn.FechaInicio AS FechaInicioPeriodo, pn.FechaFin AS FechaFinPeriodo, pn.FechaPago AS FechaPagoPeriodo, " +
                "pn.Estatus AS EstatusPeriodo " +
                "FROM dbo.DocReciboNomina r " +
                "INNER JOIN dbo.ProcPeriodoNomina pn ON pn.PeriodoId = r.PeriodoId " +
                "INNER JOIN dbo.CatEmpresa emp ON emp.EmpresaId = pn.EmpresaId " +
                "INNER JOIN dbo.CatPeriodicidadPago tp ON tp.PeriodicidadPagoId = pn.PeriodicidadPagoId " +
                "INNER JOIN dbo.CatEmpleado e ON e.EmpleadoId = r.EmpleadoId " +
                "LEFT JOIN dbo.CatEmpleadoDatosFiscales f ON f.EmpleadoId = e.EmpleadoId " +
                "LEFT JOIN dbo.CatEmpleadoDatosIMSS i ON i.EmpleadoId = e.EmpleadoId " +
                "WHERE r.ReciboId = @Id",
                AccesoDatos.Parametro("@Id", reciboId));

            if (t.Rows.Count == 0)
            {
                return null;
            }

            DataRow f0 = t.Rows[0];
            ReciboImpresion impresion = new ReciboImpresion();
            impresion.Recibo = new DocReciboNomina
            {
                ReciboId = Fila.Obtener<int>(f0, "ReciboId"),
                PeriodoId = Fila.Obtener<int>(f0, "PeriodoId"),
                EmpleadoId = Fila.Obtener<int>(f0, "EmpleadoId"),
                NumFolioRecibo = Fila.Obtener<string>(f0, "NumFolioRecibo"),
                TipoNomina = Fila.Obtener<string>(f0, "TipoNomina"),
                TotalPercepciones = Fila.Obtener<decimal>(f0, "TotalPercepciones"),
                TotalDeducciones = Fila.Obtener<decimal>(f0, "TotalDeducciones"),
                NetoPagar = Fila.Obtener<decimal>(f0, "NetoPagar"),
                TotalGravado = Fila.Obtener<decimal?>(f0, "TotalGravado"),
                TotalExento = Fila.Obtener<decimal?>(f0, "TotalExento"),
                AntiguedadSemanas = Fila.Obtener<int?>(f0, "AntiguedadSemanas"),
                SnapshotFechaInicialPago = Fila.Obtener<DateTime?>(f0, "SnapshotFechaInicialPago"),
                SnapshotFechaFinalPago = Fila.Obtener<DateTime?>(f0, "SnapshotFechaFinalPago"),
                SnapshotFechaPago = Fila.Obtener<DateTime?>(f0, "SnapshotFechaPago"),
                SnapshotDiasPagados = Fila.Obtener<decimal?>(f0, "SnapshotDiasPagados"),
                SnapshotTipoJornada = Fila.Obtener<string>(f0, "SnapshotTipoJornada"),
                SnapshotPeriodicidadPago = Fila.Obtener<string>(f0, "SnapshotPeriodicidadPago"),
                SnapshotTipoRegimen = Fila.Obtener<string>(f0, "SnapshotTipoRegimen"),
                SnapshotTipoContrato = Fila.Obtener<string>(f0, "SnapshotTipoContrato"),
                SnapshotRegimenFiscalReceptor = Fila.Obtener<string>(f0, "SnapshotRegimenFiscalReceptor"),
                SnapshotBanco = Fila.Obtener<string>(f0, "SnapshotBanco"),
                SnapshotCuentaBancaria = Fila.Obtener<string>(f0, "SnapshotCuentaBancaria"),
                TotalSueldos = Fila.Obtener<decimal?>(f0, "TotalSueldos"),
                TotalSeparacionIndemnizacion = Fila.Obtener<decimal?>(f0, "TotalSeparacionIndemnizacion"),
                TotalJubilacionPensionRetiro = Fila.Obtener<decimal?>(f0, "TotalJubilacionPensionRetiro"),
                TotalOtrosPagos = Fila.Obtener<decimal>(f0, "TotalOtrosPagos"),
                SnapshotSalarioDiario = Fila.Obtener<decimal?>(f0, "SnapshotSalarioDiario"),
                SnapshotSDI = Fila.Obtener<decimal?>(f0, "SnapshotSDI"),
                SnapshotSBC = Fila.Obtener<decimal?>(f0, "SnapshotSBC"),
                SnapshotPuesto = Fila.Obtener<string>(f0, "SnapshotPuesto"),
                SnapshotDepartamento = Fila.Obtener<string>(f0, "SnapshotDepartamento"),
                SnapshotTipoContratoDesc = Fila.Obtener<string>(f0, "SnapshotTipoContratoDesc"),
                SnapshotRegimenFiscalDesc = Fila.Obtener<string>(f0, "SnapshotRegimenFiscalDesc"),
                FechaCalculo = Fila.Obtener<DateTime>(f0, "FechaCalculo"),
                Estatus = Fila.Obtener<string>(f0, "Estatus"),
                Notas = Fila.Obtener<string>(f0, "Notas")
            };
            impresion.Empresa = Fila.Obtener<string>(f0, "EmpresaNombre");
            impresion.EmpresaRFC = Fila.Obtener<string>(f0, "EmpresaRFC");
            impresion.RegistroPatronal = Fila.Obtener<string>(f0, "RegistroPatronal");
            impresion.EmpresaLogo = Fila.Obtener<byte[]>(f0, "EmpresaLogo");
            impresion.NumeroEmpleado = Fila.Obtener<string>(f0, "NumeroEmpleado");
            impresion.NombreEmpleado = Fila.Obtener<string>(f0, "NombreEmpleado");
            impresion.EmpleadoRFC = Fila.Obtener<string>(f0, "EmpleadoRFC");
            impresion.EmpleadoCURP = Fila.Obtener<string>(f0, "EmpleadoCURP");
            impresion.EmpleadoNSS = Fila.Obtener<string>(f0, "EmpleadoNSS");
            impresion.NumeroPeriodo = Fila.Obtener<int>(f0, "NumeroPeriodo");
            impresion.TipoPeriodo = Fila.Obtener<string>(f0, "TipoPeriodo");
            impresion.FechaInicioPeriodo = Fila.Obtener<DateTime>(f0, "FechaInicioPeriodo");
            impresion.FechaFinPeriodo = Fila.Obtener<DateTime>(f0, "FechaFinPeriodo");
            impresion.FechaPagoPeriodo = Fila.Obtener<DateTime>(f0, "FechaPagoPeriodo");
            impresion.EstatusPeriodo = Fila.Obtener<string>(f0, "EstatusPeriodo");

            impresion.Detalles = ListarDetalle(reciboId);
            return impresion;
        }

        /// <summary>Detalle de todos los recibos de un periodo en una sola consulta, agrupado por ReciboId.</summary>
        public System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<DocReciboDetalle>> ListarDetallePorPeriodo(int periodoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT d.DetalleId, d.ReciboId, d.TipoConcepto, d.ClaveConceptoSAT, d.Descripcion, d.ImporteGravado, d.ImporteExento, d.Cantidad " +
                "FROM dbo.DocReciboDetalle d INNER JOIN dbo.DocReciboNomina r ON r.ReciboId = d.ReciboId " +
                "WHERE r.PeriodoId = @Id ORDER BY d.ReciboId, d.DetalleId",
                AccesoDatos.Parametro("@Id", periodoId));

            System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<DocReciboDetalle>> resultado =
                new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<DocReciboDetalle>>();
            foreach (DataRow fila in t.Rows)
            {
                DocReciboDetalle d = MapearDetalle(fila);
                System.Collections.Generic.List<DocReciboDetalle> lista;
                if (!resultado.TryGetValue(d.ReciboId, out lista))
                {
                    lista = new System.Collections.Generic.List<DocReciboDetalle>();
                    resultado[d.ReciboId] = lista;
                }
                lista.Add(d);
            }
            return resultado;
        }

        public System.Collections.Generic.List<DocReciboDetalle> ListarDetalle(int reciboId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT DetalleId, ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento, Cantidad " +
                "FROM dbo.DocReciboDetalle WHERE ReciboId = @Id " +
                "ORDER BY CASE TipoConcepto WHEN 'P' THEN 1 WHEN 'O' THEN 2 ELSE 3 END, DetalleId",
                AccesoDatos.Parametro("@Id", reciboId));

            System.Collections.Generic.List<DocReciboDetalle> lista = new System.Collections.Generic.List<DocReciboDetalle>();
            foreach (DataRow fila in t.Rows)
            {
                lista.Add(MapearDetalle(fila));
            }
            return lista;
        }

        /// <summary>Cabeceras (borradores y definitivos) de los recibos de un periodo, con sus notas.</summary>
        public System.Collections.Generic.List<DocReciboNomina> ListarCabecerasPorPeriodo(int periodoId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT ReciboId, PeriodoId, EmpleadoId, TotalPercepciones, TotalDeducciones, NetoPagar, TotalGravado, TotalExento, " +
                "SnapshotDiasPagados, SnapshotSalarioDiario, SnapshotSDI, SnapshotSBC, SnapshotPuesto, SnapshotDepartamento, " +
                "FechaCalculo, Estatus, Notas " +
                "FROM dbo.DocReciboNomina WHERE PeriodoId = @Id",
                AccesoDatos.Parametro("@Id", periodoId));

            System.Collections.Generic.List<DocReciboNomina> lista = new System.Collections.Generic.List<DocReciboNomina>();
            foreach (DataRow fila in t.Rows)
            {
                lista.Add(new DocReciboNomina
                {
                    ReciboId = Fila.Obtener<int>(fila, "ReciboId"),
                    PeriodoId = Fila.Obtener<int>(fila, "PeriodoId"),
                    EmpleadoId = Fila.Obtener<int>(fila, "EmpleadoId"),
                    TotalPercepciones = Fila.Obtener<decimal>(fila, "TotalPercepciones"),
                    TotalDeducciones = Fila.Obtener<decimal>(fila, "TotalDeducciones"),
                    NetoPagar = Fila.Obtener<decimal>(fila, "NetoPagar"),
                    TotalGravado = Fila.Obtener<decimal?>(fila, "TotalGravado"),
                    TotalExento = Fila.Obtener<decimal?>(fila, "TotalExento"),
                    SnapshotDiasPagados = Fila.Obtener<decimal?>(fila, "SnapshotDiasPagados"),
                    SnapshotSalarioDiario = Fila.Obtener<decimal?>(fila, "SnapshotSalarioDiario"),
                    SnapshotSDI = Fila.Obtener<decimal?>(fila, "SnapshotSDI"),
                    SnapshotSBC = Fila.Obtener<decimal?>(fila, "SnapshotSBC"),
                    SnapshotPuesto = Fila.Obtener<string>(fila, "SnapshotPuesto"),
                    SnapshotDepartamento = Fila.Obtener<string>(fila, "SnapshotDepartamento"),
                    FechaCalculo = Fila.Obtener<DateTime>(fila, "FechaCalculo"),
                    Estatus = Fila.Obtener<string>(fila, "Estatus"),
                    Notas = Fila.Obtener<string>(fila, "Notas")
                });
            }
            return lista;
        }

        private static DocReciboDetalle MapearDetalle(DataRow fila)
        {
            return new DocReciboDetalle
            {
                DetalleId = Fila.Obtener<int>(fila, "DetalleId"),
                ReciboId = Fila.Obtener<int>(fila, "ReciboId"),
                TipoConcepto = Fila.Obtener<string>(fila, "TipoConcepto"),
                ClaveConceptoSAT = Fila.Obtener<string>(fila, "ClaveConceptoSAT"),
                Descripcion = Fila.Obtener<string>(fila, "Descripcion"),
                ImporteGravado = Fila.Obtener<decimal>(fila, "ImporteGravado"),
                ImporteExento = Fila.Obtener<decimal>(fila, "ImporteExento"),
                Cantidad = Fila.Obtener<decimal?>(fila, "Cantidad")
            };
        }
    }
}
