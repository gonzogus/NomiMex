# Revision tecnica de NomiMex

**Fecha:** 13 de noviembre de 2026.  
**Estado:** Reporte historico. Los archivos `NomiMex.sql` y `NomiMex_Refactorizado.sql` que describe no estan en esta copia; sus hallazgos no certifican el instalador disponible.
**Instalador disponible:** [NomiMex_Instalacion.sql](NomiMex_Instalacion.sql), que requiere variables SQLCMD y recrea la base cuyo nombre se proporciona.

El original permanece intacto. Su SHA-256 es `720796351945ed666e2b23c0148a66762aa9aa6131efb3b64cc669635e764304`.

## 1. Reporte de hallazgos

Las referencias de linea corresponden al original. Se distingue entre correcciones implementadas y riesgos que requieren una decision funcional o validacion fiscal adicional. **La instalacion correcta del esquema no certifica el calculo de impuestos ni la aceptacion de un CFDI por SAT/PAC.**

### Errores corregidos

| Severidad | Evidencia original | Problema y tratamiento |
|---|---|---|
| Critica | lineas 76-85 | El despliegue desconecta usuarios y elimina la base existente. La entrega no ejecuta `DROP DATABASE`, `ALTER DATABASE`, `CREATE DATABASE` ni `USE`; exige una base vacia. Usa `:ON ERROR EXIT` para detener todos los lotes al fallar. |
| Alta | lineas 3708-3753, `vw_ResumenPeriodo` | El JOIN recibo-detalle multiplica tanto el conteo de empleados como los totales de cabecera. El ISR se preagrega por recibo antes de sumar. Se excluyen borradores y cancelados. |
| Alta | lineas 2925-2961, `sp_CalcularAguinaldo` | Reemplaza el renglon anterior, pero vuelve a incrementar la cabecera con todo el aguinaldo. Ademas, `@ReciboId` puede conservar el recibo del empleado anterior. Se restan importes previos y se reinicia la variable por empleado. |
| Alta | lineas 2148-2241, `sp_CargarDatosFiscales` | Tarifas incrustadas, subsidio historico aplicado sin vigencia y reutilizacion de 2026 para cualquier ejercicio futuro. Se eliminan las semillas fiscales no verificadas; el procedimiento conserva su firma pero exige parametros importados, sin borrarlos ni inventarlos. |
| Alta | lineas 1841-1870, 2228 | Las funciones UMA/SMG sustituyen faltantes por cero y aceptan ejercicios anteriores indiscriminadamente. Se agregan funciones por fecha con inicio y fin de cobertura explicitos; los auxiliares anuales requieren el ejercicio exacto. `sp_ActualizarSBC` usa fecha efectiva y rechaza parametros ausentes. |
| Alta | lineas 1906-1925 | Un intervalo ISR no encontrado devuelve cero; con precision de cuatro decimales una base puede caer en el hueco entre limites publicados a centavos. La busqueda normaliza la base a centavos y devuelve NULL cuando falta tarifa; el proceso rechaza ese resultado. Se impiden rangos superpuestos. |
| Alta | lineas 1954-1999 | `SELECT @Total = @Total + ...` sobre varias filas no garantiza una agregacion correcta. Se reemplaza por `SUM`; se conserva DECIMAL y el resultado se redondea explicitamente. |
| Alta | lineas 2020-2033 | El ISR anual se obtiene multiplicando el mensual por doce. Se crea `CatTablaISRAnual`, independiente, y se exige su carga. |
| Alta | lineas 2620-2627, 2656-2659 | El subsidio se incluye en TotalPercepciones, no se llena TotalOtrosPagos y el importe causado se trata como efectivo entregado. Se separan ambos totales, se conserva `SubsidioCausado` en un nodo propio y la entrega de excedente solo se permite por un parametro explicito. |
| Alta | lineas 2484-2706 | Redondear los importes globales antes de comparar con renglones redondeados puede producir diferencias de centavos. Las cabeceras se consolidan desde los importes realmente almacenados en el detalle. CHECK exige el cuadre de neto. |
| Alta | lineas 1179-1223 | CFDI de nomina permite monedas distintas de MXN, TipoCambio, FormaPago=99 y exportaciones incompatibles con la guia. Se restringe a MXN, sin TipoCambio ni FormaPago, Exportacion=01, MetodoPago=PUE, TipoDeComprobante=N y UsoCFDI=CN01. |
| Alta | lineas 1317, 3322-3393 | PTU mezcla empleadores del mismo ejercicio, estima dias con `30 * COUNT(*)` e ignora elegibilidad y topes individuales. `ProcBasePTU` exige dias computables, salario de reparto, elegibilidad, tope y validacion. El procedimiento opera por empresa y no borra resultados de otras empresas. La determinacion juridica de esas bases no se inventa en SQL. |
| Alta | lineas 3038-3135 | Autorizar/cerrar/reversar validan el estado fuera de la transaccion. El cierre podria duplicar acumulados y amortizaciones ante ejecuciones simultaneas. La transicion y la validacion se serializan sobre el periodo; MERGE usa HOLDLOCK. Se verifico el rechazo de cierre repetido; no se hizo una prueba de estres concurrente. |
| Alta | lineas 3076-3084 | A cada prestamo se le descuenta el renglon agregado de todos los prestamos del empleado. Se aplica la cuota individual y se rechaza el cierre si las cuotas actuales ya no coinciden con el recibo. |
| Alta | lineas 3595-3674 | Validaciones de politica hacen RETURN despues de BEGIN TRANSACTION y pueden dejarla abierta; una accion desconocida devuelve exito. Se utilizan errores explicitos y rollback, y se comprueban accion, empresa y existencia de la politica. |
| Alta | lineas 1049-1072 | Conceptos P/D/O pueden no apuntar a ningun catalogo SAT o tener ClaveSAT diferente de la clave referenciada. Se exige exactamente el catalogo correspondiente y un trigger comprueba coherencia clave/ID. Las incidencias I siguen siendo codigos internos no timbrables. |
| Alta | lineas 1493-1514, 1117-1156 | No hay validacion cruzada de empresa empleado-periodo. Se rechazan incidencias/recibos de otra empresa y fechas de incidencia fuera del periodo. Una FK compuesta asegura la asignacion empresa-periodicidad, incluso en UPDATE. |
| Alta | lineas 3149-3264 | El finiquito aplica un tope de exencion de separacion a prima de antiguedad y otro a indemnizaciones; su detalle no refleja la exencion de estas ultimas. Se comparte un solo tope, se consideran anios cumplidos y la fraccion mayor de seis meses, y se distribuye la exencion en el detalle. **Esto no resuelve todos los problemas legales del finiquito; vease el apartado siguiente.** |
| Media | lineas 2787-2818 | La transmision del Poder Ejecutivo se registra el 1 de diciembre incluso para 2024 en adelante. Se utiliza el 1 de octubre a partir de 2024; se conserva diciembre para los ejercicios anteriores. |
| Media | lineas 4425-4431 | La escala vacacional mantiene veinte dias entre los anios cinco y nueve. El minimo legal es veinte en el quinto, veintidos del sexto al decimo, veinticuatro del onceavo al quinceavo. La funcion calcula ese minimo y solo admite mejoras de la tabla configurada. |
| Media | lineas 2620, 2628-2630 | `DATEDIFF(WEEK)` cuenta cruces de semana, no semanas transcurridas; el regimen receptor se obtiene convirtiendo un ID a texto. Se usan dias completos/7 y la Clave del catalogo referenciado. |
| Media | lineas 1647-1655 | Indices RFC, CURP y NSS duplican los indices de restricciones UNIQUE. Se retiran esos tres indices redundantes. |
| Media | lineas 1708-1709 | Serie/Folio es unico globalmente en una base multiempresa. La unicidad se delimita por EmpresaId; UUID mantiene unicidad global filtrada. |
| Media | lineas 607-614, 1456-1473 | Puede haber varias cuentas bancarias activas por empleado y varios paquetes default por alcance. Se agregan indices unicos filtrados. Es una regla funcional explicita de esta propuesta; si se requiere dispersion fraccionada debe modelarse separadamente antes de migrar. |
| Media | lineas 4061-4093 | La validacion de certificados depende del CSD actualmente activo; rotarlo puede impedir operar un comprobante historico. Se verifica el numero de certificado utilizado, la empresa y su vigencia a la fecha de emision, no su estado activo actual. |
| Media | lineas 3339-3366, 3421-3475 | Respuestas PAC incompletas pueden marcar un documento timbrado/cancelado. Se exige XML parseable, datos de timbrado, coherencia UUID-XML y acuse de cancelacion parseable. Un CHECK impide estados finales sin UUID, fecha y XML. Esto NO autentica la respuesta PAC ni sustituye validacion criptografica. |

