using System;

namespace SAGA.BOL
{
    public class AboutMe
    {
        public Guid Id { get; set; }
        public string AcercaDeMi { get; set; }
        public string PuestoDeseado { get; set; }
        public decimal SalarioAceptable { get; set; }
        public decimal SalarioDeseado { get; set; }
        public int AreaExperienciaId { get; set; }
        public virtual AreaInteres AreaExperiencia { get; set; }
        public int? AreaInteresId { get; set; }
        public virtual AreaInteres AreaInteres { get; set; }
        public int? PerfilExperienciaId { get; set; }
        public virtual PerfilExperiencia PerfilExperiencia { get; set; }

        public string SitioWeb { get; set; }

        public Guid PerfilCandidatoId { get; set; }
        public virtual PerfilCandidato PerfilCandidato { get; set; }

        public AboutMe()
        {
            this.Id = Guid.NewGuid();
        }
    }
}