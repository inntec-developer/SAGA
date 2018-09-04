using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Component
{
    [RoutePrefix("api/reclutamiento/SeguimientoVacante")]
    public class InfoCandidatoController : ApiController
    {
        private SAGADBContext db;

        public InfoCandidatoController()
        {
            db = new SAGADBContext();
        }

        [Route("getInfoCandidato")]
        [HttpGet]
        public IHttpActionResult _GetInfoCandidato(Guid Id)
        {
            try
            {
                string fecha = "07/12/1990";
                var edad = DateTime.Today.AddTicks(-Convert.ToDateTime(Convert.ToDateTime(fecha)).Ticks).Year - 1;

                var infoCanditato = db.PerfilCandidato.Where(p => p.CandidatoId.Equals(Id)).Select(p => new InfoCandidato
                {
                    Id = p.CandidatoId,
                    Nombre = p.Candidato.Nombre + " " + p.Candidato.ApellidoPaterno + " " + p.Candidato.ApellidoMaterno,
                    Foto = p.Candidato.ImgProfileUrl,
                    Genero = p.Candidato.Genero.genero,
                    FechaNacimiento = p.Candidato.FechaNacimiento,
                    Candidato = p.Candidato,
                    AboutMe = p.AboutMe,
                    Cursos = p.Cursos,
                    Conocimientos = p.Conocimientos,
                    Idiomas = p.Idiomas,
                    Formaciones = p.Formaciones,
                    Experiencias = p.Experiencias,
                    Certificaciones = p.Certificaciones,
                    Direccion = p.Candidato.direcciones.FirstOrDefault(),
                    Email = p.Candidato.emails.FirstOrDefault(),
                    Telefono = p.Candidato.telefonos.ToList(),
                    Estatus = db.ProcesoCandidatos.Where(e => e.CandidatoId.Equals(p.CandidatoId)).FirstOrDefault(),
                    RedSocial = db.RedesSociales.Where(r => r.EntidadId.Equals(p.CandidatoId)).Select(r => r.redSocial).ToList()

                }).FirstOrDefault();

                infoCanditato.Edad = DateTime.Today.AddTicks(-Convert.ToDateTime(Convert.ToDateTime(infoCanditato.FechaNacimiento)).Ticks).Year - 1;
                return Ok(infoCanditato);
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
                return Ok(StatusCode(HttpStatusCode.NotFound));
            }
        }

        [Route("getMisVacantes")]
        [HttpGet]
        public IHttpActionResult _GetMisVacantes(Guid Id)
        {
            try
            {
                List<Guid> grp = new List<Guid>();

                var Grupos = db.GruposUsuarios
                    .Where(g => g.EntidadId.Equals(Id) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();


                foreach (var grps in Grupos)
                {
                    grp = GetGrupo(grps, grp);
                }


                grp.Add(Id);

                var asig = db.AsignacionRequis
                    .OrderByDescending(e => e.Id)
                    .Where(a => grp.Distinct().Contains(a.GrpUsrId))
                    .Select(a => a.RequisicionId)
                    .Distinct()
                    .ToList();

                var vacantes = db.Requisiciones
                    .Where(e => asig.Contains(e.Id) && e.Activo.Equals(true))
                    .Select(e => new
                    {
                        Id = e.Id,
                        Folio = e.Folio,
                        VBtra = e.VBtra,
                        Cliente = e.Cliente.Nombrecomercial,
                        TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento,
                        tipoReclutamientoId = e.TipoReclutamientoId,
                        Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        fch_Cumplimiento = e.fch_Cumplimiento,
                        Estatus = e.Estatus.Descripcion,
                        EstatusId = e.EstatusId,
                        Confidencial = e.Confidencial
                    }).ToList().OrderByDescending(x => x.Folio.ToString());
                return Ok(vacantes);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        public List<Guid> GetGrupo(Guid grupo, List<Guid> listaIds)
        {
            if (!listaIds.Contains(grupo))
            {
                listaIds.Add(grupo);
                var listadoNuevo = db.GruposUsuarios
                    .Where(g => g.EntidadId.Equals(grupo) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();
                foreach (Guid g in listadoNuevo)
                {
                    var gp = db.GruposUsuarios
                        .Where(x => x.EntidadId.Equals(g))
                        .Select(x => x.GrupoId)
                        .ToList();
                    foreach(Guid gr in gp)
                    {
                        GetGrupo(gr, listaIds);
                    }
                }
            }
            return listaIds;
        }

        [Route("getPostulaciones")]
        [HttpGet]
        public IHttpActionResult GetPostulaciones(Guid Id)
        {
            try
            {
                var postulaciones = db.Postulaciones
                    .Where(p => p.CandidatoId.Equals(Id) && p.Requisicion.Activo.Equals(true))
                    .Select(p => new
                    {
                        Folio = p.Requisicion.Folio,
                        vBtra = p.Requisicion.VBtra,
                        Estatus = p.Status.Status,
                        EstatusId = p.StatusId
                    }).OrderByDescending(p => p.Folio).ToList();
                return Ok(postulaciones);
            }
            catch(Exception ex)
            {
                string msd = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("apartarCandidato")]
        [HttpPost]
        public IHttpActionResult ApartarCandidato(ProcesoCandidato proceso)
        {
            try
            {
                if(db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(proceso.CandidatoId)).Count() == 0)
                {
                    db.ProcesoCandidatos.Add(proceso);
                    db.SaveChanges();
                    return Ok(HttpStatusCode.OK);
                }
                else
                {
                    return Ok(HttpStatusCode.NotModified);
                }
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [Route("desapartarCandidato")]
        [HttpPost]
        public IHttpActionResult LiberarCandidato(int Id)
        {
            try
            {
                ProcesoCandidato liberar = db.ProcesoCandidatos.Find(Id);
                db.ProcesoCandidatos.Remove(liberar);
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
