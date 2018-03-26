using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class Damfo290GralDto
    {
        public Guid Id { get; set; }
        public string Cliente { get; set; }
        public string NombrePerfil { get; set; }
        public string GiroEmpresa { get; set; }
        public string ActividadEmpresa { get; set; }
        public string TipoReclutamiento { get; set; }
        public string ClaseReclutamiento { get; set; }
        public decimal SueldoMinimo { get; set; }
        public decimal SueldoMaximo { get; set; }
        public DateTime fch_Creacion { get; set; }
    }
}