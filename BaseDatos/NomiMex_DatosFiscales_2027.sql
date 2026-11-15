-- =============================================================================
-- NomiMex - Carga de parámetros fiscales del ejercicio 2027
-- -----------------------------------------------------------------------------
-- Prepara los valores que cambian cada año para que el motor los aplique por
-- VIGENCIA (según la fecha fin de cada periodo). La UMA y el salario mínimo SIEMPRE
-- cambian: captúralos abajo con los valores oficiales. Las tarifas ISR, el subsidio
-- y las cuotas IMSS se cargan como BASE PROVISIONAL (reutilizan la tarifa 2026
-- vigente); si el DOF publica cambios para 2027, actualízalos (ver sección 5).
--
-- Requiere que la base ya esté instalada (NomiMex_Instalacion.sql).
-- Ejecútalo con:  sqlcmd -S .\SQLEXPRESS -E -I -d NomiMex -i NomiMex_DatosFiscales_2027.sql
-- (archivo en UTF-8 con BOM; si tu sqlcmd no respeta el BOM, añade -f 65001)
-- =============================================================================
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

-- =============================================================================
-- 1) CAPTURA AQUÍ LOS VALORES OFICIALES 2027  (mientras estén en NULL, no carga nada)
-- =============================================================================
DECLARE @UMA_Diaria_2027   DECIMAL(12,4) = NULL;  -- INEGI (DOF ~ene-2027). Vigencia 2027-02-01 a 2028-01-31
DECLARE @UMA_Mensual_2027  DECIMAL(12,4) = NULL;  -- INEGI (valor mensual publicado, no derivado)
DECLARE @UMA_Anual_2027    DECIMAL(14,4) = NULL;  -- INEGI (valor anual publicado)
DECLARE @SMG_General_2027  DECIMAL(12,4) = NULL;  -- CONASAMI (DOF ~dic-2026). Vigencia 2027-01-01 a 2027-12-31
DECLARE @SMG_ZFN_2027      DECIMAL(12,4) = NULL;  -- CONASAMI - Zona Libre de la Frontera Norte

IF @UMA_Diaria_2027 IS NULL OR @UMA_Mensual_2027 IS NULL OR @UMA_Anual_2027 IS NULL
   OR @SMG_General_2027 IS NULL OR @SMG_ZFN_2027 IS NULL
BEGIN
    PRINT '>>> Falta capturar los valores 2027 (UMA diaria/mensual/anual y SMG General/ZFN) en la sección 1.';
    PRINT '>>> No se cargó nada. Edita el script con los valores oficiales del DOF y vuelve a ejecutarlo.';
    RETURN;
END

-- =============================================================================
-- 2) Ejercicio fiscal 2027 (semana laboral de 46 horas, reforma vigente desde 2027)
-- =============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.CatEjercicioFiscal WHERE Anio = 2027)
    INSERT INTO dbo.CatEjercicioFiscal (Anio, Estatus, JornadaSemanalHoras)
    VALUES (2027, 'ABIERTO', 46);

-- =============================================================================
-- 3) UMA 2027  (la UMA cambia el 1 de febrero: vigencia 2027-02-01 a 2028-01-31)
-- =============================================================================
DELETE FROM dbo.CatUMA WHERE Anio = 2027;
INSERT INTO dbo.CatUMA (Anio, ValorDiario, ValorMensual, ValorAnual, VigenciaDesde, VigenciaHasta)
VALUES (2027, @UMA_Diaria_2027, @UMA_Mensual_2027, @UMA_Anual_2027, '2027-02-01', '2028-01-31');

-- =============================================================================
-- 4) Salario mínimo 2027  (cambia el 1 de enero: vigencia 2027-01-01 a 2027-12-31)
-- =============================================================================
DELETE FROM dbo.CatSalarioMinimo WHERE Anio = 2027;
INSERT INTO dbo.CatSalarioMinimo (Anio, Zona, ValorDiario, VigenciaDesde, VigenciaHasta) VALUES
(2027, 'GENERAL', @SMG_General_2027, '2027-01-01', '2027-12-31'),
(2027, 'ZFN',     @SMG_ZFN_2027,     '2027-01-01', '2027-12-31');

-- =============================================================================
-- 5) Tarifas ISR (mensual y anual), Subsidio para el Empleo y cuotas IMSS 2027
-- -----------------------------------------------------------------------------
-- Carga una BASE PROVISIONAL: sp_CargarDatosFiscales reutiliza la tarifa 2026
-- vigente (DELETE + INSERT por ejercicio, idempotente). Revisa y actualiza si el
-- DOF 2027 publica cambios en:
--   * La tarifa ISR del Art. 96 LISR (sólo se actualiza cuando la inflación
--     acumulada desde la última actualización supera el 10%).
--   * El Subsidio para el Empleo.
--   * La cuota PATRONAL de Cesantía y Vejez (CEV): la reforma de pensiones 2020
--     la incrementa gradualmente cada año hasta 2030; la cuota OBRERA se mantiene.
-- Para capturar valores 2027 distintos, edita las tarifas en dbo.sp_CargarDatosFiscales
-- (añadiendo una rama IF @Anio = 2027) o ejecuta un UPDATE posterior sobre las tablas.
-- =============================================================================
EXEC dbo.sp_CargarDatosFiscales @Anio = 2027;

PRINT 'Ejercicio 2027 listo: UMA y SMG con valores oficiales; ISR/Subsidio/IMSS = base provisional 2026.';
PRINT 'Verifica y actualiza las tarifas si el DOF publica cambios para 2027 (ver sección 5).';
GO

-- =============================================================================
-- 6) Verificación
-- =============================================================================
SELECT 'UMA'  AS Catalogo, CAST(ValorDiario AS VARCHAR(20)) AS ValorDiario, VigenciaDesde, VigenciaHasta
FROM dbo.CatUMA WHERE Anio = 2027
UNION ALL
SELECT 'SMG ' + Zona, CAST(ValorDiario AS VARCHAR(20)), VigenciaDesde, VigenciaHasta
FROM dbo.CatSalarioMinimo WHERE Anio = 2027;

SELECT e.Anio, e.JornadaSemanalHoras,
       (SELECT COUNT(*) FROM dbo.CatTablaISR            t WHERE t.EjercicioId = e.EjercicioId) AS RenglonesISR,
       (SELECT COUNT(*) FROM dbo.CatTablaISRAnual       t WHERE t.EjercicioId = e.EjercicioId) AS RenglonesISRAnual,
       (SELECT COUNT(*) FROM dbo.CatTablaSubsidioEmpleo t WHERE t.EjercicioId = e.EjercicioId) AS RenglonesSubsidio,
       (SELECT COUNT(*) FROM dbo.CatCuotaIMSS           t WHERE t.EjercicioId = e.EjercicioId) AS ConceptosIMSS
FROM dbo.CatEjercicioFiscal e WHERE e.Anio = 2027;
GO
