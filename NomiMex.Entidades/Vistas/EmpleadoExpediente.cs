using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Documentos;

namespace NomiMex.Entidades.Vistas
{
    /// <summary>
    /// Agregado de edición de un empleado: reúne las tablas CatEmpleado, CatEmpleadoDatosFiscales,
    /// CatEmpleadoDatosIMSS, DocContratoEmpleado y CatEmpleadoBancoCuenta que se guardan en una sola transacción.
    /// </summary>
    public class EmpleadoExpediente
    {
        public EmpleadoExpediente()
        {
            Empleado = new CatEmpleado();
            Fiscales = new CatEmpleadoDatosFiscales();
            IMSS = new CatEmpleadoDatosIMSS();
            Contrato = new DocContratoEmpleado();
            Cuenta = new CatEmpleadoBancoCuenta();
        }

        public CatEmpleado Empleado { get; set; }
        public CatEmpleadoDatosFiscales Fiscales { get; set; }
        public CatEmpleadoDatosIMSS IMSS { get; set; }
        public DocContratoEmpleado Contrato { get; set; }
        public CatEmpleadoBancoCuenta Cuenta { get; set; }
    }
}
