using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class TicketReclutador
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ReclutadorId { get; set; }
        public Guid TicketId { get; set; }
        public DateTime fch_Atencion { get; set; }
        public DateTime fch_Final { get; set; }

        public Usuarios Reclutador { get; set; }
        public Ticket Ticket { get; set; }
    }
}