### Tablas duplicadas, redundancias y normalizacion

1. **No existen nombres de tabla duplicados:** los 82 CREATE TABLE del original tienen nombres distintos. `ProcIncidencia` y `CatTipoIncidencia` se mencionan como objetos retirados, pero no se crean; no se atribuye una duplicidad inexistente a esta version.
2. **SAT mezclado con reglas internas:** Jornada contiene horas/aplicacion; Periodicidad contiene DiasBase; Riesgo contiene primas/clase operativa; Incapacidad contiene reglas de pago. Se separan en cuatro tablas `CatRegla...`, relacionadas 1:1 con el catalogo oficial. Las vistas de compatibilidad no duplican datos.
3. **CatMetodoPago contiene formas, no metodos:** sus claves 01/02/03/99 son formas de dispersion. Se renombra `CatFormaPagoDispersion`; se crean por separado `CatSatMetodoPago` y `CatSatFormaPago` con los campos de sus hojas oficiales.
4. **TipoPersona fiscal pierde informacion:** F/M/T es una compresion local de las columnas independientes Fisica/Moral del Excel; se almacenan ambas banderas. TipoPersona permanece solo como proyeccion de compatibilidad.
5. **Snapshots de recibos son desnormalizacion intencional:** se conservan y amplian; deben representar el dato fiscal utilizado al emitir, no una consulta a maestros que pueden cambiar despues.
6. **Redundancias que requieren decidir la fuente de verdad antes de migrar:** RegistroPatronal en CatEmpresa y CatEmpresaRegistroPatronal; SDI/SBC en datos IMSS, contratos e historiales; FormulaId en CatConcepto y RelConceptoFormula; Nombre de estado en CatEntidadFederativa y el nuevo CatSatEstado. No se eliminan columnas ni se fusionan registros sin conocer los casos de multi-registro, politica y temporalidad.
7. **Unicidad de personas entre empleadores:** RFC/CURP/NSS globalmente unicos en la ficha del empleado impiden representar al mismo trabajador en dos empresas. Una solucion durable es separar persona y relacion laboral. Esa migracion afecta altas, expedientes, consultas e historial; no se hizo silenciosamente en un script de instalacion separado.

