namespace NomiMex.Datos
{
    /// <summary>
    /// Fragmento SQL común para conocer las horas de la jornada de un trabajador, con la misma regla que sp_CalcularNomina:
    /// primero las horas de su turno (CatTurno) y, si no tiene turno, las horas máximas del tipo de jornada de su contrato.
    /// Devuelve 0 cuando no hay ninguna configurada.
    /// </summary>
    internal static class ConsultasJornada
    {
        public static string HorasJornada(string aliasEmpleado)
        {
            // Solo las horas del turno propio del trabajador. Si no tiene turno, el motor aplica la jornada diaria
            // estándar del ejercicio (jornada semanal de ley / 6), de modo que el salario por hora sigue la reforma 48->46 h.
            return "COALESCE((SELECT t.Horas FROM dbo.CatTurno t WHERE t.TurnoId = " + aliasEmpleado + ".TurnoId), 0)";
        }
    }
}
