using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class EstadoCivilDto
    {
        public byte Id { get; set; }
        public string estadoCivil { get; set; }
        public bool Activo { get; set; }
    }
}