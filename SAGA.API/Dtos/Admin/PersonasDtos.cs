using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;
using SAGA.DAL;

namespace SAGA.API.Dtos
{
    public class PersonasDtos
    {
        public Guid Id { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string tipoUsuario { get; set; }
        public string Usuario { get; set; } //alias
        public ICollection<Email> Email { get; set; }
        public Guid DepartamentoId { get; set; }
        public string Clave { get; set; }
        public string UsuarioAlta { get; set; }
        public string Password { get; set; }
        public string Departamento { get; set;  }
        public bool Activo { get; set; }


    }
}