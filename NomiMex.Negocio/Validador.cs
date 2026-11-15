using System.Globalization;
using System.Text.RegularExpressions;

namespace NomiMex.Negocio
{
    /// <summary>Validaciones y normalización de campos obligatorios, formatos (RFC, CURP, NSS, CLABE, CP, correo) y números.</summary>
    public static class Validador
    {
        private static readonly Regex PatronRFC = new Regex(@"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$", RegexOptions.Compiled);
        private static readonly Regex PatronCURP = new Regex(@"^[A-Z]{4}\d{6}[HMX][A-Z]{5}[A-Z0-9]\d$", RegexOptions.Compiled);
        private static readonly Regex PatronNSS = new Regex(@"^\d{11}$", RegexOptions.Compiled);
        private static readonly Regex PatronCP = new Regex(@"^\d{5}$", RegexOptions.Compiled);
        private static readonly Regex PatronCLABE = new Regex(@"^\d{18}$", RegexOptions.Compiled);
        private static readonly Regex PatronCorreo = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$", RegexOptions.Compiled);

        #region Textos obligatorios y opcionales

        public static string Requerido(string valor, string campo, int longitudMaxima)
        {
            string texto = (valor ?? string.Empty).Trim();
            if (texto.Length == 0)
            {
                throw new ExcepcionNegocio("El campo '" + campo + "' es obligatorio.");
            }
            if (texto.Length > longitudMaxima)
            {
                throw new ExcepcionNegocio("El campo '" + campo + "' no puede exceder " + longitudMaxima + " caracteres.");
            }
            return texto;
        }

        /// <summary>Devuelve null cuando el texto está vacío.</summary>
        public static string Opcional(string valor, string campo, int longitudMaxima)
        {
            string texto = (valor ?? string.Empty).Trim();
            if (texto.Length == 0)
            {
                return null;
            }
            if (texto.Length > longitudMaxima)
            {
                throw new ExcepcionNegocio("El campo '" + campo + "' no puede exceder " + longitudMaxima + " caracteres.");
            }
            return texto;
        }

        public static int SeleccionRequerida(int? valor, string campo)
        {
            if (!valor.HasValue || valor.Value <= 0)
            {
                throw new ExcepcionNegocio("Seleccione un valor en el campo '" + campo + "'.");
            }
            return valor.Value;
        }

        #endregion

        #region Formatos mexicanos

        public static bool EsRFCValido(string rfc, bool soloPersonaFisica)
        {
            if (string.IsNullOrWhiteSpace(rfc))
            {
                return false;
            }
            string valor = rfc.Trim().ToUpperInvariant();
            if (!PatronRFC.IsMatch(valor))
            {
                return false;
            }
            if (soloPersonaFisica && valor.Length != 13)
            {
                return false;
            }
            int inicioFecha = valor.Length == 13 ? 4 : 3;
            return FechaAAMMDDValida(valor.Substring(inicioFecha, 6));
        }

        public static bool EsCURPValida(string curp)
        {
            if (string.IsNullOrWhiteSpace(curp))
            {
                return false;
            }
            string valor = curp.Trim().ToUpperInvariant();
            return PatronCURP.IsMatch(valor) && FechaAAMMDDValida(valor.Substring(4, 6));
        }

        public static bool EsNSSValido(string nss)
        {
            return !string.IsNullOrWhiteSpace(nss) && PatronNSS.IsMatch(nss.Trim());
        }

        public static bool EsCodigoPostalValido(string cp)
        {
            return !string.IsNullOrWhiteSpace(cp) && PatronCP.IsMatch(cp.Trim());
        }

        public static bool EsCorreoValido(string correo)
        {
            return !string.IsNullOrWhiteSpace(correo) && PatronCorreo.IsMatch(correo.Trim());
        }

        /// <summary>CLABE interbancaria: 18 dígitos con dígito verificador (ponderación 3-7-1, módulo 10).</summary>
        public static bool EsCLABEValida(string clabe)
        {
            if (string.IsNullOrWhiteSpace(clabe) || !PatronCLABE.IsMatch(clabe.Trim()))
            {
                return false;
            }
            string valor = clabe.Trim();
            int[] pesos = { 3, 7, 1 };
            int suma = 0;
            for (int i = 0; i < 17; i++)
            {
                suma += ((valor[i] - '0') * pesos[i % 3]) % 10;
            }
            int digitoCalculado = (10 - (suma % 10)) % 10;
            return digitoCalculado == (valor[17] - '0');
        }

