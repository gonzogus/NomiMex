using System;
using System.Collections.Generic;
using NomiMex.Datos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>Catálogo de días festivos (de ley y de empresa) por año fiscal.</summary>
    public class DiaFestivoBLL : BllBase
    {
        private readonly DiaFestivoDAL _dal = new DiaFestivoDAL();
        private readonly PrerrequisitosNomina _prerrequisitos = new PrerrequisitosNomina();

        public List<DiaFestivoListado> Listar(int anio, int? empresaId)
        {
            return Ejecutar(() => _dal.Listar(anio, empresaId));
        }

        public int Agregar(DateTime fecha, string descripcion, string tipo, int? empresaId)
        {
            ControlAcceso.Exigir(ModuloSistema.Periodos, AccionSistema.Escribir);

            if (fecha.Year < 2000 || fecha.Year > 2100)
            {
                throw new ExcepcionNegocio("La fecha del día festivo debe estar entre los años 2000 y 2100.");
            }
            string texto = Validador.Requerido(descripcion, "Descripción", 100);

            if (tipo != TipoDiaFestivo.Ley && tipo != TipoDiaFestivo.Empresa)
            {
                throw new ExcepcionNegocio("Seleccione si el día festivo es de ley o de empresa.");
            }
            if (tipo == TipoDiaFestivo.Empresa)
            {
                if (!empresaId.HasValue || empresaId.Value <= 0)
                {
                    throw new ExcepcionNegocio("Seleccione la empresa a la que pertenece el día festivo.");
                }
                _prerrequisitos.ExigirEmpresaActiva(empresaId.Value);
            }
            else
            {
                empresaId = null;
            }

            CatDiaFestivo festivo = new CatDiaFestivo
            {
                EmpresaId = empresaId,
                Fecha = fecha.Date,
                Descripcion = texto,
                Tipo = tipo
            };
            return Ejecutar(() => _dal.Insertar(festivo));
        }

        public void CambiarEstado(int diaFestivoId, bool activo)
        {
            ControlAcceso.Exigir(ModuloSistema.Periodos, AccionSistema.Escribir);
            Validador.SeleccionRequerida(diaFestivoId, "Día festivo");
            Ejecutar(() => _dal.CambiarEstado(diaFestivoId, activo));
        }

        /// <summary>Genera los días de descanso obligatorio de ley (Art. 74 LFT) del año. Devuelve cuántos festivos de ley tiene el año.</summary>
        public int GenerarFestivosLey(int anio)
        {
            ControlAcceso.Exigir(ModuloSistema.Periodos, AccionSistema.Escribir);
            if (anio < 2000 || anio > 2100)
            {
                throw new ExcepcionNegocio("El año debe estar entre 2000 y 2100.");
            }
            return Ejecutar(() => _dal.GenerarFestivosLey(anio));
        }
    }
}
