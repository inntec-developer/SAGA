using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Reporte
{
    public class IndicadorDto
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string[] label { get; set; }
        public int cubierta { get; set; }
        public int parcial { get; set; }
        public int medios { get; set; }
        public int cliente { get; set; }
        public int promo { get; set; }
        public int operacion { get; set; }
        public int possicion { get; set; }
        public int Totalcubierta { get; set; }
    }
}