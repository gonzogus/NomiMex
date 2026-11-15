using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Relaciones;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    public partial class FrmCatEmpleados : Form
    {
        private readonly EmpleadoBLL _empleadoBll = new EmpleadoBLL();
        private readonly EmpresaBLL _empresaBll = new EmpresaBLL();
        private readonly CatalogoBLL _catalogoBll = new CatalogoBLL();
        private readonly EstadoBLL _estadoBll = new EstadoBLL();
        private readonly PuestoBLL _puestoBll = new PuestoBLL();
        private readonly DepartamentoBLL _departamentoBll = new DepartamentoBLL();
        private readonly ConceptoIncidenciaBLL _incidenciaFijaBll = new ConceptoIncidenciaBLL();
        private int _incidenciaFijaId;
        private bool _baseSeleccionada;

        private List<EmpleadoListado> _empleados = new List<EmpleadoListado>();
        private List<ItemCatalogo> _puestosTodos = new List<ItemCatalogo>();
        private List<ItemCatalogo> _departamentosTodos = new List<ItemCatalogo>();
        private int _empleadoIdSeleccionado;
        private bool _cargandoCombos;

        public FrmCatEmpleados()
        {
            InitializeComponent();
            ConfigurarColumnasEmpleados();
        }

        private void ConfigurarColumnasEmpleados()
        {
            dgvEmpleados.AutoGenerateColumns = false;
            dgvEmpleados.Columns.Clear();
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNumeroEmpleado",
                HeaderText = "No. empleado",
                DataPropertyName = "NumeroEmpleado",
                FillWeight = 15
            });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombreCompleto",
                HeaderText = "Nombre",
                DataPropertyName = "NombreCompleto",
                FillWeight = 30
            });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEmpresa",
                HeaderText = "Empresa",
                DataPropertyName = "Empresa",
                FillWeight = 25
            });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPuesto",
                HeaderText = "Puesto",
                DataPropertyName = "Puesto",
                FillWeight = 20
            });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstatus",
                HeaderText = "Estatus",
                DataPropertyName = "Estatus",
                FillWeight = 10
            });
        }

        private void FrmCatEmpleados_Load(object sender, EventArgs e)
        {
            try
            {
                // Paso 1: sin una empresa activa no se pueden registrar empleados
                new PrerrequisitosNomina().ExigirEmpresaRegistrada();
                CargarCombos();
                CargarEmpleados();
                LimpiarFormulario();
                if (dgvEmpleados.Rows.Count > 0)
                {
                    dgvEmpleados.Rows[0].Selected = true;
                    dgvEmpleados_CellClick(dgvEmpleados, new DataGridViewCellEventArgs(0, 0));
                }
            }
            catch (Exception ex)
            {
                Enabled = false;
                Mensajes.Mostrar(ex, "Catálogo de empleados");
            }
        }

        // La periodicidad del empleado solo puede elegirse entre las asignadas a su empresa
        private void cmbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos)
            {
                return;
            }

            try
            {
                CargarPeriodicidadesEmpresa();
                CargarDepartamentosPuestosEmpresa();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empleados");
            }
        }

        private void CargarPeriodicidadesEmpresa()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            List<ItemCatalogo> periodicidades = _empresaBll.ListarPeriodicidadesItems(empresaId);
            AyudaCombos.CargarCatalogo(cmbTipoPeriodo, periodicidades);
            if (periodicidades.Count == 1)
            {
                AyudaCombos.SeleccionarId(cmbTipoPeriodo, periodicidades[0].Id);
            }
        }

        // Departamentos y puestos dependen de la empresa; se recargan al cambiarla.
        private void CargarDepartamentosPuestosEmpresa()
        {
            int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            _puestosTodos = empresaId > 0
                ? _puestoBll.Listar(empresaId, false)
                    .Select(p => new ItemCatalogo { Id = p.PuestoId, Clave = p.Clave, Descripcion = p.Descripcion + (p.Activo ? string.Empty : " (inactivo)") })
                    .ToList()
                : new List<ItemCatalogo>();
            _departamentosTodos = empresaId > 0
                ? _departamentoBll.Listar(empresaId, false)
                    .Select(d => new ItemCatalogo { Id = d.DepartamentoId, Clave = d.Clave, Descripcion = d.Descripcion + (d.Activo ? string.Empty : " (inactivo)") })
                    .ToList()
                : new List<ItemCatalogo>();
            AyudaCombos.CargarCatalogo(cmbPuesto, _puestosTodos.Where(p => !p.Descripcion.EndsWith("(inactivo)")));
            AyudaCombos.CargarCatalogo(cmbDepartamento, _departamentosTodos.Where(d => !d.Descripcion.EndsWith("(inactivo)")));
        }

        private void CargarCombos()
        {
            _cargandoCombos = true;
            AyudaCombos.CargarCatalogo(cmbEmpresa, _empresaBll.Listar(true)
                .Select(x => new ItemCatalogo { Id = x.EmpresaId, Clave = x.RFC, Descripcion = x.RazonSocial }));
            List<ItemCatalogo> empresasFiltro = new List<ItemCatalogo>
            {
                new ItemCatalogo { Id = 0, Descripcion = "Todas las empresas" }
            };
            empresasFiltro.AddRange(_empresaBll.Listar(false)
                .Select(x => new ItemCatalogo
                {
                    Id = x.EmpresaId,
                    Clave = x.RFC,
                    Descripcion = x.RazonSocial + (x.Activa ? string.Empty : " (inactiva)")
                }));
            cmbFiltroEmpresa.DisplayMember = "Texto";
            cmbFiltroEmpresa.ValueMember = "Id";
            cmbFiltroEmpresa.DataSource = empresasFiltro;
            _cargandoCombos = false;
            AyudaCombos.CargarTextos(cmbEstatus, _catalogoBll.ListarEstatusEmpleado(), false);
            AyudaCombos.CargarTextos(cmbSexo, _catalogoBll.ListarSexos(), true);
            AyudaCombos.CargarTextos(cmbEstadoCivil, _catalogoBll.ListarEstadosCiviles(), true);

            AyudaCombos.CargarCatalogo(cmbRegimenFiscal, _catalogoBll.ListarRegimenesFiscales());
            AyudaCombos.CargarEstados(cmbEstadoFiscal, _estadoBll.Listar(true));

            AyudaCombos.CargarCatalogo(cmbRiesgo, _catalogoBll.ListarRiesgosPuesto());
            AyudaCombos.CargarCatalogo(cmbBanco, _catalogoBll.ListarBancos());

            AyudaCombos.CargarCatalogo(cmbTipoContrato, _catalogoBll.ListarTiposContrato());
            AyudaCombos.CargarCatalogo(cmbTipoRegimen, _catalogoBll.ListarTiposRegimen());
            AyudaCombos.CargarCatalogo(cmbTipoJornada, _catalogoBll.ListarTiposJornada());
            AyudaCombos.CargarCatalogo(cmbTipoPeriodo, new List<ItemCatalogo>());
            AyudaCombos.CargarDiasLaborados(cmbDiasSemana, "Según la jornada");
            AyudaCombos.CargarTextos(cmbZonaSMG, _catalogoBll.ListarZonasSMG(), false);

            _puestosTodos = new List<ItemCatalogo>();
            _departamentosTodos = new List<ItemCatalogo>();
            AyudaCombos.CargarCatalogo(cmbPuesto, _puestosTodos);
            AyudaCombos.CargarCatalogo(cmbDepartamento, _departamentosTodos);
            cmbConceptoFijo.DisplayMember = "Texto";
            cmbConceptoFijo.ValueMember = "ConceptoId";
            cmbConceptoFijo.DataSource = _incidenciaFijaBll.ListarConceptos();
        }

        private void CargarEmpleados()
        {
            _empleados = _empleadoBll.Listar(AyudaCombos.Id(cmbFiltroEmpresa), chkSoloActivos.Checked);
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            List<EmpleadoListado> filtrados = _empleadoBll.Filtrar(_empleados, txtBuscar.Text);
            dgvEmpleados.DataSource = filtrados;
            lblConteo.Text = filtrados.Count + " de " + _empleados.Count + " registros";
        }

        private void LimpiarFormulario()
        {
            _empleadoIdSeleccionado = 0;
            dgvIncidenciasFijas.DataSource = new List<RelEmpleadoIncidencia>();
            LimpiarIncidenciaFija();

            AyudaCombos.SeleccionarId(cmbEmpresa, EmpresaUnicaPreseleccionada());
            txtNumeroEmpleado.Clear();
            AyudaCombos.SeleccionarTexto(cmbEstatus, EstatusEmpleado.Activo);
            txtNombres.Clear();
            txtApellidoPaterno.Clear();
            txtApellidoMaterno.Clear();
            dtpFechaIngreso.Value = DateTime.Today;
            dtpFechaNacimiento.Value = DateTime.Today.AddYears(-30);
            dtpFechaNacimiento.Checked = false;
            AyudaCombos.SeleccionarTexto(cmbSexo, null);
            AyudaCombos.SeleccionarTexto(cmbEstadoCivil, null);
            txtNacionalidad.Text = "MEXICANA";

            txtRFC.Clear();
            txtCURP.Clear();
            AyudaCombos.SeleccionarId(cmbRegimenFiscal, null);
            txtCodigoPostal.Clear();
            AyudaCombos.SeleccionarClave(cmbEstadoFiscal, null);
            txtEmail.Clear();
            txtTelefono.Clear();

            txtNSS.Clear();
            txtSalarioDiario.Clear();
            txtSDI.Clear();
            txtSBC.Clear();
            lblFactor.Text = "Factor de integración: -";
            AyudaCombos.SeleccionarId(cmbRiesgo, null);
            AyudaCombos.SeleccionarId(cmbBanco, null);
            txtCLABE.Clear();

            AyudaCombos.SeleccionarId(cmbTipoContrato, null);
            AyudaCombos.SeleccionarId(cmbTipoRegimen, null);
            AyudaCombos.SeleccionarId(cmbTipoJornada, null);
            AyudaCombos.SeleccionarId(cmbTipoPeriodo, null);
            AyudaCombos.SeleccionarId(cmbDiasSemana, null);
            AyudaCombos.SeleccionarTexto(cmbZonaSMG, "GENERAL");
            dtpFechaInicioContrato.Value = DateTime.Today;
            AyudaCombos.SeleccionarId(cmbDepartamento, null);
            AyudaCombos.SeleccionarId(cmbPuesto, null);

            dgvEmpleados.ClearSelection();
            tabEmpleado.SelectedIndex = 0;
            ActualizarBotones();
            txtNumeroEmpleado.Focus();
        }

        // Si solo existe una empresa activa se preselecciona para agilizar la captura.
        private int? EmpresaUnicaPreseleccionada()
        {
            List<ItemCatalogo> empresas = cmbEmpresa.DataSource as List<ItemCatalogo>;
            if (empresas != null && empresas.Count == 2)
            {
                return empresas[1].Id;
            }
            return null;
        }

        private void ActualizarBotones()
        {
            bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Empleados, AccionSistema.Escribir);
            bool haySeleccion = _empleadoIdSeleccionado > 0;
            btnGuardar.Enabled = puedeEscribir && !haySeleccion;
            btnEditar.Enabled = puedeEscribir && haySeleccion;
            btnEliminar.Enabled = puedeEscribir && haySeleccion;
            btnCalcularSDI.Enabled = puedeEscribir;
            btnCatalogoSAT.Enabled = puedeEscribir && haySeleccion;
            btnGuardarFija.Enabled = puedeEscribir && haySeleccion && !_baseSeleccionada;
            btnDesactivarFija.Enabled = puedeEscribir && haySeleccion && _incidenciaFijaId > 0 && !_baseSeleccionada;
            btnNuevaFija.Enabled = puedeEscribir && haySeleccion;
        }

        private EmpleadoExpediente LeerFormulario()
        {
            EmpleadoExpediente x = new EmpleadoExpediente();

            x.Empleado.EmpleadoId = _empleadoIdSeleccionado;
            x.Empleado.EmpresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
            x.Empleado.NumeroEmpleado = txtNumeroEmpleado.Text;
            x.Empleado.Estatus = AyudaCombos.Texto(cmbEstatus);
            x.Empleado.Nombres = txtNombres.Text;
            x.Empleado.ApellidoPaterno = txtApellidoPaterno.Text;
            x.Empleado.ApellidoMaterno = txtApellidoMaterno.Text;
            x.Empleado.FechaIngreso = dtpFechaIngreso.Value.Date;
            x.Empleado.FechaNacimiento = dtpFechaNacimiento.Checked ? dtpFechaNacimiento.Value.Date : (DateTime?)null;
            x.Empleado.Sexo = AyudaCombos.Texto(cmbSexo);
            x.Empleado.EstadoCivil = AyudaCombos.Texto(cmbEstadoCivil);
            x.Empleado.Nacionalidad = txtNacionalidad.Text;

            x.Fiscales.RFC = txtRFC.Text;
            x.Fiscales.CURP = txtCURP.Text;
            x.Fiscales.RegimenFiscalSAT = AyudaCombos.Id(cmbRegimenFiscal) ?? 0;
            x.Fiscales.CodigoPostal = txtCodigoPostal.Text;
            x.Fiscales.ClaveEntidadFederativa = AyudaCombos.Clave(cmbEstadoFiscal);
            x.Fiscales.Email = txtEmail.Text;
            x.Fiscales.Telefono = txtTelefono.Text;

            x.IMSS.NSS = txtNSS.Text;
            x.IMSS.SalarioDiario = Validador.DecimalRequerido(txtSalarioDiario.Text, "Salario diario");
            x.IMSS.TipoRiesgo = AyudaCombos.Id(cmbRiesgo) ?? 0;

            x.Cuenta.BancoId = AyudaCombos.Id(cmbBanco);
            x.Cuenta.CLABE = txtCLABE.Text;

            x.Contrato.TipoContratoId = AyudaCombos.Id(cmbTipoContrato) ?? 0;
            x.Contrato.TipoRegimenId = AyudaCombos.Id(cmbTipoRegimen) ?? 0;
            x.Contrato.TipoJornadaId = AyudaCombos.Id(cmbTipoJornada) ?? 0;
            x.Contrato.PeriodicidadPagoId = AyudaCombos.Id(cmbTipoPeriodo) ?? 0;
            x.Contrato.ZonaSMG = AyudaCombos.Texto(cmbZonaSMG);
            x.Contrato.DiasLaboradosSemana = AyudaCombos.Id(cmbDiasSemana);
            x.Contrato.FechaInicio = dtpFechaInicioContrato.Value.Date;
            x.Contrato.DepartamentoId = AyudaCombos.Id(cmbDepartamento);
            x.Contrato.PuestoId = AyudaCombos.Id(cmbPuesto);
            return x;
        }

        private void CargarFormulario(EmpleadoExpediente x)
        {
            CatEmpleado e = x.Empleado;
            _empleadoIdSeleccionado = e.EmpleadoId;

            AyudaCombos.SeleccionarId(cmbEmpresa, e.EmpresaId);
            txtNumeroEmpleado.Text = e.NumeroEmpleado;
            AyudaCombos.SeleccionarTexto(cmbEstatus, e.Estatus);
            txtNombres.Text = e.Nombres;
            txtApellidoPaterno.Text = e.ApellidoPaterno;
            txtApellidoMaterno.Text = e.ApellidoMaterno;
            dtpFechaIngreso.Value = AjustarFecha(dtpFechaIngreso, e.FechaIngreso);
            if (e.FechaNacimiento.HasValue)
            {
                dtpFechaNacimiento.Value = AjustarFecha(dtpFechaNacimiento, e.FechaNacimiento.Value);
                dtpFechaNacimiento.Checked = true;
            }
            else
            {
                dtpFechaNacimiento.Checked = false;
            }
            AyudaCombos.SeleccionarTexto(cmbSexo, e.Sexo);
            AyudaCombos.SeleccionarTexto(cmbEstadoCivil, e.EstadoCivil);
            txtNacionalidad.Text = e.Nacionalidad;

            txtRFC.Text = x.Fiscales.RFC;
            txtCURP.Text = x.Fiscales.CURP;
            AyudaCombos.SeleccionarId(cmbRegimenFiscal, x.Fiscales.RegimenFiscalSAT);
            txtCodigoPostal.Text = x.Fiscales.CodigoPostal;
            AyudaCombos.SeleccionarClave(cmbEstadoFiscal, x.Fiscales.ClaveEntidadFederativa);
            txtEmail.Text = x.Fiscales.Email;
            txtTelefono.Text = x.Fiscales.Telefono;

            txtNSS.Text = x.IMSS.NSS;
            txtSalarioDiario.Text = x.IMSS.SalarioDiario > 0m ? x.IMSS.SalarioDiario.ToString("0.00##") : string.Empty;
            txtSDI.Text = x.IMSS.SalarioDiarioIntegrado > 0m ? x.IMSS.SalarioDiarioIntegrado.ToString("N4") : string.Empty;
            txtSBC.Text = x.IMSS.SBC > 0m ? x.IMSS.SBC.ToString("N4") : string.Empty;
            decimal factor = CalculadoraNomina.ObtenerFactorIntegracion(x.IMSS.SalarioDiario, x.IMSS.SalarioDiarioIntegrado);
            lblFactor.Text = "Factor de integración: " + (factor > 0m ? factor.ToString("0.000000") : "-");
            AyudaCombos.SeleccionarId(cmbRiesgo, x.IMSS.TipoRiesgo);
            AyudaCombos.SeleccionarId(cmbBanco, x.Cuenta.BancoId);
            txtCLABE.Text = x.Cuenta.CLABE;

            AyudaCombos.SeleccionarId(cmbTipoContrato, x.Contrato.TipoContratoId);
            AyudaCombos.SeleccionarId(cmbTipoRegimen, x.Contrato.TipoRegimenId);
            AyudaCombos.SeleccionarId(cmbTipoJornada, x.Contrato.TipoJornadaId);
            AyudaCombos.SeleccionarId(cmbTipoPeriodo, x.Contrato.PeriodicidadPagoId);
            AyudaCombos.SeleccionarTexto(cmbZonaSMG, x.Contrato.ZonaSMG);
            AyudaCombos.SeleccionarId(cmbDiasSemana, x.Contrato.DiasLaboradosSemana);
            dtpFechaInicioContrato.Value = x.Contrato.FechaInicio == default(DateTime)
                ? DateTime.Today : AjustarFecha(dtpFechaInicioContrato, x.Contrato.FechaInicio);

            AsegurarElemento(cmbPuesto, _puestosTodos, x.Contrato.PuestoId);
            AsegurarElemento(cmbDepartamento, _departamentosTodos, x.Contrato.DepartamentoId);
            AyudaCombos.SeleccionarId(cmbDepartamento, x.Contrato.DepartamentoId);
            AyudaCombos.SeleccionarId(cmbPuesto, x.Contrato.PuestoId);

            CargarIncidenciasFijas();
            ActualizarBotones();
        }

        private void CargarIncidenciasFijas()
        {
            dgvIncidenciasFijas.DataSource = _incidenciaFijaBll.ListarAsignadas(_empleadoIdSeleccionado);
            dgvIncidenciasFijas.ClearSelection();
            LimpiarIncidenciaFija();
        }

        private void LimpiarIncidenciaFija()
        {
            _incidenciaFijaId = 0;
            _baseSeleccionada = false;
            nudCantidadFija.Value = 1m;
            txtObservacionesFija.Clear();
            chkActivaFija.Checked = true;
            ActualizarBotones();
        }

        private void btnNuevaFija_Click(object sender, EventArgs e)
        {
            dgvIncidenciasFijas.ClearSelection();
            LimpiarIncidenciaFija();
        }

        private void dgvIncidenciasFijas_SelectionChanged(object sender, EventArgs e)
        {
            RelEmpleadoIncidencia r = dgvIncidenciasFijas.SelectedRows.Count == 0 ? null :
                dgvIncidenciasFijas.SelectedRows[0].DataBoundItem as RelEmpleadoIncidencia;
            if (r == null) return;
            // Los conceptos base los calcula el sistema: se muestran pero no se editan ni se quitan.
            _baseSeleccionada = r.EsBase;
            _incidenciaFijaId = r.EsBase ? 0 : r.IdEmpleadoIncidencia;
            if (!r.EsBase)
            {
                cmbConceptoFijo.SelectedValue = r.IdConceptoSAT;
                nudCantidadFija.Value = Math.Min(Math.Max(r.CantidadOValor, nudCantidadFija.Minimum), nudCantidadFija.Maximum);
                txtObservacionesFija.Text = r.Observaciones;
                chkActivaFija.Checked = r.Activo;
            }
            ActualizarBotones();
        }

        private void btnCatalogoSAT_Click(object sender, EventArgs e)
        {
            try
            {
                using (FrmSeleccionarConceptoSAT selector = new FrmSeleccionarConceptoSAT())
                {
                    if (selector.ShowDialog(this) != DialogResult.OK) return;
                    cmbConceptoFijo.DataSource = _incidenciaFijaBll.ListarConceptos();
                    cmbConceptoFijo.SelectedValue = selector.ConceptoSeleccionado.ConceptoId;
                }
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Catálogo SAT"); }
        }

        private void cmbConceptoFijo_SelectedIndexChanged(object sender, EventArgs e)
        {
            CatConcepto concepto = cmbConceptoFijo.SelectedItem as CatConcepto;
            lblCantidadFija.Text = ConceptoIncidenciaBLL.EtiquetaCantidad(concepto);
        }

        private void btnGuardarFija_Click(object sender, EventArgs e)
        {
            try
            {
                CatConcepto c = cmbConceptoFijo.SelectedItem as CatConcepto;
                _incidenciaFijaBll.GuardarAsignada(new RelEmpleadoIncidencia
                {
                    IdEmpleadoIncidencia = _incidenciaFijaId, IdEmpleado = _empleadoIdSeleccionado,
                    IdConceptoSAT = c == null ? 0 : c.ConceptoId, CantidadOValor = nudCantidadFija.Value,
                    EsPermanente = true,
                    Observaciones = txtObservacionesFija.Text, Activo = chkActivaFija.Checked
                });
                CargarIncidenciasFijas();
                Mensajes.Informacion("El concepto asignado se guardó. No modifica periodos ya inicializados.", "Conceptos del empleado");
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Conceptos del empleado"); }
        }

        private void btnDesactivarFija_Click(object sender, EventArgs e)
        {
            try
            {
                RelEmpleadoIncidencia r = dgvIncidenciasFijas.SelectedRows.Count == 0 ? null :
                    dgvIncidenciasFijas.SelectedRows[0].DataBoundItem as RelEmpleadoIncidencia;
                if (r == null) throw new ExcepcionNegocio("Seleccione un concepto asignado.");
                if (!Mensajes.Confirmar("¿Quitar el concepto del empleado? No cambiará periodos ya inicializados.", "Conceptos del empleado")) return;
                _incidenciaFijaBll.DesactivarAsignada(_empleadoIdSeleccionado, r.IdEmpleadoIncidencia);
                CargarIncidenciasFijas();
            }
            catch (Exception ex) { Mensajes.Mostrar(ex, "Conceptos del empleado"); }
        }

        // Si el puesto/departamento del empleado ya está inactivo no aparece en la lista; se agrega para poder mostrarlo.
        private static void AsegurarElemento(ComboBox combo, List<ItemCatalogo> todos, int? id)
        {
            List<ItemCatalogo> actuales = combo.DataSource as List<ItemCatalogo>;
            if (actuales == null || !id.HasValue || actuales.Any(i => i.Id == id.Value))
            {
                return;
            }

            ItemCatalogo faltante = todos.FirstOrDefault(i => i.Id == id.Value);
            if (faltante != null)
            {
                List<ItemCatalogo> nueva = new List<ItemCatalogo>(actuales) { faltante };
                combo.DataSource = nueva;
            }
        }

        private static DateTime AjustarFecha(DateTimePicker picker, DateTime fecha)
        {
            if (fecha < picker.MinDate)
            {
                return picker.MinDate;
            }
            return fecha > picker.MaxDate ? picker.MaxDate : fecha;
        }

        private void Guardar(string mensajeExito)
        {
            try
            {
                EmpleadoExpediente expediente = LeerFormulario();
                _empleadoBll.Guardar(expediente);
                CargarEmpleados();
                LimpiarFormulario();
                Mensajes.Informacion(mensajeExito, "Catálogo de empleados");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empleados");
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar("El empleado se guardó correctamente.");
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Guardar("El empleado se actualizó correctamente.");
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_empleadoIdSeleccionado <= 0)
                {
                    Mensajes.Advertencia("Seleccione un empleado de la lista.", "Catálogo de empleados");
                    return;
                }

                if (!Mensajes.Confirmar("El empleado pasará a estatus BAJA y su contrato vigente se cerrará (eliminación lógica). ¿Desea continuar?",
                        "Catálogo de empleados"))
                {
                    return;
                }

                _empleadoBll.DarDeBaja(_empleadoIdSeleccionado);
                CargarEmpleados();
                LimpiarFormulario();
                Mensajes.Informacion("El empleado se dio de baja correctamente.", "Catálogo de empleados");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empleados");
            }
        }

        private void btnDiasJornada_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialogo = new FrmJornadas())
                {
                    dialogo.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, Text);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            AplicarFiltro();
            LimpiarFormulario();
        }

        private void btnCalcularSDI_Click(object sender, EventArgs e)
        {
            try
            {
                int empresaId = AyudaCombos.Id(cmbEmpresa) ?? 0;
                decimal salarioDiario = Validador.DecimalRequerido(txtSalarioDiario.Text, "Salario diario");
                IntegracionSalarial integracion = _empleadoBll.CalcularIntegracion(empresaId, dtpFechaIngreso.Value, salarioDiario);

                txtSDI.Text = integracion.SDI.ToString("N4");
                txtSBC.Text = integracion.SBC.ToString("N4");
                lblFactor.Text = "Factor de integración: " + integracion.FactorIntegracion.ToString("0.000000") +
                                 "   UMA: " + integracion.UMADiaria.ToString("N2");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Integración salarial");
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
                Mensajes.Mostrar(ex, "Catálogo de empleados");
            }
        }

        private void cmbFiltroEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos)
            {
                return;
            }

            try
            {
                CargarEmpleados();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empleados");
            }
        }

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CargarEmpleados();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empleados");
            }
        }

        private void dgvEmpleados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            try
            {
                EmpleadoListado fila = dgvEmpleados.Rows[e.RowIndex].DataBoundItem as EmpleadoListado;
                if (fila == null)
                {
                    return;
                }
                CargarFormulario(_empleadoBll.ObtenerExpediente(fila.EmpleadoId));
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogo de empleados");
            }
        }

        private void dgvEmpleados_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            EmpleadoListado fila = dgvEmpleados.Rows[e.RowIndex].DataBoundItem as EmpleadoListado;
            if (fila != null && fila.Estatus == EstatusEmpleado.Baja)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }
    }
}
