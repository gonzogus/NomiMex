-- ==============================================================================
-- NomiMex - Datos REALES de la empresa (constancia de situación fiscal y recibos de nómina)
-- Ejecutar con sqlcmd -d <base> en una base creada por NomiMex_Instalacion.sql,
-- después de los datos iniciales.
-- Es idempotente y contiene datos reales; no ejecutarlo en pruebas ni en bases con información de negocio.
-- Crea: la empresa JOSE ANTONIO SAUTTO MARTINEZ (Más Farmacia, Santa María del Río, S.L.P.) con periodicidades
--       Semanal (domingo a sábado) y Quincenal, 4 empleadas (3 semanales y 1 quincenal), los periodos del año en curso
--       con sus días festivos de ley.
-- Fuentes: Constancia de situación fiscal (18/08/2025), recibos quincenales #1 y #20, recibos semanales #43 y #45 de 2026.
-- DATOS PENDIENTES (no aparecen en los documentos; se cargan con valores marcados para sustituirlos después):
--   • Registro patronal de la empresa ........... 'PENDIENTE'
--   • Homoclave del RFC de las empleadas ........ 'XXX'
--   • CURP (caracteres 12-18) ................... 'XX', 'XXX', '00' (ej. VEHA951112MXXXXX00); HEVM770501MSP conocido hasta el estado
--   • NSS de las empleadas 001-003 .............. 00000000001 a 00000000003
--   • Código postal / entidad de las empleadas .. se usa el de la empresa (79560, SLP)
-- ==============================================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Anio INT = YEAR(GETDATE());
DECLARE @RFC NVARCHAR(13) = N'SAMA510517S97';
DECLARE @EmpresaId INT = (SELECT EmpresaId FROM dbo.CatEmpresa WHERE RFC = @RFC);

BEGIN TRANSACTION;

-- 1. Empresa activa (persona física con actividad empresarial, régimen 612) -------------------
IF @EmpresaId IS NULL
BEGIN
    INSERT INTO dbo.CatEmpresa (RazonSocial, RFC, CURP, RegistroPatronal, RegimenFiscalId, CodigoPostal, LugarExpedicion,
                                ClaveEntidadFederativa, ClaveOrigenRecurso, Activa)
    VALUES (N'JOSE ANTONIO SAUTTO MARTINEZ', @RFC, N'SAMA510517HGTTRN06', N'H33-11004-10-0', 612, N'79560', N'79560', 'SLP', 'IP', 1);
    SET @EmpresaId = SCOPE_IDENTITY();
END

-- 2. Periodicidades de la empresa (1:N) con sus reglas de generación -------------------------
DECLARE @TipoSem INT = (SELECT PeriodicidadPagoId FROM dbo.CatPeriodicidadPago WHERE Clave = '02');  -- Semanal
DECLARE @TipoQui INT = (SELECT PeriodicidadPagoId FROM dbo.CatPeriodicidadPago WHERE Clave = '04');  -- Quincenal

-- Semana de domingo (7) a sábado (6); se paga el sábado y se trabaja el domingo con prima dominical del 25 %
IF NOT EXISTS (SELECT 1 FROM dbo.RelEmpresaPeriodicidad WHERE EmpresaId = @EmpresaId AND PeriodicidadPagoId = @TipoSem)
    INSERT INTO dbo.RelEmpresaPeriodicidad (EmpresaId, PeriodicidadPagoId, Descripcion, CalculaSeptimoDia, DiaInicio, DiaFin, DiaPago, DiaDescanso,
                                           TrabajaDomingo, PorcentajePrimaDominical)
    VALUES (@EmpresaId, @TipoSem, N'Semanal', 1, 7, 6, 6, 7, 1, 25.00);

