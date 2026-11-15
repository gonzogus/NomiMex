using System;
using System.Collections.Generic;

using NomiMex.Entidades.Documentos;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Recibo completo listo para consulta o impresión: cabecera, empresa, empleado y conceptos.</summary>
    public class ReciboImpresion
    {
        public ReciboImpresion()
        {
            Recibo = new DocReciboNomina();
            Detalles = new List<DocReciboDetalle>();
        }

        public DocReciboNomina Recibo { get; set; }
        public List<DocReciboDetalle> Detalles { get; set; }

        public string Empresa { get; set; }
        public string EmpresaRFC { get; set; }
        public string RegistroPatronal { get; set; }
        /// <summary>Imagen del logo de la empresa (null si no tiene); se dibuja en el encabezado del recibo.</summary>
        public byte[] EmpresaLogo { get; set; }
        public string NumeroEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string EmpleadoRFC { get; set; }
        public string EmpleadoCURP { get; set; }
        public string EmpleadoNSS { get; set; }
        public int NumeroPeriodo { get; set; }
        public string TipoPeriodo { get; set; }
        public DateTime FechaInicioPeriodo { get; set; }
        public DateTime FechaFinPeriodo { get; set; }
        public DateTime FechaPagoPeriodo { get; set; }
        public string EstatusPeriodo { get; set; }
    }
}
