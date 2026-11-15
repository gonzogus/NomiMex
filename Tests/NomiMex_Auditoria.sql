:ON ERROR EXIT
-- Solo para una base exclusiva de pruebas ya instalada y todavia sin datos de negocio.
-- Los valores siguientes son fixtures sinteticos, NO tarifas ni catalogos oficiales.
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
IF DB_NAME() NOT LIKE N'NomiMex_Audit[_]%'
    THROW 51900, 'Estas pruebas solo pueden ejecutarse en NomiMex_Audit_<identificador>.', 1;
IF EXISTS (SELECT 1 FROM dbo.CatEmpleado)
    THROW 51900, 'La base de auditoria ya contiene empleados. No se altera.', 1;
GO
CREATE PROCEDURE #ExigirError @SQL NVARCHAR(MAX), @Numero INT, @Caso NVARCHAR(100)
AS
BEGIN
    BEGIN TRY
        EXEC sys.sp_executesql @SQL;
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() <> @Numero
        BEGIN
            ;THROW;
        END
        IF @@TRANCOUNT <> 0 THROW 51901, 'Una validacion dejo una transaccion abierta.', 1;
        PRINT CONCAT('OK: ',@Caso);
        RETURN;
    END CATCH;
    THROW 51901, 'La operacion invalida no fue rechazada.', 1;
END
GO
-- Enlazar todas las rutinas despues del despliegue descubre dependencias diferidas.
DECLARE @Objeto NVARCHAR(517);
DECLARE objetos CURSOR LOCAL FAST_FORWARD FOR
    SELECT QUOTENAME(OBJECT_SCHEMA_NAME(object_id))+N'.'+QUOTENAME(name)
    FROM sys.objects WHERE type IN ('P','FN','IF','TF','V','TR') AND is_ms_shipped = 0;
OPEN objetos;
FETCH NEXT FROM objetos INTO @Objeto;
WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC sys.sp_refreshsqlmodule @name = @Objeto;
    FETCH NEXT FROM objetos INTO @Objeto;
END
CLOSE objetos;
DEALLOCATE objetos;
PRINT 'OK: enlace de todas las rutinas y vistas';

IF dbo.fn_CalcularISRMensual(100,999) IS NOT NULL
    THROW 51901, 'La falta de tarifa ISR produjo un cero silencioso.', 1;
IF dbo.fn_ObtenerUMADiariaFecha('20260101') IS NOT NULL
    THROW 51901, 'UMA sin importar no debe tener valor.', 1;
PRINT 'OK: parametros fiscales ausentes no producen valores inventados';
EXEC #ExigirError N'EXEC dbo.sp_CargarDatosFiscales @Anio=2026;',51010,N'carga fiscal sin tarifas';

