using System;
using System.Collections.Generic;
using System.Linq;
using NomiMex.Entidades.Constantes;

namespace NomiMex.Negocio
{
    /// <summary>
    /// Control de acceso por rol (CatRol). Define qué acciones puede ejecutar cada rol en cada módulo.
    /// Los servicios de negocio lo invocan antes de escribir, de modo que la restricción no depende solo de la UI.
    /// </summary>
    public static class ControlAcceso
    {
        private static readonly Dictionary<string, Dictionary<ModuloSistema, AccionSistema[]>> Matriz =
            new Dictionary<string, Dictionary<ModuloSistema, AccionSistema[]>>();

        private static readonly AccionSistema[] Lectura = { AccionSistema.Consultar };
        private static readonly AccionSistema[] Edicion = { AccionSistema.Consultar, AccionSistema.Escribir };
        private static readonly AccionSistema[] Todas =
        {
            AccionSistema.Consultar, AccionSistema.Escribir, AccionSistema.Calcular, AccionSistema.Autorizar
        };

        static ControlAcceso()
        {
            Matriz[RolesSistema.SuperAdmin] = Enum.GetValues(typeof(ModuloSistema)).Cast<ModuloSistema>()
                .ToDictionary(m => m, m => Todas);

            Matriz[RolesSistema.RHNomina] = new Dictionary<ModuloSistema, AccionSistema[]>
            {
                { ModuloSistema.Empresa, Lectura },
                { ModuloSistema.Empleados, Edicion },
                { ModuloSistema.Puestos, Edicion },
                { ModuloSistema.Departamentos, Edicion },
                { ModuloSistema.Estados, Edicion },
                { ModuloSistema.Nomina, new[] { AccionSistema.Consultar, AccionSistema.Escribir, AccionSistema.Calcular } },
                { ModuloSistema.Periodos, Edicion },
                { ModuloSistema.Recibos, Lectura }
            };

            Matriz[RolesSistema.DirFinanzas] = new Dictionary<ModuloSistema, AccionSistema[]>
            {
                { ModuloSistema.Empresa, Lectura },
                { ModuloSistema.Empleados, Lectura },
                { ModuloSistema.Nomina, new[] { AccionSistema.Consultar, AccionSistema.Autorizar } },
                { ModuloSistema.Periodos, Lectura },
                { ModuloSistema.Recibos, Lectura }
            };

            Matriz[RolesSistema.Contabilidad] = new Dictionary<ModuloSistema, AccionSistema[]>
            {
                { ModuloSistema.Empresa, Lectura },
                { ModuloSistema.Nomina, Lectura },
                { ModuloSistema.Periodos, Lectura },
                { ModuloSistema.Recibos, Lectura }
            };

            Matriz[RolesSistema.Consulta] = new Dictionary<ModuloSistema, AccionSistema[]>
            {
                { ModuloSistema.Empresa, Lectura },
                { ModuloSistema.Empleados, Lectura },
                { ModuloSistema.Puestos, Lectura },
                { ModuloSistema.Departamentos, Lectura },
                { ModuloSistema.Estados, Lectura },
                { ModuloSistema.Periodos, Lectura },
                { ModuloSistema.Recibos, Lectura }
            };
        }

        public static bool Puede(ModuloSistema modulo, AccionSistema accion)
        {
            if (!SesionActual.HaySesion)
            {
                return false;
            }

            foreach (string rol in SesionActual.Usuario.Roles)
            {
                Dictionary<ModuloSistema, AccionSistema[]> permisos;
                AccionSistema[] acciones;
                if (Matriz.TryGetValue(rol, out permisos) && permisos.TryGetValue(modulo, out acciones) && acciones.Contains(accion))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool PuedeAcceder(ModuloSistema modulo)
        {
            return Puede(modulo, AccionSistema.Consultar);
        }

        public static void Exigir(ModuloSistema modulo, AccionSistema accion)
        {
            if (!Puede(modulo, accion))
            {
                throw new ExcepcionNegocio("Su rol no tiene permiso para realizar esta operación (" + modulo + " / " + accion + ").");
            }
        }
    }
}

