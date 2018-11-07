using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class FolioIncidencia
    {
        [Key]
        public Guid Id { get; set; }
        public int EstatusId { get; set; }
        public string Folio { get; set; }
        public Guid ComentarioId { get; set; }

        public virtual Estatus Estatus { get; set; }
        public virtual ComentarioVacante Comentario { get; set; }
    }
}
