using System;
using System.Collections.Generic;

namespace SAGA.BOL
{
    public class PerfilCandidato
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public virtual Candidato Candidato { get; set; }
        public virtual ICollection<AboutMe> AboutMe { get; set; }
        public virtual ICollection<Curso> Cursos { get; set; }
        public virtual ICollection<ConocimientoOHabilidad> Conocimientos { get; set; }
        public virtual ICollection <PerfilIdioma> Idiomas { get; set; }
        public virtual ICollection<Formacion> Formaciones { get; set; }
        public virtual ICollection<ExperienciaProfesional> Experiencias { get; set; }
        public virtual ICollection<Certificacion> Certificaciones { get; set; }

        public PerfilCandidato()
        {
            AboutMe = new List<AboutMe>();
            Idiomas = new List<PerfilIdioma>();
            Formaciones = new List<Formacion>();
            Cursos = new List<Curso>();
            Conocimientos = new List<ConocimientoOHabilidad>();
            Experiencias = new List<ExperienciaProfesional>();
            Certificaciones = new List<Certificacion>();
            this.Id = Guid.NewGuid();
        }

        public PerfilCandidato(Guid CandidatoId)
        {
            this.CandidatoId = CandidatoId;
            this.Id = Guid.NewGuid();
        }
    }
}