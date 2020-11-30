using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;
namespace SAGA.API.Dtos
{
    public class CapturaDto
    {
        public Guid HorarioId { get; set; }
        public Guid UsuarioId { get; set; }
        public DtosPersonales dtosPersonales { get; set; }
        public CandidatosGenerales dtosGenerales { get; set; }
       public CandidatoLaborales dtosLaborales { get; set; }
        public CandidatosExtras dtosExtras { get; set; }
        public DtoBiometricos Biometricos { get; set; }
    }

    public class DtosPersonales
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string NSS { get; set; }
        public int PaisNacimientoId { get; set; }
        public int EstadoNacimientoId { get; set; }
        public int MunicipioNacimientoId { get; set; }
        public int GeneroId { get; set; }
        public int ReclutadorId { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid CandidatoId { get; set; }
        public string Conyuge { get; set; }
        public string NomPadre { get; set; }
        public string NomMadre { get; set; }
        public string NomBeneficiario { get; set; }
        public string Nacionalidad { get; set; }
        public int GradoEstudioId { get; set; }
        public string Observaciones { get; set; }
    }

    public class DtosGenerales
    {
        public int PaisId { get; set; }
        public int EstadoId { get; set; }
        public int MunicipioId { get; set; }
        public int ColoniaId { get; set; }
        public string CodigoPostal { get; set; }
        public string Calle { get; set; }
        public string NumeroInterior { get; set; }
        public string NumeroExterior { get; set; }
        public int EstadoCivilId { get; set; }
        public string ImgUrl { get; set; }
        public string email { get; set; }
        public int GrupoSanguineoId { get; set; }
    }

    public class DtosLaborales
    {
        public DateTime FechaIngreso { get; set; }
        public string ClaveTurno { get; set; }
        public string ClaveSucursal { get; set; }
        public string NoCuenta { get; set; }
        public string Departamento { get; set; }
        public DateTime FechaFormaPago { get; set; }
        public string Puesto { get; set; }
        public string ClaveJefe { get; set; }
        public string ClaveExt { get; set; }
        public string Celula { get; set; }
        public string SoporteFacturacion { get; set; }
        public string Sueldo { get; set; }
        public int BancoId { get; set; }
        public int MotivoId { get; set; }
        public decimal SueldoMensual { get; set; }
        public decimal SueldoDiario { get; set; }
        public int SueldoIntegrado { get; set; }
        public int FormaPagoId { get; set; }


    }

    public class DtoBiometricos
    {
        public Guid CandidatosInfoId { get; set; }
        public byte[] FingerPrint { get; set; }
        public bool Activo { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }
    }

}