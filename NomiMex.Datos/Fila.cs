using System;
using System.Data;

namespace NomiMex.Datos
{
    /// <summary>Conversión segura de columnas DataRow (DBNull y tipos anulables).</summary>
    public static class Fila
    {
        public static T Obtener<T>(DataRow fila, string columna)
        {
            object valor = fila[columna];
            if (valor == null || valor == DBNull.Value)
            {
                return default(T);
            }

            Type destino = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (destino == typeof(byte[]))
            {
                return (T)valor;
            }
            return (T)Convert.ChangeType(valor, destino);
        }
    }
}
