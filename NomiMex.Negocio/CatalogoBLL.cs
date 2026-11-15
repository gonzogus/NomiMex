using System.Collections.Generic;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>Listas de catálogos de solo lectura para los ComboBox de la interfaz.</summary>
    public class CatalogoBLL : BllBase
    {
        private readonly CatalogoDAL _dal = new CatalogoDAL();

        public List<ItemCatalogo> ListarTiposPeriodo() { return Ejecutar(() => _dal.ListarTiposPeriodo()); }
        public List<ItemCatalogo> ListarTiposContrato() { return Ejecutar(() => _dal.ListarTiposContrato()); }
        public List<ItemCatalogo> ListarTiposJornada() { return Ejecutar(() => _dal.ListarTiposJornada()); }
        public List<ItemCatalogo> ListarTiposRegimen() { return Ejecutar(() => _dal.ListarTiposRegimen()); }

        public List<JornadaDias> ListarJornadasConDias() { return Ejecutar(() => _dal.ListarJornadasConDias()); }

        /// <summary>Define los días por semana de un tipo de jornada (1 a 7); null o 0 lo deja sin definir.</summary>
        public void GuardarDiasJornada(int tipoJornadaId, int? dias)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            int? valor = dias.HasValue && dias.Value > 0 ? dias : null;
            if (valor.HasValue && valor.Value > 7)
            {
                throw new ExcepcionNegocio("Los días laborados por semana deben estar entre 1 y 7.");
            }
            Ejecutar(() => _dal.GuardarDiasJornada(tipoJornadaId, valor));
        }
        public List<ItemCatalogo> ListarRegimenesFiscales() { return Ejecutar(() => _dal.ListarRegimenesFiscales()); }
        public List<ItemCatalogo> ListarRiesgosPuesto() { return Ejecutar(() => _dal.ListarRiesgosPuesto()); }
        public List<ItemCatalogo> ListarBancos() { return Ejecutar(() => _dal.ListarBancos()); }
        public List<CatTipoIncidencia> ListarTiposIncidencia() { return Ejecutar(() => _dal.ListarTiposIncidencia()); }

        public List<string> ListarOrigenesRecurso() { return Ejecutar(() => _dal.ListarOrigenesRecurso()); }
        public List<ItemCatalogo> ListarOrigenesRecursoItems() { return Ejecutar(() => _dal.ListarOrigenesRecursoItems()); }

        public List<string> ListarEstadosCiviles()
        {
            return new List<string> { "SOLTERO", "CASADO", "DIVORCIADO", "VIUDO", "UNION_LIBRE", "CONCUBINATO" };
        }

        public List<string> ListarSexos()
        {
            return new List<string> { "H", "M", "X" };
        }

        public List<string> ListarEstatusEmpleado()
        {
            return new List<string> { EstatusEmpleado.Activo, EstatusEmpleado.Incapacidad, EstatusEmpleado.Suspendido, EstatusEmpleado.Baja };
        }

        public List<string> ListarZonasSMG()
        {
            return new List<string> { "GENERAL", "ZFN" };
        }
    }
}
