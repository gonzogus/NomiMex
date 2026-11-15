using System.Collections.Generic;
using System.Windows.Forms;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Presentacion
{
    /// <summary>Utilidades para llenar y leer ComboBox ya declarados en los formularios (no crea controles).</summary>
    internal static class AyudaCombos
    {
        public const string TextoSeleccione = "(Seleccione)";

        public static void CargarCatalogo(ComboBox combo, IEnumerable<ItemCatalogo> elementos)
        {
            List<ItemCatalogo> lista = new List<ItemCatalogo> { new ItemCatalogo { Id = 0, Descripcion = TextoSeleccione } };
            lista.AddRange(elementos);
            combo.DataSource = null;
            combo.DisplayMember = "Texto";
            combo.ValueMember = "Id";
            combo.DataSource = lista;
            combo.SelectedIndex = 0;
        }

        /// <summary>
        /// Días laborados por semana: un primer elemento sin valor (Id 0, que Id() lee como null) con el texto indicado, de 1 a 6 días
        /// y "Jornada completa" (7). Se lee y selecciona con Id y SeleccionarId.
        /// </summary>
        public static void CargarDiasLaborados(ComboBox combo, string textoSinValor)
        {
            List<ItemCatalogo> lista = new List<ItemCatalogo> { new ItemCatalogo { Id = 0, Descripcion = textoSinValor } };
            for (int dias = 1; dias <= 6; dias++)
            {
                lista.Add(new ItemCatalogo { Id = dias, Descripcion = dias + (dias == 1 ? " día por semana" : " días por semana") });
            }
            lista.Add(new ItemCatalogo { Id = 7, Descripcion = "Jornada completa (7 días)" });
            combo.DataSource = null;
            combo.DisplayMember = "Texto";
            combo.ValueMember = "Id";
            combo.DataSource = lista;
            combo.SelectedIndex = 0;
        }

        public static int? Id(ComboBox combo)
        {
            if (combo.SelectedValue is int && (int)combo.SelectedValue > 0)
            {
                return (int)combo.SelectedValue;
            }
            return null;
        }

        public static void SeleccionarId(ComboBox combo, int? id)
        {
            combo.SelectedValue = id.HasValue ? id.Value : 0;
            if (combo.SelectedIndex < 0 && combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }

        /// <summary>Clave SAT del elemento seleccionado de un combo cargado con CargarCatalogo; null si no hay selección.</summary>
        public static string ClaveCatalogo(ComboBox combo)
        {
            ItemCatalogo item = combo.SelectedItem as ItemCatalogo;
            return item == null || item.Id <= 0 ? null : item.Clave;
        }

        public static void SeleccionarClaveCatalogo(ComboBox combo, string clave)
        {
            int indice = 0;
            for (int i = 0; i < combo.Items.Count; i++)
            {
                ItemCatalogo item = combo.Items[i] as ItemCatalogo;
                if (item != null && item.Id > 0 && item.Clave == clave)
                {
                    indice = i;
                    break;
                }
            }
            combo.SelectedIndex = combo.Items.Count > 0 ? indice : -1;
        }

        public static void CargarEstados(ComboBox combo, IEnumerable<CatEntidadFederativa> estados)
        {
            List<CatEntidadFederativa> lista = new List<CatEntidadFederativa>
            {
                new CatEntidadFederativa { CveEntidad = string.Empty, Nombre = TextoSeleccione, Activo = true }
            };
            lista.AddRange(estados);
            combo.DataSource = null;
            combo.DisplayMember = "Nombre";
            combo.ValueMember = "CveEntidad";
            combo.DataSource = lista;
            combo.SelectedIndex = 0;
        }

        public static string Clave(ComboBox combo)
        {
            string clave = combo.SelectedValue as string;
            return string.IsNullOrEmpty(clave) ? null : clave;
        }

        public static void SeleccionarClave(ComboBox combo, string clave)
        {
            combo.SelectedValue = clave ?? string.Empty;
            if (combo.SelectedIndex < 0 && combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }

        public static void CargarTextos(ComboBox combo, IEnumerable<string> valores, bool incluirVacio)
        {
            List<string> lista = new List<string>();
            if (incluirVacio)
            {
                lista.Add(string.Empty);
            }
            lista.AddRange(valores);
            combo.DataSource = null;
            combo.DataSource = lista;
            combo.SelectedIndex = 0;
        }

        public static string Texto(ComboBox combo)
        {
            string texto = combo.SelectedItem as string;
            return string.IsNullOrEmpty(texto) ? null : texto;
        }

        public static void SeleccionarTexto(ComboBox combo, string texto)
        {
            int indice = combo.Items.IndexOf(texto ?? string.Empty);
            combo.SelectedIndex = indice >= 0 ? indice : 0;
        }

        public static void CargarChecks(CheckedListBox lista, IEnumerable<ItemCatalogo> elementos)
        {
            lista.Items.Clear();
            foreach (ItemCatalogo elemento in elementos)
            {
                lista.Items.Add(elemento, false);
            }
        }

        public static List<int> IdsMarcados(CheckedListBox lista)
        {
            List<int> ids = new List<int>();
            foreach (object item in lista.CheckedItems)
            {
                ids.Add(((ItemCatalogo)item).Id);
            }
            return ids;
        }

        public static void MarcarIds(CheckedListBox lista, IEnumerable<int> ids)
        {
            HashSet<int> marcados = new HashSet<int>(ids);
            for (int i = 0; i < lista.Items.Count; i++)
            {
                lista.SetItemChecked(i, marcados.Contains(((ItemCatalogo)lista.Items[i]).Id));
            }
        }

        /// <summary>Llena un ComboBox con los días de la semana ISO (1 = lunes ... 7 = domingo); opcionalmente con una opción vacía.</summary>
        public static void CargarDiasSemana(ComboBox combo, bool incluirVacio)
        {
            List<KeyValuePair<int, string>> dias = new List<KeyValuePair<int, string>>();
            if (incluirVacio)
            {
                dias.Add(new KeyValuePair<int, string>(0, "(Ninguno)"));
            }
            dias.AddRange(NomiMex.Entidades.Constantes.DiaSemanaISO.Todos());
            combo.DataSource = null;
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";
            combo.DataSource = dias;
            combo.SelectedIndex = 0;
        }

        public static byte? Dia(ComboBox combo)
        {
            return combo.SelectedValue is int && (int)combo.SelectedValue > 0 ? (byte?)(int)combo.SelectedValue : null;
        }

        public static void SeleccionarDia(ComboBox combo, byte? dia)
        {
            combo.SelectedValue = dia.HasValue ? (int)dia.Value : 0;
            if (combo.SelectedIndex < 0 && combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }
    }
}
