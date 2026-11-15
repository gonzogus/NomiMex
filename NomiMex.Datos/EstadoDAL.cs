using System.Collections.Generic;
using System.Data;
using NomiMex.Entidades.Catalogos;

namespace NomiMex.Datos
{
    /// <summary>Acceso a dbo.CatEntidadFederativa (catálogo de Estados).</summary>
    public class EstadoDAL
    {
        public List<CatEntidadFederativa> Listar(bool soloActivos)
        {
            string sql = "SELECT CveEntidad, Nombre, Abreviatura, Activo FROM dbo.CatEntidadFederativa" +
                         (soloActivos ? " WHERE Activo = 1" : string.Empty) + " ORDER BY Nombre";
            DataTable tabla = AccesoDatos.Consultar(sql);
            List<CatEntidadFederativa> lista = new List<CatEntidadFederativa>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new CatEntidadFederativa
                {
                    CveEntidad = Fila.Obtener<string>(fila, "CveEntidad"),
                    Nombre = Fila.Obtener<string>(fila, "Nombre"),
                    Abreviatura = Fila.Obtener<string>(fila, "Abreviatura"),
                    Activo = Fila.Obtener<bool>(fila, "Activo")
                });
            }
            return lista;
        }

        public bool Existe(string cveEntidad)
        {
            object r = AccesoDatos.Escalar("SELECT COUNT(1) FROM dbo.CatEntidadFederativa WHERE CveEntidad = @Cve",
                AccesoDatos.Parametro("@Cve", cveEntidad));
            return System.Convert.ToInt32(r) > 0;
        }

        public void Insertar(CatEntidadFederativa e)
        {
            AccesoDatos.EjecutarComando(
                "INSERT INTO dbo.CatEntidadFederativa (CveEntidad, Nombre, Abreviatura, Activo) VALUES (@Cve, @Nombre, @Abrev, @Activo)",
                AccesoDatos.Parametro("@Cve", e.CveEntidad),
                AccesoDatos.Parametro("@Nombre", e.Nombre),
                AccesoDatos.Parametro("@Abrev", e.Abreviatura),
                AccesoDatos.Parametro("@Activo", e.Activo));
        }

        public void Actualizar(CatEntidadFederativa e)
        {
            AccesoDatos.EjecutarComando(
                "UPDATE dbo.CatEntidadFederativa SET Nombre = @Nombre, Abreviatura = @Abrev, Activo = @Activo WHERE CveEntidad = @Cve",
                AccesoDatos.Parametro("@Cve", e.CveEntidad),
                AccesoDatos.Parametro("@Nombre", e.Nombre),
                AccesoDatos.Parametro("@Abrev", e.Abreviatura),
                AccesoDatos.Parametro("@Activo", e.Activo));
        }

        public void DesactivarLogico(string cveEntidad)
        {
            AccesoDatos.EjecutarComando("UPDATE dbo.CatEntidadFederativa SET Activo = 0 WHERE CveEntidad = @Cve",
                AccesoDatos.Parametro("@Cve", cveEntidad));
        }
    }
}
