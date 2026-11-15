using System;
using System.Windows.Forms;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    public partial class FrmLogin : Form
    {
        private readonly AutenticacionBLL _autenticacion = new AutenticacionBLL();

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Shown(object sender, EventArgs e)
        {
            txtUsuario.Focus();
        }

        private void chkMostrarContrasena_CheckedChanged(object sender, EventArgs e)
        {
            txtContrasena.UseSystemPasswordChar = !chkMostrarContrasena.Checked;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnIngresar.Enabled = false;

                _autenticacion.IniciarSesion(txtUsuario.Text, txtContrasena.Text);

                if (SesionActual.HaySesion && SesionActual.Usuario.RequiereCambioPwd)
                {
                    using (FrmCambiarContrasena frm = new FrmCambiarContrasena(ModoCambioContrasena.Voluntario))
                    {
                        if (frm.ShowDialog(this) != DialogResult.OK)
                        {
                            _autenticacion.CerrarSesion();
                            Mensajes.Advertencia("Debe actualizar su contraseña para poder ingresar al sistema.", "Inicio de sesión");
                            txtContrasena.Clear();
                            txtContrasena.Focus();
                            return;
                        }
                    }
                }

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Inicio de sesión");
                txtContrasena.Clear();
                txtContrasena.Focus();
            }
            finally
            {
                Cursor = Cursors.Default;
                btnIngresar.Enabled = true;
            }
        }

        private void lnkOlvideContrasena_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                using (FrmCambiarContrasena frm = new FrmCambiarContrasena(ModoCambioContrasena.Recuperacion))
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Recuperar acceso");
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
