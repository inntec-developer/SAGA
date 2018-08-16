﻿using System;

namespace SAGA.BOL
{
    public class Postulacion
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid RequisicionId { get; set; }
        public int StatusId { get; set; }
        public DateTime fch_Postulacion { get; set; }

        public Candidato Candidato { get; set; }
        public  Requisicion Requisicion { get; set; }
        public virtual StatusPostulacion Status { get; set; }
    }
}