using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Relaciones;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Datos
{
    /// <summary>Acceso a dbo.CatEmpresa.</summary>
    public class EmpresaDAL
    {
        private const string ColumnasSelect =
            "EmpresaId, RazonSocial, RFC, CURP, RegistroPatronal, RegimenFiscalId, CodigoPostal, LugarExpedicion, " +
            "ClaveEntidadFederativa, ClaveOrigenRecurso, NombreComercial, RfcPatronOrigen, MontoRecursoPropio, RiesgoPuestoId, " +
            "PrimaRiesgoTrabajoPct, Activa, FechaAlta, FechaModificacion";

        public List<CatEmpresa> Listar(bool soloActivas)
        {
            string sql = "SELECT " + ColumnasSelect + " FROM dbo.CatEmpresa" +
                         (soloActivas ? " WHERE Activa = 1" : string.Empty) + " ORDER BY RazonSocial";
            DataTable tabla = AccesoDatos.Consultar(sql);
            List<CatEmpresa> lista = new List<CatEmpresa>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new CatEmpresa
                {
                    EmpresaId = Fila.Obtener<int>(fila, "EmpresaId"),
                    RazonSocial = Fila.Obtener<string>(fila, "RazonSocial"),
                    RFC = Fila.Obtener<string>(fila, "RFC"),
                    CURP = Fila.Obtener<string>(fila, "CURP"),
                    RegistroPatronal = Fila.Obtener<string>(fila, "RegistroPatronal"),
                    RegimenFiscalId = Fila.Obtener<int>(fila, "RegimenFiscalId"),
                    CodigoPostal = Fila.Obtener<string>(fila, "CodigoPostal"),
                    LugarExpedicion = Fila.Obtener<string>(fila, "LugarExpedicion"),
                    ClaveEntidadFederativa = Fila.Obtener<string>(fila, "ClaveEntidadFederativa"),
                    ClaveOrigenRecurso = Fila.Obtener<string>(fila, "ClaveOrigenRecurso"),
                    NombreComercial = Fila.Obtener<string>(fila, "NombreComercial"),
                    RfcPatronOrigen = Fila.Obtener<string>(fila, "RfcPatronOrigen"),
                    MontoRecursoPropio = Fila.Obtener<decimal?>(fila, "MontoRecursoPropio"),
                    RiesgoPuestoId = Fila.Obtener<int?>(fila, "RiesgoPuestoId"),
                    PrimaRiesgoTrabajoPct = Fila.Obtener<decimal?>(fila, "PrimaRiesgoTrabajoPct"),
                    Activa = Fila.Obtener<bool>(fila, "Activa"),
                    FechaAlta = Fila.Obtener<System.DateTime>(fila, "FechaAlta"),
                    FechaModificacion = Fila.Obtener<System.DateTime?>(fila, "FechaModificacion")
                });
            }
            return lista;
        }

        /// <summary>Inserta la empresa y sincroniza sus periodicidades de pago en una sola transacción.</summary>
        public int Insertar(CatEmpresa e, IList<RelEmpresaPeriodicidad> periodicidades)
        {
            return AccesoDatos.EjecutarEnTransaccion<int>(delegate(SqlConnection cn, SqlTransaction tx)
            {
                object id = AccesoDatos.Escalar(cn, tx,
                    "INSERT INTO dbo.CatEmpresa (RazonSocial, RFC, CURP, RegistroPatronal, RegimenFiscalId, CodigoPostal, " +
                    "LugarExpedicion, ClaveEntidadFederativa, ClaveOrigenRecurso, NombreComercial, RfcPatronOrigen, MontoRecursoPropio, " +
                    "RiesgoPuestoId, PrimaRiesgoTrabajoPct, Logo, Activa) " +
                    "VALUES (@RazonSocial, @RFC, @CURP, @RegPat, @Regimen, @CP, @Lugar, @Entidad, @Origen, @NombreComercial, @RfcOrigen, " +
                    "@MontoPropio, @Riesgo, @PrimaRT, @Logo, @Activa); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    ParametrosConLogo(e));
                int empresaId = System.Convert.ToInt32(id);
                PeriodicidadDAL.Sincronizar(cn, tx, empresaId, periodicidades);
                return empresaId;
            });
        }

        /// <summary>Actualiza la empresa y sincroniza sus periodicidades de pago en una sola transacción.</summary>
        public void Actualizar(CatEmpresa e, IList<RelEmpresaPeriodicidad> periodicidades)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.CatEmpresa SET RazonSocial = @RazonSocial, RFC = @RFC, CURP = @CURP, RegistroPatronal = @RegPat, " +
                    "RegimenFiscalId = @Regimen, CodigoPostal = @CP, LugarExpedicion = @Lugar, ClaveEntidadFederativa = @Entidad, " +
                    "ClaveOrigenRecurso = @Origen, NombreComercial = @NombreComercial, RfcPatronOrigen = @RfcOrigen, " +
                    "MontoRecursoPropio = @MontoPropio, RiesgoPuestoId = @Riesgo, PrimaRiesgoTrabajoPct = @PrimaRT, " +
                    "Activa = @Activa WHERE EmpresaId = @Id",
                    Parametros(e));
                if (e.LogoModificado)
                {
                    AccesoDatos.EjecutarComando(cn, tx, "UPDATE dbo.CatEmpresa SET Logo = @Logo WHERE EmpresaId = @Id",
                        AccesoDatos.Parametro("@Id", e.EmpresaId), ParametroLogo(e.Logo));
                }
                PeriodicidadDAL.Sincronizar(cn, tx, e.EmpresaId, periodicidades);
            });
        }

        /// <summary>Logo de la empresa (null si no tiene). Se consulta aparte para no cargar la imagen en los listados.</summary>
        public byte[] ObtenerLogo(int empresaId)
        {
            object logo = AccesoDatos.Escalar("SELECT Logo FROM dbo.CatEmpresa WHERE EmpresaId = @Id",
                AccesoDatos.Parametro("@Id", empresaId));
            return logo as byte[];
        }

        /// <summary>Certificados de sello digital activos de la empresa (sin contenido binario ni contraseña).</summary>
        public List<EmpresaCertificado> ListarCertificadosActivos(int empresaId)
        {
            DataTable tabla = AccesoDatos.Consultar(
                "SELECT CSDId, EmpresaId, NoCertificado, VigenciaDesde, VigenciaHasta, Activo FROM dbo.HistEmpresaCSD " +
                "WHERE EmpresaId = @Id AND Activo = 1 ORDER BY VigenciaHasta DESC",
                AccesoDatos.Parametro("@Id", empresaId));
            List<EmpresaCertificado> lista = new List<EmpresaCertificado>();
            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new EmpresaCertificado
                {
                    CSDId = Fila.Obtener<int>(fila, "CSDId"),
                    EmpresaId = Fila.Obtener<int>(fila, "EmpresaId"),
                    NoCertificado = Fila.Obtener<string>(fila, "NoCertificado"),
                    VigenciaDesde = Fila.Obtener<System.DateTime>(fila, "VigenciaDesde"),
                    VigenciaHasta = Fila.Obtener<System.DateTime>(fila, "VigenciaHasta"),
                    Activo = Fila.Obtener<bool>(fila, "Activo")
                });
            }
            return lista;
        }

        /// <summary>Da de baja el CSD activo de la empresa (si existe) e inserta el nuevo en una sola transacción.</summary>
        public void ReemplazarCertificado(HistEmpresaCSD c)
        {
            AccesoDatos.EjecutarEnTransaccion(delegate(SqlConnection cn, SqlTransaction tx)
            {
                AccesoDatos.EjecutarComando(cn, tx,
                    "UPDATE dbo.HistEmpresaCSD SET Activo = 0, FechaBaja = GETDATE(), MotivoBaja = @Motivo " +
                    "WHERE EmpresaId = @Empresa AND Activo = 1",
                    AccesoDatos.Parametro("@Empresa", c.EmpresaId),
                    AccesoDatos.Parametro("@Motivo", "Reemplazado por el certificado " + c.NoCertificado));
                AccesoDatos.EjecutarComando(cn, tx,
                    "INSERT INTO dbo.HistEmpresaCSD (EmpresaId, NoCertificado, CertificadoCSD, LlaveCSD, PasswordCSD_Encrypted, " +
                    "PasswordCSD_IV, VigenciaDesde, VigenciaHasta) " +
                    "VALUES (@Empresa, @NoCertificado, @Cer, @Key, @Password, @IV, @Desde, @Hasta)",
                    AccesoDatos.Parametro("@Empresa", c.EmpresaId),
                    AccesoDatos.Parametro("@NoCertificado", c.NoCertificado),
                    new SqlParameter("@Cer", SqlDbType.VarBinary, -1) { Value = c.CertificadoCSD },
                    new SqlParameter("@Key", SqlDbType.VarBinary, -1) { Value = c.LlaveCSD },
                    new SqlParameter("@Password", SqlDbType.VarBinary, 256) { Value = c.PasswordCSDCifrada },
                    new SqlParameter("@IV", SqlDbType.VarBinary, 16) { Value = c.PasswordCSDIV },
                    new SqlParameter("@Desde", SqlDbType.Date) { Value = c.VigenciaDesde.Date },
                    new SqlParameter("@Hasta", SqlDbType.Date) { Value = c.VigenciaHasta.Date });
            });
        }

        public int ContarActivas()
        {
            return System.Convert.ToInt32(AccesoDatos.Escalar("SELECT COUNT(1) FROM dbo.CatEmpresa WHERE Activa = 1"));
        }

        public bool EstaActiva(int empresaId)
        {
            return System.Convert.ToInt32(AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.CatEmpresa WHERE EmpresaId = @Id AND Activa = 1",
                AccesoDatos.Parametro("@Id", empresaId))) > 0;
        }

        public int ContarEmpleadosActivos(int empresaId)
        {
            object r = AccesoDatos.Escalar(
                "SELECT COUNT(1) FROM dbo.CatEmpleado WHERE EmpresaId = @Id AND Estatus <> 'BAJA'",
                AccesoDatos.Parametro("@Id", empresaId));
            return System.Convert.ToInt32(r);
        }

        public void DesactivarLogico(int empresaId)
        {
            AccesoDatos.EjecutarComando("UPDATE dbo.CatEmpresa SET Activa = 0 WHERE EmpresaId = @Id",
                AccesoDatos.Parametro("@Id", empresaId));
        }

        private static SqlParameter ParametroLogo(byte[] logo)
        {
            return new SqlParameter("@Logo", SqlDbType.VarBinary, -1)
            {
                Value = logo != null && logo.Length > 0 ? (object)logo : System.DBNull.Value
            };
        }

        private static SqlParameter[] ParametrosConLogo(CatEmpresa e)
        {
            return Parametros(e).Concat(new[] { ParametroLogo(e.Logo) }).ToArray();
        }

        private static System.Data.SqlClient.SqlParameter[] Parametros(CatEmpresa e)
        {
            return new[]
            {
                AccesoDatos.Parametro("@Id", e.EmpresaId),
                AccesoDatos.Parametro("@RazonSocial", e.RazonSocial),
                AccesoDatos.Parametro("@RFC", e.RFC),
                AccesoDatos.Parametro("@CURP", e.CURP),
                AccesoDatos.Parametro("@RegPat", e.RegistroPatronal),
                AccesoDatos.Parametro("@Regimen", e.RegimenFiscalId),
                AccesoDatos.Parametro("@CP", e.CodigoPostal),
                AccesoDatos.Parametro("@Lugar", e.LugarExpedicion),
                AccesoDatos.Parametro("@Entidad", e.ClaveEntidadFederativa),
                AccesoDatos.Parametro("@Origen", e.ClaveOrigenRecurso),
                AccesoDatos.Parametro("@NombreComercial", e.NombreComercial),
                AccesoDatos.Parametro("@RfcOrigen", e.RfcPatronOrigen),
                AccesoDatos.ParametroDecimal("@MontoPropio", e.MontoRecursoPropio, 18, 2),
                AccesoDatos.ParametroEntero("@Riesgo", e.RiesgoPuestoId),
                AccesoDatos.ParametroDecimal("@PrimaRT", e.PrimaRiesgoTrabajoPct, 7, 5),
                AccesoDatos.Parametro("@Activa", e.Activa)
            };
        }
    }
}