### Riesgos fiscales/funcionales que NO se consideran resueltos ni certificados

- Las exenciones de horas extra/descansos siguen calculandose por agregado del periodo, no por semanas reales ni con todas las condiciones de salario minimo. Los nodos fiscales de horas extra requieren captura/poblacion adicional; el timbrado rechaza su ausencia.
- IMSS requiere tasas CEV por tramo y ejercicio, prima RT del registro patronal y tratamiento de ausencias/incapacidades. La tabla de porcentajes general y las firmas de las funciones no cubren toda esa casuistica.
- El finiquito todavia necesita integrar SDI para las indemnizaciones que correspondan, saldo vacacional real, aniversario y pagos previos; determinar cuando procede el pago de veinte dias por anio; y aplicar correctamente las reglas de ISR de separacion/retencion, no solo la tarifa mensual. No debe liberarse como calculadora legal definitiva.
- Agregar aguinaldo a un recibo no recalcula por si solo la retencion que legalmente corresponda ni controla exenciones ya consumidas en otros pagos. La correccion implementada evita duplicacion, no certifica la retencion.
- Las funciones RFC/CURP originales son controles sintacticos parciales, no verifican padron SAT/RENAPO ni todos los digitos verificadores. Los CHECK de longitud no prueban validez fiscal. Deben conservarse validaciones de importacion y conciliacion con constancias.
- El nuevo validador de timbrado comprueba datos obligatorios de recibo, cuadre, claves de conceptos, vigencia y varios nodos condicionales. No implementa toda la matriz SAT, todas las validaciones de atributos/fechas, todos los nodos, ni validacion de sellos/certificados/cadena original.
- El estado CANCELADO debe provenir de una respuesta autenticada y confirmada, no de la mera existencia de XML. La aplicacion debe validar esa procedencia antes de llamar al procedimiento.
- No se modifico la aplicacion .NET ni su calculadora paralela. Las vistas/sinonimos conservan nombres y muchas consultas, **no garantizan compatibilidad fiscal o funcional integral**.

