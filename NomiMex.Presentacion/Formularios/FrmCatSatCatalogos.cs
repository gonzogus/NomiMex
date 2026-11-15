using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    /// <summary>
    /// Módulo de configuración para consultar los catálogos oficiales del SAT y actualizarlos importando los
    /// archivos de Excel que publica la autoridad fiscal. Es un formulario de solo lectura sobre la rejilla: toda
    /// la lectura del Excel y los INSERT/UPDATE masivos los realiza la capa de negocio (NomiMex.Negocio).
    /// </summary>
    public partial class FrmCatSatCatalogos : Form
    {
        private readonly CatalogoSatBLL _bll = new CatalogoSatBLL();
        private bool _cargando;

        public FrmCatSatCatalogos()
        {
            InitializeComponent();
        }

        private void FrmCatSatCatalogos_Load(object sender, EventArgs e)
        {
            try
            {
                btnImportarExcel.Enabled = ControlAcceso.Puede(ModuloSistema.Configuracion, AccionSistema.Escribir);
                CargarCatalogos();
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Catálogos del SAT");
            }
        }

        private void CargarCatalogos()
        {
            _cargando = true;
            cmbCatalogosSat.DataSource = _bll.ListarCatalogos();
            cmbCatalogosSat.DisplayMember = "Nombre";
            cmbCatalogosSat.ValueMember = "Clave";
            cmbCatalogosSat.SelectedIndex = cmbCatalogosSat.Items.Count > 0 ? 0 : -1;
            _cargando = false;
            CargarCatalogoSeleccionado();
        }

        private void cmbCatalogosSat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_cargando)
            {
                CargarCatalogoSeleccionado();
            }
        }

        private void CargarCatalogoSeleccionado()
        {
            CatalogoSatOpcion catalogo = cmbCatalogosSat.SelectedItem as CatalogoSatOpcion;
            if (catalogo == null)
            {
                return;
            }

            try
            {
                dgvCatalogosSat.DataSource = _bll.ObtenerDatos(catalogo.Clave);
                lblConteo.Text = dgvCatalogosSat.Rows.Count + " registros";
            }
            catch (Exception ex)
            {
                dgvCatalogosSat.DataSource = null;
                lblConteo.Text = "0 registros";
                Mensajes.Mostrar(ex, "Catálogos del SAT");
            }
        }

        private void btnImportarExcel_Click(object sender, EventArgs e)
        {
            CatalogoSatOpcion catalogo = cmbCatalogosSat.SelectedItem as CatalogoSatOpcion;
            if (catalogo == null)
            {
                Mensajes.Advertencia("Seleccione primero el catálogo que desea actualizar.", "Catálogos del SAT");
                return;
            }

            if (openFileDialogExcel.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            if (!Mensajes.Confirmar("Se actualizará el catálogo '" + catalogo.Nombre +
                "' con el contenido del archivo seleccionado. ¿Desea continuar?", "Importar actualización"))
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                ResultadoImportacionSat resultado = _bll.ImportarActualizacion(catalogo.Clave, openFileDialogExcel.FileName);
                CargarCatalogoSeleccionado();
                Mensajes.Informacion("La actualización se aplicó correctamente.\n\n" + resultado.Resumen(),
                    "Importar actualización");
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, "Importar actualización");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void dgvCatalogosSat_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || !dgvCatalogosSat.Columns.Contains("Activo"))
            {
                return;
            }

            object valor = dgvCatalogosSat.Rows[e.RowIndex].Cells["Activo"].Value;
            if (valor is bool && !(bool)valor)
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
        }
    }
}