INSERT dbo.CatSatRegimenFiscal(RegimenFiscalId,Clave,Descripcion,AplicaPersonaFisica,AplicaPersonaMoral,FechaInicioVigencia)
VALUES(601,N'601',N'Fixture emisor',0,1,'20200101'),(605,N'605',N'Fixture receptor',1,0,'20200101');
INSERT dbo.CatSatTipoNomina(TipoNominaId,Clave,Descripcion,FechaInicioVigencia)
VALUES(1,N'O',N'Fixture ordinaria','20200101');
INSERT dbo.CatSatTipoJornada(TipoJornadaId,Clave,Descripcion,FechaInicioVigencia)
VALUES(1,N'01',N'Fixture jornada','20200101');
INSERT dbo.CatSatTipoContrato(TipoContratoId,Clave,Descripcion,FechaInicioVigencia)
VALUES(1,N'01',N'Fixture contrato','20200101');
INSERT dbo.CatSatTipoRegimen(TipoRegimenId,Clave,Descripcion,FechaInicioVigencia)
VALUES(2,N'02',N'Fixture regimen','20200101');
INSERT dbo.CatSatPeriodicidadPago(PeriodicidadPagoId,Clave,Descripcion,FechaInicioVigencia)
VALUES(4,N'04',N'Fixture quincena','20200101'),(2,N'02',N'Fixture semana','20200101'),(3,N'03',N'Fixture catorcena','20200101');
INSERT dbo.CatSatRiesgoPuesto(RiesgoPuestoId,Clave,Descripcion,FechaInicioVigencia)
VALUES(1,N'1',N'Fixture riesgo','20200101');
INSERT dbo.CatSatTipoPercepcion(TipoPercepcionId,Clave,Descripcion,FechaInicioVigencia)
VALUES(1,N'001',N'Fixture sueldo','20200101'),(2,N'002',N'Fixture aguinaldo','20200101');
INSERT dbo.CatSatTipoDeduccion(TipoDeduccionId,Clave,Descripcion,FechaInicioVigencia)
VALUES(1,N'001',N'Fixture cuota','20200101'),(2,N'002',N'Fixture ISR','20200101'),(12,N'012',N'Fixture prestamo','20200101');
INSERT dbo.CatSatTipoOtroPago(TipoOtroPagoId,Clave,Descripcion,FechaInicioVigencia)
VALUES(2,N'002',N'Fixture subsidio','20200101');
INSERT dbo.CatSatPais(Clave,Descripcion,FechaInicioVigencia) VALUES(N'MEX',N'Fixture pais','20200101');
INSERT dbo.CatSatEstado(Clave,ClavePais,Descripcion,FechaInicioVigencia) VALUES(N'AGU',N'MEX',N'Fixture estado','20200101');
INSERT dbo.CatSatCodigoPostal(Clave,ClaveEstado,EstimuloFranjaFronteriza,FechaInicioVigencia)
VALUES(N'00000',N'AGU',0,'20200101');
INSERT dbo.CatSatMetodoPago(Clave,Descripcion,FechaInicioVigencia) VALUES(N'PUE',N'Fixture metodo','20200101');
INSERT dbo.CatSatTipoDeComprobante(Clave,Descripcion,FechaInicioVigencia) VALUES(N'N',N'Fixture comprobante','20200101');
INSERT dbo.CatSatUsoCFDI(Clave,Descripcion,AplicaPersonaFisica,AplicaPersonaMoral,FechaInicioVigencia)
VALUES(N'CN01',N'Fixture uso',1,0,'20200101');
INSERT dbo.CatSatMoneda(Clave,Descripcion,Decimales,FechaInicioVigencia) VALUES(N'MXN',N'Fixture moneda',2,'20200101');
INSERT dbo.CatSatExportacion(Clave,Descripcion,FechaInicioVigencia) VALUES(N'01',N'Fixture exportacion','20200101');
INSERT dbo.CatUMA(Anio,ValorDiario,ValorMensual,ValorAnual,VigenciaDesde,VigenciaHasta)
VALUES(2025,100,3000,36000,'20250201','20260131'),(2026,200,6000,72000,'20260201','20270131');
IF dbo.fn_ObtenerUMADiariaFecha('20260131') <> 100 OR dbo.fn_ObtenerUMADiariaFecha('20260201') <> 200
    THROW 51901, 'Seleccion incorrecta de la UMA en enero/febrero.', 1;
IF dbo.fn_ObtenerUMADiariaFecha('20280101') IS NOT NULL
    THROW 51901, 'Se reutilizo una UMA expirada.', 1;
PRINT 'OK: vigencia efectiva UMA, incluido enero y vencimiento';

DECLARE @Ejercicio INT;
INSERT dbo.CatEjercicioFiscal(Anio) VALUES(2026);
SET @Ejercicio=SCOPE_IDENTITY();
INSERT dbo.CatTablaISR(EjercicioId,Renglon,LimiteInferior,LimiteSuperior,CuotaFija,TasaExcedente)
VALUES(@Ejercicio,1,0,999999,0,0.1);
INSERT dbo.CatTablaISRAnual(EjercicioId,Renglon,LimiteInferior,LimiteSuperior,CuotaFija,TasaExcedente)
VALUES(@Ejercicio,1,0,999999,0,0.2);
IF dbo.fn_CalcularISRAnual(1000,@Ejercicio) <> 200
    THROW 51901, 'ISR anual sigue derivandose de la tarifa mensual.', 1;
