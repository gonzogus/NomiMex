using System;
using System.Collections.Generic;
using System.Linq;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;
using NomiMex.Entidades.Procesos;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Cálculo del pre-recibo de un trabajador: sueldo, séptimo día, horas extra, prima dominical, días festivos, bonos,
    /// subsidio, ISR, cuota obrera IMSS, INFONAVIT, préstamos y descuentos. Son funciones puras (sin acceso a datos) que
    /// aplican las mismas reglas y la misma precisión intermedia que sp_CalcularNomina, de modo que el pre-recibo coincide
    /// con el recibo que emite el servidor. Cada concepto se redondea a centavos y los totales son la suma de los conceptos.
    /// </summary>
    public static class CalculadoraPreRecibo
    {
        private const decimal DiasPorMes = 30.4167m;
        private const decimal HorasPorJornada = ConceptoIncidenciaBLL.HorasJornadaPredeterminada;
        private static readonly string[] ClavesCuotaObrera = { "EM_EXC", "EM_DIN", "EM_GMP", "IV", "CEV" };

        // Conceptos base del motor (tipo|clave SAT) que una empresa puede omitir en un periodo
        public const string LlaveSueldo = "P|001";
        public const string LlaveCuotaObrera = "D|001";
        public const string LlaveISR = "D|002";
        public const string LlaveSubsidio = "O|002";

        public static PreReciboDTO Calcular(PeriodoResumen periodo, ParametrosCalculoNomina par, DatosEmpleadoCalculo empleado,
                                            IncidenciasEmpleado incidencias, IEnumerable<ProcIncidenciaConcepto> conceptos = null,
                                            IEnumerable<ConceptoPeriodoPaquete> omitidos = null)
        {
            List<ProcIncidenciaConcepto> movimientos = (conceptos ?? Enumerable.Empty<ProcIncidenciaConcepto>()).ToList();
            List<ConceptoPeriodoPaquete> omisiones = (omitidos ?? Enumerable.Empty<ConceptoPeriodoPaquete>()).ToList();
            HashSet<string> baseOmitidos = new HashSet<string>(omisiones.Where(o => o.EsBase).Select(o => o.Llave));
            bool sueldoOmitido = baseOmitidos.Contains(LlaveSueldo);
            bool cuotaObreraOmitida = baseOmitidos.Contains(LlaveCuotaObrera);
            bool isrOmitido = baseOmitidos.Contains(LlaveISR);
            bool subsidioOmitido = baseOmitidos.Contains(LlaveSubsidio);
            IncidenciasEmpleado inc = incidencias ?? new IncidenciasEmpleado();
            decimal sd = empleado.SalarioDiario;
            decimal uma = par.UMADiaria;
            // Parámetros resueltos por empresa/periodo/empleado (RelParametroCalculo); si no hay override, los de la política.
            decimal porcentajePrima = empleado.PorcentajePrimaDominical > 0m ? empleado.PorcentajePrimaDominical : par.PorcentajePrimaDominical;
            decimal factorHED = empleado.FactorHoraExtraDoble > 0m ? empleado.FactorHoraExtraDoble : par.FactorHoraExtraDoble;
            decimal topePrimaUMAs = empleado.ExencionPrimaDominicalUMAs > 0m ? empleado.ExencionPrimaDominicalUMAs : par.ExencionPrimaDominicalUMAs;

            // Séptimo día: descansos del periodo y días laborables sobre los que se prorratea
            int descansos = CalculadoraNomina.ContarDescansos(periodo);
            decimal diasLaborables = Math.Max(periodo.DiasPeriodo - descansos, 0m);

            decimal faltas = Dec(inc.DiasDescontados, 2);
            decimal horasDobles = Dec(inc.HorasExtraDobles, 2);
            decimal horasTriples = Dec(inc.HorasExtraTriples, 2);
            decimal diasFestivosLaborados = Dec(inc.DiasFestivosLaborados, 2);
            decimal diasPrimaDominical = Dec(inc.DiasPrimaDominical, 2);
            decimal bonos = Dec(inc.Bonos, 2);
            decimal descuentos = Dec(inc.Descuentos, 2);

            decimal diasLaborablesPagados = Math.Max(CalculadoraNomina.CalcularDiasContratados(periodo.DiasPeriodo, diasLaborables, empleado.DiasLaboradosSemana) - faltas, 0m);
            decimal diasSeptimo = descansos > 0 && diasLaborables > 0m ? Dec(descansos * diasLaborablesPagados / diasLaborables, 4) : 0m;
            decimal diasTrabajados = diasLaborablesPagados + diasSeptimo;

            decimal sueldoDias = sueldoOmitido ? 0m : Dec(sd * diasLaborablesPagados, 4);
            decimal pagoSeptimo = sueldoOmitido ? 0m : Dec(sd * diasSeptimo, 4);
            decimal salarioBase = sueldoDias + pagoSeptimo;
            decimal jornadaDefecto = par.JornadaDiariaEstandar > 0m ? par.JornadaDiariaEstandar : HorasPorJornada;
            decimal salarioHora = Dec(sd / (empleado.HorasJornada > 0m ? empleado.HorasJornada : jornadaDefecto), 4);

            // Horas extra: la mitad de las dobles está exenta con el tope semanal prorrateado al periodo
            decimal importeDobles = Dec(horasDobles * salarioHora * factorHED, 4);
            decimal importeTriples = Dec(horasTriples * salarioHora * par.FactorHoraExtraTriple, 4);
            decimal topeExentoSemanal = Dec(uma * par.ExencionHorasExtraUMAsSemanal * (periodo.DiasPeriodo / 7m), 4);
            decimal exentoHorasExtra = horasDobles <= par.LimiteHorasExtraSinGravar
                ? Math.Min(Dec(importeDobles * 0.5m, 4), topeExentoSemanal)
                : 0m;
            decimal gravadoHorasExtra = Dec(importeDobles + importeTriples - exentoHorasExtra, 4);

            // Días festivos laborados (LFT Art. 75): limitados a los festivos del periodo; 50% exento con el tope compartido con horas extra
            decimal diasFestivos = Math.Min(diasFestivosLaborados, periodo.TotalFestivos);
            decimal importeFestivo = Dec(diasFestivos * sd * par.FactorDiaFestivoLaborado, 4);
            decimal exentoFestivo = Math.Min(Dec(importeFestivo * 0.5m, 4), Math.Max(topeExentoSemanal - exentoHorasExtra, 0m));

            // Prima dominical: porcentaje sobre el salario diario, exenta hasta N UMA por domingo laborado
            decimal importePrima = Dec(diasPrimaDominical * sd * porcentajePrima / 100m, 4);
            decimal exentoPrima = Math.Min(importePrima, Dec(diasPrimaDominical * uma * topePrimaUMAs, 4));

            // Aguinaldo (LFT Art. 87): días x salario diario; exento hasta N UMA (default 30, LISR 93-XIV)
            decimal diasAguinaldo = Dec(inc.DiasAguinaldo, 2);
            decimal topeAguiUMAs = empleado.TopeUMAAguinaldo > 0m ? empleado.TopeUMAAguinaldo : 30m;
            decimal importeAguinaldo = Dec(diasAguinaldo * sd, 4);
            decimal exentoAguinaldo = Math.Min(importeAguinaldo, Dec(topeAguiUMAs * uma, 4));

            // Prima vacacional (LFT Art. 80): días de vacaciones x salario diario x %; exenta hasta N UMA (default 15)
            decimal diasPrimaVac = Dec(inc.DiasPrimaVacacional, 2);
            decimal pctPrimaVac = empleado.PorcentajePrimaVacacional > 0m ? empleado.PorcentajePrimaVacacional : 25m;
            decimal topePrimaVacUMAs = empleado.TopeUMAPrimaVacacional > 0m ? empleado.TopeUMAPrimaVacacional : 15m;
            decimal importePrimaVac = Dec(diasPrimaVac * sd * pctPrimaVac / 100m, 4);
            decimal exentoPrimaVac = Math.Min(importePrimaVac, Dec(topePrimaVacUMAs * uma, 4));

            // Prima de antigüedad (022, LFT Art. 162) e indemnizaciones por separación (025): importe = días x salario diario;
            // exención de separación COMPARTIDA = 90 UMA por año de servicio (LISR 93-XIII), repartida a prorrata.
            decimal diasAntiguedad = Dec(inc.DiasAntiguedad, 2);
            decimal diasIndemniz = Dec(inc.DiasIndemnizacion, 2);
            decimal importeAntiguedad = Dec(diasAntiguedad * sd, 4);
            decimal importeIndemniz = Dec(diasIndemniz * sd, 4);
            decimal importeSepar = importeAntiguedad + importeIndemniz;
            int aniosServicio = AniosCumplidos(empleado.FechaIngreso, periodo.FechaFin);
            decimal exentoSepar = Math.Min(importeSepar, Dec(aniosServicio * 90m * uma, 4));
            decimal exentoAntiguedad = importeSepar > 0m ? Dec(exentoSepar * importeAntiguedad / importeSepar, 4) : 0m;
            decimal exentoIndemniz = exentoSepar - exentoAntiguedad;

            decimal totalGravado = salarioBase + gravadoHorasExtra + (importeFestivo - exentoFestivo) + (importePrima - exentoPrima)
                + (importeAguinaldo - exentoAguinaldo) + (importePrimaVac - exentoPrimaVac)
                + (importeAntiguedad - exentoAntiguedad) + (importeIndemniz - exentoIndemniz) + bonos;
            decimal totalExento = exentoHorasExtra + exentoFestivo + exentoPrima + exentoAguinaldo + exentoPrimaVac
                + exentoAntiguedad + exentoIndemniz;
            totalGravado += movimientos.Where(i => i.Concepto.TipoConcepto == "P" && i.Concepto.EsGravado).Sum(i => i.ImporteCalculado);
            totalExento += movimientos.Where(i => i.Concepto.TipoConcepto == "P" && !i.Concepto.EsGravado).Sum(i => i.ImporteCalculado);

            // ISR mensualizado menos subsidio; ambos se regresan al periodo con el mismo factor
            decimal factorMes = Dec(DiasPorMes / periodo.DiasPeriodo, 6);
            decimal baseMensual = Dec(totalGravado * factorMes, 4);
            decimal isrMensual = CalcularISRMensual(baseMensual, par.TablaISR);
            decimal subsidioMensual = subsidioOmitido ? 0m : CalcularSubsidioMensual(baseMensual, par.TablaSubsidio);
            decimal isrNetoMensual = Math.Max(isrMensual - subsidioMensual, 0m);
            decimal subsidioPeriodo = Dec(subsidioMensual / factorMes, 2);
            decimal isrPeriodo = isrOmitido ? 0m : Dec(isrNetoMensual / factorMes, 2);

            decimal cuotaObrera = cuotaObreraOmitida ? 0m : CalcularCuotaObrera(empleado.SBC, uma, Dec(diasTrabajados, 2), par.CuotasIMSS);
            decimal infonavit = CalcularDescuentoInfonavit(empleado, par.SalarioMinimoGeneral, diasTrabajados);
            decimal prestamos = Dec(empleado.DescuentoPrestamos, 4);

            PreReciboDTO pre = new PreReciboDTO
            {
                PeriodoId = periodo.PeriodoId,
                EmpleadoId = empleado.EmpleadoId,
                NumeroEmpleado = empleado.NumeroEmpleado,
                NombreCompleto = empleado.NombreCompleto,
                RFC = empleado.RFC,
                CURP = empleado.CURP,
                NSS = empleado.NSS,
                Puesto = empleado.Puesto,
                Departamento = empleado.Departamento,
                SalarioDiario = sd,
                SDI = empleado.SDI,
                SBC = empleado.SBC,
                DiasPagados = diasTrabajados,
                DiasSeptimo = diasSeptimo,
                DiasSemana = empleado.DiasLaboradosSemana,
                DiasSemanaSoloPeriodo = empleado.DiasSemanaSoloPeriodo,
                TotalGravado = Dec(totalGravado, 4),
                TotalExento = Dec(totalExento, 4),
                TotalIncidencias = inc.TotalActivas + movimientos.Count,
                BloqueaTimbrado = omisiones.Any(o => o.EsLegal),
                ConceptosOmitidos = omisiones.Select(o => o.Concepto).ToList()
            };

            AgregarConcepto(pre.Percepciones, "P", "001", "Sueldos, Salarios Rayas y Jornales", diasLaborablesPagados, sueldoDias, 0m, true);
            AgregarConcepto(pre.Percepciones, "P", "001", "Séptimo día (proporcional)", diasSeptimo, pagoSeptimo, 0m, pagoSeptimo > 0m);
            AgregarConcepto(pre.Percepciones, "P", "019", "Horas Extra", horasDobles + horasTriples, gravadoHorasExtra, exentoHorasExtra,
                importeDobles + importeTriples > 0m, CodigoIncidencia.HoraExtraDoble, CodigoIncidencia.HoraExtraTriple);
            AgregarConcepto(pre.Percepciones, "P", "020", "Prima Dominical", diasPrimaDominical, importePrima - exentoPrima, exentoPrima,
                importePrima > 0m, CodigoIncidencia.PrimaDominical);
            AgregarConcepto(pre.Percepciones, "P", "002", "Gratificación Anual (Aguinaldo)", diasAguinaldo, importeAguinaldo - exentoAguinaldo, exentoAguinaldo,
                importeAguinaldo > 0m);
            AgregarConcepto(pre.Percepciones, "P", "021", "Prima Vacacional", diasPrimaVac, importePrimaVac - exentoPrimaVac, exentoPrimaVac,
                importePrimaVac > 0m);
            AgregarConcepto(pre.Percepciones, "P", "022", "Prima de Antigüedad", diasAntiguedad, importeAntiguedad - exentoAntiguedad, exentoAntiguedad,
                importeAntiguedad > 0m);
            AgregarConcepto(pre.Percepciones, "P", "025", "Indemnizaciones", diasIndemniz, importeIndemniz - exentoIndemniz, exentoIndemniz,
                importeIndemniz > 0m);
            AgregarConcepto(pre.Percepciones, "P", "038", "Pago adicional por día festivo laborado (Art. 75 LFT)", diasFestivos,
                importeFestivo - exentoFestivo, exentoFestivo, importeFestivo > 0m, CodigoIncidencia.DiaFestivoLaborado);
            AgregarConcepto(pre.Percepciones, "P", "038", "Bono o gratificación", null, bonos, 0m, bonos > 0m, CodigoIncidencia.Bono);
            AgregarConcepto(pre.Percepciones, "O", "002", "Subsidio para el Empleo Efectivamente Entregado", null, 0m, subsidioPeriodo,
                subsidioPeriodo > 0m);

            AgregarConcepto(pre.Deducciones, "D", "002", "ISR Retenido", null, isrPeriodo, 0m, isrPeriodo > 0m);
            AgregarConcepto(pre.Deducciones, "D", "001", "Seguridad Social (Cuota Obrera IMSS)", null, cuotaObrera, 0m, cuotaObrera > 0m);
            AgregarConcepto(pre.Deducciones, "D", "010", "Crédito INFONAVIT (Pago por crédito de vivienda)", null, infonavit, 0m, infonavit > 0m);
            AgregarConcepto(pre.Deducciones, "D", "012", "Anticipo / Préstamo Empresa (Anticipo de salarios)", null, prestamos, 0m, prestamos > 0m);
            AgregarConcepto(pre.Deducciones, "D", "004", "Otros descuentos", null, descuentos, 0m, descuentos > 0m, CodigoIncidencia.Descuento);

            foreach (ProcIncidenciaConcepto i in movimientos)
            {
                ConceptoIncidenciaBLL.ExigirConcepto(i.Concepto);
                bool percepcion = i.Concepto.TipoConcepto == "P";
                var linea = new PreReciboConceptoDTO
                {
                    IdIncidenciaConcepto = i.IdIncidencia,
                    EsPermanente = i.EsPermanente,
                    TipoConcepto = i.Concepto.TipoConcepto,
                    Clave = i.Concepto.ClaveSAT,
                    Descripcion = i.Concepto.Descripcion,
                    Cantidad = i.CantidadOValor,
                    UnidadCaptura = i.Concepto.FormaCalculo == "HORAS" ? " h" :
                        i.Concepto.FormaCalculo == "DIAS" ? " d" : i.Concepto.FormaCalculo == "PORCENTAJE_SD" ? " %" : " $",
                    ImporteGravado = !percepcion || i.Concepto.EsGravado ? i.ImporteCalculado : 0m,
                    ImporteExento = percepcion && !i.Concepto.EsGravado ? i.ImporteCalculado : 0m
                };
                (percepcion ? pre.Percepciones : pre.Deducciones).Add(linea);
            }

            pre.TotalPercepciones = pre.Percepciones.Sum(c => c.Total);
            pre.TotalDeducciones = pre.Deducciones.Sum(c => c.Total);
            pre.NetoPagar = CalculadoraNomina.CalcularNeto(pre.TotalPercepciones, pre.TotalDeducciones);
            return pre;
        }

        /// <summary>ISR mensual (Art. 96 LISR): cuota fija más la tasa sobre el excedente del límite inferior; 0 si no hay tarifa.</summary>
        public static decimal CalcularISRMensual(decimal baseMensual, IEnumerable<TramoISR> tarifa)
        {
            TramoISR tramo = tarifa.OrderBy(t => t.Renglon)
                .FirstOrDefault(t => baseMensual >= t.LimiteInferior && baseMensual <= t.LimiteSuperior);
            return tramo == null ? 0m : Dec(tramo.CuotaFija + (baseMensual - tramo.LimiteInferior) * tramo.TasaExcedente, 4);
        }

        /// <summary>Subsidio al empleo mensual: monto del primer renglón cuyo tope cubre la base; 0 si no hay tabla.</summary>
        public static decimal CalcularSubsidioMensual(decimal baseMensual, IEnumerable<TramoSubsidio> tabla)
        {
            TramoSubsidio tramo = tabla.OrderBy(t => t.HastaIngreso).FirstOrDefault(t => baseMensual <= t.HastaIngreso);
            return tramo == null ? 0m : Dec(tramo.MontoSubsidio, 4);
        }

        /// <summary>Cuota obrera IMSS (enfermedad y maternidad, invalidez y vida, cesantía y vejez) sobre el SBC de los días trabajados.</summary>
        public static decimal CalcularCuotaObrera(decimal sbc, decimal umaDiaria, decimal diasTrabajados, IEnumerable<CuotaIMSSObrera> cuotas)
        {
            decimal baseSBC = sbc * diasTrabajados;
            decimal excedente = sbc > 3m * umaDiaria ? (sbc - 3m * umaDiaria) * diasTrabajados : 0m;

            decimal total = 0m;
            foreach (CuotaIMSSObrera c in cuotas.Where(x => x.PorcentajeObrero > 0m && Array.IndexOf(ClavesCuotaObrera, x.ClaveConcepto) >= 0))
            {
                total += c.PorcentajeObrero * (c.BaseCalculo == "SBC_EXCEDENTE_3UMA" ? excedente : baseSBC);
            }
            return Dec(total, 4);
        }

        public static decimal CalcularDescuentoInfonavit(DatosEmpleadoCalculo empleado, decimal salarioMinimo, decimal diasTrabajados)
        {
            decimal monto = empleado.MontoDescuentoInfonavit;
            switch (empleado.TipoDescuentoInfonavit)
            {
                case "CUOTA_FIJA": return Dec(monto, 4);
                case "PORCENTAJE": return Dec(empleado.SBC * diasTrabajados * monto / 100m, 4);
                case "VSM": return Dec(monto * salarioMinimo * diasTrabajados, 4);
                default: return 0m;
            }
        }

        // Un concepto solo se agrega cuando aplica y su importe, ya redondeado a centavos, es mayor a cero
        private static void AgregarConcepto(List<PreReciboConceptoDTO> destino, string tipo, string clave, string descripcion,
                                            decimal? cantidad, decimal gravado, decimal exento, bool aplica, params string[] codigosOrigen)
        {
            decimal g = Dec(gravado, 2);
            decimal e = Dec(exento, 2);
            if (!aplica || g + e <= 0m)
            {
                return;
            }

            destino.Add(new PreReciboConceptoDTO
            {
                TipoConcepto = tipo,
                CodigosOrigen = codigosOrigen,
                Clave = clave,
                Descripcion = descripcion,
                Cantidad = cantidad.HasValue ? Dec(cantidad.Value, 4) : (decimal?)null,
                ImporteGravado = g,
                ImporteExento = e
            });
        }

        private static decimal Dec(decimal valor, int decimales)
        {
            return Math.Round(valor, decimales, MidpointRounding.AwayFromZero);
        }

        /// <summary>Años completos de servicio entre ingreso y corte (equivalente a dbo.fn_AniosCumplidos).</summary>
        private static int AniosCumplidos(DateTime ingreso, DateTime corte)
        {
            if (corte < ingreso) return 0;
            int anios = corte.Year - ingreso.Year;
            if (ingreso.AddYears(anios) > corte) anios--;
            return anios;
        }
    }
}
