using System.Collections.Generic;
using System.Linq;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Renglón del pre-recibo (percepción, otro pago o deducción) con la misma estructura que DocReciboDetalle.</summary>
    public class PreReciboConceptoDTO
    {
        public int? IdIncidenciaConcepto { get; set; }

        /// <summary>Verdadero si la incidencia es permanente (se precarga en periodos siguientes).</summary>
        public bool EsPermanente { get; set; }

        /// <summary>Indicador para la columna del pre-recibo: "Permanente", "Solo periodo" o vacío.</summary>
        public string AlcanceTexto
        {
            get { return !IdIncidenciaConcepto.HasValue ? string.Empty : EsPermanente ? "Permanente" : "Solo periodo"; }
        }

        public string UnidadCaptura { get; set; }
        public string[] CodigosOrigen { get; set; }
        public List<int> IdsIncidenciasLegales { get; set; }
        /// <summary>P = percepción, O = otro pago, D = deducción.</summary>
        public string TipoConcepto { get; set; }

        public string Clave { get; set; }
        public string Descripcion { get; set; }

        /// <summary>Días u horas que originan el importe; nulo cuando el concepto no tiene cantidad.</summary>
        public decimal? Cantidad { get; set; }

        public decimal ImporteGravado { get; set; }
        public decimal ImporteExento { get; set; }

        public decimal Total
        {
            get { return ImporteGravado + ImporteExento; }
        }

        public string CantidadTexto
        {
            get
            {
                if (!Cantidad.HasValue)
                {
                    return string.Empty;
                }
                if (UnidadCaptura != null)
                    return Cantidad.Value.ToString("0.####") + UnidadCaptura;
                // La clave SAT 019 (horas extra) es el único concepto cuya cantidad son horas
                string unidad = TipoConcepto == "P" && Clave == "019" ? " h" : " d";
                return Cantidad.Value.ToString("0.##") + unidad;
            }
        }
    }

    /// <summary>
    /// Pre-recibo de un trabajador en un periodo: datos del trabajador, conceptos calculados por la capa de negocio,
    /// incidencias activas, notas de impresión y totales.
    /// </summary>
    public class PreReciboDTO
    {
        public PreReciboDTO()
        {
            Percepciones = new List<PreReciboConceptoDTO>();
            Deducciones = new List<PreReciboConceptoDTO>();
            Incidencias = new List<IncidenciaListado>();
            Estatus = NomiMex.Entidades.Constantes.EstatusTrabajadorNomina.Pendiente;
        }

        public int PeriodoId { get; set; }
        public int EmpleadoId { get; set; }

        /// <summary>Recibo guardado (borrador o definitivo) de este trabajador en el periodo; nulo si aún no existe.</summary>
        public int? ReciboId { get; set; }

        public string NumeroEmpleado { get; set; }
        public string NombreCompleto { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }

        public decimal SalarioDiario { get; set; }
        public decimal SDI { get; set; }
        public decimal SBC { get; set; }

        /// <summary>Días pagados: laborables efectivamente trabajados más el séptimo día proporcional.</summary>
        public decimal DiasPagados { get; set; }

        public decimal DiasSeptimo { get; set; }

        /// <summary>Días laborados por semana vigentes (contrato o ajuste del periodo); nulo = jornada completa.</summary>
        public int? DiasSemana { get; set; }

        /// <summary>Verdadero si DiasSemana es un ajuste solo de este periodo y no el valor fijo del contrato.</summary>
        public bool DiasSemanaSoloPeriodo { get; set; }

        /// <summary>Verdadero si la empresa omitió un concepto legal en el periodo: el recibo no se puede timbrar.</summary>
        public bool BloqueaTimbrado { get; set; }

        /// <summary>Conceptos que la empresa omite en el periodo (clave y descripción).</summary>
        public List<string> ConceptosOmitidos { get; set; }
        public decimal TotalPercepciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal NetoPagar { get; set; }
        public decimal TotalGravado { get; set; }
        public decimal TotalExento { get; set; }

        /// <summary>Pendiente, Con Incidencias o Calculado.</summary>
        public string Estatus { get; set; }

        /// <summary>Cantidad de incidencias activas del trabajador en el periodo.</summary>
        public int TotalIncidencias { get; set; }

        /// <summary>Notas que se imprimen en el recibo únicamente si contienen texto.</summary>
        public string Notas { get; set; }

        /// <summary>Verdadero si el periodo está abierto y el pre-recibo admite incidencias, notas y guardado.</summary>
        public bool EsEditable { get; set; }

        public List<PreReciboConceptoDTO> Percepciones { get; set; }
        public List<PreReciboConceptoDTO> Deducciones { get; set; }
        public List<IncidenciaListado> Incidencias { get; set; }

        public string PuestoDepartamento
        {
            get
            {
                string[] partes = new[] { Puesto, Departamento }.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
                return string.Join(" / ", partes);
            }
        }
    }
}
