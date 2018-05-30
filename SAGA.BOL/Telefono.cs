using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Telefono
    {
        [Key]
        public Guid Id { get; set; }
        public string ClavePais { get; set; }
        public String ClaveLada { get; set; }
        public String Extension { get; set; }
        public string telefono { get; set; }
        public byte TipoTelefonoId { get; set; }
        public virtual TipoTelefono TipoTelefono { get; set; }
        public bool Activo { get; set; }
        public bool esPrincipal { get; set; }
        public Guid EntidadId { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public virtual Entidad Entidad { get; set; }

        public Telefono(string clavePais, string telefono, byte tipoTelefono, Guid idPersona)
        {
            ClavePais = clavePais;
            this.telefono = telefono;
            TipoTelefonoId = tipoTelefono;
            EntidadId = idPersona;
            this.Id = Guid.NewGuid();
        }

        public Telefono()
        {
            this.Id = Guid.NewGuid();
        }
    }
}