namespace NomiMex.Entidades.Vistas
{
    /// <summary>Sueldo base y horas de la jornada del trabajador con que se calculan los conceptos capturados por incidencia.</summary>
    public class BaseCalculoEmpleado
    {
        public decimal SalarioDiario { get; set; }

        /// <summary>Horas de la jornada que se usan para el salario por hora (las del turno, o las del tipo de jornada).</summary>
        public decimal HorasJornada { get; set; }

        /// <summary>Verdadero cuando el trabajador no tiene turno ni tipo de jornada con horas y se usó la jornada legal diurna.</summary>
        public bool HorasPredeterminadas { get; set; }
    }
}
