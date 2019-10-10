using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Mail;
using System.Configuration;
using Infobip.Api.Model.Sms.Mt.Send;
using Infobip.Api.Model.Sms.Mt.Send.Textual;
using Infobip.Api.Client;
using Infobip.Api.Config;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using SAGA.API.Utilerias;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/reclutamiento/SeguimientoVacante")]
    public class PostulateVacantController : ApiController
    {
        public SAGADBContext db;
        public Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
        private SendEmails SendEmail;
        public PostulateVacantController()
        {
            db = new SAGADBContext();
            SendEmail = new SendEmails();
        }

        [HttpGet]
        [Route("validarEmailCandidato")]
        [Authorize]
        public IHttpActionResult ValidarEmailCandidato(string email)
        {
            var e = db.Emails.Where(x => x.email.Equals(email)).Count();

            if(e == 0)
            {
                return Ok(HttpStatusCode.OK);
            }
            else
            {
                return Ok(HttpStatusCode.Found);
            }
        }
        [HttpGet]
        [Route("validarTelCandidato")]
        [Authorize]
        public IHttpActionResult ValidarTelCandidato(string lada, string telefono)
        {
            var e = db.Telefonos.Where(x => x.ClaveLada.Equals(lada) && x.telefono.Equals(telefono)).Count();

            if (e == 0)
            {
                return Ok(HttpStatusCode.OK);
            }
            else
            {
                return Ok(HttpStatusCode.Found);
            }
        }
        [HttpPost]
        [Route("registrarCandidatos")]
        [Authorize]
        public IHttpActionResult RegistrarCandidatos(CandidatosGralDto datos)
        {
            var candidato = new Candidato();
            List<Guid> candidatosIds = new List<Guid>();
            CandidatosInfo obj = new CandidatosInfo();
            ProcesoCandidato proceso = new ProcesoCandidato();
            PerfilCandidato PC = new PerfilCandidato();
            ProcesoDto PCDto = new ProcesoDto();

            //foreach (var r in datos)
            //{
                //var tran = db.Database.BeginTransaction();
                try
                {
                    AspNetUsers usuario = new AspNetUsers();
                    var username = "";
                    var pass = datos.Nombre.Substring(0, 1).ToLower() + datos.ApellidoPaterno.Trim().ToLower();
                    if(pass.Length < 7)
                    {
                        pass.PadLeft(7, '0');
                    }
                    if (datos.OpcionRegistro == 1)
                    {
                        username = datos.Email[0].email.ToString();
                    }
                    else
                    {
                        username = datos.Telefono[0].ClaveLada.ToString() + datos.Telefono[0].telefono.ToString();
                    }
                    usuario.Id = Guid.NewGuid().ToString();
                    usuario.PhoneNumber = datos.Telefono[0].telefono.ToString();
                    usuario.Clave = "00000";
                    usuario.Pasword = pass;
                    usuario.RegistroClave = DateTime.Now;
                    usuario.PhoneNumberConfirmed = false;
                    usuario.EmailConfirmed = true;
                    usuario.LockoutEnabled = false;
                    usuario.AccessFailedCount = 0;
                    usuario.Email = datos.Email[0].email.ToString();
                    usuario.UserName = username;
                    usuario.Activo = 0;

                    db.AspNetUsers.Add(usuario);
                    db.SaveChanges();

                    var add = db.Database.ExecuteSqlCommand("spEncriptarPasword @id", new SqlParameter("id", usuario.Id));

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

                    if(datos.OpcionRegistro == 1)
                    {
                        candidato.emails = datos.Email;
                    }
                    else
                    {
                        candidato.telefonos = datos.Telefono;
                    }

                    db.Candidatos.Add(candidato);
                    db.SaveChanges();

                    PC.CandidatoId = candidato.Id;
                    PC.Estatus = 41;

                    db.PerfilCandidato.Add(PC);

                    db.SaveChanges();

                    var t = db.AspNetUsers.Find(usuario.Id);
                    db.Entry(t).Property(x => x.IdPersona).IsModified = true;

                    t.IdPersona = candidato.Id;
                    db.SaveChanges();

                    var horario = auxID;
                    if (datos.horarioId == auxID)
                    {
                        horario = db.HorariosRequis.Where(x => x.RequisicionId.Equals(datos.requisicionId)).Select(h => h.Id).FirstOrDefault();
                    }
                    else
                    {
                        horario = datos.horarioId;
                    }

                    proceso.CandidatoId = candidato.Id;
                    proceso.RequisicionId = datos.requisicionId;
                    proceso.Folio = db.Requisiciones.Where(x => x.Id.Equals(datos.requisicionId)).Select(R => R.Folio).FirstOrDefault();
                    proceso.Reclutador = "SIN ASIGNAR";
                    proceso.ReclutadorId = datos.reclutadorId;
                    proceso.EstatusId = 12;
                    proceso.TpContrato = 0;
                    proceso.HorarioId = horario;
                    proceso.Fch_Modificacion = DateTime.Now;
                    proceso.DepartamentoId = new Guid("d89bec78-ed5b-4ac5-8f82-24565ff394e5");
                    proceso.TipoMediosId = datos.tipoMediosId;

                    db.ProcesoCandidatos.Add(proceso);
                    db.SaveChanges();

                    obj.CandidatoId = candidato.Id;
                    obj.CURP = datos.Curp;
                    obj.RFC = "SIN ASIGNAR";
                    obj.NSS = "SIN ASIGNAR";
                    obj.FechaNacimiento = datos.FechaNac;
                    obj.Nombre = datos.Nombre;
                    obj.ApellidoPaterno = datos.ApellidoPaterno;
                    obj.ApellidoMaterno = datos.ApellidoMaterno;
                    obj.PaisNacimientoId = 42;
                    obj.EstadoNacimientoId = datos.EstadoNacimientoId;
                    obj.MunicipioNacimientoId = 0;
                    obj.GeneroId = datos.GeneroId;
                    obj.ReclutadorId = datos.reclutadorId;

                    obj.fch_Modificacion = DateTime.Now;
                    obj.fch_Modificacion.ToUniversalTime();

                    db.CandidatosInfo.Add(obj);
                    db.SaveChanges();

                    candidatosIds.Add(candidato.Id);

                    //var IDR = db.Requisiciones.Find(datos.requisicionId);
                    //if (IDR.EstatusId != 33 && IDR.EstatusId != 8 && IDR.Activo)
                    //{
                    //    db.Entry(IDR).Property(u => u.EstatusId).IsModified = true;
                    //    db.Entry(IDR).Property(u => u.fch_Modificacion).IsModified = true;

                    //    IDR.EstatusId = 33;
                    //    IDR.fch_Modificacion = DateTime.Now;

                    //    db.SaveChanges();
                    //}

                    //tran.Commit();

                    candidato = new Candidato();
                    PC = new PerfilCandidato();
                    proceso = new ProcesoCandidato();
                    obj = new CandidatosInfo();

                return Ok(candidatosIds);
            }
                catch
                {
                    //tran.Rollback();
                    return Ok(HttpStatusCode.ExpectationFailed);
                }
           
        }

        [HttpPost]
        [Route("cubrirMasivos")]
        [Authorize]
        public IHttpActionResult CubrirMasivos(List<ProcesoDto> datos)
        {
            try
            {
                foreach (var r in datos)
                {
                    var id = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(r.candidatoId) && x.RequisicionId.Equals(r.requisicionId)).Select(x => x.Id).FirstOrDefault();
                    if (id != auxID)
                    {
                        var c = db.ProcesoCandidatos.Find(id);

                        db.Entry(c).Property(x => x.EstatusId).IsModified = true;
                        db.Entry(c).Property(x => x.HorarioId).IsModified = true;
                        db.Entry(c).Property(x => x.Fch_Modificacion).IsModified = true;
                        db.Entry(c).Property(x => x.ReclutadorId).IsModified = true;

                        c.EstatusId = r.estatusId;
                        c.HorarioId = r.horarioId;
                        c.Fch_Modificacion = DateTime.Now;
                        c.ReclutadorId = r.ReclutadorId;

                        db.SaveChanges();
                    }
                    else
                    { // si el candidato no esta en proceso
                        if (r.candidatoId != auxID)
                        {
                            var horario = auxID;
                            if (r.horarioId == auxID)
                            {
                                horario = db.HorariosRequis.Where(x => x.RequisicionId.Equals(r.requisicionId)).Select(h => h.Id).FirstOrDefault();
                            }
                            else
                            {
                                horario = r.horarioId;
                            }

                            ProcesoCandidato proceso = new ProcesoCandidato();

                            proceso.CandidatoId = r.candidatoId;
                            proceso.RequisicionId = r.requisicionId;
                            proceso.Folio = db.Requisiciones.Where(x => x.Id.Equals(r.requisicionId)).Select(R => R.Folio).FirstOrDefault();
                            proceso.Reclutador = "SIN ASIGNAR";
                            proceso.ReclutadorId = r.ReclutadorId;
                            proceso.EstatusId = r.estatusId;
                            proceso.TpContrato = 0;
                            proceso.HorarioId = horario;
                            proceso.Fch_Modificacion = DateTime.Now;
                            proceso.DepartamentoId = new Guid("d89bec78-ed5b-4ac5-8f82-24565ff394e5");
                            proceso.TipoMediosId = 2;

                            db.ProcesoCandidatos.Add(proceso);
                            db.SaveChanges();
                        }
                    }
                }
                var idr = datos.Select(x => x.requisicionId).FirstOrDefault();
                var IDR = db.Requisiciones.Find(idr);
                if (IDR.EstatusId != 33 && IDR.EstatusId != 8 && IDR.Activo)
                {
                    db.Entry(IDR).Property(u => u.EstatusId).IsModified = true;
                    db.Entry(IDR).Property(u => u.fch_Modificacion).IsModified = true;

                    IDR.EstatusId = 33;
                    IDR.fch_Modificacion = DateTime.Now;

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
        [Route("getPostulate")]
        [Authorize]
        public IHttpActionResult GetPostulate(Guid VacanteId)
        {
            // Si el candidato es NR se queda atorado en proceso candidato ???
            //por lo pronto se queda así

            var postulate = db.Postulaciones.Where(p => p.RequisicionId.Equals(VacanteId) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).ToList();

            var candidatos = db.PerfilCandidato.Where(x => postulate.Contains(x.CandidatoId)).Select(x => new {
                CandidatoId = x.CandidatoId,
                nombre = x.Candidato.Nombre + " " + x.Candidato.ApellidoPaterno + " " + x.Candidato.ApellidoMaterno,
                AreaExp = x.AboutMe.Select(ae => ae.AreaExperiencia.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ae => ae.AreaExperiencia.areaInteres).FirstOrDefault() : "" ,
                AreaInt = x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() : "" ,
                localidad = x.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault() + " / " + x.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault(),
                sueldoMinimo = x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault().ToString() != null ? x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault() : 0 ,
                edad = x.Candidato.FechaNacimiento,
                rfc = x.Candidato.RFC != null ? x.Candidato.RFC : "",
                curp = x.Candidato.CURP, 
                EstatusId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(c => c.CandidatoId.Equals(x.CandidatoId)).Select(cc => cc.EstatusId).FirstOrDefault()
            }).ToList();
            return Ok(candidatos);
        }

        [HttpGet]
        [Route("getProceso")]
        [Authorize]
        public IHttpActionResult GetProceso(Guid VacanteId, Guid ReclutadorId)
        {
            try
            {
                var tipo = db.Usuarios.Where(x => x.Id.Equals(ReclutadorId)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (tipo == 4)
                {
                    var postulate = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.RequisicionId.Equals(VacanteId) & x.EstatusId != 27 & x.EstatusId != 40).Select(c => new
                    {
                        Id = c.Id,
                        folio = c.Folio,
                        tipoReclutamientoId = c.Requisicion.TipoReclutamientoId,
                        candidatoId = c.CandidatoId,
                        estatus = c.Estatus.Descripcion,
                        estatusId = c.EstatusId,
                        horarioId = c.HorarioId,
                        horario = db.HorariosRequis.Where(x => x.Id.Equals(c.HorarioId)).Select(h => h.Nombre + " de " + h.deHora.Hour + " a " + h.aHora.Hour).FirstOrDefault(),
                        contratados = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(p => new
                        {
                            nombre = p.Nombre == null ? "" : p.Nombre,
                            apellidoPaterno = String.IsNullOrEmpty( p.ApellidoPaterno) ? "Sin registro" : p.ApellidoPaterno,
                            apellidoMaterno = String.IsNullOrEmpty(p.ApellidoMaterno) ? "Sin registro" : p.ApellidoMaterno,
                            edad = p.FechaNacimiento,
                            rfc = String.IsNullOrEmpty(p.RFC) ? "Sin registro" : p.RFC,
                            curp = String.IsNullOrEmpty(p.CURP) ? "Sin registro" : p.CURP,
                            nss = String.IsNullOrEmpty(p.NSS) ? "Sin registro" : p.NSS,
                            paisNacimiento = p.PaisNacimientoId,
                            estadoNacimiento = p.EstadoNacimientoId,
                            municipioNacimiento = p.MunicipioNacimientoId,
                            localidad = p.municipioNacimiento.municipio + " / " + p.estadoNacimiento.estado,
                            generoId = p.GeneroId
                        }).ToList(),

                        perfil = db.PerfilCandidato.Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(x => new
                        {
                            foto = String.IsNullOrEmpty(x.Candidato.ImgProfileUrl) ? "utilerias/img/user/default.jpg" : x.Candidato.ImgProfileUrl,
                            AreaExp = x.AboutMe.Select(ae => ae.AreaExperiencia.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ae => ae.AreaExperiencia.areaInteres).FirstOrDefault() : "",
                            AreaInt = x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() : "",
                            sueldoMinimo = x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault().ToString() != null ? x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault() : 0,
                            nombre = x.Candidato.Nombre,
                            apellidoPaterno = String.IsNullOrEmpty(x.Candidato.ApellidoPaterno) ? "Sin registro" : x.Candidato.ApellidoPaterno,
                            apellidoMaterno = String.IsNullOrEmpty(x.Candidato.ApellidoMaterno) ? "Sin registro" : x.Candidato.ApellidoMaterno,
                            localidad = x.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault() + " / " + x.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault(),
                            edad = x.Candidato.FechaNacimiento,
                            rfc = String.IsNullOrEmpty(x.Candidato.RFC) ? "Sin registro" : x.Candidato.RFC,
                            curp = String.IsNullOrEmpty(x.Candidato.CURP) ? "Sin registro" : x.Candidato.CURP,
                            nss = String.IsNullOrEmpty(x.Candidato.NSS) ? "Sin registro" : x.Candidato.NSS,
                            paisNacimiento = x.Candidato.PaisNacimientoId,
                            estadoNacimiento = x.Candidato.EstadoNacimientoId,
                            municipioNacimiento = x.Candidato.MunicipioNacimientoId,
                            generoId = x.Candidato.GeneroId
                        }).ToList(),
                        usuario = c.Reclutador,
                        usuarioId = c.ReclutadorId,
                        fecha = c.Fch_Modificacion,
                        areaReclutamiento = c.Departamentos.Nombre,
                        areaReclutamientoId = c.DepartamentoId,
                        fuenteReclutamiento = c.TipoMedios.Nombre,
                        fuenteReclutamientoId = c.TipoMediosId
                    }).ToList();

                    return Ok(postulate);
                }
                else
                {
                    var postulate = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.RequisicionId.Equals(VacanteId) & x.ReclutadorId.Equals(ReclutadorId) & x.EstatusId != 27 & x.EstatusId != 40).Select(c => new
                    {
                        Id = c.Id,
                        folio = c.Folio,
                        tipoReclutamientoId = c.Requisicion.TipoReclutamientoId,
                        candidatoId = c.CandidatoId,
                        estatus = c.Estatus.Descripcion,
                        estatusId = c.EstatusId,
                        horarioId = c.HorarioId,
                        horario = db.HorariosRequis.Where(x => x.Id.Equals(c.HorarioId)).Select(h => h.Nombre + " de " + h.deHora.Hour + " a " + h.aHora.Hour).FirstOrDefault(),
                        contratados = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(p => new
                        {
                            nombre = p.Nombre,
                            apellidoPaterno = p.ApellidoPaterno,
                            apellidoMaterno = String.IsNullOrEmpty(p.ApellidoMaterno) ? "" : p.ApellidoMaterno,
                            edad = p.FechaNacimiento,
                            rfc = String.IsNullOrEmpty(p.RFC) ? "Sin registro" : p.RFC,
                            curp = String.IsNullOrEmpty(p.CURP) ? "Sin registro" : p.CURP,
                            nss = String.IsNullOrEmpty(p.NSS) ? "Sin registro" : p.NSS,
                            paisNacimiento = p.PaisNacimientoId,
                            estadoNacimiento = p.EstadoNacimientoId,
                            municipioNacimiento = p.MunicipioNacimientoId,
                            localidad = p.municipioNacimiento.municipio + " / " + p.estadoNacimiento.estado,
                            generoId = p.GeneroId
                        }).ToList(),

                        perfil = db.PerfilCandidato.Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(x => new
                        {
                            foto = String.IsNullOrEmpty(x.Candidato.ImgProfileUrl) ? "utilerias/img/user/default.jpg" : x.Candidato.ImgProfileUrl,
                            AreaExp = x.AboutMe.Select(ae => ae.AreaExperiencia.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ae => ae.AreaExperiencia.areaInteres).FirstOrDefault() : "",
                            AreaInt = x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() : "",
                            sueldoMinimo = x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault().ToString() != null ? x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault() : 0,
                            nombre = x.Candidato.Nombre,
                            apellidoPaterno = x.Candidato.ApellidoPaterno,
                            apellidoMaterno = String.IsNullOrEmpty(x.Candidato.ApellidoMaterno) ? "" : x.Candidato.ApellidoMaterno,
                            localidad = x.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault() + " / " + x.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault(),
                            edad = x.Candidato.FechaNacimiento,
                            rfc = String.IsNullOrEmpty(x.Candidato.RFC) ? "Sin registro" : x.Candidato.RFC,
                            curp = String.IsNullOrEmpty(x.Candidato.CURP) ? "Sin registro" : x.Candidato.CURP,
                            nss = String.IsNullOrEmpty(x.Candidato.NSS) ? "Sin registro" : x.Candidato.NSS,
                            paisNacimiento = x.Candidato.PaisNacimientoId,
                            estadoNacimiento = x.Candidato.EstadoNacimientoId,
                            municipioNacimiento = x.Candidato.MunicipioNacimientoId,
                            generoId = x.Candidato.GeneroId
                        }).ToList(),
                        usuario = c.Reclutador,
                        usuarioId = c.ReclutadorId,
                        fecha = c.Fch_Modificacion,
                        areaReclutamiento = c.Departamentos.Nombre,
                        areaReclutamientoId = c.DepartamentoId,
                        fuenteReclutamiento = c.TipoMedios.Nombre,
                        fuenteReclutamientoId = c.TipoMediosId
                    }).ToList();

                    return Ok(postulate);
                }
            }
            catch(Exception ex)
            {
                 return Ok(HttpStatusCode.BadRequest);
            }

        }

        [HttpGet]
        [Route("getCandidatosCubiertos")]
        [Authorize]
        public IHttpActionResult GetCandidatosCubiertos(Guid requisicionId)
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
                    informacion = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(p => new
                    {
                        nombre = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
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
                        reclutadorId = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Id).FirstOrDefault()
                }).FirstOrDefault()
                });
                    
                return Ok(candidatos);
            }
            catch
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpPost]
        [Route("updateStatusVacante")]
        [Authorize]
        public IHttpActionResult UpdateStatusVacante(ProcesoDto datos)
        {
            try
            {
                var R = db.Requisiciones.Find(datos.requisicionId);
                db.Entry(R).Property(u => u.EstatusId).IsModified = true;
                db.Entry(R).Property(u => u.fch_Modificacion).IsModified = true;

                R.EstatusId = datos.estatusId;
                R.fch_Modificacion = DateTime.Now;
                //R.AprobadorId = datos.ReclutadorId;
                db.SaveChanges();

                if(datos.estatusId >= 34 && datos.estatusId <= 37 || datos.estatusId == 47 || datos.estatusId == 48)
                {
                    R.Publicado = false;
                    db.SaveChanges();

                    UpdateStatusBolsaFinalizado(datos);
                }  

                if (datos.estatusId == 44)
                {
                    datos.estatusId = 4;
                    db.Entry(R).Property(u => u.EstatusId).IsModified = true;
                    db.Entry(R).Property(u => u.fch_Modificacion).IsModified = true;
                    R.EstatusId = datos.estatusId;
                    R.fch_Modificacion = DateTime.Now;
                    db.SaveChanges();
                }

                /*
                 * Este proceso se modifico ya no es necesario buscar al Gerente de reclutamiento para hacerle la asigancion de la requisicion
                 * cuendo la misma es de tipoReclutamiento 1 (PURO). 
                 */
                //if (datos.estatusId == 4)
                //{
                //    var requi = db.Requisiciones
                //        .Where(r => r.Id.Equals(datos.requisicionId))
                //        .Select(r => new
                //        {
                //            vBtra = r.VBtra,
                //            folio = r.Folio,
                //            estado = r.Direccion.EstadoId,
                //            tipoReclutamiento = r.TipoReclutamientoId,
                //            prop = db.Entidad
                //                .Where(e => e.Id.Equals(r.PropietarioId)).
                //                Select(x => x.Nombre + "" + x.ApellidoPaterno).FirstOrDefault()
                //        }).FirstOrDefault();

                //    if (datos.estatusId == 4 && requi.tipoReclutamiento == 1)
                //    {
                //        Guid GReclutamiento = Guid.NewGuid();

                        
                //        GReclutamiento = db.Usuarios
                //            .Where(u => u.TipoUsuarioId.Equals(3) && u.Departamento.Clave.Equals("RECL") && u.Activo.Equals(true))
                //            .Select(u => u.Id)
                //            .FirstOrDefault();

                //        AsignacionRequi agr = new AsignacionRequi();
                //        agr.RequisicionId = datos.requisicionId;
                //        agr.GrpUsrId = GReclutamiento;
                //        agr.CRUD = "";
                //        agr.UsuarioAlta = "SISTEMA";
                //        agr.UsuarioMod = "SISTEMA";
                //        agr.fch_Modificacion = DateTime.Now;

                //        db.AsignacionRequis.Add(agr);
                //        db.SaveChanges();

                //        List<AsignacionRequi> asignaciones = new List<AsignacionRequi>();
                //        asignaciones.Add(agr);
                //        SendEmail.ConstructEmail(asignaciones, null, "C", requi.folio, requi.prop, requi.vBtra, null);
                //    }
                //}

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpPost]
        [Route("updateStatus")]
        [Authorize]
        public IHttpActionResult UpdateStatus(ProcesoDto datos)
        {
            FoliosIncidenciasController obj = new FoliosIncidenciasController();
            try
            {
                var id = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(datos.candidatoId) && x.RequisicionId.Equals(datos.requisicionId)).Select(x => x.Id).FirstOrDefault();
                if (id != auxID)
                {
                    var c = db.ProcesoCandidatos.Find(id);

                    if (datos.estatusId == 12 && (c.EstatusId == 27 || c.EstatusId == 10))
                    {
                        db.Entry(c).Property(x => x.EstatusId).IsModified = true;
                        db.Entry(c).Property(x => x.HorarioId).IsModified = true;
                        db.Entry(c).Property(x => x.Fch_Modificacion).IsModified = true;
                        db.Entry(c).Property(x => x.ReclutadorId).IsModified = true;

                        c.EstatusId = datos.estatusId;
                        c.HorarioId = datos.horarioId;
                        c.Fch_Modificacion = DateTime.Now;
                        c.ReclutadorId = datos.ReclutadorId;

                        db.SaveChanges();

                        var requi = db.EstatusRequisiciones.Where(x => x.RequisicionId.Equals(datos.requisicionId) && x.EstatusId.Equals(29)).Count();
                        if (requi == 0)
                        {
                            datos.estatusId = 29;
                            UpdateStatusVacante(datos);

                        }
                        return Ok(HttpStatusCode.Created);
                    }
                    else if (datos.estatusId != 12)
                    {
                        db.Entry(c).Property(x => x.EstatusId).IsModified = true;
                        db.Entry(c).Property(x => x.HorarioId).IsModified = true;
                        db.Entry(c).Property(x => x.Fch_Modificacion).IsModified = true;
                        db.Entry(c).Property(x => x.ReclutadorId).IsModified = true;

                        c.EstatusId = datos.estatusId;
                        c.HorarioId = datos.horarioId;
                        c.Fch_Modificacion = DateTime.Now;
                        c.ReclutadorId = datos.ReclutadorId;

                        db.SaveChanges();

                        if (datos.estatusId == 42)
                        {
                            obj.EnviarEmailNR(datos.candidatoId, datos.requisicionId, datos.ReclutadorId);
                        }
                        return Ok(HttpStatusCode.Created);
                    }
                    else
                    {
                        return Ok(HttpStatusCode.Ambiguous);
                    }
                }
                else
                {
                    if (datos.candidatoId != auxID)
                    {
                        var horario = auxID;
                        if (datos.horarioId == auxID)
                        {
                            horario = db.HorariosRequis.Where(x => x.RequisicionId.Equals(datos.requisicionId)).Select(h => h.Id).FirstOrDefault();
                        }
                        else
                        {
                            horario = datos.horarioId;
                        }

                        ProcesoCandidato proceso = new ProcesoCandidato();

                        proceso.CandidatoId = datos.candidatoId;
                        proceso.RequisicionId = datos.requisicionId;
                        proceso.Folio = db.Requisiciones.Where(x => x.Id.Equals(datos.requisicionId)).Select(R => R.Folio).FirstOrDefault();
                        proceso.Reclutador = "SIN ASIGNAR";
                        proceso.ReclutadorId = datos.ReclutadorId;
                        proceso.EstatusId = datos.estatusId;
                        proceso.TpContrato = 0;
                        proceso.HorarioId = horario;
                        proceso.Fch_Modificacion = DateTime.Now;
                        proceso.DepartamentoId = new Guid("d89bec78-ed5b-4ac5-8f82-24565ff394e5");
                        proceso.TipoMediosId = 2;

                        db.ProcesoCandidatos.Add(proceso);
                        db.SaveChanges();

                        if (datos.estatusId == 12)
                        {
                            var requi = db.EstatusRequisiciones.Where(x => x.RequisicionId.Equals(datos.requisicionId) && x.EstatusId.Equals(29)).Count();
                            if (requi == 0)
                            {
                                datos.estatusId = 29;
                                UpdateStatusVacante(datos);
                            }
                        }
                    }
                    return Ok(HttpStatusCode.OK);
                }
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }
        
   
        public IHttpActionResult LiberarCandidatos(ProcesoDto datos)
        {
            Guid aux = new Guid("00000000-0000-0000-0000-000000000000");
            try
            {
                var id = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(datos.candidatoId) && x.RequisicionId.Equals(datos.requisicionId)).Select(x => x.Id).FirstOrDefault();
                var c = db.ProcesoCandidatos.Find(id);

                db.Entry(c).Property(x => x.EstatusId).IsModified = true;
                c.EstatusId = datos.estatusId;

                db.SaveChanges();

                var ids = db.Postulaciones.Where(x => x.RequisicionId.Equals(datos.requisicionId) && x.CandidatoId.Equals(datos.candidatoId)).Select(x => x.Id).FirstOrDefault();
                if (ids != auxID)
                {
                    var cc = db.Postulaciones.Find(ids);

                    db.Entry(cc).Property(x => x.StatusId).IsModified = true;
                    cc.StatusId = 5;

                    db.SaveChanges();
                }
                    
                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
        [HttpPost]
        [Route("updateStatusBolsa")]
        [Authorize]
        public IHttpActionResult UpdateStatusBolsa(ProcesoDto datos)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");

                var id = db.Postulaciones.Where(x => x.CandidatoId.Equals(datos.candidatoId) && x.RequisicionId.Equals(datos.requisicionId)).Select(x => x.Id).FirstOrDefault();

                if (id == aux)
                {
                    Postulacion obj = new Postulacion();
                    obj.RequisicionId = datos.requisicionId;
                    obj.CandidatoId = datos.candidatoId;
                    obj.StatusId = datos.estatusId;

                    db.Postulaciones.Add(obj);
                    db.SaveChanges();

                    return Ok(HttpStatusCode.Created);
                }
                else
                {

                    var c = db.Postulaciones.Find(id);
                    db.Entry(c).Property(x => x.StatusId).IsModified = true;
                    c.StatusId = datos.estatusId;

                    db.SaveChanges();

                    return Ok(HttpStatusCode.Created);
                }
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        public IHttpActionResult UpdateStatusBolsaFinalizado(ProcesoDto datos)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");
                var ids = db.Postulaciones.Where(x => x.RequisicionId.Equals(datos.requisicionId)).Select(x => x.Id).ToList();

                foreach(Guid id in ids)
                {

                    if (id == aux)
                    {
                        Postulacion obj = new Postulacion();
                        obj.RequisicionId = datos.requisicionId;
                        obj.CandidatoId = datos.candidatoId;
                        obj.StatusId = 5;

                        db.Postulaciones.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        var c = db.Postulaciones.Find(id);
                        db.Entry(c).Property(x => x.StatusId).IsModified = true;
                        c.StatusId = 5;

                        db.SaveChanges();

                    }

                }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        public async Task<IHttpActionResult> EnviarSMS(string telefono, string vacante, int estatusId)
        {
            Regex reg = new Regex("[^a-zA-Z0-9] ");
            List<string> Destino = new List<string>(1) { ConfigurationManager.AppSettings["Lada"] + telefono };
            BasicAuthConfiguration BASIC_AUTH_CONFIGURATION = new BasicAuthConfiguration(ConfigurationManager.AppSettings["BaseUrl"], ConfigurationManager.AppSettings["UserInfobip"], ConfigurationManager.AppSettings["PassInfobip"]);

            SendSingleTextualSms smsClient = new SendSingleTextualSms(BASIC_AUTH_CONFIGURATION);

            if (estatusId == 17)
            {
                string texto = "Bolsa Trabajo DAMSA te felicita por Iniciar proceso a la vacante " + vacante + ". Da click https://www.damsa.com.mx/bt para dar seguimiento";
                texto = texto.Normalize(NormalizationForm.FormD);
                texto = reg.Replace(texto, " ");

                SMSTextualRequest request = new SMSTextualRequest
                {
                    From = "DAMSA",
                    To = Destino,
                    Text = ConfigurationManager.AppSettings["NameAppMsj"] + texto

                };

                SMSResponse smsResponse = await smsClient.ExecuteAsync(request); // Manda el mensaje con código.

                SMSResponseDetails sentMessageInfo = smsResponse.Messages[0];

                vacante = "";
            }
            else if(estatusId == 21)
            {
                string texto = "Bolsa Trabajo DAMSA, te felicita por ser finalista a la vacante " + vacante + ". Da click https://www.damsa.com.mx/bt para dar seguimiento";
                texto = texto.Normalize(NormalizationForm.FormD);
                texto = reg.Replace(texto, " ");

                SMSTextualRequest request = new SMSTextualRequest
                {
                    From = "DAMSA",
                    To = Destino,
                    Text = ConfigurationManager.AppSettings["NameAppMsj"] + texto

                };

                SMSResponse smsResponse = await smsClient.ExecuteAsync(request); // Manda el mensaje con código.

                SMSResponseDetails sentMessageInfo = smsResponse.Messages[0];

            }
            else if (estatusId == 27)
            {
                string texto = string.Format("Bolsa Trabajo DAMSA, informa que el cliente cubrió la vacante {0}. https://www.damsa.com.mx/bt ", vacante);

                texto = texto.Normalize(NormalizationForm.FormD);
                texto = reg.Replace(texto, " ");

                SMSTextualRequest request = new SMSTextualRequest
                {
                    From = "DAMSA",
                    To = Destino,
                    Text = ConfigurationManager.AppSettings["NameAppMsj"] + texto

                };

                SMSResponse smsResponse = await smsClient.ExecuteAsync(request); // Manda el mensaje con código.

                SMSResponseDetails sentMessageInfo = smsResponse.Messages[0];
            }
            else if(estatusId == 24)
            {
                string texto = string.Format("Bolsa Trabajo DAMSA, te felicita por ser contratado a la vacante {0}. https://www.damsa.com.mx/bt", vacante);

                texto = texto.Normalize(NormalizationForm.FormD);
                texto = reg.Replace(texto, " ");

                SMSTextualRequest request = new SMSTextualRequest
                {
                    From = "DAMSA",
                    To = Destino,
                    Text = ConfigurationManager.AppSettings["NameAppMsj"] + texto

                };

                SMSResponse smsResponse = await smsClient.ExecuteAsync(request); // Manda el mensaje con código.

                SMSResponseDetails sentMessageInfo = smsResponse.Messages[0];

            }
            return Ok(HttpStatusCode.Created);
        }

        [HttpPost]
        [Route("sendEmailCandidato")]
        [Authorize]
        public IHttpActionResult SendEmailCandidato(ProcesoDto datos)
        {
            var path = "~/logo/logo.png";
            string fullPath = "http://192.168.8.124:333/logo/logo.png";
            path = "~/logo/boton.png";
            string fullPath2 = "http://192.168.8.124:333/logo/boton.png";
            string body = "";
            string usuario = "";
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SAGADB"].ConnectionString);
                SqlCommand cmd = new SqlCommand();
             

                cmd.CommandText = "SELECT * FROM sist.AspNetUsers WHERE IdPersona=@candidatoId";
                cmd.Parameters.AddWithValue("@candidatoId", datos.candidatoId);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = reader["UserName"].ToString();
                    }
                }

            
                conn.Close();

                //usuario = "bmorales@damsa.com.mx";

                if (usuario != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "DAMSA");
                    m.Subject = "Bolsa de Trabajo DAMSA";

                    if (datos.estatusId == 17)
                    {
                        if (usuario.Contains("@"))
                        {
                            m.To.Add(usuario);
                            body = "<html><head></head>";
                            body = body + "<body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                            body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                            body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", datos.nombre);
                            body = body + "<br/><br/><h1>¡Felicidades!</h1>";
                            body = body + "<p>Eres uno(a) de los/las candidatos/as que inicia proceso para la vacante de</p>";
                            body = body + string.Format("<h1 style=\"color:#3366cc;\">{0}</h1>", datos.vacante);
                            body = body + "<p>Solo puedes estar en un proceso de seguimiento</p>";
                            body = body + "<p> Si esta vacante no es de tu inter&eacute;s puedes declinar a esta postulaci&oacute;n.</p>";
                            body = body + string.Format("<a href=\"https://www.damsa.com.mx/bt\" target =\"_blank\"><img src=\"{0}\"></a>", fullPath2);
                            body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font style:\"text-decoration:none\"; color=\"#5d9cec\">{0}</font></p>", usuario);
                            body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";
                            m.Body = body;
                            m.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);

                            //var msj = new SqlParameter("@msj", body);
                            //var email = new SqlParameter("@email", usuario);
                            //var result = db.Database.ExecuteSqlCommand("dbo.sp_SendEmail @msj = {0}, @email = {1}", msj, email);

                        }
                        else
                        {
                            return Ok(EnviarSMS(usuario, datos.vacante, datos.estatusId));
                        }

                    }
                    else if (datos.estatusId == 21)
                    {
                        if (usuario.Contains("@"))
                        {
                            m.To.Add(usuario);
                            body = "<html><head></head>";
                            body = body + "<body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                            body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                            body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", datos.nombre);
                            body = body + "<br/><br/><h1>¡Felicidades!</h1>";
                            body = body + string.Format("<p>Eres uno(a) de los/las finalistas para la vacante de <h1 style=\"color:#3366cc;\">{0}</h1></p>", datos.vacante);
                            body = body + "<p>Solo puedes estar en un proceso de seguimiento</p>";
                            body = body + "<p> Si esta vacante no es de tu inter&eacute;s puedes declinar a esta postulaci&oacute;n.</p>";
                            body = body + string.Format("<a href=\"https://www.damsa.com.mx/bt\" target =\"_blank\"><img src=\"{0}\"></a>", fullPath2);
                            body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font style:\"text-decoration:none\"; color=\"#5d9cec\">{0}</font></p>", usuario);
                            body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";
                            m.Body = body;
                            m.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);

                            //var msj = new SqlParameter("@msj", body);
                            //var email = new SqlParameter("@email", usuario);
                            //var result = db.Database.ExecuteSqlCommand("dbo.sp_SendEmail @msj = {0}, @email = {1}", "body", "usuario");
                        }
                        else
                        {
                            return Ok(EnviarSMS(usuario, datos.vacante, datos.estatusId));
                        }

                    }
                    else if (datos.estatusId == 27)
                    {
                        if (usuario.Contains("@"))
                        {
                            m.To.Add(usuario);
                            body = "<html><head></head><body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                            body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                            body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", datos.nombre);
                            body = body + "<br/><br/>";
                            body = body + string.Format("<p>Gracias por tu inter&eacute;s en nuestra empresa y por el tiempo que has dedicado para el proceso de <h1 style=\"color:#3366cc;\">{0}</h1></p>", datos.vacante);
                            body = body + "<p>Te escribimos para informarte que el cliente ha seleccionado un candidato, sin embargo y con tu conformidad, conservaremos tu CV en nuestra base de datos para futuras selecciones.</p>";
                            body = body + "<p>Agradecemos tu participaci&oacute;n</p>";
                            body = body + "<p>En la siguiente liga puedes encontrar vacantes similares:</p>";
                            body = body + string.Format("<a href=\"https://www.damsa.com.mx/bt\" target =\"_blank\"><img src=\"{0}\"></a>", fullPath2);
                            body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font style:\"text-decoration:none\"; color=\"#5d9cec\">{0}</font></p>", usuario);
                            body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";

                            m.Body = body;
                            m.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);

                            //var msj = new SqlParameter("@msj", body);
                            //var email = new SqlParameter("@email", usuario);
                            //var result = db.Database.ExecuteSqlCommand("dbo.sp_SendEmail @msj = {0}, @email = {1}", msj, email);
                        }
                        else
                        {
                            return Ok(EnviarSMS(usuario, datos.vacante, datos.estatusId));
                        }
                    }
                    else if(datos.estatusId == 24)
                    {
                        if (usuario.Contains("@"))
                        {
                            m.To.Add(usuario);
                            body = "<html><head></head><body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                            body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                            body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", datos.nombre);
                            body = body + "<br/><br/><h1>¡Felicidades!</h1>";
                            body = body + string.Format("<p>Eres uno(a) de los/las contratados para la vacante <h1 style=\"color:#3366cc;\">{0}</h1></p>", datos.vacante);
                            body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font style:\"text-decoration:none\"; color=\"#5d9cec\">{0}</font></p>", usuario);
                            body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";

                            m.Body = body;
                            m.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);

                            //var msj = new SqlParameter("@msj", body);
                            //var email = new SqlParameter("@email", usuario);
                            //var result = db.Database.ExecuteSqlCommand("dbo.sp_SendEmail @msj = {0}, @email = {1}", "body", "usuario");
                        }
                        else
                        {
                            return Ok(EnviarSMS(usuario, datos.vacante, datos.estatusId));
                        }

                    }
                }
                return Ok(HttpStatusCode.Created);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

            //
       
        }

        [HttpPost]
        [Route("sendEmailContratados")]
        [Authorize]
        public IHttpActionResult SendEmailContratados(List<Guid> datos)
        {
            string fullPath = "http://192.168.8.124:333/logo/logo.png";

            string body = "";
            string usuario = "";
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SAGADB"].ConnectionString);
                SqlCommand cmd = new SqlCommand();

                foreach (var r in datos)
                {

                    cmd.CommandText = "SELECT * FROM sist.AspNetUsers WHERE IdPersona=@candidatoId";
                    cmd.Parameters.AddWithValue("@candidatoId", r);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = reader["UserName"].ToString();
                        }
                    }


                    conn.Close();

                    var D = db.Candidatos.Where(x => x.Id.Equals(r)).Select(n => new
                    {
                        nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                        vacante = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(r) && x.EstatusId.Equals(24)).Select(P => P.Requisicion.VBtra).FirstOrDefault()
                    }).FirstOrDefault();

                    if (usuario != "")
                    {
                        string from = "noreply@damsa.com.mx";
                        MailMessage m = new MailMessage();
                        m.From = new MailAddress(from, "DAMSA");
                        m.Subject = "Bolsa de Trabajo DAMSA";

                        if (usuario.Contains("@"))
                        {
                            m.To.Add(usuario);
                            m.Bcc.Add("idelatorre@damsa.com.mx");
                            m.Bcc.Add("bmorales@damsa.com.mx");
                            body = "<html><head></head><body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                            body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                            body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", D.nombre);
                            body = body + "<br/><br/><h1>¡Felicidades!</h1>";
                            body = body + string.Format("<p>Eres uno(a) de los/las contratados para la vacante <h1 style=\"color:#3366cc;\">{0}</h1></p>", D.vacante);
                            body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font style:\"text-decoration:none\"; color=\"#5d9cec\">{0}</font></p>", usuario);
                            body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";

                            m.Body = body;
                            m.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);

                            //var msj = new SqlParameter("@msj", body);
                            //var email = new SqlParameter("@email", usuario);
                            //var result = db.Database.ExecuteSqlCommand("dbo.sp_SendEmail @msj = {0}, @email = {1}", "body", "usuario");
                        }
                        else
                        {
                            return Ok(EnviarSMS(usuario, D.vacante, 24));
                        }
                    }
                }
                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

            //

        }

        //falta mensaje
        [HttpPost]
        [Route("sendEmailsNoContratado")]
        [Authorize]
        public IHttpActionResult SendEmailsNoContratados(List<ProcesoDto> datos)
        {
            var path = "~/utilerias/img/logo/logo.png";
            string fullPath = "http://192.168.8.124:333/logo/logo.png"; // System.Web.Hosting.HostingEnvironment.MapPath(path);
            path = "~/utilerias/img/logo/boton.png";
            string fullPath2 = "http://192.168.8.124:333/logo/boton.png";
            string body = "";
            string usuario = "bmorales@damsa.com.mx";

            string from = "noreply@damsa.com.mx";
            MailMessage m = new MailMessage();
            m.From = new MailAddress(from, "DAMSA");
            m.Subject = "Bolsa de Trabajo DAMSA";

            try
            {
                foreach (var e in datos)
                {
                    m.To.Clear();
                    
                    if (e.email.Contains("@"))
                    {
                        var res = LiberarCandidatos(e);

                        m.To.Add(e.email);
                        body = "<html><head></head><body style=\"text-align:center; font-family:'calibri'\">";
                        body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                        body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", e.nombre);
                        body = body + "<br/><br/><br/>";
                        body = body + string.Format("<p>Gracias por tu inter&eacute;s en nuestra empresa y por el tiempo que has dedicado para el proceso de <h1 style=\"color:#3366cc;\">{0}</h1></p>", e.vacante);
                        body = body + "<p>Te escribimos para informarte que el cliente ha seleccionado un candidato, sin embargo y con tu conformidad, conservaremos tu CV en nuestra base de datos para futuras selecciones.</p>";
                        body = body + "<p>Agradecemos tu participaci&oacute;n</p>";
                        body = body + "<p>En la siguiente liga puedes encontrar vacantes similares:</p>";
                        body = body + string.Format("<a href=\"https://www.damsa.com.mx/bt\" target =\"_blank\"><img src=\"{0}\"></a>", fullPath2);
                        body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font color=\"#5d9cec\">{0}</font></p>", e.email);
                        body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                        body = body + "</body></html>";

                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                        smtp.Send(m);
                    }
                    else
                    {
                        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SAGADB"].ConnectionString);
                        SqlCommand cmd = new SqlCommand();


                        cmd.CommandText = "SELECT * FROM sist.AspNetUsers WHERE IdPersona=@candidatoId";
                        cmd.Parameters.AddWithValue("@candidatoId", e.candidatoId);
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuario = reader["UserName"].ToString();
                            }
                        }


                        conn.Close();

                        var result = LiberarCandidatos(e);
                        

                        if (usuario != "")
                        {
                            var res = EnviarSMS(usuario, e.vacante, 27);
                        }

                    }
                }

               return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

            //

        }
    }
}
