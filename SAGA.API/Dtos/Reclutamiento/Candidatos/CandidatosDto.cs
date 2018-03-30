using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;
using SAGA.DAL;

namespace SAGA.API.Dtos
{
    public class CandidatosDto
    {

        public ICollection<CandidatosGralDto> Candidatos { get; set; }
        //Filtros
        public ICollection<Pais> Paises { get; set; }
        public ICollection<Estado> Estados { get; set; }
        public ICollection<Municipio> Municipios { get; set; }
        public ICollection<Colonia> Colonias { get; set; }

        public ICollection<ExperienciaProfesional> ExperenciasProfesionales { get; set; }

    }
}