        public static string RFC(string valor, string campo, bool obligatorio, bool soloPersonaFisica)
        {
            string texto = (valor ?? string.Empty).Trim().ToUpperInvariant();
            if (texto.Length == 0)
            {
                if (obligatorio)
                {
                    throw new ExcepcionNegocio("El campo '" + campo + "' es obligatorio.");
                }
                return null;
            }
            if (!EsRFCValido(texto, soloPersonaFisica))
            {
                throw new ExcepcionNegocio("El " + campo + " no tiene un formato válido" +
                    (soloPersonaFisica ? " (13 caracteres: 4 letras, fecha AAMMDD y homoclave)." : " (12 o 13 caracteres con fecha AAMMDD)."));
            }
            return texto;
        }

        public static string CURP(string valor, string campo, bool obligatorio)
        {
            string texto = (valor ?? string.Empty).Trim().ToUpperInvariant();
            if (texto.Length == 0)
            {
                if (obligatorio)
                {
                    throw new ExcepcionNegocio("El campo '" + campo + "' es obligatorio.");
                }
                return null;
            }
            if (!EsCURPValida(texto))
            {
                throw new ExcepcionNegocio("La " + campo + " no tiene un formato válido (18 caracteres).");
            }
            return texto;
        }

        public static string NSS(string valor, string campo)
        {
            string texto = Requerido(valor, campo, 11);
            if (!EsNSSValido(texto))
            {
                throw new ExcepcionNegocio("El " + campo + " debe contener exactamente 11 dígitos.");
            }
            return texto;
        }

        public static string CodigoPostal(string valor, string campo, bool obligatorio)
        {
            string texto = (valor ?? string.Empty).Trim();
            if (texto.Length == 0)
            {
                if (obligatorio)
                {
                    throw new ExcepcionNegocio("El campo '" + campo + "' es obligatorio.");
                }
                return null;
            }
            if (!EsCodigoPostalValido(texto))
            {
                throw new ExcepcionNegocio("El " + campo + " debe contener exactamente 5 dígitos.");
            }
            return texto;
        }

        public static string Correo(string valor, string campo, bool obligatorio, int longitudMaxima)
        {
            string texto = (valor ?? string.Empty).Trim();
            if (texto.Length == 0)
            {
                if (obligatorio)
                {
                    throw new ExcepcionNegocio("El campo '" + campo + "' es obligatorio.");
                }
                return null;
            }
            if (texto.Length > longitudMaxima || !EsCorreoValido(texto))
            {
                throw new ExcepcionNegocio("El campo '" + campo + "' no tiene un formato de correo válido.");
            }
            return texto;
        }

        #endregion

        #region Números

        public static decimal DecimalRequerido(string texto, string campo)
        {
            decimal? valor = DecimalOpcional(texto, campo);
            if (!valor.HasValue)
            {
                throw new ExcepcionNegocio("El campo '" + campo + "' es obligatorio.");
            }
            return valor.Value;
        }

        /// <summary>Convierte texto a decimal (admite $ y separador de miles). Devuelve null si está vacío.</summary>
        public static decimal? DecimalOpcional(string texto, string campo)
        {
            string limpio = (texto ?? string.Empty).Trim();
            if (limpio.Length == 0)
            {
                return null;
            }

            decimal valor;
            if (!decimal.TryParse(limpio, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out valor))
            {
                throw new ExcepcionNegocio("El campo '" + campo + "' debe ser un número válido.");
            }
            return valor;
        }

        #endregion

        private static bool FechaAAMMDDValida(string aammdd)
        {
            int mes = int.Parse(aammdd.Substring(2, 2), CultureInfo.InvariantCulture);
            int dia = int.Parse(aammdd.Substring(4, 2), CultureInfo.InvariantCulture);
            if (mes < 1 || mes > 12 || dia < 1)
            {
                return false;
            }
            int[] diasPorMes = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            return dia <= diasPorMes[mes - 1];
        }
    }
}
