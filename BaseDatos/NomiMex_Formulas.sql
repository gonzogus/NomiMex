-- =============================================================================
-- NomiMex - Catálogo de fórmulas de cálculo (motor de fórmulas parametrizables)
-- -----------------------------------------------------------------------------
-- Siembra/refresca CatTipoFormula, CatFormulaCalculo, RelConceptoFormula y los
-- parámetros globales por defecto (RelParametroCalculo). Es idempotente: al
-- re-ejecutarlo regenera el catálogo de fórmulas, por lo que también corrige
-- cualquier texto con acentos que se haya almacenado mal.
--
-- Requiere que la base ya esté instalada (NomiMex_Instalacion.sql).
--
-- IMPORTANTE (acentos): este archivo está en UTF-8 CON BOM. Ejecútalo con:
--     sqlcmd -S .\SQLEXPRESS -E -I -d NomiMex -i NomiMex_Formulas.sql
-- Si tu sqlcmd no respeta el BOM, añade:  -f 65001
-- =============================================================================
SET NOCOUNT ON;
GO

-- 1. Tipo de fórmula aritmética
IF NOT EXISTS (SELECT 1 FROM dbo.CatTipoFormula WHERE Codigo = 'ARIT')
    INSERT INTO dbo.CatTipoFormula (Codigo, Descripcion, EjemploExpresion, Activo)
    VALUES ('ARIT', N'Expresión aritmética sobre variables de nómina', N'@DIAS * @SD * @PORCENTAJE / 100', 1);
GO

-- 2. Refrescar catálogo de fórmulas (borra ligas y fórmulas y las vuelve a sembrar con texto correcto)
DELETE FROM dbo.RelConceptoFormula;
DELETE FROM dbo.CatFormulaCalculo;
GO

DECLARE @TipoArit INT = (SELECT TipoFormulaId FROM dbo.CatTipoFormula WHERE Codigo = 'ARIT');

INSERT INTO dbo.CatFormulaCalculo (TipoFormulaId, Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles)
VALUES
    (@TipoArit, N'Prima dominical', N'Prima dominical (LFT Art. 71): % del salario diario por domingo laborado',
            N'@DIAS * @SD * @PORCENTAJE / 100', N'DIAS,SD,PORCENTAJE'),
    (@TipoArit, N'Prima dominical exenta', N'Exención de prima dominical: hasta TOPE_UMA UMA por domingo',
            N'@DIAS * @UMA * @TOPE_UMA', N'DIAS,UMA,TOPE_UMA'),
    (@TipoArit, N'Horas extra dobles', N'Horas extra dobles (LFT Art. 67): horas x salario por hora x factor',
            N'@HORAS * @SALARIO_HORA * @FACTOR', N'HORAS,SALARIO_HORA,FACTOR'),
    (@TipoArit, N'Aguinaldo', N'Aguinaldo (LFT Art. 87): días de aguinaldo x salario diario',
            N'@DIAS * @SD', N'DIAS,SD'),
    (@TipoArit, N'Prima vacacional', N'Prima vacacional (LFT Art. 80): días de vacaciones x salario diario x %',
            N'@DIAS * @SD * @PORCENTAJE / 100', N'DIAS,SD,PORCENTAJE'),
    (@TipoArit, N'Prima de antigüedad', N'Prima de antigüedad (LFT Art. 162): días (12 por año) x salario diario',
            N'@DIAS * @SD', N'DIAS,SD'),
    (@TipoArit, N'Indemnizaciones', N'Indemnización por separación (LFT Art. 48/50): días x salario diario',
            N'@DIAS * @SD', N'DIAS,SD');
GO

-- 3. Ligar cada fórmula con su concepto del catálogo SAT
INSERT INTO dbo.RelConceptoFormula (ConceptoId, FormulaId, OrdenEjecucion)
SELECT c.ConceptoId, f.FormulaId, 1
FROM (VALUES
    ('020', N'Prima dominical'),
    ('019', N'Horas extra dobles'),
    ('002', N'Aguinaldo'),
    ('021', N'Prima vacacional'),
    ('022', N'Prima de antigüedad'),
    ('025', N'Indemnizaciones')
) m (ClaveSAT, Formula)
JOIN dbo.CatConcepto c ON c.ClaveSAT = m.ClaveSAT AND c.TipoConcepto = 'P'
JOIN dbo.CatFormulaCalculo f ON f.Nombre = m.Formula;
GO

-- 4. Marcar los conceptos como incidencia capturable con su forma de cálculo
UPDATE dbo.CatConcepto SET EsIncidencia = 1, FormaCalculo = 'DIAS'  WHERE ClaveSAT = '020' AND TipoConcepto = 'P';
UPDATE dbo.CatConcepto SET EsIncidencia = 1, FormaCalculo = 'HORAS' WHERE ClaveSAT = '019' AND TipoConcepto = 'P';
UPDATE dbo.CatConcepto SET EsIncidencia = 1, FormaCalculo = 'DIAS'  WHERE TipoConcepto = 'P' AND ClaveSAT IN ('002','021','022','025');
GO

-- 5. Parámetros globales por defecto (sobrescribibles por empresa/periodo/empleado)
INSERT INTO dbo.RelParametroCalculo (Nombre, Valor)
SELECT v.Nombre, v.Valor FROM (VALUES
    ('PORCENTAJE_PRIMA_DOMINICAL', CAST(25 AS DECIMAL(18,6))),
    ('TOPE_UMA_PRIMA_DOMINICAL',   CAST(1  AS DECIMAL(18,6))),
    ('FACTOR_HORAS_EXTRA_DOBLE',   CAST(2  AS DECIMAL(18,6))),
    ('PORCENTAJE_PRIMA_VACACIONAL',CAST(25 AS DECIMAL(18,6))),
    ('TOPE_UMA_AGUINALDO',         CAST(30 AS DECIMAL(18,6))),
    ('TOPE_UMA_PRIMA_VACACIONAL',  CAST(15 AS DECIMAL(18,6)))
) v (Nombre, Valor)
WHERE NOT EXISTS (SELECT 1 FROM dbo.RelParametroCalculo p
                  WHERE p.Nombre = v.Nombre AND p.EmpresaId IS NULL AND p.PeriodoId IS NULL AND p.EmpleadoId IS NULL);
GO

-- 6. Verificación
SELECT f.FormulaId, f.Nombre, f.Descripcion, f.ExpresionCalculo, f.VariablesDisponibles,
       c.ClaveSAT AS ConceptoSAT
FROM dbo.CatFormulaCalculo f
LEFT JOIN dbo.RelConceptoFormula rf ON rf.FormulaId = f.FormulaId AND rf.Activa = 1
LEFT JOIN dbo.CatConcepto c ON c.ConceptoId = rf.ConceptoId
ORDER BY f.FormulaId;
GO
