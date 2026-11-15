using System.Collections.Generic;
using System;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Relaciones;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>Reglas del catálogo de Empresas (CatEmpresa) y de las periodicidades de pago que se les asignan (1:N).</summary>
    public class EmpresaBLL : BllBase
    {
        private readonly EmpresaDAL _dal = new EmpresaDAL();
        private readonly PeriodicidadDAL _periodicidadDal = new PeriodicidadDAL();
        private readonly CatalogoDAL _catalogoDal = new CatalogoDAL();

        public List<CatEmpresa> Listar(bool soloActivas)
        {
            return Ejecutar(() => _dal.Listar(soloActivas));
        }

        public byte[] ObtenerLogo(int empresaId)
        {
            return empresaId <= 0 ? null : Ejecutar(() => _dal.ObtenerLogo(empresaId));
        }

                public List<CatEmpresa> Filtrar(IEnumerable<CatEmpresa> empresas, string texto)
        {
            return FiltroBusqueda.Filtrar(empresas, texto, e => e.RazonSocial, e => e.RFC, e => e.RegistroPatronal, e => e.CodigoPostal);
        }

        public List<EmpresaPeriodicidad> ListarPeriodicidades(int empresaId)
        {
            if (empresaId <= 0)
            {
                return new List<EmpresaPeriodicidad>();
            }
            return Ejecutar(() => _periodicidadDal.ListarPorEmpresa(empresaId, true));
        }

        /// <summary>Periodicidades activas de la empresa como elementos de lista desplegable (vacía si no hay empresa).</summary>
        public List<ItemCatalogo> ListarPeriodicidadesItems(int empresaId)
        {
            return ListarPeriodicidades(empresaId)
                .Select(p => new ItemCatalogo { Id = p.PeriodicidadPagoId, Clave = p.PeriodicidadClave, Descripcion = p.PeriodicidadDescripcion })
                .ToList();
        }

        /// <summary>Certificados de sello digital activos de la empresa (vacía si aún no se guarda).</summary>
        public List<EmpresaCertificado> ListarCertificados(int empresaId)
        {
            if (empresaId <= 0)
            {
                return new List<EmpresaCertificado>();
            }
            return Ejecutar(() => _dal.ListarCertificadosActivos(empresaId));
        }

        /// <summary>
        /// Valida el par .cer/.key y lo registra como el certificado activo de la empresa;
        /// el anterior, si existe, queda dado de baja. La contraseña se guarda cifrada.
        /// </summary>
        public void CargarCertificado(int empresaId, byte[] cer, byte[] key, string password)
        {
            ControlAcceso.Exigir(ModuloSistema.Empresa, AccionSistema.Escribir);
            if (empresaId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione una empresa de la lista antes de cargar su certificado.");
            }

            CatEmpresa empresa = Ejecutar(() => _dal.Listar(false)).FirstOrDefault(e => e.EmpresaId == empresaId);
            if (empresa == null || !empresa.Activa)
            {
                throw new ExcepcionNegocio("La empresa no existe o está inactiva.");
            }

            DatosCsd datos = ValidadorCsd.Validar(cer, key, password, empresa.RFC, DateTime.Today);

            byte[] passwordCifrada;
            byte[] iv;
            ProtectorSecretos.Cifrar(password, out passwordCifrada, out iv);

            HistEmpresaCSD csd = new HistEmpresaCSD
            {
                EmpresaId = empresaId,
                NoCertificado = datos.NoCertificado,
                CertificadoCSD = cer,
                LlaveCSD = key,
                PasswordCSDCifrada = passwordCifrada,
                PasswordCSDIV = iv,
                VigenciaDesde = datos.VigenciaDesde,
                VigenciaHasta = datos.VigenciaHasta
            };
            Ejecutar(() => _dal.ReemplazarCertificado(csd));
        }

        /// <summary>Indica si el origen de recurso exige capturar el monto de recurso propio (EntidadSNCF, origen mixto).</summary>
        public bool RequiereMontoRecursoPropio(string claveOrigenRecurso)
        {
            return claveOrigenRecurso == ClaveOrigenRecurso.RecursoMixto;
        }

        public int Guardar(CatEmpresa empresa, IList<int> tiposPeriodoIds)
        {
            ControlAcceso.Exigir(ModuloSistema.Empresa, AccionSistema.Escribir);

            empresa.RazonSocial = Validador.Requerido(empresa.RazonSocial, "Razón social", 200);
            empresa.RFC = Validador.RFC(empresa.RFC, "RFC", true, false);
            empresa.CURP = Validador.CURP(empresa.CURP, "CURP", false);
            empresa.RegistroPatronal = Validador.Requerido(empresa.RegistroPatronal, "Registro patronal", 20).ToUpperInvariant();
            empresa.RegimenFiscalId = Validador.SeleccionRequerida(empresa.RegimenFiscalId, "Régimen fiscal");
            empresa.CodigoPostal = Validador.CodigoPostal(empresa.CodigoPostal, "Código postal", true);
            empresa.LugarExpedicion = Validador.CodigoPostal(
                string.IsNullOrWhiteSpace(empresa.LugarExpedicion) ? empresa.CodigoPostal : empresa.LugarExpedicion,
                "Lugar de expedición", true);
            empresa.ClaveEntidadFederativa = Validador.Opcional(empresa.ClaveEntidadFederativa, "Estado", 3);
            empresa.NombreComercial = Validador.Opcional(empresa.NombreComercial, "Nombre comercial", 200);
            empresa.RfcPatronOrigen = Validador.RFC(empresa.RfcPatronOrigen, "RFC del patrón de origen", false, false);

            if (!Ejecutar(() => _catalogoDal.ListarOrigenesRecurso()).Contains(empresa.ClaveOrigenRecurso))
            {
                throw new ExcepcionNegocio("Seleccione un origen de recurso válido (IP, IF o IM).");
            }
            ValidarMontoRecursoPropio(empresa);
            ValidarRiesgoTrabajo(empresa);
            ValidarLogo(empresa);

            List<RelEmpresaPeriodicidad> periodicidades = ConstruirPeriodicidades(tiposPeriodoIds);

            if (empresa.EmpresaId == 0)
            {
                return Ejecutar(() => _dal.Insertar(empresa, periodicidades));
            }

            if (!empresa.Activa)
            {
                ValidarSinEmpleadosActivos(empresa.EmpresaId);
            }
            ValidarPeriodicidadesRetiradas(empresa.EmpresaId, periodicidades);
            Ejecutar(() => _dal.Actualizar(empresa, periodicidades));
            return empresa.EmpresaId;
        }

        public void DesactivarLogico(int empresaId)
        {
            ControlAcceso.Exigir(ModuloSistema.Empresa, AccionSistema.Escribir);
            if (empresaId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione una empresa.");
            }
            ValidarSinEmpleadosActivos(empresaId);
            Ejecutar(() => _dal.DesactivarLogico(empresaId));
        }

        private const int LogoMaxBytes = 2 * 1024 * 1024;

        // El logo se guarda solo cuando cambió; un arreglo vacío equivale a quitarlo
        private static void ValidarLogo(CatEmpresa empresa)
        {
            if (!empresa.LogoModificado)
            {
                empresa.Logo = null;
                return;
            }
            if (empresa.Logo != null && empresa.Logo.Length == 0)
            {
                empresa.Logo = null;
            }
            if (empresa.Logo != null && empresa.Logo.Length > LogoMaxBytes)
            {
                throw new ExcepcionNegocio("El logo no debe pesar más de 2 MB.");
            }
        }

        // El monto de recurso propio solo existe en el nodo EntidadSNCF cuando el origen del recurso es mixto
        private void ValidarMontoRecursoPropio(CatEmpresa empresa)
        {
            if (!RequiereMontoRecursoPropio(empresa.ClaveOrigenRecurso))
            {
                empresa.MontoRecursoPropio = null;
                return;
            }
            if (!empresa.MontoRecursoPropio.HasValue || empresa.MontoRecursoPropio.Value < 0m)
            {
                throw new ExcepcionNegocio("Capture el monto de recurso propio (cero o mayor); es obligatorio cuando el origen del recurso es IM.");
            }
            empresa.MontoRecursoPropio = decimal.Round(empresa.MontoRecursoPropio.Value, 2);
        }

        private void ValidarRiesgoTrabajo(CatEmpresa empresa)
        {
            if (empresa.PrimaRiesgoTrabajoPct.HasValue && !empresa.RiesgoPuestoId.HasValue)
            {
                throw new ExcepcionNegocio("Seleccione la clase de riesgo de trabajo para la prima capturada.");
            }
            if (empresa.RiesgoPuestoId.HasValue &&
                !Ejecutar(() => _catalogoDal.ListarRiesgosPuesto()).Any(r => r.Id == empresa.RiesgoPuestoId.Value))
            {
                throw new ExcepcionNegocio("El riesgo de trabajo seleccionado no existe o está inactivo.");
            }
            if (empresa.PrimaRiesgoTrabajoPct.HasValue && (empresa.PrimaRiesgoTrabajoPct.Value < 0.5m || empresa.PrimaRiesgoTrabajoPct.Value > 15m))
            {
                throw new ExcepcionNegocio("La prima de riesgo de trabajo debe estar entre 0.5% y 15%.");
            }
        }

        // Una empresa debe operar al menos una periodicidad; las nuevas nacen con la configuración predeterminada de su tipo
        private List<RelEmpresaPeriodicidad> ConstruirPeriodicidades(IList<int> tiposPeriodoIds)
        {
            if (tiposPeriodoIds == null || tiposPeriodoIds.Count == 0)
            {
                throw new ExcepcionNegocio("Asigne a la empresa al menos una periodicidad de pago (por ejemplo Semanal y/o Quincenal).");
            }

            List<ItemCatalogo> catalogo = Ejecutar(() => _catalogoDal.ListarTiposPeriodo());
            List<RelEmpresaPeriodicidad> resultado = new List<RelEmpresaPeriodicidad>();
            foreach (int id in tiposPeriodoIds.Distinct())
            {
                ItemCatalogo tipo = catalogo.FirstOrDefault(t => t.Id == id);
                if (tipo == null)
                {
                    throw new ExcepcionNegocio("La periodicidad seleccionada no existe o está inactiva.");
                }
                resultado.Add(GeneradorPeriodos.ConfiguracionPredeterminada(tipo.Clave, tipo.Id, tipo.Descripcion));
            }
            return resultado;
        }

        private void ValidarPeriodicidadesRetiradas(int empresaId, List<RelEmpresaPeriodicidad> deseadas)
        {
            foreach (EmpresaPeriodicidad actual in Ejecutar(() => _periodicidadDal.ListarPorEmpresa(empresaId, true)))
            {
                if (deseadas.Any(d => d.PeriodicidadPagoId == actual.PeriodicidadPagoId))
                {
                    continue;
                }

                int empleados = Ejecutar(() => _periodicidadDal.ContarEmpleadosConPeriodicidad(empresaId, actual.PeriodicidadPagoId));
                if (empleados > 0)
                {
                    throw new ExcepcionNegocio("No se puede quitar la periodicidad '" + actual.PeriodicidadDescripcion + "': " + empleados +
                                               " empleado(s) de la empresa la utilizan. Cámbieles la periodicidad primero.");
                }
            }
        }

        private void ValidarSinEmpleadosActivos(int empresaId)
        {
            int empleados = Ejecutar(() => _dal.ContarEmpleadosActivos(empresaId));
            if (empleados > 0)
            {
                throw new ExcepcionNegocio("No se puede desactivar la empresa: tiene " + empleados + " empleado(s) que no están en baja.");
            }
        }
    }
}
