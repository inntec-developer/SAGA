using SAGA.API.Utilerias;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Dtos;
using SAGA.BOL;
using System.Data.Entity;
using SAGA.API.Dtos.Reclutamiento.Ingresos;

namespace SAGA.API.Controllers.SistGestionPersonal
{
    [RoutePrefix("api/gestionpersonal")]
    public class SistGestionPersonalController : ApiController
    {
        Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
        private SAGADBContext db;
        APISAGALog apilog = new APISAGALog();
        public SistGestionPersonalController()
        {
            db = new SAGADBContext();
        }
        public HttpResponseMessage CRUDAsigVacaciones(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    PeriodoVacaciones a = new PeriodoVacaciones();
                    a = datos.PeriodoVacaciones;
                    a.fchAlta = DateTime.Now;
                   
                    db.PeriodoVacaciones.Add(a);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoVacaciones.Find(datos.PeriodoVacaciones.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodoVacaciones.CandidatosInfoId;
                    a.dias = datos.PeriodoVacaciones.dias;
                    a.fchIncio = datos.PeriodoVacaciones.fchIncio;
                    a.fchFin = datos.PeriodoVacaciones.fchFin;
                    a.Comentario = datos.PeriodoVacaciones.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodoVacaciones.UsuarioAltaId;
              
                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoVacaciones.Find(datos.PeriodoVacaciones.Id);
                    db.Entry(a).State = EntityState.Deleted;
                 
                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeriodoVacaciones - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDAsigDiasEconomicos(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    PeriodoDE a = new PeriodoDE();
                    a = datos.PeriodoDE;
                    a.fchAlta = DateTime.Now;

                    db.PeriodoDE.Add(a);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoDE.Find(datos.PeriodoDE.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodoDE.CandidatosInfoId;
                    a.dias = datos.PeriodoDE.dias;
                    a.fchIncio = datos.PeriodoDE.fchIncio;
                    a.fchFin = datos.PeriodoDE.fchFin;
                    a.Comentario = datos.PeriodoDE.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodoDE.UsuarioAltaId;

                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoDE.Find(datos.PeriodoDE.Id);
                    db.Entry(a).State = EntityState.Deleted;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeridoDE - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDAsigIncapacidades(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    PeriodoIncapacidad a = new PeriodoIncapacidad();
                    a = datos.PeriodoIncapacidad;
                    a.fchAlta = DateTime.Now;

                    db.PeriodoIncapacidad.Add(a);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoIncapacidad.Find(datos.PeriodoIncapacidad.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodoIncapacidad.CandidatosInfoId;
                    a.dias = datos.PeriodoIncapacidad.dias;
                    a.fchIncio = datos.PeriodoIncapacidad.fchIncio;
                    a.fchFin = datos.PeriodoIncapacidad.fchFin;
                    a.Archivo = datos.PeriodoIncapacidad.Archivo;
                    a.SerieFolio = datos.PeriodoIncapacidad.SerieFolio;
                    a.TiposIncapacidadId = datos.PeriodoIncapacidad.TiposIncapacidadId;
                    a.Comentario = datos.PeriodoIncapacidad.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodoIncapacidad.UsuarioAltaId;

                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoIncapacidad.Find(datos.PeriodoIncapacidad.Id);
                    db.Entry(a).State = EntityState.Deleted;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeriodoIncapacidad - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDAsigPermisos(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    PeriodoPermisos a = new PeriodoPermisos();
                    a = datos.PeriodoPermisos;
                    a.fchAlta = DateTime.Now;

                    db.PeriodoPermisos.Add(a);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoPermisos.Find(datos.PeriodoPermisos.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodoPermisos.CandidatosInfoId;
                    a.dias = datos.PeriodoPermisos.dias;
                    a.fchIncio = datos.PeriodoPermisos.fchIncio;
                    a.fchFin = datos.PeriodoPermisos.fchFin;
                    a.TipoJustificacionId = datos.PeriodoPermisos.TipoJustificacionId;
                    a.Sueldo = datos.PeriodoPermisos.Sueldo;
                    a.Tipo = datos.PeriodoPermisos.Tipo;
                    a.Comentario = datos.PeriodoIncapacidad.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodoIncapacidad.UsuarioAltaId;

                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoPermisos.Find(datos.PeriodoIncapacidad.Id);
                    db.Entry(a).State = EntityState.Deleted;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeriodoPermisos - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDAsigGuardia(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    PeriodoGuardia a = new PeriodoGuardia();
                    a = datos.PeriodoGuardia;
                    a.fchAlta = DateTime.Now;

                    db.PeriodoGuardia.Add(a);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoGuardia.Find(datos.PeriodoGuardia.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodoGuardia.CandidatosInfoId;
                    a.Fecha = datos.PeriodoGuardia.Fecha;
                    a.CubridorId = datos.PeriodoGuardia.CubridorId;
                    a.Comentario = datos.PeriodoIncapacidad.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodoIncapacidad.UsuarioAltaId;

                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoGuardia.Find(datos.PeriodoIncapacidad.Id);
                    db.Entry(a).State = EntityState.Deleted;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeriodoGuardia - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDAsigSuspension(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    if (datos.PeriodoSuspension != null)
                    {
                        PeriodoSuspension a = new PeriodoSuspension();
                        a = datos.PeriodoSuspension;
                        a.fchAlta = DateTime.Now;

                        db.PeriodoSuspension.Add(a);
                        db.SaveChanges();

                    }
                    else if (datos.PeriodoSuspensionList.Count() > 0)
                    {
                        foreach (var o in datos.PeriodoSuspensionList)
                        {
                            o.fchAlta = DateTime.Now;
                        }

                        db.PeriodoSuspension.AddRange(datos.PeriodoSuspensionList);
                        db.SaveChanges();
                    }
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoSuspension.Find(datos.PeriodoSuspension.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodoSuspension.CandidatosInfoId;
                    a.fchIncio = datos.PeriodoSuspension.fchIncio;
                    a.fchFin = datos.PeriodoSuspension.fchFin;
                    a.dias = datos.PeriodoSuspension.dias;
                    a.Comentario = datos.PeriodoSuspension.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodoSuspension.UsuarioAltaId;

                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoSuspension.Find(datos.PeriodoSuspension.Id);
                    db.Entry(a).State = EntityState.Deleted;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeriodoSuspension - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDAsigActa(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    if (datos.PeriodoActaList.Count() > 0)
                    {
                        foreach (var o in datos.PeriodoActaList)
                        {
                            o.fchAlta = DateTime.Now;
                        }

                        db.PeriodoActa.AddRange(datos.PeriodoActaList);
                        db.SaveChanges();
                    }
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoActa.Find(datos.PeriodoActa.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodoActa.CandidatosInfoId;
                    a.fchIncio = datos.PeriodoActa.fchIncio;
                    a.fchFin = datos.PeriodoActa.fchFin;
                    a.dias = datos.PeriodoActa.dias;
                    a.Faltas = datos.PeriodoActa.Faltas;
                    a.Comentario = datos.PeriodoActa.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodoActa.UsuarioAltaId;

                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoActa.Find(datos.PeriodoActa.Id);
                    db.Entry(a).State = EntityState.Deleted;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeriodoSuspension - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDAsigBono(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    if (datos.PeriodoBonosList.Count() > 0)
                    {
                        foreach (var o in datos.PeriodoBonosList)
                        {
                            o.fchAlta = DateTime.Now;
                        }

                        db.PeriodoBonos.AddRange(datos.PeriodoBonosList);
                        db.SaveChanges();
                    }
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoBonos.Find(datos.PeriodosBonos.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodosBonos.CandidatosInfoId;
                    a.fchIncio = datos.PeriodosBonos.fchIncio;
                    a.fchFin = datos.PeriodosBonos.fchFin;
                    a.ConfigBonoId = datos.PeriodosBonos.ConfigBonoId;
                    a.Porcentaje = datos.PeriodosBonos.Porcentaje;
                    a.Comentario = datos.PeriodosBonos.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodosBonos.UsuarioAltaId;

                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoBonos.Find(datos.PeriodosBonos.Id);
                    db.Entry(a).State = EntityState.Deleted;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeriodoBonos - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDAsigComp(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    if (datos.PeriodoCompensacionesList.Count() > 0)
                    {
                        foreach (var o in datos.PeriodoCompensacionesList)
                        {
                            o.fchAlta = DateTime.Now;
                        }

                        db.PeriodoCompensaciones.AddRange(datos.PeriodoCompensacionesList);
                        db.SaveChanges();
                    }
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoCompensaciones.Find(datos.PeriodoCompensaciones.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodoCompensaciones.CandidatosInfoId;
                    a.Fecha = datos.PeriodoCompensaciones.Fecha;
                    a.Tipo = datos.PeriodoCompensaciones.Tipo;
                    a.Comentario = datos.PeriodoCompensaciones.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodoCompensaciones.UsuarioAltaId;

                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoCompensaciones.Find(datos.PeriodoCompensaciones.Id);
                    db.Entry(a).State = EntityState.Deleted;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeriodoCompensaciones - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDAsigHorasExtra(CRUDAsignacionesDtos datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    if (datos.PeriodoTiempoExtraList.Count() > 0)
                    {
                        foreach (var o in datos.PeriodoTiempoExtraList)
                        {
                            o.fchAlta = DateTime.Now;
                        }

                        db.PeriodoHorasExtras.AddRange(datos.PeriodoTiempoExtraList);
                        db.SaveChanges();
                    }
                }
                else if (datos.crud == 3)
                {
                    var a = db.PeriodoHorasExtras.Find(datos.PeriodoTiempoExtra.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.CandidatosInfoId = datos.PeriodoTiempoExtra.CandidatosInfoId;
                    a.fchIncio = datos.PeriodoTiempoExtra.fchIncio;
                    a.fchFin = datos.PeriodoTiempoExtra.fchFin;
                    a.Tiempo = datos.PeriodoTiempoExtra.Tiempo;
                    a.Comentario = datos.PeriodoTiempoExtra.Comentario;
                    a.fchAlta = DateTime.Now;
                    a.UsuarioAltaId = datos.PeriodoTiempoExtra.UsuarioAltaId;

                    db.SaveChanges();
                }
                else if (datos.crud == 4)
                {
                    var a = db.PeriodoHorasExtras.Find(datos.PeriodoTiempoExtra.Id);
                    db.Entry(a).State = EntityState.Deleted;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.PeriodoHorasExtras - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        #region CRUD
        [HttpPost]
        [Route("crudAsignacionesPersonal")]
        [Authorize]
        public IHttpActionResult CRUDConfiguracionIngresos(CRUDAsignacionesDtos datos)
        {
            try
            {
                if (datos.Catalogo.ToLower().Equals("vacaciones"))
                {
                    var result = this.CRUDAsigVacaciones(datos).StatusCode;

                    return Ok(result);
                }
                else if(datos.Catalogo.ToLower().Equals("dias"))
                {
                    var result = this.CRUDAsigDiasEconomicos(datos).StatusCode;
                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("incapacidad"))
                {
                    var result = this.CRUDAsigIncapacidades(datos).StatusCode;
                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("permisos"))
                {
                    var result = this.CRUDAsigPermisos(datos).StatusCode;
                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("guardia"))
                {
                    var result = this.CRUDAsigGuardia(datos).StatusCode;
                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("suspension"))
                {
                    var result = this.CRUDAsigSuspension(datos).StatusCode;
                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("tiempo_extra"))
                {
                    var result = this.CRUDAsigHorasExtra(datos).StatusCode;
                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("compensaciones"))
                {
                    var result = this.CRUDAsigComp(datos).StatusCode;
                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("bonos"))
                {
                    var result = this.CRUDAsigBono(datos).StatusCode;
                    return Ok(result);
                }
                else
                {
                    return Ok(HttpStatusCode.Continue);
                }
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        #endregion
        [HttpGet]
        [Route("getDatosContratados")]
        [Authorize]
        public IHttpActionResult GetDatosContratados(Guid clienteId)
        {
            try
            {
                var candidatos = db.ProcesoCandidatos.Where(x => x.Estatus.Descripcion.ToLower().Equals("contratado")
                && x.Requisicion.ClienteId.Equals(clienteId)).Select(C => C.CandidatoId).ToList();
                var datos = db.CandidatosInfo.Where(x => candidatos.Contains(x.CandidatoId))
                .Select(p => new
                {
                    id = p.Id,
                    vacante = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion)
                    .Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => new
                    {
                        v.RequisicionId,
                        v.Requisicion.SueldoMinimo,
                        v.Requisicion.SueldoMaximo,
                        v.Requisicion.Cliente.Nombrecomercial,
                        v.Requisicion.ClienteId,
                        v.Requisicion.VBtra
                    }).FirstOrDefault(),
                    candidatoId = p.CandidatoId,
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                    nom = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    p.Nombre,
                    apellidoPaterno = p.ApellidoPaterno,
                    apellidoMaterno = p.ApellidoMaterno,
                    fch_Ingreso = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(f => f.FechaIngreso).FirstOrDefault(),
                    edad = p.FechaNacimiento,
                    rfc = String.IsNullOrEmpty(p.RFC) ? "" : p.RFC,
                    curp = String.IsNullOrEmpty(p.CURP) ? "" : p.CURP,
                    nss = String.IsNullOrEmpty(p.NSS) ? "" : p.NSS,
                    genero = p.Genero.genero,
                    lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                    telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                    email = String.IsNullOrEmpty(db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault()) ? "SIN REGISTRO" : db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault(),
                    puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(v => v.PuestosIngresos.Nombre).FirstOrDefault(),
                    foto = db.DocumentosCandidato.Where(x => x.candidatoId.Equals(p.Id) && x.Documento.Descripcion.ToLower().Equals("foto")).Select(r => r.Ruta).FirstOrDefault(),
                    direccion = db.CandidatoGenerales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(d => new
                    {
                        direccion = d.Calle + " " + d.NumeroExterior + " " + d.Colonia.colonia + " " + d.Colonia.CP + " " + d.Municipio.municipio + " " + d.Estado.estado
                    }).FirstOrDefault()
                }).OrderBy(o => o.fch_Ingreso).ThenBy(oo => oo.apellidoPaterno).ToList();

              return Ok(datos);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getDatosGenerales")]
        [Authorize]
        public IHttpActionResult GetDtosGenerales(Guid candidatoId)
        {
            try
            {
                var datos = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(candidatoId)).Select(p => new
                {
                    id = p.Id,
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                    nom = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    nombre = p.Nombre,
                    apellidoPaterno = p.ApellidoPaterno,
                    apellidoMaterno = p.ApellidoMaterno,
                    edad = p.FechaNacimiento,
                    rfc = String.IsNullOrEmpty(p.RFC) ? "" : p.RFC,
                    curp = String.IsNullOrEmpty(p.CURP) ? "" : p.CURP,
                    nss = String.IsNullOrEmpty(p.NSS) ? "" : p.NSS,
                    paisNacimiento = p.PaisNacimientoId,
                    estadoNacimiento = p.estadoNacimiento.estado,
                    claveedo = p.estadoNacimiento.Clave,
                    municipioNacimiento = p.MunicipioNacimientoId,
                    localidad = p.municipioNacimiento.municipio + " / " + p.estadoNacimiento.estado,
                    genero = p.Genero.genero,
                    p.GeneroId,
                    lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                    telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                    email = String.IsNullOrEmpty(db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault()) ? "SIN REGISTRO" : db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault(),
                    direccion = db.PerfilCandidato
                    .Where(c => c.CandidatoId.Equals(candidatoId))
                    .Select(c => c.Candidato.direcciones).FirstOrDefault().Select(d => new {
                        d.EstadoId,
                        d.MunicipioId,
                        d.ColoniaId,
                        d.Calle,
                        d.NumeroExterior,
                        d.NumeroInterior,
                        d.Colonia.CP
                    }),
                    vacante = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(candidatoId)).Select(v => new
                    {
                        v.Requisicion.SueldoMinimo,
                        v.Requisicion.SueldoMaximo,
                        v.Requisicion.Cliente.Nombrecomercial,
                        v.Requisicion.ClienteId,
                        v.Requisicion.VBtra
                    }).FirstOrDefault()
                }).ToList();

                return Ok(datos);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getDatosGafetes")]
        [Authorize]
        public IHttpActionResult GetDtosGafetes()
        {
            try
            {
                var candidatos = db.Gafetes.Where(xx => xx.Activo).Select(g => g.CandidatoId).ToList();
                var datos = db.CandidatosInfo.Where(
                    x => candidatos.Contains(x.CandidatoId)
                && db.CandidatoLaborales.Select(ci => ci.CandidatoInfoId).ToList().Contains(x.Id))
                .Select(p => new
                {
                    id = p.Id,
                    requisicionId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(rr => rr.RequisicionId).FirstOrDefault(),
                    candidatoId = p.CandidatoId,
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                    nom = p.Nombre,
                    apellidoPaterno = p.ApellidoPaterno,
                    apellidoMaterno = p.ApellidoMaterno,
                    fch_Ingreso = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(f => f.FechaIngreso).FirstOrDefault(),
                    edad = p.FechaNacimiento,
                    rfc = String.IsNullOrEmpty(p.RFC) ? "" : p.RFC,
                    curp = String.IsNullOrEmpty(p.CURP) ? "" : p.CURP,
                    nss = String.IsNullOrEmpty(p.NSS) ? "" : p.NSS,
                    genero = p.Genero.genero,
                    lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                    telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                    email = String.IsNullOrEmpty(db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault()) ? "SIN REGISTRO" : db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault(),
                    puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(v => v.PuestosIngresos.Nombre).FirstOrDefault(),
                    foto = db.DocumentosCandidato.Where(x => x.candidatoId.Equals(p.Id) && x.Documento.Descripcion.ToLower().Equals("foto")).Select(r => r.Ruta).FirstOrDefault(),
                    direccion = db.CandidatoGenerales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(d => new
                    {
                        direccion = d.Calle + " " + d.NumeroExterior + " " + d.Colonia.colonia + " " + d.Colonia.CP + " " + d.Municipio.municipio + " " + d.Estado.estado
                    })
                }).OrderBy(o => o.fch_Ingreso).ThenBy(oo => oo.apellidoPaterno).ToList();

                var requis = datos.Select(c => c.requisicionId).Distinct().ToList();
                var clientes = db.Requisiciones.Where(x => requis.Distinct().Contains(x.Id)).Select(c => new
                {
                    requisicionId = c.Id,
                    c.Cliente.Id,
                    c.Cliente.Nombrecomercial,
                    c.Cliente.RazonSocial
                }).ToList();

                return Ok(
                    new { datos, clientes });

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getDatosGafetesByClave")]
        [Authorize]
        public IHttpActionResult GetDtosGafetes(string clave)
        {
            try
            {
                var id = db.Gafetes.Where(x => x.Clave.ToLower().Equals(clave.ToLower())).Select(d => d.CandidatoId).FirstOrDefault();
                var datos = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(id)
                && db.CandidatoLaborales.Select(ci => ci.CandidatoInfoId).ToList().Contains(x.Id))
                .Select(p => new
                {
                    id = p.Id,
                    candidatoId = p.CandidatoId,
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                    nom = p.Nombre,
                    apellidoPaterno = p.ApellidoPaterno,
                    apellidoMaterno = p.ApellidoMaterno,
                    fch_Ingreso = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(f => f.FechaIngreso).FirstOrDefault(),
                    edad = p.FechaNacimiento,
                    rfc = String.IsNullOrEmpty(p.RFC) ? "" : p.RFC,
                    curp = String.IsNullOrEmpty(p.CURP) ? "" : p.CURP,
                    nss = String.IsNullOrEmpty(p.NSS) ? "" : p.NSS,
                    genero = p.Genero.genero,
                    lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                    telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                    email = String.IsNullOrEmpty(db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault()) ? "SIN REGISTRO" : db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault(),
                    puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(v => v.PuestosIngresos.Nombre).FirstOrDefault(),
                    foto = db.DocumentosCandidato.Where(x => x.candidatoId.Equals(p.Id) && x.Documento.Descripcion.ToLower().Equals("foto")).Select(r => r.Ruta).FirstOrDefault()
                    //direccion = db.PerfilCandidato
                    //.Where(c => c.CandidatoId.Equals(p.CandidatoId))
                    //.Select(c => c.Candidato.direcciones).FirstOrDefault().Select(d => new {
                    //    d.EstadoId,
                    //    d.MunicipioId,
                    //    d.ColoniaId,
                    //    d.Calle,
                    //    d.NumeroExterior,
                    //    d.NumeroInterior,
                    //    d.Colonia.CP
                    //})
                }).OrderBy(o => o.nom);

                return Ok(datos);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [Route("agregarGafetes")]
        public IHttpActionResult AgregarGafetes(List<Gafetes> gafetes)
        {
            try
            {
                db.Gafetes.AddRange(gafetes);
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getDocumentos")]
        public IHttpActionResult GetDocumentos(Guid candidatoId)
        {
            try
            {
                var documentos = db.Documentos.Where(x => x.Activo && x.TipoDocumentoId.Equals(1)).Select(d => new {
                    id = d.Id,
                    descripcion = d.Descripcion,
                    activo = db.DocumentosCandidato.Where(x => x.candidatoId.Equals(candidatoId) && x.documentoId.Equals(d.Id)).Select(dd => dd.Id).Count() > 0 ? true : false,
                    fecha = db.DocumentosCandidato.Where(x => x.candidatoId.Equals(candidatoId) && x.documentoId.Equals(d.Id)).Select(f => f.fch_Creacion).FirstOrDefault()
                });
                return Ok(documentos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
        [HttpPost]
        [Route("actualizarDocumentos")]
        [Authorize]
        public IHttpActionResult ActualizarDocumentos(DocsDto dtos)
        {
            bool modificar = false;
            try
            {
                var val = db.DocumentosCandidato.Where(x => x.candidatoId.Equals(dtos.candidatoId) && x.documentoId.Equals(dtos.documentoId)).Select(d => d.Id).ToList();
                if (val.Count() == 0)
                {
                    DocumentosCandidato obj = new DocumentosCandidato();
                    obj.candidatoId = dtos.id; //candidatosinfoId
                    obj.documentoId = dtos.documentoId;
                    obj.usuarioId = dtos.usuarioId;
                    obj.Ruta = "/utilerias/Files/users/" + dtos.candidatoId + '/' + dtos.descripcion;
                    obj.fch_Modificacion = DateTime.Now;
                    obj.usuarioMod = dtos.usuarioId;
                    db.DocumentosCandidato.Add(obj);

                    db.SaveChanges();

                    return Ok(HttpStatusCode.OK);
                }
                else
                {
                    modificar = true;
                    var mod = db.DocumentosCandidato.Find(val);
                    db.Entry(mod).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(mod).Property(x => x.usuarioMod).IsModified = true;
                    mod.fch_Modificacion = DateTime.Now;
                    mod.usuarioMod = dtos.usuarioId;

                    return Ok(HttpStatusCode.OK);
                }

            }
            catch (Exception ex)
            {
                if (!modificar)
                {
                    FileManagerController fmc = new FileManagerController();
                    fmc.DeleteFiles("/utilerias/Files/users/" + dtos.candidatoId + '/' + dtos.descripcion);
                }
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }


    }
}
