using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace NomiMex.Negocio
{
    /// <summary>Datos extraídos de un certificado de sello digital (CSD) del SAT.</summary>
    public class DatosCsd
    {
        public string NoCertificado { get; set; }
        public DateTime VigenciaDesde { get; set; }
        public DateTime VigenciaHasta { get; set; }
    }

    /// <summary>Valida un par .cer/.key del SAT: pertenencia a la empresa, vigencia, contraseña y correspondencia de la llave.</summary>
    public static class ValidadorCsd
    {
        public static DatosCsd Validar(byte[] cer, byte[] key, string password, string rfcEmpresa, DateTime hoy)
        {
            if (cer == null || cer.Length == 0)
            {
                throw new ExcepcionNegocio("Seleccione el archivo del certificado (.cer).");
            }
            if (key == null || key.Length == 0)
            {
                throw new ExcepcionNegocio("Seleccione el archivo de la llave privada (.key).");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ExcepcionNegocio("Capture la contraseña de la llave privada.");
            }
            if (password.Length > 200)
            {
                throw new ExcepcionNegocio("La contraseña no puede exceder 200 caracteres.");
            }

            X509Certificate2 certificado = LeerCertificado(cer);
            string noCertificado = NumeroDeCertificado(certificado);

            if (string.IsNullOrEmpty(rfcEmpresa) || certificado.Subject.IndexOf(rfcEmpresa, StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new ExcepcionNegocio("El certificado no pertenece al RFC de la empresa (" + rfcEmpresa + ").");
            }
            if (certificado.NotAfter.Date < hoy.Date)
            {
                throw new ExcepcionNegocio("El certificado venció el " + certificado.NotAfter.ToString("dd/MM/yyyy") + ".");
            }
            if (certificado.NotBefore.Date > hoy.Date)
            {
                throw new ExcepcionNegocio("El certificado aún no es vigente (inicia el " + certificado.NotBefore.ToString("dd/MM/yyyy") + ").");
            }

            VerificarLlave(certificado, key, password);

            return new DatosCsd
            {
                NoCertificado = noCertificado,
                VigenciaDesde = certificado.NotBefore.Date,
                VigenciaHasta = certificado.NotAfter.Date
            };
        }

        private static X509Certificate2 LeerCertificado(byte[] cer)
        {
            try
            {
                return new X509Certificate2(cer);
            }
            catch (CryptographicException)
            {
                throw new ExcepcionNegocio("El archivo .cer no es un certificado válido.");
            }
        }

        // El número de certificado del SAT son 20 dígitos ASCII codificados como el número de serie hexadecimal
        private static string NumeroDeCertificado(X509Certificate2 certificado)
        {
            string hex = certificado.SerialNumber ?? string.Empty;
            if (hex.Length % 2 != 0)
            {
                hex = "0" + hex;
            }

            StringBuilder numero = new StringBuilder();
            for (int i = 0; i < hex.Length; i += 2)
            {
                numero.Append((char)Convert.ToInt32(hex.Substring(i, 2), 16));
            }

            string resultado = numero.ToString();
            if (resultado.Length != 20 || !resultado.All(char.IsDigit))
            {
                throw new ExcepcionNegocio("El certificado no tiene un número de certificado del SAT válido (20 dígitos). ¿Es un CSD?");
            }
            return resultado;
        }

        private static void VerificarLlave(X509Certificate2 certificado, byte[] key, string password)
        {
            RsaPrivateCrtKeyParameters llave;
            try
            {
                llave = PrivateKeyFactory.DecryptKey(password.ToCharArray(), key) as RsaPrivateCrtKeyParameters;
            }
            catch (Exception)
            {
                throw new ExcepcionNegocio("No se pudo abrir la llave privada: la contraseña es incorrecta o el archivo .key no es válido.");
            }
            if (llave == null)
            {
                throw new ExcepcionNegocio("La llave privada no es una llave RSA válida.");
            }

            RSA publica = certificado.GetRSAPublicKey();
            if (publica == null)
            {
                throw new ExcepcionNegocio("El certificado no contiene una llave pública RSA.");
            }
            byte[] modulo = publica.ExportParameters(false).Modulus;
            if (!modulo.SequenceEqual(llave.Modulus.ToByteArrayUnsigned()))
            {
                throw new ExcepcionNegocio("La llave privada (.key) no corresponde al certificado (.cer).");
            }
        }
    }
}
