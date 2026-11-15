using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    /// <summary>
    /// Configuración y generación anual de periodos por empresa y periodicidad, y catálogo de días festivos.
    /// Es independiente del proceso de nómina: aquí no se calcula nada, solo se prepara el calendario.
    /// </summary>
    public partial class FrmConfigPeriodos : Form
    {
        private const string Titulo = "Periodos y días festivos";
        private const int TipoFestivoLey = 1;
        private const int TipoFestivoEmpresa = 2;

        private readonly EmpresaBLL _empresaBll = new EmpresaBLL();
        private readonly PeriodoBLL _periodoBll = new PeriodoBLL();
        private readonly DiaFestivoBLL _festivoBll = new DiaFestivoBLL();
        private readonly PrerrequisitosNomina _prerrequisitos = new PrerrequisitosNomina();
        private bool _cargando;

        public FrmConfigPeriodos()
        {
            InitializeComponent();
        }

        private void FrmConfigPeriodos_Load(object sender, EventArgs e)
        {
            try
            {
                // Paso 1: sin una empresa activa no se configuran periodicidades, periodos ni festivos de empresa
                _prerrequisitos.ExigirEmpresaRegistrada();

                _cargando = true;
                nudEjercicio.Value = DateTime.Today.Year;
                nudAnioFestivo.Value = DateTime.Today.Year;

                List<ItemCatalogo> empresas = _empresaBll.Listar(true)
                    .Select(x => new ItemCatalogo { Id = x.EmpresaId, Clave = x.RFC, Descripcion = x.RazonSocial }).ToList();
                AyudaCombos.CargarCatalogo(cmbEmpresa, empresas);
                AyudaCombos.CargarCatalogo(cmbTipoPeriodo, new List<ItemCatalogo>());
                AyudaCombos.CargarDiasSemana(cmbDiaInicio, false);
                AyudaCombos.CargarDiasSemana(cmbDiaFin, false);
                AyudaCombos.CargarDiasSemana(cmbDiaPago, false);
                AyudaCombos.CargarDiasSemana(cmbDiaDescanso, true);

                List<ItemCatalogo> filtro = new List<ItemCatalogo> { new ItemCatalogo { Id = 0, Descripcion = "(Todas las empresas)" } };
                filtro.AddRange(empresas);
                cmbEmpresaFiltroFestivo.DataSource = filtro;
                cmbEmpresaFiltroFestivo.DisplayMember = "Texto";
                cmbEmpresaFiltroFestivo.ValueMember = "Id";
                AyudaCombos.CargarCatalogo(cmbEmpresaNuevoFestivo, empresas);

                cmbTipoFestivo.DataSource = new List<ItemCatalogo>
                {
                    new ItemCatalogo { Id = TipoFestivoLey, Descripcion = "De ley (aplica a todas las empresas)" },
                    new ItemCatalogo { Id = TipoFestivoEmpresa, Descripcion = "De empresa (solo a la empresa elegida)" }
                };
                cmbTipoFestivo.DisplayMember = "Texto";
                cmbTipoFestivo.ValueMember = "Id";
                cmbTipoFestivo.SelectedIndex = 0;
                cmbEmpresaNuevoFestivo.Enabled = false;

                bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Periodos, AccionSistema.Escribir);
                grpConfiguracion.Enabled = puedeEscribir;
                grpNuevoFestivo.Enabled = puedeEscribir;
                btnGenerarLey.Enabled = puedeEscribir;
                _cargando = false;

                if (empresas.Count == 1)
                {
                    cmbEmpresa.SelectedIndex = 1;
                }
                ActualizarPeriodicidades();
                CargarFestivos();
            }
            catch (Exception ex)
            {
                _cargando = false;
                Enabled = false;
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        #region Periodos del ejercicio

        private int EmpresaSeleccionada()
        {
            return AyudaCombos.Id(cmbEmpresa) ?? 0;
        }

        private ItemCatalogo PeriodicidadSeleccionada()
        {
            ItemCatalogo item = cmbTipoPeriodo.SelectedItem as ItemCatalogo;
            return item == null || item.Id <= 0 ? null : item;
        }

        // Paso 2: solo se ofrecen las periodicidades asignadas a la empresa
        private void ActualizarPeriodicidades()
        {
            int empresaId = EmpresaSeleccionada();
            List<ItemCatalogo> periodicidades = _empresaBll.ListarPeriodicidadesItems(empresaId);

            _cargando = true;
            AyudaCombos.CargarCatalogo(cmbTipoPeriodo, periodicidades);
            if (periodicidades.Count > 0)
            {
                cmbTipoPeriodo.SelectedIndex = 1;
            }
            _cargando = false;

            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            int empresaId = EmpresaSeleccionada();
            ItemCatalogo tipo = PeriodicidadSeleccionada();
            bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Periodos, AccionSistema.Escribir);

            if (empresaId <= 0 || tipo == null)
            {
                btnGuardarConfig.Enabled = false;
                btnGenerar.Enabled = false;
                cmbDiaInicio.Enabled = false;
                cmbDiaPago.Enabled = false;
                lblNotaConfig.Text = empresaId > 0
                    ? "La empresa no tiene periodicidades de pago asignadas: edítela en el Catálogo de empresas y asigne al menos una."
                    : "Seleccione una empresa y una de sus periodicidades.";
                dgvPeriodos.DataSource = new List<PeriodoResumen>();
                lblResultado.Text = lblNotaConfig.Text;
                return;
            }

            EmpresaPeriodicidad config = _periodoBll.ObtenerConfiguracion(empresaId, tipo.Id);
            bool porSemana = GeneradorPeriodos.EsPorSemana(tipo.Clave);

            _cargando = true;
            AyudaCombos.SeleccionarDia(cmbDiaInicio, config.DiaInicio ?? (byte)1);
            AyudaCombos.SeleccionarDia(cmbDiaPago, config.DiaPago ?? (byte)5);
            AyudaCombos.SeleccionarDia(cmbDiaDescanso, config.DiaDescanso);
            chkCalculaSeptimo.Checked = config.CalculaSeptimoDia;
            _cargando = false;
            ActualizarDiaFin();

            cmbDiaInicio.Enabled = porSemana;
            cmbDiaPago.Enabled = porSemana;
            btnGuardarConfig.Enabled = puedeEscribir;
            btnGenerar.Enabled = puedeEscribir && GeneradorPeriodos.EsSoportada(tipo.Clave);
            lblNotaConfig.Text = DescribirRegla(tipo.Clave);
            CargarPeriodos();
        }

        private static string DescribirRegla(string clave)
        {
            switch (clave)
            {
                case ClavePeriodicidad.Semanal:
                    return "Semanal: 52 o 53 periodos. Cada semana inicia en el día de inicio y termina el día anterior; se paga el primer " +
                           "día de pago posterior al cierre. Cada periodo pertenece al ejercicio en que termina.";
                case ClavePeriodicidad.Catorcenal:
                    return "Catorcenal: 26 periodos de 14 días que continúan el calendario del ejercicio anterior; se paga el primer día de pago posterior al cierre.";
                case ClavePeriodicidad.Quincenal:
                    return "Quincenal: 24 periodos exactos. 1ra quincena del día 1 al 15 (inicio de mes) y 2da del 16 al último día real del mes " +
                           "(fin de mes: 28, 29, 30 o 31). Se paga el último día de cada quincena.";
                case ClavePeriodicidad.Mensual:
                    return "Mensual: 12 periodos, del día 1 al último día real de cada mes. Se paga el último día del mes.";
                case ClavePeriodicidad.Decenal:
                    return "Decenal: 36 periodos (1-10, 11-20 y 21-fin de mes). Se paga el último día de cada periodo.";
                default:
                    return "La generación automática no está disponible para esta periodicidad.";
            }
        }

        private void ActualizarDiaFin()
        {
            byte? inicio = AyudaCombos.Dia(cmbDiaInicio);
            AyudaCombos.SeleccionarDia(cmbDiaFin, inicio.HasValue ? (byte?)GeneradorPeriodos.DiaFinSemana(inicio.Value) : null);
        }

        private void CargarPeriodos()
        {
            int empresaId = EmpresaSeleccionada();
            ItemCatalogo tipo = PeriodicidadSeleccionada();
            if (empresaId <= 0 || tipo == null)
            {
                return;
            }

            int anio = (int)nudEjercicio.Value;
            List<PeriodoResumen> periodos = _periodoBll.ListarPorEmpresaTipo(empresaId, tipo.Id, anio);
            dgvPeriodos.DataSource = periodos;
            dgvPeriodos.ClearSelection();

            lblResultado.Text = periodos.Count == 0
                ? "No hay periodos generados para el ejercicio " + anio + ". Use 'Generar periodos del ejercicio'."
                : string.Format("Ejercicio {0}: {1} periodos de {2}  |  {3} con días festivos ({4} festivos vinculados)  |  {5} abiertos",
                    anio, periodos.Count, tipo.Descripcion, periodos.Count(p => p.TotalFestivos > 0), periodos.Sum(p => p.TotalFestivos),
                    periodos.Count(p => p.EstaAbierto));
        }

        private EmpresaPeriodicidad LeerConfiguracion()
        {
            return new EmpresaPeriodicidad
            {
                EmpresaId = EmpresaSeleccionada(),
                PeriodicidadPagoId = PeriodicidadSeleccionada() == null ? 0 : PeriodicidadSeleccionada().Id,
                DiaInicio = AyudaCombos.Dia(cmbDiaInicio),
                DiaPago = AyudaCombos.Dia(cmbDiaPago),
                DiaDescanso = AyudaCombos.Dia(cmbDiaDescanso),
                CalculaSeptimoDia = chkCalculaSeptimo.Checked
            };
        }

        private void cmbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                ActualizarPeriodicidades();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void cmbTipoPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                CargarConfiguracion();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void nudEjercicio_ValueChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                CargarPeriodos();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void cmbDiaInicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_cargando)
            {
                ActualizarDiaFin();
            }
        }

        private void btnGuardarConfig_Click(object sender, EventArgs e)
        {
            try
            {
                _periodoBll.GuardarConfiguracion(LeerConfiguracion());
                Mensajes.Informacion("La configuración se guardó. Los periodos ya generados no cambian hasta que vuelva a generar el ejercicio.", Titulo);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                ItemCatalogo tipo = PeriodicidadSeleccionada();
                if (EmpresaSeleccionada() <= 0 || tipo == null)
                {
                    Mensajes.Advertencia("Seleccione la empresa y una de sus periodicidades.", Titulo);
                    return;
                }

                int anio = (int)nudEjercicio.Value;
                if (!Mensajes.Confirmar("Se generarán todos los periodos " + tipo.Descripcion.ToLowerInvariant() + " del ejercicio " + anio +
                                        " con la configuración mostrada y se vincularán sus días festivos.\n\n" +
                                        "Los periodos que ya fueron procesados no se modifican. ¿Desea continuar?", Titulo))
                {
                    return;
                }

                Cursor = Cursors.WaitCursor;
                _periodoBll.GuardarConfiguracion(LeerConfiguracion());
                ResultadoGeneracionPeriodos r = _periodoBll.GenerarEjercicio(EmpresaSeleccionada(), tipo.Id, anio);
                CargarPeriodos();

                Mensajes.Informacion(string.Format(
                    "Ejercicio {0}: {1} periodos.\n\nNuevos: {2}\nActualizados: {3}\nOmitidos (ya procesados): {4}\nDías festivos vinculados: {5}",
                    r.Anio, r.TotalPeriodos, r.Creados, r.Actualizados, r.Omitidos, r.FestivosVinculados), Titulo);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void tabConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Al volver a la pestaña de periodos se refleja cualquier cambio en el catálogo de festivos
                if (tabConfig.SelectedTab == tabPeriodos && !_cargando)
                {
                    CargarPeriodos();
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        #endregion

        #region Días festivos

        private void CargarFestivos()
        {
            int? empresaId = AyudaCombos.Id(cmbEmpresaFiltroFestivo);
            dgvFestivos.DataSource = _festivoBll.Listar((int)nudAnioFestivo.Value, empresaId);
            dgvFestivos.ClearSelection();
        }

        private DiaFestivoListado FestivoSeleccionado()
        {
            return dgvFestivos.SelectedRows.Count == 0 ? null : dgvFestivos.SelectedRows[0].DataBoundItem as DiaFestivoListado;
        }

        private void nudAnioFestivo_ValueChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                CargarFestivos();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void cmbEmpresaFiltroFestivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                CargarFestivos();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void cmbTipoFestivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool esEmpresa = AyudaCombos.Id(cmbTipoFestivo) == TipoFestivoEmpresa;
            cmbEmpresaNuevoFestivo.Enabled = esEmpresa;
            if (!esEmpresa)
            {
                AyudaCombos.SeleccionarId(cmbEmpresaNuevoFestivo, null);
            }
        }

        private void btnGenerarLey_Click(object sender, EventArgs e)
        {
            try
            {
                int anio = (int)nudAnioFestivo.Value;
                int total = _festivoBll.GenerarFestivosLey(anio);
                CargarFestivos();
                Mensajes.Informacion("El año " + anio + " tiene " + total + " día(s) festivo(s) de ley. Los periodos abiertos se actualizaron.", Titulo);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void btnAgregarFestivo_Click(object sender, EventArgs e)
        {
            try
            {
                string tipo = AyudaCombos.Id(cmbTipoFestivo) == TipoFestivoEmpresa ? TipoDiaFestivo.Empresa : TipoDiaFestivo.Ley;
                _festivoBll.Agregar(dtpFechaFestivo.Value, txtDescripcionFestivo.Text, tipo, AyudaCombos.Id(cmbEmpresaNuevoFestivo));

                txtDescripcionFestivo.Clear();
                if ((int)nudAnioFestivo.Value != dtpFechaFestivo.Value.Year)
                {
                    nudAnioFestivo.Value = dtpFechaFestivo.Value.Year;
                }
                else
                {
                    CargarFestivos();
                }
                Mensajes.Informacion("El día festivo se agregó y se vinculó a su periodo.", Titulo);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void btnCambiarEstadoFestivo_Click(object sender, EventArgs e)
        {
            try
            {
                DiaFestivoListado festivo = FestivoSeleccionado();
                if (festivo == null)
                {
                    Mensajes.Advertencia("Seleccione un día festivo de la lista.", Titulo);
                    return;
                }

                _festivoBll.CambiarEstado(festivo.DiaFestivoId, !festivo.Activo);
                CargarFestivos();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void dgvFestivos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DiaFestivoListado festivo = dgvFestivos.Rows[e.RowIndex].DataBoundItem as DiaFestivoListado;
            if (festivo != null && !festivo.Activo)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }

        #endregion
    }
}
