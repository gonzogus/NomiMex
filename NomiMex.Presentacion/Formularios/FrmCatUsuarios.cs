using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    public partial class FrmCatUsuarios : Form
    {
        private readonly UsuarioBLL _usuarioBll = new UsuarioBLL();
        private List<CatUsuario> _usuarios = new List<CatUsuario>();
        private int _usuarioIdSeleccionado;

        public FrmCatUsuarios()
        {
            InitializeComponent();
        }

        private void FrmCatUsuarios_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (CatRol rol in _usuarioBll.ListarRoles())
                {
                    clbRoles.Items.Add(rol.Codigo);
                }
                CargarUsuarios();
                LimpiarFormulario();
                if(dgvUsuarios.Rows.Count > 0)
                {
                    dgvUsuarios.Rows[0].Selected = true;
                    dgvUsuarios_CellClick(dgvUsuarios, new DataGridViewCellEventArgs(0, 0));
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Usuarios y roles");
            }
        }

        private void CargarUsuarios()
        {
            _usuarios = _usuarioBll.Listar();
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            List<CatUsuario> filtrados = _usuarioBll.Filtrar(_usuarios, txtBuscar.Text);
            dgvUsuarios.DataSource = filtrados;
            lblConteo.Text = filtrados.Count + " de " + _usuarios.Count + " registros";
        }

        private void LimpiarFormulario()
        {
            _usuarioIdSeleccionado = 0;
            txtUsuario.Clear();
            txtNombreMostrar.Clear();
            txtEmail.Clear();
            txtContrasena.Clear();
            txtConfirmacion.Clear();
            chkRequiereCambio.Checked = false;
            chkActivo.Checked = true;
            chkBloqueado.Checked = false;
            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                clbRoles.SetItemChecked(i, false);
            }
            dgvUsuarios.ClearSelection();
            ActualizarBotones();
            txtUsuario.Focus();
        }

        private void ActualizarBotones()
        {
            bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Usuarios, AccionSistema.Escribir);
            bool haySeleccion = _usuarioIdSeleccionado > 0;
            btnGuardar.Enabled = puedeEscribir && !haySeleccion;
            btnEditar.Enabled = puedeEscribir && haySeleccion;
            btnEliminar.Enabled = puedeEscribir && haySeleccion;
            txtUsuario.ReadOnly = haySeleccion;
            grpDatos.Text = haySeleccion ? "Editando usuario" : "Nuevo usuario";
        }

        private CatUsuario LeerFormulario()
        {
            return new CatUsuario
            {
                UsuarioId = _usuarioIdSeleccionado,
                NombreUsuario = txtUsuario.Text,
                NombreMostrar = txtNombreMostrar.Text,
                Email = txtEmail.Text,
                RequiereCambioPwd = chkRequiereCambio.Checked,
                Activo = chkActivo.Checked,
                Bloqueado = chkBloqueado.Checked
            };
        }

        private List<string> LeerRoles()
        {
            return clbRoles.CheckedItems.Cast<string>().ToList();
        }

        private void Guardar(string mensajeExito)
        {
            try
            {
                _usuarioBll.Guardar(LeerFormulario(), txtContrasena.Text, txtConfirmacion.Text, LeerRoles());
                CargarUsuarios();
                LimpiarFormulario();
                Mensajes.Informacion(mensajeExito, "Usuarios y roles");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Usuarios y roles");
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar("El usuario se guardó correctamente.");
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Guardar("El usuario se actualizó correctamente.");
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioIdSeleccionado <= 0)
                {
                    Mensajes.Advertencia("Seleccione un usuario de la lista.", "Usuarios y roles");
                    return;
                }

                if (!Mensajes.Confirmar("El usuario se marcará como inactivo y se cerrarán sus sesiones (eliminación lógica). ¿Desea continuar?", "Usuarios y roles"))
                {
                    return;
                }

                _usuarioBll.DesactivarLogico(_usuarioIdSeleccionado);
                CargarUsuarios();
                LimpiarFormulario();
                Mensajes.Informacion("El usuario se desactivó correctamente.", "Usuarios y roles");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Usuarios y roles");
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
                Mensajes.Mostrar(ex, "Usuarios y roles");
            }
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CatUsuario usuario = dgvUsuarios.Rows[e.RowIndex].DataBoundItem as CatUsuario;
            if (usuario == null)
            {
                return;
            }

            _usuarioIdSeleccionado = usuario.UsuarioId;
            txtUsuario.Text = usuario.NombreUsuario;
            txtNombreMostrar.Text = usuario.NombreMostrar;
            txtEmail.Text = usuario.Email;
            txtContrasena.Clear();
            txtConfirmacion.Clear();
            chkRequiereCambio.Checked = usuario.RequiereCambioPwd;
            chkActivo.Checked = usuario.Activo;
            chkBloqueado.Checked = usuario.Bloqueado;

            string[] codigos = (usuario.RolesTexto ?? string.Empty).Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                clbRoles.SetItemChecked(i, codigos.Contains((string)clbRoles.Items[i]));
            }
            ActualizarBotones();
        }

        private void dgvUsuarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CatUsuario usuario = dgvUsuarios.Rows[e.RowIndex].DataBoundItem as CatUsuario;
            if (usuario == null)
            {
                return;
            }

            if (!usuario.Activo)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }

            if (dgvUsuarios.Columns[e.ColumnIndex] == colUltimoAcceso && usuario.UltimoAccesoUtc.HasValue)
            {
                DateTime local = DateTime.SpecifyKind(usuario.UltimoAccesoUtc.Value, DateTimeKind.Utc).ToLocalTime();
                e.Value = local.ToString("dd/MM/yyyy HH:mm");
                e.FormattingApplied = true;
            }
        }
    }
}
