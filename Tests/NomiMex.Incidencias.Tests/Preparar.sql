-- Datos exclusivamente sinteticos. Ejecutar solo en una base aislada NomiMex_Audit_*.
:ON ERROR EXIT
IF DB_NAME() NOT LIKE N'NomiMex_Audit[_]%'
    THROW 51900, 'Estas pruebas solo pueden ejecutarse en NomiMex_Audit_<identificador>.', 1;
GO
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
BEGIN TRANSACTION;
DECLARE @Empresa INT, @OtraEmpresa INT, @Periodicidad INT, @OtraPeriodicidad INT, @Ejercicio INT;
INSERT dbo.CatEmpresa(RazonSocial,RFC,RegistroPatronal,RegimenFiscalId,CodigoPostal,LugarExpedicion,Activa)
VALUES(N'Empresa sintetica de pruebas','AAA010101AA1','PRUEBA',601,'00000','00000',1);
SET @Empresa = SCOPE_IDENTITY();
INSERT dbo.CatEmpresa(RazonSocial,RFC,RegistroPatronal,RegimenFiscalId,CodigoPostal,LugarExpedicion,Activa)
VALUES(N'Otra empresa sintetica','AAA010101AA2','PRUEBA2',601,'00000','00000',1);
SET @OtraEmpresa = SCOPE_IDENTITY();
SET @Periodicidad = (SELECT PeriodicidadPagoId FROM dbo.CatPeriodicidadPago WHERE Clave='04');      -- Quincenal
SET @OtraPeriodicidad = (SELECT PeriodicidadPagoId FROM dbo.CatPeriodicidadPago WHERE Clave='02');  -- Semanal
INSERT dbo.RelEmpresaPeriodicidad(EmpresaId,PeriodicidadPagoId,Descripcion,CalculaSeptimoDia)
VALUES(@Empresa,@Periodicidad,N'Prueba quincenal',0),(@Empresa,@OtraPeriodicidad,N'Prueba semanal',0),(@OtraEmpresa,@Periodicidad,N'Prueba otra empresa',0);
IF NOT EXISTS(SELECT 1 FROM dbo.CatEjercicioFiscal WHERE Anio=2026)
    INSERT dbo.CatEjercicioFiscal(Anio) VALUES(2026);
SELECT @Ejercicio=EjercicioId FROM dbo.CatEjercicioFiscal WHERE Anio=2026;
INSERT dbo.CatEmpleado(EmpresaId,NumeroEmpleado,Nombres,ApellidoPaterno,FechaIngreso,Estatus)
VALUES(@Empresa,'PRUEBA1',N'Empleado',N'Sintetico','20260101','ACTIVO'),
      (@Empresa,'PRUEBA2',N'Empleado',N'Inactivo','20260101','BAJA'),
      (@OtraEmpresa,'PRUEBA3',N'Empleado',N'OtraEmpresa','20260101','ACTIVO'),
      (@Empresa,'PRUEBA4',N'Empleado',N'OtraPeriodicidad','20260101','ACTIVO');
INSERT dbo.CatEmpleadoDatosIMSS(EmpleadoId,NSS,SalarioDiario,SalarioDiarioIntegrado,SBC,TipoRiesgo)
SELECT EmpleadoId,RIGHT('00000000000'+CAST(EmpleadoId AS VARCHAR(11)),11),300,315,315,1 FROM dbo.CatEmpleado;
INSERT dbo.CatEmpleadoDatosFiscales(EmpleadoId,RFC,CURP,RegimenFiscalSAT,CodigoPostal)
SELECT EmpleadoId,'AAAA010101'+RIGHT('000'+CAST(EmpleadoId AS VARCHAR(3)),3),
       'AAAA010101HSPXX'+RIGHT('000'+CAST(EmpleadoId AS VARCHAR(3)),3),605,'00000' FROM dbo.CatEmpleado;
INSERT dbo.DocContratoEmpleado(EmpleadoId,TipoContratoId,PeriodicidadPagoId,FechaInicio)
SELECT e.EmpleadoId,(SELECT TipoContratoId FROM dbo.CatTipoContrato WHERE Clave='01'),
       CASE WHEN e.NumeroEmpleado='PRUEBA4' THEN @OtraPeriodicidad ELSE @Periodicidad END,'20260101'
FROM dbo.CatEmpleado e;
INSERT dbo.ProcPeriodoNomina(EmpresaId,EjercicioId,PeriodicidadPagoId,NumeroPeriodo,FechaInicio,FechaFin,FechaPago,DiasPeriodo,EsInicioMes,EsFinMes)
SELECT @Empresa,@Ejercicio,@Periodicidad,n,DATEADD(DAY,(n-1)*15,'20261101'),DATEADD(DAY,n*15-1,'20261101'),DATEADD(DAY,n*15-1,'20261101'),15,
       CASE WHEN n=1 THEN 1 ELSE 0 END,CASE WHEN n=2 THEN 1 ELSE 0 END
FROM (VALUES(1),(2),(3),(4),(5),(6)) x(n);
-- Conceptos permanentes individuales del trabajador PRUEBA1 (apoyo 036 y bono 038 del Catalogo SAT).
DECLARE @Empleado INT=(SELECT EmpleadoId FROM dbo.CatEmpleado WHERE NumeroEmpleado='PRUEBA1');
INSERT dbo.RelEmpleadoIncidencia(IdEmpleado,IdConceptoSAT,CantidadOValor,EsPermanente,Activo)
SELECT @Empleado, c.ConceptoId, v.Cantidad, 1, 1
FROM (VALUES ('036','P',100.0000),('038','P',200.0000)) v(ClaveSAT,Tipo,Cantidad)
JOIN dbo.CatConcepto c ON c.ClaveSAT=v.ClaveSAT AND c.TipoConcepto=v.Tipo AND c.EsIncidencia=1;
-- Paquete base de su periodicidad (sueldo, IMSS, ISR y subsidio): se muestran en la ficha pero no se precargan al periodo.
INSERT dbo.RelEmpleadoIncidencia(IdEmpleado,IdConceptoSAT,CantidadOValor,EsPermanente,Activo)
SELECT @Empleado, rp.IdConceptoSAT, rp.CantidadOValor, rp.EsPermanente, 1
FROM dbo.CatPaqueteIncidencia p
JOIN dbo.RelPaqueteConcepto rp ON rp.IdPaquete=p.IdPaquete AND rp.Activo=1
WHERE p.EsDefault=1 AND p.EmpresaId IS NULL AND p.PeriodicidadPagoId=@Periodicidad;
COMMIT;
GO
EXEC dbo.sp_CargarDatosFiscales @Anio=2026;
GO
