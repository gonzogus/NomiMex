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
    public partial class FrmCatPuestos : Form
    {
        private readonly PuestoBLL _puestoBll = new PuestoBLL();
        private readonly EmpresaBLL _empresaBll = new EmpresaBLL();
        private List<CatPuesto> _puestos = new List<CatPuesto>();
        private int _puestoIdSeleccionado;
        private bool _cargandoEmpresas;

        public FrmCatPuestos()
        {
            InitializeComponent();
        }

        private void FrmCatPuestos_Load(object sender, EventArgs e)
        {
            try
            {
                CargarEmpresas();
                CargarPuestos();
                LimpiarFormulario();
                if(dgvPuestos.Columns.Count > 0)
                {
                    //seleccionamos la primera columna para que se vea el encabezado de la tabla
                    dgvPuestos.Columns[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de puestos");
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
                CargarPuestos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de puestos");
            }
        }

        private void CargarPuestos()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            _puestos = empresaId > 0 ? _puestoBll.Listar(empresaId, false) : new List<CatPuesto>();
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            List<CatPuesto> filtrados = _puestoBll.Filtrar(_puestos, txtBuscar.Text);
            dgvPuestos.DataSource = filtrados;
            lblConteo.Text = filtrados.Count + " de " + _puestos.Count + " registros";
        }

        private void LimpiarFormulario()
        {
            _puestoIdSeleccionado = 0;
            txtClave.Clear();
            txtDescripcion.Clear();
            txtSalarioMin.Clear();
            txtSalarioMax.Clear();
            chkActivo.Checked = true;
            dgvPuestos.ClearSelection();
            ActualizarBotones();
            txtClave.Focus();
        }

        private void ActualizarBotones()
        {
            bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Puestos, AccionSistema.Escribir);
            bool haySeleccion = _puestoIdSeleccionado > 0;
            cmbEmpresa.Enabled = !haySeleccion;
            btnGuardar.Enabled = puedeEscribir && !haySeleccion;
            btnEditar.Enabled = puedeEscribir && haySeleccion;
            btnEliminar.Enabled = puedeEscribir && haySeleccion;
            grpDatos.Text = haySeleccion ? "Editando puesto" : "Nuevo puesto";
        }

        private CatPuesto LeerFormulario()
        {
            return new CatPuesto
            {
                PuestoId = _puestoIdSeleccionado,
                EmpresaId = AyudaCombos.Id(cmbEmpresa) ?? 0,
                Clave = txtClave.Text,
                Descripcion = txtDescripcion.Text,
                SalarioBandaMin = Validador.DecimalOpcional(txtSalarioMin.Text, "Salario mínimo"),
                SalarioBandaMax = Validador.DecimalOpcional(txtSalarioMax.Text, "Salario máximo"),
                Activo = chkActivo.Checked
            };
        }

        private void Guardar(string mensajeExito)
        {
            try
            {
                CatPuesto puesto = LeerFormulario();
                _puestoBll.Guardar(puesto);
                CargarPuestos();
                LimpiarFormulario();
                Mensajes.Informacion(mensajeExito, "Catálogo de puestos");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de puestos");
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar("El puesto se guardó correctamente.");
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Guardar("El puesto se actualizó correctamente.");
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_puestoIdSeleccionado <= 0)
                {
                    Mensajes.Advertencia("Seleccione un puesto de la lista.", "Catálogo de puestos");
                    return;
                }

                if (!Mensajes.Confirmar("El puesto se marcará como inactivo (eliminación lógica). ¿Desea continuar?", "Catálogo de puestos"))
                {
                    return;
                }

                _puestoBll.DesactivarLogico(_puestoIdSeleccionado);
                CargarPuestos();
                LimpiarFormulario();
                Mensajes.Informacion("El puesto se desactivó correctamente.", "Catálogo de puestos");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de puestos");
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
                Mensajes.Mostrar(ex, "Catálogo de puestos");
            }
        }

        private void dgvPuestos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CatPuesto puesto = dgvPuestos.Rows[e.RowIndex].DataBoundItem as CatPuesto;
            if (puesto == null)
            {
                return;
            }

            _puestoIdSeleccionado = puesto.PuestoId;
            txtClave.Text = puesto.Clave;
            txtDescripcion.Text = puesto.Descripcion;
            txtSalarioMin.Text = puesto.SalarioBandaMin.HasValue ? puesto.SalarioBandaMin.Value.ToString("0.00##") : string.Empty;
            txtSalarioMax.Text = puesto.SalarioBandaMax.HasValue ? puesto.SalarioBandaMax.Value.ToString("0.00##") : string.Empty;
            chkActivo.Checked = puesto.Activo;
            ActualizarBotones();
        }

        private void dgvPuestos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CatPuesto puesto = dgvPuestos.Rows[e.RowIndex].DataBoundItem as CatPuesto;
            if (puesto != null && !puesto.Activo)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }
    }
}