IF NOT EXISTS (SELECT 1 FROM dbo.RelEmpresaPeriodicidad WHERE EmpresaId = @EmpresaId AND PeriodicidadPagoId = @TipoQui)
    INSERT INTO dbo.RelEmpresaPeriodicidad (EmpresaId, PeriodicidadPagoId, Descripcion, CalculaSeptimoDia, DiaInicio, DiaFin, DiaPago, DiaDescanso)
    VALUES (@EmpresaId, @TipoQui, N'Quincenal', 0, NULL, NULL, NULL, 7);

-- 3. Estructura organizacional ------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.CatDepartamento WHERE EmpresaId = @EmpresaId AND Clave = N'SMR')
    INSERT INTO dbo.CatDepartamento (EmpresaId, Clave, Descripcion) VALUES (@EmpresaId, N'SMR', N'Santa María del Río');
IF NOT EXISTS (SELECT 1 FROM dbo.CatPuesto WHERE EmpresaId = @EmpresaId AND Clave = N'MOSTRADOR')
    INSERT INTO dbo.CatPuesto (EmpresaId, Clave, Descripcion) VALUES (@EmpresaId, N'MOSTRADOR', N'Empleado de Mostrador');
IF NOT EXISTS (SELECT 1 FROM dbo.CatPuesto WHERE EmpresaId = @EmpresaId AND Clave = N'ENC_FARM')
    INSERT INTO dbo.CatPuesto (EmpresaId, Clave, Descripcion) VALUES (@EmpresaId, N'ENC_FARM', N'Encargado de Farmacia');

DECLARE @DeptoId INT = (SELECT DepartamentoId FROM dbo.CatDepartamento WHERE EmpresaId = @EmpresaId AND Clave = N'SMR');

-- 4. Empleados ---------------------------------------------------------------------------------
DECLARE @TipoContrato INT = (SELECT TipoContratoId FROM dbo.CatTipoContrato WHERE Clave = '01');
DECLARE @TopeSBC DECIMAL(18,4) = 25 * dbo.fn_ObtenerUMADiaria(@Anio);

DECLARE @Emp TABLE (Numero NVARCHAR(20), Nombres NVARCHAR(100), Paterno NVARCHAR(100), Materno NVARCHAR(100),
                    Ingreso DATE, Nacimiento DATE, RFC NVARCHAR(13), CURP NVARCHAR(18), NSS NVARCHAR(11), SD DECIMAL(18,4),
                    PeriodicidadPagoId INT, PuestoClave NVARCHAR(20), Anios INT, Factor DECIMAL(12,6));
-- El nacimiento se toma de la fecha contenida en el RFC. La periodicidad y el salario diario provienen de los recibos.
INSERT INTO @Emp (Numero, Nombres, Paterno, Materno, Ingreso, Nacimiento, RFC, CURP, NSS, SD, PeriodicidadPagoId, PuestoClave) VALUES
 (N'001', N'María Alejandra',  N'Ventura',   N'Hernández', '2015-11-06', '1995-11-12', N'VEHA951112XXX', N'VEHA951112MXXXXX00', N'00000000001', 315.04, @TipoSem, N'MOSTRADOR'),
 (N'002', N'María Oralia',     N'Hernández', N'del Cruz',  '2024-04-07', '1989-12-06', N'HECO891206XXX', N'HECO891206MXXXXX00', N'00000000002', 315.04, @TipoSem, N'MOSTRADOR'),
 (N'003', N'Alicia',           N'Hernández', N'Ventura',   '2025-04-13', '1981-02-09', N'HEVA810209XXX', N'HEVA810209MXXXXX00', N'00000000003', 315.04, @TipoSem, N'MOSTRADOR'),
 (N'011', N'María Guadalupe',  N'Hernández', N'Ventura',   '2007-03-14', '1977-05-01', N'HEVM770501XXX', N'HEVM770501MSPXXX00', N'41937716946', 350.00, @TipoQui, N'ENC_FARM');

