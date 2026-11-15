using System;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Error de regla de negocio o de acceso a datos ya traducido a un mensaje para el usuario.
    /// EsErrorTecnico distingue una falla de infraestructura (conexión, tiempo de espera) de una validación.
    /// </summary>
    public class ExcepcionNegocio : Exception
    {
        public ExcepcionNegocio(string mensaje)
            : base(mensaje)
        {
        }

        public ExcepcionNegocio(string mensaje, Exception interna, bool esErrorTecnico)
            : base(mensaje, interna)
        {
            EsErrorTecnico = esErrorTecnico;
        }

        public bool EsErrorTecnico { get; private set; }
    }
}
