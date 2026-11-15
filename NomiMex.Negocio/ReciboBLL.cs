using System.Collections.Generic;
using System.Linq;
using NomiMex.Datos;
using NomiMex.Entidades.Documentos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>Consulta de recibos de nómina generados (DocReciboNomina / DocReciboDetalle).</summary>
    public class ReciboBLL : BllBase
    {
        private readonly NominaDAL _nominaDal = new NominaDAL();
        private readonly ReciboDAL _reciboDal = new ReciboDAL();
        private readonly PeriodoDAL _periodoDal = new PeriodoDAL();

        /// <summary>Periodos de la empresa (y periodicidad, si se indica) que ya tienen recibos calculados.</summary>
        public List<PeriodoResumen> ListarPeriodosConRecibos(int empresaId, int? tipoPeriodoId)
        {
            if (empresaId <= 0)
            {
                return new List<PeriodoResumen>();
            }
            return Ejecutar(() => _periodoDal.ListarPorEmpresa(empresaId))
                .Where(p => p.TotalEmpleados > 0 && (!tipoPeriodoId.HasValue || p.PeriodicidadPagoId == tipoPeriodoId.Value))
                .ToList();
        }

        public List<ReciboListado> ListarPorPeriodo(int periodoId)
        {
            if (periodoId <= 0)
            {
                return new List<ReciboListado>();
            }
            return Ejecutar(() => _nominaDal.ListarRecibos(periodoId));
        }

        public List<ReciboListado> Filtrar(IEnumerable<ReciboListado> recibos, string texto)
        {
            return FiltroBusqueda.Filtrar(recibos, texto, r => r.NumeroEmpleado, r => r.NombreEmpleado, r => r.Puesto, r => r.Departamento, r => r.Estatus);
        }

        public List<DocReciboDetalle> ListarDetalle(int reciboId)
        {
            if (reciboId <= 0)
            {
                return new List<DocReciboDetalle>();
            }
            return Ejecutar(() => _reciboDal.ListarDetalle(reciboId));
        }

        public ReciboImpresion ObtenerRecibo(int reciboId)
        {
            ReciboImpresion recibo = Ejecutar(() => _reciboDal.ObtenerRecibo(reciboId));
            if (recibo == null)
            {
                throw new ExcepcionNegocio("El recibo seleccionado ya no existe.");
            }
            return recibo;
        }
    }
}