-- Años completos de servicio y factor de integración (15 días de aguinaldo, 25 % de prima vacacional, Art. 76 LFT)
UPDATE @Emp
SET Anios  = DATEDIFF(YEAR, Ingreso, GETDATE()) - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, Ingreso, GETDATE()), Ingreso) > GETDATE() THEN 1 ELSE 0 END;
UPDATE @Emp SET Factor = dbo.fn_FactorIntegracion(Anios, 15, 25, 0, SD);

INSERT INTO dbo.CatEmpleado (EmpresaId, NumeroEmpleado, Nombres, ApellidoPaterno, ApellidoMaterno, FechaIngreso, FechaNacimiento, Sexo, Estatus)
SELECT @EmpresaId, e.Numero, e.Nombres, e.Paterno, e.Materno, e.Ingreso, e.Nacimiento, 'M', 'ACTIVO'
FROM @Emp e
WHERE NOT EXISTS (SELECT 1 FROM dbo.CatEmpleado x WHERE x.EmpresaId = @EmpresaId AND x.NumeroEmpleado = e.Numero);

INSERT INTO dbo.CatEmpleadoDatosFiscales (EmpleadoId, RFC, CURP, RegimenFiscalSAT, CodigoPostal, ClaveEntidadFederativa)
SELECT m.EmpleadoId, e.RFC, e.CURP, 605, N'79560', 'SLP'
FROM @Emp e INNER JOIN dbo.CatEmpleado m ON m.EmpresaId = @EmpresaId AND m.NumeroEmpleado = e.Numero
WHERE NOT EXISTS (SELECT 1 FROM dbo.CatEmpleadoDatosFiscales f WHERE f.EmpleadoId = m.EmpleadoId);

INSERT INTO dbo.CatEmpleadoDatosIMSS (EmpleadoId, NSS, SalarioDiario, SalarioDiarioIntegrado, SBC, TipoRiesgo)
SELECT m.EmpleadoId, e.NSS, e.SD, ROUND(e.SD * e.Factor, 4),
       CASE WHEN ROUND(e.SD * e.Factor, 4) > @TopeSBC THEN @TopeSBC ELSE ROUND(e.SD * e.Factor, 4) END, 1
FROM @Emp e INNER JOIN dbo.CatEmpleado m ON m.EmpresaId = @EmpresaId AND m.NumeroEmpleado = e.Numero
WHERE NOT EXISTS (SELECT 1 FROM dbo.CatEmpleadoDatosIMSS i WHERE i.EmpleadoId = m.EmpleadoId);

INSERT INTO dbo.DocContratoEmpleado (EmpleadoId, TipoContratoId, DepartamentoId, PuestoId, TipoJornadaId, PeriodicidadPagoId,
                                     TipoRegimenId, SalarioBaseCotizacion, SalarioDiarioIntegrado, ZonaSMG, FechaInicio, Activo)
SELECT m.EmpleadoId, @TipoContrato, @DeptoId, p.PuestoId, 1, e.PeriodicidadPagoId,
       (SELECT TipoRegimenId FROM dbo.CatTipoRegimen WHERE Clave = '02'),
       i.SBC, i.SalarioDiarioIntegrado, 'GENERAL', e.Ingreso, 1
FROM @Emp e
       INNER JOIN dbo.CatEmpleado m ON m.EmpresaId = @EmpresaId AND m.NumeroEmpleado = e.Numero
       INNER JOIN dbo.CatEmpleadoDatosIMSS i ON i.EmpleadoId = m.EmpleadoId
INNER JOIN dbo.CatPuesto p ON p.EmpresaId = @EmpresaId AND p.Clave = e.PuestoClave
WHERE NOT EXISTS (SELECT 1 FROM dbo.DocContratoEmpleado c WHERE c.EmpleadoId = m.EmpleadoId AND c.Activo = 1);

