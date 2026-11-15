using System.Collections.Generic;
using System.Data;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>Catálogos de solo lectura usados para llenar listas desplegables y parámetros legales.</summary>
    public class CatalogoDAL
    {
        public List<ItemCatalogo> ListarTiposPeriodo()
        {
            return Listar("SELECT PeriodicidadPagoId AS Id, Clave, Descripcion FROM dbo.CatSatPeriodicidadPago WHERE Activo = 1 ORDER BY PeriodicidadPagoId");
        }

        public List<ItemCatalogo> ListarTiposContrato()
        {
            return Listar("SELECT TipoContratoId AS Id, Clave, Descripcion FROM dbo.CatTipoContrato WHERE Activo = 1 ORDER BY TipoContratoId");
        }

        public List<ItemCatalogo> ListarTiposJornada()
        {
            return Listar("SELECT TipoJornadaId AS Id, Clave, Descripcion FROM dbo.CatTipoJornada WHERE Activo = 1 ORDER BY TipoJornadaId");
        }

        public List<ItemCatalogo> ListarTiposRegimen()
        {
            return Listar("SELECT TipoRegimenId AS Id, Clave, Descripcion FROM dbo.CatTipoRegimen WHERE Activo = 1 ORDER BY TipoRegimenId");
        }

        public List<JornadaDias> ListarJornadasConDias()
        {
            DataTable t = AccesoDatos.Consultar(
                "SELECT j.TipoJornadaId, j.Clave, j.Descripcion, r.HorasMaximasLegal, ISNULL(r.DiasSemana, 0) AS DiasSemana " +
                "FROM dbo.CatSatTipoJornada j LEFT JOIN dbo.CatReglaTipoJornada r ON r.TipoJornadaId = j.TipoJornadaId " +
                "WHERE j.Activo = 1 ORDER BY j.TipoJornadaId");
            List<JornadaDias> lista = new List<JornadaDias>();
            foreach (DataRow f in t.Rows)
            {
                lista.Add(new JornadaDias
                {
                    TipoJornadaId = Fila.Obtener<int>(f, "TipoJornadaId"),
                    Clave = Fila.Obtener<string>(f, "Clave"),
                    Descripcion = Fila.Obtener<string>(f, "Descripcion"),
                    HorasMaximasLegal = Fila.Obtener<decimal?>(f, "HorasMaximasLegal"),
                    DiasSemana = Fila.Obtener<int>(f, "DiasSemana")
                });
            }
            return lista;
        }

        /// <summary>Guarda los días por semana de un tipo de jornada; null o 0 lo deja sin definir (jornada completa).</summary>
        public void GuardarDiasJornada(int tipoJornadaId, int? dias)
        {
            AccesoDatos.EjecutarComando(
                "MERGE dbo.CatReglaTipoJornada AS r USING (SELECT @Id AS TipoJornadaId) AS o ON r.TipoJornadaId = o.TipoJornadaId " +
                "WHEN MATCHED THEN UPDATE SET DiasSemana = @Dias " +
                "WHEN NOT MATCHED THEN INSERT (TipoJornadaId, DiasSemana) VALUES (@Id, @Dias);",
                AccesoDatos.Parametro("@Id", tipoJornadaId),
                AccesoDatos.ParametroEntero("@Dias", dias));
        }

        public List<ItemCatalogo> ListarRegimenesFiscales()
        {
            return Listar("SELECT RegimenFiscalId AS Id, Clave, Descripcion FROM dbo.CatRegimenFiscal WHERE Activo = 1 ORDER BY RegimenFiscalId");
        }

        public List<ItemCatalogo> ListarRiesgosPuesto()
        {
            return Listar("SELECT RiesgoPuestoId AS Id, Clave, Descripcion FROM dbo.CatRiesgoPuesto WHERE Activo = 1 ORDER BY RiesgoPuestoId");
        }

        public List<ItemCatalogo> ListarBancos()
        {
            return Listar("SELECT BancoId AS Id, Clave, Descripcion FROM dbo.CatBanco WHERE Activo = 1 ORDER BY Descripcion");
        }

        /// <summary>Orígenes de recurso vigentes (c_OrigenRecurso) con su descripción para listas desplegables.</summary>
        public List<ItemCatalogo> ListarOrigenesRecursoItems()
        {
            return Listar("SELECT OrigenRecursoId AS Id, Clave, Descripcion FROM dbo.CatOrigenRecurso WHERE Activo = 1 ORDER BY OrigenRecursoId");
        }

        /// <summary>Claves vigentes de c_OrigenRecurso (IP, IF, IM).</summary>
        public List<string> ListarOrigenesRecurso()
        {
            DataTable tabla = AccesoDatos.Consultar("SELECT Clave FROM dbo.CatOrigenRecurso WHERE Activo = 1 ORDER BY OrigenRecursoId");
            List<string> lista = new List<string>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(Fila.Obtener<string>(fila, "Clave"));
            }
            return lista;
        }

        /// <summary>Incidencias legales: conceptos TipoConcepto 'I' de dbo.CatConcepto que interpreta el motor de nómina.</summary>
        public List<CatTipoIncidencia> ListarTiposIncidencia()
        {
            DataTable tabla = AccesoDatos.Consultar(
                "SELECT ConceptoId, ClaveSAT, Descripcion, DisminuyeDias, " +
                "CAST(CASE WHEN TipoHorasId IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS GeneraHorasExtra, Activo " +
                "FROM dbo.CatConcepto WHERE TipoConcepto = 'I' AND Activo = 1 ORDER BY ConceptoId");
            List<CatTipoIncidencia> lista = new List<CatTipoIncidencia>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new CatTipoIncidencia
                {
                    TipoIncidenciaId = Fila.Obtener<int>(fila, "ConceptoId"),
                    Codigo = Fila.Obtener<string>(fila, "ClaveSAT"),
                    Descripcion = Fila.Obtener<string>(fila, "Descripcion"),
                    AfectaSueldo = Fila.Obtener<bool>(fila, "DisminuyeDias"),
                    DisminuyeDias = Fila.Obtener<bool>(fila, "DisminuyeDias"),
                    GeneraHorasExtra = Fila.Obtener<bool>(fila, "GeneraHorasExtra"),
                    Activo = Fila.Obtener<bool>(fila, "Activo")
                });
            }
            return lista;
        }

        /// <summary>
        /// Parámetros para integrar el salario: política activa de la empresa (o valores mínimos de ley),
        /// días de vacaciones por antigüedad (fn_DiasVacaciones) y UMA diaria vigente.
        /// </summary>
        public ParametrosIntegracion ObtenerParametrosIntegracion(int empresaId, int aniosServicio)
        {
            DataTable tabla = AccesoDatos.Consultar(
                "DECLARE @Pol INT = dbo.fn_ObtenerPoliticaActiva(@EmpresaId, NULL); " +
                "SELECT " +
                "  ISNULL((SELECT DiasAguinaldo FROM dbo.CatPoliticaEmpresarial WHERE PoliticaId = @Pol), 15) AS DiasAguinaldo, " +
                "  ISNULL((SELECT PorcentajePrimaVacacional FROM dbo.CatPoliticaEmpresarial WHERE PoliticaId = @Pol), 25) AS PrimaVac, " +
                "  CAST(dbo.fn_DiasVacaciones(@Anios) AS DECIMAL(10,4)) AS DiasVacaciones, " +
                "  dbo.fn_ObtenerUMADiaria(YEAR(GETDATE())) AS UMA",
                AccesoDatos.Parametro("@EmpresaId", empresaId),
                AccesoDatos.Parametro("@Anios", aniosServicio));

            DataRow fila = tabla.Rows[0];
            return new ParametrosIntegracion
            {
                DiasAguinaldo = Fila.Obtener<decimal>(fila, "DiasAguinaldo"),
                PrimaVacacionalPorcentaje = Fila.Obtener<decimal>(fila, "PrimaVac"),
                DiasVacaciones = Fila.Obtener<decimal>(fila, "DiasVacaciones"),
                UMADiaria = Fila.Obtener<decimal>(fila, "UMA")
            };
        }

        /// <summary>Salario mínimo diario vigente para el año y zona (fn_ObtenerSMGDiario); 0 si no hay tabla cargada.</summary>
        public decimal ObtenerSalarioMinimoDiario(int anio, string zona)
        {
            object r = AccesoDatos.Escalar("SELECT dbo.fn_ObtenerSMGDiario(@Anio, @Zona)",
                AccesoDatos.Parametro("@Anio", anio),
                AccesoDatos.Parametro("@Zona", zona));
            return r == null ? 0m : System.Convert.ToDecimal(r);
        }

        private static List<ItemCatalogo> Listar(string sql)
        {
            DataTable tabla = AccesoDatos.Consultar(sql);
            List<ItemCatalogo> lista = new List<ItemCatalogo>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new ItemCatalogo
                {
                    Id = Fila.Obtener<int>(fila, "Id"),
                    Clave = Fila.Obtener<string>(fila, "Clave"),
                    Descripcion = Fila.Obtener<string>(fila, "Descripcion")
                });
            }
            return lista;
        }
    }
}
