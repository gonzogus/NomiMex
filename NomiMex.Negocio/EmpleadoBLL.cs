using System;
using System.Collections.Generic;
using NomiMex.Datos;
using NomiMex.Entidades.Constantes;
using NomiMex.Entidades.Documentos;
using NomiMex.Entidades.Catalogos;
using NomiMex.Entidades.Vistas;

namespace NomiMex.Negocio
{
    /// <summary>Reglas del catálogo de Empleados: validación del expediente, integración salarial y baja lógica.</summary>
    public class EmpleadoBLL : BllBase
    {
        private readonly EmpleadoDAL _dal = new EmpleadoDAL();
        private readonly CatalogoDAL _catalogoDal = new CatalogoDAL();
        private readonly PrerrequisitosNomina _prerrequisitos = new PrerrequisitosNomina();

        public List<EmpleadoListado> Listar(int? empresaId, bool soloActivos)
        {
            return Ejecutar(() => _dal.Listar(empresaId, soloActivos));
        }

        /// <summary>Empleados que procesará la nómina de un periodo (activos de la empresa con la periodicidad del periodo).</summary>
        public List<EmpleadoListado> ListarElegibles(int empresaId, int tipoPeriodoId, DateTime fechaFinPeriodo)
        {
            if (empresaId <= 0 || tipoPeriodoId <= 0)
            {
                return new List<EmpleadoListado>();
            }
            return Ejecutar(() => _dal.ListarElegibles(empresaId, tipoPeriodoId, fechaFinPeriodo));
        }

        public List<EmpleadoListado> Filtrar(IEnumerable<EmpleadoListado> empleados, string texto)
        {
            return FiltroBusqueda.Filtrar(empleados, texto, e => e.NumeroEmpleado, e => e.NombreCompleto, e => e.RFC, e => e.CURP,
                e => e.NSS, e => e.Puesto, e => e.Departamento, e => e.Estatus, e => e.Empresa);
        }

        public EmpleadoExpediente ObtenerExpediente(int empleadoId)
        {
            EmpleadoExpediente expediente = Ejecutar(() => _dal.ObtenerExpediente(empleadoId));
            if (expediente == null)
            {
                throw new ExcepcionNegocio("El empleado seleccionado ya no existe.");
            }
            return expediente;
        }

        /// <summary>Calcula factor de integración, SDI y SBC (tope 25 UMA) con la política vigente de la empresa.</summary>
        public IntegracionSalarial CalcularIntegracion(int empresaId, DateTime fechaIngreso, decimal salarioDiario)
        {
            if (empresaId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione la empresa del empleado para integrar el salario.");
            }
            if (salarioDiario <= 0m)
            {
                throw new ExcepcionNegocio("El salario diario debe ser mayor a cero.");
            }

            int aniosServicio = Math.Max(1, AniosCumplidos(fechaIngreso, DateTime.Today));
            ParametrosIntegracion p = Ejecutar(() => _catalogoDal.ObtenerParametrosIntegracion(empresaId, aniosServicio));

            decimal factor = CalculadoraNomina.CalcularFactorIntegracion(p.DiasAguinaldo, p.PrimaVacacionalPorcentaje, p.DiasVacaciones);
            decimal sdi = CalculadoraNomina.CalcularSDI(salarioDiario, factor);
            return new IntegracionSalarial
            {
                SalarioDiario = salarioDiario,
                FactorIntegracion = factor,
                SDI = sdi,
                SBC = CalculadoraNomina.CalcularSBC(sdi, p.UMADiaria),
                UMADiaria = p.UMADiaria
            };
        }

        public int Guardar(EmpleadoExpediente x)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);

            CatEmpleado e = x.Empleado;
            CatEmpleadoDatosFiscales f = x.Fiscales;
            CatEmpleadoDatosIMSS i = x.IMSS;
            DocContratoEmpleado c = x.Contrato;
            CatEmpleadoBancoCuenta cuenta = x.Cuenta;

            // Datos generales
            e.EmpresaId = Validador.SeleccionRequerida(e.EmpresaId, "Empresa");
            e.NumeroEmpleado = Validador.Requerido(e.NumeroEmpleado, "No. de empleado", 15).ToUpperInvariant();
            if (e.NumeroEmpleado.IndexOf('|') >= 0)
            {
                throw new ExcepcionNegocio("El No. de empleado no puede contener el carácter '|'.");
            }
            e.Nombres = Validador.Requerido(e.Nombres, "Nombre(s)", 100);
            e.ApellidoPaterno = Validador.Requerido(e.ApellidoPaterno, "Apellido paterno", 100);
            e.ApellidoMaterno = Validador.Opcional(e.ApellidoMaterno, "Apellido materno", 100);
            e.Nacionalidad = Validador.Opcional(e.Nacionalidad, "Nacionalidad", 50);
            e.Sexo = Validador.Opcional(e.Sexo, "Sexo", 1);
            e.EstadoCivil = Validador.Opcional(e.EstadoCivil, "Estado civil", 20);
            if (string.IsNullOrEmpty(e.Estatus))
            {
                e.Estatus = EstatusEmpleado.Activo;
            }

            if (e.FechaIngreso.Date > DateTime.Today)
            {
                throw new ExcepcionNegocio("La fecha de ingreso no puede ser posterior a hoy.");
            }
            if (e.FechaNacimiento.HasValue && AniosCumplidos(e.FechaNacimiento.Value, e.FechaIngreso) < 15)
            {
                throw new ExcepcionNegocio("El empleado debe tener al menos 15 años cumplidos a la fecha de ingreso (Art. 22 LFT).");
            }

