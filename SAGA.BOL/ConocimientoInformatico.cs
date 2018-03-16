using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.BOL
{
    public class ConocimientoInformatico
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }

        public ConocimientoInformatico()
        {
            this.Id = Guid.NewGuid();
        }
    }
}