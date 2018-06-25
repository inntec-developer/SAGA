using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class AsignadosDto
    {
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public Guid GrpUsrId { get; set; }
        public string CRUD { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }
    }
}