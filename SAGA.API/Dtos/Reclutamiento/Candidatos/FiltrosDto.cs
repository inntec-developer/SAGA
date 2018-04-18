using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;

namespace SAGA.API.Dtos
{
    public class FiltrosDto
    {
        public Guid IdCandidato { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string curp { get; set; }
        public string rfc { get; set; }
        public string nss { get; set; }
        public int IdPais { get; set; }
        public int? IdEstado { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdColonia { get; set; }
        public string cp { get; set; }
        public ICollection<Formacion> Formaciones { get; set; }
        public ICollection<ExperienciaProfesional> Experiencias { get; set; }
        public ICollection<AboutMe> Acercademi { get; set; }
    }
}