using System;

namespace NomiMex.Datos
{
    /// <summary>
    /// Excepción de la capa de datos. Mensaje ya traducido a texto entendible por el usuario.
    /// EsMensajeDeNegocio indica que proviene de un RAISERROR/THROW de un procedimiento almacenado.
    /// </summary>
    public class ExcepcionDatos : Exception
    {
        public ExcepcionDatos(string mensaje, int numero, bool esMensajeDeNegocio, Exception interna)
            : base(mensaje, interna)
        {
            Numero = numero;
            EsMensajeDeNegocio = esMensajeDeNegocio;
        }

        public ExcepcionDatos(string mensaje)
            : this(mensaje, 0, false, null)
        {
        }

        public int Numero { get; private set; }
        public bool EsMensajeDeNegocio { get; private set; }
    }
}
