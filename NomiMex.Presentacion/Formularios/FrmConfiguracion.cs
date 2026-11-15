using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using NomiMex.Entidades.Configuracion;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    /// <summary>
    /// Configuración técnica e infraestructura local de la aplicación (conexión a BD, correo SMTP,
    /// bitácoras y parámetros generales). Solo depende de NomiMex.Negocio y NomiMex.Entidades: no
    /// construye cadenas de conexión ni usa SqlClient o System.Net.Mail directamente.
    /// </summary>
    public partial class FrmConfiguracion : Form
    {
        private readonly ConfiguracionBLL _configuracion = new ConfiguracionBLL();

        public FrmConfiguracion()
        {
            InitializeComponent();
        }

        private void FrmConfiguracion_Load(object sender, EventArgs e)
        {
            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            try
            {
                MostrarEnFormulario(_configuracion.Cargar());
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Configuración");
            }
        }

        private void MostrarEnFormulario(AppConfiguracion config)
        {
            txtServidor.Text = config.ServidorBaseDatos;
            txtBaseDatos.Text = config.NombreBaseDatos;
            cmbAutenticacion.SelectedIndex = config.EsAutenticacionWindows ? 0 : 1;
            txtUsuario.Text = config.UsuarioBaseDatos;
            txtPassword.Text = config.PasswordBaseDatos;
            numTimeout.Value = Acotar(config.TiempoEsperaConexion, numTimeout);

            txtSmtpServidor.Text = config.ServidorSmtp;
            numPuerto.Value = Acotar(config.PuertoSmtp, numPuerto);
            chkSsl.Checked = config.UsarSsl;
            txtRemitente.Text = config.CorreoRemitente;
            txtNombreMostrar.Text = config.NombreMostrar;
            txtSmtpPassword.Text = config.PasswordSmtp;
            txtAsunto.Text = config.AsuntoRecibo;
            txtCuerpo.Text = config.CuerpoRecibo;

            txtCarpetaLogs.Text = config.CarpetaLogs;
            cmbNivelLog.SelectedIndex = AcotarIndice((int)config.Nivel, cmbNivelLog);
            numDiasRetencion.Value = Acotar(config.DiasRetencionLogs, numDiasRetencion);
            txtRutaExportRecibos.Text = config.RutaExportacionRecibos;

            numMinutosInactividad.Value = Acotar(config.MinutosInactividad, numMinutosInactividad);
            chkVerificarConexionInicio.Checked = config.VerificarConexionAlInicio;
            txtRutaRespaldos.Text = config.RutaRespaldosLocal;

            ActualizarCamposAutenticacion();
        }

        private AppConfiguracion LeerFormulario()
        {
            return new AppConfiguracion
            {
                ServidorBaseDatos = txtServidor.Text,
                NombreBaseDatos = txtBaseDatos.Text,
                Autenticacion = cmbAutenticacion.SelectedIndex == 0 ? TipoAutenticacion.Windows : TipoAutenticacion.SqlServer,
                UsuarioBaseDatos = txtUsuario.Text,
                PasswordBaseDatos = txtPassword.Text,
                TiempoEsperaConexion = (int)numTimeout.Value,

                ServidorSmtp = txtSmtpServidor.Text,
                PuertoSmtp = (int)numPuerto.Value,
                UsarSsl = chkSsl.Checked,
                CorreoRemitente = txtRemitente.Text,
                NombreMostrar = txtNombreMostrar.Text,
                PasswordSmtp = txtSmtpPassword.Text,
                AsuntoRecibo = txtAsunto.Text,
                CuerpoRecibo = txtCuerpo.Text,

                CarpetaLogs = txtCarpetaLogs.Text,
                Nivel = (NivelBitacora)Math.Max(0, cmbNivelLog.SelectedIndex),
                DiasRetencionLogs = (int)numDiasRetencion.Value,
                RutaExportacionRecibos = txtRutaExportRecibos.Text,

                MinutosInactividad = (int)numMinutosInactividad.Value,
                VerificarConexionAlInicio = chkVerificarConexionInicio.Checked,
                RutaRespaldosLocal = txtRutaRespaldos.Text
            };
        }

        private void cmbAutenticacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarCamposAutenticacion();
        }

        private void ActualizarCamposAutenticacion()
        {
            bool usaSql = cmbAutenticacion.SelectedIndex != 0;
            txtUsuario.Enabled = usaSql;
            txtPassword.Enabled = usaSql;
            chkMostrarPassword.Enabled = usaSql;
        }

        private void chkMostrarPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
        }

        private void chkMostrarSmtp_CheckedChanged(object sender, EventArgs e)
        {
            txtSmtpPassword.UseSystemPasswordChar = !chkMostrarSmtp.Checked;
        }

        private void btnProbarConexion_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnProbarConexion.Enabled = false;
                lblEstadoConexion.Text = "Conectando...";

                string version = _configuracion.ProbarConexion(LeerFormulario());

                lblEstadoConexion.ForeColor = System.Drawing.Color.FromArgb(0, 135, 90);
                lblEstadoConexion.Text = "Conexión correcta. SQL Server " + version;
                Mensajes.Informacion("La conexión con la base de datos se estableció correctamente.", "Probar conexión");
            }
            catch (Exception ex)
            {
                lblEstadoConexion.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
                lblEstadoConexion.Text = "No fue posible conectar.";
                Mensajes.Mostrar(ex, "Probar conexión");
            }
            finally
            {
                Cursor = Cursors.Default;
                btnProbarConexion.Enabled = true;
            }
        }

        private void btnEnviarPrueba_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnEnviarPrueba.Enabled = false;

                _configuracion.EnviarCorreoPrueba(LeerFormulario(), txtCorreoPrueba.Text);

                Mensajes.Informacion("El correo de prueba se envió correctamente.", "Correo de prueba");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Correo de prueba");
            }
            finally
            {
                Cursor = Cursors.Default;
                btnEnviarPrueba.Enabled = true;
            }
        }

        private void btnExaminarLogs_Click(object sender, EventArgs e)
        {
            SeleccionarCarpeta(txtCarpetaLogs);
        }

        private void btnExaminarExport_Click(object sender, EventArgs e)
        {
            SeleccionarCarpeta(txtRutaExportRecibos);
        }

        private void btnExaminarRespaldos_Click(object sender, EventArgs e)
        {
            SeleccionarCarpeta(txtRutaRespaldos);
        }

        private void SeleccionarCarpeta(TextBox destino)
        {
            if (!string.IsNullOrWhiteSpace(destino.Text) && Directory.Exists(destino.Text))
            {
                folderBrowserDialog.SelectedPath = destino.Text;
            }

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                destino.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnAbrirCarpetaLogs_Click(object sender, EventArgs e)
        {
            string carpeta = txtCarpetaLogs.Text.Trim();
            if (string.IsNullOrEmpty(carpeta) || !Directory.Exists(carpeta))
            {
                Mensajes.Advertencia("La carpeta de bitácoras indicada no existe todavía. Guarde la configuración para crearla.",
                    "Bitácoras");
                return;
            }

            try
            {
                Process.Start("explorer.exe", carpeta);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Bitácoras");
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnGuardar.Enabled = false;

                _configuracion.Guardar(LeerFormulario());

                Mensajes.Informacion("La configuración se guardó correctamente.", "Configuración");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Configuración");
            }
            finally
            {
                Cursor = Cursors.Default;
                btnGuardar.Enabled = true;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static decimal Acotar(int valor, NumericUpDown control)
        {
            if (valor < control.Minimum)
            {
                return control.Minimum;
            }
            if (valor > control.Maximum)
            {
                return control.Maximum;
            }
            return valor;
        }

        private static int AcotarIndice(int indice, ComboBox combo)
        {
            if (indice < 0)
            {
                return 0;
            }
            if (indice > combo.Items.Count - 1)
            {
                return combo.Items.Count - 1;
            }
            return indice;
        }
    }
}
