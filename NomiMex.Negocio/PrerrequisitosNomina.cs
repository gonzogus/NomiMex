using System.Collections.Generic;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Precondiciones del flujo de nómina, en orden: 1) existe una empresa activa, 2) la empresa tiene al menos una periodicidad
    /// de pago asignada, 3) existen empleados activos para la combinación Empresa + Periodicidad. Cada método lanza una
    /// ExcepcionNegocio con el mensaje que debe verse en pantalla cuando el paso no se cumple.
    /// </summary>
    public class PrerrequisitosNomina : BllBase
    {
        private readonly EmpresaDAL _empresaDal = new EmpresaDAL();
        private readonly PeriodicidadDAL _periodicidadDal = new PeriodicidadDAL();
        private readonly EmpleadoDAL _empleadoDal = new EmpleadoDAL();

        public bool HayEmpresaActiva()
        {
            return Ejecutar(() => _empresaDal.ContarActivas()) > 0;
        }

        /// <summary>Paso 1: bloquea los módulos dependientes mientras no exista una empresa activa registrada.</summary>
        public void ExigirEmpresaRegistrada()
        {
            if (!HayEmpresaActiva())
            {
                throw new ExcepcionNegocio("No hay ninguna empresa activa registrada. Registre primero una empresa en el " +
                                           "Catálogo de empresas antes de configurar periodicidades, periodos, empleados o la nómina.");
            }
        }

        public void ExigirEmpresaActiva(int empresaId)
        {
            ExigirEmpresaRegistrada();
            if (empresaId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione una empresa.");
            }
            if (!Ejecutar(() => _empresaDal.EstaActiva(empresaId)))
            {
                throw new ExcepcionNegocio("La empresa seleccionada no está activa.");
            }
        }

        /// <summary>Paso 2: la empresa debe tener asignada al menos una periodicidad de pago. Devuelve las periodicidades activas.</summary>
        public List<EmpresaPeriodicidad> ExigirPeriodicidades(int empresaId)
        {
            ExigirEmpresaActiva(empresaId);
            List<EmpresaPeriodicidad> periodicidades = Ejecutar(() => _periodicidadDal.ListarPorEmpresa(empresaId, true));
            if (periodicidades.Count == 0)
            {
                throw new ExcepcionNegocio("La empresa no tiene periodicidades de pago asignadas. Edite la empresa en el Catálogo de " +
                                           "empresas y asigne al menos una (por ejemplo Semanal o Quincenal).");
            }
            return periodicidades;
        }

        public EmpresaPeriodicidad ExigirPeriodicidadDeEmpresa(int empresaId, int tipoPeriodoId)
        {
            List<EmpresaPeriodicidad> periodicidades = ExigirPeriodicidades(empresaId);
            if (tipoPeriodoId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione la periodicidad de pago.");
            }

            EmpresaPeriodicidad periodicidad = periodicidades.FirstOrDefault(p => p.PeriodicidadPagoId == tipoPeriodoId);
            if (periodicidad == null)
            {
                throw new ExcepcionNegocio("La periodicidad seleccionada no está asignada a la empresa. Periodicidades habilitadas: " +
                                           string.Join(", ", periodicidades.Select(p => p.PeriodicidadDescripcion)) + ".");
            }
            return periodicidad;
        }

        /// <summary>Paso 4: debe haber empleados activos (&gt; 0) asignados a la combinación Empresa + Periodicidad.</summary>
        public int ExigirEmpleadosActivos(int empresaId, int tipoPeriodoId)
        {
            EmpresaPeriodicidad periodicidad = ExigirPeriodicidadDeEmpresa(empresaId, tipoPeriodoId);
            int empleados = Ejecutar(() => _empleadoDal.ContarActivosPorPeriodicidad(empresaId, tipoPeriodoId));
            if (empleados == 0)
            {
                throw new ExcepcionNegocio("La empresa no tiene empleados activos con periodicidad '" + periodicidad.PeriodicidadDescripcion +
                                           "'. Registre empleados en el Catálogo de empleados antes de abrir o ejecutar la prenómina.");
            }
            return empleados;
        }
    }
}
