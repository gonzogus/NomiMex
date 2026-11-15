using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    /// <summary>
    /// Proceso de nómina por trabajador. Izquierda: empresa, periodicidad, periodos del ejercicio, datos del periodo en cálculo
    /// y trabajadores del periodo. Derecha: pre-recibo tradicional del trabajador seleccionado con captura de incidencias,
    /// notas de impresión y totales. Todo cálculo y acceso a datos vive en NomiMex.Negocio.
    /// </summary>
    public partial class FrmProcNomina : Form
    {
        private const string Titulo = "Proceso de nómina";

        private readonly EmpresaBLL _empresaBll = new EmpresaBLL();
        private readonly PeriodoBLL _periodoBll = new PeriodoBLL();
        private readonly PreNominaBLL _preNominaBll = new PreNominaBLL();
        private readonly NominaBLL _nominaBll = new NominaBLL();
        private readonly PrerrequisitosNomina _prerrequisitos = new PrerrequisitosNomina();

        private List<CatEmpresa> _empresas = new List<CatEmpresa>();
        private List<PeriodoResumen> _periodos = new List<PeriodoResumen>();
        private List<PreReciboDTO> _preRecibos = new List<PreReciboDTO>();
        private PeriodoResumen _periodoActual;
        private PreReciboDTO _preActual;
        private ReciboImpresion _reciboVistaPrevia;
        private string _avisoDeteccion = string.Empty;
        private bool _cargando;

        public FrmProcNomina()
        {
            InitializeComponent();
        }

        private void FrmProcNomina_Load(object sender, EventArgs e)
        {
            bool anterior = _cargando;
            try
            {
                // Sin una empresa activa no hay nada que procesar
                _prerrequisitos.ExigirEmpresaRegistrada();

                _cargando = true;
                _empresas = _empresaBll.Listar(true);
                AyudaCombos.CargarCatalogo(cmbEmpresa, _empresas
                    .Select(x => new ItemCatalogo { Id = x.EmpresaId, Clave = x.RFC, Descripcion = x.RazonSocial }));
                AyudaCombos.CargarCatalogo(cmbPeriodicidad, new List<ItemCatalogo>());

                if (_empresas.Count > 0)
                {
                    cmbEmpresa.SelectedIndex = 1;
                }
                _cargando = anterior;

                MostrarEmpresa();
                ActualizarPeriodicidades();
            }
            catch (Exception ex)
            {
                _cargando = anterior;
                Enabled = false;
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        #region Empresa, periodicidad y periodos del ejercicio

        private void MostrarEmpresa()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            CatEmpresa empresa = _empresas.FirstOrDefault(x => x.EmpresaId == empresaId);
            lblRFC.Text = empresa == null ? string.Empty : empresa.RFC;
            lblRegistroPatronal.Text = empresa == null ? string.Empty : empresa.RegistroPatronal;
        }

        // Solo se ofrecen las periodicidades asignadas a la empresa
        private void ActualizarPeriodicidades()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            List<ItemCatalogo> periodicidades = _empresaBll.ListarPeriodicidadesItems(empresaId);

            bool anterior = _cargando;
            _cargando = true;
            AyudaCombos.CargarCatalogo(cmbPeriodicidad, periodicidades);
            if (periodicidades.Count > 0)
            {
                cmbPeriodicidad.SelectedIndex = 1;
            }
            _cargando = anterior;

            if (empresaId > 0 && periodicidades.Count == 0)
            {
                MostrarSinPeriodo("La empresa no tiene periodicidades de pago asignadas. Edite la empresa en el Catálogo de empresas y asigne al menos una.", true);
                return;
            }
            DetectarPeriodo();
        }

        // Posiciona el periodo vigente a la fecha de proceso (o el abierto más cercano) y lista el ejercicio fiscal completo
        private void DetectarPeriodo()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            int tipoPeriodoId = AyudaCombos.Id(cmbPeriodicidad) ?? 0;
            if (empresaId <= 0 || tipoPeriodoId <= 0)
            {
                MostrarSinPeriodo("Seleccione la empresa y su periodicidad de pago.", false);
                return;
            }

            DeteccionPeriodo deteccion = _periodoBll.DetectarPeriodo(empresaId, tipoPeriodoId, DateTime.Today);
            _avisoDeteccion = deteccion.Mensaje;
            if (deteccion.AnterioresPendientes > 0 && deteccion.Periodo != null)
            {
                _avisoDeteccion += " Hay " + deteccion.AnterioresPendientes + " periodo(s) anterior(es) sin procesar; puede elegirlos en la lista.";
            }

            if (deteccion.Periodo == null)
            {
                MostrarSinPeriodo(_avisoDeteccion, true);
                return;
            }

            lblAvisoPeriodo.ForeColor = deteccion.EsVigente ? Color.FromArgb(22, 101, 52) : Color.FromArgb(180, 83, 9);
            CargarPeriodos(deteccion.Periodo);
        }

        private void CargarPeriodos(PeriodoResumen seleccionar)
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            int tipoPeriodoId = AyudaCombos.Id(cmbPeriodicidad) ?? 0;
            _periodos = _periodoBll.ListarPorEmpresaTipo(empresaId, tipoPeriodoId, seleccionar.Anio);

            bool anterior = _cargando;
            _cargando = true;
            dgvPeriodosFiscal.DataSource = _periodos;
            dgvPeriodosFiscal.ClearSelection();
            int indice = _periodos.FindIndex(p => p.PeriodoId == seleccionar.PeriodoId);
            if (indice >= 0)
            {
                dgvPeriodosFiscal.Rows[indice].Selected = true;
                dgvPeriodosFiscal.CurrentCell = dgvPeriodosFiscal.Rows[indice].Cells[0];
            }
            _cargando = anterior;

            CargarPeriodoSeleccionado(null);
        }

        private void MostrarSinPeriodo(string mensaje, bool esAlerta)
        {
            bool anterior = _cargando;
            _cargando = true;
            _periodos = new List<PeriodoResumen>();
            dgvPeriodosFiscal.DataSource = _periodos;
            _cargando = anterior;

            _periodoActual = null;
            lblAvisoPeriodo.ForeColor = esAlerta ? Color.FromArgb(185, 28, 28) : Color.FromArgb(100, 116, 139);
            lblAvisoPeriodo.Text = mensaje;
            MostrarPeriodo(null);
            MostrarTrabajadores(new List<PreReciboDTO>(), null);
        }

        // Muestra el periodo seleccionado en la lista y carga los pre-recibos de sus trabajadores
        private void CargarPeriodoSeleccionado(int? empleadoId)
        {
            DataGridViewRow fila = dgvPeriodosFiscal.SelectedRows.Count > 0 ? dgvPeriodosFiscal.SelectedRows[0] : dgvPeriodosFiscal.CurrentRow;
            _periodoActual = fila == null ? null : fila.DataBoundItem as PeriodoResumen;
            if (_periodoActual == null)
            {
                MostrarSinPeriodo("No hay un periodo seleccionado.", false);
                return;
            }

            MostrarPeriodo(_periodoActual);
            List<PreReciboDTO> preRecibos;
            try
            {
                preRecibos = _preNominaBll.ListarPreRecibos(_periodoActual.PeriodoId);
                RefrescarEstatusPeriodo();
            }
            catch (ExcepcionNegocio ex)
            {
                if (ex.EsErrorTecnico)
                {
                    throw;
                }
                preRecibos = new List<PreReciboDTO>();
                lblAvisoPeriodo.ForeColor = Color.FromArgb(185, 28, 28);
                lblAvisoPeriodo.Text = ex.Message;
                MostrarTrabajadores(preRecibos, null);
                return;
            }

            lblAvisoPeriodo.ForeColor = _periodoActual.EstaAbierto ? Color.FromArgb(100, 116, 139) : Color.FromArgb(180, 83, 9);
            lblAvisoPeriodo.Text = _periodoActual.EstaAbierto
                ? _avisoDeteccion
                : "El periodo está " + _periodoActual.Estatus + ": sus recibos se muestran en modo de solo lectura.";
            MostrarTrabajadores(preRecibos, empleadoId);
        }

        // Datos del periodo en cálculo (solo lectura)
        private void MostrarPeriodo(PeriodoResumen periodo)
        {
            if (periodo == null)
            {
                txtNumeroPeriodo.Clear();
                txtEjercicio.Clear();
                txtFechaInicio.Clear();
                txtFechaFin.Clear();
                txtFechaPago.Clear();
                txtEstatusPeriodo.Clear();
                txtDiaDescanso.Clear();
                txtFestivos.Clear();
                chkCalculaSeptimo.Checked = false;
                return;
            }

            txtNumeroPeriodo.Text = periodo.NumeroPeriodo.ToString();
            txtEjercicio.Text = periodo.Anio.ToString();
            txtFechaInicio.Text = periodo.FechaInicio.ToString("dd/MM/yyyy");
            txtFechaFin.Text = periodo.FechaFin.ToString("dd/MM/yyyy");
            txtFechaPago.Text = periodo.FechaPago.ToString("dd/MM/yyyy");
            txtEstatusPeriodo.Text = periodo.EstatusGeneral;
            txtDiaDescanso.Text = periodo.DiaDescansoTexto;
            chkCalculaSeptimo.Checked = periodo.CalculaSeptimoDia;
            txtFestivos.Text = periodo.Festivos.Count == 0
                ? "Sin días festivos en el periodo"
                : string.Join(Environment.NewLine, periodo.Festivos.Select(f => f.Fecha.ToString("dd/MM/yyyy") + " " + f.Descripcion));
        }

        // Vuelve a leer el ejercicio y el periodo conservando la selección (estatus, totales y trabajadores)
        private void RecargarPeriodoActual()
        {
            if (_periodoActual == null)
            {
                return;
            }

            int periodoId = _periodoActual.PeriodoId;
            PeriodoResumen actualizado = _periodoBll.ObtenerPorId(periodoId);
            int? empleadoId = _preActual == null ? (int?)null : _preActual.EmpleadoId;
            CargarPeriodos(actualizado);
            if (empleadoId.HasValue && _preRecibos.Any(p => p.EmpleadoId == empleadoId.Value))
            {
                SeleccionarTrabajador(empleadoId.Value);
            }
        }

        // Refleja en la lista el cambio de estatus del periodo (p. ej. CREADO pasa a PRENOMINA al capturar una incidencia)
        private void RefrescarEstatusPeriodo()
        {
            if (_periodoActual == null)
            {
                return;
            }

            _periodoActual.Estatus = _periodoBll.ObtenerPorId(_periodoActual.PeriodoId).Estatus;
            txtEstatusPeriodo.Text = _periodoActual.EstatusGeneral;
            dgvPeriodosFiscal.Invalidate();
        }

        #endregion

        #region Trabajadores del periodo

        private void MostrarTrabajadores(List<PreReciboDTO> preRecibos, int? empleadoId)
        {
            _preRecibos = preRecibos;

            bool anterior = _cargando;
            _cargando = true;
            bsEmpleadosPeriodo.DataSource = _preRecibos;
            dgvEmpleadosPeriodo.ClearSelection();
            _cargando = anterior;

            grpTrabajadores.Text = "Trabajadores del periodo en proceso (" + _preRecibos.Count + ")";
            if (_preRecibos.Count == 0)
            {
                MostrarPreRecibo(null);
                return;
            }

            int indice = empleadoId.HasValue ? _preRecibos.FindIndex(p => p.EmpleadoId == empleadoId.Value) : 0;
            SeleccionarTrabajador(_preRecibos[indice >= 0 ? indice : 0].EmpleadoId);
        }

        private void SeleccionarTrabajador(int empleadoId)
        {
            int indice = _preRecibos.FindIndex(p => p.EmpleadoId == empleadoId);
            if (indice < 0)
            {
                return;
            }

            bool anterior = _cargando;
            _cargando = true;
            dgvEmpleadosPeriodo.ClearSelection();
            dgvEmpleadosPeriodo.Rows[indice].Selected = true;
            dgvEmpleadosPeriodo.CurrentCell = dgvEmpleadosPeriodo.Rows[indice].Cells[0];
            _cargando = anterior;

            RefrescarTrabajadorSeleccionado();
        }

        private PreReciboDTO TrabajadorSeleccionado()
        {
            DataGridViewRow fila = dgvEmpleadosPeriodo.SelectedRows.Count > 0 ? dgvEmpleadosPeriodo.SelectedRows[0] : dgvEmpleadosPeriodo.CurrentRow;
            return fila == null ? null : fila.DataBoundItem as PreReciboDTO;
        }

        // Recalcula en Negocio el pre-recibo del trabajador seleccionado (conservando sus notas en edición) y lo muestra
        private void RefrescarTrabajadorSeleccionado()
        {
            PreReciboDTO seleccionado = TrabajadorSeleccionado();
            if (seleccionado == null || _periodoActual == null)
            {
                MostrarPreRecibo(null);
                return;
            }

            AplicarPreRecibo(_preNominaBll.ObtenerPreRecibo(_periodoActual.PeriodoId, seleccionado.EmpleadoId, seleccionado.Notas));
        }

        // Sustituye el pre-recibo en la lista (actualiza su fila) y lo muestra en la parte derecha
        private void AplicarPreRecibo(PreReciboDTO preRecibo)
        {
            int indice = _preRecibos.FindIndex(p => p.EmpleadoId == preRecibo.EmpleadoId);
            if (indice >= 0)
            {
                bool anterior = _cargando;
                _cargando = true;
                _preRecibos[indice] = preRecibo;
                bsEmpleadosPeriodo.ResetItem(indice);
                _cargando = anterior;
            }
            MostrarPreRecibo(preRecibo);
        }

        private void dgvEmpleadosPeriodo_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                RefrescarTrabajadorSeleccionado();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void dgvEmpleadosPeriodo_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _preRecibos.Count || e.Value == null)
            {
                return;
            }

            if (e.ColumnIndex == colEmpEstatus.Index)
            {
                switch (e.Value as string)
                {
                    case EstatusTrabajadorNomina.Calculado: e.CellStyle.ForeColor = Color.FromArgb(22, 101, 52); break;
                    case EstatusTrabajadorNomina.ConIncidencias: e.CellStyle.ForeColor = Color.FromArgb(180, 83, 9); break;
                    default: e.CellStyle.ForeColor = Color.FromArgb(100, 116, 139); break;
                }
            }
            else if (e.ColumnIndex == colEmpNeto.Index && _preRecibos[e.RowIndex].NetoPagar < 0m)
            {
                e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28);
            }
        }

        #endregion

        #region Pre-recibo del trabajador (parte derecha)

        private void MostrarPreRecibo(PreReciboDTO preRecibo)
        {
            bool anterior = _cargando;
            _cargando = true;
            _preActual = preRecibo;

            if (preRecibo == null)
            {
                lblNombreTrabajador.Text = string.Empty;
                lblNumeroEmpleado.Text = string.Empty;
                lblRFCTrabajador.Text = string.Empty;
                lblCURP.Text = string.Empty;
                lblNSS.Text = string.Empty;
                lblPuestoDepartamento.Text = string.Empty;
                lblSalarioDiario.Text = string.Empty;
                lblSBC.Text = string.Empty;
                lblDiasPagados.Text = string.Empty;
                dgvPercepciones.DataSource = new List<PreReciboConceptoDTO>();
                dgvDeducciones.DataSource = new List<PreReciboConceptoDTO>();
                txtNotasRecibo.Clear();
                MostrarTotales(0m, 0m, 0m);
            }
            else
            {
                lblNombreTrabajador.Text = preRecibo.NombreCompleto;
                lblNumeroEmpleado.Text = preRecibo.NumeroEmpleado;
                lblRFCTrabajador.Text = preRecibo.RFC;
                lblCURP.Text = preRecibo.CURP;
                lblNSS.Text = preRecibo.NSS;
                lblPuestoDepartamento.Text = preRecibo.PuestoDepartamento;
                lblSalarioDiario.Text = preRecibo.SalarioDiario.ToString("C2");
                lblSBC.Text = preRecibo.SBC.ToString("C2");
                lblDiasPagados.Text = preRecibo.DiasPagados.ToString("0.##") +
                    (preRecibo.DiasSemana.HasValue && preRecibo.DiasSemana.Value < 7 ? " (" + preRecibo.DiasSemana.Value + " d/sem)" : string.Empty);
                dgvPercepciones.DataSource = preRecibo.Percepciones;
                dgvDeducciones.DataSource = preRecibo.Deducciones;
                txtNotasRecibo.Text = preRecibo.Notas ?? string.Empty;
                MostrarTotales(preRecibo.TotalPercepciones, preRecibo.TotalDeducciones, preRecibo.NetoPagar);
            }

            dgvPercepciones.ClearSelection();
            dgvDeducciones.ClearSelection();
            _cargando = anterior;
            ActualizarControles();
        }

        private void MostrarTotales(decimal percepciones, decimal deducciones, decimal neto)
        {
            lblTotalPercepciones.Text = percepciones.ToString("C2");
            lblTotalDeducciones.Text = deducciones.ToString("C2");
            lblTotalPagar.Text = neto.ToString("C2");
            lblTotalPagar.ForeColor = neto < 0m ? Color.FromArgb(185, 28, 28) : Color.FromArgb(22, 101, 52);
        }

        private void ActualizarControles()
        {
            string estatus = _periodoActual == null ? string.Empty : _periodoActual.Estatus;
            bool hayPeriodo = _periodoActual != null;
            bool hayTrabajador = _preActual != null;
            bool abierto = hayPeriodo && PeriodoBLL.PermiteCapturarIncidencias(estatus);
            bool editable = abierto && hayTrabajador && ControlAcceso.Puede(ModuloSistema.Nomina, AccionSistema.Escribir);

            txtNotasRecibo.ReadOnly = !editable;
            btnAgregarPercepcion.Enabled = editable;
            btnAgregarDeduccion.Enabled = editable;
            EstablecerBoton(btnGuardarPreRecibo, editable, Color.FromArgb(37, 99, 235));
            EstablecerBoton(btnVistaPrevia, hayTrabajador, Color.FromArgb(71, 85, 105));

            bool puedeCerrar = (abierto && _preRecibos.Count > 0) || PeriodoBLL.PermiteAutorizar(estatus) || PeriodoBLL.PermiteCerrar(estatus);
            EstablecerBoton(btnCerrarNomina, hayPeriodo && puedeCerrar && ControlAcceso.Puede(ModuloSistema.Nomina, AccionSistema.Autorizar),
                Color.FromArgb(22, 163, 74));
        }

        // Un botón plano conserva su color al deshabilitarse: se atenúa para que el estado sea evidente
        private static void EstablecerBoton(Button boton, bool habilitado, Color color)
        {
            boton.Enabled = habilitado;
            boton.BackColor = habilitado ? color : Color.FromArgb(148, 163, 184);
        }

        #endregion

        #region Selección de empresa, periodicidad y periodo

        private void cmbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                MostrarEmpresa();
                ActualizarPeriodicidades();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void cmbPeriodicidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                DetectarPeriodo();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        private void dgvPeriodosFiscal_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                CargarPeriodoSeleccionado(null);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Titulo);
            }
        }

        #endregion

        #region Incidencias

        private void AbrirIncidencias(string tipo)
        {
            try
            {
                if (_periodoActual == null || _preActual == null)
                {
                    Mensajes.Advertencia("Seleccione un periodo y un trabajador.", "Incidencias");
                    return;
                }

                using (var dialogo = new FrmSeleccionarConceptoSAT(_preActual, _periodoActual, tipo))
                {
                    if (dialogo.ShowDialog(this) == DialogResult.OK)
                    {
                        RefrescarEstatusPeriodo();
                        RefrescarTrabajadorSeleccionado();
                    }
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Incidencias");
            }
        }

        private void btnAgregarPercepcion_Click(object sender, EventArgs e) { AbrirIncidencias("P"); }
        private void btnAgregarDeduccion_Click(object sender, EventArgs e) { AbrirIncidencias("D"); }

        private void dgvRecibo_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            DataGridView grid = (DataGridView)sender;
            grid.ClearSelection();
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                grid.Rows[e.RowIndex].Selected = true;
            }
        }

        private void cmsMenuRecibo_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool editable = _preActual != null && _preActual.EsEditable && _periodoActual != null && _periodoActual.EstaAbierto;
            mnuAgregarIncidencia.Enabled = editable;
            mnuQuitarConcepto.Enabled = editable;
            mnuEditarDiasSemana.Enabled = editable;
        }

        private void mnuEditarDiasSemana_Click(object sender, EventArgs e)
        {
            try
            {
                if (_periodoActual == null || _preActual == null)
                {
                    Mensajes.Advertencia("Seleccione un periodo y un trabajador.", "Días laborados");
                    return;
                }

                using (var dialogo = new FrmDiasSemana(_preActual, _periodoActual))
                {
                    if (dialogo.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }
                    AplicarPreRecibo(_preNominaBll.CambiarDiasSemana(_periodoActual.PeriodoId, _preActual.EmpleadoId, dialogo.Dias, dialogo.Fijo, _preActual.Notas));
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Días laborados");
            }
        }

        private void mnuAgregarIncidencia_Click(object sender, EventArgs e)
        {
            AbrirIncidencias(cmsMenuRecibo.SourceControl == dgvDeducciones ? "D" : "P");
        }

        private void mnuQuitarConcepto_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridView grid = cmsMenuRecibo.SourceControl as DataGridView;
                PreReciboConceptoDTO incidencia = grid == null || grid.SelectedRows.Count == 0
                    ? null : grid.SelectedRows[0].DataBoundItem as PreReciboConceptoDTO;
                if (_periodoActual == null || _preActual == null || incidencia == null)
                {
                    Mensajes.Advertencia("Seleccione el renglón que desea quitar.", "Incidencias");
                    return;
                }
                if (!incidencia.IdIncidenciaConcepto.HasValue &&
                    (incidencia.IdsIncidenciasLegales == null || incidencia.IdsIncidenciasLegales.Count == 0))
                {
                    Mensajes.Advertencia("No se puede eliminar un concepto automático base. Las incidencias legales se administran desde el diálogo de captura.", "Incidencias");
                    return;
                }
                string detalle = incidencia.IdsIncidenciasLegales != null && incidencia.IdsIncidenciasLegales.Count > 1
                    ? " (incluye " + incidencia.IdsIncidenciasLegales.Count + " incidencias agrupadas)" : string.Empty;
                bool tambienPermanente = false;
                if (incidencia.IdIncidenciaConcepto.HasValue && incidencia.EsPermanente)
                {
                    DialogResult resp = MessageBox.Show(this,
                        "'" + incidencia.Descripcion + "' es un concepto permanente.\n\n" +
                        "Sí: quitarlo también de la configuración permanente del empleado.\n" +
                        "No: quitarlo solo del Periodo #" + _periodoActual.NumeroPeriodo + ".",
                        "Incidencias", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (resp == DialogResult.Cancel) return;
                    tambienPermanente = resp == DialogResult.Yes;
                }
                else if (MessageBox.Show(this, "Se quitará '" + incidencia.Descripcion + detalle +
                    "' únicamente de este periodo. ¿Desea continuar?", "Incidencias",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                AplicarPreRecibo(_preNominaBll.QuitarConcepto(_periodoActual.PeriodoId, _preActual.EmpleadoId, incidencia, _preActual.Notas, tambienPermanente));
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Incidencias");
            }
        }

        #endregion

        #region Notas, guardado, vista previa y cierre

        // Las notas en edición se conservan en memoria por trabajador hasta guardarlas o cerrar el periodo
        private void txtNotasRecibo_TextChanged(object sender, EventArgs e)
        {
            if (_cargando || _preActual == null)
            {
                return;
            }
            _preActual.Notas = txtNotasRecibo.Text;
        }

        private void btnGuardarPreRecibo_Click(object sender, EventArgs e)
        {
            try
            {
                if (_periodoActual == null || _preActual == null)
                {
                    Mensajes.Advertencia("Seleccione un periodo y un trabajador.", "Pre-recibo");
                    return;
                }

                PreReciboDTO guardado = _preNominaBll.GuardarPreRecibo(_periodoActual.PeriodoId, _preActual.EmpleadoId, txtNotasRecibo.Text);
                RefrescarEstatusPeriodo();
                AplicarPreRecibo(guardado);
                Mensajes.Informacion("El pre-recibo de " + guardado.NombreCompleto + " se guardó.", "Pre-recibo");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Pre-recibo");
            }
        }

        private void btnVistaPrevia_Click(object sender, EventArgs e)
        {
            try
            {
                if (_periodoActual == null || _preActual == null)
                {
                    Mensajes.Advertencia("Seleccione un periodo y un trabajador.", "Vista previa del recibo");
                    return;
                }

                _reciboVistaPrevia = _preNominaBll.ObtenerVistaPrevia(_periodoActual.PeriodoId, _preActual.EmpleadoId, txtNotasRecibo.Text);
                ppdRecibo.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Vista previa del recibo");
            }
        }

        private void printDocRecibo_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_reciboVistaPrevia == null)
            {
                e.Cancel = true;
                return;
            }
            ImpresorRecibo.Dibujar(e, _reciboVistaPrevia);
        }

        private void btnCerrarNomina_Click(object sender, EventArgs e)
        {
            try
            {
                if (_periodoActual == null)
                {
                    Mensajes.Advertencia("No hay un periodo seleccionado.", "Cierre de nómina");
                    return;
                }

                decimal neto = _periodoActual.EstaAbierto ? _preRecibos.Sum(p => p.NetoPagar) : _periodoActual.SumaNetoPagar;
                string pregunta = "Se emitirán los recibos definitivos de " + _preRecibos.Count + " trabajador(es) por un neto de " + neto.ToString("C2") +
                                  ", el periodo " + _periodoActual.NumeroPeriodo + " quedará CERRADO, se amortizarán los préstamos, se consolidarán " +
                                  "los acumulados fiscales y se generarán los CFDI pendientes de timbrado.\n\nEl cierre es definitivo.";

                List<string> advertencias = _nominaBll.ObtenerAdvertenciasPrevias(_periodoActual.PeriodoId);
                if (advertencias.Count > 0)
                {
                    pregunta += "\n\nAdvertencias:\n - " + string.Join("\n - ", advertencias);
                }
                if (!Mensajes.Confirmar(pregunta + "\n\n¿Desea continuar?", "Timbrar / Cerrar nómina"))
                {
                    return;
                }

                Dictionary<int, string> notas = _preRecibos.Where(p => p.Notas != null).ToDictionary(p => p.EmpleadoId, p => p.Notas);

                Cursor = Cursors.WaitCursor;
                ResultadoCierreNomina resultado = _preNominaBll.CerrarNomina(_periodoActual.PeriodoId, notas);
                RecargarPeriodoActual();
                Mensajes.Informacion("Nómina cerrada: " + resultado.TotalRecibos + " recibo(s) por un neto de " + resultado.TotalNeto.ToString("C2") +
                                     ".\n\nLos CFDI quedaron pendientes de timbrado.", "Timbrar / Cerrar nómina");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Timbrar / Cerrar nómina");
                RecargarSinError();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Tras un error en una operación del periodo se vuelve a leer su estado real sin mostrar un segundo mensaje
        private void RecargarSinError()
        {
            try
            {
                RecargarPeriodoActual();
            }
            catch (Exception)
            {
                // Se conserva el mensaje del error original
            }
        }

        #endregion
    }
}
