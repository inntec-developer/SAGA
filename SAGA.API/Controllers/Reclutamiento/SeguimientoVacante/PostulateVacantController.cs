using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/reclutamiento/SeguimientoVacante")]
    public class PostulateVacantController : ApiController
    {
        public SAGADBContext db;

        public PostulateVacantController()
        {
            db = new SAGADBContext();
        }
            
        [HttpGet]
        [Route("getPostulate")]
        public IHttpActionResult GetPostulate(Guid VacanteId)
        {
            var postulate = db.Postulaciones.Where(x => x.RequisicionId.Equals(VacanteId)).Select(x => x.CandidatoId).ToList();
            var candidatos = db.PerfilCandidato.Where(x => postulate.Contains(x.CandidatoId)).Select(x => new {
                nombre = x.Candidato.Nombre + " " + x.Candidato.ApellidoPaterno + " " + x.Candidato.ApellidoMaterno,
                AreaExp = x.AboutMe.Select(ae => ae.AreaExperiencia.areaExperiencia).FirstOrDefault(),
                AreaInt = x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault(),
                localidad = x.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault() + " / " + x.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault(),
                sueldoMinimo = x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault(),
                edad = x.Candidato.FechaNacimiento,
                rfc = x.Candidato.RFC,
                curp = x.Candidato.CURP
            }).ToList();



            return Ok(candidatos);
        }

    }
}
