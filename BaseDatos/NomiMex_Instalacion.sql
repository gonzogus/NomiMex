-- ==============================================================================
-- NomiMex - Script Maestro de Instalación Integral
-- Destino: instancia indicada al ejecutar sqlcmd | base indicada en BaseDatos
-- Taxonomía SAT: Cat, Proc, Doc, Rel, Log, Hist
-- Incluye:
--   1. Esquema Maestro Completo Auditado y Optimizado (117 tablas, FK, CHECK, DEFAULT, UQ)
--   2. Catálogos Oficiales SAT (Nómina 1.2 y CFDI 4.0) y de Sistema
--   3. Módulo de configuración de correo, parámetros, bitácora del sistema y recuperación de acceso
--   4. Datos Iniciales de Operación (Admin SUPERADMIN, Ejercicio Fiscal 2026, Tarifas y Festivos)
--   5. Datos de demostración de la empresa (opcionales; contienen datos reales)
-- Ejecutar con sqlcmd -v BaseDatos=NomiMex_Nueva CargarDatosDemostracion=0.
-- Use una base nueva: si ya existe, este script la elimina y la vuelve a crear.
-- CargarDatosDemostracion=1 agrega los registros de demostración reales.
-- ==============================================================================
:ON ERROR EXIT
USE master;
GO

DECLARE @NombreBase SYSNAME = N'$(BaseDatos)';
IF LEN(@NombreBase) = 0 OR LEN(@NombreBase) > 128 OR @NombreBase LIKE N'%[^A-Za-z0-9_]%'
    THROW 51000, 'BaseDatos debe contener solo letras ASCII, números o guion bajo.', 1;
IF N'$(CargarDatosDemostracion)' NOT IN (N'0', N'1')
    THROW 51000, 'CargarDatosDemostracion debe ser 0 o 1.', 1;
GO

IF DB_ID(N'$(BaseDatos)') IS NOT NULL
BEGIN
    PRINT 'Eliminando base de datos previa $(BaseDatos)...';
    ALTER DATABASE [$(BaseDatos)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(BaseDatos)];
END
GO

PRINT 'Creando base de datos $(BaseDatos) (intercalación Modern_Spanish_CI_AS)...';
CREATE DATABASE [$(BaseDatos)] COLLATE Modern_Spanish_CI_AS;
GO

USE [$(BaseDatos)];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
GO
-- ==============================================================================
-- [BLOQUE-01] CATÁLOGOS SAT Y DE SISTEMA (Cat)
-- ==============================================================================
PRINT '[01/21] Creando catálogos del SAT y del sistema (Prefijo Cat)...';
GO

-- c_RegimenFiscal (SAT CFDI 4.0)
IF OBJECT_ID('dbo.CatSatRegimenFiscal', 'U') IS NULL
CREATE TABLE dbo.CatSatRegimenFiscal (
    RegimenFiscalId INT NOT NULL CONSTRAINT PK_CatSatRegimenFiscal PRIMARY KEY,
    Clave NVARCHAR(3) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    AplicaPersonaFisica BIT NOT NULL,
    AplicaPersonaMoral BIT NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatRegimenFiscal_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatRegimenFiscal_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatRegimenFiscal_Clave CHECK (LEN(Clave) BETWEEN 1 AND 3 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatRegimenFiscal_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatRegimenFiscal_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_TipoJornada (SAT Nómina 1.2 / Art. 61 LFT)
IF OBJECT_ID('dbo.CatSatTipoJornada', 'U') IS NULL
CREATE TABLE dbo.CatSatTipoJornada (
    TipoJornadaId INT NOT NULL CONSTRAINT PK_CatSatTipoJornada PRIMARY KEY,
    Clave NVARCHAR(2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatTipoJornada_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatTipoJornada_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatTipoJornada_Clave CHECK (LEN(Clave) BETWEEN 1 AND 2 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatTipoJornada_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatTipoJornada_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO
CREATE TABLE dbo.CatReglaTipoJornada (
    TipoJornadaId INT NOT NULL CONSTRAINT PK_CatReglaTipoJornada PRIMARY KEY
        CONSTRAINT FK_CatReglaTipoJornada_SAT REFERENCES dbo.CatSatTipoJornada(TipoJornadaId),
    Aplicacion NVARCHAR(200) NULL,
    HorasMaximasLegal DECIMAL(4,2) NULL,
    DiasSemana TINYINT NULL,
    CONSTRAINT CK_CatReglaTipoJornada_Horas CHECK (HorasMaximasLegal > 0 AND HorasMaximasLegal <= 24),
    CONSTRAINT CK_CatReglaTipoJornada_DiasSemana CHECK (DiasSemana IS NULL OR DiasSemana BETWEEN 1 AND 7)
);
GO

-- c_PeriodicidadPago (SAT Nómina 1.2)
IF OBJECT_ID('dbo.CatSatPeriodicidadPago', 'U') IS NULL
CREATE TABLE dbo.CatSatPeriodicidadPago (
    PeriodicidadPagoId INT NOT NULL CONSTRAINT PK_CatSatPeriodicidadPago PRIMARY KEY,
    Clave NVARCHAR(2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatPeriodicidadPago_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatPeriodicidadPago_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatPeriodicidadPago_Clave CHECK (LEN(Clave) BETWEEN 1 AND 2 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatPeriodicidadPago_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatPeriodicidadPago_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO
CREATE TABLE dbo.CatReglaPeriodicidadPago (
    PeriodicidadPagoId INT NOT NULL CONSTRAINT PK_CatReglaPeriodicidadPago PRIMARY KEY
        CONSTRAINT FK_CatReglaPeriodicidadPago_SAT REFERENCES dbo.CatSatPeriodicidadPago(PeriodicidadPagoId),
    DiasBase DECIMAL(5,2) NULL,
    CONSTRAINT CK_CatReglaPeriodicidadPago_Dias CHECK (DiasBase > 0)
);
GO

-- c_TipoRegimen (SAT Nómina 1.2)
IF OBJECT_ID('dbo.CatSatTipoRegimen', 'U') IS NULL
CREATE TABLE dbo.CatSatTipoRegimen (
    TipoRegimenId INT NOT NULL CONSTRAINT PK_CatSatTipoRegimen PRIMARY KEY,
    Clave NVARCHAR(2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatTipoRegimen_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatTipoRegimen_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatTipoRegimen_Clave CHECK (LEN(Clave) BETWEEN 1 AND 2 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatTipoRegimen_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatTipoRegimen_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_RiesgoPuesto (LSS Art. 73)
IF OBJECT_ID('dbo.CatSatRiesgoPuesto', 'U') IS NULL
CREATE TABLE dbo.CatSatRiesgoPuesto (
    RiesgoPuestoId INT NOT NULL CONSTRAINT PK_CatSatRiesgoPuesto PRIMARY KEY,
    Clave NVARCHAR(2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatRiesgoPuesto_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatRiesgoPuesto_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatRiesgoPuesto_Clave CHECK (LEN(Clave) BETWEEN 1 AND 2 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatRiesgoPuesto_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatRiesgoPuesto_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO
CREATE TABLE dbo.CatReglaRiesgoPuesto (
    RiesgoPuestoId INT NOT NULL CONSTRAINT PK_CatReglaRiesgoPuesto PRIMARY KEY
        CONSTRAINT FK_CatReglaRiesgoPuesto_SAT REFERENCES dbo.CatSatRiesgoPuesto(RiesgoPuestoId),
    Clase NVARCHAR(10) NULL,
    PrimaMedia DECIMAL(10,8) NULL,
    PrimaMinima DECIMAL(10,8) NULL,
    PrimaMaxima DECIMAL(10,8) NULL,
    CONSTRAINT CK_CatReglaRiesgoPuesto_Primas CHECK
        (PrimaMinima >= 0 AND PrimaMedia BETWEEN PrimaMinima AND PrimaMaxima AND PrimaMaxima <= 1)
);
GO

-- c_MotivoCancelacion (SAT CFDI 4.0)
IF OBJECT_ID('dbo.CatSatMotivoCancelacion', 'U') IS NULL
CREATE TABLE dbo.CatSatMotivoCancelacion (
    MotivoCancelacionId INT NOT NULL CONSTRAINT PK_CatSatMotivoCancelacion PRIMARY KEY,
    Clave NVARCHAR(2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    RequiereSustitucion AS CONVERT(BIT, CASE WHEN Clave = N'01' THEN 1 ELSE 0 END) PERSISTED,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatMotivoCancelacion_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatMotivoCancelacion_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatMotivoCancelacion_Clave CHECK (LEN(Clave) BETWEEN 1 AND 2 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatMotivoCancelacion_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatMotivoCancelacion_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_TipoIncapacidad (SAT Nómina 1.2 / IMSS)
IF OBJECT_ID('dbo.CatSatTipoIncapacidad', 'U') IS NULL
CREATE TABLE dbo.CatSatTipoIncapacidad (
    TipoIncapacidadId INT NOT NULL CONSTRAINT PK_CatSatTipoIncapacidad PRIMARY KEY,
    Clave NVARCHAR(2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatTipoIncapacidad_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatTipoIncapacidad_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatTipoIncapacidad_Clave CHECK (LEN(Clave) BETWEEN 1 AND 2 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatTipoIncapacidad_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatTipoIncapacidad_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO
CREATE TABLE dbo.CatReglaTipoIncapacidad (
    TipoIncapacidadId INT NOT NULL CONSTRAINT PK_CatReglaTipoIncapacidad PRIMARY KEY
        CONSTRAINT FK_CatReglaTipoIncapacidad_SAT REFERENCES dbo.CatSatTipoIncapacidad(TipoIncapacidadId),
    PorcentajePago DECIMAL(5,2) NULL,
    DiasEspera INT NOT NULL CONSTRAINT DF_CatReglaTipoIncapacidad_Espera DEFAULT 0,
    PagaPatron BIT NOT NULL CONSTRAINT DF_CatReglaTipoIncapacidad_Patron DEFAULT 0,
    CONSTRAINT CK_CatReglaTipoIncapacidad_Porcentaje CHECK (PorcentajePago BETWEEN 0 AND 100),
    CONSTRAINT CK_CatReglaTipoIncapacidad_Espera CHECK (DiasEspera >= 0)
);
GO

-- CatEstadoCFDI (Ciclo de vida del comprobante fiscal)
IF OBJECT_ID('dbo.CatEstadoCFDI', 'U') IS NULL
CREATE TABLE dbo.CatEstadoCFDI (
    EstadoCFDIId INT PRIMARY KEY,
    Clave        NVARCHAR(20) NOT NULL,
    Descripcion  NVARCHAR(100) NOT NULL,
    Activo       BIT NOT NULL CONSTRAINT DF_CatEstadoCFDI_Activo DEFAULT 1,
    CONSTRAINT UQ_CatEstadoCFDI_Clave UNIQUE (Clave)
);
GO

-- c_OrigenRecurso (SAT Nómina 1.2: origen de los recursos de entes públicos)
IF OBJECT_ID('dbo.CatSatOrigenRecurso', 'U') IS NULL
CREATE TABLE dbo.CatSatOrigenRecurso (
    OrigenRecursoId INT NOT NULL CONSTRAINT PK_CatSatOrigenRecurso PRIMARY KEY,
    Clave NVARCHAR(2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatOrigenRecurso_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatOrigenRecurso_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatOrigenRecurso_Clave CHECK (LEN(Clave) BETWEEN 1 AND 2 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatOrigenRecurso_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatOrigenRecurso_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_TipoNomina (SAT Nómina 1.2: O = ordinaria, E = extraordinaria)
IF OBJECT_ID('dbo.CatSatTipoNomina', 'U') IS NULL
CREATE TABLE dbo.CatSatTipoNomina (
    TipoNominaId INT NOT NULL CONSTRAINT PK_CatSatTipoNomina PRIMARY KEY,
    Clave NVARCHAR(1) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatTipoNomina_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatTipoNomina_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatTipoNomina_Clave CHECK (LEN(Clave) BETWEEN 1 AND 1 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatTipoNomina_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatTipoNomina_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_TipoHoras (SAT Nómina 1.2: tipo de hora extra; lo referencian los conceptos de horas extra de CatConcepto)
IF OBJECT_ID('dbo.CatSatTipoHoras', 'U') IS NULL
CREATE TABLE dbo.CatSatTipoHoras (
    TipoHorasId INT NOT NULL CONSTRAINT PK_CatSatTipoHoras PRIMARY KEY,
    Clave NVARCHAR(2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatTipoHoras_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatTipoHoras_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatTipoHoras_Clave CHECK (LEN(Clave) BETWEEN 1 AND 2 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatTipoHoras_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatTipoHoras_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_Banco (SAT / Banxico)
IF OBJECT_ID('dbo.CatSatBanco', 'U') IS NULL
CREATE TABLE dbo.CatSatBanco (
    BancoId INT NOT NULL CONSTRAINT PK_CatSatBanco PRIMARY KEY,
    Clave NVARCHAR(3) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    RazonSocial NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatBanco_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatBanco_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatBanco_Clave CHECK (LEN(Clave) BETWEEN 1 AND 3 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatBanco_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatBanco_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_MetodoPago (SAT)
IF OBJECT_ID('dbo.CatFormaPagoDispersion', 'U') IS NULL
CREATE TABLE dbo.CatFormaPagoDispersion (
    MetodoPagoId INT PRIMARY KEY,
    Clave        NVARCHAR(10) NOT NULL,
    Descripcion  NVARCHAR(100) NOT NULL,
    Activo       BIT NOT NULL CONSTRAINT DF_CatFormaPagoDispersion_Activo DEFAULT 1,
    CONSTRAINT UQ_CatFormaPagoDispersion_Clave UNIQUE (Clave)
);
GO

-- c_EntidadFederativa (SAT c_CveEntidad)
IF OBJECT_ID('dbo.CatEntidadFederativa', 'U') IS NULL
CREATE TABLE dbo.CatEntidadFederativa (
    CveEntidad NVARCHAR(3) PRIMARY KEY,
    Nombre     NVARCHAR(100) NOT NULL,
    Abreviatura NVARCHAR(10) NOT NULL,
    Activo     BIT NOT NULL CONSTRAINT DF_CatEntidadFederativa_Activo DEFAULT 1
);
GO

-- c_TipoPercepcion (SAT Nómina 1.2)
IF OBJECT_ID('dbo.CatSatTipoPercepcion', 'U') IS NULL
CREATE TABLE dbo.CatSatTipoPercepcion (
    TipoPercepcionId INT NOT NULL CONSTRAINT PK_CatSatTipoPercepcion PRIMARY KEY,
    Clave NVARCHAR(3) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatTipoPercepcion_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatTipoPercepcion_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatTipoPercepcion_Clave CHECK (LEN(Clave) BETWEEN 1 AND 3 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatTipoPercepcion_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatTipoPercepcion_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_TipoDeduccion (SAT Nómina 1.2)
IF OBJECT_ID('dbo.CatSatTipoDeduccion', 'U') IS NULL
CREATE TABLE dbo.CatSatTipoDeduccion (
    TipoDeduccionId INT NOT NULL CONSTRAINT PK_CatSatTipoDeduccion PRIMARY KEY,
    Clave NVARCHAR(3) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatTipoDeduccion_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatTipoDeduccion_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatTipoDeduccion_Clave CHECK (LEN(Clave) BETWEEN 1 AND 3 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatTipoDeduccion_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatTipoDeduccion_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_TipoOtroPago (SAT Nómina 1.2)
IF OBJECT_ID('dbo.CatSatTipoOtroPago', 'U') IS NULL
CREATE TABLE dbo.CatSatTipoOtroPago (
    TipoOtroPagoId INT NOT NULL CONSTRAINT PK_CatSatTipoOtroPago PRIMARY KEY,
    Clave NVARCHAR(3) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatTipoOtroPago_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatTipoOtroPago_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatTipoOtroPago_Clave CHECK (LEN(Clave) BETWEEN 1 AND 3 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatTipoOtroPago_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatTipoOtroPago_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- c_TipoContrato (SAT Nómina 1.2)
IF OBJECT_ID('dbo.CatSatTipoContrato', 'U') IS NULL
CREATE TABLE dbo.CatSatTipoContrato (
    TipoContratoId INT NOT NULL CONSTRAINT PK_CatSatTipoContrato PRIMARY KEY,
    Clave NVARCHAR(2) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL CONSTRAINT DF_CatSatTipoContrato_Activo DEFAULT 1,
    CONSTRAINT UQ_CatSatTipoContrato_Clave UNIQUE (Clave),
    CONSTRAINT CK_CatSatTipoContrato_Clave CHECK (LEN(Clave) BETWEEN 1 AND 2 AND Clave NOT LIKE N'%[^A-Z0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatTipoContrato_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    CONSTRAINT CK_CatSatTipoContrato_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- CatTipoFormula (Motor de Fórmulas Parametrizables)
IF OBJECT_ID('dbo.CatTipoFormula', 'U') IS NULL
CREATE TABLE dbo.CatTipoFormula (
    TipoFormulaId    INT IDENTITY(1,1) PRIMARY KEY,
    Codigo           NVARCHAR(30) NOT NULL,
    Descripcion      NVARCHAR(200) NOT NULL,
    EjemploExpresion NVARCHAR(500) NULL,
    Activo           BIT NOT NULL CONSTRAINT DF_CatTipoFormula_Activo DEFAULT 1,
    CONSTRAINT UQ_CatTipoFormula_Codigo UNIQUE (Codigo)
);
GO

-- CatRol (Seguridad y RBAC)
IF OBJECT_ID('dbo.CatRol', 'U') IS NULL
CREATE TABLE dbo.CatRol (
    RolId       INT IDENTITY(1,1) PRIMARY KEY,
    Codigo      NVARCHAR(30) NOT NULL,
    Descripcion NVARCHAR(200) NOT NULL,
    Activo      BIT NOT NULL CONSTRAINT DF_CatRol_Activo DEFAULT 1,
    CONSTRAINT UQ_CatRol_Codigo UNIQUE (Codigo)
);
GO

-- CatPermiso (Seguridad y RBAC)
IF OBJECT_ID('dbo.CatPermiso', 'U') IS NULL
CREATE TABLE dbo.CatPermiso (
    PermisoId   INT IDENTITY(1,1) PRIMARY KEY,
    Codigo      NVARCHAR(60) NOT NULL,
    Descripcion NVARCHAR(200) NOT NULL,
    Modulo      NVARCHAR(40) NOT NULL,
    Activo      BIT NOT NULL CONSTRAINT DF_CatPermiso_Activo DEFAULT 1,
    CONSTRAINT UQ_CatPermiso_Codigo UNIQUE (Codigo)
);
GO

-- CatUsuario (Seguridad y Autenticación del Sistema)
IF OBJECT_ID('dbo.CatUsuario', 'U') IS NULL
CREATE TABLE dbo.CatUsuario (
    UsuarioId          INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario      NVARCHAR(50)  NOT NULL,
    Email              NVARCHAR(150) NOT NULL,
    NombreMostrar      NVARCHAR(150) NOT NULL,
    ContrasenaHash     VARBINARY(64) NOT NULL,
    ContrasenaSalt     VARBINARY(32) NOT NULL,
    Activo             BIT NOT NULL CONSTRAINT DF_CatUsuario_Activo DEFAULT 1,
    Bloqueado          BIT NOT NULL CONSTRAINT DF_CatUsuario_Bloqueado DEFAULT 0,
    IntentosFallidos   SMALLINT NOT NULL CONSTRAINT DF_CatUsuario_Intentos DEFAULT 0,
    UltimoAccesoUtc    DATETIME2(0) NULL,
    RequiereCambioPwd  BIT NOT NULL CONSTRAINT DF_CatUsuario_CambioPwd DEFAULT 0,
    FechaAltaUtc       DATETIME2(0) NOT NULL CONSTRAINT DF_CatUsuario_FechaAlta DEFAULT SYSUTCDATETIME(),
    FechaUpdateUtc     DATETIME2(0) NOT NULL CONSTRAINT DF_CatUsuario_FechaUpdate DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_CatUsuario_NombreUsuario UNIQUE (NombreUsuario),
    CONSTRAINT UQ_CatUsuario_Email UNIQUE (Email)
);
GO

-- Estructura Organizacional: CatCentroCosto, CatTurno
-- (CatDepartamento y CatPuesto se crean junto a CatEmpresa por depender de ella.)
IF OBJECT_ID('dbo.CatCentroCosto', 'U') IS NULL
CREATE TABLE dbo.CatCentroCosto (
    CentroCostoId INT IDENTITY(1,1) PRIMARY KEY,
    Clave         NVARCHAR(20)  NOT NULL,
    Descripcion   NVARCHAR(100) NOT NULL,
    Activo        BIT NOT NULL CONSTRAINT DF_CatCentroCosto_Activo DEFAULT 1,
    CONSTRAINT UQ_CatCentroCosto_Clave UNIQUE (Clave)
);
GO

IF OBJECT_ID('dbo.CatTurno', 'U') IS NULL
CREATE TABLE dbo.CatTurno (
    TurnoId       INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion   NVARCHAR(50) NOT NULL,
    Horas         DECIMAL(4,2) NOT NULL,
    TipoJornadaId INT NULL CONSTRAINT FK_CatTurno_TipoJornada REFERENCES dbo.CatSatTipoJornada(TipoJornadaId),
    PeriodicidadPagoId INT NULL CONSTRAINT FK_CatTurno_Periodicidad REFERENCES dbo.CatSatPeriodicidadPago(PeriodicidadPagoId),
    HoraInicio    TIME NULL,
    HoraFin       TIME NULL,
    Activo        BIT NOT NULL CONSTRAINT DF_CatTurno_Activo DEFAULT 1,
    CONSTRAINT CK_CatTurno_Horas CHECK (Horas > 0 AND Horas <= 24)
);
GO

-- ==============================================================================
-- [BLOQUE-02] ENTIDADES MAESTRAS DEL NEGOCIO (Cat) E HISTÓRICOS ASOCIADOS (Hist)
-- ==============================================================================
PRINT '[02/21] Creando entidades maestras del negocio (Prefijo Cat) e históricos (Hist)...';
GO

-- Catalogos CFDI 4.0. Columnas normalizadas del Excel oficial 2026-11-06.
-- Clave/Descripcion/Vigencia son datos SAT; Activo es un control local.
CREATE TABLE dbo.CatSatPais (
    Clave NVARCHAR(3) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    FormatoCodigoPostal NVARCHAR(500) NULL,
    FormatoRegistroIdentidadTributaria NVARCHAR(500) NULL,
    ValidacionRegistroIdentidadTributaria NVARCHAR(500) NULL,
    Agrupaciones NVARCHAR(500) NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatPais_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatEstado (
    Clave NVARCHAR(3) NOT NULL,
    ClavePais NVARCHAR(3) NOT NULL REFERENCES dbo.CatSatPais(Clave),
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_CatSatEstado PRIMARY KEY (Clave,ClavePais),
    CONSTRAINT CK_CatSatEstado_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatCodigoPostal (
    Clave NVARCHAR(5) NOT NULL PRIMARY KEY,
    ClaveEstado NVARCHAR(3) NOT NULL,
    ClavePais NVARCHAR(3) NOT NULL DEFAULT N'MEX',
    ClaveMunicipio NVARCHAR(3) NULL,
    ClaveLocalidad NVARCHAR(2) NULL,
    EstimuloFranjaFronteriza BIT NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    DescripcionHusoHorario NVARCHAR(500) NULL,
    MesInicioHorarioVerano NVARCHAR(50) NULL,
    DiaInicioHorarioVerano NVARCHAR(50) NULL,
    DiaSemanaInicioHorarioVerano NVARCHAR(50) NULL,
    DiferenciaHorariaVerano NVARCHAR(20) NULL,
    MesInicioHorarioInvierno NVARCHAR(50) NULL,
    DiaInicioHorarioInvierno NVARCHAR(50) NULL,
    DiaSemanaInicioHorarioInvierno NVARCHAR(50) NULL,
    DiferenciaHorariaInvierno NVARCHAR(20) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_CatSatCodigoPostal_Estado FOREIGN KEY (ClaveEstado,ClavePais) REFERENCES dbo.CatSatEstado(Clave,ClavePais),
    CONSTRAINT CK_CatSatCodigoPostal_Clave CHECK (LEN(Clave)=5 AND Clave NOT LIKE N'%[^0-9]%' COLLATE Latin1_General_100_BIN2),
    CONSTRAINT CK_CatSatCodigoPostal_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatMetodoPago (
    Clave NVARCHAR(3) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatMetodoPago_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatFormaPago (
    Clave NVARCHAR(2) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    Bancarizado NVARCHAR(30) NOT NULL,
    NumeroOperacion NVARCHAR(30) NULL,
    RFCEmisorCuentaOrdenante NVARCHAR(30) NULL,
    CuentaOrdenante NVARCHAR(30) NULL,
    PatronCuentaOrdenante NVARCHAR(1000) NULL,
    RFCEmisorCuentaBeneficiario NVARCHAR(30) NULL,
    CuentaBeneficiario NVARCHAR(30) NULL,
    PatronCuentaBeneficiario NVARCHAR(1000) NULL,
    TipoCadenaPago NVARCHAR(30) NULL,
    NombreBancoEmisorExtranjero NVARCHAR(100) NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatFormaPago_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatMoneda (
    Clave NVARCHAR(3) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    Decimales TINYINT NOT NULL,
    PorcentajeVariacion DECIMAL(10,4) NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatMoneda_Decimales CHECK (Decimales <= 6),
    CONSTRAINT CK_CatSatMoneda_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatTipoDeComprobante (
    Clave NVARCHAR(1) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    ValorMaximo DECIMAL(24,6) NULL,
    ValorMaximoNS DECIMAL(24,6) NULL,
    ValorMaximoNdS DECIMAL(24,6) NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatTipoDeComprobante_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatExportacion (
    Clave NVARCHAR(2) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatExportacion_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatUsoCFDI (
    Clave NVARCHAR(4) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    AplicaPersonaFisica BIT NOT NULL,
    AplicaPersonaMoral BIT NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatUsoCFDI_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.RelSatUsoCFDIRegimenFiscal (
    ClaveUsoCFDI NVARCHAR(4) NOT NULL REFERENCES dbo.CatSatUsoCFDI(Clave),
    ClaveRegimenFiscal NVARCHAR(3) NOT NULL REFERENCES dbo.CatSatRegimenFiscal(Clave),
    CONSTRAINT PK_RelSatUsoCFDIRegimenFiscal PRIMARY KEY (ClaveUsoCFDI,ClaveRegimenFiscal)
);
CREATE TABLE dbo.CatSatClaveProdServ (
    Clave NVARCHAR(8) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    IncluirIVATrasladado NVARCHAR(30) NOT NULL,
    IncluirIEPSTrasladado NVARCHAR(30) NOT NULL,
    ComplementoDebeIncluir NVARCHAR(500) NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    EstimuloFranjaFronteriza BIT NOT NULL,
    PalabrasSimilares NVARCHAR(MAX) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatClaveProdServ_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatClaveUnidad (
    Clave NVARCHAR(3) NOT NULL PRIMARY KEY,
    Nombre NVARCHAR(500) NOT NULL,
    Descripcion NVARCHAR(MAX) NULL,
    Nota NVARCHAR(MAX) NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Simbolo NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatClaveUnidad_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatObjetoImp (
    Clave NVARCHAR(2) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatObjetoImp_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
CREATE TABLE dbo.CatSatTipoRelacion (
    Clave NVARCHAR(2) NOT NULL PRIMARY KEY,
    Descripcion NVARCHAR(500) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT CK_CatSatTipoRelacion_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- CatEmpresa (Datos del emisor para nómina y CFDI 4.0)
IF OBJECT_ID('dbo.CatEmpresa', 'U') IS NULL
CREATE TABLE dbo.CatEmpresa (
    EmpresaId              INT IDENTITY(1,1) PRIMARY KEY,
    RazonSocial            NVARCHAR(200) NOT NULL,
    RFC                    NVARCHAR(13)  NOT NULL,
    CURP                   NVARCHAR(18)  NULL,
    RegistroPatronal       NVARCHAR(20)  NOT NULL,
    RegimenFiscalId        INT           NOT NULL CONSTRAINT FK_CatEmpresa_RegimenFiscal REFERENCES dbo.CatSatRegimenFiscal(RegimenFiscalId),
    CodigoPostal           NVARCHAR(5)   NOT NULL,
    LugarExpedicion        NVARCHAR(5)   NOT NULL,
    ClaveEntidadFederativa NVARCHAR(3)   NULL CONSTRAINT FK_CatEmpresa_Entidad REFERENCES dbo.CatEntidadFederativa(CveEntidad),
    ClaveOrigenRecurso     NVARCHAR(2) NULL CONSTRAINT FK_CatEmpresa_OrigenRecurso REFERENCES dbo.CatSatOrigenRecurso(Clave),
    NombreComercial        NVARCHAR(200) NULL,
    RfcPatronOrigen        NVARCHAR(13)  NULL,
    MontoRecursoPropio     DECIMAL(18,2) NULL,
    RiesgoPuestoId         INT           NULL CONSTRAINT FK_CatEmpresa_RiesgoPuesto REFERENCES dbo.CatSatRiesgoPuesto(RiesgoPuestoId),
    PrimaRiesgoTrabajoPct  DECIMAL(7,5)  NULL,
    Logo                   VARBINARY(MAX) NULL,
    Activa                 BIT           NOT NULL CONSTRAINT DF_CatEmpresa_Activa DEFAULT 1,
    FechaAlta              DATETIME      NOT NULL CONSTRAINT DF_CatEmpresa_FechaAlta DEFAULT GETDATE(),
    FechaModificacion      DATETIME      NULL,
    CONSTRAINT UQ_CatEmpresa_RFC UNIQUE (RFC),
    CONSTRAINT CK_CatEmpresa_RFC CHECK (LEN(RFC) IN (12,13)),
    CONSTRAINT CK_CatEmpresa_CURP CHECK (CURP IS NULL OR LEN(CURP)=18),
    CONSTRAINT CK_CatEmpresa_RfcPatronOrigen CHECK (RfcPatronOrigen IS NULL OR LEN(RfcPatronOrigen) IN (12,13)),
    CONSTRAINT CK_CatEmpresa_MontoRecursoPropio CHECK (MontoRecursoPropio IS NULL OR MontoRecursoPropio >= 0),
    CONSTRAINT CK_CatEmpresa_PrimaRT CHECK (PrimaRiesgoTrabajoPct IS NULL OR PrimaRiesgoTrabajoPct BETWEEN 0.5 AND 15),
    CONSTRAINT CK_CatEmpresa_CP CHECK (LEN(CodigoPostal)=5 AND CodigoPostal NOT LIKE '%[^0-9]%'),
);
GO

-- CatDepartamento y CatPuesto: estructura organizacional por empresa
IF OBJECT_ID('dbo.CatDepartamento', 'U') IS NULL
CREATE TABLE dbo.CatDepartamento (
    DepartamentoId INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId      INT NOT NULL CONSTRAINT FK_CatDepartamento_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    Clave          NVARCHAR(20)  NOT NULL,
    Descripcion    NVARCHAR(100) NOT NULL,
    Activo         BIT NOT NULL CONSTRAINT DF_CatDepartamento_Activo DEFAULT 1,
    CONSTRAINT UQ_CatDepartamento_Clave UNIQUE (EmpresaId, Clave)
);
GO

IF OBJECT_ID('dbo.CatPuesto', 'U') IS NULL
CREATE TABLE dbo.CatPuesto (
    PuestoId        INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId       INT NOT NULL CONSTRAINT FK_CatPuesto_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    Clave           NVARCHAR(20)  NOT NULL,
    Descripcion     NVARCHAR(100) NOT NULL,
    SalarioBandaMin DECIMAL(18,4) NULL,
    SalarioBandaMax DECIMAL(18,4) NULL,
    Activo          BIT NOT NULL CONSTRAINT DF_CatPuesto_Activo DEFAULT 1,
    CONSTRAINT UQ_CatPuesto_Clave UNIQUE (EmpresaId, Clave),
    CONSTRAINT CK_CatPuesto_Bandas CHECK (SalarioBandaMin IS NULL OR SalarioBandaMax IS NULL OR SalarioBandaMin <= SalarioBandaMax)
);
GO

-- HistEmpresaCSD (Certificados de Sello Digital y rotación)
IF OBJECT_ID('dbo.HistEmpresaCSD', 'U') IS NULL
CREATE TABLE dbo.HistEmpresaCSD (
    CSDId                 INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId             INT             NOT NULL CONSTRAINT FK_HistEmpresaCSD_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    NoCertificado         NVARCHAR(20)    NOT NULL,
    CertificadoCSD        VARBINARY(MAX)  NOT NULL,
    LlaveCSD              VARBINARY(MAX)  NOT NULL,
    PasswordCSD_Encrypted VARBINARY(256)  NOT NULL,
    PasswordCSD_IV        VARBINARY(16)   NOT NULL,
    VigenciaDesde         DATE            NOT NULL,
    VigenciaHasta         DATE            NOT NULL,
    Activo                BIT             NOT NULL CONSTRAINT DF_HistEmpresaCSD_Activo DEFAULT 1,
    FechaAlta             DATETIME        NOT NULL CONSTRAINT DF_HistEmpresaCSD_FechaAlta DEFAULT GETDATE(),
    FechaBaja             DATETIME        NULL,
    MotivoBaja            NVARCHAR(200)   NULL,
    CONSTRAINT UQ_HistEmpresaCSD_NoCert UNIQUE (NoCertificado),
    CONSTRAINT CK_HistEmpresaCSD_Vigencia CHECK (VigenciaHasta >= VigenciaDesde)
);
GO

-- CatEmpresaRegistroPatronal (Multi-registro patronal multi-entidad)
IF OBJECT_ID('dbo.CatEmpresaRegistroPatronal', 'U') IS NULL
CREATE TABLE dbo.CatEmpresaRegistroPatronal (
    RegistroPatronalId     INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId              INT          NOT NULL CONSTRAINT FK_CatERP_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    ClaveEntidadFederativa NVARCHAR(3)  NOT NULL CONSTRAINT FK_CatERP_Entidad REFERENCES dbo.CatEntidadFederativa(CveEntidad),
    NumeroRegistroPatronal NVARCHAR(20) NOT NULL,
    ClaseRiesgoActual      INT          NOT NULL CONSTRAINT FK_CatERP_Riesgo REFERENCES dbo.CatSatRiesgoPuesto(RiesgoPuestoId),
    PrimaRTVigente         DECIMAL(10,8) NOT NULL,
    VigenciaPrimaDesde     DATE         NOT NULL,
    VigenciaPrimaHasta     DATE         NULL,
    Activo                 BIT          NOT NULL CONSTRAINT DF_CatERP_Activo DEFAULT 1,
    FechaAlta              DATETIME     NOT NULL CONSTRAINT DF_CatERP_FechaAlta DEFAULT GETDATE(),
    CONSTRAINT UQ_CatERP_Empresa_Num UNIQUE (EmpresaId, NumeroRegistroPatronal),
    CONSTRAINT CK_CatERP_PrimaRT CHECK (PrimaRTVigente BETWEEN 0.00500 AND 0.15000),
    CONSTRAINT CK_CatERP_Vigencia CHECK (VigenciaPrimaHasta IS NULL OR VigenciaPrimaHasta >= VigenciaPrimaDesde)
);
GO

-- CatEjercicioFiscal (Control anual de periodos e impuestos)
IF OBJECT_ID('dbo.CatEjercicioFiscal', 'U') IS NULL
CREATE TABLE dbo.CatEjercicioFiscal (
    EjercicioId INT IDENTITY(1,1) PRIMARY KEY,
    Anio        INT NOT NULL,
    Estatus     NVARCHAR(20) NOT NULL CONSTRAINT DF_CatEjercicioFiscal_Estatus DEFAULT 'ABIERTO',
    -- Jornada máxima semanal de ley vigente en el ejercicio (LFT Art. 61): 48 h hasta 2026, 46 h desde 2027.
    -- La jornada diaria estándar (y por tanto el salario por hora) se deriva como JornadaSemanalHoras / 6.
    JornadaSemanalHoras DECIMAL(5,2) NOT NULL CONSTRAINT DF_CatEjercicioFiscal_Jornada DEFAULT 48,
    CONSTRAINT UQ_CatEjercicioFiscal_Anio UNIQUE (Anio),
    CONSTRAINT CK_CatEjercicioFiscal_Estatus CHECK (Estatus IN ('ABIERTO','CERRADO')),
    CONSTRAINT CK_CatEjercicioFiscal_Jornada CHECK (JornadaSemanalHoras BETWEEN 1 AND 48)
);
GO

-- CatEmpleado (Maestro principal del trabajador)
IF OBJECT_ID('dbo.CatEmpleado', 'U') IS NULL
CREATE TABLE dbo.CatEmpleado (
    EmpleadoId        INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId         INT NOT NULL CONSTRAINT FK_CatEmpleado_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    NumeroEmpleado    NVARCHAR(15) NOT NULL,
    Nombres           NVARCHAR(100) NOT NULL,
    ApellidoPaterno   NVARCHAR(100) NOT NULL,
    ApellidoMaterno   NVARCHAR(100) NULL,
    FechaIngreso      DATE NOT NULL,
    FechaNacimiento   DATE NULL,
    Sexo              CHAR(1) NULL,
    EstadoCivil       NVARCHAR(20) NULL,
    Nacionalidad      NVARCHAR(50) NULL CONSTRAINT DF_CatEmpleado_Nacionalidad DEFAULT 'MEXICANA',
    Estatus           NVARCHAR(20) NOT NULL CONSTRAINT DF_CatEmpleado_Estatus DEFAULT 'ACTIVO',
    TurnoId           INT NULL CONSTRAINT FK_CatEmpleado_Turno REFERENCES dbo.CatTurno(TurnoId),
    FechaAlta         DATETIME NOT NULL CONSTRAINT DF_CatEmpleado_FechaAlta DEFAULT GETDATE(),
    FechaModificacion DATETIME NULL,
    CONSTRAINT UQ_CatEmpleado_NumEmp UNIQUE (EmpresaId, NumeroEmpleado),
    CONSTRAINT CK_CatEmpleado_Sexo CHECK (Sexo IS NULL OR Sexo IN ('H','M','X')),
    CONSTRAINT CK_CatEmpleado_EstadoCivil CHECK (EstadoCivil IS NULL OR EstadoCivil IN ('SOLTERO','CASADO','DIVORCIADO','VIUDO','UNION_LIBRE','CONCUBINATO')),
    CONSTRAINT CK_CatEmpleado_Estatus CHECK (Estatus IN ('ACTIVO','BAJA','INCAPACIDAD','SUSPENDIDO'))
);
GO

-- CatEmpleadoDatosFiscales (Datos fiscales del receptor CFDI)
IF OBJECT_ID('dbo.CatEmpleadoDatosFiscales', 'U') IS NULL
CREATE TABLE dbo.CatEmpleadoDatosFiscales (
    EmpleadoId             INT PRIMARY KEY CONSTRAINT FK_CatEDF_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    RFC                    NVARCHAR(13) NOT NULL,
    CURP                   NVARCHAR(18) NOT NULL,
    RegimenFiscalSAT       INT NOT NULL CONSTRAINT FK_CatEDF_Regimen REFERENCES dbo.CatSatRegimenFiscal(RegimenFiscalId),
    CodigoPostal           NVARCHAR(5) NOT NULL,
    ClaveEntidadFederativa NVARCHAR(3) NULL CONSTRAINT FK_CatEDF_Entidad REFERENCES dbo.CatEntidadFederativa(CveEntidad),
    Email                  NVARCHAR(100) NULL,
    Telefono               NVARCHAR(20) NULL,
    CONSTRAINT UQ_CatEDF_RFC UNIQUE (RFC),
    CONSTRAINT UQ_CatEDF_CURP UNIQUE (CURP),
    CONSTRAINT CK_CatEDF_RFC CHECK (LEN(RFC) IN (12,13)),
    CONSTRAINT CK_CatEDF_CURP CHECK (LEN(CURP)=18),
    CONSTRAINT CK_CatEDF_CP CHECK (LEN(CodigoPostal)=5 AND CodigoPostal NOT LIKE '%[^0-9]%'),
    CONSTRAINT CK_CatEDF_Email CHECK (Email IS NULL OR Email LIKE '%_@_%.__%')
);
GO

-- CatEmpleadoDatosIMSS (Datos de seguridad social y cotización)
IF OBJECT_ID('dbo.CatEmpleadoDatosIMSS', 'U') IS NULL
CREATE TABLE dbo.CatEmpleadoDatosIMSS (
    EmpleadoId             INT PRIMARY KEY CONSTRAINT FK_CatEDI_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    NSS                    NVARCHAR(11) NOT NULL,
    SalarioDiario          DECIMAL(18,4) NOT NULL,
    SalarioDiarioIntegrado DECIMAL(18,4) NOT NULL,
    SBC                    DECIMAL(18,4) NOT NULL,
    TipoRiesgo             INT NOT NULL CONSTRAINT FK_CatEDI_Riesgo REFERENCES dbo.CatSatRiesgoPuesto(RiesgoPuestoId),
    CONSTRAINT UQ_CatEDI_NSS UNIQUE (NSS),
    CONSTRAINT CK_CatEDI_NSS CHECK (LEN(NSS)=11 AND NSS NOT LIKE '%[^0-9]%'),
    CONSTRAINT CK_CatEDI_Salarios CHECK (SalarioDiario > 0 AND SalarioDiarioIntegrado > 0 AND SBC > 0)
);
GO

-- CatEmpleadoBancoCuenta (Dispersión bancaria de nómina)
IF OBJECT_ID('dbo.CatEmpleadoBancoCuenta', 'U') IS NULL
CREATE TABLE dbo.CatEmpleadoBancoCuenta (
    BancoCuentaId INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId    INT NOT NULL CONSTRAINT FK_CatEBC_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    BancoId       INT NULL CONSTRAINT FK_CatEBC_Banco REFERENCES dbo.CatSatBanco(BancoId),
    MetodoPagoId  INT NULL CONSTRAINT FK_CatEBC_Metodo REFERENCES dbo.CatFormaPagoDispersion(MetodoPagoId),
    CLABE         CHAR(18) NOT NULL,
    Activa        BIT NOT NULL CONSTRAINT DF_CatEBC_Activa DEFAULT 1,
    CONSTRAINT CK_CatEBC_CLABE_Digits CHECK (CLABE NOT LIKE '%[^0-9]%')
);
GO

-- HistEmpleadoSalario (SCD - Trazabilidad histórica de modificaciones salariales)
IF OBJECT_ID('dbo.HistEmpleadoSalario', 'U') IS NULL
CREATE TABLE dbo.HistEmpleadoSalario (
    SalarioHistId     INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId        INT NOT NULL CONSTRAINT FK_HistSalario_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    FechaEfectiva     DATE NOT NULL,
    SalarioDiario     DECIMAL(18,4) NOT NULL,
    SDI               DECIMAL(18,4) NOT NULL,
    SBC               DECIMAL(18,4) NOT NULL,
    FactorIntegracion DECIMAL(10,6) NOT NULL,
    MotivoCambio      NVARCHAR(200) NULL,
    FechaRegistro     DATETIME NOT NULL CONSTRAINT DF_HistSalario_FechaReg DEFAULT GETDATE()
);
GO

-- HistEmpleadoDatosFiscales (SCD - Trazabilidad de cambios de domicilio/RFC/CURP)
IF OBJECT_ID('dbo.HistEmpleadoDatosFiscales', 'U') IS NULL
CREATE TABLE dbo.HistEmpleadoDatosFiscales (
    HistoricoId            INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId             INT NOT NULL CONSTRAINT FK_HistEDF_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    RFC                    NVARCHAR(13) NULL,
    CURP                   NVARCHAR(18) NULL,
    RegimenFiscalSAT       INT NULL CONSTRAINT FK_HistEDF_Regimen REFERENCES dbo.CatSatRegimenFiscal(RegimenFiscalId),
    CodigoPostal           NVARCHAR(5) NULL,
    ClaveEntidadFederativa NVARCHAR(3) NULL CONSTRAINT FK_HistEDF_Entidad REFERENCES dbo.CatEntidadFederativa(CveEntidad),
    FechaDesde             DATE NOT NULL,
    FechaHasta             DATE NULL,
    UsuarioId              INT NULL CONSTRAINT FK_HistEDF_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    FechaRegistro          DATETIME NOT NULL CONSTRAINT DF_HistEDF_FechaReg DEFAULT GETDATE()
);
GO

-- ==============================================================================
-- [BLOQUE-03] PROCESOS TRANSACCIONALES DE NÓMINA (Proc)
-- ==============================================================================
PRINT '[03/21] Creando tablas de procesos transaccionales (Prefijo Proc)...';
GO

-- ProcPeriodoNomina (Periodos de cálculo)
IF OBJECT_ID('dbo.ProcPeriodoNomina', 'U') IS NULL
CREATE TABLE dbo.ProcPeriodoNomina (
    PeriodoId            INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId            INT NOT NULL CONSTRAINT     FK_ProcPeriodo_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
        EjercicioId          INT NOT NULL CONSTRAINT FK_ProcPeriodo_Ejercicio REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    PeriodicidadPagoId   INT NOT NULL CONSTRAINT FK_ProcPeriodo_Periodicidad REFERENCES dbo.CatSatPeriodicidadPago(PeriodicidadPagoId),
    NumeroPeriodo        INT NOT NULL,
    FechaInicio          DATE NOT NULL,
    FechaFin             DATE NOT NULL,
    FechaPago            DATE NOT NULL,
    FechaFinalPago       DATE NULL,
    DiasPeriodo          DECIMAL(5,2) NOT NULL,
    EsInicioMes          BIT NOT NULL CONSTRAINT DF_ProcPeriodo_IniciaMes DEFAULT 0,
    EsFinMes             BIT NOT NULL CONSTRAINT DF_ProcPeriodo_FinMes DEFAULT 0,
    DiaDescanso          TINYINT NULL,
    CalculaSeptimoDia    BIT NOT NULL CONSTRAINT DF_ProcPeriodo_Septimo DEFAULT 0,
    CalendarioIMSS       DATE NULL,
    Estatus              NVARCHAR(20) NOT NULL CONSTRAINT DF_ProcPeriodo_Estatus DEFAULT 'CREADO',
    UsuarioCalculando    NVARCHAR(150) NULL,
    FechaInicioCalculo   DATETIME NULL,
    CONSTRAINT UQ_ProcPeriodoNomina UNIQUE (EmpresaId, EjercicioId, PeriodicidadPagoId, NumeroPeriodo),
    CONSTRAINT CK_ProcPeriodo_Fechas CHECK (FechaInicio <= FechaFin AND FechaFin <= FechaPago),
    CONSTRAINT CK_ProcPeriodo_Dias CHECK (DiasPeriodo > 0),
    CONSTRAINT CK_ProcPeriodo_DiaDescanso CHECK (DiaDescanso IS NULL OR DiaDescanso BETWEEN 1 AND 7),
    CONSTRAINT CK_ProcPeriodo_Estatus CHECK (Estatus IN ('CREADO','PRENOMINA','EN_CALCULO','CALCULADO','AUTORIZADO','CERRADO','TIMBRADO','CANCELADO'))
);
GO

-- (Las incidencias legales —faltas, horas extra, retardos, permisos— se capturan en ProcIncidencias con un concepto
--  TipoConcepto 'I' de CatConcepto; ya no existe ProcIncidencia ni CatTipoIncidencia.)

-- ProcIncapacidad (Incapacidades oficiales del IMSS)
IF OBJECT_ID('dbo.ProcIncapacidad', 'U') IS NULL
CREATE TABLE dbo.ProcIncapacidad (
    IncapacidadId     INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId        INT NOT NULL CONSTRAINT FK_ProcIncapacidad_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    PeriodoId         INT NULL CONSTRAINT FK_ProcIncapacidad_Periodo REFERENCES dbo.ProcPeriodoNomina(PeriodoId),
    TipoIncapacidadId INT NOT NULL CONSTRAINT FK_ProcIncapacidad_Tipo REFERENCES dbo.CatSatTipoIncapacidad(TipoIncapacidadId),
    FolioIMSS         NVARCHAR(20) NULL,
    DiasIncapacidad   DECIMAL(5,2) NOT NULL,
    FechaInicio       DATE NOT NULL,
    FechaFin          DATE NOT NULL,
    PorcentajePago    DECIMAL(5,2) NOT NULL,
    ImportePagado     DECIMAL(18,2) NULL,
    CONSTRAINT CK_ProcIncapacidad_Dias CHECK (DiasIncapacidad > 0),
    CONSTRAINT CK_ProcIncapacidad_Fechas CHECK (FechaInicio <= FechaFin)
);
GO

-- ProcCreditoInfonavit (Descuentos de créditos hipotecarios INFONAVIT)
IF OBJECT_ID('dbo.ProcCreditoInfonavit', 'U') IS NULL
CREATE TABLE dbo.ProcCreditoInfonavit (
    CreditoId       INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId      INT NOT NULL CONSTRAINT FK_ProcCreditoInf_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    NumeroCredito   NVARCHAR(20) NOT NULL,
    TipoDescuento   NVARCHAR(20) NOT NULL,
    MontoDescuento  DECIMAL(18,4) NOT NULL,
    SaldoPendiente  DECIMAL(18,4) NULL CONSTRAINT DF_ProcCreditoInf_Saldo DEFAULT 0,
    ValorDescuento  DECIMAL(18,4) NULL,
    FechaInicio     DATE NOT NULL,
    FechaFin        DATE NULL,
    Activo          BIT NOT NULL CONSTRAINT DF_ProcCreditoInf_Activo DEFAULT 1,
    Estatus         NVARCHAR(20) NOT NULL CONSTRAINT DF_ProcCreditoInf_Estatus DEFAULT 'ACTIVO',
    CONSTRAINT UQ_ProcCreditoInf_Num UNIQUE (EmpleadoId, NumeroCredito),
    CONSTRAINT CK_ProcCreditoInf_Tipo CHECK (TipoDescuento IN ('CUOTA_FIJA','PORCENTAJE','VSM'))
);
GO

-- ProcCreditoFonacot (Descuentos de préstamos FONACOT)
IF OBJECT_ID('dbo.ProcCreditoFonacot', 'U') IS NULL
CREATE TABLE dbo.ProcCreditoFonacot (
    CreditoFONACOTId  INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId        INT NOT NULL CONSTRAINT FK_ProcCreditoFon_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    NumeroCredito     NVARCHAR(20) NOT NULL,
    MontoOriginal     DECIMAL(18,2) NOT NULL,
    SaldoActual       DECIMAL(18,2) NOT NULL,
    DescuentoPeriodo  DECIMAL(18,2) NOT NULL,
    FechaInicio       DATE NOT NULL,
    FechaEstimadaPago DATE NULL,
    Activo            BIT NOT NULL CONSTRAINT DF_ProcCreditoFon_Activo DEFAULT 1,
    CONSTRAINT UQ_ProcCreditoFon_Num UNIQUE (NumeroCredito),
    CONSTRAINT CK_ProcCreditoFon_Saldo CHECK (SaldoActual >= 0),
    CONSTRAINT CK_ProcCreditoFon_Descuento CHECK (DescuentoPeriodo > 0)
);
GO

-- ProcVacacionesSaldo (Control de días devengados por aniversario)
IF OBJECT_ID('dbo.ProcVacacionesSaldo', 'U') IS NULL
CREATE TABLE dbo.ProcVacacionesSaldo (
    SaldoVacacionesId INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId        INT NOT NULL CONSTRAINT FK_ProcVacSaldo_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    AnioAniversario   INT NOT NULL,
    FechaAniversario  DATE NOT NULL,
    DiasDevengados    DECIMAL(5,2) NOT NULL,
    DiasConsumidos    DECIMAL(5,2) NOT NULL CONSTRAINT DF_ProcVacSaldo_Consumidos DEFAULT 0,
    DiasDisponibles   AS (DiasDevengados - DiasConsumidos) PERSISTED,
    FechaExpiracion   DATE NULL,
    Activo            BIT NOT NULL CONSTRAINT DF_ProcVacSaldo_Activo DEFAULT 1,
    CONSTRAINT UQ_ProcVacSaldo UNIQUE (EmpleadoId, AnioAniversario),
    CONSTRAINT CK_ProcVacSaldo_Consumidos CHECK (DiasConsumidos >= 0 AND DiasConsumidos <= DiasDevengados)
);
GO

-- ProcVacacionesDisfrute (Registro de goce de periodos vacacionales y prima)
IF OBJECT_ID('dbo.ProcVacacionesDisfrute', 'U') IS NULL
CREATE TABLE dbo.ProcVacacionesDisfrute (
    VacacionesId    INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId      INT NOT NULL CONSTRAINT FK_ProcVacDisfrute_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    PeriodoId       INT NULL CONSTRAINT FK_ProcVacDisfrute_Periodo REFERENCES dbo.ProcPeriodoNomina(PeriodoId),
    DiasDisfrutados DECIMAL(5,2) NOT NULL,
    PrimaVacacional DECIMAL(18,2) NOT NULL,
    FechaInicio     DATE NULL,
    FechaFin        DATE NULL,
    CONSTRAINT CK_ProcVacDisfrute_Dias CHECK (DiasDisfrutados > 0)
);
GO

-- ProcPrestamo (Préstamos internos de empresa y amortización)
IF OBJECT_ID('dbo.ProcPrestamo', 'U') IS NULL
CREATE TABLE dbo.ProcPrestamo (
    PrestamoId          INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId          INT NOT NULL CONSTRAINT FK_ProcPrestamo_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    MontoTotal          DECIMAL(18,2) NOT NULL,
    SaldoRestante       DECIMAL(18,2) NOT NULL,
    DescuentoPorPeriodo DECIMAL(18,2) NOT NULL,
    NumPeriodos         INT NULL,
    Estatus             NVARCHAR(20) NOT NULL CONSTRAINT DF_ProcPrestamo_Estatus DEFAULT 'ACTIVO',
    FechaAlta           DATETIME NOT NULL CONSTRAINT DF_ProcPrestamo_FechaAlta DEFAULT GETDATE(),
    CONSTRAINT CK_ProcPrestamo_Estatus CHECK (Estatus IN ('ACTIVO','LIQUIDADO','CANCELADO')),
    CONSTRAINT CK_ProcPrestamo_Montos CHECK (MontoTotal > 0 AND SaldoRestante >= 0 AND DescuentoPorPeriodo > 0)
);
GO

-- ProcRegistroAsistencia (Checadas yPrenómina)
IF OBJECT_ID('dbo.ProcRegistroAsistencia', 'U') IS NULL
CREATE TABLE dbo.ProcRegistroAsistencia (
    RegistroId       BIGINT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId       INT NOT NULL CONSTRAINT FK_ProcRegAsist_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    Fecha            DATE NOT NULL,
    HoraEntrada      TIME NULL,
    HoraSalida       TIME NULL,
    MinutosRetardo   INT NOT NULL CONSTRAINT DF_ProcRegAsist_Retardo DEFAULT 0,
    HorasTrabajadas  DECIMAL(5,2) NULL,
    HorasExtraDoble  DECIMAL(5,2) NOT NULL CONSTRAINT DF_ProcRegAsist_HED DEFAULT 0,
    HorasExtraTriple DECIMAL(5,2) NOT NULL CONSTRAINT DF_ProcRegAsist_HET DEFAULT 0,
    Origen           NVARCHAR(20) NOT NULL CONSTRAINT DF_ProcRegAsist_Origen DEFAULT 'MANUAL',
    Observaciones    NVARCHAR(300) NULL,
    CONSTRAINT UQ_ProcRegAsist UNIQUE (EmpleadoId, Fecha),
    CONSTRAINT CK_ProcRegAsist_Origen CHECK (Origen IN ('MANUAL','CHECADOR','IMPORT')),
    CONSTRAINT CK_ProcRegAsist_Horas CHECK (HorasTrabajadas IS NULL OR HorasTrabajadas BETWEEN 0 AND 24),
    CONSTRAINT CK_ProcRegAsist_HE CHECK (HorasExtraDoble >= 0 AND HorasExtraTriple >= 0)
);
GO

-- ProcAguinaldo (Cálculo anualizado de aguinaldo LFT Art. 87)
IF OBJECT_ID('dbo.ProcAguinaldo', 'U') IS NULL
CREATE TABLE dbo.ProcAguinaldo (
    AguinaldoId        INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId         INT NOT NULL CONSTRAINT     FK_ProcAguinaldo_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
        EjercicioId        INT NOT NULL CONSTRAINT FK_ProcAguinaldo_Ejercicio REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    DiasProporcionales DECIMAL(5,2) NOT NULL,
    MontoPagar         DECIMAL(18,2) NOT NULL,
    CONSTRAINT UQ_ProcAguinaldo UNIQUE (EmpleadoId, EjercicioId)
);
GO

-- ProcPTU (Participación de los Trabajadores en las Utilidades Arts. 117-131 LFT)
IF OBJECT_ID('dbo.ProcPTU', 'U') IS NULL
CREATE TABLE dbo.ProcPTU (
    PTUId               INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId          INT NOT NULL CONSTRAINT     FK_ProcPTU_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
        EjercicioId         INT NOT NULL CONSTRAINT FK_ProcPTU_Ejercicio REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    DiasTrabajadosAnio  INT NOT NULL,
    SalarioBaseAnual    DECIMAL(18,4) NOT NULL,
    MontoPorDias        DECIMAL(18,4) NOT NULL,
    MontoPorSalario     DECIMAL(18,4) NOT NULL,
    MontoTotal          DECIMAL(18,4) NOT NULL,
    MontoExento         DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcPTU_Exento DEFAULT 0,
    MontoGravado        DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcPTU_Gravado DEFAULT 0,
    FechaCalculo        DATETIME NOT NULL CONSTRAINT DF_ProcPTU_FechaCalc DEFAULT GETDATE(),
    CONSTRAINT UQ_ProcPTU UNIQUE (EmpleadoId, EjercicioId)
);
GO

-- ProcAcumuladoMensual (Acumulados fiscales para cálculo de ISR anual y PTU)
IF OBJECT_ID('dbo.ProcAcumuladoMensual', 'U') IS NULL
CREATE TABLE dbo.ProcAcumuladoMensual (
    AcumuladoId       INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId        INT NOT NULL CONSTRAINT     FK_ProcAcumulado_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
        EjercicioId       INT NOT NULL CONSTRAINT FK_ProcAcumulado_Ejercicio REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    Mes               INT NOT NULL,
    BaseGravadaISR    DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAcum_Gravada DEFAULT 0,
    BaseExentaISR     DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAcum_Exenta DEFAULT 0,
    BaseIMSS          DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAcum_BaseIMSS DEFAULT 0,
    SubsidioEntregado DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAcum_Subsidio DEFAULT 0,
    ISRRetenido       DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAcum_ISRRet DEFAULT 0,
    ISRPagadoExceso   DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAcum_ISRExc DEFAULT 0,
    TotalPercepciones DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAcum_Perc DEFAULT 0,
    TotalDeducciones  DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAcum_Ded DEFAULT 0,
    CuotaObrera       DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAcum_CuotaObr DEFAULT 0,
    CONSTRAINT UQ_ProcAcumuladoMensual UNIQUE (EmpleadoId, EjercicioId, Mes),
    CONSTRAINT CK_ProcAcumulado_Mes CHECK (Mes BETWEEN 1 AND 12)
);
GO

-- ProcAjusteAnualISR (Ajuste fiscal anual Art. 97 LISR)
IF OBJECT_ID('dbo.ProcAjusteAnualISR', 'U') IS NULL
CREATE TABLE dbo.ProcAjusteAnualISR (
    AjusteId            INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId          INT NOT NULL CONSTRAINT     FK_ProcAjusteISR_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
        EjercicioId         INT NOT NULL CONSTRAINT FK_ProcAjusteISR_Ejercicio REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    BaseGravadaAnual    DECIMAL(18,4) NOT NULL,
    ISRDeterminado      DECIMAL(18,4) NOT NULL,
    ISRRetenidoEnAnio   DECIMAL(18,4) NOT NULL,
    SubsidioEntregado   DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAjusteISR_Sub DEFAULT 0,
    DiferenciaFavorSAT  DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAjusteISR_SAT DEFAULT 0,
    DiferenciaFavorTrab DECIMAL(18,4) NOT NULL CONSTRAINT DF_ProcAjusteISR_Trab DEFAULT 0,
    FechaCalculo        DATETIME NOT NULL CONSTRAINT DF_ProcAjusteISR_Fecha DEFAULT GETDATE(),
    Aplicado            BIT NOT NULL CONSTRAINT DF_ProcAjusteISR_Aplicado DEFAULT 0,
    CONSTRAINT UQ_ProcAjusteAnual UNIQUE (EmpleadoId, EjercicioId)
);
GO

-- ==============================================================================
-- [BLOQUE-04] CONFIGURACIÓN FISCAL Y POLÍTICAS DEL SISTEMA (Cat)
-- ==============================================================================
PRINT '[04/21] Creando tablas de configuración fiscal y políticas (Prefijo Cat)...';
GO

-- CatUMA (Valores de la Unidad de Medida y Actualización)
IF OBJECT_ID('dbo.CatUMA', 'U') IS NULL
CREATE TABLE dbo.CatUMA (
    UMAId         INT IDENTITY(1,1) PRIMARY KEY,
    Anio          INT NOT NULL,
    ValorDiario   DECIMAL(12,4) NOT NULL,
    ValorMensual  DECIMAL(12,4) NOT NULL,
    ValorAnual    DECIMAL(12,4) NOT NULL,
    VigenciaDesde DATE NOT NULL,
    VigenciaHasta DATE NOT NULL,
    CONSTRAINT CK_CatUMA_Vigencia CHECK (VigenciaHasta >= VigenciaDesde),
    CONSTRAINT UQ_CatUMA_Anio UNIQUE (Anio)
);
GO

-- CatSalarioMinimo (Salarios mínimos generales y Zona Libre de la Frontera Norte)
IF OBJECT_ID('dbo.CatSalarioMinimo', 'U') IS NULL
CREATE TABLE dbo.CatSalarioMinimo (
    SMId          INT IDENTITY(1,1) PRIMARY KEY,
    Anio          INT NOT NULL,
    Zona          NVARCHAR(10) NOT NULL,
    ValorDiario   DECIMAL(12,4) NOT NULL,
    VigenciaDesde DATE NOT NULL,
    VigenciaHasta DATE NOT NULL,
    CONSTRAINT CK_CatSalarioMinimo_Vigencia CHECK (VigenciaHasta >= VigenciaDesde),
    CONSTRAINT UQ_CatSalarioMinimo UNIQUE (Anio, Zona),
    CONSTRAINT CK_CatSalarioMinimo_Zona CHECK (Zona IN ('GENERAL','ZFN'))
);
GO

-- CatTablaISR (Tarifas mensuales Art. 96 LISR)
IF OBJECT_ID('dbo.CatTablaISR', 'U') IS NULL
CREATE TABLE dbo.CatTablaISR (
    TablaISRId     INT IDENTITY(1,1) PRIMARY KEY,
    EjercicioId    INT NOT NULL CONSTRAINT FK_CatTablaISR_Ejercicio REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    Renglon        INT NOT NULL,
    LimiteInferior DECIMAL(18,4) NOT NULL,
    LimiteSuperior DECIMAL(18,4) NOT NULL,
    CuotaFija      DECIMAL(18,4) NOT NULL,
    TasaExcedente  DECIMAL(10,8) NOT NULL,
    CONSTRAINT UQ_CatTablaISR UNIQUE (EjercicioId, Renglon),
    CONSTRAINT CK_CatTablaISR_Renglon CHECK (Renglon > 0),
    CONSTRAINT CK_CatTablaISR_Limites CHECK (LimiteSuperior >= LimiteInferior)
);
GO

CREATE TABLE dbo.CatTablaISRAnual (
    TablaISRAnualId INT IDENTITY PRIMARY KEY,
    EjercicioId INT NOT NULL REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    Renglon INT NOT NULL CHECK (Renglon > 0),
    LimiteInferior DECIMAL(18,4) NOT NULL,
    LimiteSuperior DECIMAL(18,4) NOT NULL,
    CuotaFija DECIMAL(18,4) NOT NULL,
    TasaExcedente DECIMAL(10,8) NOT NULL,
    CONSTRAINT UQ_CatTablaISRAnual UNIQUE (EjercicioId,Renglon),
    CONSTRAINT CK_CatTablaISRAnual_Valores CHECK (LimiteSuperior >= LimiteInferior AND LimiteInferior >= 0 AND CuotaFija >= 0 AND TasaExcedente BETWEEN 0 AND 1)
);
GO
-- CatTablaSubsidioEmpleo (Decreto Subsidio al Empleo)
IF OBJECT_ID('dbo.CatTablaSubsidioEmpleo', 'U') IS NULL
CREATE TABLE dbo.CatTablaSubsidioEmpleo (
    SubsidioId    INT IDENTITY(1,1) PRIMARY KEY,
    EjercicioId   INT NOT NULL CONSTRAINT FK_CatTablaSubsidio_Ejercicio REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    HastaIngreso  DECIMAL(18,4) NOT NULL,
    MontoSubsidio DECIMAL(18,4) NOT NULL,
    PermiteEntrega BIT NOT NULL CONSTRAINT DF_CatTablaSubsidio_Entrega DEFAULT 0
);
GO

-- CatCuotaIMSS (Tasas obrero-patronales LSS)
IF OBJECT_ID('dbo.CatCuotaIMSS', 'U') IS NULL
CREATE TABLE dbo.CatCuotaIMSS (
    ConfigIMSSId       INT IDENTITY(1,1) PRIMARY KEY,
    EjercicioId        INT NOT NULL CONSTRAINT FK_CatCuotaIMSS_Ejercicio REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    ClaveConcepto      NVARCHAR(10)  NOT NULL,
    Descripcion        NVARCHAR(150) NOT NULL,
    PorcentajePatronal DECIMAL(10,8) NOT NULL,
    PorcentajeObrero   DECIMAL(10,8) NOT NULL,
    BaseCalculo        NVARCHAR(30)  NOT NULL,
    CONSTRAINT CK_CatCuotaIMSS_Base CHECK (BaseCalculo IN ('SBC','SBC_EXCEDENTE_3UMA','UMA_DIARIA_POR_TRAB'))
);
GO

-- CatTablaVacaciones (Escala legal Art. 76 LFT)
IF OBJECT_ID('dbo.CatTablaVacaciones', 'U') IS NULL
CREATE TABLE dbo.CatTablaVacaciones (
    TablaVacId         INT IDENTITY(1,1) PRIMARY KEY,
    AniosServicioDesde INT NOT NULL,
    AniosServicioHasta INT NOT NULL,
    DiasVacaciones     INT NOT NULL,
    CONSTRAINT CK_CatTablaVac_Anios CHECK (AniosServicioHasta >= AniosServicioDesde),
    CONSTRAINT CK_CatTablaVac_Dias CHECK (DiasVacaciones >= 12)
);
GO

-- CatSistema (Parámetros y feature flags globales)
IF OBJECT_ID('dbo.CatSistema', 'U') IS NULL
CREATE TABLE dbo.CatSistema (
    ConfiguracionId INT IDENTITY(1,1) PRIMARY KEY,
    Clave           NVARCHAR(80) NOT NULL,
    Valor           NVARCHAR(1000) NULL,
    Descripcion     NVARCHAR(300) NULL,
    Cifrado         BIT NOT NULL CONSTRAINT DF_CatSistema_Cifrado DEFAULT 0,
    EditadoPor      INT NULL CONSTRAINT FK_CatSistema_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    FechaEdicion    DATETIME NOT NULL CONSTRAINT DF_CatSistema_FechaEdicion DEFAULT GETDATE(),
    CONSTRAINT UQ_CatSistema_Clave UNIQUE (Clave)
);
GO

-- CatPoliticaEmpresarial (Políticas de beneficios y prestaciones por empresa)
IF OBJECT_ID('dbo.CatPoliticaEmpresarial', 'U') IS NULL
CREATE TABLE dbo.CatPoliticaEmpresarial (
    PoliticaId                    INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId                     INT NOT NULL CONSTRAINT FK_CatPolitica_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    DiasAguinaldo                 DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_DiasAgui DEFAULT 15.00,
    ExencionAguinaldoUMAs         DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_ExenAgui DEFAULT 30.00,
    PorcentajePrimaVacacional     DECIMAL(5,2)  NOT NULL CONSTRAINT DF_CatPol_PorcPV DEFAULT 25.00,
    ExencionPrimaVacacionalUMAs   DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_ExenPV DEFAULT 15.00,
    DiasPrimaAntiguedad           DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_DiasPA DEFAULT 12.00,
    FactorTopePrimaAntiguedad     DECIMAL(5,2)  NOT NULL CONSTRAINT DF_CatPol_TopePA DEFAULT 2.00,
    FactorHoraExtraDoble          DECIMAL(5,2)  NOT NULL CONSTRAINT DF_CatPol_FacHED DEFAULT 2.00,
    FactorHoraExtraTriple         DECIMAL(5,2)  NOT NULL CONSTRAINT DF_CatPol_FacHET DEFAULT 3.00,
    ExencionHorasExtraUMAsSemanal DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_ExenHE DEFAULT 5.00,
    LimiteHorasExtraSinGravar     DECIMAL(5,2)  NOT NULL CONSTRAINT DF_CatPol_LimHE DEFAULT 9.00,
    PorcentajeFondoAhorroEmpleado DECIMAL(5,2)  NOT NULL CONSTRAINT DF_CatPol_FAEmp DEFAULT 0.00,
    PorcentajeFondoAhorroEmpresa  DECIMAL(5,2)  NOT NULL CONSTRAINT DF_CatPol_FAPat DEFAULT 0.00,
    TopeExentoFondoAhorroSMG      DECIMAL(5,2)  NOT NULL CONSTRAINT DF_CatPol_TopeFA DEFAULT 1.30,
    PorcentajePrimaDominical      DECIMAL(5,2)  NOT NULL CONSTRAINT DF_CatPol_PorcPD DEFAULT 25.00,
    ExencionPrimaDominicalUMAs    DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_ExenPD DEFAULT 1.00,
    ExencionPTUUMAs               DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_ExenPTU DEFAULT 15.00,
    TopeExentoValesDespensaUMAs   DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_TopeVales DEFAULT 1.00,
    DiasIndemnizacion3Meses       DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_Indem90 DEFAULT 90.00,
    DiasIndemnizacion20DiasAnio   DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatPol_Indem20 DEFAULT 20.00,
    FechaInicioVigencia           DATE NOT NULL CONSTRAINT DF_CatPol_FechaInicio DEFAULT GETDATE(),
    FechaFinVigencia              DATE NULL,
    Activa                        BIT NOT NULL CONSTRAINT DF_CatPol_Activa DEFAULT 1,
    FechaCreacion                 DATETIME NOT NULL CONSTRAINT DF_CatPol_FechaCreacion DEFAULT GETDATE(),
    CreadoPor                     NVARCHAR(100) NULL,
    CONSTRAINT UQ_CatPol_Vigencia UNIQUE (EmpresaId, FechaInicioVigencia),
    CONSTRAINT CK_CatPol_Aguinaldo CHECK (DiasAguinaldo >= 15.00),
    CONSTRAINT CK_CatPol_PrimaVac CHECK (PorcentajePrimaVacacional >= 25.00),
    CONSTRAINT CK_CatPol_PrimaAntig CHECK (DiasPrimaAntiguedad >= 12.00),
    CONSTRAINT CK_CatPol_PrimaDom CHECK (PorcentajePrimaDominical >= 25.00),
    CONSTRAINT CK_CatPol_Vigencia CHECK (FechaFinVigencia IS NULL OR FechaFinVigencia >= FechaInicioVigencia)
);
GO

-- CatFormulaCalculo (Motor de fórmulas de percepciones y deducciones)
IF OBJECT_ID('dbo.CatFormulaCalculo', 'U') IS NULL
CREATE TABLE dbo.CatFormulaCalculo (
    FormulaId            INT IDENTITY(1,1) PRIMARY KEY,
    TipoFormulaId        INT NOT NULL CONSTRAINT FK_CatFormula_Tipo REFERENCES dbo.CatTipoFormula(TipoFormulaId),
    Nombre               NVARCHAR(100) NOT NULL,
    Descripcion          NVARCHAR(300) NULL,
    ExpresionCalculo     NVARCHAR(1000) NOT NULL,
    ParametrosDefault    NVARCHAR(MAX) NULL,
    VariablesDisponibles NVARCHAR(500) NULL,
    RequiereAprobacion   BIT NOT NULL CONSTRAINT DF_CatFormula_Aprobacion DEFAULT 0,
    Activa               BIT NOT NULL CONSTRAINT DF_CatFormula_Activa DEFAULT 1,
    FechaCreacion        DATETIME NOT NULL CONSTRAINT DF_CatFormula_FechaCreacion DEFAULT GETDATE(),
    CreadoPor            NVARCHAR(100) NULL,
    CONSTRAINT UQ_CatFormulaCalculo_Nombre UNIQUE (Nombre)
);
GO

-- CatConcepto (Catálogo operativo de conceptos de nómina y mapeo a SAT y Fórmulas)
IF OBJECT_ID('dbo.CatConcepto', 'U') IS NULL
CREATE TABLE dbo.CatConcepto (
    ConceptoId       INT IDENTITY(1,1) PRIMARY KEY,
    ClaveSAT         NVARCHAR(20) NOT NULL,
    TipoConcepto     CHAR(1)      NOT NULL,
    Descripcion      NVARCHAR(500) NOT NULL,
    EsGravado        BIT NOT NULL CONSTRAINT DF_CatConcepto_EsGravado DEFAULT 1,
    FormaCalculo     NVARCHAR(20) NULL,
    FactorMultiplicador DECIMAL(10,4) NOT NULL CONSTRAINT DF_CatConcepto_Factor DEFAULT 1,
    EsIncidencia     BIT NOT NULL CONSTRAINT DF_CatConcepto_EsIncidencia DEFAULT 0,
    EsBase           BIT NOT NULL CONSTRAINT DF_CatConcepto_EsBase DEFAULT 0,
    -- TipoConcepto 'I' = incidencia legal que el motor de nómina interpreta por su código (ClaveSAT guarda ese código:
    -- FALTA, HE_DOBLE, PRIMA_DOM, BONO...) con las reglas de la LFT/LISR y la política de la empresa. Reemplaza al antiguo
    -- CatTipoIncidencia. No es una clave SAT: nunca se timbra directamente, el cálculo emite sus renglones con la clave SAT que corresponda.
    DisminuyeDias    BIT NOT NULL CONSTRAINT DF_CatConcepto_DisminuyeDias DEFAULT 0,
    TipoHorasId      INT NULL CONSTRAINT FK_CatConcepto_TipoHoras REFERENCES dbo.CatSatTipoHoras(TipoHorasId),
    TopeUMAAnual     DECIMAL(10,4) NULL,
    TipoPercepcionId INT NULL CONSTRAINT FK_CatConcepto_Percepcion REFERENCES dbo.CatSatTipoPercepcion(TipoPercepcionId),
    TipoDeduccionId  INT NULL CONSTRAINT FK_CatConcepto_Deduccion REFERENCES dbo.CatSatTipoDeduccion(TipoDeduccionId),
    TipoOtroPagoId   INT NULL CONSTRAINT FK_CatConcepto_OtroPago REFERENCES dbo.CatSatTipoOtroPago(TipoOtroPagoId),
    FormulaId        INT NULL CONSTRAINT FK_CatConcepto_Formula REFERENCES dbo.CatFormulaCalculo(FormulaId),
    Activo           BIT NOT NULL CONSTRAINT DF_CatConcepto_Activo DEFAULT 1,
    -- Denominación/unidad de medida oficial del SAT (c_ClaveUnidad): DIA, HUR (hora) o ACT (actividad/monto).
    ClaveUnidadSAT   NVARCHAR(3) NOT NULL CONSTRAINT DF_CatConcepto_Unidad DEFAULT N'ACT'
        CONSTRAINT FK_CatConcepto_Unidad REFERENCES dbo.CatSatClaveUnidad(Clave),
    CONSTRAINT UQ_CatConcepto_SAT UNIQUE (ClaveSAT, TipoConcepto),
    CONSTRAINT CK_CatConcepto_Tipo CHECK (TipoConcepto IN ('P','D','O','I')),
    CONSTRAINT CK_CatConcepto_Legal CHECK (
        TipoConcepto <> 'I' OR (FormaCalculo IS NOT NULL AND EsIncidencia = 0 AND EsBase = 0)
    ),
    CONSTRAINT CK_CatConcepto_DisminuyeDias CHECK ((DisminuyeDias = 0 AND TipoHorasId IS NULL) OR TipoConcepto = 'I'),
    CONSTRAINT CK_CatConcepto_UnSoloCatSAT CHECK (
        (CASE WHEN TipoPercepcionId IS NOT NULL THEN 1 ELSE 0 END)
      + (CASE WHEN TipoDeduccionId  IS NOT NULL THEN 1 ELSE 0 END)
      + (CASE WHEN TipoOtroPagoId   IS NOT NULL THEN 1 ELSE 0 END) <= 1
    ),
    CONSTRAINT CK_CatConcepto_CatCoherente CHECK (
        (TipoConcepto = 'P' AND TipoPercepcionId IS NOT NULL AND TipoDeduccionId IS NULL AND TipoOtroPagoId IS NULL)
     OR (TipoConcepto = 'D' AND TipoDeduccionId IS NOT NULL AND TipoPercepcionId IS NULL AND TipoOtroPagoId IS NULL)
     OR (TipoConcepto = 'O' AND TipoOtroPagoId IS NOT NULL AND TipoPercepcionId IS NULL AND TipoDeduccionId IS NULL)
     OR (TipoConcepto = 'I' AND TipoPercepcionId IS NULL AND TipoDeduccionId IS NULL AND TipoOtroPagoId IS NULL)
    ),
    CONSTRAINT CK_CatConcepto_FormaCalculo CHECK (
        FormaCalculo IS NULL OR FormaCalculo IN ('MONTO_FIJO','DIAS','HORAS','PORCENTAJE_SD')
    ),
    -- Un concepto capturable como incidencia exige regla de cálculo y ser Percepción o Deducción.
    CONSTRAINT CK_CatConcepto_Incidencia CHECK (
        EsIncidencia = 0 OR (FormaCalculo IS NOT NULL AND FactorMultiplicador > 0 AND TipoConcepto IN ('P','D'))
    ),
    -- Un concepto base lo calcula el motor de nómina: no se captura como incidencia.
    CONSTRAINT CK_CatConcepto_Base CHECK (EsBase = 0 OR EsIncidencia = 0)
);
GO

-- ==============================================================================
-- [BLOQUE-05] DOCUMENTOS LEGALES, CONTRATOS Y COMPROBANTES (Doc)
-- ==============================================================================
PRINT '[05/21] Creando documentos y comprobantes (Prefijo Doc)...';
GO

-- DocContratoEmpleado (Contrato laboral individual)
IF OBJECT_ID('dbo.DocContratoEmpleado', 'U') IS NULL
CREATE TABLE dbo.DocContratoEmpleado (
    EmpleadoContratoId     INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId             INT NOT NULL CONSTRAINT FK_DocContrato_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    TipoContratoId         INT NOT NULL CONSTRAINT FK_DocContrato_TipoContrato REFERENCES dbo.CatSatTipoContrato(TipoContratoId),
    DepartamentoId         INT NULL CONSTRAINT FK_DocContrato_Depto REFERENCES dbo.CatDepartamento(DepartamentoId),
    PuestoId               INT NULL CONSTRAINT FK_DocContrato_Puesto REFERENCES dbo.CatPuesto(PuestoId),
    CentroCostoId          INT NULL CONSTRAINT FK_DocContrato_CentroCosto REFERENCES dbo.CatCentroCosto(CentroCostoId),
    TipoJornadaId          INT NOT NULL CONSTRAINT DF_DocContrato_Jornada DEFAULT 1 CONSTRAINT FK_DocContrato_TipoJornada REFERENCES dbo.CatSatTipoJornada(TipoJornadaId),
    PeriodicidadPagoId     INT NOT NULL CONSTRAINT DF_DocContrato_Periodicidad DEFAULT 4 CONSTRAINT FK_DocContrato_Periodicidad REFERENCES dbo.CatSatPeriodicidadPago(PeriodicidadPagoId),
    TipoRegimenId          INT NOT NULL CONSTRAINT DF_DocContrato_Regimen DEFAULT 2 CONSTRAINT FK_DocContrato_TipoRegimen REFERENCES dbo.CatSatTipoRegimen(TipoRegimenId),
    SalarioBaseCotizacion  DECIMAL(18,4) NULL,
    SalarioDiarioIntegrado DECIMAL(18,4) NULL,
    ZonaSMG                NVARCHAR(10) NOT NULL CONSTRAINT DF_DocContrato_Zona DEFAULT 'GENERAL',
    DiasLaboradosSemana    TINYINT NULL,
    FechaInicio            DATE NOT NULL,
    FechaFin               DATE NULL,
    Activo                 BIT NOT NULL CONSTRAINT DF_DocContrato_Activo DEFAULT 1,
    CONSTRAINT CK_DocContrato_Zona CHECK (ZonaSMG IN ('GENERAL','ZFN')),
    CONSTRAINT CK_DocContrato_DiasSemana CHECK (DiasLaboradosSemana IS NULL OR DiasLaboradosSemana BETWEEN 1 AND 7),
    CONSTRAINT CK_DocContrato_Fechas CHECK (FechaFin IS NULL OR FechaFin >= FechaInicio)
);
GO

-- DocReciboNomina (Cabecera de recibo de nómina calculado)
IF OBJECT_ID('dbo.DocReciboNomina', 'U') IS NULL
CREATE TABLE dbo.DocReciboNomina (
    ReciboId                      INT IDENTITY(1,1) PRIMARY KEY,
    PeriodoId                     INT NOT NULL CONSTRAINT FK_DocRecibo_Periodo REFERENCES dbo.ProcPeriodoNomina(PeriodoId),
    EmpleadoId                    INT NOT NULL CONSTRAINT FK_DocRecibo_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    NumFolioRecibo                NVARCHAR(20) NULL,
    TipoNomina                    NVARCHAR(1) NULL CONSTRAINT DF_DocRecibo_TipoNomina DEFAULT 'O' CONSTRAINT FK_DocRecibo_TipoNomina REFERENCES dbo.CatSatTipoNomina(Clave),
    TotalPercepciones             DECIMAL(18,2) NOT NULL,
    TotalDeducciones              DECIMAL(18,2) NOT NULL,
    NetoPagar                     DECIMAL(18,2) NOT NULL,
    TotalGravado                  DECIMAL(18,4) NULL,
    TotalExento                   DECIMAL(18,4) NULL,
    AntiguedadSemanas             INT NULL,
    SnapshotFechaInicialPago      DATE NULL,
    SnapshotFechaFinalPago        DATE NULL,
    SnapshotFechaPago             DATE NULL,
    SnapshotDiasPagados           DECIMAL(10,4) NULL,
    SnapshotTipoJornada           NVARCHAR(2) NULL,
    SnapshotPeriodicidadPago      NVARCHAR(2) NULL,
    SnapshotTipoRegimen           NVARCHAR(2) NULL,
    SnapshotTipoContrato          NVARCHAR(2) NULL,
    SnapshotRegimenFiscalReceptor NVARCHAR(3) NULL,
    SnapshotBanco                 NVARCHAR(3) NULL,
    SnapshotCuentaBancaria        NVARCHAR(18) NULL,
    TotalSueldos                  DECIMAL(18,4) NULL,
    TotalSeparacionIndemnizacion  DECIMAL(18,4) NULL,
    TotalJubilacionPensionRetiro  DECIMAL(18,4) NULL,
    TotalOtrosPagos               DECIMAL(18,4) NOT NULL CONSTRAINT DF_DocRecibo_OtrosPagos DEFAULT 0,
    SnapshotSalarioDiario         DECIMAL(18,4) NULL,
    SnapshotSDI                   DECIMAL(18,4) NULL,
    SnapshotSBC                   DECIMAL(18,4) NULL,
    SnapshotPuesto                NVARCHAR(100) NULL,
    SnapshotDepartamento          NVARCHAR(100) NULL,
    SnapshotTipoContratoDesc      NVARCHAR(500) NULL,
    SnapshotRegimenFiscalDesc     NVARCHAR(500) NULL,
    FechaCalculo                  DATETIME NOT NULL CONSTRAINT DF_DocRecibo_FechaCalc DEFAULT GETDATE(),
    Estatus                       NVARCHAR(20) NOT NULL CONSTRAINT DF_DocRecibo_Estatus DEFAULT 'CALCULADO',
    Notas                         NVARCHAR(500) NULL,
    CONSTRAINT UQ_DocReciboNomina UNIQUE (PeriodoId, EmpleadoId),
    CONSTRAINT CK_DocRecibo_Estatus CHECK (Estatus IN ('PRERECIBO','CALCULADO','AUTORIZADO','TIMBRADO','CANCELADO')),
    CONSTRAINT CK_DocRecibo_FechasPago CHECK (SnapshotFechaInicialPago IS NULL OR SnapshotFechaInicialPago <= SnapshotFechaFinalPago)
);
GO

-- DocReciboDetalle (Detalle analítico de percepciones, deducciones y otros pagos)
IF OBJECT_ID('dbo.DocReciboDetalle', 'U') IS NULL
CREATE TABLE dbo.DocReciboDetalle (
    DetalleId        INT IDENTITY(1,1) PRIMARY KEY,
    ReciboId         INT NOT NULL CONSTRAINT FK_DocReciboDetalle_Recibo REFERENCES dbo.DocReciboNomina(ReciboId) ON DELETE CASCADE,
    TipoConcepto     CHAR(1) NOT NULL,
    ClaveConceptoSAT NVARCHAR(20) NOT NULL,
    Descripcion      NVARCHAR(100) NOT NULL,
    ImporteGravado   DECIMAL(18,2) NOT NULL CONSTRAINT DF_DocReciboDetalle_Gravado DEFAULT 0,
    ImporteExento    DECIMAL(18,2) NOT NULL CONSTRAINT DF_DocReciboDetalle_Exento DEFAULT 0,
    Cantidad         DECIMAL(18,4) NULL,
    EsIncidencia     BIT NOT NULL CONSTRAINT DF_DocReciboDetalle_EsIncidencia DEFAULT 0,
    CONSTRAINT CK_DocReciboDetalle_Tipo CHECK (TipoConcepto IN ('P','D','O')),
    CONSTRAINT FK_DocReciboDetalle_Concepto FOREIGN KEY (ClaveConceptoSAT, TipoConcepto) REFERENCES dbo.CatConcepto(ClaveSAT, TipoConcepto)
);
GO

-- DocCFDINomina (Comprobante fiscal digital por internet CFDI 4.0 con complemento de nómina 1.2)
IF OBJECT_ID('dbo.DocCFDINomina', 'U') IS NULL
CREATE TABLE dbo.DocCFDINomina (
    CFDIId                INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId             INT NOT NULL CONSTRAINT FK_DocCFDI_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    ReciboId              INT NOT NULL CONSTRAINT FK_DocCFDI_Recibo REFERENCES dbo.DocReciboNomina(ReciboId),
    Version               NVARCHAR(3)   NOT NULL CONSTRAINT DF_DocCFDI_Version DEFAULT '4.0',
    Serie                 NVARCHAR(25)  NULL,
    Folio                 NVARCHAR(40)  NULL,
    FormaPago             NVARCHAR(3) NULL,
    MetodoPago            NVARCHAR(3) NOT NULL CONSTRAINT DF_DocCFDI_Metodo DEFAULT 'PUE' CONSTRAINT FK_DocCFDI_Metodo REFERENCES dbo.CatSatMetodoPago(Clave),
    TipoDeComprobante     NVARCHAR(1) NOT NULL CONSTRAINT DF_DocCFDI_Tipo DEFAULT 'N' CONSTRAINT FK_DocCFDI_Tipo REFERENCES dbo.CatSatTipoDeComprobante(Clave),
    UsoCFDI               NVARCHAR(4) NOT NULL CONSTRAINT DF_DocCFDI_Uso DEFAULT 'CN01' CONSTRAINT FK_DocCFDI_Uso REFERENCES dbo.CatSatUsoCFDI(Clave),
    VersionNomina         NVARCHAR(3) NOT NULL CONSTRAINT DF_DocCFDI_Nomina DEFAULT '1.2',
    Moneda                NVARCHAR(3)   NOT NULL CONSTRAINT DF_DocCFDI_Moneda DEFAULT 'MXN',
    TipoCambio            DECIMAL(10,6) NULL,
    Exportacion           NVARCHAR(2)   NOT NULL CONSTRAINT DF_DocCFDI_Exportacion DEFAULT '01',
    FechaEmision          DATETIME      NULL,
    LugarExpedicionSnap   NVARCHAR(5)   NULL,
    SubsidioCausado       DECIMAL(18,4) NULL,
    UUID                  UNIQUEIDENTIFIER NULL,
    SelloDigital          NVARCHAR(MAX) NULL,
    SelloSAT              NVARCHAR(MAX) NULL,
    CadenaOriginal        NVARCHAR(MAX) NULL,
    CadenaOriginalSAT     NVARCHAR(MAX) NULL,
    NoCertificadoEmisor   NVARCHAR(20)  NULL,
    NoCertificadoSAT      NVARCHAR(20)  NULL,
    RfcProvCertif         NVARCHAR(13)  NULL,
    FechaTimbrado         DATETIME      NULL,
    XMLTimbrado           NVARCHAR(MAX) NULL,
    RutaPDF               NVARCHAR(500) NULL,
    EstadoCFDI            INT           NOT NULL CONSTRAINT DF_DocCFDI_Estado DEFAULT 1 CONSTRAINT FK_DocCFDI_Estado REFERENCES dbo.CatEstadoCFDI(EstadoCFDIId),
    FechaCancelacion      DATETIME      NULL,
    MotivoCancelacion     INT           NULL CONSTRAINT FK_DocCFDI_MotivoCanc REFERENCES dbo.CatSatMotivoCancelacion(MotivoCancelacionId),
    UUIDSustitucion       UNIQUEIDENTIFIER NULL,
    AcuseCancelacionXML   NVARCHAR(MAX) NULL,
    PACCodigoError        NVARCHAR(10)  NULL,
    PACMensaje            NVARCHAR(500) NULL,
    IntentosTimbrado      SMALLINT      NOT NULL CONSTRAINT DF_DocCFDI_IntentosTimb DEFAULT 0,
    IntentosCancelacion   SMALLINT      NOT NULL CONSTRAINT DF_DocCFDI_IntentosCanc DEFAULT 0,
    UltimoIntentoTimbrado DATETIME      NULL,
    ProximoReintento      DATETIME      NULL,
    BloquearReintentos    BIT           NOT NULL CONSTRAINT DF_DocCFDI_Bloquear DEFAULT 0,
    MotivoBloqueo         NVARCHAR(500) NULL,
    SyncNubeEstatus       NVARCHAR(20)  NOT NULL CONSTRAINT DF_DocCFDI_SyncEstatus DEFAULT 'PENDIENTE',
    SyncNubeFecha         DATETIME      NULL,
    CONSTRAINT CK_DocCFDI_Moneda CHECK (Moneda = 'MXN'),
    CONSTRAINT CK_DocCFDI_TipoCambio CHECK (TipoCambio IS NULL),
    CONSTRAINT CK_DocCFDI_Exportacion CHECK (Exportacion = '01'),
    CONSTRAINT CK_DocCFDI_Version CHECK (Version = '4.0' AND VersionNomina = '1.2'),
    CONSTRAINT CK_DocCFDI_Nomina CHECK (FormaPago IS NULL AND MetodoPago = 'PUE' AND TipoDeComprobante = 'N' AND UsoCFDI = 'CN01'),
    CONSTRAINT FK_DocCFDI_Moneda FOREIGN KEY (Moneda) REFERENCES dbo.CatSatMoneda(Clave),
    CONSTRAINT FK_DocCFDI_Exportacion FOREIGN KEY (Exportacion) REFERENCES dbo.CatSatExportacion(Clave),
    CONSTRAINT CK_DocCFDI_LugarExp CHECK (LugarExpedicionSnap IS NULL OR (LEN(LugarExpedicionSnap)=5 AND LugarExpedicionSnap NOT LIKE '%[^0-9]%')),
    CONSTRAINT CK_DocCFDI_RfcPAC CHECK (RfcProvCertif IS NULL OR LEN(RfcProvCertif) IN (12,13))
);
GO

-- Estructuras documentales de nodos condicionales del complemento Nomina 1.2.
CREATE TABLE dbo.DocReciboHorasExtra (
    HorasExtraId INT IDENTITY PRIMARY KEY,
    DetalleId INT NOT NULL REFERENCES dbo.DocReciboDetalle(DetalleId),
    Dias INT NOT NULL,
    TipoHoras NVARCHAR(2) NOT NULL REFERENCES dbo.CatSatTipoHoras(Clave),
    HorasExtra INT NOT NULL,
    ImportePagado DECIMAL(18,2) NOT NULL,
    CONSTRAINT CK_DocReciboHorasExtra_Valores CHECK (Dias > 0 AND HorasExtra > 0 AND ImportePagado >= 0)
);
CREATE INDEX IX_DocReciboHorasExtra_Detalle ON dbo.DocReciboHorasExtra(DetalleId);
CREATE TABLE dbo.DocReciboIncapacidad (
    IncapacidadReciboId INT IDENTITY PRIMARY KEY,
    ReciboId INT NOT NULL REFERENCES dbo.DocReciboNomina(ReciboId),
    DiasIncapacidad INT NOT NULL CHECK (DiasIncapacidad > 0),
    TipoIncapacidad NVARCHAR(2) NOT NULL REFERENCES dbo.CatSatTipoIncapacidad(Clave),
    ImporteMonetario DECIMAL(18,2) NULL CHECK (ImporteMonetario >= 0)
);
CREATE INDEX IX_DocReciboIncapacidad_Recibo ON dbo.DocReciboIncapacidad(ReciboId);
CREATE TABLE dbo.DocReciboOtroPago (
    DetalleId INT NOT NULL PRIMARY KEY REFERENCES dbo.DocReciboDetalle(DetalleId),
    SubsidioCausado DECIMAL(18,2) NULL CHECK (SubsidioCausado >= 0),
    SaldoAFavor DECIMAL(18,2) NULL CHECK (SaldoAFavor > 0),
    Anio SMALLINT NULL CHECK (Anio BETWEEN 1900 AND 9999),
    RemanenteSalFav DECIMAL(18,2) NULL CHECK (RemanenteSalFav >= 0)
);
CREATE TABLE dbo.DocReciboAccionesTitulos (
    DetalleId INT NOT NULL PRIMARY KEY REFERENCES dbo.DocReciboDetalle(DetalleId),
    ValorMercado DECIMAL(24,6) NOT NULL CHECK (ValorMercado > 0),
    PrecioAlOtorgarse DECIMAL(24,6) NOT NULL CHECK (PrecioAlOtorgarse > 0)
);
CREATE TABLE dbo.DocReciboSeparacionIndemnizacion (
    ReciboId INT NOT NULL PRIMARY KEY REFERENCES dbo.DocReciboNomina(ReciboId),
    TotalPagado DECIMAL(18,2) NOT NULL CHECK (TotalPagado >= 0),
    NumAniosServicio INT NOT NULL CHECK (NumAniosServicio >= 0),
    UltimoSueldoMensOrd DECIMAL(18,2) NOT NULL CHECK (UltimoSueldoMensOrd >= 0),
    IngresoAcumulable DECIMAL(18,2) NOT NULL CHECK (IngresoAcumulable >= 0),
    IngresoNoAcumulable DECIMAL(18,2) NOT NULL CHECK (IngresoNoAcumulable >= 0)
);
CREATE TABLE dbo.DocReciboJubilacionPensionRetiro (
    ReciboId INT NOT NULL PRIMARY KEY REFERENCES dbo.DocReciboNomina(ReciboId),
    TotalUnaExhibicion DECIMAL(18,2) NULL CHECK (TotalUnaExhibicion >= 0),
    TotalParcialidad DECIMAL(18,2) NULL CHECK (TotalParcialidad >= 0),
    MontoDiario DECIMAL(18,2) NULL CHECK (MontoDiario >= 0),
    IngresoAcumulable DECIMAL(18,2) NOT NULL CHECK (IngresoAcumulable >= 0),
    IngresoNoAcumulable DECIMAL(18,2) NOT NULL CHECK (IngresoNoAcumulable >= 0),
    CONSTRAINT CK_DocReciboJubilacion_Forma CHECK (
        (TotalUnaExhibicion IS NOT NULL AND TotalParcialidad IS NULL AND MontoDiario IS NULL)
        OR (TotalUnaExhibicion IS NULL AND TotalParcialidad IS NOT NULL AND MontoDiario IS NOT NULL))
);
CREATE TABLE dbo.RelReciboSubContratacion (
    ReciboId INT NOT NULL REFERENCES dbo.DocReciboNomina(ReciboId),
    RfcLabora NVARCHAR(13) NOT NULL,
    PorcentajeTiempo DECIMAL(6,3) NOT NULL CHECK (PorcentajeTiempo BETWEEN 0.001 AND 100),
    CONSTRAINT PK_RelReciboSubContratacion PRIMARY KEY (ReciboId,RfcLabora)
);
CREATE TABLE dbo.RelCFDIRelacionado (
    CFDIId INT NOT NULL REFERENCES dbo.DocCFDINomina(CFDIId),
    TipoRelacion NVARCHAR(2) NOT NULL REFERENCES dbo.CatSatTipoRelacion(Clave),
    UUIDRelacionado UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_RelCFDIRelacionado PRIMARY KEY (CFDIId,TipoRelacion,UUIDRelacionado)
);
CREATE TABLE dbo.DocCFDIConcepto (
    CFDIId INT NOT NULL PRIMARY KEY REFERENCES dbo.DocCFDINomina(CFDIId),
    ClaveProdServ NVARCHAR(8) NOT NULL DEFAULT N'84111505' REFERENCES dbo.CatSatClaveProdServ(Clave),
    Cantidad DECIMAL(18,6) NOT NULL DEFAULT 1 CHECK (Cantidad = 1),
    ClaveUnidad NVARCHAR(3) NOT NULL DEFAULT N'ACT' REFERENCES dbo.CatSatClaveUnidad(Clave),
    Descripcion NVARCHAR(100) NOT NULL DEFAULT N'Pago de nomina',
    ValorUnitario DECIMAL(18,2) NOT NULL,
    Importe DECIMAL(18,2) NOT NULL,
    Descuento DECIMAL(18,2) NOT NULL,
    ObjetoImp NVARCHAR(2) NOT NULL DEFAULT N'01' REFERENCES dbo.CatSatObjetoImp(Clave),
    CONSTRAINT CK_DocCFDIConcepto_Nomina CHECK (ClaveProdServ = N'84111505' AND ClaveUnidad = N'ACT'
        AND ObjetoImp = N'01' AND Importe >= 0 AND ValorUnitario = Importe AND Descuento BETWEEN 0 AND Importe)
);
GO

-- LogCFDIEvento (Bitácora de eventos del CFDI)
IF OBJECT_ID('dbo.LogCFDIEvento', 'U') IS NULL
CREATE TABLE dbo.LogCFDIEvento (
    EventoId       INT IDENTITY(1,1) PRIMARY KEY,
    CFDIId         INT NOT NULL CONSTRAINT FK_LogCFDIEvento_CFDI REFERENCES dbo.DocCFDINomina(CFDIId),
    FechaEvento    DATETIME NOT NULL CONSTRAINT DF_LogCFDIEvento_Fecha DEFAULT GETDATE(),
    TipoEvento     NVARCHAR(30) NOT NULL,
    EstadoAnterior NVARCHAR(20) NULL,
    EstadoNuevo    NVARCHAR(20) NOT NULL,
    Detalle        NVARCHAR(500) NULL,
    Usuario        NVARCHAR(150) NULL CONSTRAINT DF_LogCFDIEvento_Usuario DEFAULT SYSTEM_USER
);
GO

-- DocEmpleadoBaja (Registro formal de terminación laboral)
IF OBJECT_ID('dbo.DocEmpleadoBaja', 'U') IS NULL
CREATE TABLE dbo.DocEmpleadoBaja (
    BajaId        INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId    INT NOT NULL CONSTRAINT FK_DocBaja_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    FechaBaja     DATE NOT NULL,
    TipoBaja      NVARCHAR(30) NOT NULL,
    Observaciones NVARCHAR(500) NULL,
    FechaRegistro DATETIME NOT NULL CONSTRAINT DF_DocBaja_FechaReg DEFAULT GETDATE(),
    CONSTRAINT CK_DocBaja_Tipo CHECK (TipoBaja IN ('VOLUNTARIA','JUSTIFICADA','INJUSTIFICADA','DEFUNCION','PENSION','RESCISION_PATRON'))
);
GO

-- DocFiniquito (Comprobante y liquidación de finiquito)
IF OBJECT_ID('dbo.DocFiniquito', 'U') IS NULL
CREATE TABLE dbo.DocFiniquito (
    FiniquitoId  INT IDENTITY(1,1) PRIMARY KEY,
    BajaId       INT NOT NULL CONSTRAINT FK_DocFiniquito_Baja REFERENCES dbo.DocEmpleadoBaja(BajaId),
    EmpleadoId   INT NOT NULL CONSTRAINT FK_DocFiniquito_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    FechaCalculo DATETIME NOT NULL CONSTRAINT DF_DocFiniquito_Fecha DEFAULT GETDATE(),
    TotalBruto   DECIMAL(18,4) NOT NULL,
    ISRFiniquito DECIMAL(18,4) NOT NULL CONSTRAINT DF_DocFiniquito_ISR DEFAULT 0,
    TotalNeto    DECIMAL(18,4) NOT NULL,
    Estatus      NVARCHAR(20) NOT NULL CONSTRAINT DF_DocFiniquito_Estatus DEFAULT 'CALCULADO',
    CONSTRAINT CK_DocFiniquito_Estatus CHECK (Estatus IN ('CALCULADO','REVISADO','PAGADO'))
);
GO

-- DocFiniquitoDetalle (Desglose analítico de finiquito/liquidación)
IF OBJECT_ID('dbo.DocFiniquitoDetalle', 'U') IS NULL
CREATE TABLE dbo.DocFiniquitoDetalle (
    DetalleFinId    INT IDENTITY(1,1) PRIMARY KEY,
    FiniquitoId     INT NOT NULL CONSTRAINT FK_DocFinDet_Finiquito REFERENCES dbo.DocFiniquito(FiniquitoId) ON DELETE CASCADE,
    Concepto        NVARCHAR(150) NOT NULL,
    DiasBase        DECIMAL(10,4) NULL,
    SalarioBase     DECIMAL(18,4) NULL,
    ImporteBruto    DECIMAL(18,4) NOT NULL,
    ImporteExento   DECIMAL(18,4) NOT NULL CONSTRAINT DF_DocFinDet_Exento DEFAULT 0,
    ImporteGravado  DECIMAL(18,4) NOT NULL CONSTRAINT DF_DocFinDet_Gravado DEFAULT 0
);
GO

-- ==============================================================================
-- [BLOQUE-06] TABLAS DE RELACIÓN (Rel)
-- ==============================================================================
PRINT '[06/21] Creando tablas de relación muchos a muchos (Prefijo Rel)...';
GO

-- RelRolPermiso (Mapeo de permisos asignados a roles)
IF OBJECT_ID('dbo.RelRolPermiso', 'U') IS NULL
CREATE TABLE dbo.RelRolPermiso (
    RolId     INT NOT NULL CONSTRAINT FK_RelRolPermiso_Rol REFERENCES dbo.CatRol(RolId),
    PermisoId INT NOT NULL CONSTRAINT FK_RelRolPermiso_Permiso REFERENCES dbo.CatPermiso(PermisoId),
    CONSTRAINT PK_RelRolPermiso PRIMARY KEY (RolId, PermisoId)
);
GO

-- RelUsuarioRol (Mapeo de roles asignados a usuarios)
IF OBJECT_ID('dbo.RelUsuarioRol', 'U') IS NULL
CREATE TABLE dbo.RelUsuarioRol (
    UsuarioId INT NOT NULL CONSTRAINT FK_RelUsuarioRol_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    RolId     INT NOT NULL CONSTRAINT FK_RelUsuarioRol_Rol REFERENCES dbo.CatRol(RolId),
    FechaAlta DATETIME NOT NULL CONSTRAINT DF_RelUsuarioRol_FechaAlta DEFAULT GETDATE(),
    CONSTRAINT PK_RelUsuarioRol PRIMARY KEY (UsuarioId, RolId)
);
GO

-- RelConceptoFormula (Asociación entre conceptos y fórmulas de cálculo)
IF OBJECT_ID('dbo.RelConceptoFormula', 'U') IS NULL
CREATE TABLE dbo.RelConceptoFormula (
    ConceptoFormulaId     INT IDENTITY(1,1) PRIMARY KEY,
    ConceptoId            INT NOT NULL CONSTRAINT FK_RelConcForm_Concepto REFERENCES dbo.CatConcepto(ConceptoId),
    FormulaId             INT NOT NULL CONSTRAINT     FK_RelConcForm_Formula REFERENCES dbo.CatFormulaCalculo(FormulaId),
        PoliticaId            INT NULL CONSTRAINT FK_RelConcForm_Politica REFERENCES dbo.CatPoliticaEmpresarial(PoliticaId),
    ParametrosEspecificos NVARCHAR(MAX) NULL,
    OrdenEjecucion        SMALLINT NOT NULL CONSTRAINT DF_RelConcForm_Orden DEFAULT 1,
    Activa                BIT NOT NULL CONSTRAINT DF_RelConcForm_Activa DEFAULT 1,
    FechaCreacion         DATETIME NOT NULL CONSTRAINT DF_RelConcForm_FechaCreacion DEFAULT GETDATE(),
    CONSTRAINT UQ_RelConceptoFormula UNIQUE (ConceptoId, FormulaId, OrdenEjecucion)
);
GO

-- RelEmpresaPeriodicidad (Periodicidades de pago habilitadas por empresa: relación 1:N Empresa -> Periodicidad)
-- Una empresa puede operar varias periodicidades (p. ej. Semanal y Quincenal). La fila guarda además las reglas
-- con las que se generan los periodos del ejercicio (días de inicio/fin/pago/descanso y cálculo del séptimo día).
IF OBJECT_ID('dbo.RelEmpresaPeriodicidad', 'U') IS NULL
CREATE TABLE dbo.RelEmpresaPeriodicidad (
    EmpresaPeriodicidadId    INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId                INT NOT NULL CONSTRAINT FK_RelEmpPer_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    PeriodicidadPagoId       INT NOT NULL CONSTRAINT FK_RelEmpPer_Periodicidad REFERENCES dbo.CatSatPeriodicidadPago(PeriodicidadPagoId),
    Descripcion              NVARCHAR(100) NULL,
    CalculaSeptimoDia        BIT NOT NULL CONSTRAINT DF_RelETP_SeptimoDia DEFAULT 1,
    DiaInicio                TINYINT NULL,
    DiaFin                   TINYINT NULL,
    DiaPago                  TINYINT NULL,
    DiaDescanso              TINYINT NULL,
    TrabajaDomingo           BIT NOT NULL CONSTRAINT DF_RelETP_TrabDomingo DEFAULT 0,
    PorcentajePrimaDominical DECIMAL(5,2) NOT NULL CONSTRAINT DF_RelETP_PrimaDom DEFAULT 25.00,
    Activo                   BIT NOT NULL CONSTRAINT DF_RelETP_Activo DEFAULT 1,
    FechaAlta                DATETIME NOT NULL CONSTRAINT DF_RelETP_FechaAlta DEFAULT GETDATE(),
    FechaModificacion        DATETIME NULL,
    CONSTRAINT UQ_RelEmpPer_EmpresaPeriodicidad UNIQUE (EmpresaId, PeriodicidadPagoId),
    CONSTRAINT CK_RelETP_DiaInicio CHECK (DiaInicio IS NULL OR DiaInicio BETWEEN 1 AND 7),
    CONSTRAINT CK_RelETP_DiaFin CHECK (DiaFin IS NULL OR DiaFin BETWEEN 1 AND 7),
    CONSTRAINT CK_RelETP_DiaPago CHECK (DiaPago IS NULL OR DiaPago BETWEEN 1 AND 7),
    CONSTRAINT CK_RelETP_DiaDescanso CHECK (DiaDescanso IS NULL OR DiaDescanso BETWEEN 1 AND 7),
    CONSTRAINT CK_RelETP_PrimaDom CHECK (PorcentajePrimaDominical >= 25.00 AND PorcentajePrimaDominical <= 100.00)
);
GO

-- CatDiaFestivo (Días festivos oficiales de ley -LFT Art. 74- y días festivos propios de cada empresa)
-- EmpresaId NULL = día festivo de ley (aplica a todas las empresas); EmpresaId informado = festivo de esa empresa.
IF OBJECT_ID('dbo.CatDiaFestivo', 'U') IS NULL
CREATE TABLE dbo.CatDiaFestivo (
    DiaFestivoId INT IDENTITY(1,1) PRIMARY KEY,
    EmpresaId    INT NULL CONSTRAINT FK_CatDiaFestivo_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    Fecha        DATE NOT NULL,
    Anio         AS YEAR(Fecha) PERSISTED,
    Descripcion  NVARCHAR(100) NOT NULL,
    Tipo         NVARCHAR(10) NOT NULL,
    Activo       BIT NOT NULL CONSTRAINT DF_CatDiaFestivo_Activo DEFAULT 1,
    FechaAlta    DATETIME NOT NULL CONSTRAINT DF_CatDiaFestivo_FechaAlta DEFAULT GETDATE(),
    CONSTRAINT CK_CatDiaFestivo_Tipo CHECK (Tipo IN ('LEY','EMPRESA')),
    CONSTRAINT CK_CatDiaFestivo_Alcance CHECK ((Tipo = 'LEY' AND EmpresaId IS NULL) OR (Tipo = 'EMPRESA' AND EmpresaId IS NOT NULL))
);
GO

-- Un mismo día no puede registrarse dos veces como festivo de ley ni dos veces para la misma empresa
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_CatDiaFestivo_Ley' AND object_id = OBJECT_ID('dbo.CatDiaFestivo'))
    CREATE UNIQUE INDEX UX_CatDiaFestivo_Ley ON dbo.CatDiaFestivo(Fecha) WHERE EmpresaId IS NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_CatDiaFestivo_Empresa' AND object_id = OBJECT_ID('dbo.CatDiaFestivo'))
    CREATE UNIQUE INDEX UX_CatDiaFestivo_Empresa ON dbo.CatDiaFestivo(EmpresaId, Fecha) WHERE EmpresaId IS NOT NULL;
GO

-- RelPeriodoDiaFestivo (Días festivos que caen dentro de cada periodo: FechaFestivo BETWEEN FechaInicio AND FechaFin)
IF OBJECT_ID('dbo.RelPeriodoDiaFestivo', 'U') IS NULL
CREATE TABLE dbo.RelPeriodoDiaFestivo (
    PeriodoId    INT NOT NULL CONSTRAINT FK_RelPeriodoFestivo_Periodo REFERENCES dbo.ProcPeriodoNomina(PeriodoId) ON DELETE CASCADE,
    DiaFestivoId INT NOT NULL CONSTRAINT FK_RelPeriodoFestivo_Festivo REFERENCES dbo.CatDiaFestivo(DiaFestivoId),
    CONSTRAINT PK_RelPeriodoDiaFestivo PRIMARY KEY (PeriodoId, DiaFestivoId)
);
GO

-- ==============================================================================
-- [BLOQUE-06b] EXTENSIONES DEL ESQUEMA DE INSTALACION
-- Campos adicionales del esquema original, incluidos en esta instalacion nueva.
-- ==============================================================================
PRINT '[06b/21] Completando campos de periodicidades, periodos y festivos...';
GO

IF COL_LENGTH('dbo.ProcPeriodoNomina', 'IniciaMes') IS NOT NULL
    EXEC sp_rename 'dbo.ProcPeriodoNomina.IniciaMes', 'EsInicioMes', 'COLUMN';
IF COL_LENGTH('dbo.ProcPeriodoNomina', 'FinMes') IS NOT NULL
    EXEC sp_rename 'dbo.ProcPeriodoNomina.FinMes', 'EsFinMes', 'COLUMN';
IF COL_LENGTH('dbo.ProcPeriodoNomina', 'DiaDescanso') IS NULL
    ALTER TABLE dbo.ProcPeriodoNomina ADD DiaDescanso TINYINT NULL;
IF COL_LENGTH('dbo.ProcPeriodoNomina', 'CalculaSeptimoDia') IS NULL
    ALTER TABLE dbo.ProcPeriodoNomina ADD CalculaSeptimoDia BIT NOT NULL CONSTRAINT DF_ProcPeriodo_Septimo DEFAULT 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_ProcPeriodo_DiaDescanso')
    ALTER TABLE dbo.ProcPeriodoNomina ADD CONSTRAINT CK_ProcPeriodo_DiaDescanso CHECK (DiaDescanso IS NULL OR DiaDescanso BETWEEN 1 AND 7);
GO

-- (Las normalizaciones idempotentes de RelEmpresaPeriodicidad quedaron integradas en su CREATE para base nueva.)
GO

-- Factor adicional por día festivo laborado (LFT Art. 75: salario doble además del que corresponde por el descanso)
IF COL_LENGTH('dbo.CatPoliticaEmpresarial', 'FactorDiaFestivoLaborado') IS NULL
    ALTER TABLE dbo.CatPoliticaEmpresarial ADD FactorDiaFestivoLaborado DECIMAL(5,2) NOT NULL CONSTRAINT DF_CatPol_FacFest DEFAULT 2.00;
GO

-- Pre-recibo de nómina (FrmProcNomina): notas por recibo (se imprimen solo si traen texto), cantidad por concepto
-- y estatus PRERECIBO para el borrador que el usuario guarda antes de cerrar el periodo.
IF COL_LENGTH('dbo.DocReciboNomina', 'Notas') IS NULL
    ALTER TABLE dbo.DocReciboNomina ADD Notas NVARCHAR(500) NULL;
GO

IF COL_LENGTH('dbo.DocReciboDetalle', 'Cantidad') IS NULL
    ALTER TABLE dbo.DocReciboDetalle ADD Cantidad DECIMAL(18,4) NULL;
GO

IF COL_LENGTH('dbo.DocReciboDetalle', 'EsIncidencia') IS NULL
    ALTER TABLE dbo.DocReciboDetalle ADD EsIncidencia BIT NOT NULL CONSTRAINT DF_DocReciboDetalle_EsIncidencia DEFAULT 0;
GO

IF EXISTS (SELECT 1 FROM sys.check_constraints
           WHERE name = 'CK_DocRecibo_Estatus' AND parent_object_id = OBJECT_ID('dbo.DocReciboNomina')
             AND definition NOT LIKE '%PRERECIBO%')
    ALTER TABLE dbo.DocReciboNomina DROP CONSTRAINT CK_DocRecibo_Estatus;
GO
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints
               WHERE name = 'CK_DocRecibo_Estatus' AND parent_object_id = OBJECT_ID('dbo.DocReciboNomina'))
    ALTER TABLE dbo.DocReciboNomina ADD CONSTRAINT CK_DocRecibo_Estatus
        CHECK (Estatus IN ('PRERECIBO','CALCULADO','AUTORIZADO','TIMBRADO','CANCELADO'));
GO

-- ==============================================================================
-- Incidencias por concepto. Única fuente de conceptos: dbo.CatConcepto, tanto los conceptos SAT de ingresos y deducciones
-- (P/D) como las incidencias legales del motor de nómina (TipoConcepto 'I': faltas, retardos, permisos, horas extra, prima
-- dominical, días festivos, bonos y descuentos).
--   • CatPaqueteIncidencia / RelPaqueteConcepto: paquete por defecto por periodicidad que se asigna al alta del empleado.
--   • RelEmpleadoIncidencia: conceptos permanentes asignados individualmente a cada trabajador.
--   • ProcIncidencias: movimientos del periodo (EsPermanente = se sincroniza con RelEmpleadoIncidencia). Las incidencias
--     legales llevan FechaIncidencia, se cancelan (Estatus) en lugar de borrarse y su importe lo calcula sp_CalcularNomina.
-- ==============================================================================
IF COL_LENGTH('dbo.ProcPeriodoNomina','IncidenciasInicializadas') IS NULL
    ALTER TABLE dbo.ProcPeriodoNomina ADD IncidenciasInicializadas BIT NOT NULL
        CONSTRAINT DF_ProcPeriodoNomina_IncidenciasInicializadas DEFAULT 0;
GO

CREATE TABLE dbo.CatPaqueteIncidencia
(
    IdPaquete     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CatPaqueteIncidencia PRIMARY KEY,
    Nombre        NVARCHAR(100) NOT NULL,
    PeriodicidadPagoId INT NULL CONSTRAINT FK_CatPaqueteIncidencia_Periodicidad REFERENCES dbo.CatSatPeriodicidadPago(PeriodicidadPagoId),
    EmpresaId     INT NULL CONSTRAINT FK_CatPaqueteIncidencia_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    EsDefault     BIT NOT NULL CONSTRAINT DF_CatPaqueteIncidencia_EsDefault DEFAULT 0,
    Activo        BIT NOT NULL CONSTRAINT DF_CatPaqueteIncidencia_Activo DEFAULT 1
);
GO

CREATE TABLE dbo.RelPaqueteConcepto
(
    IdPaqueteConcepto INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RelPaqueteConcepto PRIMARY KEY,
    IdPaquete      INT NOT NULL CONSTRAINT FK_RelPaqueteConcepto_Paquete REFERENCES dbo.CatPaqueteIncidencia(IdPaquete),
    IdConceptoSAT  INT NOT NULL CONSTRAINT FK_RelPaqueteConcepto_Concepto REFERENCES dbo.CatConcepto(ConceptoId),
    CantidadOValor DECIMAL(18,4) NOT NULL CONSTRAINT CK_RelPaqueteConcepto_Cantidad CHECK (CantidadOValor >= 0),
    EsPermanente   BIT NOT NULL CONSTRAINT DF_RelPaqueteConcepto_EsPermanente DEFAULT 1,
    Activo         BIT NOT NULL CONSTRAINT DF_RelPaqueteConcepto_Activo DEFAULT 1,
    CONSTRAINT UQ_RelPaqueteConcepto UNIQUE (IdPaquete, IdConceptoSAT)
);
GO

CREATE TABLE dbo.RelEmpleadoIncidencia
(
    IdEmpleadoIncidencia INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RelEmpleadoIncidencia PRIMARY KEY,
    IdEmpleado     INT NOT NULL CONSTRAINT FK_RelEmpleadoIncidencia_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    IdConceptoSAT  INT NOT NULL CONSTRAINT FK_RelEmpleadoIncidencia_Concepto REFERENCES dbo.CatConcepto(ConceptoId),
    CantidadOValor DECIMAL(18,4) NOT NULL CONSTRAINT CK_RelEmpleadoIncidencia_Cantidad CHECK (CantidadOValor >= 0),
    EsPermanente   BIT NOT NULL CONSTRAINT DF_RelEmpleadoIncidencia_EsPermanente DEFAULT 1,
    Observaciones  NVARCHAR(250) NULL,
    Activo         BIT NOT NULL CONSTRAINT DF_RelEmpleadoIncidencia_Activo DEFAULT 1,
    CONSTRAINT UQ_RelEmpleadoIncidencia UNIQUE (IdEmpleado, IdConceptoSAT)
);
GO

CREATE TABLE dbo.ProcIncidencias
(
    IdIncidencia   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProcIncidencias PRIMARY KEY,
    IdPeriodo      INT NOT NULL CONSTRAINT FK_ProcIncidencias_Periodo REFERENCES dbo.ProcPeriodoNomina(PeriodoId),
    IdEmpleado     INT NOT NULL CONSTRAINT FK_ProcIncidencias_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    IdConceptoSAT  INT NOT NULL CONSTRAINT FK_ProcIncidencias_Concepto REFERENCES dbo.CatConcepto(ConceptoId),
    CantidadOValor DECIMAL(18,4) NOT NULL CONSTRAINT CK_ProcIncidencias_Cantidad CHECK (CantidadOValor > 0),
    ImporteCalculado DECIMAL(18,2) NOT NULL,
    EsPermanente   BIT NOT NULL CONSTRAINT DF_ProcIncidencias_EsPermanente DEFAULT 0,
    Observaciones  NVARCHAR(300) NULL,
    FechaIncidencia DATE NULL,
    Estatus        NVARCHAR(20) NOT NULL CONSTRAINT DF_ProcIncidencias_Estatus DEFAULT 'ACTIVA',
    CONSTRAINT CK_ProcIncidencias_Estatus CHECK (Estatus IN ('ACTIVA','CANCELADA','APLICADA')),
    -- El importe de un concepto SAT es positivo; el de una incidencia legal (con fecha) lo calcula el motor de nómina.
    CONSTRAINT CK_ProcIncidencias_Importe CHECK (ImporteCalculado > 0 OR (ImporteCalculado = 0 AND FechaIncidencia IS NOT NULL))
);
GO
CREATE INDEX IX_ProcIncidencias_PeriodoEmpleado ON dbo.ProcIncidencias(IdPeriodo,IdEmpleado);
CREATE INDEX IX_RelEmpleadoIncidencia_Empleado  ON dbo.RelEmpleadoIncidencia(IdEmpleado,Activo);
GO

-- Días laborados por semana (no es una incidencia). Orden de precedencia:
--   1) ProcDiasSemanaPeriodo: ajuste solo de un periodo (7 = jornada completa en ese periodo),
--   2) DocContratoEmpleado.DiasLaboradosSemana: valor fijo del empleado (NULL = según la jornada),
--   3) CatReglaTipoJornada.DiasSemana: días por semana de su tipo de jornada (NULL = jornada completa).
IF COL_LENGTH('dbo.DocContratoEmpleado', 'DiasLaboradosSemana') IS NULL
    ALTER TABLE dbo.DocContratoEmpleado ADD DiasLaboradosSemana TINYINT NULL
        CONSTRAINT CK_DocContrato_DiasSemana CHECK (DiasLaboradosSemana IS NULL OR DiasLaboradosSemana BETWEEN 1 AND 7);
GO

IF COL_LENGTH('dbo.CatReglaTipoJornada', 'DiasSemana') IS NULL
    ALTER TABLE dbo.CatReglaTipoJornada ADD DiasSemana TINYINT NULL
        CONSTRAINT CK_CatReglaTipoJornada_DiasSemana CHECK (DiasSemana IS NULL OR DiasSemana BETWEEN 1 AND 7);
GO

IF OBJECT_ID('dbo.ProcDiasSemanaPeriodo', 'U') IS NULL
CREATE TABLE dbo.ProcDiasSemanaPeriodo
(
    PeriodoId   INT NOT NULL CONSTRAINT FK_ProcDiasSemanaPeriodo_Periodo REFERENCES dbo.ProcPeriodoNomina(PeriodoId),
    EmpleadoId  INT NOT NULL CONSTRAINT FK_ProcDiasSemanaPeriodo_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    DiasSemana  TINYINT NOT NULL CONSTRAINT CK_ProcDiasSemanaPeriodo_Dias CHECK (DiasSemana BETWEEN 1 AND 7),
    CONSTRAINT PK_ProcDiasSemanaPeriodo PRIMARY KEY (PeriodoId, EmpleadoId)
);
GO

-- Días por semana vigentes del empleado en el periodo; NULL = jornada completa
CREATE OR ALTER FUNCTION dbo.fn_ObtenerDiasSemanaEmpleado (@EmpleadoId INT, @PeriodoId INT)
RETURNS TINYINT
AS
BEGIN
    RETURN COALESCE(
        (SELECT p.DiasSemana FROM dbo.ProcDiasSemanaPeriodo p WHERE p.PeriodoId = @PeriodoId AND p.EmpleadoId = @EmpleadoId),
        (SELECT TOP (1) COALESCE(c.DiasLaboradosSemana, r.DiasSemana)
           FROM dbo.DocContratoEmpleado c
           LEFT JOIN dbo.CatReglaTipoJornada r ON r.TipoJornadaId = c.TipoJornadaId
          WHERE c.EmpleadoId = @EmpleadoId AND c.Activo = 1 ORDER BY c.FechaInicio DESC));
END;
GO

-- ==============================================================================
-- MOTOR DE FÓRMULAS DE CÁLCULO (parámetros con alcance empresa/periodo/empleado)
-- ==============================================================================

-- Valores de parámetros de cálculo con alcance (empleado > periodo > empresa > global).
IF OBJECT_ID('dbo.RelParametroCalculo','U') IS NULL
CREATE TABLE dbo.RelParametroCalculo (
    ParametroId INT IDENTITY(1,1) CONSTRAINT PK_RelParametroCalculo PRIMARY KEY,
    Nombre      NVARCHAR(60)  NOT NULL,
    Valor       DECIMAL(18,6) NOT NULL,
    EmpresaId   INT NULL CONSTRAINT FK_RelParametroCalculo_Empresa  REFERENCES dbo.CatEmpresa(EmpresaId),
    PeriodoId   INT NULL CONSTRAINT FK_RelParametroCalculo_Periodo  REFERENCES dbo.ProcPeriodoNomina(PeriodoId),
    EmpleadoId  INT NULL CONSTRAINT FK_RelParametroCalculo_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    Activo      BIT NOT NULL CONSTRAINT DF_RelParametroCalculo_Activo DEFAULT 1,
    CONSTRAINT CK_RelParametroCalculo_Nombre CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0)
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RelParametroCalculo_Resolucion')
    CREATE INDEX IX_RelParametroCalculo_Resolucion
        ON dbo.RelParametroCalculo(Nombre, Activo, EmpresaId, PeriodoId, EmpleadoId) INCLUDE (Valor);
GO

-- Valor del parámetro más específico aplicable al empleado/periodo/empresa; NULL si no hay ninguno.
CREATE OR ALTER FUNCTION dbo.fn_ResolverParametro
    (@Nombre NVARCHAR(60), @EmpresaId INT, @PeriodoId INT, @EmpleadoId INT)
RETURNS DECIMAL(18,6)
AS
BEGIN
    RETURN (
        SELECT TOP (1) Valor
        FROM dbo.RelParametroCalculo
        WHERE Nombre = @Nombre AND Activo = 1
          AND (EmpleadoId = @EmpleadoId OR EmpleadoId IS NULL)
          AND (PeriodoId  = @PeriodoId  OR PeriodoId  IS NULL)
          AND (EmpresaId  = @EmpresaId  OR EmpresaId  IS NULL)
        ORDER BY CASE WHEN EmpleadoId IS NOT NULL THEN 0 ELSE 1 END,
                 CASE WHEN PeriodoId  IS NOT NULL THEN 0 ELSE 1 END,
                 CASE WHEN EmpresaId  IS NOT NULL THEN 0 ELSE 1 END);
END;
GO

-- Evaluador seguro de una expresión de CatFormulaCalculo. Las variables viajan como parámetros tipados de
-- sp_executesql (nunca se concatenan valores), y la expresión solo admite variables, números y operadores aritméticos.
CREATE OR ALTER PROCEDURE dbo.sp_EvaluarFormula
    @Expresion    NVARCHAR(1000),
    @SD           DECIMAL(18,4) = 0,
    @SDI          DECIMAL(18,4) = 0,
    @SBC          DECIMAL(18,4) = 0,
    @DIAS         DECIMAL(18,4) = 0,
    @HORAS        DECIMAL(18,4) = 0,
    @UMA          DECIMAL(18,4) = 0,
    @SMG          DECIMAL(18,4) = 0,
    @SALARIO_HORA DECIMAL(18,4) = 0,
    @PORCENTAJE   DECIMAL(18,6) = 0,
    @FACTOR       DECIMAL(18,6) = 0,
    @TOPE_UMA     DECIMAL(18,6) = 0,
    @Resultado    DECIMAL(18,4) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @Expresion IS NULL OR LEN(LTRIM(RTRIM(@Expresion))) = 0
        THROW 51050, 'La expresión de la fórmula está vacía.', 1;
    IF @Expresion LIKE '%[^A-Za-z0-9_@. +()/*-]%' COLLATE Latin1_General_100_BIN2
        THROW 51051, 'La expresión de la fórmula contiene caracteres no permitidos.', 1;

    DECLARE @sql NVARCHAR(MAX) = N'SET @Resultado = (' + @Expresion + N');';
    EXEC sys.sp_executesql @sql,
        N'@SD DECIMAL(18,4), @SDI DECIMAL(18,4), @SBC DECIMAL(18,4), @DIAS DECIMAL(18,4), @HORAS DECIMAL(18,4),
          @UMA DECIMAL(18,4), @SMG DECIMAL(18,4), @SALARIO_HORA DECIMAL(18,4), @PORCENTAJE DECIMAL(18,6),
          @FACTOR DECIMAL(18,6), @TOPE_UMA DECIMAL(18,6), @Resultado DECIMAL(18,4) OUTPUT',
        @SD=@SD, @SDI=@SDI, @SBC=@SBC, @DIAS=@DIAS, @HORAS=@HORAS, @UMA=@UMA, @SMG=@SMG,
        @SALARIO_HORA=@SALARIO_HORA, @PORCENTAJE=@PORCENTAJE, @FACTOR=@FACTOR, @TOPE_UMA=@TOPE_UMA,
        @Resultado=@Resultado OUTPUT;

    SET @Resultado = ROUND(ISNULL(@Resultado,0), 4);
END;
GO

-- Paquetes de incidencias por empresa y periodo (FrmAsignacionPaquetes). La llave compuesta (PeriodoId, EmpresaId) garantiza
-- que el periodo pertenece a la empresa. Los conceptos de cada paquete asignado se aplican a los trabajadores del periodo y
-- ProcIncidencias.IdPaquete conserva su origen para retirarlos si el paquete se quita del periodo.
IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_ProcPeriodo_PeriodoEmpresa')
    ALTER TABLE dbo.ProcPeriodoNomina ADD CONSTRAINT UQ_ProcPeriodo_PeriodoEmpresa UNIQUE (PeriodoId, EmpresaId);
GO

IF OBJECT_ID('dbo.RelEmpresaPeriodoPaquete', 'U') IS NULL
CREATE TABLE dbo.RelEmpresaPeriodoPaquete
(
    EmpresaId         INT NOT NULL CONSTRAINT FK_RelEmpresaPeriodoPaquete_Empresa REFERENCES dbo.CatEmpresa(EmpresaId),
    PeriodoId         INT NOT NULL,
    IdPaquete         INT NOT NULL CONSTRAINT FK_RelEmpresaPeriodoPaquete_Paquete REFERENCES dbo.CatPaqueteIncidencia(IdPaquete),
    FechaAsignacion   DATETIME NOT NULL CONSTRAINT DF_RelEmpresaPeriodoPaquete_Fecha DEFAULT GETDATE(),
    UsuarioAsignacion NVARCHAR(150) NULL CONSTRAINT DF_RelEmpresaPeriodoPaquete_Usuario DEFAULT SYSTEM_USER,
    CONSTRAINT PK_RelEmpresaPeriodoPaquete PRIMARY KEY (EmpresaId, PeriodoId, IdPaquete),
    CONSTRAINT FK_RelEmpresaPeriodoPaquete_Periodo FOREIGN KEY (PeriodoId, EmpresaId)
        REFERENCES dbo.ProcPeriodoNomina(PeriodoId, EmpresaId)
);
GO

IF COL_LENGTH('dbo.ProcIncidencias', 'IdPaquete') IS NULL
    ALTER TABLE dbo.ProcIncidencias ADD IdPaquete INT NULL
        CONSTRAINT FK_ProcIncidencias_Paquete REFERENCES dbo.CatPaqueteIncidencia(IdPaquete);
GO

-- Conceptos que una empresa omite en un periodo exacto, incluso los legales (base: sueldo, IMSS, ISR y subsidio, e incidencias
-- legales). BloqueaTimbrado es la bandera de un concepto legal omitido: el recibo se calcula y el periodo puede cerrarse, pero
-- sp_ValidarReciboTimbrado impide timbrarlo porque no cumple los requisitos del CFDI de nómina.
IF OBJECT_ID('dbo.RelEmpresaPeriodoConceptoOmitido', 'U') IS NULL
CREATE TABLE dbo.RelEmpresaPeriodoConceptoOmitido
(
    EmpresaId        INT NOT NULL,
    PeriodoId        INT NOT NULL,
    ConceptoId       INT NOT NULL CONSTRAINT FK_RelEmpresaPeriodoConceptoOmitido_Concepto REFERENCES dbo.CatConcepto(ConceptoId),
    EsLegal          BIT NOT NULL,
    BloqueaTimbrado  BIT NOT NULL,
    Motivo           NVARCHAR(250) NULL,
    FechaRegistro    DATETIME NOT NULL CONSTRAINT DF_RelEmpresaPeriodoConceptoOmitido_Fecha DEFAULT GETDATE(),
    UsuarioRegistro  NVARCHAR(150) NULL CONSTRAINT DF_RelEmpresaPeriodoConceptoOmitido_Usuario DEFAULT SYSTEM_USER,
    CONSTRAINT PK_RelEmpresaPeriodoConceptoOmitido PRIMARY KEY (EmpresaId, PeriodoId, ConceptoId),
    CONSTRAINT FK_RelEmpresaPeriodoConceptoOmitido_Periodo FOREIGN KEY (PeriodoId, EmpresaId)
        REFERENCES dbo.ProcPeriodoNomina(PeriodoId, EmpresaId),
    CONSTRAINT CK_RelEmpresaPeriodoConceptoOmitido_Motivo CHECK (BloqueaTimbrado = 0 OR LEN(LTRIM(RTRIM(ISNULL(Motivo, N'')))) >= 5)
);
GO

-- Precarga para un periodo limpio los conceptos permanentes y activos asignados a cada trabajador elegible.
CREATE OR ALTER PROCEDURE dbo.sp_ListarIncidenciasFijasAplicables
    @PeriodoId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT r.IdEmpleado, r.CantidadOValor, r.Observaciones, d.SalarioDiario,
           c.ConceptoId, c.ClaveSAT, c.TipoConcepto, c.Descripcion, c.EsGravado,
           c.FormaCalculo, c.FactorMultiplicador, c.EsIncidencia, c.EsBase, c.Activo
    FROM dbo.RelEmpleadoIncidencia r
    JOIN dbo.CatConcepto c ON c.ConceptoId = r.IdConceptoSAT
    JOIN dbo.CatEmpleado e ON e.EmpleadoId = r.IdEmpleado
    JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId
    JOIN dbo.ProcPeriodoNomina p ON p.PeriodoId = @PeriodoId
    WHERE r.Activo = 1 AND r.EsPermanente = 1 AND c.Activo = 1 AND c.EsIncidencia = 1 AND e.Estatus = 'ACTIVO'
      AND e.EmpresaId = p.EmpresaId AND e.FechaIngreso <= p.FechaFin
      AND EXISTS (SELECT 1 FROM dbo.DocContratoEmpleado ec
                  WHERE ec.EmpleadoId = e.EmpleadoId AND ec.Activo = 1 AND ec.PeriodicidadPagoId = p.PeriodicidadPagoId);
END;
GO

-- Índices de apoyo para detectar el periodo vigente y cruzar festivos
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RelPeriodoFestivo_Festivo' AND object_id = OBJECT_ID('dbo.RelPeriodoDiaFestivo'))
    CREATE INDEX IX_RelPeriodoFestivo_Festivo ON dbo.RelPeriodoDiaFestivo(DiaFestivoId, PeriodoId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProcPeriodo_Vigente' AND object_id = OBJECT_ID('dbo.ProcPeriodoNomina'))
    CREATE INDEX IX_ProcPeriodo_Vigente ON dbo.ProcPeriodoNomina(EmpresaId, PeriodicidadPagoId, FechaInicio, FechaFin) INCLUDE (Estatus, NumeroPeriodo);
GO

-- ==============================================================================
-- [BLOQUE-07] BITÁCORAS DE AUDITORÍA Y SEGURIDAD (Log)
-- ==============================================================================
PRINT '[07/21] Creando bitácoras de auditoría y seguridad (Prefijo Log)...';
GO

-- ProcSesionUsuario (Sesiones activas con tokens)
IF OBJECT_ID('dbo.ProcSesionUsuario', 'U') IS NULL
CREATE TABLE dbo.ProcSesionUsuario (
    SesionId    UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_LogSesion_Id DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    UsuarioId   INT NOT NULL CONSTRAINT FK_LogSesion_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    TokenHash   VARBINARY(64) NOT NULL,
    CreadaUtc   DATETIME2(0) NOT NULL CONSTRAINT DF_LogSesion_Creada DEFAULT SYSUTCDATETIME(),
    ExpiraUtc   DATETIME2(0) NOT NULL,
    RevocadaUtc DATETIME2(0) NULL,
    IpOrigen    NVARCHAR(45) NULL,
    UserAgent   NVARCHAR(300) NULL,
    CONSTRAINT UQ_LogSesion_TokenHash UNIQUE (TokenHash)
);
GO

-- ProcRecuperacionContrasena (Solicitudes de reseteo)
IF OBJECT_ID('dbo.ProcRecuperacionContrasena', 'U') IS NULL
CREATE TABLE dbo.ProcRecuperacionContrasena (
    RecuperacionId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_LogRecuperacion_Id DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    UsuarioId      INT NOT NULL CONSTRAINT FK_LogRecup_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    TokenHash      VARBINARY(64) NOT NULL,
    CreadoUtc      DATETIME2(0) NOT NULL CONSTRAINT DF_LogRecup_Creado DEFAULT SYSUTCDATETIME(),
    ExpiraUtc      DATETIME2(0) NOT NULL,
    Usado          BIT NOT NULL CONSTRAINT DF_LogRecup_Usado DEFAULT 0,
    UsadoUtc       DATETIME2(0) NULL,
    IpSolicitud    NVARCHAR(45) NULL,
    CONSTRAINT UQ_LogRecuperacion_TokenHash UNIQUE (TokenHash)
);
GO

-- LogAuth (Auditoría de inicios de sesión y autenticación)
IF OBJECT_ID('dbo.LogAuth', 'U') IS NULL
CREATE TABLE dbo.LogAuth (
    BitacoraAuthId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId      INT NULL CONSTRAINT FK_LogAuth_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    Evento         NVARCHAR(40) NOT NULL,
    Exitoso        BIT NOT NULL,
    Detalle        NVARCHAR(500) NULL,
    IpOrigen       NVARCHAR(45) NULL,
    UserAgent      NVARCHAR(300) NULL,
    FechaUtc       DATETIME2(0) NOT NULL CONSTRAINT DF_LogAuth_Fecha DEFAULT SYSUTCDATETIME()
);
GO

-- LogCalculo (Bitácora de operaciones de cálculo de nómina)
IF OBJECT_ID('dbo.LogCalculo', 'U') IS NULL
CREATE TABLE dbo.LogCalculo (
    BitacoraId INT IDENTITY(1,1) PRIMARY KEY,
    PeriodoId  INT NULL CONSTRAINT FK_LogCalculo_Periodo REFERENCES dbo.ProcPeriodoNomina(PeriodoId),
    EmpleadoId INT NULL CONSTRAINT FK_LogCalculo_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    FechaHora  DATETIME NOT NULL CONSTRAINT DF_LogCalculo_Fecha DEFAULT GETDATE(),
    Tipo       NVARCHAR(30) NOT NULL,
    Mensaje    NVARCHAR(MAX) NOT NULL,
    Usuario    NVARCHAR(150) NOT NULL CONSTRAINT DF_LogCalculo_Usuario DEFAULT SYSTEM_USER
);
GO

-- LogAuditoriaSalario (Bitácora de auditoría sobre modificaciones al salario y SBC)
IF OBJECT_ID('dbo.LogAuditoriaSalario', 'U') IS NULL
CREATE TABLE dbo.LogAuditoriaSalario (
    AuditoriaId      INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId       INT NOT NULL CONSTRAINT FK_LogSalario_Empleado REFERENCES dbo.CatEmpleado(EmpleadoId),
    FechaCambio      DATETIME NOT NULL CONSTRAINT DF_LogSalario_Fecha DEFAULT GETDATE(),
    UsuarioId        INT NULL CONSTRAINT FK_LogSalario_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    CampoModificado  NVARCHAR(50) NOT NULL,
    ValorAnterior    DECIMAL(18,4) NULL,
    ValorNuevo       DECIMAL(18,4) NOT NULL,
    MotivoCambio     NVARCHAR(200) NULL,
    IPOrigen         NVARCHAR(45) NULL,
    CONSTRAINT CK_LogSalario_Campo CHECK (CampoModificado IN ('SalarioDiario','SDI','SBC','FactorIntegracion'))
);
GO

-- LogAuditoriaPolitica (Auditoría de políticas y fórmulas de cálculo)
IF OBJECT_ID('dbo.LogAuditoriaPolitica', 'U') IS NULL
CREATE TABLE dbo.LogAuditoriaPolitica (
    AuditoriaId       INT IDENTITY(1,1) PRIMARY KEY,
    TipoEntidad       NVARCHAR(50) NOT NULL,
    EntidadId         INT NOT NULL,
    Accion            NVARCHAR(20) NOT NULL,
    UsuarioId         INT NULL CONSTRAINT FK_LogPolAud_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    FechaAccion       DATETIME NOT NULL CONSTRAINT DF_LogPolAud_Fecha DEFAULT GETDATE(),
    ValoresAnteriores NVARCHAR(MAX) NULL,
    ValoresNuevos     NVARCHAR(MAX) NULL,
    Justificacion     NVARCHAR(500) NULL,
    IPOrigen          NVARCHAR(45) NULL,
    CONSTRAINT CK_LogPolAud_Accion CHECK (Accion IN ('INSERT','UPDATE','DELETE')),
    CONSTRAINT CK_LogPolAud_Tipo CHECK (TipoEntidad IN ('POLITICA','FORMULA','CONCEPTO_FORMULA'))
);
GO

-- ==============================================================================
-- [BLOQUE-08] ÍNDICES DE RENDIMIENTO (Clustered, Nonclustered, Filtrados, Includes)
-- ==============================================================================
PRINT '[08/21] Creando índices de optimización y cobertura...';
GO

-- Índices en CatEmpresa y CSD
CREATE UNIQUE INDEX UX_HistEmpresaCSD_UnoActivo
    ON dbo.HistEmpresaCSD(EmpresaId) WHERE Activo = 1;

CREATE INDEX IX_CatERP_Empresa
    ON dbo.CatEmpresaRegistroPatronal(EmpresaId, ClaveEntidadFederativa) WHERE Activo = 1;

CREATE INDEX IX_CatDepartamento_Empresa
    ON dbo.CatDepartamento(EmpresaId) INCLUDE (Clave, Descripcion, Activo);

CREATE INDEX IX_CatPuesto_Empresa
    ON dbo.CatPuesto(EmpresaId) INCLUDE (Clave, Descripcion, Activo);

-- Índices en CatEmpleado y Datos
CREATE INDEX IX_CatEmpleado_Estatus
    ON dbo.CatEmpleado(EmpresaId, Estatus) INCLUDE (NumeroEmpleado, FechaIngreso);

CREATE INDEX IX_CatEmpleado_NumeroEmpleado
    ON dbo.CatEmpleado(NumeroEmpleado);




CREATE UNIQUE INDEX UX_CatEBC_UnaActiva
    ON dbo.CatEmpleadoBancoCuenta(EmpleadoId) WHERE Activa = 1;

CREATE INDEX IX_CatEBC_Empleado_Activa
    ON dbo.CatEmpleadoBancoCuenta(EmpleadoId, Activa);

-- Índices en DocContratoEmpleado
CREATE UNIQUE INDEX UX_DocContrato_UnoActivo
    ON dbo.DocContratoEmpleado(EmpleadoId) WHERE Activo = 1;

CREATE INDEX IX_DocContrato_Busqueda
    ON dbo.DocContratoEmpleado(EmpleadoId, FechaInicio, FechaFin)
    INCLUDE (TipoJornadaId, PeriodicidadPagoId, SalarioBaseCotizacion)
    WHERE Activo = 1;

-- Índices en ProcPeriodoNomina
CREATE INDEX IX_ProcPeriodo_Busqueda
    ON dbo.ProcPeriodoNomina(EmpresaId, Estatus, FechaInicio, FechaFin)
    INCLUDE (EjercicioId, PeriodicidadPagoId, NumeroPeriodo);

-- Índices en ProcIncidencias (incidencias legales activas del periodo, consumidas por el cálculo de nómina) y Asistencia
CREATE INDEX IX_ProcIncidencias_PeriodoEmp
    ON dbo.ProcIncidencias(IdPeriodo, IdEmpleado, IdConceptoSAT, FechaIncidencia)
    INCLUDE (CantidadOValor, Estatus) WHERE Estatus = 'ACTIVA';

CREATE INDEX IX_ProcRegAsist_FechaEmp
    ON dbo.ProcRegistroAsistencia(Fecha, EmpleadoId)
    INCLUDE (HorasTrabajadas, HorasExtraDoble, HorasExtraTriple);

CREATE INDEX IX_ProcIncapacidad_EmpFecha
    ON dbo.ProcIncapacidad(EmpleadoId, FechaInicio DESC);

-- Índices en Créditos y Préstamos
CREATE INDEX IX_ProcCreditoInf_Empleado
    ON dbo.ProcCreditoInfonavit(EmpleadoId, Estatus)
    INCLUDE (TipoDescuento, MontoDescuento, ValorDescuento) WHERE Estatus = 'ACTIVO';

CREATE INDEX IX_ProcCreditoFon_Empleado
    ON dbo.ProcCreditoFonacot(EmpleadoId) WHERE Activo = 1;

CREATE INDEX IX_ProcPrestamo_Empleado
    ON dbo.ProcPrestamo(EmpleadoId, Estatus)
    INCLUDE (SaldoRestante, DescuentoPorPeriodo) WHERE Estatus = 'ACTIVO';

-- Índices en Recibos y CFDI
CREATE INDEX IX_DocRecibo_PeriodoEstatus
    ON dbo.DocReciboNomina(PeriodoId, Estatus)
    INCLUDE (EmpleadoId, TotalPercepciones, TotalDeducciones, NetoPagar);

CREATE INDEX IX_DocReciboDetalle_ReciboConcepto
    ON dbo.DocReciboDetalle(ReciboId, TipoConcepto, ClaveConceptoSAT);

CREATE UNIQUE INDEX UX_DocCFDI_UUID
    ON dbo.DocCFDINomina(UUID) WHERE UUID IS NOT NULL;

CREATE UNIQUE INDEX UX_DocCFDI_SerieFolio
    ON dbo.DocCFDINomina(EmpresaId, Serie, Folio) WHERE Serie IS NOT NULL AND Folio IS NOT NULL;

CREATE INDEX IX_DocCFDI_PendientesTimbrado
    ON dbo.DocCFDINomina(ReciboId, EstadoCFDI, ProximoReintento)
    WHERE EstadoCFDI IN (1, 5) AND BloquearReintentos = 0;

CREATE INDEX IX_LogCFDIEvento_CFDIFecha
    ON dbo.LogCFDIEvento(CFDIId, FechaEvento);

-- Índices en Acumulados y Eventos Anuales
CREATE INDEX IX_ProcAcumulado_EjercicioEmpMes
    ON dbo.ProcAcumuladoMensual(EjercicioId, EmpleadoId, Mes);

CREATE INDEX IX_ProcAguinaldo_Ejercicio
    ON dbo.ProcAguinaldo(EjercicioId, EmpleadoId);

CREATE INDEX IX_ProcPTU_Ejercicio
    ON dbo.ProcPTU(EjercicioId, EmpleadoId);

CREATE INDEX IX_ProcAjusteISR_Pendientes
    ON dbo.ProcAjusteAnualISR(EjercicioId, Aplicado) WHERE Aplicado = 0;

-- Índices en Históricos y Bitácoras
CREATE INDEX IX_HistSalario_EmpleadoFecha
    ON dbo.HistEmpleadoSalario(EmpleadoId, FechaEfectiva DESC);

CREATE INDEX IX_LogSesion_UsuarioExpira
    ON dbo.ProcSesionUsuario(UsuarioId, ExpiraUtc);

CREATE INDEX IX_LogRecuperacion_UsuarioExpira
    ON dbo.ProcRecuperacionContrasena(UsuarioId, ExpiraUtc);

CREATE INDEX IX_LogCalculo_TipoFecha
    ON dbo.LogCalculo(Tipo, FechaHora DESC) INCLUDE (PeriodoId, EmpleadoId);

CREATE INDEX IX_LogSalario_EmpFecha
    ON dbo.LogAuditoriaSalario(EmpleadoId, FechaCambio DESC);

-- Índices en Políticas y Fórmulas
CREATE INDEX IX_CatPol_EmpresaActiva
    ON dbo.CatPoliticaEmpresarial(EmpresaId, Activa, FechaInicioVigencia) WHERE Activa = 1;

CREATE INDEX IX_CatFormula_TipoActiva
    ON dbo.CatFormulaCalculo(TipoFormulaId, Activa) WHERE Activa = 1;

CREATE INDEX IX_RelConcForm_ConceptoActivo
    ON dbo.RelConceptoFormula(ConceptoId, Activa, OrdenEjecucion) WHERE Activa = 1;

-- Índices de apoyo para llaves foráneas de tablas transaccionales (joins de la prenómina y borrado/validación de padres)
CREATE INDEX IX_ProcPeriodo_Ejercicio     ON dbo.ProcPeriodoNomina(EjercicioId);
CREATE INDEX IX_ProcPeriodo_Periodicidad  ON dbo.ProcPeriodoNomina(PeriodicidadPagoId);
CREATE INDEX IX_DocRecibo_Empleado        ON dbo.DocReciboNomina(EmpleadoId, PeriodoId);
CREATE INDEX IX_ProcIncidencias_Empleado  ON dbo.ProcIncidencias(IdEmpleado);
CREATE INDEX IX_ProcIncidencias_Concepto  ON dbo.ProcIncidencias(IdConceptoSAT);
CREATE INDEX IX_RelEmpleadoIncidencia_Concepto ON dbo.RelEmpleadoIncidencia(IdConceptoSAT);
CREATE INDEX IX_DocContrato_Periodicidad  ON dbo.DocContratoEmpleado(PeriodicidadPagoId);
CREATE INDEX IX_DocContrato_Puesto        ON dbo.DocContratoEmpleado(PuestoId);
CREATE INDEX IX_DocContrato_Depto         ON dbo.DocContratoEmpleado(DepartamentoId);
CREATE INDEX IX_RelEmpPer_Periodicidad    ON dbo.RelEmpresaPeriodicidad(PeriodicidadPagoId);
CREATE INDEX IX_LogCalculo_Periodo        ON dbo.LogCalculo(PeriodoId);
CREATE INDEX IX_DocFiniquito_Empleado     ON dbo.DocFiniquito(EmpleadoId);
CREATE INDEX IX_DocBaja_Empleado          ON dbo.DocEmpleadoBaja(EmpleadoId);

GO

-- ==============================================================================
-- Integridad financiera, vigencias y relaciones de negocio.
ALTER TABLE dbo.CatUMA ADD CONSTRAINT CK_CatUMA_Valores CHECK (ValorDiario > 0 AND ValorMensual > 0 AND ValorAnual > 0);
ALTER TABLE dbo.CatSalarioMinimo ADD CONSTRAINT CK_CatSalarioMinimo_Valor CHECK (ValorDiario > 0);
ALTER TABLE dbo.CatTablaISR ADD CONSTRAINT CK_CatTablaISR_Valores CHECK (LimiteInferior >= 0 AND CuotaFija >= 0 AND TasaExcedente BETWEEN 0 AND 1);
ALTER TABLE dbo.CatTablaSubsidioEmpleo ADD CONSTRAINT UQ_CatTablaSubsidioEmpleo UNIQUE (EjercicioId,HastaIngreso),
    CONSTRAINT CK_CatTablaSubsidioEmpleo_Valores CHECK (HastaIngreso >= 0 AND MontoSubsidio >= 0);
ALTER TABLE dbo.CatCuotaIMSS ADD CONSTRAINT UQ_CatCuotaIMSS_Concepto UNIQUE (EjercicioId,ClaveConcepto),
    CONSTRAINT CK_CatCuotaIMSS_Tasas CHECK (PorcentajePatronal BETWEEN 0 AND 1 AND PorcentajeObrero BETWEEN 0 AND 1);
ALTER TABLE dbo.CatEmpleado ADD CONSTRAINT CK_CatEmpleado_Numero CHECK (LEN(LTRIM(RTRIM(NumeroEmpleado))) BETWEEN 1 AND 15 AND NumeroEmpleado NOT LIKE N'%|%');
ALTER TABLE dbo.CatEmpresa ADD CONSTRAINT FK_CatEmpresa_CP FOREIGN KEY (CodigoPostal) REFERENCES dbo.CatSatCodigoPostal(Clave),
    CONSTRAINT FK_CatEmpresa_Expedicion FOREIGN KEY (LugarExpedicion) REFERENCES dbo.CatSatCodigoPostal(Clave);
ALTER TABLE dbo.CatEmpleadoDatosFiscales ADD CONSTRAINT FK_CatEDF_CP FOREIGN KEY (CodigoPostal) REFERENCES dbo.CatSatCodigoPostal(Clave);
ALTER TABLE dbo.ProcPeriodoNomina ADD CONSTRAINT FK_ProcPeriodo_Asignacion FOREIGN KEY (EmpresaId,PeriodicidadPagoId)
    REFERENCES dbo.RelEmpresaPeriodicidad(EmpresaId,PeriodicidadPagoId),
    CONSTRAINT CK_ProcPeriodo_Numero CHECK (NumeroPeriodo > 0),
    CONSTRAINT CK_ProcPeriodo_FechaFinalPago CHECK (FechaFinalPago IS NULL OR FechaFinalPago >= FechaPago);
ALTER TABLE dbo.ProcIncapacidad ADD CONSTRAINT CK_ProcIncapacidad_Valores CHECK (PorcentajePago BETWEEN 0 AND 100 AND (ImportePagado IS NULL OR ImportePagado >= 0));
ALTER TABLE dbo.ProcCreditoInfonavit ADD CONSTRAINT CK_ProcCreditoInf_Valores CHECK (MontoDescuento > 0 AND (SaldoPendiente IS NULL OR SaldoPendiente >= 0)),
    CONSTRAINT CK_ProcCreditoInf_Vigencia CHECK (FechaFin IS NULL OR FechaFin >= FechaInicio);
ALTER TABLE dbo.ProcPrestamo ADD CONSTRAINT CK_ProcPrestamo_Saldo CHECK (SaldoRestante <= MontoTotal AND (NumPeriodos IS NULL OR NumPeriodos > 0));
ALTER TABLE dbo.ProcRegistroAsistencia ADD CONSTRAINT CK_ProcRegistroAsistencia_Retardo CHECK (MinutosRetardo >= 0);
ALTER TABLE dbo.HistEmpleadoSalario ADD CONSTRAINT CK_HistEmpleadoSalario_Valores CHECK (SalarioDiario > 0 AND SDI > 0 AND SBC > 0 AND FactorIntegracion >= 1);
ALTER TABLE dbo.HistEmpleadoDatosFiscales ADD CONSTRAINT CK_HistEDF_Vigencia CHECK (FechaHasta IS NULL OR FechaHasta >= FechaDesde);
ALTER TABLE dbo.DocContratoEmpleado ADD CONSTRAINT CK_DocContrato_Salarios CHECK
    ((SalarioBaseCotizacion IS NULL OR SalarioBaseCotizacion > 0) AND (SalarioDiarioIntegrado IS NULL OR SalarioDiarioIntegrado > 0));
ALTER TABLE dbo.DocReciboDetalle ADD CONSTRAINT CK_DocReciboDetalle_Importes CHECK
    (ImporteGravado >= 0 AND ImporteExento >= 0 AND (Cantidad IS NULL OR Cantidad >= 0)),
    CONSTRAINT CK_DocReciboDetalle_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) BETWEEN 1 AND 100 AND Descripcion NOT LIKE N'%|%');
ALTER TABLE dbo.DocReciboNomina ADD
    SnapshotRFCEmisor NVARCHAR(13) NULL,
    SnapshotNombreEmisor NVARCHAR(254) NULL,
    SnapshotRegimenFiscalEmisor NVARCHAR(3) NULL REFERENCES dbo.CatSatRegimenFiscal(Clave),
    SnapshotRFCReceptor NVARCHAR(13) NULL,
    SnapshotNombreReceptor NVARCHAR(254) NULL,
    SnapshotCURPReceptor NVARCHAR(18) NULL,
    SnapshotCodigoPostalReceptor NVARCHAR(5) NULL REFERENCES dbo.CatSatCodigoPostal(Clave),
    SnapshotClaveEntFed NVARCHAR(3) NULL,
    SnapshotNumEmpleado NVARCHAR(15) NULL,
    SnapshotNSS NVARCHAR(15) NULL,
    SnapshotFechaIngreso DATE NULL,
    SnapshotRegistroPatronal NVARCHAR(20) NULL,
    SnapshotOrigenRecurso NVARCHAR(2) NULL REFERENCES dbo.CatSatOrigenRecurso(Clave),
    SnapshotMontoRecursoPropio DECIMAL(18,2) NULL,
    SnapshotSindicalizado BIT NULL,
    SnapshotRfcPatronOrigen NVARCHAR(13) NULL,
    CONSTRAINT CK_DocRecibo_Importes CHECK (TotalPercepciones >= 0 AND TotalDeducciones >= 0 AND TotalOtrosPagos >= 0),
    CONSTRAINT CK_DocRecibo_Neto CHECK (NetoPagar = TotalPercepciones + TotalOtrosPagos - TotalDeducciones),
    CONSTRAINT FK_DocRecibo_Jornada FOREIGN KEY (SnapshotTipoJornada) REFERENCES dbo.CatSatTipoJornada(Clave),
    CONSTRAINT FK_DocRecibo_Periodicidad FOREIGN KEY (SnapshotPeriodicidadPago) REFERENCES dbo.CatSatPeriodicidadPago(Clave),
    CONSTRAINT FK_DocRecibo_Regimen FOREIGN KEY (SnapshotTipoRegimen) REFERENCES dbo.CatSatTipoRegimen(Clave),
    CONSTRAINT FK_DocRecibo_Contrato FOREIGN KEY (SnapshotTipoContrato) REFERENCES dbo.CatSatTipoContrato(Clave),
    CONSTRAINT FK_DocRecibo_Fiscal FOREIGN KEY (SnapshotRegimenFiscalReceptor) REFERENCES dbo.CatSatRegimenFiscal(Clave),
    CONSTRAINT FK_DocRecibo_Banco FOREIGN KEY (SnapshotBanco) REFERENCES dbo.CatSatBanco(Clave);
ALTER TABLE dbo.DocCFDINomina ADD CONSTRAINT CK_DocCFDI_Intentos CHECK (IntentosTimbrado >= 0 AND IntentosCancelacion >= 0),
    CONSTRAINT CK_DocCFDI_TimbradoCompleto CHECK (EstadoCFDI NOT IN (2,3,4)
        OR (UUID IS NOT NULL AND FechaTimbrado IS NOT NULL AND XMLTimbrado IS NOT NULL));
ALTER TABLE dbo.DocFiniquitoDetalle ADD CONSTRAINT CK_DocFinDet_Cuadre CHECK
    (ImporteBruto >= 0 AND ImporteExento >= 0 AND ImporteGravado >= 0 AND ImporteBruto = ImporteExento + ImporteGravado);
ALTER TABLE dbo.ProcSesionUsuario ADD CONSTRAINT CK_ProcSesion_Expira CHECK (ExpiraUtc > CreadaUtc);
ALTER TABLE dbo.ProcRecuperacionContrasena ADD CONSTRAINT CK_ProcRecuperacion_Expira CHECK (ExpiraUtc > CreadoUtc);
ALTER TABLE dbo.CatUsuario ADD CONSTRAINT CK_CatUsuario_Intentos CHECK (IntentosFallidos >= 0);
ALTER TABLE dbo.CatConcepto ADD CONSTRAINT CK_CatConcepto_FactorTope CHECK
    (FactorMultiplicador > 0 AND (TopeUMAAnual IS NULL OR TopeUMAAnual >= 0));
ALTER TABLE dbo.CatPoliticaEmpresarial ADD CONSTRAINT CK_CatPolitica_Factores CHECK
    (FactorHoraExtraDoble >= 2 AND FactorHoraExtraTriple >= 3 AND FactorDiaFestivoLaborado >= 2
     AND PorcentajeFondoAhorroEmpleado BETWEEN 0 AND 100 AND PorcentajeFondoAhorroEmpresa BETWEEN 0 AND 100);
CREATE UNIQUE INDEX UX_CatPaqueteIncidencia_Default ON dbo.CatPaqueteIncidencia(EmpresaId,PeriodicidadPagoId)
    WHERE EsDefault = 1 AND Activo = 1;
GO

CREATE OR ALTER TRIGGER dbo.trg_CatConcepto_MapeoSAT
ON dbo.CatConcepto AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i
        LEFT JOIN dbo.CatSatTipoPercepcion p ON p.TipoPercepcionId = i.TipoPercepcionId
        LEFT JOIN dbo.CatSatTipoDeduccion d ON d.TipoDeduccionId = i.TipoDeduccionId
        LEFT JOIN dbo.CatSatTipoOtroPago o ON o.TipoOtroPagoId = i.TipoOtroPagoId
        WHERE (i.TipoConcepto = 'P' AND i.ClaveSAT <> p.Clave)
           OR (i.TipoConcepto = 'D' AND i.ClaveSAT <> d.Clave)
           OR (i.TipoConcepto = 'O' AND i.ClaveSAT <> o.Clave))
        THROW 51020, 'Clave y referencia SAT no corresponden al mismo concepto.', 1;
END
GO
CREATE OR ALTER TRIGGER dbo.trg_ProcIncidencias_Integridad
ON dbo.ProcIncidencias AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN dbo.CatEmpleado e ON e.EmpleadoId = i.IdEmpleado
        JOIN dbo.ProcPeriodoNomina p ON p.PeriodoId = i.IdPeriodo
        JOIN dbo.CatConcepto c ON c.ConceptoId = i.IdConceptoSAT
        WHERE e.EmpresaId <> p.EmpresaId
           OR (i.FechaIncidencia IS NOT NULL AND i.FechaIncidencia NOT BETWEEN p.FechaInicio AND p.FechaFin)
           OR (c.TipoConcepto = 'I' AND i.FechaIncidencia IS NULL)
           OR (i.Estatus = 'ACTIVA' AND c.Activo = 0))
        THROW 51020, 'Incidencia con empresa, fecha o concepto inconsistente.', 1;
END
GO
CREATE OR ALTER TRIGGER dbo.trg_DocReciboNomina_Empresa
ON dbo.DocReciboNomina AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted i JOIN dbo.CatEmpleado e ON e.EmpleadoId = i.EmpleadoId
               JOIN dbo.ProcPeriodoNomina p ON p.PeriodoId = i.PeriodoId WHERE e.EmpresaId <> p.EmpresaId)
        THROW 51020, 'El empleado y el periodo pertenecen a empresas distintas.', 1;
END
GO
CREATE OR ALTER TRIGGER dbo.trg_DocCFDINomina_Empresa
ON dbo.DocCFDINomina AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted i JOIN dbo.DocReciboNomina r ON r.ReciboId = i.ReciboId
               JOIN dbo.ProcPeriodoNomina p ON p.PeriodoId = r.PeriodoId WHERE i.EmpresaId <> p.EmpresaId)
        THROW 51020, 'El CFDI pertenece a otra empresa.', 1;
END
GO
CREATE OR ALTER TRIGGER dbo.trg_DocReciboDetalle_Inmutable
ON dbo.DocReciboDetalle AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM (SELECT ReciboId FROM inserted UNION SELECT ReciboId FROM deleted) x
               JOIN dbo.DocReciboNomina r ON r.ReciboId = x.ReciboId
               WHERE r.Estatus IN ('TIMBRADO','CANCELADO'))
        THROW 51020, 'No se puede modificar el detalle de un recibo timbrado o cancelado.', 1;
END
GO
CREATE INDEX IX_CatTablaISR_Rango ON dbo.CatTablaISR(EjercicioId,LimiteInferior,LimiteSuperior);
GO
CREATE OR ALTER TRIGGER dbo.trg_CatTablaISR_NoTraslape
ON dbo.CatTablaISR AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted i JOIN dbo.CatTablaISR t WITH (UPDLOCK,HOLDLOCK)
               ON t.EjercicioId = i.EjercicioId AND t.TablaISRId <> i.TablaISRId
               AND t.LimiteInferior <= i.LimiteSuperior AND i.LimiteInferior <= t.LimiteSuperior)
        THROW 51022, 'La tarifa ISR contiene rangos superpuestos.', 1;
END
GO
CREATE INDEX IX_CatTablaISRAnual_Rango ON dbo.CatTablaISRAnual(EjercicioId,LimiteInferior,LimiteSuperior);
GO
CREATE OR ALTER TRIGGER dbo.trg_CatTablaISRAnual_NoTraslape
ON dbo.CatTablaISRAnual AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted i JOIN dbo.CatTablaISRAnual t WITH (UPDLOCK,HOLDLOCK)
               ON t.EjercicioId = i.EjercicioId AND t.TablaISRAnualId <> i.TablaISRAnualId
               AND t.LimiteInferior <= i.LimiteSuperior AND i.LimiteInferior <= t.LimiteSuperior)
        THROW 51022, 'La tarifa ISR anual contiene rangos superpuestos.', 1;
END
GO


-- [BLOQUE-09] FUNCIONES DE VALIDACIÓN (fn_Validar*)
-- ==============================================================================
PRINT '[09/21] Creando funciones de validación...';
GO

-- Validación matemática y sintáctica del RFC
CREATE OR ALTER FUNCTION dbo.fn_ValidarRFC (@RFC NVARCHAR(13))
RETURNS BIT
AS
BEGIN
    IF @RFC IS NULL RETURN 0;
    DECLARE @Len INT = LEN(@RFC);
    IF @Len NOT IN (12, 13) RETURN 0;

    IF @Len = 13 AND @RFC NOT LIKE '[A-ZÑ&][A-ZÑ&][A-ZÑ&][A-ZÑ&][0-9][0-9][0-9][0-9][0-9][0-9][0-9A-Z][0-9A-Z][0-9A-Z]'
        RETURN 0;
    IF @Len = 12 AND @RFC NOT LIKE '[A-ZÑ&][A-ZÑ&][A-ZÑ&][0-9][0-9][0-9][0-9][0-9][0-9][0-9A-Z][0-9A-Z][0-9A-Z]'
        RETURN 0;

    DECLARE @FechaStr NVARCHAR(6) = CASE WHEN @Len = 13 THEN SUBSTRING(@RFC, 5, 6) ELSE SUBSTRING(@RFC, 4, 6) END;
    DECLARE @Mes INT = TRY_CAST(SUBSTRING(@FechaStr, 3, 2) AS INT);
    DECLARE @Dia INT = TRY_CAST(SUBSTRING(@FechaStr, 5, 2) AS INT);
    IF @Mes IS NULL OR @Dia IS NULL OR @Mes < 1 OR @Mes > 12 OR @Dia < 1 OR @Dia > 31 RETURN 0;
    RETURN 1;
END
GO

-- Validación sintáctica de la CURP
CREATE OR ALTER FUNCTION dbo.fn_ValidarCURP (@CURP NVARCHAR(18))
RETURNS BIT
AS
BEGIN
    IF @CURP IS NULL OR LEN(@CURP) <> 18 RETURN 0;
    IF @CURP NOT LIKE '[A-Z][A-Z][A-Z][A-Z][0-9][0-9][0-9][0-9][0-9][0-9][HM][A-Z][A-Z][A-Z][A-Z][A-Z][0-9A-Z][0-9A-Z]'
        RETURN 0;
    DECLARE @Mes INT = TRY_CAST(SUBSTRING(@CURP, 7, 2) AS INT);
    DECLARE @Dia INT = TRY_CAST(SUBSTRING(@CURP, 9, 2) AS INT);
    IF @Mes IS NULL OR @Dia IS NULL OR @Mes < 1 OR @Mes > 12 OR @Dia < 1 OR @Dia > 31 RETURN 0;
    RETURN 1;
END
GO

-- Validación del dígito verificador de CLABE Interbancaria (Módulo 10 Banxico)
CREATE OR ALTER FUNCTION dbo.fn_ValidarCLABE (@CLABE CHAR(18))
RETURNS BIT
AS
BEGIN
    IF @CLABE IS NULL OR LEN(@CLABE) <> 18 OR @CLABE LIKE '%[^0-9]%' RETURN 0;

    DECLARE @i INT = 1, @suma INT = 0, @pesos NVARCHAR(17) = '37137137137137137';
    WHILE @i <= 17
    BEGIN
        SET @suma = @suma + (CAST(SUBSTRING(@CLABE, @i, 1) AS INT) *
                             CAST(SUBSTRING(@pesos, @i, 1) AS INT)) % 10;
        SET @i = @i + 1;
    END
    DECLARE @digCalc INT = (10 - (@suma % 10)) % 10;
    DECLARE @digCLABE INT = CAST(SUBSTRING(@CLABE, 18, 1) AS INT);
    RETURN CASE WHEN @digCalc = @digCLABE THEN 1 ELSE 0 END;
END
GO

-- ==============================================================================
-- [BLOQUE-10] FUNCIONES DE CÁLCULO FISCAL Y LABORAL (fn_*)
-- ==============================================================================
PRINT '[10/21] Creando funciones de cálculo fiscal y laboral...';
GO

-- Obtener UMA Diaria vigente
CREATE OR ALTER FUNCTION dbo.fn_ObtenerUMADiaria (@Anio INT)
RETURNS DECIMAL(12,4)
AS
BEGIN
    DECLARE @v DECIMAL(12,4);
    SELECT TOP 1 @v = ValorDiario FROM dbo.CatUMA WHERE Anio = @Anio ORDER BY Anio DESC;
    RETURN @v;
END
GO

-- Obtener Salario Mínimo General Diario vigente
CREATE OR ALTER FUNCTION dbo.fn_ObtenerSMGDiario (@Anio INT, @Zona NVARCHAR(10))
RETURNS DECIMAL(12,4)
AS
BEGIN
    DECLARE @v DECIMAL(12,4);
    SELECT TOP 1 @v = ValorDiario
    FROM dbo.CatSalarioMinimo WHERE Anio = @Anio AND Zona = @Zona ORDER BY Anio DESC;
    RETURN @v;
END
GO

-- Días de vacaciones según años de servicio (Art. 76 LFT)
CREATE OR ALTER FUNCTION dbo.fn_DiasVacaciones (@AniosServicio INT)
RETURNS INT
AS
BEGIN
    DECLARE @d INT;
    SELECT @d = DiasVacaciones FROM dbo.CatTablaVacaciones
    WHERE @AniosServicio BETWEEN AniosServicioDesde AND AniosServicioHasta;
    IF @AniosServicio IS NULL OR @AniosServicio < 0 RETURN NULL;
    DECLARE @Minimo INT = CASE WHEN @AniosServicio < 1 THEN 12 WHEN @AniosServicio <= 5 THEN 10 + 2 * @AniosServicio
                              ELSE 22 + 2 * ((@AniosServicio - 6) / 5) END;
    RETURN CASE WHEN @d > @Minimo THEN @d ELSE @Minimo END;
END
GO

-- Seleccion por fecha efectiva: enero usa la UMA todavia vigente del ejercicio anterior.
CREATE OR ALTER FUNCTION dbo.fn_ObtenerUMADiariaFecha (@Fecha DATE)
RETURNS DECIMAL(12,4)
AS
BEGIN
    DECLARE @Valor DECIMAL(12,4);
    SELECT TOP (1) @Valor = ValorDiario FROM dbo.CatUMA
    WHERE @Fecha BETWEEN VigenciaDesde AND VigenciaHasta ORDER BY VigenciaDesde DESC;
    RETURN @Valor;
END
GO
CREATE OR ALTER FUNCTION dbo.fn_ObtenerSMGDiarioFecha (@Fecha DATE, @Zona NVARCHAR(10))
RETURNS DECIMAL(12,4)
AS
BEGIN
    DECLARE @Valor DECIMAL(12,4);
    SELECT TOP (1) @Valor = ValorDiario FROM dbo.CatSalarioMinimo
    WHERE @Fecha BETWEEN VigenciaDesde AND VigenciaHasta AND Zona = @Zona ORDER BY VigenciaDesde DESC;
    RETURN @Valor;
END
GO

-- Factor de Integración para SDI y SBC (Art. 84 LFT / Art. 27 LSS)
CREATE OR ALTER FUNCTION dbo.fn_FactorIntegracion (
    @AniosServicio INT,
    @DiasAguinaldo DECIMAL(10,4),
    @PrimaVacacional DECIMAL(10,4),
    @OtrasPrestacionesDiarias DECIMAL(18,4) = 0,
    @SalarioDiario DECIMAL(18,4) = 1.0
)
RETURNS DECIMAL(12,6)
AS
BEGIN
    DECLARE @diasVac DECIMAL(10,4) = dbo.fn_DiasVacaciones(@AniosServicio);
    DECLARE @FactorBase DECIMAL(12,6) = 1.0 + (@PrimaVacacional/100.0 * @diasVac/365.0) + (@DiasAguinaldo/365.0);
    DECLARE @FactorOtras DECIMAL(12,6) = 0;
    IF @SalarioDiario > 0
        SET @FactorOtras = @OtrasPrestacionesDiarias / @SalarioDiario;
    RETURN @FactorBase + @FactorOtras;
END
GO

-- Cálculo de ISR Mensual (Art. 96 LISR)
CREATE OR ALTER FUNCTION dbo.fn_CalcularISRMensual (
    @BaseMensualGravable DECIMAL(18,4),
    @EjercicioId INT
)
RETURNS DECIMAL(18,4)
AS
BEGIN
    IF @BaseMensualGravable IS NULL OR @BaseMensualGravable < 0 RETURN NULL;
    IF @BaseMensualGravable = 0 RETURN 0;
    SET @BaseMensualGravable = ROUND(@BaseMensualGravable, 2);
    DECLARE @CF DECIMAL(18,4), @LI DECIMAL(18,4), @Tasa DECIMAL(10,8);
    SELECT TOP 1 @CF = CuotaFija, @LI = LimiteInferior, @Tasa = TasaExcedente
    FROM dbo.CatTablaISR
    WHERE EjercicioId = @EjercicioId
      AND @BaseMensualGravable >= LimiteInferior
      AND @BaseMensualGravable <= LimiteSuperior
    ORDER BY Renglon;
    RETURN ROUND((@CF + (@BaseMensualGravable - @LI) * @Tasa), 4);
END
GO

-- Cálculo de Subsidio para el Empleo Mensual
CREATE OR ALTER FUNCTION dbo.fn_CalcularSubsidioEmpleo (
    @BaseMensualGravable DECIMAL(18,4),
    @EjercicioId INT
)
RETURNS DECIMAL(18,4)
AS
BEGIN
    DECLARE @s DECIMAL(18,4);
    SELECT TOP 1 @s = MontoSubsidio
    FROM dbo.CatTablaSubsidioEmpleo
    WHERE EjercicioId = @EjercicioId AND @BaseMensualGravable <= HastaIngreso
    ORDER BY HastaIngreso;
    RETURN ROUND(@s, 4);
END
GO

-- Cuota IMSS Obrera
CREATE OR ALTER FUNCTION dbo.fn_CuotaIMSS_Obrero (
    @SBC DECIMAL(18,4), @EjercicioId INT, @Fecha DATE, @DiasPeriodo DECIMAL(5,2)
)
RETURNS DECIMAL(18,4)
AS
BEGIN
    DECLARE @UMADia DECIMAL(12,4) = dbo.fn_ObtenerUMADiariaFecha(@Fecha);
    DECLARE @SBCPer DECIMAL(18,4) = @SBC * @DiasPeriodo;
    DECLARE @Excedente DECIMAL(18,4) = CASE WHEN @SBC > 3.0*@UMADia
                                            THEN (@SBC - 3.0*@UMADia)*@DiasPeriodo ELSE 0 END;
    DECLARE @Total DECIMAL(18,4) = 0;
    SELECT @Total = SUM(PorcentajeObrero *
        CASE BaseCalculo WHEN 'SBC_EXCEDENTE_3UMA' THEN @Excedente
            WHEN 'UMA_DIARIA_POR_TRAB' THEN @UMADia * @DiasPeriodo ELSE @SBCPer END)
            FROM dbo.CatCuotaIMSS
    WHERE EjercicioId = @EjercicioId
      AND ClaveConcepto IN ('EM_EXC','EM_DIN','EM_GMP','IV','CEV')
      AND PorcentajeObrero > 0;
    RETURN ROUND(ISNULL(@Total, 0), 4);
END
GO

-- Cuota IMSS Patronal
CREATE OR ALTER FUNCTION dbo.fn_CuotaIMSS_Patron (
    @SBC DECIMAL(18,4), @EjercicioId INT, @Fecha DATE, @DiasPeriodo DECIMAL(5,2)
)
RETURNS DECIMAL(18,4)
AS
BEGIN
    DECLARE @UMADia DECIMAL(12,4) = dbo.fn_ObtenerUMADiariaFecha(@Fecha);
    DECLARE @SBCPer DECIMAL(18,4) = @SBC * @DiasPeriodo;
    DECLARE @UMABase DECIMAL(18,4) = @UMADia * @DiasPeriodo;
    DECLARE @Excedente DECIMAL(18,4) = CASE WHEN @SBC > 3.0*@UMADia
                                            THEN (@SBC - 3.0*@UMADia)*@DiasPeriodo ELSE 0 END;
    DECLARE @Total DECIMAL(18,4) = 0;
    SELECT @Total = SUM(PorcentajePatronal *
        CASE BaseCalculo
            WHEN 'UMA_DIARIA_POR_TRAB' THEN @UMABase
            WHEN 'SBC_EXCEDENTE_3UMA' THEN @Excedente
            ELSE @SBCPer END)
                FROM dbo.CatCuotaIMSS
    WHERE EjercicioId = @EjercicioId AND PorcentajePatronal > 0;
    RETURN ROUND(ISNULL(@Total, 0), 4);
END
GO

-- Hash de contraseñas con sal (SHA2_512)
CREATE OR ALTER FUNCTION dbo.fn_Auth_HashContrasena (@Contrasena NVARCHAR(256), @Salt VARBINARY(32))
RETURNS VARBINARY(64)
AS
BEGIN
    RETURN HASHBYTES('SHA2_512', CONVERT(VARBINARY(MAX), ISNULL(@Contrasena, N'')) + ISNULL(@Salt, 0x));
END
GO

-- Hash de tokens de sesión
CREATE OR ALTER FUNCTION dbo.fn_Auth_HashToken (@TokenPlano NVARCHAR(128))
RETURNS VARBINARY(64)
AS
BEGIN
    RETURN HASHBYTES('SHA2_512', CONVERT(VARBINARY(MAX), ISNULL(@TokenPlano, N'')));
END
GO

-- Cálculo de ISR Anual (Art. 152 LISR)
CREATE OR ALTER FUNCTION dbo.fn_CalcularISRAnual (
    @BaseAnualGravable DECIMAL(18,4),
    @EjercicioId INT
)
RETURNS DECIMAL(18,4)
AS
BEGIN
    IF @BaseAnualGravable IS NULL OR @BaseAnualGravable < 0 RETURN NULL;
    IF @BaseAnualGravable = 0 RETURN 0;
    SET @BaseAnualGravable = ROUND(@BaseAnualGravable, 2);
    DECLARE @CF DECIMAL(18,4), @LI DECIMAL(18,4), @Tasa DECIMAL(10,8);
    SELECT @CF = CuotaFija, @LI = LimiteInferior, @Tasa = TasaExcedente
    FROM dbo.CatTablaISRAnual
    WHERE EjercicioId = @EjercicioId AND @BaseAnualGravable BETWEEN LimiteInferior AND LimiteSuperior;
    RETURN ROUND(@CF + (@BaseAnualGravable - @LI) * @Tasa, 4);
END
GO

-- Años de servicio CUMPLIDOS entre dos fechas (aniversarios ya alcanzados). DATEDIFF(YEAR) cuenta cruces de año calendario
-- y sobrestima la antigüedad hasta en un año (ej. ingreso 31-dic, corte 01-ene = 1 con DATEDIFF, 0 años cumplidos).
CREATE OR ALTER FUNCTION dbo.fn_AniosCumplidos (
    @FechaIngreso DATE,
    @FechaCorte   DATE
)
RETURNS INT
AS
BEGIN
    IF @FechaIngreso IS NULL OR @FechaCorte IS NULL OR @FechaCorte < @FechaIngreso RETURN 0;
    RETURN DATEDIFF(YEAR, @FechaIngreso, @FechaCorte)
         - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, @FechaIngreso, @FechaCorte), @FechaIngreso) > @FechaCorte THEN 1 ELSE 0 END;
END
GO

-- Cálculo de antigüedad en años con fracción
CREATE OR ALTER FUNCTION dbo.fn_CalcularAntiguedadAnios (
    @FechaIngreso DATE,
    @FechaCorte   DATE
)
RETURNS DECIMAL(10,4)
AS
BEGIN
    IF @FechaCorte IS NULL OR @FechaIngreso IS NULL OR @FechaCorte < @FechaIngreso RETURN 0;
    RETURN ROUND(CAST(DATEDIFF(DAY, @FechaIngreso, @FechaCorte) AS DECIMAL(10,4)) / 365.25, 4);
END
GO

-- Salario diario vigente según SCD
CREATE OR ALTER FUNCTION dbo.fn_ObtenerSalarioVigente (
    @EmpleadoId INT,
    @FechaRef   DATE
)
RETURNS DECIMAL(18,4)
AS
BEGIN
    DECLARE @SD DECIMAL(18,4);
    SELECT TOP 1 @SD = SalarioDiario
    FROM dbo.HistEmpleadoSalario
    WHERE EmpleadoId = @EmpleadoId AND FechaEfectiva <= @FechaRef
    ORDER BY FechaEfectiva DESC;
    IF @SD IS NULL
        SELECT @SD = SalarioDiario FROM dbo.CatEmpleadoDatosIMSS WHERE EmpleadoId = @EmpleadoId;
    RETURN ISNULL(@SD, 0);
END
GO

-- Exención de Aguinaldo (Art. 93 fracc. XIV LISR - 30 UMA)
CREATE OR ALTER FUNCTION dbo.fn_ExencionAguinaldo (
    @MontoAguinaldo DECIMAL(18,4),
    @Anio INT
)
RETURNS DECIMAL(18,4)
AS
BEGIN
    DECLARE @Tope DECIMAL(18,4) = dbo.fn_ObtenerUMADiaria(@Anio) * 30.0;
    RETURN CASE WHEN @MontoAguinaldo > @Tope THEN @Tope ELSE @MontoAguinaldo END;
END
GO

-- Exención de Prima Vacacional (Art. 93 fracc. XIV LISR - 15 UMA)
CREATE OR ALTER FUNCTION dbo.fn_ExencionPrimaVacacional (
    @MontoPrimaVac DECIMAL(18,4),
    @Anio INT
)
RETURNS DECIMAL(18,4)
AS
BEGIN
    DECLARE @Tope DECIMAL(18,4) = dbo.fn_ObtenerUMADiaria(@Anio) * 15.0;
    RETURN CASE WHEN @MontoPrimaVac > @Tope THEN @Tope ELSE @MontoPrimaVac END;
END
GO

-- Exención de Indemnizaciones y Prima de Antigüedad (Art. 93 fracc. XIII LISR - 90 UMA por año)
CREATE OR ALTER FUNCTION dbo.fn_ExencionSeparacion (
    @Monto DECIMAL(18,4),
    @AniosServicio DECIMAL(10,4),
    @Anio INT
)
RETURNS DECIMAL(18,4)
AS
BEGIN
    DECLARE @Tope DECIMAL(18,4) = dbo.fn_ObtenerUMADiaria(@Anio) * 90.0 * @AniosServicio;
    RETURN CASE WHEN @Monto > @Tope THEN @Tope ELSE @Monto END;
END
GO

-- Obtener Id de política empresarial activa
CREATE OR ALTER FUNCTION dbo.fn_ObtenerPoliticaActiva (@EmpresaId INT, @FechaRef DATE = NULL)
RETURNS INT
AS
BEGIN
    IF @FechaRef IS NULL SET @FechaRef = CAST(GETDATE() AS DATE);
    DECLARE @Pol INT;
    SELECT TOP 1 @Pol = PoliticaId
        FROM dbo.CatPoliticaEmpresarial
    WHERE EmpresaId = @EmpresaId AND Activa = 1
      AND FechaInicioVigencia <= @FechaRef
      AND (FechaFinVigencia IS NULL OR FechaFinVigencia >= @FechaRef)
    ORDER BY FechaInicioVigencia DESC;
    RETURN ISNULL(@Pol, 0);
END
GO

-- Cantidad de veces que un día de la semana ISO (1 = lunes ... 7 = domingo) ocurre entre dos fechas, ambas incluidas.
-- Independiente de SET DATEFIRST: 1900-01-01 fue lunes.
CREATE OR ALTER FUNCTION dbo.fn_ContarDiaSemana (@Desde DATE, @Hasta DATE, @DiaSemanaISO TINYINT)
RETURNS INT
AS
BEGIN
    IF @Desde IS NULL OR @Hasta IS NULL OR @DiaSemanaISO IS NULL OR @Hasta < @Desde
        RETURN 0;

    DECLARE @Dias INT = DATEDIFF(DAY, @Desde, @Hasta) + 1;
    DECLARE @DiaInicial INT = (DATEDIFF(DAY, '19000101', @Desde) % 7) + 1;
    DECLARE @Salto INT = ((@DiaSemanaISO - @DiaInicial) + 7) % 7;

    RETURN CASE WHEN @Salto >= @Dias THEN 0 ELSE ((@Dias - @Salto - 1) / 7) + 1 END;
END
GO

-- ==============================================================================
CREATE OR ALTER PROCEDURE dbo.sp_ValidarReciboTimbrado @ReciboId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.DocReciboNomina r
               JOIN dbo.RelEmpresaPeriodoConceptoOmitido o ON o.PeriodoId = r.PeriodoId AND o.BloqueaTimbrado = 1
               WHERE r.ReciboId = @ReciboId)
        THROW 51031, 'No se puede timbrar: el periodo omite conceptos legales y el recibo no cumple los requisitos del CFDI de nómina. Quite la omisión en Paquetes por periodo.', 1;
    DECLARE @FechaPago DATE, @TipoNomina NVARCHAR(1), @Regimen NVARCHAR(2);
    SELECT @FechaPago = SnapshotFechaPago, @TipoNomina = TipoNomina, @Regimen = SnapshotTipoRegimen
    FROM dbo.DocReciboNomina WHERE ReciboId = @ReciboId;
    IF @FechaPago IS NULL THROW 51030, 'Recibo inexistente o sin fecha de pago fiscal.', 1;
    IF EXISTS (
        SELECT 1 FROM dbo.DocReciboNomina r WHERE ReciboId = @ReciboId AND (
            SnapshotRFCEmisor IS NULL OR SnapshotNombreEmisor IS NULL OR SnapshotRegimenFiscalEmisor IS NULL
            OR SnapshotRFCReceptor IS NULL OR SnapshotNombreReceptor IS NULL OR SnapshotCURPReceptor IS NULL
            OR SnapshotCodigoPostalReceptor IS NULL OR SnapshotNumEmpleado IS NULL OR SnapshotClaveEntFed IS NULL
            OR SnapshotTipoContrato IS NULL OR SnapshotTipoRegimen IS NULL OR SnapshotPeriodicidadPago IS NULL
            OR SnapshotRegimenFiscalReceptor IS NULL OR SnapshotRegimenFiscalReceptor <> N'605'
            OR SnapshotFechaInicialPago IS NULL OR SnapshotFechaFinalPago IS NULL OR TipoNomina IS NULL
            OR SnapshotFechaInicialPago > SnapshotFechaFinalPago OR SnapshotDiasPagados IS NULL
            OR SnapshotDiasPagados NOT BETWEEN 0.001 AND 36160
            OR NetoPagar < 0 OR TotalGravado IS NULL OR TotalExento IS NULL))
        THROW 51030, 'Faltan datos fiscales obligatorios o hay importes/fechas no timbrables.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoNomina WHERE Clave = @TipoNomina
                   AND FechaInicioVigencia <= @FechaPago AND (FechaFinVigencia IS NULL OR FechaFinVigencia >= @FechaPago))
        THROW 51030, 'Tipo de nomina fuera de vigencia.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.RelSatUsoCFDIRegimenFiscal WHERE ClaveUsoCFDI = N'CN01' AND ClaveRegimenFiscal = N'605')
        THROW 51030, 'Importe la relacion oficial UsoCFDI/RegimenFiscal.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.DocReciboDetalle WHERE ReciboId = @ReciboId)
        THROW 51030, 'El recibo no tiene detalle.', 1;
    IF EXISTS (
        SELECT 1 FROM dbo.DocReciboNomina r
        CROSS APPLY (
            SELECT SUM(CASE WHEN TipoConcepto = 'P' THEN ImporteGravado + ImporteExento ELSE 0 END) AS Percepciones,
                   SUM(CASE WHEN TipoConcepto = 'D' THEN ImporteGravado + ImporteExento ELSE 0 END) AS Deducciones,
                   SUM(CASE WHEN TipoConcepto = 'O' THEN ImporteGravado + ImporteExento ELSE 0 END) AS OtrosPagos,
                   SUM(CASE WHEN TipoConcepto = 'P' THEN ImporteGravado ELSE 0 END) AS Gravado,
                   SUM(CASE WHEN TipoConcepto = 'P' THEN ImporteExento ELSE 0 END) AS Exento
            FROM dbo.DocReciboDetalle WHERE ReciboId = r.ReciboId) d
        WHERE r.ReciboId = @ReciboId AND (r.TotalPercepciones <> d.Percepciones OR r.TotalDeducciones <> d.Deducciones
            OR r.TotalOtrosPagos <> d.OtrosPagos OR ROUND(r.TotalGravado,2) <> d.Gravado OR ROUND(r.TotalExento,2) <> d.Exento))
        THROW 51030, 'Los totales fiscales no cuadran con el detalle.', 1;
    IF EXISTS (
        SELECT 1 FROM dbo.DocReciboDetalle d
        JOIN dbo.CatConcepto c ON c.ClaveSAT = d.ClaveConceptoSAT AND c.TipoConcepto = d.TipoConcepto
        LEFT JOIN dbo.CatSatTipoPercepcion p ON p.TipoPercepcionId = c.TipoPercepcionId
        LEFT JOIN dbo.CatSatTipoDeduccion de ON de.TipoDeduccionId = c.TipoDeduccionId
        LEFT JOIN dbo.CatSatTipoOtroPago o ON o.TipoOtroPagoId = c.TipoOtroPagoId
        WHERE d.ReciboId = @ReciboId AND (
            COALESCE(p.FechaInicioVigencia,de.FechaInicioVigencia,o.FechaInicioVigencia) > @FechaPago
            OR COALESCE(p.FechaFinVigencia,de.FechaFinVigencia,o.FechaFinVigencia,'99991231') < @FechaPago))
        THROW 51030, 'Una clave de concepto SAT esta fuera de vigencia.', 1;
    IF EXISTS (
        SELECT 1 FROM dbo.DocReciboDetalle d WHERE d.ReciboId = @ReciboId AND
        ((d.TipoConcepto = 'P' AND d.ClaveConceptoSAT = '019' AND NOT EXISTS
            (SELECT 1 FROM dbo.DocReciboHorasExtra h WHERE h.DetalleId = d.DetalleId))
         OR (d.TipoConcepto = 'P' AND d.ClaveConceptoSAT = '045' AND NOT EXISTS
            (SELECT 1 FROM dbo.DocReciboAccionesTitulos a WHERE a.DetalleId = d.DetalleId))
         OR (d.TipoConcepto = 'O' AND d.ClaveConceptoSAT = '002' AND NOT EXISTS
            (SELECT 1 FROM dbo.DocReciboOtroPago o WHERE o.DetalleId = d.DetalleId AND o.SubsidioCausado IS NOT NULL))
         OR (d.TipoConcepto = 'O' AND d.ClaveConceptoSAT = '004' AND NOT EXISTS
            (SELECT 1 FROM dbo.DocReciboOtroPago o WHERE o.DetalleId = d.DetalleId AND o.SaldoAFavor IS NOT NULL
             AND o.Anio IS NOT NULL AND o.RemanenteSalFav IS NOT NULL))))
        THROW 51030, 'Falta un nodo condicional: horas extra, acciones, subsidio o compensacion.', 1;
    IF EXISTS (SELECT 1 FROM dbo.DocReciboDetalle WHERE ReciboId = @ReciboId AND TipoConcepto = 'P'
               AND ClaveConceptoSAT IN ('022','023','025'))
       AND NOT EXISTS (SELECT 1 FROM dbo.DocReciboSeparacionIndemnizacion WHERE ReciboId = @ReciboId)
        THROW 51030, 'Falta el nodo SeparacionIndemnizacion.', 1;
    IF EXISTS (SELECT 1 FROM dbo.DocReciboDetalle WHERE ReciboId = @ReciboId AND TipoConcepto = 'P'
               AND ClaveConceptoSAT IN ('039','044'))
       AND NOT EXISTS (SELECT 1 FROM dbo.DocReciboJubilacionPensionRetiro WHERE ReciboId = @ReciboId)
        THROW 51030, 'Falta el nodo JubilacionPensionRetiro.', 1;
END
GO
-- [BLOQUE-11] STORED PROCEDURES CRÍTICOS (sp_*)
-- ==============================================================================
PRINT '[11/21] Creando stored procedures operativos y transaccionales...';
GO

-- sp_CargarDatosFiscales (Carga paramétrica de tablas anuales)
CREATE OR ALTER PROCEDURE dbo.sp_CargarDatosFiscales
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @EjercicioId INT;
    SELECT @EjercicioId = EjercicioId FROM dbo.CatEjercicioFiscal WHERE Anio = @Anio;
    IF @EjercicioId IS NULL
    BEGIN
        RAISERROR('No existe EjercicioFiscal para el año %d.', 16, 1, @Anio);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE FROM dbo.CatTablaISR            WHERE EjercicioId = @EjercicioId;
        DELETE FROM dbo.CatTablaISRAnual       WHERE EjercicioId = @EjercicioId;
        DELETE FROM dbo.CatTablaSubsidioEmpleo WHERE EjercicioId = @EjercicioId;
        DELETE FROM dbo.CatCuotaIMSS           WHERE EjercicioId = @EjercicioId;

        -- Tarifa ISR mensual (Art. 96 LISR)
        IF @Anio = 2024
        INSERT INTO dbo.CatTablaISR (EjercicioId, Renglon, LimiteInferior, LimiteSuperior, CuotaFija, TasaExcedente) VALUES
        (@EjercicioId,  1,       0.01,     746.04,     0.00, 0.01920000),
        (@EjercicioId,  2,     746.05,    6332.05,    14.32, 0.06400000),
        (@EjercicioId,  3,    6332.06,   11128.01,   371.83, 0.10880000),
        (@EjercicioId,  4,   11128.02,   12935.82,   893.63, 0.16000000),
        (@EjercicioId,  5,   12935.83,   15487.71,  1182.88, 0.17920000),
        (@EjercicioId,  6,   15487.72,   31236.49,  1640.18, 0.21360000),
        (@EjercicioId,  7,   31236.50,   49233.00,  4997.55, 0.23520000),
        (@EjercicioId,  8,   49233.01,   93993.90,  9232.94, 0.30000000),
        (@EjercicioId,  9,   93993.91,  125325.20, 22665.17, 0.32000000),
        (@EjercicioId, 10,  125325.21,  375975.61, 32691.18, 0.34000000),
        (@EjercicioId, 11,  375975.62, 9999999.99,117912.32, 0.35000000);

        IF @Anio = 2025
        INSERT INTO dbo.CatTablaISR (EjercicioId, Renglon, LimiteInferior, LimiteSuperior, CuotaFija, TasaExcedente) VALUES
        (@EjercicioId,  1,       0.01,     794.63,     0.00, 0.01920000),
        (@EjercicioId,  2,     794.64,    6744.20,    15.26, 0.06400000),
        (@EjercicioId,  3,    6744.21,   11851.66,   396.03, 0.10880000),
        (@EjercicioId,  4,   11851.67,   13777.68,   951.87, 0.16000000),
        (@EjercicioId,  5,   13777.69,   16495.06,  1260.05, 0.17920000),
        (@EjercicioId,  6,   16495.07,   33266.42,  1747.14, 0.21360000),
        (@EjercicioId,  7,   33266.43,   52432.16,  5321.90, 0.23520000),
        (@EjercicioId,  8,   52432.17,  100088.26,  9831.60, 0.30000000),
        (@EjercicioId,  9,  100088.27,  133450.90, 24128.14, 0.32000000),
        (@EjercicioId, 10,  133450.91,  400302.42, 34811.13, 0.34000000),
        (@EjercicioId, 11,  400302.43, 9999999.99,125540.61, 0.35000000);

        IF @Anio >= 2026
        BEGIN
            INSERT INTO dbo.CatTablaISR (EjercicioId, Renglon, LimiteInferior, LimiteSuperior, CuotaFija, TasaExcedente) VALUES
            (@EjercicioId,  1,       0.01,     830.19,     0.00, 0.01920000),
            (@EjercicioId,  2,     830.20,    7046.36,    15.94, 0.06400000),
            (@EjercicioId,  3,    7046.37,   12381.09,   413.75, 0.10880000),
            (@EjercicioId,  4,   12381.10,   14392.72,   994.47, 0.16000000),
            (@EjercicioId,  5,   14392.73,   17232.31,  1316.44, 0.17920000),
            (@EjercicioId,  6,   17232.32,   34755.33,  1825.34, 0.21360000),
            (@EjercicioId,  7,   34755.34,   54781.09,  5559.85, 0.23520000),
            (@EjercicioId,  8,   54781.10,  104568.13, 10271.55, 0.30000000),
            (@EjercicioId,  9,  104568.14,  139419.12, 25207.79, 0.32000000),
            (@EjercicioId, 10,  139419.13,  418235.63, 36366.03, 0.34000000),
            (@EjercicioId, 11,  418235.64, 9999999.99,131155.36, 0.35000000);

            -- Tarifa ISR anual (Art. 152 LISR)
            INSERT INTO dbo.CatTablaISRAnual (EjercicioId, Renglon, LimiteInferior, LimiteSuperior, CuotaFija, TasaExcedente) VALUES
            (@EjercicioId,  1,       0.01,    9962.28,       0.00, 0.01920000),
            (@EjercicioId,  2,    9962.29,   84556.32,     191.28, 0.06400000),
            (@EjercicioId,  3,   84556.33,  148573.08,    4965.00, 0.10880000),
            (@EjercicioId,  4,  148573.09,  172712.64,   11933.64, 0.16000000),
            (@EjercicioId,  5,  172712.65,  206787.72,   15797.28, 0.17920000),
            (@EjercicioId,  6,  206787.73,  417063.96,   21904.08, 0.21360000),
            (@EjercicioId,  7,  417063.97,  657373.08,   66718.20, 0.23520000),
            (@EjercicioId,  8,  657373.09, 1254817.56,  123258.60, 0.30000000),
            (@EjercicioId,  9, 1254817.57, 1673029.44,  302493.48, 0.32000000),
            (@EjercicioId, 10, 1673029.45, 5018827.56,  436392.36, 0.34000000),
            (@EjercicioId, 11, 5018827.57,99999999.99, 1573864.32, 0.35000000);
        END

        -- Subsidio para el Empleo
        INSERT INTO dbo.CatTablaSubsidioEmpleo (EjercicioId, HastaIngreso, MontoSubsidio) VALUES
        (@EjercicioId, 1768.96, 407.02),(@EjercicioId, 2653.38, 406.83),
        (@EjercicioId, 3472.84, 406.62),(@EjercicioId, 3537.87, 392.77),
        (@EjercicioId, 4446.15, 382.46),(@EjercicioId, 4717.18, 354.23),
        (@EjercicioId, 5335.42, 324.87),(@EjercicioId, 6224.67, 294.63),
        (@EjercicioId, 7113.90, 253.54),(@EjercicioId, 7382.33, 217.61),
        (@EjercicioId, 9999999.99, 0.00);

        -- Cuotas IMSS e INFONAVIT
        INSERT INTO dbo.CatCuotaIMSS (EjercicioId, ClaveConcepto, Descripcion, PorcentajePatronal, PorcentajeObrero, BaseCalculo) VALUES
        (@EjercicioId,'EM_CF', 'E y M – Cuota Fija (20.40% UMA diaria)',  0.20400000, 0.00000000, 'UMA_DIARIA_POR_TRAB'),
        (@EjercicioId,'EM_EXC','E y M – Excedente sobre 3 UMAs',          0.01100000, 0.00400000, 'SBC_EXCEDENTE_3UMA'),
        (@EjercicioId,'EM_DIN','E y M – Prestaciones en Dinero',          0.00700000, 0.00250000, 'SBC'),
        (@EjercicioId,'EM_GMP','E y M – Gastos Médicos Pensionados',      0.01050000, 0.00375000, 'SBC'),
        (@EjercicioId,'IV',    'Invalidez y Vida',                        0.01750000, 0.00625000, 'SBC'),
        (@EjercicioId,'GUA',   'Guarderías y Prestaciones Sociales',      0.01000000, 0.00000000, 'SBC'),
        (@EjercicioId,'RET',   'Retiro (aportación a AFORE)',             0.02000000, 0.00000000, 'SBC'),
        (@EjercicioId,'CEV',   'Cesantía en Edad Avanzada y Vejez',       0.03150000, 0.01125000, 'SBC'),
        (@EjercicioId,'RT',    'Riesgo de Trabajo',                       0.00504000, 0.00000000, 'SBC'),
        (@EjercicioId,'INFO',  'INFONAVIT – Aportación Patronal 5%',      0.05000000, 0.00000000, 'SBC');

        INSERT INTO dbo.LogCalculo (Tipo, Mensaje)
        VALUES ('FISCAL', CONCAT('Datos fiscales cargados exitosamente para ejercicio ', @Anio));

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO
-- sp_ActualizarSBC (Actualización y auditoría de Salario Base de Cotización)
CREATE OR ALTER PROCEDURE dbo.sp_ActualizarSBC
    @EmpleadoId INT,
    @FechaEfectiva DATE,
    @SalarioDiario DECIMAL(18,4),
    @DiasAguinaldo DECIMAL(10,4) = 15,
    @PrimaVacacional DECIMAL(10,4) = 25,
    @MotivoCambio NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @EmpleadoId IS NULL OR @EmpleadoId <= 0
    BEGIN RAISERROR('EmpleadoId inválido.', 16, 1); RETURN; END
    IF @FechaEfectiva IS NULL
    BEGIN RAISERROR('FechaEfectiva no puede ser nula.', 16, 1); RETURN; END
    IF @FechaEfectiva > CAST(GETDATE() AS DATE)
    BEGIN RAISERROR('FechaEfectiva no puede ser futura.', 16, 1); RETURN; END
    IF @SalarioDiario IS NULL OR @SalarioDiario <= 0
    BEGIN RAISERROR('El salario diario debe ser mayor a cero.', 16, 1); RETURN; END

    DECLARE @SMGDia DECIMAL(12,4) = dbo.fn_ObtenerSMGDiarioFecha(@FechaEfectiva, 'GENERAL');
    IF @SMGDia IS NULL THROW 51010, 'Falta salario minimo para la fecha efectiva.', 1;
    IF @SalarioDiario < @SMGDia
    BEGIN
        DECLARE @MsgSMG NVARCHAR(500) = CONCAT('Salario diario $', FORMAT(@SalarioDiario,'N2'),
            ' es menor al salario mínimo $', FORMAT(@SMGDia,'N2'), ' (Art. 90 LFT).');
        RAISERROR(@MsgSMG, 16, 1); RETURN;
    END

    IF @DiasAguinaldo < 15
    BEGIN RAISERROR('Aguinaldo no puede ser menor a 15 días (Art. 87 LFT).', 16, 1); RETURN; END
    IF @PrimaVacacional < 25
    BEGIN RAISERROR('Prima vacacional no puede ser menor a 25%% (Art. 80 LFT).', 16, 1); RETURN; END

    DECLARE @FechaIngreso DATE;
        SELECT @FechaIngreso = FechaIngreso FROM dbo.CatEmpleado WHERE EmpleadoId = @EmpleadoId;
    IF @FechaIngreso IS NULL
    BEGIN RAISERROR('Empleado %d no encontrado.', 16, 1, @EmpleadoId); RETURN; END
    IF @FechaEfectiva < @FechaIngreso
    BEGIN RAISERROR('FechaEfectiva no puede ser anterior a la fecha de ingreso.', 16, 1); RETURN; END

    DECLARE @Anios INT = dbo.fn_AniosCumplidos(@FechaIngreso, @FechaEfectiva);
    DECLARE @FI DECIMAL(12,6) = dbo.fn_FactorIntegracion(@Anios, @DiasAguinaldo, @PrimaVacacional, 0, @SalarioDiario);
    DECLARE @SDI DECIMAL(18,4) = ROUND(@SalarioDiario * @FI, 4);
    DECLARE @Tope25 DECIMAL(12,4) = dbo.fn_ObtenerUMADiariaFecha(@FechaEfectiva) * 25.0;
    IF @Tope25 IS NULL THROW 51010, 'Falta UMA para la fecha efectiva.', 1;
    DECLARE @SBC DECIMAL(18,4) = CASE WHEN @SDI > @Tope25 THEN @Tope25 ELSE @SDI END;

    DECLARE @SalarioAnterior DECIMAL(18,4);
    SELECT @SalarioAnterior = SalarioDiario FROM dbo.CatEmpleadoDatosIMSS WHERE EmpleadoId = @EmpleadoId;

    IF @SalarioAnterior IS NULL THROW 51010, 'El empleado no tiene datos IMSS para actualizar.', 1;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.CatEmpleadoDatosIMSS SET
            SalarioDiario = @SalarioDiario,
            SalarioDiarioIntegrado = @SDI,
            SBC = @SBC
        WHERE EmpleadoId = @EmpleadoId;

        INSERT INTO dbo.HistEmpleadoSalario (EmpleadoId, FechaEfectiva, SalarioDiario, SDI, SBC, FactorIntegracion, MotivoCambio)
        VALUES (@EmpleadoId, @FechaEfectiva, @SalarioDiario, @SDI, @SBC, @FI, @MotivoCambio);

        INSERT INTO dbo.LogAuditoriaSalario (EmpleadoId, CampoModificado, ValorAnterior, ValorNuevo, MotivoCambio)
        VALUES (@EmpleadoId, 'SalarioDiario', @SalarioAnterior, @SalarioDiario, @MotivoCambio);

        INSERT INTO dbo.LogCalculo (EmpleadoId, Tipo, Mensaje)
        VALUES (@EmpleadoId, 'SBC', CONCAT('SBC actualizado a $', FORMAT(@SBC,'N2'), ' (FI=', FORMAT(@FI,'N6'), ')'));

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_CalcularNomina (Prenómina y emisión de recibos del periodo con concurrencia controlada y sin recálculo duplicado)
-- Reglas de negocio:
--   * La empresa debe estar activa y tener asignada la periodicidad del periodo (RelEmpresaPeriodicidad).
--   * Deben existir empleados activos (> 0) para la combinación Empresa + Periodicidad.
--   * Solo se procesan periodos CREADO/PRENOMINA. Un periodo ya CALCULADO no se recalcula: se reabre antes con sp_ReversarCalculo.
--   * Sueldo base + proporcional del séptimo día (si el periodo CalculaSeptimoDia), horas extra, prima dominical,
--     pago adicional por días festivos laborados, bonos, descuentos, ISR, subsidio, IMSS, INFONAVIT y préstamos.
CREATE OR ALTER PROCEDURE dbo.sp_CalcularNomina
    @PeriodoId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

    IF EXISTS (SELECT 1 FROM dbo.ProcIncidencias i JOIN dbo.CatConcepto c ON c.ConceptoId=i.IdConceptoSAT
      WHERE i.IdPeriodo=@PeriodoId AND c.TipoConcepto <> 'I' AND (c.Activo=0 OR c.EsIncidencia=0))
        THROW 50000, 'Una incidencia usa un concepto SAT inactivo o no capturable.', 1;

    IF EXISTS (
        SELECT 1 FROM dbo.ProcPeriodoNomina p
        JOIN dbo.CatEmpleado e ON e.EmpresaId=p.EmpresaId AND e.Estatus='ACTIVO' AND e.FechaIngreso<=p.FechaFin
        JOIN dbo.RelEmpleadoIncidencia r ON r.IdEmpleado=e.EmpleadoId AND r.Activo=1 AND r.EsPermanente=1
        JOIN dbo.CatConcepto c ON c.ConceptoId=r.IdConceptoSAT AND c.Activo=1 AND c.EsIncidencia=1
        WHERE p.PeriodoId=@PeriodoId AND p.IncidenciasInicializadas=0
          AND EXISTS(SELECT 1 FROM dbo.DocContratoEmpleado ec WHERE ec.EmpleadoId=e.EmpleadoId AND ec.Activo=1 AND ec.PeriodicidadPagoId=p.PeriodicidadPagoId)
    )
        THROW 50000, 'Inicialice las incidencias desde la aplicacion antes de calcular este periodo.', 1;

    DECLARE @EstatusActual NVARCHAR(20), @EstatusPrevio NVARCHAR(20);
    DECLARE @UsuarioCalc NVARCHAR(150), @FechaInicioCalc DATETIME;
    DECLARE @EmpresaId INT, @EjercicioId INT, @PeriodicidadId INT;
    DECLARE @DiasPeriodo DECIMAL(5,2), @FechaInicio DATE, @FechaFin DATE, @FechaPago DATE;
    DECLARE @DiaDescanso TINYINT, @CalculaSeptimo BIT, @Anio INT;
    DECLARE @Mins INT;

    SELECT
        @EstatusActual   = Estatus,
        @UsuarioCalc     = UsuarioCalculando,
        @FechaInicioCalc = FechaInicioCalculo,
        @EmpresaId       = EmpresaId,
        @EjercicioId     = EjercicioId,
        @PeriodicidadId  = PeriodicidadPagoId,
        @DiasPeriodo     = DiasPeriodo,
        @FechaInicio     = FechaInicio,
        @FechaFin        = FechaFin,
        @FechaPago       = FechaPago,
        @DiaDescanso     = DiaDescanso,
        @CalculaSeptimo  = CalculaSeptimoDia
    FROM dbo.ProcPeriodoNomina
    WHERE PeriodoId = @PeriodoId;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('El período %d no existe.', 16, 1, @PeriodoId);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.CatEmpresa WHERE EmpresaId = @EmpresaId AND Activa = 1)
    BEGIN
        RAISERROR('La empresa del período %d no está activa. Registre o active una empresa antes de procesar la nómina.', 16, 1, @PeriodoId);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.RelEmpresaPeriodicidad
                   WHERE EmpresaId = @EmpresaId AND PeriodicidadPagoId = @PeriodicidadId AND Activo = 1)
    BEGIN
        RAISERROR('La periodicidad del período %d no está asignada a la empresa.', 16, 1, @PeriodoId);
        RETURN;
    END

    IF @EstatusActual = 'CALCULADO'
    BEGIN
        RAISERROR('El período %d ya fue procesado (CALCULADO) y no puede recalcularse. Reábralo con sp_ReversarCalculo si necesita corregirlo.', 16, 1, @PeriodoId);
        RETURN;
    END

    IF @EstatusActual NOT IN ('CREADO','PRENOMINA','EN_CALCULO')
    BEGIN
        RAISERROR('El período %d tiene estatus [%s] y no puede procesarse.', 16, 1, @PeriodoId, @EstatusActual);
        RETURN;
    END

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.CatEmpleado e
        INNER JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId
        INNER JOIN dbo.DocContratoEmpleado c ON c.EmpleadoId = e.EmpleadoId AND c.Activo = 1
        WHERE e.EmpresaId = @EmpresaId AND e.Estatus = 'ACTIVO'
          AND c.PeriodicidadPagoId = @PeriodicidadId AND e.FechaIngreso <= @FechaFin)
    BEGIN
        RAISERROR('No hay empleados activos asignados a la empresa y periodicidad del período %d: no se puede calcular la prenómina.', 16, 1, @PeriodoId);
        RETURN;
    END

    -- Toma atómica del periodo: solo un usuario a la vez. Un bloqueo con 30 minutos o más se considera abandonado.
    SET @EstatusPrevio = CASE WHEN @EstatusActual = 'EN_CALCULO' THEN 'PRENOMINA' ELSE @EstatusActual END;
    UPDATE dbo.ProcPeriodoNomina
       SET Estatus            = 'EN_CALCULO',
           UsuarioCalculando  = SYSTEM_USER,
           FechaInicioCalculo = GETDATE()
     WHERE PeriodoId = @PeriodoId
       AND (Estatus IN ('CREADO','PRENOMINA')
            OR (Estatus = 'EN_CALCULO' AND DATEDIFF(MINUTE, FechaInicioCalculo, GETDATE()) >= 30));

    IF @@ROWCOUNT = 0
    BEGIN
        SET @Mins = ISNULL(DATEDIFF(MINUTE, @FechaInicioCalc, GETDATE()), 0);
        RAISERROR('Período %d ya en cálculo por %s desde hace %d min.', 16, 1, @PeriodoId, @UsuarioCalc, @Mins);
        RETURN;
    END

    DECLARE @EmpleadoId INT, @SD DECIMAL(18,4), @SDI DECIMAL(18,4), @SBC DECIMAL(18,4), @FechaIngreso DATE;
    DECLARE @Faltas DECIMAL(10,2), @HED DECIMAL(10,2), @HET DECIMAL(10,2);
    DECLARE @DiasFestLab DECIMAL(10,2), @DiasPrimaDom DECIMAL(10,2), @Bonos DECIMAL(18,2), @Descuentos DECIMAL(18,2);
    DECLARE @ConceptosGravados DECIMAL(18,2), @ConceptosExentos DECIMAL(18,2), @ConceptosDeduccion DECIMAL(18,2);
    DECLARE @DiasLabPag DECIMAL(10,4), @DiasSeptimo DECIMAL(10,4), @DiasTrab DECIMAL(10,4), @DiasFest DECIMAL(10,2);
    DECLARE @DiasSemana TINYINT, @DiasContratados DECIMAL(10,4);
    DECLARE @SueldoDias DECIMAL(18,4), @PagoSeptimo DECIMAL(18,4), @SalarioBase DECIMAL(18,4), @SalarioHora DECIMAL(18,4);
    DECLARE @ImpHED DECIMAL(18,4), @ImpHET DECIMAL(18,4), @ExHE DECIMAL(18,4), @GrHE DECIMAL(18,4);
    DECLARE @ImpFest DECIMAL(18,4), @ExFest DECIMAL(18,4), @ImpPD DECIMAL(18,4), @ExPD DECIMAL(18,4);
    DECLARE @DiasAguinaldo DECIMAL(10,2), @DiasPrimaVac DECIMAL(10,2);
    DECLARE @ImpAgui DECIMAL(18,4), @ExAgui DECIMAL(18,4), @ImpPrimaVac DECIMAL(18,4), @ExPrimaVac DECIMAL(18,4);
    DECLARE @DiasAntiguedad DECIMAL(10,2), @DiasIndemniz DECIMAL(10,2);
    DECLARE @ImpAntiguedad DECIMAL(18,4), @ExAntiguedad DECIMAL(18,4), @ImpIndemniz DECIMAL(18,4), @ExIndemniz DECIMAL(18,4);
    DECLARE @TotalGravado DECIMAL(18,4), @TotalExento DECIMAL(18,4), @TotalPerc DECIMAL(18,4);
    DECLARE @FactorMes DECIMAL(10,6), @BaseMes DECIMAL(18,4), @ISRMes DECIMAL(18,4), @SubMes DECIMAL(18,4);
    DECLARE @ISRNeto DECIMAL(18,4), @SubPer DECIMAL(18,4), @ISRPer DECIMAL(18,4), @CuoObr DECIMAL(18,4);
    DECLARE @DescINFO DECIMAL(18,4), @TipoDesc NVARCHAR(20), @MontoDesc DECIMAL(18,4), @DescPrest DECIMAL(18,4);
    DECLARE @TotalDed DECIMAL(18,4), @Neto DECIMAL(18,4), @AntiguedadSem INT;
    DECLARE @SnapTipoJornada NVARCHAR(3), @SnapPeriodicidad NVARCHAR(3), @SnapTipoRegimen NVARCHAR(3),
            @SnapTipoContrato NVARCHAR(3), @SnapPuesto NVARCHAR(100), @SnapDepto NVARCHAR(100),
            @SnapTipoContratoDesc NVARCHAR(150), @SnapRegimenFisc NVARCHAR(3);
    DECLARE @ReciboId INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM dbo.ProcIncidencias i JOIN dbo.CatConcepto c WITH (HOLDLOCK) ON c.ConceptoId=i.IdConceptoSAT
          WHERE i.IdPeriodo=@PeriodoId AND c.TipoConcepto <> 'I' AND (c.Activo=0 OR c.EsIncidencia=0))
            THROW 50000, 'Una incidencia usa un concepto SAT inactivo o no capturable.', 1;

                    SELECT @Anio = Anio FROM dbo.CatEjercicioFiscal WHERE EjercicioId = @EjercicioId;
        DECLARE @JornadaDiaria DECIMAL(6,4) =
            ISNULL((SELECT JornadaSemanalHoras FROM dbo.CatEjercicioFiscal WHERE EjercicioId = @EjercicioId), 48) / 6.0;
        -- Fórmulas de los conceptos SAT manejados por el motor (prima dominical 020, horas extra 019).
        DECLARE @FormPD NVARCHAR(1000) = (SELECT TOP (1) f.ExpresionCalculo FROM dbo.CatFormulaCalculo f
            JOIN dbo.RelConceptoFormula rf ON rf.FormulaId = f.FormulaId AND rf.Activa = 1
            JOIN dbo.CatConcepto c ON c.ConceptoId = rf.ConceptoId
            WHERE c.ClaveSAT = '020' AND c.TipoConcepto = 'P' AND f.Activa = 1);
        DECLARE @FormHE NVARCHAR(1000) = (SELECT TOP (1) f.ExpresionCalculo FROM dbo.CatFormulaCalculo f
            JOIN dbo.RelConceptoFormula rf ON rf.FormulaId = f.FormulaId AND rf.Activa = 1
            JOIN dbo.CatConcepto c ON c.ConceptoId = rf.ConceptoId
            WHERE c.ClaveSAT = '019' AND c.TipoConcepto = 'P' AND f.Activa = 1);
        DECLARE @FormAgui NVARCHAR(1000) = (SELECT TOP (1) f.ExpresionCalculo FROM dbo.CatFormulaCalculo f
            JOIN dbo.RelConceptoFormula rf ON rf.FormulaId = f.FormulaId AND rf.Activa = 1
            JOIN dbo.CatConcepto c ON c.ConceptoId = rf.ConceptoId
            WHERE c.ClaveSAT = '002' AND c.TipoConcepto = 'P' AND f.Activa = 1);
        DECLARE @FormPrimaVac NVARCHAR(1000) = (SELECT TOP (1) f.ExpresionCalculo FROM dbo.CatFormulaCalculo f
            JOIN dbo.RelConceptoFormula rf ON rf.FormulaId = f.FormulaId AND rf.Activa = 1
            JOIN dbo.CatConcepto c ON c.ConceptoId = rf.ConceptoId
            WHERE c.ClaveSAT = '021' AND c.TipoConcepto = 'P' AND f.Activa = 1);
        DECLARE @FormAntiguedad NVARCHAR(1000) = (SELECT TOP (1) f.ExpresionCalculo FROM dbo.CatFormulaCalculo f
            JOIN dbo.RelConceptoFormula rf ON rf.FormulaId = f.FormulaId AND rf.Activa = 1
            JOIN dbo.CatConcepto c ON c.ConceptoId = rf.ConceptoId
            WHERE c.ClaveSAT = '022' AND c.TipoConcepto = 'P' AND f.Activa = 1);
        DECLARE @FormIndemniz NVARCHAR(1000) = (SELECT TOP (1) f.ExpresionCalculo FROM dbo.CatFormulaCalculo f
            JOIN dbo.RelConceptoFormula rf ON rf.FormulaId = f.FormulaId AND rf.Activa = 1
            JOIN dbo.CatConcepto c ON c.ConceptoId = rf.ConceptoId
            WHERE c.ClaveSAT = '025' AND c.TipoConcepto = 'P' AND f.Activa = 1);

        -- Limpieza idempotente previa de recibos anteriores para este periodo (las notas del recibo se conservan)
        DECLARE @NotasPrevias TABLE (EmpleadoId INT PRIMARY KEY, Notas NVARCHAR(500) NOT NULL);
        INSERT INTO @NotasPrevias (EmpleadoId, Notas)
        SELECT EmpleadoId, Notas FROM dbo.DocReciboNomina WHERE PeriodoId = @PeriodoId AND Notas IS NOT NULL;

        DELETE n FROM dbo.DocReciboAccionesTitulos n
          INNER JOIN dbo.DocReciboDetalle rd ON rd.DetalleId = n.DetalleId
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = rd.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboHorasExtra n
          INNER JOIN dbo.DocReciboDetalle rd ON rd.DetalleId = n.DetalleId
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = rd.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboOtroPago n
          INNER JOIN dbo.DocReciboDetalle rd ON rd.DetalleId = n.DetalleId
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = rd.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboIncapacidad n
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = n.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboJubilacionPensionRetiro n
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = n.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboSeparacionIndemnizacion n
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = n.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.RelReciboSubContratacion n
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = n.ReciboId WHERE rn.PeriodoId = @PeriodoId;

        DELETE rd FROM dbo.DocReciboDetalle rd
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = rd.ReciboId
         WHERE rn.PeriodoId = @PeriodoId;
        DELETE FROM dbo.DocReciboNomina WHERE PeriodoId = @PeriodoId;

        -- Parámetros de la política empresarial vigente (valores de ley si la empresa no tiene política)
        DECLARE @FactHED DECIMAL(5,2) = 2.00, @FactHET DECIMAL(5,2) = 3.00, @LimHE DECIMAL(5,2) = 9.00,
                @ExenHEUMAs DECIMAL(10,4) = 5.00, @PorcPD DECIMAL(5,2) = 25.00, @ExenPDUMAs DECIMAL(10,4) = 1.00,
                @FactFest DECIMAL(5,2) = 2.00;
        SELECT
            @FactHED    = FactorHoraExtraDoble,
            @FactHET    = FactorHoraExtraTriple,
            @LimHE      = LimiteHorasExtraSinGravar,
            @ExenHEUMAs = ExencionHorasExtraUMAsSemanal,
            @PorcPD     = PorcentajePrimaDominical,
            @ExenPDUMAs = ExencionPrimaDominicalUMAs,
            @FactFest   = FactorDiaFestivoLaborado
        FROM dbo.CatPoliticaEmpresarial
        WHERE PoliticaId = dbo.fn_ObtenerPoliticaActiva(@EmpresaId, @FechaFin);

        -- Séptimo día: descansos del periodo y días laborables sobre los que se prorratea
        DECLARE @Descansos INT = CASE WHEN @CalculaSeptimo = 1 AND @DiaDescanso IS NOT NULL
                                      THEN dbo.fn_ContarDiaSemana(@FechaInicio, @FechaFin, @DiaDescanso) ELSE 0 END;
        DECLARE @DiasLaborables DECIMAL(10,4) = CASE WHEN @DiasPeriodo - @Descansos > 0 THEN @DiasPeriodo - @Descansos ELSE 0 END;
        DECLARE @FestivosPeriodo INT = (SELECT COUNT(1) FROM dbo.RelPeriodoDiaFestivo WHERE PeriodoId = @PeriodoId);
        -- UMA y SMG se resuelven por la FECHA FIN del periodo (vigencia), no por GETDATE() ni por año: una nómina de
        -- diciembre timbrada en enero, o un periodo de enero (la UMA cambia el 1-feb), aplica la tarifa exacta del periodo.
        DECLARE @UMADia DECIMAL(12,4) = dbo.fn_ObtenerUMADiariaFecha(@FechaFin);
        IF @UMADia IS NULL OR @UMADia <= 0 THROW 51010, 'Falta UMA vigente.', 1;
        IF NOT EXISTS (SELECT 1 FROM dbo.CatCuotaIMSS WHERE EjercicioId = @EjercicioId)
            THROW 51010, 'Faltan cuotas IMSS del ejercicio.', 1;
        DECLARE @TopeExSemanal DECIMAL(18,4) = @UMADia * @ExenHEUMAs * (@DiasPeriodo / 7.0);

        -- Cursor filtrado por Empresa y periodicidad del contrato vigente
        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT e.EmpleadoId, d.SalarioDiario, d.SalarioDiarioIntegrado, d.SBC, e.FechaIngreso
            FROM dbo.CatEmpleado e
            INNER JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId
            INNER JOIN dbo.DocContratoEmpleado c ON c.EmpleadoId = e.EmpleadoId AND c.Activo = 1
            WHERE e.EmpresaId = @EmpresaId
              AND e.Estatus = 'ACTIVO'
              AND c.PeriodicidadPagoId = @PeriodicidadId
              AND e.FechaIngreso <= @FechaFin;

        OPEN cur;
        FETCH NEXT FROM cur INTO @EmpleadoId, @SD, @SDI, @SBC, @FechaIngreso;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SELECT
                @Faltas       = ISNULL(SUM(CASE WHEN c.DisminuyeDias = 1 AND c.FormaCalculo = 'DIAS' THEN i.CantidadOValor ELSE 0 END), 0),
                @HED          = ISNULL(SUM(CASE WHEN c.ClaveSAT = '019' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0),
                @HET          = 0,
                @DiasFestLab  = 0,
                @DiasPrimaDom = ISNULL(SUM(CASE WHEN c.ClaveSAT = '020' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0),
                @DiasAguinaldo = ISNULL(SUM(CASE WHEN c.ClaveSAT = '002' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0),
                @DiasPrimaVac  = ISNULL(SUM(CASE WHEN c.ClaveSAT = '021' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0),
                @DiasAntiguedad = ISNULL(SUM(CASE WHEN c.ClaveSAT = '022' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0),
                @DiasIndemniz   = ISNULL(SUM(CASE WHEN c.ClaveSAT = '025' AND c.TipoConcepto = 'P' THEN i.CantidadOValor ELSE 0 END), 0),
                @Bonos        = 0,
                @Descuentos   = 0
            FROM dbo.ProcIncidencias i
            INNER JOIN dbo.CatConcepto c ON c.ConceptoId = i.IdConceptoSAT
            WHERE i.IdEmpleado = @EmpleadoId AND i.IdPeriodo = @PeriodoId AND i.Estatus = 'ACTIVA';

            -- Sueldo por días laborados + proporcional del séptimo día (se pierde en la misma proporción que las faltas).
            -- Con un contrato de menos de 7 días por semana solo se pagan los días contratados del periodo (tope: los laborables).
            SET @DiasSemana = dbo.fn_ObtenerDiasSemanaEmpleado(@EmpleadoId, @PeriodoId);
            SET @DiasContratados = CASE WHEN @DiasSemana IS NULL OR ROUND(@DiasPeriodo * @DiasSemana / 7.0, 4) >= @DiasLaborables
                                        THEN @DiasLaborables ELSE ROUND(@DiasPeriodo * @DiasSemana / 7.0, 4) END;
            SET @DiasLabPag  = CASE WHEN @DiasContratados - @Faltas < 0 THEN 0 ELSE @DiasContratados - @Faltas END;
            SET @DiasSeptimo = CASE WHEN @Descansos > 0 AND @DiasLaborables > 0
                                    THEN @Descansos * @DiasLabPag / @DiasLaborables ELSE 0 END;
            SET @DiasTrab    = @DiasLabPag + @DiasSeptimo;
            SET @SueldoDias  = @SD * @DiasLabPag;
            SET @PagoSeptimo = @SD * @DiasSeptimo;
            SET @SalarioBase = @SueldoDias + @PagoSeptimo;
            -- Salario por hora: horas del turno propio del trabajador si tiene uno; si no, la jornada diaria estándar del
            -- ejercicio (jornada semanal de ley / 6: 8 h hasta 2026, 7.6667 h desde 2027 con la semana de 46 h).
            DECLARE @HorasJornada DECIMAL(6,4);
            SET @HorasJornada = (SELECT t.Horas FROM dbo.CatEmpleado e2
                                 LEFT JOIN dbo.CatTurno t ON t.TurnoId = e2.TurnoId
                                 WHERE e2.EmpleadoId = @EmpleadoId);
            SET @HorasJornada = COALESCE(NULLIF(@HorasJornada, 0), @JornadaDiaria);
            SET @SalarioHora = @SD / NULLIF(@HorasJornada,0);

            -- Horas extra dobles: importe por el motor de fórmulas (factor resoluble por empresa/periodo/empleado).
            DECLARE @FactHERes DECIMAL(18,6) = COALESCE(dbo.fn_ResolverParametro('FACTOR_HORAS_EXTRA_DOBLE', @EmpresaId, @PeriodoId, @EmpleadoId), @FactHED);
            SET @ImpHED = 0;
            IF @HED > 0 AND @FormHE IS NOT NULL
                EXEC dbo.sp_EvaluarFormula @Expresion = @FormHE, @HORAS = @HED, @SALARIO_HORA = @SalarioHora, @FACTOR = @FactHERes, @Resultado = @ImpHED OUTPUT;
            ELSE IF @HED > 0
                SET @ImpHED = @HED * @SalarioHora * @FactHED;
            SET @ImpHET = CASE WHEN @HET = 0 THEN 0 ELSE @HET * @SalarioHora * @FactHET END;
            SET @ExHE   = CASE WHEN @HED <= @LimHE
                               THEN CASE WHEN @ImpHED * 0.5 > @TopeExSemanal THEN @TopeExSemanal ELSE @ImpHED * 0.5 END
                               ELSE 0 END;
            SET @GrHE   = (@ImpHED + @ImpHET) - @ExHE;

            -- Días festivos laborados: pago adicional (LFT Art. 75) limitado a los festivos del periodo; 50% exento con el tope semanal compartido con horas extra
            SET @DiasFest = CASE WHEN @DiasFestLab > @FestivosPeriodo THEN @FestivosPeriodo ELSE @DiasFestLab END;
            SET @ImpFest  = @DiasFest * @SD * @FactFest;
            SET @ExFest   = CASE WHEN @ImpFest * 0.5 > (CASE WHEN @TopeExSemanal - @ExHE < 0 THEN 0 ELSE @TopeExSemanal - @ExHE END)
                                 THEN (CASE WHEN @TopeExSemanal - @ExHE < 0 THEN 0 ELSE @TopeExSemanal - @ExHE END)
                                 ELSE @ImpFest * 0.5 END;

            -- Prima dominical: importe por el motor de fórmulas; el % y el tope de exención se resuelven por empresa/periodo/empleado.
            DECLARE @PorcPDRes DECIMAL(18,6) = COALESCE(dbo.fn_ResolverParametro('PORCENTAJE_PRIMA_DOMINICAL', @EmpresaId, @PeriodoId, @EmpleadoId), @PorcPD);
            DECLARE @TopePDRes DECIMAL(18,6) = COALESCE(dbo.fn_ResolverParametro('TOPE_UMA_PRIMA_DOMINICAL', @EmpresaId, @PeriodoId, @EmpleadoId), @ExenPDUMAs);
            SET @ImpPD = 0;
            IF @DiasPrimaDom > 0 AND @FormPD IS NOT NULL
                EXEC dbo.sp_EvaluarFormula @Expresion = @FormPD, @DIAS = @DiasPrimaDom, @SD = @SD, @PORCENTAJE = @PorcPDRes, @Resultado = @ImpPD OUTPUT;
            ELSE IF @DiasPrimaDom > 0
                SET @ImpPD = @DiasPrimaDom * @SD * @PorcPD / 100.0;
            SET @ExPD  = CASE WHEN @ImpPD > @DiasPrimaDom * @UMADia * @TopePDRes THEN @DiasPrimaDom * @UMADia * @TopePDRes ELSE @ImpPD END;

            -- Aguinaldo (LFT Art. 87): días x salario diario; exento hasta TOPE_UMA_AGUINALDO UMA (LISR 93-XIV).
            DECLARE @TopeAguiRes DECIMAL(18,6) = COALESCE(dbo.fn_ResolverParametro('TOPE_UMA_AGUINALDO', @EmpresaId, @PeriodoId, @EmpleadoId), 30);
            SET @ImpAgui = 0;
            IF @DiasAguinaldo > 0 AND @FormAgui IS NOT NULL
                EXEC dbo.sp_EvaluarFormula @Expresion = @FormAgui, @DIAS = @DiasAguinaldo, @SD = @SD, @Resultado = @ImpAgui OUTPUT;
            ELSE IF @DiasAguinaldo > 0
                SET @ImpAgui = @DiasAguinaldo * @SD;
            SET @ExAgui = CASE WHEN @ImpAgui > @TopeAguiRes * @UMADia THEN @TopeAguiRes * @UMADia ELSE @ImpAgui END;

            -- Prima vacacional (LFT Art. 80): días de vacaciones x salario diario x %; exenta hasta TOPE_UMA_PRIMA_VACACIONAL UMA.
            DECLARE @PctPrimaVacRes DECIMAL(18,6) = COALESCE(dbo.fn_ResolverParametro('PORCENTAJE_PRIMA_VACACIONAL', @EmpresaId, @PeriodoId, @EmpleadoId), 25);
            DECLARE @TopePrimaVacRes DECIMAL(18,6) = COALESCE(dbo.fn_ResolverParametro('TOPE_UMA_PRIMA_VACACIONAL', @EmpresaId, @PeriodoId, @EmpleadoId), 15);
            SET @ImpPrimaVac = 0;
            IF @DiasPrimaVac > 0 AND @FormPrimaVac IS NOT NULL
                EXEC dbo.sp_EvaluarFormula @Expresion = @FormPrimaVac, @DIAS = @DiasPrimaVac, @SD = @SD, @PORCENTAJE = @PctPrimaVacRes, @Resultado = @ImpPrimaVac OUTPUT;
            ELSE IF @DiasPrimaVac > 0
                SET @ImpPrimaVac = @DiasPrimaVac * @SD * @PctPrimaVacRes / 100.0;
            SET @ExPrimaVac = CASE WHEN @ImpPrimaVac > @TopePrimaVacRes * @UMADia THEN @TopePrimaVacRes * @UMADia ELSE @ImpPrimaVac END;

            -- Prima de antigüedad (022, LFT Art. 162) e indemnizaciones por separación (025): importe por el motor;
            -- exención de separación COMPARTIDA = 90 UMA por año de servicio (LISR Art. 93-XIII), repartida a prorrata.
            DECLARE @AniosServ INT = dbo.fn_AniosCumplidos(@FechaIngreso, @FechaFin);
            SET @ImpAntiguedad = 0;
            IF @DiasAntiguedad > 0 AND @FormAntiguedad IS NOT NULL
                EXEC dbo.sp_EvaluarFormula @Expresion = @FormAntiguedad, @DIAS = @DiasAntiguedad, @SD = @SD, @Resultado = @ImpAntiguedad OUTPUT;
            ELSE IF @DiasAntiguedad > 0
                SET @ImpAntiguedad = @DiasAntiguedad * @SD;
            SET @ImpIndemniz = 0;
            IF @DiasIndemniz > 0 AND @FormIndemniz IS NOT NULL
                EXEC dbo.sp_EvaluarFormula @Expresion = @FormIndemniz, @DIAS = @DiasIndemniz, @SD = @SD, @Resultado = @ImpIndemniz OUTPUT;
            ELSE IF @DiasIndemniz > 0
                SET @ImpIndemniz = @DiasIndemniz * @SD;
            DECLARE @ImpSepar DECIMAL(18,4) = @ImpAntiguedad + @ImpIndemniz;
            DECLARE @ExSepar DECIMAL(18,4) = CASE WHEN @ImpSepar > @AniosServ * 90.0 * @UMADia THEN @AniosServ * 90.0 * @UMADia ELSE @ImpSepar END;
            SET @ExAntiguedad = CASE WHEN @ImpSepar > 0 THEN ROUND(@ExSepar * @ImpAntiguedad / @ImpSepar, 4) ELSE 0 END;
            SET @ExIndemniz   = @ExSepar - @ExAntiguedad;

            -- Conceptos capturados como incidencia por monto (días de descanso laborados, festivos, bonos, descuentos, etc.).
            -- Solo se excluyen los 6 conceptos que recalcula el motor de fórmulas con rama dedicada (019, 020, 002, 021,
            -- 022, 025 en percepciones), ya sumados arriba, para no contarlos dos veces. Los demás conceptos con fórmula
            -- (p. ej. 054 días de descanso laborados) se integran aquí con el importe capturado.
            SELECT @ConceptosGravados = ISNULL(SUM(CASE WHEN c.TipoConcepto = 'P' AND c.EsGravado = 1 THEN i.ImporteCalculado ELSE 0 END),0),
                   @ConceptosExentos = ISNULL(SUM(CASE WHEN c.TipoConcepto = 'P' AND c.EsGravado = 0 THEN i.ImporteCalculado ELSE 0 END),0),
                   @ConceptosDeduccion = ISNULL(SUM(CASE WHEN c.TipoConcepto = 'D' THEN i.ImporteCalculado ELSE 0 END),0)
            FROM dbo.ProcIncidencias i
            JOIN dbo.CatConcepto c ON c.ConceptoId = i.IdConceptoSAT AND c.TipoConcepto <> 'I'
            WHERE i.IdPeriodo = @PeriodoId AND i.IdEmpleado = @EmpleadoId AND i.Estatus = 'ACTIVA'
              AND NOT (c.TipoConcepto = 'P' AND c.ClaveSAT IN ('019','020','002','021','022','025'));

            SET @TotalGravado = @SalarioBase + @GrHE + (@ImpFest - @ExFest) + (@ImpPD - @ExPD) + (@ImpAgui - @ExAgui) + (@ImpPrimaVac - @ExPrimaVac) + (@ImpAntiguedad - @ExAntiguedad) + (@ImpIndemniz - @ExIndemniz) + @Bonos + @ConceptosGravados;
            SET @TotalExento  = @ExHE + @ExFest + @ExPD + @ExAgui + @ExPrimaVac + @ExAntiguedad + @ExIndemniz + @ConceptosExentos;
            SET @TotalPerc    = @SalarioBase + @ImpHED + @ImpHET + @ImpFest + @ImpPD + @ImpAgui + @ImpPrimaVac + @ImpAntiguedad + @ImpIndemniz + @Bonos + @ConceptosGravados + @ConceptosExentos;

            SET @FactorMes = 30.4167 / @DiasPeriodo;
            SET @BaseMes   = @TotalGravado * @FactorMes;
            SET @ISRMes    = dbo.fn_CalcularISRMensual(@BaseMes, @EjercicioId);
            SET @SubMes    = dbo.fn_CalcularSubsidioEmpleo(@BaseMes, @EjercicioId);
            IF @ISRMes IS NULL OR @SubMes IS NULL
                THROW 51010, 'No existe tarifa ISR/subsidio para la base calculada.', 1;
            SET @ISRNeto   = CASE WHEN @ISRMes - @SubMes < 0 THEN 0 ELSE @ISRMes - @SubMes END;
            DECLARE @Entrega BIT;
            SELECT TOP (1) @Entrega = PermiteEntrega FROM dbo.CatTablaSubsidioEmpleo
            WHERE EjercicioId = @EjercicioId AND @BaseMes <= HastaIngreso ORDER BY HastaIngreso;
            SET @SubPer = CASE WHEN @Entrega = 1 AND @SubMes > @ISRMes
                               THEN ROUND((@SubMes - @ISRMes) / @FactorMes,2) ELSE 0 END;
            SET @ISRPer    = ROUND(@ISRNeto / @FactorMes, 2);

            SET @CuoObr = dbo.fn_CuotaIMSS_Obrero(@SBC, @EjercicioId, @FechaFin, @DiasTrab);

            -- Descuento INFONAVIT (las variables se reinician: dentro de un ciclo conservarían el valor del empleado anterior)
            SET @DescINFO = 0;
            SET @TipoDesc = NULL;
            SET @MontoDesc = NULL;
            SELECT TOP 1 @TipoDesc = TipoDescuento, @MontoDesc = MontoDescuento
            FROM dbo.ProcCreditoInfonavit WHERE EmpleadoId = @EmpleadoId AND Activo = 1 AND Estatus = 'ACTIVO';
            IF @TipoDesc IS NOT NULL
                SET @DescINFO = CASE @TipoDesc
                    WHEN 'CUOTA_FIJA' THEN @MontoDesc
                    WHEN 'PORCENTAJE' THEN @SBC * @DiasTrab * @MontoDesc / 100.0
                    WHEN 'VSM'        THEN @MontoDesc * dbo.fn_ObtenerSMGDiarioFecha(@FechaFin,'GENERAL') * @DiasTrab
                    ELSE 0 END;

            -- Descuento Préstamo Interno (calculado sin mutar el saldo definitivo hasta el cierre)
            SELECT @DescPrest = ISNULL(SUM(CASE WHEN SaldoRestante >= DescuentoPorPeriodo
                                                THEN DescuentoPorPeriodo ELSE SaldoRestante END), 0)
            FROM dbo.ProcPrestamo WHERE EmpleadoId = @EmpleadoId AND Estatus = 'ACTIVO';

            SET @TotalDed = @ISRPer + @CuoObr + @DescINFO + @DescPrest + @Descuentos + @ConceptosDeduccion;
            SET @TotalPerc = ROUND(@TotalPerc,2);
            SET @TotalDed = ROUND(@TotalDed,2);
            SET @Neto = @TotalPerc + @SubPer - @TotalDed;

            -- Snapshots del comprobante
            SET @AntiguedadSem = DATEDIFF(DAY, @FechaIngreso, @FechaFin) / 7;
            SELECT @SnapTipoJornada = NULL, @SnapPeriodicidad = NULL, @SnapTipoRegimen = NULL, @SnapTipoContrato = NULL,
                   @SnapTipoContratoDesc = NULL, @SnapPuesto = NULL, @SnapDepto = NULL, @SnapRegimenFisc = NULL;
            SELECT TOP 1
                @SnapTipoJornada      = j.Clave,
                @SnapPeriodicidad     = p.Clave,
                @SnapTipoRegimen      = tr.Clave,
                @SnapTipoContrato     = tc.Clave,
                @SnapTipoContratoDesc = tc.Descripcion,
                @SnapPuesto           = pu.Descripcion,
                @SnapDepto            = dep.Descripcion
            FROM dbo.DocContratoEmpleado ec
            LEFT JOIN dbo.CatSatTipoJornada      j   ON j.TipoJornadaId       = ec.TipoJornadaId
            LEFT JOIN dbo.CatSatPeriodicidadPago p   ON p.PeriodicidadPagoId  = ec.PeriodicidadPagoId
            LEFT JOIN dbo.CatSatTipoRegimen      tr  ON tr.TipoRegimenId      = ec.TipoRegimenId
            LEFT JOIN dbo.CatSatTipoContrato     tc  ON tc.TipoContratoId     = ec.TipoContratoId
            LEFT JOIN dbo.CatPuesto           pu  ON pu.PuestoId           = ec.PuestoId
            LEFT JOIN dbo.CatDepartamento     dep ON dep.DepartamentoId    = ec.DepartamentoId
            WHERE ec.EmpleadoId = @EmpleadoId AND ec.Activo = 1
            ORDER BY ec.FechaInicio DESC;

            SELECT @SnapRegimenFisc = rf.Clave
            FROM dbo.CatEmpleadoDatosFiscales f JOIN dbo.CatSatRegimenFiscal rf ON rf.RegimenFiscalId = f.RegimenFiscalSAT WHERE f.EmpleadoId = @EmpleadoId;

            INSERT INTO dbo.DocReciboNomina (
                PeriodoId, EmpleadoId, TotalPercepciones, TotalDeducciones, NetoPagar, TipoNomina,
                TotalGravado, TotalExento, TotalOtrosPagos, AntiguedadSemanas,
                SnapshotFechaInicialPago, SnapshotFechaFinalPago, SnapshotFechaPago, SnapshotDiasPagados,
                SnapshotTipoJornada, SnapshotPeriodicidadPago, SnapshotTipoRegimen,
                SnapshotTipoContrato, SnapshotRegimenFiscalReceptor,
                SnapshotSalarioDiario, SnapshotSDI, SnapshotSBC,
                SnapshotPuesto, SnapshotDepartamento, SnapshotTipoContratoDesc
            )
            VALUES (
                @PeriodoId, @EmpleadoId, @TotalPerc, @TotalDed, @Neto, 'O',
                @TotalGravado, @TotalExento, @SubPer, @AntiguedadSem,
                @FechaInicio, @FechaFin, @FechaPago, @DiasTrab,
                @SnapTipoJornada, @SnapPeriodicidad, @SnapTipoRegimen,
                @SnapTipoContrato, @SnapRegimenFisc,
                @SD, @SDI, @SBC,
                @SnapPuesto, @SnapDepto, @SnapTipoContratoDesc
            );
            SET @ReciboId = SCOPE_IDENTITY();

            -- Detalle del recibo
            INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
            VALUES (@ReciboId, 'P', '001', 'Sueldos, Salarios Rayas y Jornales', @SueldoDias, 0);

            IF @PagoSeptimo > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', '001', 'Séptimo día (proporcional)', @PagoSeptimo, 0);

            IF (@ImpHED + @ImpHET) > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', '019', 'Horas Extra', @GrHE, @ExHE);

            IF @ImpPD > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', '020', 'Prima Dominical', @ImpPD - @ExPD, @ExPD);

            IF @ImpAgui > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', '002', 'Gratificación Anual (Aguinaldo)', @ImpAgui - @ExAgui, @ExAgui);

            IF @ImpPrimaVac > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', '021', 'Prima Vacacional', @ImpPrimaVac - @ExPrimaVac, @ExPrimaVac);

            IF @ImpAntiguedad > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', '022', 'Prima de Antigüedad', @ImpAntiguedad - @ExAntiguedad, @ExAntiguedad);

            IF @ImpIndemniz > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', '025', 'Indemnizaciones', @ImpIndemniz - @ExIndemniz, @ExIndemniz);

            IF @ImpFest > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', CASE WHEN @Anio >= 2026 THEN '055' ELSE '038' END, 'Pago adicional por día festivo laborado (Art. 75 LFT)', @ImpFest - @ExFest, @ExFest);

            IF @Bonos > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', '038', 'Bono o gratificación', @Bonos, 0);

            IF @SubMes >= 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'O', '002', 'Subsidio para el Empleo Efectivamente Entregado', 0, @SubPer);

            IF @ISRPer > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'D', '002', 'ISR Retenido', @ISRPer, 0);

            IF @CuoObr > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'D', '001', 'Seguridad Social (Cuota Obrera IMSS)', @CuoObr, 0);

            IF @DescINFO > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'D', '010', 'Crédito INFONAVIT (Pago por crédito de vivienda)', @DescINFO, 0);

            IF @DescPrest > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'D', '012', 'Anticipo / Préstamo Empresa (Anticipo de salarios)', @DescPrest, 0);

            IF @Descuentos > 0
                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'D', '004', 'Otros descuentos', @Descuentos, 0);

            INSERT dbo.DocReciboDetalle(ReciboId,TipoConcepto,ClaveConceptoSAT,Descripcion,Cantidad,ImporteGravado,ImporteExento,EsIncidencia)
            SELECT @ReciboId,c.TipoConcepto,c.ClaveSAT,
                   LEFT(c.Descripcion,100),i.CantidadOValor,
                   CASE WHEN c.TipoConcepto = 'D' OR c.EsGravado = 1 THEN i.ImporteCalculado ELSE 0 END,
                   CASE WHEN c.TipoConcepto = 'P' AND c.EsGravado = 0 THEN i.ImporteCalculado ELSE 0 END,1
            FROM dbo.ProcIncidencias i
            JOIN dbo.CatConcepto c ON c.ConceptoId = i.IdConceptoSAT AND c.TipoConcepto <> 'I'
            WHERE i.IdPeriodo = @PeriodoId AND i.IdEmpleado = @EmpleadoId AND i.Estatus = 'ACTIVA'
              AND NOT (c.TipoConcepto = 'P' AND c.ClaveSAT IN ('019','020','002','021','022','025'));

            INSERT dbo.DocReciboOtroPago(DetalleId,SubsidioCausado)
            SELECT DetalleId,ROUND(@SubMes / @FactorMes,2)
            FROM dbo.DocReciboDetalle WHERE ReciboId = @ReciboId AND TipoConcepto = 'O' AND ClaveConceptoSAT = '002';
            UPDATE r SET TotalPercepciones = d.Percepciones, TotalDeducciones = d.Deducciones,
                TotalOtrosPagos = d.OtrosPagos, TotalGravado = d.Gravado, TotalExento = d.Exento,
                NetoPagar = d.Percepciones + d.OtrosPagos - d.Deducciones
            FROM dbo.DocReciboNomina r CROSS APPLY (
                SELECT SUM(CASE WHEN TipoConcepto = 'P' THEN ImporteGravado + ImporteExento ELSE 0 END) AS Percepciones,
                       SUM(CASE WHEN TipoConcepto = 'D' THEN ImporteGravado + ImporteExento ELSE 0 END) AS Deducciones,
                       SUM(CASE WHEN TipoConcepto = 'O' THEN ImporteGravado + ImporteExento ELSE 0 END) AS OtrosPagos,
                       SUM(CASE WHEN TipoConcepto = 'P' THEN ImporteGravado ELSE 0 END) AS Gravado,
                       SUM(CASE WHEN TipoConcepto = 'P' THEN ImporteExento ELSE 0 END) AS Exento
                FROM dbo.DocReciboDetalle WHERE ReciboId = r.ReciboId) d
            WHERE r.ReciboId = @ReciboId;
            FETCH NEXT FROM cur INTO @EmpleadoId, @SD, @SDI, @SBC, @FechaIngreso;
        END

        CLOSE cur; DEALLOCATE cur;

        UPDATE rn SET rn.Notas = n.Notas
          FROM dbo.DocReciboNomina rn
         INNER JOIN @NotasPrevias n ON n.EmpleadoId = rn.EmpleadoId
         WHERE rn.PeriodoId = @PeriodoId;

        UPDATE r SET
            SnapshotRFCEmisor = emp.RFC, SnapshotNombreEmisor = emp.RazonSocial,
            SnapshotRegimenFiscalEmisor = rf.Clave, SnapshotRFCReceptor = f.RFC,
            SnapshotNombreReceptor = CONCAT(e.Nombres,N' ',e.ApellidoPaterno,CASE WHEN e.ApellidoMaterno IS NULL THEN N'' ELSE N' ' + e.ApellidoMaterno END),
            SnapshotCURPReceptor = f.CURP, SnapshotCodigoPostalReceptor = f.CodigoPostal,
            SnapshotClaveEntFed = f.ClaveEntidadFederativa, SnapshotNumEmpleado = e.NumeroEmpleado,
            SnapshotNSS = im.NSS, SnapshotFechaIngreso = e.FechaIngreso, SnapshotRegistroPatronal = emp.RegistroPatronal
        FROM dbo.DocReciboNomina r JOIN dbo.CatEmpleado e ON e.EmpleadoId = r.EmpleadoId
        JOIN dbo.CatEmpresa emp ON emp.EmpresaId = e.EmpresaId
        JOIN dbo.CatSatRegimenFiscal rf ON rf.RegimenFiscalId = emp.RegimenFiscalId
        LEFT JOIN dbo.CatEmpleadoDatosFiscales f ON f.EmpleadoId = e.EmpleadoId
        LEFT JOIN dbo.CatEmpleadoDatosIMSS im ON im.EmpleadoId = e.EmpleadoId
        WHERE r.PeriodoId = @PeriodoId;
        UPDATE dbo.ProcPeriodoNomina
           SET Estatus = 'CALCULADO', UsuarioCalculando = NULL, FechaInicioCalculo = NULL
        WHERE PeriodoId = @PeriodoId;

        INSERT INTO dbo.LogCalculo (PeriodoId, Tipo, Mensaje)
        VALUES (@PeriodoId, 'NOMINA', CONCAT('Nómina calculada exitosamente por ', SYSTEM_USER));

        COMMIT;
    END TRY
    BEGIN CATCH
        DECLARE @MensajeError NVARCHAR(2000) = ERROR_MESSAGE();
        IF CURSOR_STATUS('local', 'cur') >= 0 BEGIN CLOSE cur; DEALLOCATE cur; END
        IF @@TRANCOUNT > 0 ROLLBACK;

        -- Ya sin transacción: se libera el periodo y vuelve al estatus con el que se tomó
        UPDATE dbo.ProcPeriodoNomina
           SET Estatus = @EstatusPrevio, UsuarioCalculando = NULL, FechaInicioCalculo = NULL
         WHERE PeriodoId = @PeriodoId AND Estatus = 'EN_CALCULO';

        INSERT INTO dbo.LogCalculo (PeriodoId, Tipo, Mensaje)
        VALUES (@PeriodoId, 'ERROR', CONCAT('Error al calcular nómina: ', @MensajeError));
        THROW;
    END CATCH
END
GO

-- sp_ReversarCalculo (Reabre la prenómina de un periodo CALCULADO: elimina sus recibos para poder corregir incidencias y reprocesar)
CREATE OR ALTER PROCEDURE dbo.sp_ReversarCalculo
    @PeriodoId  INT,
    @UsuarioId  INT = NULL,
    @Motivo     NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM dbo.ProcPeriodoNomina WITH (UPDLOCK,HOLDLOCK) WHERE PeriodoId = @PeriodoId AND Estatus = 'CALCULADO')
    BEGIN
        THROW 51013, 'Solo puede reabrirse un periodo CALCULADO.', 1;
    END

        DELETE n FROM dbo.DocReciboAccionesTitulos n
          INNER JOIN dbo.DocReciboDetalle rd ON rd.DetalleId = n.DetalleId
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = rd.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboHorasExtra n
          INNER JOIN dbo.DocReciboDetalle rd ON rd.DetalleId = n.DetalleId
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = rd.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboOtroPago n
          INNER JOIN dbo.DocReciboDetalle rd ON rd.DetalleId = n.DetalleId
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = rd.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboIncapacidad n
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = n.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboJubilacionPensionRetiro n
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = n.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.DocReciboSeparacionIndemnizacion n
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = n.ReciboId WHERE rn.PeriodoId = @PeriodoId;
        DELETE n FROM dbo.RelReciboSubContratacion n
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = n.ReciboId WHERE rn.PeriodoId = @PeriodoId;

        DELETE rd FROM dbo.DocReciboDetalle rd
          INNER JOIN dbo.DocReciboNomina rn ON rn.ReciboId = rd.ReciboId
         WHERE rn.PeriodoId = @PeriodoId;
        DELETE FROM dbo.DocReciboNomina WHERE PeriodoId = @PeriodoId;

        UPDATE dbo.ProcPeriodoNomina SET Estatus = 'PRENOMINA' WHERE PeriodoId = @PeriodoId;

        INSERT INTO dbo.LogCalculo (PeriodoId, Tipo, Mensaje)
        VALUES (@PeriodoId, 'NOMINA',
                CONCAT('Cálculo reversado por usuario ', ISNULL(CAST(@UsuarioId AS NVARCHAR(20)), SYSTEM_USER),
                       ISNULL(N'. Motivo: ' + @Motivo, '')));
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_GenerarDiasFestivosLey (Genera los días de descanso obligatorio del Art. 74 LFT para un año; idempotente)
-- Fechas: 1-ene; 1er lunes de febrero; 3er lunes de marzo; 1-may; 16-sep; 3er lunes de noviembre;
-- 1-oct cada seis anios desde 2024 para la transmision del Ejecutivo; 25-dic conforme al texto vigente consultado.
-- Las jornadas electorales y demás festivos locales se capturan como festivos de ley o de empresa desde la aplicación.
CREATE OR ALTER PROCEDURE dbo.sp_GenerarDiasFestivosLey
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Anio IS NULL OR @Anio < 2000 OR @Anio > 2100
    BEGIN
        RAISERROR('El año %d no es válido para generar días festivos (2000-2100).', 16, 1, @Anio);
        RETURN;
    END

    DECLARE @Feb DATE = DATEFROMPARTS(@Anio, 2, 1);
    DECLARE @Mar DATE = DATEFROMPARTS(@Anio, 3, 1);
    DECLARE @Nov DATE = DATEFROMPARTS(@Anio, 11, 1);

    -- Primer lunes de cada mes: 1900-01-01 fue lunes, por lo que (DATEDIFF % 7) = 0 identifica a los lunes
    DECLARE @PrimerLunesFeb DATE = DATEADD(DAY, (7 - (DATEDIFF(DAY, '19000101', @Feb) % 7)) % 7, @Feb);
    DECLARE @PrimerLunesMar DATE = DATEADD(DAY, (7 - (DATEDIFF(DAY, '19000101', @Mar) % 7)) % 7, @Mar);
    DECLARE @PrimerLunesNov DATE = DATEADD(DAY, (7 - (DATEDIFF(DAY, '19000101', @Nov) % 7)) % 7, @Nov);

    DECLARE @Festivos TABLE (Fecha DATE NOT NULL, Descripcion NVARCHAR(100) NOT NULL);
    INSERT INTO @Festivos (Fecha, Descripcion) VALUES
        (DATEFROMPARTS(@Anio, 1, 1),  N'Año Nuevo'),
        (@PrimerLunesFeb,             N'Día de la Constitución (primer lunes de febrero)'),
        (DATEADD(DAY, 14, @PrimerLunesMar), N'Natalicio de Benito Juárez (tercer lunes de marzo)'),
        (DATEFROMPARTS(@Anio, 5, 1),  N'Día del Trabajo'),
        (DATEFROMPARTS(@Anio, 9, 16), N'Día de la Independencia'),
        (DATEADD(DAY, 14, @PrimerLunesNov), N'Día de la Revolución (tercer lunes de noviembre)'),
        (DATEFROMPARTS(@Anio, 12, 25), N'Navidad');

    IF (@Anio - 1994) % 6 = 0
        INSERT INTO @Festivos (Fecha, Descripcion)
        VALUES (DATEFROMPARTS(@Anio, CASE WHEN @Anio >= 2024 THEN 10 ELSE 12 END, 1), N'Transmisión del Poder Ejecutivo Federal');

    INSERT INTO dbo.CatDiaFestivo (EmpresaId, Fecha, Descripcion, Tipo, Activo)
    SELECT NULL, f.Fecha, f.Descripcion, 'LEY', 1
    FROM @Festivos f
    WHERE NOT EXISTS (SELECT 1 FROM dbo.CatDiaFestivo x WHERE x.EmpresaId IS NULL AND x.Fecha = f.Fecha);
END
GO

-- sp_VincularFestivosPeriodos (Cruza el catálogo de festivos con los periodos: FechaFestivo BETWEEN FechaInicio AND FechaFin)
-- Parámetros en NULL = sin filtro. Solo se recalculan periodos aún abiertos (CREADO/PRENOMINA) para no alterar nóminas procesadas.
CREATE OR ALTER PROCEDURE dbo.sp_VincularFestivosPeriodos
    @EmpresaId    INT = NULL,
    @EjercicioId  INT = NULL,
    @PeriodicidadPagoId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE rpf
        FROM dbo.RelPeriodoDiaFestivo rpf
        INNER JOIN dbo.ProcPeriodoNomina p ON p.PeriodoId = rpf.PeriodoId
        WHERE p.Estatus IN ('CREADO','PRENOMINA')
          AND (@EmpresaId IS NULL OR p.EmpresaId = @EmpresaId)
          AND (@EjercicioId IS NULL OR p.EjercicioId = @EjercicioId)
          AND (@PeriodicidadPagoId IS NULL OR p.PeriodicidadPagoId = @PeriodicidadPagoId);

        INSERT INTO dbo.RelPeriodoDiaFestivo (PeriodoId, DiaFestivoId)
        SELECT p.PeriodoId, f.DiaFestivoId
        FROM dbo.ProcPeriodoNomina p
        INNER JOIN dbo.CatDiaFestivo f
                ON f.Activo = 1
               AND f.Fecha BETWEEN p.FechaInicio AND p.FechaFin
               AND (f.EmpresaId IS NULL OR f.EmpresaId = p.EmpresaId)
        WHERE p.Estatus IN ('CREADO','PRENOMINA')
          AND (@EmpresaId IS NULL OR p.EmpresaId = @EmpresaId)
          AND (@EjercicioId IS NULL OR p.EjercicioId = @EjercicioId)
          AND (@PeriodicidadPagoId IS NULL OR p.PeriodicidadPagoId = @PeriodicidadPagoId);

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_CalcularAguinaldo (Cálculo de gratificación anual LFT Art. 87)
CREATE OR ALTER PROCEDURE dbo.sp_CalcularAguinaldo
    @EjercicioId INT,
    @PeriodoId   INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @EstatusPer NVARCHAR(20), @EmpresaId INT;
        SELECT @EstatusPer = Estatus, @EmpresaId = EmpresaId FROM dbo.ProcPeriodoNomina WHERE PeriodoId = @PeriodoId;

        IF @EstatusPer IS NULL OR NOT EXISTS
           (SELECT 1 FROM dbo.ProcPeriodoNomina WHERE PeriodoId = @PeriodoId AND EjercicioId = @EjercicioId)
            THROW 51012, 'Periodo inexistente o ejercicio inconsistente.', 1;
        IF @EstatusPer IN ('AUTORIZADO','CERRADO','TIMBRADO')
        BEGIN
            RAISERROR('Período %d está en estatus [%s]. No se puede modificar.', 16, 1, @PeriodoId, @EstatusPer);
            ROLLBACK; RETURN;
        END

        DECLARE @Anio INT;
        SELECT @Anio = Anio FROM dbo.CatEjercicioFiscal WHERE EjercicioId = @EjercicioId;

        DECLARE @DiasAnio INT = CASE WHEN (@Anio % 4 = 0 AND @Anio % 100 <> 0) OR (@Anio % 400 = 0) THEN 366 ELSE 365 END;
        DECLARE @UMADia DECIMAL(12,4) = dbo.fn_ObtenerUMADiaria(@Anio);
        IF @UMADia <= 0 THROW 51010, 'Falta UMA vigente.', 1;
        DECLARE @Exencion DECIMAL(18,4) = @UMADia * 30.0;
        DECLARE @DiasMin DECIMAL(10,4) = 15.0;

        DECLARE @EmpleadoId INT, @SD DECIMAL(18,4), @FechaIngreso DATE;

        DECLARE curA CURSOR LOCAL FAST_FORWARD FOR
            SELECT e.EmpleadoId, d.SalarioDiario, e.FechaIngreso
            FROM dbo.CatEmpleado e
            INNER JOIN dbo.CatEmpleadoDatosIMSS d ON d.EmpleadoId = e.EmpleadoId
            WHERE e.EmpresaId = @EmpresaId AND e.Estatus = 'ACTIVO' AND e.FechaIngreso <= DATEFROMPARTS(@Anio,12,31);

        OPEN curA;
        FETCH NEXT FROM curA INTO @EmpleadoId, @SD, @FechaIngreso;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @Inicio DATE = CASE WHEN @FechaIngreso > DATEFROMPARTS(@Anio,1,1) THEN @FechaIngreso ELSE DATEFROMPARTS(@Anio,1,1) END;
            DECLARE @DiasLabAnio INT = DATEDIFF(DAY, @Inicio, DATEFROMPARTS(@Anio,12,31)) + 1;
            IF @DiasLabAnio < 1 SET @DiasLabAnio = 1;

            DECLARE @DiasAgui DECIMAL(10,4) = @DiasMin * (CAST(@DiasLabAnio AS DECIMAL(10,4)) / @DiasAnio);
            DECLARE @Monto DECIMAL(18,4) = ROUND(@SD * @DiasAgui, 2);
            DECLARE @Gravado DECIMAL(18,4) = CASE WHEN @Monto > @Exencion THEN @Monto - @Exencion ELSE 0 END;
            DECLARE @Exento  DECIMAL(18,4) = @Monto - @Gravado;

            IF EXISTS (SELECT 1 FROM dbo.ProcAguinaldo WHERE EmpleadoId=@EmpleadoId AND EjercicioId=@EjercicioId)
                UPDATE dbo.ProcAguinaldo SET DiasProporcionales=@DiasAgui, MontoPagar=@Monto
                WHERE EmpleadoId=@EmpleadoId AND EjercicioId=@EjercicioId;
            ELSE
                INSERT INTO dbo.ProcAguinaldo (EmpleadoId, EjercicioId, DiasProporcionales, MontoPagar)
                VALUES (@EmpleadoId, @EjercicioId, @DiasAgui, @Monto);

            DECLARE @ReciboId INT;
            SET @ReciboId = NULL;
            SELECT @ReciboId = ReciboId FROM dbo.DocReciboNomina
            WHERE PeriodoId = @PeriodoId AND EmpleadoId = @EmpleadoId;

            IF @ReciboId IS NOT NULL
            BEGIN
                DECLARE @PrevGravado DECIMAL(18,2), @PrevExento DECIMAL(18,2);
                SELECT @PrevGravado = ISNULL(SUM(ImporteGravado),0), @PrevExento = ISNULL(SUM(ImporteExento),0)
                FROM dbo.DocReciboDetalle WHERE ReciboId = @ReciboId AND ClaveConceptoSAT = '002' AND TipoConcepto = 'P';
                -- Evitar duplicados eliminando previo concepto 002 de aguinaldo si existiera
                DELETE FROM dbo.DocReciboDetalle WHERE ReciboId = @ReciboId AND ClaveConceptoSAT = '002' AND TipoConcepto = 'P';

                INSERT INTO dbo.DocReciboDetalle (ReciboId, TipoConcepto, ClaveConceptoSAT, Descripcion, ImporteGravado, ImporteExento)
                VALUES (@ReciboId, 'P', '002', CONCAT('Gratificación Anual (Aguinaldo ', @Anio, ')'), @Gravado, @Exento);

                UPDATE dbo.DocReciboNomina SET
                    TotalPercepciones = TotalPercepciones - @PrevGravado - @PrevExento + @Monto,
                    NetoPagar         = NetoPagar - @PrevGravado - @PrevExento + @Monto,
                    TotalGravado      = ISNULL(TotalGravado, 0) - @PrevGravado + @Gravado,
                    TotalExento       = ISNULL(TotalExento, 0) - @PrevExento + @Exento
                WHERE ReciboId = @ReciboId;
            END

            FETCH NEXT FROM curA INTO @EmpleadoId, @SD, @FechaIngreso;
        END
        CLOSE curA; DEALLOCATE curA;

        INSERT INTO dbo.LogCalculo (PeriodoId, Tipo, Mensaje)
        VALUES (@PeriodoId, 'AGUINALDO', CONCAT('Aguinaldo ejercicio ', @Anio, ' calculado correctamente.'));

        COMMIT;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('LOCAL','curA') >= 0 BEGIN CLOSE curA; DEALLOCATE curA; END
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_AutorizarPeriodo (Validación y pase a aprobación de finanzas)
CREATE OR ALTER PROCEDURE dbo.sp_AutorizarPeriodo
    @PeriodoId INT, @UsuarioId INT = NULL, @Comentario NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.ProcPeriodoNomina WITH (UPDLOCK)
        SET Estatus = 'AUTORIZADO' WHERE PeriodoId = @PeriodoId AND Estatus = 'CALCULADO';
        IF @@ROWCOUNT <> 1 THROW 51013, 'El periodo no esta CALCULADO.', 1;
        UPDATE dbo.DocReciboNomina SET Estatus = 'AUTORIZADO'
        WHERE PeriodoId = @PeriodoId AND Estatus = 'CALCULADO';
        INSERT dbo.LogCalculo(PeriodoId,Tipo,Mensaje)
        VALUES (@PeriodoId,'NOMINA',CONCAT('Autorizado por ',@UsuarioId,N'. ',@Comentario));
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_CerrarPeriodo (Cierre formal, impacto de amortizaciones, acumulados y generación de CFDIs)
CREATE OR ALTER PROCEDURE dbo.sp_CerrarPeriodo
    @PeriodoId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM dbo.ProcPeriodoNomina WITH (UPDLOCK,HOLDLOCK) WHERE PeriodoId=@PeriodoId AND Estatus='AUTORIZADO')
    BEGIN
        THROW 51013, 'El periodo no esta AUTORIZADO.', 1;
    END

    DECLARE @EjercicioId INT, @Mes INT, @FechaFin DATE;
    SELECT @EjercicioId = EjercicioId, @FechaFin = FechaFin FROM dbo.ProcPeriodoNomina WHERE PeriodoId = @PeriodoId;
    SET @Mes = (SELECT MONTH(FechaPago) FROM dbo.ProcPeriodoNomina WHERE PeriodoId = @PeriodoId);

        UPDATE dbo.ProcPeriodoNomina SET Estatus = 'CERRADO' WHERE PeriodoId = @PeriodoId;

        IF EXISTS (
            SELECT 1 FROM dbo.DocReciboNomina rn
            OUTER APPLY (SELECT SUM(CASE WHEN SaldoRestante >= DescuentoPorPeriodo THEN DescuentoPorPeriodo ELSE SaldoRestante END) AS Cuota
                         FROM dbo.ProcPrestamo WHERE EmpleadoId = rn.EmpleadoId AND Estatus = 'ACTIVO') p
            OUTER APPLY (SELECT SUM(ImporteGravado + ImporteExento) AS Importe
                         FROM dbo.DocReciboDetalle WHERE ReciboId = rn.ReciboId AND ClaveConceptoSAT = '012'
                           AND TipoConcepto = 'D' AND EsIncidencia = 0) d
            WHERE rn.PeriodoId = @PeriodoId AND ISNULL(p.Cuota,0) <> ISNULL(d.Importe,0))
            THROW 51014, 'Los saldos de prestamo cambiaron despues del calculo. Recalcule antes de cerrar.', 1;
        -- 1. Aplicar amortización definitiva de préstamos con descuento en este periodo
        UPDATE p
           SET p.SaldoRestante = CASE WHEN p.SaldoRestante - CASE WHEN p.SaldoRestante >= p.DescuentoPorPeriodo THEN p.DescuentoPorPeriodo ELSE p.SaldoRestante END < 0 THEN 0 ELSE p.SaldoRestante - CASE WHEN p.SaldoRestante >= p.DescuentoPorPeriodo THEN p.DescuentoPorPeriodo ELSE p.SaldoRestante END END,
               p.Estatus = CASE WHEN p.SaldoRestante - CASE WHEN p.SaldoRestante >= p.DescuentoPorPeriodo THEN p.DescuentoPorPeriodo ELSE p.SaldoRestante END <= 0 THEN 'LIQUIDADO' ELSE 'ACTIVO' END
        FROM dbo.ProcPrestamo p
        INNER JOIN dbo.DocReciboNomina rn ON rn.EmpleadoId = p.EmpleadoId AND rn.PeriodoId = @PeriodoId
        INNER JOIN (SELECT ReciboId, SUM(ImporteGravado + ImporteExento) AS Importe
                    FROM dbo.DocReciboDetalle
                    WHERE ClaveConceptoSAT = '012' AND TipoConcepto = 'D' AND EsIncidencia = 0
                    GROUP BY ReciboId) rd ON rd.ReciboId = rn.ReciboId
        WHERE p.Estatus = 'ACTIVO';

        -- 2. Consolidar acumulados mensuales fiscales
        MERGE dbo.ProcAcumuladoMensual WITH (HOLDLOCK) AS TARGET
        USING (
            SELECT
                rn.EmpleadoId,
                @EjercicioId AS EjercicioId,
                @Mes AS Mes,
                SUM(ISNULL(rn.TotalGravado, 0)) AS SumaGravado,
                SUM(ISNULL(rn.TotalExento, 0)) AS SumaExento,
                SUM(ISNULL(rn.SnapshotSBC * rn.SnapshotDiasPagados, 0)) AS SumaBaseIMSS,
                SUM(ISNULL(detalle.Subsidio, 0)) AS SumaSubsidio,
                SUM(ISNULL(detalle.ISR, 0)) AS SumaISR,
                SUM(ISNULL(rn.TotalPercepciones, 0)) AS SumaPerc,
                SUM(ISNULL(rn.TotalDeducciones, 0)) AS SumaDed,
                SUM(ISNULL(detalle.IMSS, 0)) AS SumaCuotaObrera
            FROM dbo.DocReciboNomina rn
            OUTER APPLY (
                SELECT SUM(CASE WHEN d.ClaveConceptoSAT='002' AND d.TipoConcepto='D' THEN d.ImporteGravado ELSE 0 END) AS ISR,
                       SUM(CASE WHEN d.ClaveConceptoSAT='002' AND d.TipoConcepto='O' THEN d.ImporteExento ELSE 0 END) AS Subsidio,
                       SUM(CASE WHEN d.ClaveConceptoSAT='001' AND d.TipoConcepto='D' THEN d.ImporteGravado ELSE 0 END) AS IMSS
                FROM dbo.DocReciboDetalle d WHERE d.ReciboId=rn.ReciboId
            ) detalle
            WHERE rn.PeriodoId = @PeriodoId
            GROUP BY rn.EmpleadoId
        ) AS SOURCE
        ON TARGET.EmpleadoId = SOURCE.EmpleadoId AND TARGET.EjercicioId = SOURCE.EjercicioId AND TARGET.Mes = SOURCE.Mes
        WHEN MATCHED THEN
            UPDATE SET
                TARGET.BaseGravadaISR    = TARGET.BaseGravadaISR + SOURCE.SumaGravado,
                TARGET.BaseExentaISR     = TARGET.BaseExentaISR + SOURCE.SumaExento,
                TARGET.BaseIMSS          = TARGET.BaseIMSS + SOURCE.SumaBaseIMSS,
                TARGET.SubsidioEntregado = TARGET.SubsidioEntregado + SOURCE.SumaSubsidio,
                TARGET.ISRRetenido       = TARGET.ISRRetenido + SOURCE.SumaISR,
                TARGET.TotalPercepciones = TARGET.TotalPercepciones + SOURCE.SumaPerc,
                TARGET.TotalDeducciones  = TARGET.TotalDeducciones + SOURCE.SumaDed,
                TARGET.CuotaObrera       = TARGET.CuotaObrera + SOURCE.SumaCuotaObrera
        WHEN NOT MATCHED THEN
            INSERT (EmpleadoId, EjercicioId, Mes, BaseGravadaISR, BaseExentaISR, BaseIMSS, SubsidioEntregado, ISRRetenido, TotalPercepciones, TotalDeducciones, CuotaObrera)
            VALUES (SOURCE.EmpleadoId, SOURCE.EjercicioId, SOURCE.Mes, SOURCE.SumaGravado, SOURCE.SumaExento, SOURCE.SumaBaseIMSS, SOURCE.SumaSubsidio, SOURCE.SumaISR, SOURCE.SumaPerc, SOURCE.SumaDed, SOURCE.SumaCuotaObrera);

        -- 3. Crear registros de CFDI pendientes de timbrado si no existen
        INSERT INTO dbo.DocCFDINomina (EmpresaId, ReciboId, SyncNubeEstatus, EstadoCFDI)
        SELECT e.EmpresaId, rn.ReciboId, 'PENDIENTE', 1
        FROM dbo.DocReciboNomina rn
        JOIN dbo.CatEmpleado e ON e.EmpleadoId = rn.EmpleadoId
        WHERE rn.PeriodoId = @PeriodoId
          AND NOT EXISTS (SELECT 1 FROM dbo.DocCFDINomina WHERE ReciboId = rn.ReciboId);

        INSERT INTO dbo.LogCalculo (PeriodoId, Tipo, Mensaje)
        VALUES (@PeriodoId, 'NOMINA', 'Período cerrado exitosamente. Acumulados consolidados y CFDIs en espera de timbrado.');

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_CalcularFiniquito (Cálculo conforme a LFT y LISR con corrección de topes y gravados)
CREATE OR ALTER PROCEDURE dbo.sp_CalcularFiniquito
    @EmpleadoId  INT,
    @BajaId      INT,
    @FiniquitoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @FechaBaja DATE, @TipoBaja NVARCHAR(30), @FechaIngreso DATE, @SD DECIMAL(18,4);
        SELECT @FechaBaja = FechaBaja, @TipoBaja = TipoBaja
        FROM dbo.DocEmpleadoBaja WHERE BajaId = @BajaId AND EmpleadoId = @EmpleadoId;

        IF @FechaBaja IS NULL
        BEGIN RAISERROR('Baja %d no encontrada para empleado %d.', 16, 1, @BajaId, @EmpleadoId); ROLLBACK; RETURN; END

        SELECT @FechaIngreso = FechaIngreso FROM dbo.CatEmpleado WHERE EmpleadoId = @EmpleadoId;
        SET @SD = dbo.fn_ObtenerSalarioVigente(@EmpleadoId, @FechaBaja);
        IF @SD <= 0
        BEGIN RAISERROR('No se pudo determinar salario diario del empleado %d.', 16, 1, @EmpleadoId); ROLLBACK; RETURN; END

        DECLARE @Anio INT = YEAR(@FechaBaja);
        DECLARE @EjercicioId INT;
        SELECT @EjercicioId = EjercicioId         FROM dbo.CatEjercicioFiscal WHERE Anio = @Anio;
        IF @EjercicioId IS NULL
        BEGIN RAISERROR('No hay ejercicio fiscal abierto para el año %d.', 16, 1, @Anio); ROLLBACK; RETURN; END

        DECLARE @UMADia DECIMAL(12,4) = dbo.fn_ObtenerUMADiaria(@Anio);
        IF @UMADia IS NULL OR @UMADia <= 0 THROW 51010, 'Falta UMA vigente.', 1;
        DECLARE @SMGDia DECIMAL(12,4) = dbo.fn_ObtenerSMGDiario(@Anio, 'GENERAL');
        IF @SMGDia IS NULL OR @SMGDia <= 0 THROW 51010, 'Falta salario minimo vigente.', 1;
        DECLARE @Anios  DECIMAL(10,4) = dbo.fn_CalcularAntiguedadAnios(@FechaIngreso, @FechaBaja);
        DECLARE @AniosEnt INT = dbo.fn_AniosCumplidos(@FechaIngreso,@FechaBaja);

        DECLARE @DiasAnio INT = CASE WHEN (@Anio%4=0 AND @Anio%100<>0) OR @Anio%400=0 THEN 366 ELSE 365 END;
        DECLARE @Inicio DATE = CASE WHEN @FechaIngreso > DATEFROMPARTS(@Anio,1,1) THEN @FechaIngreso ELSE DATEFROMPARTS(@Anio,1,1) END;
        DECLARE @DiasTrabAnio INT = DATEDIFF(DAY, @Inicio, @FechaBaja) + 1;

        -- Proporcionales de ley
        DECLARE @DiasVacFull DECIMAL(10,4) = dbo.fn_DiasVacaciones(CASE WHEN @AniosEnt < 1 THEN 1 ELSE @AniosEnt END);
        DECLARE @DiasVacProp DECIMAL(10,4) = ROUND(@DiasVacFull * @DiasTrabAnio / @DiasAnio, 4);
        DECLARE @DiasAguiProp DECIMAL(10,4) = ROUND(15.0 * @DiasTrabAnio / @DiasAnio, 4);

        DECLARE @Aguinaldo DECIMAL(18,4) = ROUND(@SD * @DiasAguiProp, 4);
        DECLARE @Vacaciones DECIMAL(18,4) = ROUND(@SD * @DiasVacProp, 4);
        DECLARE @PrimaVac   DECIMAL(18,4) = ROUND(@Vacaciones * 0.25, 4);

        -- Indemnizaciones y Prima de Antigüedad
        -- LFT Art. 162 y 486: El tope de la prima de antigüedad es 2 Salarios Mínimos Generales (SMG)
        DECLARE @Indem90 DECIMAL(18,4) = 0, @Indem20 DECIMAL(18,4) = 0, @PrimaAntig DECIMAL(18,4) = 0;
        DECLARE @SDTopeSMG DECIMAL(18,4) = CASE WHEN @SD > (@SMGDia * 2.0) THEN (@SMGDia * 2.0) ELSE @SD END;

        IF @TipoBaja IN ('INJUSTIFICADA','RESCISION_PATRON')
        BEGIN
            SET @Indem90    = ROUND(@SD * 90.0, 4);
            SET @Indem20    = ROUND(@SD * 20.0 * @Anios, 4);
            SET @PrimaAntig = ROUND(@SDTopeSMG * 12.0 * @Anios, 4);
        END
        ELSE IF @TipoBaja = 'VOLUNTARIA' AND @AniosEnt >= 15
            SET @PrimaAntig = ROUND(@SDTopeSMG * 12.0 * @Anios, 4);
        ELSE IF @TipoBaja IN ('DEFUNCION','PENSION')
            SET @PrimaAntig = ROUND(@SDTopeSMG * 12.0 * @Anios, 4);

        DECLARE @TotalBruto DECIMAL(18,4) = @Aguinaldo + @Vacaciones + @PrimaVac + @Indem90 + @Indem20 + @PrimaAntig;

        -- Exenciones fiscales (LISR Art. 93)
        DECLARE @ExenAgui DECIMAL(18,4) = dbo.fn_ExencionAguinaldo(@Aguinaldo, @Anio);
        DECLARE @ExenPV   DECIMAL(18,4) = dbo.fn_ExencionPrimaVacacional(@PrimaVac, @Anio);
        DECLARE @AniosExencion INT = @AniosEnt
            + CASE WHEN DATEADD(MONTH,6,DATEADD(YEAR,@AniosEnt,@FechaIngreso)) < @FechaBaja THEN 1 ELSE 0 END;
        DECLARE @ExenCompartida DECIMAL(18,4) = dbo.fn_ExencionSeparacion(@PrimaAntig + @Indem90 + @Indem20,@AniosExencion,@Anio);
        DECLARE @ExenPA DECIMAL(18,4) = CASE WHEN @PrimaAntig < @ExenCompartida THEN @PrimaAntig ELSE @ExenCompartida END;
        DECLARE @ExenIndem DECIMAL(18,4) = @ExenCompartida - @ExenPA;

        -- Vacaciones proporcionales son 100% gravadas bajo LISR
        DECLARE @Gravado DECIMAL(18,4) =
              (@Aguinaldo - @ExenAgui)
            + @Vacaciones
            + (@PrimaVac - @ExenPV)
            + (@Indem90 + @Indem20 - @ExenIndem)
            + (@PrimaAntig - @ExenPA);
        IF @Gravado < 0 SET @Gravado = 0;

        DECLARE @ISR DECIMAL(18,4) = dbo.fn_CalcularISRMensual(@Gravado, @EjercicioId);
        IF @ISR IS NULL THROW 51010, 'Falta tarifa ISR para el finiquito.', 1;
        DECLARE @Neto DECIMAL(18,4) = @TotalBruto - @ISR;

        INSERT INTO dbo.DocFiniquito (BajaId, EmpleadoId, TotalBruto, ISRFiniquito, TotalNeto)
        VALUES (@BajaId, @EmpleadoId, @TotalBruto, @ISR, @Neto);
        SET @FiniquitoId = SCOPE_IDENTITY();

        INSERT INTO dbo.DocFiniquitoDetalle (FiniquitoId, Concepto, DiasBase, SalarioBase, ImporteBruto, ImporteExento, ImporteGravado)
        VALUES
          (@FiniquitoId, 'Aguinaldo proporcional',    @DiasAguiProp, @SD, @Aguinaldo,  @ExenAgui, @Aguinaldo - @ExenAgui),
          (@FiniquitoId, 'Vacaciones proporcionales', @DiasVacProp,  @SD, @Vacaciones, 0,         @Vacaciones),
          (@FiniquitoId, 'Prima vacacional (25%)',    @DiasVacProp,  @SD, @PrimaVac,   @ExenPV,   @PrimaVac - @ExenPV);

        DECLARE @Exen90 DECIMAL(18,4) = CASE WHEN @Indem90 < @ExenIndem THEN @Indem90 ELSE @ExenIndem END;
        IF @Indem90 > 0
            INSERT INTO dbo.DocFiniquitoDetalle (FiniquitoId, Concepto, DiasBase, SalarioBase, ImporteBruto, ImporteExento, ImporteGravado)
            VALUES (@FiniquitoId, 'Indemnización constitucional 3 meses (Art. 50 LFT)', 90, @SD, @Indem90, @Exen90, @Indem90 - @Exen90);
        IF @Indem20 > 0
            INSERT INTO dbo.DocFiniquitoDetalle (FiniquitoId, Concepto, DiasBase, SalarioBase, ImporteBruto, ImporteExento, ImporteGravado)
            VALUES (@FiniquitoId, 'Indemnización 20 días por año (Art. 50 LFT)', ROUND(20*@Anios,2), @SD, @Indem20, @ExenIndem - @Exen90, @Indem20 - (@ExenIndem - @Exen90));
        IF @PrimaAntig > 0
            INSERT INTO dbo.DocFiniquitoDetalle (FiniquitoId, Concepto, DiasBase, SalarioBase, ImporteBruto, ImporteExento, ImporteGravado)
            VALUES (@FiniquitoId, 'Prima de antigüedad (Art. 162 LFT - tope 2 SMG)', ROUND(12*@Anios,2), @SDTopeSMG, @PrimaAntig, @ExenPA, @PrimaAntig - @ExenPA);

        UPDATE dbo.CatEmpleado SET Estatus = 'BAJA' WHERE EmpleadoId = @EmpleadoId;

        INSERT INTO dbo.LogCalculo (EmpleadoId, Tipo, Mensaje)
        VALUES (@EmpleadoId, 'FINIQUITO',
                CONCAT('Finiquito calculado: Tipo=', @TipoBaja, ' Bruto=$', FORMAT(@TotalBruto,'N2'),
                       ' ISR=$', FORMAT(@ISR,'N2'), ' Neto=$', FORMAT(@Neto,'N2')));

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_CalcularPTU (Reparto de Utilidades LFT)
CREATE OR ALTER PROCEDURE dbo.sp_CalcularPTU
    @EjercicioId INT, @UtilidadRepartible DECIMAL(18,4), @EmpresaId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @UtilidadRepartible IS NULL OR @UtilidadRepartible < 0
        THROW 51040, 'La utilidad repartible no puede ser negativa.', 1;
    IF @EmpresaId IS NULL
    BEGIN
        IF (SELECT COUNT(DISTINCT e.EmpresaId) FROM dbo.ProcBasePTU b
            JOIN dbo.CatEmpleado e ON e.EmpleadoId = b.EmpleadoId WHERE b.EjercicioId = @EjercicioId) <> 1
            THROW 51040, 'Seleccione una empresa y cargue sus bases PTU validadas.', 1;
        SELECT @EmpresaId = MIN(e.EmpresaId) FROM dbo.ProcBasePTU b
        JOIN dbo.CatEmpleado e ON e.EmpleadoId = b.EmpleadoId WHERE b.EjercicioId = @EjercicioId;
    END
    IF NOT EXISTS (SELECT 1 FROM dbo.ProcBasePTU b JOIN dbo.CatEmpleado e ON e.EmpleadoId = b.EmpleadoId
                   WHERE b.EjercicioId = @EjercicioId AND e.EmpresaId = @EmpresaId AND b.Elegible = 1 AND b.Validado = 1)
       OR EXISTS (SELECT 1 FROM dbo.ProcBasePTU b JOIN dbo.CatEmpleado e ON e.EmpleadoId = b.EmpleadoId
                  WHERE b.EjercicioId = @EjercicioId AND e.EmpresaId = @EmpresaId AND b.Validado = 0)
        THROW 51040, 'PTU requiere dias reales, salario computable, elegibilidad y tope legal validados por empleado.', 1;
    DECLARE @Anio INT = (SELECT Anio FROM dbo.CatEjercicioFiscal WHERE EjercicioId = @EjercicioId);
    DECLARE @UMA DECIMAL(12,4) = dbo.fn_ObtenerUMADiaria(@Anio);
    IF @UMA IS NULL THROW 51040, 'Falta UMA del ejercicio para PTU.', 1;
    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE p FROM dbo.ProcPTU p JOIN dbo.CatEmpleado e ON e.EmpleadoId = p.EmpleadoId
        WHERE p.EjercicioId = @EjercicioId AND e.EmpresaId = @EmpresaId;
        ;WITH Base AS (
            SELECT b.* FROM dbo.ProcBasePTU b JOIN dbo.CatEmpleado e ON e.EmpleadoId = b.EmpleadoId
            WHERE b.EjercicioId = @EjercicioId AND e.EmpresaId = @EmpresaId AND b.Elegible = 1 AND b.Validado = 1
        ), Totales AS (
            SELECT SUM(CONVERT(BIGINT,DiasComputables)) AS Dias, SUM(SalarioBaseReparto) AS Salarios FROM Base
        ), Reparto AS (
            SELECT b.*,
                ROUND(@UtilidadRepartible / 2 * b.DiasComputables / NULLIF(t.Dias,0),4) AS PorDias,
                ROUND(@UtilidadRepartible / 2 * b.SalarioBaseReparto / NULLIF(t.Salarios,0),4) AS PorSalario
            FROM Base b CROSS JOIN Totales t
        )
        INSERT dbo.ProcPTU(EmpleadoId,EjercicioId,DiasTrabajadosAnio,SalarioBaseAnual,
                           MontoPorDias,MontoPorSalario,MontoTotal,MontoExento,MontoGravado)
        SELECT EmpleadoId,EjercicioId,DiasComputables,SalarioBaseReparto,PorDias,PorSalario,
               CASE WHEN PorDias + PorSalario > TopeIndividual THEN TopeIndividual ELSE PorDias + PorSalario END,0,0
        FROM Reparto;
        UPDATE p SET MontoExento = CASE WHEN MontoTotal > 15*@UMA THEN 15*@UMA ELSE MontoTotal END,
                     MontoGravado = CASE WHEN MontoTotal > 15*@UMA THEN MontoTotal-15*@UMA ELSE 0 END
        FROM dbo.ProcPTU p JOIN dbo.CatEmpleado e ON e.EmpleadoId = p.EmpleadoId
        WHERE p.EjercicioId = @EjercicioId AND e.EmpresaId = @EmpresaId;
        INSERT dbo.LogCalculo(Tipo,Mensaje) VALUES ('PTU',CONCAT('PTU con bases validadas. Empresa=',@EmpresaId,N' Ejercicio=',@EjercicioId));
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_CalcularAjusteAnualISR (Determinación anual de diferencias SAT vs Trabajador)
CREATE OR ALTER PROCEDURE dbo.sp_CalcularAjusteAnualISR
    @EjercicioId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Anio INT;
        SELECT @Anio = Anio FROM dbo.CatEjercicioFiscal WHERE EjercicioId = @EjercicioId;
        IF @Anio IS NULL BEGIN RAISERROR('Ejercicio %d no existe.', 16, 1, @EjercicioId); ROLLBACK; RETURN; END

        IF NOT EXISTS (SELECT 1 FROM dbo.CatTablaISRAnual WHERE EjercicioId = @EjercicioId)
            THROW 51010, 'Importe la tarifa anual del articulo 152; no se obtiene multiplicando la mensual.', 1;
        DELETE FROM dbo.ProcAjusteAnualISR WHERE EjercicioId = @EjercicioId AND Aplicado = 0;

        ;WITH Base AS (
            SELECT EmpleadoId,
                   SUM(BaseGravadaISR)    AS BaseAnual,
                   SUM(ISRRetenido)       AS ISRRet,
                   SUM(SubsidioEntregado) AS SubEnt
            FROM dbo.ProcAcumuladoMensual am
            WHERE NOT EXISTS (SELECT 1 FROM dbo.ProcAjusteAnualISR a WHERE a.EmpleadoId = am.EmpleadoId AND a.EjercicioId = am.EjercicioId AND a.Aplicado = 1)
              AND EjercicioId = @EjercicioId
            GROUP BY EmpleadoId
        ), Calc AS (
            SELECT b.*, dbo.fn_CalcularISRAnual(b.BaseAnual, @EjercicioId) AS ISRDet FROM Base b
        )
        INSERT INTO dbo.ProcAjusteAnualISR (EmpleadoId, EjercicioId, BaseGravadaAnual, ISRDeterminado,
                                            ISRRetenidoEnAnio, SubsidioEntregado, DiferenciaFavorSAT, DiferenciaFavorTrab)
        SELECT
            EmpleadoId, @EjercicioId, BaseAnual, ISRDet, ISRRet, SubEnt,
            CASE WHEN ISRDet > ISRRet THEN ISRDet - ISRRet ELSE 0 END,
            CASE WHEN ISRDet < ISRRet THEN ISRRet - ISRDet ELSE 0 END
        FROM Calc;

        INSERT INTO dbo.LogCalculo (Tipo, Mensaje)
        VALUES ('AJUSTE_ANUAL', CONCAT('Ajuste anual ISR ejercicio ', @Anio, ' calculado exitosamente.'));

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_DevengarVacacionesAniversario (Generación de saldo vacacional anual Art. 76 LFT)
CREATE OR ALTER PROCEDURE dbo.sp_DevengarVacacionesAniversario
    @FechaCorte DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @FechaCorte IS NULL SET @FechaCorte = CAST(GETDATE() AS DATE);

    DECLARE @EmpId INT, @FIng DATE, @Anios INT, @FAniv DATE, @Dias INT;
    DECLARE @Insertados INT = 0;

    DECLARE curV CURSOR LOCAL FAST_FORWARD FOR
        SELECT EmpleadoId, FechaIngreso FROM dbo.CatEmpleado WHERE Estatus = 'ACTIVO';
    OPEN curV;
    FETCH NEXT FROM curV INTO @EmpId, @FIng;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @Anios = dbo.fn_AniosCumplidos(@FIng, @FechaCorte);
        IF @Anios >= 1
        BEGIN
            SET @FAniv = DATEADD(YEAR, @Anios, @FIng);
            IF @FAniv <= @FechaCorte
               AND NOT EXISTS (SELECT 1 FROM dbo.ProcVacacionesSaldo
                               WHERE EmpleadoId=@EmpId AND AnioAniversario=@Anios)
            BEGIN
                SET @Dias = dbo.fn_DiasVacaciones(@Anios);
                INSERT INTO dbo.ProcVacacionesSaldo (EmpleadoId, AnioAniversario, FechaAniversario, DiasDevengados, FechaExpiracion)
                VALUES (@EmpId, @Anios, @FAniv, @Dias, DATEADD(YEAR, 1, @FAniv));
                SET @Insertados = @Insertados + 1;
            END
        END
        FETCH NEXT FROM curV INTO @EmpId, @FIng;
    END
    CLOSE curV; DEALLOCATE curV;

    INSERT INTO dbo.LogCalculo (Tipo, Mensaje)
    VALUES ('VACACIONES', CONCAT('Devengados ', @Insertados, ' saldos vacacionales al ', CONVERT(NVARCHAR(10),@FechaCorte,120)));
END
GO

-- sp_TimbrarCFDI (Actualización de respuesta del PAC tras timbrado exitoso)
CREATE OR ALTER PROCEDURE dbo.sp_TimbrarCFDI
    @CFDIId              INT,
    @UUID                UNIQUEIDENTIFIER,
    @SelloDigital        NVARCHAR(MAX),
    @SelloSAT            NVARCHAR(MAX),
    @CadenaOriginalSAT   NVARCHAR(MAX),
    @NoCertificadoSAT    NVARCHAR(20),
    @NoCertificadoEmisor NVARCHAR(20) = NULL,
    @FechaTimbrado       DATETIME,
    @XMLTimbrado         NVARCHAR(MAX),
    @RfcProvCertif       NVARCHAR(13) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @EstAnt INT, @ReciboId INT;
        SELECT @EstAnt = EstadoCFDI, @ReciboId = ReciboId FROM dbo.DocCFDINomina WITH (UPDLOCK,HOLDLOCK) WHERE CFDIId = @CFDIId;

        IF @EstAnt IS NULL
        BEGIN RAISERROR('CFDI %d no existe.', 16, 1, @CFDIId); ROLLBACK; RETURN; END
        IF @EstAnt NOT IN (1, 5)
        BEGIN RAISERROR('CFDI %d no está pendiente de timbrar (estado actual=%d).', 16, 1, @CFDIId, @EstAnt); ROLLBACK; RETURN; END

        IF @UUID IS NULL OR @FechaTimbrado IS NULL OR NULLIF(@SelloDigital,N'') IS NULL
           OR NULLIF(@SelloSAT,N'') IS NULL OR TRY_CONVERT(XML,@XMLTimbrado) IS NULL
            THROW 51015, 'Respuesta PAC incompleta o XML invalido.', 1;
        EXEC dbo.sp_ValidarReciboTimbrado @ReciboId = @ReciboId;
        DECLARE @XML XML = CONVERT(XML,@XMLTimbrado);
        DECLARE @UUIDXML NVARCHAR(36) = @XML.value('declare namespace tfd="http://www.sat.gob.mx/TimbreFiscalDigital"; (//tfd:TimbreFiscalDigital/@UUID)[1]','nvarchar(36)');
        IF TRY_CONVERT(UNIQUEIDENTIFIER,@UUIDXML) IS NULL OR TRY_CONVERT(UNIQUEIDENTIFIER,@UUIDXML) <> @UUID
            THROW 51015, 'El UUID de la respuesta PAC no corresponde al timbre del XML.', 1;
        UPDATE dbo.DocCFDINomina SET
            UUID                  = @UUID,
            SelloDigital          = @SelloDigital,
            SelloSAT              = @SelloSAT,
            CadenaOriginalSAT     = @CadenaOriginalSAT,
            NoCertificadoSAT      = @NoCertificadoSAT,
            NoCertificadoEmisor   = ISNULL(@NoCertificadoEmisor, NoCertificadoEmisor),
            FechaTimbrado         = @FechaTimbrado,
            XMLTimbrado           = @XMLTimbrado,
            RfcProvCertif         = @RfcProvCertif,
            EstadoCFDI            = 3, -- VIGENTE
            IntentosTimbrado      = IntentosTimbrado + 1,
            UltimoIntentoTimbrado = GETDATE(),
            PACCodigoError        = NULL,
            PACMensaje            = NULL
        WHERE CFDIId = @CFDIId;

        UPDATE dbo.DocReciboNomina SET Estatus = 'TIMBRADO' WHERE ReciboId = @ReciboId;

        INSERT INTO dbo.LogCFDIEvento (CFDIId, TipoEvento, EstadoAnterior, EstadoNuevo, Detalle)
        VALUES (@CFDIId, 'TIMBRADO', CAST(@EstAnt AS NVARCHAR(3)), '3',
                CONCAT('UUID=', CAST(@UUID AS NVARCHAR(50)), ' PAC=', ISNULL(@RfcProvCertif,'?')));

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- sp_CancelarCFDI (Cancelación conforme a Anexo 20 RMF / CFF Art. 29-A)
CREATE OR ALTER PROCEDURE dbo.sp_CancelarCFDI
    @CFDIId               INT,
    @MotivoCancelacionId  INT,
    @UUIDSustitucion      UNIQUEIDENTIFIER = NULL,
    @AcuseXML             NVARCHAR(MAX) = NULL,
    @UsuarioId            INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @EstAnt INT, @ReqSust BIT, @ClaveMotivo NVARCHAR(3);
        SELECT @EstAnt = EstadoCFDI FROM dbo.DocCFDINomina WITH (UPDLOCK,HOLDLOCK) WHERE CFDIId = @CFDIId;
        SELECT @ClaveMotivo = Clave, @ReqSust = RequiereSustitucion
        FROM dbo.CatSatMotivoCancelacion WHERE MotivoCancelacionId = @MotivoCancelacionId;

        IF @EstAnt IS NULL
        BEGIN RAISERROR('CFDI %d no existe.', 16, 1, @CFDIId); ROLLBACK; RETURN; END
        IF @ClaveMotivo IS NULL
        BEGIN RAISERROR('Motivo de cancelación %d inválido.', 16, 1, @MotivoCancelacionId); ROLLBACK; RETURN; END
        IF @EstAnt NOT IN (2, 3)
        BEGIN RAISERROR('CFDI %d no puede cancelarse en estado actual %d.', 16, 1, @CFDIId, @EstAnt); ROLLBACK; RETURN; END
        IF @ReqSust = 1 AND @UUIDSustitucion IS NULL
        BEGIN RAISERROR('Motivo %s requiere UUIDSustitucion.', 16, 1, @ClaveMotivo); ROLLBACK; RETURN; END

        IF TRY_CONVERT(XML,@AcuseXML) IS NULL
            THROW 51015, 'Se requiere acuse XML de cancelacion confirmada por SAT/PAC.', 1;
        UPDATE dbo.DocCFDINomina SET
            EstadoCFDI          = 4, -- CANCELADO
            FechaCancelacion    = GETDATE(),
            MotivoCancelacion   = @MotivoCancelacionId,
            UUIDSustitucion     = @UUIDSustitucion,
            AcuseCancelacionXML = @AcuseXML,
            IntentosCancelacion = IntentosCancelacion + 1
        WHERE CFDIId = @CFDIId;

        UPDATE rn SET Estatus = 'CANCELADO'
        FROM dbo.DocReciboNomina rn
        INNER JOIN dbo.DocCFDINomina c ON c.ReciboId = rn.ReciboId
        WHERE c.CFDIId = @CFDIId;

        INSERT INTO dbo.LogCFDIEvento (CFDIId, TipoEvento, EstadoAnterior, EstadoNuevo, Detalle, Usuario)
        VALUES (@CFDIId, 'CANCELACION', CAST(@EstAnt AS NVARCHAR(3)), '4',
                CONCAT('Motivo=', @ClaveMotivo,
                       CASE WHEN @UUIDSustitucion IS NOT NULL
                            THEN CONCAT(' UUIDSust=', CAST(@UUIDSustitucion AS NVARCHAR(50)))
                            ELSE '' END),
                ISNULL(CAST(@UsuarioId AS NVARCHAR(20)), SYSTEM_USER));

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- Procedimientos de Seguridad y RBAC
CREATE OR ALTER PROCEDURE dbo.sp_Auth_CrearUsuario
    @NombreUsuario NVARCHAR(50),
    @Email NVARCHAR(150),
    @NombreMostrar NVARCHAR(150),
    @Contrasena NVARCHAR(256),
    @RequiereCambioPwd BIT = 0,
    @UsuarioId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF LEN(ISNULL(@NombreUsuario, N'')) < 4
    BEGIN RAISERROR('NombreUsuario debe tener al menos 4 caracteres.', 16, 1); RETURN; END
    IF LEN(ISNULL(@Contrasena, N'')) < 10
    BEGIN RAISERROR('La contraseña debe tener al menos 10 caracteres.', 16, 1); RETURN; END
    IF @Email NOT LIKE '%_@_%._%'
    BEGIN RAISERROR('Email no tiene formato válido.', 16, 1); RETURN; END
    IF EXISTS (SELECT 1 FROM dbo.CatUsuario WHERE NombreUsuario = @NombreUsuario)
    BEGIN RAISERROR('NombreUsuario ya existe.', 16, 1); RETURN; END
    IF EXISTS (SELECT 1 FROM dbo.CatUsuario WHERE Email = @Email)
    BEGIN RAISERROR('Email ya existe.', 16, 1); RETURN; END

    DECLARE @Salt VARBINARY(32) = CRYPT_GEN_RANDOM(32);
    DECLARE @Hash VARBINARY(64) = dbo.fn_Auth_HashContrasena(@Contrasena, @Salt);

    INSERT INTO dbo.CatUsuario (NombreUsuario, Email, NombreMostrar, ContrasenaHash, ContrasenaSalt, RequiereCambioPwd)
    VALUES (@NombreUsuario, @Email, @NombreMostrar, @Hash, @Salt, @RequiereCambioPwd);

    SET @UsuarioId = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Auth_IniciarSesion
    @NombreUsuario NVARCHAR(50),
    @Contrasena    NVARCHAR(256),
    @IpOrigen      NVARCHAR(45)  = NULL,
    @UserAgent     NVARCHAR(300) = NULL,
    @TokenSesion   NVARCHAR(128) OUTPUT,
    @UsuarioId     INT           OUTPUT,
    @RequiereCambio BIT          OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @UsuarioId = NULL, @TokenSesion = NULL, @RequiereCambio = NULL;
    DECLARE @Salt VARBINARY(32), @HashAlmacenado VARBINARY(64), @Activo BIT, @Bloqueado BIT;

    SELECT @UsuarioId = UsuarioId, @Salt = ContrasenaSalt, @HashAlmacenado = ContrasenaHash,
           @Activo = Activo, @Bloqueado = Bloqueado, @RequiereCambio = RequiereCambioPwd
    FROM dbo.CatUsuario WHERE NombreUsuario = @NombreUsuario;

    IF @UsuarioId IS NULL
    BEGIN
        INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle, IpOrigen, UserAgent)
        VALUES (NULL, 'LOGIN_FALLIDO', 0, CONCAT('Usuario inexistente: ', @NombreUsuario), @IpOrigen, @UserAgent);
        RAISERROR('Credenciales inválidas.', 16, 1); RETURN;
    END

    IF @Activo = 0 OR @Bloqueado = 1
    BEGIN
        INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle, IpOrigen, UserAgent)
        VALUES (@UsuarioId, 'LOGIN_BLOQUEADO', 0, 'Cuenta inactiva o bloqueada.', @IpOrigen, @UserAgent);
        RAISERROR('Usuario inactivo o bloqueado.', 16, 1); RETURN;
    END

    IF dbo.fn_Auth_HashContrasena(@Contrasena, @Salt) <> @HashAlmacenado
    BEGIN
        UPDATE dbo.CatUsuario SET IntentosFallidos = IntentosFallidos + 1,
            Bloqueado = CASE WHEN IntentosFallidos + 1 >= 5 THEN 1 ELSE 0 END
        WHERE UsuarioId = @UsuarioId;

        INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle, IpOrigen, UserAgent)
        VALUES (@UsuarioId, 'LOGIN_FALLIDO', 0, 'Contraseña incorrecta.', @IpOrigen, @UserAgent);
        RAISERROR('Credenciales inválidas.', 16, 1); RETURN;
    END

    -- Éxito
    UPDATE dbo.CatUsuario SET IntentosFallidos = 0, UltimoAccesoUtc = SYSUTCDATETIME() WHERE UsuarioId = @UsuarioId;
    SET @TokenSesion = CONVERT(NVARCHAR(36), NEWID()) + '-' + CONVERT(NVARCHAR(36), NEWID());
    DECLARE @TokenHash VARBINARY(64) = dbo.fn_Auth_HashToken(@TokenSesion);

    INSERT INTO dbo.ProcSesionUsuario (UsuarioId, TokenHash, ExpiraUtc, IpOrigen, UserAgent)
    VALUES (@UsuarioId, @TokenHash, DATEADD(MINUTE, 480, SYSUTCDATETIME()), @IpOrigen, @UserAgent);

    INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle, IpOrigen, UserAgent)
    VALUES (@UsuarioId, 'LOGIN_EXITOSO', 1, 'Inicio de sesión completado.', @IpOrigen, @UserAgent);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Auth_ValidarSesion
    @TokenSesion NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TokenHash VARBINARY(64) = dbo.fn_Auth_HashToken(@TokenSesion);
    SELECT s.UsuarioId, u.NombreUsuario, u.NombreMostrar, u.Email, u.RequiereCambioPwd
    FROM dbo.ProcSesionUsuario s
    INNER JOIN dbo.CatUsuario u ON u.UsuarioId = s.UsuarioId
    WHERE s.TokenHash = @TokenHash
      AND s.RevocadaUtc IS NULL
      AND s.ExpiraUtc > SYSUTCDATETIME()
      AND u.Activo = 1 AND u.Bloqueado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Auth_CerrarSesion
    @TokenSesion NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TokenHash VARBINARY(64) = dbo.fn_Auth_HashToken(@TokenSesion);
    UPDATE dbo.ProcSesionUsuario SET RevocadaUtc = SYSUTCDATETIME()
    WHERE TokenHash = @TokenHash AND RevocadaUtc IS NULL;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_AsignarRolUsuario
    @UsuarioId INT,
    @CodigoRol NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @RolId INT;
    SELECT @RolId = RolId FROM dbo.CatRol WHERE Codigo = @CodigoRol AND Activo = 1;
    IF @RolId IS NULL BEGIN RAISERROR('Rol %s no existe o está inactivo.', 16, 1, @CodigoRol); RETURN; END
    IF NOT EXISTS (SELECT 1 FROM dbo.CatUsuario WHERE UsuarioId = @UsuarioId)
        BEGIN RAISERROR('Usuario %d no existe.', 16, 1, @UsuarioId); RETURN; END
    IF NOT EXISTS (SELECT 1 FROM dbo.RelUsuarioRol WHERE UsuarioId = @UsuarioId AND RolId = @RolId)
        INSERT INTO dbo.RelUsuarioRol (UsuarioId, RolId) VALUES (@UsuarioId, @RolId);

    INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle)
    VALUES (@UsuarioId, 'ROL_ASIGNADO', 1, CONCAT('Rol=', @CodigoRol));
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_QuitarRolUsuario
    @UsuarioId INT,
    @CodigoRol NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @RolId INT;
    SELECT @RolId = RolId FROM dbo.CatRol WHERE Codigo = @CodigoRol;
    IF @RolId IS NULL RETURN;
    DELETE FROM dbo.RelUsuarioRol WHERE UsuarioId = @UsuarioId AND RolId = @RolId;
    IF @@ROWCOUNT > 0
        INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle)
        VALUES (@UsuarioId, 'ROL_REMOVIDO', 1, CONCAT('Rol=', @CodigoRol));
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GestionarPoliticaEmpresarial
    @Accion NVARCHAR(10),
    @PoliticaId INT = NULL OUTPUT,
    @EmpresaId INT,
    @DiasAguinaldo DECIMAL(10,4) = 15.00,
    @PorcentajePrimaVacacional DECIMAL(5,2) = 25.00,
    @DiasPrimaAntiguedad DECIMAL(10,4) = 12.00,
    @PorcentajeFondoAhorroEmpleado DECIMAL(5,2) = 0.00,
    @PorcentajeFondoAhorroEmpresa DECIMAL(5,2) = 0.00,
    @FechaInicioVigencia DATE = NULL,
    @UsuarioId INT = NULL,
    @Justificacion NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        IF @FechaInicioVigencia IS NULL SET @FechaInicioVigencia = CAST(GETDATE() AS DATE);

        IF @DiasAguinaldo < 15.00
        THROW 51011, 'Aguinaldo minimo de 15 dias.', 1;
        IF @PorcentajePrimaVacacional < 25.00
        THROW 51011, 'Prima vacacional minima de 25%.', 1;
        IF @DiasPrimaAntiguedad < 12.00
        THROW 51011, 'Prima de antiguedad minima de 12 dias.', 1;

        IF @Accion IS NULL OR @Accion NOT IN ('INSERT','UPDATE')
            THROW 51011, 'Accion de politica invalida.', 1;
        IF @Accion = 'UPDATE' AND NOT EXISTS
            (SELECT 1 FROM dbo.CatPoliticaEmpresarial WHERE PoliticaId = @PoliticaId AND EmpresaId = @EmpresaId)
            THROW 51011, 'La politica no existe para esta empresa.', 1;
        DECLARE @ValAnt NVARCHAR(MAX), @ValNvo NVARCHAR(MAX);

        IF @Accion = 'INSERT'
        BEGIN
            UPDATE dbo.CatPoliticaEmpresarial
            SET FechaFinVigencia = DATEADD(DAY, -1, @FechaInicioVigencia), Activa = 0
            WHERE EmpresaId = @EmpresaId AND Activa = 1 AND FechaInicioVigencia < @FechaInicioVigencia;

            INSERT INTO dbo.CatPoliticaEmpresarial (
                EmpresaId, DiasAguinaldo, PorcentajePrimaVacacional, DiasPrimaAntiguedad,
                PorcentajeFondoAhorroEmpleado, PorcentajeFondoAhorroEmpresa,
                FechaInicioVigencia, Activa
            ) VALUES (
                @EmpresaId, @DiasAguinaldo, @PorcentajePrimaVacacional, @DiasPrimaAntiguedad,
                @PorcentajeFondoAhorroEmpleado, @PorcentajeFondoAhorroEmpresa,
                @FechaInicioVigencia, 1
            );
            SET @PoliticaId = SCOPE_IDENTITY();

            SET @ValNvo = CONCAT('{"DiasAguinaldo":', @DiasAguinaldo,
                ',"PrimaVacacional":', @PorcentajePrimaVacacional,
                ',"PrimaAntiguedad":', @DiasPrimaAntiguedad, '}');
            INSERT INTO dbo.LogAuditoriaPolitica (TipoEntidad, EntidadId, Accion, UsuarioId, ValoresNuevos, Justificacion)
            VALUES ('POLITICA', @PoliticaId, 'INSERT', @UsuarioId, @ValNvo, @Justificacion);
        END
        ELSE IF @Accion = 'UPDATE'
        BEGIN
            SELECT @ValAnt = CONCAT('{"DiasAguinaldo":', DiasAguinaldo,
                ',"PrimaVacacional":', PorcentajePrimaVacacional,
                ',"PrimaAntiguedad":', DiasPrimaAntiguedad, '}')
            FROM dbo.CatPoliticaEmpresarial WHERE PoliticaId = @PoliticaId;

            UPDATE dbo.CatPoliticaEmpresarial SET
                DiasAguinaldo = @DiasAguinaldo,
                PorcentajePrimaVacacional = @PorcentajePrimaVacacional,
                DiasPrimaAntiguedad = @DiasPrimaAntiguedad,
                PorcentajeFondoAhorroEmpleado = @PorcentajeFondoAhorroEmpleado,
                PorcentajeFondoAhorroEmpresa = @PorcentajeFondoAhorroEmpresa
            WHERE PoliticaId = @PoliticaId;

            SET @ValNvo = CONCAT('{"DiasAguinaldo":', @DiasAguinaldo,
                ',"PrimaVacacional":', @PorcentajePrimaVacacional,
                ',"PrimaAntiguedad":', @DiasPrimaAntiguedad, '}');
            INSERT INTO dbo.LogAuditoriaPolitica (TipoEntidad, EntidadId, Accion, UsuarioId, ValoresAnteriores, ValoresNuevos, Justificacion)
            VALUES ('POLITICA', @PoliticaId, 'UPDATE', @UsuarioId, @ValAnt, @ValNvo, @Justificacion);
        END

        COMMIT;
        SELECT @PoliticaId AS PoliticaId, 'Política gestionada correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- ==============================================================================
-- [BLOQUE-12] VISTAS DE CONSULTA Y REPORTES (vw_*)
-- ==============================================================================
PRINT '[12/21] Creando vistas del sistema...';
GO

-- Vista integral del empleado
CREATE OR ALTER VIEW dbo.vw_EmpleadoCompleto AS
SELECT
    e.EmpleadoId,
    e.EmpresaId,
    emp.RazonSocial AS Empresa,
    e.NumeroEmpleado,
    CONCAT(e.ApellidoPaterno, ' ', ISNULL(e.ApellidoMaterno + ' ', ''), e.Nombres) AS NombreCompleto,
    e.FechaIngreso,
    dbo.fn_AniosCumplidos(e.FechaIngreso, CAST(GETDATE() AS DATE)) AS AniosServicio,
    e.Estatus,
    f.RFC,
    f.CURP,
    f.RegimenFiscalSAT,
    f.ClaveEntidadFederativa,
    i.NSS,
    i.SalarioDiario AS SD,
    i.SalarioDiarioIntegrado AS SDI,
    i.SBC,
    i.TipoRiesgo,
    dbo.fn_DiasVacaciones(dbo.fn_AniosCumplidos(e.FechaIngreso, CAST(GETDATE() AS DATE))) AS DiasVacaciones,
    dbo.fn_FactorIntegracion(dbo.fn_AniosCumplidos(e.FechaIngreso, CAST(GETDATE() AS DATE)), 15, 25, 0, i.SalarioDiario) AS FactorIntegracion,
    dep.Descripcion AS Departamento,
    pu.Descripcion  AS Puesto,
    tc.Descripcion  AS TipoContrato,
    tp.Descripcion  AS TipoPeriodo
FROM dbo.CatEmpleado e
INNER JOIN dbo.CatEmpresa emp ON emp.EmpresaId = e.EmpresaId
LEFT JOIN dbo.CatEmpleadoDatosFiscales f ON f.EmpleadoId = e.EmpleadoId
LEFT JOIN dbo.CatEmpleadoDatosIMSS    i ON i.EmpleadoId = e.EmpleadoId
LEFT JOIN dbo.DocContratoEmpleado    ec ON ec.EmpleadoId = e.EmpleadoId AND ec.Activo = 1
LEFT JOIN dbo.CatDepartamento       dep ON dep.DepartamentoId = ec.DepartamentoId
LEFT JOIN dbo.CatPuesto              pu ON pu.PuestoId = ec.PuestoId
LEFT JOIN dbo.CatSatTipoContrato        tc ON tc.TipoContratoId = ec.TipoContratoId
LEFT JOIN dbo.CatSatPeriodicidadPago    tp ON tp.PeriodicidadPagoId = ec.PeriodicidadPagoId;
GO

-- Resumen analítico por periodo de nómina
CREATE OR ALTER VIEW dbo.vw_ResumenPeriodo AS
SELECT
    pn.PeriodoId,
    pn.EmpresaId,
    emp.RazonSocial AS Empresa,
    ef.Anio,
    tp.Descripcion AS TipoPeriodo,
    pn.NumeroPeriodo,
    pn.FechaInicio,
    pn.FechaFin,
    pn.FechaPago,
    pn.FechaFinalPago,
    pn.Estatus,
    COUNT(rn.ReciboId) AS TotalEmpleados,
    ISNULL(SUM(rn.TotalPercepciones), 0) AS SumaPercepciones,
    ISNULL(SUM(rn.TotalDeducciones), 0)  AS SumaDeducciones,
    ISNULL(SUM(rn.NetoPagar), 0)         AS SumaNetoPagar,
    ISNULL(SUM(CASE WHEN rd.ISR IS NOT NULL THEN rd.ISR ELSE 0 END), 0) AS ISRTotalRetenido
FROM dbo.ProcPeriodoNomina pn
INNER JOIN dbo.CatEmpresa emp ON emp.EmpresaId = pn.EmpresaId
INNER JOIN dbo.CatEjercicioFiscal ef ON ef.EjercicioId = pn.EjercicioId
INNER JOIN dbo.CatSatPeriodicidadPago tp ON tp.PeriodicidadPagoId = pn.PeriodicidadPagoId
LEFT  JOIN dbo.DocReciboNomina rn    ON rn.PeriodoId = pn.PeriodoId AND rn.Estatus NOT IN ('PRERECIBO','CANCELADO')
OUTER APPLY (SELECT SUM(d.ImporteGravado + d.ImporteExento) AS ISR
             FROM dbo.DocReciboDetalle d
             WHERE d.ReciboId = rn.ReciboId AND d.ClaveConceptoSAT = '002' AND d.TipoConcepto = 'D') rd
GROUP BY pn.PeriodoId, pn.EmpresaId, emp.RazonSocial, ef.Anio, tp.Descripcion, pn.NumeroPeriodo,
         pn.FechaInicio, pn.FechaFin, pn.FechaPago, pn.FechaFinalPago, pn.Estatus;
GO

-- Políticas activas por empresa
CREATE OR ALTER VIEW dbo.vw_PoliticasActivasPorEmpresa AS
SELECT
    p.PoliticaId, p.EmpresaId, e.RazonSocial, e.RFC,
    p.DiasAguinaldo, p.PorcentajePrimaVacacional, p.DiasPrimaAntiguedad,
    p.PorcentajePrimaDominical,
    p.PorcentajeFondoAhorroEmpleado, p.PorcentajeFondoAhorroEmpresa,
    p.ExencionAguinaldoUMAs, p.ExencionPrimaVacacionalUMAs, p.ExencionHorasExtraUMAsSemanal,
    p.FechaInicioVigencia, p.FechaFinVigencia, p.FechaCreacion
FROM dbo.CatPoliticaEmpresarial p
INNER JOIN dbo.CatEmpresa e ON e.EmpresaId = p.EmpresaId
WHERE p.Activa = 1;
GO

-- Conceptos con su fórmula asignada
CREATE OR ALTER VIEW dbo.vw_ConceptosConFormulas AS
SELECT
    c.ConceptoId, c.ClaveSAT, c.TipoConcepto,
    CASE c.TipoConcepto WHEN 'P' THEN 'Percepción' WHEN 'D' THEN 'Deducción' WHEN 'O' THEN 'Otro Pago' END AS TipoConceptoDesc,
    c.Descripcion AS ConceptoDesc,
    c.EsGravado, c.TopeUMAAnual,
    cf.ConceptoFormulaId, f.FormulaId, f.Nombre AS FormulaNombre,
    f.ExpresionCalculo, f.ParametrosDefault,
    cf.ParametrosEspecificos, cf.OrdenEjecucion,
    tf.Codigo AS TipoFormula, tf.Descripcion AS TipoFormulaDesc
FROM dbo.CatConcepto c
LEFT JOIN dbo.RelConceptoFormula cf ON cf.ConceptoId = c.ConceptoId AND cf.Activa = 1
LEFT JOIN dbo.CatFormulaCalculo f   ON f.FormulaId = cf.FormulaId AND f.Activa = 1
LEFT JOIN dbo.CatTipoFormula tf     ON tf.TipoFormulaId = f.TipoFormulaId
WHERE c.Activo = 1;
GO

-- ==============================================================================
-- [BLOQUE-13] TRIGGERS DE INTEGRIDAD Y AUDITORÍA (trg_*)
-- ==============================================================================
PRINT '[13/21] Creando triggers de auditoría y validación...';
GO

-- Actualización automática de FechaModificacion en CatEmpresa
CREATE OR ALTER TRIGGER dbo.trg_CatEmpresa_UpdateFechaMod
ON dbo.CatEmpresa
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(FechaModificacion)
        UPDATE e
        SET    FechaModificacion = GETDATE()
        FROM   dbo.CatEmpresa e
        INNER JOIN inserted i ON i.EmpresaId = e.EmpresaId;
END
GO

-- Actualización automática de FechaModificacion en CatEmpleado
CREATE OR ALTER TRIGGER dbo.trg_CatEmpleado_UpdateFechaMod
ON dbo.CatEmpleado
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(FechaModificacion)
        UPDATE e
        SET    FechaModificacion = GETDATE()
        FROM   dbo.CatEmpleado e
        INNER JOIN inserted i ON i.EmpleadoId = e.EmpleadoId;
END
GO

-- Los periodos solo pueden generarse para empresas activas y para periodicidades asignadas a la empresa
CREATE OR ALTER TRIGGER dbo.trg_ProcPeriodoNomina_ValidarPeriodicidad
ON dbo.ProcPeriodoNomina
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted i
               WHERE NOT EXISTS (SELECT 1 FROM dbo.CatEmpresa e WHERE e.EmpresaId = i.EmpresaId AND e.Activa = 1))
    BEGIN
        RAISERROR('No se pueden generar periodos: la empresa no está activa. Registre o active una empresa primero.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM inserted i
               WHERE NOT EXISTS (SELECT 1 FROM dbo.RelEmpresaPeriodicidad r
                                 WHERE r.EmpresaId = i.EmpresaId AND r.PeriodicidadPagoId = i.PeriodicidadPagoId AND r.Activo = 1))
    BEGIN
        RAISERROR('No se pueden generar periodos: la periodicidad no está asignada a la empresa.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO

-- El contrato vigente de un empleado debe usar una periodicidad habilitada para la empresa del empleado
CREATE OR ALTER TRIGGER dbo.trg_DocContratoEmpleado_ValidarPeriodicidad
ON dbo.DocContratoEmpleado
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN dbo.CatEmpleado e ON e.EmpleadoId = i.EmpleadoId
        WHERE i.Activo = 1
          AND NOT EXISTS (SELECT 1 FROM dbo.RelEmpresaPeriodicidad r
                          WHERE r.EmpresaId = e.EmpresaId AND r.PeriodicidadPagoId = i.PeriodicidadPagoId AND r.Activo = 1))
    BEGIN
        RAISERROR('La periodicidad de pago del empleado no está asignada a su empresa. Asigne la periodicidad a la empresa o elija otra.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO

-- No se puede retirar de una empresa una periodicidad que aún usan empleados que no están en baja
CREATE OR ALTER TRIGGER dbo.trg_RelEmpresaPeriodicidad_ValidarBaja
ON dbo.RelEmpresaPeriodicidad
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(Activo)
        RETURN;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN deleted d ON d.EmpresaPeriodicidadId = i.EmpresaPeriodicidadId
        WHERE d.Activo = 1 AND i.Activo = 0
          AND EXISTS (SELECT 1
                      FROM dbo.CatEmpleado e
                      INNER JOIN dbo.DocContratoEmpleado c ON c.EmpleadoId = e.EmpleadoId AND c.Activo = 1
                      WHERE e.EmpresaId = i.EmpresaId AND c.PeriodicidadPagoId = i.PeriodicidadPagoId AND e.Estatus <> 'BAJA'))
    BEGIN
        RAISERROR('No se puede quitar la periodicidad de la empresa: hay empleados que todavía la utilizan.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO

-- Validación estricta de CSD activo en timbrado de CFDI
CREATE OR ALTER TRIGGER dbo.trg_DocCFDINomina_ValidarCSD
ON dbo.DocCFDINomina AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i JOIN dbo.DocReciboNomina r ON r.ReciboId = i.ReciboId
        JOIN dbo.ProcPeriodoNomina p ON p.PeriodoId = r.PeriodoId
        WHERE i.FechaTimbrado IS NOT NULL AND (
            i.FechaEmision IS NULL OR i.NoCertificadoEmisor IS NULL
            OR NOT EXISTS (SELECT 1 FROM dbo.HistEmpresaCSD c
                           WHERE c.EmpresaId = p.EmpresaId AND c.NoCertificado = i.NoCertificadoEmisor
                             AND CAST(i.FechaEmision AS DATE) BETWEEN c.VigenciaDesde AND c.VigenciaHasta)))
        THROW 51021, 'El certificado emisor no corresponde a la empresa o no estaba vigente al emitir.', 1;
END
GO

-- ==============================================================================
-- Compatibilidad de nombres del DAL: no duplica tablas ni datos SAT.
CREATE VIEW dbo.CatRegimenFiscal AS
SELECT s.*, FechaInicioVigencia AS FechaInicio, FechaFinVigencia AS FechaFin,
       CONVERT(CHAR(1), CASE WHEN AplicaPersonaFisica = 1 AND AplicaPersonaMoral = 1 THEN 'T'
                           WHEN AplicaPersonaFisica = 1 THEN 'F' ELSE 'M' END) AS TipoPersona
FROM dbo.CatSatRegimenFiscal s;
GO
CREATE VIEW dbo.CatTipoJornada AS SELECT s.*, r.Aplicacion, r.HorasMaximasLegal FROM dbo.CatSatTipoJornada s LEFT JOIN dbo.CatReglaTipoJornada r ON r.TipoJornadaId = s.TipoJornadaId;
GO
CREATE VIEW dbo.CatPeriodicidadPago AS SELECT s.*, r.DiasBase FROM dbo.CatSatPeriodicidadPago s LEFT JOIN dbo.CatReglaPeriodicidadPago r ON r.PeriodicidadPagoId = s.PeriodicidadPagoId;
GO
CREATE SYNONYM dbo.CatTipoRegimen FOR dbo.CatSatTipoRegimen;
GO
CREATE VIEW dbo.CatRiesgoPuesto AS SELECT s.*, r.Clase, r.PrimaMedia, r.PrimaMinima, r.PrimaMaxima FROM dbo.CatSatRiesgoPuesto s LEFT JOIN dbo.CatReglaRiesgoPuesto r ON r.RiesgoPuestoId = s.RiesgoPuestoId;
GO
CREATE SYNONYM dbo.CatMotivoCancelacion FOR dbo.CatSatMotivoCancelacion;
GO
CREATE VIEW dbo.CatTipoIncapacidad AS SELECT s.*, r.PorcentajePago, r.DiasEspera, r.PagaPatron FROM dbo.CatSatTipoIncapacidad s LEFT JOIN dbo.CatReglaTipoIncapacidad r ON r.TipoIncapacidadId = s.TipoIncapacidadId;
GO
CREATE SYNONYM dbo.CatOrigenRecurso FOR dbo.CatSatOrigenRecurso;
GO
CREATE SYNONYM dbo.CatTipoNomina FOR dbo.CatSatTipoNomina;
GO
CREATE SYNONYM dbo.CatTipoHoras FOR dbo.CatSatTipoHoras;
GO
CREATE SYNONYM dbo.CatBanco FOR dbo.CatSatBanco;
GO
CREATE SYNONYM dbo.CatTipoPercepcion FOR dbo.CatSatTipoPercepcion;
GO
CREATE SYNONYM dbo.CatTipoDeduccion FOR dbo.CatSatTipoDeduccion;
GO
CREATE SYNONYM dbo.CatTipoOtroPago FOR dbo.CatSatTipoOtroPago;
GO
CREATE SYNONYM dbo.CatTipoContrato FOR dbo.CatSatTipoContrato;
GO
CREATE SYNONYM dbo.CatMetodoPago FOR dbo.CatFormaPagoDispersion;
GO
CREATE SYNONYM dbo.DocCFDIEvento FOR dbo.LogCFDIEvento;
GO
CREATE SYNONYM dbo.LogSesionUsuario FOR dbo.ProcSesionUsuario;
GO
CREATE SYNONYM dbo.LogRecuperacionContrasena FOR dbo.ProcRecuperacionContrasena;
GO
-- Validacion relacional previa a registrar una respuesta PAC.
-- No sustituye validacion XSD, matriz SAT, cadena original ni validacion criptografica del XML.

CREATE TABLE dbo.ProcBasePTU (
    EmpleadoId INT NOT NULL REFERENCES dbo.CatEmpleado(EmpleadoId),
    EjercicioId INT NOT NULL REFERENCES dbo.CatEjercicioFiscal(EjercicioId),
    DiasComputables INT NOT NULL CHECK (DiasComputables BETWEEN 0 AND 366),
    SalarioBaseReparto DECIMAL(18,4) NOT NULL CHECK (SalarioBaseReparto >= 0),
    TopeIndividual DECIMAL(18,4) NOT NULL CHECK (TopeIndividual >= 0),
    Elegible BIT NOT NULL,
    Validado BIT NOT NULL DEFAULT 0,
    Justificacion NVARCHAR(500) NOT NULL,
    CONSTRAINT PK_ProcBasePTU PRIMARY KEY (EmpleadoId,EjercicioId)
);
GO

-- ==============================================================================
-- [BLOQUE-14] POBLACIÓN DE DATOS SEMILLA Y CATÁLOGOS SAT OFICIALES
-- ==============================================================================
PRINT '[14/21] Insertando catalogos SAT, reglas operativas y configuracion base...';
GO

-- 1. CatSatRegimenFiscal
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatRegimenFiscal)
INSERT INTO dbo.CatSatRegimenFiscal (RegimenFiscalId, Clave, Descripcion, AplicaPersonaFisica, AplicaPersonaMoral, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(601, '601', N'General de Ley Personas Morales', 0, 1, '2017-01-01', NULL, 1),
(603, '603', N'Personas Morales con Fines no Lucrativos', 0, 1, '2017-01-01', NULL, 1),
(605, '605', N'Sueldos y Salarios e Ingresos Asimilados a Salarios', 1, 0, '2017-01-01', NULL, 1),
(606, '606', N'Arrendamiento', 1, 0, '2017-01-01', NULL, 1),
(607, '607', N'Régimen de Enajenación o Adquisición de Bienes', 1, 0, '2017-01-01', NULL, 1),
(608, '608', N'Demás ingresos', 1, 0, '2017-01-01', NULL, 1),
(610, '610', N'Residentes en el Extranjero sin Establecimiento Permanente en México', 1, 1, '2017-01-01', NULL, 1),
(611, '611', N'Ingresos por Dividendos (socios y accionistas)', 1, 0, '2017-01-01', NULL, 1),
(612, '612', N'Personas Físicas con Actividades Empresariales y Profesionales', 1, 0, '2017-01-01', NULL, 1),
(614, '614', N'Ingresos por intereses', 1, 0, '2017-01-01', NULL, 1),
(615, '615', N'Régimen de los ingresos por obtención de premios', 1, 0, '2017-01-01', NULL, 1),
(616, '616', N'Sin obligaciones fiscales', 1, 0, '2017-01-01', NULL, 1),
(620, '620', N'Sociedades Cooperativas de Producción que optan por diferir sus ingresos', 0, 1, '2017-01-01', NULL, 1),
(621, '621', N'Incorporación Fiscal', 1, 0, '2017-01-01', NULL, 1),
(622, '622', N'Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras', 1, 1, '2017-01-01', NULL, 1),
(623, '623', N'Opcional para Grupos de Sociedades', 0, 1, '2017-01-01', NULL, 1),
(624, '624', N'Coordinados', 0, 1, '2017-01-01', NULL, 1),
(625, '625', N'Actividades Empresariales con ingresos a través de Plataformas Tecnológicas', 1, 0, '2017-01-01', NULL, 1),
(626, '626', N'Régimen Simplificado de Confianza', 1, 1, '2017-01-01', NULL, 1);
GO

-- 2. CatSatOrigenRecurso
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatOrigenRecurso)
INSERT INTO dbo.CatSatOrigenRecurso (OrigenRecursoId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(1, 'IP', N'Ingresos propios.', '2017-01-01', NULL, 1),
(2, 'IF', N'Ingreso federales.', '2017-01-01', NULL, 1),
(3, 'IM', N'Ingresos mixtos.', '2017-01-01', NULL, 1);
GO

-- 3. CatSatTipoNomina
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoNomina)
INSERT INTO dbo.CatSatTipoNomina (TipoNominaId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(1, 'O', N'Nómina ordinaria', '2017-01-01', NULL, 1),
(2, 'E', N'Nómina extraordinaria', '2017-01-01', NULL, 1);
GO

-- 4. CatSatTipoHoras
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoHoras)
INSERT INTO dbo.CatSatTipoHoras (TipoHorasId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(1, '01', N'Dobles', '2017-01-01', NULL, 1),
(2, '02', N'Triples', '2017-01-01', NULL, 1),
(3, '03', N'Simples', '2017-01-01', NULL, 1);
GO

-- 5. CatSatTipoContrato
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoContrato)
INSERT INTO dbo.CatSatTipoContrato (TipoContratoId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(1, '01', N'Contrato de trabajo por tiempo indeterminado', '2017-01-01', NULL, 1),
(2, '02', N'Contrato de trabajo para obra determinada', '2017-01-01', NULL, 1),
(3, '03', N'Contrato de trabajo por tiempo determinado', '2017-01-01', NULL, 1),
(4, '04', N'Contrato de trabajo por temporada', '2017-01-01', NULL, 1),
(5, '05', N'Contrato de trabajo sujeto a prueba', '2017-01-01', NULL, 1),
(6, '06', N'Contrato de trabajo con capacitación inicial', '2017-01-01', NULL, 1),
(7, '07', N'Modalidad de contratación por pago de hora laborada', '2017-01-01', NULL, 1),
(8, '08', N'Modalidad de trabajo por comisión laboral', '2017-01-01', NULL, 1),
(9, '09', N'Modalidades de contratación donde no existe relación de trabajo', '2017-01-01', NULL, 1),
(10, '10', N'Jubilación, pensión, retiro.', '2017-01-01', NULL, 1),
(99, '99', N'Otro contrato', '2017-01-01', NULL, 1);
GO

-- 6. CatSatTipoJornada & CatReglaTipoJornada
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoJornada)
BEGIN
    INSERT INTO dbo.CatSatTipoJornada (TipoJornadaId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
    (1, '01', N'Diurna', '2017-01-01', NULL, 1),
    (2, '02', N'Nocturna', '2017-01-01', NULL, 1),
    (3, '03', N'Mixta', '2017-01-01', NULL, 1),
    (4, '04', N'Por hora', '2017-01-01', NULL, 1),
    (5, '05', N'Reducida', '2017-01-01', NULL, 1),
    (6, '06', N'Continuada', '2017-01-01', NULL, 1),
    (7, '07', N'Partida', '2017-01-01', NULL, 1),
    (8, '08', N'Por turnos', '2017-01-01', NULL, 1),
    (99, '99', N'Otra Jornada', '2017-01-01', NULL, 1);

    INSERT INTO dbo.CatReglaTipoJornada (TipoJornadaId, Aplicacion, HorasMaximasLegal) VALUES
    (1, N'Comprendida entre las 6:00 y las 20:00 horas (Máximo 8 horas diarias)', 8.00),
    (2, N'Comprendida entre las 20:00 y las 6:00 horas (Máximo 7 horas diarias)', 7.00),
    (3, N'Comprende periodos de tiempo de las jornadas diurna y nocturna (Máximo 7.5 horas)', 7.50),
    (4, N'Pago por cada hora laborada pactada', 8.00),
    (5, N'Jornada menor a la legal por acuerdo', 6.00),
    (6, N'Se laboran las horas de corrido, con al menos media hora de descanso', 8.00),
    (7, N'La jornada se divide en dos periodos', 8.00),
    (8, N'Trabajadores que rotan de horario periódicamente', 8.00),
    (99, N'Cualquier esquema que no encaje en las descripciones anteriores', NULL);
END
GO

-- 7. CatSatPeriodicidadPago & CatReglaPeriodicidadPago
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatPeriodicidadPago)
BEGIN
    INSERT INTO dbo.CatSatPeriodicidadPago (PeriodicidadPagoId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
    (1, '01', N'Diario', '2016-11-01', NULL, 1),
    (2, '02', N'Semanal', '2016-11-01', NULL, 1),
    (3, '03', N'Catorcenal', '2016-11-01', NULL, 1),
    (4, '04', N'Quincenal', '2016-11-01', NULL, 1),
    (5, '05', N'Mensual', '2016-11-01', NULL, 1),
    (6, '06', N'Bimestral', '2016-11-01', NULL, 1),
    (7, '07', N'Unidad obra', '2016-11-01', NULL, 1),
    (8, '08', N'Comisión', '2016-11-01', NULL, 1),
    (9, '09', N'Precio alzado', '2016-11-01', NULL, 1),
    (10, '10', N'Decenal', '2017-01-19', NULL, 1),
    (99, '99', N'Otra Periodicidad', '2016-11-01', NULL, 1);

    INSERT INTO dbo.CatReglaPeriodicidadPago (PeriodicidadPagoId, DiasBase) VALUES
    (1, NULL),
    (2, 7.00),
    (3, 14.00),
    (4, 15.00),
    (5, 30.00),
    (6, NULL),
    (7, NULL),
    (8, NULL),
    (9, NULL),
    (10, 10.00),
    (99, NULL);
END
GO

-- 8. CatSatTipoRegimen
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoRegimen)
INSERT INTO dbo.CatSatTipoRegimen (TipoRegimenId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(2, '02', N'Sueldos (Incluye ingresos señalados en la fracción I del artículo 94 de LISR)', '2017-01-01', NULL, 1),
(3, '03', N'Jubilados', '2017-01-01', NULL, 1),
(4, '04', N'Pensionados', '2017-01-01', NULL, 1),
(5, '05', N'Asimilados Miembros Sociedades Cooperativas Produccion', '2017-01-01', NULL, 1),
(6, '06', N'Asimilados Integrantes Sociedades Asociaciones Civiles', '2017-01-01', NULL, 1),
(7, '07', N'Asimilados Miembros consejos', '2017-01-01', NULL, 1),
(8, '08', N'Asimilados comisionistas', '2017-01-01', NULL, 1),
(9, '09', N'Asimilados Honorarios', '2017-01-01', NULL, 1),
(10, '10', N'Asimilados acciones', '2017-01-01', NULL, 1),
(11, '11', N'Asimilados otros', '2017-01-01', NULL, 1),
(12, '12', N'Jubilados o Pensionados', '2017-03-27', NULL, 1),
(13, '13', N'Indemnización o Separación', '2018-10-15', NULL, 1),
(99, '99', N'Otro Regimen', '2017-01-01', NULL, 1);
GO

-- 9. CatSatRiesgoPuesto & CatReglaRiesgoPuesto
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatRiesgoPuesto)
BEGIN
    INSERT INTO dbo.CatSatRiesgoPuesto (RiesgoPuestoId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
    (1, '1', N'Clase I', '2017-01-01', NULL, 1),
    (2, '2', N'Clase II', '2017-01-01', NULL, 1),
    (3, '3', N'Clase III', '2017-01-01', NULL, 1),
    (4, '4', N'Clase IV', '2017-01-01', NULL, 1),
    (5, '5', N'Clase V', '2017-01-01', NULL, 1),
    (99, '99', N'No aplica', '2017-08-13', NULL, 1);

    INSERT INTO dbo.CatReglaRiesgoPuesto (RiesgoPuestoId, Clase, PrimaMedia, PrimaMinima, PrimaMaxima) VALUES
    (1, 'I', 0.0054355, 0.00500, 0.15000),
    (2, 'II', 0.0113065, 0.00500, 0.15000),
    (3, 'III', 0.0259840, 0.00500, 0.15000),
    (4, 'IV', 0.0465325, 0.00500, 0.15000),
    (5, 'V', 0.0758875, 0.00500, 0.15000),
    (99, 'N/A', 0, 0, 0);
END
GO

-- 10. CatSatMotivoCancelacion
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatMotivoCancelacion)
INSERT INTO dbo.CatSatMotivoCancelacion (MotivoCancelacionId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(1, '01', N'Comprobante emitido con errores con relación', '2022-01-01', NULL, 1),
(2, '02', N'Comprobante emitido con errores sin relación', '2022-01-01', NULL, 1),
(3, '03', N'No se llevó a cabo la operación', '2022-01-01', NULL, 1),
(4, '04', N'Operación nominativa relacionada en una factura global', '2022-01-01', NULL, 1);
GO

-- 11. CatSatTipoIncapacidad & CatReglaTipoIncapacidad
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoIncapacidad)
BEGIN
    INSERT INTO dbo.CatSatTipoIncapacidad (TipoIncapacidadId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
    (1, '01', N'Riesgo de trabajo.', '2020-01-01', NULL, 1),
    (2, '02', N'Enfermedad en general.', '2020-01-01', NULL, 1),
    (3, '03', N'Maternidad.', '2020-01-01', NULL, 1),
    (4, '04', N'Licencia por cuidados médicos de hijos diagnosticados con cáncer.', '2020-01-01', NULL, 1);

    INSERT INTO dbo.CatReglaTipoIncapacidad (TipoIncapacidadId, PorcentajePago, DiasEspera, PagaPatron) VALUES
    (1, 100.00, 0, 0),
    (2, 60.00, 3, 1),
    (3, 100.00, 0, 0),
    (4, 60.00, 0, 0);
END
GO

-- 12. CatEstadoCFDI
IF NOT EXISTS (SELECT 1 FROM dbo.CatEstadoCFDI)
INSERT INTO dbo.CatEstadoCFDI (EstadoCFDIId, Clave, Descripcion, Activo) VALUES
(1,'PENDIENTE', 'Comprobante generado, pendiente de timbrado ante PAC', 1),
(2,'TIMBRADO',  'Timbrado exitoso, XML con UUID asignado', 1),
(3,'VIGENTE',   'CFDI vigente reportado en el SAT', 1),
(4,'CANCELADO', 'CFDI formalmente cancelado con acuse SAT', 1),
(5,'ERROR',     'Error durante el timbrado o validación con PAC', 1);
GO

-- 13. CatSatBanco
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatBanco)
INSERT INTO dbo.CatSatBanco (BancoId, Clave, Descripcion, RazonSocial, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(2,'002',N'BANAMEX',N'Banco Nacional de México, S.A., Institución de Banca Múltiple, Grupo Financiero Banamex','2017-01-01',NULL,1),
(6,'006',N'BANCOMEXT',N'Banco Nacional de Comercio Exterior, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo','2017-01-01',NULL,1),
(9,'009',N'BANOBRAS',N'Banco Nacional de Obras y Servicios Públicos, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo','2017-01-01',NULL,1),
(12,'012',N'BBVA BANCOMER',N'BBVA Bancomer, S.A., Institución de Banca Múltiple, Grupo Financiero BBVA Bancomer','2017-01-01',NULL,1),
(14,'014',N'SANTANDER',N'Banco Santander (México), S.A., Institución de Banca Múltiple, Grupo Financiero Santander','2017-01-01',NULL,1),
(19,'019',N'BANJERCITO',N'Banco Nacional del Ejército, Fuerza Aérea y Armada, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo','2017-01-01',NULL,1),
(21,'021',N'HSBC',N'HSBC México, S.A., institución De Banca Múltiple, Grupo Financiero HSBC','2017-01-01',NULL,1),
(30,'030',N'BAJIO',N'Banco del Bajío, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(32,'032',N'IXE',N'IXE Banco, S.A., Institución de Banca Múltiple, IXE Grupo Financiero','2017-01-01','2017-08-13',0),
(36,'036',N'INBURSA',N'Banco Inbursa, S.A., Institución de Banca Múltiple, Grupo Financiero Inbursa','2017-01-01',NULL,1),
(37,'037',N'INTERACCIONES',N'Banco Interacciones, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(42,'042',N'MIFEL',N'Banca Mifel, S.A., Institución de Banca Múltiple, Grupo Financiero Mifel','2017-01-01',NULL,1),
(44,'044',N'SCOTIABANK',N'Scotiabank Inverlat, S.A.','2017-01-01',NULL,1),
(58,'058',N'BANREGIO',N'Banco Regional de Monterrey, S.A., Institución de Banca Múltiple, Banregio Grupo Financiero','2017-01-01',NULL,1),
(59,'059',N'INVEX',N'Banco Invex, S.A., Institución de Banca Múltiple, Invex Grupo Financiero','2017-01-01',NULL,1),
(60,'060',N'BANSI',N'Bansi, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(62,'062',N'AFIRME',N'Banca Afirme, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(72,'072',N'BANORTE/IXE',N'Banco Mercantil del Norte, S.A., Institución de Banca Múltiple, Grupo Financiero Banorte','2017-01-01',NULL,1),
(102,'102',N'THE ROYAL BANK',N'The Royal Bank of Scotland México, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(103,'103',N'AMERICAN EXPRESS',N'American Express Bank (México), S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(106,'106',N'BAMSA',N'Bank of America México, S.A., Institución de Banca Múltiple, Grupo Financiero Bank of America','2017-01-01',NULL,1),
(108,'108',N'TOKYO',N'Bank of Tokyo-Mitsubishi UFJ (México), S.A.','2017-01-01',NULL,1),
(110,'110',N'JP MORGAN',N'Banco J.P. Morgan, S.A., Institución de Banca Múltiple, J.P. Morgan Grupo Financiero','2017-01-01',NULL,1),
(112,'112',N'BMONEX',N'Banco Monex, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(113,'113',N'VE POR MAS',N'Banco Ve Por Mas, S.A. Institución de Banca Múltiple','2017-01-01',NULL,1),
(116,'116',N'ING',N'ING Bank (México), S.A., Institución de Banca Múltiple, ING Grupo Financiero','2017-01-01','2017-08-13',0),
(124,'124',N'DEUTSCHE',N'Deutsche Bank México, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(126,'126',N'CREDIT SUISSE',N'Banco Credit Suisse (México), S.A. Institución de Banca Múltiple, Grupo Financiero Credit Suisse (México)','2017-01-01',NULL,1),
(127,'127',N'AZTECA',N'Banco Azteca, S.A. Institución de Banca Múltiple','2017-01-01',NULL,1),
(128,'128',N'AUTOFIN',N'Banco Autofin México, S.A. Institución de Banca Múltiple','2017-01-01',NULL,1),
(129,'129',N'BARCLAYS',N'Barclays Bank México, S.A., Institución de Banca Múltiple, Grupo Financiero Barclays México','2017-01-01',NULL,1),
(130,'130',N'COMPARTAMOS',N'Banco Compartamos, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(131,'131',N'BANCO FAMSA',N'Banco Ahorro Famsa, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(132,'132',N'BMULTIVA',N'Banco Multiva, S.A., Institución de Banca Múltiple, Multivalores Grupo Financiero','2017-01-01',NULL,1),
(133,'133',N'ACTINVER',N'Banco Actinver, S.A. Institución de Banca Múltiple, Grupo Financiero Actinver','2017-01-01',NULL,1),
(134,'134',N'WAL-MART',N'Banco Wal-Mart de México Adelante, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(135,'135',N'NAFIN',N'Nacional Financiera, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo','2017-01-01',NULL,1),
(136,'136',N'INTERBANCO',N'Inter Banco, S.A. Institución de Banca Múltiple','2017-01-01',NULL,1),
(137,'137',N'BANCOPPEL',N'BanCoppel, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(138,'138',N'ABC CAPITAL',N'ABC Capital, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(139,'139',N'UBS BANK',N'UBS Bank México, S.A., Institución de Banca Múltiple, UBS Grupo Financiero','2017-01-01',NULL,1),
(140,'140',N'CONSUBANCO',N'Consubanco, S.A. Institución de Banca Múltiple','2017-01-01',NULL,1),
(141,'141',N'VOLKSWAGEN',N'Volkswagen Bank, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(143,'143',N'CIBANCO',N'CIBanco, S.A.','2017-01-01',NULL,1),
(145,'145',N'BBASE',N'Banco Base, S.A., Institución de Banca Múltiple','2017-01-01',NULL,1),
(166,'166',N'BANSEFI',N'Banco del Ahorro Nacional y Servicios Financieros, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo','2017-01-01',NULL,1),
(168,'168',N'HIPOTECARIA FEDERAL',N'Sociedad Hipotecaria Federal, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo','2017-01-01',NULL,1),
(600,'600',N'MONEXCB',N'Monex Casa de Bolsa, S.A. de C.V. Monex Grupo Financiero','2017-01-01',NULL,1),
(601,'601',N'GBM',N'GBM Grupo Bursátil Mexicano, S.A. de C.V. Casa de Bolsa','2017-01-01',NULL,1),
(602,'602',N'MASARI',N'Masari Casa de Bolsa, S.A.','2017-01-01',NULL,1),
(605,'605',N'VALUE',N'Value, S.A. de C.V. Casa de Bolsa','2017-01-01',NULL,1),
(606,'606',N'ESTRUCTURADORES',N'Estructuradores del Mercado de Valores, S.A. de C.V., Casa de Bolsa','2017-01-01',NULL,1),
(607,'607',N'TIBER',N'Casa de Cambio Tiber, S.A. de C.V.','2017-01-01',NULL,1),
(608,'608',N'VECTOR',N'Vector Casa de Bolsa, S.A. de C.V.','2017-01-01',NULL,1),
(610,'610',N'B&B',N'B y B, Casa de Cambio, S.A. de C.V.','2017-01-01',NULL,1),
(614,'614',N'ACCIVAL',N'Acciones y Valores Banamex, S.A. de C.V., Casa de Bolsa','2017-01-01',NULL,1),
(615,'615',N'MERRILL LYNCH',N'Merrill Lynch México, S.A. de C.V. Casa de Bolsa','2017-01-01',NULL,1),
(616,'616',N'FINAMEX',N'Casa de Bolsa Finamex, S.A. de C.V.','2017-01-01',NULL,1),
(617,'617',N'VALMEX',N'Valores Mexicanos Casa de Bolsa, S.A. de C.V.','2017-01-01',NULL,1),
(618,'618',N'UNICA',N'Unica Casa de Cambio, S.A. de C.V.','2017-01-01',NULL,1),
(619,'619',N'MAPFRE',N'MAPFRE Tepeyac, S.A.','2017-01-01',NULL,1),
(620,'620',N'PROFUTURO',N'Profuturo G.N.P., S.A. de C.V., Afore','2017-01-01',NULL,1),
(621,'621',N'CB ACTINVER',N'Actinver Casa de Bolsa, S.A. de C.V.','2017-01-01',NULL,1),
(622,'622',N'OACTIN',N'OPERADORA ACTINVER, S.A. DE C.V.','2017-01-01',NULL,1),
(623,'623',N'SKANDIA',N'Skandia Vida, S.A. de C.V.','2017-01-01',NULL,1),
(626,'626',N'CBDEUTSCHE',N'Deutsche Securities, S.A. de C.V. CASA DE BOLSA','2017-01-01',NULL,1),
(627,'627',N'ZURICH',N'Zurich Compañía de Seguros, S.A.','2017-01-01',NULL,1),
(628,'628',N'ZURICHVI',N'Zurich Vida, Compañía de Seguros, S.A.','2017-01-01',NULL,1),
(629,'629',N'SU CASITA',N'Hipotecaria Su Casita, S.A. de C.V. SOFOM ENR','2017-01-01',NULL,1),
(630,'630',N'CB INTERCAM',N'Intercam Casa de Bolsa, S.A. de C.V.','2017-01-01',NULL,1),
(631,'631',N'CI BOLSA',N'CI Casa de Bolsa, S.A. de C.V.','2017-01-01',NULL,1),
(632,'632',N'BULLTICK C.B.',N'Bulltick Casa de Bolsa, S.A., de C.V.','2017-01-01',NULL,1),
(633,'633',N'STERLING',N'Sterling Casa de Cambio, S.A. de C.V.','2017-01-01',NULL,1),
(634,'634',N'FINCOMUN',N'Fincomún, Servicios Financieros Comunitarios, S.A. de C.V.','2017-01-01',NULL,1),
(636,'636',N'HDI SEGUROS',N'HDI Seguros, S.A. de C.V.','2017-01-01',NULL,1),
(637,'637',N'ORDER',N'OrderExpress Casa de Cambio, S.A. de C.V','2017-01-01',NULL,1),
(638,'638',N'AKALA',N'Akala, S.A. de C.V., Sociedad Financiera Popular','2017-01-01',NULL,1),
(640,'640',N'CB J.P. MORGAN',N'J.P. Morgan Casa de Bolsa, S.A. de C.V. J.P. Morgan Grupo Financiero','2017-01-01',NULL,1),
(642,'642',N'REFORMA',N'Operadora de Recursos Reforma, S.A. de C.V., S.F.P.','2017-01-01',NULL,1),
(646,'646',N'STP',N'Sistema de Transferencias y Pagos STP, S.A. de C.V.SOFOM ENR','2017-01-01',NULL,1),
(647,'647',N'TELECOMM',N'Telecomunicaciones de México, Organismo Público Descentralizado','2017-01-01',NULL,1),
(648,'648',N'EVERCORE',N'Evercore Casa de Bolsa, S.A. de C.V.','2017-01-01',NULL,1),
(649,'649',N'SKANDIA',N'Skandia Operadora de Fondos, S.A. de C.V.','2017-01-01',NULL,1),
(651,'651',N'SEGMTY',N'Seguros Monterrey New York Life, S.A de C.V.','2017-01-01',NULL,1),
(652,'652',N'ASEA',N'Solución Asea, S.A. de C.V., Sociedad Financiera Popular','2017-01-01',NULL,1),
(653,'653',N'KUSPIT',N'Kuspit Casa de Bolsa, S.A. de C.V.','2017-01-01',NULL,1),
(655,'655',N'SOFIEXPRESS',N'J.P. SOFIEXPRESS, S.A. de C.V., S.F.P.','2017-01-01',NULL,1),
(656,'656',N'UNAGRA',N'UNAGRA, S.A. de C.V., S.F.P.','2017-01-01',NULL,1),
(659,'659',N'OPCIONES EMPRESARIALES DEL NOROESTE',N'OPCIONES EMPRESARIALES DEL NORESTE, S.A. DE C.V., S.F.P.','2017-01-01',NULL,1),
(670,'670',N'LIBERTAD',N'Libertad Servicios Financieros, S.A. De C.V.','2017-01-01',NULL,1),
(901,'901',N'CLS',N'Cls Bank International','2017-01-01',NULL,1),
(902,'902',N'INDEVAL',N'SD. Indeval, S.A. de C.V.','2017-01-01',NULL,1);
GO

-- 14. CatFormaPagoDispersion
IF NOT EXISTS (SELECT 1 FROM dbo.CatFormaPagoDispersion)
INSERT INTO dbo.CatFormaPagoDispersion (MetodoPagoId, Clave, Descripcion, Activo) VALUES
(1, '01', N'Efectivo', 1),
(2, '02', N'Cheque nominativo', 1),
(3, '03', N'Transferencia electrónica de fondos', 1),
(99, '99', N'Por definir', 1);
GO

-- 15. CatEntidadFederativa
IF NOT EXISTS (SELECT 1 FROM dbo.CatEntidadFederativa)
INSERT INTO dbo.CatEntidadFederativa (CveEntidad, Nombre, Abreviatura, Activo) VALUES
('AGU','Aguascalientes','AGS',1),('BCN','Baja California','BC',1),('BCS','Baja California Sur','BCS',1),
('CAM','Campeche','CAMP',1),('COA','Coahuila','COAH',1),('COL','Colima','COL',1),
('CHP','Chiapas','CHIS',1),('CHH','Chihuahua','CHIH',1),('DIF','Ciudad de México','CDMX',1),
('DUR','Durango','DGO',1),('GUA','Guanajuato','GTO',1),('GRO','Guerrero','GRO',1),
('HID','Hidalgo','HGO',1),('JAL','Jalisco','JAL',1),('MEX','Estado de México','MEX',1),
('MIC','Michoacán','MICH',1),('MOR','Morelos','MOR',1),('NAY','Nayarit','NAY',1),
('NLE','Nuevo León','NL',1),('OAX','Oaxaca','OAX',1),('PUE','Puebla','PUE',1),
('QUE','Querétaro','QRO',1),('ROO','Quintana Roo','QROO',1),('SLP','San Luis Potosí','SLP',1),
('SIN','Sinaloa','SIN',1),('SON','Sonora','SON',1),('TAB','Tabasco','TAB',1),
('TAM','Tamaulipas','TAMPS',1),('TLA','Tlaxcala','TLAX',1),('VER','Veracruz','VER',1),
('YUC','Yucatán','YUC',1),('ZAC','Zacatecas','ZAC',1);
GO

-- 16. Catálogos CFDI 4.0 Generales (CatSatPais, CatSatEstado, CatSatCodigoPostal, etc.)
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatPais)
INSERT INTO dbo.CatSatPais (Clave, Descripcion, FechaInicioVigencia) VALUES
(N'MEX', N'México', '2017-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatEstado)
INSERT INTO dbo.CatSatEstado (Clave, ClavePais, Descripcion, FechaInicioVigencia) VALUES
(N'AGU', N'MEX', N'Aguascalientes', '2017-01-01'),
(N'BCN', N'MEX', N'Baja California', '2017-01-01'),
(N'BCS', N'MEX', N'Baja California Sur', '2017-01-01'),
(N'CAM', N'MEX', N'Campeche', '2017-01-01'),
(N'CHP', N'MEX', N'Chiapas', '2017-01-01'),
(N'CHH', N'MEX', N'Chihuahua', '2017-01-01'),
(N'COA', N'MEX', N'Coahuila', '2017-01-01'),
(N'COL', N'MEX', N'Colima', '2017-01-01'),
(N'DIF', N'MEX', N'Ciudad de México', '2017-01-01'),
(N'DUR', N'MEX', N'Durango', '2017-01-01'),
(N'GUA', N'MEX', N'Guanajuato', '2017-01-01'),
(N'GRO', N'MEX', N'Guerrero', '2017-01-01'),
(N'HID', N'MEX', N'Hidalgo', '2017-01-01'),
(N'JAL', N'MEX', N'Jalisco', '2017-01-01'),
(N'MEX', N'MEX', N'Estado de México', '2017-01-01'),
(N'MIC', N'MEX', N'Michoacán', '2017-01-01'),
(N'MOR', N'MEX', N'Morelos', '2017-01-01'),
(N'NAY', N'MEX', N'Nayarit', '2017-01-01'),
(N'NLE', N'MEX', N'Nuevo León', '2017-01-01'),
(N'OAX', N'MEX', N'Oaxaca', '2017-01-01'),
(N'PUE', N'MEX', N'Puebla', '2017-01-01'),
(N'QUE', N'MEX', N'Querétaro', '2017-01-01'),
(N'ROO', N'MEX', N'Quintana Roo', '2017-01-01'),
(N'SLP', N'MEX', N'San Luis Potosí', '2017-01-01'),
(N'SIN', N'MEX', N'Sinaloa', '2017-01-01'),
(N'SON', N'MEX', N'Sonora', '2017-01-01'),
(N'TAB', N'MEX', N'Tabasco', '2017-01-01'),
(N'TAM', N'MEX', N'Tamaulipas', '2017-01-01'),
(N'TLA', N'MEX', N'Tlaxcala', '2017-01-01'),
(N'VER', N'MEX', N'Veracruz', '2017-01-01'),
(N'YUC', N'MEX', N'Yucatán', '2017-01-01'),
(N'ZAC', N'MEX', N'Zacatecas', '2017-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatCodigoPostal WHERE Clave = N'79560')
INSERT INTO dbo.CatSatCodigoPostal (Clave, ClaveEstado, EstimuloFranjaFronteriza, FechaInicioVigencia, ClavePais) VALUES
(N'79560', N'SLP', 0, '2017-01-01', N'MEX');

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatCodigoPostal WHERE Clave = N'01000')
INSERT INTO dbo.CatSatCodigoPostal (Clave, ClaveEstado, EstimuloFranjaFronteriza, FechaInicioVigencia, ClavePais) VALUES
(N'01000', N'DIF', 0, '2017-01-01', N'MEX');

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatCodigoPostal WHERE Clave = N'00000')
INSERT INTO dbo.CatSatCodigoPostal (Clave, ClaveEstado, EstimuloFranjaFronteriza, FechaInicioVigencia, ClavePais) VALUES
(N'00000', N'AGU', 0, '2017-01-01', N'MEX');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatMetodoPago)
INSERT INTO dbo.CatSatMetodoPago (Clave, Descripcion, FechaInicioVigencia) VALUES
(N'PUE', N'Pago en una sola exhibición', '2017-01-01'),
(N'PPD', N'Pago en parcialidades o diferido', '2017-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatFormaPago)
INSERT INTO dbo.CatSatFormaPago (Clave, Descripcion, Bancarizado, FechaInicioVigencia) VALUES
(N'01', N'Efectivo', N'No', '2017-01-01'),
(N'02', N'Cheque nominativo', N'Sí', '2017-01-01'),
(N'03', N'Transferencia electrónica de fondos', N'Sí', '2017-01-01'),
(N'99', N'Por definir', N'No', '2017-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatMoneda)
INSERT INTO dbo.CatSatMoneda (Clave, Descripcion, Decimales, FechaInicioVigencia) VALUES
(N'MXN', N'Peso Mexicano', 2, '2017-01-01'),
(N'USD', N'Dólar estadounidense', 2, '2017-01-01'),
(N'XXX', N'Los códigos asignados para las transacciones en que intervenga ninguna moneda', 0, '2017-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoDeComprobante)
INSERT INTO dbo.CatSatTipoDeComprobante (Clave, Descripcion, FechaInicioVigencia, ValorMaximoNS, ValorMaximoNdS) VALUES
(N'I', N'Ingreso', '2017-01-01', 0, 0),
(N'E', N'Egreso', '2017-01-01', 0, 0),
(N'T', N'Traslado', '2017-01-01', 0, 0),
(N'N', N'Nómina', '2017-01-01', 0, 0),
(N'P', N'Pago', '2017-01-01', 0, 0);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatExportacion)
INSERT INTO dbo.CatSatExportacion (Clave, Descripcion, FechaInicioVigencia) VALUES
(N'01', N'No aplica', '2022-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatUsoCFDI)
INSERT INTO dbo.CatSatUsoCFDI (Clave, Descripcion, AplicaPersonaFisica, AplicaPersonaMoral, FechaInicioVigencia) VALUES
(N'CN01', N'Nómina', 1, 0, '2022-01-01'),
(N'G01', N'Adquisición de mercancías', 1, 1, '2022-01-01'),
(N'G03', N'Gastos en general', 1, 1, '2022-01-01'),
(N'CP01', N'Pagos', 1, 1, '2022-01-01'),
(N'S01', N'Sin efectos fiscales', 1, 1, '2022-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.RelSatUsoCFDIRegimenFiscal)
INSERT INTO dbo.RelSatUsoCFDIRegimenFiscal (ClaveUsoCFDI, ClaveRegimenFiscal) VALUES
(N'CN01', N'605');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatClaveProdServ)
INSERT INTO dbo.CatSatClaveProdServ (Clave, Descripcion, IncluirIVATrasladado, IncluirIEPSTrasladado, EstimuloFranjaFronteriza, FechaInicioVigencia) VALUES
(N'84111505', N'Servicios de contabilidad de sueldos y salarios', N'No', N'No', 0, '2017-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatClaveUnidad)
INSERT INTO dbo.CatSatClaveUnidad (Clave, Nombre, FechaInicioVigencia) VALUES
(N'ACT', N'Actividad', '2017-01-01'),
(N'DIA', N'Día',       '2017-01-01'),
(N'HUR', N'Hora',      '2017-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatObjetoImp)
INSERT INTO dbo.CatSatObjetoImp (Clave, Descripcion, FechaInicioVigencia) VALUES
(N'01', N'No objeto de impuesto', '2022-01-01'),
(N'02', N'Sí objeto de impuesto', '2022-01-01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoRelacion)
INSERT INTO dbo.CatSatTipoRelacion (Clave, Descripcion, FechaInicioVigencia) VALUES
(N'01', N'Nota de crédito de los documentos relacionados', '2017-01-01'),
(N'04', N'Sustitución de los CFDI previos', '2017-01-01');
GO

-- 17. CatSatTipoPercepcion
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoPercepcion)
INSERT INTO dbo.CatSatTipoPercepcion (TipoPercepcionId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(1,'001',N'Sueldos, Salarios Rayas y Jornales','2016-11-01',NULL,1),
(2,'002',N'Gratificación Anual (Aguinaldo)','2016-11-01',NULL,1),
(3,'003',N'Participación de los Trabajadores en las Utilidades PTU','2016-11-01',NULL,1),
(4,'004',N'Reembolso de Gastos Médicos Dentales y Hospitalarios','2016-11-01',NULL,1),
(5,'005',N'Fondo de Ahorro','2016-11-01',NULL,1),
(6,'006',N'Caja de ahorro','2016-11-01',NULL,1),
(9,'009',N'Contribuciones a Cargo del Trabajador Pagadas por el Patrón','2016-11-01',NULL,1),
(10,'010',N'Premios por puntualidad','2016-11-01',NULL,1),
(11,'011',N'Prima de Seguro de vida','2016-11-01',NULL,1),
(12,'012',N'Seguro de Gastos Médicos Mayores','2016-11-01',NULL,1),
(13,'013',N'Cuotas Sindicales Pagadas por el Patrón','2016-11-01',NULL,1),
(14,'014',N'Subsidios por incapacidad','2016-11-01',NULL,1),
(15,'015',N'Becas para trabajadores y/o hijos','2016-11-01',NULL,1),
(19,'019',N'Horas extra','2016-11-01',NULL,1),
(20,'020',N'Prima dominical','2016-11-01',NULL,1),
(21,'021',N'Prima vacacional','2016-11-01',NULL,1),
(22,'022',N'Prima por antigüedad','2016-11-01',NULL,1),
(23,'023',N'Pagos por separación','2016-11-01',NULL,1),
(24,'024',N'Seguro de retiro','2016-11-01',NULL,1),
(25,'025',N'Indemnizaciones','2016-11-01',NULL,1),
(26,'026',N'Reembolso por funeral','2016-11-01',NULL,1),
(27,'027',N'Cuotas de seguridad social pagadas por el patrón','2016-11-01',NULL,1),
(28,'028',N'Comisiones','2016-11-01',NULL,1),
(29,'029',N'Vales de despensa','2016-11-01',NULL,1),
(30,'030',N'Vales de restaurante','2016-11-01',NULL,1),
(31,'031',N'Vales de gasolina','2016-11-01',NULL,1),
(32,'032',N'Vales de ropa','2016-11-01',NULL,1),
(33,'033',N'Ayuda para renta','2016-11-01',NULL,1),
(34,'034',N'Ayuda para artículos escolares','2016-11-01',NULL,1),
(35,'035',N'Ayuda para anteojos','2016-11-01',NULL,1),
(36,'036',N'Ayuda para transporte','2016-11-01',NULL,1),
(37,'037',N'Ayuda para gastos de funeral','2016-11-01',NULL,1),
(38,'038',N'Otros ingresos por salarios','2016-11-01',NULL,1),
(39,'039',N'Jubilaciones, pensiones o haberes de retiro','2016-11-01',NULL,1),
(44,'044',N'Jubilaciones, pensiones o haberes de retiro en parcialidades','2016-11-01',NULL,1),
(45,'045',N'Ingresos en acciones o títulos valor que representan bienes','2016-11-01',NULL,1),
(46,'046',N'Ingresos asimilados a salarios','2016-11-01',NULL,1),
(47,'047',N'Alimentación diferentes a los establecidos en el Art 94 penúltimo párrafo LISR','2016-11-01',NULL,1),
(48,'048',N'Habitación','2016-11-01',NULL,1),
(49,'049',N'Premios por asistencia','2016-11-01',NULL,1),
(50,'050',N'Viáticos','2017-01-06',NULL,1),
(51,'051',N'Pagos por gratificaciones, primas, compensaciones, recompensas u otros a extrabajadores derivados de jubilación en parcialidades','2018-10-15',NULL,1),
(52,'052',N'Pagos que se realicen a extrabajadores que obtengan una jubilación en parcialidades derivados de la ejecución de resoluciones judicial o de un laudo','2018-10-15',NULL,1),
(53,'053',N'Pagos que se realicen a extrabajadores que obtengan una jubilación en una sola exhibición derivados de la ejecución de resoluciones judicial o de un laudo','2018-10-15',NULL,1),
(54,'054',N'Días de descanso laborados','2026-01-01',NULL,1),
(55,'055',N'Días de descanso obligatorios laborados','2026-01-01',NULL,1),
(56,'056',N'Previsión social, Art 93 fracciones VIII y IX LISR','2025-12-17',NULL,1),
(57,'057',N'Premios por concursos científicos, artísticos, literarios u otorgados por la federación Art 93 fracción XXIV LISR','2026-06-12',NULL,1);
GO

-- 18. CatSatTipoDeduccion
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoDeduccion)
INSERT INTO dbo.CatSatTipoDeduccion (TipoDeduccionId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(1,'001',N'Seguridad social','2016-11-01',NULL,1),
(2,'002',N'ISR','2016-11-01',NULL,1),
(3,'003',N'Aportaciones a retiro, cesantía en edad avanzada y vejez.','2016-11-01',NULL,1),
(4,'004',N'Otros','2016-11-01',NULL,1),
(5,'005',N'Aportaciones a Fondo de vivienda','2016-11-01',NULL,1),
(6,'006',N'Descuento por incapacidad','2016-11-01',NULL,1),
(7,'007',N'Pensión alimenticia','2016-11-01',NULL,1),
(8,'008',N'Renta','2016-11-01',NULL,1),
(9,'009',N'Préstamos provenientes del Fondo Nacional de la Vivienda para los Trabajadores','2016-11-01',NULL,1),
(10,'010',N'Pago por crédito de vivienda','2016-11-01',NULL,1),
(11,'011',N'Pago de abonos INFONACOT','2016-11-01',NULL,1),
(12,'012',N'Anticipo de salarios','2016-11-01',NULL,1),
(13,'013',N'Pagos hechos con exceso al trabajador','2016-11-01',NULL,1),
(14,'014',N'Errores','2016-11-01',NULL,1),
(15,'015',N'Pérdidas','2016-11-01',NULL,1),
(16,'016',N'Averías','2016-11-01',NULL,1),
(17,'017',N'Adquisición de artículos producidos por la empresa o establecimiento','2016-11-01',NULL,1),
(18,'018',N'Cuotas para la constitución y fomento de sociedades cooperativas y de cajas de ahorro','2016-11-01',NULL,1),
(19,'019',N'Cuotas sindicales','2016-11-01',NULL,1),
(20,'020',N'Ausencia (Ausentismo)','2016-11-01',NULL,1),
(21,'021',N'Cuotas obrero patronales','2016-11-01',NULL,1),
(22,'022',N'Impuestos Locales','2016-11-01',NULL,1),
(23,'023',N'Aportaciones voluntarias','2016-11-01',NULL,1),
(24,'024',N'Ajuste en Gratificación Anual (Aguinaldo) Exento','2017-01-06',NULL,1),
(25,'025',N'Ajuste en Gratificación Anual (Aguinaldo) Gravado','2017-01-06',NULL,1),
(26,'026',N'Ajuste en Participación de los Trabajadores en las Utilidades PTU Exento','2017-01-06',NULL,1),
(27,'027',N'Ajuste en Participación de los Trabajadores en las Utilidades PTU Gravado','2017-01-06',NULL,1),
(28,'028',N'Ajuste en Reembolso de Gastos Médicos Dentales y Hospitalarios Exento','2017-01-06',NULL,1),
(29,'029',N'Ajuste en Fondo de ahorro Exento','2017-01-06',NULL,1),
(30,'030',N'Ajuste en Caja de ahorro Exento','2017-01-06',NULL,1),
(31,'031',N'Ajuste en Contribuciones a Cargo del Trabajador Pagadas por el Patrón Exento','2017-01-06',NULL,1),
(32,'032',N'Ajuste en Premios por puntualidad Gravado','2017-01-06',NULL,1),
(33,'033',N'Ajuste en Prima de Seguro de vida Exento','2017-01-06',NULL,1),
(34,'034',N'Ajuste en Seguro de Gastos Médicos Mayores Exento','2017-01-06',NULL,1),
(35,'035',N'Ajuste en Cuotas Sindicales Pagadas por el Patrón Exento','2017-01-06',NULL,1),
(36,'036',N'Ajuste en Subsidios por incapacidad Exento','2017-01-06',NULL,1),
(37,'037',N'Ajuste en Becas para trabajadores y/o hijos Exento','2017-01-06',NULL,1),
(38,'038',N'Ajuste en Horas extra Exento','2017-01-06',NULL,1),
(39,'039',N'Ajuste en Horas extra Gravado','2017-01-06',NULL,1),
(40,'040',N'Ajuste en Prima dominical Exento','2017-01-06',NULL,1),
(41,'041',N'Ajuste en Prima dominical Gravado','2017-01-06',NULL,1),
(42,'042',N'Ajuste en Prima vacacional Exento','2017-01-06',NULL,1),
(43,'043',N'Ajuste en Prima vacacional Gravado','2017-01-06',NULL,1),
(44,'044',N'Ajuste en Prima por antigüedad Exento','2017-01-06',NULL,1),
(45,'045',N'Ajuste en Prima por antigüedad Gravado','2017-01-06',NULL,1),
(46,'046',N'Ajuste en Pagos por separación Exento','2017-01-06',NULL,1),
(47,'047',N'Ajuste en Pagos por separación Gravado','2017-01-06',NULL,1),
(48,'048',N'Ajuste en Seguro de retiro Exento','2017-01-06',NULL,1),
(49,'049',N'Ajuste en Indemnizaciones Exento','2017-01-06',NULL,1),
(50,'050',N'Ajuste en Indemnizaciones Gravado','2017-01-06',NULL,1),
(51,'051',N'Ajuste en Reembolso por funeral Exento','2017-01-06',NULL,1),
(52,'052',N'Ajuste en Cuotas de seguridad social pagadas por el patrón Exento','2017-01-06',NULL,1),
(53,'053',N'Ajuste en Comisiones Gravado','2017-01-06',NULL,1),
(54,'054',N'Ajuste en Vales de despensa Exento','2017-01-06',NULL,1),
(55,'055',N'Ajuste en Vales de restaurante Exento','2017-01-06',NULL,1),
(56,'056',N'Ajuste en Vales de gasolina Exento','2017-01-06',NULL,1),
(57,'057',N'Ajuste en Vales de ropa Exento','2017-01-06',NULL,1),
(58,'058',N'Ajuste en Ayuda para renta Exento','2017-01-06',NULL,1),
(59,'059',N'Ajuste en Ayuda para artículos escolares Exento','2017-01-06',NULL,1),
(60,'060',N'Ajuste en Ayuda para anteojos Exento','2017-01-06',NULL,1),
(61,'061',N'Ajuste en Ayuda para transporte Exento','2017-01-06',NULL,1),
(62,'062',N'Ajuste en Ayuda para gastos de funeral Exento','2017-01-06',NULL,1),
(63,'063',N'Ajuste en Otros ingresos por salarios Exento','2017-01-06',NULL,1),
(64,'064',N'Ajuste en Otros ingresos por salarios Gravado','2017-01-06',NULL,1),
(65,'065',N'Ajuste en Jubilaciones, pensiones o haberes de retiro en una sola exhibición Exento','2017-01-06',NULL,1),
(66,'066',N'Ajuste en Jubilaciones, pensiones o haberes de retiro en una sola exhibición Gravado','2017-01-06',NULL,1),
(67,'067',N'Ajuste en Pagos por separación Acumulable','2017-01-06',NULL,1),
(68,'068',N'Ajuste en Pagos por separación No acumulable','2017-01-06',NULL,1),
(69,'069',N'Ajuste en Jubilaciones, pensiones o haberes de retiro en parcialidades Exento','2018-10-15',NULL,1),
(70,'070',N'Ajuste en Jubilaciones, pensiones o haberes de retiro en parcialidades Gravado','2018-10-15',NULL,1),
(71,'071',N'Ajuste en Subsidio para el empleo (efectivamente entregado al trabajador)','2017-01-06',NULL,1),
(72,'072',N'Ajuste en Ingresos en acciones o títulos valor que representan bienes Exento','2017-01-06','2018-10-14',0),
(73,'073',N'Ajuste en Ingresos en acciones o títulos valor que representan bienes Gravado','2017-01-06',NULL,1),
(74,'074',N'Ajuste en Alimentación Exento','2017-01-06',NULL,1),
(75,'075',N'Ajuste en Alimentación Gravado','2017-01-06',NULL,1),
(76,'076',N'Ajuste en Habitación Exento','2017-01-06',NULL,1),
(77,'077',N'Ajuste en Habitación Gravado','2017-01-06',NULL,1),
(78,'078',N'Ajuste en Premios por asistencia','2017-01-06',NULL,1),
(79,'079',N'Ajuste en Pagos distintos a los listados y que no deben considerarse como ingreso por sueldos, salarios o ingresos asimilados.','2017-01-06',NULL,1),
(80,'080',N'Ajuste en Viáticos gravados','2017-01-06',NULL,1),
(81,'081',N'Ajuste en Viáticos (entregados al trabajador)','2017-01-06',NULL,1),
(82,'082',N'Ajuste en Fondo de ahorro Gravado','2017-01-06',NULL,1),
(83,'083',N'Ajuste en Caja de ahorro Gravado','2017-01-06',NULL,1),
(84,'084',N'Ajuste en Prima de Seguro de vida Gravado','2017-01-06',NULL,1),
(85,'085',N'Ajuste en Seguro de Gastos Médicos Mayores Gravado','2017-01-06',NULL,1),
(86,'086',N'Ajuste en Subsidios por incapacidad Gravado','2017-01-06',NULL,1),
(87,'087',N'Ajuste en Becas para trabajadores y/o hijos Gravado','2017-01-06',NULL,1),
(88,'088',N'Ajuste en Seguro de retiro Gravado','2017-01-06',NULL,1),
(89,'089',N'Ajuste en Vales de despensa Gravado','2017-01-06',NULL,1),
(90,'090',N'Ajuste en Vales de restaurante Gravado','2017-01-06',NULL,1),
(91,'091',N'Ajuste en Vales de gasolina Gravado','2017-01-06',NULL,1),
(92,'092',N'Ajuste en Vales de ropa Gravado','2017-01-06',NULL,1),
(93,'093',N'Ajuste en Ayuda para renta Gravado','2017-01-06',NULL,1),
(94,'094',N'Ajuste en Ayuda para artículos escolares Gravado','2017-01-06',NULL,1),
(95,'095',N'Ajuste en Ayuda para anteojos Gravado','2017-01-06',NULL,1),
(96,'096',N'Ajuste en Ayuda para transporte Gravado','2017-01-06',NULL,1),
(97,'097',N'Ajuste en Ayuda para gastos de funeral Gravado','2017-01-06',NULL,1),
(98,'098',N'Ajuste a ingresos asimilados a salarios gravados','2017-01-06',NULL,1),
(99,'099',N'Ajuste a ingresos por sueldos y salarios gravados','2017-01-06',NULL,1),
(100,'100',N'Ajuste en Viáticos exentos','2017-01-06',NULL,1),
(101,'101',N'ISR Retenido de ejercicio anterior','2017-12-05',NULL,1),
(102,'102',N'Ajuste a pagos por gratificaciones, primas, compensaciones, recompensas u otros a extrabajadores derivados de jubilación en parcialidades, gravados','2018-10-15',NULL,1),
(103,'103',N'Ajuste a pagos que se realicen a extrabajadores que obtengan una jubilación en parcialidades derivados de la ejecución de una resolución judicial o de un laudo gravados','2018-10-15',NULL,1),
(104,'104',N'Ajuste a pagos que se realicen a extrabajadores que obtengan una jubilación en parcialidades derivados de la ejecución de una resolución judicial o de un laudo exentos','2018-10-15',NULL,1),
(105,'105',N'Ajuste a pagos que se realicen a extrabajadores que obtengan una jubilación en una sola exhibición derivados de la ejecución de una resolución judicial o de un laudo gravados','2018-10-15',NULL,1),
(106,'106',N'Ajuste a pagos que se realicen a extrabajadores que obtengan una jubilación en una sola exhibición derivados de la ejecución de una resolución judicial o de un laudo exentos','2018-10-15',NULL,1),
(107,'107',N'Ajuste al Subsidio Causado','2020-01-01',NULL,1),
(108,'108',N'Ajuste a días de descanso laborados gravados','2026-01-01',NULL,1),
(109,'109',N'Ajuste a días de descanso laborados exentos','2026-01-01',NULL,1),
(110,'110',N'Ajuste a días de descanso obligatorios laborados gravados','2026-01-01',NULL,1),
(111,'111',N'Ajuste a días de descanso obligatorios laborados exentos','2026-01-01',NULL,1),
(112,'112',N'Ajuste previsión social Art 93 fracciones VIII y IX LISR gravados','2025-12-17',NULL,1),
(113,'113',N'Ajuste previsión social Art 93 fracciones VIII y IX LISR exentos','2025-12-17',NULL,1),
(114,'114',N'Ajuste a premios por concursos científicos, artísticos, literarios u otorgados por la federación Art 93 fracción XXIV LISR gravados','2026-06-12',NULL,1),
(115,'115',N'Ajuste a premios por concursos científicos, artísticos, literarios u otorgados por la federación Art 93 fracción XXIV LISR exentos','2026-06-12',NULL,1);
GO

-- 19. CatSatTipoOtroPago
IF NOT EXISTS (SELECT 1 FROM dbo.CatSatTipoOtroPago)
INSERT INTO dbo.CatSatTipoOtroPago (TipoOtroPagoId, Clave, Descripcion, FechaInicioVigencia, FechaFinVigencia, Activo) VALUES
(1,'001',N'Reintegro de ISR retenido en exceso (siempre que no haya sido enterado al SAT).','2017-01-01',NULL,1),
(2,'002',N'Subsidio para el empleo (efectivamente entregado al trabajador).','2017-01-01',NULL,1),
(3,'003',N'Viáticos (entregados al trabajador).','2017-01-01',NULL,1),
(4,'004',N'Aplicación de saldo a favor por compensación anual.','2017-01-01',NULL,1),
(5,'005',N'Reintegro de ISR retenido en exceso de ejercicio anterior (siempre que no haya sido enterado al SAT).','2017-12-05',NULL,1),
(6,'006',N'Alimentos en bienes (Servicios de comedor y comida) Art 94 penúltimo párrafo LISR.','2020-01-01',NULL,1),
(7,'007',N'ISR ajustado por subsidio.','2020-01-01',NULL,1),
(8,'008',N'Subsidio efectivamente entregado que no correspondía (Aplica sólo cuando haya ajuste al cierre de mes en relación con el Apéndice 7 de la guía de llenado de nómina).','2020-01-01',NULL,1),
(9,'009',N'Reembolso de descuentos efectuados para el crédito de vivienda.','2020-04-20',NULL,1),
(999,'999',N'Pagos distintos a los listados y que no deben considerarse como ingreso por sueldos, salarios o ingresos asimilados.','2017-01-01',NULL,1);
GO

-- 20. CatUMA
IF NOT EXISTS (SELECT 1 FROM dbo.CatUMA)
INSERT INTO dbo.CatUMA (Anio, ValorDiario, ValorMensual, ValorAnual, VigenciaDesde, VigenciaHasta) VALUES
(2022,  96.22, 2925.09, 35101.08, '2022-02-01', '2023-01-31'),
(2023, 103.74, 3153.70, 37844.40, '2023-02-01', '2024-01-31'),
(2024, 108.57, 3300.53, 39606.36, '2024-02-01', '2025-01-31'),
(2025, 113.14, 3439.26, 41271.12, '2025-02-01', '2026-01-31'),
(2026, 118.85, 3611.86, 43342.32, '2026-02-01', '2027-01-31');
GO

-- 21. CatSalarioMinimo
IF NOT EXISTS (SELECT 1 FROM dbo.CatSalarioMinimo)
INSERT INTO dbo.CatSalarioMinimo (Anio, Zona, ValorDiario, VigenciaDesde, VigenciaHasta) VALUES
(2023,'GENERAL',207.44,'2023-01-01','2023-12-31'),(2023,'ZFN',312.41,'2023-01-01','2023-12-31'),
(2024,'GENERAL',248.93,'2024-01-01','2024-12-31'),(2024,'ZFN',374.89,'2024-01-01','2024-12-31'),
(2025,'GENERAL',278.80,'2025-01-01','2025-12-31'),(2025,'ZFN',419.88,'2025-01-01','2025-12-31'),
(2026,'GENERAL',312.42,'2026-01-01','2026-12-31'),(2026,'ZFN',470.28,'2026-01-01','2026-12-31');
GO

-- 22. CatTablaVacaciones (Escala legal Vacaciones Dignas Art. 76 LFT)
IF NOT EXISTS (SELECT 1 FROM dbo.CatTablaVacaciones)
INSERT INTO dbo.CatTablaVacaciones (AniosServicioDesde, AniosServicioHasta, DiasVacaciones) VALUES
(1,1,12),(2,2,14),(3,3,16),(4,4,18),(5,5,20),
(6,10,22),(11,15,24),(16,20,26),(21,25,28),(26,30,30),(31,35,32),(36,99,34);
GO

-- 23. CatRol y CatPermiso
IF NOT EXISTS (SELECT 1 FROM dbo.CatRol)
INSERT INTO dbo.CatRol (Codigo, Descripcion, Activo) VALUES
('SUPERADMIN',    'Administrador con acceso total al sistema', 1),
('RH_NOMINA',     'Recursos Humanos - Cálculo y captura de nómina', 1),
('DIR_FINANZAS',  'Dirección de Finanzas - Autorización de nómina', 1),
('CONTABILIDAD',  'Contabilidad - Timbrado y consulta de CFDI', 1),
('CONSULTA',      'Solo consulta (dashboard/reportes)', 1);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CatPermiso)
INSERT INTO dbo.CatPermiso (Codigo, Descripcion, Modulo, Activo) VALUES
('NOMINA.CALCULAR',    'Ejecutar sp_CalcularNomina',                 'NOMINA', 1),
('NOMINA.AUTORIZAR',   'Ejecutar sp_AutorizarPeriodo',               'NOMINA', 1),
('NOMINA.CERRAR',      'Ejecutar sp_CerrarPeriodo',                  'NOMINA', 1),
('NOMINA.RECALCULAR',  'Reversar y recalcular un periodo abierto',   'NOMINA', 1),
('EMPLEADO.ALTA',      'Alta de empleados y datos fiscales',         'EMPLEADO', 1),
('EMPLEADO.EDITAR',    'Edición de datos de empleado',               'EMPLEADO', 1),
('EMPLEADO.BAJA',      'Baja y finiquito de empleado',               'EMPLEADO', 1),
('SALARIO.ACTUALIZAR', 'Ejecutar sp_ActualizarSBC',                  'EMPLEADO', 1),
('CFDI.TIMBRAR',       'Timbrar recibos ante PAC',                   'CFDI', 1),
('CFDI.CANCELAR',      'Cancelar CFDI vigente',                      'CFDI', 1),
('POLITICA.EDITAR',    'Editar políticas empresariales y fórmulas',  'CONFIGURACION', 1),
('CATALOGO.EDITAR',    'Editar catálogos maestros',                  'CONFIGURACION', 1),
('REPORTE.CONSULTAR',  'Consultar reportes y vistas',                'REPORTES', 1),
('PERIODO.GENERAR',    'Generar periodos del ejercicio y administrar días festivos', 'NOMINA', 1);
GO

-- 24. CatSistema
IF NOT EXISTS (SELECT 1 FROM dbo.CatSistema)
INSERT INTO dbo.CatSistema (Clave, Valor, Descripcion) VALUES
('PAC.ProveedorNombre',       NULL, 'Nombre comercial del PAC contratado'),
('PAC.RFC',                   NULL, 'RFC del Proveedor Autorizado de Certificación'),
('PAC.EndpointTimbrado',      NULL, 'URL del webservice de timbrado del PAC'),
('PAC.EndpointCancelacion',   NULL, 'URL del webservice de cancelación del PAC'),
('PAC.Usuario',               NULL, 'Usuario para autenticación con el PAC'),
('PAC.PasswordCifrado',       NULL, 'Password del PAC cifrada (AES + IV)'),
('CFDI.SerieDefault',         'N',  'Serie por defecto para recibos de nómina'),
('CFDI.FolioSiguiente',       '1',  'Contador atómico del siguiente folio'),
('CFDI.LugarExpedicionDef',   NULL, 'CP por defecto para LugarExpedicion'),
('SEGURIDAD.MaxIntentosLogin','5',  'Intentos fallidos antes de bloquear usuario'),
('SEGURIDAD.MinutosSesion',   '480','Duración máxima de la sesión en minutos'),
('NOTIFICACION.EmailFrom',    NULL, 'Remitente para envío de recibos por correo'),
('NOTIFICACION.SMTPHost',     NULL, 'Servidor SMTP de salida'),
('NOTIFICACION.SMTPPort',     '587','Puerto SMTP'),
('EMPRESA.ZonaHoraria',       'America/Mexico_City', 'Zona horaria para timestamps de UI');
GO

-- 25. CatConcepto (Catálogo Unificado de Conceptos de Nómina)
IF NOT EXISTS (SELECT 1 FROM dbo.CatConcepto)
BEGIN
    -- (a) Claves de uso común y cálculo automático
    INSERT INTO dbo.CatConcepto (ClaveSAT, TipoConcepto, Descripcion, EsGravado, TopeUMAAnual, TipoPercepcionId, TipoDeduccionId, TipoOtroPagoId)
    SELECT v.ClaveSAT, v.TipoConcepto, v.Descripcion, v.EsGravado, v.TopeUMAAnual, p.TipoPercepcionId, d.TipoDeduccionId, o.TipoOtroPagoId
    FROM (VALUES
        ('001','P',N'Sueldos, Salarios Rayas y Jornales',             1, NULL),
        ('002','P',N'Gratificación Anual (Aguinaldo)',                1, 30.00),
        ('003','P',N'Participación de los Trabajadores en Utilidades',1, 15.00),
        ('019','P',N'Horas extra',                                   1, NULL),
        ('020','P',N'Prima dominical',                                1, 1.00),
        ('021','P',N'Prima vacacional',                               1, 15.00),
        ('022','P',N'Prima por antigüedad',                           1, NULL),
        ('025','P',N'Indemnizaciones',                                1, 90.00),
        ('028','P',N'Comisiones',                                     1, NULL),
        ('029','P',N'Vales de despensa',                              0, NULL),
        ('038','P',N'Otros ingresos por salarios (bonos)',            1, NULL),
        ('001','D',N'Seguridad social (cuota obrera IMSS)',           0, NULL),
        ('002','D',N'ISR retenido',                                   0, NULL),
        ('004','D',N'Otros descuentos',                               0, NULL),
        ('007','D',N'Pensión alimenticia',                            0, NULL),
        ('009','D',N'Préstamos INFONAVIT',                            0, NULL),
        ('010','D',N'Pago por crédito de vivienda',                   0, NULL),
        ('011','D',N'Pago de abonos INFONACOT',                       0, NULL),
        ('012','D',N'Anticipo de salarios',                           0, NULL),
        ('001','O',N'Reintegro de ISR pagado en exceso',              0, NULL),
        ('002','O',N'Subsidio para el Empleo Efectivamente Entregado',0, NULL),
        ('003','O',N'Viáticos (entregados al trabajador)',            0, NULL),
        ('004','O',N'Aplicación saldo a favor por compensación anual',0, NULL)
    ) v (ClaveSAT, TipoConcepto, Descripcion, EsGravado, TopeUMAAnual)
    LEFT JOIN dbo.CatSatTipoPercepcion p ON v.TipoConcepto = 'P' AND p.Clave = v.ClaveSAT
    LEFT JOIN dbo.CatSatTipoDeduccion  d ON v.TipoConcepto = 'D' AND d.Clave = v.ClaveSAT
    LEFT JOIN dbo.CatSatTipoOtroPago   o ON v.TipoConcepto = 'O' AND o.Clave = v.ClaveSAT;

    -- (b) Resto de claves SAT de percepciones, deducciones y otros pagos
    INSERT INTO dbo.CatConcepto (ClaveSAT, TipoConcepto, Descripcion, EsGravado, TipoPercepcionId, Activo)
    SELECT p.Clave, 'P', LEFT(p.Descripcion, 150), 1, p.TipoPercepcionId, p.Activo FROM dbo.CatSatTipoPercepcion p
    WHERE NOT EXISTS (SELECT 1 FROM dbo.CatConcepto c WHERE c.ClaveSAT = p.Clave AND c.TipoConcepto = 'P');

    INSERT INTO dbo.CatConcepto (ClaveSAT, TipoConcepto, Descripcion, EsGravado, TipoDeduccionId, Activo)
    SELECT d.Clave, 'D', LEFT(d.Descripcion, 150), 1, d.TipoDeduccionId, d.Activo FROM dbo.CatSatTipoDeduccion d
    WHERE NOT EXISTS (SELECT 1 FROM dbo.CatConcepto c WHERE c.ClaveSAT = d.Clave AND c.TipoConcepto = 'D');

    INSERT INTO dbo.CatConcepto (ClaveSAT, TipoConcepto, Descripcion, EsGravado, TipoOtroPagoId, Activo)
    SELECT o.Clave, 'O', LEFT(o.Descripcion, 150), 0, o.TipoOtroPagoId, o.Activo FROM dbo.CatSatTipoOtroPago o
    WHERE NOT EXISTS (SELECT 1 FROM dbo.CatConcepto c WHERE c.ClaveSAT = o.Clave AND c.TipoConcepto = 'O');

    -- Nota: los conceptos internos de incidencia (tipo 'I': FALTA, HE_DOBLE, PRIMA_DOM, BONO, etc.) se retiraron
    -- del catálogo. dbo.CatConcepto contiene únicamente claves oficiales del SAT (percepciones, deducciones y otros pagos).
END
GO

-- Denominación (unidad de medida SAT) de los conceptos precargados de la nómina general.
-- Por defecto ACT (actividad/monto); se marcan en DÍA y HORA los que así se cuantifican.
UPDATE dbo.CatConcepto SET ClaveUnidadSAT = N'HUR' WHERE ClaveSAT = '019' AND TipoConcepto = 'P';            -- Horas extra -> hora
UPDATE dbo.CatConcepto SET ClaveUnidadSAT = N'DIA' WHERE TipoConcepto = 'P' AND ClaveSAT IN ('001','002','020','021'); -- Sueldo, aguinaldo, prima dominical, prima vacacional -> día
GO

-- Conceptos base obligatorios del recibo de nómina
UPDATE c SET EsBase = 1
FROM dbo.CatConcepto c
WHERE (c.ClaveSAT = '001' AND c.TipoConcepto = 'P')
   OR (c.ClaveSAT = '001' AND c.TipoConcepto = 'D')
   OR (c.ClaveSAT = '002' AND c.TipoConcepto = 'D')
   OR (c.ClaveSAT = '002' AND c.TipoConcepto = 'O');
GO

-- Conceptos capturables como incidencia por defecto
UPDATE c SET EsIncidencia = 1, FormaCalculo = 'MONTO_FIJO', FactorMultiplicador = 1
FROM dbo.CatConcepto c
WHERE c.Activo = 1 AND ((c.ClaveSAT = '036' AND c.TipoConcepto = 'P')
                     OR (c.ClaveSAT = '038' AND c.TipoConcepto = 'P')
                     OR (c.ClaveSAT = '004' AND c.TipoConcepto = 'D'));
GO

-- Conceptos SAT manejados por el motor de fórmulas, capturables como incidencia (prima dominical en días, horas extra en horas).
UPDATE dbo.CatConcepto SET EsIncidencia = 1, FormaCalculo = 'DIAS'  WHERE ClaveSAT = '020' AND TipoConcepto = 'P';
UPDATE dbo.CatConcepto SET EsIncidencia = 1, FormaCalculo = 'HORAS' WHERE ClaveSAT = '019' AND TipoConcepto = 'P';
UPDATE dbo.CatConcepto SET EsIncidencia = 1, FormaCalculo = 'DIAS'  WHERE TipoConcepto = 'P' AND ClaveSAT IN ('002','021'); -- aguinaldo, prima vacacional
UPDATE dbo.CatConcepto SET EsIncidencia = 1, FormaCalculo = 'DIAS'  WHERE TipoConcepto = 'P' AND ClaveSAT IN ('022','025'); -- prima de antigüedad, indemnizaciones
GO

-- Catálogo SAT completo capturable: todas las percepciones (ingresos) y deducciones (egresos) no-base pueden capturarse
-- como incidencia (por monto por defecto); los conceptos base (sueldo, IMSS, ISR, subsidio) los calcula el motor y no se capturan.
UPDATE dbo.CatConcepto SET EsIncidencia = 1, FormaCalculo = ISNULL(FormaCalculo, 'MONTO_FIJO')
WHERE TipoConcepto IN ('P','D') AND EsBase = 0;
GO

-- 25b. Motor de fórmulas: tipo, fórmulas por concepto y parámetros por defecto (alcance empresa/periodo/empleado).
IF NOT EXISTS (SELECT 1 FROM dbo.CatTipoFormula WHERE Codigo = 'ARIT')
    INSERT INTO dbo.CatTipoFormula (Codigo, Descripcion, EjemploExpresion, Activo)
    VALUES ('ARIT', N'Expresión aritmética sobre variables de nómina', N'@DIAS * @SD * @PORCENTAJE / 100', 1);
GO

DECLARE @TipoArit INT = (SELECT TipoFormulaId FROM dbo.CatTipoFormula WHERE Codigo = 'ARIT');
IF NOT EXISTS (SELECT 1 FROM dbo.CatFormulaCalculo WHERE Nombre = N'Prima dominical')
    INSERT INTO dbo.CatFormulaCalculo (TipoFormulaId, Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles)
    VALUES (@TipoArit, N'Prima dominical', N'Prima dominical (LFT Art. 71): % del salario diario por domingo laborado',
            N'@DIAS * @SD * @PORCENTAJE / 100', N'DIAS,SD,PORCENTAJE');
IF NOT EXISTS (SELECT 1 FROM dbo.CatFormulaCalculo WHERE Nombre = N'Prima dominical exenta')
    INSERT INTO dbo.CatFormulaCalculo (TipoFormulaId, Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles)
    VALUES (@TipoArit, N'Prima dominical exenta', N'Exención de prima dominical: hasta TOPE_UMA UMA por domingo',
            N'@DIAS * @UMA * @TOPE_UMA', N'DIAS,UMA,TOPE_UMA');
IF NOT EXISTS (SELECT 1 FROM dbo.CatFormulaCalculo WHERE Nombre = N'Horas extra dobles')
    INSERT INTO dbo.CatFormulaCalculo (TipoFormulaId, Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles)
    VALUES (@TipoArit, N'Horas extra dobles', N'Horas extra dobles (LFT Art. 67): horas x salario por hora x factor',
            N'@HORAS * @SALARIO_HORA * @FACTOR', N'HORAS,SALARIO_HORA,FACTOR');
IF NOT EXISTS (SELECT 1 FROM dbo.CatFormulaCalculo WHERE Nombre = N'Aguinaldo')
    INSERT INTO dbo.CatFormulaCalculo (TipoFormulaId, Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles)
    VALUES (@TipoArit, N'Aguinaldo', N'Aguinaldo (LFT Art. 87): días de aguinaldo x salario diario',
            N'@DIAS * @SD', N'DIAS,SD');
IF NOT EXISTS (SELECT 1 FROM dbo.CatFormulaCalculo WHERE Nombre = N'Prima vacacional')
    INSERT INTO dbo.CatFormulaCalculo (TipoFormulaId, Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles)
    VALUES (@TipoArit, N'Prima vacacional', N'Prima vacacional (LFT Art. 80): días de vacaciones x salario diario x %',
            N'@DIAS * @SD * @PORCENTAJE / 100', N'DIAS,SD,PORCENTAJE');
IF NOT EXISTS (SELECT 1 FROM dbo.CatFormulaCalculo WHERE Nombre = N'Prima de antigüedad')
    INSERT INTO dbo.CatFormulaCalculo (TipoFormulaId, Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles)
    VALUES (@TipoArit, N'Prima de antigüedad', N'Prima de antigüedad (LFT Art. 162): días (12 por año) x salario diario',
            N'@DIAS * @SD', N'DIAS,SD');
IF NOT EXISTS (SELECT 1 FROM dbo.CatFormulaCalculo WHERE Nombre = N'Indemnizaciones')
    INSERT INTO dbo.CatFormulaCalculo (TipoFormulaId, Nombre, Descripcion, ExpresionCalculo, VariablesDisponibles)
    VALUES (@TipoArit, N'Indemnizaciones', N'Indemnización por separación (LFT Art. 48/50): días x salario diario',
            N'@DIAS * @SD', N'DIAS,SD');
GO

-- Liga fórmula -> concepto SAT 020 (prima dominical) y 019 (horas extra)
INSERT INTO dbo.RelConceptoFormula (ConceptoId, FormulaId, OrdenEjecucion)
SELECT c.ConceptoId, f.FormulaId, 1
FROM dbo.CatConcepto c
JOIN dbo.CatFormulaCalculo f ON f.Nombre = N'Prima dominical'
WHERE c.ClaveSAT = '020' AND c.TipoConcepto = 'P'
  AND NOT EXISTS (SELECT 1 FROM dbo.RelConceptoFormula rf WHERE rf.ConceptoId = c.ConceptoId AND rf.FormulaId = f.FormulaId);
INSERT INTO dbo.RelConceptoFormula (ConceptoId, FormulaId, OrdenEjecucion)
SELECT c.ConceptoId, f.FormulaId, 1
FROM dbo.CatConcepto c
JOIN dbo.CatFormulaCalculo f ON f.Nombre = N'Horas extra dobles'
WHERE c.ClaveSAT = '019' AND c.TipoConcepto = 'P'
  AND NOT EXISTS (SELECT 1 FROM dbo.RelConceptoFormula rf WHERE rf.ConceptoId = c.ConceptoId AND rf.FormulaId = f.FormulaId);
INSERT INTO dbo.RelConceptoFormula (ConceptoId, FormulaId, OrdenEjecucion)
SELECT c.ConceptoId, f.FormulaId, 1
FROM dbo.CatConcepto c
JOIN dbo.CatFormulaCalculo f ON f.Nombre = N'Aguinaldo'
WHERE c.ClaveSAT = '002' AND c.TipoConcepto = 'P'
  AND NOT EXISTS (SELECT 1 FROM dbo.RelConceptoFormula rf WHERE rf.ConceptoId = c.ConceptoId AND rf.FormulaId = f.FormulaId);
INSERT INTO dbo.RelConceptoFormula (ConceptoId, FormulaId, OrdenEjecucion)
SELECT c.ConceptoId, f.FormulaId, 1
FROM dbo.CatConcepto c
JOIN dbo.CatFormulaCalculo f ON f.Nombre = N'Prima vacacional'
WHERE c.ClaveSAT = '021' AND c.TipoConcepto = 'P'
  AND NOT EXISTS (SELECT 1 FROM dbo.RelConceptoFormula rf WHERE rf.ConceptoId = c.ConceptoId AND rf.FormulaId = f.FormulaId);
INSERT INTO dbo.RelConceptoFormula (ConceptoId, FormulaId, OrdenEjecucion)
SELECT c.ConceptoId, f.FormulaId, 1
FROM dbo.CatConcepto c
JOIN dbo.CatFormulaCalculo f ON f.Nombre = N'Prima de antigüedad'
WHERE c.ClaveSAT = '022' AND c.TipoConcepto = 'P'
  AND NOT EXISTS (SELECT 1 FROM dbo.RelConceptoFormula rf WHERE rf.ConceptoId = c.ConceptoId AND rf.FormulaId = f.FormulaId);
INSERT INTO dbo.RelConceptoFormula (ConceptoId, FormulaId, OrdenEjecucion)
SELECT c.ConceptoId, f.FormulaId, 1
FROM dbo.CatConcepto c
JOIN dbo.CatFormulaCalculo f ON f.Nombre = N'Indemnizaciones'
WHERE c.ClaveSAT = '025' AND c.TipoConcepto = 'P'
  AND NOT EXISTS (SELECT 1 FROM dbo.RelConceptoFormula rf WHERE rf.ConceptoId = c.ConceptoId AND rf.FormulaId = f.FormulaId);
GO

-- Parámetros por defecto (alcance global); pueden sobreescribirse por empresa, periodo o empleado.
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

-- 26. CatPaqueteIncidencia y RelPaqueteConcepto (Paquetes base por periodicidad)
INSERT dbo.CatPaqueteIncidencia(Nombre, PeriodicidadPagoId, EmpresaId, EsDefault, Activo)
SELECT N'Paquete base ' + LOWER(pp.Descripcion), pp.PeriodicidadPagoId, NULL, 1, 1
FROM dbo.CatPeriodicidadPago pp
WHERE pp.DiasBase IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.CatPaqueteIncidencia p WHERE p.PeriodicidadPagoId = pp.PeriodicidadPagoId AND p.EmpresaId IS NULL AND p.EsDefault = 1);

INSERT dbo.RelPaqueteConcepto(IdPaquete, IdConceptoSAT, CantidadOValor, EsPermanente, Activo)
SELECT p.IdPaquete, c.ConceptoId, 0, 1, 1
FROM dbo.CatPaqueteIncidencia p
CROSS JOIN dbo.CatConcepto c
WHERE p.EsDefault = 1 AND c.EsBase = 1
  AND NOT EXISTS (SELECT 1 FROM dbo.RelPaqueteConcepto rp WHERE rp.IdPaquete = p.IdPaquete AND rp.IdConceptoSAT = c.ConceptoId);
GO

-- ==============================================================================
-- [BLOQUE-14B] MÓDULO DE CONFIGURACIÓN, BITÁCORA Y RECUPERACIÓN DE ACCESO
-- (integra las antiguas migraciones de configuración del sistema y de recuperación de contraseña)
-- ==============================================================================
PRINT '[14b/21] Instalando módulo de configuración, bitácora y recuperación de acceso...';
GO

/* ============================================================================
   NomiMex - Migración: Módulo integral de configuración del sistema
   ----------------------------------------------------------------------------
   Taxonomía oficial respetada:
     - Cat*  : catálogos / configuraciones -> dbo.CatConfiguracionCorreo
                                               dbo.CatParametrosSistema
     - Log*  : bitácoras / auditoría        -> dbo.LogSistema
   Objetos:
     Tablas    : CatConfiguracionCorreo, CatParametrosSistema, LogSistema
     Procs      : sp_Config_ObtenerCorreo, sp_Config_GuardarCorreo
                  sp_Config_ObtenerParametros, sp_Config_GuardarParametros
                  sp_Log_Insertar, sp_Log_Consultar, sp_Log_Depurar
                  sp_Respaldo_EjecutarCompleto
   Idempotente: puede ejecutarse varias veces sin error.
   La contraseña SMTP se guarda SIEMPRE cifrada (AES-256) desde la capa de
   negocio; aquí nunca se persiste ni se procesa en texto plano.
============================================================================ */
SET NOCOUNT ON;
GO

PRINT 'Creando tabla dbo.CatConfiguracionCorreo...';
GO

-- Parámetros SMTP para el envío de recibos (DocRecibos) y recuperación de contraseñas.
IF OBJECT_ID('dbo.CatConfiguracionCorreo', 'U') IS NULL
CREATE TABLE dbo.CatConfiguracionCorreo (
    IdConfigCorreo          INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_CatConfiguracionCorreo PRIMARY KEY,
    ServidorSmtp            NVARCHAR(150)  NOT NULL,
    Puerto                  INT            NOT NULL
        CONSTRAINT DF_CatConfigCorreo_Puerto DEFAULT 587,
    UsaSsl                  BIT            NOT NULL
        CONSTRAINT DF_CatConfigCorreo_Ssl DEFAULT 1,
    CorreoRemitente         NVARCHAR(150)  NOT NULL,
    NombreMostrar           NVARCHAR(150)  NULL,
    UsuarioSmtp             NVARCHAR(150)  NULL,
    PasswordCifrado         NVARCHAR(512)  NULL,
    AsuntoDefaultRecibo     NVARCHAR(200)  NULL,
    CuerpoDefaultRecibo     NVARCHAR(MAX)  NULL,
    Activo                  BIT            NOT NULL
        CONSTRAINT DF_CatConfigCorreo_Activo DEFAULT 1,
    FechaUltimaModificacion DATETIME2(0)   NOT NULL
        CONSTRAINT DF_CatConfigCorreo_Fecha DEFAULT SYSUTCDATETIME(),
    CONSTRAINT CK_CatConfigCorreo_Puerto CHECK (Puerto BETWEEN 1 AND 65535)
);
GO

PRINT 'Creando tabla dbo.CatParametrosSistema...';
GO

-- Rutas y preferencias generales de operación.
IF OBJECT_ID('dbo.CatParametrosSistema', 'U') IS NULL
CREATE TABLE dbo.CatParametrosSistema (
    IdParametro             INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_CatParametrosSistema PRIMARY KEY,
    RutaRespaldosDefault    NVARCHAR(400)  NULL,
    RutaExportacionRecibos  NVARCHAR(400)  NULL,
    RutaArchivosSat         NVARCHAR(400)  NULL,
    DiasRetencionLogs       INT            NOT NULL
        CONSTRAINT DF_CatParametros_Retencion DEFAULT 90,
    ModoTimbradoPruebas     BIT            NOT NULL
        CONSTRAINT DF_CatParametros_ModoPruebas DEFAULT 1,
    FechaUltimaModificacion DATETIME2(0)   NOT NULL
        CONSTRAINT DF_CatParametros_Fecha DEFAULT SYSUTCDATETIME(),
    CONSTRAINT CK_CatParametros_Retencion CHECK (DiasRetencionLogs BETWEEN 1 AND 3650)
);
GO

PRINT 'Creando tabla dbo.LogSistema...';
GO

-- Bitácora de auditoría y errores del sistema.
IF OBJECT_ID('dbo.LogSistema', 'U') IS NULL
CREATE TABLE dbo.LogSistema (
    IdLog          BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_LogSistema PRIMARY KEY,
    FechaHora      DATETIME       NOT NULL
        CONSTRAINT DF_LogSistema_Fecha DEFAULT GETDATE(),
    Nivel          VARCHAR(12)    NOT NULL,
    Modulo         NVARCHAR(60)   NULL,
    Mensaje        NVARCHAR(1000) NOT NULL,
    DetalleTecnico NVARCHAR(MAX)  NULL,
    IdUsuario      INT            NULL
        CONSTRAINT FK_LogSistema_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    CONSTRAINT CK_LogSistema_Nivel CHECK (Nivel IN ('INFO', 'ADVERTENCIA', 'ERROR', 'SEGURIDAD'))
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LogSistema_FechaNivel')
    CREATE INDEX IX_LogSistema_FechaNivel ON dbo.LogSistema(FechaHora, Nivel);
GO

/* ============================================================================
   CONFIGURACIÓN DE CORREO (SMTP)
============================================================================ */

/* ----------------------------------------------------------------------------
   Devuelve la configuración SMTP activa (registro único de operación).
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Config_ObtenerCorreo
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        IdConfigCorreo, ServidorSmtp, Puerto, UsaSsl, CorreoRemitente, NombreMostrar,
        UsuarioSmtp, PasswordCifrado, AsuntoDefaultRecibo, CuerpoDefaultRecibo,
        Activo, FechaUltimaModificacion
    FROM dbo.CatConfiguracionCorreo
    ORDER BY Activo DESC, IdConfigCorreo DESC;
END
GO

/* ----------------------------------------------------------------------------
   Inserta o actualiza la configuración SMTP (registro único).
   Si @PasswordCifrado llega NULL se conserva la contraseña previamente guardada.
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Config_GuardarCorreo
    @IdConfigCorreo      INT            = NULL,
    @ServidorSmtp        NVARCHAR(150),
    @Puerto              INT,
    @UsaSsl              BIT,
    @CorreoRemitente     NVARCHAR(150),
    @NombreMostrar       NVARCHAR(150)  = NULL,
    @UsuarioSmtp         NVARCHAR(150)  = NULL,
    @PasswordCifrado     NVARCHAR(512)  = NULL,
    @AsuntoDefaultRecibo NVARCHAR(200)  = NULL,
    @CuerpoDefaultRecibo NVARCHAR(MAX)  = NULL,
    @Activo              BIT            = 1,
    @IdResultado         INT            OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @IdConfigCorreo IS NULL
        SELECT @IdConfigCorreo = IdConfigCorreo
        FROM dbo.CatConfiguracionCorreo
        ORDER BY IdConfigCorreo ASC;

    IF @IdConfigCorreo IS NULL
    BEGIN
        INSERT INTO dbo.CatConfiguracionCorreo
            (ServidorSmtp, Puerto, UsaSsl, CorreoRemitente, NombreMostrar, UsuarioSmtp,
             PasswordCifrado, AsuntoDefaultRecibo, CuerpoDefaultRecibo, Activo, FechaUltimaModificacion)
        VALUES
            (@ServidorSmtp, @Puerto, @UsaSsl, @CorreoRemitente, @NombreMostrar, @UsuarioSmtp,
             @PasswordCifrado, @AsuntoDefaultRecibo, @CuerpoDefaultRecibo, @Activo, SYSUTCDATETIME());

        SET @IdResultado = CAST(SCOPE_IDENTITY() AS INT);
    END
    ELSE
    BEGIN
        UPDATE dbo.CatConfiguracionCorreo
        SET ServidorSmtp            = @ServidorSmtp,
            Puerto                  = @Puerto,
            UsaSsl                  = @UsaSsl,
            CorreoRemitente         = @CorreoRemitente,
            NombreMostrar           = @NombreMostrar,
            UsuarioSmtp             = @UsuarioSmtp,
            PasswordCifrado         = COALESCE(@PasswordCifrado, PasswordCifrado),
            AsuntoDefaultRecibo     = @AsuntoDefaultRecibo,
            CuerpoDefaultRecibo     = @CuerpoDefaultRecibo,
            Activo                  = @Activo,
            FechaUltimaModificacion = SYSUTCDATETIME()
        WHERE IdConfigCorreo = @IdConfigCorreo;

        SET @IdResultado = @IdConfigCorreo;
    END
END
GO

/* ============================================================================
   PARÁMETROS GENERALES DEL SISTEMA
============================================================================ */

/* ----------------------------------------------------------------------------
   Devuelve los parámetros generales (registro único de operación).
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Config_ObtenerParametros
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        IdParametro, RutaRespaldosDefault, RutaExportacionRecibos, RutaArchivosSat,
        DiasRetencionLogs, ModoTimbradoPruebas, FechaUltimaModificacion
    FROM dbo.CatParametrosSistema
    ORDER BY IdParametro ASC;
END
GO

/* ----------------------------------------------------------------------------
   Inserta o actualiza los parámetros generales (registro único).
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Config_GuardarParametros
    @IdParametro            INT           = NULL,
    @RutaRespaldosDefault   NVARCHAR(400) = NULL,
    @RutaExportacionRecibos NVARCHAR(400) = NULL,
    @RutaArchivosSat        NVARCHAR(400) = NULL,
    @DiasRetencionLogs      INT,
    @ModoTimbradoPruebas    BIT,
    @IdResultado            INT           OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @DiasRetencionLogs IS NULL OR @DiasRetencionLogs < 1
        SET @DiasRetencionLogs = 90;

    IF @IdParametro IS NULL
        SELECT @IdParametro = IdParametro
        FROM dbo.CatParametrosSistema
        ORDER BY IdParametro ASC;

    IF @IdParametro IS NULL
    BEGIN
        INSERT INTO dbo.CatParametrosSistema
            (RutaRespaldosDefault, RutaExportacionRecibos, RutaArchivosSat,
             DiasRetencionLogs, ModoTimbradoPruebas, FechaUltimaModificacion)
        VALUES
            (@RutaRespaldosDefault, @RutaExportacionRecibos, @RutaArchivosSat,
             @DiasRetencionLogs, @ModoTimbradoPruebas, SYSUTCDATETIME());

        SET @IdResultado = CAST(SCOPE_IDENTITY() AS INT);
    END
    ELSE
    BEGIN
        UPDATE dbo.CatParametrosSistema
        SET RutaRespaldosDefault    = @RutaRespaldosDefault,
            RutaExportacionRecibos  = @RutaExportacionRecibos,
            RutaArchivosSat         = @RutaArchivosSat,
            DiasRetencionLogs       = @DiasRetencionLogs,
            ModoTimbradoPruebas     = @ModoTimbradoPruebas,
            FechaUltimaModificacion = SYSUTCDATETIME()
        WHERE IdParametro = @IdParametro;

        SET @IdResultado = @IdParametro;
    END
END
GO

/* ============================================================================
   BITÁCORA DEL SISTEMA (LogSistema)
============================================================================ */

/* ----------------------------------------------------------------------------
   Inserta un evento en la bitácora. Valida el nivel permitido.
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Log_Insertar
    @Nivel          VARCHAR(12),
    @Modulo         NVARCHAR(60)   = NULL,
    @Mensaje        NVARCHAR(1000),
    @DetalleTecnico NVARCHAR(MAX)  = NULL,
    @IdUsuario      INT            = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Nivel NOT IN ('INFO', 'ADVERTENCIA', 'ERROR', 'SEGURIDAD')
        SET @Nivel = 'INFO';

    IF @IdUsuario IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.CatUsuario WHERE UsuarioId = @IdUsuario)
        SET @IdUsuario = NULL;

    INSERT INTO dbo.LogSistema (Nivel, Modulo, Mensaje, DetalleTecnico, IdUsuario)
    VALUES (@Nivel, @Modulo, LEFT(ISNULL(@Mensaje, N''), 1000), @DetalleTecnico, @IdUsuario);
END
GO

/* ----------------------------------------------------------------------------
   Consulta la bitácora con filtros opcionales por rango de fechas, nivel y módulo.
   @Nivel = NULL o 'TODOS' devuelve todos los niveles.
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Log_Consultar
    @FechaInicio DATETIME     = NULL,
    @FechaFin    DATETIME     = NULL,
    @Nivel       VARCHAR(12)  = NULL,
    @Modulo      NVARCHAR(60) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Nivel = 'TODOS' SET @Nivel = NULL;

    SELECT
        l.IdLog, l.FechaHora, l.Nivel, l.Modulo, l.Mensaje, l.DetalleTecnico,
        l.IdUsuario, u.NombreUsuario
    FROM dbo.LogSistema l
    LEFT JOIN dbo.CatUsuario u ON u.UsuarioId = l.IdUsuario
    WHERE (@FechaInicio IS NULL OR l.FechaHora >= @FechaInicio)
      AND (@FechaFin    IS NULL OR l.FechaHora <  DATEADD(DAY, 1, CAST(@FechaFin AS DATE)))
      AND (@Nivel       IS NULL OR l.Nivel  = @Nivel)
      AND (@Modulo      IS NULL OR l.Modulo = @Modulo)
    ORDER BY l.FechaHora DESC, l.IdLog DESC;
END
GO

/* ----------------------------------------------------------------------------
   Depura los registros de bitácora anteriores a @DiasRetencion días.
   Devuelve el número de filas eliminadas.
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Log_Depurar
    @DiasRetencion INT,
    @Eliminados    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @DiasRetencion IS NULL OR @DiasRetencion < 1
        SET @DiasRetencion = 90;

    DECLARE @Limite DATETIME = DATEADD(DAY, -@DiasRetencion, GETDATE());

    DELETE FROM dbo.LogSistema
    WHERE FechaHora < @Limite;

    SET @Eliminados = @@ROWCOUNT;
END
GO

/* ============================================================================
   RESPALDO COMPLETO DE LA BASE DE DATOS
============================================================================ */

/* ----------------------------------------------------------------------------
   Ejecuta un BACKUP DATABASE completo hacia la ruta indicada. El nombre del
   archivo lo arma la capa de negocio con sello de fecha/hora (.bak). El nombre
   de la base de datos no se concatena: se obtiene con DB_NAME() y se delimita
   con QUOTENAME para evitar inyección en el SQL dinámico.
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Respaldo_EjecutarCompleto
    @RutaCompleta NVARCHAR(400)
AS
BEGIN
    SET NOCOUNT ON;

    IF @RutaCompleta IS NULL OR LEN(LTRIM(RTRIM(@RutaCompleta))) = 0
    BEGIN
        RAISERROR('Debe indicar la ruta completa del archivo de respaldo (.bak).', 16, 1);
        RETURN;
    END

    DECLARE @BaseDatos SYSNAME = DB_NAME();
    DECLARE @Descripcion NVARCHAR(255) =
        N'Respaldo completo de ' + @BaseDatos + N' - ' +
        CONVERT(NVARCHAR(19), GETDATE(), 120);

    DECLARE @Sql NVARCHAR(MAX) =
        N'BACKUP DATABASE ' + QUOTENAME(@BaseDatos) +
        N' TO DISK = @Destino WITH INIT, FORMAT, SKIP, STATS = 10, NAME = @Nombre;';

    EXEC sp_executesql
        @Sql,
        N'@Destino NVARCHAR(400), @Nombre NVARCHAR(255)',
        @Destino = @RutaCompleta,
        @Nombre  = @Descripcion;

    SELECT @RutaCompleta AS RutaGenerada;
END
GO

PRINT 'Migración del módulo de configuración aplicada correctamente.';
GO

/* ============================================================================
   NomiMex - Migración: Recuperación de acceso por correo y cambio de contraseña
   ----------------------------------------------------------------------------
   Taxonomía oficial respetada:
     - Cat*  : catálogos           -> dbo.CatUsuario (seguridad)
     - Proc* : procesos            -> dbo.ProcRecuperacionContrasena (tokens)
     - Log*  : bitácoras/auditoría -> dbo.LogAuth
   Idempotente: puede ejecutarse varias veces sin error.
   El hash de contraseña y de token se calcula SIEMPRE en el servidor
   (SHA2_512 con sal) reutilizando dbo.fn_Auth_HashContrasena y
   dbo.fn_Auth_HashToken ya existentes en la instalación base.
============================================================================ */
SET NOCOUNT ON;
GO

PRINT 'Verificando columnas de dbo.CatUsuario (Email, RequiereCambioPwd)...';
GO

-- Correo electrónico del usuario (en NomiMex la columna oficial se llama Email).
IF COL_LENGTH('dbo.CatUsuario', 'Email') IS NULL
BEGIN
    ALTER TABLE dbo.CatUsuario ADD Email NVARCHAR(150) NOT NULL
        CONSTRAINT DF_CatUsuario_Email DEFAULT N'';
END
GO

-- Restricción de unicidad del correo.
IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_CatUsuario_Email')
    ALTER TABLE dbo.CatUsuario ADD CONSTRAINT UQ_CatUsuario_Email UNIQUE (Email);
GO

-- Bandera que obliga al usuario a cambiar su contraseña en el próximo acceso.
IF COL_LENGTH('dbo.CatUsuario', 'RequiereCambioPwd') IS NULL
BEGIN
    ALTER TABLE dbo.CatUsuario ADD RequiereCambioPwd BIT NOT NULL
        CONSTRAINT DF_CatUsuario_CambioPwd DEFAULT 0;
END
GO

PRINT 'Verificando tabla transaccional dbo.ProcRecuperacionContrasena...';
GO

-- Tabla de tokens de recuperación (código de verificación hasheado + vigencia).
IF OBJECT_ID('dbo.ProcRecuperacionContrasena', 'U') IS NULL
CREATE TABLE dbo.ProcRecuperacionContrasena (
    RecuperacionId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_LogRecuperacion_Id DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    UsuarioId      INT NOT NULL CONSTRAINT FK_LogRecup_Usuario REFERENCES dbo.CatUsuario(UsuarioId),
    TokenHash      VARBINARY(64) NOT NULL,
    CreadoUtc      DATETIME2(0) NOT NULL CONSTRAINT DF_LogRecup_Creado DEFAULT SYSUTCDATETIME(),
    ExpiraUtc      DATETIME2(0) NOT NULL,
    Usado          BIT NOT NULL CONSTRAINT DF_LogRecup_Usado DEFAULT 0,
    UsadoUtc       DATETIME2(0) NULL,
    IpSolicitud    NVARCHAR(45) NULL,
    CONSTRAINT UQ_LogRecuperacion_TokenHash UNIQUE (TokenHash)
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LogRecuperacion_UsuarioExpira')
    CREATE INDEX IX_LogRecuperacion_UsuarioExpira
        ON dbo.ProcRecuperacionContrasena(UsuarioId, ExpiraUtc);
GO

/* ----------------------------------------------------------------------------
   SP 1: Validar que el correo exista y pertenezca a un usuario activo.
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Auth_ValidarCorreoRecuperacion
    @Email         NVARCHAR(150),
    @UsuarioId     INT           OUTPUT,
    @NombreMostrar NVARCHAR(150) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @UsuarioId = NULL, @NombreMostrar = NULL;

    SELECT @UsuarioId = UsuarioId, @NombreMostrar = NombreMostrar
    FROM dbo.CatUsuario
    WHERE Email = @Email AND Activo = 1 AND Bloqueado = 0;

    IF @UsuarioId IS NULL
    BEGIN
        RAISERROR('No existe una cuenta activa asociada a ese correo electrónico.', 16, 1);
        RETURN;
    END
END
GO

/* ----------------------------------------------------------------------------
   SP 2: Registrar un nuevo código temporal (vigencia 15 min por defecto) e
   invalidar los códigos previos no utilizados del mismo usuario.
   El código viaja en claro desde la capa de negocio; aquí sólo se guarda su
   hash (combinado con el UsuarioId para evitar colisiones entre usuarios).
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Auth_GenerarCodigoRecuperacion
    @Email           NVARCHAR(150),
    @Codigo          NVARCHAR(128),
    @MinutosVigencia INT           = 15,
    @IpSolicitud     NVARCHAR(45)  = NULL,
    @UsuarioId       INT           OUTPUT,
    @NombreMostrar   NVARCHAR(150) OUTPUT,
    @ExpiraUtc       DATETIME2(0)  OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @UsuarioId = UsuarioId, @NombreMostrar = NombreMostrar
    FROM dbo.CatUsuario
    WHERE Email = @Email AND Activo = 1 AND Bloqueado = 0;

    IF @UsuarioId IS NULL
    BEGIN
        RAISERROR('No existe una cuenta activa asociada a ese correo electrónico.', 16, 1);
        RETURN;
    END

    IF @MinutosVigencia IS NULL OR @MinutosVigencia <= 0
        SET @MinutosVigencia = 15;

    DECLARE @TokenHash VARBINARY(64) = dbo.fn_Auth_HashToken(CONCAT(@Codigo, N'|', @UsuarioId));
    SET @ExpiraUtc = DATEADD(MINUTE, @MinutosVigencia, SYSUTCDATETIME());

    DECLARE @OuterTran INT = @@TRANCOUNT;
    IF @OuterTran = 0 BEGIN TRANSACTION;

    BEGIN TRY
        -- Invalida cualquier código previo vigente/no utilizado del usuario.
        UPDATE dbo.ProcRecuperacionContrasena
        SET Usado = 1, UsadoUtc = SYSUTCDATETIME()
        WHERE UsuarioId = @UsuarioId AND Usado = 0;

        INSERT INTO dbo.ProcRecuperacionContrasena (UsuarioId, TokenHash, ExpiraUtc, IpSolicitud)
        VALUES (@UsuarioId, @TokenHash, @ExpiraUtc, @IpSolicitud);

        INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle, IpOrigen)
        VALUES (@UsuarioId, 'RECUP_SOLICITUD', 1, 'Código de recuperación generado.', @IpSolicitud);

        IF @OuterTran = 0 COMMIT;
    END TRY
    BEGIN CATCH
        IF @OuterTran = 0 AND XACT_STATE() <> 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

/* ----------------------------------------------------------------------------
   SP 3: Validar el código (vigente y Usado = 0), actualizar la contraseña
   cifrada, reiniciar RequiereCambioPwd = 0 y marcar el código como utilizado,
   todo dentro de una transacción. Revoca las sesiones activas por seguridad.
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Auth_RestablecerContrasena
    @Email           NVARCHAR(150),
    @Codigo          NVARCHAR(128),
    @NuevaContrasena NVARCHAR(256),
    @IpSolicitud     NVARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UsuarioId INT;
    SELECT @UsuarioId = UsuarioId
    FROM dbo.CatUsuario
    WHERE Email = @Email AND Activo = 1 AND Bloqueado = 0;

    IF @UsuarioId IS NULL
    BEGIN
        RAISERROR('No existe una cuenta activa asociada a ese correo electrónico.', 16, 1);
        RETURN;
    END

    IF LEN(ISNULL(@NuevaContrasena, N'')) < 10
    BEGIN
        RAISERROR('La nueva contraseña debe tener al menos 10 caracteres.', 16, 1);
        RETURN;
    END

    DECLARE @TokenHash VARBINARY(64) = dbo.fn_Auth_HashToken(CONCAT(@Codigo, N'|', @UsuarioId));
    DECLARE @RecuperacionId UNIQUEIDENTIFIER;

    SELECT @RecuperacionId = RecuperacionId
    FROM dbo.ProcRecuperacionContrasena
    WHERE UsuarioId = @UsuarioId AND TokenHash = @TokenHash
      AND Usado = 0 AND ExpiraUtc > SYSUTCDATETIME();

    IF @RecuperacionId IS NULL
    BEGIN
        INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle, IpOrigen)
        VALUES (@UsuarioId, 'RECUP_FALLIDO', 0, 'Código inválido, expirado o ya utilizado.', @IpSolicitud);
        RAISERROR('El código es inválido, expiró o ya fue utilizado. Solicite uno nuevo.', 16, 1);
        RETURN;
    END

    DECLARE @OuterTran INT = @@TRANCOUNT;
    IF @OuterTran = 0 BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @Salt VARBINARY(32) = CRYPT_GEN_RANDOM(32);

        UPDATE dbo.CatUsuario
        SET ContrasenaSalt    = @Salt,
            ContrasenaHash    = dbo.fn_Auth_HashContrasena(@NuevaContrasena, @Salt),
            RequiereCambioPwd = 0,
            IntentosFallidos  = 0,
            Bloqueado         = 0,
            FechaUpdateUtc    = SYSUTCDATETIME()
        WHERE UsuarioId = @UsuarioId;

        -- Quema el token utilizado.
        UPDATE dbo.ProcRecuperacionContrasena
        SET Usado = 1, UsadoUtc = SYSUTCDATETIME()
        WHERE RecuperacionId = @RecuperacionId;

        -- Revoca sesiones abiertas: tras un reseteo deben volver a autenticarse.
        UPDATE dbo.ProcSesionUsuario
        SET RevocadaUtc = SYSUTCDATETIME()
        WHERE UsuarioId = @UsuarioId AND RevocadaUtc IS NULL;

        INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle, IpOrigen)
        VALUES (@UsuarioId, 'RECUP_EXITO', 1, 'Contraseña restablecida con código de recuperación.', @IpSolicitud);

        IF @OuterTran = 0 COMMIT;
    END TRY
    BEGIN CATCH
        IF @OuterTran = 0 AND XACT_STATE() <> 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

/* ----------------------------------------------------------------------------
   SP 4: Cambiar la contraseña desde una sesión iniciada, validando primero la
   contraseña actual. Reinicia RequiereCambioPwd = 0.
---------------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_Auth_CambiarContrasena
    @UsuarioId        INT,
    @ContrasenaActual NVARCHAR(256),
    @NuevaContrasena  NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Salt VARBINARY(32), @Hash VARBINARY(64), @Activo BIT, @Bloqueado BIT;
    SELECT @Salt = ContrasenaSalt, @Hash = ContrasenaHash, @Activo = Activo, @Bloqueado = Bloqueado
    FROM dbo.CatUsuario
    WHERE UsuarioId = @UsuarioId;

    IF @Salt IS NULL
    BEGIN RAISERROR('El usuario no existe.', 16, 1); RETURN; END

    IF @Activo = 0 OR @Bloqueado = 1
    BEGIN RAISERROR('El usuario está inactivo o bloqueado.', 16, 1); RETURN; END

    IF dbo.fn_Auth_HashContrasena(@ContrasenaActual, @Salt) <> @Hash
    BEGIN
        INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle)
        VALUES (@UsuarioId, 'PWD_CAMBIO', 0, 'Contraseña actual incorrecta.');
        RAISERROR('La contraseña actual es incorrecta.', 16, 1);
        RETURN;
    END

    IF LEN(ISNULL(@NuevaContrasena, N'')) < 10
    BEGIN RAISERROR('La nueva contraseña debe tener al menos 10 caracteres.', 16, 1); RETURN; END

    IF dbo.fn_Auth_HashContrasena(@NuevaContrasena, @Salt) = @Hash
    BEGIN RAISERROR('La nueva contraseña debe ser diferente de la actual.', 16, 1); RETURN; END

    DECLARE @OuterTran INT = @@TRANCOUNT;
    IF @OuterTran = 0 BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @NuevaSalt VARBINARY(32) = CRYPT_GEN_RANDOM(32);

        UPDATE dbo.CatUsuario
        SET ContrasenaSalt    = @NuevaSalt,
            ContrasenaHash    = dbo.fn_Auth_HashContrasena(@NuevaContrasena, @NuevaSalt),
            RequiereCambioPwd = 0,
            FechaUpdateUtc    = SYSUTCDATETIME()
        WHERE UsuarioId = @UsuarioId;

        INSERT INTO dbo.LogAuth (UsuarioId, Evento, Exitoso, Detalle)
        VALUES (@UsuarioId, 'PWD_CAMBIO', 1, 'Contraseña actualizada desde sesión iniciada.');

        IF @OuterTran = 0 COMMIT;
    END TRY
    BEGIN CATCH
        IF @OuterTran = 0 AND XACT_STATE() <> 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

PRINT 'Migración de recuperación de contraseña aplicada correctamente.';
GO

-- ==============================================================================
-- [BLOQUE-15] DATOS INICIALES DE OPERACIÓN (NomiMex_DatosIniciales.sql)
-- ==============================================================================
PRINT '[15/21] Configurando datos iniciales (ejercicio fiscal, festivos, tarifas y superadmin)...';
GO

DECLARE @Anio INT = YEAR(GETDATE());

EXEC dbo.sp_GenerarDiasFestivosLey @Anio = 2025;
EXEC dbo.sp_GenerarDiasFestivosLey @Anio = 2026;
EXEC dbo.sp_GenerarDiasFestivosLey @Anio = 2027;

IF NOT EXISTS (SELECT 1 FROM dbo.CatEjercicioFiscal WHERE Anio = @Anio)
    INSERT INTO dbo.CatEjercicioFiscal (Anio, Estatus, JornadaSemanalHoras) VALUES (@Anio, 'ABIERTO', CASE WHEN @Anio >= 2027 THEN 46 ELSE 48 END);

IF NOT EXISTS (SELECT 1 FROM dbo.CatTablaISR t
               INNER JOIN dbo.CatEjercicioFiscal e ON e.EjercicioId = t.EjercicioId
               WHERE e.Anio = @Anio)
    EXEC dbo.sp_CargarDatosFiscales @Anio = @Anio;

IF NOT EXISTS (SELECT 1 FROM dbo.CatUsuario WHERE NombreUsuario = N'admin')
BEGIN
    DECLARE @UsuarioId INT;
    EXEC dbo.sp_Auth_CrearUsuario
        @NombreUsuario     = N'admin',
        @Email             = N'admin@nomimex.local',
        @NombreMostrar     = N'Administrador del Sistema',
        @Contrasena        = N'Admin#2026Nomi',
        @RequiereCambioPwd = 1,
        @UsuarioId         = @UsuarioId OUTPUT;

    EXEC dbo.sp_AsignarRolUsuario @UsuarioId = @UsuarioId, @CodigoRol = N'SUPERADMIN';
END
GO

IF N'$(CargarDatosDemostracion)' = N'1'
BEGIN
    PRINT '[16/21] Cargando datos de demostración de la empresa...';

-- ==============================================================================
-- NomiMex - Datos REALES de la empresa (constancia de situación fiscal y recibos de nómina)
-- Es idempotente. No ejecutar en bases con información de negocio.
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
    VALUES (N'JOSE ANTONIO SAUTTO MARTINEZ', @RFC, N'SAMA510517HGTTRN06', N'PENDIENTE', 612, N'79560', N'79560', 'SLP', 'IP', 1);
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
END
GO

PRINT '';
PRINT '══════════════════════════════════════════════════════════════════════════';
PRINT '  ✅ NomiMex - Base de Datos Instalada y Configurada Exitosamente';
PRINT '══════════════════════════════════════════════════════════════════════════';
PRINT '';
