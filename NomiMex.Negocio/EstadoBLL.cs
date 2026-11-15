using System.Collections.Generic;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;

namespace NomiMex.Negocio
{
    /// <summary>Reglas del catálogo de Estados (CatEntidadFederativa).</summary>
    public class EstadoBLL : BllBase
    {
        private readonly EstadoDAL _dal = new EstadoDAL();

        public List<CatEntidadFederativa> Listar(bool soloActivos)
        {
            return Ejecutar(() => _dal.Listar(soloActivos));
        }

        public List<CatEntidadFederativa> Filtrar(IEnumerable<CatEntidadFederativa> estados, string texto)
        {
            return FiltroBusqueda.Filtrar(estados, texto, e => e.CveEntidad, e => e.Nombre, e => e.Abreviatura);
        }

        /// <param name="esNuevo">La clave es el identificador natural: en altas se verifica que no exista.</param>
        public void Guardar(CatEntidadFederativa estado, bool esNuevo)
        {
            ControlAcceso.Exigir(ModuloSistema.Estados, AccionSistema.Escribir);

            estado.CveEntidad = Validador.Requerido(estado.CveEntidad, "Clave", 3).ToUpperInvariant();
            estado.Nombre = Validador.Requerido(estado.Nombre, "Nombre", 100);
            estado.Abreviatura = Validador.Requerido(estado.Abreviatura, "Abreviatura", 10);

            if (esNuevo)
            {
                if (Ejecutar(() => _dal.Existe(estado.CveEntidad)))
                {
                    throw new ExcepcionNegocio("Ya existe un estado con la clave '" + estado.CveEntidad + "'.");
                }
                Ejecutar(() => _dal.Insertar(estado));
            }
            else
            {
                Ejecutar(() => _dal.Actualizar(estado));
            }
        }

        public void DesactivarLogico(string cveEntidad)
        {
            ControlAcceso.Exigir(ModuloSistema.Estados, AccionSistema.Escribir);
            if (string.IsNullOrWhiteSpace(cveEntidad))
            {
                throw new ExcepcionNegocio("Seleccione un estado.");
            }
            Ejecutar(() => _dal.DesactivarLogico(cveEntidad));
        }
    }
}
