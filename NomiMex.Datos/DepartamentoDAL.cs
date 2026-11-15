using System.Collections.Generic;
using System.Data;
using NomiMex.Entidades.Catalogos;

namespace NomiMex.Datos
{
    /// <summary>Acceso a dbo.CatDepartamento.</summary>
    public class DepartamentoDAL
    {
        public List<CatDepartamento> Listar(int empresaId, bool soloActivos)
        {
            string sql = "SELECT DepartamentoId, EmpresaId, Clave, Descripcion, Activo FROM dbo.CatDepartamento WHERE EmpresaId = @EmpresaId" +
                         (soloActivos ? " AND Activo = 1" : string.Empty) + " ORDER BY Descripcion";
            DataTable tabla = AccesoDatos.Consultar(sql, AccesoDatos.Parametro("@EmpresaId", empresaId));
            List<CatDepartamento> lista = new List<CatDepartamento>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new CatDepartamento
                {
                    DepartamentoId = Fila.Obtener<int>(fila, "DepartamentoId"),
                    EmpresaId = Fila.Obtener<int>(fila, "EmpresaId"),
                    Clave = Fila.Obtener<string>(fila, "Clave"),
                    Descripcion = Fila.Obtener<string>(fila, "Descripcion"),
                    Activo = Fila.Obtener<bool>(fila, "Activo")
                });
            }
            return lista;
        }

        public bool ExisteClave(int empresaId, string clave, int excluirDepartamentoId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.CatDepartamento WHERE EmpresaId = @EmpresaId AND Clave = @Clave AND DepartamentoId <> @Id",
                AccesoDatos.Parametro("@EmpresaId", empresaId),
                AccesoDatos.Parametro("@Clave", clave),
                AccesoDatos.Parametro("@Id", excluirDepartamentoId));
            return System.Convert.ToInt32(r) > 0;
        }

        public int Insertar(CatDepartamento d)
        {
            object id = AccesoDatos.Escalar(
                "INSERT INTO dbo.CatDepartamento (EmpresaId, Clave, Descripcion, Activo) VALUES (@EmpresaId, @Clave, @Descripcion, @Activo); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);",
                AccesoDatos.Parametro("@EmpresaId", d.EmpresaId),
                AccesoDatos.Parametro("@Clave", d.Clave),
                AccesoDatos.Parametro("@Descripcion", d.Descripcion),
                AccesoDatos.Parametro("@Activo", d.Activo));
            return System.Convert.ToInt32(id);
        }

        public void Actualizar(CatDepartamento d)
        {
            AccesoDatos.EjecutarComando(
                "UPDATE dbo.CatDepartamento SET Clave = @Clave, Descripcion = @Descripcion, Activo = @Activo WHERE DepartamentoId = @Id",
                AccesoDatos.Parametro("@Id", d.DepartamentoId),
                AccesoDatos.Parametro("@Clave", d.Clave),
                AccesoDatos.Parametro("@Descripcion", d.Descripcion),
                AccesoDatos.Parametro("@Activo", d.Activo));
        }

        public int ContarEmpleadosActivos(int departamentoId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.DocContratoEmpleado c                 INNER JOIN dbo.CatEmpleado e ON e.EmpleadoId = c.EmpleadoId " +
                "WHERE c.DepartamentoId = @Id AND c.Activo = 1 AND e.Estatus <> 'BAJA'",
                AccesoDatos.Parametro("@Id", departamentoId));
            return System.Convert.ToInt32(r);
        }

        public void DesactivarLogico(int departamentoId)
        {
            AccesoDatos.EjecutarComando("UPDATE dbo.CatDepartamento SET Activo = 0 WHERE DepartamentoId = @Id",
                AccesoDatos.Parametro("@Id", departamentoId));
        }
    }
}