PRINT 'OK: tarifa anual independiente';
INSERT dbo.CatTablaSubsidioEmpleo(EjercicioId,HastaIngreso,MontoSubsidio)
VALUES(@Ejercicio,999999,100);
INSERT dbo.CatCuotaIMSS(EjercicioId,ClaveConcepto,Descripcion,PorcentajePatronal,PorcentajeObrero,BaseCalculo)
VALUES(@Ejercicio,N'IV',N'Fixture 1',0.02,0.01,N'SBC'),
      (@Ejercicio,N'CEV',N'Fixture 2',0.03,0.02,N'SBC');
IF dbo.fn_CuotaIMSS_Obrero(100,@Ejercicio,2026,10) <> 30
    OR dbo.fn_CuotaIMSS_Patron(100,@Ejercicio,2026,10) <> 50
    THROW 51901, 'Las cuotas no agregan todas las filas.', 1;
PRINT 'OK: suma set-based de cuotas IMSS';
IF dbo.fn_DiasVacaciones(5) <> 20 OR dbo.fn_DiasVacaciones(6) <> 22 OR dbo.fn_DiasVacaciones(10) <> 22
    OR dbo.fn_DiasVacaciones(11) <> 24
    THROW 51901, 'Escala vacacional incorrecta en limites quinquenales.', 1;
PRINT 'OK: escala vacacional del sexto anio y quinquenios';

DECLARE @Empresa INT,@OtraEmpresa INT,@Empleado INT,@OtroEmpleado INT,@Periodo INT,@Recibo INT;
INSERT dbo.CatEmpresa(RazonSocial,RFC,RegistroPatronal,RegimenFiscalId,CodigoPostal,LugarExpedicion)
VALUES(N'Fixture uno',N'AAA010101AA1',N'RP1',601,N'00000',N'00000');
SET @Empresa=SCOPE_IDENTITY();
INSERT dbo.CatEmpresa(RazonSocial,RFC,RegistroPatronal,RegimenFiscalId,CodigoPostal,LugarExpedicion)
VALUES(N'Fixture dos',N'AAA010101AA2',N'RP2',601,N'00000',N'00000');
SET @OtraEmpresa=SCOPE_IDENTITY();
INSERT dbo.RelEmpresaPeriodicidad(EmpresaId,PeriodicidadPagoId) VALUES(@Empresa,4),(@OtraEmpresa,4);
INSERT dbo.CatEmpleado(EmpresaId,NumeroEmpleado,Nombres,ApellidoPaterno,FechaIngreso)
VALUES(@Empresa,N'UNO',N'Fixture',N'Uno','20260101');
SET @Empleado=SCOPE_IDENTITY();
INSERT dbo.CatEmpleado(EmpresaId,NumeroEmpleado,Nombres,ApellidoPaterno,FechaIngreso)
VALUES(@OtraEmpresa,N'DOS',N'Fixture',N'Dos','20260101');
SET @OtroEmpleado=SCOPE_IDENTITY();
INSERT dbo.CatEmpleadoDatosIMSS(EmpleadoId,NSS,SalarioDiario,SalarioDiarioIntegrado,SBC,TipoRiesgo)
VALUES(@Empleado,N'00000000001',100,110,110,1);
INSERT dbo.ProcPrestamo(EmpleadoId,MontoTotal,SaldoRestante,DescuentoPorPeriodo)
VALUES(@Empleado,200,200,10),(@Empleado,400,400,20);
INSERT dbo.DocContratoEmpleado(EmpleadoId,TipoContratoId,FechaInicio)
VALUES(@Empleado,1,'20260101');
INSERT dbo.CatConcepto(ClaveSAT,TipoConcepto,Descripcion,TipoPercepcionId)
VALUES(N'001','P',N'Fixture sueldo',1),(N'002','P',N'Fixture aguinaldo',2);
INSERT dbo.CatConcepto(ClaveSAT,TipoConcepto,Descripcion,TipoDeduccionId)
VALUES(N'001','D',N'Fixture IMSS',1),(N'002','D',N'Fixture ISR',2),(N'012','D',N'Fixture prestamo',12);
INSERT dbo.CatConcepto(ClaveSAT,TipoConcepto,Descripcion,TipoOtroPagoId)
VALUES(N'002','O',N'Fixture subsidio',2);
INSERT dbo.ProcPeriodoNomina(EmpresaId,EjercicioId,PeriodicidadPagoId,NumeroPeriodo,FechaInicio,FechaFin,FechaPago,DiasPeriodo)
VALUES(@Empresa,@Ejercicio,4,1,'20261101','20261115','20261115',15);
SET @Periodo=SCOPE_IDENTITY();
EXEC dbo.sp_CalcularNomina @Periodo;
SELECT @Recibo=ReciboId FROM dbo.DocReciboNomina WHERE PeriodoId=@Periodo;
IF (SELECT TotalOtrosPagos FROM dbo.DocReciboNomina WHERE ReciboId=@Recibo) <> 0
    THROW 51901, 'El subsidio causado se entrego indebidamente como efectivo.', 1;
