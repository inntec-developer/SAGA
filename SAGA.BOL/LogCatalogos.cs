using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class LogCatalogos
    {
        public Guid Id { get; set; }
        public int CatalogoId { get; set; }
        public string Campo { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaAct { get; set; }
        public string TpMov { get; set; }
    }
}
