using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    public partial class FrmCatEstados : Form
    {
        private readonly EstadoBLL _estadoBll = new EstadoBLL();
        private List<CatEntidadFederativa> _estados = new List<CatEntidadFederativa>();
        private string _claveSeleccionada;

        public FrmCatEstados()
        {
            InitializeComponent();
        }

        private void FrmCatEstados_Load(object sender, EventArgs e)
        {
            try
            {
                CargarEstados();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de estados");
            }
        }

        private void CargarEstados()
        {
            _estados = _estadoBll.Listar(false);
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            List<CatEntidadFederativa> filtrados = _estadoBll.Filtrar(_estados, txtBuscar.Text);
            dgvEstados.DataSource = filtrados;
            lblConteo.Text = filtrados.Count + " de " + _estados.Count + " registros";
        }

        private void LimpiarFormulario()
        {
            _claveSeleccionada = null;
            txtClave.Clear();
            txtNombre.Clear();
            txtAbreviatura.Clear();
            chkActivo.Checked = true;
            dgvEstados.ClearSelection();
            ActualizarBotones();
            txtClave.Focus();
        }

        private void ActualizarBotones()
        {
            bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Estados, AccionSistema.Escribir);
            bool haySeleccion = _claveSeleccionada != null;
            btnGuardar.Enabled = puedeEscribir && !haySeleccion;
            btnEditar.Enabled = puedeEscribir && haySeleccion;
            btnEliminar.Enabled = puedeEscribir && haySeleccion;
            txtClave.ReadOnly = haySeleccion;
            grpDatos.Text = haySeleccion ? "Editando estado" : "Nuevo estado";
        }

        private CatEntidadFederativa LeerFormulario()
        {
            return new CatEntidadFederativa
            {
                CveEntidad = txtClave.Text,
                Nombre = txtNombre.Text,
                Abreviatura = txtAbreviatura.Text,
                Activo = chkActivo.Checked
            };
        }

        private void Guardar(bool esNuevo, string mensajeExito)
        {
            try
            {
                _estadoBll.Guardar(LeerFormulario(), esNuevo);
                CargarEstados();
                LimpiarFormulario();
                Mensajes.Informacion(mensajeExito, "Catálogo de estados");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de estados");
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar(true, "El estado se guardó correctamente.");
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Guardar(false, "El estado se actualizó correctamente.");
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_claveSeleccionada == null)
                {
                    Mensajes.Advertencia("Seleccione un estado de la lista.", "Catálogo de estados");
                    return;
                }

                if (!Mensajes.Confirmar("El estado se marcará como inactivo (eliminación lógica). ¿Desea continuar?", "Catálogo de estados"))
                {
                    return;
                }

                _estadoBll.DesactivarLogico(_claveSeleccionada);
                CargarEstados();
                LimpiarFormulario();
                Mensajes.Informacion("El estado se desactivó correctamente.", "Catálogo de estados");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de estados");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            AplicarFiltro();
            LimpiarFormulario();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de estados");
            }
        }

        private void dgvEstados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CatEntidadFederativa estado = dgvEstados.Rows[e.RowIndex].DataBoundItem as CatEntidadFederativa;
            if (estado == null)
            {
                return;
            }

            _claveSeleccionada = estado.CveEntidad;
            txtClave.Text = estado.CveEntidad;
            txtNombre.Text = estado.Nombre;
            txtAbreviatura.Text = estado.Abreviatura;
            chkActivo.Checked = estado.Activo;
            ActualizarBotones();
        }

        private void dgvEstados_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CatEntidadFederativa estado = dgvEstados.Rows[e.RowIndex].DataBoundItem as CatEntidadFederativa;
            if (estado != null && !estado.Activo)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }
    }
}
