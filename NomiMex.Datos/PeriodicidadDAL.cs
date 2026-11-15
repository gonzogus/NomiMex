using System;
using System.Collections.Generic;
using System.Data;
using NomiMex.Entidades.Relaciones;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>Acceso a dbo.RelEmpresaPeriodicidad: periodicidades de pago asignadas a cada empresa y su configuración.</summary>
    public class PeriodicidadDAL
    {
        private const string SelectPeriodicidad =
            "SELECT r.EmpresaPeriodicidadId, r.EmpresaId, r.PeriodicidadPagoId, r.Descripcion, r.CalculaSeptimoDia, r.DiaInicio, r.DiaFin, " +
            "r.DiaPago, r.DiaDescanso, r.TrabajaDomingo, r.PorcentajePrimaDominical, r.Activo, " +
            "pp.Clave AS PeriodicidadClave, pp.Descripcion AS PeriodicidadDescripcion, pp.DiasBase " +
            "FROM dbo.RelEmpresaPeriodicidad r INNER JOIN dbo.CatPeriodicidadPago pp ON pp.PeriodicidadPagoId = r.PeriodicidadPagoId ";

        public List<EmpresaPeriodicidad> ListarPorEmpresa(int empresaId, bool soloActivas)
        {
            DataTable tabla = AccesoDatos.Consultar(
                SelectPeriodicidad + "WHERE r.EmpresaId = @Empresa AND (@SoloActivas = 0 OR r.Activo = 1) ORDER BY pp.PeriodicidadPagoId",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@SoloActivas", soloActivas));

            List<EmpresaPeriodicidad> lista = new List<EmpresaPeriodicidad>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new EmpresaPeriodicidad
                {
                    EmpresaPeriodicidadId = Fila.Obtener<int>(fila, "EmpresaPeriodicidadId"),
                    EmpresaId = Fila.Obtener<int>(fila, "EmpresaId"),
                    PeriodicidadPagoId = Fila.Obtener<int>(fila, "PeriodicidadPagoId"),
                    Descripcion = Fila.Obtener<string>(fila, "Descripcion"),
                    CalculaSeptimoDia = Fila.Obtener<bool>(fila, "CalculaSeptimoDia"),
                    DiaInicio = Fila.Obtener<byte?>(fila, "DiaInicio"),
                    DiaFin = Fila.Obtener<byte?>(fila, "DiaFin"),
                    DiaPago = Fila.Obtener<byte?>(fila, "DiaPago"),
                    DiaDescanso = Fila.Obtener<byte?>(fila, "DiaDescanso"),
                    TrabajaDomingo = Fila.Obtener<bool>(fila, "TrabajaDomingo"),
                    PorcentajePrimaDominical = Fila.Obtener<decimal>(fila, "PorcentajePrimaDominical"),
                    Activo = Fila.Obtener<bool>(fila, "Activo"),
                    PeriodicidadClave = Fila.Obtener<string>(fila, "PeriodicidadClave"),
                    PeriodicidadDescripcion = Fila.Obtener<string>(fila, "PeriodicidadDescripcion"),
                    DiasBase = Fila.Obtener<decimal>(fila, "DiasBase")
                });
            }
            return lista;
        }

        public EmpresaPeriodicidad Obtener(int empresaId, int periodicidadPagoId)
        {
            foreach (EmpresaPeriodicidad p in ListarPorEmpresa(empresaId, true))
            {
                if (p.PeriodicidadPagoId == periodicidadPagoId)
                {
                    return p;
                }
            }
            return null;
        }

        /// <summary>Actualiza las reglas de generación de periodos de una periodicidad ya asignada a la empresa.</summary>
        public void GuardarConfiguracion(RelEmpresaPeriodicidad r)
        {
            int filas = AccesoDatos.EjecutarComando(
                "UPDATE dbo.RelEmpresaPeriodicidad SET CalculaSeptimoDia = @Septimo, DiaInicio = @Inicio, DiaFin = @Fin, DiaPago = @Pago, " +
                "DiaDescanso = @Descanso, FechaModificacion = GETDATE() " +
                "WHERE EmpresaId = @Empresa AND PeriodicidadPagoId = @Periodicidad AND Activo = 1",
                AccesoDatos.Parametro("@Septimo", r.CalculaSeptimoDia),
                AccesoDatos.Parametro("@Inicio", r.DiaInicio.HasValue ? (object)r.DiaInicio.Value : null),
                AccesoDatos.Parametro("@Fin", r.DiaFin.HasValue ? (object)r.DiaFin.Value : null),
                AccesoDatos.Parametro("@Pago", r.DiaPago.HasValue ? (object)r.DiaPago.Value : null),
                AccesoDatos.Parametro("@Descanso", r.DiaDescanso.HasValue ? (object)r.DiaDescanso.Value : null),
                AccesoDatos.Parametro("@Empresa", r.EmpresaId),
                AccesoDatos.Parametro("@Periodicidad", r.PeriodicidadPagoId));
            if (filas == 0)
            {
                throw new ExcepcionDatos("La periodicidad ya no está asignada a la empresa.", 0, true, null);
            }
        }

        /// <summary>Empleados que no están en baja y cuyo contrato vigente usa la periodicidad en la empresa.</summary>
        public int ContarEmpleadosConPeriodicidad(int empresaId, int periodicidadPagoId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.CatEmpleado e " +
                "INNER JOIN dbo.DocContratoEmpleado c ON c.EmpleadoId = e.EmpleadoId AND c.Activo = 1 " +
                "WHERE e.EmpresaId = @Empresa AND c.PeriodicidadPagoId = @Periodicidad AND e.Estatus <> 'BAJA'",
                AccesoDatos.Parametro("@Empresa", empresaId),
                AccesoDatos.Parametro("@Periodicidad", periodicidadPagoId));
            return Convert.ToInt32(r);
        }

        /// <summary>
        /// Sincroniza las periodicidades de una empresa dentro de la transacción del llamador: las nuevas se insertan con la
        /// configuración predeterminada recibida, las ya existentes se reactivan y las que ya no se desean se desactivan.
        /// </summary>
        internal static void Sincronizar(System.Data.SqlClient.SqlConnection cn, System.Data.SqlClient.SqlTransaction tx,
                                         int empresaId, IList<RelEmpresaPeriodicidad> deseadas)
        {
            List<int> ids = new List<int>();
            foreach (RelEmpresaPeriodicidad d in deseadas)
            {
                ids.Add(d.PeriodicidadPagoId);
                AccesoDatos.EjecutarComando(cn, tx,
                    "IF EXISTS (SELECT 1 FROM dbo.RelEmpresaPeriodicidad WHERE EmpresaId = @Empresa AND PeriodicidadPagoId = @Periodicidad) " +
                    "UPDATE dbo.RelEmpresaPeriodicidad SET Activo = 1, FechaModificacion = GETDATE() " +
                    "WHERE EmpresaId = @Empresa AND PeriodicidadPagoId = @Periodicidad AND Activo = 0 " +
                    "ELSE INSERT INTO dbo.RelEmpresaPeriodicidad (EmpresaId, PeriodicidadPagoId, Descripcion, CalculaSeptimoDia, DiaInicio, DiaFin, " +
                    "DiaPago, DiaDescanso, TrabajaDomingo, PorcentajePrimaDominical, Activo) " +
                    "VALUES (@Empresa, @Periodicidad, @Descripcion, @Septimo, @Inicio, @Fin, @Pago, @Descanso, @Domingo, @PrimaDom, 1)",
                    AccesoDatos.Parametro("@Empresa", empresaId),
                    AccesoDatos.Parametro("@Periodicidad", d.PeriodicidadPagoId),
                    AccesoDatos.Parametro("@Descripcion", d.Descripcion),
                    AccesoDatos.Parametro("@Septimo", d.CalculaSeptimoDia),
                    AccesoDatos.Parametro("@Inicio", d.DiaInicio.HasValue ? (object)d.DiaInicio.Value : null),
                    AccesoDatos.Parametro("@Fin", d.DiaFin.HasValue ? (object)d.DiaFin.Value : null),
                    AccesoDatos.Parametro("@Pago", d.DiaPago.HasValue ? (object)d.DiaPago.Value : null),
                    AccesoDatos.Parametro("@Descanso", d.DiaDescanso.HasValue ? (object)d.DiaDescanso.Value : null),
                    AccesoDatos.Parametro("@Domingo", d.TrabajaDomingo),
                    AccesoDatos.ParametroDecimal("@PrimaDom", d.PorcentajePrimaDominical, 5, 2));
            }

            // Se desactivan las que ya no se desean; el trigger trg_RelEmpresaPeriodicidad_ValidarBaja impide retirar las que usan empleados
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                AccesoDatos.Parametro("@Empresa", empresaId)
            };
            List<string> marcadores = new List<string>();
            for (int i = 0; i < ids.Count; i++)
            {
                marcadores.Add("@T" + i);
                parametros.Add(AccesoDatos.Parametro("@T" + i, ids[i]));
            }

            AccesoDatos.EjecutarComando(cn, tx,
                "UPDATE dbo.RelEmpresaPeriodicidad SET Activo = 0, FechaModificacion = GETDATE() " +
                "WHERE EmpresaId = @Empresa AND Activo = 1" +
                (marcadores.Count == 0 ? string.Empty : " AND PeriodicidadPagoId NOT IN (" + string.Join(",", marcadores) + ")"),
                parametros.ToArray());
        }
    }
}
