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
        public ICollection<Pais> Paises { get; set; }
    }
}