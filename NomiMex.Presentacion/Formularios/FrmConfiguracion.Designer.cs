namespace NomiMex.Presentacion.Formularios
{
    partial class FrmConfiguracion
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlAcciones = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.tabConfiguracion = new System.Windows.Forms.TabControl();
            this.tabConexion = new System.Windows.Forms.TabPage();
            this.lblEstadoConexion = new System.Windows.Forms.Label();
            this.btnProbarConexion = new System.Windows.Forms.Button();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.chkMostrarPassword = new System.Windows.Forms.CheckBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.cmbAutenticacion = new System.Windows.Forms.ComboBox();
            this.lblAutenticacion = new System.Windows.Forms.Label();
            this.txtBaseDatos = new System.Windows.Forms.TextBox();
            this.lblBaseDatos = new System.Windows.Forms.Label();
            this.txtServidor = new System.Windows.Forms.TextBox();
            this.lblServidor = new System.Windows.Forms.Label();
            this.tabCorreo = new System.Windows.Forms.TabPage();
            this.btnEnviarPrueba = new System.Windows.Forms.Button();
            this.txtCorreoPrueba = new System.Windows.Forms.TextBox();
            this.lblCorreoPrueba = new System.Windows.Forms.Label();
            this.txtCuerpo = new System.Windows.Forms.TextBox();
            this.lblCuerpo = new System.Windows.Forms.Label();
            this.txtAsunto = new System.Windows.Forms.TextBox();
            this.lblAsunto = new System.Windows.Forms.Label();
            this.chkMostrarSmtp = new System.Windows.Forms.CheckBox();
            this.txtSmtpPassword = new System.Windows.Forms.TextBox();
            this.lblSmtpPassword = new System.Windows.Forms.Label();
            this.txtNombreMostrar = new System.Windows.Forms.TextBox();
            this.lblNombreMostrar = new System.Windows.Forms.Label();
            this.txtRemitente = new System.Windows.Forms.TextBox();
            this.lblRemitente = new System.Windows.Forms.Label();
            this.chkSsl = new System.Windows.Forms.CheckBox();
            this.numPuerto = new System.Windows.Forms.NumericUpDown();
            this.lblPuerto = new System.Windows.Forms.Label();
            this.txtSmtpServidor = new System.Windows.Forms.TextBox();
            this.lblSmtpServidor = new System.Windows.Forms.Label();
            this.tabBitacoras = new System.Windows.Forms.TabPage();
            this.btnExaminarExport = new System.Windows.Forms.Button();
            this.txtRutaExportRecibos = new System.Windows.Forms.TextBox();
            this.lblRutaExport = new System.Windows.Forms.Label();
            this.numDiasRetencion = new System.Windows.Forms.NumericUpDown();
            this.lblDiasRetencion = new System.Windows.Forms.Label();
            this.cmbNivelLog = new System.Windows.Forms.ComboBox();
            this.lblNivelLog = new System.Windows.Forms.Label();
            this.btnAbrirCarpetaLogs = new System.Windows.Forms.Button();
            this.btnExaminarLogs = new System.Windows.Forms.Button();
            this.txtCarpetaLogs = new System.Windows.Forms.TextBox();
            this.lblCarpetaLogs = new System.Windows.Forms.Label();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.btnExaminarRespaldos = new System.Windows.Forms.Button();
            this.txtRutaRespaldos = new System.Windows.Forms.TextBox();
            this.lblRutaRespaldos = new System.Windows.Forms.Label();
            this.chkVerificarConexionInicio = new System.Windows.Forms.CheckBox();
            this.numMinutosInactividad = new System.Windows.Forms.NumericUpDown();
            this.lblMinutosInactividad = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.pnlEncabezado.SuspendLayout();
            this.pnlAcciones.SuspendLayout();
            this.tabConfiguracion.SuspendLayout();
            this.tabConexion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            this.tabCorreo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPuerto)).BeginInit();
            this.tabBitacoras.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDiasRetencion)).BeginInit();
            this.tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinutosInactividad)).BeginInit();
            this.SuspendLayout();
            //
            // pnlEncabezado
            //
            this.pnlEncabezado.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.pnlEncabezado.Controls.Add(this.lblSubtitulo);
            this.pnlEncabezado.Controls.Add(this.lblTitulo);
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Location = new System.Drawing.Point(0, 0);
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.pnlEncabezado.Size = new System.Drawing.Size(584, 70);
            this.pnlEncabezado.TabIndex = 0;
            //
            // lblSubtitulo
            //
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.lblSubtitulo.Location = new System.Drawing.Point(22, 42);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(270, 17);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Parámetros técnicos e infraestructura local";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(20, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(252, 28);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Configuración del sistema";
            //
            // pnlAcciones
            //
            this.pnlAcciones.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.pnlAcciones.Controls.Add(this.btnCancelar);
            this.pnlAcciones.Controls.Add(this.btnGuardar);
            this.pnlAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAcciones.Location = new System.Drawing.Point(0, 582);
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(584, 58);
            this.pnlAcciones.TabIndex = 2;
            //
            // btnCancelar
            //
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnCancelar.Location = new System.Drawing.Point(300, 12);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(120, 34);
            this.btnCancelar.TabIndex = 0;
            this.btnCancelar.Text = "Cerrar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // btnGuardar
            //
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(0, 135, 90);
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 110, 74);
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(432, 12);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(140, 34);
            this.btnGuardar.TabIndex = 1;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // tabConfiguracion
            //
            this.tabConfiguracion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabConfiguracion.Controls.Add(this.tabConexion);
            this.tabConfiguracion.Controls.Add(this.tabCorreo);
            this.tabConfiguracion.Controls.Add(this.tabBitacoras);
            this.tabConfiguracion.Controls.Add(this.tabGeneral);
            this.tabConfiguracion.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.tabConfiguracion.Location = new System.Drawing.Point(12, 82);
            this.tabConfiguracion.Name = "tabConfiguracion";
            this.tabConfiguracion.SelectedIndex = 0;
            this.tabConfiguracion.Size = new System.Drawing.Size(560, 488);
            this.tabConfiguracion.TabIndex = 1;
            //
            // tabConexion
            //
            this.tabConexion.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.tabConexion.Controls.Add(this.lblEstadoConexion);
            this.tabConexion.Controls.Add(this.btnProbarConexion);
            this.tabConexion.Controls.Add(this.numTimeout);
            this.tabConexion.Controls.Add(this.lblTimeout);
            this.tabConexion.Controls.Add(this.chkMostrarPassword);
            this.tabConexion.Controls.Add(this.txtPassword);
            this.tabConexion.Controls.Add(this.lblPassword);
            this.tabConexion.Controls.Add(this.txtUsuario);
            this.tabConexion.Controls.Add(this.lblUsuario);
            this.tabConexion.Controls.Add(this.cmbAutenticacion);
            this.tabConexion.Controls.Add(this.lblAutenticacion);
            this.tabConexion.Controls.Add(this.txtBaseDatos);
            this.tabConexion.Controls.Add(this.lblBaseDatos);
            this.tabConexion.Controls.Add(this.txtServidor);
            this.tabConexion.Controls.Add(this.lblServidor);
            this.tabConexion.Location = new System.Drawing.Point(4, 26);
            this.tabConexion.Name = "tabConexion";
            this.tabConexion.Padding = new System.Windows.Forms.Padding(3);
            this.tabConexion.Size = new System.Drawing.Size(552, 458);
            this.tabConexion.TabIndex = 0;
            this.tabConexion.Text = "Conexión a BD";
            //
            // lblEstadoConexion
            //
            this.lblEstadoConexion.AutoSize = true;
            this.lblEstadoConexion.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEstadoConexion.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblEstadoConexion.Location = new System.Drawing.Point(20, 330);
            this.lblEstadoConexion.Name = "lblEstadoConexion";
            this.lblEstadoConexion.Size = new System.Drawing.Size(0, 15);
            this.lblEstadoConexion.TabIndex = 14;
            //
            // btnProbarConexion
            //
            this.btnProbarConexion.BackColor = System.Drawing.Color.FromArgb(0, 135, 90);
            this.btnProbarConexion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProbarConexion.FlatAppearance.BorderSize = 0;
            this.btnProbarConexion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 110, 74);
            this.btnProbarConexion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProbarConexion.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnProbarConexion.ForeColor = System.Drawing.Color.White;
            this.btnProbarConexion.Location = new System.Drawing.Point(200, 285);
            this.btnProbarConexion.Name = "btnProbarConexion";
            this.btnProbarConexion.Size = new System.Drawing.Size(180, 36);
            this.btnProbarConexion.TabIndex = 13;
            this.btnProbarConexion.Text = "Probar conexión";
            this.btnProbarConexion.UseVisualStyleBackColor = false;
            this.btnProbarConexion.Click += new System.EventHandler(this.btnProbarConexion_Click);
            //
            // numTimeout
            //
            this.numTimeout.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numTimeout.Location = new System.Drawing.Point(200, 236);
            this.numTimeout.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            this.numTimeout.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(90, 25);
            this.numTimeout.TabIndex = 12;
            this.numTimeout.Value = new decimal(new int[] { 15, 0, 0, 0 });
            //
            // lblTimeout
            //
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblTimeout.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblTimeout.Location = new System.Drawing.Point(20, 239);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(165, 17);
            this.lblTimeout.TabIndex = 11;
            this.lblTimeout.Text = "Tiempo de espera (segundos)";
            //
            // chkMostrarPassword
            //
            this.chkMostrarPassword.AutoSize = true;
            this.chkMostrarPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkMostrarPassword.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.chkMostrarPassword.Location = new System.Drawing.Point(200, 205);
            this.chkMostrarPassword.Name = "chkMostrarPassword";
            this.chkMostrarPassword.Size = new System.Drawing.Size(141, 19);
            this.chkMostrarPassword.TabIndex = 10;
            this.chkMostrarPassword.Text = "Mostrar contraseña";
            this.chkMostrarPassword.UseVisualStyleBackColor = true;
            this.chkMostrarPassword.CheckedChanged += new System.EventHandler(this.chkMostrarPassword_CheckedChanged);
            //
            // txtPassword
            //
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPassword.Location = new System.Drawing.Point(200, 175);
            this.txtPassword.MaxLength = 128;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(320, 25);
            this.txtPassword.TabIndex = 9;
            this.txtPassword.UseSystemPasswordChar = true;
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblPassword.Location = new System.Drawing.Point(20, 178);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(77, 17);
            this.lblPassword.TabIndex = 8;
            this.lblPassword.Text = "Contraseña";
            //
            // txtUsuario
            //
            this.txtUsuario.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsuario.Location = new System.Drawing.Point(200, 137);
            this.txtUsuario.MaxLength = 128;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(320, 25);
            this.txtUsuario.TabIndex = 7;
            //
            // lblUsuario
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblUsuario.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblUsuario.Location = new System.Drawing.Point(20, 140);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(56, 17);
            this.lblUsuario.TabIndex = 6;
            this.lblUsuario.Text = "Usuario";
            //
            // cmbAutenticacion
            //
            this.cmbAutenticacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutenticacion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbAutenticacion.FormattingEnabled = true;
            this.cmbAutenticacion.Items.AddRange(new object[] {
            "Windows",
            "SQL Server"});
            this.cmbAutenticacion.Location = new System.Drawing.Point(200, 98);
            this.cmbAutenticacion.Name = "cmbAutenticacion";
            this.cmbAutenticacion.Size = new System.Drawing.Size(200, 25);
            this.cmbAutenticacion.TabIndex = 5;
            this.cmbAutenticacion.SelectedIndexChanged += new System.EventHandler(this.cmbAutenticacion_SelectedIndexChanged);
            //
            // lblAutenticacion
            //
            this.lblAutenticacion.AutoSize = true;
            this.lblAutenticacion.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblAutenticacion.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblAutenticacion.Location = new System.Drawing.Point(20, 101);
            this.lblAutenticacion.Name = "lblAutenticacion";
            this.lblAutenticacion.Size = new System.Drawing.Size(93, 17);
            this.lblAutenticacion.TabIndex = 4;
            this.lblAutenticacion.Text = "Autenticación";
            //
            // txtBaseDatos
            //
            this.txtBaseDatos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBaseDatos.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtBaseDatos.Location = new System.Drawing.Point(200, 60);
            this.txtBaseDatos.MaxLength = 128;
            this.txtBaseDatos.Name = "txtBaseDatos";
            this.txtBaseDatos.Size = new System.Drawing.Size(320, 25);
            this.txtBaseDatos.TabIndex = 3;
            //
            // lblBaseDatos
            //
            this.lblBaseDatos.AutoSize = true;
            this.lblBaseDatos.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblBaseDatos.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblBaseDatos.Location = new System.Drawing.Point(20, 63);
            this.lblBaseDatos.Name = "lblBaseDatos";
            this.lblBaseDatos.Size = new System.Drawing.Size(95, 17);
            this.lblBaseDatos.TabIndex = 2;
            this.lblBaseDatos.Text = "Base de datos";
            //
            // txtServidor
            //
            this.txtServidor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServidor.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtServidor.Location = new System.Drawing.Point(200, 22);
            this.txtServidor.MaxLength = 128;
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(320, 25);
            this.txtServidor.TabIndex = 1;
            //
            // lblServidor
            //
            this.lblServidor.AutoSize = true;
            this.lblServidor.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblServidor.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblServidor.Location = new System.Drawing.Point(20, 25);
            this.lblServidor.Name = "lblServidor";
            this.lblServidor.Size = new System.Drawing.Size(123, 17);
            this.lblServidor.TabIndex = 0;
            this.lblServidor.Text = "Servidor / Instancia";
            //
            // tabCorreo
            //
            this.tabCorreo.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.tabCorreo.Controls.Add(this.btnEnviarPrueba);
            this.tabCorreo.Controls.Add(this.txtCorreoPrueba);
            this.tabCorreo.Controls.Add(this.lblCorreoPrueba);
            this.tabCorreo.Controls.Add(this.txtCuerpo);
            this.tabCorreo.Controls.Add(this.lblCuerpo);
            this.tabCorreo.Controls.Add(this.txtAsunto);
            this.tabCorreo.Controls.Add(this.lblAsunto);
            this.tabCorreo.Controls.Add(this.chkMostrarSmtp);
            this.tabCorreo.Controls.Add(this.txtSmtpPassword);
            this.tabCorreo.Controls.Add(this.lblSmtpPassword);
            this.tabCorreo.Controls.Add(this.txtNombreMostrar);
            this.tabCorreo.Controls.Add(this.lblNombreMostrar);
            this.tabCorreo.Controls.Add(this.txtRemitente);
            this.tabCorreo.Controls.Add(this.lblRemitente);
            this.tabCorreo.Controls.Add(this.chkSsl);
            this.tabCorreo.Controls.Add(this.numPuerto);
            this.tabCorreo.Controls.Add(this.lblPuerto);
            this.tabCorreo.Controls.Add(this.txtSmtpServidor);
            this.tabCorreo.Controls.Add(this.lblSmtpServidor);
            this.tabCorreo.Location = new System.Drawing.Point(4, 26);
            this.tabCorreo.Name = "tabCorreo";
            this.tabCorreo.Padding = new System.Windows.Forms.Padding(3);
            this.tabCorreo.Size = new System.Drawing.Size(552, 458);
            this.tabCorreo.TabIndex = 1;
            this.tabCorreo.Text = "Correo electrónico";
            //
            // btnEnviarPrueba
            //
            this.btnEnviarPrueba.BackColor = System.Drawing.Color.FromArgb(0, 135, 90);
            this.btnEnviarPrueba.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnviarPrueba.FlatAppearance.BorderSize = 0;
            this.btnEnviarPrueba.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 110, 74);
            this.btnEnviarPrueba.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnviarPrueba.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnEnviarPrueba.ForeColor = System.Drawing.Color.White;
            this.btnEnviarPrueba.Location = new System.Drawing.Point(410, 388);
            this.btnEnviarPrueba.Name = "btnEnviarPrueba";
            this.btnEnviarPrueba.Size = new System.Drawing.Size(130, 32);
            this.btnEnviarPrueba.TabIndex = 18;
            this.btnEnviarPrueba.Text = "Enviar prueba";
            this.btnEnviarPrueba.UseVisualStyleBackColor = false;
            this.btnEnviarPrueba.Click += new System.EventHandler(this.btnEnviarPrueba_Click);
            //
            // txtCorreoPrueba
            //
            this.txtCorreoPrueba.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCorreoPrueba.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCorreoPrueba.Location = new System.Drawing.Point(200, 390);
            this.txtCorreoPrueba.MaxLength = 150;
            this.txtCorreoPrueba.Name = "txtCorreoPrueba";
            this.txtCorreoPrueba.Size = new System.Drawing.Size(200, 25);
            this.txtCorreoPrueba.TabIndex = 17;
            //
            // lblCorreoPrueba
            //
            this.lblCorreoPrueba.AutoSize = true;
            this.lblCorreoPrueba.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCorreoPrueba.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblCorreoPrueba.Location = new System.Drawing.Point(20, 393);
            this.lblCorreoPrueba.Name = "lblCorreoPrueba";
            this.lblCorreoPrueba.Size = new System.Drawing.Size(165, 17);
            this.lblCorreoPrueba.TabIndex = 16;
            this.lblCorreoPrueba.Text = "Correo de prueba (destino)";
            //
            // txtCuerpo
            //
            this.txtCuerpo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCuerpo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCuerpo.Location = new System.Drawing.Point(200, 271);
            this.txtCuerpo.Multiline = true;
            this.txtCuerpo.Name = "txtCuerpo";
            this.txtCuerpo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCuerpo.Size = new System.Drawing.Size(340, 100);
            this.txtCuerpo.TabIndex = 15;
            //
            // lblCuerpo
            //
            this.lblCuerpo.AutoSize = true;
            this.lblCuerpo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCuerpo.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblCuerpo.Location = new System.Drawing.Point(20, 274);
            this.lblCuerpo.Name = "lblCuerpo";
            this.lblCuerpo.Size = new System.Drawing.Size(141, 17);
            this.lblCuerpo.TabIndex = 14;
            this.lblCuerpo.Text = "Cuerpo base del mensaje";
            //
            // txtAsunto
            //
            this.txtAsunto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAsunto.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtAsunto.Location = new System.Drawing.Point(200, 233);
            this.txtAsunto.MaxLength = 200;
            this.txtAsunto.Name = "txtAsunto";
            this.txtAsunto.Size = new System.Drawing.Size(340, 25);
            this.txtAsunto.TabIndex = 13;
            //
            // lblAsunto
            //
            this.lblAsunto.AutoSize = true;
            this.lblAsunto.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblAsunto.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblAsunto.Location = new System.Drawing.Point(20, 236);
            this.lblAsunto.Name = "lblAsunto";
            this.lblAsunto.Size = new System.Drawing.Size(152, 17);
            this.lblAsunto.TabIndex = 12;
            this.lblAsunto.Text = "Asunto predeterminado";
            //
            // chkMostrarSmtp
            //
            this.chkMostrarSmtp.AutoSize = true;
            this.chkMostrarSmtp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkMostrarSmtp.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.chkMostrarSmtp.Location = new System.Drawing.Point(200, 202);
            this.chkMostrarSmtp.Name = "chkMostrarSmtp";
            this.chkMostrarSmtp.Size = new System.Drawing.Size(141, 19);
            this.chkMostrarSmtp.TabIndex = 11;
            this.chkMostrarSmtp.Text = "Mostrar contraseña";
            this.chkMostrarSmtp.UseVisualStyleBackColor = true;
            this.chkMostrarSmtp.CheckedChanged += new System.EventHandler(this.chkMostrarSmtp_CheckedChanged);
            //
            // txtSmtpPassword
            //
            this.txtSmtpPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSmtpPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSmtpPassword.Location = new System.Drawing.Point(200, 172);
            this.txtSmtpPassword.MaxLength = 200;
            this.txtSmtpPassword.Name = "txtSmtpPassword";
            this.txtSmtpPassword.Size = new System.Drawing.Size(340, 25);
            this.txtSmtpPassword.TabIndex = 10;
            this.txtSmtpPassword.UseSystemPasswordChar = true;
            //
            // lblSmtpPassword
            //
            this.lblSmtpPassword.AutoSize = true;
            this.lblSmtpPassword.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSmtpPassword.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblSmtpPassword.Location = new System.Drawing.Point(20, 175);
            this.lblSmtpPassword.Name = "lblSmtpPassword";
            this.lblSmtpPassword.Size = new System.Drawing.Size(165, 17);
            this.lblSmtpPassword.TabIndex = 9;
            this.lblSmtpPassword.Text = "Contraseña / Clave de app";
            //
            // txtNombreMostrar
            //
            this.txtNombreMostrar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombreMostrar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNombreMostrar.Location = new System.Drawing.Point(200, 134);
            this.txtNombreMostrar.MaxLength = 150;
            this.txtNombreMostrar.Name = "txtNombreMostrar";
            this.txtNombreMostrar.Size = new System.Drawing.Size(340, 25);
            this.txtNombreMostrar.TabIndex = 8;
            //
            // lblNombreMostrar
            //
            this.lblNombreMostrar.AutoSize = true;
            this.lblNombreMostrar.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblNombreMostrar.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblNombreMostrar.Location = new System.Drawing.Point(20, 137);
            this.lblNombreMostrar.Name = "lblNombreMostrar";
            this.lblNombreMostrar.Size = new System.Drawing.Size(137, 17);
            this.lblNombreMostrar.TabIndex = 7;
            this.lblNombreMostrar.Text = "Nombre para mostrar";
            //
            // txtRemitente
            //
            this.txtRemitente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRemitente.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtRemitente.Location = new System.Drawing.Point(200, 96);
            this.txtRemitente.MaxLength = 150;
            this.txtRemitente.Name = "txtRemitente";
            this.txtRemitente.Size = new System.Drawing.Size(340, 25);
            this.txtRemitente.TabIndex = 6;
            //
            // lblRemitente
            //
            this.lblRemitente.AutoSize = true;
            this.lblRemitente.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblRemitente.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblRemitente.Location = new System.Drawing.Point(20, 99);
            this.lblRemitente.Name = "lblRemitente";
            this.lblRemitente.Size = new System.Drawing.Size(109, 17);
            this.lblRemitente.TabIndex = 5;
            this.lblRemitente.Text = "Correo remitente";
            //
            // chkSsl
            //
            this.chkSsl.AutoSize = true;
            this.chkSsl.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.chkSsl.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.chkSsl.Location = new System.Drawing.Point(320, 61);
            this.chkSsl.Name = "chkSsl";
            this.chkSsl.Size = new System.Drawing.Size(106, 21);
            this.chkSsl.TabIndex = 4;
            this.chkSsl.Text = "Usar SSL/TLS";
            this.chkSsl.UseVisualStyleBackColor = true;
            //
            // numPuerto
            //
            this.numPuerto.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numPuerto.Location = new System.Drawing.Point(200, 59);
            this.numPuerto.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.numPuerto.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numPuerto.Name = "numPuerto";
            this.numPuerto.Size = new System.Drawing.Size(90, 25);
            this.numPuerto.TabIndex = 3;
            this.numPuerto.Value = new decimal(new int[] { 587, 0, 0, 0 });
            //
            // lblPuerto
            //
            this.lblPuerto.AutoSize = true;
            this.lblPuerto.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblPuerto.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblPuerto.Location = new System.Drawing.Point(20, 61);
            this.lblPuerto.Name = "lblPuerto";
            this.lblPuerto.Size = new System.Drawing.Size(49, 17);
            this.lblPuerto.TabIndex = 2;
            this.lblPuerto.Text = "Puerto";
            //
            // txtSmtpServidor
            //
            this.txtSmtpServidor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSmtpServidor.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSmtpServidor.Location = new System.Drawing.Point(200, 22);
            this.txtSmtpServidor.MaxLength = 200;
            this.txtSmtpServidor.Name = "txtSmtpServidor";
            this.txtSmtpServidor.Size = new System.Drawing.Size(340, 25);
            this.txtSmtpServidor.TabIndex = 1;
            //
            // lblSmtpServidor
            //
            this.lblSmtpServidor.AutoSize = true;
            this.lblSmtpServidor.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSmtpServidor.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblSmtpServidor.Location = new System.Drawing.Point(20, 25);
            this.lblSmtpServidor.Name = "lblSmtpServidor";
            this.lblSmtpServidor.Size = new System.Drawing.Size(97, 17);
            this.lblSmtpServidor.TabIndex = 0;
            this.lblSmtpServidor.Text = "Servidor SMTP";
            //
            // tabBitacoras
            //
            this.tabBitacoras.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.tabBitacoras.Controls.Add(this.btnExaminarExport);
            this.tabBitacoras.Controls.Add(this.txtRutaExportRecibos);
            this.tabBitacoras.Controls.Add(this.lblRutaExport);
            this.tabBitacoras.Controls.Add(this.numDiasRetencion);
            this.tabBitacoras.Controls.Add(this.lblDiasRetencion);
            this.tabBitacoras.Controls.Add(this.cmbNivelLog);
            this.tabBitacoras.Controls.Add(this.lblNivelLog);
            this.tabBitacoras.Controls.Add(this.btnAbrirCarpetaLogs);
            this.tabBitacoras.Controls.Add(this.btnExaminarLogs);
            this.tabBitacoras.Controls.Add(this.txtCarpetaLogs);
            this.tabBitacoras.Controls.Add(this.lblCarpetaLogs);
            this.tabBitacoras.Location = new System.Drawing.Point(4, 26);
            this.tabBitacoras.Name = "tabBitacoras";
            this.tabBitacoras.Padding = new System.Windows.Forms.Padding(3);
            this.tabBitacoras.Size = new System.Drawing.Size(552, 458);
            this.tabBitacoras.TabIndex = 2;
            this.tabBitacoras.Text = "Bitácoras y rutas";
            //
            // btnExaminarExport
            //
            this.btnExaminarExport.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnExaminarExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExaminarExport.FlatAppearance.BorderSize = 0;
            this.btnExaminarExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExaminarExport.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnExaminarExport.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnExaminarExport.Location = new System.Drawing.Point(448, 168);
            this.btnExaminarExport.Name = "btnExaminarExport";
            this.btnExaminarExport.Size = new System.Drawing.Size(44, 27);
            this.btnExaminarExport.TabIndex = 10;
            this.btnExaminarExport.Text = "...";
            this.btnExaminarExport.UseVisualStyleBackColor = false;
            this.btnExaminarExport.Click += new System.EventHandler(this.btnExaminarExport_Click);
            //
            // txtRutaExportRecibos
            //
            this.txtRutaExportRecibos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRutaExportRecibos.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtRutaExportRecibos.Location = new System.Drawing.Point(200, 169);
            this.txtRutaExportRecibos.MaxLength = 260;
            this.txtRutaExportRecibos.Name = "txtRutaExportRecibos";
            this.txtRutaExportRecibos.Size = new System.Drawing.Size(240, 25);
            this.txtRutaExportRecibos.TabIndex = 9;
            //
            // lblRutaExport
            //
            this.lblRutaExport.AutoSize = true;
            this.lblRutaExport.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblRutaExport.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblRutaExport.Location = new System.Drawing.Point(20, 172);
            this.lblRutaExport.Name = "lblRutaExport";
            this.lblRutaExport.Size = new System.Drawing.Size(170, 17);
            this.lblRutaExport.TabIndex = 8;
            this.lblRutaExport.Text = "Ruta exportación de recibos";
            //
            // numDiasRetencion
            //
            this.numDiasRetencion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numDiasRetencion.Location = new System.Drawing.Point(200, 131);
            this.numDiasRetencion.Maximum = new decimal(new int[] { 3650, 0, 0, 0 });
            this.numDiasRetencion.Name = "numDiasRetencion";
            this.numDiasRetencion.Size = new System.Drawing.Size(90, 25);
            this.numDiasRetencion.TabIndex = 7;
            this.numDiasRetencion.Value = new decimal(new int[] { 30, 0, 0, 0 });
            //
            // lblDiasRetencion
            //
            this.lblDiasRetencion.AutoSize = true;
            this.lblDiasRetencion.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblDiasRetencion.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblDiasRetencion.Location = new System.Drawing.Point(20, 134);
            this.lblDiasRetencion.Name = "lblDiasRetencion";
            this.lblDiasRetencion.Size = new System.Drawing.Size(165, 17);
            this.lblDiasRetencion.TabIndex = 6;
            this.lblDiasRetencion.Text = "Días de retención de logs";
            //
            // cmbNivelLog
            //
            this.cmbNivelLog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNivelLog.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbNivelLog.FormattingEnabled = true;
            this.cmbNivelLog.Items.AddRange(new object[] {
            "Solo errores",
            "Advertencias y errores",
            "Informativo / Debug"});
            this.cmbNivelLog.Location = new System.Drawing.Point(200, 92);
            this.cmbNivelLog.Name = "cmbNivelLog";
            this.cmbNivelLog.Size = new System.Drawing.Size(240, 25);
            this.cmbNivelLog.TabIndex = 5;
            //
            // lblNivelLog
            //
            this.lblNivelLog.AutoSize = true;
            this.lblNivelLog.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblNivelLog.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblNivelLog.Location = new System.Drawing.Point(20, 95);
            this.lblNivelLog.Name = "lblNivelLog";
            this.lblNivelLog.Size = new System.Drawing.Size(113, 17);
            this.lblNivelLog.TabIndex = 4;
            this.lblNivelLog.Text = "Nivel de registro";
            //
            // btnAbrirCarpetaLogs
            //
            this.btnAbrirCarpetaLogs.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnAbrirCarpetaLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbrirCarpetaLogs.FlatAppearance.BorderSize = 0;
            this.btnAbrirCarpetaLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbrirCarpetaLogs.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnAbrirCarpetaLogs.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnAbrirCarpetaLogs.Location = new System.Drawing.Point(200, 52);
            this.btnAbrirCarpetaLogs.Name = "btnAbrirCarpetaLogs";
            this.btnAbrirCarpetaLogs.Size = new System.Drawing.Size(160, 28);
            this.btnAbrirCarpetaLogs.TabIndex = 3;
            this.btnAbrirCarpetaLogs.Text = "Abrir carpeta";
            this.btnAbrirCarpetaLogs.UseVisualStyleBackColor = false;
            this.btnAbrirCarpetaLogs.Click += new System.EventHandler(this.btnAbrirCarpetaLogs_Click);
            //
            // btnExaminarLogs
            //
            this.btnExaminarLogs.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnExaminarLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExaminarLogs.FlatAppearance.BorderSize = 0;
            this.btnExaminarLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExaminarLogs.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnExaminarLogs.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnExaminarLogs.Location = new System.Drawing.Point(448, 20);
            this.btnExaminarLogs.Name = "btnExaminarLogs";
            this.btnExaminarLogs.Size = new System.Drawing.Size(44, 27);
            this.btnExaminarLogs.TabIndex = 2;
            this.btnExaminarLogs.Text = "...";
            this.btnExaminarLogs.UseVisualStyleBackColor = false;
            this.btnExaminarLogs.Click += new System.EventHandler(this.btnExaminarLogs_Click);
            //
            // txtCarpetaLogs
            //
            this.txtCarpetaLogs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCarpetaLogs.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCarpetaLogs.Location = new System.Drawing.Point(200, 21);
            this.txtCarpetaLogs.MaxLength = 260;
            this.txtCarpetaLogs.Name = "txtCarpetaLogs";
            this.txtCarpetaLogs.Size = new System.Drawing.Size(240, 25);
            this.txtCarpetaLogs.TabIndex = 1;
            //
            // lblCarpetaLogs
            //
            this.lblCarpetaLogs.AutoSize = true;
            this.lblCarpetaLogs.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCarpetaLogs.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblCarpetaLogs.Location = new System.Drawing.Point(20, 24);
            this.lblCarpetaLogs.Name = "lblCarpetaLogs";
            this.lblCarpetaLogs.Size = new System.Drawing.Size(140, 17);
            this.lblCarpetaLogs.TabIndex = 0;
            this.lblCarpetaLogs.Text = "Carpeta de bitácoras";
            //
            // tabGeneral
            //
            this.tabGeneral.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.tabGeneral.Controls.Add(this.btnExaminarRespaldos);
            this.tabGeneral.Controls.Add(this.txtRutaRespaldos);
            this.tabGeneral.Controls.Add(this.lblRutaRespaldos);
            this.tabGeneral.Controls.Add(this.chkVerificarConexionInicio);
            this.tabGeneral.Controls.Add(this.numMinutosInactividad);
            this.tabGeneral.Controls.Add(this.lblMinutosInactividad);
            this.tabGeneral.Location = new System.Drawing.Point(4, 26);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(552, 458);
            this.tabGeneral.TabIndex = 3;
            this.tabGeneral.Text = "General";
            //
            // btnExaminarRespaldos
            //
            this.btnExaminarRespaldos.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnExaminarRespaldos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExaminarRespaldos.FlatAppearance.BorderSize = 0;
            this.btnExaminarRespaldos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExaminarRespaldos.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.btnExaminarRespaldos.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnExaminarRespaldos.Location = new System.Drawing.Point(448, 95);
            this.btnExaminarRespaldos.Name = "btnExaminarRespaldos";
            this.btnExaminarRespaldos.Size = new System.Drawing.Size(44, 27);
            this.btnExaminarRespaldos.TabIndex = 5;
            this.btnExaminarRespaldos.Text = "...";
            this.btnExaminarRespaldos.UseVisualStyleBackColor = false;
            this.btnExaminarRespaldos.Click += new System.EventHandler(this.btnExaminarRespaldos_Click);
            //
            // txtRutaRespaldos
            //
            this.txtRutaRespaldos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRutaRespaldos.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtRutaRespaldos.Location = new System.Drawing.Point(200, 96);
            this.txtRutaRespaldos.MaxLength = 260;
            this.txtRutaRespaldos.Name = "txtRutaRespaldos";
            this.txtRutaRespaldos.Size = new System.Drawing.Size(240, 25);
            this.txtRutaRespaldos.TabIndex = 4;
            //
            // lblRutaRespaldos
            //
            this.lblRutaRespaldos.AutoSize = true;
            this.lblRutaRespaldos.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblRutaRespaldos.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblRutaRespaldos.Location = new System.Drawing.Point(20, 99);
            this.lblRutaRespaldos.Name = "lblRutaRespaldos";
            this.lblRutaRespaldos.Size = new System.Drawing.Size(159, 17);
            this.lblRutaRespaldos.TabIndex = 3;
            this.lblRutaRespaldos.Text = "Ruta de respaldos local";
            //
            // chkVerificarConexionInicio
            //
            this.chkVerificarConexionInicio.AutoSize = true;
            this.chkVerificarConexionInicio.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.chkVerificarConexionInicio.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.chkVerificarConexionInicio.Location = new System.Drawing.Point(20, 59);
            this.chkVerificarConexionInicio.Name = "chkVerificarConexionInicio";
            this.chkVerificarConexionInicio.Size = new System.Drawing.Size(305, 21);
            this.chkVerificarConexionInicio.TabIndex = 2;
            this.chkVerificarConexionInicio.Text = "Verificar la conexión al iniciar el sistema";
            this.chkVerificarConexionInicio.UseVisualStyleBackColor = true;
            //
            // numMinutosInactividad
            //
            this.numMinutosInactividad.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numMinutosInactividad.Location = new System.Drawing.Point(280, 21);
            this.numMinutosInactividad.Maximum = new decimal(new int[] { 480, 0, 0, 0 });
            this.numMinutosInactividad.Name = "numMinutosInactividad";
            this.numMinutosInactividad.Size = new System.Drawing.Size(90, 25);
            this.numMinutosInactividad.TabIndex = 1;
            this.numMinutosInactividad.Value = new decimal(new int[] { 15, 0, 0, 0 });
            //
            // lblMinutosInactividad
            //
            this.lblMinutosInactividad.AutoSize = true;
            this.lblMinutosInactividad.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblMinutosInactividad.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblMinutosInactividad.Location = new System.Drawing.Point(20, 24);
            this.lblMinutosInactividad.Name = "lblMinutosInactividad";
            this.lblMinutosInactividad.Size = new System.Drawing.Size(241, 17);
            this.lblMinutosInactividad.TabIndex = 0;
            this.lblMinutosInactividad.Text = "Inactividad para bloqueo de sesión (min)";
            //
            // FrmConfiguracion
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(584, 640);
            this.Controls.Add(this.tabConfiguracion);
            this.Controls.Add(this.pnlAcciones);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfiguracion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NomiMex - Configuración";
            this.Load += new System.EventHandler(this.FrmConfiguracion_Load);
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.pnlAcciones.ResumeLayout(false);
            this.tabConfiguracion.ResumeLayout(false);
            this.tabConexion.ResumeLayout(false);
            this.tabConexion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            this.tabCorreo.ResumeLayout(false);
            this.tabCorreo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPuerto)).EndInit();
            this.tabBitacoras.ResumeLayout(false);
            this.tabBitacoras.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDiasRetencion)).EndInit();
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinutosInactividad)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.TabControl tabConfiguracion;
        private System.Windows.Forms.TabPage tabConexion;
        private System.Windows.Forms.Label lblServidor;
        private System.Windows.Forms.TextBox txtServidor;
        private System.Windows.Forms.Label lblBaseDatos;
        private System.Windows.Forms.TextBox txtBaseDatos;
        private System.Windows.Forms.Label lblAutenticacion;
        private System.Windows.Forms.ComboBox cmbAutenticacion;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkMostrarPassword;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.Button btnProbarConexion;
        private System.Windows.Forms.Label lblEstadoConexion;
        private System.Windows.Forms.TabPage tabCorreo;
        private System.Windows.Forms.Label lblSmtpServidor;
        private System.Windows.Forms.TextBox txtSmtpServidor;
        private System.Windows.Forms.Label lblPuerto;
        private System.Windows.Forms.NumericUpDown numPuerto;
        private System.Windows.Forms.CheckBox chkSsl;
        private System.Windows.Forms.Label lblRemitente;
        private System.Windows.Forms.TextBox txtRemitente;
        private System.Windows.Forms.Label lblNombreMostrar;
        private System.Windows.Forms.TextBox txtNombreMostrar;
        private System.Windows.Forms.Label lblSmtpPassword;
        private System.Windows.Forms.TextBox txtSmtpPassword;
        private System.Windows.Forms.CheckBox chkMostrarSmtp;
        private System.Windows.Forms.Label lblAsunto;
        private System.Windows.Forms.TextBox txtAsunto;
        private System.Windows.Forms.Label lblCuerpo;
        private System.Windows.Forms.TextBox txtCuerpo;
        private System.Windows.Forms.Label lblCorreoPrueba;
        private System.Windows.Forms.TextBox txtCorreoPrueba;
        private System.Windows.Forms.Button btnEnviarPrueba;
        private System.Windows.Forms.TabPage tabBitacoras;
        private System.Windows.Forms.Label lblCarpetaLogs;
        private System.Windows.Forms.TextBox txtCarpetaLogs;
        private System.Windows.Forms.Button btnExaminarLogs;
        private System.Windows.Forms.Button btnAbrirCarpetaLogs;
        private System.Windows.Forms.Label lblNivelLog;
        private System.Windows.Forms.ComboBox cmbNivelLog;
        private System.Windows.Forms.Label lblDiasRetencion;
        private System.Windows.Forms.NumericUpDown numDiasRetencion;
        private System.Windows.Forms.Label lblRutaExport;
        private System.Windows.Forms.TextBox txtRutaExportRecibos;
        private System.Windows.Forms.Button btnExaminarExport;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.Label lblMinutosInactividad;
        private System.Windows.Forms.NumericUpDown numMinutosInactividad;
        private System.Windows.Forms.CheckBox chkVerificarConexionInicio;
        private System.Windows.Forms.Label lblRutaRespaldos;
        private System.Windows.Forms.TextBox txtRutaRespaldos;
        private System.Windows.Forms.Button btnExaminarRespaldos;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}
