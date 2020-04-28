using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class DocumentosCandidato
    {
        [Key]
        public int Id { get; set; }
        public int documentoId { get; set; }
        public Guid candidatoId { get; set; }
        public DateTime fch_Creacion { get; set; }
        public Guid usuarioId { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid usuarioMod { get; set; }
        public string Ruta { get; set; }

        public virtual Documentos Documento { get; set; }
        public virtual CandidatosInfo Candidato { get; set; }
        public virtual Usuarios Usuario { get; set; }
    }
}
