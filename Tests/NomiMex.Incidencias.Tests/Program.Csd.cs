using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using NomiMex.Datos;
using NomiMex.Negocio;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

// Pruebas de la carga de certificados de sello digital (CSD). Los certificados se fabrican en memoria con RSA de prueba:
// no se usa ningún certificado real del SAT.
internal static partial class Program
{
    private const string ClavePruebaCsd = "AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8=";
    private const string RfcPruebaCsd = "AAA010101AA1";
    private const string PasswordPruebaCsd = "Clave*Prueba 123";

    private static AsymmetricCipherKeyPair _parLlavesA;
    private static AsymmetricCipherKeyPair _parLlavesB;

    private static AsymmetricCipherKeyPair ParLlaves(bool segundo)
    {
        Func<AsymmetricCipherKeyPair> generar = () =>
        {
            var gen = new RsaKeyPairGenerator();
            gen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            return gen.GenerateKeyPair();
        };
        if (segundo) return _parLlavesB ?? (_parLlavesB = generar());
        return _parLlavesA ?? (_parLlavesA = generar());
    }

    // Número de certificado del SAT: 20 dígitos ASCII codificados como número de serie.
    private static void FabricarCsd(string rfc, string noCertificado, DateTime desde, DateTime hasta, string password,
        out byte[] cer, out byte[] key, bool llaveDeOtroPar = false)
    {
        AsymmetricCipherKeyPair par = ParLlaves(false);
        var generador = new X509V3CertificateGenerator();
        var sujeto = new X509Name("CN=EMPRESA PRUEBA SA DE CV, 2.5.4.45=" + rfc + " / HEGT760101HDFRRN09");
        generador.SetSerialNumber(new BigInteger(1, Encoding.ASCII.GetBytes(noCertificado)));
        generador.SetIssuerDN(sujeto);
        generador.SetSubjectDN(sujeto);
        generador.SetNotBefore(desde);
        generador.SetNotAfter(hasta);
        generador.SetPublicKey(par.Public);
        cer = generador.Generate(new Asn1SignatureFactory("SHA256WITHRSA", par.Private)).GetEncoded();

        AsymmetricCipherKeyPair parLlave = ParLlaves(llaveDeOtroPar);
        byte[] sal = new byte[8];
        new SecureRandom().NextBytes(sal);
        key = EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(
            PkcsObjectIdentifiers.PbeWithShaAnd3KeyTripleDesCbc.Id, password.ToCharArray(), sal, 2048, parLlave.Private).GetEncoded();
    }

    private static byte[] Vigente(out byte[] key, string noCertificado = "30001000000500003416", string rfc = RfcPruebaCsd)
    {
        byte[] cer;
        FabricarCsd(rfc, noCertificado, DateTime.Today.AddDays(-30), DateTime.Today.AddDays(365), PasswordPruebaCsd, out cer, out key);
        return cer;
    }

    // ConfigurationManager.AppSettings es de solo lectura; se abre solo durante la prueba para poner la clave de cifrado.
    private static void ClaveCifradoPrueba(string valor)
    {
        NameValueCollection ajustes = ConfigurationManager.AppSettings;
        PropertyInfo soloLectura = typeof(NameObjectCollectionBase).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        soloLectura.SetValue(ajustes, false, null);
        ajustes.Set("CsdClaveCifrado", valor ?? string.Empty);
        soloLectura.SetValue(ajustes, true, null);
    }

