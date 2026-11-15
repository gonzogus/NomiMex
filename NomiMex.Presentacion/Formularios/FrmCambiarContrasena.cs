using System;
using System.Globalization;
using System.Windows.Forms;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    /// <summary>Modo de operación del formulario de contraseña.</summary>
    public enum ModoCambioContrasena
    {
        /// <summary>Recuperación por correo (código de verificación), invocado desde FrmLogin.</summary>
        Recuperacion,

        /// <summary>Cambio voluntario u obligatorio con la contraseña actual, en sesión iniciada.</summary>
        Voluntario
    }

    public partial class FrmCambiarContrasena : Form
    {
        private readonly RecuperacionBLL _recuperacion = new RecuperacionBLL();
        private readonly ModoCambioContrasena _modo;
        private bool _codigoEnviado;

        /// <summary>Constructor por omisión: abre el formulario en modo recuperación.</summary>
        public FrmCambiarContrasena()
            : this(ModoCambioContrasena.Recuperacion)
        {
        }

        /// <summary>Constructor sobrecargado que selecciona el modo de operación.</summary>
        public FrmCambiarContrasena(ModoCambioContrasena modo)
        {
            _modo = modo;
            InitializeComponent();
            ConfigurarModo();
        }

        private void ConfigurarModo()
        {
            if (_modo == ModoCambioContrasena.Recuperacion)
            {
                Text = "NomiMex - Restablecer contraseña";
                lblTitulo.Text = "Recuperar acceso";
                lblSubtitulo.Text = "Le enviaremos un código a su correo";
                btnGuardar.Text = "Restablecer contraseña";

                pnlModoRecuperacion.Visible = true;
                pnlModoVoluntario.Visible = false;

                // Hasta no enviar el código, los campos de restablecimiento quedan deshabilitados.
                HabilitarRestablecimiento(false);
            }
            else
            {
                Text = "NomiMex - Cambiar contraseña";
                lblTitulo.Text = "Cambiar contraseña";
                lblSubtitulo.Text = "Actualice su contraseña de acceso";
                btnGuardar.Text = "Guardar";

                pnlModoRecuperacion.Visible = false;
                pnlModoVoluntario.Visible = true;
            }
        }

        private void HabilitarRestablecimiento(bool habilitado)
        {
            txtCodigo.Enabled = habilitado;
            txtNuevaPassword.Enabled = habilitado;
            txtConfirmarPassword.Enabled = habilitado;
            chkMostrarPassword.Enabled = habilitado;
            btnGuardar.Enabled = habilitado;
        }

        private void btnEnviarCodigo_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnEnviarCodigo.Enabled = false;

                DateTime expiraUtc = _recuperacion.SolicitarCodigo(txtCorreo.Text);

                _codigoEnviado = true;
                HabilitarRestablecimiento(true);

                DateTime expiraLocal = expiraUtc.ToLocalTime();
                lblInfoCodigo.Text = "Código enviado. Vigente hasta las " +
                    expiraLocal.ToString("HH:mm", CultureInfo.CurrentCulture) + ". Revise su correo.";

                Mensajes.Informacion(
                    "Se envió un código de verificación al correo indicado. Captúrelo junto con su nueva contraseña.",
                    "Recuperar acceso");

                txtCodigo.Focus();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Recuperar acceso");
            }
            finally
            {
                Cursor = Cursors.Default;
                btnEnviarCodigo.Enabled = true;
            }
        }

        private void chkMostrarPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool ocultar = !chkMostrarPassword.Checked;
            txtPasswordActual.UseSystemPasswordChar = ocultar;
            txtNuevaPassword.UseSystemPasswordChar = ocultar;
            txtConfirmarPassword.UseSystemPasswordChar = ocultar;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnGuardar.Enabled = false;

                if (_modo == ModoCambioContrasena.Recuperacion)
                {
                    if (!_codigoEnviado)
                    {
                        Mensajes.Advertencia("Primero solicite un código con el botón \"Enviar código\".", "Recuperar acceso");
                        return;
                    }

                    _recuperacion.Restablecer(txtCorreo.Text, txtCodigo.Text, txtNuevaPassword.Text, txtConfirmarPassword.Text);
                    Mensajes.Informacion("Su contraseña se restableció correctamente. Ya puede iniciar sesión.", "Recuperar acceso");
                }
                else
                {
                    _recuperacion.CambiarPropia(txtPasswordActual.Text, txtNuevaPassword.Text, txtConfirmarPassword.Text);
                    Mensajes.Informacion("Su contraseña se actualizó correctamente.", "Cambiar contraseña");
                }

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Cambiar contraseña");
            }
            finally
            {
                Cursor = Cursors.Default;
                btnGuardar.Enabled = true;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