IF NOT EXISTS(SELECT 1 FROM dbo.DocReciboOtroPago o JOIN dbo.DocReciboDetalle d ON d.DetalleId=o.DetalleId
              WHERE d.ReciboId=@Recibo AND o.SubsidioCausado > 0)
    THROW 51901, 'No se conservo el subsidio causado.', 1;
PRINT 'OK: subsidio causado separado de efectivo y percepciones';
IF EXISTS (SELECT 1 FROM dbo.vw_ResumenPeriodo v JOIN dbo.DocReciboNomina r ON r.PeriodoId=v.PeriodoId
           WHERE r.ReciboId=@Recibo AND (v.TotalEmpleados<>1 OR v.SumaPercepciones<>r.TotalPercepciones OR v.SumaNetoPagar<>r.NetoPagar))
    THROW 51901, 'Resumen multiplicado por filas de detalle.', 1;
PRINT 'OK: resumen sin multiplicacion de importes';
EXEC dbo.sp_CalcularAguinaldo @Ejercicio,@Periodo;
DECLARE @TotalAntes DECIMAL(18,2)=(SELECT TotalPercepciones FROM dbo.DocReciboNomina WHERE ReciboId=@Recibo);
EXEC dbo.sp_CalcularAguinaldo @Ejercicio,@Periodo;
IF (SELECT TotalPercepciones FROM dbo.DocReciboNomina WHERE ReciboId=@Recibo) <> @TotalAntes
    THROW 51901, 'Aguinaldo duplicado por segunda ejecucion.', 1;
PRINT 'OK: aguinaldo idempotente';
EXEC dbo.sp_AutorizarPeriodo @Periodo;
EXEC dbo.sp_CerrarPeriodo @Periodo;
IF (SELECT SUM(SaldoRestante) FROM dbo.ProcPrestamo WHERE EmpleadoId=@Empleado) <> 570
    THROW 51901, 'Cada prestamo recibio el descuento agregado de todos los prestamos.', 1;
PRINT 'OK: descuento individual de multiples prestamos';
DECLARE @AcumuladoAntes DECIMAL(18,4)=(SELECT TotalPercepciones FROM dbo.ProcAcumuladoMensual WHERE EmpleadoId=@Empleado);
DECLARE @CerrarSQL NVARCHAR(MAX)=CONCAT(N'EXEC dbo.sp_CerrarPeriodo @PeriodoId=',@Periodo);
EXEC #ExigirError @CerrarSQL,51013,N'cierre repetido rechazado';
IF (SELECT TotalPercepciones FROM dbo.ProcAcumuladoMensual WHERE EmpleadoId=@Empleado) <> @AcumuladoAntes
    THROW 51901, 'Un segundo cierre duplico los acumulados.', 1;
PRINT 'OK: acumulados no duplicados por cierre repetido';
INSERT dbo.RelEmpresaPeriodicidad(EmpresaId,PeriodicidadPagoId) VALUES(@Empresa,2);
UPDATE dbo.DocContratoEmpleado SET Activo=0 WHERE EmpleadoId=@Empleado;
INSERT dbo.DocContratoEmpleado(EmpleadoId,TipoContratoId,PeriodicidadPagoId,FechaInicio)
VALUES(@Empleado,1,2,'20261116');
UPDATE dbo.CatEmpleadoDatosIMSS SET SalarioDiario=100.004 WHERE EmpleadoId=@Empleado;
DECLARE @PeriodoSemanal INT;
INSERT dbo.ProcPeriodoNomina(EmpresaId,EjercicioId,PeriodicidadPagoId,NumeroPeriodo,FechaInicio,FechaFin,FechaPago,DiasPeriodo,DiaDescanso,CalculaSeptimoDia)
VALUES(@Empresa,@Ejercicio,2,1,'20261116','20261122','20261122',7,7,1);
SET @PeriodoSemanal=SCOPE_IDENTITY();
EXEC dbo.sp_CalcularNomina @PeriodoSemanal;
IF (SELECT TotalPercepciones FROM dbo.DocReciboNomina WHERE PeriodoId=@PeriodoSemanal) <> 700.02
    THROW 51901, 'La cabecera no suma los conceptos monetarios redondeados individualmente.', 1;
