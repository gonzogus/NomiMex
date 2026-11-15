using System;
using System.Collections.Generic;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Relaciones;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Reglas de la asignación de paquetes de incidencias por empresa y periodo. Cada empresa tiene su propia política: solo se
    /// pueden asignar paquetes activos que sean generales o de esa empresa y de la periodicidad del periodo elegido, y solo
    /// mientras el periodo siga abierto.
    /// </summary>
    public class AsignacionPaqueteBLL : BllBase
    {
        private readonly AsignacionPaqueteDAL _dal = new AsignacionPaqueteDAL();
        private readonly EmpresaBLL _empresaBll = new EmpresaBLL();
        private readonly PeriodoBLL _periodoBll = new PeriodoBLL();

        /// <summary>Empresas activas registradas.</summary>
        public List<CatEmpresa> ListarEmpresas()
        {
            return _empresaBll.Listar(true);
        }

        /// <summary>Periodos de la empresa seleccionada y únicamente de ella.</summary>
        public List<PeriodoResumen> ListarPeriodos(int empresaId)
        {
            Validador.SeleccionRequerida(empresaId, "Empresa");
            return _periodoBll.ListarPorEmpresa(empresaId);
        }

        /// <summary>Paquetes que la política de la empresa permite en el periodo, con su estado de asignación.</summary>
        public List<PaqueteAsignacion> ListarPaquetes(int empresaId, int periodoId)
        {
            ExigirPeriodoDeEmpresa(empresaId, periodoId);
            return Ejecutar(() => _dal.ListarPaquetes(empresaId, periodoId));
        }

        /// <summary>Asignaciones guardadas de la empresa en el periodo.</summary>
        public List<RelEmpresaPeriodoPaquete> ListarAsignaciones(int empresaId, int periodoId)
        {
            ExigirPeriodoDeEmpresa(empresaId, periodoId);
            return Ejecutar(() => _dal.ListarAsignaciones(empresaId, periodoId));
        }

        /// <summary>Conceptos de los paquetes asignados a la empresa en el periodo, con su estado de omisión.</summary>
        public List<ConceptoPeriodoPaquete> ListarConceptos(int empresaId, int periodoId)
        {
            ExigirPeriodoDeEmpresa(empresaId, periodoId);
            return Ejecutar(() => _dal.ListarConceptosPeriodo(empresaId, periodoId));
        }

        /// <summary>Conceptos que la empresa omite en el periodo (base y no base), para aplicarlos al cálculo del recibo.</summary>
        public List<ConceptoPeriodoPaquete> ListarOmitidos(int periodoId)
        {
            return Ejecutar(() => _dal.ListarOmitidos(periodoId));
        }

        /// <summary>
        /// Conceptos legales omitidos en el periodo: mientras existan, los recibos del periodo no se pueden timbrar porque no
        /// cumplen los requisitos del CFDI de nómina.
        /// </summary>
        public List<ConceptoPeriodoPaquete> ListarOmisionesQueBloqueanTimbrado(int periodoId)
        {
            return ListarOmitidos(periodoId).Where(c => c.EsLegal).ToList();
        }

        /// <summary>
        /// Guarda los paquetes de la empresa en el periodo (IdEmpresa, IdPeriodo, IdPaquete) y los conceptos que omite, tras
        /// validar la política de la empresa en ese periodo exacto. Omitir un concepto legal exige un motivo y activa la bandera
        /// que impide timbrar el periodo. Una lista vacía de paquetes deja el periodo sin paquetes.
        /// </summary>
        public void Guardar(int empresaId, int periodoId, IEnumerable<int> paquetesIds, IEnumerable<RelEmpresaPeriodoConceptoOmitido> omisiones)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            PeriodoResumen periodo = ExigirPeriodoDeEmpresa(empresaId, periodoId);
            if (!periodo.EstaAbierto)
            {
                throw new ExcepcionNegocio("El periodo está " + periodo.Estatus + " y ya no admite cambios de paquetes.");
            }

            List<int> ids = (paquetesIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            List<RelEmpresaPeriodoConceptoOmitido> omitidos = ValidarOmisiones(empresaId, periodoId, omisiones);
            List<PaqueteAsignacion> permitidos = Ejecutar(() => _dal.ListarPaquetes(empresaId, periodoId));
            foreach (int id in ids)
            {
                PaqueteAsignacion paquete = permitidos.FirstOrDefault(p => p.IdPaquete == id);
                if (paquete == null)
                {
                    throw new ExcepcionNegocio("El paquete " + id + " no está permitido por la política de la empresa en este periodo.");
                }
                if (!paquete.Activo)
                {
                    throw new ExcepcionNegocio("El paquete '" + paquete.Nombre + "' está inactivo y no puede asignarse.");
                }
                if (paquete.EmpresaId.HasValue && paquete.EmpresaId.Value != empresaId)
                {
                    throw new ExcepcionNegocio("El paquete '" + paquete.Nombre + "' pertenece a otra empresa.");
                }
                if (paquete.PeriodicidadPagoId.HasValue && paquete.PeriodicidadPagoId.Value != periodo.PeriodicidadPagoId)
                {
                    throw new ExcepcionNegocio("El paquete '" + paquete.Nombre + "' es de otra periodicidad de pago que la del periodo (" +
                                               periodo.TipoPeriodo + ").");
                }
            }

            Ejecutar(() => _dal.Guardar(empresaId, periodoId, ids, omitidos, ConceptoIncidenciaBLL.CalcularImporte));
        }

        /// <summary>
        /// Periodos de la empresa que recibirían la configuración del periodo de origen: los demás periodos abiertos de la misma
        /// empresa y de la misma periodicidad de pago. Nunca incluye periodos de otras empresas.
        /// </summary>
        public List<PeriodoResumen> ListarPeriodosDestino(int empresaId, int periodoOrigenId)
        {
            PeriodoResumen origen = ExigirPeriodoDeEmpresa(empresaId, periodoOrigenId);
            return _periodoBll.ListarPorEmpresa(empresaId)
                .Where(p => p.EmpresaId == empresaId && p.PeriodicidadPagoId == origen.PeriodicidadPagoId &&
                            p.EstaAbierto && p.PeriodoId != periodoOrigenId)
                .OrderBy(p => p.FechaInicio)
                .ToList();
        }

        /// <summary>
        /// Guarda los paquetes y las omisiones en el periodo de origen y los replica en los demás periodos abiertos de la misma
        /// empresa y periodicidad, validando la política de la empresa en cada periodo. Cada periodo se guarda en su propia
        /// transacción: si uno falla, los demás se actualizan y el fallo se reporta.
        /// </summary>
        public ResultadoCopiaPaquetes CopiarAPeriodosAbiertos(int empresaId, int periodoOrigenId, IEnumerable<int> paquetesIds,
                                                              IEnumerable<RelEmpresaPeriodoConceptoOmitido> omisiones)
        {
            List<int> ids = (paquetesIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            List<RelEmpresaPeriodoConceptoOmitido> modelo = (omisiones ?? Enumerable.Empty<RelEmpresaPeriodoConceptoOmitido>()).ToList();

            Guardar(empresaId, periodoOrigenId, ids, Clonar(modelo));

            ResultadoCopiaPaquetes resultado = new ResultadoCopiaPaquetes();
            foreach (PeriodoResumen destino in ListarPeriodosDestino(empresaId, periodoOrigenId))
            {
                try
                {
                    Guardar(empresaId, destino.PeriodoId, ids, Clonar(modelo));
                    resultado.PeriodosActualizados++;
                }
                catch (ExcepcionNegocio ex)
                {
                    resultado.Fallos.Add("Periodo #" + destino.NumeroPeriodo + " (" + destino.Anio + "): " + ex.Message);
                }
            }
            return resultado;
        }

        // La validación completa los datos de cada omisión, así que cada periodo recibe sus propias copias
        private static List<RelEmpresaPeriodoConceptoOmitido> Clonar(IEnumerable<RelEmpresaPeriodoConceptoOmitido> omisiones)
        {
            return omisiones.Select(o => new RelEmpresaPeriodoConceptoOmitido { ConceptoId = o.ConceptoId, Motivo = o.Motivo }).ToList();
        }

        // Cada concepto debe existir y estar activo; omitir uno legal exige un motivo (queda en la bandera que bloquea el timbrado)
        private List<RelEmpresaPeriodoConceptoOmitido> ValidarOmisiones(int empresaId, int periodoId, IEnumerable<RelEmpresaPeriodoConceptoOmitido> omisiones)
        {
            List<RelEmpresaPeriodoConceptoOmitido> lista = (omisiones ?? Enumerable.Empty<RelEmpresaPeriodoConceptoOmitido>())
                .GroupBy(o => o.ConceptoId).Select(g => g.First()).ToList();
            List<CatConcepto> conceptos = Ejecutar(() => _dal.ListarConceptos(lista.Select(o => o.ConceptoId)));

            foreach (RelEmpresaPeriodoConceptoOmitido omision in lista)
            {
                CatConcepto concepto = conceptos.FirstOrDefault(c => c.ConceptoId == omision.ConceptoId);
                if (concepto == null || !concepto.Activo)
                {
                    throw new ExcepcionNegocio("El concepto " + omision.ConceptoId + " no existe o está inactivo.");
                }

                omision.EmpresaId = empresaId;
                omision.PeriodoId = periodoId;
                omision.Motivo = Validador.Opcional(omision.Motivo, "Motivo de la omisión", 250);
                omision.EsLegal = concepto.EsBase || concepto.TipoConcepto == "I";
                omision.BloqueaTimbrado = omision.EsLegal;
                if (omision.EsLegal && (omision.Motivo == null || omision.Motivo.Length < 5))
                {
                    throw new ExcepcionNegocio("Indique el motivo (mínimo 5 caracteres) para omitir el concepto legal '" +
                                               concepto.ClaveSAT + " - " + concepto.Descripcion + "': el periodo ya no podrá timbrarse.");
                }
            }
            return lista;
        }

        // El periodo debe existir y pertenecer exactamente a la empresa: la política de paquetes depende de esa combinación
        private PeriodoResumen ExigirPeriodoDeEmpresa(int empresaId, int periodoId)
        {
            Validador.SeleccionRequerida(empresaId, "Empresa");
            Validador.SeleccionRequerida(periodoId, "Periodo");
            PeriodoResumen periodo = _periodoBll.ObtenerPorId(periodoId);
            if (periodo == null || periodo.EmpresaId != empresaId)
            {
                throw new ExcepcionNegocio("El periodo seleccionado no pertenece a la empresa.");
            }
            return periodo;
        }
    }
}
