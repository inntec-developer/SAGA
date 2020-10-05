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
        [Route("getDatosIngresos")]
        public IHttpActionResult GetDtosIngresos()
        {
            try
            {
                var id = db.ProcesoCandidatos.Where(x => x.Estatus.Descripcion.ToLower().Equals("cubierto")).
                    Select(c => c.CandidatoId).Distinct().ToList();

                var datos = db.CandidatosInfo.Where(x => id.Contains(x.CandidatoId))
                .Select(p => new
                {
                    candidatoId = p.CandidatoId,
                    nombre = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    nom = p.Nombre,
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
                    estado = p.estadoNacimiento.estado
                }).OrderByDescending(o => o.fch_Creacion).ThenBy(t => t.nombre);

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
                        }).ToList()
                }).ToList();

                return Ok(candidatos);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        [Route("contratarCandidatos")]
        [HttpPost]
        public IHttpActionResult ContratarCandidatos(List<CandidatoLiberado> lc)
        {
            try
            {
                Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");

                foreach (var c in lc)
                {
                    var pc = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.RequisicionId.Equals(c.RequisicionId) && x.CandidatoId.Equals(c.CandidatoId)).Select(PC => PC.Id).FirstOrDefault();

                    ProcesoCandidato contratar = db.ProcesoCandidatos.Find(pc);
                    db.Entry(contratar).State = EntityState.Modified;
                    contratar.EstatusId = 49;
           
                    db.SaveChanges();

                    var ci = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(CI => CI.Id).FirstOrDefault();
                    CandidatosInfo C = db.CandidatosInfo.Find(ci);
                    C.ReclutadorId = c.ReclutadorId;
                    C.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();
                }
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
        public IHttpActionResult LiberarCandidatos(List<LiberarCandidatoDto> lc)
        {
            try
            {
                Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");

                foreach (var c in lc)
                {
                    var pc = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.RequisicionId.Equals(c.RequisicionId) && x.CandidatoId.Equals(c.CandidatoId)).Select(PC => PC.Id).FirstOrDefault();

                    ProcesoCandidato liberar = db.ProcesoCandidatos.Find(pc);
                    db.Entry(liberar).State = EntityState.Modified;
                    liberar.EstatusId = 27;

                    CandidatoLiberado cl = new CandidatoLiberado();
                    cl.RequisicionId = c.RequisicionId;
                    cl.CandidatoId = c.CandidatoId;
                    cl.ReclutadorId = c.ReclutadorId;
                    cl.MotivoId = c.MotivoId;
                    cl.Comentario = c.Comentario;

                    db.CandidatosLiberados.Add(cl);

                    db.SaveChanges();

                }
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
    }
}
