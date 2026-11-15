using System;
using System.Collections.Generic;
using System.Data;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Relaciones;

namespace NomiMex.Datos
{
    /// <summary>
    /// Acceso a dbo.CatPaqueteIncidencia y dbo.RelPaqueteConcepto: paquetes de conceptos SAT que se asignan al trabajador
    /// en su alta según la periodicidad de su contrato.
    /// </summary>
    public class PaqueteIncidenciaDAL
    {
        public List<CatPaqueteIncidencia> Listar()
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT p.IdPaquete, p.Nombre, p.PeriodicidadPagoId, p.EmpresaId, p.EsDefault, p.Activo, " +
                "ISNULL(pp.Descripcion, 'Todas') AS Periodicidad, " +
                "(SELECT COUNT(1) FROM dbo.RelPaqueteConcepto rp WHERE rp.IdPaquete = p.IdPaquete AND rp.Activo = 1) AS Total " +
                "FROM dbo.CatPaqueteIncidencia p LEFT JOIN dbo.CatPeriodicidadPago pp ON pp.PeriodicidadPagoId = p.PeriodicidadPagoId " +
                "ORDER BY ISNULL(p.PeriodicidadPagoId, 0), p.Nombre");
            var lista = new List<CatPaqueteIncidencia>();
            foreach (DataRow f in t.Rows)
                lista.Add(new CatPaqueteIncidencia
                {
                    IdPaquete = Fila.Obtener<int>(f, "IdPaquete"),
                    Nombre = Fila.Obtener<string>(f, "Nombre"),
                    PeriodicidadPagoId = Fila.Obtener<int?>(f, "PeriodicidadPagoId"),
                    EmpresaId = Fila.Obtener<int?>(f, "EmpresaId"),
                    EsDefault = Fila.Obtener<bool>(f, "EsDefault"),
                    Activo = Fila.Obtener<bool>(f, "Activo"),
                    PeriodicidadDescripcion = Fila.Obtener<string>(f, "Periodicidad"),
                    TotalConceptos = Fila.Obtener<int>(f, "Total")
                });
            return lista;
        }

        public List<RelPaqueteConcepto> ListarConceptos(int paqueteId)
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT rp.IdPaqueteConcepto, rp.IdPaquete, rp.IdConceptoSAT, rp.CantidadOValor, rp.EsPermanente, rp.Activo AS RelActivo, " +
                ConceptoIncidenciaDAL.ColumnasConcepto +
                " FROM dbo.RelPaqueteConcepto rp JOIN dbo.CatConcepto c ON c.ConceptoId = rp.IdConceptoSAT " +
                "WHERE rp.IdPaquete = @Paquete AND rp.Activo = 1 ORDER BY c.EsBase DESC, c.TipoConcepto, c.ClaveSAT",
                AccesoDatos.Parametro("@Paquete", paqueteId));
            var lista = new List<RelPaqueteConcepto>();
            foreach (DataRow f in t.Rows)
                lista.Add(new RelPaqueteConcepto
                {
                    IdPaqueteConcepto = Fila.Obtener<int>(f, "IdPaqueteConcepto"),
                    IdPaquete = Fila.Obtener<int>(f, "IdPaquete"),
                    IdConceptoSAT = Fila.Obtener<int>(f, "IdConceptoSAT"),
                    CantidadOValor = Fila.Obtener<decimal>(f, "CantidadOValor"),
                    EsPermanente = Fila.Obtener<bool>(f, "EsPermanente"),
                    Activo = Fila.Obtener<bool>(f, "RelActivo"),
                    Concepto = ConceptoIncidenciaDAL.LeerConcepto(f)
                });
            return lista;
        }

        /// <summary>
        /// Inserta o actualiza un paquete. Solo puede haber un paquete por defecto por periodicidad y empresa: al marcar uno
        /// se desmarca el anterior. Un paquete nuevo nace con los conceptos base. Devuelve el identificador del paquete.
        /// </summary>
        public int Guardar(CatPaqueteIncidencia p)
        {
            return AccesoDatos.EjecutarEnTransaccion<int>((cn, tx) =>
            {
                int id = p.IdPaquete;
                if (p.EsDefault)
                    AccesoDatos.EjecutarComando(cn, tx,
                        "UPDATE dbo.CatPaqueteIncidencia SET EsDefault = 0 WHERE IdPaquete <> @Id AND EsDefault = 1 " +
                        "AND ISNULL(PeriodicidadPagoId, 0) = ISNULL(@Periodicidad, 0) AND ISNULL(EmpresaId, 0) = ISNULL(@Empresa, 0)",
                        AccesoDatos.Parametro("@Id", id), AccesoDatos.ParametroEntero("@Periodicidad", p.PeriodicidadPagoId),
                        AccesoDatos.ParametroEntero("@Empresa", p.EmpresaId));
                if (id == 0)
                {
                    id = Convert.ToInt32(AccesoDatos.Escalar(cn, tx,
                        "INSERT dbo.CatPaqueteIncidencia(Nombre, PeriodicidadPagoId, EmpresaId, EsDefault, Activo) " +
                        "VALUES(@Nombre, @Periodicidad, @Empresa, @Default, @Activo); SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        AccesoDatos.Parametro("@Nombre", p.Nombre), AccesoDatos.ParametroEntero("@Periodicidad", p.PeriodicidadPagoId),
                        AccesoDatos.ParametroEntero("@Empresa", p.EmpresaId), AccesoDatos.Parametro("@Default", p.EsDefault),
                        AccesoDatos.Parametro("@Activo", p.Activo)));
                    AccesoDatos.EjecutarComando(cn, tx,
                        "INSERT dbo.RelPaqueteConcepto(IdPaquete, IdConceptoSAT, CantidadOValor, EsPermanente, Activo) " +
                        "SELECT @Id, ConceptoId, 0, 1, 1 FROM dbo.CatConcepto WHERE EsBase = 1",
                        AccesoDatos.Parametro("@Id", id));
                }
                else
                {
                    int filas = AccesoDatos.EjecutarComando(cn, tx,
                        "UPDATE dbo.CatPaqueteIncidencia SET Nombre = @Nombre, PeriodicidadPagoId = @Periodicidad, EmpresaId = @Empresa, " +
                        "EsDefault = @Default, Activo = @Activo WHERE IdPaquete = @Id",
                        AccesoDatos.Parametro("@Id", id), AccesoDatos.Parametro("@Nombre", p.Nombre),
                        AccesoDatos.ParametroEntero("@Periodicidad", p.PeriodicidadPagoId), AccesoDatos.ParametroEntero("@Empresa", p.EmpresaId),
                        AccesoDatos.Parametro("@Default", p.EsDefault), AccesoDatos.Parametro("@Activo", p.Activo));
                    if (filas != 1) throw new ExcepcionDatos("El paquete ya no existe.", 50000, true, null);
                }
                return id;
            });
        }

        /// <summary>Agrega el concepto al paquete o, si ya estaba, actualiza su cantidad y permanencia.</summary>
        public void AgregarConcepto(RelPaqueteConcepto r)
        {
            AccesoDatos.EjecutarComando(
                "MERGE dbo.RelPaqueteConcepto WITH (HOLDLOCK) AS t " +
                "USING (SELECT @Paquete AS IdPaquete, @Concepto AS IdConceptoSAT) AS s " +
                "ON t.IdPaquete = s.IdPaquete AND t.IdConceptoSAT = s.IdConceptoSAT " +
                "WHEN MATCHED THEN UPDATE SET CantidadOValor = @Cantidad, EsPermanente = @Permanente, Activo = 1 " +
                "WHEN NOT MATCHED THEN INSERT(IdPaquete, IdConceptoSAT, CantidadOValor, EsPermanente, Activo) " +
                "VALUES(@Paquete, @Concepto, @Cantidad, @Permanente, 1);",
                AccesoDatos.Parametro("@Paquete", r.IdPaquete), AccesoDatos.Parametro("@Concepto", r.IdConceptoSAT),
                AccesoDatos.ParametroDecimal("@Cantidad", r.CantidadOValor, 18, 4), AccesoDatos.Parametro("@Permanente", r.EsPermanente));
        }

        /// <summary>Quita del paquete un concepto que no sea base. No altera a los trabajadores que ya lo recibieron.</summary>
        public void QuitarConcepto(int paqueteId, int conceptoId)
        {
            int filas = AccesoDatos.EjecutarComando(
                "DELETE rp FROM dbo.RelPaqueteConcepto rp JOIN dbo.CatConcepto c ON c.ConceptoId = rp.IdConceptoSAT " +
                "WHERE rp.IdPaquete = @Paquete AND rp.IdConceptoSAT = @Concepto AND c.EsBase = 0",
                AccesoDatos.Parametro("@Paquete", paqueteId), AccesoDatos.Parametro("@Concepto", conceptoId));
            if (filas != 1) throw new ExcepcionDatos("El concepto no está en el paquete o es un concepto base que no se puede quitar.", 50000, true, null);
        }
    }
}
