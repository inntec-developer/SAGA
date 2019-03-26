﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.Data.Entity;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Examenes")]
    public class PsicometricosController : ApiController
    {
        private SAGADBContext db;
        public PsicometricosController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getRequisiciones")]
        public IHttpActionResult GetRequisiciones()
        {
            try
            {

                var requisiciones = db.PsicometriasDamsaRequis.Where(x => x.PsicometriaId > 0).Select(R => R.RequisicionId).ToList().Distinct();

                var psico = db.Requisiciones.Where(x => requisiciones.Contains(x.Id)).Select(R => new
                {
                    requisicionId = R.Id,
                    folio = R.Folio,
                    vBtra = R.VBtra,
                    psicometricos = db.PsicometriasDamsaRequis.Where(x => x.RequisicionId.Equals(R.Id)).Select(C => new { nombre = C.Psicometria.tipoPsicometria }).ToList(),
                    claves = db.RequiClaves.Where(x => x.RequisicionId.Equals(R.Id)).Count()
                }).ToList();

                return Ok(psico);
            }
            catch(Exception)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpPost]
        [Route("insertClaves")]
        public IHttpActionResult InsertClaves(List<RequiClaves> claves)
        {
            try
            {
                RequiClaves RC = new RequiClaves();

                foreach(var item in claves)
                {
                    RC.Clave = item.Clave;
                    RC.RequisicionId = item.RequisicionId;
                    RC.fch_Creacion = DateTime.Now;
                    RC.Activo = 0;
                    RC.UsuarioId = item.UsuarioId;

                    db.RequiClaves.Add(RC);
                    db.SaveChanges();

                    RC = new RequiClaves();
                }

                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpPost]
        [Route("agregarResultado")]
        public IHttpActionResult AgregarResultado(PsicometriaCandidato resultado)
        {
            try
            {
                var ind = db.PsicometriaCandidato.Where(x => x.RequiClaveId.Equals(resultado.RequiClaveId)).Select(ID => ID.Id).FirstOrDefault();
                var psicometria = db.PsicometriaCandidato.Find(ind);

                db.Entry(psicometria).State = EntityState.Modified;
                psicometria.Resultado = resultado.Resultado;
                psicometria.fch_Resultado = DateTime.Now;
                psicometria.UsuarioId = resultado.UsuarioId;

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpPost]
        [Route("asignarClaveCandidato")]
        public IHttpActionResult AsignarClave(PsicometriaCandidato clave)
        {
            try
            {
                clave.fch_Creacion = DateTime.Now;
                clave.fch_Resultado = DateTime.Now;

                db.PsicometriaCandidato.Add(clave);

                db.SaveChanges();

                var ind = db.RequiClaves.Find(clave.RequiClaveId);
                db.Entry(ind).Property(x => x.Activo).IsModified = true;

                ind.Activo = 1;

                db.SaveChanges();

                var cc = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(clave.CandidatoId) && x.RequisicionId.Equals(clave.RequisicionId)).Select(c => c.Id).FirstOrDefault();
                var C = db.ProcesoCandidatos.Find(cc);

                db.Entry(C).Property(x => x.EstatusId).IsModified = true;
                db.Entry(C).Property(x => x.Fch_Modificacion).IsModified = true;


                if (db.InformeRequisiciones.Where(x => x.CandidatoId.Equals(clave.CandidatoId) && x.RequisicionId.Equals(clave.RequisicionId) && x.EstatusId.Equals(18)).Count() > 0)
                {
                    C.EstatusId = 14;
                    C.Fch_Modificacion = DateTime.Now;

                    db.SaveChanges();
                }
                else
                {
                    C.EstatusId = 18;
                    C.Fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                    C.EstatusId = 14;
                    C.Fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                }


                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getClaves")]
        public IHttpActionResult GetClaves(Guid requisicionId)
        {
            var claves = db.Requisiciones.Where(x => x.Id.Equals(requisicionId)).Select(C => new
            {
                vBtra = C.VBtra,
                folio = C.Folio,
                claves = db.RequiClaves.Where(x => x.RequisicionId.Equals(requisicionId)).Select(CC => new { activo = CC.Activo, clave = CC.Clave, id = CC.Id })
            }).ToList();
            return Ok(claves);
        }
        [HttpGet]
        [Route("getClaveCandidatos")]
        public IHttpActionResult GetClaveCandidatos()
        {
            var candidatos = db.PsicometriaCandidato.Select(C => new
            {
                requisicionId = C.RequisicionId,
                candidatoId = C.CandidatoId,
                requiClaveId = C.RequiClaveId,
                nombre = C.Candidato.Nombre,
                apellidoPaterno = C.Candidato.ApellidoPaterno,
                apellidoMaterno = C.Candidato.ApellidoMaterno,
                clave = C.RequiClave.Clave,
                psicometricos = db.PsicometriasDamsaRequis.Where(x => x.RequisicionId.Equals(C.RequisicionId)).Select(P => new { nombre = P.Psicometria.tipoPsicometria }).ToList(),
                resultado = C.Resultado.ToUpper()
            }).ToList();

            return Ok(candidatos);
        }
        [HttpGet]
        [Route("getClavesCandidatos")]
        public IHttpActionResult GetClavesCandidatos()
        {
            var candidatos = db.PsicometriaCandidato.Select(c => c.CandidatoId).Distinct().ToList();

            var resultado = db.Candidatos.Where(x => candidatos.Contains(x.Id)).Select(C => new
            {
                candidatoId = C.Id,
                nombre = C.Nombre.ToString() + " " + C.ApellidoPaterno.ToString() + " " + C.ApellidoMaterno.ToString(),
                claves = db.PsicometriaCandidato.Where(x => x.CandidatoId.Equals(C.Id)).Select(CC => new
                {
                    requisicionId = CC.RequisicionId,
                    folio = CC.Requisicion.Folio,
                    vBtra = CC.Requisicion.VBtra,
                    requiClaveId = CC.RequiClaveId,
                    clave = CC.RequiClave.Clave,
                    psicometricos = db.PsicometriasDamsaRequis.Where(x => x.RequisicionId.Equals(CC.RequisicionId)).Select(P => new { nombre = P.Psicometria.tipoPsicometria }).ToList(),
                    resultado = CC.Resultado.ToUpper(),
                    fecha = CC.fch_Creacion
                }).ToList()

            }).ToList();
            

            return Ok(resultado);
        }


    }

   
}
