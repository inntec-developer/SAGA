using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class CoincidenciasDto
    {
        public string Nombre { get; set; }
        public string Subcategoria { get; set; }
        public int AreaExpId { get; set; }
        public decimal SueldoMinimo { get; set; }
        public decimal SueldoMaximo { get; set; }
        public string Genero { get; set; }
        public int GeneroId { get; set; }
        public string EstadoCivil { get; set; }
        public int EstadoCivilId { get; set; }
        public string Formaciones { get; set; }
        public int FormacionId { get; set; }
        public int Edad { get; set; }
    }

}