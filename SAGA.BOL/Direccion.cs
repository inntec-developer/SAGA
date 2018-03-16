using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Direccion
    {
        [Key]       
        public Guid Id { get; set; }
        public int TipoDireccionId { get; set; }
        public virtual TipoDireccion TipoDireccion { get; set; }
        public bool esMoral { get; set; } 
        public string Calle { get; set; } 
        public string NumeroInterior { get; set; } 
        public string NumeroExterior { get; set; } 
        public int PaisId { get; set; }
        public virtual Pais Pais { get; set; } 
        public int? EstadoId { get; set; }         
        public virtual Estado Estado { get; set; } 
        public int? MunicipioId { get; set; }        
        public virtual Municipio Municipio { get; set; }        
        public int?  ColoniaId { get; set; }
        public virtual Colonia Colonia { get; set; }
        public string CodigoPostal { get; set; }
        public bool esPrincipal { get; set; } 
        public bool Activo { get; set; }
        public string Referencia { get; set; }
        public Guid PersonaId { get; set; }
        public virtual Persona PersonaFisicaMoral { get; set; }

        public Direccion()
        {
            this.Id = Guid.NewGuid();
        }
    }
}