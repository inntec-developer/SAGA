
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAGA.BOL
{
    public partial class Cliente : Persona
    {
        public string RazonSocial { get; set; }
        public string Nombrecomercial { get; set; }
        public string RFC { get; set; }
        public int GiroEmpresaId { get; set; }
        public int ActividadEmpresaId { get; set; }
        public int TamanoEmpresaId { get; set; }
        public int TipoEmpresaId { get; set; }
        public int TipoBaseId { get; set; }
        public bool otraAgencia { get; set; }
       
        public bool esCliente { get; set; }
        public string Clasificacion { get; set; }
        public int NumeroEmpleados { get; set; }
        public bool Activo { get; set; } = true;
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime fch_Modificacion { get; set; }

        public virtual GiroEmpresa GiroEmpresas { get; set; }
        public virtual ActividadEmpresa ActividadEmpresas { get; set; }
        public virtual TamanoEmpresa TamanoEmpresas { get; set; }
        public virtual TipoEmpresa TipoEmpresas { get; set; }
        public virtual TipoBase TipoBases { get; set; }


        public virtual ICollection<Agencia> Agencias { get; set; }
        public virtual ICollection<Referenciado> Referenciados { get; set; }
        public virtual ICollection<Contacto> Contactos { get; set; }
    }
}