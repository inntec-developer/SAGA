using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class Damfo290Dto
    {
        public ICollection<Damfo290GralDto> Damfo290Gral { get; set; }
        public ICollection<Damfo290AllDto> Damfo290All { get; set; }
    }
}   