using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_EstatusEmails
    {
        [Key]
        public int Id { get; set; }
        public int EstatusBitacoraId { get; set; }
        public int ConfigBitacoraId { get; set; }
        public string Email { get; set; }

        public FIRM_EstatusBitacora EstatusBitacora { get; set; }
    }
}
