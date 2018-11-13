using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class FolioIncidenciasCandidatos
    {
        [Key]
        public Guid Id { get; set; }
        public int EstatusId { get; set; }
        public string Folio { get; set; }
        public Guid ComentarioId { get; set; }

        public virtual Estatus Estatus { get; set; }
        public virtual ComentarioEntrevista Comentario { get; set; }
    }
}
