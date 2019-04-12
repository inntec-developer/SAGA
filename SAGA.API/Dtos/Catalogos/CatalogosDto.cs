using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class CatalogosDto
    {
        public Catalogos Catalogos { get; set; }
        public List<Pais> Pais { get; set; }
        public List<EstadoDto> Estado { get; set; }
        public List<Municipio> Municipio { get; set; }
    }
}