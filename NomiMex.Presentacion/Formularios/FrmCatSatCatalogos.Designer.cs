namespace NomiMex.Presentacion.Formularios
{
    partial class FrmCatSatCatalogos
    {
        /// <summary>Variable del diseñador necesaria.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Limpiar los recursos que se estén usando.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlLista = new System.Windows.Forms.Panel();
            this.dgvCatalogosSat = new System.Windows.Forms.DataGridView();
            this.pnlBarra = new System.Windows.Forms.Panel();
            this.lblConteo = new System.Windows.Forms.Label();
            this.btnImportarExcel = new System.Windows.Forms.Button();
            this.cmbCatalogosSat = new System.Windows.Forms.ComboBox();
            this.lblCatalogo = new System.Windows.Forms.Label();
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.openFileDialogExcel = new System.Windows.Forms.OpenFileDialog();
            this.pnlLista.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCatalogosSat)).BeginInit();
            this.pnlBarra.SuspendLayout();
            this.pnlEncabezado.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlLista
            //
            this.pnlLista.Controls.Add(this.dgvCatalogosSat);
            this.pnlLista.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLista.Location = new System.Drawing.Point(0, 124);
            this.pnlLista.Name = "pnlLista";
            this.pnlLista.Padding = new System.Windows.Forms.Padding(18, 12, 18, 18);
            this.pnlLista.Size = new System.Drawing.Size(1050, 580);
            this.pnlLista.TabIndex = 2;
            //
            // dgvCatalogosSat
            //
            this.dgvCatalogosSat.AllowUserToAddRows = false;
            this.dgvCatalogosSat.AllowUserToDeleteRows = false;
            this.dgvCatalogosSat.AllowUserToOrderColumns = true;
            this.dgvCatalogosSat.AllowUserToResizeRows = false;
            this.dgvCatalogosSat.AutoGenerateColumns = true;
            this.dgvCatalogosSat.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCatalogosSat.BackgroundColor = System.Drawing.Color.White;
            this.dgvCatalogosSat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCatalogosSat.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCatalogosSat.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCatalogosSat.ColumnHeadersHeight = 34;
            this.dgvCatalogosSat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCatalogosSat.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCatalogosSat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCatalogosSat.EnableHeadersVisualStyles = false;
            this.dgvCatalogosSat.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dgvCatalogosSat.Location = new System.Drawing.Point(18, 12);
            this.dgvCatalogosSat.MultiSelect = false;
            this.dgvCatalogosSat.Name = "dgvCatalogosSat";
            this.dgvCatalogosSat.ReadOnly = true;
            this.dgvCatalogosSat.RowHeadersVisible = false;
            this.dgvCatalogosSat.RowTemplate.Height = 28;
            this.dgvCatalogosSat.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCatalogosSat.Size = new System.Drawing.Size(1014, 550);
            this.dgvCatalogosSat.TabIndex = 0;
            this.dgvCatalogosSat.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvCatalogosSat_CellFormatting);
            //
            // pnlBarra
            //
            this.pnlBarra.BackColor = System.Drawing.Color.White;
            this.pnlBarra.Controls.Add(this.lblConteo);
            this.pnlBarra.Controls.Add(this.btnImportarExcel);
            this.pnlBarra.Controls.Add(this.cmbCatalogosSat);
            this.pnlBarra.Controls.Add(this.lblCatalogo);
            this.pnlBarra.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBarra.Location = new System.Drawing.Point(0, 64);
            this.pnlBarra.Name = "pnlBarra";
            this.pnlBarra.Size = new System.Drawing.Size(1050, 60);
            this.pnlBarra.TabIndex = 1;
            //
            // lblConteo
            //
            this.lblConteo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConteo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblConteo.Location = new System.Drawing.Point(540, 20);
            this.lblConteo.Name = "lblConteo";
            this.lblConteo.Size = new System.Drawing.Size(240, 20);
            this.lblConteo.TabIndex = 2;
            this.lblConteo.Text = "0 registros";
            this.lblConteo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnImportarExcel
            //
            this.btnImportarExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportarExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnImportarExcel.FlatAppearance.BorderSize = 0;
            this.btnImportarExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImportarExcel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnImportarExcel.ForeColor = System.Drawing.Color.White;
            this.btnImportarExcel.Location = new System.Drawing.Point(790, 14);
            this.btnImportarExcel.Name = "btnImportarExcel";
            this.btnImportarExcel.Size = new System.Drawing.Size(242, 33);
            this.btnImportarExcel.TabIndex = 3;
            this.btnImportarExcel.Text = "Importar Actualización (Excel)";
            this.btnImportarExcel.UseVisualStyleBackColor = false;
            this.btnImportarExcel.Click += new System.EventHandler(this.btnImportarExcel_Click);
            //
            // cmbCatalogosSat
            //
            this.cmbCatalogosSat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCatalogosSat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCatalogosSat.FormattingEnabled = true;
            this.cmbCatalogosSat.Location = new System.Drawing.Point(150, 16);
            this.cmbCatalogosSat.Name = "cmbCatalogosSat";
            this.cmbCatalogosSat.Size = new System.Drawing.Size(360, 28);
            this.cmbCatalogosSat.TabIndex = 1;
            this.cmbCatalogosSat.SelectedIndexChanged += new System.EventHandler(this.cmbCatalogosSat_SelectedIndexChanged);
            //
            // lblCatalogo
            //
            this.lblCatalogo.AutoSize = true;
            this.lblCatalogo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCatalogo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblCatalogo.Location = new System.Drawing.Point(18, 21);
            this.lblCatalogo.Name = "lblCatalogo";
            this.lblCatalogo.Size = new System.Drawing.Size(120, 15);
            this.lblCatalogo.TabIndex = 0;
            this.lblCatalogo.Text = "Catálogo del SAT:";
            //
            // pnlEncabezado
            //
            this.pnlEncabezado.BackColor = System.Drawing.Color.White;
            this.pnlEncabezado.Controls.Add(this.lblSubtitulo);
            this.pnlEncabezado.Controls.Add(this.lblTitulo);
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Location = new System.Drawing.Point(0, 0);
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.pnlEncabezado.Padding = new System.Windows.Forms.Padding(18, 0, 18, 0);
            this.pnlEncabezado.Size = new System.Drawing.Size(1050, 64);
            this.pnlEncabezado.TabIndex = 0;
            //
            // lblSubtitulo
            //
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(20, 38);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(430, 15);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Consulte los catálogos oficiales y actualícelos importando los archivos del SAT.";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTitulo.Location = new System.Drawing.Point(18, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(168, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Catálogos del SAT";
            //
            // openFileDialogExcel
            //
            this.openFileDialogExcel.Filter = "Archivos de Excel del SAT (*.xlsx)|*.xlsx";
            this.openFileDialogExcel.Title = "Seleccione el archivo de Excel publicado por el SAT";
            //
            // FrmCatSatCatalogos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(1050, 704);
            this.Controls.Add(this.pnlLista);
            this.Controls.Add(this.pnlBarra);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmCatSatCatalogos";
            this.Text = "Catálogos del SAT";
            this.Load += new System.EventHandler(this.FrmCatSatCatalogos_Load);
            this.pnlLista.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCatalogosSat)).EndInit();
            this.pnlBarra.ResumeLayout(false);
            this.pnlBarra.PerformLayout();
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlLista;
        private System.Windows.Forms.DataGridView dgvCatalogosSat;
        private System.Windows.Forms.Panel pnlBarra;
        private System.Windows.Forms.Label lblCatalogo;
        private System.Windows.Forms.ComboBox cmbCatalogosSat;
        private System.Windows.Forms.Button btnImportarExcel;
        private System.Windows.Forms.Label lblConteo;
        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.OpenFileDialog openFileDialogExcel;
    }
}
