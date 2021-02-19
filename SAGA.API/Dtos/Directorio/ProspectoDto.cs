using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;

namespace SAGA.API.Dtos
{
    public class ProspectoDto
    {
        public string Clave { get; set; }
        public int ActividadEmpresaId { get; set; }
        public int GiroEmpresaId { get; set; }
        public string Nombrecomercial { get; set; }
        public int TamanoEmpresaId { get; set; }
        public int NumeroEmpleados { get; set; }
        public int TipoEmpresaId { get; set; }
        public int TipoBaseId { get; set; }
        public string Clasificacion { get; set; }
        public string Usuario { get; set; }
        public ICollection<Direccion> Direcciones { get; set; }
        public ICollection<Telefono> Telefonos { get; set; }
        public ICollection<Email> Emails { get; set; }
        public ICollection<Contacto> Contactos { get; set; }
        public ICollection<DireccionEmailDto> DireccionEmail { get; set; }
        public ICollection<DireccionTelefonoDto> DireccionTelefono { get; set; }
        public ICollection<DireccionContactoDto> DireccionContacto { get; set; }
    }

    public class DireccionEmailDto
    {
        public string Calle { get; set; }
        public string NumeroInterior { get; set; }
        public string NumeroExterior { get; set; }
        public string CodigoPostal { get; set; }
        public string Email { get; set; }
    }

    public class DireccionTelefonoDto
    {
        public string Calle { get; set; }
        public string NumeroInterior { get; set; }
        public string NumeroExterior { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string Extension { get; set; }
    }
    public class DireccionContactoDto
    {
        public string Calle { get; set; }
        public string NumeroInterior { get; set; }
        public string NumeroExterior { get; set; }
        public string CodigoPostal { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Puesto { get; set; }
    }
}