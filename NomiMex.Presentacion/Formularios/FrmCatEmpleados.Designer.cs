namespace NomiMex.Presentacion.Formularios
{
    partial class FrmCatEmpleados
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabPage tabIncidenciasFijas;
        private System.Windows.Forms.DataGridView dgvIncidenciasFijas;
        private System.Windows.Forms.ComboBox cmbConceptoFijo;
        private System.Windows.Forms.NumericUpDown nudCantidadFija;
        private System.Windows.Forms.TextBox txtObservacionesFija;
        private System.Windows.Forms.CheckBox chkActivaFija;
        private System.Windows.Forms.Button btnGuardarFija;
        private System.Windows.Forms.Button btnDesactivarFija;
        private System.Windows.Forms.Button btnNuevaFija;
        private System.Windows.Forms.Label lblConceptoFijo;
        private System.Windows.Forms.Label lblCantidadFija;
        private System.Windows.Forms.Label lblObsFija;
        private System.Windows.Forms.Button btnCatalogoSAT;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabEmpleado = new System.Windows.Forms.TabControl();
            this.tabGenerales = new System.Windows.Forms.TabPage();
            this.txtNacionalidad = new System.Windows.Forms.TextBox();
            this.lblNacionalidad = new System.Windows.Forms.Label();
            this.cmbEstadoCivil = new System.Windows.Forms.ComboBox();
            this.lblEstadoCivil = new System.Windows.Forms.Label();
            this.cmbSexo = new System.Windows.Forms.ComboBox();
            this.lblSexo = new System.Windows.Forms.Label();
            this.dtpFechaNacimiento = new System.Windows.Forms.DateTimePicker();
            this.lblFechaNacimiento = new System.Windows.Forms.Label();
            this.dtpFechaIngreso = new System.Windows.Forms.DateTimePicker();
            this.lblFechaIngreso = new System.Windows.Forms.Label();
            this.txtApellidoMaterno = new System.Windows.Forms.TextBox();
            this.lblApellidoMaterno = new System.Windows.Forms.Label();
            this.txtApellidoPaterno = new System.Windows.Forms.TextBox();
            this.lblApellidoPaterno = new System.Windows.Forms.Label();
            this.txtNombres = new System.Windows.Forms.TextBox();
            this.lblNombres = new System.Windows.Forms.Label();
            this.cmbEstatus = new System.Windows.Forms.ComboBox();
            this.lblEstatus = new System.Windows.Forms.Label();
            this.txtNumeroEmpleado = new System.Windows.Forms.TextBox();
            this.lblNumeroEmpleado = new System.Windows.Forms.Label();
            this.cmbEmpresa = new System.Windows.Forms.ComboBox();
            this.lblEmpresa = new System.Windows.Forms.Label();
            this.tabFiscales = new System.Windows.Forms.TabPage();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.cmbEstadoFiscal = new System.Windows.Forms.ComboBox();
            this.lblEstadoFiscal = new System.Windows.Forms.Label();
            this.txtCodigoPostal = new System.Windows.Forms.TextBox();
            this.lblCodigoPostal = new System.Windows.Forms.Label();
            this.cmbRegimenFiscal = new System.Windows.Forms.ComboBox();
            this.lblRegimenFiscal = new System.Windows.Forms.Label();
            this.txtCURP = new System.Windows.Forms.TextBox();
            this.lblCURP = new System.Windows.Forms.Label();
            this.txtRFC = new System.Windows.Forms.TextBox();
            this.lblRFC = new System.Windows.Forms.Label();
            this.tabIMSS = new System.Windows.Forms.TabPage();
            this.txtCLABE = new System.Windows.Forms.TextBox();
            this.lblCLABE = new System.Windows.Forms.Label();
            this.cmbBanco = new System.Windows.Forms.ComboBox();
            this.lblBanco = new System.Windows.Forms.Label();
            this.lblFactor = new System.Windows.Forms.Label();
            this.btnCalcularSDI = new System.Windows.Forms.Button();
            this.txtSBC = new System.Windows.Forms.TextBox();
            this.lblSBC = new System.Windows.Forms.Label();
            this.txtSDI = new System.Windows.Forms.TextBox();
            this.lblSDI = new System.Windows.Forms.Label();
            this.cmbRiesgo = new System.Windows.Forms.ComboBox();
            this.lblRiesgo = new System.Windows.Forms.Label();
            this.txtSalarioDiario = new System.Windows.Forms.TextBox();
            this.lblSalarioDiario = new System.Windows.Forms.Label();
            this.txtNSS = new System.Windows.Forms.TextBox();
            this.lblNSS = new System.Windows.Forms.Label();
            this.tabContrato = new System.Windows.Forms.TabPage();
            this.cmbPuesto = new System.Windows.Forms.ComboBox();
            this.lblPuesto = new System.Windows.Forms.Label();
            this.cmbDepartamento = new System.Windows.Forms.ComboBox();
            this.lblDepartamento = new System.Windows.Forms.Label();
            this.dtpFechaInicioContrato = new System.Windows.Forms.DateTimePicker();
            this.lblFechaInicioContrato = new System.Windows.Forms.Label();
            this.cmbZonaSMG = new System.Windows.Forms.ComboBox();
            this.cmbDiasSemana = new System.Windows.Forms.ComboBox();
            this.lblDiasSemana = new System.Windows.Forms.Label();
            this.btnDiasJornada = new System.Windows.Forms.Button();
            this.lblZonaSMG = new System.Windows.Forms.Label();
            this.cmbTipoPeriodo = new System.Windows.Forms.ComboBox();
            this.lblTipoPeriodo = new System.Windows.Forms.Label();
            this.cmbTipoJornada = new System.Windows.Forms.ComboBox();
            this.lblTipoJornada = new System.Windows.Forms.Label();
            this.cmbTipoRegimen = new System.Windows.Forms.ComboBox();
            this.lblTipoRegimen = new System.Windows.Forms.Label();
            this.cmbTipoContrato = new System.Windows.Forms.ComboBox();
            this.lblTipoContrato = new System.Windows.Forms.Label();
            this.tabIncidenciasFijas = new System.Windows.Forms.TabPage();
            this.dgvIncidenciasFijas = new System.Windows.Forms.DataGridView();
            this.btnCatalogoSAT = new System.Windows.Forms.Button();
            this.cmbConceptoFijo = new System.Windows.Forms.ComboBox();
            this.nudCantidadFija = new System.Windows.Forms.NumericUpDown();
            this.txtObservacionesFija = new System.Windows.Forms.TextBox();
            this.chkActivaFija = new System.Windows.Forms.CheckBox();
            this.btnGuardarFija = new System.Windows.Forms.Button();
            this.btnDesactivarFija = new System.Windows.Forms.Button();
            this.btnNuevaFija = new System.Windows.Forms.Button();
            this.lblConceptoFijo = new System.Windows.Forms.Label();
            this.lblCantidadFija = new System.Windows.Forms.Label();
            this.lblObsFija = new System.Windows.Forms.Label();
            this.pnlBotones = new System.Windows.Forms.Panel();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.pnlLista = new System.Windows.Forms.Panel();
            this.dgvEmpleados = new System.Windows.Forms.DataGridView();
            this.pnlBusqueda = new System.Windows.Forms.Panel();
            this.lblConteo = new System.Windows.Forms.Label();
            this.cmbFiltroEmpresa = new System.Windows.Forms.ComboBox();
            this.lblFiltroEmpresa = new System.Windows.Forms.Label();
            this.chkSoloActivos = new System.Windows.Forms.CheckBox();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.tabEmpleado.SuspendLayout();
            this.tabGenerales.SuspendLayout();
            this.tabFiscales.SuspendLayout();
            this.tabIMSS.SuspendLayout();
            this.tabContrato.SuspendLayout();
            this.tabIncidenciasFijas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIncidenciasFijas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadFija)).BeginInit();
            this.pnlBotones.SuspendLayout();
            this.pnlLista.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpleados)).BeginInit();
            this.pnlBusqueda.SuspendLayout();
            this.pnlEncabezado.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabEmpleado
            // 
            this.tabEmpleado.Controls.Add(this.tabGenerales);
            this.tabEmpleado.Controls.Add(this.tabFiscales);
            this.tabEmpleado.Controls.Add(this.tabIMSS);
            this.tabEmpleado.Controls.Add(this.tabContrato);
            this.tabEmpleado.Controls.Add(this.tabIncidenciasFijas);
            this.tabEmpleado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabEmpleado.Location = new System.Drawing.Point(0, 314);
            this.tabEmpleado.Name = "tabEmpleado";
            this.tabEmpleado.SelectedIndex = 0;
            this.tabEmpleado.Size = new System.Drawing.Size(1050, 328);
            this.tabEmpleado.TabIndex = 0;
            // 
            // tabGenerales
            // 
            this.tabGenerales.AutoScroll = true;
            this.tabGenerales.BackColor = System.Drawing.Color.White;
            this.tabGenerales.Controls.Add(this.txtNacionalidad);
            this.tabGenerales.Controls.Add(this.lblNacionalidad);
            this.tabGenerales.Controls.Add(this.cmbEstadoCivil);
            this.tabGenerales.Controls.Add(this.lblEstadoCivil);
            this.tabGenerales.Controls.Add(this.cmbSexo);
            this.tabGenerales.Controls.Add(this.lblSexo);
            this.tabGenerales.Controls.Add(this.dtpFechaNacimiento);
            this.tabGenerales.Controls.Add(this.lblFechaNacimiento);
            this.tabGenerales.Controls.Add(this.dtpFechaIngreso);
            this.tabGenerales.Controls.Add(this.lblFechaIngreso);
            this.tabGenerales.Controls.Add(this.txtApellidoMaterno);
            this.tabGenerales.Controls.Add(this.lblApellidoMaterno);
            this.tabGenerales.Controls.Add(this.txtApellidoPaterno);
            this.tabGenerales.Controls.Add(this.lblApellidoPaterno);
            this.tabGenerales.Controls.Add(this.txtNombres);
            this.tabGenerales.Controls.Add(this.lblNombres);
            this.tabGenerales.Controls.Add(this.cmbEstatus);
            this.tabGenerales.Controls.Add(this.lblEstatus);
            this.tabGenerales.Controls.Add(this.txtNumeroEmpleado);
            this.tabGenerales.Controls.Add(this.lblNumeroEmpleado);
            this.tabGenerales.Controls.Add(this.cmbEmpresa);
            this.tabGenerales.Controls.Add(this.lblEmpresa);
            this.tabGenerales.Location = new System.Drawing.Point(4, 24);
            this.tabGenerales.Name = "tabGenerales";
            this.tabGenerales.Padding = new System.Windows.Forms.Padding(3);
            this.tabGenerales.Size = new System.Drawing.Size(1042, 300);
            this.tabGenerales.TabIndex = 0;
            this.tabGenerales.Text = "Datos generales";
            // 
            // txtNacionalidad
            // 
            this.txtNacionalidad.Location = new System.Drawing.Point(300, 197);
            this.txtNacionalidad.MaxLength = 50;
            this.txtNacionalidad.Name = "txtNacionalidad";
            this.txtNacionalidad.Size = new System.Drawing.Size(265, 23);
            this.txtNacionalidad.TabIndex = 21;
            // 
            // lblNacionalidad
            // 
            this.lblNacionalidad.AutoSize = true;
            this.lblNacionalidad.Location = new System.Drawing.Point(300, 177);
            this.lblNacionalidad.Name = "lblNacionalidad";
            this.lblNacionalidad.Size = new System.Drawing.Size(77, 15);
            this.lblNacionalidad.TabIndex = 20;
            this.lblNacionalidad.Text = "Nacionalidad";
            // 
            // cmbEstadoCivil
            // 
            this.cmbEstadoCivil.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstadoCivil.FormattingEnabled = true;
            this.cmbEstadoCivil.Location = new System.Drawing.Point(15, 197);
            this.cmbEstadoCivil.Name = "cmbEstadoCivil";
            this.cmbEstadoCivil.Size = new System.Drawing.Size(265, 23);
            this.cmbEstadoCivil.TabIndex = 19;
            // 
            // lblEstadoCivil
            // 
            this.lblEstadoCivil.AutoSize = true;
            this.lblEstadoCivil.Location = new System.Drawing.Point(15, 177);
            this.lblEstadoCivil.Name = "lblEstadoCivil";
            this.lblEstadoCivil.Size = new System.Drawing.Size(66, 15);
            this.lblEstadoCivil.TabIndex = 18;
            this.lblEstadoCivil.Text = "Estado civil";
            // 
            // cmbSexo
            // 
            this.cmbSexo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSexo.FormattingEnabled = true;
            this.cmbSexo.Location = new System.Drawing.Point(585, 142);
            this.cmbSexo.Name = "cmbSexo";
            this.cmbSexo.Size = new System.Drawing.Size(265, 23);
            this.cmbSexo.TabIndex = 17;
            // 
            // lblSexo
            // 
            this.lblSexo.AutoSize = true;
            this.lblSexo.Location = new System.Drawing.Point(585, 122);
            this.lblSexo.Name = "lblSexo";
            this.lblSexo.Size = new System.Drawing.Size(31, 15);
            this.lblSexo.TabIndex = 16;
            this.lblSexo.Text = "Sexo";
            // 
            // dtpFechaNacimiento
            // 
            this.dtpFechaNacimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaNacimiento.Location = new System.Drawing.Point(300, 142);
            this.dtpFechaNacimiento.Name = "dtpFechaNacimiento";
            this.dtpFechaNacimiento.ShowCheckBox = true;
            this.dtpFechaNacimiento.Size = new System.Drawing.Size(265, 23);
            this.dtpFechaNacimiento.TabIndex = 15;
            // 
            // lblFechaNacimiento
            // 
            this.lblFechaNacimiento.AutoSize = true;
            this.lblFechaNacimiento.Location = new System.Drawing.Point(300, 122);
            this.lblFechaNacimiento.Name = "lblFechaNacimiento";
            this.lblFechaNacimiento.Size = new System.Drawing.Size(117, 15);
            this.lblFechaNacimiento.TabIndex = 14;
            this.lblFechaNacimiento.Text = "Fecha de nacimiento";
            // 
            // dtpFechaIngreso
            // 
            this.dtpFechaIngreso.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaIngreso.Location = new System.Drawing.Point(15, 142);
            this.dtpFechaIngreso.Name = "dtpFechaIngreso";
            this.dtpFechaIngreso.Size = new System.Drawing.Size(265, 23);
            this.dtpFechaIngreso.TabIndex = 13;
            // 
            // lblFechaIngreso
            // 
            this.lblFechaIngreso.AutoSize = true;
            this.lblFechaIngreso.Location = new System.Drawing.Point(15, 122);
            this.lblFechaIngreso.Name = "lblFechaIngreso";
            this.lblFechaIngreso.Size = new System.Drawing.Size(104, 15);
            this.lblFechaIngreso.TabIndex = 12;
            this.lblFechaIngreso.Text = "Fecha de ingreso *";
            // 
            // txtApellidoMaterno
            // 
            this.txtApellidoMaterno.Location = new System.Drawing.Point(585, 87);
            this.txtApellidoMaterno.MaxLength = 100;
            this.txtApellidoMaterno.Name = "txtApellidoMaterno";
            this.txtApellidoMaterno.Size = new System.Drawing.Size(265, 23);
            this.txtApellidoMaterno.TabIndex = 11;
            // 
            // lblApellidoMaterno
            // 
            this.lblApellidoMaterno.AutoSize = true;
            this.lblApellidoMaterno.Location = new System.Drawing.Point(585, 67);
            this.lblApellidoMaterno.Name = "lblApellidoMaterno";
            this.lblApellidoMaterno.Size = new System.Drawing.Size(99, 15);
            this.lblApellidoMaterno.TabIndex = 10;
            this.lblApellidoMaterno.Text = "Apellido materno";
            // 
            // txtApellidoPaterno
            // 
            this.txtApellidoPaterno.Location = new System.Drawing.Point(300, 87);
            this.txtApellidoPaterno.MaxLength = 100;
            this.txtApellidoPaterno.Name = "txtApellidoPaterno";
            this.txtApellidoPaterno.Size = new System.Drawing.Size(265, 23);
            this.txtApellidoPaterno.TabIndex = 9;
            // 
            // lblApellidoPaterno
            // 
            this.lblApellidoPaterno.AutoSize = true;
            this.lblApellidoPaterno.Location = new System.Drawing.Point(300, 67);
            this.lblApellidoPaterno.Name = "lblApellidoPaterno";
            this.lblApellidoPaterno.Size = new System.Drawing.Size(103, 15);
            this.lblApellidoPaterno.TabIndex = 8;
            this.lblApellidoPaterno.Text = "Apellido paterno *";
            // 
            // txtNombres
            // 
            this.txtNombres.Location = new System.Drawing.Point(15, 87);
            this.txtNombres.MaxLength = 100;
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.Size = new System.Drawing.Size(265, 23);
            this.txtNombres.TabIndex = 7;
            // 
            // lblNombres
            // 
            this.lblNombres.AutoSize = true;
            this.lblNombres.Location = new System.Drawing.Point(15, 67);
            this.lblNombres.Name = "lblNombres";
            this.lblNombres.Size = new System.Drawing.Size(72, 15);
            this.lblNombres.TabIndex = 6;
            this.lblNombres.Text = "Nombre(s) *";
            // 
            // cmbEstatus
            // 
            this.cmbEstatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstatus.FormattingEnabled = true;
            this.cmbEstatus.Location = new System.Drawing.Point(585, 32);
            this.cmbEstatus.Name = "cmbEstatus";
            this.cmbEstatus.Size = new System.Drawing.Size(265, 23);
            this.cmbEstatus.TabIndex = 5;
            // 
            // lblEstatus
            // 
            this.lblEstatus.AutoSize = true;
            this.lblEstatus.Location = new System.Drawing.Point(585, 12);
            this.lblEstatus.Name = "lblEstatus";
            this.lblEstatus.Size = new System.Drawing.Size(44, 15);
            this.lblEstatus.TabIndex = 4;
            this.lblEstatus.Text = "Estatus";
            // 
            // txtNumeroEmpleado
            // 
            this.txtNumeroEmpleado.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNumeroEmpleado.Location = new System.Drawing.Point(300, 32);
            this.txtNumeroEmpleado.MaxLength = 15;
            this.txtNumeroEmpleado.Name = "txtNumeroEmpleado";
            this.txtNumeroEmpleado.Size = new System.Drawing.Size(265, 23);
            this.txtNumeroEmpleado.TabIndex = 3;
            // 
            // lblNumeroEmpleado
            // 
            this.lblNumeroEmpleado.AutoSize = true;
            this.lblNumeroEmpleado.Location = new System.Drawing.Point(300, 12);
            this.lblNumeroEmpleado.Name = "lblNumeroEmpleado";
            this.lblNumeroEmpleado.Size = new System.Drawing.Size(106, 15);
            this.lblNumeroEmpleado.TabIndex = 2;
            this.lblNumeroEmpleado.Text = "No. de empleado *";
            // 
            // cmbEmpresa
            // 
            this.cmbEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmpresa.FormattingEnabled = true;
            this.cmbEmpresa.Location = new System.Drawing.Point(15, 32);
            this.cmbEmpresa.Name = "cmbEmpresa";
            this.cmbEmpresa.Size = new System.Drawing.Size(265, 23);
            this.cmbEmpresa.TabIndex = 1;
            this.cmbEmpresa.SelectedIndexChanged += new System.EventHandler(this.cmbEmpresa_SelectedIndexChanged);
            // 
            // lblEmpresa
            // 
            this.lblEmpresa.AutoSize = true;
            this.lblEmpresa.Location = new System.Drawing.Point(15, 12);
            this.lblEmpresa.Name = "lblEmpresa";
            this.lblEmpresa.Size = new System.Drawing.Size(60, 15);
            this.lblEmpresa.TabIndex = 0;
            this.lblEmpresa.Text = "Empresa *";
            // 
            // tabFiscales
            // 
            this.tabFiscales.AutoScroll = true;
            this.tabFiscales.BackColor = System.Drawing.Color.White;
            this.tabFiscales.Controls.Add(this.txtTelefono);
            this.tabFiscales.Controls.Add(this.lblTelefono);
            this.tabFiscales.Controls.Add(this.txtEmail);
            this.tabFiscales.Controls.Add(this.lblEmail);
            this.tabFiscales.Controls.Add(this.cmbEstadoFiscal);
            this.tabFiscales.Controls.Add(this.lblEstadoFiscal);
            this.tabFiscales.Controls.Add(this.txtCodigoPostal);
            this.tabFiscales.Controls.Add(this.lblCodigoPostal);
            this.tabFiscales.Controls.Add(this.cmbRegimenFiscal);
            this.tabFiscales.Controls.Add(this.lblRegimenFiscal);
            this.tabFiscales.Controls.Add(this.txtCURP);
            this.tabFiscales.Controls.Add(this.lblCURP);
            this.tabFiscales.Controls.Add(this.txtRFC);
            this.tabFiscales.Controls.Add(this.lblRFC);
            this.tabFiscales.Location = new System.Drawing.Point(4, 24);
            this.tabFiscales.Name = "tabFiscales";
            this.tabFiscales.Padding = new System.Windows.Forms.Padding(3);
            this.tabFiscales.Size = new System.Drawing.Size(1042, 300);
            this.tabFiscales.TabIndex = 1;
            this.tabFiscales.Text = "Datos fiscales";
            // 
            // txtTelefono
            // 
            this.txtTelefono.Location = new System.Drawing.Point(15, 142);
            this.txtTelefono.MaxLength = 20;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(265, 23);
            this.txtTelefono.TabIndex = 13;
            // 
            // lblTelefono
            // 
            this.lblTelefono.AutoSize = true;
            this.lblTelefono.Location = new System.Drawing.Point(15, 122);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(53, 15);
            this.lblTelefono.TabIndex = 12;
            this.lblTelefono.Text = "Teléfono";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(585, 87);
            this.txtEmail.MaxLength = 100;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(265, 23);
            this.txtEmail.TabIndex = 11;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(585, 67);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(105, 15);
            this.lblEmail.TabIndex = 10;
            this.lblEmail.Text = "Correo electrónico";
            // 
            // cmbEstadoFiscal
            // 
            this.cmbEstadoFiscal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstadoFiscal.FormattingEnabled = true;
            this.cmbEstadoFiscal.Location = new System.Drawing.Point(300, 87);
            this.cmbEstadoFiscal.Name = "cmbEstadoFiscal";
            this.cmbEstadoFiscal.Size = new System.Drawing.Size(265, 23);
            this.cmbEstadoFiscal.TabIndex = 9;
            // 
            // lblEstadoFiscal
            // 
            this.lblEstadoFiscal.AutoSize = true;
            this.lblEstadoFiscal.Location = new System.Drawing.Point(300, 67);
            this.lblEstadoFiscal.Name = "lblEstadoFiscal";
            this.lblEstadoFiscal.Size = new System.Drawing.Size(42, 15);
            this.lblEstadoFiscal.TabIndex = 8;
            this.lblEstadoFiscal.Text = "Estado";
            // 
            // txtCodigoPostal
            // 
            this.txtCodigoPostal.Location = new System.Drawing.Point(15, 87);
            this.txtCodigoPostal.MaxLength = 5;
            this.txtCodigoPostal.Name = "txtCodigoPostal";
            this.txtCodigoPostal.Size = new System.Drawing.Size(265, 23);
            this.txtCodigoPostal.TabIndex = 7;
            // 
            // lblCodigoPostal
            // 
            this.lblCodigoPostal.AutoSize = true;
            this.lblCodigoPostal.Location = new System.Drawing.Point(15, 67);
            this.lblCodigoPostal.Name = "lblCodigoPostal";
            this.lblCodigoPostal.Size = new System.Drawing.Size(89, 15);
            this.lblCodigoPostal.TabIndex = 6;
            this.lblCodigoPostal.Text = "Código postal *";
            // 
            // cmbRegimenFiscal
            // 
            this.cmbRegimenFiscal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRegimenFiscal.FormattingEnabled = true;
            this.cmbRegimenFiscal.Location = new System.Drawing.Point(585, 32);
            this.cmbRegimenFiscal.Name = "cmbRegimenFiscal";
            this.cmbRegimenFiscal.Size = new System.Drawing.Size(265, 23);
            this.cmbRegimenFiscal.TabIndex = 5;
            // 
            // lblRegimenFiscal
            // 
            this.lblRegimenFiscal.AutoSize = true;
            this.lblRegimenFiscal.Location = new System.Drawing.Point(585, 12);
            this.lblRegimenFiscal.Name = "lblRegimenFiscal";
            this.lblRegimenFiscal.Size = new System.Drawing.Size(92, 15);
            this.lblRegimenFiscal.TabIndex = 4;
            this.lblRegimenFiscal.Text = "Régimen fiscal *";
            // 
            // txtCURP
            // 
            this.txtCURP.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCURP.Location = new System.Drawing.Point(300, 32);
            this.txtCURP.MaxLength = 18;
            this.txtCURP.Name = "txtCURP";
            this.txtCURP.Size = new System.Drawing.Size(265, 23);
            this.txtCURP.TabIndex = 3;
            // 
            // lblCURP
            // 
            this.lblCURP.AutoSize = true;
            this.lblCURP.Location = new System.Drawing.Point(300, 12);
            this.lblCURP.Name = "lblCURP";
            this.lblCURP.Size = new System.Drawing.Size(45, 15);
            this.lblCURP.TabIndex = 2;
            this.lblCURP.Text = "CURP *";
            // 
            // txtRFC
            // 
            this.txtRFC.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRFC.Location = new System.Drawing.Point(15, 32);
            this.txtRFC.MaxLength = 13;
            this.txtRFC.Name = "txtRFC";
            this.txtRFC.Size = new System.Drawing.Size(265, 23);
            this.txtRFC.TabIndex = 1;
            // 
            // lblRFC
            // 
            this.lblRFC.AutoSize = true;
            this.lblRFC.Location = new System.Drawing.Point(15, 12);
            this.lblRFC.Name = "lblRFC";
            this.lblRFC.Size = new System.Drawing.Size(36, 15);
            this.lblRFC.TabIndex = 0;
            this.lblRFC.Text = "RFC *";
            // 
            // tabIMSS
            // 
            this.tabIMSS.AutoScroll = true;
            this.tabIMSS.BackColor = System.Drawing.Color.White;
            this.tabIMSS.Controls.Add(this.txtCLABE);
            this.tabIMSS.Controls.Add(this.lblCLABE);
            this.tabIMSS.Controls.Add(this.cmbBanco);
            this.tabIMSS.Controls.Add(this.lblBanco);
            this.tabIMSS.Controls.Add(this.lblFactor);
            this.tabIMSS.Controls.Add(this.btnCalcularSDI);
            this.tabIMSS.Controls.Add(this.txtSBC);
            this.tabIMSS.Controls.Add(this.lblSBC);
            this.tabIMSS.Controls.Add(this.txtSDI);
            this.tabIMSS.Controls.Add(this.lblSDI);
            this.tabIMSS.Controls.Add(this.cmbRiesgo);
            this.tabIMSS.Controls.Add(this.lblRiesgo);
            this.tabIMSS.Controls.Add(this.txtSalarioDiario);
            this.tabIMSS.Controls.Add(this.lblSalarioDiario);
            this.tabIMSS.Controls.Add(this.txtNSS);
            this.tabIMSS.Controls.Add(this.lblNSS);
            this.tabIMSS.Location = new System.Drawing.Point(4, 24);
            this.tabIMSS.Name = "tabIMSS";
            this.tabIMSS.Padding = new System.Windows.Forms.Padding(3);
            this.tabIMSS.Size = new System.Drawing.Size(1042, 300);
            this.tabIMSS.TabIndex = 2;
            this.tabIMSS.Text = "IMSS y salario";
            // 
            // txtCLABE
            // 
            this.txtCLABE.Location = new System.Drawing.Point(300, 142);
            this.txtCLABE.MaxLength = 18;
            this.txtCLABE.Name = "txtCLABE";
            this.txtCLABE.Size = new System.Drawing.Size(265, 23);
            this.txtCLABE.TabIndex = 15;
            // 
            // lblCLABE
            // 
            this.lblCLABE.AutoSize = true;
            this.lblCLABE.Location = new System.Drawing.Point(300, 122);
            this.lblCLABE.Name = "lblCLABE";
            this.lblCLABE.Size = new System.Drawing.Size(137, 15);
            this.lblCLABE.TabIndex = 14;
            this.lblCLABE.Text = "CLABE interbancaria (18)";
            // 
            // cmbBanco
            // 
            this.cmbBanco.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBanco.FormattingEnabled = true;
            this.cmbBanco.Location = new System.Drawing.Point(15, 142);
            this.cmbBanco.Name = "cmbBanco";
            this.cmbBanco.Size = new System.Drawing.Size(265, 23);
            this.cmbBanco.TabIndex = 13;
            // 
            // lblBanco
            // 
            this.lblBanco.AutoSize = true;
            this.lblBanco.Location = new System.Drawing.Point(15, 122);
            this.lblBanco.Name = "lblBanco";
            this.lblBanco.Size = new System.Drawing.Size(40, 15);
            this.lblBanco.TabIndex = 12;
            this.lblBanco.Text = "Banco";
            // 
            // lblFactor
            // 
            this.lblFactor.AutoSize = true;
            this.lblFactor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblFactor.Location = new System.Drawing.Point(585, 118);
            this.lblFactor.Name = "lblFactor";
            this.lblFactor.Size = new System.Drawing.Size(130, 15);
            this.lblFactor.TabIndex = 11;
            this.lblFactor.Text = "Factor de integración: -";
            // 
            // btnCalcularSDI
            // 
            this.btnCalcularSDI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnCalcularSDI.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCalcularSDI.FlatAppearance.BorderSize = 0;
            this.btnCalcularSDI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalcularSDI.ForeColor = System.Drawing.Color.White;
            this.btnCalcularSDI.Location = new System.Drawing.Point(585, 86);
            this.btnCalcularSDI.Name = "btnCalcularSDI";
            this.btnCalcularSDI.Size = new System.Drawing.Size(265, 26);
            this.btnCalcularSDI.TabIndex = 10;
            this.btnCalcularSDI.Text = "Calcular SDI / SBC";
            this.btnCalcularSDI.UseVisualStyleBackColor = false;
            this.btnCalcularSDI.Click += new System.EventHandler(this.btnCalcularSDI_Click);
            // 
            // txtSBC
            // 
            this.txtSBC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtSBC.Location = new System.Drawing.Point(300, 87);
            this.txtSBC.Name = "txtSBC";
            this.txtSBC.ReadOnly = true;
            this.txtSBC.Size = new System.Drawing.Size(265, 23);
            this.txtSBC.TabIndex = 9;
            this.txtSBC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblSBC
            // 
            this.lblSBC.AutoSize = true;
            this.lblSBC.Location = new System.Drawing.Point(300, 67);
            this.lblSBC.Name = "lblSBC";
            this.lblSBC.Size = new System.Drawing.Size(174, 15);
            this.lblSBC.TabIndex = 8;
            this.lblSBC.Text = "Salario base de cotización (SBC)";
            // 
            // txtSDI
            // 
            this.txtSDI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtSDI.Location = new System.Drawing.Point(15, 87);
            this.txtSDI.Name = "txtSDI";
            this.txtSDI.ReadOnly = true;
            this.txtSDI.Size = new System.Drawing.Size(265, 23);
            this.txtSDI.TabIndex = 7;
            this.txtSDI.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblSDI
            // 
            this.lblSDI.AutoSize = true;
            this.lblSDI.Location = new System.Drawing.Point(15, 67);
            this.lblSDI.Name = "lblSDI";
            this.lblSDI.Size = new System.Drawing.Size(157, 15);
            this.lblSDI.TabIndex = 6;
            this.lblSDI.Text = "Salario diario integrado (SDI)";
            // 
            // cmbRiesgo
            // 
            this.cmbRiesgo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRiesgo.FormattingEnabled = true;
            this.cmbRiesgo.Location = new System.Drawing.Point(585, 32);
            this.cmbRiesgo.Name = "cmbRiesgo";
            this.cmbRiesgo.Size = new System.Drawing.Size(265, 23);
            this.cmbRiesgo.TabIndex = 5;
            // 
            // lblRiesgo
            // 
            this.lblRiesgo.AutoSize = true;
            this.lblRiesgo.Location = new System.Drawing.Point(585, 12);
            this.lblRiesgo.Name = "lblRiesgo";
            this.lblRiesgo.Size = new System.Drawing.Size(94, 15);
            this.lblRiesgo.TabIndex = 4;
            this.lblRiesgo.Text = "Clase de riesgo *";
            // 
            // txtSalarioDiario
            // 
            this.txtSalarioDiario.Location = new System.Drawing.Point(300, 32);
            this.txtSalarioDiario.MaxLength = 18;
            this.txtSalarioDiario.Name = "txtSalarioDiario";
            this.txtSalarioDiario.Size = new System.Drawing.Size(265, 23);
            this.txtSalarioDiario.TabIndex = 3;
            this.txtSalarioDiario.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblSalarioDiario
            // 
            this.lblSalarioDiario.AutoSize = true;
            this.lblSalarioDiario.Location = new System.Drawing.Point(300, 12);
            this.lblSalarioDiario.Name = "lblSalarioDiario";
            this.lblSalarioDiario.Size = new System.Drawing.Size(83, 15);
            this.lblSalarioDiario.TabIndex = 2;
            this.lblSalarioDiario.Text = "Salario diario *";
            // 
            // txtNSS
            // 
            this.txtNSS.Location = new System.Drawing.Point(15, 32);
            this.txtNSS.MaxLength = 11;
            this.txtNSS.Name = "txtNSS";
            this.txtNSS.Size = new System.Drawing.Size(265, 23);
            this.txtNSS.TabIndex = 1;
            // 
            // lblNSS
            // 
            this.lblNSS.AutoSize = true;
            this.lblNSS.Location = new System.Drawing.Point(15, 12);
            this.lblNSS.Name = "lblNSS";
            this.lblNSS.Size = new System.Drawing.Size(98, 15);
            this.lblNSS.TabIndex = 0;
            this.lblNSS.Text = "NSS (11 dígitos) *";
            // 
            // tabContrato
            // 
            this.tabContrato.AutoScroll = true;
            this.tabContrato.BackColor = System.Drawing.Color.White;
            this.tabContrato.Controls.Add(this.cmbPuesto);
            this.tabContrato.Controls.Add(this.lblPuesto);
            this.tabContrato.Controls.Add(this.cmbDepartamento);
            this.tabContrato.Controls.Add(this.lblDepartamento);
            this.tabContrato.Controls.Add(this.dtpFechaInicioContrato);
            this.tabContrato.Controls.Add(this.lblFechaInicioContrato);
            this.tabContrato.Controls.Add(this.cmbZonaSMG);
            this.tabContrato.Controls.Add(this.cmbDiasSemana);
            this.tabContrato.Controls.Add(this.lblDiasSemana);
            this.tabContrato.Controls.Add(this.btnDiasJornada);
            this.tabContrato.Controls.Add(this.lblZonaSMG);
            this.tabContrato.Controls.Add(this.cmbTipoPeriodo);
            this.tabContrato.Controls.Add(this.lblTipoPeriodo);
            this.tabContrato.Controls.Add(this.cmbTipoJornada);
            this.tabContrato.Controls.Add(this.lblTipoJornada);
            this.tabContrato.Controls.Add(this.cmbTipoRegimen);
            this.tabContrato.Controls.Add(this.lblTipoRegimen);
            this.tabContrato.Controls.Add(this.cmbTipoContrato);
            this.tabContrato.Controls.Add(this.lblTipoContrato);
            this.tabContrato.Location = new System.Drawing.Point(4, 24);
            this.tabContrato.Name = "tabContrato";
            this.tabContrato.Padding = new System.Windows.Forms.Padding(3);
            this.tabContrato.Size = new System.Drawing.Size(1042, 300);
            this.tabContrato.TabIndex = 3;
            this.tabContrato.Text = "Contrato y puesto";
            // 
            // cmbPuesto
            // 
            this.cmbPuesto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPuesto.FormattingEnabled = true;
            this.cmbPuesto.Location = new System.Drawing.Point(300, 142);
            this.cmbPuesto.Name = "cmbPuesto";
            this.cmbPuesto.Size = new System.Drawing.Size(265, 23);
            this.cmbPuesto.TabIndex = 15;
            // 
            // lblPuesto
            // 
            this.lblPuesto.AutoSize = true;
            this.lblPuesto.Location = new System.Drawing.Point(300, 122);
            this.lblPuesto.Name = "lblPuesto";
            this.lblPuesto.Size = new System.Drawing.Size(43, 15);
            this.lblPuesto.TabIndex = 14;
            this.lblPuesto.Text = "Puesto";
            // 
            // cmbDepartamento
            // 
            this.cmbDepartamento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepartamento.FormattingEnabled = true;
            this.cmbDepartamento.Location = new System.Drawing.Point(15, 142);
            this.cmbDepartamento.Name = "cmbDepartamento";
            this.cmbDepartamento.Size = new System.Drawing.Size(265, 23);
            this.cmbDepartamento.TabIndex = 13;
            // 
            // lblDepartamento
            // 
            this.lblDepartamento.AutoSize = true;
            this.lblDepartamento.Location = new System.Drawing.Point(15, 122);
            this.lblDepartamento.Name = "lblDepartamento";
            this.lblDepartamento.Size = new System.Drawing.Size(83, 15);
            this.lblDepartamento.TabIndex = 12;
            this.lblDepartamento.Text = "Departamento";
            // 
            // dtpFechaInicioContrato
            // 
            this.dtpFechaInicioContrato.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaInicioContrato.Location = new System.Drawing.Point(585, 87);
            this.dtpFechaInicioContrato.Name = "dtpFechaInicioContrato";
            this.dtpFechaInicioContrato.Size = new System.Drawing.Size(265, 23);
            this.dtpFechaInicioContrato.TabIndex = 11;
            // 
            // lblFechaInicioContrato
            // 
            this.lblFechaInicioContrato.AutoSize = true;
            this.lblFechaInicioContrato.Location = new System.Drawing.Point(585, 67);
            this.lblFechaInicioContrato.Name = "lblFechaInicioContrato";
            this.lblFechaInicioContrato.Size = new System.Drawing.Size(111, 15);
            this.lblFechaInicioContrato.TabIndex = 10;
            this.lblFechaInicioContrato.Text = "Inicio del contrato *";
            // 
            // cmbZonaSMG
            // 
            this.cmbZonaSMG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZonaSMG.FormattingEnabled = true;
            this.cmbZonaSMG.Location = new System.Drawing.Point(300, 87);
            this.cmbZonaSMG.Name = "cmbZonaSMG";
            this.cmbZonaSMG.Size = new System.Drawing.Size(265, 23);
            this.cmbZonaSMG.TabIndex = 9;
            // 
            // cmbDiasSemana
            // 
            this.cmbDiasSemana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiasSemana.FormattingEnabled = true;
            this.cmbDiasSemana.Location = new System.Drawing.Point(585, 142);
            this.cmbDiasSemana.Name = "cmbDiasSemana";
            this.cmbDiasSemana.Size = new System.Drawing.Size(265, 23);
            this.cmbDiasSemana.TabIndex = 17;
            // 
            // lblDiasSemana
            // 
            this.lblDiasSemana.AutoSize = true;
            this.lblDiasSemana.Location = new System.Drawing.Point(585, 122);
            this.lblDiasSemana.Name = "lblDiasSemana";
            this.lblDiasSemana.Size = new System.Drawing.Size(220, 15);
            this.lblDiasSemana.TabIndex = 16;
            this.lblDiasSemana.Text = "Días por semana (sobrescribe la jornada)";
            // 
            // btnDiasJornada
            // 
            this.btnDiasJornada.Location = new System.Drawing.Point(585, 176);
            this.btnDiasJornada.Name = "btnDiasJornada";
            this.btnDiasJornada.Size = new System.Drawing.Size(265, 28);
            this.btnDiasJornada.TabIndex = 18;
            this.btnDiasJornada.Text = "Días por semana de cada jornada...";
            this.btnDiasJornada.UseVisualStyleBackColor = true;
            this.btnDiasJornada.Click += new System.EventHandler(this.btnDiasJornada_Click);
            // 
            // lblZonaSMG
            // 
            this.lblZonaSMG.AutoSize = true;
            this.lblZonaSMG.Location = new System.Drawing.Point(300, 67);
            this.lblZonaSMG.Name = "lblZonaSMG";
            this.lblZonaSMG.Size = new System.Drawing.Size(116, 15);
            this.lblZonaSMG.TabIndex = 8;
            this.lblZonaSMG.Text = "Zona salario mínimo";
            // 
            // cmbTipoPeriodo
            // 
            this.cmbTipoPeriodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoPeriodo.FormattingEnabled = true;
            this.cmbTipoPeriodo.Location = new System.Drawing.Point(15, 87);
            this.cmbTipoPeriodo.Name = "cmbTipoPeriodo";
            this.cmbTipoPeriodo.Size = new System.Drawing.Size(265, 23);
            this.cmbTipoPeriodo.TabIndex = 7;
            // 
            // lblTipoPeriodo
            // 
            this.lblTipoPeriodo.AutoSize = true;
            this.lblTipoPeriodo.Location = new System.Drawing.Point(15, 67);
            this.lblTipoPeriodo.Name = "lblTipoPeriodo";
            this.lblTipoPeriodo.Size = new System.Drawing.Size(211, 15);
            this.lblTipoPeriodo.TabIndex = 6;
            this.lblTipoPeriodo.Text = "Periodicidad de pago (de la empresa) *";
            // 
            // cmbTipoJornada
            // 
            this.cmbTipoJornada.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoJornada.FormattingEnabled = true;
            this.cmbTipoJornada.Location = new System.Drawing.Point(585, 32);
            this.cmbTipoJornada.Name = "cmbTipoJornada";
            this.cmbTipoJornada.Size = new System.Drawing.Size(265, 23);
            this.cmbTipoJornada.TabIndex = 5;
            // 
            // lblTipoJornada
            // 
            this.lblTipoJornada.AutoSize = true;
            this.lblTipoJornada.Location = new System.Drawing.Point(585, 12);
            this.lblTipoJornada.Name = "lblTipoJornada";
            this.lblTipoJornada.Size = new System.Drawing.Size(98, 15);
            this.lblTipoJornada.TabIndex = 4;
            this.lblTipoJornada.Text = "Tipo de jornada *";
            // 
            // cmbTipoRegimen
            // 
            this.cmbTipoRegimen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoRegimen.FormattingEnabled = true;
            this.cmbTipoRegimen.Location = new System.Drawing.Point(300, 32);
            this.cmbTipoRegimen.Name = "cmbTipoRegimen";
            this.cmbTipoRegimen.Size = new System.Drawing.Size(265, 23);
            this.cmbTipoRegimen.TabIndex = 3;
            // 
            // lblTipoRegimen
            // 
            this.lblTipoRegimen.AutoSize = true;
            this.lblTipoRegimen.Location = new System.Drawing.Point(300, 12);
            this.lblTipoRegimen.Name = "lblTipoRegimen";
            this.lblTipoRegimen.Size = new System.Drawing.Size(148, 15);
            this.lblTipoRegimen.TabIndex = 2;
            this.lblTipoRegimen.Text = "Régimen de contratación *";
            // 
            // cmbTipoContrato
            // 
            this.cmbTipoContrato.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoContrato.FormattingEnabled = true;
            this.cmbTipoContrato.Location = new System.Drawing.Point(15, 32);
            this.cmbTipoContrato.Name = "cmbTipoContrato";
            this.cmbTipoContrato.Size = new System.Drawing.Size(265, 23);
            this.cmbTipoContrato.TabIndex = 1;
            // 
            // lblTipoContrato
            // 
            this.lblTipoContrato.AutoSize = true;
            this.lblTipoContrato.Location = new System.Drawing.Point(15, 12);
            this.lblTipoContrato.Name = "lblTipoContrato";
            this.lblTipoContrato.Size = new System.Drawing.Size(103, 15);
            this.lblTipoContrato.TabIndex = 0;
            this.lblTipoContrato.Text = "Tipo de contrato *";
            // 
            // tabIncidenciasFijas
            // 
            this.tabIncidenciasFijas.Controls.Add(this.dgvIncidenciasFijas);
            this.tabIncidenciasFijas.Controls.Add(this.btnCatalogoSAT);
            this.tabIncidenciasFijas.Controls.Add(this.cmbConceptoFijo);
            this.tabIncidenciasFijas.Controls.Add(this.nudCantidadFija);
            this.tabIncidenciasFijas.Controls.Add(this.txtObservacionesFija);
            this.tabIncidenciasFijas.Controls.Add(this.chkActivaFija);
            this.tabIncidenciasFijas.Controls.Add(this.btnGuardarFija);
            this.tabIncidenciasFijas.Controls.Add(this.btnDesactivarFija);
            this.tabIncidenciasFijas.Controls.Add(this.btnNuevaFija);
            this.tabIncidenciasFijas.Controls.Add(this.lblConceptoFijo);
            this.tabIncidenciasFijas.Controls.Add(this.lblCantidadFija);
            this.tabIncidenciasFijas.Controls.Add(this.lblObsFija);
            this.tabIncidenciasFijas.Location = new System.Drawing.Point(4, 24);
            this.tabIncidenciasFijas.Name = "tabIncidenciasFijas";
            this.tabIncidenciasFijas.Size = new System.Drawing.Size(1042, 300);
            this.tabIncidenciasFijas.TabIndex = 4;
            this.tabIncidenciasFijas.Text = "Conceptos / Incidencias fijas";
            // 
            // dgvIncidenciasFijas
            // 
            this.dgvIncidenciasFijas.AllowUserToAddRows = false;
            this.dgvIncidenciasFijas.AllowUserToDeleteRows = false;
            this.dgvIncidenciasFijas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvIncidenciasFijas.Location = new System.Drawing.Point(12, 12);
            this.dgvIncidenciasFijas.MultiSelect = false;
            this.dgvIncidenciasFijas.Name = "dgvIncidenciasFijas";
            this.dgvIncidenciasFijas.ReadOnly = true;
            this.dgvIncidenciasFijas.RowHeadersVisible = false;
            this.dgvIncidenciasFijas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIncidenciasFijas.Size = new System.Drawing.Size(465, 260);
            this.dgvIncidenciasFijas.TabIndex = 0;
            this.dgvIncidenciasFijas.SelectionChanged += new System.EventHandler(this.dgvIncidenciasFijas_SelectionChanged);
            // 
            // btnCatalogoSAT
            // 
            this.btnCatalogoSAT.Location = new System.Drawing.Point(898, 8);
            this.btnCatalogoSAT.Name = "btnCatalogoSAT";
            this.btnCatalogoSAT.Size = new System.Drawing.Size(102, 28);
            this.btnCatalogoSAT.TabIndex = 11;
            this.btnCatalogoSAT.Text = "Catálogo SAT...";
            this.btnCatalogoSAT.Click += new System.EventHandler(this.btnCatalogoSAT_Click);
            // 
            // cmbConceptoFijo
            // 
            this.cmbConceptoFijo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConceptoFijo.DropDownWidth = 520;
            this.cmbConceptoFijo.Location = new System.Drawing.Point(620, 10);
            this.cmbConceptoFijo.Name = "cmbConceptoFijo";
            this.cmbConceptoFijo.Size = new System.Drawing.Size(270, 23);
            this.cmbConceptoFijo.TabIndex = 1;
            this.cmbConceptoFijo.SelectedIndexChanged += new System.EventHandler(this.cmbConceptoFijo_SelectedIndexChanged);
            // 
            // nudCantidadFija
            // 
            this.nudCantidadFija.DecimalPlaces = 4;
            this.nudCantidadFija.Location = new System.Drawing.Point(620, 44);
            this.nudCantidadFija.Maximum = new decimal(new int[] {
            -1486618625,
            232830643,
            0,
            262144});
            this.nudCantidadFija.Name = "nudCantidadFija";
            this.nudCantidadFija.Size = new System.Drawing.Size(180, 23);
            this.nudCantidadFija.TabIndex = 2;
            this.nudCantidadFija.ThousandsSeparator = true;
            this.nudCantidadFija.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtObservacionesFija
            // 
            this.txtObservacionesFija.Location = new System.Drawing.Point(620, 80);
            this.txtObservacionesFija.MaxLength = 250;
            this.txtObservacionesFija.Name = "txtObservacionesFija";
            this.txtObservacionesFija.Size = new System.Drawing.Size(380, 23);
            this.txtObservacionesFija.TabIndex = 7;
            // 
            // chkActivaFija
            // 
            this.chkActivaFija.Checked = true;
            this.chkActivaFija.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivaFija.Location = new System.Drawing.Point(820, 44);
            this.chkActivaFija.Name = "chkActivaFija";
            this.chkActivaFija.Size = new System.Drawing.Size(160, 24);
            this.chkActivaFija.TabIndex = 4;
            this.chkActivaFija.Text = "Activa";
            // 
            // btnGuardarFija
            // 
            this.btnGuardarFija.Location = new System.Drawing.Point(620, 124);
            this.btnGuardarFija.Name = "btnGuardarFija";
            this.btnGuardarFija.Size = new System.Drawing.Size(200, 34);
            this.btnGuardarFija.TabIndex = 9;
            this.btnGuardarFija.Text = "Guardar / actualizar concepto";
            this.btnGuardarFija.Click += new System.EventHandler(this.btnGuardarFija_Click);
            // 
            // btnDesactivarFija
            // 
            this.btnDesactivarFija.Location = new System.Drawing.Point(830, 124);
            this.btnDesactivarFija.Name = "btnDesactivarFija";
            this.btnDesactivarFija.Size = new System.Drawing.Size(170, 34);
            this.btnDesactivarFija.TabIndex = 10;
            this.btnDesactivarFija.Text = "Quitar del trabajador";
            this.btnDesactivarFija.Click += new System.EventHandler(this.btnDesactivarFija_Click);
            // 
            // btnNuevaFija
            // 
            this.btnNuevaFija.Location = new System.Drawing.Point(490, 124);
            this.btnNuevaFija.Name = "btnNuevaFija";
            this.btnNuevaFija.Size = new System.Drawing.Size(120, 34);
            this.btnNuevaFija.TabIndex = 8;
            this.btnNuevaFija.Text = "Nueva";
            this.btnNuevaFija.Click += new System.EventHandler(this.btnNuevaFija_Click);
            // 
            // lblConceptoFijo
            // 
            this.lblConceptoFijo.Location = new System.Drawing.Point(490, 12);
            this.lblConceptoFijo.Name = "lblConceptoFijo";
            this.lblConceptoFijo.Size = new System.Drawing.Size(118, 24);
            this.lblConceptoFijo.TabIndex = 12;
            this.lblConceptoFijo.Text = "Concepto SAT";
            // 
            // lblCantidadFija
            // 
            this.lblCantidadFija.Location = new System.Drawing.Point(490, 46);
            this.lblCantidadFija.Name = "lblCantidadFija";
            this.lblCantidadFija.Size = new System.Drawing.Size(118, 40);
            this.lblCantidadFija.TabIndex = 13;
            this.lblCantidadFija.Text = "Cantidad";
            // 
            // lblObsFija
            // 
            this.lblObsFija.Location = new System.Drawing.Point(490, 82);
            this.lblObsFija.Name = "lblObsFija";
            this.lblObsFija.Size = new System.Drawing.Size(118, 24);
            this.lblObsFija.TabIndex = 14;
            this.lblObsFija.Text = "Observaciones";
            // 
            // pnlBotones
            // 
            this.pnlBotones.BackColor = System.Drawing.Color.White;
            this.pnlBotones.Controls.Add(this.btnLimpiar);
            this.pnlBotones.Controls.Add(this.btnEliminar);
            this.pnlBotones.Controls.Add(this.btnEditar);
            this.pnlBotones.Controls.Add(this.btnGuardar);
            this.pnlBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBotones.Location = new System.Drawing.Point(0, 642);
            this.pnlBotones.Name = "pnlBotones";
            this.pnlBotones.Size = new System.Drawing.Size(1050, 62);
            this.pnlBotones.TabIndex = 1;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnLimpiar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimpiar.FlatAppearance.BorderSize = 0;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(525, 11);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(160, 40);
            this.btnLimpiar.TabIndex = 3;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.FlatAppearance.BorderSize = 0;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Location = new System.Drawing.Point(355, 11);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(160, 40);
            this.btnEliminar.TabIndex = 2;
            this.btnEliminar.Text = "Eliminar (baja)";
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnEditar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditar.FlatAppearance.BorderSize = 0;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.ForeColor = System.Drawing.Color.White;
            this.btnEditar.Location = new System.Drawing.Point(185, 11);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(160, 40);
            this.btnEditar.TabIndex = 1;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = false;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(15, 11);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(160, 40);
            this.btnGuardar.TabIndex = 0;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // pnlLista
            // 
            this.pnlLista.Controls.Add(this.dgvEmpleados);
            this.pnlLista.Controls.Add(this.pnlBusqueda);
            this.pnlLista.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLista.Location = new System.Drawing.Point(0, 64);
            this.pnlLista.Name = "pnlLista";
            this.pnlLista.Padding = new System.Windows.Forms.Padding(15, 10, 15, 5);
            this.pnlLista.Size = new System.Drawing.Size(1050, 250);
            this.pnlLista.TabIndex = 2;
            // 
            // dgvEmpleados
            // 
            this.dgvEmpleados.AllowUserToAddRows = false;
            this.dgvEmpleados.AllowUserToDeleteRows = false;
            this.dgvEmpleados.AllowUserToResizeRows = false;
            this.dgvEmpleados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmpleados.BackgroundColor = System.Drawing.Color.White;
            this.dgvEmpleados.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEmpleados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvEmpleados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvEmpleados.ColumnHeadersHeight = 32;
            this.dgvEmpleados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvEmpleados.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvEmpleados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmpleados.EnableHeadersVisualStyles = false;
            this.dgvEmpleados.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dgvEmpleados.Location = new System.Drawing.Point(15, 54);
            this.dgvEmpleados.MultiSelect = false;
            this.dgvEmpleados.Name = "dgvEmpleados";
            this.dgvEmpleados.ReadOnly = true;
            this.dgvEmpleados.RowHeadersVisible = false;
            this.dgvEmpleados.RowTemplate.Height = 26;
            this.dgvEmpleados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmpleados.Size = new System.Drawing.Size(1020, 191);
            this.dgvEmpleados.TabIndex = 1;
            this.dgvEmpleados.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEmpleados_CellClick);
            this.dgvEmpleados.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvEmpleados_CellFormatting);
            // 
            // pnlBusqueda
            // 
            this.pnlBusqueda.Controls.Add(this.lblConteo);
            this.pnlBusqueda.Controls.Add(this.cmbFiltroEmpresa);
            this.pnlBusqueda.Controls.Add(this.lblFiltroEmpresa);
            this.pnlBusqueda.Controls.Add(this.chkSoloActivos);
            this.pnlBusqueda.Controls.Add(this.txtBuscar);
            this.pnlBusqueda.Controls.Add(this.lblBuscar);
            this.pnlBusqueda.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBusqueda.Location = new System.Drawing.Point(15, 10);
            this.pnlBusqueda.Name = "pnlBusqueda";
            this.pnlBusqueda.Size = new System.Drawing.Size(1020, 44);
            this.pnlBusqueda.TabIndex = 0;
            // 
            // lblConteo
            // 
            this.lblConteo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConteo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblConteo.Location = new System.Drawing.Point(767, 12);
            this.lblConteo.Name = "lblConteo";
            this.lblConteo.Size = new System.Drawing.Size(250, 20);
            this.lblConteo.TabIndex = 3;
            this.lblConteo.Text = "0 registros";
            this.lblConteo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbFiltroEmpresa
            // 
            this.cmbFiltroEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltroEmpresa.FormattingEnabled = true;
            this.cmbFiltroEmpresa.Location = new System.Drawing.Point(550, 9);
            this.cmbFiltroEmpresa.Name = "cmbFiltroEmpresa";
            this.cmbFiltroEmpresa.Size = new System.Drawing.Size(205, 23);
            this.cmbFiltroEmpresa.TabIndex = 4;
            this.cmbFiltroEmpresa.SelectedIndexChanged += new System.EventHandler(this.cmbFiltroEmpresa_SelectedIndexChanged);
            // 
            // lblFiltroEmpresa
            // 
            this.lblFiltroEmpresa.AutoSize = true;
            this.lblFiltroEmpresa.Location = new System.Drawing.Point(495, 12);
            this.lblFiltroEmpresa.Name = "lblFiltroEmpresa";
            this.lblFiltroEmpresa.Size = new System.Drawing.Size(49, 15);
            this.lblFiltroEmpresa.TabIndex = 5;
            this.lblFiltroEmpresa.Text = "Empresa:";
            // 
            // chkSoloActivos
            // 
            this.chkSoloActivos.AutoSize = true;
            this.chkSoloActivos.Checked = true;
            this.chkSoloActivos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSoloActivos.Location = new System.Drawing.Point(338, 11);
            this.chkSoloActivos.Name = "chkSoloActivos";
            this.chkSoloActivos.Size = new System.Drawing.Size(150, 19);
            this.chkSoloActivos.TabIndex = 2;
            this.chkSoloActivos.Text = "Solo empleados activos";
            this.chkSoloActivos.UseVisualStyleBackColor = true;
            this.chkSoloActivos.CheckedChanged += new System.EventHandler(this.chkSoloActivos_CheckedChanged);
            // 
            // txtBuscar
            // 
            this.txtBuscar.Location = new System.Drawing.Point(74, 9);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(250, 23);
            this.txtBuscar.TabIndex = 1;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // lblBuscar
            // 
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.Location = new System.Drawing.Point(9, 12);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new System.Drawing.Size(45, 15);
            this.lblBuscar.TabIndex = 0;
            this.lblBuscar.Text = "Buscar:";
            // 
            // pnlEncabezado
            // 
            this.pnlEncabezado.BackColor = System.Drawing.Color.White;
            this.pnlEncabezado.Controls.Add(this.lblSubtitulo);
            this.pnlEncabezado.Controls.Add(this.lblTitulo);
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Location = new System.Drawing.Point(0, 0);
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.pnlEncabezado.Size = new System.Drawing.Size(1050, 64);
            this.pnlEncabezado.TabIndex = 3;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(21, 38);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(544, 15);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Expediente completo: datos generales, fiscales, IMSS/salario y contrato. Seleccio" +
    "ne una fila para editar.";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTitulo.Location = new System.Drawing.Point(20, 8);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(104, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Empleados";
            // 
            // FrmCatEmpleados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(1050, 704);
            this.Controls.Add(this.tabEmpleado);
            this.Controls.Add(this.pnlBotones);
            this.Controls.Add(this.pnlLista);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmCatEmpleados";
            this.Text = "Catálogo de empleados";
            this.Load += new System.EventHandler(this.FrmCatEmpleados_Load);
            this.tabEmpleado.ResumeLayout(false);
            this.tabGenerales.ResumeLayout(false);
            this.tabGenerales.PerformLayout();
            this.tabFiscales.ResumeLayout(false);
            this.tabFiscales.PerformLayout();
            this.tabIMSS.ResumeLayout(false);
            this.tabIMSS.PerformLayout();
            this.tabContrato.ResumeLayout(false);
            this.tabContrato.PerformLayout();
            this.tabIncidenciasFijas.ResumeLayout(false);
            this.tabIncidenciasFijas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIncidenciasFijas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadFija)).EndInit();
            this.pnlBotones.ResumeLayout(false);
            this.pnlLista.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpleados)).EndInit();
            this.pnlBusqueda.ResumeLayout(false);
            this.pnlBusqueda.PerformLayout();
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabEmpleado;
        private System.Windows.Forms.TabPage tabGenerales;
        private System.Windows.Forms.TextBox txtNacionalidad;
        private System.Windows.Forms.Label lblNacionalidad;
        private System.Windows.Forms.ComboBox cmbEstadoCivil;
        private System.Windows.Forms.Label lblEstadoCivil;
        private System.Windows.Forms.ComboBox cmbSexo;
        private System.Windows.Forms.Label lblSexo;
        private System.Windows.Forms.DateTimePicker dtpFechaNacimiento;
        private System.Windows.Forms.Label lblFechaNacimiento;
        private System.Windows.Forms.DateTimePicker dtpFechaIngreso;
        private System.Windows.Forms.Label lblFechaIngreso;
        private System.Windows.Forms.TextBox txtApellidoMaterno;
        private System.Windows.Forms.Label lblApellidoMaterno;
        private System.Windows.Forms.TextBox txtApellidoPaterno;
        private System.Windows.Forms.Label lblApellidoPaterno;
        private System.Windows.Forms.TextBox txtNombres;
        private System.Windows.Forms.Label lblNombres;
        private System.Windows.Forms.ComboBox cmbEstatus;
        private System.Windows.Forms.Label lblEstatus;
        private System.Windows.Forms.TextBox txtNumeroEmpleado;
        private System.Windows.Forms.Label lblNumeroEmpleado;
        private System.Windows.Forms.ComboBox cmbEmpresa;
        private System.Windows.Forms.Label lblEmpresa;
        private System.Windows.Forms.TabPage tabFiscales;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.ComboBox cmbEstadoFiscal;
        private System.Windows.Forms.Label lblEstadoFiscal;
        private System.Windows.Forms.TextBox txtCodigoPostal;
        private System.Windows.Forms.Label lblCodigoPostal;
        private System.Windows.Forms.ComboBox cmbRegimenFiscal;
        private System.Windows.Forms.Label lblRegimenFiscal;
        private System.Windows.Forms.TextBox txtCURP;
        private System.Windows.Forms.Label lblCURP;
        private System.Windows.Forms.TextBox txtRFC;
        private System.Windows.Forms.Label lblRFC;
        private System.Windows.Forms.TabPage tabIMSS;
        private System.Windows.Forms.TextBox txtCLABE;
        private System.Windows.Forms.Label lblCLABE;
        private System.Windows.Forms.ComboBox cmbBanco;
        private System.Windows.Forms.Label lblBanco;
        private System.Windows.Forms.Label lblFactor;
        private System.Windows.Forms.Button btnCalcularSDI;
        private System.Windows.Forms.TextBox txtSBC;
        private System.Windows.Forms.Label lblSBC;
        private System.Windows.Forms.TextBox txtSDI;
        private System.Windows.Forms.Label lblSDI;
        private System.Windows.Forms.ComboBox cmbRiesgo;
        private System.Windows.Forms.Label lblRiesgo;
        private System.Windows.Forms.TextBox txtSalarioDiario;
        private System.Windows.Forms.Label lblSalarioDiario;
        private System.Windows.Forms.TextBox txtNSS;
        private System.Windows.Forms.Label lblNSS;
        private System.Windows.Forms.TabPage tabContrato;
        private System.Windows.Forms.ComboBox cmbPuesto;
        private System.Windows.Forms.Label lblPuesto;
        private System.Windows.Forms.ComboBox cmbDepartamento;
        private System.Windows.Forms.Label lblDepartamento;
        private System.Windows.Forms.DateTimePicker dtpFechaInicioContrato;
        private System.Windows.Forms.Label lblFechaInicioContrato;
        private System.Windows.Forms.ComboBox cmbZonaSMG;
        private System.Windows.Forms.ComboBox cmbDiasSemana;
        private System.Windows.Forms.Label lblDiasSemana;
        private System.Windows.Forms.Button btnDiasJornada;
        private System.Windows.Forms.Label lblZonaSMG;
        private System.Windows.Forms.ComboBox cmbTipoPeriodo;
        private System.Windows.Forms.Label lblTipoPeriodo;
        private System.Windows.Forms.ComboBox cmbTipoJornada;
        private System.Windows.Forms.Label lblTipoJornada;
        private System.Windows.Forms.ComboBox cmbTipoRegimen;
        private System.Windows.Forms.Label lblTipoRegimen;
        private System.Windows.Forms.ComboBox cmbTipoContrato;
        private System.Windows.Forms.Label lblTipoContrato;
        private System.Windows.Forms.Panel pnlBotones;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Panel pnlLista;
        private System.Windows.Forms.DataGridView dgvEmpleados;
        private System.Windows.Forms.Panel pnlBusqueda;
        private System.Windows.Forms.Label lblConteo;
        private System.Windows.Forms.ComboBox cmbFiltroEmpresa;
        private System.Windows.Forms.Label lblFiltroEmpresa;
        private System.Windows.Forms.CheckBox chkSoloActivos;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
    }
}
