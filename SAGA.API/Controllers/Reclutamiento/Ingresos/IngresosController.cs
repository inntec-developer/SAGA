using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using SAGA.API.Utilerias;
using System.Data.Entity;

namespace SAGA.API.Controllers.Reclutamiento.Ingresos
{
    [RoutePrefix("api/reclutamiento/ingresos")]
    public class IngresosController : ApiController
    {
        private SAGADBContext db;

        public IngresosController()
        {
            db = new SAGADBContext();
        }
        [HttpGet]
        [Route("getDatosCubiertos")]
        [Authorize]
        public IHttpActionResult GetDtosCubiertos()
        {
            try
            {
                var id = db.ProcesoCandidatos.Where(x => x.Estatus.Descripcion.ToLower().Equals("cubierto")).
                    Select(c => c.CandidatoId).Distinct().ToList();

                var datos = db.CandidatosInfo.Where(x => id.Contains(x.CandidatoId))
                .Select(p => new
                {
                    p.Id,
                    candidatoId = p.CandidatoId,
                    nom = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    p.Nombre,
                    p.ApellidoPaterno,
                    p.ApellidoMaterno,
                    foto = "img/Candidatos/ " + p.CandidatoId + "/foto.jpg",
                    requisicionId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(c => c.RequisicionId).FirstOrDefault(),
                    fch_Creacion = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(c => c.Fch_Modificacion).FirstOrDefault(),
                    fch_Ingreso = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(c => c.Fch_Modificacion).FirstOrDefault(),
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                    edad = p.FechaNacimiento,
                    rfc = String.IsNullOrEmpty(p.RFC) ? "" : p.RFC,
                    curp = String.IsNullOrEmpty(p.CURP) ? "" : p.CURP,
                    nss = String.IsNullOrEmpty(p.NSS) ? "" : p.NSS,
                    p.Genero.genero,
                    generoId = p.Genero.Id,
                    lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                    telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                    email = String.IsNullOrEmpty(db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault()) ? "SIN REGISTRO" : db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault(),
                    folio = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.Folio).FirstOrDefault(),
                    vbtra = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.VBtra).FirstOrDefault(),
                    clienteId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.ClienteId).FirstOrDefault(),
                    nombrecomercial = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.Cliente.Nombrecomercial).FirstOrDefault(),
                    razonSocial = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.Cliente.RazonSocial).FirstOrDefault(),
                    direccion = db.Candidatos.Where(x => x.Id.Equals(p.CandidatoId)).Select(d => d.direcciones).FirstOrDefault().Select(c => c.Calle + " " + c.NumeroExterior + " " + c.Colonia.colonia + " " + c.Colonia.CP + " " + c.Municipio.municipio + " " + c.Estado.estado).FirstOrDefault(),
                    estado = p.estadoNacimiento.estado,
                    curpval = db.ValidacionCURPRFC.Where(x => x.CandidatosInfoId.Equals(p.Id)).Select(c => c.CURP).FirstOrDefault(),
                    rfcval = db.ValidacionCURPRFC.Where(x => x.CandidatosInfoId.Equals(p.Id)).Select(c => c.RFC).FirstOrDefault(),
                }).OrderBy(o => o.nom).ThenBy(f => f.fch_Creacion);

                return Ok(datos);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getDatosIngresos")]
        public IHttpActionResult GetDtosIngresos()
        {
            try
            {
                var id = db.ProcesoCandidatos.Where(x => x.Estatus.Descripcion.ToLower().Equals("ingreso")).
                    Select(c => c.CandidatoId).Distinct().ToList();

                var datos = db.CandidatosInfo.Where(x => id.Contains(x.CandidatoId))
                .Select(p => new
                {
                    p.Id,
                    candidatoId = p.CandidatoId,
                    nom = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    nombre = p.Nombre,
                    p.ApellidoPaterno,
                    p.ApellidoMaterno,
                    foto = "img/Candidatos/ " + p.CandidatoId + "/foto.jpg",
                    requisicionId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(c => c.RequisicionId).FirstOrDefault(),
                    fch_Creacion = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(c => c.Fch_Modificacion).FirstOrDefault(),
                    fch_Ingreso = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(c => c.Fch_Modificacion).FirstOrDefault(),
                    edad = p.FechaNacimiento,
                    rfc = String.IsNullOrEmpty(p.RFC) ? "" : p.RFC,
                    curp = String.IsNullOrEmpty(p.CURP) ? "" : p.CURP,
                    nss = String.IsNullOrEmpty(p.NSS) ? "" : p.NSS,
                    genero = p.Genero.genero,
                    lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                    telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                    email = String.IsNullOrEmpty(db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault()) ? "SIN REGISTRO" : db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault(),
                    folio = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.Folio).FirstOrDefault(),
                    vbtra = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.VBtra).FirstOrDefault(),
                    clienteId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.ClienteId).FirstOrDefault(),
                    nombrecomercial = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.Cliente.Nombrecomercial).FirstOrDefault(),
                    razonSocial = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.Cliente.RazonSocial).FirstOrDefault(),
                    direccion = db.Candidatos.Where(x => x.Id.Equals(p.CandidatoId)).Select(d => d.direcciones).FirstOrDefault().Select(c => c.Calle + " " + c.NumeroExterior + " " + c.Colonia.colonia + " " + c.Colonia.CP + " " + c.Municipio.municipio + " " + c.Estado.estado).FirstOrDefault(),
                    estado = p.estadoNacimiento.estado,
                    curpval = db.ValidacionCURPRFC.Where(x => x.CandidatosInfoId.Equals(p.Id)).Select(c => c.CURP).FirstOrDefault(),
                    rfcval = db.ValidacionCURPRFC.Where(x => x.CandidatosInfoId.Equals(p.Id)).Select(c => c.RFC).FirstOrDefault(),
                }).OrderBy(o => o.nombre).ThenBy(f => f.fch_Creacion);

                return Ok(datos);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getContratados")]
        public IHttpActionResult GetContratados()
        {
            try
            {

                var candidatos = db.ProcesoCandidatos.Where(x => x.Estatus.Descripcion.ToLower().Equals("contratado")).GroupBy(g => g.CandidatoId).Select(C => new
                {
                    info = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(C.OrderByDescending(o => o.Fch_Modificacion).FirstOrDefault().CandidatoId)).Select(p => new
                        {
                            requisicionId = C.OrderByDescending(o => o.Fch_Modificacion).FirstOrDefault().RequisicionId,
                            reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                            foto = @"https://apierp.damsa.com.mx/img/" +
                            db.Usuarios.Where(x => x.Id.Equals(p.CandidatoId)).Select(cc => cc.Clave).FirstOrDefault() + ".jpg",
                            id = p.Id,
                            candidatoId = p.CandidatoId,
                            nombre = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                            nom = p.Nombre,
                            apellidoPaterno = p.ApellidoPaterno,
                            apellidoMaterno = String.IsNullOrEmpty(p.ApellidoMaterno) ? "Sin registro" : p.ApellidoMaterno,
                            edad = p.FechaNacimiento,
                            rfc = String.IsNullOrEmpty(p.RFC) ? "Sin registro" : p.RFC,
                            curp = String.IsNullOrEmpty(p.CURP) ? "Sin registro" : p.CURP,
                            nss = String.IsNullOrEmpty(p.NSS) ? "Sin registro" : p.NSS,
                            paisNacimiento = p.PaisNacimientoId,
                            estadoNacimiento = p.EstadoNacimientoId,
                            municipioNacimiento = p.MunicipioNacimientoId,
                            localidad = p.municipioNacimiento.municipio + " / " + p.estadoNacimiento.estado,
                            generoId = p.GeneroId,
                            p.Genero.genero,
                            fch_Creacion = p.fch_Creacion,
                            fch_Modificacion = p.fch_Modificacion,
                            fch_Ingreso = p.fch_Modificacion,
                            lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                            telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                            direccion = db.Candidatos.Where(x => x.Id.Equals(p.CandidatoId)).Select(d => d.direcciones).FirstOrDefault().Select(c => c.Calle + " " + c.NumeroExterior + " " + c.Colonia.colonia + " " + c.Colonia.CP + " " + c.Municipio.municipio + " " + c.Estado.estado).FirstOrDefault(),
                            folio = db.Requisiciones.Where(x => x.Id.Equals(C.OrderByDescending(o => o.Fch_Modificacion).FirstOrDefault().RequisicionId)).Select(req => req.Folio).FirstOrDefault(),
                            vbtra = db.Requisiciones.Where(x => x.Id.Equals(C.OrderByDescending(o => o.Fch_Modificacion).FirstOrDefault().RequisicionId)).Select(req => req.VBtra).FirstOrDefault(),
                            clienteId = db.Requisiciones.Where(x => x.Id.Equals(C.OrderByDescending(o => o.Fch_Modificacion).FirstOrDefault().RequisicionId)).Select(cl => cl.Cliente.Id).FirstOrDefault(),
                            nombrecomercial = db.Requisiciones.Where(x => x.Id.Equals(C.OrderByDescending(o => o.Fch_Modificacion).FirstOrDefault().RequisicionId)).Select(cl => cl.Cliente.Nombrecomercial).FirstOrDefault(),
                            razonSocial = db.Requisiciones.Where(x => x.Id.Equals(C.OrderByDescending(o => o.Fch_Modificacion).FirstOrDefault().RequisicionId)).Select(cl => cl.Cliente.RazonSocial).FirstOrDefault()
                        }).OrderByDescending(o => o.nombre).ToList()
                }).ToList();

                return Ok(candidatos);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
        [HttpGet]
        [Route("getContratados2")]
        public IHttpActionResult GetContratados2()
        {
            try
            {

                var candidatos = db.ProcesoCandidatos.Where(x => x.Estatus.Descripcion.ToLower().Equals("contratado")).Select(C => C.CandidatoId).ToList();
                var info = db.CandidatosInfo.Where(x => candidatos.Contains(x.CandidatoId)).Select(p => new
                {
                    requisicionId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).FirstOrDefault().RequisicionId,
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                    foto = @"https://apierp.damsa.com.mx/img/" +
                            db.Usuarios.Where(x => x.Id.Equals(p.CandidatoId)).Select(cc => cc.Clave).FirstOrDefault() + ".jpg",
                    id = p.Id,
                    candidatoId = p.CandidatoId,
                    nombreCompleto = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    p.Nombre,
                    apellidoPaterno = p.ApellidoPaterno,
                    apellidoMaterno = String.IsNullOrEmpty(p.ApellidoMaterno) ? "Sin registro" : p.ApellidoMaterno,
                    p.FechaNacimiento,
                    rfc = String.IsNullOrEmpty(p.RFC) ? "Sin registro" : p.RFC,
                    curp = String.IsNullOrEmpty(p.CURP) ? "Sin registro" : p.CURP,
                    nss = String.IsNullOrEmpty(p.NSS) ? "Sin registro" : p.NSS,
                    paisNacimiento = p.PaisNacimientoId,
                    estadoNacimiento = p.EstadoNacimientoId,
                    municipioNacimiento = p.MunicipioNacimientoId,
                    localidad = p.municipioNacimiento.municipio + " / " + p.estadoNacimiento.estado,
                    generoId = p.GeneroId,
                    p.Genero.genero,
                    fch_Creacion = p.fch_Creacion,
                    fch_Modificacion = p.fch_Modificacion,
                    fch_Ingreso = p.fch_Modificacion,
                    lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                    telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                    direccion = db.Candidatos.Where(x => x.Id.Equals(p.CandidatoId)).Select(d => d.direcciones).FirstOrDefault().Select(c => c.Calle + " " + c.NumeroExterior + " " + c.Colonia.colonia + " " + c.Colonia.CP + " " + c.Municipio.municipio + " " + c.Estado.estado).FirstOrDefault(),
                    folio = db.Requisiciones.Where(x => x.Id.Equals(db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(xx => xx.CandidatoId.Equals(p.CandidatoId)).FirstOrDefault().RequisicionId)).Select(req => req.Folio).FirstOrDefault(),
                    vbtra = db.Requisiciones.Where(x => x.Id.Equals(db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(xx => xx.CandidatoId.Equals(p.CandidatoId)).FirstOrDefault().RequisicionId)).Select(req => req.VBtra).FirstOrDefault(),
                    clienteId = db.Requisiciones.Where(x => x.Id.Equals(db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(xx => xx.CandidatoId.Equals(p.CandidatoId)).FirstOrDefault().RequisicionId)).Select(cl => cl.Cliente.Id).FirstOrDefault(),
                    nombrecomercial = db.Requisiciones.Where(x => x.Id.Equals(db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(xx => xx.CandidatoId.Equals(p.CandidatoId)).FirstOrDefault().RequisicionId)).Select(cl => cl.Cliente.Nombrecomercial).FirstOrDefault(),
                    razonSocial = db.Requisiciones.Where(x => x.Id.Equals(db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(xx => xx.CandidatoId.Equals(p.CandidatoId)).FirstOrDefault().RequisicionId)).Select(cl => cl.Cliente.RazonSocial).FirstOrDefault()
                }).OrderByDescending(o => o.nombreCompleto).ToList();
        
                return Ok(info);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
        [Route("ingresarCandidato")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult IngresarCandidato(CandidatoLiberado lc)
        {
            try
            {
                Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");

                //foreach (var c in lc)
                //{
                var pc = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.RequisicionId.Equals(lc.RequisicionId) && x.CandidatoId.Equals(lc.CandidatoId)).Select(PC => PC.Id).FirstOrDefault();

                ProcesoCandidato ingreso = db.ProcesoCandidatos.Find(pc);
                db.Entry(ingreso).Property(x => x.Fch_Modificacion).IsModified = true;
                db.Entry(ingreso).Property(x => x.EstatusId).IsModified = true;
                ingreso.EstatusId = 50;
                ingreso.Fch_Modificacion = DateTime.Now;

                db.SaveChanges();

                //var ci = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(lc.CandidatoId)).Select(CI => CI.Id).FirstOrDefault();
                //CandidatosInfo C = db.CandidatosInfo.Find(ci);
                //C.ReclutadorId = lc.ReclutadorId;
                //C.fch_Modificacion = DateTime.Now;

                //db.SaveChanges();
                //}
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("contratarCandidatos")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult ContratarCandidatos(CandidatoLiberado lc)
        {
            try
            {
                Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");

                //foreach (var c in lc)
                //{
                    var pc = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.RequisicionId.Equals(lc.RequisicionId) && x.CandidatoId.Equals(lc.CandidatoId)).Select(PC => PC.Id).FirstOrDefault();

                    ProcesoCandidato contratar = db.ProcesoCandidatos.Find(pc);
                    db.Entry(contratar).State = EntityState.Modified;
                    contratar.EstatusId = 49;
           
                    db.SaveChanges();

                    var ci = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(lc.CandidatoId)).Select(CI => CI.Id).FirstOrDefault();
                    CandidatosInfo C = db.CandidatosInfo.Find(ci);
                    C.ReclutadorId = lc.ReclutadorId;
                    C.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();
                //}
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("liberarCandidatos")]
        [HttpPost]
        public IHttpActionResult LiberarCandidatos(LiberarCandidatoDto lc)
        {
            try
            {
                Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");

                //foreach (var c in lc)
                //{
                    var pc = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.RequisicionId.Equals(lc.RequisicionId) && x.CandidatoId.Equals(lc.CandidatoId)).Select(PC => PC.Id).FirstOrDefault();

                    ProcesoCandidato liberar = db.ProcesoCandidatos.Find(pc);
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

                //}
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("getTotales")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetTotales()
        {
            try
            {
                var totales = new
                {
                    total = db.ProcesoCandidatos.Where(x => x.Estatus.Descripcion.ToUpper().Equals("CUBIERTO")).Count(),
                    ingresos = db.ProcesoCandidatos.Where(x => x.Estatus.Descripcion.ToUpper().Equals("CONTRATADO")).Count()
                };

                return Ok(totales);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

        [Route("getHorarioById")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetHorarioById(Guid Id)
        {
            try
            {
                var horario = db.DiasHorasIngresos.Where(x => x.HorariosIngresosId.Equals(Id)).Select(h => new
                {
                    deDia = h.deDia.diaSemana,
                    aDia = h.aDia.diaSemana,
                    h.deDiaId,
                    h.aDiaId,
                    deHora = h.DeHora,
                    aHora = h.AHora,
                    h.Tipo,
                    h.Activo,
                    h.LimiteComida1,
                    h.LimiteComida2,
                    h.LimiteEntrada
                }).ToList();

                return Ok(horario);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("getNoValidados")]
        [Authorize]
        public IHttpActionResult GetNoValidados()
        {

            try
            {
                var candidatos = db.ValidacionCURPRFC.Where(x => x.CURP && x.RFC).Select(c => c.CandidatosInfoId).ToList();
                var contratados = db.CandidatosInfo.OrderByDescending(o => o.fch_Creacion).Where(x => !candidatos.Contains(x.Id)).Select(p => new
                {
                    id = p.Id,
                    candidatoId = p.CandidatoId,
                    foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(p.CandidatoId)).Select(c => c.Clave).FirstOrDefault() + ".jpg",
                    nom = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    nombre = p.Nombre == null ? "" : p.Nombre,
                    apellidoPaterno = p.ApellidoPaterno,
                    apellidoMaterno = String.IsNullOrEmpty(p.ApellidoMaterno) ? "Sin registro" : p.ApellidoMaterno,
                    edad = p.FechaNacimiento,
                    rfc = p.RFC,
                    curp = p.CURP,
                    nss = p.NSS,
                    paisNacimiento = p.PaisNacimientoId,
                    estadoNacimiento = p.EstadoNacimientoId,
                    claveedo = p.estadoNacimiento.Clave,
                    estado = p.estadoNacimiento.estado,
                    municipioNacimiento = p.MunicipioNacimientoId,
                    localidad = p.municipioNacimiento.municipio + " / " + p.estadoNacimiento.estado,
                    generoId = p.GeneroId,
                    genero = p.Genero.genero,
                    fch_Creacion = p.fch_Creacion,
                    fch_Modificacion = p.fch_Modificacion, 
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault()
                }).ToList();

                return Ok(contratados);

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        [Route("validarCURPRFC")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult ValidarCURPRFC(ValidacionCURPRFC datos)
        {
            try
            {
                var id = db.ValidacionCURPRFC.Where(x => x.CandidatosInfoId.Equals(datos.CandidatosInfoId)).Select(d => d.Id).FirstOrDefault();
                if (id != new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    var c = db.ValidacionCURPRFC.Find(id);

                    db.Entry(c).Property(x => x.CURP).IsModified = true;
                    db.Entry(c).Property(x => x.RFC).IsModified = true;
                    db.Entry(c).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(c).Property(x => x.UsuarioMod).IsModified = true;

                    c.CURP = datos.CURP;
                    c.RFC = datos.RFC;
                    c.fch_Modificacion = DateTime.Now;
                    c.UsuarioMod = datos.UsuarioMod;

                    db.SaveChanges();

                }
                else
                {
                    ValidacionCURPRFC obj = new ValidacionCURPRFC();

                    obj.fch_Modificacion = DateTime.Now;

                    db.ValidacionCURPRFC.Add(obj);

                    db.SaveChanges();
                }
                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }
        }
    }
}
