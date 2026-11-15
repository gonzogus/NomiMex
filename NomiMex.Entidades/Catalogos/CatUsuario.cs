using System;

namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatUsuario. El hash y la sal nunca se cargan hacia la UI.</summary>
    public class CatUsuario
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string NombreMostrar { get; set; }
        public bool Activo { get; set; }
        public bool Bloqueado { get; set; }
        public short IntentosFallidos { get; set; }
        public DateTime? UltimoAccesoUtc { get; set; }
        public bool RequiereCambioPwd { get; set; }
        public DateTime FechaAltaUtc { get; set; }
        public DateTime FechaUpdateUtc { get; set; }

        /// <summary>Roles asignados (RelUsuarioRol), separados por coma. Campo calculado de lectura.</summary>
        public string RolesTexto { get; set; }
    }
}
