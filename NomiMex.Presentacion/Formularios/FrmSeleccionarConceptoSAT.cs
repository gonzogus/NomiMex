using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    /// <summary>
    /// Selector del catálogo oficial SAT de percepciones y deducciones. Si el concepto elegido todavía no tiene regla de
    /// cálculo, permite definirla (forma de cálculo, factor y tratamiento fiscal) y lo habilita para capturarse.
    /// Con un trabajador y un periodo también captura el movimiento (cantidad o valor, observaciones y alcance permanente)
    /// y las incidencias legales (horas extra, faltas, permisos, descuentos).
    /// </summary>
    public partial class FrmSeleccionarConceptoSAT : Form
    {
        private readonly ConceptoIncidenciaBLL _bll = new ConceptoIncidenciaBLL();
        private readonly PreNominaBLL _preBll = new PreNominaBLL();
        private readonly string _tipoInicial;
        private readonly bool _modoCaptura;
        private PreReciboDTO _empleado;
        private BaseCalculoEmpleado _base;
        private readonly PeriodoResumen _periodo;
        private List<CatConcepto> _todos = new List<CatConcepto>();
        private HashSet<string> _codigosLegales = new HashSet<string>();
        private bool _cargando;
        private bool _cambioLegal;

        /// <summary>Concepto elegido y ya habilitado para capturarse; nulo si se canceló.</summary>
        public CatConcepto ConceptoSeleccionado { get; private set; }

        public FrmSeleccionarConceptoSAT() : this(null, null, null, null) { }

        public FrmSeleccionarConceptoSAT(string tipoInicial) : this(null, null, null, tipoInicial) { }

        /// <summary>Captura una percepción ("P") o deducción ("D") del trabajador en el periodo indicado.</summary>
        public FrmSeleccionarConceptoSAT(PreReciboDTO empleado, PeriodoResumen periodo, string tipo)
            : this(empleado, periodo, tipo, tipo) { }

        private FrmSeleccionarConceptoSAT(PreReciboDTO empleado, PeriodoResumen periodo, string tipo, string tipoInicial)
        {
            InitializeComponent();
            _empleado = empleado;
            _periodo = periodo;
            _tipoInicial = tipoInicial == "D" ? "D" : "P";
            _modoCaptura = empleado != null && periodo != null;
            if (!_modoCaptura)
            {
                ConfigurarSoloSelector();
            }
        }

        // Sin trabajador no hay captura: solo queda el catálogo y la ventana se reduce a su tamaño de selector.
        private void ConfigurarSoloSelector()
        {
            ClientSize = new Size(ClientSize.Width, ClientSize.Height - pnlEncabezado.Height - pnlCaptura.Height);
            pnlEncabezado.Visible = false;
            pnlCaptura.Visible = false;
            tabCaptura.TabPages.Remove(tabLegales);
        }

        private void FrmSeleccionarConceptoSAT_Load(object sender, EventArgs e)
        {
            try
            {
                _cargando = true;
                cmbFormaCalculo.DataSource = ConceptoIncidenciaBLL.ListarFormasCalculo();
                _todos = _bll.ListarCatalogo();
                cmbTipoConcepto.SelectedIndex = _tipoInicial == "D" ? 1 : 0;
                if (_modoCaptura)
                {
                    PrepararCaptura();
                }
                _cargando = false;
                AplicarFiltro();
                if (_modoCaptura)
                {
                    ActualizarLegal();
                    CargarLegales();
                }
            }
            catch (Exception ex)
            {
                _cargando = false;
                btnAceptar.Enabled = false;
                Mensajes.Mostrar(ex, "Catálogo SAT");
            }
        }

        private void PrepararCaptura()
        {
            bool percepcion = _tipoInicial == "P";
            _empleado = _preBll.ObtenerPreRecibo(_periodo.PeriodoId, _empleado.EmpleadoId, _empleado.Notas);
            _base = _bll.ObtenerBaseCalculo(_empleado.EmpleadoId);
            lblTituloTipo.Text = percepcion ? "Agregar percepción" : "Agregar deducción";
            pnlEncabezado.BackColor = percepcion ? Color.FromArgb(226, 240, 217) : Color.FromArgb(252, 228, 214);
            Text = lblTituloTipo.Text;
            btnAceptar.Text = "Guardar";
            lblInstruccion.Text = "Elija el concepto del catálogo oficial del SAT y capture su cantidad o valor. " +
                "Si todavía no tiene regla de cálculo, defínala para habilitarlo.";
            cmbTipoConcepto.Enabled = false;
            tabLegales.Text = percepcion ? "Horas extra, festivos y bonos" : "Faltas, permisos y descuentos";
            lblAyudaLegal.Text = percepcion
                ? "Horas extra, prima dominical, festivos y bonos: conservan sus reglas legales."
                : "Faltas, retardos, permisos, suspensiones y descuentos: conservan sus reglas legales.";
            lblEmpleado.Text = _empleado.NumeroEmpleado + " - " + _empleado.NombreCompleto;
            lblPeriodo.Text = string.Format("Periodo {0}: {1:dd/MM/yyyy} al {2:dd/MM/yyyy}",
                _periodo.NumeroPeriodo, _periodo.FechaInicio, _periodo.FechaFin);
            chkPermanente.Text = "Dejar permanente (aplica en todos los periodos siguientes); desmarcado aplica solo para el Periodo #" +
                _periodo.NumeroPeriodo;
            chkPermanente.Enabled = ControlAcceso.Puede(ModuloSistema.Empleados, AccionSistema.Escribir);
            _codigosLegales = new HashSet<string>(_bll.ListarTiposLegales(_tipoInicial, false).Select(t => t.Codigo));
            cmbTipoLegal.DataSource = _bll.ListarTiposLegales(_tipoInicial, true);
            dtpFechaLegal.MinDate = _periodo.FechaInicio.Date;
            dtpFechaLegal.MaxDate = _periodo.FechaFin.Date;
            dtpFechaLegal.Value = _periodo.FechaInicio.Date;
        }

        private string TipoActual()
        {
            return cmbTipoConcepto.SelectedIndex == 1 ? "D" : "P";
        }

        private CatConcepto Seleccionado()
        {
            return dgvCatalogo.SelectedRows.Count == 0 ? null : dgvCatalogo.SelectedRows[0].DataBoundItem as CatConcepto;
        }

        private void filtro_Changed(object sender, EventArgs e)
        {
            if (!_cargando) AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            string tipo = TipoActual();
            string texto = (txtBuscarConcepto.Text ?? string.Empty).Trim();
            List<CatConcepto> filtrados = _todos.Where(c => c.TipoConcepto == tipo &&
                (texto.Length == 0 ||
                 c.ClaveSAT.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0 ||
                 c.Descripcion.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
            _cargando = true;
            dgvCatalogo.DataSource = filtrados;
            _cargando = false;
            if (filtrados.Count > 0) dgvCatalogo.Rows[0].Selected = true;
            ActualizarRegla();
        }

        private void dgvCatalogo_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) ActualizarRegla();
        }

        private void dgvCatalogo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (_modoCaptura) nudCantidadOValor.Focus();
            else btnAceptar_Click(sender, EventArgs.Empty);
        }

        // Un concepto habilitado muestra su regla vigente (solo lectura); uno sin regla permite definirla.
        private void ActualizarRegla()
        {
            _cargando = true;
            try
            {
                CatConcepto c = Seleccionado();
                bool habilitado = c != null && c.EsIncidencia;
                bool editable = c != null && !c.EsIncidencia;
                cmbFormaCalculo.Enabled = editable;
                nudFactor.Enabled = editable;
                chkGravadoConcepto.Enabled = editable && c.TipoConcepto == "P";
                if (c == null)
                {
                    lblReglaEstado.Text = "No hay conceptos para el filtro indicado.";
                }
                else if (habilitado)
                {
                    cmbFormaCalculo.SelectedValue = c.FormaCalculo;
                    nudFactor.Value = Math.Min(Math.Max(c.FactorMultiplicador, nudFactor.Minimum), nudFactor.Maximum);
                    chkGravadoConcepto.Checked = c.EsGravado;
                    lblReglaEstado.Text = "Concepto habilitado: la regla de cálculo ya está definida.";
                }
                else
                {
                    cmbFormaCalculo.SelectedIndex = 0;
                    nudFactor.Value = 1m;
                    chkGravadoConcepto.Checked = true;
                    lblReglaEstado.Text = "Sin regla de cálculo: al seleccionarlo se habilitará con la regla indicada arriba." +
                        (c.TipoConcepto == "P" ? " Verifique el tratamiento fiscal (gravado o exento)." : string.Empty);
                }
            }
            finally
            {
                _cargando = false;
            }
            ActualizarCaptura();
        }

        private void captura_ValueChanged(object sender, EventArgs e)
        {
            if (!_cargando) ActualizarCaptura();
        }

        private void tabCaptura_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_cargando) ActualizarCaptura();
        }

        // Concepto tal como quedaría al guardar: si aún no tiene regla se usa la definida en pantalla.
        private CatConcepto ConceptoParaCaptura()
        {
            CatConcepto c = Seleccionado();
            if (c == null || c.EsIncidencia) return c;
            return new CatConcepto
            {
                ConceptoId = c.ConceptoId,
                ClaveSAT = c.ClaveSAT,
                TipoConcepto = c.TipoConcepto,
                Descripcion = c.Descripcion,
                FormaCalculo = cmbFormaCalculo.SelectedValue as string,
                FactorMultiplicador = nudFactor.Value,
                EsGravado = c.TipoConcepto == "D" || chkGravadoConcepto.Checked,
                EsIncidencia = true,
                Activo = true
            };
        }

        private decimal? ImporteActual(CatConcepto c)
        {
            if (_base == null || c == null) return null;
            try { return ConceptoIncidenciaBLL.CalcularImporte(c, nudCantidadOValor.Value, _base.SalarioDiario, _base.HorasJornada); }
            catch (ExcepcionNegocio) { return null; }
        }

        private void ActualizarCaptura()
        {
            if (!_modoCaptura || _base == null) return;

            CatConcepto c = ConceptoParaCaptura();
            lblCantidad.Text = ConceptoIncidenciaBLL.EtiquetaCantidad(c);
            lblFormula.Text = "Base: sueldo diario " + _base.SalarioDiario.ToString("C2") + " · jornada " +
                _base.HorasJornada.ToString("0.####") + " h" + (_base.HorasPredeterminadas ? " (predeterminada)" : string.Empty) +
                "\n" + ConceptoIncidenciaBLL.DescribirCalculo(c, nudCantidadOValor.Value, _base.SalarioDiario, _base.HorasJornada);
            if (c == null)
            {
                lblImporteCalculado.Text = "Seleccione un concepto.";
            }
            else
            {
                try
                {
                    lblImporteCalculado.Text = "Importe: " +
                        ConceptoIncidenciaBLL.CalcularImporte(c, nudCantidadOValor.Value, _base.SalarioDiario, _base.HorasJornada).ToString("C2");
                }
                catch (ExcepcionNegocio ex)
                {
                    lblImporteCalculado.Text = ex.Message;
                }
            }
            ActualizarTimbrado(c);
        }

        private void ActualizarTimbrado(CatConcepto c)
        {
            if (tabCaptura.SelectedTab != tabLegales)
            {
                if (c == null)
                {
                    lblClaveSAT.Text = "-";
                    lblTratamiento.Text = "-";
                    lblImportesFiscales.Text = "-";
                    return;
                }

                lblClaveSAT.Text = c.Texto;
                lblTratamiento.Text = _tipoInicial == "D" ? "Deducción (se resta del neto)"
                    : c.EsGravado ? "Gravado (integra la base del ISR)" : "Exento (no integra la base del ISR)";
                decimal? importe = ImporteActual(c);
                if (importe.HasValue)
                {
                    decimal gravado, exento;
                    ConceptoIncidenciaBLL.DesgloseFiscal(c, importe.Value, out gravado, out exento);
                    lblImportesFiscales.Text = "Gravado " + gravado.ToString("C2") + "   Exento " + exento.ToString("C2");
                }
                else
                {
                    lblImportesFiscales.Text = "-";
                }
                return;
            }

            CatTipoIncidencia t = cmbTipoLegal.SelectedItem as CatTipoIncidencia;
            string clave = ConceptoIncidenciaBLL.ClaveSATLegal(t);
            lblClaveSAT.Text = t == null ? "-" : clave != null ? "Clave SAT " + clave
                : "Sin concepto en el recibo: reduce los días pagados";
            lblTratamiento.Text = t == null || clave == null ? "-" : _tipoInicial == "D" ? "Deducción (se resta del neto)"
                : "Gravado y exento según las reglas de la LFT y la LISR";
            lblImportesFiscales.Text = clave == null ? "-" : "Se determinan al guardar y recalcular el recibo";
        }

        private void cmbTipoLegal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_cargando) ActualizarLegal();
        }

        private void ActualizarLegal()
        {
            CatTipoIncidencia tipo = cmbTipoLegal.SelectedItem as CatTipoIncidencia;
            lblUnidadLegal.Text = _preBll.UnidadDe(tipo);
            if (tipo != null && tipo.Codigo == CodigoIncidencia.DiaFestivoLaborado &&
                _periodo != null && _periodo.Festivos.Count > 0)
                dtpFechaLegal.Value = _periodo.Festivos[0].Fecha;
            ActualizarCaptura();
        }

        private void CargarLegales()
        {
            dgvLegales.DataSource = _preBll.ObtenerPreRecibo(_periodo.PeriodoId, _empleado.EmpleadoId, _empleado.Notas)
                .Incidencias.Where(i => _codigosLegales.Contains(i.Codigo)).ToList();
            dgvLegales.ClearSelection();
        }

        private void btnQuitarLegal_Click(object sender, EventArgs e)
        {
            try
            {
                IncidenciaListado i = dgvLegales.SelectedRows.Count == 0 ? null :
                    dgvLegales.SelectedRows[0].DataBoundItem as IncidenciaListado;
                if (i == null) throw new ExcepcionNegocio("Seleccione una incidencia legal.");
                if (!Mensajes.Confirmar("¿Quitar '" + i.TipoIncidencia + "' de este periodo?", "Incidencias")) return;
                _preBll.QuitarIncidencia(_periodo.PeriodoId, _empleado.EmpleadoId, i.IncidenciaId, _empleado.Notas);
                _cambioLegal = true;
                CargarLegales();
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Incidencias"); }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_modoCaptura)
                {
                    SeleccionarConcepto();
                    return;
                }
                if (tabCaptura.SelectedTab == tabLegales)
                {
                    GuardarIncidenciaLegal();
                    return;
                }

                CatConcepto c = Seleccionado();
                if (c == null) throw new ExcepcionNegocio("Seleccione un concepto del catálogo.");
                HabilitarSiFalta(c);
                _bll.Registrar(_periodo.PeriodoId, _empleado.EmpleadoId, c.ConceptoId,
                    nudCantidadOValor.Value, txtObservaciones.Text, chkPermanente.Checked);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, _modoCaptura ? "Agregar incidencias" : "Catálogo SAT"); }
        }

        private void HabilitarSiFalta(CatConcepto c)
        {
            if (!c.EsIncidencia)
            {
                _bll.HabilitarConcepto(c.ConceptoId, cmbFormaCalculo.SelectedValue as string, nudFactor.Value,
                    c.TipoConcepto == "D" || chkGravadoConcepto.Checked);
            }
        }

        private void SeleccionarConcepto()
        {
            CatConcepto c = Seleccionado();
            if (c == null) throw new ExcepcionNegocio("Seleccione un concepto del catálogo.");
            HabilitarSiFalta(c);
            ConceptoSeleccionado = _bll.ListarConceptos().Single(x => x.ConceptoId == c.ConceptoId);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void GuardarIncidenciaLegal()
        {
            string aviso;
            _preBll.AgregarIncidencia(_periodo.PeriodoId, _empleado.EmpleadoId,
                cmbTipoLegal.SelectedItem as CatTipoIncidencia, nudCantidadLegal.Value, dtpFechaLegal.Value,
                txtObservaciones.Text, _empleado.Notas, out aviso);
            if (aviso != null) Mensajes.Informacion(aviso, "Incidencias");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FrmSeleccionarConceptoSAT_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Una eliminación ya confirmada debe refrescar el recibo aunque se cierre sin agregar otro concepto.
            if (_cambioLegal) DialogResult = DialogResult.OK;
        }
    }
}
