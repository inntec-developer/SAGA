using SAGA.API.Dtos.Reclutamiento.Seguimientovacantes;
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
                CandidatoId = x.CandidatoId,
                nombre = x.Candidato.Nombre + " " + x.Candidato.ApellidoPaterno + " " + x.Candidato.ApellidoMaterno,
                AreaExp = x.AboutMe.Select(ae => ae.AreaExperiencia.areaExperiencia).FirstOrDefault() != null ? x.AboutMe.Select(ae => ae.AreaExperiencia.areaExperiencia).FirstOrDefault() : "" ,
                AreaInt = x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() : "" ,
                localidad = x.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault() + " / " + x.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault(),
                sueldoMinimo = x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault().ToString() != null ? x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault() : 0 ,
                edad = x.Candidato.FechaNacimiento,
                rfc = x.Candidato.RFC != null ? x.Candidato.RFC : "",
                curp = x.Candidato.CURP
            }).ToList();
            return Ok(candidatos);
        }

        [HttpGet]
        [Route("getProceso")]
        public IHttpActionResult GetProceso(Guid VacanteId, Guid ReclutadorId)
        {
            var postulate = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(VacanteId) & x.ReclutadorId.Equals(ReclutadorId)).Select(c => new
            {
                candidatoId = c.CandidatoId,
                estatus = c.Estatus.Descripcion,
                perfil = db.PerfilCandidato.Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(x => new
                {
                    nombre = x.Candidato.Nombre + " " + x.Candidato.ApellidoPaterno + " " + x.Candidato.ApellidoMaterno,
                    AreaExp = x.AboutMe.Select(ae => ae.AreaExperiencia.areaExperiencia).FirstOrDefault() != null ? x.AboutMe.Select(ae => ae.AreaExperiencia.areaExperiencia).FirstOrDefault() : "",
                    AreaInt = x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() : "",
                    localidad = x.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault() + " / " + x.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault(),
                    sueldoMinimo = x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault().ToString() != null ? x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault() : 0,
                    edad = x.Candidato.FechaNacimiento,
                    rfc = x.Candidato.RFC != null ? x.Candidato.RFC : "",
                    curp = x.Candidato.CURP != null ? x.Candidato.CURP : ""
                })
            });

            return Ok(postulate);

        }

        [HttpPost]
        [Route("updateStatus")]
        public IHttpActionResult UpdateStatus(List<ProcesoDto> datos)
        {
            try
            {
                foreach (var d in datos)
                {
                    var id = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(d.candidatoId)).Select(x => x.Id).FirstOrDefault();
                    
                    var c = db.ProcesoCandidatos.Find(id);
                    db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                    c.EstatusId = d.estatusId;

                    db.SaveChanges();
                }

                return Ok(HttpStatusCode.Created);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }


    }
}
