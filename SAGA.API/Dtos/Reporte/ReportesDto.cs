using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Reporte
{
    public class ReportesDto
    {
        public int[] rowIndex { get; set; }
        public string clave { get; set; }
        public int tipo { get; set; }
        public string fini { get; set; }
        public string ffin { get; set; }
        public int edad { get; set; }
        public int genero { get; set; }
        public List<int> estadoId { get; set; }
        public List<int> ofc { get; set; }
        public List<Guid> emp { get; set; }
        public List<Guid> sol { get; set; }
        public List<int> trcl { get; set; }
        public List<int> coord { get; set; }
        public List<int> stus { get; set; }
        public List<Guid> recl { get; set; }
        public List<Guid> usercoor { get; set; }
        public Guid usuario { get; set; }
    }
}