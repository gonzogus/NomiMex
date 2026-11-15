using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using NomiMex.Entidades.Documentos;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    public partial class FrmDocRecibos : Form
    {
        private readonly EmpresaBLL _empresaBll = new EmpresaBLL();
        private readonly ReciboBLL _reciboBll = new ReciboBLL();

        private List<ReciboListado> _recibos = new List<ReciboListado>();
        private ReciboImpresion _reciboImpresion;
        private bool _cargando;

        public FrmDocRecibos()
        {
            InitializeComponent();
        }

        private void FrmDocRecibos_Load(object sender, EventArgs e)
        {
            try
            {
                new PrerrequisitosNomina().ExigirEmpresaRegistrada();

                _cargando = true;
                AyudaCombos.CargarCatalogo(cmbEmpresa, _empresaBll.Listar(true)
                    .Select(x => new ItemCatalogo { Id = x.EmpresaId, Clave = x.RFC, Descripcion = x.RazonSocial }));
                AyudaCombos.CargarCatalogo(cmbTipoPeriodo, new List<ItemCatalogo>());
                _cargando = false;

                List<ItemCatalogo> empresas = cmbEmpresa.DataSource as List<ItemCatalogo>;
                if (empresas != null && empresas.Count > 1)
                {
                    cmbEmpresa.SelectedIndex = 1;
                }
                CargarPeriodicidades();
                CargarPeriodos();
            }
            catch (Exception ex)
            {
                _cargando = false;
                Enabled = false;
                Mensajes.Mostrar(ex, "Recibos de nómina");
            }
        }

        // Solo se ofrecen las periodicidades asignadas a la empresa (sin selección = todas)
        private void CargarPeriodicidades()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            _cargando = true;
            AyudaCombos.CargarCatalogo(cmbTipoPeriodo, _empresaBll.ListarPeriodicidadesItems(empresaId));
            _cargando = false;
        }

        private void CargarPeriodos()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            List<PeriodoResumen> periodos = _reciboBll.ListarPeriodosConRecibos(empresaId, AyudaCombos.Id(cmbTipoPeriodo));

            _cargando = true;
            cmbPeriodo.DisplayMember = "Descripcion";
            cmbPeriodo.ValueMember = "PeriodoId";
            cmbPeriodo.DataSource = periodos;
            cmbPeriodo.SelectedIndex = periodos.Count > 0 ? 0 : -1;
            _cargando = false;

            CargarRecibos();
        }

        private void CargarRecibos()
        {
            PeriodoResumen periodo = cmbPeriodo.SelectedItem as PeriodoResumen;
            _recibos = periodo == null ? new List<ReciboListado>() : _reciboBll.ListarPorPeriodo(periodo.PeriodoId);
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            List<ReciboListado> filtrados = _reciboBll.Filtrar(_recibos, txtBuscar.Text);
            dgvRecibos.DataSource = filtrados;
            dgvRecibos.ClearSelection();
            dgvDetalle.DataSource = new List<DocReciboDetalle>();
            lblDatosRecibo.Text = "Seleccione un recibo para ver su detalle.";

            lblTotales.Text = string.Format("{0} recibo(s)   Percepciones: {1:C2}   Deducciones: {2:C2}   Neto: {3:C2}",
                filtrados.Count, filtrados.Sum(r => r.TotalPercepciones), filtrados.Sum(r => r.TotalDeducciones), filtrados.Sum(r => r.NetoPagar));
            ActualizarBotones();
        }

        private void ActualizarBotones()
        {
            bool haySeleccion = ReciboSeleccionado() != null;
            btnVistaPrevia.Enabled = haySeleccion;
            btnImprimir.Enabled = haySeleccion;
        }

        private ReciboListado ReciboSeleccionado()
        {
            if (dgvRecibos.CurrentRow == null || !dgvRecibos.CurrentRow.Selected)
            {
                return null;
            }
            return dgvRecibos.CurrentRow.DataBoundItem as ReciboListado;
        }

        private void cmbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                CargarPeriodicidades();
                CargarPeriodos();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Recibos de nómina");
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
                CargarPeriodos();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Recibos de nómina");
            }
        }

        private void cmbPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargando)
            {
                return;
            }

            try
            {
                CargarRecibos();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Recibos de nómina");
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Recibos de nómina");
            }
        }

        private void dgvRecibos_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                ReciboListado recibo = ReciboSeleccionado();
                ActualizarBotones();
                if (recibo == null)
                {
                    return;
                }

                dgvDetalle.DataSource = _reciboBll.ListarDetalle(recibo.ReciboId);
                lblDatosRecibo.Text = string.Format("{0} - {1}{2}{3} / {4}{2}Percepciones {5:C2}  Deducciones {6:C2}{2}Neto a pagar {7:C2}",
                    recibo.NumeroEmpleado, recibo.NombreEmpleado, Environment.NewLine, recibo.Puesto, recibo.Departamento,
                    recibo.TotalPercepciones, recibo.TotalDeducciones, recibo.NetoPagar);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Recibos de nómina");
            }
        }

        private void btnVistaPrevia_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CargarReciboSeleccionado())
                {
                    return;
                }
                ppdRecibo.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Vista previa del recibo");
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CargarReciboSeleccionado())
                {
                    return;
                }

                if (pdgRecibo.ShowDialog(this) == DialogResult.OK)
                {
                    prnRecibo.Print();
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Impresión del recibo");
            }
        }

        private bool CargarReciboSeleccionado()
        {
            ReciboListado recibo = ReciboSeleccionado();
            if (recibo == null)
            {
                Mensajes.Advertencia("Seleccione un recibo de la lista.", "Recibos de nómina");
                return false;
            }

            _reciboImpresion = _reciboBll.ObtenerRecibo(recibo.ReciboId);
            return true;
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            ReciboImpresion r = _reciboImpresion;
            if (r == null)
            {
                e.Cancel = true;
                return;
            }

            ImpresorRecibo.Dibujar(e, r);
        }
    }
}
