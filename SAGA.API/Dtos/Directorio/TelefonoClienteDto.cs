using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class TelefonoClienteDto
    {
        public Guid Id { get; set; }
        public string ClavePais { get; set; }
        public String ClaveLada { get; set; }
        public String Extension { get; set; }
        public string telefono { get; set; }
        public byte TipoTelefonoId { get; set; }
        public bool Activo { get; set; }
        public bool esPrincipal { get; set; }
        public Guid EntidadId { get; set; }
        public string Usuario { get; set; }
        public Guid DireccionId { get; set; }
        public Guid IdDT { get; set; }

    }
}