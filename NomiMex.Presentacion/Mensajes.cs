using System;
using System.Windows.Forms;
using NomiMex.Negocio;

namespace NomiMex.Presentacion
{
    /// <summary>Alertas estándar de la aplicación (MessageBox) para errores de negocio, técnicos y confirmaciones.</summary>
    internal static class Mensajes
    {
        public static void Mostrar(Exception ex, string titulo)
        {
            ExcepcionNegocio negocio = ex as ExcepcionNegocio;
            if (negocio != null && !negocio.EsErrorTecnico)
            {
                MessageBox.Show(negocio.Message, titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (negocio != null)
            {
                MessageBox.Show(negocio.Message, titulo + " - Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Informacion(string mensaje, string titulo)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Advertencia(string mensaje, string titulo)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static bool Confirmar(string mensaje, string titulo)
        {
            return MessageBox.Show(mensaje, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                   == DialogResult.Yes;
        }
    }
}
