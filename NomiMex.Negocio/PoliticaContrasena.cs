using System.Linq;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Reglas de complejidad de la contraseña de NomiMex. El cifrado/hash se realiza siempre en
    /// el servidor (SHA2_512 con sal) mediante los procedimientos sp_Auth_*; aquí sólo se valida
    /// la robustez del texto antes de enviarlo.
    /// </summary>
    public static class PoliticaContrasena
    {
        public const int LongitudMinima = 10;

        /// <summary>
        /// Valida longitud mínima, presencia de mayúsculas y dígitos, que no coincida con la
        /// identidad del usuario y que la confirmación sea igual. Lanza ExcepcionNegocio si falla.
        /// </summary>
        public static void Validar(string identidad, string contrasena, string confirmacion)
        {
            if (string.IsNullOrEmpty(contrasena))
            {
                throw new ExcepcionNegocio("Capture la nueva contraseña.");
            }
            if (contrasena.Length < LongitudMinima)
            {
                throw new ExcepcionNegocio("La contraseña debe tener al menos " + LongitudMinima + " caracteres.");
            }
            if (!contrasena.Any(char.IsUpper))
            {
                throw new ExcepcionNegocio("La contraseña debe incluir al menos una letra mayúscula.");
            }
            if (!contrasena.Any(char.IsLower))
            {
                throw new ExcepcionNegocio("La contraseña debe incluir al menos una letra minúscula.");
            }
            if (!contrasena.Any(char.IsDigit))
            {
                throw new ExcepcionNegocio("La contraseña debe incluir al menos un número.");
            }
            if (!string.IsNullOrEmpty(identidad) &&
                string.Equals(contrasena, identidad, System.StringComparison.OrdinalIgnoreCase))
            {
                throw new ExcepcionNegocio("La contraseña no puede ser igual a su usuario o correo.");
            }
            if (contrasena != confirmacion)
            {
                throw new ExcepcionNegocio("La confirmación de la contraseña no coincide.");
            }
        }
    }
}
