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
    /// Administración de los paquetes de incidencias: conceptos del catálogo SAT que se asignan al trabajador en su alta
    /// según la periodicidad de su contrato. Permite quitar y agregar conceptos del catálogo oficial.
    /// </summary>
    public partial class FrmCatPaquetes : Form
    {
        private readonly PaqueteIncidenciaBLL _bll = new PaqueteIncidenciaBLL();
        private readonly CatalogoBLL _catalogoBll = new CatalogoBLL();
        private int _paqueteId;
        private bool _cargando;

        public FrmCatPaquetes() { InitializeComponent(); }

        private void FrmCatPaquetes_Load(object sender, EventArgs e)
        {
            try
            {
                List<ItemCatalogo> periodicidades = new List<ItemCatalogo>
                {
                    new ItemCatalogo { Id = 0, Descripcion = "(Todas las periodicidades)" }
                };
                periodicidades.AddRange(_catalogoBll.ListarTiposPeriodo());
                cmbPeriodicidadPaquete.DisplayMember = "Texto";
                cmbPeriodicidadPaquete.ValueMember = "Id";
                cmbPeriodicidadPaquete.DataSource = periodicidades;
                CargarPaquetes(null);
                if(dgvConceptosPaquete.Columns.Count > 0)
                {
                    //seleccionamos el primer registro de la lista de conceptos del paquete
                    dgvConceptosPaquete.Rows[0].Selected = true;
                }
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Paquetes de incidencias"); }
        }

        private RelPaqueteConcepto ConceptoSeleccionado()
        {
            return dgvConceptosPaquete.SelectedRows.Count == 0 ? null :
                dgvConceptosPaquete.SelectedRows[0].DataBoundItem as RelPaqueteConcepto;
        }

        private void CargarPaquetes(int? seleccionarId)
        {
            _cargando = true;
            List<CatPaqueteIncidencia> paquetes = _bll.Listar();
            dgvPaquetes.DataSource = paquetes;
            dgvPaquetes.ClearSelection();
            _cargando = false;

            int indice = seleccionarId.HasValue ? paquetes.FindIndex(p => p.IdPaquete == seleccionarId.Value) : (paquetes.Count > 0 ? 0 : -1);
            if (indice >= 0)
            {
                dgvPaquetes.Rows[indice].Selected = true;
                CargarPaqueteSeleccionado();
            }
            else
            {
                NuevoPaquete();
            }
        }

        private void dgvPaquetes_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) CargarPaqueteSeleccionado();
        }

        private void CargarPaqueteSeleccionado()
        {
            CatPaqueteIncidencia p = dgvPaquetes.SelectedRows.Count == 0 ? null :
                dgvPaquetes.SelectedRows[0].DataBoundItem as CatPaqueteIncidencia;
            if (p == null) return;
            try
            {
                _paqueteId = p.IdPaquete;
                txtNombrePaquete.Text = p.Nombre;
                AyudaCombos.SeleccionarId(cmbPeriodicidadPaquete, p.PeriodicidadPagoId);
                chkPredeterminado.Checked = p.EsDefault;
                chkPaqueteActivo.Checked = p.Activo;
                dgvConceptosPaquete.DataSource = _bll.ListarConceptos(p.IdPaquete);
                dgvConceptosPaquete.ClearSelection();
                ActualizarBotones();
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Paquetes de incidencias"); }
        }

        private void NuevoPaquete()
        {
            _paqueteId = 0;
            dgvPaquetes.ClearSelection();
            txtNombrePaquete.Clear();
            AyudaCombos.SeleccionarId(cmbPeriodicidadPaquete, null);
            chkPredeterminado.Checked = false;
            chkPaqueteActivo.Checked = true;
            dgvConceptosPaquete.DataSource = new List<RelPaqueteConcepto>();
            ActualizarBotones();
        }

        private void ActualizarBotones()
        {
            bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Empleados, AccionSistema.Escribir);
            RelPaqueteConcepto c = ConceptoSeleccionado();
            bool editable = puedeEscribir && _paqueteId > 0 && c != null && !c.Concepto.EsBase;
            btnNuevoPaquete.Enabled = puedeEscribir;
            btnGuardarPaquete.Enabled = puedeEscribir;
            btnAgregarConcepto.Enabled = puedeEscribir && _paqueteId > 0;
            btnActualizarConcepto.Enabled = editable;
            btnQuitarConcepto.Enabled = editable;
        }

        private void dgvConceptosPaquete_SelectionChanged(object sender, EventArgs e)
        {
            RelPaqueteConcepto c = ConceptoSeleccionado();
            if (c != null && !c.Concepto.EsBase)
            {
                nudCantidadPaquete.Value = Math.Min(Math.Max(c.CantidadOValor, nudCantidadPaquete.Minimum), nudCantidadPaquete.Maximum);
                chkPermanentePaquete.Checked = c.EsPermanente;
            }
            ActualizarBotones();
        }

        private void btnNuevoPaquete_Click(object sender, EventArgs e)
        {
            NuevoPaquete();
            txtNombrePaquete.Focus();
        }

        private void btnGuardarPaquete_Click(object sender, EventArgs e)
        {
            try
            {
                int id = _bll.Guardar(new CatPaqueteIncidencia
                {
                    IdPaquete = _paqueteId,
                    Nombre = txtNombrePaquete.Text,
                    PeriodicidadPagoId = AyudaCombos.Id(cmbPeriodicidadPaquete),
                    EsDefault = chkPredeterminado.Checked,
                    Activo = chkPaqueteActivo.Checked
                });
                CargarPaquetes(id);
                Mensajes.Informacion("El paquete se guardó.", "Paquetes de incidencias");
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Paquetes de incidencias"); }
        }

        private void btnAgregarConcepto_Click(object sender, EventArgs e)
        {
            try
            {
                using (FrmSeleccionarConceptoSAT selector = new FrmSeleccionarConceptoSAT())
                {
                    if (selector.ShowDialog(this) != DialogResult.OK) return;
                    _bll.AgregarConcepto(_paqueteId, selector.ConceptoSeleccionado.ConceptoId, nudCantidadPaquete.Value, chkPermanentePaquete.Checked);
                    CargarPaquetes(_paqueteId);
                }
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Paquetes de incidencias"); }
        }

        private void btnActualizarConcepto_Click(object sender, EventArgs e)
        {
            try
            {
                RelPaqueteConcepto c = ConceptoSeleccionado();
                if (c == null) throw new ExcepcionNegocio("Seleccione el concepto que desea actualizar.");
                _bll.AgregarConcepto(_paqueteId, c.IdConceptoSAT, nudCantidadPaquete.Value, chkPermanentePaquete.Checked);
                CargarPaquetes(_paqueteId);
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Paquetes de incidencias"); }
        }

        private void btnQuitarConcepto_Click(object sender, EventArgs e)
        {
            try
            {
                RelPaqueteConcepto c = ConceptoSeleccionado();
                if (c == null) throw new ExcepcionNegocio("Seleccione el concepto que desea quitar.");
                if (!Mensajes.Confirmar("¿Quitar '" + c.ConceptoTexto + "' del paquete? No cambia a los trabajadores ya dados de alta.",
                    "Paquetes de incidencias")) return;
                _bll.QuitarConcepto(_paqueteId, c.IdConceptoSAT);
                CargarPaquetes(_paqueteId);
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Paquetes de incidencias"); }
        }
    }
}
