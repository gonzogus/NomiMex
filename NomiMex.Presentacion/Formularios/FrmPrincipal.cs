using System;
using System.Drawing;
using System.Windows.Forms;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Seguridad;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    public partial class FrmPrincipal : Form
    {
        private static readonly Color ColorMenu = Color.FromArgb(15, 44, 89);
        private static readonly Color ColorMenuActivo = Color.FromArgb(0, 135, 90);

        private readonly AutenticacionBLL _autenticacion = new AutenticacionBLL();
        private readonly PrerrequisitosNomina _prerrequisitos = new PrerrequisitosNomina();
        private Form _formularioActual;
        private Button _botonActivo;

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarSesion();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Menú principal");
            }
        }

        private void FrmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (SesionActual.HaySesion)
                {
                    _autenticacion.CerrarSesion();
                }
            }
            catch (ExcepcionNegocio)
            {
                // Sin conexión al cerrar: el token de sesión expira por sí solo en el servidor.
            }
        }

        private void ConfigurarSesion()
        {
            SesionUsuario sesion = SesionActual.Usuario;
            lblUsuarioSesion.Text = sesion.NombreMostrar + "  (" + sesion.NombreUsuario + ")";
            lblRolSesion.Text = "Rol: " + sesion.RolesTexto;

            btnEmpresa.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Empresa);
            btnEmpleados.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Empleados);
            btnPuestos.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Puestos);
            btnDepartamentos.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Departamentos);
            btnEstados.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Estados);
            btnPaquetes.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Empleados);
            btnAsignacionPaquetes.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Empleados);
            btnUsuarios.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Usuarios);
            btnCatalogosSat.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Configuracion);
            btnConfiguracion.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Configuracion);
            btnNomina.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Nomina);
            btnPeriodos.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Periodos);
            btnRecibos.Visible = ControlAcceso.PuedeAcceder(ModuloSistema.Recibos);

            lblSeccionCatalogos.Visible = btnEmpresa.Visible || btnEmpleados.Visible || btnPuestos.Visible ||
                                          btnDepartamentos.Visible || btnEstados.Visible || btnPaquetes.Visible || btnAsignacionPaquetes.Visible;
            lblSeccionAdministracion.Visible = btnUsuarios.Visible;
            lblSeccionConfiguracion.Visible = btnCatalogosSat.Visible || btnConfiguracion.Visible;
            lblSeccionProcesos.Visible = btnNomina.Visible || btnPeriodos.Visible;
            lblSeccionDocumentos.Visible = btnRecibos.Visible;

            lblModulo.Text = "Inicio";
            lblBienvenida.Text = "Bienvenido(a), " + sesion.NombreMostrar + Environment.NewLine +
                                 "Seleccione una opción del menú lateral.";
            lblBienvenida.Visible = true;
        }

        private void AbrirFormulario(Form formulario, string titulo, Button boton)
        {
            CerrarFormularioActual();

            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;

            pnlContenedor.Controls.Add(formulario);
            lblBienvenida.Visible = false;
            formulario.BringToFront();
            formulario.Show();

            _formularioActual = formulario;
            lblModulo.Text = titulo;
            MarcarBotonActivo(boton);
        }

        private void CerrarFormularioActual()
        {
            if (_formularioActual != null)
            {
                pnlContenedor.Controls.Remove(_formularioActual);
                _formularioActual.Dispose();
                _formularioActual = null;
            }
            MarcarBotonActivo(null);
        }

        private void MarcarBotonActivo(Button boton)
        {
            if (_botonActivo != null)
            {
                _botonActivo.BackColor = ColorMenu;
            }
            _botonActivo = boton;
            if (_botonActivo != null)
            {
                _botonActivo.BackColor = ColorMenuActivo;
            }
        }

        private void btnEmpresa_Click(object sender, EventArgs e)
        {
            try { AbrirFormulario(new FrmCatEmpresa(), "Catálogo de empresas", btnEmpresa); }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Empresas"); }
        }

        private void btnEmpleados_Click(object sender, EventArgs e)
        {
            try
            {
                _prerrequisitos.ExigirEmpresaRegistrada();
                AbrirFormulario(new FrmCatEmpleados(), "Catálogo de empleados", btnEmpleados);
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Empleados"); }
        }

        private void btnPuestos_Click(object sender, EventArgs e)
        {
            try { AbrirFormulario(new FrmCatPuestos(), "Catálogo de puestos", btnPuestos); }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Puestos"); }
        }

        private void btnDepartamentos_Click(object sender, EventArgs e)
        {
            try { AbrirFormulario(new FrmCatDepartamentos(), "Catálogo de departamentos", btnDepartamentos); }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Departamentos"); }
        }

        private void btnEstados_Click(object sender, EventArgs e)
        {
            try { AbrirFormulario(new FrmCatEstados(), "Catálogo de estados", btnEstados); }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Estados"); }
        }

        private void btnAsignacionPaquetes_Click(object sender, EventArgs e)
        {
            try { AbrirFormulario(new FrmAsignacionPaquetes(), "Paquetes por periodo", btnAsignacionPaquetes); }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Paquetes por periodo"); }
        }

        private void btnPaquetes_Click(object sender, EventArgs e)
        {
            try { AbrirFormulario(new FrmCatPaquetes(), "Paquetes de incidencias", btnPaquetes); }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Paquetes de incidencias"); }
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            try { AbrirFormulario(new FrmCatUsuarios(), "Usuarios y roles", btnUsuarios); }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Usuarios"); }
        }

        private void btnCatalogosSat_Click(object sender, EventArgs e)
        {
            try { AbrirFormulario(new FrmCatSatCatalogos(), "Catálogos del SAT", btnCatalogosSat); }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Catálogos del SAT"); }
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            try
            {
                using (FrmConfiguracion frm = new FrmConfiguracion())
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Configuración");
            }
        }

        private void btnPeriodos_Click(object sender, EventArgs e)
        {
            try
            {
                _prerrequisitos.ExigirEmpresaRegistrada();
                AbrirFormulario(new FrmConfigPeriodos(), "Periodos y días festivos", btnPeriodos);
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Periodos"); }
        }

        private void btnNomina_Click(object sender, EventArgs e)
        {
            try
            {
                _prerrequisitos.ExigirEmpresaRegistrada();
                AbrirFormulario(new FrmProcNomina(), "Proceso de nómina", btnNomina);
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Nómina"); }
        }

        private void btnRecibos_Click(object sender, EventArgs e)
        {
            try
            {
                _prerrequisitos.ExigirEmpresaRegistrada();
                AbrirFormulario(new FrmDocRecibos(), "Recibos de nómina", btnRecibos);
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Recibos"); }
        }

        private void btnMiContrasena_Click(object sender, EventArgs e)
        {
            try
            {
                using (FrmCambiarContrasena frm = new FrmCambiarContrasena(ModoCambioContrasena.Voluntario))
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Cambiar contraseña");
            }
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            if (!Mensajes.Confirmar("¿Desea cerrar la sesión actual?", "Cerrar sesión"))
            {
                return;
            }

            try
            {
                _autenticacion.CerrarSesion();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Cerrar sesión");
            }

            CerrarFormularioActual();
            Hide();
            using (FrmLogin login = new FrmLogin())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    ConfigurarSesion();
                    Show();
                }
                else
                {
                    Close();
                }
            }
        }
    }
}