## 2. Mapeo de taxonomia

`Cat` incluye maestros y configuracion; `CatSat` es una especializacion de `Cat`, no otro prefijo. `Proc` contiene transacciones y estado operativo. `Doc` guarda documentos y nodos de comprobantes. `Rel` representa asociaciones. `Hist` conserva versiones. `Log` registra eventos.

Las sesiones activas y recuperaciones pendientes pasan a `Proc`: no son una bitacora inmutable. Los eventos CFDI pasan a `Log`. No se crean prefijos adicionales porque todas las tablas encajan en las categorias solicitadas.

| Tabla original | Tabla fisica refactorizada |
|---|---|
| `CatRegimenFiscal` | `CatSatRegimenFiscal` |
| `CatTipoJornada` | `CatSatTipoJornada` |
| `CatPeriodicidadPago` | `CatSatPeriodicidadPago` |
| `CatTipoRegimen` | `CatSatTipoRegimen` |
| `CatRiesgoPuesto` | `CatSatRiesgoPuesto` |
| `CatMotivoCancelacion` | `CatSatMotivoCancelacion` |
| `CatTipoIncapacidad` | `CatSatTipoIncapacidad` |
| `CatEstadoCFDI` | `CatEstadoCFDI` |
| `CatOrigenRecurso` | `CatSatOrigenRecurso` |
| `CatTipoNomina` | `CatSatTipoNomina` |
| `CatTipoHoras` | `CatSatTipoHoras` |
| `CatBanco` | `CatSatBanco` |
| `CatMetodoPago` | `CatFormaPagoDispersion` |
| `CatEntidadFederativa` | `CatEntidadFederativa` |
| `CatTipoPercepcion` | `CatSatTipoPercepcion` |
| `CatTipoDeduccion` | `CatSatTipoDeduccion` |
| `CatTipoOtroPago` | `CatSatTipoOtroPago` |
| `CatTipoContrato` | `CatSatTipoContrato` |
| `CatTipoFormula` | `CatTipoFormula` |
| `CatRol` | `CatRol` |
| `CatPermiso` | `CatPermiso` |
| `CatUsuario` | `CatUsuario` |
| `CatDepartamento` | `CatDepartamento` |
| `CatPuesto` | `CatPuesto` |
| `CatCentroCosto` | `CatCentroCosto` |
| `CatTurno` | `CatTurno` |
| `CatEmpresa` | `CatEmpresa` |
| `HistEmpresaCSD` | `HistEmpresaCSD` |
| `CatEmpresaRegistroPatronal` | `CatEmpresaRegistroPatronal` |
| `CatEjercicioFiscal` | `CatEjercicioFiscal` |
| `CatEmpleado` | `CatEmpleado` |
| `CatEmpleadoDatosFiscales` | `CatEmpleadoDatosFiscales` |
| `CatEmpleadoDatosIMSS` | `CatEmpleadoDatosIMSS` |
| `CatEmpleadoBancoCuenta` | `CatEmpleadoBancoCuenta` |
| `HistEmpleadoSalario` | `HistEmpleadoSalario` |
| `HistEmpleadoDatosFiscales` | `HistEmpleadoDatosFiscales` |
| `ProcPeriodoNomina` | `ProcPeriodoNomina` |
| `ProcIncapacidad` | `ProcIncapacidad` |
| `ProcCreditoInfonavit` | `ProcCreditoInfonavit` |
| `ProcCreditoFonacot` | `ProcCreditoFonacot` |
| `ProcVacacionesSaldo` | `ProcVacacionesSaldo` |
| `ProcVacacionesDisfrute` | `ProcVacacionesDisfrute` |
| `ProcPrestamo` | `ProcPrestamo` |
| `ProcRegistroAsistencia` | `ProcRegistroAsistencia` |
| `ProcAguinaldo` | `ProcAguinaldo` |
| `ProcPTU` | `ProcPTU` |
| `ProcAcumuladoMensual` | `ProcAcumuladoMensual` |
| `ProcAjusteAnualISR` | `ProcAjusteAnualISR` |
| `CatUMA` | `CatUMA` |
| `CatSalarioMinimo` | `CatSalarioMinimo` |
| `CatTablaISR` | `CatTablaISR` |
| `CatTablaSubsidioEmpleo` | `CatTablaSubsidioEmpleo` |
| `CatCuotaIMSS` | `CatCuotaIMSS` |
| `CatTablaVacaciones` | `CatTablaVacaciones` |
| `CatSistema` | `CatSistema` |
| `CatPoliticaEmpresarial` | `CatPoliticaEmpresarial` |
| `CatFormulaCalculo` | `CatFormulaCalculo` |
| `CatConcepto` | `CatConcepto` |
| `DocContratoEmpleado` | `DocContratoEmpleado` |
| `DocReciboNomina` | `DocReciboNomina` |
| `DocReciboDetalle` | `DocReciboDetalle` |
| `DocCFDINomina` | `DocCFDINomina` |
| `DocCFDIEvento` | `LogCFDIEvento` |
| `DocEmpleadoBaja` | `DocEmpleadoBaja` |
| `DocFiniquito` | `DocFiniquito` |
| `DocFiniquitoDetalle` | `DocFiniquitoDetalle` |
| `RelRolPermiso` | `RelRolPermiso` |
| `RelUsuarioRol` | `RelUsuarioRol` |
| `RelConceptoFormula` | `RelConceptoFormula` |
| `RelEmpresaPeriodicidad` | `RelEmpresaPeriodicidad` |
| `CatDiaFestivo` | `CatDiaFestivo` |
| `RelPeriodoDiaFestivo` | `RelPeriodoDiaFestivo` |
| `CatPaqueteIncidencia` | `CatPaqueteIncidencia` |
| `RelPaqueteConcepto` | `RelPaqueteConcepto` |
| `RelEmpleadoIncidencia` | `RelEmpleadoIncidencia` |
| `ProcIncidencias` | `ProcIncidencias` |
| `LogSesionUsuario` | `ProcSesionUsuario` |
| `LogRecuperacionContrasena` | `ProcRecuperacionContrasena` |
| `LogAuth` | `LogAuth` |
| `LogCalculo` | `LogCalculo` |
| `LogAuditoriaSalario` | `LogAuditoriaSalario` |
| `LogAuditoriaPolitica` | `LogAuditoriaPolitica` |

