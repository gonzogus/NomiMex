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
-- =============================================================================
-- NomiMex - Ampliación del Catálogo de Fórmulas (Percepciones y Deducciones SAT)
-- Compatible con sp_EvaluarFormula, LFT 2026, LISR 2026 y Complemento Nómina 1.2 Rev. E
-- =============================================================================
SET NOCOUNT ON;
GO

DECLARE @TipoArit INT = (SELECT TipoFormulaId FROM dbo.CatTipoFormula WHERE Codigo = 'ARIT');

-- -----------------------------------------------------------------------------
-- 1. INSERTAR NUEVAS FÓRMULAS EN CatFormulaCalculo (Idempotente)
-- -----------------------------------------------------------------------------
MERGE dbo.CatFormulaCalculo AS TARGET
USING (VALUES
    -- === PERCEPCIONES (INGRESOS) ===
    (N'Horas extra triples',
     N'Horas extra triples (LFT Art. 68): excedente de 9 horas semanales al 300% (100% gravado)',
     N'@HORAS * @SALARIO_HORA * @FACTOR',
     N'HORAS,SALARIO_HORA,FACTOR'),

    (N'Días de descanso laborados',
     N'Descanso semanal laborado (LFT Art. 73 / SAT 054): días x salario diario x factor adicional (2x)',
     N'@DIAS * @SD * @FACTOR',
     N'DIAS,SD,FACTOR'),

    (N'Días de descanso obligatorios laborados',
     N'Día festivo oficial laborado (LFT Art. 75 / SAT 055): días x salario diario x factor adicional (2x)',
     N'@DIAS * @SD * @FACTOR',
     N'DIAS,SD,FACTOR'),

    (N'Premio por puntualidad',
     N'Premio por puntualidad (SAT 010 / LSS Art. 27-VII): % sobre salario base del periodo',
     N'@DIAS * @SD * @PORCENTAJE / 100',
     N'DIAS,SD,PORCENTAJE'),

    (N'Premio por asistencia',
     N'Premio por asistencia (SAT 049 / LSS Art. 27-VII): % sobre salario base del periodo',
     N'@DIAS * @SD * @PORCENTAJE / 100',
     N'DIAS,SD,PORCENTAJE'),

    (N'Vales de despensa por día',
     N'Vales de despensa (SAT 029): días del periodo x UMA diaria x % de otorgamiento',
     N'@DIAS * @UMA * @PORCENTAJE / 100',
     N'DIAS,UMA,PORCENTAJE'),

    (N'Vales de despensa tope exento IMSS',
     N'Tope de exención IMSS vales de despensa (LSS Art. 27-VI): hasta 40% de UMA diaria por día',
     N'@DIAS * @UMA * @TOPE_UMA',
     N'DIAS,UMA,TOPE_UMA'),

    (N'Fondo de ahorro aportación patronal',
     N'Fondo de ahorro patrón (SAT 005 / LISR Art. 93-XI): días x salario diario x % aportación',
     N'@DIAS * @SD * @PORCENTAJE / 100',
     N'DIAS,SD,PORCENTAJE'),

    (N'Tope exento fondo de ahorro SMG',
     N'Tope legal exento fondo de ahorro (LISR Art. 93-XI): 13% sobre hasta 1.3 SMG por día',
     N'@DIAS * @SMG * @FACTOR * @PORCENTAJE / 100',
     N'DIAS,SMG,FACTOR,PORCENTAJE'),

    (N'Prima de antigüedad topada 2 SMG',
     N'Prima de antigüedad (LFT Art. 162/486): días (12/año) x tope de 2 Salarios Mínimos Generales',
     N'@DIAS * @SMG * @FACTOR',
     N'DIAS,SMG,FACTOR'),

    (N'Pago por separación (20 días por año)',
     N'Indemnización por antigüedad en separación (LFT Art. 50-II): días (20/año) x salario diario',
     N'@DIAS * @SD',
     N'DIAS,SD'),

    (N'Exención separación e indemnización',
     N'Exención fiscal por separación (LISR Art. 93-XIII): 90 UMA por año de servicio',
     N'@DIAS * @UMA * @TOPE_UMA',
     N'DIAS,UMA,TOPE_UMA'),

    (N'Exención PTU',
     N'Exención fiscal de PTU (LISR Art. 93-XIV): hasta 15 UMA diarias',
     N'@UMA * @TOPE_UMA',
     N'UMA,TOPE_UMA'),

    (N'Subsidio por incapacidad empresa',
     N'Complemento patronal por incapacidad (SAT 014): días x salario diario x % cubierto',
     N'@DIAS * @SD * @PORCENTAJE / 100',
     N'DIAS,SD,PORCENTAJE'),

    (N'Ayuda para transporte por día',
     N'Ayuda para transporte (SAT 036): días laborados x cuota diaria parametrizada en FACTOR',
     N'@DIAS * @FACTOR',
     N'DIAS,FACTOR'),

    (N'Alimentación en especie / comedor',
     N'Alimentación (SAT 047 / LSS Art. 27-V): días x UMA diaria x % de valor',
     N'@DIAS * @UMA * @PORCENTAJE / 100',
     N'DIAS,UMA,PORCENTAJE'),

    (N'Habitación',
     N'Habitación (SAT 048 / LSS Art. 27-V): días x UMA diaria x % de valor',
     N'@DIAS * @UMA * @PORCENTAJE / 100',
     N'DIAS,UMA,PORCENTAJE'),

    -- === DEDUCCIONES (EGRESOS) ===
    (N'Descuento por ausentismo',
     N'Descuento por faltas injustificadas (SAT 020): días de ausencia x salario diario',
     N'@DIAS * @SD',
     N'DIAS,SD'),

    (N'Descuento por incapacidad',
     N'Descuento de días amparados por certificado IMSS (SAT 006): días x salario diario',
     N'@DIAS * @SD',
     N'DIAS,SD'),

    (N'Descuento por retardo en horas',
     N'Descuento proporcional por retardos acumulados (SAT 020): horas x salario por hora',
     N'@HORAS * @SALARIO_HORA',
     N'HORAS,SALARIO_HORA'),

    (N'Crédito INFONAVIT porcentaje SBC',
     N'Amortización crédito vivienda por % (SAT 010): días cotizados x SBC x % descuento',
     N'@DIAS * @SBC * @PORCENTAJE / 100',
     N'DIAS,SBC,PORCENTAJE'),

    (N'Crédito INFONAVIT factor VSM/UMI',
     N'Amortización crédito vivienda en veces salario/UMI (SAT 010): días x SMG/UMI x factor',
     N'@DIAS * @SMG * @FACTOR',
     N'DIAS,SMG,FACTOR'),

    (N'Pensión alimenticia porcentaje',
     N'Retención judicial de pensión alimenticia (SAT 007 / LFT Art. 110-V): % sobre base salarial',
     N'@DIAS * @SD * @PORCENTAJE / 100',
     N'DIAS,SD,PORCENTAJE'),

    (N'Fondo de ahorro retención trabajador',
     N'Aportación obrera a fondo de ahorro (SAT 018 / LFT Art. 110-IV): días x SD x %',
     N'@DIAS * @SD * @PORCENTAJE / 100',
     N'DIAS,SD,PORCENTAJE'),

    (N'Caja de ahorro retención trabajador',
     N'Aportación obrera a caja de ahorro (SAT 018 / LFT Art. 110-IV): días x SD x %',
     N'@DIAS * @SD * @PORCENTAJE / 100',
     N'DIAS,SD,PORCENTAJE'),

    (N'Cuota sindical porcentaje',
     N'Cuota sindical ordinaria (SAT 019 / LFT Art. 110-VI): % sobre salario ordinario',
     N'@DIAS * @SD * @PORCENTAJE / 100',
     N'DIAS,SD,PORCENTAJE'),

    (N'Aportación voluntaria AFORE / retiro',
     N'Aportaciones voluntarias al retiro (SAT 023): % sobre SBC del periodo',
     N'@DIAS * @SBC * @PORCENTAJE / 100',
     N'DIAS,SBC,PORCENTAJE'),

    (N'Impuesto local sobre nómina / cedular',
     N'Retención de impuesto local cuando aplique por entidad (SAT 022): % sobre percepción gravada',
     N'@DIAS * @SD * @PORCENTAJE / 100',
     N'DIAS,SD,PORCENTAJE')

) AS SOURCE (Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles)
ON TARGET.Nombre = SOURCE.Nombre
WHEN MATCHED THEN
    UPDATE SET
        TipoFormulaId        = @TipoArit,
        Descripcion          = SOURCE.Descripcion,
        ExpresionCalculo     = SOURCE.ExpresionCalculo,
        VariablesDisponibles = SOURCE.VariablesDisponibles,
        Activa               = 1
