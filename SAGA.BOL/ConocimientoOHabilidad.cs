using System;

namespace SAGA.BOL
{
    public class ConocimientoOHabilidad
    {
        public Guid Id { get; set; }
        public string Conocimiento { get; set; }
        public string Herramienta { get; set; }

        public Guid? InstitucionEducativaId { get; set; }
        public virtual InstitucionEducativa InstitucionEducativa { get; set; }

        public byte? NivelId { get; set; }
        public virtual Nivel nivel { get; set; }

        public Guid PerfilCandidatoId { get; set; }
        public PerfilCandidato PerfilCandidato { get; set; }

        public ConocimientoOHabilidad()
        {
            this.Id = Guid.NewGuid();
        }
    }
}