### Tablas adicionales

- Reglas internas: `CatReglaTipoJornada`, `CatReglaPeriodicidadPago`, `CatReglaRiesgoPuesto`, `CatReglaTipoIncapacidad`.
- CFDI 4.0: `CatSatPais`, `CatSatEstado`, `CatSatCodigoPostal`, `CatSatMetodoPago`, `CatSatFormaPago`, `CatSatMoneda`, `CatSatTipoDeComprobante`, `CatSatExportacion`, `CatSatUsoCFDI`, `CatSatClaveProdServ`, `CatSatClaveUnidad`, `CatSatObjetoImp`, `CatSatTipoRelacion`, `RelSatUsoCFDIRegimenFiscal`.
- Documentos/nodos: `DocReciboHorasExtra`, `DocReciboIncapacidad`, `DocReciboOtroPago`, `DocReciboAccionesTitulos`, `DocReciboSeparacionIndemnizacion`, `DocReciboJubilacionPensionRetiro`, `RelReciboSubContratacion`, `RelCFDIRelacionado`, `DocCFDIConcepto`.
- Fiscal/proceso: `CatTablaISRAnual`, `ProcBasePTU`.

## 3. Script SQL completo y contrato de importacion

El codigo completo disponible esta en [NomiMex_Instalacion.sql](NomiMex_Instalacion.sql). Las afirmaciones de esta revision corresponden al artefacto historico indicado arriba y requieren verificacion antes de usarse como garantia sobre el instalador actual.

### Estructura SAT

Los trece catalogos de Nomina del Excel local se conservan como `CatSat...`. Sus campos comunes son:

- ID tecnico con el nombre previo, para mantener referencias de la aplicacion; no es una columna del Excel SAT.
- `Clave` textual y UNIQUE: se conservan ceros iniciales (`001`, `01`, `002`).
- `Descripcion` Unicode de hasta 500 caracteres, sin truncar el catalogo importado.
- `FechaInicioVigencia`, `FechaFinVigencia`, con CHECK de orden.
- `Activo`, bandera **local**, distinta de la vigencia oficial.

