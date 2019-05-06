using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class PrestacionesdeLeyDto
    {
        public int Id { get; set; }
        public string prestacionLey { get; set; }
        public bool activo { get; set; }
    }
}