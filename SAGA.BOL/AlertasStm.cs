using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class AlertasStm
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EntidadId { get; set; }
        public string Alert { get; set; }
        public int TipoAlertaId { get; set; }
        public string Icon { get; set; }
        public DateTime Creacion { get; set; }
        public bool Activo { get; set; }

        public Entidad Entidad { get; set; }
        public TipoAlerta TipoAlerta { get; set; }
    }
}
