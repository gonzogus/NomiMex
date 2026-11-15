namespace NomiMex.Presentacion.Formularios
{
    partial class FrmConfigPeriodos
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabConfig = new System.Windows.Forms.TabControl();
            this.tabPeriodos = new System.Windows.Forms.TabPage();
            this.dgvPeriodos = new System.Windows.Forms.DataGridView();
            this.colNumero = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colInicio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPago = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescanso = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSeptimo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colInicioMes = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colFinMes = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colTotalFestivos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFestivos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlResultado = new System.Windows.Forms.Panel();
            this.lblResultado = new System.Windows.Forms.Label();
            this.grpConfiguracion = new System.Windows.Forms.GroupBox();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.btnGuardarConfig = new System.Windows.Forms.Button();
            this.lblNotaConfig = new System.Windows.Forms.Label();
            this.chkCalculaSeptimo = new System.Windows.Forms.CheckBox();
            this.cmbDiaDescanso = new System.Windows.Forms.ComboBox();
            this.lblDiaDescanso = new System.Windows.Forms.Label();
            this.cmbDiaPago = new System.Windows.Forms.ComboBox();
            this.lblDiaPago = new System.Windows.Forms.Label();
            this.cmbDiaFin = new System.Windows.Forms.ComboBox();
            this.lblDiaFin = new System.Windows.Forms.Label();
            this.cmbDiaInicio = new System.Windows.Forms.ComboBox();
            this.lblDiaInicio = new System.Windows.Forms.Label();
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.nudEjercicio = new System.Windows.Forms.NumericUpDown();
            this.lblEjercicio = new System.Windows.Forms.Label();
            this.cmbTipoPeriodo = new System.Windows.Forms.ComboBox();
            this.lblTipoPeriodo = new System.Windows.Forms.Label();
            this.cmbEmpresa = new System.Windows.Forms.ComboBox();
            this.lblEmpresa = new System.Windows.Forms.Label();
            this.tabFestivos = new System.Windows.Forms.TabPage();
            this.dgvFestivos = new System.Windows.Forms.DataGridView();
            this.colFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiaSemana = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescripcionFestivo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlcance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActivoFestivo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pnlNuevoFestivo = new System.Windows.Forms.Panel();
            this.grpNuevoFestivo = new System.Windows.Forms.GroupBox();
            this.lblNotaFestivos = new System.Windows.Forms.Label();
            this.btnCambiarEstadoFestivo = new System.Windows.Forms.Button();
            this.btnAgregarFestivo = new System.Windows.Forms.Button();
            this.cmbEmpresaNuevoFestivo = new System.Windows.Forms.ComboBox();
            this.lblEmpresaNuevoFestivo = new System.Windows.Forms.Label();
            this.cmbTipoFestivo = new System.Windows.Forms.ComboBox();
            this.lblTipoFestivo = new System.Windows.Forms.Label();
            this.txtDescripcionFestivo = new System.Windows.Forms.TextBox();
            this.lblDescripcionFestivo = new System.Windows.Forms.Label();
            this.dtpFechaFestivo = new System.Windows.Forms.DateTimePicker();
            this.lblFechaFestivo = new System.Windows.Forms.Label();
            this.pnlFiltrosFestivos = new System.Windows.Forms.Panel();
            this.btnGenerarLey = new System.Windows.Forms.Button();
            this.cmbEmpresaFiltroFestivo = new System.Windows.Forms.ComboBox();
            this.lblEmpresaFiltroFestivo = new System.Windows.Forms.Label();
            this.nudAnioFestivo = new System.Windows.Forms.NumericUpDown();
            this.lblAnioFestivo = new System.Windows.Forms.Label();
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.tabConfig.SuspendLayout();
            this.tabPeriodos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPeriodos)).BeginInit();
            this.pnlResultado.SuspendLayout();
            this.grpConfiguracion.SuspendLayout();
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEjercicio)).BeginInit();
            this.tabFestivos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFestivos)).BeginInit();
            this.pnlNuevoFestivo.SuspendLayout();
            this.grpNuevoFestivo.SuspendLayout();
            this.pnlFiltrosFestivos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAnioFestivo)).BeginInit();
            this.pnlEncabezado.SuspendLayout();
            this.SuspendLayout();
            //
            // tabConfig
            //
            this.tabConfig.Controls.Add(this.tabPeriodos);
            this.tabConfig.Controls.Add(this.tabFestivos);
            this.tabConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabConfig.Location = new System.Drawing.Point(0, 64);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.SelectedIndex = 0;
            this.tabConfig.Size = new System.Drawing.Size(1050, 640);
            this.tabConfig.TabIndex = 0;
            this.tabConfig.SelectedIndexChanged += new System.EventHandler(this.tabConfig_SelectedIndexChanged);
            //
            // tabPeriodos
            //
            this.tabPeriodos.BackColor = System.Drawing.Color.White;
            this.tabPeriodos.Controls.Add(this.dgvPeriodos);
            this.tabPeriodos.Controls.Add(this.pnlResultado);
            this.tabPeriodos.Controls.Add(this.grpConfiguracion);
            this.tabPeriodos.Controls.Add(this.pnlFiltros);
            this.tabPeriodos.Location = new System.Drawing.Point(4, 24);
            this.tabPeriodos.Name = "tabPeriodos";
            this.tabPeriodos.Padding = new System.Windows.Forms.Padding(10);
            this.tabPeriodos.Size = new System.Drawing.Size(1042, 612);
            this.tabPeriodos.TabIndex = 0;
            this.tabPeriodos.Text = "1. Periodos del ejercicio";
            //
            // dgvPeriodos
            //
            this.dgvPeriodos.AllowUserToAddRows = false;
            this.dgvPeriodos.AllowUserToDeleteRows = false;
            this.dgvPeriodos.AllowUserToResizeRows = false;
            this.dgvPeriodos.AutoGenerateColumns = false;
            this.dgvPeriodos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPeriodos.BackgroundColor = System.Drawing.Color.White;
            this.dgvPeriodos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPeriodos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPeriodos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPeriodos.ColumnHeadersHeight = 34;
            this.dgvPeriodos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPeriodos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNumero,
            this.colInicio,
            this.colFin,
            this.colPago,
            this.colDescanso,
            this.colSeptimo,
            this.colInicioMes,
            this.colFinMes,
            this.colTotalFestivos,
            this.colFestivos,
            this.colEstatus});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPeriodos.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPeriodos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPeriodos.EnableHeadersVisualStyles = false;
            this.dgvPeriodos.GridColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.dgvPeriodos.Location = new System.Drawing.Point(10, 230);
            this.dgvPeriodos.MultiSelect = false;
            this.dgvPeriodos.Name = "dgvPeriodos";
            this.dgvPeriodos.ReadOnly = true;
            this.dgvPeriodos.RowHeadersVisible = false;
            this.dgvPeriodos.RowTemplate.Height = 28;
            this.dgvPeriodos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPeriodos.Size = new System.Drawing.Size(1022, 332);
            this.dgvPeriodos.TabIndex = 3;
            //
            // colNumero
            //
            this.colNumero.DataPropertyName = "NumeroPeriodo";
            this.colNumero.FillWeight = 6F;
            this.colNumero.HeaderText = "No.";
            this.colNumero.Name = "colNumero";
            this.colNumero.ReadOnly = true;
            //
            // colInicio
            //
            this.colInicio.DataPropertyName = "FechaInicio";
            dataGridViewCellStyle3.Format = "dd/MM/yyyy";
            this.colInicio.DefaultCellStyle = dataGridViewCellStyle3;
            this.colInicio.FillWeight = 10F;
            this.colInicio.HeaderText = "Inicio";
            this.colInicio.Name = "colInicio";
            this.colInicio.ReadOnly = true;
            //
            // colFin
            //
            this.colFin.DataPropertyName = "FechaFin";
            dataGridViewCellStyle4.Format = "dd/MM/yyyy";
            this.colFin.DefaultCellStyle = dataGridViewCellStyle4;
            this.colFin.FillWeight = 10F;
            this.colFin.HeaderText = "Fin";
            this.colFin.Name = "colFin";
            this.colFin.ReadOnly = true;
            //
            // colPago
            //
            this.colPago.DataPropertyName = "FechaPago";
            dataGridViewCellStyle5.Format = "dd/MM/yyyy";
            this.colPago.DefaultCellStyle = dataGridViewCellStyle5;
            this.colPago.FillWeight = 10F;
            this.colPago.HeaderText = "Pago";
            this.colPago.Name = "colPago";
            this.colPago.ReadOnly = true;
            //
            // colDescanso
            //
            this.colDescanso.DataPropertyName = "DiaDescansoTexto";
            this.colDescanso.FillWeight = 9F;
            this.colDescanso.HeaderText = "Descanso";
            this.colDescanso.Name = "colDescanso";
            this.colDescanso.ReadOnly = true;
            //
            // colSeptimo
            //
            this.colSeptimo.DataPropertyName = "CalculaSeptimoDia";
            this.colSeptimo.FillWeight = 7F;
            this.colSeptimo.HeaderText = "7mo día";
            this.colSeptimo.Name = "colSeptimo";
            this.colSeptimo.ReadOnly = true;
            //
            // colInicioMes
            //
            this.colInicioMes.DataPropertyName = "EsInicioMes";
            this.colInicioMes.FillWeight = 8F;
            this.colInicioMes.HeaderText = "Inicio mes";
            this.colInicioMes.Name = "colInicioMes";
            this.colInicioMes.ReadOnly = true;
            //
            // colFinMes
            //
            this.colFinMes.DataPropertyName = "EsFinMes";
            this.colFinMes.FillWeight = 8F;
            this.colFinMes.HeaderText = "Fin de mes";
            this.colFinMes.Name = "colFinMes";
            this.colFinMes.ReadOnly = true;
            //
            // colTotalFestivos
            //
            this.colTotalFestivos.DataPropertyName = "TotalFestivos";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTotalFestivos.DefaultCellStyle = dataGridViewCellStyle6;
            this.colTotalFestivos.FillWeight = 7F;
            this.colTotalFestivos.HeaderText = "Festivos";
            this.colTotalFestivos.Name = "colTotalFestivos";
            this.colTotalFestivos.ReadOnly = true;
            //
            // colFestivos
            //
            this.colFestivos.DataPropertyName = "FestivosTexto";
            this.colFestivos.FillWeight = 28F;
            this.colFestivos.HeaderText = "Días festivos del periodo";
            this.colFestivos.Name = "colFestivos";
            this.colFestivos.ReadOnly = true;
            //
            // colEstatus
            //
            this.colEstatus.DataPropertyName = "Estatus";
            this.colEstatus.FillWeight = 10F;
            this.colEstatus.HeaderText = "Estatus";
            this.colEstatus.Name = "colEstatus";
            this.colEstatus.ReadOnly = true;
            //
            // pnlResultado
            //
            this.pnlResultado.Controls.Add(this.lblResultado);
            this.pnlResultado.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlResultado.Location = new System.Drawing.Point(10, 562);
            this.pnlResultado.Name = "pnlResultado";
            this.pnlResultado.Size = new System.Drawing.Size(1022, 40);
            this.pnlResultado.TabIndex = 2;
            //
            // lblResultado
            //
            this.lblResultado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblResultado.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblResultado.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblResultado.Location = new System.Drawing.Point(0, 0);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(1022, 40);
            this.lblResultado.TabIndex = 0;
            this.lblResultado.Text = "Seleccione empresa, periodicidad y ejercicio fiscal.";
            this.lblResultado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpConfiguracion
            //
            this.grpConfiguracion.Controls.Add(this.btnGenerar);
            this.grpConfiguracion.Controls.Add(this.btnGuardarConfig);
            this.grpConfiguracion.Controls.Add(this.lblNotaConfig);
            this.grpConfiguracion.Controls.Add(this.chkCalculaSeptimo);
            this.grpConfiguracion.Controls.Add(this.cmbDiaDescanso);
            this.grpConfiguracion.Controls.Add(this.lblDiaDescanso);
            this.grpConfiguracion.Controls.Add(this.cmbDiaPago);
            this.grpConfiguracion.Controls.Add(this.lblDiaPago);
            this.grpConfiguracion.Controls.Add(this.cmbDiaFin);
            this.grpConfiguracion.Controls.Add(this.lblDiaFin);
            this.grpConfiguracion.Controls.Add(this.cmbDiaInicio);
            this.grpConfiguracion.Controls.Add(this.lblDiaInicio);
            this.grpConfiguracion.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpConfiguracion.Location = new System.Drawing.Point(10, 80);
            this.grpConfiguracion.Name = "grpConfiguracion";
            this.grpConfiguracion.Size = new System.Drawing.Size(1022, 150);
            this.grpConfiguracion.TabIndex = 1;
            this.grpConfiguracion.TabStop = false;
            this.grpConfiguracion.Text = "Reglas de generación (se guardan por empresa y periodicidad)";
            //
            // btnGenerar
            //
            this.btnGenerar.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.FlatAppearance.BorderSize = 0;
            this.btnGenerar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerar.ForeColor = System.Drawing.Color.White;
            this.btnGenerar.Location = new System.Drawing.Point(790, 70);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(215, 44);
            this.btnGenerar.TabIndex = 11;
            this.btnGenerar.Text = "Generar periodos del ejercicio";
            this.btnGenerar.UseVisualStyleBackColor = false;
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // btnGuardarConfig
            //
            this.btnGuardarConfig.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnGuardarConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardarConfig.FlatAppearance.BorderSize = 0;
            this.btnGuardarConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardarConfig.ForeColor = System.Drawing.Color.White;
            this.btnGuardarConfig.Location = new System.Drawing.Point(790, 20);
            this.btnGuardarConfig.Name = "btnGuardarConfig";
            this.btnGuardarConfig.Size = new System.Drawing.Size(215, 40);
            this.btnGuardarConfig.TabIndex = 10;
            this.btnGuardarConfig.Text = "Guardar configuración";
            this.btnGuardarConfig.UseVisualStyleBackColor = false;
            this.btnGuardarConfig.Click += new System.EventHandler(this.btnGuardarConfig_Click);
            //
            // lblNotaConfig
            //
            this.lblNotaConfig.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblNotaConfig.Location = new System.Drawing.Point(15, 90);
            this.lblNotaConfig.Name = "lblNotaConfig";
            this.lblNotaConfig.Size = new System.Drawing.Size(760, 50);
            this.lblNotaConfig.TabIndex = 9;
            this.lblNotaConfig.Text = "Seleccione una empresa y una de sus periodicidades.";
            //
            // chkCalculaSeptimo
            //
            this.chkCalculaSeptimo.AutoSize = true;
            this.chkCalculaSeptimo.Location = new System.Drawing.Point(640, 50);
            this.chkCalculaSeptimo.Name = "chkCalculaSeptimo";
            this.chkCalculaSeptimo.Size = new System.Drawing.Size(137, 19);
            this.chkCalculaSeptimo.TabIndex = 8;
            this.chkCalculaSeptimo.Text = "¿Se calcula 7mo día?";
            this.chkCalculaSeptimo.UseVisualStyleBackColor = true;
            //
            // cmbDiaDescanso
            //
            this.cmbDiaDescanso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiaDescanso.FormattingEnabled = true;
            this.cmbDiaDescanso.Location = new System.Drawing.Point(480, 48);
            this.cmbDiaDescanso.Name = "cmbDiaDescanso";
            this.cmbDiaDescanso.Size = new System.Drawing.Size(140, 23);
            this.cmbDiaDescanso.TabIndex = 7;
            //
            // lblDiaDescanso
            //
            this.lblDiaDescanso.AutoSize = true;
            this.lblDiaDescanso.Location = new System.Drawing.Point(480, 28);
            this.lblDiaDescanso.Name = "lblDiaDescanso";
            this.lblDiaDescanso.Size = new System.Drawing.Size(92, 15);
            this.lblDiaDescanso.TabIndex = 6;
            this.lblDiaDescanso.Text = "Día de descanso";
            //
            // cmbDiaPago
            //
            this.cmbDiaPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiaPago.FormattingEnabled = true;
            this.cmbDiaPago.Location = new System.Drawing.Point(325, 48);
            this.cmbDiaPago.Name = "cmbDiaPago";
            this.cmbDiaPago.Size = new System.Drawing.Size(140, 23);
            this.cmbDiaPago.TabIndex = 5;
            //
            // lblDiaPago
            //
            this.lblDiaPago.AutoSize = true;
            this.lblDiaPago.Location = new System.Drawing.Point(325, 28);
            this.lblDiaPago.Name = "lblDiaPago";
            this.lblDiaPago.Size = new System.Drawing.Size(120, 15);
            this.lblDiaPago.TabIndex = 4;
            this.lblDiaPago.Text = "Día de pago (semanal)";
            //
            // cmbDiaFin
            //
            this.cmbDiaFin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiaFin.Enabled = false;
            this.cmbDiaFin.FormattingEnabled = true;
            this.cmbDiaFin.Location = new System.Drawing.Point(170, 48);
            this.cmbDiaFin.Name = "cmbDiaFin";
            this.cmbDiaFin.Size = new System.Drawing.Size(140, 23);
            this.cmbDiaFin.TabIndex = 3;
            //
            // lblDiaFin
            //
            this.lblDiaFin.AutoSize = true;
            this.lblDiaFin.Location = new System.Drawing.Point(170, 28);
            this.lblDiaFin.Name = "lblDiaFin";
            this.lblDiaFin.Size = new System.Drawing.Size(115, 15);
            this.lblDiaFin.TabIndex = 2;
            this.lblDiaFin.Text = "Día de fin (automático)";
            //
            // cmbDiaInicio
            //
            this.cmbDiaInicio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiaInicio.FormattingEnabled = true;
            this.cmbDiaInicio.Location = new System.Drawing.Point(15, 48);
            this.cmbDiaInicio.Name = "cmbDiaInicio";
            this.cmbDiaInicio.Size = new System.Drawing.Size(140, 23);
            this.cmbDiaInicio.TabIndex = 1;
            this.cmbDiaInicio.SelectedIndexChanged += new System.EventHandler(this.cmbDiaInicio_SelectedIndexChanged);
            //
            // lblDiaInicio
            //
            this.lblDiaInicio.AutoSize = true;
            this.lblDiaInicio.Location = new System.Drawing.Point(15, 28);
            this.lblDiaInicio.Name = "lblDiaInicio";
            this.lblDiaInicio.Size = new System.Drawing.Size(130, 15);
            this.lblDiaInicio.TabIndex = 0;
            this.lblDiaInicio.Text = "Día de inicio de semana";
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.nudEjercicio);
            this.pnlFiltros.Controls.Add(this.lblEjercicio);
            this.pnlFiltros.Controls.Add(this.cmbTipoPeriodo);
            this.pnlFiltros.Controls.Add(this.lblTipoPeriodo);
            this.pnlFiltros.Controls.Add(this.cmbEmpresa);
            this.pnlFiltros.Controls.Add(this.lblEmpresa);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(10, 10);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1022, 70);
            this.pnlFiltros.TabIndex = 0;
            //
            // nudEjercicio
            //
            this.nudEjercicio.Location = new System.Drawing.Point(560, 30);
            this.nudEjercicio.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            this.nudEjercicio.Minimum = new decimal(new int[] { 2000, 0, 0, 0 });
            this.nudEjercicio.Name = "nudEjercicio";
            this.nudEjercicio.Size = new System.Drawing.Size(110, 23);
            this.nudEjercicio.TabIndex = 5;
            this.nudEjercicio.Value = new decimal(new int[] { 2026, 0, 0, 0 });
            this.nudEjercicio.ValueChanged += new System.EventHandler(this.nudEjercicio_ValueChanged);
            //
            // lblEjercicio
            //
            this.lblEjercicio.AutoSize = true;
            this.lblEjercicio.Location = new System.Drawing.Point(560, 10);
            this.lblEjercicio.Name = "lblEjercicio";
            this.lblEjercicio.Size = new System.Drawing.Size(88, 15);
            this.lblEjercicio.TabIndex = 4;
            this.lblEjercicio.Text = "Ejercicio fiscal *";
            //
            // cmbTipoPeriodo
            //
            this.cmbTipoPeriodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoPeriodo.FormattingEnabled = true;
            this.cmbTipoPeriodo.Location = new System.Drawing.Point(340, 30);
            this.cmbTipoPeriodo.Name = "cmbTipoPeriodo";
            this.cmbTipoPeriodo.Size = new System.Drawing.Size(200, 23);
            this.cmbTipoPeriodo.TabIndex = 3;
            this.cmbTipoPeriodo.SelectedIndexChanged += new System.EventHandler(this.cmbTipoPeriodo_SelectedIndexChanged);
            //
            // lblTipoPeriodo
            //
            this.lblTipoPeriodo.AutoSize = true;
            this.lblTipoPeriodo.Location = new System.Drawing.Point(340, 10);
            this.lblTipoPeriodo.Name = "lblTipoPeriodo";
            this.lblTipoPeriodo.Size = new System.Drawing.Size(150, 15);
            this.lblTipoPeriodo.TabIndex = 2;
            this.lblTipoPeriodo.Text = "Periodicidad de la empresa *";
            //
            // cmbEmpresa
            //
            this.cmbEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmpresa.FormattingEnabled = true;
            this.cmbEmpresa.Location = new System.Drawing.Point(10, 30);
            this.cmbEmpresa.Name = "cmbEmpresa";
            this.cmbEmpresa.Size = new System.Drawing.Size(310, 23);
            this.cmbEmpresa.TabIndex = 1;
            this.cmbEmpresa.SelectedIndexChanged += new System.EventHandler(this.cmbEmpresa_SelectedIndexChanged);
            //
            // lblEmpresa
            //
            this.lblEmpresa.AutoSize = true;
            this.lblEmpresa.Location = new System.Drawing.Point(10, 10);
            this.lblEmpresa.Name = "lblEmpresa";
            this.lblEmpresa.Size = new System.Drawing.Size(58, 15);
            this.lblEmpresa.TabIndex = 0;
            this.lblEmpresa.Text = "Empresa *";
            //
            // tabFestivos
            //
            this.tabFestivos.BackColor = System.Drawing.Color.White;
            this.tabFestivos.Controls.Add(this.dgvFestivos);
            this.tabFestivos.Controls.Add(this.pnlNuevoFestivo);
            this.tabFestivos.Controls.Add(this.pnlFiltrosFestivos);
            this.tabFestivos.Location = new System.Drawing.Point(4, 24);
            this.tabFestivos.Name = "tabFestivos";
            this.tabFestivos.Padding = new System.Windows.Forms.Padding(10);
            this.tabFestivos.Size = new System.Drawing.Size(1042, 612);
            this.tabFestivos.TabIndex = 1;
            this.tabFestivos.Text = "2. Días festivos";
            //
            // dgvFestivos
            //
            this.dgvFestivos.AllowUserToAddRows = false;
            this.dgvFestivos.AllowUserToDeleteRows = false;
            this.dgvFestivos.AllowUserToResizeRows = false;
            this.dgvFestivos.AutoGenerateColumns = false;
            this.dgvFestivos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFestivos.BackgroundColor = System.Drawing.Color.White;
            this.dgvFestivos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFestivos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFestivos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvFestivos.ColumnHeadersHeight = 34;
            this.dgvFestivos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFestivos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFecha,
            this.colDiaSemana,
            this.colDescripcionFestivo,
            this.colAlcance,
            this.colActivoFestivo});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFestivos.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvFestivos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFestivos.EnableHeadersVisualStyles = false;
            this.dgvFestivos.GridColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.dgvFestivos.Location = new System.Drawing.Point(370, 80);
            this.dgvFestivos.MultiSelect = false;
            this.dgvFestivos.Name = "dgvFestivos";
            this.dgvFestivos.ReadOnly = true;
            this.dgvFestivos.RowHeadersVisible = false;
            this.dgvFestivos.RowTemplate.Height = 28;
            this.dgvFestivos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFestivos.Size = new System.Drawing.Size(662, 522);
            this.dgvFestivos.TabIndex = 2;
            this.dgvFestivos.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvFestivos_CellFormatting);
            //
            // colFecha
            //
            this.colFecha.DataPropertyName = "Fecha";
            dataGridViewCellStyle9.Format = "dd/MM/yyyy";
            this.colFecha.DefaultCellStyle = dataGridViewCellStyle9;
            this.colFecha.FillWeight = 16F;
            this.colFecha.HeaderText = "Fecha";
            this.colFecha.Name = "colFecha";
            this.colFecha.ReadOnly = true;
            //
            // colDiaSemana
            //
            this.colDiaSemana.DataPropertyName = "DiaSemana";
            this.colDiaSemana.FillWeight = 14F;
            this.colDiaSemana.HeaderText = "Día";
            this.colDiaSemana.Name = "colDiaSemana";
            this.colDiaSemana.ReadOnly = true;
            //
            // colDescripcionFestivo
            //
            this.colDescripcionFestivo.DataPropertyName = "Descripcion";
            this.colDescripcionFestivo.FillWeight = 38F;
            this.colDescripcionFestivo.HeaderText = "Descripción";
            this.colDescripcionFestivo.Name = "colDescripcionFestivo";
            this.colDescripcionFestivo.ReadOnly = true;
            //
            // colAlcance
            //
            this.colAlcance.DataPropertyName = "Alcance";
            this.colAlcance.FillWeight = 24F;
            this.colAlcance.HeaderText = "Aplica a";
            this.colAlcance.Name = "colAlcance";
            this.colAlcance.ReadOnly = true;
            //
            // colActivoFestivo
            //
            this.colActivoFestivo.DataPropertyName = "Activo";
            this.colActivoFestivo.FillWeight = 8F;
            this.colActivoFestivo.HeaderText = "Activo";
            this.colActivoFestivo.Name = "colActivoFestivo";
            this.colActivoFestivo.ReadOnly = true;
            //
            // pnlNuevoFestivo
            //
            this.pnlNuevoFestivo.Controls.Add(this.grpNuevoFestivo);
            this.pnlNuevoFestivo.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlNuevoFestivo.Location = new System.Drawing.Point(10, 80);
            this.pnlNuevoFestivo.Name = "pnlNuevoFestivo";
            this.pnlNuevoFestivo.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.pnlNuevoFestivo.Size = new System.Drawing.Size(360, 522);
            this.pnlNuevoFestivo.TabIndex = 1;
            //
            // grpNuevoFestivo
            //
            this.grpNuevoFestivo.Controls.Add(this.lblNotaFestivos);
            this.grpNuevoFestivo.Controls.Add(this.btnCambiarEstadoFestivo);
            this.grpNuevoFestivo.Controls.Add(this.btnAgregarFestivo);
            this.grpNuevoFestivo.Controls.Add(this.cmbEmpresaNuevoFestivo);
            this.grpNuevoFestivo.Controls.Add(this.lblEmpresaNuevoFestivo);
            this.grpNuevoFestivo.Controls.Add(this.cmbTipoFestivo);
            this.grpNuevoFestivo.Controls.Add(this.lblTipoFestivo);
            this.grpNuevoFestivo.Controls.Add(this.txtDescripcionFestivo);
            this.grpNuevoFestivo.Controls.Add(this.lblDescripcionFestivo);
            this.grpNuevoFestivo.Controls.Add(this.dtpFechaFestivo);
            this.grpNuevoFestivo.Controls.Add(this.lblFechaFestivo);
            this.grpNuevoFestivo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpNuevoFestivo.Location = new System.Drawing.Point(0, 0);
            this.grpNuevoFestivo.Name = "grpNuevoFestivo";
            this.grpNuevoFestivo.Size = new System.Drawing.Size(350, 522);
            this.grpNuevoFestivo.TabIndex = 0;
            this.grpNuevoFestivo.TabStop = false;
            this.grpNuevoFestivo.Text = "Agregar día festivo";
            //
            // lblNotaFestivos
            //
            this.lblNotaFestivos.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblNotaFestivos.Location = new System.Drawing.Point(15, 370);
            this.lblNotaFestivos.Name = "lblNotaFestivos";
            this.lblNotaFestivos.Size = new System.Drawing.Size(320, 90);
            this.lblNotaFestivos.TabIndex = 10;
            this.lblNotaFestivos.Text = "Los festivos activos se vinculan automáticamente al periodo en que caen (FechaFestivo entre inicio y fin). Los de ley aplican a todas las empresas; los de empresa solo a la seleccionada.";
            //
            // btnCambiarEstadoFestivo
            //
            this.btnCambiarEstadoFestivo.BackColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.btnCambiarEstadoFestivo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCambiarEstadoFestivo.FlatAppearance.BorderSize = 0;
            this.btnCambiarEstadoFestivo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCambiarEstadoFestivo.ForeColor = System.Drawing.Color.White;
            this.btnCambiarEstadoFestivo.Location = new System.Drawing.Point(15, 310);
            this.btnCambiarEstadoFestivo.Name = "btnCambiarEstadoFestivo";
            this.btnCambiarEstadoFestivo.Size = new System.Drawing.Size(320, 38);
            this.btnCambiarEstadoFestivo.TabIndex = 9;
            this.btnCambiarEstadoFestivo.Text = "Activar / desactivar el seleccionado";
            this.btnCambiarEstadoFestivo.UseVisualStyleBackColor = false;
            this.btnCambiarEstadoFestivo.Click += new System.EventHandler(this.btnCambiarEstadoFestivo_Click);
            //
            // btnAgregarFestivo
            //
            this.btnAgregarFestivo.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnAgregarFestivo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAgregarFestivo.FlatAppearance.BorderSize = 0;
            this.btnAgregarFestivo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregarFestivo.ForeColor = System.Drawing.Color.White;
            this.btnAgregarFestivo.Location = new System.Drawing.Point(15, 262);
            this.btnAgregarFestivo.Name = "btnAgregarFestivo";
            this.btnAgregarFestivo.Size = new System.Drawing.Size(320, 38);
            this.btnAgregarFestivo.TabIndex = 8;
            this.btnAgregarFestivo.Text = "Agregar día festivo";
            this.btnAgregarFestivo.UseVisualStyleBackColor = false;
            this.btnAgregarFestivo.Click += new System.EventHandler(this.btnAgregarFestivo_Click);
            //
            // cmbEmpresaNuevoFestivo
            //
            this.cmbEmpresaNuevoFestivo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmpresaNuevoFestivo.FormattingEnabled = true;
            this.cmbEmpresaNuevoFestivo.Location = new System.Drawing.Point(15, 223);
            this.cmbEmpresaNuevoFestivo.Name = "cmbEmpresaNuevoFestivo";
            this.cmbEmpresaNuevoFestivo.Size = new System.Drawing.Size(320, 23);
            this.cmbEmpresaNuevoFestivo.TabIndex = 7;
            //
            // lblEmpresaNuevoFestivo
            //
            this.lblEmpresaNuevoFestivo.AutoSize = true;
            this.lblEmpresaNuevoFestivo.Location = new System.Drawing.Point(15, 203);
            this.lblEmpresaNuevoFestivo.Name = "lblEmpresaNuevoFestivo";
            this.lblEmpresaNuevoFestivo.Size = new System.Drawing.Size(172, 15);
            this.lblEmpresaNuevoFestivo.TabIndex = 6;
            this.lblEmpresaNuevoFestivo.Text = "Empresa (solo festivos de empresa)";
            //
            // cmbTipoFestivo
            //
            this.cmbTipoFestivo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoFestivo.FormattingEnabled = true;
            this.cmbTipoFestivo.Location = new System.Drawing.Point(15, 168);
            this.cmbTipoFestivo.Name = "cmbTipoFestivo";
            this.cmbTipoFestivo.Size = new System.Drawing.Size(320, 23);
            this.cmbTipoFestivo.TabIndex = 5;
            this.cmbTipoFestivo.SelectedIndexChanged += new System.EventHandler(this.cmbTipoFestivo_SelectedIndexChanged);
            //
            // lblTipoFestivo
            //
            this.lblTipoFestivo.AutoSize = true;
            this.lblTipoFestivo.Location = new System.Drawing.Point(15, 148);
            this.lblTipoFestivo.Name = "lblTipoFestivo";
            this.lblTipoFestivo.Size = new System.Drawing.Size(34, 15);
            this.lblTipoFestivo.TabIndex = 4;
            this.lblTipoFestivo.Text = "Tipo *";
            //
            // txtDescripcionFestivo
            //
            this.txtDescripcionFestivo.Location = new System.Drawing.Point(15, 113);
            this.txtDescripcionFestivo.MaxLength = 100;
            this.txtDescripcionFestivo.Name = "txtDescripcionFestivo";
            this.txtDescripcionFestivo.Size = new System.Drawing.Size(320, 23);
            this.txtDescripcionFestivo.TabIndex = 3;
            //
            // lblDescripcionFestivo
            //
            this.lblDescripcionFestivo.AutoSize = true;
            this.lblDescripcionFestivo.Location = new System.Drawing.Point(15, 93);
            this.lblDescripcionFestivo.Name = "lblDescripcionFestivo";
            this.lblDescripcionFestivo.Size = new System.Drawing.Size(73, 15);
            this.lblDescripcionFestivo.TabIndex = 2;
            this.lblDescripcionFestivo.Text = "Descripción *";
            //
            // dtpFechaFestivo
            //
            this.dtpFechaFestivo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaFestivo.Location = new System.Drawing.Point(15, 58);
            this.dtpFechaFestivo.Name = "dtpFechaFestivo";
            this.dtpFechaFestivo.Size = new System.Drawing.Size(320, 23);
            this.dtpFechaFestivo.TabIndex = 1;
            //
            // lblFechaFestivo
            //
            this.lblFechaFestivo.AutoSize = true;
            this.lblFechaFestivo.Location = new System.Drawing.Point(15, 38);
            this.lblFechaFestivo.Name = "lblFechaFestivo";
            this.lblFechaFestivo.Size = new System.Drawing.Size(47, 15);
            this.lblFechaFestivo.TabIndex = 0;
            this.lblFechaFestivo.Text = "Fecha *";
            //
            // pnlFiltrosFestivos
            //
            this.pnlFiltrosFestivos.Controls.Add(this.btnGenerarLey);
            this.pnlFiltrosFestivos.Controls.Add(this.cmbEmpresaFiltroFestivo);
            this.pnlFiltrosFestivos.Controls.Add(this.lblEmpresaFiltroFestivo);
            this.pnlFiltrosFestivos.Controls.Add(this.nudAnioFestivo);
            this.pnlFiltrosFestivos.Controls.Add(this.lblAnioFestivo);
            this.pnlFiltrosFestivos.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltrosFestivos.Location = new System.Drawing.Point(10, 10);
            this.pnlFiltrosFestivos.Name = "pnlFiltrosFestivos";
            this.pnlFiltrosFestivos.Size = new System.Drawing.Size(1022, 70);
            this.pnlFiltrosFestivos.TabIndex = 0;
            //
            // btnGenerarLey
            //
            this.btnGenerarLey.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnGenerarLey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerarLey.FlatAppearance.BorderSize = 0;
            this.btnGenerarLey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarLey.ForeColor = System.Drawing.Color.White;
            this.btnGenerarLey.Location = new System.Drawing.Point(450, 22);
            this.btnGenerarLey.Name = "btnGenerarLey";
            this.btnGenerarLey.Size = new System.Drawing.Size(300, 36);
            this.btnGenerarLey.TabIndex = 4;
            this.btnGenerarLey.Text = "Generar festivos de ley del año (Art. 74 LFT)";
            this.btnGenerarLey.UseVisualStyleBackColor = false;
            this.btnGenerarLey.Click += new System.EventHandler(this.btnGenerarLey_Click);
            //
            // cmbEmpresaFiltroFestivo
            //
            this.cmbEmpresaFiltroFestivo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmpresaFiltroFestivo.FormattingEnabled = true;
            this.cmbEmpresaFiltroFestivo.Location = new System.Drawing.Point(130, 28);
            this.cmbEmpresaFiltroFestivo.Name = "cmbEmpresaFiltroFestivo";
            this.cmbEmpresaFiltroFestivo.Size = new System.Drawing.Size(300, 23);
            this.cmbEmpresaFiltroFestivo.TabIndex = 3;
            this.cmbEmpresaFiltroFestivo.SelectedIndexChanged += new System.EventHandler(this.cmbEmpresaFiltroFestivo_SelectedIndexChanged);
            //
            // lblEmpresaFiltroFestivo
            //
            this.lblEmpresaFiltroFestivo.AutoSize = true;
            this.lblEmpresaFiltroFestivo.Location = new System.Drawing.Point(130, 8);
            this.lblEmpresaFiltroFestivo.Name = "lblEmpresaFiltroFestivo";
            this.lblEmpresaFiltroFestivo.Size = new System.Drawing.Size(207, 15);
            this.lblEmpresaFiltroFestivo.TabIndex = 2;
            this.lblEmpresaFiltroFestivo.Text = "Ver festivos de ley y de la empresa";
            //
            // nudAnioFestivo
            //
            this.nudAnioFestivo.Location = new System.Drawing.Point(10, 28);
            this.nudAnioFestivo.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            this.nudAnioFestivo.Minimum = new decimal(new int[] { 2000, 0, 0, 0 });
            this.nudAnioFestivo.Name = "nudAnioFestivo";
            this.nudAnioFestivo.Size = new System.Drawing.Size(100, 23);
            this.nudAnioFestivo.TabIndex = 1;
            this.nudAnioFestivo.Value = new decimal(new int[] { 2026, 0, 0, 0 });
            this.nudAnioFestivo.ValueChanged += new System.EventHandler(this.nudAnioFestivo_ValueChanged);
            //
            // lblAnioFestivo
            //
            this.lblAnioFestivo.AutoSize = true;
            this.lblAnioFestivo.Location = new System.Drawing.Point(10, 8);
            this.lblAnioFestivo.Name = "lblAnioFestivo";
            this.lblAnioFestivo.Size = new System.Drawing.Size(88, 15);
            this.lblAnioFestivo.TabIndex = 0;
            this.lblAnioFestivo.Text = "Año fiscal *";
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
            this.pnlEncabezado.TabIndex = 1;
            //
            // lblSubtitulo
            //
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblSubtitulo.Location = new System.Drawing.Point(21, 38);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(560, 15);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Genera todos los periodos del ejercicio fiscal por empresa y periodicidad y los vincula con los días festivos.";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblTitulo.Location = new System.Drawing.Point(20, 8);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(224, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Periodos y días festivos";
            //
            // FrmConfigPeriodos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.ClientSize = new System.Drawing.Size(1050, 704);
            this.Controls.Add(this.tabConfig);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmConfigPeriodos";
            this.Text = "Periodos y días festivos";
            this.Load += new System.EventHandler(this.FrmConfigPeriodos_Load);
            this.tabConfig.ResumeLayout(false);
            this.tabPeriodos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPeriodos)).EndInit();
            this.pnlResultado.ResumeLayout(false);
            this.grpConfiguracion.ResumeLayout(false);
            this.grpConfiguracion.PerformLayout();
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEjercicio)).EndInit();
            this.tabFestivos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFestivos)).EndInit();
            this.pnlNuevoFestivo.ResumeLayout(false);
            this.grpNuevoFestivo.ResumeLayout(false);
            this.grpNuevoFestivo.PerformLayout();
            this.pnlFiltrosFestivos.ResumeLayout(false);
            this.pnlFiltrosFestivos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAnioFestivo)).EndInit();
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabConfig;
        private System.Windows.Forms.TabPage tabPeriodos;
        private System.Windows.Forms.DataGridView dgvPeriodos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumero;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInicio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFin;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPago;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescanso;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSeptimo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colInicioMes;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colFinMes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalFestivos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFestivos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstatus;
        private System.Windows.Forms.Panel pnlResultado;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.GroupBox grpConfiguracion;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Button btnGuardarConfig;
        private System.Windows.Forms.Label lblNotaConfig;
        private System.Windows.Forms.CheckBox chkCalculaSeptimo;
        private System.Windows.Forms.ComboBox cmbDiaDescanso;
        private System.Windows.Forms.Label lblDiaDescanso;
        private System.Windows.Forms.ComboBox cmbDiaPago;
        private System.Windows.Forms.Label lblDiaPago;
        private System.Windows.Forms.ComboBox cmbDiaFin;
        private System.Windows.Forms.Label lblDiaFin;
        private System.Windows.Forms.ComboBox cmbDiaInicio;
        private System.Windows.Forms.Label lblDiaInicio;
        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.NumericUpDown nudEjercicio;
        private System.Windows.Forms.Label lblEjercicio;
        private System.Windows.Forms.ComboBox cmbTipoPeriodo;
        private System.Windows.Forms.Label lblTipoPeriodo;
        private System.Windows.Forms.ComboBox cmbEmpresa;
        private System.Windows.Forms.Label lblEmpresa;
        private System.Windows.Forms.TabPage tabFestivos;
        private System.Windows.Forms.DataGridView dgvFestivos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiaSemana;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescripcionFestivo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlcance;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colActivoFestivo;
        private System.Windows.Forms.Panel pnlNuevoFestivo;
        private System.Windows.Forms.GroupBox grpNuevoFestivo;
        private System.Windows.Forms.Label lblNotaFestivos;
        private System.Windows.Forms.Button btnCambiarEstadoFestivo;
        private System.Windows.Forms.Button btnAgregarFestivo;
        private System.Windows.Forms.ComboBox cmbEmpresaNuevoFestivo;
        private System.Windows.Forms.Label lblEmpresaNuevoFestivo;
        private System.Windows.Forms.ComboBox cmbTipoFestivo;
        private System.Windows.Forms.Label lblTipoFestivo;
        private System.Windows.Forms.TextBox txtDescripcionFestivo;
        private System.Windows.Forms.Label lblDescripcionFestivo;
        private System.Windows.Forms.DateTimePicker dtpFechaFestivo;
        private System.Windows.Forms.Label lblFechaFestivo;
        private System.Windows.Forms.Panel pnlFiltrosFestivos;
        private System.Windows.Forms.Button btnGenerarLey;
        private System.Windows.Forms.ComboBox cmbEmpresaFiltroFestivo;
        private System.Windows.Forms.Label lblEmpresaFiltroFestivo;
        private System.Windows.Forms.NumericUpDown nudAnioFestivo;
        private System.Windows.Forms.Label lblAnioFestivo;
        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
    }
}
