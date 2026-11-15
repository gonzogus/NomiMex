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
    public partial class FrmCatDepartamentos : Form
    {
        private readonly DepartamentoBLL _departamentoBll = new DepartamentoBLL();
        private readonly EmpresaBLL _empresaBll = new EmpresaBLL();
        private List<CatDepartamento> _departamentos = new List<CatDepartamento>();
        private int _departamentoIdSeleccionado;
        private bool _cargandoEmpresas;

        public FrmCatDepartamentos()
        {
            InitializeComponent();
        }

        private void FrmCatDepartamentos_Load(object sender, EventArgs e)
        {
            try
            {
                CargarEmpresas();
                CargarDepartamentos();
                LimpiarFormulario();
                if (dgvDepartamentos.Columns.Count > 0)
                {
                    dgvDepartamentos.Columns["DepartamentoId"].Visible = false;
                    dgvDepartamentos.Columns["EmpresaId"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de departamentos");
            }
        }

        private void CargarEmpresas()
        {
            _cargandoEmpresas = true;
            AyudaCombos.CargarCatalogo(cmbEmpresa, _empresaBll.Listar(true)
                .Select(x => new ItemCatalogo { Id = x.EmpresaId, Clave = x.RFC, Descripcion = x.RazonSocial }));
            List<ItemCatalogo> empresas = cmbEmpresa.DataSource as List<ItemCatalogo>;
            if (empresas != null && empresas.Count == 2)
            {
                AyudaCombos.SeleccionarId(cmbEmpresa, empresas[1].Id);
            }
            _cargandoEmpresas = false;
        }

        private void cmbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoEmpresas)
            {
                return;
            }

            try
            {
                CargarDepartamentos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de departamentos");
            }
        }

        private void CargarDepartamentos()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            _departamentos = empresaId > 0 ? _departamentoBll.Listar(empresaId, false) : new List<CatDepartamento>();
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            List<CatDepartamento> filtrados = _departamentoBll.Filtrar(_departamentos, txtBuscar.Text);
            dgvDepartamentos.DataSource = filtrados;
            lblConteo.Text = filtrados.Count + " de " + _departamentos.Count + " registros";
        }

        private void LimpiarFormulario()
        {
            _departamentoIdSeleccionado = 0;
            txtClave.Clear();
            txtDescripcion.Clear();
            chkActivo.Checked = true;
            dgvDepartamentos.ClearSelection();
            ActualizarBotones();
            txtClave.Focus();
        }

        private void ActualizarBotones()
        {
            bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Departamentos, AccionSistema.Escribir);
            bool haySeleccion = _departamentoIdSeleccionado > 0;
            cmbEmpresa.Enabled = !haySeleccion;
            btnGuardar.Enabled = puedeEscribir && !haySeleccion;
            btnEditar.Enabled = puedeEscribir && haySeleccion;
            btnEliminar.Enabled = puedeEscribir && haySeleccion;
            grpDatos.Text = haySeleccion ? "Editando departamento" : "Nuevo departamento";
        }

        private CatDepartamento LeerFormulario()
        {
            return new CatDepartamento
            {
                DepartamentoId = _departamentoIdSeleccionado,
                EmpresaId = AyudaCombos.Id(cmbEmpresa) ?? 0,
                Clave = txtClave.Text,
                Descripcion = txtDescripcion.Text,
                Activo = chkActivo.Checked
            };
        }

        private void Guardar(string mensajeExito)
        {
            try
            {
                _departamentoBll.Guardar(LeerFormulario());
                CargarDepartamentos();
                LimpiarFormulario();
                Mensajes.Informacion(mensajeExito, "Catálogo de departamentos");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de departamentos");
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar("El departamento se guardó correctamente.");
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Guardar("El departamento se actualizó correctamente.");
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_departamentoIdSeleccionado <= 0)
                {
                    Mensajes.Advertencia("Seleccione un departamento de la lista.", "Catálogo de departamentos");
                    return;
                }

                if (!Mensajes.Confirmar("El departamento se marcará como inactivo (eliminación lógica). ¿Desea continuar?", "Catálogo de departamentos"))
                {
                    return;
                }

                _departamentoBll.DesactivarLogico(_departamentoIdSeleccionado);
                CargarDepartamentos();
                LimpiarFormulario();
                Mensajes.Informacion("El departamento se desactivó correctamente.", "Catálogo de departamentos");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de departamentos");
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
                Mensajes.Mostrar(ex, "Catálogo de departamentos");
            }
        }

        private void dgvDepartamentos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CatDepartamento departamento = dgvDepartamentos.Rows[e.RowIndex].DataBoundItem as CatDepartamento;
            if (departamento == null)
            {
                return;
            }

            _departamentoIdSeleccionado = departamento.DepartamentoId;
            txtClave.Text = departamento.Clave;
            txtDescripcion.Text = departamento.Descripcion;
            chkActivo.Checked = departamento.Activo;
            ActualizarBotones();
        }

        private void dgvDepartamentos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CatDepartamento departamento = dgvDepartamentos.Rows[e.RowIndex].DataBoundItem as CatDepartamento;
            if (departamento != null && !departamento.Activo)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }
    }
}