WHEN NOT MATCHED THEN
    INSERT (TipoFormulaId, Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles, Activa)
    VALUES (@TipoArit, SOURCE.Nombre, SOURCE.Descripcion, SOURCE.ExpresionCalculo, SOURCE.VariablesDisponibles, 1);
GO

-- -----------------------------------------------------------------------------
-- 2. LIGAR FÓRMULAS CON EL CATÁLOGO SAT (RelConceptoFormula)
-- -----------------------------------------------------------------------------
INSERT INTO dbo.RelConceptoFormula (ConceptoId, FormulaId, OrdenEjecucion, Activa)
SELECT c.ConceptoId, f.FormulaId, m.Orden, 1
FROM (VALUES
    -- Percepciones (TipoConcepto = 'P')
    ('003', 'P', N'Exención PTU',                           2),
    ('005', 'P', N'Fondo de ahorro aportación patronal',    1),
    ('005', 'P', N'Tope exento fondo de ahorro SMG',        2),
    ('010', 'P', N'Premio por puntualidad',                 1),
    ('014', 'P', N'Subsidio por incapacidad empresa',       1),
    ('019', 'P', N'Horas extra triples',                    2),
    ('022', 'P', N'Prima de antigüedad topada 2 SMG',       2),
    ('023', 'P', N'Pago por separación (20 días por año)',  1),
    ('023', 'P', N'Exención separación e indemnización',    2),
    ('025', 'P', N'Exención separación e indemnización',    2),
    ('029', 'P', N'Vales de despensa por día',              1),
    ('029', 'P', N'Vales de despensa tope exento IMSS',     2),
    ('036', 'P', N'Ayuda para transporte por día',          1),
    ('047', 'P', N'Alimentación en especie / comedor',      1),
    ('048', 'P', N'Habitación',                             1),
    ('049', 'P', N'Premio por asistencia',                  1),
    ('054', 'P', N'Días de descanso laborados',             1),
    ('055', 'P', N'Días de descanso obligatorios laborados',1),

    -- Deducciones (TipoConcepto = 'D')
    ('006', 'D', N'Descuento por incapacidad',              1),
    ('007', 'D', N'Pensión alimenticia porcentaje',         1),
    ('010', 'D', N'Crédito INFONAVIT porcentaje SBC',       1),
    ('010', 'D', N'Crédito INFONAVIT factor VSM/UMI',       2),
    ('018', 'D', N'Fondo de ahorro retención trabajador',   1),
    ('018', 'D', N'Caja de ahorro retención trabajador',    2),
    ('019', 'D', N'Cuota sindical porcentaje',              1),
    ('020', 'D', N'Descuento por ausentismo',               1),
    ('020', 'D', N'Descuento por retardo en horas',         2),
    ('022', 'D', N'Impuesto local sobre nómina / cedular',  1),
    ('023', 'D', N'Aportación voluntaria AFORE / retiro',   1)
) m (ClaveSAT, TipoConcepto, Formula, Orden)
JOIN dbo.CatConcepto c
  ON c.ClaveSAT = m.ClaveSAT AND c.TipoConcepto = m.TipoConcepto
