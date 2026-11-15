using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Lector mínimo de archivos .xlsx (Office Open XML) sin dependencias externas: abre el libro como paquete ZIP,
    /// resuelve la tabla de cadenas compartidas y devuelve la primera hoja como un DataTable de texto, usando el
    /// primer renglón como encabezados. Pensado para los catálogos oficiales que publica el SAT.
    /// </summary>
    internal static class LectorExcelSat
    {
        private const string NsMain = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        public static DataTable LeerPrimeraHoja(string ruta)
        {
            using (FileStream fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                string[] cadenas = LeerCadenasCompartidas(zip);
                string rutaHoja = ResolverPrimeraHoja(zip);
                ZipArchiveEntry hoja = zip.GetEntry(rutaHoja);
                if (hoja == null)
                {
                    throw new ExcepcionNegocio("El archivo de Excel no contiene hojas de datos.");
                }
                return LeerHoja(hoja, cadenas);
            }
        }

        private static string[] LeerCadenasCompartidas(ZipArchive zip)
        {
            ZipArchiveEntry entrada = zip.GetEntry("xl/sharedStrings.xml");
            if (entrada == null)
            {
                return new string[0];
            }

            List<string> cadenas = new List<string>();
            using (Stream s = entrada.Open())
            {
                XDocument doc = XDocument.Load(s);
                foreach (XElement si in doc.Root.Elements())
                {
                    cadenas.Add(TextoDeElemento(si));
                }
            }
            return cadenas.ToArray();
        }

        // Une todos los nodos <t> de un <si>, incluidos los fragmentos con formato enriquecido (<r><t>).
        private static string TextoDeElemento(XElement elemento)
        {
            return string.Concat(elemento.Descendants(XName.Get("t", NsMain)).Select(t => t.Value));
        }

        private static string ResolverPrimeraHoja(ZipArchive zip)
        {
            ZipArchiveEntry libro = zip.GetEntry("xl/workbook.xml");
            ZipArchiveEntry relaciones = zip.GetEntry("xl/_rels/workbook.xml.rels");
            if (libro != null && relaciones != null)
            {
                string idRelacion = PrimerIdDeHoja(libro);
                if (idRelacion != null)
                {
                    string destino = DestinoDeRelacion(relaciones, idRelacion);
                    if (destino != null)
                    {
                        return "xl/" + destino.TrimStart('/').Replace("//", "/");
                    }
                }
            }
            return "xl/worksheets/sheet1.xml";
        }

        private static string PrimerIdDeHoja(ZipArchiveEntry libro)
        {
            using (Stream s = libro.Open())
            {
                XDocument doc = XDocument.Load(s);
                XElement hoja = doc.Descendants(XName.Get("sheet", NsMain)).FirstOrDefault();
                if (hoja == null)
                {
                    return null;
                }
                XAttribute id = hoja.Attributes().FirstOrDefault(a => a.Name.LocalName == "id");
                return id == null ? null : id.Value;
            }
        }

        private static string DestinoDeRelacion(ZipArchiveEntry relaciones, string idRelacion)
        {
            using (Stream s = relaciones.Open())
            {
                XDocument doc = XDocument.Load(s);
                XElement relacion = doc.Root.Elements()
                    .FirstOrDefault(e => (string)e.Attribute("Id") == idRelacion);
                return relacion == null ? null : (string)relacion.Attribute("Target");
            }
        }

        private static DataTable LeerHoja(ZipArchiveEntry hoja, string[] cadenas)
        {
            DataTable tabla = new DataTable();
            using (Stream s = hoja.Open())
            {
                XDocument doc = XDocument.Load(s);
                List<XElement> renglones = doc.Descendants(XName.Get("row", NsMain)).ToList();
                if (renglones.Count == 0)
                {
                    return tabla;
                }

                string[] encabezados = ValoresDeRenglon(renglones[0], cadenas);
                for (int i = 0; i < encabezados.Length; i++)
                {
                    string nombre = string.IsNullOrWhiteSpace(encabezados[i]) ? "Columna" + (i + 1) : encabezados[i].Trim();
                    tabla.Columns.Add(NombreUnico(tabla, nombre));
                }

                for (int r = 1; r < renglones.Count; r++)
                {
                    string[] valores = ValoresDeRenglon(renglones[r], cadenas);
                    if (valores.All(string.IsNullOrWhiteSpace))
                    {
                        continue;
                    }
                    DataRow fila = tabla.NewRow();
                    for (int c = 0; c < tabla.Columns.Count && c < valores.Length; c++)
                    {
                        fila[c] = valores[c] ?? string.Empty;
                    }
                    tabla.Rows.Add(fila);
                }
            }
            return tabla;
        }

        private static string NombreUnico(DataTable tabla, string nombre)
        {
            string candidato = nombre;
            int sufijo = 2;
            while (tabla.Columns.Contains(candidato))
            {
                candidato = nombre + "_" + sufijo++;
            }
            return candidato;
        }

        // Expande las celdas de un renglón respetando su referencia (A, B, C...) para no perder columnas vacías.
        private static string[] ValoresDeRenglon(XElement renglon, string[] cadenas)
        {
            Dictionary<int, string> porColumna = new Dictionary<int, string>();
            int maxima = -1;
            foreach (XElement celda in renglon.Elements(XName.Get("c", NsMain)))
            {
                int indice = IndiceColumna(celda);
                if (indice < 0)
                {
                    continue;
                }
                porColumna[indice] = ValorDeCelda(celda, cadenas);
                if (indice > maxima)
                {
                    maxima = indice;
                }
            }

            string[] valores = new string[maxima + 1];
            for (int i = 0; i <= maxima; i++)
            {
                string valor;
                valores[i] = porColumna.TryGetValue(i, out valor) ? valor : string.Empty;
            }
            return valores;
        }

        private static string ValorDeCelda(XElement celda, string[] cadenas)
        {
            string tipo = (string)celda.Attribute("t");
            if (tipo == "inlineStr")
            {
                return TextoDeElemento(celda);
            }

            XElement valor = celda.Element(XName.Get("v", NsMain));
            if (valor == null)
            {
                return string.Empty;
            }
            if (tipo == "s")
            {
                int indice;
                if (int.TryParse(valor.Value, out indice) && indice >= 0 && indice < cadenas.Length)
                {
                    return cadenas[indice];
                }
                return string.Empty;
            }
            return valor.Value;
        }

        private static int IndiceColumna(XElement celda)
        {
            string referencia = (string)celda.Attribute("r");
            if (string.IsNullOrEmpty(referencia))
            {
                return -1;
            }
            int indice = 0;
            foreach (char c in referencia)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    indice = indice * 26 + (c - 'A' + 1);
                }
                else if (c >= 'a' && c <= 'z')
                {
                    indice = indice * 26 + (c - 'a' + 1);
                }
                else
                {
                    break;
                }
            }
            return indice - 1;
        }
    }
}
