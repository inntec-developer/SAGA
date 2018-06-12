using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;
using SAGA.DAL;

namespace SAGA.API.Dtos
{
    public class PrivilegiosDtos
    {
        public int Id { get; set; }
        public int IdPadre { get; set; }
        public string Nombre { get; set; }
        public int TipoEstructuraId { get; set; }
        public string TipoEstructura { get; set; }
        public ICollection<PrivilegiosDtos> Children { get; set; }
    }
}