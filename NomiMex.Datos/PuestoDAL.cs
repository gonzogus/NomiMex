using System.Collections.Generic;
using System.Data;
using NomiMex.Entidades.Catalogos;

namespace NomiMex.Datos
{
    /// <summary>Acceso a dbo.CatPuesto.</summary>
    public class PuestoDAL
    {
        public List<CatPuesto> Listar(int empresaId, bool soloActivos)
        {
            string sql = "SELECT PuestoId, EmpresaId, Clave, Descripcion, SalarioBandaMin, SalarioBandaMax, Activo FROM dbo.CatPuesto WHERE EmpresaId = @EmpresaId" +
                         (soloActivos ? " AND Activo = 1" : string.Empty) + " ORDER BY Descripcion";
            DataTable tabla = AccesoDatos.Consultar(sql, AccesoDatos.Parametro("@EmpresaId", empresaId));
            List<CatPuesto> lista = new List<CatPuesto>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new CatPuesto
                {
                    PuestoId = Fila.Obtener<int>(fila, "PuestoId"),
                    EmpresaId = Fila.Obtener<int>(fila, "EmpresaId"),
                    Clave = Fila.Obtener<string>(fila, "Clave"),
                    Descripcion = Fila.Obtener<string>(fila, "Descripcion"),
                    SalarioBandaMin = Fila.Obtener<decimal?>(fila, "SalarioBandaMin"),
                    SalarioBandaMax = Fila.Obtener<decimal?>(fila, "SalarioBandaMax"),
                    Activo = Fila.Obtener<bool>(fila, "Activo")
                });
            }
            return lista;
        }

        public bool ExisteClave(int empresaId, string clave, int excluirPuestoId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.CatPuesto WHERE EmpresaId = @EmpresaId AND Clave = @Clave AND PuestoId <> @Id",
                AccesoDatos.Parametro("@EmpresaId", empresaId),
                AccesoDatos.Parametro("@Clave", clave),
                AccesoDatos.Parametro("@Id", excluirPuestoId));
            return System.Convert.ToInt32(r) > 0;
        }

        public int Insertar(CatPuesto p)
        {
            object id = AccesoDatos.Escalar(
                "INSERT INTO dbo.CatPuesto (EmpresaId, Clave, Descripcion, SalarioBandaMin, SalarioBandaMax, Activo) " +
                "VALUES (@EmpresaId, @Clave, @Descripcion, @Min, @Max, @Activo); SELECT CAST(SCOPE_IDENTITY() AS INT);",
                AccesoDatos.Parametro("@EmpresaId", p.EmpresaId),
                AccesoDatos.Parametro("@Clave", p.Clave),
                AccesoDatos.Parametro("@Descripcion", p.Descripcion),
                AccesoDatos.ParametroDecimal("@Min", p.SalarioBandaMin, 18, 4),
                AccesoDatos.ParametroDecimal("@Max", p.SalarioBandaMax, 18, 4),
                AccesoDatos.Parametro("@Activo", p.Activo));
            return System.Convert.ToInt32(id);
        }

        public void Actualizar(CatPuesto p)
        {
            AccesoDatos.EjecutarComando(
                "UPDATE dbo.CatPuesto SET Clave = @Clave, Descripcion = @Descripcion, SalarioBandaMin = @Min, " +
                "SalarioBandaMax = @Max, Activo = @Activo WHERE PuestoId = @Id",
                AccesoDatos.Parametro("@Id", p.PuestoId),
                AccesoDatos.Parametro("@Clave", p.Clave),
                AccesoDatos.Parametro("@Descripcion", p.Descripcion),
                AccesoDatos.ParametroDecimal("@Min", p.SalarioBandaMin, 18, 4),
                AccesoDatos.ParametroDecimal("@Max", p.SalarioBandaMax, 18, 4),
                AccesoDatos.Parametro("@Activo", p.Activo));
        }

        public int ContarEmpleadosActivos(int puestoId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.DocContratoEmpleado c INNER JOIN dbo.CatEmpleado e ON e.EmpleadoId = c.EmpleadoId " +
                "WHERE c.PuestoId = @Id AND c.Activo = 1 AND e.Estatus <> 'BAJA'",
                AccesoDatos.Parametro("@Id", puestoId));
            return System.Convert.ToInt32(r);
        }

        public void DesactivarLogico(int puestoId)
        {
            AccesoDatos.EjecutarComando("UPDATE dbo.CatPuesto SET Activo = 0 WHERE PuestoId = @Id",
                AccesoDatos.Parametro("@Id", puestoId));
        }
    }
}
