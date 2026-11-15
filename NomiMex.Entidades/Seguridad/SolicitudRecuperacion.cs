using System;

namespace NomiMex.Entidades.Seguridad
{
    /// <summary>
    /// Resultado de generar un código de recuperación: identifica al usuario destino y la
    /// vigencia del código, sin exponer el valor del código (que sólo se envía por correo).
    /// </summary>
    public class SolicitudRecuperacion
    {
        public int UsuarioId { get; set; }
        public string NombreMostrar { get; set; }
        public string Email { get; set; }
        public DateTime ExpiraUtc { get; set; }
    }
}
