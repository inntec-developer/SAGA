using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAGA.BOL
{
    public class ObservacionesPerfil
    {
        [Key]
        public Guid Id { get; set; }
        public string Observaciones { get; set; }
        public Guid DAMFO290Id { get; set; }

        public virtual DAMFO_290 DAMFO290 { get; set; }

        public ObservacionesPerfil()
        {
           this.Id = Guid.NewGuid();
        }
    }
}