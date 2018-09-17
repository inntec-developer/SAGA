using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class MiCVUpload
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public string UrlCV { get; set; }

        public MiCVUpload()
        {
            this.Id = Guid.NewGuid();
        }

    }
}
