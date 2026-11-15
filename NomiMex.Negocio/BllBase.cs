using System;
using NomiMex.Datos;

namespace NomiMex.Negocio
{
    /// <summary>Base de los servicios de negocio: convierte las excepciones de la capa de datos en ExcepcionNegocio.</summary>
    public abstract class BllBase
    {
        protected static T Ejecutar<T>(Func<T> operacion)
        {
            try
            {
                return operacion();
            }
            catch (ExcepcionDatos ex)
            {
                throw new ExcepcionNegocio(ex.Message, ex, !ex.EsMensajeDeNegocio);
            }
        }

        protected static void Ejecutar(Action operacion)
        {
            Ejecutar<bool>(delegate
            {
                operacion();
                return true;
            });
        }
    }
}
