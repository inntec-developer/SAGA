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
        public ICollection<DamfoEscDto> DamfoEsc { get; set; }
        public ICollection<DamfoAptDto> DamfoApt { get; set; }
        public ICollection<DamfoHorDto> DamfoHor { get; set; }
        public ICollection<DamfoActDto> DamfoAct { get; set; }
        public ICollection<DamfoObsDto> DamfoObs { get; set; }
        public ICollection<DamfoPsDDto> DamfoPsD { get; set; }
        public ICollection<DamfoPsCDto> DamfoPsC { get; set; }
        public ICollection<DamfoBenDto> DamfoBen { get; set; }
        public ICollection<DamfoDocDto> DamfoDoc { get; set; }
        public ICollection<DamfoProDto> DamfoPro { get; set; }
        public ICollection<DamfoPreDto> DamfoPre { get; set; }
        public ICollection<DamfoCmADto> DamfoCmA { get; set; }
        public ICollection<DamfoCmCDto> DamfoCmD { get; set; }
        public ICollection<DamfoCmGDto> DamfoCmG { get; set; }
    }
}   