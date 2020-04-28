using SAGA.API.Dtos;
using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Web;
using System.IO;
using SAGA.API.Utilerias;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/admin")]
    public class CapturaController : ApiController
    {
        private SAGADBContext db;
        APISAGALog apilog = new APISAGALog();
        public CapturaController()
        {
            db = new SAGADBContext();
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
                    nombre = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    nom = p.Nombre,
                    apellidoPaterno = p.ApellidoPaterno,
                    apellidoMaterno = p.ApellidoMaterno,
                    edad = p.FechaNacimiento,
                    rfc = String.IsNullOrEmpty(p.RFC) ? "" : p.RFC,
                    curp = String.IsNullOrEmpty(p.CURP) ? "" : p.CURP,
                    nss = String.IsNullOrEmpty(p.NSS) ? "" : p.NSS,
                    paisNacimiento = p.PaisNacimientoId,
                    estadoNacimiento = p.estadoNacimiento.estado,
                    municipioNacimiento = p.MunicipioNacimientoId,
                    localidad = p.municipioNacimiento.municipio + " / " + p.estadoNacimiento.estado,
                    genero = p.GeneroId,
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
                    vacante = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(candidatoId) && x.EstatusId.Equals(24)).Select(v => new
                    {
                        v.Requisicion.SueldoMinimo,
                        v.Requisicion.SueldoMaximo
                    }).FirstOrDefault()
                });

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
                var datos = db.CandidatosInfo.Where(x => !db.GaFETES.Where(xx => xx.Activo).Select(g => g.CandidatoId).ToList().Contains(x.CandidatoId)
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
                    puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(v => v.Puesto).FirstOrDefault(),
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
                }).OrderBy(o => o.fch_Ingreso).ThenBy(oo => oo.apellidoPaterno).ToList();

                return Ok(datos);

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
                var id = db.GaFETES.Where(x => x.Clave.ToLower().Equals(clave.ToLower())).Select(d => d.CandidatoId).FirstOrDefault();
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
                    puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(v => v.Puesto).FirstOrDefault(),
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
        [HttpGet]
        [Route("getDatosIngresos")]
        public IHttpActionResult GetDtosIngresos()
        {
            try
            {
                var id = db.GaFETES.Select(d => d.CandidatoId).ToList();
                var datos = db.CandidatosInfo.Where(x => id.Contains(x.CandidatoId)
                && db.CandidatoLaborales.Select(ci => ci.CandidatoInfoId).ToList().Contains(x.Id))
                .Select(p => new
                {
                    id = p.Id,
                    candidatoId = p.CandidatoId,
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                    nombre = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    fch_Creacion = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(f => f.FechaIngreso).FirstOrDefault(),
                    edad = p.FechaNacimiento,
                    rfc = String.IsNullOrEmpty(p.RFC) ? "" : p.RFC,
                    curp = String.IsNullOrEmpty(p.CURP) ? "" : p.CURP,
                    nss = String.IsNullOrEmpty(p.NSS) ? "" : p.NSS,
                    genero = p.Genero.genero,
                    lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                    telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                    email = String.IsNullOrEmpty(db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault()) ? "SIN REGISTRO" : db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault(),
                    folio = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.Folio).FirstOrDefault(),
                    vbtra = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(v => v.Puesto).FirstOrDefault(),
                    clienteId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.ClienteId ).FirstOrDefault(),
                    nombrecomercial = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.Cliente.Nombrecomercial).FirstOrDefault(),
                    razonSocial = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => v.Requisicion.Cliente.RazonSocial).FirstOrDefault(),
                   
                }).OrderByDescending(o => o.fch_Creacion).ThenBy(t => t.nombre);

                return Ok(datos);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getTotales")]
        public IHttpActionResult GetTotales()
        {
            try
            {
                var totales = new
                {
                    total = db.CandidatosInfo.Where(x => !db.CandidatoLaborales.Select(cinf => cinf.CandidatoInfoId).ToList().Contains(x.Id)).Count(),
                    ingresos = db.CandidatosInfo.Where(x => db.CandidatoLaborales.Select(cinf => cinf.CandidatoInfoId).ToList().Contains(x.Id)).Count(),
                };

                return Ok(totales);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [Route("agregarDatos")]
        [Authorize]
        public IHttpActionResult AgregarDatos(CapturaDto dtos)
        {
            try
            {
                
                CandidatosInfo ci = new CandidatosInfo();
                var idx = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(dtos.dtosPersonales.CandidatoId)).Select(c => c.Id).FirstOrDefault();
                if (idx != null)
                {
                    var up = db.CandidatosInfo.Find(idx);
                    //db.Entry(up).Property(x => x.RFC).IsModified = true;
                    //db.Entry(up).Property(x => x.NSS).IsModified = true;
                    db.Entry(up).Property(x => x.fch_Modificacion).IsModified = true;

                    //ci.RFC = dtos.dtosPersonales.RFC;
                    //ci.NSS = dtos.dtosPersonales.NSS;
                    ci.fch_Modificacion = DateTime.Now;

                    dtos.dtosGenerales.CandidatoInfoId = idx;
                    dtos.dtosGenerales.PaisId = 42;
                    dtos.dtosLaborales.CandidatoInfoId = idx;
                    dtos.dtosExtras.CandidatoInfoId = idx;

                    db.CandidatoGenerales.Add(dtos.dtosGenerales);
                    db.CandidatoLaborales.Add(dtos.dtosLaborales);
                    db.CandidatoExtras.Add(dtos.dtosExtras);

                    db.SaveChanges();
                    return Ok(HttpStatusCode.OK);

                }
               
                apilog.WriteError("No se encunetra candidato en candidatosInfo");
                return Ok(HttpStatusCode.BadRequest);

            }
            catch (Exception ex)
            {
                apilog.WriteError(ex.Message);
                apilog.WriteError(ex.InnerException.Message);
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [Route("agregarGafetes")]
        public IHttpActionResult AgregarGafetes(List<Gafetes> gafetes)
        {
            try
            {
                db.GaFETES.AddRange(gafetes);
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
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
                    fecha = db.DocumentosCandidato.Where(x => x.candidatoId.Equals(candidatoId) && x.documentoId.Equals(d.Id)).Select( f => f.fch_Creacion).FirstOrDefault()
                });
                return Ok(documentos);
            }
            catch( Exception ex)
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
            catch(Exception ex)
            {
                if(!modificar)
                {
                    FileManagerController fmc = new FileManagerController();
                    fmc.DeleteFiles("/utilerias/Files/users/" + dtos.candidatoId + '/' + dtos.descripcion);
                }
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
    }
}