    private static void CertificadosCsd()
    {
        Caso("CSD: un par .cer/.key valido devuelve numero de certificado y vigencia", () =>
        {
            byte[] key;
            byte[] cer = Vigente(out key);
            DatosCsd datos = ValidadorCsd.Validar(cer, key, PasswordPruebaCsd, RfcPruebaCsd, DateTime.Today);
            Verificar(datos.NoCertificado == "30001000000500003416", "Numero de certificado incorrecto: " + datos.NoCertificado);
            Verificar(datos.VigenciaDesde == DateTime.Today.AddDays(-30) && datos.VigenciaHasta == DateTime.Today.AddDays(365), "Vigencia incorrecta.");
            Verificar(ValidadorCsd.Validar(cer, key, PasswordPruebaCsd, RfcPruebaCsd.ToLowerInvariant(), DateTime.Today) != null, "El RFC debe compararse sin distinguir mayusculas.");
        });
        Caso("CSD: contrasena incorrecta, llave ajena o archivos invalidos se rechazan", () =>
        {
            byte[] key;
            byte[] cer = Vigente(out key);
            Rechaza(() => ValidadorCsd.Validar(cer, key, "otra contrasena", RfcPruebaCsd, DateTime.Today));
            Rechaza(() => ValidadorCsd.Validar(cer, key, string.Empty, RfcPruebaCsd, DateTime.Today));
            Rechaza(() => ValidadorCsd.Validar(cer, key, new string('x', 201), RfcPruebaCsd, DateTime.Today));
            byte[] cerAjeno, keyAjena;
            FabricarCsd(RfcPruebaCsd, "30001000000500003416", DateTime.Today.AddDays(-30), DateTime.Today.AddDays(365),
                PasswordPruebaCsd, out cerAjeno, out keyAjena, true);
            Rechaza(() => ValidadorCsd.Validar(cerAjeno, keyAjena, PasswordPruebaCsd, RfcPruebaCsd, DateTime.Today));
            Rechaza(() => ValidadorCsd.Validar(null, key, PasswordPruebaCsd, RfcPruebaCsd, DateTime.Today));
            Rechaza(() => ValidadorCsd.Validar(cer, new byte[0], PasswordPruebaCsd, RfcPruebaCsd, DateTime.Today));
            Rechaza(() => ValidadorCsd.Validar(Encoding.ASCII.GetBytes("no soy un certificado"), key, PasswordPruebaCsd, RfcPruebaCsd, DateTime.Today));
            Rechaza(() => ValidadorCsd.Validar(cer, Encoding.ASCII.GetBytes("no soy una llave"), PasswordPruebaCsd, RfcPruebaCsd, DateTime.Today));
        });
        Caso("CSD: RFC distinto, vencido, aun no vigente o sin numero SAT se rechazan", () =>
        {
            byte[] key;
            byte[] cer = Vigente(out key);
            Rechaza(() => ValidadorCsd.Validar(cer, key, PasswordPruebaCsd, "BBB010101BB1", DateTime.Today));
            Rechaza(() => ValidadorCsd.Validar(cer, key, PasswordPruebaCsd, string.Empty, DateTime.Today));
            Rechaza(() => ValidadorCsd.Validar(cer, key, PasswordPruebaCsd, RfcPruebaCsd, DateTime.Today.AddDays(366)));
            Rechaza(() => ValidadorCsd.Validar(cer, key, PasswordPruebaCsd, RfcPruebaCsd, DateTime.Today.AddDays(-31)));
            byte[] cerSerieCorta, keyCorta;
            FabricarCsd(RfcPruebaCsd, "123456", DateTime.Today.AddDays(-30), DateTime.Today.AddDays(365), PasswordPruebaCsd, out cerSerieCorta, out keyCorta);
            Rechaza(() => ValidadorCsd.Validar(cerSerieCorta, keyCorta, PasswordPruebaCsd, RfcPruebaCsd, DateTime.Today));
        });
        Caso("CSD: la contrasena se cifra con AES, con IV propio, y solo se recupera con la clave configurada", () =>
        {
            Type protector = typeof(EmpresaBLL).Assembly.GetType("NomiMex.Negocio.ProtectorSecretos");
            MethodInfo cifrar = protector.GetMethod("Cifrar");
            MethodInfo descifrar = protector.GetMethod("Descifrar");
            Func<string, object[]> cifrarTexto = texto =>
            {
                object[] a = { texto, null, null };
                try { cifrar.Invoke(null, a); }
                catch (TargetInvocationException ex) { throw ex.InnerException; }
                return a;
            };

            ClaveCifradoPrueba(ClavePruebaCsd);
            object[] primero = cifrarTexto(PasswordPruebaCsd);
            object[] segundo = cifrarTexto(PasswordPruebaCsd);
            byte[] cifrado = (byte[])primero[1];
            byte[] iv = (byte[])primero[2];
            Verificar(iv.Length == 16 && cifrado.Length > 0 && cifrado.Length <= 256, "Tamanos de IV o cifrado incorrectos.");
            Verificar(Encoding.UTF8.GetString(cifrado) != PasswordPruebaCsd, "La contrasena quedo sin cifrar.");
            Verificar(!iv.SequenceEqual((byte[])segundo[2]) && !cifrado.SequenceEqual((byte[])segundo[1]), "Cada cifrado debe usar un IV distinto.");
            Verificar((string)descifrar.Invoke(null, new object[] { cifrado, iv }) == PasswordPruebaCsd, "No se recupero la contrasena.");
            var maxima = cifrarTexto(new string('ñ', 100));
            Verificar(((byte[])maxima[1]).Length <= 256, "Una contrasena de 100 caracteres no cabe en PasswordCSD_Encrypted.");

            foreach (string invalida in new[] { null, string.Empty, "no es base64 !!", Convert.ToBase64String(new byte[16]) })
            {
                ClaveCifradoPrueba(invalida);
                Rechaza(() => cifrarTexto(PasswordPruebaCsd));
            }
            ClaveCifradoPrueba(ClavePruebaCsd);
        });
    }

