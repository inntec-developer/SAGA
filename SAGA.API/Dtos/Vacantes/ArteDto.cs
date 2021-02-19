using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Vacantes
{
    public class ArteDto
    {
        public string crud { get; set; }
        public string arte { get; set; }
        public Guid requisicionId { get; set; }
        public ArteRequi ArteRequi { get; set; }
        public List<ArteRequi> ArteRequiList { get; set; }
    }
}