INSERT INTO dbo.HistEmpleadoSalario (EmpleadoId, FechaEfectiva, SalarioDiario, SDI, SBC, FactorIntegracion, MotivoCambio)
SELECT m.EmpleadoId, e.Ingreso, i.SalarioDiario, i.SalarioDiarioIntegrado, i.SBC, e.Factor, N'Alta de empleado (datos reales)'
FROM @Emp e
INNER JOIN dbo.CatEmpleado m ON m.EmpresaId = @EmpresaId AND m.NumeroEmpleado = e.Numero
INNER JOIN dbo.CatEmpleadoDatosIMSS i ON i.EmpleadoId = m.EmpleadoId
WHERE NOT EXISTS (SELECT 1 FROM dbo.HistEmpleadoSalario h WHERE h.EmpleadoId = m.EmpleadoId);

-- 5. Periodos del año (se omiten los que ya existen) ---------------------------------
DECLARE @EjercicioId INT = (SELECT EjercicioId FROM dbo.CatEjercicioFiscal WHERE Anio = @Anio);
IF @EjercicioId IS NULL
BEGIN
    INSERT INTO dbo.CatEjercicioFiscal (Anio, Estatus, JornadaSemanalHoras) VALUES (@Anio, 'ABIERTO', CASE WHEN @Anio >= 2027 THEN 46 ELSE 48 END);
    SET @EjercicioId = SCOPE_IDENTITY();
END

DECLARE @PerQui INT = @TipoQui;
DECLARE @PerSem INT = @TipoSem;
DECLARE @Ene1 DATE = DATEFROMPARTS(@Anio, 1, 1);
DECLARE @Dic31 DATE = DATEFROMPARTS(@Anio, 12, 31);

DECLARE @Per TABLE (PeriodicidadPagoId INT, Numero INT, Inicio DATE, Fin DATE, Pago DATE, Dias DECIMAL(5,2), Septimo BIT);

-- Quincenal: 24 periodos (1-15 y 16-fin de mes real); la quincena paga el último día
INSERT INTO @Per
SELECT @PerQui, (m.n - 1) * 2 + q.k,
       DATEFROMPARTS(@Anio, m.n, CASE q.k WHEN 1 THEN 1 ELSE 16 END),
       CASE q.k WHEN 1 THEN DATEFROMPARTS(@Anio, m.n, 15) ELSE EOMONTH(DATEFROMPARTS(@Anio, m.n, 1)) END,
       CASE q.k WHEN 1 THEN DATEFROMPARTS(@Anio, m.n, 15) ELSE EOMONTH(DATEFROMPARTS(@Anio, m.n, 1)) END,
       15, 0
FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12)) m(n)
CROSS JOIN (VALUES (1),(2)) q(k);

-- Semanal (domingo a sábado): la semana pertenece al ejercicio en que termina; el primer periodo contiene el 1 de enero
-- 19000107 fue domingo, por lo que el residuo de días contra esa fecha es la distancia al domingo anterior.
DECLARE @InicioSem DATE = DATEADD(DAY, -(DATEDIFF(DAY, '19000107', @Ene1) % 7), @Ene1);
INSERT INTO @Per
SELECT @PerSem, ROW_NUMBER() OVER (ORDER BY s.Inicio), s.Inicio, DATEADD(DAY, 6, s.Inicio),
       DATEADD(DAY, 6, s.Inicio),    -- se paga el sábado, último día del periodo
       7, 1
FROM (SELECT DATEADD(DAY, 7 * v.n, @InicioSem) AS Inicio
      FROM (SELECT TOP (54) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n FROM sys.all_objects) v) s
WHERE DATEADD(DAY, 6, s.Inicio) <= @Dic31;

INSERT INTO dbo.ProcPeriodoNomina (EmpresaId, EjercicioId, PeriodicidadPagoId, NumeroPeriodo, FechaInicio, FechaFin,
                                   FechaPago, DiasPeriodo, EsInicioMes, EsFinMes, DiaDescanso, CalculaSeptimoDia, Estatus)
