namespace NomiMex.Presentacion.Formularios
{
    partial class FrmDiasSemana
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblEncabezado;
        private System.Windows.Forms.Label lblDias;
        private System.Windows.Forms.ComboBox _cmbDias;
        private System.Windows.Forms.RadioButton _rdoPeriodo;
        private System.Windows.Forms.RadioButton _rdoFijo;
        private System.Windows.Forms.Label lblNota;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;

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
            this.lblEncabezado = new System.Windows.Forms.Label();
            this.lblDias = new System.Windows.Forms.Label();
            this._cmbDias = new System.Windows.Forms.ComboBox();
            this._rdoPeriodo = new System.Windows.Forms.RadioButton();
            this._rdoFijo = new System.Windows.Forms.RadioButton();
            this.lblNota = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblEncabezado
            //
            this.lblEncabezado.Location = new System.Drawing.Point(16, 14);
            this.lblEncabezado.Name = "lblEncabezado";
            this.lblEncabezado.Size = new System.Drawing.Size(438, 36);
            this.lblEncabezado.TabIndex = 0;
            //
            // lblDias
            //
            this.lblDias.AutoSize = true;
            this.lblDias.Location = new System.Drawing.Point(16, 62);
            this.lblDias.Name = "lblDias";
            this.lblDias.Size = new System.Drawing.Size(142, 15);
            this.lblDias.TabIndex = 1;
            this.lblDias.Text = "Días laborados por semana";
            //
            // _cmbDias
            //
            this._cmbDias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbDias.FormattingEnabled = true;
            this._cmbDias.Location = new System.Drawing.Point(16, 82);
            this._cmbDias.Name = "_cmbDias";
            this._cmbDias.Size = new System.Drawing.Size(438, 23);
            this._cmbDias.TabIndex = 2;
            //
            // _rdoPeriodo
            //
            this._rdoPeriodo.Location = new System.Drawing.Point(16, 116);
            this._rdoPeriodo.Name = "_rdoPeriodo";
            this._rdoPeriodo.Size = new System.Drawing.Size(438, 22);
            this._rdoPeriodo.TabIndex = 3;
            this._rdoPeriodo.TabStop = true;
            this._rdoPeriodo.UseVisualStyleBackColor = true;
            this._rdoPeriodo.Checked = true;
            //
            // _rdoFijo
            //
            this._rdoFijo.Location = new System.Drawing.Point(16, 142);
            this._rdoFijo.Name = "_rdoFijo";
            this._rdoFijo.Size = new System.Drawing.Size(438, 22);
            this._rdoFijo.TabIndex = 4;
            this._rdoFijo.UseVisualStyleBackColor = true;
            //
            // lblNota
            //
            this.lblNota.Location = new System.Drawing.Point(16, 170);
            this.lblNota.Name = "lblNota";
            this.lblNota.Size = new System.Drawing.Size(438, 18);
            this.lblNota.TabIndex = 5;
            this.lblNota.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblNota.Text = "Fijo: \"Según contrato / jornada\" regresa a los días de su tipo de jornada.";
            //
            // btnAceptar
            //
            this.btnAceptar.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAceptar.Location = new System.Drawing.Point(228, 186);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(110, 32);
            this.btnAceptar.TabIndex = 6;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            //
            // btnCancelar
            //
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(344, 186);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(110, 32);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            //
            // FrmDiasSemana
            //
            this.AcceptButton = this.btnAceptar;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(470, 232);
            this.Controls.Add(this.lblEncabezado);
            this.Controls.Add(this.lblDias);
            this.Controls.Add(this._cmbDias);
            this.Controls.Add(this._rdoPeriodo);
            this.Controls.Add(this._rdoFijo);
            this.Controls.Add(this.lblNota);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.Name = "FrmDiasSemana";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Días laborados por semana";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
