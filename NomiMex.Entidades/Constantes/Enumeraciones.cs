namespace NomiMex.Entidades.Constantes
{
    /// <summary>Módulos de la aplicación sujetos a control de acceso por rol.</summary>
    public enum ModuloSistema
    {
        Empresa,
        Empleados,
        Puestos,
        Departamentos,
        Estados,
        Usuarios,
        Nomina,
        Periodos,
        Recibos,
        Configuracion
    }

    /// <summary>Acciones que un rol puede ejecutar dentro de un módulo.</summary>
    public enum AccionSistema
    {
        Consultar,
        Escribir,
        Calcular,
        Autorizar
    }
}