SELECT @EmpresaId, @EjercicioId, p.PeriodicidadPagoId, p.Numero, p.Inicio, p.Fin, p.Pago, p.Dias,
       CASE WHEN DAY(p.Inicio) = 1 OR MONTH(p.Inicio) <> MONTH(p.Fin) THEN 1 ELSE 0 END,
       CASE WHEN p.Fin = EOMONTH(p.Fin) OR MONTH(p.Inicio) <> MONTH(p.Fin) THEN 1 ELSE 0 END,
       7, p.Septimo, 'CREADO'
FROM @Per p
WHERE NOT EXISTS (SELECT 1 FROM dbo.ProcPeriodoNomina x
                  WHERE x.EmpresaId = @EmpresaId AND x.EjercicioId = @EjercicioId AND x.PeriodicidadPagoId = p.PeriodicidadPagoId AND x.NumeroPeriodo = p.Numero);

-- 6. Días festivos de ley y vínculo con los periodos -------------------------------------
EXEC dbo.sp_GenerarDiasFestivosLey @Anio = @Anio;
EXEC dbo.sp_VincularFestivosPeriodos @EmpresaId = @EmpresaId;

-- 7. Paquete base de incidencias de la periodicidad de cada empleado (lo mismo que hace el alta desde la aplicación) -----
INSERT INTO dbo.RelEmpleadoIncidencia (IdEmpleado, IdConceptoSAT, CantidadOValor, EsPermanente, Observaciones, Activo)
SELECT e.EmpleadoId, rp.IdConceptoSAT, rp.CantidadOValor, rp.EsPermanente, NULL, 1
FROM dbo.CatEmpleado e
INNER JOIN dbo.DocContratoEmpleado c ON c.EmpleadoId = e.EmpleadoId AND c.Activo = 1
INNER JOIN dbo.CatPaqueteIncidencia p ON p.EsDefault = 1 AND p.Activo = 1 AND p.EmpresaId IS NULL AND p.PeriodicidadPagoId = c.PeriodicidadPagoId
INNER JOIN dbo.RelPaqueteConcepto rp ON rp.IdPaquete = p.IdPaquete AND rp.Activo = 1
WHERE e.EmpresaId = @EmpresaId
  AND NOT EXISTS (SELECT 1 FROM dbo.RelEmpleadoIncidencia r WHERE r.IdEmpleado = e.EmpleadoId AND r.IdConceptoSAT = rp.IdConceptoSAT);

COMMIT;

SELECT 'Empresa' AS Dato, CAST(COUNT(*) AS NVARCHAR(20)) AS Total FROM dbo.CatEmpresa WHERE EmpresaId = @EmpresaId
UNION ALL SELECT 'Periodicidades', CAST(COUNT(*) AS NVARCHAR(20)) FROM dbo.RelEmpresaPeriodicidad WHERE EmpresaId = @EmpresaId AND Activo = 1
UNION ALL SELECT 'Empleados', CAST(COUNT(*) AS NVARCHAR(20)) FROM dbo.CatEmpleado WHERE EmpresaId = @EmpresaId
UNION ALL SELECT 'Periodos quincenales', CAST(COUNT(*) AS NVARCHAR(20)) FROM dbo.ProcPeriodoNomina WHERE EmpresaId = @EmpresaId AND PeriodicidadPagoId = @TipoQui AND EjercicioId = @EjercicioId
UNION ALL SELECT 'Periodos semanales', CAST(COUNT(*) AS NVARCHAR(20)) FROM dbo.ProcPeriodoNomina WHERE EmpresaId = @EmpresaId AND PeriodicidadPagoId = @TipoSem AND EjercicioId = @EjercicioId
UNION ALL SELECT 'Festivos vinculados', CAST(COUNT(*) AS NVARCHAR(20)) FROM dbo.RelPeriodoDiaFestivo rf INNER JOIN dbo.ProcPeriodoNomina p ON p.PeriodoId = rf.PeriodoId WHERE p.EmpresaId = @EmpresaId;