JOIN dbo.CatFormulaCalculo f
  ON f.Nombre = m.Formula
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.RelConceptoFormula rf
    WHERE rf.ConceptoId = c.ConceptoId AND rf.FormulaId = f.FormulaId
);
GO

-- -----------------------------------------------------------------------------
-- 3. ACTUALIZAR UNIDADES DE MEDIDA Y FORMA DE CÁLCULO EN CatConcepto
-- -----------------------------------------------------------------------------
-- Conceptos cuantificados en DÍAS (ClaveUnidadSAT = 'DIA')
UPDATE dbo.CatConcepto
SET EsIncidencia = 1, FormaCalculo = 'DIAS', ClaveUnidadSAT = N'DIA'
WHERE TipoConcepto = 'P'
  AND ClaveSAT IN ('005','010','014','023','029','036','047','048','049','054','055');

UPDATE dbo.CatConcepto
SET EsIncidencia = 1, FormaCalculo = 'DIAS', ClaveUnidadSAT = N'DIA'
WHERE TipoConcepto = 'D'
  AND ClaveSAT IN ('006','020');

-- Conceptos cuantificados por PORCENTAJE_SD (ClaveUnidadSAT = 'ACT')
UPDATE dbo.CatConcepto
SET EsIncidencia = 1, FormaCalculo = 'PORCENTAJE_SD', ClaveUnidadSAT = N'ACT'
WHERE TipoConcepto = 'D'
  AND ClaveSAT IN ('007','010','018','019','022','023');

