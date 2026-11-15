using System.Collections.Generic;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;

namespace NomiMex.Negocio
{
    /// <summary>Reglas del catálogo de Puestos (CatPuesto).</summary>
    public class PuestoBLL : BllBase
    {
        private readonly PuestoDAL _dal = new PuestoDAL();

        public List<CatPuesto> Listar(int empresaId, bool soloActivos)
        {
            return Ejecutar(() => _dal.Listar(empresaId, soloActivos));
        }

        public List<CatPuesto> Filtrar(IEnumerable<CatPuesto> puestos, string texto)
        {
            return FiltroBusqueda.Filtrar(puestos, texto, p => p.Clave, p => p.Descripcion);
        }

        public int Guardar(CatPuesto puesto)
        {
            ControlAcceso.Exigir(ModuloSistema.Puestos, AccionSistema.Escribir);

            if (puesto.EmpresaId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione la empresa del puesto.");
            }

            puesto.Clave = Validador.Requerido(puesto.Clave, "Clave", 20).ToUpperInvariant();
            puesto.Descripcion = Validador.Requerido(puesto.Descripcion, "Descripción", 100);

            if (puesto.SalarioBandaMin.HasValue && puesto.SalarioBandaMin.Value < 0m ||
                puesto.SalarioBandaMax.HasValue && puesto.SalarioBandaMax.Value < 0m)
            {
                throw new ExcepcionNegocio("Los salarios de la banda no pueden ser negativos.");
            }
            if (puesto.SalarioBandaMin.HasValue && puesto.SalarioBandaMax.HasValue &&
                puesto.SalarioBandaMin.Value > puesto.SalarioBandaMax.Value)
            {
                throw new ExcepcionNegocio("El salario mínimo de la banda no puede ser mayor que el máximo.");
            }

            if (Ejecutar(() => _dal.ExisteClave(puesto.EmpresaId, puesto.Clave, puesto.PuestoId)))
            {
                throw new ExcepcionNegocio("Ya existe un puesto con la clave '" + puesto.Clave + "' en la empresa seleccionada.");
            }

            if (puesto.PuestoId == 0)
            {
                return Ejecutar(() => _dal.Insertar(puesto));
            }

            if (!puesto.Activo)
            {
                ValidarSinEmpleadosActivos(puesto.PuestoId);
            }
            Ejecutar(() => _dal.Actualizar(puesto));
            return puesto.PuestoId;
        }

        public void DesactivarLogico(int puestoId)
        {
            ControlAcceso.Exigir(ModuloSistema.Puestos, AccionSistema.Escribir);
            if (puestoId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione un puesto.");
            }
            ValidarSinEmpleadosActivos(puestoId);
            Ejecutar(() => _dal.DesactivarLogico(puestoId));
        }

        private void ValidarSinEmpleadosActivos(int puestoId)
        {
            int empleados = Ejecutar(() => _dal.ContarEmpleadosActivos(puestoId));
            if (empleados > 0)
            {
                throw new ExcepcionNegocio("No se puede desactivar el puesto: está asignado a " + empleados + " empleado(s) activo(s).");
            }
        }
    }
}
