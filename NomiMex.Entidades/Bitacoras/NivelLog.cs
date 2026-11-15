namespace NomiMex.Entidades.Bitacoras
{
    /// <summary>Niveles válidos de la bitácora dbo.LogSistema (restricción CK_LogSistema_Nivel).</summary>
    public static class NivelLog
    {
        public const string Info = "INFO";
        public const string Advertencia = "ADVERTENCIA";
        public const string Error = "ERROR";
        public const string Seguridad = "SEGURIDAD";

        /// <summary>Valor lógico para el filtro de consulta: no restringe por nivel.</summary>
        public const string Todos = "TODOS";
    }
}
