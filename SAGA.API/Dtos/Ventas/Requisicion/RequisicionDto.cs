using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class RequisicionDto
    {
        public ICollection<Requi290GralDto> Requi290 { get; set; }
        public ICollection<Requi290AllDto> Dafmo290All { get; set; }
        public ICollection<RequiEscDto> RequiEsc { get; set; }
        public ICollection<RequiAptDto> RequiApt { get; set; }
        public ICollection<RequiHorDto> RequiHor { get; set; }
        public ICollection<RequiActDto> RequiAct { get; set; }
        public ICollection<RequiObsDto> RequiObs { get; set; }
        public ICollection<RequiPsDDto> RequiPsD { get; set; }
        public ICollection<RequiPsCDto> RequiPsC { get; set; }
        public ICollection<RequiBenDto> RequiBen { get; set; }
        public ICollection<RequiDocDto> RequiDoc { get; set; }
        public ICollection<RequiProDto> RequiPro { get; set; }
        public ICollection<RequiPreDto> RequiPre { get; set; }
        public ICollection<RequiCmADto> RequiCmA { get; set; }
        public ICollection<RequiCmCDto> RequiCmD { get; set; }
        public ICollection<RequiCmGDto> RequiCmG { get; set; }
    }
}