-- ==============================================================================
-- NomiMex - Datos iniciales para operar la aplicación de escritorio
-- Ejecutar con sqlcmd -d <base> en una base creada por NomiMex_Instalacion.sql.
-- Es idempotente.
--   1. Crea el ejercicio fiscal del año en curso.
--   2. Carga tarifas ISR, subsidio e IMSS del año (dbo.sp_CargarDatosFiscales).
--   3. Crea el usuario administrador inicial (rol SUPERADMIN).
--   4. Genera los días festivos de ley (Art. 74 LFT) del año en curso para cruzarlos con los periodos.
-- Credenciales iniciales:  usuario = admin   contraseña = Admin#2026Nomi
-- (cámbiela desde Catálogos > Usuarios en el primer inicio de sesión)
-- Flujo de la aplicación después de este script: registrar una empresa con sus periodicidades de pago,
-- generar los periodos del ejercicio en "Periodos y festivos", registrar empleados y procesar la nómina.
-- ==============================================================================
SET NOCOUNT ON;

DECLARE @Anio INT = YEAR(GETDATE());

EXEC dbo.sp_GenerarDiasFestivosLey @Anio = @Anio;

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

SELECT u.UsuarioId, u.NombreUsuario, r.Codigo AS Rol
FROM dbo.CatUsuario u
LEFT JOIN dbo.RelUsuarioRol ur ON ur.UsuarioId = u.UsuarioId
LEFT JOIN dbo.CatRol r ON r.RolId = ur.RolId
WHERE u.NombreUsuario = N'admin';