Banco incorpora RazonSocial. RegimenFiscal/UsoCFDI conservan Fisica/Moral por separado. Estado conserva la clave de pais en su PK compuesta. CodigoPostal conserva municipio, localidad, estimulo y las columnas de referencia horaria; se cargan **ambas** hojas Parte_1/Parte_2. ClaveUnidad conserva Nombre, Descripcion, Nota y Simbolo. TipoDeComprobante distingue los valores maximos NS/NdS de las subcolumnas de Nomina. Las listas de regimenes permitidos de UsoCFDI se normalizan a `RelSatUsoCFDIRegimenFiscal`.

La estructura usa nombres SQL normalizados equivalentes a los encabezados oficiales; no pretende que ID, Activo o banderas derivadas sean columnas SAT. `CatSatMotivoCancelacion` pertenece al esquema de cancelacion, no a las trece hojas de `cat_Nomina_2026.xls`; su bandera RequiereSustitucion se deriva de la clave 01.

**No hay INSERT de catalogos CatSat en el script entregado**, ni tarifas/UMA/SMG prellenados. Las semillas de roles, permisos, estados internos y configuracion no son catalogos SAT.

### Orden y reglas de carga futura

1. Importar hojas oficiales a un area de staging externa al esquema operativo, preservando todas las columnas y una copia del archivo con fecha/version/hash.
2. Convertir claves numericas de Excel a cadenas con el ancho oficial; no guardar `1.0` ni perder ceros iniciales. RiesgoPuesto admite claves de uno o dos caracteres; TipoNomina y OrigenRecurso son alfabeticos.
3. En hojas sin vigencia por fila, usar la vigencia del **encabezado del catalogo**, no la fecha de importacion ni GETDATE. En columnas existentes vacias, FechaFinVigencia es NULL.
4. Mantener el contrato de IDs existente: para catalogos numericos, el ID tecnico debe conservar el numero historico asociado a la clave. Los catalogos alfabeticos requieren un mapeo estable explicito; nunca regenerar IDs a partir del orden del Excel.
5. Cargar paises -> estados -> codigos postales; regimenes y usos -> relacion UsoCFDI/Regimen; luego los catalogos de Nomina y otros catalogos CFDI.
6. Cargar las cuatro tablas CatRegla por sus IDs SAT; sus datos proceden de politicas y normas laborales, **no** del Excel SAT.
7. Configurar CatConcepto con referencias SAT validas, reglas y textos operativos de hasta 100 caracteres para el XML. El booleano EsGravado no demuestra por si mismo la exencion legal de un concepto.
8. Cargar tarifas mensuales/anuales, subsidio e IMSS del ejercicio y las coberturas temporales de UMA/SMG desde fuentes oficiales verificadas; usar DECIMAL y revisar continuidad/cobertura de intervalos.
9. Ejecutar la carga en una transaccion, validar numero de filas, rechazos, claves duplicadas, FK y vigencia. No usar NOCHECK ni eliminar/recrear catalogos que ya tienen referencias.

Se contrastaron los encabezados del archivo local de Nomina y del Excel CFDI 4.0 enlazado por SAT con publicacion de 6 de noviembre de 2026. No se equipara el nombre local `2026` a una certificacion de origen o autenticidad del archivo; debe verificarse al implementar su importador.

### Despliegue

Crear previamente una base vacia con la collation corporativa aprobada. No cambiar automaticamente la collation de una base existente.

```powershell
sqlcmd -S '.\SQLEXPRESS' -E -I -b -f 65001 -v BaseDatos=NomiMex_Nueva CargarDatosDemostracion=0 -i '.\BaseDatos\NomiMex_Instalacion.sql'
```

El script requiere SQLCMD. **Si la base indicada ya existe, se elimina y se vuelve a crear**; elegir un nombre nuevo. `CargarDatosDemostracion=1` carga datos reales de demostracion y no debe usarse salvo que se necesiten expresamente. Una instalacion interrumpida puede dejar objetos parciales; no se afirma atomicidad de todo el despliegue entre lotes GO.

El ejecutor [Ejecutar.ps1](../Tests/NomiMex.Incidencias.Tests/Ejecutar.ps1) crea una base nueva `NomiMex_Audit_*` sin datos reales, prepara fixtures sinteticos y elimina esa base al terminar correctamente. Si falla, la conserva para diagnostico. En particular:

