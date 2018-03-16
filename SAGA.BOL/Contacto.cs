using System;
using System.Collections.Generic;

namespace SAGA.BOL
{
    public class Contacto : Persona
    {
        public string Puesto { get; set; }
        public Guid ClienteId { get; set; }

        public virtual Cliente Cliente { get; set; }
    }
}