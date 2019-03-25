using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class HistoricoTicket
    {
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public Guid ReclutadorId { get; set; }
        public Guid CandidatoId { get; set; }
        public string Numero { get; set; }
        public int MovimientoId { get; set; }
        public int Estatus { get; set; }
        public int ModuloId { get; set; }
        public DateTime fch_Modificacion { get; set; }

        public virtual Requisicion Requisicion { get; set; }
        public virtual ModulosReclutamiento Modulo { get; set; }

    }
}
