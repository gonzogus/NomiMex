namespace NomiMex.Presentacion.Formularios
{
    partial class FrmAsignacionPaquetes
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.lblEmpresa = new System.Windows.Forms.Label();
            this.cmbEmpresa = new System.Windows.Forms.ComboBox();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.cmbPeriodos = new System.Windows.Forms.ComboBox();
            this.lblInfoPeriodo = new System.Windows.Forms.Label();
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.grpConceptos = new System.Windows.Forms.GroupBox();
            this.dgvConceptos = new System.Windows.Forms.DataGridView();
            this.colOmitir = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colConcepto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTipoConcepto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLegal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPaquetesOrigen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMotivo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblAvisoTimbrado = new System.Windows.Forms.Label();
            this.grpPaquetes = new System.Windows.Forms.GroupBox();
            this.dgvPaquetes = new System.Windows.Forms.DataGridView();
            this.colAsignado = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlcance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPeriodicidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConceptos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPredeterminado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActivo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pnlBotones = new System.Windows.Forms.Panel();
            this.lblEstado = new System.Windows.Forms.Label();
            this.btnRecargar = new System.Windows.Forms.Button();
            this.btnAplicarATodos = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.pnlEncabezado.SuspendLayout();
            this.pnlFiltros.SuspendLayout();
            this.pnlContenido.SuspendLayout();
            this.grpPaquetes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).BeginInit();
            this.grpConceptos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConceptos)).BeginInit();
            this.pnlBotones.SuspendLayout();
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
            this.lblSubtitulo.Size = new System.Drawing.Size(640, 15);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Cada empresa tiene su propia política de paquetes: se asignan por periodo y solo se ofrecen los que aplican a la empresa y a su periodicidad.";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblTitulo.Location = new System.Drawing.Point(18, 6);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(285, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Asignación de paquetes por periodo";
            //
            // pnlFiltros
            //
            this.pnlFiltros.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.pnlFiltros.Controls.Add(this.lblInfoPeriodo);
            this.pnlFiltros.Controls.Add(this.cmbPeriodos);
            this.pnlFiltros.Controls.Add(this.lblPeriodo);
            this.pnlFiltros.Controls.Add(this.cmbEmpresa);
            this.pnlFiltros.Controls.Add(this.lblEmpresa);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(0, 64);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(1050, 96);
            this.pnlFiltros.TabIndex = 1;
            //
            // lblEmpresa
            //
            this.lblEmpresa.AutoSize = true;
            this.lblEmpresa.Location = new System.Drawing.Point(20, 12);
            this.lblEmpresa.Name = "lblEmpresa";
            this.lblEmpresa.Size = new System.Drawing.Size(54, 15);
            this.lblEmpresa.TabIndex = 0;
            this.lblEmpresa.Text = "Empresa";
            //
            // cmbEmpresa
            //
            this.cmbEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmpresa.FormattingEnabled = true;
            this.cmbEmpresa.Location = new System.Drawing.Point(20, 32);
            this.cmbEmpresa.Name = "cmbEmpresa";
            this.cmbEmpresa.Size = new System.Drawing.Size(420, 23);
            this.cmbEmpresa.TabIndex = 1;
            this.cmbEmpresa.SelectedIndexChanged += new System.EventHandler(this.cmbEmpresa_SelectedIndexChanged);
            //
            // lblPeriodo
            //
            this.lblPeriodo.AutoSize = true;
            this.lblPeriodo.Location = new System.Drawing.Point(460, 12);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(46, 15);
            this.lblPeriodo.TabIndex = 2;
            this.lblPeriodo.Text = "Periodo";
            //
            // cmbPeriodos
            //
            this.cmbPeriodos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPeriodos.FormattingEnabled = true;
            this.cmbPeriodos.Location = new System.Drawing.Point(460, 32);
            this.cmbPeriodos.Name = "cmbPeriodos";
            this.cmbPeriodos.Size = new System.Drawing.Size(560, 23);
            this.cmbPeriodos.TabIndex = 3;
            this.cmbPeriodos.SelectedIndexChanged += new System.EventHandler(this.cmbPeriodos_SelectedIndexChanged);
            //
            // lblInfoPeriodo
            //
            this.lblInfoPeriodo.AutoSize = true;
            this.lblInfoPeriodo.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblInfoPeriodo.Location = new System.Drawing.Point(20, 66);
            this.lblInfoPeriodo.Name = "lblInfoPeriodo";
            this.lblInfoPeriodo.Size = new System.Drawing.Size(0, 15);
            this.lblInfoPeriodo.TabIndex = 4;
            //
            // pnlContenido
            //
            this.pnlContenido.Controls.Add(this.grpConceptos);
            this.pnlContenido.Controls.Add(this.grpPaquetes);
            this.pnlContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenido.Location = new System.Drawing.Point(0, 160);
            this.pnlContenido.Name = "pnlContenido";
            this.pnlContenido.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlContenido.Size = new System.Drawing.Size(1050, 482);
            this.pnlContenido.TabIndex = 2;
            //
            // grpConceptos
            //
            this.grpConceptos.Controls.Add(this.dgvConceptos);
            this.grpConceptos.Controls.Add(this.lblAvisoTimbrado);
            this.grpConceptos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConceptos.Location = new System.Drawing.Point(16, 218);
            this.grpConceptos.Name = "grpConceptos";
            this.grpConceptos.Size = new System.Drawing.Size(1018, 256);
            this.grpConceptos.TabIndex = 1;
            this.grpConceptos.TabStop = false;
            this.grpConceptos.Text = "Conceptos de los paquetes en el periodo (marque los que la empresa omite; los legales impiden timbrar)";
            //
            // dgvConceptos
            //
            this.dgvConceptos.AllowUserToAddRows = false;
            this.dgvConceptos.AllowUserToDeleteRows = false;
            this.dgvConceptos.AllowUserToResizeRows = false;
            this.dgvConceptos.AutoGenerateColumns = false;
            this.dgvConceptos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvConceptos.BackgroundColor = System.Drawing.Color.White;
            this.dgvConceptos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOmitir,
            this.colConcepto,
            this.colTipoConcepto,
            this.colLegal,
            this.colPaquetesOrigen,
            this.colMotivo});
            this.dgvConceptos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvConceptos.Location = new System.Drawing.Point(3, 19);
            this.dgvConceptos.MultiSelect = false;
            this.dgvConceptos.Name = "dgvConceptos";
            this.dgvConceptos.RowHeadersVisible = false;
            this.dgvConceptos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConceptos.Size = new System.Drawing.Size(1012, 210);
            this.dgvConceptos.TabIndex = 0;
            this.dgvConceptos.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvConceptos_CellFormatting);
            this.dgvConceptos.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvConceptos_CurrentCellDirtyStateChanged);
            //
            // colOmitir
            //
            this.colOmitir.DataPropertyName = "Omitido";
            this.colOmitir.FillWeight = 9F;
            this.colOmitir.HeaderText = "Omitir";
            this.colOmitir.Name = "colOmitir";
            //
            // colConcepto
            //
            this.colConcepto.DataPropertyName = "Concepto";
            this.colConcepto.FillWeight = 32F;
            this.colConcepto.HeaderText = "Concepto SAT";
            this.colConcepto.Name = "colConcepto";
            this.colConcepto.ReadOnly = true;
            //
            // colTipoConcepto
            //
            this.colTipoConcepto.DataPropertyName = "TipoTexto";
            this.colTipoConcepto.FillWeight = 11F;
            this.colTipoConcepto.HeaderText = "Tipo";
            this.colTipoConcepto.Name = "colTipoConcepto";
            this.colTipoConcepto.ReadOnly = true;
            //
            // colLegal
            //
            this.colLegal.DataPropertyName = "LegalTexto";
            this.colLegal.FillWeight = 8F;
            this.colLegal.HeaderText = "Legal";
            this.colLegal.Name = "colLegal";
            this.colLegal.ReadOnly = true;
            //
            // colPaquetesOrigen
            //
            this.colPaquetesOrigen.DataPropertyName = "Paquetes";
            this.colPaquetesOrigen.FillWeight = 20F;
            this.colPaquetesOrigen.HeaderText = "Paquete(s)";
            this.colPaquetesOrigen.Name = "colPaquetesOrigen";
            this.colPaquetesOrigen.ReadOnly = true;
            //
            // colMotivo
            //
            this.colMotivo.DataPropertyName = "Motivo";
            this.colMotivo.FillWeight = 36F;
            this.colMotivo.HeaderText = "Motivo (obligatorio si es legal)";
            this.colMotivo.Name = "colMotivo";
            //
            // lblAvisoTimbrado
            //
            this.lblAvisoTimbrado.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblAvisoTimbrado.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
            this.lblAvisoTimbrado.Location = new System.Drawing.Point(3, 229);
            this.lblAvisoTimbrado.Name = "lblAvisoTimbrado";
            this.lblAvisoTimbrado.Size = new System.Drawing.Size(1012, 24);
            this.lblAvisoTimbrado.TabIndex = 1;
            this.lblAvisoTimbrado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpPaquetes
            //
            this.grpPaquetes.Controls.Add(this.dgvPaquetes);
            this.grpPaquetes.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPaquetes.Location = new System.Drawing.Point(16, 8);
            this.grpPaquetes.Name = "grpPaquetes";
            this.grpPaquetes.Size = new System.Drawing.Size(1018, 210);
            this.grpPaquetes.TabIndex = 0;
            this.grpPaquetes.TabStop = false;
            this.grpPaquetes.Text = "Paquetes de la empresa en el periodo (marque los que se asignan)";
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
            this.colAsignado,
            this.colNombre,
            this.colAlcance,
            this.colPeriodicidad,
            this.colConceptos,
            this.colPredeterminado,
            this.colActivo});
            this.dgvPaquetes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPaquetes.Location = new System.Drawing.Point(3, 19);
            this.dgvPaquetes.MultiSelect = false;
            this.dgvPaquetes.Name = "dgvPaquetes";
            this.dgvPaquetes.RowHeadersVisible = false;
            this.dgvPaquetes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPaquetes.Size = new System.Drawing.Size(1012, 444);
            this.dgvPaquetes.TabIndex = 0;
            this.dgvPaquetes.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvPaquetes_CurrentCellDirtyStateChanged);
            //
            // colAsignado
            //
            this.colAsignado.DataPropertyName = "Asignado";
            this.colAsignado.FillWeight = 12F;
            this.colAsignado.HeaderText = "Asignado";
            this.colAsignado.Name = "colAsignado";
            //
            // colNombre
            //
            this.colNombre.DataPropertyName = "Nombre";
            this.colNombre.FillWeight = 38F;
            this.colNombre.HeaderText = "Paquete";
            this.colNombre.Name = "colNombre";
            this.colNombre.ReadOnly = true;
            //
            // colAlcance
            //
            this.colAlcance.DataPropertyName = "Alcance";
            this.colAlcance.FillWeight = 14F;
            this.colAlcance.HeaderText = "Alcance";
            this.colAlcance.Name = "colAlcance";
            this.colAlcance.ReadOnly = true;
            //
            // colPeriodicidad
            //
            this.colPeriodicidad.DataPropertyName = "Periodicidad";
            this.colPeriodicidad.FillWeight = 18F;
            this.colPeriodicidad.HeaderText = "Periodicidad";
            this.colPeriodicidad.Name = "colPeriodicidad";
            this.colPeriodicidad.ReadOnly = true;
            //
            // colConceptos
            //
            this.colConceptos.DataPropertyName = "TotalConceptos";
            this.colConceptos.FillWeight = 12F;
            this.colConceptos.HeaderText = "Conceptos";
            this.colConceptos.Name = "colConceptos";
            this.colConceptos.ReadOnly = true;
            //
            // colPredeterminado
            //
            this.colPredeterminado.DataPropertyName = "EsDefaultTexto";
            this.colPredeterminado.FillWeight = 14F;
            this.colPredeterminado.HeaderText = "Predeterminado";
            this.colPredeterminado.Name = "colPredeterminado";
            this.colPredeterminado.ReadOnly = true;
            //
            // colActivo
            //
            this.colActivo.DataPropertyName = "Activo";
            this.colActivo.FillWeight = 10F;
            this.colActivo.HeaderText = "Activo";
            this.colActivo.Name = "colActivo";
            this.colActivo.ReadOnly = true;
            //
            // pnlBotones
            //
            this.pnlBotones.BackColor = System.Drawing.Color.White;
            this.pnlBotones.Controls.Add(this.lblEstado);
            this.pnlBotones.Controls.Add(this.btnAplicarATodos);
            this.pnlBotones.Controls.Add(this.btnRecargar);
            this.pnlBotones.Controls.Add(this.btnGuardar);
            this.pnlBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBotones.Location = new System.Drawing.Point(0, 642);
            this.pnlBotones.Name = "pnlBotones";
            this.pnlBotones.Size = new System.Drawing.Size(1050, 62);
            this.pnlBotones.TabIndex = 3;
            //
            // lblEstado
            //
            this.lblEstado.AutoSize = true;
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblEstado.Location = new System.Drawing.Point(620, 23);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(0, 15);
            this.lblEstado.TabIndex = 2;
            //
            // btnAplicarATodos
            //
            this.btnAplicarATodos.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnAplicarATodos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAplicarATodos.FlatAppearance.BorderSize = 0;
            this.btnAplicarATodos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicarATodos.ForeColor = System.Drawing.Color.White;
            this.btnAplicarATodos.Location = new System.Drawing.Point(340, 11);
            this.btnAplicarATodos.Name = "btnAplicarATodos";
            this.btnAplicarATodos.Size = new System.Drawing.Size(260, 40);
            this.btnAplicarATodos.TabIndex = 2;
            this.btnAplicarATodos.Text = "Guardar y aplicar a todos los periodos abiertos";
            this.btnAplicarATodos.UseVisualStyleBackColor = false;
            this.btnAplicarATodos.Click += new System.EventHandler(this.btnAplicarATodos_Click);
            //
            // btnRecargar
            //
            this.btnRecargar.BackColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.btnRecargar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRecargar.FlatAppearance.BorderSize = 0;
            this.btnRecargar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecargar.ForeColor = System.Drawing.Color.White;
            this.btnRecargar.Location = new System.Drawing.Point(180, 11);
            this.btnRecargar.Name = "btnRecargar";
            this.btnRecargar.Size = new System.Drawing.Size(150, 40);
            this.btnRecargar.TabIndex = 1;
            this.btnRecargar.Text = "Recargar";
            this.btnRecargar.UseVisualStyleBackColor = false;
            this.btnRecargar.Click += new System.EventHandler(this.btnRecargar_Click);
            //
            // btnGuardar
            //
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(20, 11);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(150, 40);
            this.btnGuardar.TabIndex = 0;
            this.btnGuardar.Text = "Guardar paquetes";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // FrmAsignacionPaquetes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.ClientSize = new System.Drawing.Size(1050, 704);
            this.Controls.Add(this.pnlContenido);
            this.Controls.Add(this.pnlBotones);
            this.Controls.Add(this.pnlFiltros);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmAsignacionPaquetes";
            this.Text = "Asignación de paquetes por periodo";
            this.Load += new System.EventHandler(this.FrmAsignacionPaquetes_Load);
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.pnlContenido.ResumeLayout(false);
            this.grpPaquetes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).EndInit();
            this.grpConceptos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConceptos)).EndInit();
            this.pnlBotones.ResumeLayout(false);
            this.pnlBotones.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblEmpresa;
        private System.Windows.Forms.ComboBox cmbEmpresa;
        private System.Windows.Forms.Label lblPeriodo;
        private System.Windows.Forms.ComboBox cmbPeriodos;
        private System.Windows.Forms.Label lblInfoPeriodo;
        private System.Windows.Forms.Panel pnlContenido;
        private System.Windows.Forms.GroupBox grpPaquetes;
        private System.Windows.Forms.DataGridView dgvPaquetes;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colAsignado;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlcance;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPeriodicidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConceptos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPredeterminado;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colActivo;
        private System.Windows.Forms.GroupBox grpConceptos;
        private System.Windows.Forms.DataGridView dgvConceptos;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colOmitir;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConcepto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTipoConcepto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLegal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPaquetesOrigen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMotivo;
        private System.Windows.Forms.Label lblAvisoTimbrado;
        private System.Windows.Forms.Panel pnlBotones;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Button btnRecargar;
        private System.Windows.Forms.Button btnAplicarATodos;
        private System.Windows.Forms.Button btnGuardar;
    }
}
