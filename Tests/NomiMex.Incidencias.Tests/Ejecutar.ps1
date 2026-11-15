param(
    [string]$Servidor = '.\SQLEXPRESS',
    [switch]$ConservarBase
)
$ErrorActionPreference = 'Stop'
$raiz = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$base = Join-Path $raiz 'BaseDatos'
$basePruebas = 'NomiMex_Audit_' + [Guid]::NewGuid().ToString('N')
$log  = [System.IO.Path]::GetTempFileName()
$baseCreada = $false
$pruebasCorrectas = $false

function Sql-Archivo([string]$Archivo, [string]$Catalogo, [string[]]$Variables = @()) {
    $argumentos = @('-S', $Servidor, '-E', '-b', '-f', '65001')
    if ($Catalogo) { $argumentos += @('-d', $Catalogo) }
    if ($Variables.Count -gt 0) { $argumentos += @('-v') + $Variables }
    $argumentos += @('-i', $Archivo, '-o', $log)
    & sqlcmd @argumentos
    if ($LASTEXITCODE -ne 0) { Get-Content $log -Tail 40; throw "Fallo al ejecutar $Archivo (codigo $LASTEXITCODE)." }
}
function Sql-EliminarBasePruebas {
    $consulta = "ALTER DATABASE [$basePruebas] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$basePruebas];"
    & sqlcmd -S $Servidor -E -b -d master -Q $consulta
    if ($LASTEXITCODE -ne 0) { throw "No se pudo eliminar la base aislada $basePruebas (codigo $LASTEXITCODE)." }
}

try {
    # Cada ejecucion usa una base nueva; nunca reinstala ni modifica NomiMex.
    $comprobarBase = "IF DB_ID(N'$basePruebas') IS NOT NULL THROW 51999, 'La base aislada ya existe; se cancela para protegerla.', 1;"
    & sqlcmd -S $Servidor -E -b -d master -Q $comprobarBase
    if ($LASTEXITCODE -ne 0) { throw "No se pudo verificar que $basePruebas no exista." }

    Sql-Archivo (Join-Path $base 'NomiMex_Instalacion.sql') $null @("BaseDatos=$basePruebas", 'CargarDatosDemostracion=0')
    $baseCreada = $true
    Sql-Archivo (Join-Path $PSScriptRoot 'Preparar.sql') $basePruebas

    & dotnet build (Join-Path $PSScriptRoot 'NomiMex.Incidencias.Tests.csproj') -v:minimal
    if ($LASTEXITCODE -ne 0) { throw "Fallo la compilacion del proyecto de pruebas." }

    $cadena = "Data Source=$Servidor;Initial Catalog=$basePruebas;Integrated Security=True;Encrypt=False;Application Name=NomiMexPruebas"
    & (Join-Path $PSScriptRoot 'bin\Debug\net481\NomiMex.Incidencias.Tests.exe') $cadena
    if ($LASTEXITCODE -ne 0) { throw "Las pruebas de integracion fallaron (codigo $LASTEXITCODE)." }
    Write-Output 'OK: pruebas de integracion del modulo de incidencias.'
    $pruebasCorrectas = $true
}
finally {
    if ($pruebasCorrectas -and -not $ConservarBase -and $baseCreada) {
        Sql-EliminarBasePruebas
        Write-Output "Base aislada eliminada: $basePruebas"
    }
    elseif ($baseCreada) {
        Write-Output "Base aislada conservada para inspeccion: $basePruebas"
    }
    Remove-Item -LiteralPath $log -ErrorAction SilentlyContinue
}
