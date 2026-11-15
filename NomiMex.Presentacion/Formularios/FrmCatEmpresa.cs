using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    public partial class FrmCatEmpresa : Form
    {
        private readonly EmpresaBLL _empresaBll = new EmpresaBLL();
        private readonly CatalogoBLL _catalogoBll = new CatalogoBLL();
        private readonly EstadoBLL _estadoBll = new EstadoBLL();
        private readonly ErrorProvider _errorProvider;
        private List<CatEmpresa> _empresas = new List<CatEmpresa>();
        private List<ItemCatalogo> _tiposPeriodicidad = new List<ItemCatalogo>();
        private readonly List<ItemCatalogo> _periodicidadesSeleccionadas = new List<ItemCatalogo>();
        private int _empresaIdSeleccionada;
        private byte[] _logo;
        private bool _logoModificado;

        public FrmCatEmpresa()
        {
            InitializeComponent();
            ConfigurarColumnasEmpresas();
            if (components == null)
            {
                components = new Container();
            }
            _errorProvider = new ErrorProvider(components)
            {
                ContainerControl = this,
                BlinkStyle = ErrorBlinkStyle.NeverBlink
            };
            RegistrarValidaciones();
        }

        private void ConfigurarColumnasEmpresas()
        {
            dgvEmpresas.AutoGenerateColumns = false;
            dgvEmpresas.Columns.Clear();
            dgvEmpresas.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "colEmpresaId",
                    HeaderText = "ID",
                    DataPropertyName = nameof(CatEmpresa.EmpresaId),
                    FillWeight = 4
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colRazonSocial",
                    HeaderText = "Razón social",
                    DataPropertyName = nameof(CatEmpresa.RazonSocial),
                    FillWeight = 22
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colNombreComercial",
                    HeaderText = "Nombre comercial",
                    DataPropertyName = nameof(CatEmpresa.NombreComercial),
                    FillWeight = 18
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colRFC",
                    HeaderText = "RFC",
                    DataPropertyName = nameof(CatEmpresa.RFC),
                    FillWeight = 12
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colCURP",
                    HeaderText = "CURP",
                    DataPropertyName = nameof(CatEmpresa.CURP),
                    FillWeight = 14
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colRegistroPatronal",
                    HeaderText = "Registro patronal",
                    DataPropertyName = nameof(CatEmpresa.RegistroPatronal),
                    FillWeight = 10
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colCodigoPostal",
                    HeaderText = "C.P.",
                    DataPropertyName = nameof(CatEmpresa.CodigoPostal),
                    FillWeight = 6
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colLugarExpedicion",
                    HeaderText = "Expedición",
                    DataPropertyName = nameof(CatEmpresa.LugarExpedicion),
                    FillWeight = 6
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colPrimaRiesgoTrabajo",
                    HeaderText = "Prima (%)",
                    DataPropertyName = nameof(CatEmpresa.PrimaRiesgoTrabajoPct),
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "0.#####" },
                    FillWeight = 5
                },
                new DataGridViewCheckBoxColumn
                {
                    Name = "colActiva",
                    HeaderText = "Activa",
                    DataPropertyName = nameof(CatEmpresa.Activa),
                    FillWeight = 3
                }
            });
        }

        private void RegistrarValidaciones()
        {
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            Control[] controles =
            {
                txtRazonSocial, txtNombreComercial, txtRFC, txtCURP, txtRegistroPatronal,
                cmbRegimenFiscal, txtRfcPatronOrigen, txtCodigoPostal, txtLugarExpedicion,
                cmbOrigenRecurso, txtMontoRecursoPropio, cmbRiesgoPuesto, txtPrimaRiesgoTrabajo
            };

            foreach (Control control in controles)
            {
                control.Validating += Campo_Validating;
                TextBox cuadro = control as TextBox;
                if (cuadro != null)
                {
                    cuadro.TextChanged += Campo_TextChanged;
                }
                ComboBox combo = control as ComboBox;
                if (combo != null)
                {
                    combo.SelectedIndexChanged += Campo_Changed;
                }
            }
        }

        private void Campo_Validating(object sender, CancelEventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                ValidarControl(control);
            }
        }

        private void Campo_TextChanged(object sender, EventArgs e)
        {
            LimpiarError(sender as Control);
        }

        private void Campo_Changed(object sender, EventArgs e)
        {
            LimpiarError(sender as Control);
        }

        private void LimpiarError(Control control)
        {
            if (control != null)
            {
                _errorProvider.SetError(control, string.Empty);
            }
        }

        private bool ValidarControl(Control control)
        {
            string error = null;
            try
            {
                if (control == txtRazonSocial)
                {
                    Validador.Requerido(txtRazonSocial.Text, "Razón social", 200);
                }
                else if (control == txtNombreComercial)
                {
                    Validador.Opcional(txtNombreComercial.Text, "Nombre comercial", 200);
                }
                else if (control == txtRFC)
                {
                    Validador.RFC(txtRFC.Text, "RFC", true, false);
                }
                else if (control == txtCURP)
                {
                    Validador.CURP(txtCURP.Text, "CURP", false);
                }
                else if (control == txtRegistroPatronal)
                {
                    Validador.Requerido(txtRegistroPatronal.Text, "Registro patronal", 20);
                }
                else if (control == cmbRegimenFiscal)
                {
                    Validador.SeleccionRequerida(AyudaCombos.Id(cmbRegimenFiscal), "Régimen fiscal");
                }
                else if (control == txtRfcPatronOrigen)
                {
                    Validador.RFC(txtRfcPatronOrigen.Text, "RFC del patrón de origen", false, false);
                }
                else if (control == txtCodigoPostal)
                {
                    Validador.CodigoPostal(txtCodigoPostal.Text, "Código postal", true);
                }
                else if (control == txtLugarExpedicion)
                {
                    Validador.CodigoPostal(txtLugarExpedicion.Text, "Lugar de expedición", true);
                }
                else if (control == cmbOrigenRecurso)
                {
                    Validador.SeleccionRequerida(AyudaCombos.Id(cmbOrigenRecurso), "Origen del recurso");
                }
                else if (control == txtMontoRecursoPropio && txtMontoRecursoPropio.Enabled)
                {
                    decimal? monto = Validador.DecimalOpcional(txtMontoRecursoPropio.Text, "Monto de recurso propio");
                    if (!monto.HasValue || monto.Value < 0m)
                    {
                        throw new ExcepcionNegocio("Capture un monto de recurso propio igual o mayor a cero.");
                    }
                }
                else if (control == cmbRiesgoPuesto)
                {
                    decimal? prima = Validador.DecimalOpcional(txtPrimaRiesgoTrabajo.Text, "Prima de riesgo de trabajo (%)");
                    if (prima.HasValue && !AyudaCombos.Id(cmbRiesgoPuesto).HasValue)
                    {
                        throw new ExcepcionNegocio("Seleccione la clase de riesgo de trabajo para la prima capturada.");
                    }
                }
                else if (control == txtPrimaRiesgoTrabajo)
                {
                    decimal? prima = Validador.DecimalOpcional(txtPrimaRiesgoTrabajo.Text, "Prima de riesgo de trabajo (%)");
                    if (prima.HasValue && !AyudaCombos.Id(cmbRiesgoPuesto).HasValue)
                    {
                        throw new ExcepcionNegocio("Seleccione la clase de riesgo de trabajo para la prima capturada.");
                    }
                    if (prima.HasValue && (prima.Value < 0.5m || prima.Value > 15m))
                    {
                        throw new ExcepcionNegocio("La prima de riesgo de trabajo debe estar entre 0.5% y 15%.");
                    }
                }
                else if (control == dgvPeriodicidades && _periodicidadesSeleccionadas.Count == 0)
                {
                    throw new ExcepcionNegocio("Asigne a la empresa al menos una periodicidad de pago.");
                }
            }
            catch (ExcepcionNegocio ex)
            {
                error = ex.Message;
            }

            _errorProvider.SetError(control, error ?? string.Empty);
            return error == null;
        }

        private bool ValidarFormularioEntrada()
        {
            Control[] controles =
            {
                txtRazonSocial, txtNombreComercial, txtRFC, txtCURP, txtRegistroPatronal,
                cmbRegimenFiscal, txtRfcPatronOrigen, txtCodigoPostal, txtLugarExpedicion,
                cmbOrigenRecurso, txtMontoRecursoPropio, cmbRiesgoPuesto, txtPrimaRiesgoTrabajo,
                dgvPeriodicidades
            };
            Control primerControlInvalido = null;
            foreach (Control control in controles)
            {
                if (!ValidarControl(control) && primerControlInvalido == null)
                {
                    primerControlInvalido = control;
                }
            }

            if (primerControlInvalido == null)
            {
                return true;
            }

            if (primerControlInvalido.CanFocus)
            {
                primerControlInvalido.Focus();
            }
            Mensajes.Advertencia("Corrija los campos marcados antes de guardar.", "Validación de datos");
            return false;
        }

        private void FrmCatEmpresa_Load(object sender, EventArgs e)
        {
            try
            {
                AyudaCombos.CargarCatalogo(cmbRegimenFiscal, _catalogoBll.ListarRegimenesFiscales());
                AyudaCombos.CargarEstados(cmbEstado, _estadoBll.Listar(true));
                AyudaCombos.CargarCatalogo(cmbOrigenRecurso, _catalogoBll.ListarOrigenesRecursoItems());
                AyudaCombos.CargarCatalogo(cmbRiesgoPuesto, _catalogoBll.ListarRiesgosPuesto());
                _tiposPeriodicidad = _catalogoBll.ListarTiposPeriodo();
                AyudaCombos.CargarCatalogo(cmbPeriodicidad, _tiposPeriodicidad);
                CargarEmpresas();
                LimpiarFormulario();
                
                if (dgvEmpresas.Rows.Count > 0)
                {
                    dgvEmpresas.Rows[0].Selected = true;
                    dgvEmpresas_CellClick(dgvEmpresas, new DataGridViewCellEventArgs(0, 0));
                }

            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empresas");
            }
        }

        private void CargarEmpresas()
        {
            _empresas = _empresaBll.Listar(false);
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            List<CatEmpresa> filtradas = _empresaBll.Filtrar(_empresas, txtBuscar.Text);
            dgvEmpresas.DataSource = filtradas;
            lblConteo.Text = filtradas.Count + " de " + _empresas.Count + " registros";
            
        }

        private void LimpiarFormulario()
        {
            _empresaIdSeleccionada = 0;
            txtRazonSocial.Clear();
            txtNombreComercial.Clear();
            txtRFC.Clear();
            txtCURP.Clear();
            txtRegistroPatronal.Clear();
            AyudaCombos.SeleccionarId(cmbRegimenFiscal, null);
            txtRfcPatronOrigen.Clear();
            txtCodigoPostal.Clear();
            txtLugarExpedicion.Clear();
            AyudaCombos.SeleccionarClave(cmbEstado, null);
            cmbOrigenRecurso.SelectedIndex = cmbOrigenRecurso.Items.Count > 1 ? 1 : 0;
            txtMontoRecursoPropio.Clear();
            AyudaCombos.SeleccionarId(cmbRiesgoPuesto, null);
            txtPrimaRiesgoTrabajo.Clear();
            EstablecerLogo(null, false);
            _periodicidadesSeleccionadas.Clear();
            ActualizarPeriodicidades();
            AyudaCombos.SeleccionarId(cmbPeriodicidad, 0);
            dgvCertificados.DataSource = new List<EmpresaCertificado>();
            LimpiarCamposCsd();
            chkActiva.Checked = true;
            dgvEmpresas.ClearSelection();
            ActualizarBotones();
            tabEmpresa.SelectedTab = tabGenerales;
            txtRazonSocial.Focus();
        }

        private void ActualizarBotones()
        {
            bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Empresa, AccionSistema.Escribir);
            bool haySeleccion = _empresaIdSeleccionada > 0;
            btnGuardar.Enabled = puedeEscribir && !haySeleccion;
            btnEditar.Enabled = puedeEscribir && haySeleccion;
            btnEliminar.Enabled = puedeEscribir && haySeleccion;
            btnCargarCsd.Enabled = puedeEscribir && haySeleccion;
            lblModo.Text = haySeleccion ? "Editando empresa" : "Nueva empresa";
        }

        private CatEmpresa LeerFormulario()
        {
            return new CatEmpresa
            {
                EmpresaId = _empresaIdSeleccionada,
                RazonSocial = txtRazonSocial.Text,
                NombreComercial = txtNombreComercial.Text,
                RFC = txtRFC.Text,
                CURP = txtCURP.Text,
                RegistroPatronal = txtRegistroPatronal.Text,
                RegimenFiscalId = AyudaCombos.Id(cmbRegimenFiscal) ?? 0,
                RfcPatronOrigen = txtRfcPatronOrigen.Text,
                CodigoPostal = txtCodigoPostal.Text,
                LugarExpedicion = txtLugarExpedicion.Text,
                ClaveEntidadFederativa = AyudaCombos.Clave(cmbEstado),
                ClaveOrigenRecurso = AyudaCombos.ClaveCatalogo(cmbOrigenRecurso),
                MontoRecursoPropio = Validador.DecimalOpcional(txtMontoRecursoPropio.Text, "Monto de recurso propio"),
                RiesgoPuestoId = AyudaCombos.Id(cmbRiesgoPuesto),
                PrimaRiesgoTrabajoPct = Validador.DecimalOpcional(txtPrimaRiesgoTrabajo.Text, "Prima de riesgo de trabajo (%)"),
                Logo = _logo,
                LogoModificado = _logoModificado,
                Activa = chkActiva.Checked
            };
        }

        private void Guardar(string mensajeExito)
        {
            try
            {
                if (!ValidarFormularioEntrada())
                {
                    return;
                }

                _empresaBll.Guardar(LeerFormulario(), _periodicidadesSeleccionadas.Select(p => p.Id).ToList());
                CargarEmpresas();
                LimpiarFormulario();
                Mensajes.Informacion(mensajeExito, "Catálogo de empresas");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empresas");
            }
        }

        private void ActualizarPeriodicidades()
        {
            dgvPeriodicidades.DataSource = _periodicidadesSeleccionadas.ToList();
            dgvPeriodicidades.ClearSelection();
            btnEliminarPeriodicidad.Enabled = false;
            if (_periodicidadesSeleccionadas.Count > 0)
            {
                LimpiarError(dgvPeriodicidades);
            }
        }

        private void btnAgregarPeriodicidad_Click(object sender, EventArgs e)
        {
            ItemCatalogo periodicidad = cmbPeriodicidad.SelectedItem as ItemCatalogo;
            if (periodicidad == null || periodicidad.Id <= 0)
            {
                Mensajes.Advertencia("Seleccione una periodicidad para agregar.", "Catálogo de empresas");
                return;
            }
            if (_periodicidadesSeleccionadas.Any(p => p.Id == periodicidad.Id))
            {
                Mensajes.Advertencia("La periodicidad seleccionada ya está agregada.", "Catálogo de empresas");
                return;
            }

            _periodicidadesSeleccionadas.Add(periodicidad);
            ActualizarPeriodicidades();
            AyudaCombos.SeleccionarId(cmbPeriodicidad, 0);
        }

        private void btnEliminarPeriodicidad_Click(object sender, EventArgs e)
        {
            if (dgvPeriodicidades.SelectedRows.Count == 0)
            {
                Mensajes.Advertencia("Seleccione una periodicidad de la lista para quitarla.", "Catálogo de empresas");
                return;
            }

            ItemCatalogo periodicidad = dgvPeriodicidades.SelectedRows[0].DataBoundItem as ItemCatalogo;
            if (periodicidad != null)
            {
                _periodicidadesSeleccionadas.RemoveAll(p => p.Id == periodicidad.Id);
                ActualizarPeriodicidades();
            }
        }

        private void dgvPeriodicidades_SelectionChanged(object sender, EventArgs e)
        {
            btnEliminarPeriodicidad.Enabled = dgvPeriodicidades.SelectedRows.Count > 0;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar("La empresa se guardó correctamente con sus periodicidades de pago.\n\nSiguiente paso: genere los periodos del ejercicio en 'Periodos y festivos'.");
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Guardar("La empresa se actualizó correctamente.");
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_empresaIdSeleccionada <= 0)
                {
                    Mensajes.Advertencia("Seleccione una empresa de la lista.", "Catálogo de empresas");
                    return;
                }

                if (!Mensajes.Confirmar("La empresa se marcará como inactiva (eliminación lógica). ¿Desea continuar?", "Catálogo de empresas"))
                {
                    return;
                }

                _empresaBll.DesactivarLogico(_empresaIdSeleccionada);
                CargarEmpresas();
                LimpiarFormulario();
                Mensajes.Informacion("La empresa se desactivó correctamente.", "Catálogo de empresas");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empresas");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                txtBuscar.Clear();
                AplicarFiltro();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empresas");
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empresas");
            }
        }

        private void dgvEmpresas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            try
            {
                CatEmpresa empresa = dgvEmpresas.Rows[e.RowIndex].DataBoundItem as CatEmpresa;
                if (empresa == null)
                {
                    return;
                }

                _empresaIdSeleccionada = empresa.EmpresaId;
                txtRazonSocial.Text = empresa.RazonSocial;
                txtNombreComercial.Text = empresa.NombreComercial;
                txtRFC.Text = empresa.RFC;
                txtCURP.Text = empresa.CURP;
                txtRegistroPatronal.Text = empresa.RegistroPatronal;
                AyudaCombos.SeleccionarId(cmbRegimenFiscal, empresa.RegimenFiscalId);
                txtRfcPatronOrigen.Text = empresa.RfcPatronOrigen;
                txtCodigoPostal.Text = empresa.CodigoPostal;
                txtLugarExpedicion.Text = empresa.LugarExpedicion;
                AyudaCombos.SeleccionarClave(cmbEstado, empresa.ClaveEntidadFederativa);
                AyudaCombos.SeleccionarClaveCatalogo(cmbOrigenRecurso, empresa.ClaveOrigenRecurso);
                txtMontoRecursoPropio.Text = empresa.MontoRecursoPropio.HasValue ? empresa.MontoRecursoPropio.Value.ToString("0.00") : string.Empty;
                AyudaCombos.SeleccionarId(cmbRiesgoPuesto, empresa.RiesgoPuestoId);
                txtPrimaRiesgoTrabajo.Text = empresa.PrimaRiesgoTrabajoPct.HasValue ? empresa.PrimaRiesgoTrabajoPct.Value.ToString("0.#####") : string.Empty;
                _periodicidadesSeleccionadas.Clear();
                _periodicidadesSeleccionadas.AddRange(_empresaBll.ListarPeriodicidades(empresa.EmpresaId)
                    .Select(p => new ItemCatalogo
                    {
                        Id = p.PeriodicidadPagoId,
                        Clave = p.PeriodicidadClave,
                        Descripcion = p.PeriodicidadDescripcion
                    }));
                ActualizarPeriodicidades();
                dgvCertificados.DataSource = _empresaBll.ListarCertificados(empresa.EmpresaId);
                EstablecerLogo(_empresaBll.ObtenerLogo(empresa.EmpresaId), false);
                chkActiva.Checked = empresa.Activa;
                ActualizarBotones();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empresas");
            }
        }

        private void btnExaminarCer_Click(object sender, EventArgs e)
        {
            if (ofdCer.ShowDialog(this) == DialogResult.OK)
            {
                txtArchivoCer.Text = ofdCer.FileName;
            }
        }

        private void btnExaminarKey_Click(object sender, EventArgs e)
        {
            if (ofdKey.ShowDialog(this) == DialogResult.OK)
            {
                txtArchivoKey.Text = ofdKey.FileName;
            }
        }

        private void btnCargarCsd_Click(object sender, EventArgs e)
        {
            try
            {
                if (_empresaIdSeleccionada <= 0)
                {
                    Mensajes.Advertencia("Seleccione una empresa de la lista para cargar su certificado.", "Catálogo de empresas");
                    return;
                }
                if (txtArchivoCer.Text.Length == 0 || txtArchivoKey.Text.Length == 0)
                {
                    Mensajes.Advertencia("Seleccione el archivo .cer y el archivo .key.", "Catálogo de empresas");
                    return;
                }

                byte[] cer = File.ReadAllBytes(txtArchivoCer.Text);
                byte[] key = File.ReadAllBytes(txtArchivoKey.Text);
                _empresaBll.CargarCertificado(_empresaIdSeleccionada, cer, key, txtPasswordCsd.Text);

                dgvCertificados.DataSource = _empresaBll.ListarCertificados(_empresaIdSeleccionada);
                LimpiarCamposCsd();
                Mensajes.Informacion("El certificado se cargó correctamente. El certificado anterior, si existía, quedó dado de baja.", "Catálogo de empresas");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empresas");
            }
        }

        private void btnExaminarLogo_Click(object sender, EventArgs e)
        {
            if (ofdLogo.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                EstablecerLogo(PrepararLogo(ofdLogo.FileName), true);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is OutOfMemoryException || ex is IOException)
            {
                Mensajes.Advertencia("El archivo seleccionado no es una imagen válida (use PNG, JPG, BMP o GIF).", "Catálogo de empresas");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empresas");
            }
        }

        private void btnQuitarLogo_Click(object sender, EventArgs e)
        {
            EstablecerLogo(null, true);
        }

        // Reduce la imagen a un máximo de 600 x 240 px y la guarda como PNG para que el recibo no cargue archivos pesados
        private static byte[] PrepararLogo(string ruta)
        {
            const int anchoMax = 600;
            const int altoMax = 240;

            using (Image original = Image.FromFile(ruta))
            {
                double escala = Math.Min(1d, Math.Min((double)anchoMax / original.Width, (double)altoMax / original.Height));
                int ancho = Math.Max(1, (int)Math.Round(original.Width * escala));
                int alto = Math.Max(1, (int)Math.Round(original.Height * escala));

                using (Bitmap reducida = new Bitmap(ancho, alto))
                {
                    using (Graphics g = Graphics.FromImage(reducida))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(original, 0, 0, ancho, alto);
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        reducida.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
        }

        private void EstablecerLogo(byte[] logo, bool modificado)
        {
            _logo = logo;
            _logoModificado = modificado;

            Image anterior = picLogo.Image;
            picLogo.Image = null;
            if (anterior != null)
            {
                anterior.Dispose();
            }

            if (logo != null && logo.Length > 0)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(logo))
                    using (Image imagen = Image.FromStream(ms))
                    {
                        picLogo.Image = new Bitmap(imagen);
                    }
                }
                catch (ArgumentException)
                {
                    // Imagen dañada en la base: se muestra vacío y se puede reemplazar subiendo otro logo.
                }
            }
            btnQuitarLogo.Enabled = logo != null && logo.Length > 0;
        }

        private void LimpiarCamposCsd()
        {
            txtArchivoCer.Clear();
            txtArchivoKey.Clear();
            txtPasswordCsd.Clear();
        }

        private void cmbOrigenRecurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                bool requiereMonto = _empresaBll.RequiereMontoRecursoPropio(AyudaCombos.ClaveCatalogo(cmbOrigenRecurso));
                txtMontoRecursoPropio.Enabled = requiereMonto;
                if (!requiereMonto)
                {
                    txtMontoRecursoPropio.Clear();
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empresas");
            }
        }

        private void dgvEmpresas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CatEmpresa empresa = dgvEmpresas.Rows[e.RowIndex].DataBoundItem as CatEmpresa;
            if (empresa != null && !empresa.Activa)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }
    }
}
