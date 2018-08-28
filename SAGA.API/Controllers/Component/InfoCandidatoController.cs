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
        List<GrupoUsuarios> Grupos;
        List<Guid> Requis;

        public InfoCandidatoController()
        {
            db = new SAGADBContext();
            Grupos = new List<GrupoUsuarios>();
            Requis = new List<Guid>();
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
                Grupos = db.GruposUsuarios.ToList();
                //var Usario = db.Usuarios.Where(u => u.Id.Equals(id)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                // var Grupos = db.GruposUsuarios // Obtenemos los Ids de las celulas o grupos a los que pertenece.
                //.Where(g => g.EntidadId.Equals(Id))
                //.Select(g => g.GrupoId)
                //.ToList();


                // var GruposEnGrupos = db.GruposUsuarios
                //     .Where(x => Grupos.Contains(x.EntidadId))
                //     .Select(g => g.GrupoId)
                //     .ToList();


                // var RequisicionesGrupos = db.AsignacionRequis
                //     .Where(r => Grupos.Contains(r.GrpUsrId))
                //     .Select(r => r.RequisicionId)
                //     .ToList();

                // var RequiGrupoEnGrupo = db.AsignacionRequis
                //     .Where(r => GruposEnGrupos.Contains(r.GrpUsrId))
                //     .Select(r => r.RequisicionId)
                //     .ToList();

                // var RequisicionesInd = db.AsignacionRequis
                //     .Where(r => r.GrpUsrId.Equals(Id))
                //     .Select(r => r.RequisicionId)
                //     .ToList();




                var grp = db.GruposUsuarios.Where(x => x.EntidadId.Equals(Id)).ToList();

                

                var requisGrup = checkGrupsReclutador(grp);
                var requiUser = db.AsignacionRequis.Where(r => r.GrpUsrId.Equals(Id)).Select(r => r.RequisicionId).ToList();

                var requis = requisGrup.Union(requiUser);

                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => requis.Contains(e.Id))
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
                    }).Distinct().ToList();
                return Ok(vacantes);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }


        /* Mandamos el Usario que esta solicitando las requisiciones, y los grupos a los cueles pertenece*/
        public IEnumerable<Guid> checkGrupsReclutador(List<GrupoUsuarios> grp)
        {
            foreach (GrupoUsuarios gp in grp)
            {
                var requis = db.AsignacionRequis.Where(r => r.GrpUsrId.Equals(gp.GrupoId)).Select( r=> r.RequisicionId).ToList();
                Requis.AddRange(requis);
                foreach(GrupoUsuarios gps in Grupos)
                {
                    if(gps.EntidadId == gp.GrupoId)
                    {
                        var rama = Grupos.Where(x => x.GrupoId.Equals(gps.EntidadId)).ToList();
                        checkGrupsReclutador(rama);
                    }
                }
            }
            return Requis.Distinct();
        }
    }
}
