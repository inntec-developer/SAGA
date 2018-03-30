using System;
using System.Collections.Generic;


namespace SAGA.BOL
{
    public class Postulacion
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid RequisicionId { get; set; }
        public int StatusId { get; set; }

        public Candidato Candidato { get; set; }
        public Requisicion Requisicion { get; set; }
        public virtual StatusPostulacion Status { get; set; }
    }
}