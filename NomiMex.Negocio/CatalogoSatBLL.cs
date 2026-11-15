using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NomiMex.Datos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Servicios de los catálogos oficiales del SAT: lista los catálogos disponibles, entrega su contenido para
    /// consultarlo y procesa los archivos de Excel publicados por la autoridad fiscal para aplicar los INSERT/UPDATE
    /// masivos. La interfaz solo envía la clave del catálogo y la ruta del archivo; aquí se lee el Excel y se delega
    /// la escritura transaccional a la capa de datos.
    /// </summary>
    public class CatalogoSatBLL : BllBase
    {
        private readonly CatalogoSatDAL _dal = new CatalogoSatDAL();

        private static readonly List<CatalogoSatDescriptor> Catalogos = new List<CatalogoSatDescriptor>
        {
            Definir("REGIMEN_FISCAL", "Régimen Fiscal", "dbo.CatSatRegimenFiscal", "RegimenFiscalId"),
            Definir("TIPO_CONTRATO", "Tipo de Contrato", "dbo.CatSatTipoContrato", "TipoContratoId"),
            Definir("TIPO_JORNADA", "Tipo de Jornada", "dbo.CatSatTipoJornada", "TipoJornadaId"),
            Definir("TIPO_REGIMEN", "Tipo de Régimen", "dbo.CatSatTipoRegimen", "TipoRegimenId"),
            Definir("PERIODICIDAD_PAGO", "Periodicidad de Pago", "dbo.CatSatPeriodicidadPago", "PeriodicidadPagoId"),
            Definir("TIPO_NOMINA", "Tipo de Nómina", "dbo.CatSatTipoNomina", "TipoNominaId"),
            Definir("TIPO_HORAS", "Tipo de Horas", "dbo.CatSatTipoHoras", "TipoHorasId"),
            Definir("ORIGEN_RECURSO", "Origen del Recurso", "dbo.CatSatOrigenRecurso", "OrigenRecursoId"),
            Definir("RIESGO_PUESTO", "Riesgo de Puesto", "dbo.CatSatRiesgoPuesto", "RiesgoPuestoId"),
            Definir("TIPO_INCAPACIDAD", "Tipo de Incapacidad", "dbo.CatSatTipoIncapacidad", "TipoIncapacidadId"),
            Definir("MOTIVO_CANCELACION", "Motivo de Cancelación", "dbo.CatSatMotivoCancelacion", "MotivoCancelacionId"),
            DefinirBanco()
        };

        private static CatalogoSatDescriptor Definir(string clave, string nombre, string tabla, string columnaId)
        {
            return new CatalogoSatDescriptor
            {
                Clave = clave,
                Nombre = nombre,
                Tabla = tabla,
                ColumnaId = columnaId,
                ColumnaClave = "Clave",
                ColumnaDescripcion = "Descripcion",
                TieneVigencia = true,
                TieneActivo = true
            };
        }

        private static CatalogoSatDescriptor DefinirBanco()
        {
            CatalogoSatDescriptor banco = Definir("BANCO", "Banco", "dbo.CatSatBanco", "BancoId");
            banco.ColumnaTextoExtra = "RazonSocial";
            return banco;
        }

        /// <summary>Catálogos del SAT disponibles para consultar y actualizar (para la lista desplegable).</summary>
        public List<CatalogoSatOpcion> ListarCatalogos()
        {
            ControlAcceso.Exigir(ModuloSistema.Configuracion, AccionSistema.Consultar);
            return Catalogos.Select(c => new CatalogoSatOpcion { Clave = c.Clave, Nombre = c.Nombre }).ToList();
        }

        /// <summary>Contenido completo del catálogo indicado para mostrarlo en la rejilla de solo lectura.</summary>
        public DataTable ObtenerDatos(string claveCatalogo)
        {
            ControlAcceso.Exigir(ModuloSistema.Configuracion, AccionSistema.Consultar);
            CatalogoSatDescriptor catalogo = Buscar(claveCatalogo);
            return Ejecutar(() => _dal.Consultar(catalogo));
        }

        /// <summary>
        /// Lee el archivo de Excel del SAT para el catálogo indicado y aplica los INSERT/UPDATE masivos.
        /// Devuelve un resumen con lo insertado, lo actualizado y los renglones omitidos.
        /// </summary>
        public ResultadoImportacionSat ImportarActualizacion(string claveCatalogo, string rutaArchivo)
        {
            ControlAcceso.Exigir(ModuloSistema.Configuracion, AccionSistema.Escribir);
            CatalogoSatDescriptor catalogo = Buscar(claveCatalogo);

            if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
            {
                throw new ExcepcionNegocio("No se encontró el archivo seleccionado.");
            }
            if (!rutaArchivo.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new ExcepcionNegocio("El archivo debe tener extensión .xlsx (formato de Excel publicado por el SAT).");
            }

            DataTable hoja = LeerHoja(rutaArchivo);
            ResultadoImportacionSat resultado = new ResultadoImportacionSat { Catalogo = catalogo.Nombre };
            List<CatalogoSatFila> filas = Mapear(hoja, catalogo, resultado);

            if (filas.Count == 0)
            {
                throw new ExcepcionNegocio("El archivo no contiene renglones válidos para el catálogo " + catalogo.Nombre +
                    ". Verifique que corresponde al catálogo seleccionado.");
            }

            int[] conteos = Ejecutar(() => _dal.ActualizarMasivo(catalogo, filas));
            resultado.Insertados = conteos[0];
            resultado.Actualizados = conteos[1];
            return resultado;
        }

        private static DataTable LeerHoja(string rutaArchivo)
        {
            try
            {
                return LectorExcelSat.LeerPrimeraHoja(rutaArchivo);
            }
            catch (ExcepcionNegocio)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExcepcionNegocio("No fue posible leer el archivo de Excel. Asegúrese de que no esté abierto " +
                    "y que sea un archivo .xlsx válido.", ex, true);
            }
        }

        private static CatalogoSatDescriptor Buscar(string claveCatalogo)
        {
            CatalogoSatDescriptor catalogo = Catalogos.FirstOrDefault(c => c.Clave == claveCatalogo);
            if (catalogo == null)
            {
                throw new ExcepcionNegocio("Catálogo del SAT no reconocido.");
            }
            return catalogo;
        }

        private static List<CatalogoSatFila> Mapear(DataTable hoja, CatalogoSatDescriptor catalogo, ResultadoImportacionSat resultado)
        {
            int colClave = BuscarColumna(hoja, 0, "clave", "codigo", "c_", "cve");
            int colDescripcion = BuscarColumna(hoja, 1, "descripcion", "texto", "nombre");
            int colInicio = BuscarColumna(hoja, -1, "iniciodevigencia", "fechainicio", "iniciovigencia");
            int colFin = BuscarColumna(hoja, -1, "findevigencia", "fechafin", "finvigencia", "terminovigencia");

            List<CatalogoSatFila> filas = new List<CatalogoSatFila>();
            HashSet<string> clavesVistas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow renglon in hoja.Rows)
            {
                resultado.RenglonesLeidos++;
                string clave = Valor(renglon, colClave);
                string descripcion = Valor(renglon, colDescripcion);

                if (string.IsNullOrWhiteSpace(clave) || string.IsNullOrWhiteSpace(descripcion))
                {
                    resultado.Omitidos.Add("Renglón " + resultado.RenglonesLeidos + ": clave o descripción vacía.");
                    continue;
                }
                clave = clave.Trim();
                if (!clavesVistas.Add(clave))
                {
                    continue;
                }

                DateTime? inicio = ParsearFecha(Valor(renglon, colInicio));
                DateTime? fin = ParsearFecha(Valor(renglon, colFin));
                filas.Add(new CatalogoSatFila
                {
                    Clave = clave,
                    Descripcion = descripcion.Trim(),
                    TextoExtra = descripcion.Trim(),
                    InicioVigencia = inicio ?? DateTime.Today,
                    FinVigencia = fin,
                    Activo = !fin.HasValue || fin.Value.Date >= DateTime.Today
                });
            }
            return filas;
        }

        private static string Valor(DataRow renglon, int columna)
        {
            if (columna < 0 || columna >= renglon.Table.Columns.Count)
            {
                return string.Empty;
            }
            object valor = renglon[columna];
            return valor == null || valor == DBNull.Value ? string.Empty : valor.ToString();
        }

        // Busca la columna cuyo encabezado normalizado contenga alguna de las pistas; si no la halla usa el índice por omisión.
        private static int BuscarColumna(DataTable hoja, int porOmision, params string[] pistas)
        {
            for (int i = 0; i < hoja.Columns.Count; i++)
            {
                string encabezado = Normalizar(hoja.Columns[i].ColumnName);
                if (pistas.Any(p => encabezado.Contains(p)))
                {
                    return i;
                }
            }
            return porOmision < hoja.Columns.Count ? porOmision : -1;
        }

        private static string Normalizar(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return string.Empty;
            }
            string sinAcentos = new string(texto.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
            StringBuilder sb = new StringBuilder();
            foreach (char c in sinAcentos.ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static DateTime? ParsearFecha(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return null;
            }
            texto = texto.Trim();

            DateTime fecha;
            if (DateTime.TryParse(texto, CultureInfo.GetCultureInfo("es-MX"), DateTimeStyles.None, out fecha) ||
                DateTime.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                return fecha;
            }

            double serial;
            if (double.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out serial) && serial > 0 && serial < 60000)
            {
                try
                {
                    return DateTime.FromOADate(serial);
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }
            return null;
        }
    }
}