    // Preparación mínima para ejecutar solo la integración de CSD en una base de pruebas aislada.
    private static void IntegracionSoloCsd(string cadena)
    {
        if (!cadena.Contains("Application Name=NomiMexPruebas"))
            throw new InvalidOperationException("Las pruebas integradas exigen 'Application Name=NomiMexPruebas' en la cadena de conexion.");
        ConexionBD.CadenaConexion = cadena;
        AccesoDatos.EjecutarComando("INSERT dbo.CatUsuario(NombreUsuario,Email,NombreMostrar,ContrasenaHash,ContrasenaSalt) " +
            "VALUES('pruebas-csd','pruebas-csd@example.invalid','Pruebas CSD',0x00,0x00)");
        typeof(SesionActual).GetMethod("Iniciar", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[]
        {
            new NomiMex.Entidades.Seguridad.SesionUsuario
            {
                UsuarioId = Numero("SELECT UsuarioId FROM dbo.CatUsuario WHERE NombreUsuario='pruebas-csd'"),
                NombreUsuario = "pruebas-csd",
                Roles = new List<string> { NomiMex.Entidades.Constantes.RolesSistema.SuperAdmin }
            }
        });
        CertificadosCsdIntegracion();
    }

    private static int EmpresaPruebaCsd(string rfc, bool activa)
    {
        object existente = AccesoDatos.Escalar("SELECT EmpresaId FROM dbo.CatEmpresa WHERE RFC=@RFC", AccesoDatos.Parametro("@RFC", rfc));
        if (existente != null) return Convert.ToInt32(existente);
        AccesoDatos.EjecutarComando(
            "INSERT dbo.CatEmpresa(RazonSocial,RFC,RegistroPatronal,RegimenFiscalId,CodigoPostal,LugarExpedicion,Activa) " +
            "VALUES(N'Empresa sintetica CSD',@RFC,'PRUEBACSD',601,'00000','00000',@Activa)",
            AccesoDatos.Parametro("@RFC", rfc), AccesoDatos.Parametro("@Activa", activa));
        return Numero("SELECT EmpresaId FROM dbo.CatEmpresa WHERE RFC='" + rfc + "'");
    }