            // Datos fiscales
            f.RFC = Validador.RFC(f.RFC, "RFC", true, true);
            f.CURP = Validador.CURP(f.CURP, "CURP", true);
            f.RegimenFiscalSAT = Validador.SeleccionRequerida(f.RegimenFiscalSAT, "Régimen fiscal");
            f.CodigoPostal = Validador.CodigoPostal(f.CodigoPostal, "Código postal fiscal", true);
            f.ClaveEntidadFederativa = Validador.Opcional(f.ClaveEntidadFederativa, "Estado", 3);
            f.Email = Validador.Correo(f.Email, "Correo electrónico", false, 100);
            f.Telefono = Validador.Opcional(f.Telefono, "Teléfono", 20);

            // IMSS y salario
            i.NSS = Validador.NSS(i.NSS, "NSS");
            i.TipoRiesgo = Validador.SeleccionRequerida(i.TipoRiesgo, "Clase de riesgo");
            if (i.SalarioDiario <= 0m)
            {
                throw new ExcepcionNegocio("El salario diario debe ser mayor a cero.");
            }

            // Contrato
            c.TipoContratoId = Validador.SeleccionRequerida(c.TipoContratoId, "Tipo de contrato");
            c.PeriodicidadPagoId = Validador.SeleccionRequerida(c.PeriodicidadPagoId, "Tipo de periodo de pago");
            c.TipoJornadaId = Validador.SeleccionRequerida(c.TipoJornadaId, "Tipo de jornada");
            c.TipoRegimenId = Validador.SeleccionRequerida(c.TipoRegimenId, "Régimen de contratación");
            if (c.DiasLaboradosSemana.HasValue && (c.DiasLaboradosSemana.Value < 1 || c.DiasLaboradosSemana.Value > 7))
            {
                throw new ExcepcionNegocio("Los días laborados por semana deben estar entre 1 y 7.");
            }
            if (c.FechaInicio == default(DateTime))
            {
                c.FechaInicio = e.FechaIngreso;
            }
            if (c.FechaInicio.Date < e.FechaIngreso.Date)
            {
                throw new ExcepcionNegocio("El contrato no puede iniciar antes de la fecha de ingreso.");
            }

            // Paso 1 y 2 del flujo: el empleado pertenece a una empresa activa y su periodicidad debe estar habilitada para ella
            if (e.Estatus != EstatusEmpleado.Baja)
            {
                _prerrequisitos.ExigirPeriodicidadDeEmpresa(e.EmpresaId, c.PeriodicidadPagoId);
            }
            if (string.IsNullOrEmpty(c.ZonaSMG))
            {
                c.ZonaSMG = "GENERAL";
            }

            decimal salarioMinimo = Ejecutar(() => _catalogoDal.ObtenerSalarioMinimoDiario(DateTime.Today.Year, c.ZonaSMG));
            if (salarioMinimo > 0m && i.SalarioDiario < salarioMinimo)
            {
                throw new ExcepcionNegocio("El salario diario no puede ser menor al salario mínimo vigente (" +
                                           salarioMinimo.ToString("C2") + " zona " + c.ZonaSMG + ").");
            }

            // Cuenta bancaria (opcional)
            if (cuenta != null && !string.IsNullOrWhiteSpace(cuenta.CLABE))
            {
                cuenta.CLABE = cuenta.CLABE.Trim();
                if (!Validador.EsCLABEValida(cuenta.CLABE))
                {
                    throw new ExcepcionNegocio("La CLABE interbancaria no es válida (18 dígitos con dígito verificador correcto).");
                }
                if (!cuenta.BancoId.HasValue || cuenta.BancoId.Value <= 0)
                {
                    throw new ExcepcionNegocio("Seleccione el banco de la cuenta CLABE.");
                }
            }

            if (Ejecutar(() => _dal.ExisteNumeroEmpleado(e.EmpresaId, e.NumeroEmpleado, e.EmpleadoId)))
            {
                throw new ExcepcionNegocio("Ya existe un empleado con el número '" + e.NumeroEmpleado + "' en la empresa seleccionada.");
            }

            // El SDI y el SBC siempre se recalculan en el servidor de negocio; no se confía en lo capturado en pantalla.
            IntegracionSalarial integracion = CalcularIntegracion(e.EmpresaId, e.FechaIngreso, i.SalarioDiario);
            i.SalarioDiarioIntegrado = integracion.SDI;
            i.SBC = integracion.SBC;

            int? usuarioId = SesionActual.HaySesion ? SesionActual.Usuario.UsuarioId : (int?)null;
            bool esAlta = e.EmpleadoId <= 0;
            int idGuardado = Ejecutar(() => _dal.Guardar(x, usuarioId, integracion.FactorIntegracion));

            // Al dar de alta un trabajador se precarga el paquete de incidencias por defecto de su periodicidad.
            if (esAlta && e.Estatus != EstatusEmpleado.Baja)
            {
                new ConceptoIncidenciaBLL().AsignarPaqueteDefault(idGuardado, e.EmpresaId, c.PeriodicidadPagoId);
            }
            return idGuardado;
        }

        public void DarDeBaja(int empleadoId)
        {
            ControlAcceso.Exigir(ModuloSistema.Empleados, AccionSistema.Escribir);
            if (empleadoId <= 0)
            {
                throw new ExcepcionNegocio("Seleccione un empleado.");
            }
            Ejecutar(() => _dal.DarDeBajaLogica(empleadoId));
        }

        private static int AniosCumplidos(DateTime desde, DateTime hasta)
        {
            int anios = hasta.Year - desde.Year;
            if (hasta.Date < desde.Date.AddYears(anios))
            {
                anios--;
            }
            return anios;
        }
    }
}