- CatSatEstado y CatEntidadFederativa deben sincronizarse con una unica fuente oficial.
- La captura debe registrar datos fiscales exactos del receptor y lugar de expedicion, no asumir que el nombre reconstruido coincide con SAT.
- El guardado de prerecibos debe separar TotalPercepciones/TotalOtrosPagos y respetar el nuevo CHECK del neto.
- El generador CFDI debe poblar DocCFDIConcepto, nodos condicionales, fechas, certificado y XML autenticado.
- CatUMA/CatSalarioMinimo requieren VigenciaHasta explicita. Las consultas .NET por anio deben adoptar seleccion por fecha.
- Las consultas que modifican las vistas con joins necesitan actualizar las tablas de reglas; los sinonimos/vistas de lectura no son una API de importacion.

### Verificacion historica

El despliegue y la suite SQL descritos a continuacion se ejecutaron historicamente contra SQL Server Express local 2025 (17.x), en una base exclusiva `NomiMex_Audit_3fbda2f8`. La suite SQL no se ha repetido contra [NomiMex_Instalacion.sql](NomiMex_Instalacion.sql).

**Resultado de metadatos:** 111 tablas, todas con PK; 151 FK y 208 CHECK, todos confiables. Se comprobo que todos los catalogos CatSat estaban vacios inmediatamente despues de instalar.

Las [pruebas dirigidas](../Tests/NomiMex_Auditoria.sql) comprueban: tarifas ausentes, UMA en enero/febrero y vencimiento, tarifa anual independiente, SUM de cuotas, escala vacacional, subsidio causado sin entrega indebida, resumen sin multiplicacion, aguinaldo idempotente, amortizacion individual, cierre repetido, cuadre exacto de centavos, fecha de transmision del Ejecutivo, periodicidad en UPDATE, empresa cruzada, concepto sin SAT o con mapeo incorrecto, rollback de politica, recibo incompleto, PTU sin bases, rangos ISR superpuestos, estado CFDI final sin UUID/XML y vigencias invertidas. Todas finalizaron satisfactoriamente.

No se ejecutaron llamadas a PAC/SAT. Las pruebas .NET mencionadas en este reporte no corresponden al ejecutor aislado agregado posteriormente.

**Evidencia conservada:** [Validacion_NomiMex_Audit_20261113094431.log](../Tests/Resultados/Validacion_NomiMex_Audit_20261113094431.log) registra una reejecucion completa en base nueva (`Modern_Spanish_CI_AS`): hashes SHA-256 del original y del refactorizado, salida del despliegue (exit 0), inventario de metadatos (111 tablas, 0 sin PK, 151 FK, 208 CHECK, 196 DEFAULT, 59 UNIQUE, 0 restricciones no confiables, 28 tablas CatSat con 0 filas, 0 tablas sin prefijo valido), las 26 lineas `OK` de pruebas (exit 0), el rechazo esperado al reejecutar sobre una base con objetos (Msg 51000) y la eliminacion de la base temporal.

## Fuentes verificadas

- [SAT: complemento Nomina 1.2, documentos tecnicos y guia](http://omawww.sat.gob.mx/tramitesyservicios/Paginas/complemento_nomina.htm).
- [SAT: XSD Nomina 1.2](https://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd).
- [SAT: guia de llenado de Nomina](http://omawww.sat.gob.mx/tramitesyservicios/Paginas/documentos/Guia_llenado_Nomina.pdf).
- [SAT: Anexo 20 CFDI 4.0](http://omawww.sat.gob.mx/tramitesyservicios/Paginas/anexo_20.htm).
- [SAT: Excel CFDI 4.0, publicacion 2026-11-06](http://omawww.sat.gob.mx/tramitesyservicios/Paginas/documentos/catCFDI_V_4_20261106.xls).
- [Camara de Diputados: LFT, texto consultado con ultima reforma 2026-05-14](https://www.diputados.gob.mx/LeyesBiblio/pdf/LFT.pdf), arts. 74, 76 y 127. El texto consultado incluye el 25 de diciembre en el art. 74; por eso se conserva, sin aplicar una regla historica desactualizada.
- [Camara de Diputados: LISR](https://www.diputados.gob.mx/LeyesBiblio/pdf/LISR.pdf), arts. 93, 95, 96, 97 y 152.
