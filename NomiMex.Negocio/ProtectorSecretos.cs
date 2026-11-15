using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Cifra con AES-256-CBC los secretos que se guardan en la base de datos (contraseña de la llave del CSD).
    /// La clave de 32 bytes NO se guarda en el código ni en la base: se lee del appSetting "CsdClaveCifrado" (Base64).
    /// </summary>
    internal static class ProtectorSecretos
    {
        public const string NombreConfiguracion = "CsdClaveCifrado";

        public static void Cifrar(string texto, out byte[] cifrado, out byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = ObtenerClave();
                aes.GenerateIV();
                iv = aes.IV;
                byte[] datos = Encoding.UTF8.GetBytes(texto);
                using (ICryptoTransform cifrador = aes.CreateEncryptor())
                {
                    cifrado = cifrador.TransformFinalBlock(datos, 0, datos.Length);
                }
            }
        }

        public static string Descifrar(byte[] cifrado, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = ObtenerClave();
                aes.IV = iv;
                using (ICryptoTransform descifrador = aes.CreateDecryptor())
                {
                    return Encoding.UTF8.GetString(descifrador.TransformFinalBlock(cifrado, 0, cifrado.Length));
                }
            }
        }

        private static byte[] ObtenerClave()
        {
            string base64 = ConfigurationManager.AppSettings[NombreConfiguracion];
            byte[] clave = null;
            if (!string.IsNullOrWhiteSpace(base64))
            {
                try
                {
                    clave = Convert.FromBase64String(base64.Trim());
                }
                catch (FormatException)
                {
                    clave = null;
                }
            }

            if (clave == null || clave.Length != 32)
            {
                throw new ExcepcionNegocio(
                    "Falta la clave de cifrado de certificados. Agregue en App.config, dentro de <appSettings>, la entrada " +
                    "<add key=\"" + NombreConfiguracion + "\" value=\"...\" /> con 32 bytes aleatorios en Base64.\n\n" +
                    "Para generarla en PowerShell:\n$b=New-Object byte[] 32;[Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b);[Convert]::ToBase64String($b)\n\n" +
                    "Guarde esa clave en un lugar seguro: sin ella no se pueden descifrar los certificados ya cargados.");
            }
            return clave;
        }
    }
}
