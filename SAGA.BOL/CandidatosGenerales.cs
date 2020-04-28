using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class CandidatosGenerales
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CandidatoInfoId { get; set; }
        public virtual CandidatosInfo CandidatoInfo { get; set; }

        public int PaisId { get; set; }
        public virtual Pais Pais { get; set; }

        public int EstadoId { get; set; }
        public virtual Estado Estado { get; set; }

        public int MunicipioId { get; set; }
        public virtual Municipio Municipio { get; set; }

        public int ColoniaId { get; set; }
        public virtual Colonia Colonia { get; set; }

        public string CodigoPostal { get; set; }
        public string Calle { get; set; }
        public string NumeroInterior { get; set; }
        public string NumeroExterior { get; set; }

        public byte EstadoCivilId { get; set; }
        public virtual EstadoCivil EstadoCivil { get; set; }

        public int GrupoSanguineoId { get; set; }
        public virtual GrupoSanguineo GrupoSanguineo { get; set; }

        public string ImgUrl { get; set; }
        public string email { get; set; }

    }
}
