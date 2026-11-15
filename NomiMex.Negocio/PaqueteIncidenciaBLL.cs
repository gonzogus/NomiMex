using System;
using System.Collections.Generic;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Relaciones;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Reglas de los paquetes de incidencias: conjunto de conceptos SAT que se asigna al trabajador en su alta según la
    /// periodicidad de su contrato. Todo paquete contiene los conceptos base y se personaliza con conceptos del catálogo SAT.
    /// </summary>
    public class PaqueteIncidenciaBLL : BllBase
    {
        private readonly PaqueteIncidenciaDAL _dal = new PaqueteIncidenciaDAL();
        private readonly ConceptoIncidenciaBLL _conceptos = new ConceptoIncidenciaBLL();

        public List<CatPaqueteIncidencia> Listar()
        {
            return Ejecutar(() => _dal.Listar());
        }

        public List<RelPaqueteConcepto> ListarConceptos(int paqueteId)
        {
            Validador.SeleccionRequerida(paqueteId, "Paquete");
            return Ejecutar(() => _dal.ListarConceptos(paqueteId));
        }

        public int Guardar(CatPaqueteIncidencia p)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            if (p == null) throw new ExcepcionNegocio("Falta el paquete.");
            p.Nombre = Validador.Requerido(p.Nombre, "Nombre del paquete", 100);
            if (p.EsDefault && !p.Activo)
                throw new ExcepcionNegocio("Un paquete inactivo no puede ser el predeterminado.");
            if (Listar().Any(x => x.IdPaquete != p.IdPaquete && string.Equals(x.Nombre, p.Nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ExcepcionNegocio("Ya existe un paquete con ese nombre.");
            return Ejecutar(() => _dal.Guardar(p));
        }

        /// <summary>Agrega al paquete un concepto del catálogo SAT ya habilitado, o actualiza su cantidad y permanencia.</summary>
        public void AgregarConcepto(int paqueteId, int conceptoId, decimal cantidad, bool permanente)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            Validador.SeleccionRequerida(paqueteId, "Paquete");
            CatConcepto c = _conceptos.ListarConceptos().FirstOrDefault(x => x.ConceptoId == conceptoId && x.Activo);
            if (c == null)
                throw new ExcepcionNegocio("El concepto no está habilitado: defina primero su regla de cálculo.");
            if (cantidad <= 0m || cantidad > 99999999999999.9999m)
                throw new ExcepcionNegocio("La cantidad debe ser positiva y caber en DECIMAL(18,4).");
            if (Math.Round(cantidad, 4) != cantidad)
                throw new ExcepcionNegocio("La cantidad admite como máximo cuatro decimales.");
            Ejecutar(() => _dal.AgregarConcepto(new RelPaqueteConcepto
            {
                IdPaquete = paqueteId, IdConceptoSAT = conceptoId, CantidadOValor = cantidad, EsPermanente = permanente
            }));
        }

        public void QuitarConcepto(int paqueteId, int conceptoId)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            Validador.SeleccionRequerida(paqueteId, "Paquete");
            Validador.SeleccionRequerida(conceptoId, "Concepto");
            Ejecutar(() => _dal.QuitarConcepto(paqueteId, conceptoId));
        }
    }
}
