using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class AsignacionRequi
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public Guid GrpUsrId { get; set; }
        public string CRUD { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }


        public virtual Requisicion Requisicion { get; set; }
        public virtual Entidad GrpUsr { get; set; }

        public AsignacionRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}