using System;

namespace NomiMex.Entidades.Bitacoras
{
    /// <summary>
    /// Evento de la bitácora de auditoría y errores (dbo.LogSistema).
    /// </summary>
    public class LogSistema
    {
        public long IdLog { get; set; }
        public DateTime FechaHora { get; set; }

        /// <summary>INFO, ADVERTENCIA, ERROR o SEGURIDAD (ver <see cref="NivelLog"/>).</summary>
        public string Nivel { get; set; }

        public string Modulo { get; set; }
        public string Mensaje { get; set; }
        public string DetalleTecnico { get; set; }

        /// <summary>Usuario asociado al evento (FK opcional a dbo.CatUsuario).</summary>
        public int? IdUsuario { get; set; }

        /// <summary>Nombre del usuario resuelto por JOIN para su presentación (no persiste).</summary>
        public string NombreUsuario { get; set; }
    }
}
