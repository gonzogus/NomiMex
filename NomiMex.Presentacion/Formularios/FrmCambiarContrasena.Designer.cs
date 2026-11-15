namespace NomiMex.Presentacion.Formularios
{
    partial class FrmCambiarContrasena
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
            this.pnlModoRecuperacion = new System.Windows.Forms.Panel();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.lblInfoCodigo = new System.Windows.Forms.Label();
            this.btnEnviarCodigo = new System.Windows.Forms.Button();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.lblCorreo = new System.Windows.Forms.Label();
            this.pnlModoVoluntario = new System.Windows.Forms.Panel();
            this.txtPasswordActual = new System.Windows.Forms.TextBox();
            this.lblPasswordActual = new System.Windows.Forms.Label();
            this.lblNuevaPassword = new System.Windows.Forms.Label();
            this.txtNuevaPassword = new System.Windows.Forms.TextBox();
            this.lblConfirmarPassword = new System.Windows.Forms.Label();
            this.txtConfirmarPassword = new System.Windows.Forms.TextBox();
            this.chkMostrarPassword = new System.Windows.Forms.CheckBox();
            this.lblRequisitos = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlEncabezado.SuspendLayout();
            this.pnlModoRecuperacion.SuspendLayout();
            this.pnlModoVoluntario.SuspendLayout();
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
            this.pnlEncabezado.Size = new System.Drawing.Size(460, 74);
            this.pnlEncabezado.TabIndex = 0;
            //
            // lblSubtitulo
            //
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.lblSubtitulo.Location = new System.Drawing.Point(26, 44);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(241, 17);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Le enviaremos un código a su correo";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(23, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(181, 28);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Recuperar acceso";
            //
            // pnlModoRecuperacion
            //
            this.pnlModoRecuperacion.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.pnlModoRecuperacion.Controls.Add(this.txtCodigo);
            this.pnlModoRecuperacion.Controls.Add(this.lblCodigo);
            this.pnlModoRecuperacion.Controls.Add(this.lblInfoCodigo);
            this.pnlModoRecuperacion.Controls.Add(this.btnEnviarCodigo);
            this.pnlModoRecuperacion.Controls.Add(this.txtCorreo);
            this.pnlModoRecuperacion.Controls.Add(this.lblCorreo);
            this.pnlModoRecuperacion.Location = new System.Drawing.Point(20, 86);
            this.pnlModoRecuperacion.Name = "pnlModoRecuperacion";
            this.pnlModoRecuperacion.Size = new System.Drawing.Size(420, 150);
            this.pnlModoRecuperacion.TabIndex = 1;
            //
            // txtCodigo
            //
            this.txtCodigo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCodigo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtCodigo.Location = new System.Drawing.Point(0, 114);
            this.txtCodigo.MaxLength = 128;
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(420, 27);
            this.txtCodigo.TabIndex = 5;
            //
            // lblCodigo
            //
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCodigo.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblCodigo.Location = new System.Drawing.Point(-3, 93);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.Size = new System.Drawing.Size(144, 17);
            this.lblCodigo.TabIndex = 4;
            this.lblCodigo.Text = "Código de verificación";
            //
            // lblInfoCodigo
            //
            this.lblInfoCodigo.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblInfoCodigo.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblInfoCodigo.Location = new System.Drawing.Point(-3, 55);
            this.lblInfoCodigo.Name = "lblInfoCodigo";
            this.lblInfoCodigo.Size = new System.Drawing.Size(423, 32);
            this.lblInfoCodigo.TabIndex = 3;
            this.lblInfoCodigo.Text = "Se enviará un código de verificación al correo registrado.";
            //
            // btnEnviarCodigo
            //
            this.btnEnviarCodigo.BackColor = System.Drawing.Color.FromArgb(0, 135, 90);
            this.btnEnviarCodigo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnviarCodigo.FlatAppearance.BorderSize = 0;
            this.btnEnviarCodigo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 110, 74);
            this.btnEnviarCodigo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnviarCodigo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnEnviarCodigo.ForeColor = System.Drawing.Color.White;
            this.btnEnviarCodigo.Location = new System.Drawing.Point(290, 21);
            this.btnEnviarCodigo.Name = "btnEnviarCodigo";
            this.btnEnviarCodigo.Size = new System.Drawing.Size(130, 28);
            this.btnEnviarCodigo.TabIndex = 2;
            this.btnEnviarCodigo.Text = "Enviar código";
            this.btnEnviarCodigo.UseVisualStyleBackColor = false;
            this.btnEnviarCodigo.Click += new System.EventHandler(this.btnEnviarCodigo_Click);
            //
            // txtCorreo
            //
            this.txtCorreo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCorreo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtCorreo.Location = new System.Drawing.Point(0, 22);
            this.txtCorreo.MaxLength = 150;
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new System.Drawing.Size(280, 27);
            this.txtCorreo.TabIndex = 1;
            //
            // lblCorreo
            //
            this.lblCorreo.AutoSize = true;
            this.lblCorreo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCorreo.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblCorreo.Location = new System.Drawing.Point(-3, 1);
            this.lblCorreo.Name = "lblCorreo";
            this.lblCorreo.Size = new System.Drawing.Size(116, 17);
            this.lblCorreo.TabIndex = 0;
            this.lblCorreo.Text = "Correo electrónico";
            //
            // pnlModoVoluntario
            //
            this.pnlModoVoluntario.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.pnlModoVoluntario.Controls.Add(this.txtPasswordActual);
            this.pnlModoVoluntario.Controls.Add(this.lblPasswordActual);
            this.pnlModoVoluntario.Location = new System.Drawing.Point(20, 86);
            this.pnlModoVoluntario.Name = "pnlModoVoluntario";
            this.pnlModoVoluntario.Size = new System.Drawing.Size(420, 150);
            this.pnlModoVoluntario.TabIndex = 2;
            //
            // txtPasswordActual
            //
            this.txtPasswordActual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPasswordActual.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPasswordActual.Location = new System.Drawing.Point(0, 22);
            this.txtPasswordActual.MaxLength = 256;
            this.txtPasswordActual.Name = "txtPasswordActual";
            this.txtPasswordActual.Size = new System.Drawing.Size(420, 27);
            this.txtPasswordActual.TabIndex = 1;
            this.txtPasswordActual.UseSystemPasswordChar = true;
            //
            // lblPasswordActual
            //
            this.lblPasswordActual.AutoSize = true;
            this.lblPasswordActual.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblPasswordActual.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblPasswordActual.Location = new System.Drawing.Point(-3, 1);
            this.lblPasswordActual.Name = "lblPasswordActual";
            this.lblPasswordActual.Size = new System.Drawing.Size(114, 17);
            this.lblPasswordActual.TabIndex = 0;
            this.lblPasswordActual.Text = "Contraseña actual";
            //
            // lblNuevaPassword
            //
            this.lblNuevaPassword.AutoSize = true;
            this.lblNuevaPassword.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblNuevaPassword.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblNuevaPassword.Location = new System.Drawing.Point(17, 246);
            this.lblNuevaPassword.Name = "lblNuevaPassword";
            this.lblNuevaPassword.Size = new System.Drawing.Size(118, 17);
            this.lblNuevaPassword.TabIndex = 3;
            this.lblNuevaPassword.Text = "Nueva contraseña";
            //
            // txtNuevaPassword
            //
            this.txtNuevaPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNuevaPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtNuevaPassword.Location = new System.Drawing.Point(20, 268);
            this.txtNuevaPassword.MaxLength = 256;
            this.txtNuevaPassword.Name = "txtNuevaPassword";
            this.txtNuevaPassword.Size = new System.Drawing.Size(420, 27);
            this.txtNuevaPassword.TabIndex = 4;
            this.txtNuevaPassword.UseSystemPasswordChar = true;
            //
            // lblConfirmarPassword
            //
            this.lblConfirmarPassword.AutoSize = true;
            this.lblConfirmarPassword.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblConfirmarPassword.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lblConfirmarPassword.Location = new System.Drawing.Point(17, 302);
            this.lblConfirmarPassword.Name = "lblConfirmarPassword";
            this.lblConfirmarPassword.Size = new System.Drawing.Size(177, 17);
            this.lblConfirmarPassword.TabIndex = 5;
            this.lblConfirmarPassword.Text = "Confirmar nueva contraseña";
            //
            // txtConfirmarPassword
            //
            this.txtConfirmarPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConfirmarPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtConfirmarPassword.Location = new System.Drawing.Point(20, 324);
            this.txtConfirmarPassword.MaxLength = 256;
            this.txtConfirmarPassword.Name = "txtConfirmarPassword";
            this.txtConfirmarPassword.Size = new System.Drawing.Size(420, 27);
            this.txtConfirmarPassword.TabIndex = 6;
            this.txtConfirmarPassword.UseSystemPasswordChar = true;
            //
            // chkMostrarPassword
            //
            this.chkMostrarPassword.AutoSize = true;
            this.chkMostrarPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkMostrarPassword.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.chkMostrarPassword.Location = new System.Drawing.Point(20, 358);
            this.chkMostrarPassword.Name = "chkMostrarPassword";
            this.chkMostrarPassword.Size = new System.Drawing.Size(146, 19);
            this.chkMostrarPassword.TabIndex = 7;
            this.chkMostrarPassword.Text = "Mostrar contraseñas";
            this.chkMostrarPassword.UseVisualStyleBackColor = true;
            this.chkMostrarPassword.CheckedChanged += new System.EventHandler(this.chkMostrarPassword_CheckedChanged);
            //
            // lblRequisitos
            //
            this.lblRequisitos.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblRequisitos.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblRequisitos.Location = new System.Drawing.Point(17, 382);
            this.lblRequisitos.Name = "lblRequisitos";
            this.lblRequisitos.Size = new System.Drawing.Size(423, 30);
            this.lblRequisitos.TabIndex = 8;
            this.lblRequisitos.Text = "Mínimo 10 caracteres, con mayúsculas, minúsculas y números.";
            //
            // btnGuardar
            //
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(0, 135, 90);
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 110, 74);
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(20, 418);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(420, 44);
            this.btnGuardar.TabIndex = 9;
            this.btnGuardar.Text = "Restablecer contraseña";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnCancelar.Location = new System.Drawing.Point(20, 468);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(420, 34);
            this.btnCancelar.TabIndex = 10;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // FrmCambiarContrasena
            //
            this.AcceptButton = this.btnGuardar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(460, 520);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.lblRequisitos);
            this.Controls.Add(this.chkMostrarPassword);
            this.Controls.Add(this.txtConfirmarPassword);
            this.Controls.Add(this.lblConfirmarPassword);
            this.Controls.Add(this.txtNuevaPassword);
            this.Controls.Add(this.lblNuevaPassword);
            this.Controls.Add(this.pnlModoVoluntario);
            this.Controls.Add(this.pnlModoRecuperacion);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCambiarContrasena";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NomiMex - Contraseña";
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.pnlModoRecuperacion.ResumeLayout(false);
            this.pnlModoRecuperacion.PerformLayout();
            this.pnlModoVoluntario.ResumeLayout(false);
            this.pnlModoVoluntario.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlModoRecuperacion;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.Label lblInfoCodigo;
        private System.Windows.Forms.Button btnEnviarCodigo;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.Label lblCorreo;
        private System.Windows.Forms.Panel pnlModoVoluntario;
        private System.Windows.Forms.TextBox txtPasswordActual;
        private System.Windows.Forms.Label lblPasswordActual;
        private System.Windows.Forms.Label lblNuevaPassword;
        private System.Windows.Forms.TextBox txtNuevaPassword;
        private System.Windows.Forms.Label lblConfirmarPassword;
        private System.Windows.Forms.TextBox txtConfirmarPassword;
        private System.Windows.Forms.CheckBox chkMostrarPassword;
        private System.Windows.Forms.Label lblRequisitos;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
