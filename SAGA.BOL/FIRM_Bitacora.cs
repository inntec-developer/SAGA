using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_Bitacora
    {
        [Key]
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid PropietarioId { get; set; }
        public bool Retardo { get; set; }
        public bool Porques { get; set; }

        public Guid FechasEstatusId { get; set; }
        public FIRM_FechasEstatus FechasEstatus { get; set; }
    }
}
