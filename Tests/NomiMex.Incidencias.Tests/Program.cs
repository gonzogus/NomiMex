using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Procesos;
using NomiMex.Entidades.Relaciones;
using NomiMex.Entidades.Seguridad;
using NomiMex.Entidades.Vistas;
using NomiMex.Negocio;
using NomiMex.Presentacion.Formularios;

internal static partial class Program
{
    private static int _casos;

    private static void Verificar(bool condicion, string mensaje)
    {
        if (!condicion) throw new InvalidOperationException(mensaje);
    }

    private static void Caso(string nombre, Action prueba)
    {
        prueba();
        Console.WriteLine("OK: " + nombre);
        _casos++;
    }

    private static void Rechaza(Action operacion)
    {
        try { operacion(); }
        catch (ExcepcionNegocio) { return; }
        throw new InvalidOperationException("Se esperaba una validacion de negocio.");
    }

    private static void RechazaDatos(Action operacion)
    {
        try { operacion(); }
        catch (ExcepcionDatos) { return; }
        throw new InvalidOperationException("Se esperaba un rechazo transaccional.");
    }

    // Concepto SAT sintético capturable como incidencia.
    private static CatConcepto Concepto(string forma, decimal factor = 1m, string tipo = "P", bool gravado = true)
    {
        return new CatConcepto
        {
            ConceptoId = 1, ClaveSAT = tipo == "P" ? "038" : "004", TipoConcepto = tipo,
            Descripcion = "Concepto de prueba", FormaCalculo = forma, FactorMultiplicador = factor,
            EsGravado = gravado, EsIncidencia = true, Activo = true
        };
    }

