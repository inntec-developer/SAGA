using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class InfoCandidato
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Foto { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public  ProcesoCandidato Estatus { get; set; }
        public Candidato Candidato { get; set; }
        public ICollection<AboutMe> AboutMe { get; set; }
        public ICollection<Curso> Cursos { get; set; }
        public ICollection<ConocimientoOHabilidad> Conocimientos { get; set; }
        public ICollection<PerfilIdioma> Idiomas { get; set; }
        public ICollection<Formacion> Formaciones { get; set; }
        public ICollection<ExperienciaProfesional> Experiencias { get; set; }
        public ICollection<Certificacion> Certificaciones { get; set; }
        public Direccion Direccion { get; set; }
        public Email Email { get; set; }
        public ICollection<Telefono> Telefono { get; set; }
        public string Genero { get; set; }
        public List<string> RedSocial { get; set; }
        public Guid propietarioId { get; set; }
    }
}