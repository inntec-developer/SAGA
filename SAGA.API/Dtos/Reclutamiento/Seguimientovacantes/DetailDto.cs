using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;
using SAGA.DAL;
using System.Collections.ObjectModel;

namespace SAGA.API.Dtos
{
    public class DetailDto
    {
        public Requisicion Requisicion { get; set; }
        public ICollection<DocumentosDamsa> DocumentosDamsa { get; set; }
        public ICollection<PrestacionLey> PrestacionesLey { get; set; }
    }
}