    private static void Calculos()
    {
        Caso("Cuatro formas de calculo y redondeo a centavos", () =>
        {
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("MONTO_FIJO"), 1.0050m, 300m) == 1.01m, "Redondeo incorrecto.");
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("DIAS", 2m), 1.5m, 300m) == 900m, "Dias incorrectos.");
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("HORAS", 2m), 3m, 300m) == 225m, "Horas incorrectas.");
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("PORCENTAJE_SD"), 25m, 300m) == 75m, "Porcentaje incorrecto.");
        });
        Caso("Sueldo base: horas usan las horas del turno y los dias, el sueldo diario, ambos por el factor del concepto", () =>
        {
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("HORAS", 2m), 3m, 300m, 8m) == 225m, "Horas dobles en turno de 8 h.");
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("HORAS", 3m), 2m, 300m, 10m) == 180m, "Horas triples en turno de 10 h.");
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("HORAS", 2m), 3m, 300m, 7m) == 257.14m, "Horas en turno de 7 h.");
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("HORAS", 2m), 3m, 300m, 0m) == 225m, "Sin horas de turno se usa la jornada legal de 8 h.");
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("DIAS", 2m), 1m, 300m, 10m) == 600m, "Descanso laborado: sueldo diario x 2.");
            Verificar(ConceptoIncidenciaBLL.CalcularImporte(Concepto("DIAS", 2m), 1.5m, 300m) == 900m, "El calculo sin horas debe seguir igual.");
            string texto = ConceptoIncidenciaBLL.DescribirCalculo(Concepto("HORAS", 2m), 3m, 300m, 8m);
            Verificar(texto.Contains(225m.ToString("C2")) && texto.Contains(37.5m.ToString("C2")), "La operacion mostrada no coincide con el importe.");
            Verificar(ConceptoIncidenciaBLL.DescribirCalculo(Concepto("DIAS", 2m), 0m, 300m, 8m) == string.Empty, "Sin cantidad valida no hay operacion.");
        });
        Caso("Las horas extra del pre-recibo usan las horas del turno del trabajador", () =>
        {
            var periodo = new PeriodoResumen { DiasPeriodo = 15, FechaInicio = new DateTime(2026, 11, 1), FechaFin = new DateTime(2026, 11, 15) };
            var par = new ParametrosCalculoNomina { UMADiaria = 100, SalarioMinimoGeneral = 100, FactorHoraExtraDoble = 2m, FactorHoraExtraTriple = 3m };
            par.TablaISR.Add(new TramoISR { LimiteInferior = 0, LimiteSuperior = 999999, TasaExcedente = 0.1m });
            var sinHoras = new DatosEmpleadoCalculo { SalarioDiario = 300, SBC = 315 };
            var turno10 = new DatosEmpleadoCalculo { SalarioDiario = 300, SBC = 315, HorasJornada = 10m };
            var horas = new IncidenciasEmpleado { HorasExtraDobles = 2m, HorasExtraTriples = 1m };
            decimal base8 = CalculadoraPreRecibo.Calcular(periodo, par, sinHoras, null).TotalPercepciones;
            decimal con8 = CalculadoraPreRecibo.Calcular(periodo, par, sinHoras, horas).TotalPercepciones - base8;
            decimal con10 = CalculadoraPreRecibo.Calcular(periodo, par, turno10, horas).TotalPercepciones -
                            CalculadoraPreRecibo.Calcular(periodo, par, turno10, null).TotalPercepciones;
            Verificar(con8 == 262.5m, "Con 8 h: 37.50 x (2x2 + 1x3) = 262.50, se obtuvo " + con8);
            Verificar(con10 == 210m, "Con turno de 10 h: 30 x (2x2 + 1x3) = 210, se obtuvo " + con10);
        });
        Caso("Dias laborados por semana del contrato limitan el sueldo y el septimo dia proporcional", () =>
        {
            var semana = new PeriodoResumen { DiasPeriodo = 7, FechaInicio = new DateTime(2026, 11, 2), FechaFin = new DateTime(2026, 11, 8), DiaDescanso = 7, CalculaSeptimoDia = true };
            var par = new ParametrosCalculoNomina { UMADiaria = 100, SalarioMinimoGeneral = 100 };
            par.TablaISR.Add(new TramoISR { LimiteInferior = 0, LimiteSuperior = 999999, TasaExcedente = 0.1m });
            var completa = CalculadoraPreRecibo.Calcular(semana, par, new DatosEmpleadoCalculo { SalarioDiario = 300, SBC = 315 }, null);
            var cuatro = CalculadoraPreRecibo.Calcular(semana, par, new DatosEmpleadoCalculo { SalarioDiario = 300, SBC = 315, DiasLaboradosSemana = 4 }, null);
            Verificar(completa.DiasPagados == 7m, "Jornada completa: 6 laborables + 1 de septimo, se obtuvo " + completa.DiasPagados);
            Verificar(cuatro.Percepciones[0].Cantidad == 4m && cuatro.DiasSeptimo == Math.Round(4m / 6m, 4) && cuatro.DiasSemana == 4,
                "Con 4 dias/semana se pagan 4 dias y 4/6 de septimo.");
            var faltas = CalculadoraPreRecibo.Calcular(semana, par, new DatosEmpleadoCalculo { SalarioDiario = 300, SBC = 315, DiasLaboradosSemana = 4 },
                new IncidenciasEmpleado { DiasDescontados = 1m });
            Verificar(faltas.Percepciones[0].Cantidad == 3m, "Las faltas se descuentan de los dias contratados.");
            var quincena = new PeriodoResumen { DiasPeriodo = 15, FechaInicio = new DateTime(2026, 11, 1), FechaFin = new DateTime(2026, 11, 15) };
            var q = CalculadoraPreRecibo.Calcular(quincena, par, new DatosEmpleadoCalculo { SalarioDiario = 300, SBC = 315, DiasLaboradosSemana = 7 }, null);
            Verificar(q.DiasPagados == 15m, "7 dias por semana equivale a jornada completa.");
            Verificar(Math.Abs(CalculadoraNomina.CalcularDiasPagados(7, 1, 0m, 4).DiasPagados - cuatro.DiasPagados) < CalculadoraNomina.ToleranciaDias,
                "La conciliacion debe usar la misma regla.");
        });
        Caso("Conceptos legales omitidos por la empresa salen del recibo y activan la bandera que impide timbrar", () =>
        {
            var periodo = new PeriodoResumen { DiasPeriodo = 15, FechaInicio = new DateTime(2026, 11, 1), FechaFin = new DateTime(2026, 11, 15) };
            var par = new ParametrosCalculoNomina { UMADiaria = 100, SalarioMinimoGeneral = 100 };
            par.TablaISR.Add(new TramoISR { LimiteInferior = 0, LimiteSuperior = 999999, TasaExcedente = 0.1m });
            par.CuotasIMSS.Add(new CuotaIMSSObrera { ClaveConcepto = "IV", PorcentajeObrero = 0.01m, BaseCalculo = "SBC" });
            var empleado = new DatosEmpleadoCalculo { SalarioDiario = 300, SBC = 315 };
            Func<string, string, ConceptoPeriodoPaquete> omitido = (tipo, clave) =>
                new ConceptoPeriodoPaquete { TipoConcepto = tipo, ClaveSAT = clave, Descripcion = "x", EsBase = true, EsLegal = true, Omitido = true };

            var normal = CalculadoraPreRecibo.Calcular(periodo, par, empleado, null);
            Verificar(!normal.BloqueaTimbrado && normal.Deducciones.Any(d => d.Clave == "002") && normal.Deducciones.Any(d => d.Clave == "001"),
                "Sin omisiones deben existir ISR e IMSS.");

            var sinIsrNiImss = CalculadoraPreRecibo.Calcular(periodo, par, empleado, null, null,
                new[] { omitido("D", "002"), omitido("D", "001") });
            Verificar(sinIsrNiImss.BloqueaTimbrado, "Omitir conceptos legales debe activar la bandera.");
            Verificar(!sinIsrNiImss.Deducciones.Any(d => d.Clave == "002" || d.Clave == "001"), "ISR e IMSS omitidos no deben aparecer.");
            Verificar(sinIsrNiImss.NetoPagar == sinIsrNiImss.TotalPercepciones, "Sin deducciones el neto es igual a las percepciones.");

            var sinSueldo = CalculadoraPreRecibo.Calcular(periodo, par, empleado, null, null, new[] { omitido("P", "001") });
            Verificar(sinSueldo.BloqueaTimbrado && !sinSueldo.Percepciones.Any(p => p.Clave == "001"), "El sueldo omitido no debe pagarse.");
        });
        Caso("Entradas invalidas no generan importes ni desbordamientos sin validar", () =>
        {
            Rechaza(() => ConceptoIncidenciaBLL.CalcularImporte(Concepto("DIAS"), 0m, 300m));
            Rechaza(() => ConceptoIncidenciaBLL.CalcularImporte(Concepto("DIAS"), 1m, 0m));
            Rechaza(() => ConceptoIncidenciaBLL.CalcularImporte(Concepto("OTRA"), 1m, 300m));
            Rechaza(() => ConceptoIncidenciaBLL.CalcularImporte(Concepto("MONTO_FIJO"), 0.0001m, 300m));
            Rechaza(() => ConceptoIncidenciaBLL.CalcularImporte(Concepto("MONTO_FIJO"), 1.00001m, 300m));
            Rechaza(() => ConceptoIncidenciaBLL.CalcularImporte(Concepto("DIAS", 999999m), 99999999999999m, decimal.MaxValue));
        });
        Caso("Incidencias legales se clasifican por percepcion o deduccion con su clave SAT de timbrado", () =>
        {
            Func<string, bool, CatTipoIncidencia> tipo = (codigo, he) => new CatTipoIncidencia { Codigo = codigo, GeneraHorasExtra = he };
            foreach (var p in new[] { tipo("HE_DOBLE", true), tipo("PRIMA_DOM", false), tipo("DIAS_FESTIVOS", false), tipo("BONO", false) })
                Verificar(ConceptoIncidenciaBLL.TipoMovimientoLegal(p) == "P", "Debe ser percepcion: " + p.Codigo);
            foreach (var d in new[] { tipo("FALTA", false), tipo("RETARDO", false), tipo("PERMISO_SG", false), tipo("DESCUENTO", false) })
                Verificar(ConceptoIncidenciaBLL.TipoMovimientoLegal(d) == "D", "Debe ser deduccion: " + d.Codigo);
            Verificar(ConceptoIncidenciaBLL.ClaveSATLegal(tipo("HE_TRIPLE", true)) == "019" &&
                ConceptoIncidenciaBLL.ClaveSATLegal(tipo("PRIMA_DOM", false)) == "020" &&
                ConceptoIncidenciaBLL.ClaveSATLegal(tipo("BONO", false)) == "038" &&
                ConceptoIncidenciaBLL.ClaveSATLegal(tipo("DESCUENTO", false)) == "004" &&
                ConceptoIncidenciaBLL.ClaveSATLegal(tipo("FALTA", false)) == null, "Clave SAT legal incorrecta.");
            decimal gravado, exento;
            ConceptoIncidenciaBLL.DesgloseFiscal(Concepto("MONTO_FIJO", 1, "P", false), 80m, out gravado, out exento);
            Verificar(gravado == 0m && exento == 80m, "Percepcion exenta mal desglosada.");
            ConceptoIncidenciaBLL.DesgloseFiscal(Concepto("MONTO_FIJO", 1, "D", false), 25m, out gravado, out exento);
            Verificar(gravado == 25m && exento == 0m, "Deduccion mal desglosada.");
        });
        Caso("Incidencias gravadas y exentas participan en ISR, totales, alcance e identidad eliminable", () =>
        {
            var periodo = new PeriodoResumen { DiasPeriodo = 15, FechaInicio = new DateTime(2026, 11, 1), FechaFin = new DateTime(2026, 11, 15) };
            var par = new ParametrosCalculoNomina { UMADiaria = 100, SalarioMinimoGeneral = 100 };
            par.TablaISR.Add(new TramoISR { LimiteInferior = 0, LimiteSuperior = 999999, TasaExcedente = 0.1m });
            var empleado = new DatosEmpleadoCalculo { SalarioDiario = 300, SBC = 315 };
            PreReciboDTO antes = CalculadoraPreRecibo.Calcular(periodo, par, empleado, null);
            var inc = new[]
            {
                new ProcIncidenciaConcepto { IdIncidencia = 11, CantidadOValor = 200, ImporteCalculado = 200, EsPermanente = true, Concepto = Concepto("MONTO_FIJO") },
                new ProcIncidenciaConcepto { IdIncidencia = 12, CantidadOValor = 50, ImporteCalculado = 50, Concepto = Concepto("MONTO_FIJO", 1, "P", false) },
                new ProcIncidenciaConcepto { IdIncidencia = 13, CantidadOValor = 25, ImporteCalculado = 25, Concepto = Concepto("MONTO_FIJO", 1, "D", false) }
            };
            PreReciboDTO despues = CalculadoraPreRecibo.Calcular(periodo, par, empleado, null, inc);
            Verificar(despues.TotalPercepciones - antes.TotalPercepciones == 250m, "Percepciones incorrectas.");
            Verificar(despues.TotalDeducciones - antes.TotalDeducciones == 45m, "ISR o deducciones incorrectas.");
            Verificar(despues.TotalGravado - antes.TotalGravado == 200m && despues.TotalExento == 50m, "Base fiscal incorrecta.");
            var permanente = despues.Percepciones.Single(c => c.IdIncidenciaConcepto == 11);
            Verificar(permanente.EsPermanente && permanente.AlcanceTexto == "Permanente", "Alcance permanente perdido.");
            var soloPeriodo = despues.Percepciones.Single(c => c.IdIncidenciaConcepto == 12);
            Verificar(!soloPeriodo.EsPermanente && soloPeriodo.AlcanceTexto == "Solo periodo", "Alcance solo periodo perdido.");
            Verificar(despues.Percepciones.First().IdIncidenciaConcepto == null && despues.Percepciones.First().AlcanceTexto == string.Empty,
                "Sueldo no debe ser eliminable ni tener alcance.");
            inc[0].Concepto.ClaveSAT = "036";
            despues = CalculadoraPreRecibo.Calcular(periodo, par, empleado, null, inc);
            Verificar(despues.Percepciones.Single(c => c.IdIncidenciaConcepto == 11).Clave == "036", "Se uso una clave SAT generica.");
            inc[0].Concepto.EsIncidencia = false;
            Rechaza(() => CalculadoraPreRecibo.Calcular(periodo, par, empleado, null, inc));
        });
    }

    private static Control ControlDe(Control raiz, string nombre)
    {
        return raiz.Controls.Find(nombre, true).Single();
    }

    private static void Disenadores()
    {
        Caso("Formularios construibles; casilla permanente, columna de alcance y menus operativos", () =>
        {
            using (var nomina = new FrmProcNomina())
            using (var empleados = new FrmCatEmpleados())
            using (var modal = new FrmSeleccionarConceptoSAT(new PreReciboDTO(), new PeriodoResumen(), "P"))
            using (var paquetes = new FrmCatPaquetes())
            using (var selector = new FrmSeleccionarConceptoSAT())
            {
                ControlDe(paquetes, "dgvPaquetes");
                ControlDe(paquetes, "dgvConceptosPaquete");
                ControlDe(paquetes, "btnAgregarConcepto");
                ControlDe(paquetes, "btnQuitarConcepto");
                ControlDe(selector, "dgvCatalogo");
                ControlDe(selector, "cmbFormaCalculo");
                ControlDe(empleados, "btnCatalogoSAT");
                var grid = (DataGridView)ControlDe(nomina, "dgvPercepciones");
                var deducciones = (DataGridView)ControlDe(nomina, "dgvDeducciones");
                Verificar(grid.ContextMenuStrip != null && grid.ContextMenuStrip == deducciones.ContextMenuStrip, "Menus no asignados.");
                Verificar(grid.Columns.Cast<DataGridViewColumn>().Any(c => c.DataPropertyName == "AlcanceTexto"), "Falta columna de alcance en percepciones.");
                Verificar(deducciones.Columns.Cast<DataGridViewColumn>().Any(c => c.DataPropertyName == "AlcanceTexto"), "Falta columna de alcance en deducciones.");
                ControlDe(nomina, "btnAgregarPercepcion");
                ControlDe(nomina, "btnAgregarDeduccion");
                ControlDe(empleados, "tabIncidenciasFijas");
                ControlDe(modal, "chkPermanente");
                Verificar(modal.FormBorderStyle == FormBorderStyle.FixedDialog && !modal.MaximizeBox && !modal.MinimizeBox &&
                    modal.StartPosition == FormStartPosition.CenterParent, "Modal incorrecto.");
                var tabs = (TabControl)ControlDe(modal, "tabCaptura");
                Verificar(tabs.TabPages.ContainsKey("tabLegales"), "La captura debe conservar las incidencias legales.");
                var tabsSelector = (TabControl)ControlDe(selector, "tabCaptura");
                Verificar(!tabsSelector.TabPages.ContainsKey("tabLegales"), "El selector solo debe mostrar el catalogo.");
                Verificar(selector.ClientSize.Height < modal.ClientSize.Height, "El selector debe ser mas pequeno que la captura.");
            }
        });
        Caso("Tablas de nomina: solo columnas de usuario, encabezados legibles y desplazamiento en paneles estrechos", () =>
        {
            using (var nomina = new FrmProcNomina())
            {
                typeof(FrmProcNomina).GetField("_cargando", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(nomina, true);
                var periodos = (DataGridView)ControlDe(nomina, "dgvPeriodosFiscal");
                var percepciones = (DataGridView)ControlDe(nomina, "dgvPercepciones");
                var deducciones = (DataGridView)ControlDe(nomina, "dgvDeducciones");
                VerificarTablaNomina(periodos, new List<PeriodoResumen>
                {
                    new PeriodoResumen { NumeroPeriodo = 46, FechaInicio = new DateTime(2026, 11, 8),
                        FechaFin = new DateTime(2026, 11, 14), Estatus = EstatusPeriodo.Cerrado }
                }, new[] { "NumeroPeriodo", "FechaInicio", "FechaFin", "EstatusGeneral" });
                Verificar(periodos.Rows[0].Cells["colPeriodoInicio"].FormattedValue.ToString() == "08/11/2026",
                    "La fecha de inicio no usa el formato legible esperado.");
                Verificar(periodos.Rows[0].Cells["colPeriodoEstatus"].Value.ToString() == "Cerrado",
                    "El periodo no muestra su estatus resumido.");

                var conceptos = new List<PreReciboConceptoDTO>
                {
                    new PreReciboConceptoDTO { Clave = "001", Descripcion = "Sueldo ordinario del periodo",
                        Cantidad = 6, ImporteGravado = 1890.24m, ImporteExento = 0 }
                };
                string[] campos = { "Clave", "Descripcion", "CantidadTexto", "ImporteGravado", "ImporteExento", "Total", "AlcanceTexto" };
                VerificarTablaNomina(percepciones, conceptos, campos);
                VerificarTablaNomina(deducciones, conceptos, campos);
                foreach (var tabla in new[] { percepciones, deducciones })
                {
                    var total = tabla.Columns.Cast<DataGridViewColumn>().Single(c => c.DataPropertyName == "Total");
                    Verificar(tabla.Rows[0].Cells[total.Index].FormattedValue.ToString() == 1890.24m.ToString("N2"),
                        "El importe no conserva dos decimales.");
                    tabla.DataSource = new List<PreReciboConceptoDTO>();
                    tabla.DataSource = conceptos;
                    Verificar(tabla.Columns.Count == campos.Length, "Volver a enlazar el recibo genero columnas internas.");
                }
            }
        });
    }

    private static void VerificarTablaNomina(DataGridView tabla, object datos, string[] campos)
    {
        using (var host = new Form())
        {
            host.ShowInTaskbar = false;
            host.StartPosition = FormStartPosition.Manual;
            host.Location = new System.Drawing.Point(-32000, -32000);
            host.Controls.Add(tabla);
            tabla.Dock = DockStyle.Fill;
            tabla.DataSource = datos;
            host.Show();
            Verificar(tabla.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName).SequenceEqual(campos),
                "La tabla muestra campos internos o tiene columnas fuera de orden.");
            Verificar(tabla.Rows.Count == 1, "No se enlazo la fila de prueba.");
            foreach (int ancho in new[] { 300, 750 })
            {
                host.ClientSize = new System.Drawing.Size(ancho, 240);
                host.PerformLayout();
                foreach (DataGridViewColumn columna in tabla.Columns)
                {
                    int anchoTitulo = TextRenderer.MeasureText(columna.HeaderText, tabla.ColumnHeadersDefaultCellStyle.Font).Width;
                    Verificar(columna.Width >= anchoTitulo + 20,
                        "Encabezado comprimido: " + columna.HeaderText + " (" + columna.Width + " < " + (anchoTitulo + 20) + ").");
                }
                if (tabla.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) > tabla.ClientSize.Width)
                    Verificar(tabla.Controls.OfType<HScrollBar>().Any(b => b.Visible), "Falta desplazamiento horizontal.");
            }
            host.Controls.Remove(tabla);
        }
    }

    private static int Numero(string sql) { return Convert.ToInt32(AccesoDatos.Escalar(sql)); }

    private static int ConceptoSAT(string claveSAT, string tipo)
    {
        return Numero("SELECT ConceptoId FROM dbo.CatConcepto WHERE ClaveSAT='" + claveSAT + "' AND TipoConcepto='" + tipo + "'");
    }

    private static void Integracion(string cadena)
    {
        // Resguardo: las pruebas mutan datos. Exigimos el marcador de aplicacion de pruebas en la cadena.
        if (!cadena.Contains("Application Name=NomiMexPruebas"))
            throw new InvalidOperationException("Las pruebas integradas exigen 'Application Name=NomiMexPruebas' en la cadena de conexion.");
        ConexionBD.CadenaConexion = cadena;
        AccesoDatos.EjecutarComando("INSERT dbo.CatUsuario(NombreUsuario,Email,NombreMostrar,ContrasenaHash,ContrasenaSalt) " +
            "VALUES('pruebas','pruebas@example.invalid','Pruebas sinteticas',0x00,0x00)");
        typeof(SesionActual).GetMethod("Iniciar", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[]
        {
            new SesionUsuario { UsuarioId = Numero("SELECT UsuarioId FROM dbo.CatUsuario WHERE NombreUsuario='pruebas'"),
                NombreUsuario = "pruebas", Roles = new List<string> { RolesSistema.SuperAdmin } }
        });

        var bll = new ConceptoIncidenciaBLL();
        var dal = new ConceptoIncidenciaDAL();
        var preBll = new PreNominaBLL();
        int empleado = Numero("SELECT EmpleadoId FROM dbo.CatEmpleado WHERE NumeroEmpleado='PRUEBA1'");
        int otroEmpleado = Numero("SELECT EmpleadoId FROM dbo.CatEmpleado WHERE NumeroEmpleado='PRUEBA3'");
        int empresa = Numero("SELECT EmpresaId FROM dbo.CatEmpleado WHERE NumeroEmpleado='PRUEBA3'");
        int tipoQui = Numero("SELECT PeriodicidadPagoId FROM dbo.CatPeriodicidadPago WHERE Clave='04'");
        int p1 = Numero("SELECT PeriodoId FROM dbo.ProcPeriodoNomina WHERE NumeroPeriodo=1");
        int p2 = Numero("SELECT PeriodoId FROM dbo.ProcPeriodoNomina WHERE NumeroPeriodo=2");
        int p3 = Numero("SELECT PeriodoId FROM dbo.ProcPeriodoNomina WHERE NumeroPeriodo=3");
        int c036 = ConceptoSAT("036", "P");
        int c038 = ConceptoSAT("038", "P");
        int c004 = ConceptoSAT("004", "D");

        Caso("Solo se ofrecen conceptos SAT capturables como incidencia, con su regla de calculo", () =>
        {
            var conceptos = bll.ListarConceptos();
            Verificar(conceptos.All(c => c.EsIncidencia && !string.IsNullOrEmpty(c.ClaveSAT) && (c.TipoConcepto == "P" || c.TipoConcepto == "D")),
                "El catalogo ofrece conceptos no capturables.");
            Verificar(conceptos.Any(c => c.ClaveSAT == "036" && c.TipoConcepto == "P" && c.FormaCalculo == "MONTO_FIJO"), "Falta el apoyo 036.");
            Verificar(conceptos.Any(c => c.ClaveSAT == "004" && c.TipoConcepto == "D"), "Falta la deduccion 004.");
        });
        Caso("El paquete base se precarga al alta con los conceptos base y no se duplica", () =>
        {
            Verificar(bll.ListarAsignadas(otroEmpleado).Count == 0, "El empleado no deberia tener conceptos aun.");
            new ConceptoIncidenciaBLL().AsignarPaqueteDefault(otroEmpleado, empresa, tipoQui);
            var asignadas = bll.ListarAsignadas(otroEmpleado);
            Verificar(asignadas.Count == 4 && asignadas.All(a => a.EsBase && a.Activo && a.EsPermanente), "El paquete base no se precargo completo.");
            Verificar(asignadas.Count(a => a.Concepto.StartsWith("001 - ")) == 2 && asignadas.Count(a => a.Concepto.StartsWith("002 - ")) == 2,
                "Faltan sueldo (001), IMSS (001), ISR (002) o subsidio (002).");
            Verificar(asignadas.All(a => a.EsPermanenteTexto == "Base (automático)" && a.CantidadTexto == "Automático"), "Texto de concepto base incorrecto.");
            new ConceptoIncidenciaBLL().AsignarPaqueteDefault(otroEmpleado, empresa, tipoQui);
            Verificar(bll.ListarAsignadas(otroEmpleado).Count == 4, "El paquete base se duplico.");
        });
        Caso("El calculo SQL no omite la precarga pendiente de conceptos permanentes", () =>
        {
            RechazaDatos(() => AccesoDatos.EjecutarSP("dbo.sp_CalcularNomina", AccesoDatos.Parametro("@PeriodoId", p1)));
            Verificar(Numero("SELECT COUNT(*) FROM dbo.DocReciboNomina") == 0, "El calculo directo omitio la precarga.");
        });
        Caso("Precarga unica e idempotente de los conceptos permanentes del empleado", () =>
        {
            Task.WaitAll(Task.Run(() => bll.Inicializar(p1)), Task.Run(() => bll.Inicializar(p1)));
            List<ProcIncidenciaConcepto> mov = dal.ListarPeriodo(p1);
            Verificar(mov.Count == 2 && mov.All(i => i.IdEmpleado == empleado && i.EsPermanente), "Precarga o concurrencia incorrecta.");
            Verificar(mov.Any(i => i.IdConceptoSAT == c036 && i.ImporteCalculado == 100m) &&
                      mov.Any(i => i.IdConceptoSAT == c038 && i.ImporteCalculado == 200m), "Importe precargado incorrecto.");
        });
        Caso("Captura solo-periodo no toca la configuracion permanente; captura permanente la sincroniza", () =>
        {
            bll.Inicializar(p2);
            int permanentesAntes = bll.ListarAsignadas(empleado).Count(a => a.Activo);
            bll.Registrar(p2, empleado, c038, 10m, "Eventual", false);
            var eventual = dal.ListarPeriodo(p2).Single(i => i.Observaciones == "Eventual");
            Verificar(!eventual.EsPermanente, "La captura eventual quedo marcada como permanente.");
            Verificar(bll.ListarAsignadas(empleado).Count(a => a.Activo) == permanentesAntes, "La captura eventual altero la config permanente.");
            bll.Registrar(p2, empleado, c004, 15m, "Permanente nuevo", true);
            var perm = dal.ListarPeriodo(p2).Single(i => i.Observaciones == "Permanente nuevo");
            Verificar(perm.EsPermanente, "La captura permanente no se marco.");
            Verificar(bll.ListarAsignadas(empleado).Any(a => a.IdConceptoSAT == c004 && a.Activo && a.EsPermanente),
                "La captura permanente no se sincronizo con RelEmpleadoIncidencia.");
        });
        Caso("Quitar un permanente permite elegir solo-periodo o tambien la configuracion permanente", () =>
        {
            var solo = dal.ListarPeriodo(p2).Single(i => i.Observaciones == "Permanente nuevo");
            bll.Quitar(p2, empleado, solo.IdIncidencia, true);
            Verificar(dal.ListarPeriodo(p2).All(i => i.IdIncidencia != solo.IdIncidencia), "No se quito del periodo.");
            Verificar(bll.ListarAsignadas(empleado).Single(a => a.IdConceptoSAT == c004).Activo == false,
                "No se desactivo la configuracion permanente.");
        });
        Caso("Las incidencias respetan la pertenencia de empresa, periodicidad y periodo abierto", () =>
        {
            Rechaza(() => bll.Registrar(p2, otroEmpleado, c036, 1m, null, false));
            Rechaza(() => bll.Registrar(p2, empleado, c036, 1m, new string('X', 251), false));
        });
        Caso("El calculo SQL persiste las claves SAT y concilia con el pre-recibo de Negocio", () =>
        {
            PreReciboDTO previo = preBll.ObtenerPreRecibo(p1, empleado, null);
            var resultado = new NominaBLL().EjecutarCalculo(p1);
            Verificar(resultado.TotalRecibos >= 1 && resultado.Advertencias.Count == 0, "Calculo SQL no concilia: " + string.Join("; ", resultado.Advertencias));
            Verificar(resultado.TotalPercepciones == previo.TotalPercepciones && resultado.TotalDeducciones == previo.TotalDeducciones,
                "SQL y Negocio difieren.");
            Verificar(Numero("SELECT COUNT(*) FROM dbo.DocReciboDetalle d JOIN dbo.DocReciboNomina r ON r.ReciboId=d.ReciboId " +
                "WHERE r.PeriodoId=" + p1 + " AND d.EsIncidencia=1 AND d.ClaveConceptoSAT IN ('036','038')") == 2,
                "El calculo SQL no persistio las claves SAT de las incidencias.");
        });
        Caso("Un periodo calculado bloquea la captura hasta reabrirlo", () =>
        {
            Rechaza(() => bll.Registrar(p1, empleado, c036, 1m, null, false));
        });
        Caso("Las incidencias legales viven en ProcIncidencias/CatConcepto y el calculo SQL concilia con el pre-recibo", () =>
        {
            Verificar(Numero("SELECT COUNT(*) FROM sys.tables WHERE name IN ('ProcIncidencia','CatTipoIncidencia')") == 0,
                "Siguen existiendo las tablas duplicadas ProcIncidencia o CatTipoIncidencia.");
            var tipos = new CatalogoBLL().ListarTiposIncidencia();
            Func<string, CatTipoIncidencia> tipo = codigo => tipos.Single(t => t.Codigo == codigo);
            Verificar(tipos.Count == 11 && tipo("FALTA").DisminuyeDias && tipo("HE_DOBLE").GeneraHorasExtra && !tipo("BONO").GeneraHorasExtra,
                "El catalogo de incidencias legales no se leyo de CatConcepto.");
            Verificar(Numero("SELECT TipoHorasId FROM dbo.CatConcepto WHERE ClaveSAT='HE_DOBLE' AND TipoConcepto='I'") == 1 &&
                      Numero("SELECT TipoHorasId FROM dbo.CatConcepto WHERE ClaveSAT='HE_TRIPLE' AND TipoConcepto='I'") == 2,
                "Las horas extra no apuntan a c_TipoHoras.");

            var incidencias = new IncidenciaBLL();
            DateTime fecha = Convert.ToDateTime(AccesoDatos.Escalar("SELECT FechaInicio FROM dbo.ProcPeriodoNomina WHERE PeriodoId=" + p3));
            bll.Inicializar(p3);
            incidencias.Registrar(p3, empleado, tipo("FALTA"), 1m, UnidadIncidencia.Dia, fecha, "Falta de prueba");
            incidencias.Registrar(p3, empleado, tipo("HE_DOBLE"), 3m, UnidadIncidencia.Hora, fecha, null);
            incidencias.Registrar(p3, empleado, tipo("PRIMA_DOM"), 1m, UnidadIncidencia.Dia, fecha, null);
            incidencias.Registrar(p3, empleado, tipo("BONO"), 500m, UnidadIncidencia.Monto, fecha, null);
            incidencias.Registrar(p3, empleado, tipo("DESCUENTO"), 50m, UnidadIncidencia.Monto, fecha, null);
            incidencias.Registrar(p3, empleado, tipo("RETARDO"), 2m, UnidadIncidencia.Hora, fecha, "Cancelable");
            var listado = incidencias.ListarPorPeriodo(p3);
            Verificar(listado.Count == 6 && listado.Single(i => i.Codigo == "HE_DOBLE").Unidad == UnidadIncidencia.Hora &&
                      listado.Single(i => i.Codigo == "BONO").Unidad == UnidadIncidencia.Monto &&
                      listado.Single(i => i.Codigo == "FALTA").Unidad == UnidadIncidencia.Dia, "El listado de incidencias legales es incorrecto.");
            Verificar(dal.ListarPeriodo(p3).All(i => i.Concepto.TipoConcepto != "I"), "Las incidencias legales se mezclaron con los conceptos SAT.");

            incidencias.Cancelar(p3, listado.Single(i => i.Codigo == "RETARDO").IncidenciaId);
            Verificar(incidencias.ListarPorPeriodo(p3).Single(i => i.Codigo == "RETARDO").Estatus == "CANCELADA", "La incidencia no se cancelo.");
            Verificar(preBll.ObtenerPreRecibo(p3, empleado, null).Incidencias.Count == 5, "El pre-recibo no excluye la incidencia cancelada.");

            PreReciboDTO previo = preBll.ObtenerPreRecibo(p3, empleado, null);
            var resultado = new NominaBLL().EjecutarCalculo(p3);
            Verificar(resultado.Advertencias.Count == 0, "Calculo SQL no concilia: " + string.Join("; ", resultado.Advertencias));
            Verificar(resultado.TotalPercepciones == previo.TotalPercepciones && resultado.TotalDeducciones == previo.TotalDeducciones,
                "SQL y Negocio difieren con incidencias legales.");
            Verificar(previo.DiasPagados == 14m, "La falta no descuento un dia.");
            Verificar(Numero("SELECT COUNT(*) FROM dbo.DocReciboDetalle d JOIN dbo.DocReciboNomina r ON r.ReciboId=d.ReciboId " +
                "WHERE r.PeriodoId=" + p3 + " AND d.EsIncidencia=0 AND ((d.TipoConcepto='P' AND d.ClaveConceptoSAT IN ('019','020','038')) " +
                "OR (d.TipoConcepto='D' AND d.ClaveConceptoSAT='004'))") == 4, "El calculo SQL no timbro las incidencias legales con su clave SAT.");
        });
        Caso("Los conceptos base no se quitan de la ficha ni del paquete ni se habilitan como incidencia", () =>
        {
            var paquetes = new PaqueteIncidenciaBLL();
            var baseFicha = bll.ListarAsignadas(empleado).First(a => a.EsBase);
            Rechaza(() => bll.DesactivarAsignada(empleado, baseFicha.IdEmpleadoIncidencia));
            var paquete = paquetes.Listar().Single(p => p.PeriodicidadPagoId == tipoQui && p.EsDefault);
            var baseEnPaquete = paquetes.ListarConceptos(paquete.IdPaquete).First(c => c.Concepto.EsBase);
            Rechaza(() => paquetes.QuitarConcepto(paquete.IdPaquete, baseEnPaquete.IdConceptoSAT));
            Rechaza(() => bll.HabilitarConcepto(ConceptoSAT("001", "P"), "MONTO_FIJO", 1m, true));
        });
        Caso("Un concepto del catalogo SAT se habilita con su regla y se agrega y quita del paquete", () =>
        {
            var paquetes = new PaqueteIncidenciaBLL();
            var paquete = paquetes.Listar().Single(p => p.PeriodicidadPagoId == tipoQui && p.EsDefault);
            int c010 = ConceptoSAT("010", "P");
            Verificar(bll.ListarCatalogo().Any(c => c.ConceptoId == c010 && !c.EsIncidencia), "El catalogo debe ofrecer conceptos sin regla.");
            Rechaza(() => paquetes.AgregarConcepto(paquete.IdPaquete, c010, 100m, true));
            Rechaza(() => bll.HabilitarConcepto(c010, "OTRA", 1m, true));
            Rechaza(() => bll.HabilitarConcepto(c010, "MONTO_FIJO", 0m, true));
            bll.HabilitarConcepto(c010, "MONTO_FIJO", 1m, true);
            Verificar(bll.ListarConceptos().Any(c => c.ConceptoId == c010 && c.EsIncidencia), "El concepto no quedo habilitado.");
            paquetes.AgregarConcepto(paquete.IdPaquete, c010, 100m, true);
            Verificar(paquetes.ListarConceptos(paquete.IdPaquete).Any(c => c.IdConceptoSAT == c010 && c.CantidadOValor == 100m && c.EsPermanente),
                "El concepto no quedo en el paquete.");
            int prueba2 = Numero("SELECT EmpleadoId FROM dbo.CatEmpleado WHERE NumeroEmpleado='PRUEBA2'");
            int empresa2 = Numero("SELECT EmpresaId FROM dbo.CatEmpleado WHERE NumeroEmpleado='PRUEBA2'");
            bll.AsignarPaqueteDefault(prueba2, empresa2, tipoQui);
            Verificar(bll.ListarAsignadas(prueba2).Any(a => a.IdConceptoSAT == c010 && a.CantidadOValor == 100m) && bll.ListarAsignadas(prueba2).Count == 5,
                "El alta no recibio el concepto agregado al paquete.");
            paquetes.QuitarConcepto(paquete.IdPaquete, c010);
            Verificar(paquetes.ListarConceptos(paquete.IdPaquete).All(c => c.IdConceptoSAT != c010), "El concepto no se quito del paquete.");
            Verificar(bll.ListarAsignadas(prueba2).Any(a => a.IdConceptoSAT == c010), "Quitar del paquete no debe alterar a quien ya lo recibio.");
        });
        Caso("Un paquete nuevo nace con los conceptos base y solo uno es el predeterminado por periodicidad", () =>
        {
            var paquetes = new PaqueteIncidenciaBLL();
            Rechaza(() => paquetes.Guardar(new CatPaqueteIncidencia { Nombre = "   ", PeriodicidadPagoId = tipoQui, Activo = true }));
            int id = paquetes.Guardar(new CatPaqueteIncidencia { Nombre = "Paquete de prueba", PeriodicidadPagoId = tipoQui, EsDefault = true, Activo = true });
            Verificar(paquetes.ListarConceptos(id).Count(c => c.Concepto.EsBase) == 4, "El paquete nuevo no trae los conceptos base.");
            Verificar(paquetes.Listar().Count(p => p.PeriodicidadPagoId == tipoQui && p.EsDefault) == 1 &&
                      paquetes.Listar().Single(p => p.PeriodicidadPagoId == tipoQui && p.EsDefault).IdPaquete == id,
                "Debe haber un solo paquete predeterminado por periodicidad.");
            Rechaza(() => paquetes.Guardar(new CatPaqueteIncidencia { Nombre = "paquete DE prueba", PeriodicidadPagoId = tipoQui, Activo = true }));
        });
        Caso("El selector del catalogo SAT lista y filtra los conceptos por tipo y texto", () =>
        {
            using (var selector = new FrmSeleccionarConceptoSAT("P"))
            {
                typeof(FrmSeleccionarConceptoSAT).GetMethod("FrmSeleccionarConceptoSAT_Load", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(selector, new object[] { selector, EventArgs.Empty });
                var grid = (DataGridView)ControlDe(selector, "dgvCatalogo");
                int percepciones = Numero("SELECT COUNT(*) FROM dbo.CatConcepto WHERE TipoConcepto='P' AND EsBase=0 AND Activo=1");
                Verificar(grid.Rows.Count == percepciones, "El selector no lista todo el catalogo de percepciones.");
                ((ComboBox)ControlDe(selector, "cmbTipoConcepto")).SelectedIndex = 1;
                Verificar(grid.Rows.Count == Numero("SELECT COUNT(*) FROM dbo.CatConcepto WHERE TipoConcepto='D' AND EsBase=0 AND Activo=1"),
                    "El selector no cambio a deducciones.");
                ControlDe(selector, "txtBuscarConcepto").Text = "ausencia";
                Verificar(grid.Rows.Count == 1 && ((CatConcepto)grid.Rows[0].DataBoundItem).ClaveSAT == "020", "La busqueda por texto es incorrecta.");
                Verificar(ControlDe(selector, "cmbFormaCalculo").Enabled, "Un concepto sin regla debe permitir definirla.");
            }
        });
        CertificadosCsdIntegracion();
    }

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Calculos();
            Disenadores();
            CertificadosCsd();
            if (args.Length == 2 && args[0] == "--solo-csd") IntegracionSoloCsd(args[1]);
            else if (args.Length == 1) Integracion(args[0]);
            Console.WriteLine("Casos aprobados: " + _casos);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}
