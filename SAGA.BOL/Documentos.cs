using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{ 
    /// <summary>
    /// catalogo de documentos requeridos para todo el sistema
    /// </summary>
    public class Documentos
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public string Observaciones { get; set; }
        public int TipoDocumentoId { get; set; } // por lo pronto solo tendremos reclutamiento 
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificiacion { get; set; }
        public Guid usuarioId { get; set; }

        public virtual TipoDocumentos TipoDocumento { get; set; }


    }
}
