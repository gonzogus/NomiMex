namespace NomiMex.Presentacion.Formularios
{
    partial class FrmJornadas
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView _grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colJornada;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colHoras;
        private System.Windows.Forms.DataGridViewComboBoxColumn _colDias;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCerrar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._grid = new System.Windows.Forms.DataGridView();
            this._colJornada = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colHoras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDias = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.SuspendLayout();
            //
            // _grid
            //
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AutoGenerateColumns = false;
            this._grid.BackgroundColor = System.Drawing.Color.White;
            this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._colJornada,
            this._colHoras,
            this._colDias});
            this._grid.Location = new System.Drawing.Point(12, 12);
            this._grid.Name = "_grid";
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(796, 336);
            this._grid.TabIndex = 0;
            this._grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grid_DataError);
            //
            // _colJornada
            //
            this._colJornada.DataPropertyName = "Texto";
            this._colJornada.HeaderText = "Tipo de jornada";
            this._colJornada.ReadOnly = true;
            this._colJornada.Width = 470;
            //
            // _colHoras
            //
            this._colHoras.DataPropertyName = "HorasMaximasLegal";
            this._colHoras.HeaderText = "Horas";
            this._colHoras.ReadOnly = true;
            this._colHoras.Width = 60;
            //
            // _colDias
            //
            this._colDias.DataPropertyName = "DiasSemana";
            this._colDias.DisplayMember = "Descripcion";
            this._colDias.HeaderText = "Días por semana";
            this._colDias.ReadOnly = false;
            this._colDias.ValueMember = "Id";
            this._colDias.Width = 230;
            //
            // btnGuardar
            //
            this.btnGuardar.Location = new System.Drawing.Point(582, 358);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(110, 32);
            this.btnGuardar.TabIndex = 1;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.Guardar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCerrar.Location = new System.Drawing.Point(698, 358);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(110, 32);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            //
            // FrmJornadas
            //
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(820, 400);
            this.Controls.Add(this._grid);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCerrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmJornadas";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Días por jornada";
            this.Load += new System.EventHandler(this.FrmJornadas_Load);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
