using System;

namespace NomiMex.Entidades.Catalogos
{
    /// <summary>Tabla dbo.CatDiaFestivo: festivos de ley (EmpresaId nulo) y festivos propios de una empresa.</summary>
    public class CatDiaFestivo
    {
        public int DiaFestivoId { get; set; }
        public int? EmpresaId { get; set; }
        public DateTime Fecha { get; set; }
        public int Anio { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public bool Activo { get; set; }
    }
}
