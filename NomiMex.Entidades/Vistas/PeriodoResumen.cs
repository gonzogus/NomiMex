using System;
using System.Collections.Generic;
using System.Linq;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Periodo de nómina con su configuración, días festivos y totales calculados (lectura).</summary>
    public class PeriodoResumen
    {
        public PeriodoResumen()
        {
            Festivos = new List<CatDiaFestivo>();
        }

        public int PeriodoId { get; set; }
        public int EmpresaId { get; set; }
        public string Empresa { get; set; }
        public int EjercicioId { get; set; }
        public int Anio { get; set; }
        public int PeriodicidadPagoId { get; set; }
        public string PeriodicidadClave { get; set; }
        public string TipoPeriodo { get; set; }
        public int NumeroPeriodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal DiasPeriodo { get; set; }
        public bool EsInicioMes { get; set; }
        public bool EsFinMes { get; set; }
        public byte? DiaDescanso { get; set; }
        public bool CalculaSeptimoDia { get; set; }
        public string Estatus { get; set; }
        public int TotalEmpleados { get; set; }
        public decimal SumaPercepciones { get; set; }
        public decimal SumaDeducciones { get; set; }
        public decimal SumaNetoPagar { get; set; }
        public int TotalFestivos { get; set; }

        /// <summary>Días festivos que caen dentro del periodo (FechaFestivo BETWEEN FechaInicio AND FechaFin).</summary>
        public List<CatDiaFestivo> Festivos { get; set; }

        public string DiaDescansoTexto
        {
            get { return DiaSemanaISO.Nombre(DiaDescanso); }
        }

        public string FestivosTexto
        {
            get
            {
                return Festivos.Count == 0
                    ? "Sin festivos"
                    : string.Join("; ", Festivos.Select(f => f.Fecha.ToString("dd/MM") + " " + f.Descripcion));
            }
        }

        /// <summary>Verdadero mientras el periodo admite captura de incidencias y cálculo (aún no procesado).</summary>
        public bool EstaAbierto
        {
            get { return Estatus == EstatusPeriodo.Creado || Estatus == EstatusPeriodo.Prenomina; }
        }

        /// <summary>Estatus resumido para la lista del ejercicio fiscal: Abierto, En Proceso o Cerrado.</summary>
        public string EstatusGeneral
        {
            get
            {
                switch (Estatus)
                {
                    case EstatusPeriodo.Creado: return "Abierto";
                    case EstatusPeriodo.Prenomina:
                    case EstatusPeriodo.EnCalculo:
                    case EstatusPeriodo.Calculado:
                    case EstatusPeriodo.Autorizado: return "En Proceso";
                    case EstatusPeriodo.Cerrado:
                    case EstatusPeriodo.Timbrado: return "Cerrado";
                    case EstatusPeriodo.Cancelado: return "Cancelado";
                    default: return Estatus;
                }
            }
        }

        /// <summary>Texto visible en los ComboBox de selección de periodo.</summary>
        public string Descripcion
        {
            get
            {
                return string.Format("{0} #{1} | {2:dd/MM/yyyy} - {3:dd/MM/yyyy} | {4}",
                    TipoPeriodo, NumeroPeriodo, FechaInicio, FechaFin, Estatus);
            }
        }
    }
}
