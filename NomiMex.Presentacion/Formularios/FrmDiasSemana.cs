using System;
using System.Windows.Forms;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    /// <summary>
    /// Captura los días que el trabajador labora por semana (no es una incidencia): se puede ajustar solo para el periodo en
    /// proceso o dejar fijo en el empleado; sin valor se aplican los de su tipo de jornada.
    /// </summary>
    internal sealed partial class FrmDiasSemana : Form
    {
        public FrmDiasSemana(PreReciboDTO pre, PeriodoResumen periodo)
        {
            InitializeComponent();
            lblEncabezado.Text = pre.NombreCompleto + "\nVigente: " + DescribirActual(pre);
            AyudaCombos.CargarDiasLaborados(_cmbDias, "Según contrato / jornada (quita el ajuste del periodo)");
            AyudaCombos.SeleccionarId(_cmbDias, pre.DiasSemana);
            _rdoPeriodo.Text = "Solo para el Periodo #" + periodo.NumeroPeriodo;
            _rdoFijo.Text = "Dejar fijo en el empleado (aplica en todos los periodos siguientes)";
            _rdoFijo.Enabled = ControlAcceso.Puede(ModuloSistema.Empleados, AccionSistema.Escribir);
        }

        /// <summary>Verdadero si el valor se guarda fijo en el empleado; falso si solo aplica al periodo.</summary>
        public bool Fijo
        {
            get { return _rdoFijo.Checked; }
        }

        /// <summary>
        /// Días por semana a guardar (7 = jornada completa). Nulo regresa a los de su contrato o jornada: en el periodo quita
        /// el ajuste y en el empleado deja el valor sin definir.
        /// </summary>
        public int? Dias
        {
            get { return AyudaCombos.Id(_cmbDias); }
        }

        private static string DescribirActual(PreReciboDTO pre)
        {
            string dias = pre.DiasSemana.HasValue && pre.DiasSemana.Value < 7
                ? pre.DiasSemana.Value + (pre.DiasSemana.Value == 1 ? " día por semana" : " días por semana")
                : "jornada completa";
            return dias + (pre.DiasSemanaSoloPeriodo ? " (ajuste solo de este periodo)" : string.Empty);
        }
    }
}
