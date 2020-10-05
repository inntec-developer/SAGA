using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Equipos
{
    public class ReclutadoresDto
    {
        public Guid clienteId { get; set; }
        public Guid requisicionId { get; set; }
        public Guid reclutadorId { get; set; }
        public int posiciones { get; set; }
        public int cubiertas { get; set; }
        public int tipoUsuario { get; set; }
        public string tipo { get; set; }
        public string nombre { get; set; }
        public string foto { get; set; }
    }
}