    private static void CertificadosCsdIntegracion()
    {
        ClaveCifradoPrueba(ClavePruebaCsd);
        var bll = new EmpresaBLL();
        int empresa = EmpresaPruebaCsd(RfcPruebaCsd, true);
        int inactiva = EmpresaPruebaCsd("CCC010101CC1", false);
        string primero = "30001000000500003416";
        string segundo = "30001000000500003417";

        Caso("CSD: se guarda el certificado con la contrasena cifrada y la llave tal cual", () =>
        {
            byte[] key;
            byte[] cer = Vigente(out key, primero);
            bll.CargarCertificado(empresa, cer, key, PasswordPruebaCsd);
            var activos = bll.ListarCertificados(empresa);
            Verificar(activos.Count == 1 && activos[0].NoCertificado == primero && activos[0].Activo, "El certificado no quedo activo.");
            Verificar(activos[0].VigenciaHasta == DateTime.Today.AddDays(365), "Vigencia guardada incorrecta.");
            byte[] guardadaLlave = (byte[])AccesoDatos.Escalar("SELECT LlaveCSD FROM dbo.HistEmpresaCSD WHERE NoCertificado=@N", AccesoDatos.Parametro("@N", primero));
            byte[] guardadaCer = (byte[])AccesoDatos.Escalar("SELECT CertificadoCSD FROM dbo.HistEmpresaCSD WHERE NoCertificado=@N", AccesoDatos.Parametro("@N", primero));
            byte[] password = (byte[])AccesoDatos.Escalar("SELECT PasswordCSD_Encrypted FROM dbo.HistEmpresaCSD WHERE NoCertificado=@N", AccesoDatos.Parametro("@N", primero));
            byte[] iv = (byte[])AccesoDatos.Escalar("SELECT PasswordCSD_IV FROM dbo.HistEmpresaCSD WHERE NoCertificado=@N", AccesoDatos.Parametro("@N", primero));
            Verificar(guardadaLlave.SequenceEqual(key) && guardadaCer.SequenceEqual(cer), "Los archivos guardados no coinciden con los cargados.");
            Verificar(iv.Length == 16 && !Encoding.UTF8.GetString(password).Contains(PasswordPruebaCsd), "La contrasena no se guardo cifrada.");
            string recuperada = (string)typeof(EmpresaBLL).Assembly.GetType("NomiMex.Negocio.ProtectorSecretos")
                .GetMethod("Descifrar").Invoke(null, new object[] { password, iv });
            Verificar(recuperada == PasswordPruebaCsd, "La contrasena guardada no se puede descifrar.");
        });
        Caso("CSD: un certificado nuevo da de baja al anterior y solo queda uno activo", () =>
        {
            byte[] key;
            byte[] cer = Vigente(out key, segundo);
            bll.CargarCertificado(empresa, cer, key, PasswordPruebaCsd);
            var activos = bll.ListarCertificados(empresa);
            Verificar(activos.Count == 1 && activos[0].NoCertificado == segundo, "Debe quedar activo solo el certificado nuevo.");
            Verificar(Numero("SELECT COUNT(*) FROM dbo.HistEmpresaCSD WHERE EmpresaId=" + empresa) == 2, "El historial debe conservar ambos certificados.");
            Verificar(Numero("SELECT COUNT(*) FROM dbo.HistEmpresaCSD WHERE NoCertificado='" + primero + "' AND Activo=0 AND FechaBaja IS NOT NULL AND MotivoBaja IS NOT NULL") == 1,
                "El certificado anterior no quedo dado de baja con fecha y motivo.");
        });
        Caso("CSD: un rechazo no altera el certificado activo (transaccional)", () =>
        {
            byte[] key;
            byte[] repetido = Vigente(out key, primero);
            Rechaza(() => bll.CargarCertificado(empresa, repetido, key, PasswordPruebaCsd));
            byte[] cerOk = Vigente(out key, segundo);
            Rechaza(() => bll.CargarCertificado(empresa, cerOk, key, "contrasena equivocada"));
            byte[] cerOtroRfc = Vigente(out key, "30001000000500003420", "BBB010101BB1");
            Rechaza(() => bll.CargarCertificado(empresa, cerOtroRfc, key, PasswordPruebaCsd));
            var activos = bll.ListarCertificados(empresa);
            Verificar(activos.Count == 1 && activos[0].NoCertificado == segundo, "El certificado activo cambio tras un rechazo.");
            Verificar(Numero("SELECT COUNT(*) FROM dbo.HistEmpresaCSD WHERE EmpresaId=" + empresa + " AND Activo=1") == 1, "Debe haber exactamente un certificado activo.");
        });
        Caso("CSD: sin empresa seleccionada, empresa inactiva o sin clave de cifrado no se guarda nada", () =>
        {
            byte[] key;
            byte[] cer = Vigente(out key, "30001000000500003418", "CCC010101CC1");
            Rechaza(() => bll.CargarCertificado(0, cer, key, PasswordPruebaCsd));
            Rechaza(() => bll.CargarCertificado(inactiva, cer, key, PasswordPruebaCsd));
            Rechaza(() => bll.CargarCertificado(987654, cer, key, PasswordPruebaCsd));
            byte[] cerEmpresa = Vigente(out key, "30001000000500003419");
            ClaveCifradoPrueba(null);
            Rechaza(() => bll.CargarCertificado(empresa, cerEmpresa, key, PasswordPruebaCsd));
            ClaveCifradoPrueba(ClavePruebaCsd);
            Verificar(Numero("SELECT COUNT(*) FROM dbo.HistEmpresaCSD WHERE NoCertificado IN ('30001000000500003418','30001000000500003419')") == 0,
                "No debio guardarse ningun certificado.");
            Verificar(bll.ListarCertificados(0).Count == 0, "Sin empresa no hay certificados.");
        });
    }
}
