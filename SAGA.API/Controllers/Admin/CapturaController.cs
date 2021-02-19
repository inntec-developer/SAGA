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
        Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
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
                var clienteId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(candidatoId))
                    .Select(c => c.Requisicion.ClienteId).FirstOrDefault();
                var requis = db.Requisiciones.Where(x => x.ClienteId.Equals(clienteId)).Select(r => r.Id).ToList();
                var tot = db.InformeRequisiciones.Where(x => requis.Distinct().Contains(x.RequisicionId) && x.Estatus.Descripcion.ToLower().Equals("contratado")).Count();
                var datos = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(candidatoId)).Select(p => new
                {
                    id = p.Id,
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                    nom = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                    clave = tot + 1,
                    foto = "img/Candidatos/" + p.CandidatoId + "/foto.jpg",
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
        [Route("getRegistroPatronal")]
        [Authorize]
        public IHttpActionResult GetRegistroPatronal(Guid sucursalId)
        {
            try
            {
                var datos = db.Sucursales.Where(x => x.Activo && x.Id.Equals(sucursalId)).Select(d => new
                {
                    id = d.RegistroPatronalId,
                    d.RegistroPatronal.RP_Clave,
                    d.RegistroPatronal.RP_IMSS,
                }).ToList();

                return Ok(datos);

            }
            catch (Exception)
            {

                return Ok(HttpStatusCode.BadRequest);
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
                    puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(v => v.PuestosIngresos).FirstOrDefault(),
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
        [HttpPost]
        [Route("agregarDatos")]
        [Authorize]
        public IHttpActionResult AgregarDatos(CapturaDto dtos)
        {
            CandidatosInfo ci = new CandidatosInfo();
            var idx = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(dtos.dtosPersonales.CandidatoId)).Select(c => c.Id).FirstOrDefault();
            if (idx != null)
            {
                using (DbContextTransaction beginTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var up = db.CandidatosInfo.Find(idx);
                        db.Entry(up).State = EntityState.Modified;

                        up.Nombre = dtos.dtosPersonales.Nombre;
                        up.ApellidoPaterno = dtos.dtosPersonales.ApellidoPaterno;
                        up.ApellidoMaterno = dtos.dtosPersonales.ApellidoMaterno;
                        up.RFC = dtos.dtosPersonales.RFC;
                        up.NSS = dtos.dtosPersonales.NSS;
                        up.CURP = dtos.dtosPersonales.CURP;
                        up.GeneroId = dtos.dtosPersonales.GeneroId;
                        up.FechaNacimiento = dtos.dtosPersonales.FechaNacimiento;
                        up.EstadoNacimientoId = dtos.dtosPersonales.EstadoNacimientoId;
                        up.fch_Modificacion = DateTime.Now;

                        dtos.dtosGenerales.CandidatoInfoId = idx;
                        dtos.dtosGenerales.PaisId = 42;
                        dtos.dtosLaborales.CandidatoInfoId = idx;
                        dtos.dtosExtras.CandidatoInfoId = idx;

                        var dg = db.CandidatoGenerales.Where(x => x.CandidatoInfoId.Equals(idx));
                        db.CandidatoGenerales.RemoveRange(dg);

                        var dl = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(idx));
                        db.CandidatoLaborales.RemoveRange(dl);

                        var dex = db.CandidatoExtras.Where(x => x.CandidatoInfoId.Equals(idx));
                        db.CandidatoExtras.RemoveRange(dex);

                        db.CandidatoGenerales.Add(dtos.dtosGenerales);
                        db.CandidatoLaborales.Add(dtos.dtosLaborales);
                        db.CandidatoExtras.Add(dtos.dtosExtras);

                        if (dtos.dtosPersonales.SoporteFacturacionId > 0)
                        {
                            EmpleadosSoporte esop = new EmpleadosSoporte();
                            esop.CandidatosInfoId = idx;
                            esop.SoporteFacturacionId = dtos.dtosPersonales.SoporteFacturacionId;
                            esop.Porcentaje = 0;
                            db.EmpleadosSoporte.Add(esop);
                        }

                        EmpleadoHorario obj = new EmpleadoHorario();
                        var aux = db.EmpleadoHorario.Where(x => x.empleadoId.Equals(idx)).Select(id => id.Id).FirstOrDefault();
                        if (aux != auxID && dtos.HorarioId != auxID)
                        {
                            var ue = db.EmpleadoHorario.Find(aux);
                            db.Entry(ue).Property(x => x.HorariosIngresosId).IsModified = true;
                            db.Entry(ue).Property(x => x.fch_Modificacion).IsModified = true;
                            db.Entry(ue).Property(x => x.UsuarioMod).IsModified = true;

                            obj.UsuarioMod = dtos.UsuarioId;
                            obj.HorariosIngresosId = dtos.HorarioId;
                            obj.fch_Modificacion = DateTime.Now;
                        }
                        else if (aux == auxID && dtos.HorarioId != auxID)
                        {
                            obj.empleadoId = idx;
                            obj.Activo = true;
                            obj.UsuarioAlta = dtos.UsuarioId;
                            obj.UsuarioMod = dtos.UsuarioId;
                            obj.HorariosIngresosId = dtos.HorarioId;
                            obj.fch_Creacion = DateTime.Now;
                            obj.fch_Modificacion = DateTime.Now;

                            db.EmpleadoHorario.Add(obj);

                        }

                        var idP = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(dtos.dtosPersonales.CandidatoId)).Select(x => x.Id).FirstOrDefault();
                        if (idP != auxID)
                        {
                            var c = db.ProcesoCandidatos.Find(idP);
                            db.Entry(c).Property(x => x.EstatusId).IsModified = true;
                            db.Entry(c).Property(x => x.Fch_Modificacion).IsModified = true;

                            c.EstatusId = db.Estatus.Where(x => x.Descripcion.ToLower().Equals("contratado")).Select(e => e.Id).FirstOrDefault();
                            c.Fch_Modificacion = DateTime.Now;

                        }
                        db.SaveChanges();

                        beginTran.Commit();
                        return Ok(HttpStatusCode.OK);
                    }
                    catch (Exception ex)
                    {
                        beginTran.Rollback();

                        apilog.WriteError(ex.Message);
                        apilog.WriteError(ex.InnerException.Message);
                        return Ok(HttpStatusCode.ExpectationFailed);
                    }
                }
            }
            else
            {
                apilog.WriteError("No se encuentra candidato en candidatosInfo");
                return Ok(HttpStatusCode.BadRequest);
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
        [HttpPost]
        [Route("agregarBiometricos")]
        public IHttpActionResult AgregarBiometricos(DtoBiometricos dtos)
        {
            try
            {
                var aux = db.BiometricosFP.Where(x => x.CandidatosInfoId.Equals(dtos.CandidatosInfoId)).Select(id => id.Id).FirstOrDefault();
                if (aux == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    BiometricosFP fp = new BiometricosFP();
                    fp.CandidatosInfoId = dtos.CandidatosInfoId;
                    fp.FingerPrint = dtos.FingerPrint;
                    fp.Activo = dtos.Activo;
                    fp.UsuarioAlta = dtos.UsuarioAlta;
                    fp.UsuarioMod = dtos.UsuarioMod;
                    fp.fch_Creacion = DateTime.Now;
                    fp.fch_Modificacion = DateTime.Now;

                    db.BiometricosFP.Add(fp);

                    db.SaveChanges();
                }
                else
                {
                    var mod = db.BiometricosFP.Find(aux);
                    db.Entry(mod).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(mod).Property(x => x.UsuarioMod).IsModified = true;
                    db.Entry(mod).Property(x => x.FingerPrint).IsModified = true;

                    mod.fch_Modificacion = DateTime.Now;
                    mod.UsuarioMod = dtos.UsuarioMod;
                    mod.FingerPrint = dtos.FingerPrint;

                    db.SaveChanges();
                }
                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("getFingerPrint")]
        public IHttpActionResult GetFingerPrint(Guid empleado)
        {
            try
            {
                var fingerprint = db.BiometricosFP.Where(x => x.CandidatosInfoId.Equals(empleado)).Select(emp => emp.FingerPrint).FirstOrDefault();
                return Ok(fingerprint);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }
        }
    }
}
