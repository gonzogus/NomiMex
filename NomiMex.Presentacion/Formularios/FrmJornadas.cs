using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;

namespace NomiMex.Presentacion.Formularios
{
    /// <summary>
    /// Días que se laboran por semana en cada tipo de jornada. Los empleados con esa jornada los heredan (salvo que tengan
    /// un valor fijo propio o un ajuste del periodo); "Jornada completa" no recorta los días del periodo.
    /// </summary>
    internal sealed partial class FrmJornadas : Form
    {
        private const string TituloDialogo = "Días por jornada";
        private readonly CatalogoBLL _bll = new CatalogoBLL();
        private List<JornadaDias> _jornadas = new List<JornadaDias>();
        private Dictionary<int, int> _originales = new Dictionary<int, int>();

        public FrmJornadas()
        {
            InitializeComponent();
            _colDias.DataSource = OpcionesDias();
            bool puedeEscribir = ControlAcceso.Puede(ModuloSistema.Empleados, AccionSistema.Escribir);
            _colDias.ReadOnly = !puedeEscribir;
            btnGuardar.Enabled = puedeEscribir;
        }

        private static List<ItemCatalogo> OpcionesDias()
        {
            List<ItemCatalogo> lista = new List<ItemCatalogo> { new ItemCatalogo { Id = 0, Descripcion = "Jornada completa (sin definir)" } };
            for (int dias = 1; dias <= 6; dias++)
            {
                lista.Add(new ItemCatalogo { Id = dias, Descripcion = dias + (dias == 1 ? " día por semana" : " días por semana") });
            }
            return lista;
        }

        private void FrmJornadas_Load(object sender, EventArgs e)
        {
            Cargar();
        }

        private void grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void Cargar()
        {
            try
            {
                _jornadas = _bll.ListarJornadasConDias();
                _originales = _jornadas.ToDictionary(j => j.TipoJornadaId, j => j.DiasSemana);
                _grid.DataSource = _jornadas;
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, TituloDialogo);
            }
        }

        private void Guardar_Click(object sender, EventArgs e)
        {
            try
            {
                _grid.EndEdit();
                List<JornadaDias> cambios = _jornadas.Where(j => _originales[j.TipoJornadaId] != j.DiasSemana).ToList();
                foreach (JornadaDias j in cambios)
                {
                    _bll.GuardarDiasJornada(j.TipoJornadaId, j.DiasSemana);
                }
                Cargar();
                Mensajes.Informacion(cambios.Count == 0 ? "No hay cambios por guardar." : "Días por jornada guardados (" + cambios.Count + ").", TituloDialogo);
            }
            catch (Exception ex)
            {
                Mensajes.Mostrar(ex, TituloDialogo);
            }
        }
    }
}
