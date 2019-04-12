using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class ContactoClienteDto
    {
        public Guid Id { get; set; }
        public Guid DireccionId { get; set; }
        public Guid IdDCn { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public int TipoEntidadId { get; set; }
        
        public string Puesto { get; set; }
        public Guid ClienteId { get; set; }
        public string Usuario { get; set; }

        public virtual ICollection<Telefono> telefonos { get; set; }
        public virtual ICollection<Email> emails { get; set; }
    }
}