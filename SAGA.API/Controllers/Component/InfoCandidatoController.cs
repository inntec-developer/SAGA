﻿using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using SAGA.API.Utilerias;

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
                //var mocos = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(Id)).Select(pp => pp.Nombre).FirstOrDefault();
                var infoCanditato = db.PerfilCandidato.Where(p => p.CandidatoId.Equals(Id)).Select(p => new
                {
                    Id = p.CandidatoId,
                    Nombre = p.Candidato.Nombre + " " + p.Candidato.ApellidoPaterno + " " + p.Candidato.ApellidoMaterno,
                    //Nombre = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(Id)).Select(pp => pp.Nombre).FirstOrDefault() == null ? p.Candidato.Nombre + " " + p.Candidato.ApellidoPaterno + " " + p.Candidato.ApellidoMaterno : db.CandidatosInfo.Where(x => x.CandidatoId.Equals(Id)).Select(pp => pp.Nombre + " " + pp.ApellidoPaterno + " " + pp.ApellidoMaterno).FirstOrDefault(),
                    Foto = p.Candidato.ImgProfileUrl,
                    //Genero = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(pp => pp.Genero.genero).FirstOrDefault(),
                    Genero = p.Candidato.Genero.genero,
                    //FechaNacimiento = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(pp => pp.FechaNacimiento).FirstOrDefault(),
                    FechaNacimiento = p.Candidato.FechaNacimiento,
                    Candidato = new { p.Candidato.FechaNacimiento,
                        p.Candidato.estadoNacimiento.estado,
                        p.Candidato.paisNacimiento.pais,
                        p.Candidato.esDiscapacitado,
                        p.Candidato.TipoDiscapacidad.tipoDiscapacidad,
                        p.Candidato.OtraDiscapacidad,
                        p.Candidato.tieneLicenciaConducir,
                        p.Candidato.TipoLicencia.tipoLicencia,
                        p.Candidato.TipoLicencia.Descripcion,
                        p.Candidato.tieneVehiculoPropio,
                        p.Candidato.puedeViajar,
                        p.Candidato.puedeRehubicarse,
                        p.Candidato.CURP,
                        p.Candidato.RFC,
                        p.Candidato.NSS,
                    },
                    email = p.Candidato.emails.FirstOrDefault().email,
                    AboutMe = p.AboutMe,
                    Cursos = p.Cursos,
                    Conocimientos = p.Conocimientos,
                    Idiomas = p.Idiomas,
                    Formaciones = p.Formaciones,
                    Experiencias = p.Experiencias,
                    Certificaciones = p.Certificaciones,
                    Direccion = p.Candidato.direcciones.Select(dd => new {
                        dd.Municipio.municipio,
                        dd.Estado.estado,
                       dd.Pais.pais,
                        dd.Colonia.TipoColonia,
                        dd.Colonia.colonia,
                        dd.Calle,
                        dd.NumeroExterior,
                        dd.NumeroInterior,
                        dd.CodigoPostal
                        //p.Candidato.direcciones.FirstOrDefault().Municipio.municipio,
                        //p.Candidato.direcciones.FirstOrDefault().Estado.estado,
                        //p.Candidato.direcciones.FirstOrDefault().Pais.pais,
                        //p.Candidato.direcciones.FirstOrDefault().Colonia.TipoColonia,
                        //p.Candidato.direcciones.FirstOrDefault().Colonia.colonia,
                        //p.Candidato.direcciones.FirstOrDefault().Calle,
                        //p.Candidato.direcciones.FirstOrDefault().NumeroExterior,
                        //p.Candidato.direcciones.FirstOrDefault().NumeroInterior,
                        //p.Candidato.direcciones.FirstOrDefault().CodigoPostal

                    }).FirstOrDefault(),

                        //Email = p.Candidato.emails.FirstOrDefault(),
                    Telefono = p.Candidato.telefonos.Select(tt => new
                    {
                        tt.TipoTelefono.Tipo,
                        tt.telefono
                    }),
                    Estatus = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(e => e.CandidatoId.Equals(p.CandidatoId)).Select(PC => new {
                        id = PC.Id,
                        EstatusId = PC.EstatusId,
                        descripcion = PC.Estatus.Descripcion,
                        requisicionId = PC.RequisicionId,
                        reclutador = db.Usuarios.Where(x => x.Id.Equals(PC.ReclutadorId)).Select(RR => RR.Nombre + " " + RR.ApellidoPaterno + " " + RR.ApellidoMaterno).FirstOrDefault(),
                        reclutadorId = PC.ReclutadorId
                    }).FirstOrDefault(),
                    RedSocial = db.RedesSociales.Where(r => r.EntidadId.Equals(p.CandidatoId)).Select(r => r.redSocial).ToList(),
                    propietarioId = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(p.CandidatoId) && x.EstatusId.Equals(24)).Count() > 0 ? db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(p.CandidatoId)).OrderByDescending(o => o.Fch_Modificacion).Select(id => id.Requisicion.PropietarioId).FirstOrDefault() : new Guid("00000000-0000-0000-0000-000000000000"),
                    URLCv = db.MiCVUpload.Where(x => x.CandidatoId.Equals(p.CandidatoId)).Count() > 0 ? db.MiCVUpload.OrderByDescending(x => x.Id).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(x => x.UrlCV).FirstOrDefault() : "",
                    Edad =0
            }).FirstOrDefault();


                //var infoCanditato = db.CandidatosInfo.Where(p => p.EntidadId.Equals(Id)).Select(p => new InfoCandidato
                //{
                //    Id = p.EntidadId,
                //    Nombre = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                //    Foto = db.PerfilCandidato.Where(x => x.CandidatoId.Equals(p.EntidadId)).Select(pp => pp.Candidato.ImgProfileUrl).FirstOrDefault(),
                //    Genero = p.Genero.genero,
                //    FechaNacimiento = p.FechaNacimiento,
                //    Candidato = db.Candidatos.Where(x => x.Id.Equals(p.EntidadId)).FirstOrDefault(),
                //    AboutMe = db.PerfilCandidato.Where(x => x.Id.Equals(p.EntidadId)).Select(pp => pp.AboutMe).FirstOrDefault(),
                //    Cursos = db.PerfilCandidato.Where(x => x.Id.Equals(p.EntidadId)).Select(pp => pp.Cursos).FirstOrDefault(),
                //    Conocimientos = db.PerfilCandidato.Where(x => x.Id.Equals(p.EntidadId)).Select(pp => pp.Conocimientos).FirstOrDefault(),
                //    Idiomas = db.PerfilCandidato.Where(x => x.Id.Equals(p.EntidadId)).Select(pp => pp.Idiomas).FirstOrDefault(),
                //    Formaciones = p.Formaciones,
                //    Experiencias = p.Experiencias,
                //    Certificaciones = p.Certificaciones,
                //    Direccion = p.Candidato.direcciones.FirstOrDefault(),
                //    Email = p.Candidato.emails.FirstOrDefault(),
                //    Telefono = p.Candidato.telefonos.ToList(),
                //    Estatus = db.ProcesoCandidatos.Where(e => e.CandidatoId.Equals(p.CandidatoId)).OrderByDescending(x => x.Fch_Modificacion).FirstOrDefault(),
                //    RedSocial = db.RedesSociales.Where(r => r.EntidadId.Equals(p.CandidatoId)).Select(r => r.redSocial).ToList(),

                //}).FirstOrDefault();

                //infoCanditato.Edad = DateTime.Today.AddTicks(-Convert.ToDateTime(Convert.ToDateTime(infoCanditato.FechaNacimiento)).Ticks).Year - 1;
                return Ok(infoCanditato);
            }
            catch (Exception ex)
            {
                APISAGALog obj = new APISAGALog();
                obj.WriteError(ex.Message);
                obj.WriteError(ex.InnerException.Message);
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
                    .Where(e => e.EstatusId.Equals(6)
                    || e.EstatusId.Equals(7)
                    || e.EstatusId.Equals(29)
                    || e.EstatusId.Equals(30)
                    || e.EstatusId.Equals(31)
                    || e.EstatusId.Equals(32)
                    || e.EstatusId.Equals(33)
                    || e.EstatusId.Equals(38)
                    || e.EstatusId.Equals(39))
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
                        Confidencial = e.Confidencial,
                        contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(e.Id) && x.EstatusId == 24).Count()
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
                    foreach (Guid gr in gp)
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
            catch (Exception ex)
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
                ProcesoDto datos = new ProcesoDto();
                PostulateVacantController obj = new PostulateVacantController();

                var horario = db.HorariosRequis.Where(x => x.RequisicionId.Equals(proceso.RequisicionId)).Select(h => h.Id).FirstOrDefault();
                var candidato = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(proceso.CandidatoId) && x.RequisicionId.Equals(proceso.RequisicionId)).FirstOrDefault();
                var estatus = 12;
                if (candidato == null)
                {
                    proceso.HorarioId = horario;
                    proceso.Fch_Modificacion = DateTime.Now;
                    proceso.DepartamentoId = new Guid("d89bec78-ed5b-4ac5-8f82-24565ff394e5");
                    proceso.TipoMediosId = 2;

                    db.ProcesoCandidatos.Add(proceso);
                    db.SaveChanges();

                    var requi = db.EstatusRequisiciones.Where(x => x.RequisicionId.Equals(proceso.RequisicionId) && x.EstatusId.Equals(29)).Count();
                    if (requi == 0)
                    {
                        datos.requisicionId = proceso.RequisicionId;
                        datos.estatusId = 29;
                        obj.UpdateStatusVacante(datos);

                    }

                    return Ok(HttpStatusCode.OK);
                }
                else if (candidato.EstatusId == 27 || candidato.EstatusId == 40)
                {
                    if(candidato.EstatusId == 40)
                    {
                        estatus = db.InformeRequisiciones.OrderByDescending(f => f.fch_Modificacion).Where(x => x.CandidatoId.Equals(proceso.CandidatoId) && !x.EstatusId.Equals(40)).Select(e => e.EstatusId).FirstOrDefault();
                    }
                    db.Entry(candidato).State = EntityState.Modified;
                    candidato.Reclutador = proceso.Reclutador;
                    candidato.ReclutadorId = proceso.ReclutadorId;
                    candidato.RequisicionId = proceso.RequisicionId;
                    candidato.Folio = proceso.Folio;
                    candidato.EstatusId = estatus;
                    candidato.Fch_Modificacion = DateTime.Now;
                    candidato.HorarioId = horario;

                    db.SaveChanges();

                    var requi = db.EstatusRequisiciones.Where(x => x.RequisicionId.Equals(proceso.RequisicionId) && x.EstatusId.Equals(29)).Count();
                    if (requi == 0)
                    {
                        datos.requisicionId = proceso.RequisicionId;
                        datos.estatusId = 29;
                        obj.UpdateStatusVacante(datos);

                    }

                    return Ok(HttpStatusCode.OK);
                }
                else
                {
                    return Ok(HttpStatusCode.NotModified);
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("liberarCandidato")]
        [HttpPost]
        public IHttpActionResult LiberarCandidato(LiberarCandidatoDto lc)
        {
            try
            {
                Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");

                if (lc.ProcesoCandidatoId == auxID)
                {
                    var pc = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.RequisicionId.Equals(lc.RequisicionId) && x.CandidatoId.Equals(lc.CandidatoId)).Select(PC => PC.Id).FirstOrDefault();

                    lc.ProcesoCandidatoId = pc;
                }

                ProcesoCandidato liberar = db.ProcesoCandidatos.Find(lc.ProcesoCandidatoId);
                db.Entry(liberar).State = EntityState.Modified;
                liberar.EstatusId = 27;

                CandidatoLiberado cl = new CandidatoLiberado();
                cl.RequisicionId = lc.RequisicionId;
                cl.CandidatoId = lc.CandidatoId;
                cl.ReclutadorId = lc.ReclutadorId;
                cl.MotivoId = lc.MotivoId;
                cl.Comentario = lc.Comentario;

                db.CandidatosLiberados.Add(cl);

                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
