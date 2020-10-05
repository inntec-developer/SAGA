using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_CausaEfecto
    {
        [Key]
        public Guid Id { get; set; }
        public Guid Damfo022Id { get; set; }
        public FIRM_Damfo022 Damfo022 { get; set; }
        public byte IshikawaId { get; set; }
        public FIRM_Ishikawa Ishikawa { get; set; }

    }
}
