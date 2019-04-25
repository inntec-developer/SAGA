using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class ColoniasDto
    {
        public int Id { get; set; }
        public string colonia { get; set; }
        public string CP { get; set; }
        public string TipoColonia { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public bool Activo { get; set; }
    }
}