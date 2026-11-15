using System.Collections.Generic;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Resultado de replicar la configuración de paquetes de un periodo en los demás periodos abiertos de la empresa.</summary>
    public class ResultadoCopiaPaquetes
    {
        public ResultadoCopiaPaquetes()
        {
            Fallos = new List<string>();
        }

        /// <summary>Periodos abiertos de la misma empresa y periodicidad que recibieron la configuración.</summary>
        public int PeriodosActualizados { get; set; }

        /// <summary>Periodos que no pudieron actualizarse, con el motivo.</summary>
        public List<string> Fallos { get; set; }
    }
}
