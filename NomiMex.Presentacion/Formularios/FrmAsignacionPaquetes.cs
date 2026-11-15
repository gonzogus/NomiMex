using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Relaciones;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    /// <summary>
    /// Asignación de paquetes de incidencias por empresa y periodo. Cada empresa tiene su propia política: al elegir la empresa
    /// solo se cargan sus periodos y, al elegir el periodo, solo los paquetes que la política de esa empresa permite en él.
    /// Todo acceso a datos y toda validación viven en NomiMex.Negocio.
    /// </summary>
    public partial class FrmAsignacionPaquetes : Form
    {
        private const string Titulo = "Paquetes por periodo";

        private readonly AsignacionPaqueteBLL _bll = new AsignacionPaqueteBLL();

        private List<PeriodoResumen> _periodos = new List<PeriodoResumen>();
        private List<PaqueteAsignacion> _paquetes = new List<PaqueteAsignacion>();
        private List<ConceptoPeriodoPaquete> _conceptos = new List<ConceptoPeriodoPaquete>();
        private PeriodoResumen _periodoActual;
        private bool _cargando;

        public FrmAsignacionPaquetes()
        {
            InitializeComponent();
        }

        private void FrmAsignacionPaquetes_Load(object sender, EventArgs e)
        {
            bool anterior = _cargando;
            try
            {
                _cargando = true;
                List<CatEmpresa> empresas = _bll.ListarEmpresas();
                AyudaCombos.CargarCatalogo(cmbEmpresa, empresas
                    .Select(x => new ItemCatalogo { Id = x.EmpresaId, Clave = x.RFC, Descripcion = x.RazonSocial }));
                AyudaCombos.CargarCatalogo(cmbPeriodos, new List<ItemCatalogo>());
                if (empresas.Count > 0)
                {
                    cmbEmpresa.SelectedIndex = 1;
                }
                _cargando = anterior;

                CargarPeriodos();
            }
            catch (Exception ex)
            {
                _cargando = anterior;
                Enabled = false;
                MostrarError(ex);
            }
        }

        #region Cascada Empresa -> Periodo -> Paquetes

        private void cmbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
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
                MostrarError(ex);
            }
        }

        private void cmbPeriodos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                CargarPaquetes();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Solo los periodos de la empresa seleccionada
        private void CargarPeriodos()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            _periodos = empresaId > 0 ? _bll.ListarPeriodos(empresaId) : new List<PeriodoResumen>();

            bool anterior = _cargando;
            _cargando = true;
            AyudaCombos.CargarCatalogo(cmbPeriodos, _periodos
                .Select(p => new ItemCatalogo { Id = p.PeriodoId, Descripcion = p.Anio + " | " + p.Descripcion }));
            int? sugerido = PeriodoSugerido();
            if (sugerido.HasValue)
            {
                AyudaCombos.SeleccionarId(cmbPeriodos, sugerido);
            }
            _cargando = anterior;

            CargarPaquetes();
        }

        // El periodo que incluye hoy; si no hay, el primer periodo abierto; si tampoco, el más reciente
        private int? PeriodoSugerido()
        {
            if (_periodos.Count == 0)
            {
                return null;
            }

            PeriodoResumen elegido = _periodos.FirstOrDefault(p => p.FechaInicio.Date <= DateTime.Today && DateTime.Today <= p.FechaFin.Date)
                                     ?? _periodos.LastOrDefault(p => p.EstaAbierto)
                                     ?? _periodos[0];
            return elegido.PeriodoId;
        }

        // Paquetes de la combinación exacta Empresa + Periodo
        private void CargarPaquetes()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            int periodoId = AyudaCombos.Id(cmbPeriodos) ?? 0;
            _periodoActual = _periodos.FirstOrDefault(p => p.PeriodoId == periodoId);

            if (empresaId <= 0 || _periodoActual == null)
            {
                _paquetes = new List<PaqueteAsignacion>();
                _conceptos = new List<ConceptoPeriodoPaquete>();
                dgvPaquetes.DataSource = _paquetes;
                dgvConceptos.DataSource = _conceptos;
                lblInfoPeriodo.Text = empresaId > 0 ? "La empresa no tiene periodos generados." : "Seleccione una empresa.";
                lblEstado.Text = string.Empty;
                ActualizarAvisoTimbrado();
                ActualizarControles();
                return;
            }

            _paquetes = _bll.ListarPaquetes(empresaId, periodoId);
            _conceptos = _bll.ListarConceptos(empresaId, periodoId);
            dgvPaquetes.DataSource = _paquetes;
            dgvPaquetes.ClearSelection();
            dgvConceptos.DataSource = _conceptos;
            dgvConceptos.ClearSelection();

            lblInfoPeriodo.Text = "Periodicidad: " + _periodoActual.TipoPeriodo + "   |   Del " + _periodoActual.FechaInicio.ToString("dd/MM/yyyy") +
                                  " al " + _periodoActual.FechaFin.ToString("dd/MM/yyyy") + "   |   Estatus: " + _periodoActual.Estatus;
            lblEstado.Text = _paquetes.Count == 0
                ? "La política de la empresa no tiene paquetes disponibles para este periodo."
                : _paquetes.Count(p => p.Asignado) + " de " + _paquetes.Count + " paquete(s) asignado(s).";
            ActualizarAvisoTimbrado();
            ActualizarControles();
        }

        // Un periodo ya procesado o un usuario sin permiso de escritura solo consulta
        private void ActualizarControles()
        {
            bool editable = _periodoActual != null && _periodoActual.EstaAbierto && _paquetes.Count > 0 &&
                            ControlAcceso.Puede(ModuloSistema.Empleados, AccionSistema.Escribir);
            colAsignado.ReadOnly = !editable;
            colOmitir.ReadOnly = !editable;
            colMotivo.ReadOnly = !editable;
            btnGuardar.Enabled = editable;
            btnGuardar.BackColor = editable ? System.Drawing.Color.FromArgb(37, 99, 235) : System.Drawing.Color.FromArgb(148, 163, 184);
            btnAplicarATodos.Enabled = editable;
            btnAplicarATodos.BackColor = editable ? System.Drawing.Color.FromArgb(22, 163, 74) : System.Drawing.Color.FromArgb(148, 163, 184);
            btnRecargar.Enabled = _periodoActual != null;
        }

        // La bandera: omitir un concepto legal impide timbrar el periodo
        private void ActualizarAvisoTimbrado()
        {
            List<ConceptoPeriodoPaquete> legales = _conceptos.Where(c => c.Omitido && c.EsLegal).ToList();
            lblAvisoTimbrado.Text = legales.Count == 0
                ? string.Empty
                : "Atención: se omiten conceptos legales (" + string.Join(", ", legales.Select(c => c.Concepto)) +
                  "). El periodo podrá cerrarse, pero sus recibos NO se podrán timbrar.";
        }

        #endregion

        #region Edición y guardado

        // La casilla confirma el cambio al instante para que el conteo y el guardado vean el valor nuevo
        private void dgvPaquetes_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPaquetes.IsCurrentCellDirty && dgvPaquetes.CurrentCell != null && dgvPaquetes.CurrentCell.OwningColumn == colAsignado)
            {
                dgvPaquetes.CommitEdit(DataGridViewDataErrorContexts.Commit);
                lblEstado.Text = _paquetes.Count(p => p.Asignado) + " de " + _paquetes.Count +
                                 " paquete(s) marcado(s); guarde para actualizar sus conceptos.";
            }
        }

        private void dgvConceptos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvConceptos.IsCurrentCellDirty && dgvConceptos.CurrentCell != null && dgvConceptos.CurrentCell.OwningColumn == colOmitir)
            {
                dgvConceptos.CommitEdit(DataGridViewDataErrorContexts.Commit);
                ActualizarAvisoTimbrado();
                dgvConceptos.InvalidateColumn(colOmitir.Index);
            }
        }

        // Los conceptos legales se resaltan; si además se omiten, en rojo
        private void dgvConceptos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _conceptos.Count)
            {
                return;
            }

            ConceptoPeriodoPaquete concepto = _conceptos[e.RowIndex];
            if (concepto.EsLegal)
            {
                e.CellStyle.BackColor = concepto.Omitido ? System.Drawing.Color.FromArgb(254, 226, 226) : System.Drawing.Color.FromArgb(254, 249, 195);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int empresaId, periodoId;
                List<int> paquetesIds;
                List<RelEmpresaPeriodoConceptoOmitido> omisiones;
                if (!TomarSeleccion(out empresaId, out periodoId, out paquetesIds, out omisiones))
                {
                    return;
                }

                if (!ConfirmarOmisionesLegales(string.Empty))
                {
                    return;
                }

                _bll.Guardar(empresaId, periodoId, paquetesIds, omisiones);

                CargarPaquetes();
                MessageBox.Show("Paquetes y omisiones guardados para " + cmbEmpresa.Text + " en el periodo " + _periodoActual.NumeroPeriodo + ".",
                    Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Replica la configuración del periodo en los demás periodos abiertos de la misma empresa y periodicidad
        private void btnAplicarATodos_Click(object sender, EventArgs e)
        {
            try
            {
                int empresaId, periodoId;
                List<int> paquetesIds;
                List<RelEmpresaPeriodoConceptoOmitido> omisiones;
                if (!TomarSeleccion(out empresaId, out periodoId, out paquetesIds, out omisiones))
                {
                    return;
                }

                List<PeriodoResumen> destinos = _bll.ListarPeriodosDestino(empresaId, periodoId);
                string alcance = "\n\nSe guardará en el periodo " + _periodoActual.NumeroPeriodo + " y se copiará a " + destinos.Count +
                                 " periodo(s) abierto(s) más de " + cmbEmpresa.Text + " con periodicidad " + _periodoActual.TipoPeriodo +
                                 ". La configuración actual de paquetes y omisiones de esos periodos se reemplazará. Los periodos de otras empresas no se modifican.";
                if (!ConfirmarOmisionesLegales(alcance))
                {
                    return;
                }

                ResultadoCopiaPaquetes resultado = _bll.CopiarAPeriodosAbiertos(empresaId, periodoId, paquetesIds, omisiones);

                CargarPaquetes();
                string mensaje = "Configuración guardada en el periodo " + _periodoActual.NumeroPeriodo + " y copiada a " +
                                 resultado.PeriodosActualizados + " periodo(s) abierto(s) de " + cmbEmpresa.Text + ".";
                if (resultado.Fallos.Count > 0)
                {
                    mensaje += "\n\nNo se pudo actualizar:\n - " + string.Join("\n - ", resultado.Fallos.Take(8));
                }
                MessageBox.Show(mensaje, Titulo, MessageBoxButtons.OK,
                    resultado.Fallos.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // Toma de los grids lo que el usuario marcó; false si falta empresa o periodo
        private bool TomarSeleccion(out int empresaId, out int periodoId, out List<int> paquetesIds,
                                    out List<RelEmpresaPeriodoConceptoOmitido> omisiones)
        {
            empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            periodoId = AyudaCombos.Id(cmbPeriodos) ?? 0;
            paquetesIds = new List<int>();
            omisiones = new List<RelEmpresaPeriodoConceptoOmitido>();
            if (empresaId <= 0 || periodoId <= 0)
            {
                MessageBox.Show("Seleccione una empresa y un periodo.", Titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            dgvPaquetes.EndEdit();
            dgvConceptos.EndEdit();
            paquetesIds = _paquetes.Where(p => p.Asignado).Select(p => p.IdPaquete).ToList();
            omisiones = _conceptos.Where(c => c.Omitido)
                .Select(c => new RelEmpresaPeriodoConceptoOmitido { ConceptoId = c.ConceptoId, Motivo = c.Motivo }).ToList();
            return true;
        }

        // Si se omiten conceptos legales se confirma que el periodo no podrá timbrarse; sin ellos la confirmación solo aplica al alcance
        private bool ConfirmarOmisionesLegales(string alcance)
        {
            List<ConceptoPeriodoPaquete> legales = _conceptos.Where(c => c.Omitido && c.EsLegal).ToList();
            if (legales.Count == 0 && alcance.Length == 0)
            {
                return true;
            }

            string texto = legales.Count == 0
                ? "¿Desea continuar?" + alcance
                : "Se omitirán conceptos legales (" + string.Join(", ", legales.Select(c => c.Concepto)) + ") en " + cmbEmpresa.Text +
                  ".\n\nEl periodo podrá cerrarse, pero sus recibos NO se podrán timbrar porque no cumplen los requisitos del CFDI de nómina." +
                  alcance + "\n\n¿Desea continuar?";
            return MessageBox.Show(texto, Titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        private void btnRecargar_Click(object sender, EventArgs e)
        {
            try
            {
                CargarPaquetes();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        #endregion

        // Las infracciones a la política de la empresa se muestran como advertencia; los fallos técnicos, como error
        private void MostrarError(Exception ex)
        {
            ExcepcionNegocio negocio = ex as ExcepcionNegocio;
            if (negocio != null && !negocio.EsErrorTecnico)
            {
                MessageBox.Show(negocio.Message, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (negocio != null)
            {
                MessageBox.Show(negocio.Message, Titulo + " - Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
