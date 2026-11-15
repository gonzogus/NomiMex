namespace NomiMex.Presentacion.Formularios
{
    partial class FrmSeleccionarConceptoSAT
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSeleccionarConceptoSAT));
            this.tabCaptura = new System.Windows.Forms.TabControl();
            this.tabCatalogo = new System.Windows.Forms.TabPage();
            this.lblInstruccion = new System.Windows.Forms.Label();
            this.lblTipoConcepto = new System.Windows.Forms.Label();
            this.cmbTipoConcepto = new System.Windows.Forms.ComboBox();
            this.lblBuscarConcepto = new System.Windows.Forms.Label();
            this.txtBuscarConcepto = new System.Windows.Forms.TextBox();
            this.dgvCatalogo = new System.Windows.Forms.DataGridView();
            this.colCatClave = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCatDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCatEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpRegla = new System.Windows.Forms.GroupBox();
            this.lblFormaCalculo = new System.Windows.Forms.Label();
            this.cmbFormaCalculo = new System.Windows.Forms.ComboBox();
            this.lblFactor = new System.Windows.Forms.Label();
            this.nudFactor = new System.Windows.Forms.NumericUpDown();
            this.chkGravadoConcepto = new System.Windows.Forms.CheckBox();
            this.lblReglaEstado = new System.Windows.Forms.Label();
            this.tabLegales = new System.Windows.Forms.TabPage();
            this.lblAyudaLegal = new System.Windows.Forms.Label();
            this.cmbTipoLegal = new System.Windows.Forms.ComboBox();
            this.nudCantidadLegal = new System.Windows.Forms.NumericUpDown();
            this.lblUnidadLegal = new System.Windows.Forms.Label();
            this.dtpFechaLegal = new System.Windows.Forms.DateTimePicker();
            this.dgvLegales = new System.Windows.Forms.DataGridView();
            this.colLegalConcepto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLegalCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLegalFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnQuitarLegal = new System.Windows.Forms.Button();
            this.pnlCaptura = new System.Windows.Forms.Panel();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.nudCantidadOValor = new System.Windows.Forms.NumericUpDown();
            this.lblImporteCalculado = new System.Windows.Forms.Label();
            this.lblFormula = new System.Windows.Forms.Label();
            this.chkPermanente = new System.Windows.Forms.CheckBox();
            this.lblObservaciones = new System.Windows.Forms.Label();
            this.txtObservaciones = new System.Windows.Forms.TextBox();
            this.grpTimbrado = new System.Windows.Forms.GroupBox();
            this.lblClaveSATTitulo = new System.Windows.Forms.Label();
            this.lblClaveSAT = new System.Windows.Forms.Label();
            this.lblTratamientoTitulo = new System.Windows.Forms.Label();
            this.lblTratamiento = new System.Windows.Forms.Label();
            this.lblImportesTitulo = new System.Windows.Forms.Label();
            this.lblImportesFiscales = new System.Windows.Forms.Label();
            this.pnlBotones = new System.Windows.Forms.Panel();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.lblTituloTipo = new System.Windows.Forms.Label();
            this.lblEmpleado = new System.Windows.Forms.Label();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.tabCaptura.SuspendLayout();
            this.tabCatalogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCatalogo)).BeginInit();
            this.grpRegla.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFactor)).BeginInit();
            this.tabLegales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadLegal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLegales)).BeginInit();
            this.pnlCaptura.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadOValor)).BeginInit();
            this.grpTimbrado.SuspendLayout();
            this.pnlBotones.SuspendLayout();
            this.pnlEncabezado.SuspendLayout();
            this.SuspendLayout();
            //
            // tabCaptura
            //
            this.tabCaptura.Controls.Add(this.tabCatalogo);
            this.tabCaptura.Controls.Add(this.tabLegales);
            this.tabCaptura.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCaptura.Location = new System.Drawing.Point(0, 70);
            this.tabCaptura.Name = "tabCaptura";
            this.tabCaptura.SelectedIndex = 0;
            this.tabCaptura.Size = new System.Drawing.Size(820, 374);
            this.tabCaptura.TabIndex = 0;
            this.tabCaptura.SelectedIndexChanged += new System.EventHandler(this.tabCaptura_SelectedIndexChanged);
            //
            // tabCatalogo
            //
            this.tabCatalogo.Controls.Add(this.lblInstruccion);
            this.tabCatalogo.Controls.Add(this.lblTipoConcepto);
            this.tabCatalogo.Controls.Add(this.cmbTipoConcepto);
            this.tabCatalogo.Controls.Add(this.lblBuscarConcepto);
            this.tabCatalogo.Controls.Add(this.txtBuscarConcepto);
            this.tabCatalogo.Controls.Add(this.dgvCatalogo);
            this.tabCatalogo.Controls.Add(this.grpRegla);
            this.tabCatalogo.Location = new System.Drawing.Point(4, 24);
            this.tabCatalogo.Name = "tabCatalogo";
            this.tabCatalogo.Padding = new System.Windows.Forms.Padding(3);
            this.tabCatalogo.Size = new System.Drawing.Size(812, 346);
            this.tabCatalogo.TabIndex = 0;
            this.tabCatalogo.Text = "Catálogo SAT";
            this.tabCatalogo.UseVisualStyleBackColor = true;
            //
            // lblInstruccion
            //
            this.lblInstruccion.Location = new System.Drawing.Point(12, 6);
            this.lblInstruccion.Name = "lblInstruccion";
            this.lblInstruccion.Size = new System.Drawing.Size(788, 32);
            this.lblInstruccion.TabIndex = 0;
            this.lblInstruccion.Text = "Elija el concepto del catálogo oficial del SAT. Si todavía no tiene regla de cálculo, defínala para habilitarlo.";
            //
            // lblTipoConcepto
            //
            this.lblTipoConcepto.Location = new System.Drawing.Point(12, 40);
            this.lblTipoConcepto.Name = "lblTipoConcepto";
            this.lblTipoConcepto.Size = new System.Drawing.Size(180, 18);
            this.lblTipoConcepto.TabIndex = 1;
            this.lblTipoConcepto.Text = "Tipo de concepto";
            //
            // cmbTipoConcepto
            //
            this.cmbTipoConcepto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoConcepto.FormattingEnabled = true;
            this.cmbTipoConcepto.Items.AddRange(new object[] {
            "Percepciones",
            "Deducciones"});
            this.cmbTipoConcepto.Location = new System.Drawing.Point(12, 60);
            this.cmbTipoConcepto.Name = "cmbTipoConcepto";
            this.cmbTipoConcepto.Size = new System.Drawing.Size(180, 23);
            this.cmbTipoConcepto.TabIndex = 2;
            this.cmbTipoConcepto.SelectedIndexChanged += new System.EventHandler(this.filtro_Changed);
            //
            // lblBuscarConcepto
            //
            this.lblBuscarConcepto.Location = new System.Drawing.Point(210, 40);
            this.lblBuscarConcepto.Name = "lblBuscarConcepto";
            this.lblBuscarConcepto.Size = new System.Drawing.Size(432, 18);
            this.lblBuscarConcepto.TabIndex = 3;
            this.lblBuscarConcepto.Text = "Buscar por clave o descripción";
            //
            // txtBuscarConcepto
            //
            this.txtBuscarConcepto.Location = new System.Drawing.Point(210, 60);
            this.txtBuscarConcepto.Name = "txtBuscarConcepto";
            this.txtBuscarConcepto.Size = new System.Drawing.Size(590, 23);
            this.txtBuscarConcepto.TabIndex = 4;
            this.txtBuscarConcepto.TextChanged += new System.EventHandler(this.filtro_Changed);
            //
            // dgvCatalogo
            //
            this.dgvCatalogo.AllowUserToAddRows = false;
            this.dgvCatalogo.AllowUserToDeleteRows = false;
            this.dgvCatalogo.AllowUserToResizeRows = false;
            this.dgvCatalogo.AutoGenerateColumns = false;
            this.dgvCatalogo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCatalogo.BackgroundColor = System.Drawing.Color.White;
            this.dgvCatalogo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCatClave,
            this.colCatDescripcion,
            this.colCatEstado});
            this.dgvCatalogo.Location = new System.Drawing.Point(12, 92);
            this.dgvCatalogo.MultiSelect = false;
            this.dgvCatalogo.Name = "dgvCatalogo";
            this.dgvCatalogo.ReadOnly = true;
            this.dgvCatalogo.RowHeadersVisible = false;
            this.dgvCatalogo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCatalogo.Size = new System.Drawing.Size(788, 138);
            this.dgvCatalogo.TabIndex = 5;
            this.dgvCatalogo.SelectionChanged += new System.EventHandler(this.dgvCatalogo_SelectionChanged);
            this.dgvCatalogo.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCatalogo_CellDoubleClick);
            //
            // colCatClave
            //
            this.colCatClave.DataPropertyName = "ClaveSAT";
            this.colCatClave.FillWeight = 14F;
            this.colCatClave.HeaderText = "Clave SAT";
            this.colCatClave.Name = "colCatClave";
            this.colCatClave.ReadOnly = true;
            //
            // colCatDescripcion
            //
            this.colCatDescripcion.DataPropertyName = "Descripcion";
            this.colCatDescripcion.FillWeight = 64F;
            this.colCatDescripcion.HeaderText = "Descripción";
            this.colCatDescripcion.Name = "colCatDescripcion";
            this.colCatDescripcion.ReadOnly = true;
            //
            // colCatEstado
            //
            this.colCatEstado.DataPropertyName = "EstadoTexto";
            this.colCatEstado.FillWeight = 22F;
            this.colCatEstado.HeaderText = "Estado";
            this.colCatEstado.Name = "colCatEstado";
            this.colCatEstado.ReadOnly = true;
            //
            // grpRegla
            //
            this.grpRegla.Controls.Add(this.lblFormaCalculo);
            this.grpRegla.Controls.Add(this.cmbFormaCalculo);
            this.grpRegla.Controls.Add(this.lblFactor);
            this.grpRegla.Controls.Add(this.nudFactor);
            this.grpRegla.Controls.Add(this.chkGravadoConcepto);
            this.grpRegla.Controls.Add(this.lblReglaEstado);
            this.grpRegla.Location = new System.Drawing.Point(12, 236);
            this.grpRegla.Name = "grpRegla";
            this.grpRegla.Size = new System.Drawing.Size(788, 104);
            this.grpRegla.TabIndex = 6;
            this.grpRegla.TabStop = false;
            this.grpRegla.Text = "Regla de cálculo del concepto";
            //
            // lblFormaCalculo
            //
            this.lblFormaCalculo.Location = new System.Drawing.Point(14, 20);
            this.lblFormaCalculo.Name = "lblFormaCalculo";
            this.lblFormaCalculo.Size = new System.Drawing.Size(300, 18);
            this.lblFormaCalculo.TabIndex = 0;
            this.lblFormaCalculo.Text = "Forma de cálculo";
            //
            // cmbFormaCalculo
            //
            this.cmbFormaCalculo.DisplayMember = "Value";
            this.cmbFormaCalculo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormaCalculo.FormattingEnabled = true;
            this.cmbFormaCalculo.Location = new System.Drawing.Point(14, 40);
            this.cmbFormaCalculo.Name = "cmbFormaCalculo";
            this.cmbFormaCalculo.Size = new System.Drawing.Size(300, 23);
            this.cmbFormaCalculo.TabIndex = 1;
            this.cmbFormaCalculo.ValueMember = "Key";
            this.cmbFormaCalculo.SelectedIndexChanged += new System.EventHandler(this.captura_ValueChanged);
            //
            // lblFactor
            //
            this.lblFactor.Location = new System.Drawing.Point(330, 20);
            this.lblFactor.Name = "lblFactor";
            this.lblFactor.Size = new System.Drawing.Size(130, 18);
            this.lblFactor.TabIndex = 2;
            this.lblFactor.Text = "Factor multiplicador";
            //
            // nudFactor
            //
            this.nudFactor.DecimalPlaces = 4;
            this.nudFactor.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.nudFactor.Location = new System.Drawing.Point(330, 40);
            this.nudFactor.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nudFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.nudFactor.Name = "nudFactor";
            this.nudFactor.Size = new System.Drawing.Size(120, 23);
            this.nudFactor.TabIndex = 3;
            this.nudFactor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFactor.ValueChanged += new System.EventHandler(this.captura_ValueChanged);
            //
            // chkGravadoConcepto
            //
            this.chkGravadoConcepto.Checked = true;
            this.chkGravadoConcepto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGravadoConcepto.Location = new System.Drawing.Point(470, 40);
            this.chkGravadoConcepto.Name = "chkGravadoConcepto";
            this.chkGravadoConcepto.Size = new System.Drawing.Size(146, 24);
            this.chkGravadoConcepto.TabIndex = 4;
            this.chkGravadoConcepto.Text = "Gravado (ISR)";
            this.chkGravadoConcepto.CheckedChanged += new System.EventHandler(this.captura_ValueChanged);
            //
            // lblReglaEstado
            //
            this.lblReglaEstado.Location = new System.Drawing.Point(14, 68);
            this.lblReglaEstado.Name = "lblReglaEstado";
            this.lblReglaEstado.Size = new System.Drawing.Size(760, 32);
            this.lblReglaEstado.TabIndex = 5;
            this.lblReglaEstado.Text = "Seleccione un concepto.";
            //
            // tabLegales
            //
            this.tabLegales.Controls.Add(this.lblAyudaLegal);
            this.tabLegales.Controls.Add(this.cmbTipoLegal);
            this.tabLegales.Controls.Add(this.nudCantidadLegal);
            this.tabLegales.Controls.Add(this.lblUnidadLegal);
            this.tabLegales.Controls.Add(this.dtpFechaLegal);
            this.tabLegales.Controls.Add(this.dgvLegales);
            this.tabLegales.Controls.Add(this.btnQuitarLegal);
            this.tabLegales.Location = new System.Drawing.Point(4, 24);
            this.tabLegales.Name = "tabLegales";
            this.tabLegales.Padding = new System.Windows.Forms.Padding(3);
            this.tabLegales.Size = new System.Drawing.Size(812, 346);
            this.tabLegales.TabIndex = 1;
            this.tabLegales.Text = "Incidencias legales";
            this.tabLegales.UseVisualStyleBackColor = true;
            //
            // lblAyudaLegal
            //
            this.lblAyudaLegal.Location = new System.Drawing.Point(12, 8);
            this.lblAyudaLegal.Name = "lblAyudaLegal";
            this.lblAyudaLegal.Size = new System.Drawing.Size(788, 30);
            this.lblAyudaLegal.TabIndex = 0;
            this.lblAyudaLegal.Text = "Conservan sus reglas legales.";
            //
            // cmbTipoLegal
            //
            this.cmbTipoLegal.DisplayMember = "Descripcion";
            this.cmbTipoLegal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoLegal.FormattingEnabled = true;
            this.cmbTipoLegal.Location = new System.Drawing.Point(12, 40);
            this.cmbTipoLegal.Name = "cmbTipoLegal";
            this.cmbTipoLegal.Size = new System.Drawing.Size(788, 23);
            this.cmbTipoLegal.TabIndex = 1;
            this.cmbTipoLegal.ValueMember = "TipoIncidenciaId";
            this.cmbTipoLegal.SelectedIndexChanged += new System.EventHandler(this.cmbTipoLegal_SelectedIndexChanged);
            //
            // nudCantidadLegal
            //
            this.nudCantidadLegal.DecimalPlaces = 4;
            this.nudCantidadLegal.Location = new System.Drawing.Point(12, 78);
            this.nudCantidadLegal.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.nudCantidadLegal.Name = "nudCantidadLegal";
            this.nudCantidadLegal.Size = new System.Drawing.Size(150, 23);
            this.nudCantidadLegal.TabIndex = 2;
            this.nudCantidadLegal.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            //
            // lblUnidadLegal
            //
            this.lblUnidadLegal.Location = new System.Drawing.Point(172, 80);
            this.lblUnidadLegal.Name = "lblUnidadLegal";
            this.lblUnidadLegal.Size = new System.Drawing.Size(150, 20);
            this.lblUnidadLegal.TabIndex = 3;
            //
            // dtpFechaLegal
            //
            this.dtpFechaLegal.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaLegal.Location = new System.Drawing.Point(340, 78);
            this.dtpFechaLegal.Name = "dtpFechaLegal";
            this.dtpFechaLegal.Size = new System.Drawing.Size(240, 23);
            this.dtpFechaLegal.TabIndex = 4;
            //
            // dgvLegales
            //
            this.dgvLegales.AllowUserToAddRows = false;
            this.dgvLegales.AllowUserToDeleteRows = false;
            this.dgvLegales.AutoGenerateColumns = false;
            this.dgvLegales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLegales.BackgroundColor = System.Drawing.Color.White;
            this.dgvLegales.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLegalConcepto,
            this.colLegalCantidad,
            this.colLegalFecha});
            this.dgvLegales.Location = new System.Drawing.Point(12, 114);
            this.dgvLegales.MultiSelect = false;
            this.dgvLegales.Name = "dgvLegales";
            this.dgvLegales.ReadOnly = true;
            this.dgvLegales.RowHeadersVisible = false;
            this.dgvLegales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLegales.Size = new System.Drawing.Size(788, 186);
            this.dgvLegales.TabIndex = 5;
            //
            // colLegalConcepto
            //
            this.colLegalConcepto.DataPropertyName = "TipoIncidencia";
            this.colLegalConcepto.HeaderText = "Concepto";
            this.colLegalConcepto.Name = "colLegalConcepto";
            this.colLegalConcepto.ReadOnly = true;
            //
            // colLegalCantidad
            //
            this.colLegalCantidad.DataPropertyName = "Cantidad";
            this.colLegalCantidad.HeaderText = "Cantidad";
            this.colLegalCantidad.Name = "colLegalCantidad";
            this.colLegalCantidad.ReadOnly = true;
            //
            // colLegalFecha
            //
            this.colLegalFecha.DataPropertyName = "FechaIncidencia";
            this.colLegalFecha.HeaderText = "Fecha";
            this.colLegalFecha.Name = "colLegalFecha";
            this.colLegalFecha.ReadOnly = true;
            //
            // btnQuitarLegal
            //
            this.btnQuitarLegal.Location = new System.Drawing.Point(540, 308);
            this.btnQuitarLegal.Name = "btnQuitarLegal";
            this.btnQuitarLegal.Size = new System.Drawing.Size(260, 28);
            this.btnQuitarLegal.TabIndex = 6;
            this.btnQuitarLegal.Text = "Quitar incidencia legal seleccionada";
            this.btnQuitarLegal.UseVisualStyleBackColor = true;
            this.btnQuitarLegal.Click += new System.EventHandler(this.btnQuitarLegal_Click);
            //
            // pnlCaptura
            //
            this.pnlCaptura.Controls.Add(this.lblCantidad);
            this.pnlCaptura.Controls.Add(this.nudCantidadOValor);
            this.pnlCaptura.Controls.Add(this.lblImporteCalculado);
            this.pnlCaptura.Controls.Add(this.lblFormula);
            this.pnlCaptura.Controls.Add(this.chkPermanente);
            this.pnlCaptura.Controls.Add(this.lblObservaciones);
            this.pnlCaptura.Controls.Add(this.txtObservaciones);
            this.pnlCaptura.Controls.Add(this.grpTimbrado);
            this.pnlCaptura.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCaptura.Location = new System.Drawing.Point(0, 444);
            this.pnlCaptura.Name = "pnlCaptura";
            this.pnlCaptura.Size = new System.Drawing.Size(820, 192);
            this.pnlCaptura.TabIndex = 1;
            //
            // lblCantidad
            //
            this.lblCantidad.Location = new System.Drawing.Point(16, 8);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Size = new System.Drawing.Size(190, 18);
            this.lblCantidad.TabIndex = 0;
            this.lblCantidad.Text = "Cantidad";
            //
            // nudCantidadOValor
            //
            this.nudCantidadOValor.DecimalPlaces = 4;
            this.nudCantidadOValor.Location = new System.Drawing.Point(16, 28);
            this.nudCantidadOValor.Maximum = new decimal(new int[] {
            -1486618625,
            232830643,
            0,
            262144});
            this.nudCantidadOValor.Name = "nudCantidadOValor";
            this.nudCantidadOValor.Size = new System.Drawing.Size(190, 23);
            this.nudCantidadOValor.TabIndex = 1;
            this.nudCantidadOValor.ThousandsSeparator = true;
            this.nudCantidadOValor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCantidadOValor.ValueChanged += new System.EventHandler(this.captura_ValueChanged);
            //
            // lblImporteCalculado
            //
            this.lblImporteCalculado.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblImporteCalculado.Location = new System.Drawing.Point(216, 8);
            this.lblImporteCalculado.Name = "lblImporteCalculado";
            this.lblImporteCalculado.Size = new System.Drawing.Size(200, 44);
            this.lblImporteCalculado.TabIndex = 2;
            this.lblImporteCalculado.Text = "Importe";
            this.lblImporteCalculado.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // lblFormula
            //
            this.lblFormula.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblFormula.Location = new System.Drawing.Point(16, 56);
            this.lblFormula.Name = "lblFormula";
            this.lblFormula.Size = new System.Drawing.Size(400, 34);
            this.lblFormula.TabIndex = 7;
            this.lblFormula.Text = "-";
            //
            // chkPermanente
            //
            this.chkPermanente.Location = new System.Drawing.Point(16, 92);
            this.chkPermanente.Name = "chkPermanente";
            this.chkPermanente.Size = new System.Drawing.Size(400, 36);
            this.chkPermanente.TabIndex = 3;
            this.chkPermanente.Text = "Dejar permanente para los siguientes periodos";
            this.chkPermanente.UseVisualStyleBackColor = true;
            //
            // lblObservaciones
            //
            this.lblObservaciones.Location = new System.Drawing.Point(16, 130);
            this.lblObservaciones.Name = "lblObservaciones";
            this.lblObservaciones.Size = new System.Drawing.Size(400, 16);
            this.lblObservaciones.TabIndex = 4;
            this.lblObservaciones.Text = "Observaciones";
            //
            // txtObservaciones
            //
            this.txtObservaciones.Location = new System.Drawing.Point(16, 148);
            this.txtObservaciones.MaxLength = 250;
            this.txtObservaciones.Multiline = true;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.Size = new System.Drawing.Size(400, 44);
            this.txtObservaciones.TabIndex = 5;
            //
            // grpTimbrado
            //
            this.grpTimbrado.Controls.Add(this.lblClaveSATTitulo);
            this.grpTimbrado.Controls.Add(this.lblClaveSAT);
            this.grpTimbrado.Controls.Add(this.lblTratamientoTitulo);
            this.grpTimbrado.Controls.Add(this.lblTratamiento);
            this.grpTimbrado.Controls.Add(this.lblImportesTitulo);
            this.grpTimbrado.Controls.Add(this.lblImportesFiscales);
            this.grpTimbrado.Location = new System.Drawing.Point(436, 8);
            this.grpTimbrado.Name = "grpTimbrado";
            this.grpTimbrado.Size = new System.Drawing.Size(372, 176);
            this.grpTimbrado.TabIndex = 6;
            this.grpTimbrado.TabStop = false;
            this.grpTimbrado.Text = "Datos para timbrado (CFDI Nómina 1.2)";
            //
            // lblClaveSATTitulo
            //
            this.lblClaveSATTitulo.Location = new System.Drawing.Point(12, 24);
            this.lblClaveSATTitulo.Name = "lblClaveSATTitulo";
            this.lblClaveSATTitulo.Size = new System.Drawing.Size(100, 18);
            this.lblClaveSATTitulo.TabIndex = 0;
            this.lblClaveSATTitulo.Text = "Clave SAT";
            //
            // lblClaveSAT
            //
            this.lblClaveSAT.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblClaveSAT.Location = new System.Drawing.Point(116, 24);
            this.lblClaveSAT.Name = "lblClaveSAT";
            this.lblClaveSAT.Size = new System.Drawing.Size(248, 34);
            this.lblClaveSAT.TabIndex = 1;
            this.lblClaveSAT.Text = "-";
            //
            // lblTratamientoTitulo
            //
            this.lblTratamientoTitulo.Location = new System.Drawing.Point(12, 62);
            this.lblTratamientoTitulo.Name = "lblTratamientoTitulo";
            this.lblTratamientoTitulo.Size = new System.Drawing.Size(100, 32);
            this.lblTratamientoTitulo.TabIndex = 2;
            this.lblTratamientoTitulo.Text = "Tratamiento fiscal";
            //
            // lblTratamiento
            //
            this.lblTratamiento.Location = new System.Drawing.Point(116, 62);
            this.lblTratamiento.Name = "lblTratamiento";
            this.lblTratamiento.Size = new System.Drawing.Size(248, 32);
            this.lblTratamiento.TabIndex = 3;
            this.lblTratamiento.Text = "-";
            //
            // lblImportesTitulo
            //
            this.lblImportesTitulo.Location = new System.Drawing.Point(12, 100);
            this.lblImportesTitulo.Name = "lblImportesTitulo";
            this.lblImportesTitulo.Size = new System.Drawing.Size(100, 18);
            this.lblImportesTitulo.TabIndex = 4;
            this.lblImportesTitulo.Text = "Importes";
            //
            // lblImportesFiscales
            //
            this.lblImportesFiscales.Location = new System.Drawing.Point(116, 100);
            this.lblImportesFiscales.Name = "lblImportesFiscales";
            this.lblImportesFiscales.Size = new System.Drawing.Size(248, 44);
            this.lblImportesFiscales.TabIndex = 5;
            this.lblImportesFiscales.Text = "-";
            //
            // pnlBotones
            //
            this.pnlBotones.Controls.Add(this.btnAceptar);
            this.pnlBotones.Controls.Add(this.btnCancelar);
            this.pnlBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBotones.Location = new System.Drawing.Point(0, 636);
            this.pnlBotones.Name = "pnlBotones";
            this.pnlBotones.Size = new System.Drawing.Size(820, 56);
            this.pnlBotones.TabIndex = 2;
            //
            // btnAceptar
            //
            this.btnAceptar.Location = new System.Drawing.Point(572, 10);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(116, 34);
            this.btnAceptar.TabIndex = 0;
            this.btnAceptar.Text = "Seleccionar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(696, 10);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(116, 34);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            //
            // pnlEncabezado
            //
            this.pnlEncabezado.BackColor = System.Drawing.Color.FromArgb(222, 235, 247);
            this.pnlEncabezado.Controls.Add(this.lblTituloTipo);
            this.pnlEncabezado.Controls.Add(this.lblEmpleado);
            this.pnlEncabezado.Controls.Add(this.lblPeriodo);
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Location = new System.Drawing.Point(0, 0);
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.pnlEncabezado.Size = new System.Drawing.Size(820, 70);
            this.pnlEncabezado.TabIndex = 3;
            this.pnlEncabezado.TabStop = false;
            //
            // lblTituloTipo
            //
            this.lblTituloTipo.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
            this.lblTituloTipo.Location = new System.Drawing.Point(16, 6);
            this.lblTituloTipo.Name = "lblTituloTipo";
            this.lblTituloTipo.Size = new System.Drawing.Size(788, 28);
            this.lblTituloTipo.TabIndex = 0;
            this.lblTituloTipo.Text = "Agregar percepción";
            //
            // lblEmpleado
            //
            this.lblEmpleado.Location = new System.Drawing.Point(16, 34);
            this.lblEmpleado.Name = "lblEmpleado";
            this.lblEmpleado.Size = new System.Drawing.Size(788, 18);
            this.lblEmpleado.TabIndex = 1;
            this.lblEmpleado.Text = "Trabajador";
            //
            // lblPeriodo
            //
            this.lblPeriodo.Location = new System.Drawing.Point(16, 50);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(788, 18);
            this.lblPeriodo.TabIndex = 2;
            this.lblPeriodo.Text = "Periodo";
            //
            // FrmSeleccionarConceptoSAT
            //
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(820, 692);
            this.Controls.Add(this.tabCaptura);
            this.Controls.Add(this.pnlCaptura);
            this.Controls.Add(this.pnlBotones);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSeleccionarConceptoSAT";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Catálogo SAT de percepciones y deducciones";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSeleccionarConceptoSAT_FormClosing);
            this.Load += new System.EventHandler(this.FrmSeleccionarConceptoSAT_Load);
            this.tabCaptura.ResumeLayout(false);
            this.tabCatalogo.ResumeLayout(false);
            this.tabCatalogo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCatalogo)).EndInit();
            this.grpRegla.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudFactor)).EndInit();
            this.tabLegales.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadLegal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLegales)).EndInit();
            this.pnlCaptura.ResumeLayout(false);
            this.pnlCaptura.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadOValor)).EndInit();
            this.grpTimbrado.ResumeLayout(false);
            this.pnlBotones.ResumeLayout(false);
            this.pnlEncabezado.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TabControl tabCaptura;
        private System.Windows.Forms.TabPage tabCatalogo;
        private System.Windows.Forms.TabPage tabLegales;
        private System.Windows.Forms.Label lblInstruccion;
        private System.Windows.Forms.Label lblTipoConcepto;
        private System.Windows.Forms.ComboBox cmbTipoConcepto;
        private System.Windows.Forms.Label lblBuscarConcepto;
        private System.Windows.Forms.TextBox txtBuscarConcepto;
        private System.Windows.Forms.DataGridView dgvCatalogo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCatClave;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCatDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCatEstado;
        private System.Windows.Forms.GroupBox grpRegla;
        private System.Windows.Forms.Label lblFormaCalculo;
        private System.Windows.Forms.ComboBox cmbFormaCalculo;
        private System.Windows.Forms.Label lblFactor;
        private System.Windows.Forms.NumericUpDown nudFactor;
        private System.Windows.Forms.CheckBox chkGravadoConcepto;
        private System.Windows.Forms.Label lblReglaEstado;
        private System.Windows.Forms.Label lblAyudaLegal;
        private System.Windows.Forms.ComboBox cmbTipoLegal;
        private System.Windows.Forms.NumericUpDown nudCantidadLegal;
        private System.Windows.Forms.Label lblUnidadLegal;
        private System.Windows.Forms.DateTimePicker dtpFechaLegal;
        private System.Windows.Forms.DataGridView dgvLegales;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLegalConcepto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLegalCantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLegalFecha;
        private System.Windows.Forms.Button btnQuitarLegal;
        private System.Windows.Forms.Panel pnlCaptura;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.NumericUpDown nudCantidadOValor;
        private System.Windows.Forms.Label lblImporteCalculado;
        private System.Windows.Forms.Label lblFormula;
        private System.Windows.Forms.CheckBox chkPermanente;
        private System.Windows.Forms.Label lblObservaciones;
        private System.Windows.Forms.TextBox txtObservaciones;
        private System.Windows.Forms.GroupBox grpTimbrado;
        private System.Windows.Forms.Label lblClaveSATTitulo;
        private System.Windows.Forms.Label lblClaveSAT;
        private System.Windows.Forms.Label lblTratamientoTitulo;
        private System.Windows.Forms.Label lblTratamiento;
        private System.Windows.Forms.Label lblImportesTitulo;
        private System.Windows.Forms.Label lblImportesFiscales;
        private System.Windows.Forms.Panel pnlBotones;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblTituloTipo;
        private System.Windows.Forms.Label lblEmpleado;
        private System.Windows.Forms.Label lblPeriodo;
    }
}
