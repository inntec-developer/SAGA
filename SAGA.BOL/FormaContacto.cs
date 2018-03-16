using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.BOL
{
    public class FormaContacto
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Candidato candidato { get; set; }
        public Boolean CorreoElectronico { get; set; }
        public Boolean Celular { get; set; }
        public Boolean WhatsApp { get; set; }
        public Boolean TelLocal { get; set; }

        public FormaContacto()
        {
            this.Id = Guid.NewGuid();
        }

        public FormaContacto(Guid candidatoId, bool correo, bool celular, bool whatsapp, bool telLocal )
        {
            CandidatoId = candidatoId;
            CorreoElectronico = correo;
            Celular = celular;
            WhatsApp = whatsapp;
            TelLocal = telLocal;
            this.Id = Guid.NewGuid();
        }
    }

}