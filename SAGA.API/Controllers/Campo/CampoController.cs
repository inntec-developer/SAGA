using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Campo")]
    public class CampoController : ApiController
    {
        private SAGADBContext db;
        public CampoController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getReclutadores")]
        public IHttpActionResult GetReclutadores()
        {
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                var reclutadores = db.AsignacionRequis.Where(x => x.Requisicion.Activo && !estatusId.Contains(x.Requisicion.EstatusId) && !x.Requisicion.Confidencial).GroupBy(g => g.GrpUsrId)
                    .Select(R => new
                    {
                        oficinaId = db.Usuarios.Where(x => x.Id.Equals(R.Key)).Select(ofi => ofi.SucursalId).FirstOrDefault(),
                        oficina = db.Usuarios.Where(x => x.Id.Equals(R.Key)).Select(ofi => db.OficinasReclutamiento.Where(x => x.Id.Equals(ofi.SucursalId)).Select(n => n.Nombre).FirstOrDefault()).FirstOrDefault(),
                        nombre = db.Usuarios.Where(x => x.Id.Equals(R.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        reclutadorId = R.Key,
                        requis = R.Select(r => r.RequisicionId).Count()
                    });
                var mocos = reclutadores.GroupBy(x => x.oficinaId).Select(R => new
                {
                    oficinaId = R.Key,
                    oficina = R.Select(n => n.oficina).FirstOrDefault(),
                    reclutadores = R.Select(r => new
                    {
                        reclutadorId = r.reclutadorId,
                        nombre = r.nombre,
                        requis = r.requis
                    })

                });
                return Ok(mocos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getRequisReclutadores")]
        public IHttpActionResult GetRequiReclutadores(Guid reclutadorId)
        {
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                var reclutadores = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(reclutadorId) && x.Requisicion.Activo && !estatusId.Contains(x.Requisicion.EstatusId) && !x.Requisicion.Confidencial)
                    .Select(r => new
                    {

                        r.Requisicion.Id,
                        r.Requisicion.Folio,
                        r.Requisicion.VBtra,
                        cliente = r.Requisicion.Cliente.Nombrecomercial,
                        r.Requisicion.fch_Cumplimiento,
                        Vacantes = r.Requisicion.horariosRequi.Count() > 0 ? r.Requisicion.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(r.Requisicion.Id) && p.EstatusId.Equals(24)).Count(),
                    });
                return Ok(reclutadores);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("getUnidadesNegocios")]
        public IHttpActionResult GetUnidadesNegocio()
        {
            try
            {
                var unidades = db.UnidadesNegocios.ToArray();
                return Ok(unidades);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getReclutadoresByUnidades")]
        public IHttpActionResult GetReclutadoresByUnidades(int id)
        {
            try
            {
                int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
                var unidades = db.OficinasReclutamiento.Where(x => x.UnidadNegocioId.Equals(id)).Select(R => new
                {
                    oficinaId = R.Id,
                    oficina = R.Nombre,
                    reclutadores = db.Usuarios.Where(x => x.SucursalId.Equals(R.Id) & x.TipoUsuarioId.Equals(11)).Select(n => new
                    {
                        reclutadorId = n.Id,
                        nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                        requis = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(n.Id) && x.Requisicion.Activo && !estatusId.Contains(x.Requisicion.EstatusId) && !x.Requisicion.Confidencial).Count()
                    })
                });
                return Ok(unidades);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getCandidatosProceso")]
        //[Authorize]
        public IHttpActionResult GetCandidatosProceso(Guid requisicionId)
        {
            try
            {
                var candidatos = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.RequisicionId.Equals(requisicionId) & x.EstatusId != 27 & x.EstatusId != 40).Select(c => new
                {
                    procesoId = c.Id,
                    candidatoId = c.CandidatoId,
                    horarioId = c.HorarioId,
                    horario = db.HorariosRequis.Where(x => x.Id.Equals(c.HorarioId)).Select(h => h.Nombre + " de " + h.deHora.Hour + " a " + h.aHora.Hour).FirstOrDefault(),
                    vacantes = db.HorariosRequis.Where(x => x.RequisicionId.Equals(c.RequisicionId)).Count() > 0 ? db.HorariosRequis.Where(x => x.RequisicionId.Equals(c.RequisicionId)).Sum(h => h.numeroVacantes) : 0,
                    propietarioId = c.Requisicion.PropietarioId,
                    estatusId = c.EstatusId,
                    apartados = db.Candidatos.Where(x => x.Id.Equals(c.CandidatoId) && c.EstatusId.Equals(12)).Select(cc => new {
                        curp = String.IsNullOrEmpty(cc.CURP) ? "Sin registro" : cc.CURP,
                        nombre = cc.Nombre + " " + cc.ApellidoPaterno + " " + cc.ApellidoMaterno,
                        edad = cc.FechaNacimiento,
                        localidad = cc.municipioNacimiento.municipio + " / " + cc.estadoNacimiento.estado,
                        genero = cc.GeneroId == 1 ? "Hombre" : "Mujer",
                        reclutador = db.Usuarios.Where(x => x.Id.Equals(c.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                        lada = db.Telefonos.Where(x => x.EntidadId.Equals(c.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                        telefono = db.Telefonos.Where(x => x.EntidadId.Equals(c.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                        email = db.Emails.Where(x => x.EntidadId.Equals(c.CandidatoId)).Select(e => e.email).FirstOrDefault()
                    }).FirstOrDefault(),
                    informacion = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(c.CandidatoId) && c.EstatusId.Equals(24)).Select(p => new
                    {
                        nombre = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                        nom = p.Nombre,
                        apellidoPaterno = p.ApellidoPaterno,
                        apellidoMaterno = p.ApellidoMaterno,
                        edad = p.FechaNacimiento,
                        rfc = String.IsNullOrEmpty(p.RFC) ? "Sin registro" : p.RFC,
                        curp = String.IsNullOrEmpty(p.CURP) ? "Sin registro" : p.CURP,
                        nss = String.IsNullOrEmpty(p.NSS) ? "Sin registro" : p.NSS,
                        paisNacimiento = p.PaisNacimientoId,
                        estadoNacimiento = p.EstadoNacimientoId,
                        municipioNacimiento = p.MunicipioNacimientoId,
                        localidad = p.municipioNacimiento.municipio + " / " + p.estadoNacimiento.estado,
                        genero = p.GeneroId == 1 ? "Hombre" : "Mujer",
                        reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                        reclutadorId = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Id).FirstOrDefault(),
                        lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                        telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                        email = db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault()
                    }).FirstOrDefault()
                });

                return Ok(candidatos);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpPost]
        [Route("updateContratadosCampo")]
        public IHttpActionResult UpdateContratadosCampo(CandidatosGralDto datos)
        {
            var aux = new Guid("00000000-0000-0000-0000-000000000000");
            CandidatosInfo obj = new CandidatosInfo();
            try
            {
                var cc = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(datos.Id)).Select(c => c.Id).FirstOrDefault();

                if (cc == aux)
                {
                    obj.CandidatoId = datos.Id;
                    obj.CURP = datos.Curp;
                    obj.RFC = String.IsNullOrEmpty(datos.Rfc) ? "SIN REGISTRO" : datos.Rfc;
                    obj.NSS = string.IsNullOrEmpty(datos.Nss) ? "SIN REGISTRO" : datos.Nss;
                    obj.FechaNacimiento = datos.FechaNac;
                    obj.Nombre = datos.Nombre;
                    obj.ApellidoPaterno = datos.ApellidoPaterno;
                    obj.ApellidoMaterno = datos.ApellidoMaterno;
                    obj.PaisNacimientoId = datos.PaisNacimientoId;
                    obj.EstadoNacimientoId = datos.EstadoNacimientoId;
                    obj.MunicipioNacimientoId = 0;
                    obj.GeneroId = datos.GeneroId;
                    obj.ReclutadorId = datos.reclutadorId;

                    obj.fch_Modificacion = DateTime.Now;
                    obj.fch_Modificacion.ToUniversalTime();

                    db.CandidatosInfo.Add(obj);
                    db.SaveChanges();

                }
                else
                {

                    var ccc = db.CandidatosInfo.Find(cc);

                    db.Entry(ccc).State = System.Data.Entity.EntityState.Modified;
                    ccc.Nombre = datos.Nombre;
                    ccc.ApellidoPaterno = datos.ApellidoPaterno;
                    ccc.ApellidoMaterno = datos.ApellidoMaterno;
                    ccc.FechaNacimiento = datos.FechaNac;
                    ccc.CURP = datos.Curp;
                    ccc.RFC = String.IsNullOrEmpty(datos.Rfc) ? "SIN REGISTRO" : datos.Rfc;
                    ccc.NSS = string.IsNullOrEmpty(datos.Nss) ? "SIN REGISTRO" : datos.Nss;
                    ccc.ReclutadorId = datos.reclutadorId;
                    ccc.fch_Modificacion = DateTime.Now;
                    ccc.fch_Modificacion.ToUniversalTime();

                    db.SaveChanges();

                }

                var cand = db.Candidatos.Where(x => x.Id.Equals(datos.Id)).Select(c => c.Id).FirstOrDefault();
                var candidato = db.Candidatos.Find(cand);
                db.Entry(candidato).State = System.Data.Entity.EntityState.Modified;

                candidato.CURP = datos.Curp;
                candidato.Nombre = datos.Nombre;
                candidato.ApellidoPaterno = datos.ApellidoPaterno;
                candidato.ApellidoMaterno = datos.ApellidoMaterno;

                candidato.PaisNacimientoId = 42;
                candidato.EstadoNacimientoId = datos.EstadoNacimientoId;
                candidato.MunicipioNacimientoId = 0;

                candidato.GeneroId = datos.GeneroId;
                candidato.TipoEntidadId = 2;
                candidato.FechaNacimiento = datos.FechaNac;

                if (datos.OpcionRegistro == 1)
                {
                    var ec = db.Emails.Where(x => x.EntidadId.Equals(datos.Id)).Select(c => c.Id).FirstOrDefault();
                    if (ec != aux)
                    {
                        var e = db.Emails.Find(ec);

                        db.Entry(e).State = System.Data.Entity.EntityState.Modified;
                        e.email = datos.Email.Select(x => x.email).FirstOrDefault();
                        e.fch_Modificacion = DateTime.Now;
                        e.UsuarioMod = datos.Email.Select(x => x.UsuarioMod).FirstOrDefault();
                    }
                }
                else
                {
                    var tc = db.Telefonos.Where(x => x.EntidadId.Equals(datos.Id)).Select(c => c.Id).FirstOrDefault();
                    if (tc != aux)
                    {
                        var t = db.Telefonos.Find(tc);

                        db.Entry(t).State = System.Data.Entity.EntityState.Modified;
                        t.ClaveLada = datos.Telefono.Select(x => x.ClaveLada).FirstOrDefault();
                        t.telefono = datos.Telefono.Select(x => x.telefono).FirstOrDefault();
                        t.fch_Modificacion = DateTime.Now;
                        t.UsuarioMod = datos.Telefono.Select(x => x.UsuarioMod).FirstOrDefault();
                    }
                    else
                    {
                        Telefono T = new Telefono();
                        T.EntidadId = datos.Id;
                        T.TipoTelefonoId = 1;
                        T.ClavePais = "52";
                        T.ClaveLada = datos.Telefono.Select(x => x.ClaveLada).FirstOrDefault();
                        T.telefono = datos.Telefono.Select(x => x.telefono).FirstOrDefault();
                        T.UsuarioAlta = datos.Telefono.Select(x => x.UsuarioMod).FirstOrDefault();

                        db.Telefonos.Add(T);
                    }
                }
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

    }
}
