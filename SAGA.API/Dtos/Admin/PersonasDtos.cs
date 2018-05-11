using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;
using SAGA.DAL;

namespace SAGA.API.Dtos.Admin
{
    public class PersonasDtos
    {
        public Guid Id { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string tipoUsuario { get; set; }
        public string Usuario { get; set; } //alias
        public string Email { get; set; }
        public string fechaInicio { get; set; }
        public string Departamento { get; set; }
       
    }
}