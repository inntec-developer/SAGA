using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class RutaCamionDto
    {
        public Guid Id { get; set; }
        public Guid DireccionId { get; set; }
        public string Ruta { get; set; }
        public string Via { get; set; }
        public string Usuario { get; set; }
    }
}