-- Resto de Percepciones y Deducciones de monto directo (Comisiones, Viáticos, FONACOT, Préstamos, Ajustes CFDI)
UPDATE dbo.CatConcepto
SET EsIncidencia = 1, FormaCalculo = 'MONTO_FIJO', ClaveUnidadSAT = N'ACT'
WHERE TipoConcepto IN ('P','D')
  AND EsBase = 0
  AND FormaCalculo IS NULL;
GO

-- -----------------------------------------------------------------------------
-- 4. PARÁMETROS GLOBALES POR DEFECTO EN RelParametroCalculo
-- -----------------------------------------------------------------------------
INSERT INTO dbo.RelParametroCalculo (Nombre, Valor, Activo)
SELECT v.Nombre, v.Valor, 1
FROM (VALUES
    ('FACTOR_HORAS_EXTRA_TRIPLE',      CAST(3.00  AS DECIMAL(18,6))), -- LFT Art. 68
    ('FACTOR_DIA_DESCANSO_LABORADO',   CAST(2.00  AS DECIMAL(18,6))), -- LFT Art. 73 (SAT 054)
    ('FACTOR_DIA_FESTIVO_LABORADO',    CAST(2.00  AS DECIMAL(18,6))), -- LFT Art. 75 (SAT 055)
    ('FACTOR_TOPE_PRIMA_ANTIGUEDAD',   CAST(2.00  AS DECIMAL(18,6))), -- LFT Art. 162/486 (2 SMG)
    ('TOPE_UMA_EXENCION_SEPARACION',   CAST(90.00 AS DECIMAL(18,6))), -- LISR Art. 93-XIII (90 UMA/año)
    ('TOPE_UMA_PTU',                   CAST(15.00 AS DECIMAL(18,6))), -- LISR Art. 93-XIV (15 UMA)
    ('TOPE_UMA_VALES_DESPENSA_IMSS',   CAST(0.40  AS DECIMAL(18,6))), -- LSS Art. 27-VI (40% UMA)
    ('PORCENTAJE_VALES_DESPENSA',      CAST(10.00 AS DECIMAL(18,6))), -- Configurable por empresa
    ('PORCENTAJE_FONDO_AHORRO',        CAST(13.00 AS DECIMAL(18,6))), -- LISR Art. 93-XI (máx 13%)
    ('FACTOR_TOPE_FONDO_AHORRO_SMG',   CAST(1.30  AS DECIMAL(18,6))), -- LISR Art. 93-XI (1.3 SMG)
    ('PORCENTAJE_PREMIO_PUNTUALIDAD',  CAST(10.00 AS DECIMAL(18,6))), -- LSS Art. 27-VII (tope 10% SBC)
    ('PORCENTAJE_PREMIO_ASISTENCIA',   CAST(10.00 AS DECIMAL(18,6))), -- LSS Art. 27-VII (tope 10% SBC)
    ('PORCENTAJE_CUOTA_SINDICAL',      CAST(2.00  AS DECIMAL(18,6))), -- Configurable por empresa
    ('PORCENTAJE_ALIMENTACION_IMSS',   CAST(20.00 AS DECIMAL(18,6))), -- LSS Art. 27-V (20% UMA)
    ('PORCENTAJE_HABITACION_IMSS',     CAST(20.00 AS DECIMAL(18,6)))  -- LSS Art. 27-V (20% UMA)
) v (Nombre, Valor)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.RelParametroCalculo p
    WHERE p.Nombre = v.Nombre
      AND p.EmpresaId IS NULL
      AND p.PeriodoId IS NULL
      AND p.EmpleadoId IS NULL
);
GO

-- -----------------------------------------------------------------------------
-- 5. CONSULTA DE VERIFICACIÓN DEL CATÁLOGO COMPLETO
-- -----------------------------------------------------------------------------
SELECT
    c.TipoConcepto,
    c.ClaveSAT,
    c.Descripcion       AS ConceptoSAT,
    c.FormaCalculo,
    c.ClaveUnidadSAT,
    rf.OrdenEjecucion,
    f.Nombre            AS FormulaAsignada,
    f.ExpresionCalculo,
    f.VariablesDisponibles
FROM dbo.CatConcepto c
LEFT JOIN dbo.RelConceptoFormula rf ON rf.ConceptoId = c.ConceptoId AND rf.Activa = 1
LEFT JOIN dbo.CatFormulaCalculo f   ON f.FormulaId = rf.FormulaId AND f.Activa = 1
WHERE c.TipoConcepto IN ('P','D')
ORDER BY c.TipoConcepto, c.ClaveSAT, rf.OrdenEjecucion;
GO