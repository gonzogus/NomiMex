using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using NomiMex.Entidades.Documentos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Presentacion
{
    /// <summary>
    /// Dibuja el recibo de nómina en una página de impresión. Lo comparten la consulta de recibos y el pre-recibo del
    /// proceso de nómina. Las notas del recibo se imprimen únicamente cuando contienen texto.
    /// </summary>
    internal static class ImpresorRecibo
    {
        public static void Dibujar(PrintPageEventArgs e, ReciboImpresion r)
        {
            Graphics g = e.Graphics;
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;
            float ancho = e.MarginBounds.Width;
            float mitad = (ancho - 20f) / 2f;

            using (Font fTitulo = new Font("Segoe UI", 15f, FontStyle.Bold))
            using (Font fNegrita = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (Font fNormal = new Font("Segoe UI", 9.5f))
            using (Font fPequena = new Font("Segoe UI", 8f))
            using (Font fNeto = new Font("Segoe UI", 13f, FontStyle.Bold))
            using (Brush negro = new SolidBrush(Color.FromArgb(30, 41, 59)))
            using (Brush gris = new SolidBrush(Color.FromArgb(100, 116, 139)))
            using (Brush azul = new SolidBrush(Color.FromArgb(30, 41, 59)))
            using (Brush blanco = new SolidBrush(Color.White))
            using (Pen linea = new Pen(Color.FromArgb(148, 163, 184)))
            using (StringFormat derecha = new StringFormat { Alignment = StringAlignment.Far })
            {
                DibujarLogo(g, r.EmpresaLogo, x + ancho - 180f, y, 180f, 56f);
                g.DrawString(r.Empresa, fTitulo, negro, x, y);
                y += 30f;
                g.DrawString("RFC: " + r.EmpresaRFC + "     Registro patronal: " + r.RegistroPatronal, fNormal, gris, x, y);
                y += 24f;

                g.FillRectangle(azul, x, y, ancho, 24f);
                string periodo = string.Format("RECIBO DE NÓMINA  |  {0} #{1}  |  {2:dd/MM/yyyy} al {3:dd/MM/yyyy}  |  Pago: {4:dd/MM/yyyy}",
                    r.TipoPeriodo, r.NumeroPeriodo, r.FechaInicioPeriodo, r.FechaFinPeriodo, r.FechaPagoPeriodo);
                g.DrawString(periodo, fNegrita, blanco, x + 6f, y + 3f);
                y += 34f;

                float altoEmpleado = 92f;
                g.DrawRectangle(linea, x, y, ancho, altoEmpleado);
                g.DrawString("No. empleado: " + r.NumeroEmpleado, fNormal, negro, x + 8f, y + 6f);
                g.DrawString("Nombre: " + r.NombreEmpleado, fNegrita, negro, x + 8f, y + 26f);
                g.DrawString("RFC: " + r.EmpleadoRFC, fNormal, negro, x + 8f, y + 46f);
                g.DrawString("CURP: " + r.EmpleadoCURP, fNormal, negro, x + 8f, y + 66f);
                float colDer = x + mitad + 20f;
                g.DrawString("NSS: " + r.EmpleadoNSS, fNormal, negro, colDer, y + 6f);
                g.DrawString("Puesto: " + r.Recibo.SnapshotPuesto, fNormal, negro, colDer, y + 26f);
                g.DrawString("Departamento: " + r.Recibo.SnapshotDepartamento, fNormal, negro, colDer, y + 46f);
                g.DrawString(string.Format("Salario diario: {0:C2}   Días pagados: {1:0.##}",
                    r.Recibo.SnapshotSalarioDiario, r.Recibo.SnapshotDiasPagados), fNormal, negro, colDer, y + 66f);
                y += altoEmpleado + 14f;

                List<DocReciboDetalle> percepciones = r.Detalles.Where(d => d.TipoConcepto != "D").ToList();
                List<DocReciboDetalle> deducciones = r.Detalles.Where(d => d.TipoConcepto == "D").ToList();

                float inicioTabla = y;
                DibujarEncabezadoTabla(g, "PERCEPCIONES Y OTROS PAGOS", x, y, mitad, fNegrita, azul, blanco);
                DibujarEncabezadoTabla(g, "DEDUCCIONES", x + mitad + 20f, y, mitad, fNegrita, azul, blanco);
                y += 28f;

                float yIzquierda = y;
                float yDerecha = y;
                foreach (DocReciboDetalle concepto in percepciones)
                {
                    yIzquierda = DibujarConcepto(g, concepto, x, yIzquierda, mitad, fNormal, fPequena, negro, gris, derecha);
                }
                foreach (DocReciboDetalle concepto in deducciones)
                {
                    yDerecha = DibujarConcepto(g, concepto, x + mitad + 20f, yDerecha, mitad, fNormal, fPequena, negro, gris, derecha);
                }
                y = Math.Max(Math.Max(yIzquierda, yDerecha), y + 22f);

                y += 6f;
                g.DrawLine(linea, x, y, x + mitad, y);
                g.DrawLine(linea, x + mitad + 20f, y, x + ancho, y);
                y += 4f;
                g.DrawString("Total percepciones", fNegrita, negro, x, y);
                g.DrawString(r.Recibo.TotalPercepciones.ToString("C2"), fNegrita, negro, new RectangleF(x, y, mitad, 20f), derecha);
                g.DrawString("Total deducciones", fNegrita, negro, x + mitad + 20f, y);
                g.DrawString(r.Recibo.TotalDeducciones.ToString("C2"), fNegrita, negro, new RectangleF(x + mitad + 20f, y, mitad, 20f), derecha);
                y += 30f;

                g.DrawRectangle(linea, x, inicioTabla, mitad, y - inicioTabla - 6f);
                g.DrawRectangle(linea, x + mitad + 20f, inicioTabla, mitad, y - inicioTabla - 6f);

                g.FillRectangle(azul, x + ancho - 300f, y, 300f, 36f);
                g.DrawString("NETO A PAGAR", fNegrita, blanco, x + ancho - 292f, y + 9f);
                g.DrawString(r.Recibo.NetoPagar.ToString("C2"), fNeto, blanco,
                    new RectangleF(x + ancho - 300f, y + 4f, 292f, 28f), derecha);
                y += 50f;

                if (!string.IsNullOrWhiteSpace(r.Recibo.Notas))
                {
                    string notas = r.Recibo.Notas.Trim();
                    g.DrawString("Notas:", fNegrita, negro, x, y);
                    y += 20f;
                    SizeF medida = g.MeasureString(notas, fNormal, (int)ancho);
                    g.DrawString(notas, fNormal, negro, new RectangleF(x, y, ancho, medida.Height + 2f));
                    y += medida.Height + 14f;
                }
                y += 30f;

                g.DrawLine(linea, x + 40f, y, x + 280f, y);
                g.DrawString("Recibí conforme", fPequena, gris, x + 100f, y + 4f);
                g.DrawString("Estatus del recibo: " + r.Recibo.Estatus + "   Periodo: " + r.EstatusPeriodo +
                             "   Calculado: " + r.Recibo.FechaCalculo.ToString("dd/MM/yyyy HH:mm") +
                             "   Impreso: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fPequena, gris, x, e.MarginBounds.Bottom - 14f);
            }

            e.HasMorePages = false;
        }

        // Dibuja el logo ajustado al recuadro, alineado a la derecha y conservando su proporción; ignora imágenes inválidas.
        private static void DibujarLogo(Graphics g, byte[] logo, float x, float y, float anchoMax, float altoMax)
        {
            if (logo == null || logo.Length == 0)
            {
                return;
            }

            try
            {
                using (MemoryStream ms = new MemoryStream(logo))
                using (Image imagen = Image.FromStream(ms))
                {
                    float escala = Math.Min(anchoMax / imagen.Width, altoMax / imagen.Height);
                    float w = imagen.Width * escala;
                    float h = imagen.Height * escala;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imagen, x + anchoMax - w, y, w, h);
                }
            }
            catch (ArgumentException)
            {
            }
        }

        private static void DibujarEncabezadoTabla(Graphics g, string texto, float x, float y, float ancho, Font fuente, Brush fondo, Brush letra)
        {
            g.FillRectangle(fondo, x, y, ancho, 24f);
            g.DrawString(texto, fuente, letra, x + 6f, y + 3f);
        }

        // Dibuja un concepto (clave SAT, descripción con ajuste de línea e importe) y devuelve la nueva posición vertical.
        private static float DibujarConcepto(Graphics g, DocReciboDetalle d, float x, float y, float ancho, Font fuente, Font pequena,
                                             Brush texto, Brush tenue, StringFormat derecha)
        {
            float anchoTexto = ancho - 40f - 80f;
            SizeF medida = g.MeasureString(d.Descripcion, fuente, (int)anchoTexto);
            float alto = Math.Max(22f, medida.Height + 4f);

            g.DrawString(d.ClaveConceptoSAT, pequena, tenue, x + 6f, y + 3f);
            g.DrawString(d.Descripcion, fuente, texto, new RectangleF(x + 40f, y + 1f, anchoTexto, alto));
            g.DrawString(d.ImporteTotal.ToString("N2"), fuente, texto, new RectangleF(x, y + 1f, ancho - 6f, 20f), derecha);
            return y + alto;
        }
    }
}
