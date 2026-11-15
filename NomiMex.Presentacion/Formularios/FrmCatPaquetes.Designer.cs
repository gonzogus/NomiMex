namespace NomiMex.Presentacion.Formularios
{
    partial class FrmCatPaquetes
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.grpPaquetes = new System.Windows.Forms.GroupBox();
            this.dgvPaquetes = new System.Windows.Forms.DataGridView();
            this.colPaqNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPaqPeriodicidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPaqDefault = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPaqTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPaqActivo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.grpDatosPaquete = new System.Windows.Forms.GroupBox();
            this.lblNombrePaquete = new System.Windows.Forms.Label();
            this.txtNombrePaquete = new System.Windows.Forms.TextBox();
            this.lblPeriodicidadPaquete = new System.Windows.Forms.Label();
            this.cmbPeriodicidadPaquete = new System.Windows.Forms.ComboBox();
            this.chkPredeterminado = new System.Windows.Forms.CheckBox();
            this.chkPaqueteActivo = new System.Windows.Forms.CheckBox();
            this.btnNuevoPaquete = new System.Windows.Forms.Button();
            this.btnGuardarPaquete = new System.Windows.Forms.Button();
            this.grpConceptos = new System.Windows.Forms.GroupBox();
            this.dgvConceptosPaquete = new System.Windows.Forms.DataGridView();
            this.colConConcepto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConAlcance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblNotaPaquete = new System.Windows.Forms.Label();
            this.lblCantidadPaquete = new System.Windows.Forms.Label();
            this.nudCantidadPaquete = new System.Windows.Forms.NumericUpDown();
            this.chkPermanentePaquete = new System.Windows.Forms.CheckBox();
            this.btnAgregarConcepto = new System.Windows.Forms.Button();
            this.btnActualizarConcepto = new System.Windows.Forms.Button();
            this.btnQuitarConcepto = new System.Windows.Forms.Button();
            this.pnlEncabezado.SuspendLayout();
            this.pnlContenido.SuspendLayout();
            this.grpPaquetes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).BeginInit();
            this.grpDatosPaquete.SuspendLayout();
            this.grpConceptos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConceptosPaquete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadPaquete)).BeginInit();
            this.SuspendLayout();
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
            this.pnlEncabezado.TabIndex = 0;
            //
            // lblSubtitulo
            //
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblSubtitulo.Location = new System.Drawing.Point(20, 38);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Conceptos del catálogo SAT que se asignan al trabajador en su alta según su periodicidad. Los cambios aplican a las altas nuevas.";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblTitulo.Location = new System.Drawing.Point(18, 6);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Paquetes de incidencias";
            //
            // pnlContenido
            //
            this.pnlContenido.Controls.Add(this.grpPaquetes);
            this.pnlContenido.Controls.Add(this.grpDatosPaquete);
            this.pnlContenido.Controls.Add(this.grpConceptos);
            this.pnlContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenido.Location = new System.Drawing.Point(0, 64);
            this.pnlContenido.Name = "pnlContenido";
            this.pnlContenido.Size = new System.Drawing.Size(1050, 640);
            this.pnlContenido.TabIndex = 1;
            //
            // grpPaquetes
            //
            this.grpPaquetes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.grpPaquetes.Controls.Add(this.dgvPaquetes);
            this.grpPaquetes.Location = new System.Drawing.Point(16, 12);
            this.grpPaquetes.Name = "grpPaquetes";
            this.grpPaquetes.Size = new System.Drawing.Size(420, 330);
            this.grpPaquetes.TabIndex = 0;
            this.grpPaquetes.TabStop = false;
            this.grpPaquetes.Text = "Paquetes";
            //
            // dgvPaquetes
            //
            this.dgvPaquetes.AllowUserToAddRows = false;
            this.dgvPaquetes.AllowUserToDeleteRows = false;
            this.dgvPaquetes.AllowUserToResizeRows = false;
            this.dgvPaquetes.AutoGenerateColumns = false;
            this.dgvPaquetes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPaquetes.BackgroundColor = System.Drawing.Color.White;
            this.dgvPaquetes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPaqNombre,
            this.colPaqPeriodicidad,
            this.colPaqDefault,
            this.colPaqTotal,
            this.colPaqActivo});
            this.dgvPaquetes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPaquetes.MultiSelect = false;
            this.dgvPaquetes.Name = "dgvPaquetes";
            this.dgvPaquetes.ReadOnly = true;
            this.dgvPaquetes.RowHeadersVisible = false;
            this.dgvPaquetes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPaquetes.TabIndex = 0;
            this.dgvPaquetes.SelectionChanged += new System.EventHandler(this.dgvPaquetes_SelectionChanged);
            //
            // colPaqNombre
            //
            this.colPaqNombre.DataPropertyName = "Nombre";
            this.colPaqNombre.FillWeight = 38F;
            this.colPaqNombre.HeaderText = "Paquete";
            this.colPaqNombre.Name = "colPaqNombre";
            this.colPaqNombre.ReadOnly = true;
            //
            // colPaqPeriodicidad
            //
            this.colPaqPeriodicidad.DataPropertyName = "PeriodicidadDescripcion";
            this.colPaqPeriodicidad.FillWeight = 24F;
            this.colPaqPeriodicidad.HeaderText = "Periodicidad";
            this.colPaqPeriodicidad.Name = "colPaqPeriodicidad";
            this.colPaqPeriodicidad.ReadOnly = true;
            //
            // colPaqDefault
            //
            this.colPaqDefault.DataPropertyName = "EsDefaultTexto";
            this.colPaqDefault.FillWeight = 14F;
            this.colPaqDefault.HeaderText = "Predet.";
            this.colPaqDefault.Name = "colPaqDefault";
            this.colPaqDefault.ReadOnly = true;
            //
            // colPaqTotal
            //
            this.colPaqTotal.DataPropertyName = "TotalConceptos";
            this.colPaqTotal.FillWeight = 14F;
            this.colPaqTotal.HeaderText = "Conceptos";
            this.colPaqTotal.Name = "colPaqTotal";
            this.colPaqTotal.ReadOnly = true;
            //
            // colPaqActivo
            //
            this.colPaqActivo.DataPropertyName = "Activo";
            this.colPaqActivo.FillWeight = 10F;
            this.colPaqActivo.HeaderText = "Activo";
            this.colPaqActivo.Name = "colPaqActivo";
            this.colPaqActivo.ReadOnly = true;
            //
            // grpDatosPaquete
            //
            this.grpDatosPaquete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpDatosPaquete.Controls.Add(this.lblNombrePaquete);
            this.grpDatosPaquete.Controls.Add(this.txtNombrePaquete);
            this.grpDatosPaquete.Controls.Add(this.lblPeriodicidadPaquete);
            this.grpDatosPaquete.Controls.Add(this.cmbPeriodicidadPaquete);
            this.grpDatosPaquete.Controls.Add(this.chkPredeterminado);
            this.grpDatosPaquete.Controls.Add(this.chkPaqueteActivo);
            this.grpDatosPaquete.Controls.Add(this.btnNuevoPaquete);
            this.grpDatosPaquete.Controls.Add(this.btnGuardarPaquete);
            this.grpDatosPaquete.Location = new System.Drawing.Point(16, 352);
            this.grpDatosPaquete.Name = "grpDatosPaquete";
            this.grpDatosPaquete.Size = new System.Drawing.Size(420, 250);
            this.grpDatosPaquete.TabIndex = 1;
            this.grpDatosPaquete.TabStop = false;
            this.grpDatosPaquete.Text = "Datos del paquete";
            //
            // lblNombrePaquete
            //
            this.lblNombrePaquete.Location = new System.Drawing.Point(14, 26);
            this.lblNombrePaquete.Name = "lblNombrePaquete";
            this.lblNombrePaquete.Size = new System.Drawing.Size(392, 18);
            this.lblNombrePaquete.Text = "Nombre del paquete *";
            //
            // txtNombrePaquete
            //
            this.txtNombrePaquete.Location = new System.Drawing.Point(14, 46);
            this.txtNombrePaquete.MaxLength = 100;
            this.txtNombrePaquete.Name = "txtNombrePaquete";
            this.txtNombrePaquete.Size = new System.Drawing.Size(392, 23);
            this.txtNombrePaquete.TabIndex = 0;
            //
            // lblPeriodicidadPaquete
            //
            this.lblPeriodicidadPaquete.Location = new System.Drawing.Point(14, 80);
            this.lblPeriodicidadPaquete.Name = "lblPeriodicidadPaquete";
            this.lblPeriodicidadPaquete.Size = new System.Drawing.Size(392, 18);
            this.lblPeriodicidadPaquete.Text = "Periodicidad de pago a la que aplica";
            //
            // cmbPeriodicidadPaquete
            //
            this.cmbPeriodicidadPaquete.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPeriodicidadPaquete.FormattingEnabled = true;
            this.cmbPeriodicidadPaquete.Location = new System.Drawing.Point(14, 100);
            this.cmbPeriodicidadPaquete.Name = "cmbPeriodicidadPaquete";
            this.cmbPeriodicidadPaquete.Size = new System.Drawing.Size(392, 23);
            this.cmbPeriodicidadPaquete.TabIndex = 1;
            //
            // chkPredeterminado
            //
            this.chkPredeterminado.Location = new System.Drawing.Point(14, 136);
            this.chkPredeterminado.Name = "chkPredeterminado";
            this.chkPredeterminado.Size = new System.Drawing.Size(392, 24);
            this.chkPredeterminado.TabIndex = 2;
            this.chkPredeterminado.Text = "Paquete predeterminado: se asigna al dar de alta al trabajador";
            this.chkPredeterminado.UseVisualStyleBackColor = true;
            //
            // chkPaqueteActivo
            //
            this.chkPaqueteActivo.Checked = true;
            this.chkPaqueteActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPaqueteActivo.Location = new System.Drawing.Point(14, 164);
            this.chkPaqueteActivo.Name = "chkPaqueteActivo";
            this.chkPaqueteActivo.Size = new System.Drawing.Size(392, 24);
            this.chkPaqueteActivo.TabIndex = 3;
            this.chkPaqueteActivo.Text = "Activo";
            this.chkPaqueteActivo.UseVisualStyleBackColor = true;
            //
            // btnNuevoPaquete
            //
            this.btnNuevoPaquete.Location = new System.Drawing.Point(14, 202);
            this.btnNuevoPaquete.Name = "btnNuevoPaquete";
            this.btnNuevoPaquete.Size = new System.Drawing.Size(120, 34);
            this.btnNuevoPaquete.TabIndex = 4;
            this.btnNuevoPaquete.Text = "Nuevo";
            this.btnNuevoPaquete.UseVisualStyleBackColor = true;
            this.btnNuevoPaquete.Click += new System.EventHandler(this.btnNuevoPaquete_Click);
            //
            // btnGuardarPaquete
            //
            this.btnGuardarPaquete.Location = new System.Drawing.Point(146, 202);
            this.btnGuardarPaquete.Name = "btnGuardarPaquete";
            this.btnGuardarPaquete.Size = new System.Drawing.Size(120, 34);
            this.btnGuardarPaquete.TabIndex = 5;
            this.btnGuardarPaquete.Text = "Guardar";
            this.btnGuardarPaquete.UseVisualStyleBackColor = true;
            this.btnGuardarPaquete.Click += new System.EventHandler(this.btnGuardarPaquete_Click);
            //
            // grpConceptos
            //
            this.grpConceptos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConceptos.Controls.Add(this.dgvConceptosPaquete);
            this.grpConceptos.Controls.Add(this.lblNotaPaquete);
            this.grpConceptos.Controls.Add(this.lblCantidadPaquete);
            this.grpConceptos.Controls.Add(this.nudCantidadPaquete);
            this.grpConceptos.Controls.Add(this.chkPermanentePaquete);
            this.grpConceptos.Controls.Add(this.btnAgregarConcepto);
            this.grpConceptos.Controls.Add(this.btnActualizarConcepto);
            this.grpConceptos.Controls.Add(this.btnQuitarConcepto);
            this.grpConceptos.Location = new System.Drawing.Point(448, 12);
            this.grpConceptos.Name = "grpConceptos";
            this.grpConceptos.Size = new System.Drawing.Size(586, 590);
            this.grpConceptos.TabIndex = 2;
            this.grpConceptos.TabStop = false;
            this.grpConceptos.Text = "Conceptos del paquete seleccionado";
            //
            // dgvConceptosPaquete
            //
            this.dgvConceptosPaquete.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvConceptosPaquete.AllowUserToAddRows = false;
            this.dgvConceptosPaquete.AllowUserToDeleteRows = false;
            this.dgvConceptosPaquete.AllowUserToResizeRows = false;
            this.dgvConceptosPaquete.AutoGenerateColumns = false;
            this.dgvConceptosPaquete.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvConceptosPaquete.BackgroundColor = System.Drawing.Color.White;
            this.dgvConceptosPaquete.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colConConcepto,
            this.colConTipo,
            this.colConCantidad,
            this.colConAlcance});
            this.dgvConceptosPaquete.Location = new System.Drawing.Point(10, 22);
            this.dgvConceptosPaquete.MultiSelect = false;
            this.dgvConceptosPaquete.Name = "dgvConceptosPaquete";
            this.dgvConceptosPaquete.ReadOnly = true;
            this.dgvConceptosPaquete.RowHeadersVisible = false;
            this.dgvConceptosPaquete.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConceptosPaquete.Size = new System.Drawing.Size(566, 400);
            this.dgvConceptosPaquete.TabIndex = 0;
            this.dgvConceptosPaquete.SelectionChanged += new System.EventHandler(this.dgvConceptosPaquete_SelectionChanged);
            //
            // colConConcepto
            //
            this.colConConcepto.DataPropertyName = "ConceptoTexto";
            this.colConConcepto.FillWeight = 52F;
            this.colConConcepto.HeaderText = "Concepto SAT";
            this.colConConcepto.Name = "colConConcepto";
            this.colConConcepto.ReadOnly = true;
            //
            // colConTipo
            //
            this.colConTipo.DataPropertyName = "TipoTexto";
            this.colConTipo.FillWeight = 14F;
            this.colConTipo.HeaderText = "Tipo";
            this.colConTipo.Name = "colConTipo";
            this.colConTipo.ReadOnly = true;
            //
            // colConCantidad
            //
            this.colConCantidad.DataPropertyName = "CantidadTexto";
            this.colConCantidad.FillWeight = 14F;
            this.colConCantidad.HeaderText = "Cantidad o valor";
            this.colConCantidad.Name = "colConCantidad";
            this.colConCantidad.ReadOnly = true;
            //
            // colConAlcance
            //
            this.colConAlcance.DataPropertyName = "AlcanceTexto";
            this.colConAlcance.FillWeight = 20F;
            this.colConAlcance.HeaderText = "Alcance";
            this.colConAlcance.Name = "colConAlcance";
            this.colConAlcance.ReadOnly = true;
            //
            // lblNotaPaquete
            //
            this.lblNotaPaquete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNotaPaquete.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblNotaPaquete.Location = new System.Drawing.Point(10, 430);
            this.lblNotaPaquete.Name = "lblNotaPaquete";
            this.lblNotaPaquete.Size = new System.Drawing.Size(566, 36);
            this.lblNotaPaquete.Text = "Los conceptos base (sueldo, IMSS, ISR y subsidio) los calcula el sistema y no se pueden quitar. Quitar o agregar conceptos no modifica a los trabajadores ya dados de alta.";
            //
            // lblCantidadPaquete
            //
            this.lblCantidadPaquete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCantidadPaquete.Location = new System.Drawing.Point(10, 474);
            this.lblCantidadPaquete.Name = "lblCantidadPaquete";
            this.lblCantidadPaquete.Size = new System.Drawing.Size(150, 18);
            this.lblCantidadPaquete.Text = "Cantidad o valor";
            //
            // nudCantidadPaquete
            //
            this.nudCantidadPaquete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudCantidadPaquete.DecimalPlaces = 4;
            this.nudCantidadPaquete.Location = new System.Drawing.Point(10, 494);
            this.nudCantidadPaquete.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            this.nudCantidadPaquete.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            this.nudCantidadPaquete.Name = "nudCantidadPaquete";
            this.nudCantidadPaquete.Size = new System.Drawing.Size(150, 23);
            this.nudCantidadPaquete.TabIndex = 1;
            this.nudCantidadPaquete.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // chkPermanentePaquete
            //
            this.chkPermanentePaquete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkPermanentePaquete.Checked = true;
            this.chkPermanentePaquete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPermanentePaquete.Location = new System.Drawing.Point(176, 494);
            this.chkPermanentePaquete.Name = "chkPermanentePaquete";
            this.chkPermanentePaquete.Size = new System.Drawing.Size(390, 24);
            this.chkPermanentePaquete.TabIndex = 2;
            this.chkPermanentePaquete.Text = "Permanente (se precarga en todos los periodos)";
            this.chkPermanentePaquete.UseVisualStyleBackColor = true;
            //
            // btnAgregarConcepto
            //
            this.btnAgregarConcepto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAgregarConcepto.Location = new System.Drawing.Point(10, 540);
            this.btnAgregarConcepto.Name = "btnAgregarConcepto";
            this.btnAgregarConcepto.Size = new System.Drawing.Size(250, 34);
            this.btnAgregarConcepto.TabIndex = 3;
            this.btnAgregarConcepto.Text = "Agregar concepto del catálogo SAT...";
            this.btnAgregarConcepto.UseVisualStyleBackColor = true;
            this.btnAgregarConcepto.Click += new System.EventHandler(this.btnAgregarConcepto_Click);
            //
            // btnActualizarConcepto
            //
            this.btnActualizarConcepto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnActualizarConcepto.Location = new System.Drawing.Point(270, 540);
            this.btnActualizarConcepto.Name = "btnActualizarConcepto";
            this.btnActualizarConcepto.Size = new System.Drawing.Size(150, 34);
            this.btnActualizarConcepto.TabIndex = 4;
            this.btnActualizarConcepto.Text = "Actualizar cantidad";
            this.btnActualizarConcepto.UseVisualStyleBackColor = true;
            this.btnActualizarConcepto.Click += new System.EventHandler(this.btnActualizarConcepto_Click);
            //
            // btnQuitarConcepto
            //
            this.btnQuitarConcepto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnQuitarConcepto.Location = new System.Drawing.Point(430, 540);
            this.btnQuitarConcepto.Name = "btnQuitarConcepto";
            this.btnQuitarConcepto.Size = new System.Drawing.Size(146, 34);
            this.btnQuitarConcepto.TabIndex = 5;
            this.btnQuitarConcepto.Text = "Quitar del paquete";
            this.btnQuitarConcepto.UseVisualStyleBackColor = true;
            this.btnQuitarConcepto.Click += new System.EventHandler(this.btnQuitarConcepto_Click);
            //
            // FrmCatPaquetes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.ClientSize = new System.Drawing.Size(1050, 704);
            this.Controls.Add(this.pnlContenido);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmCatPaquetes";
            this.Text = "Paquetes de incidencias";
            this.Load += new System.EventHandler(this.FrmCatPaquetes_Load);
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.pnlContenido.ResumeLayout(false);
            this.grpPaquetes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).EndInit();
            this.grpDatosPaquete.ResumeLayout(false);
            this.grpDatosPaquete.PerformLayout();
            this.grpConceptos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConceptosPaquete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadPaquete)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlContenido;
        private System.Windows.Forms.GroupBox grpPaquetes;
        private System.Windows.Forms.DataGridView dgvPaquetes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPaqNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPaqPeriodicidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPaqDefault;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPaqTotal;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colPaqActivo;
        private System.Windows.Forms.GroupBox grpDatosPaquete;
        private System.Windows.Forms.Label lblNombrePaquete;
        private System.Windows.Forms.TextBox txtNombrePaquete;
        private System.Windows.Forms.Label lblPeriodicidadPaquete;
        private System.Windows.Forms.ComboBox cmbPeriodicidadPaquete;
        private System.Windows.Forms.CheckBox chkPredeterminado;
        private System.Windows.Forms.CheckBox chkPaqueteActivo;
        private System.Windows.Forms.Button btnNuevoPaquete;
        private System.Windows.Forms.Button btnGuardarPaquete;
        private System.Windows.Forms.GroupBox grpConceptos;
        private System.Windows.Forms.DataGridView dgvConceptosPaquete;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConConcepto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConCantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConAlcance;
        private System.Windows.Forms.Label lblNotaPaquete;
        private System.Windows.Forms.Label lblCantidadPaquete;
        private System.Windows.Forms.NumericUpDown nudCantidadPaquete;
        private System.Windows.Forms.CheckBox chkPermanentePaquete;
        private System.Windows.Forms.Button btnAgregarConcepto;
        private System.Windows.Forms.Button btnActualizarConcepto;
        private System.Windows.Forms.Button btnQuitarConcepto;
    }
}
