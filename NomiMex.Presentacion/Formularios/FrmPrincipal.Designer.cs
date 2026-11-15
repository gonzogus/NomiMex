namespace NomiMex.Presentacion.Formularios
{
    partial class FrmPrincipal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPrincipal));
            this.pnlContenedor = new System.Windows.Forms.Panel();
            this.lblBienvenida = new System.Windows.Forms.Label();
            this.pnlSuperior = new System.Windows.Forms.Panel();
            this.lblRolSesion = new System.Windows.Forms.Label();
            this.lblUsuarioSesion = new System.Windows.Forms.Label();
            this.lblModulo = new System.Windows.Forms.Label();
            this.pnlLateral = new System.Windows.Forms.Panel();
            this.pnlNav = new System.Windows.Forms.Panel();
            this.btnRecibos = new System.Windows.Forms.Button();
            this.lblSeccionDocumentos = new System.Windows.Forms.Label();
            this.btnNomina = new System.Windows.Forms.Button();
            this.btnPeriodos = new System.Windows.Forms.Button();
            this.lblSeccionProcesos = new System.Windows.Forms.Label();
            this.btnCatalogosSat = new System.Windows.Forms.Button();
            this.btnConfiguracion = new System.Windows.Forms.Button();
            this.lblSeccionConfiguracion = new System.Windows.Forms.Label();
            this.btnUsuarios = new System.Windows.Forms.Button();
            this.lblSeccionAdministracion = new System.Windows.Forms.Label();
            this.btnAsignacionPaquetes = new System.Windows.Forms.Button();
            this.btnPaquetes = new System.Windows.Forms.Button();
            this.btnEstados = new System.Windows.Forms.Button();
            this.btnDepartamentos = new System.Windows.Forms.Button();
            this.btnPuestos = new System.Windows.Forms.Button();
            this.btnEmpleados = new System.Windows.Forms.Button();
            this.btnEmpresa = new System.Windows.Forms.Button();
            this.lblSeccionCatalogos = new System.Windows.Forms.Label();
            this.lblSloganEjecutivo = new System.Windows.Forms.Label();
            this.pnlLogo = new System.Windows.Forms.Panel();
            this.lblMarcaLatSub = new System.Windows.Forms.Label();
            this.lblMarcaLat = new System.Windows.Forms.Label();
            this.picLogoLateral = new System.Windows.Forms.PictureBox();
            this.pnlPie = new System.Windows.Forms.Panel();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.btnMiContrasena = new System.Windows.Forms.Button();
            this.pnlContenedor.SuspendLayout();
            this.pnlSuperior.SuspendLayout();
            this.pnlLateral.SuspendLayout();
            this.pnlNav.SuspendLayout();
            this.pnlLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogoLateral)).BeginInit();
            this.pnlPie.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlContenedor
            //
            this.pnlContenedor.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.pnlContenedor.Controls.Add(this.lblBienvenida);
            this.pnlContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenedor.Location = new System.Drawing.Point(260, 64);
            this.pnlContenedor.Name = "pnlContenedor";
            this.pnlContenedor.Size = new System.Drawing.Size(1020, 696);
            this.pnlContenedor.TabIndex = 0;
            //
            // lblBienvenida
            //
            this.lblBienvenida.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBienvenida.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.lblBienvenida.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblBienvenida.Location = new System.Drawing.Point(0, 0);
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.Size = new System.Drawing.Size(1020, 696);
            this.lblBienvenida.TabIndex = 0;
            this.lblBienvenida.Text = "Bienvenido(a)";
            this.lblBienvenida.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlSuperior
            //
            this.pnlSuperior.BackColor = System.Drawing.Color.White;
            this.pnlSuperior.Controls.Add(this.lblRolSesion);
            this.pnlSuperior.Controls.Add(this.lblUsuarioSesion);
            this.pnlSuperior.Controls.Add(this.lblModulo);
            this.pnlSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSuperior.Location = new System.Drawing.Point(260, 0);
            this.pnlSuperior.Name = "pnlSuperior";
            this.pnlSuperior.Padding = new System.Windows.Forms.Padding(24, 0, 24, 0);
            this.pnlSuperior.Size = new System.Drawing.Size(1020, 64);
            this.pnlSuperior.TabIndex = 1;
            //
            // lblRolSesion
            //
            this.lblRolSesion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRolSesion.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRolSesion.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblRolSesion.Location = new System.Drawing.Point(616, 35);
            this.lblRolSesion.Name = "lblRolSesion";
            this.lblRolSesion.Size = new System.Drawing.Size(380, 18);
            this.lblRolSesion.TabIndex = 2;
            this.lblRolSesion.Text = "Rol:";
            this.lblRolSesion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUsuarioSesion
            //
            this.lblUsuarioSesion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUsuarioSesion.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblUsuarioSesion.ForeColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.lblUsuarioSesion.Location = new System.Drawing.Point(616, 12);
            this.lblUsuarioSesion.Name = "lblUsuarioSesion";
            this.lblUsuarioSesion.Size = new System.Drawing.Size(380, 22);
            this.lblUsuarioSesion.TabIndex = 1;
            this.lblUsuarioSesion.Text = "Usuario";
            this.lblUsuarioSesion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblModulo
            //
            this.lblModulo.AutoSize = true;
            this.lblModulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblModulo.ForeColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.lblModulo.Location = new System.Drawing.Point(21, 16);
            this.lblModulo.Name = "lblModulo";
            this.lblModulo.Size = new System.Drawing.Size(60, 28);
            this.lblModulo.TabIndex = 0;
            this.lblModulo.Text = "Inicio";
            //
            // pnlLateral
            //
            this.pnlLateral.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.pnlLateral.Controls.Add(this.pnlNav);
            this.pnlLateral.Controls.Add(this.pnlPie);
            this.pnlLateral.Controls.Add(this.lblSloganEjecutivo);
            this.pnlLateral.Controls.Add(this.pnlLogo);
            this.pnlLateral.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLateral.Location = new System.Drawing.Point(0, 0);
            this.pnlLateral.Name = "pnlLateral";
            this.pnlLateral.Size = new System.Drawing.Size(260, 760);
            this.pnlLateral.TabIndex = 2;
            //
            // pnlNav
            //
            this.pnlNav.AutoScroll = true;
            this.pnlNav.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.pnlNav.Controls.Add(this.btnRecibos);
            this.pnlNav.Controls.Add(this.lblSeccionDocumentos);
            this.pnlNav.Controls.Add(this.btnNomina);
            this.pnlNav.Controls.Add(this.btnPeriodos);
            this.pnlNav.Controls.Add(this.lblSeccionProcesos);
            this.pnlNav.Controls.Add(this.btnConfiguracion);
            this.pnlNav.Controls.Add(this.btnCatalogosSat);
            this.pnlNav.Controls.Add(this.lblSeccionConfiguracion);
            this.pnlNav.Controls.Add(this.btnUsuarios);
            this.pnlNav.Controls.Add(this.lblSeccionAdministracion);
            this.pnlNav.Controls.Add(this.btnAsignacionPaquetes);
            this.pnlNav.Controls.Add(this.btnPaquetes);
            this.pnlNav.Controls.Add(this.btnEstados);
            this.pnlNav.Controls.Add(this.btnDepartamentos);
            this.pnlNav.Controls.Add(this.btnPuestos);
            this.pnlNav.Controls.Add(this.btnEmpleados);
            this.pnlNav.Controls.Add(this.btnEmpresa);
            this.pnlNav.Controls.Add(this.lblSeccionCatalogos);
            this.pnlNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNav.Location = new System.Drawing.Point(0, 160);
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(260, 536);
            this.pnlNav.TabIndex = 2;
            //
            // btnRecibos
            //
            this.btnRecibos.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnRecibos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRecibos.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRecibos.FlatAppearance.BorderSize = 0;
            this.btnRecibos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnRecibos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecibos.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnRecibos.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnRecibos.Location = new System.Drawing.Point(0, 644);
            this.btnRecibos.Name = "btnRecibos";
            this.btnRecibos.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnRecibos.Size = new System.Drawing.Size(260, 44);
            this.btnRecibos.TabIndex = 16;
            this.btnRecibos.Text = "Recibos de nómina";
            this.btnRecibos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRecibos.UseVisualStyleBackColor = false;
            this.btnRecibos.Click += new System.EventHandler(this.btnRecibos_Click);
            //
            // lblSeccionDocumentos
            //
            this.lblSeccionDocumentos.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSeccionDocumentos.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSeccionDocumentos.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblSeccionDocumentos.Location = new System.Drawing.Point(0, 612);
            this.lblSeccionDocumentos.Name = "lblSeccionDocumentos";
            this.lblSeccionDocumentos.Padding = new System.Windows.Forms.Padding(24, 12, 0, 2);
            this.lblSeccionDocumentos.Size = new System.Drawing.Size(260, 32);
            this.lblSeccionDocumentos.TabIndex = 15;
            this.lblSeccionDocumentos.Text = "DOCUMENTOS";
            this.lblSeccionDocumentos.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // btnNomina
            //
            this.btnNomina.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnNomina.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNomina.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNomina.FlatAppearance.BorderSize = 0;
            this.btnNomina.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnNomina.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNomina.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnNomina.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnNomina.Location = new System.Drawing.Point(0, 568);
            this.btnNomina.Name = "btnNomina";
            this.btnNomina.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnNomina.Size = new System.Drawing.Size(260, 44);
            this.btnNomina.TabIndex = 14;
            this.btnNomina.Text = "Cálculo de nómina";
            this.btnNomina.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNomina.UseVisualStyleBackColor = false;
            this.btnNomina.Click += new System.EventHandler(this.btnNomina_Click);
            //
            // btnPeriodos
            //
            this.btnPeriodos.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnPeriodos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPeriodos.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPeriodos.FlatAppearance.BorderSize = 0;
            this.btnPeriodos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnPeriodos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPeriodos.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnPeriodos.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnPeriodos.Location = new System.Drawing.Point(0, 524);
            this.btnPeriodos.Name = "btnPeriodos";
            this.btnPeriodos.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnPeriodos.Size = new System.Drawing.Size(260, 44);
            this.btnPeriodos.TabIndex = 13;
            this.btnPeriodos.Text = "Periodos y festivos";
            this.btnPeriodos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPeriodos.UseVisualStyleBackColor = false;
            this.btnPeriodos.Click += new System.EventHandler(this.btnPeriodos_Click);
            //
            // lblSeccionProcesos
            //
            this.lblSeccionProcesos.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSeccionProcesos.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSeccionProcesos.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblSeccionProcesos.Location = new System.Drawing.Point(0, 492);
            this.lblSeccionProcesos.Name = "lblSeccionProcesos";
            this.lblSeccionProcesos.Padding = new System.Windows.Forms.Padding(24, 12, 0, 2);
            this.lblSeccionProcesos.Size = new System.Drawing.Size(260, 32);
            this.lblSeccionProcesos.TabIndex = 12;
            this.lblSeccionProcesos.Text = "PROCESOS";
            this.lblSeccionProcesos.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // btnCatalogosSat
            //
            this.btnCatalogosSat.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnCatalogosSat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCatalogosSat.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatalogosSat.FlatAppearance.BorderSize = 0;
            this.btnCatalogosSat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnCatalogosSat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatalogosSat.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnCatalogosSat.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnCatalogosSat.Location = new System.Drawing.Point(0, 448);
            this.btnCatalogosSat.Name = "btnCatalogosSat";
            this.btnCatalogosSat.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnCatalogosSat.Size = new System.Drawing.Size(260, 44);
            this.btnCatalogosSat.TabIndex = 11;
            this.btnCatalogosSat.Text = "Catálogos del SAT";
            this.btnCatalogosSat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatalogosSat.UseVisualStyleBackColor = false;
            this.btnCatalogosSat.Click += new System.EventHandler(this.btnCatalogosSat_Click);
            //
            // btnConfiguracion
            //
            this.btnConfiguracion.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnConfiguracion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfiguracion.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnConfiguracion.FlatAppearance.BorderSize = 0;
            this.btnConfiguracion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnConfiguracion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfiguracion.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnConfiguracion.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnConfiguracion.Location = new System.Drawing.Point(0, 492);
            this.btnConfiguracion.Name = "btnConfiguracion";
            this.btnConfiguracion.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnConfiguracion.Size = new System.Drawing.Size(260, 44);
            this.btnConfiguracion.TabIndex = 11;
            this.btnConfiguracion.Text = "Configuración";
            this.btnConfiguracion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfiguracion.UseVisualStyleBackColor = false;
            this.btnConfiguracion.Click += new System.EventHandler(this.btnConfiguracion_Click);
            //
            // lblSeccionConfiguracion
            //
            this.lblSeccionConfiguracion.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSeccionConfiguracion.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSeccionConfiguracion.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblSeccionConfiguracion.Location = new System.Drawing.Point(0, 416);
            this.lblSeccionConfiguracion.Name = "lblSeccionConfiguracion";
            this.lblSeccionConfiguracion.Padding = new System.Windows.Forms.Padding(24, 12, 0, 2);
            this.lblSeccionConfiguracion.Size = new System.Drawing.Size(260, 32);
            this.lblSeccionConfiguracion.TabIndex = 10;
            this.lblSeccionConfiguracion.Text = "CONFIGURACIÓN";
            this.lblSeccionConfiguracion.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // btnUsuarios
            //
            this.btnUsuarios.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnUsuarios.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUsuarios.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUsuarios.FlatAppearance.BorderSize = 0;
            this.btnUsuarios.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnUsuarios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsuarios.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnUsuarios.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnUsuarios.Location = new System.Drawing.Point(0, 372);
            this.btnUsuarios.Name = "btnUsuarios";
            this.btnUsuarios.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnUsuarios.Size = new System.Drawing.Size(260, 44);
            this.btnUsuarios.TabIndex = 9;
            this.btnUsuarios.Text = "Usuarios y roles";
            this.btnUsuarios.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUsuarios.UseVisualStyleBackColor = false;
            this.btnUsuarios.Click += new System.EventHandler(this.btnUsuarios_Click);
            //
            // lblSeccionAdministracion
            //
            this.lblSeccionAdministracion.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSeccionAdministracion.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSeccionAdministracion.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblSeccionAdministracion.Location = new System.Drawing.Point(0, 384);
            this.lblSeccionAdministracion.Name = "lblSeccionAdministracion";
            this.lblSeccionAdministracion.Padding = new System.Windows.Forms.Padding(24, 12, 0, 2);
            this.lblSeccionAdministracion.Size = new System.Drawing.Size(260, 32);
            this.lblSeccionAdministracion.TabIndex = 8;
            this.lblSeccionAdministracion.Text = "ADMINISTRACIÓN";
            this.lblSeccionAdministracion.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // btnAsignacionPaquetes
            //
            this.btnAsignacionPaquetes.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnAsignacionPaquetes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAsignacionPaquetes.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAsignacionPaquetes.FlatAppearance.BorderSize = 0;
            this.btnAsignacionPaquetes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnAsignacionPaquetes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsignacionPaquetes.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnAsignacionPaquetes.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnAsignacionPaquetes.Location = new System.Drawing.Point(0, 328);
            this.btnAsignacionPaquetes.Name = "btnAsignacionPaquetes";
            this.btnAsignacionPaquetes.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnAsignacionPaquetes.Size = new System.Drawing.Size(260, 44);
            this.btnAsignacionPaquetes.TabIndex = 7;
            this.btnAsignacionPaquetes.Text = "Paquetes por periodo";
            this.btnAsignacionPaquetes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAsignacionPaquetes.UseVisualStyleBackColor = false;
            this.btnAsignacionPaquetes.Click += new System.EventHandler(this.btnAsignacionPaquetes_Click);
            //
            // btnPaquetes
            //
            this.btnPaquetes.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnPaquetes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPaquetes.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPaquetes.FlatAppearance.BorderSize = 0;
            this.btnPaquetes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnPaquetes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPaquetes.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnPaquetes.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnPaquetes.Location = new System.Drawing.Point(0, 284);
            this.btnPaquetes.Name = "btnPaquetes";
            this.btnPaquetes.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnPaquetes.Size = new System.Drawing.Size(260, 44);
            this.btnPaquetes.TabIndex = 6;
            this.btnPaquetes.Text = "Paquetes de incidencias";
            this.btnPaquetes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPaquetes.UseVisualStyleBackColor = false;
            this.btnPaquetes.Click += new System.EventHandler(this.btnPaquetes_Click);
            //
            // btnEstados
            //
            this.btnEstados.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnEstados.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEstados.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEstados.FlatAppearance.BorderSize = 0;
            this.btnEstados.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnEstados.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEstados.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnEstados.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnEstados.Location = new System.Drawing.Point(0, 240);
            this.btnEstados.Name = "btnEstados";
            this.btnEstados.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnEstados.Size = new System.Drawing.Size(260, 44);
            this.btnEstados.TabIndex = 5;
            this.btnEstados.Text = "Estados";
            this.btnEstados.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEstados.UseVisualStyleBackColor = false;
            this.btnEstados.Click += new System.EventHandler(this.btnEstados_Click);
            //
            // btnDepartamentos
            //
            this.btnDepartamentos.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnDepartamentos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDepartamentos.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDepartamentos.FlatAppearance.BorderSize = 0;
            this.btnDepartamentos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnDepartamentos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDepartamentos.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnDepartamentos.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnDepartamentos.Location = new System.Drawing.Point(0, 196);
            this.btnDepartamentos.Name = "btnDepartamentos";
            this.btnDepartamentos.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnDepartamentos.Size = new System.Drawing.Size(260, 44);
            this.btnDepartamentos.TabIndex = 4;
            this.btnDepartamentos.Text = "Departamentos";
            this.btnDepartamentos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDepartamentos.UseVisualStyleBackColor = false;
            this.btnDepartamentos.Click += new System.EventHandler(this.btnDepartamentos_Click);
            //
            // btnPuestos
            //
            this.btnPuestos.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnPuestos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPuestos.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPuestos.FlatAppearance.BorderSize = 0;
            this.btnPuestos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnPuestos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPuestos.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnPuestos.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnPuestos.Location = new System.Drawing.Point(0, 152);
            this.btnPuestos.Name = "btnPuestos";
            this.btnPuestos.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnPuestos.Size = new System.Drawing.Size(260, 44);
            this.btnPuestos.TabIndex = 3;
            this.btnPuestos.Text = "Puestos";
            this.btnPuestos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPuestos.UseVisualStyleBackColor = false;
            this.btnPuestos.Click += new System.EventHandler(this.btnPuestos_Click);
            //
            // btnEmpleados
            //
            this.btnEmpleados.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnEmpleados.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEmpleados.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEmpleados.FlatAppearance.BorderSize = 0;
            this.btnEmpleados.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnEmpleados.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmpleados.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnEmpleados.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnEmpleados.Location = new System.Drawing.Point(0, 108);
            this.btnEmpleados.Name = "btnEmpleados";
            this.btnEmpleados.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnEmpleados.Size = new System.Drawing.Size(260, 44);
            this.btnEmpleados.TabIndex = 2;
            this.btnEmpleados.Text = "Empleados";
            this.btnEmpleados.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEmpleados.UseVisualStyleBackColor = false;
            this.btnEmpleados.Click += new System.EventHandler(this.btnEmpleados_Click);
            //
            // btnEmpresa
            //
            this.btnEmpresa.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.btnEmpresa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEmpresa.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEmpresa.FlatAppearance.BorderSize = 0;
            this.btnEmpresa.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(27, 58, 107);
            this.btnEmpresa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmpresa.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.btnEmpresa.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.btnEmpresa.Location = new System.Drawing.Point(0, 64);
            this.btnEmpresa.Name = "btnEmpresa";
            this.btnEmpresa.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.btnEmpresa.Size = new System.Drawing.Size(260, 44);
            this.btnEmpresa.TabIndex = 1;
            this.btnEmpresa.Text = "Empresas";
            this.btnEmpresa.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEmpresa.UseVisualStyleBackColor = false;
            this.btnEmpresa.Click += new System.EventHandler(this.btnEmpresa_Click);
            //
            // lblSeccionCatalogos
            //
            this.lblSeccionCatalogos.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSeccionCatalogos.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSeccionCatalogos.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblSeccionCatalogos.Location = new System.Drawing.Point(0, 32);
            this.lblSeccionCatalogos.Name = "lblSeccionCatalogos";
            this.lblSeccionCatalogos.Padding = new System.Windows.Forms.Padding(24, 12, 0, 2);
            this.lblSeccionCatalogos.Size = new System.Drawing.Size(260, 32);
            this.lblSeccionCatalogos.TabIndex = 0;
            this.lblSeccionCatalogos.Text = "CATÁLOGOS";
            this.lblSeccionCatalogos.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // lblSloganEjecutivo
            //
            this.lblSloganEjecutivo.BackColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.lblSloganEjecutivo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSloganEjecutivo.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Italic);
            this.lblSloganEjecutivo.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.lblSloganEjecutivo.Location = new System.Drawing.Point(0, 100);
            this.lblSloganEjecutivo.Name = "lblSloganEjecutivo";
            this.lblSloganEjecutivo.Padding = new System.Windows.Forms.Padding(24, 0, 18, 0);
            this.lblSloganEjecutivo.Size = new System.Drawing.Size(260, 60);
            this.lblSloganEjecutivo.TabIndex = 1;
            this.lblSloganEjecutivo.Text = "El control total de tu nómina y capital humano.";
            this.lblSloganEjecutivo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlLogo
            //
            this.pnlLogo.BackColor = System.Drawing.Color.White;
            this.pnlLogo.Controls.Add(this.lblMarcaLatSub);
            this.pnlLogo.Controls.Add(this.lblMarcaLat);
            this.pnlLogo.Controls.Add(this.picLogoLateral);
            this.pnlLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogo.Location = new System.Drawing.Point(0, 0);
            this.pnlLogo.Name = "pnlLogo";
            this.pnlLogo.Size = new System.Drawing.Size(260, 100);
            this.pnlLogo.TabIndex = 0;
            //
            // lblMarcaLatSub
            //
            this.lblMarcaLatSub.AutoSize = true;
            this.lblMarcaLatSub.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblMarcaLatSub.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblMarcaLatSub.Location = new System.Drawing.Point(90, 57);
            this.lblMarcaLatSub.Name = "lblMarcaLatSub";
            this.lblMarcaLatSub.Size = new System.Drawing.Size(126, 13);
            this.lblMarcaLatSub.TabIndex = 2;
            this.lblMarcaLatSub.Text = "Nómina y Recursos Humanos";
            //
            // lblMarcaLat
            //
            this.lblMarcaLat.AutoSize = true;
            this.lblMarcaLat.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblMarcaLat.ForeColor = System.Drawing.Color.FromArgb(15, 44, 89);
            this.lblMarcaLat.Location = new System.Drawing.Point(86, 26);
            this.lblMarcaLat.Name = "lblMarcaLat";
            this.lblMarcaLat.Size = new System.Drawing.Size(110, 28);
            this.lblMarcaLat.TabIndex = 1;
            this.lblMarcaLat.Text = "NomiMex";
            //
            // picLogoLateral
            //
            this.picLogoLateral.BackColor = System.Drawing.Color.Transparent;
            this.picLogoLateral.Image = ((System.Drawing.Image)(resources.GetObject("picLogoLateral.Image")));
            this.picLogoLateral.Location = new System.Drawing.Point(20, 22);
            this.picLogoLateral.Name = "picLogoLateral";
            this.picLogoLateral.Size = new System.Drawing.Size(56, 56);
            this.picLogoLateral.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogoLateral.TabIndex = 0;
            this.picLogoLateral.TabStop = false;
            //
            // pnlPie
            //
            this.pnlPie.BackColor = System.Drawing.Color.FromArgb(11, 33, 66);
            this.pnlPie.Controls.Add(this.btnCerrarSesion);
            this.pnlPie.Controls.Add(this.btnMiContrasena);
            this.pnlPie.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPie.Location = new System.Drawing.Point(0, 648);
            this.pnlPie.Name = "pnlPie";
            this.pnlPie.Padding = new System.Windows.Forms.Padding(16, 10, 16, 12);
            this.pnlPie.Size = new System.Drawing.Size(260, 112);
            this.pnlPie.TabIndex = 3;
            //
            // btnCerrarSesion
            //
            this.btnCerrarSesion.BackColor = System.Drawing.Color.FromArgb(185, 28, 28);
            this.btnCerrarSesion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCerrarSesion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCerrarSesion.FlatAppearance.BorderSize = 0;
            this.btnCerrarSesion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(153, 27, 27);
            this.btnCerrarSesion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarSesion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCerrarSesion.ForeColor = System.Drawing.Color.White;
            this.btnCerrarSesion.Location = new System.Drawing.Point(16, 10);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(228, 42);
            this.btnCerrarSesion.TabIndex = 0;
            this.btnCerrarSesion.Text = "Cerrar sesión";
            this.btnCerrarSesion.UseVisualStyleBackColor = false;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            //
            // btnMiContrasena
            //
            this.btnMiContrasena.BackColor = System.Drawing.Color.FromArgb(0, 135, 90);
            this.btnMiContrasena.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMiContrasena.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMiContrasena.FlatAppearance.BorderSize = 0;
            this.btnMiContrasena.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 110, 74);
            this.btnMiContrasena.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMiContrasena.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnMiContrasena.ForeColor = System.Drawing.Color.White;
            this.btnMiContrasena.Location = new System.Drawing.Point(16, 10);
            this.btnMiContrasena.Name = "btnMiContrasena";
            this.btnMiContrasena.Size = new System.Drawing.Size(228, 38);
            this.btnMiContrasena.TabIndex = 1;
            this.btnMiContrasena.Text = "Cambiar mi contraseña";
            this.btnMiContrasena.UseVisualStyleBackColor = false;
            this.btnMiContrasena.Click += new System.EventHandler(this.btnMiContrasena_Click);
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.ClientSize = new System.Drawing.Size(1280, 760);
            this.Controls.Add(this.pnlContenedor);
            this.Controls.Add(this.pnlSuperior);
            this.Controls.Add(this.pnlLateral);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NomiMex - Nómina y Recursos Humanos";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPrincipal_FormClosing);
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.pnlContenedor.ResumeLayout(false);
            this.pnlSuperior.ResumeLayout(false);
            this.pnlSuperior.PerformLayout();
            this.pnlLateral.ResumeLayout(false);
            this.pnlNav.ResumeLayout(false);
            this.pnlLogo.ResumeLayout(false);
            this.pnlLogo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogoLateral)).EndInit();
            this.pnlPie.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlContenedor;
        private System.Windows.Forms.Label lblBienvenida;
        private System.Windows.Forms.Panel pnlSuperior;
        private System.Windows.Forms.Label lblRolSesion;
        private System.Windows.Forms.Label lblUsuarioSesion;
        private System.Windows.Forms.Label lblModulo;
        private System.Windows.Forms.Panel pnlLateral;
        private System.Windows.Forms.Panel pnlNav;
        private System.Windows.Forms.Button btnRecibos;
        private System.Windows.Forms.Label lblSeccionDocumentos;
        private System.Windows.Forms.Button btnNomina;
        private System.Windows.Forms.Button btnPeriodos;
        private System.Windows.Forms.Label lblSeccionProcesos;
        private System.Windows.Forms.Button btnCatalogosSat;
        private System.Windows.Forms.Button btnConfiguracion;
        private System.Windows.Forms.Label lblSeccionConfiguracion;
        private System.Windows.Forms.Button btnUsuarios;
        private System.Windows.Forms.Label lblSeccionAdministracion;
        private System.Windows.Forms.Button btnAsignacionPaquetes;
        private System.Windows.Forms.Button btnPaquetes;
        private System.Windows.Forms.Button btnEstados;
        private System.Windows.Forms.Button btnDepartamentos;
        private System.Windows.Forms.Button btnPuestos;
        private System.Windows.Forms.Button btnEmpleados;
        private System.Windows.Forms.Button btnEmpresa;
        private System.Windows.Forms.Label lblSeccionCatalogos;
        private System.Windows.Forms.Label lblSloganEjecutivo;
        private System.Windows.Forms.Panel pnlLogo;
        private System.Windows.Forms.Label lblMarcaLatSub;
        private System.Windows.Forms.Label lblMarcaLat;
        private System.Windows.Forms.PictureBox picLogoLateral;
        private System.Windows.Forms.Panel pnlPie;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.Button btnMiContrasena;
    }
}
