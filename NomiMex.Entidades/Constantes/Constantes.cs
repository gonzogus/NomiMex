namespace NomiMex.Entidades.Constantes
{
    /// <summary>Códigos de rol definidos en dbo.CatRol.</summary>
    public static class RolesSistema
    {
        public const string SuperAdmin = "SUPERADMIN";
        public const string RHNomina = "RH_NOMINA";
        public const string DirFinanzas = "DIR_FINANZAS";
        public const string Contabilidad = "CONTABILIDAD";
        public const string Consulta = "CONSULTA";
    }

    /// <summary>Estatus válidos de ProcPeriodoNomina.Estatus.</summary>
    public static class EstatusPeriodo
    {
        public const string Creado = "CREADO";
        public const string Prenomina = "PRENOMINA";
        public const string EnCalculo = "EN_CALCULO";
        public const string Calculado = "CALCULADO";
        public const string Autorizado = "AUTORIZADO";
        public const string Cerrado = "CERRADO";
        public const string Timbrado = "TIMBRADO";
        public const string Cancelado = "CANCELADO";
    }

    /// <summary>Estatus válidos de DocReciboNomina.Estatus.</summary>
    public static class EstatusRecibo
    {
        /// <summary>Borrador guardado desde la prenómina: aún no es un recibo definitivo del periodo.</summary>
        public const string Prerecibo = "PRERECIBO";
        public const string Calculado = "CALCULADO";
        public const string Autorizado = "AUTORIZADO";
        public const string Timbrado = "TIMBRADO";
        public const string Cancelado = "CANCELADO";
    }

    /// <summary>Estatus del trabajador dentro de la prenómina de un periodo.</summary>
    public static class EstatusTrabajadorNomina
    {
        public const string Pendiente = "Pendiente";
        public const string ConIncidencias = "Con Incidencias";
        public const string Calculado = "Calculado";
    }

    /// <summary>Estatus válidos de CatEmpleado.Estatus.</summary>
    public static class EstatusEmpleado
    {
        public const string Activo = "ACTIVO";
        public const string Baja = "BAJA";
        public const string Incapacidad = "INCAPACIDAD";
        public const string Suspendido = "SUSPENDIDO";
    }

    /// <summary>Claves SAT (c_PeriodicidadPago) de dbo.CatPeriodicidadPago que la lógica de negocio necesita conocer.</summary>
    public static class ClavePeriodicidad
    {
        public const string Semanal = "02";
        public const string Catorcenal = "03";
        public const string Quincenal = "04";
        public const string Mensual = "05";
        public const string Decenal = "10";
    }

    /// <summary>Claves SAT (c_OrigenRecurso) que la lógica de negocio necesita conocer.</summary>
    public static class ClaveOrigenRecurso
    {
        public const string RecursoMixto = "IM";
    }

    /// <summary>Valores de CatDiaFestivo.Tipo.</summary>
    public static class TipoDiaFestivo
    {
        public const string Ley = "LEY";
        public const string Empresa = "EMPRESA";
    }

    /// <summary>Códigos de las incidencias legales (CatConcepto.ClaveSAT con TipoConcepto 'I') que el cálculo de nómina interpreta.</summary>
    public static class CodigoIncidencia
    {
        public const string Falta = "FALTA";
        public const string HoraExtraDoble = "HE_DOBLE";
        public const string HoraExtraTriple = "HE_TRIPLE";
        public const string PrimaDominical = "PRIMA_DOM";
        public const string DiaFestivoLaborado = "DIAS_FESTIVOS";
        public const string Bono = "BONO";
        public const string Descuento = "DESCUENTO";
    }

    /// <summary>Unidades de captura de una incidencia legal (derivadas de CatConcepto.FormaCalculo: DIAS, HORAS o MONTO_FIJO).</summary>
    public static class UnidadIncidencia
    {
        public const string Dia = "DIA";
        public const string Hora = "HORA";
        public const string Monto = "MONTO";
    }
}
