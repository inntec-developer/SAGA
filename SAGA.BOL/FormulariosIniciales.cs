using System;
using System.ComponentModel;

namespace SAGA.BOL
{
    public class FormulariosIniciales
    {
        public Guid Id { get; set; }
        public int Paso { get; set; }

        public Guid CandidatoId { get; set; }
        public virtual Candidato Candidato { get; set; }

        public FormulariosIniciales()
        {
            this.Id = Guid.NewGuid();
        }
    }
}