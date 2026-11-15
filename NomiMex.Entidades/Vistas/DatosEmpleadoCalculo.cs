using System;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>Datos del trabajador que intervienen en el cálculo del pre-recibo (salarios, deducciones fijas e identidad).</summary>
    public class DatosEmpleadoCalculo
    {
        public int EmpleadoId { get; set; }
        public string NumeroEmpleado { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }

        public decimal SalarioDiario { get; set; }
        public decimal SDI { get; set; }
        public decimal SBC { get; set; }

        /// <summary>Horas de la jornada del trabajador (turno o tipo de jornada); 0 si no hay ninguna configurada.</summary>
        public decimal HorasJornada { get; set; }

        /// <summary>Días laborados por semana vigentes en el periodo (ajuste del periodo, empleado o jornada); nulo = jornada completa.</summary>
        public int? DiasLaboradosSemana { get; set; }

        /// <summary>Verdadero si los días por semana provienen de un ajuste solo para este periodo y no del contrato.</summary>
        public bool DiasSemanaSoloPeriodo { get; set; }

        /// <summary>CUOTA_FIJA, PORCENTAJE o VSM del crédito INFONAVIT activo; nulo si no tiene.</summary>
        public string TipoDescuentoInfonavit { get; set; }

        public decimal MontoDescuentoInfonavit { get; set; }

        /// <summary>Descuento de préstamos internos activos para el periodo (sin rebasar el saldo restante).</summary>
        public decimal DescuentoPrestamos { get; set; }

        /// <summary>% de prima dominical resuelto para el trabajador (override empresa/periodo/empleado); 0 = usar el de la política.</summary>
        public decimal PorcentajePrimaDominical { get; set; }

        /// <summary>Factor de horas extra dobles resuelto para el trabajador; 0 = usar el de la política.</summary>
        public decimal FactorHoraExtraDoble { get; set; }

        /// <summary>Tope de exención de prima dominical (UMA) resuelto para el trabajador; 0 = usar el de la política.</summary>
        public decimal ExencionPrimaDominicalUMAs { get; set; }

        /// <summary>% de prima vacacional resuelto (override empresa/periodo/empleado); 0 = usar 25%.</summary>
        public decimal PorcentajePrimaVacacional { get; set; }

        /// <summary>Tope de exención de aguinaldo (UMA) resuelto; 0 = usar 30.</summary>
        public decimal TopeUMAAguinaldo { get; set; }

        /// <summary>Tope de exención de prima vacacional (UMA) resuelto; 0 = usar 15.</summary>
        public decimal TopeUMAPrimaVacacional { get; set; }
    }
}
