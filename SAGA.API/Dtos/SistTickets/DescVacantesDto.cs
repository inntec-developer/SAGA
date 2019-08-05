using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.SistTickets
{
    public class DescVacantesDto
    {
        public Guid Id { get; set; }
        public string Estatus { get; set; }
        public string Folio { get; set; }
        public string VBtra { get; set; }
        public string Experiencia { get; set; }
        public string Categoria { get; set; }
        public string Icono { get; set; }
        public int AreaId { get; set; }
        public int Cubierta { get; set; }
        public string Arte { get; set; }
        public FileStream fsarte { get; set; }
    }
}