PRINT 'OK: redondeo financiero exacto de sueldo y septimo dia';
EXEC dbo.sp_GenerarDiasFestivosLey 2030;
IF NOT EXISTS (SELECT 1 FROM dbo.CatDiaFestivo WHERE Fecha='20301001')
    OR EXISTS (SELECT 1 FROM dbo.CatDiaFestivo WHERE Fecha='20301201')
    THROW 51901, 'Fecha incorrecta para la transmision del Ejecutivo.', 1;
PRINT 'OK: transmision del Ejecutivo el primero de octubre';

DECLARE @SQL NVARCHAR(MAX);
SET @SQL=CONCAT(N'UPDATE dbo.ProcPeriodoNomina SET PeriodicidadPagoId=3 WHERE PeriodoId=',@Periodo);
EXEC #ExigirError @SQL,547,N'periodicidad no asignada en UPDATE';
SET @SQL=CONCAT(N'INSERT dbo.DocReciboNomina(PeriodoId,EmpleadoId,TotalPercepciones,TotalDeducciones,NetoPagar) VALUES(',@Periodo,N',',@OtroEmpleado,N',100,0,100)');
EXEC #ExigirError @SQL,51020,N'recibo de otra empresa';
EXEC #ExigirError N'INSERT dbo.CatConcepto(ClaveSAT,TipoConcepto,Descripcion) VALUES(N''099'',''P'',N''Sin catalogo'');',547,N'concepto sin referencia SAT';
EXEC #ExigirError N'INSERT dbo.CatConcepto(ClaveSAT,TipoConcepto,Descripcion,TipoPercepcionId) VALUES(N''099'',''P'',N''Mapeo distinto'',1);',51020,N'clave y referencia SAT distintas';
SET @SQL=CONCAT(N'EXEC dbo.sp_GestionarPoliticaEmpresarial @Accion=N''INSERT'',@EmpresaId=',@Empresa,N',@DiasAguinaldo=1;');
EXEC #ExigirError @SQL,51011,N'politica invalida sin transaccion abierta';
SET @SQL=CONCAT(N'EXEC dbo.sp_ValidarReciboTimbrado @ReciboId=',@Recibo);
EXEC #ExigirError @SQL,51030,N'recibo incompleto no timbrable';
SET @SQL=CONCAT(N'EXEC dbo.sp_CalcularPTU @EjercicioId=',@Ejercicio,N',@UtilidadRepartible=1000,@EmpresaId=',@Empresa);
EXEC #ExigirError @SQL,51040,N'PTU sin bases reales validadas';
SET @SQL=CONCAT(N'INSERT dbo.CatTablaISR(EjercicioId,Renglon,LimiteInferior,LimiteSuperior,CuotaFija,TasaExcedente) VALUES(',@Ejercicio,N',2,100,200,0,0.1);');
EXEC #ExigirError @SQL,51022,N'rangos ISR superpuestos';
SET @SQL=CONCAT(N'UPDATE dbo.DocCFDINomina SET EstadoCFDI=3 WHERE ReciboId=',@Recibo);
EXEC #ExigirError @SQL,547,N'CFDI vigente sin UUID ni XML';
EXEC #ExigirError N'INSERT dbo.CatSatTipoHoras(TipoHorasId,Clave,Descripcion,FechaInicioVigencia,FechaFinVigencia) VALUES(1,N''01'',N''Fixture'',''20260102'',''20260101'');',547,N'vigencia invertida';
PRINT 'OK: auditoria SQL completada';
GO
