using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;

namespace SAGA.API.Dtos
{
    public class PostulacionesDto
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid RequisicionId { get; set; }
        public int StatusId { get; set; }

        public virtual StatusPostulacion Status { get; set; }

        public ICollection<Requisicion> Requisiciones { get; set; }
    }
}