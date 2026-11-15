using System.Collections.Generic;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;

namespace NomiMex.Negocio
{
    /// <summary>Reglas del catálogo de Departamentos (CatDepartamento).</summary>
    public class DepartamentoBLL : BllBase
    {
        private readonly DepartamentoDAL _dal = new DepartamentoDAL();

        public List<CatDepartamento> Listar(int empresaId, bool soloActivos)
        {
            return Ejecutar(() => _dal.Listar(empresaId, soloActivos));
        }

        public List<CatDepartamento> Filtrar(IEnumerable<CatDepartamento> departamentos, string texto)
        {
            return FiltroBusqueda.Filtrar(departamentos, texto, d => d.Clave, d => d.Descripcion);
        }

        public int Guardar(CatDepartamento departamento)
        {
            ControlAcceso.Exigir(ModuloSistema.Departamentos, AccionSistema.Escribir);

            if (departamento.EmpresaId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione la empresa del departamento.");
            }

            departamento.Clave = Validador.Requerido(departamento.Clave, "Clave", 20).ToUpperInvariant();
            departamento.Descripcion = Validador.Requerido(departamento.Descripcion, "Descripción", 100);

            if (Ejecutar(() => _dal.ExisteClave(departamento.EmpresaId, departamento.Clave, departamento.DepartamentoId)))
            {
                throw new ExcepcionNegocio("Ya existe un departamento con la clave '" + departamento.Clave + "' en la empresa seleccionada.");
            }

            if (departamento.DepartamentoId == 0)
            {
                return Ejecutar(() => _dal.Insertar(departamento));
            }

            if (!departamento.Activo)
            {
                ValidarSinEmpleadosActivos(departamento.DepartamentoId);
            }
            Ejecutar(() => _dal.Actualizar(departamento));
            return departamento.DepartamentoId;
        }

        public void DesactivarLogico(int departamentoId)
        {
            ControlAcceso.Exigir(ModuloSistema.Departamentos, AccionSistema.Escribir);
            if (departamentoId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione un departamento.");
            }
            ValidarSinEmpleadosActivos(departamentoId);
            Ejecutar(() => _dal.DesactivarLogico(departamentoId));
        }

        private void ValidarSinEmpleadosActivos(int departamentoId)
        {
            int empleados = Ejecutar(() => _dal.ContarEmpleadosActivos(departamentoId));
            if (empleados > 0)
            {
                throw new ExcepcionNegocio("No se puede desactivar el departamento: tiene " + empleados + " empleado(s) activo(s).");
            }
        }
    }
}
