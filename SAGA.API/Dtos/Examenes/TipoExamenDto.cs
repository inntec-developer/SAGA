using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Examenes
{
    public class TipoExamenDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Resultado { get; set; }
        public bool Facturado { get; set; }
        public Guid requisicionId { get; set; }
        public Guid candidatoId { get; set; }
        public Guid clienteId { get; set; }
    }
}