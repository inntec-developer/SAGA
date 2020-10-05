using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using System.Data.SqlClient;
using SAGA.API.Dtos.SistTickets;
using System.Net.Mail;
using Infobip.Api.Model.Sms.Mt.Send;
using Infobip.Api.Model.Sms.Mt.Send.Textual;
using Infobip.Api.Client;
using Infobip.Api.Config;
using System.Configuration;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using SAGA.API.Utilerias;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Campo")]
    public class CampoController : ApiController
    {
        private SAGADBContext db;
        public Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
        int[] estatusList = new int[] { 4, 8, 9, 34, 35, 36, 37, 43, 46, 47, 48 };

        public CampoController()
        {
            db = new SAGADBContext();
        }

        public void GuardarImagen(string nombre, string id, string file, string type)
        {
            string x = file.Replace("data:image/jpeg;base64,", "");
            byte[] imageBytes = Convert.FromBase64String(x);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/Candidatos");
            if (!Directory.Exists(fullPath + '/' + id))
            {
                Directory.CreateDirectory(fullPath + '/' + id);
            }

           fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/Candidatos/" + id + '/' + nombre);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            image.Save(fullPath);
        }

        [HttpGet]
        [Route("DescargarApp")]
        public IHttpActionResult DescargarApp()
        {
            try
            {
                Utilerias.AppDownload obj = new Utilerias.AppDownload();
               var mocos =  obj.DownloadFiles("Utilerias/CampoApp/");           

                return Ok(mocos);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost]
        [Route("login")]
        public IHttpActionResult LogIn(LogIn logIn)
        {
            try
            {
                UsuarioDto userData = new UsuarioDto();
                LoginValidation loginValidation = new LoginValidation();
                object[] _LoginSaga = {
                        new SqlParameter("@Password", logIn.password),
                        new SqlParameter("@Email", logIn.email),
                    };

                var Data = db.Database.SqlQuery<UsuarioDto>("exec sp_LoginSagaERP @Email, @Password", _LoginSaga).FirstOrDefault();
                if (Data != null)
                {
                    object[] _params = {
                        new SqlParameter("@CLAVE", Data.Clave)
                    };
                    /*
                     * Verifica que el usario que intenta ingresar al sistea un se sea empleado de DAMSA.
                     */
                    var activo = db.Database.SqlQuery<Int32>("exec sp_ValidatorLogin @CLAVE", _params).FirstOrDefault();
                    if (activo > 0 && Data.Activo)
                    {
                        userData.Id = Data.Id;
                        userData.Nombre = Data.Nombre;
                        userData.Usuario = Data.Usuario;
                        userData.Email = Data.Email;
                        userData.Clave = Data.Clave;
                        userData.TipoUsuarioId = Data.TipoUsuarioId;
                        userData.Tipo = Data.Tipo;
                        userData.Sucursal = Data.Sucursal;
                        userData.UnidadNegocioId = Data.UnidadNegocioId;
                        userData.DepartamentoId = Data.DepartamentoId;
                        userData.Departamento = Data.Departamento;
                   
                        var token = TokenGenerator.GenerateTokenJwt(userData);
                        var dataUser = TokenGenerator.GenerateTokenUser(userData);

                        ReturnLogIn returnLogIn = new ReturnLogIn();
                        returnLogIn.Token = token;
                        returnLogIn.DataUser = dataUser;

                        return Ok(returnLogIn);
                    }
                    else
                    {
                        return Ok(HttpStatusCode.NotAcceptable);
                    }
                }
                else
                {
                    return Ok(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("registrarCandidato")]
        [Authorize]
        public IHttpActionResult RegistrarCandidato(CandidatosGralDto datos)
        {
            LoginDto login = new LoginDto();
            AspNetUsers usuario = new AspNetUsers();
            ProcesoCandidato proceso = new ProcesoCandidato();
            Candidato candidato = new Candidato();
            var username = "";
            var tran = db.Database.BeginTransaction();
            try
            {

                candidato.CURP = datos.Curp;
                candidato.Nombre = datos.Nombre;
                candidato.ApellidoPaterno = datos.ApellidoPaterno;
                candidato.ApellidoMaterno = datos.ApellidoMaterno;
                candidato.emails = datos.Email;
                candidato.PaisNacimientoId = 42;
                candidato.EstadoNacimientoId = datos.EstadoNacimientoId;
                candidato.MunicipioNacimientoId = datos.MunicipioNacimientoId;
                candidato.telefonos = datos.Telefono;
                candidato.GeneroId = datos.GeneroId;
                candidato.TipoEntidadId = 2;
                candidato.FechaNacimiento = datos.FechaNac;

                db.Candidatos.Add(candidato);

                db.SaveChanges();
                try
                {
                    if (datos.Credencial.Length > 0)
                    {
                        this.GuardarImagen("credencial.jpg", candidato.Id.ToString(), datos.Credencial, "jpeg");
                    }
                    if(datos.Foto.Length > 0)
                    {
                        this.GuardarImagen("foto.jpg", candidato.Id.ToString(), datos.Foto, "jpeg");
                    }

                }
                catch(Exception ex)
                {
                    APISAGALog obj = new APISAGALog();
                    obj.WriteError(ex.Message);
                    obj.WriteError(ex.InnerException.Message);
                }

                PerfilCandidato PC = new PerfilCandidato();

                PC.CandidatoId = candidato.Id;
                PC.Estatus = 41;
               
                db.PerfilCandidato.Add(PC);

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
                proceso.Reclutador = db.Usuarios.Where(x => x.Id.Equals(datos.reclutadorId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault();
                proceso.ReclutadorId = datos.reclutadorId;
                proceso.EstatusId = 12;
                proceso.TpContrato = 0;
                proceso.HorarioId = horario;
                proceso.Fch_Modificacion = DateTime.Now;
                proceso.DepartamentoId = new Guid("d89bec78-ed5b-4ac5-8f82-24565ff394e5");
                proceso.TipoMediosId = datos.tipoMediosId;

                db.ProcesoCandidatos.Add(proceso);
                db.SaveChanges();

                ProcesoCampo PRC = new ProcesoCampo();

                PRC.RequisicionId = datos.requisicionId;
                PRC.UsuarioId = datos.reclutadorCampoId;
                PRC.CandidatoId = candidato.Id;
                PRC.ReclutadorId = datos.reclutadorId;

                db.ProcesoCampo.Add(PRC);

                db.SaveChanges();

                tran.Commit();

                var pass = datos.Nombre.Substring(0, 1).ToLower() + datos.ApellidoPaterno.Trim().ToLower();
                if (datos.OpcionRegistro == 1)
                {
                    username = datos.Email[0].email.ToString();
                }
                else
                {
                    username = datos.Telefono[0].ClaveLada.ToString() + datos.Telefono[0].telefono.ToString();
                }
                usuario.Id = Guid.NewGuid().ToString();
                usuario.PhoneNumber = datos.Telefono == null ? "00000000" : datos.Telefono[0].ClaveLada.ToString() + datos.Telefono[0].telefono.ToString();
                usuario.Clave = "00000";
                usuario.Pasword = pass;
                usuario.RegistroClave = DateTime.Now;
                usuario.PhoneNumberConfirmed = false;
                usuario.EmailConfirmed = true;
                usuario.LockoutEnabled = false;
                usuario.AccessFailedCount = 0;
                usuario.Email = datos.Email == null ? "SIN REGISTRO" : datos.Email[0].email.ToString();
                usuario.UserName = username;
                usuario.Activo = 0;
                usuario.IdPersona = candidato.Id;

                db.AspNetUsers.Add(usuario);
                db.SaveChanges();

                var add = db.Database.ExecuteSqlCommand("spEncriptarPasword @id", new SqlParameter("id", usuario.Id));

                login.Id = candidato.Id;
                login.username = username;
                login.pass = pass;

                candidato = null;
                PC = null;
                proceso = null;
                PRC = null;

                return Ok(login);
            }
            catch (Exception ex)
            {
                if(!String.IsNullOrEmpty(username))
                {
                    return Ok(HttpStatusCode.BadGateway);
                } 
                else
                {
                    tran.Rollback();
                    return Ok(HttpStatusCode.ExpectationFailed);
                }
            }
        }

        [HttpGet]
        [Route("getReclutadoresCampo")]
        [Authorize]
        public IHttpActionResult GetReclutadoresCampo()
        {
            try
            {
                var persona = db.Usuarios.Where(x => x.TipoUsuarioId.Equals(15)).Select(u => new {
                    EntidadId = u.Id,
                    Foto = u.Foto,
                    Clave = u.Clave,
                    nombre = u.Nombre,
                    apellidoPaterno = u.ApellidoPaterno,
                    apellidoMaterno = u.ApellidoMaterno,
                    tipoUsuario = u.TipoUsuario.Tipo,
                    tipoUsuarioId = u.TipoUsuario.Id,
                    Usuario = u.Usuario,
                    Departamento = u.Departamento.Nombre,
                    DepartamentoId = u.Departamento.Id,
                    Email = u.emails.Select(e => e.email).FirstOrDefault(),
                    liderId = db.Subordinados.Where(x => x.UsuarioId.Equals(u.Id)).Select(L => L.LiderId).FirstOrDefault(),
                    nombreLider = db.Usuarios
                       .Where(x => x.Id.Equals(db.Subordinados.Where(xx => xx.UsuarioId.Equals(u.Id)).Select(L => L.LiderId).FirstOrDefault()))
                       .Select(L => L.Nombre + " " + L.ApellidoPaterno + " " + L.ApellidoMaterno).FirstOrDefault() == null ? "SIN ASIGNAR" : db.Usuarios
                                                                                                     .Select(L => L.Nombre + " " + L.ApellidoPaterno + " " + L.ApellidoMaterno).FirstOrDefault(),
                    oficinaId = u.SucursalId,
                    oficina = u.Sucursal.Nombre,
                    activo = u.Activo ? 1 : 0

            }).OrderBy(o => o.nombre).ToList();

            return Ok(persona);
            }
            catch (Exception)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getReclutadores")]
        [Authorize]
        public IHttpActionResult GetReclutadores()
        {
           
            try
            {
                var reclutadores = db.AsignacionRequis.Where(x => x.Requisicion.Activo && !estatusList.Contains(x.Requisicion.EstatusId)
                && !x.Requisicion.Confidencial && x.Requisicion.ClaseReclutamientoId != 1).GroupBy(g => g.GrpUsrId)
                    .Select(R => new
                    {
                        oficinaId = db.Usuarios.Where(x => x.Id.Equals(R.Key)).Select(ofi => ofi.SucursalId).FirstOrDefault(),
                        oficina = db.Usuarios.Where(x => x.Id.Equals(R.Key)).Select(ofi => db.OficinasReclutamiento.Where(x => x.Id.Equals(ofi.SucursalId)).Select(n => n.Nombre).FirstOrDefault()).FirstOrDefault(),
                        nombre = db.Usuarios.Where(x => x.Id.Equals(R.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        foto = db.Usuarios.Where(x => x.Id.Equals(R.Key)).Select(n => n.Foto).FirstOrDefault(),
                        clave = db.Usuarios.Where(x => x.Id.Equals(R.Key)).Select(c => c.Clave).FirstOrDefault(),
                        reclutadorId = R.Key,
                        requis = R.Select(r => new {
                            requisicionId = r.Requisicion.Id,
                            cliente = r.Requisicion.Cliente.Nombrecomercial,
                            vBtra = r.Requisicion.VBtra,
                        }).ToList()
                    });
                var data = reclutadores.GroupBy(x => x.oficinaId).Select(R => new
                {
                    oficinaId = R.Key,
                    oficina = R.Select(n => n.oficina).FirstOrDefault(),
                    reclutadores = R.Select(r => new
                    {
                        reclutadorId = r.reclutadorId,
                        nombre = r.nombre,
                        foto = String.IsNullOrEmpty(r.foto) ? @"https://apierp.damsa.com.mx/utilerias/img/user/default.jpg" : @"https://apierp.damsa.com.mx/img/" + r.clave + ".jpg",
                        requis = r.requis.Count(),
                        requisiciones = r.requis
                    })

                }).ToList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
     
        [HttpGet]
        [Route("getRequisReclutadores")]
        [Authorize]
        public IHttpActionResult GetRequiReclutadores(Guid reclutadorId, Guid reclutadorCampoId)
        {
            try
            {
                var reclutadores = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(reclutadorId) 
                && x.Requisicion.Activo && !estatusList.Contains(x.Requisicion.EstatusId)
                && !x.Requisicion.Confidencial
                && x.Requisicion.ClaseReclutamientoId != 1
                && x.Tipo == 2)
                    .Select(r => new
                    {
                        r.Requisicion.Id,
                        r.Requisicion.Folio,
                        r.Requisicion.VBtra,
                        cliente = r.Requisicion.Cliente.Nombrecomercial,
                        r.Requisicion.fch_Cumplimiento,
                        Vacantes = r.Requisicion.horariosRequi.Count() > 0 ? r.Requisicion.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(r.Requisicion.Id) && p.EstatusId.Equals(24)).Count(),
                        EnProceso = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(p => p.RequisicionId.Equals(r.Requisicion.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                        MiProceso = db.ProcesoCampo.Where(x => x.RequisicionId.Equals(r.RequisicionId) && x.UsuarioId.Equals(reclutadorCampoId) && x.ReclutadorId.Equals(reclutadorId)
                         ).Select(cc => cc.CandidatoId).ToList().Where(xx => db.ProcesoCandidatos.Where(xxx => xxx.CandidatoId.Equals(xx) && xxx.EstatusId.Equals(12)).Count() > 0).Count()
                    }).ToList();
     
                return Ok(reclutadores);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("getUnidadesNegocios")]
        [Authorize]
        public IHttpActionResult GetUnidadesNegocio()
        {
            try
            {
                var unidades = db.UnidadesNegocios.Where(x => x.Activo).ToArray();
                return Ok(unidades);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getReclutadoresByUnidades")]
        [Authorize]
        public IHttpActionResult GetReclutadoresByUnidades(int id)
        {
            try
            {
                var unidades = db.OficinasReclutamiento.
                    Where(x => db.UnidadNegocioEstados.Where(xx => xx.unidadnegocioId.Equals(id)).Select(ee => ee.estadoId)
                    .ToList().Contains(x.estadoId)).Select(R => new
                {
                    oficinaId = R.Id,
                    oficina = R.Nombre,
                    reclutadores = db.Usuarios.Where(x => x.SucursalId.Equals(R.Id)).Select(n => new
                    {
                        reclutadorId = n.Id,
                        nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                        foto = String.IsNullOrEmpty(n.Foto) ? @"https://apierp.damsa.com.mx/utilerias/img/user/default.jpg" : @"https://apierp.damsa.com.mx/img/" + n.Clave + ".jpg",
                        requis = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(n.Id) && x.Requisicion.Activo && !estatusList.Contains(x.Requisicion.EstatusId) && !x.Requisicion.Confidencial).Count()
                    }).ToList().Where(rr => rr.requis > 0)
                }).ToList().Where(rr => rr.reclutadores.Count() > 0).ToList();
                return Ok(unidades);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getCandidatosProceso")]
        [Authorize]
        public IHttpActionResult GetCandidatosProceso(Guid requisicionId, Guid reclutadorId, Guid reclutadorCampoId)
        {
            try
            {
                var cand = db.ProcesoCampo.Where(x => x.UsuarioId.Equals(reclutadorCampoId) & x.RequisicionId.Equals(requisicionId) && x.ReclutadorId.Equals(reclutadorId)).Select(c => c.CandidatoId).ToList();
                var candidatos = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => cand.Contains(x.CandidatoId) & x.EstatusId.Equals(12) ).Select(c => new
                {
                    procesoId = c.Id,
                    candidatoId = c.CandidatoId,
                    horarioId = c.HorarioId,
                    horario = db.HorariosRequis.Where(x => x.Id.Equals(c.HorarioId)).Select(h => h.Nombre + " de " + h.deHora.Hour + " a " + h.aHora.Hour).FirstOrDefault(),
                    vacantes = db.HorariosRequis.Where(x => x.RequisicionId.Equals(c.RequisicionId)).Count() > 0 ? db.HorariosRequis.Where(x => x.RequisicionId.Equals(c.RequisicionId)).Sum(h => h.numeroVacantes) : 0,
                    propietarioId = c.Requisicion.PropietarioId,
                    estatusId = c.EstatusId,
                    informacion = db.Candidatos.Where(x => x.Id.Equals(c.CandidatoId) && c.EstatusId.Equals(12)).Select(cc => new {
                        foto = @"https://apisb.damsa.com.mx/utilerias/img/Candidatos/" + c.CandidatoId + "/foto.jpg",
                        credencial = @"https://apisb.damsa.com.mx/utilerias/img/Candidatos/" + c.CandidatoId + "/credencial.jpg",
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
                }).ToList();

                return Ok(candidatos);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getCandidatosApartados")]
        [Authorize]
        public IHttpActionResult GetCandidatosApartados(Guid requisicionId, Guid reclutadorId)
        {
            try
            {
                var candidatos = db.ProcesoCampo.OrderByDescending(f => f.Fch_Creacion).Where(x => x.RequisicionId.Equals(requisicionId) && x.UsuarioId.Equals(reclutadorId)).Select(c => new
                {
                    procesoId = c.Id,
                    candidatoId = c.CandidatoId,
                    foto = @"https://apisb.damsa.com.mx/utilerias/img/Candidatos/" + c.CandidatoId + "/foto.jpg",
                    requisiconId = requisicionId,
                    vacantes = db.HorariosRequis.Where(x => x.RequisicionId.Equals(c.RequisicionId)).Count() > 0 ? db.HorariosRequis.Where(x => x.RequisicionId.Equals(c.RequisicionId)).Sum(h => h.numeroVacantes) : 0,
                    curp = String.IsNullOrEmpty(c.Candidato.CURP) ? "Sin registro" : c.Candidato.CURP,
                    nombreCompleto = c.Candidato.Nombre + " " + c.Candidato.ApellidoPaterno + " " + c.Candidato.ApellidoMaterno,
                    nombre = c.Candidato.Nombre,
                    apellidoPaterno = c.Candidato.ApellidoPaterno,
                    apellidoMaterno = c.Candidato.ApellidoMaterno,
                    edad = c.Candidato.FechaNacimiento,
                    fechaNac = c.Candidato.FechaNacimiento,
                    localidad = c.Candidato.municipioNacimiento.municipio + " / " + c.Candidato.estadoNacimiento.estado,
                    genero = c.Candidato.GeneroId == 1 ? "Hombre" : "Mujer",
                    generoId = c.Candidato.GeneroId,
                    lada = c.Candidato.telefonos.FirstOrDefault().ClaveLada.Trim(),
                    telefono = c.Candidato.telefonos.FirstOrDefault().telefono.Trim(),
                    email = c.Candidato.emails.FirstOrDefault().email.Trim(),
                    estadoNacimiento = c.Candidato.estadoNacimiento.estado,
                    estadoNacimientoId = c.Candidato.EstadoNacimientoId,
                    clave = c.Candidato.estadoNacimiento.Clave,
                    c.Fch_Creacion,
                    estatus = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(c.CandidatoId) && x.RequisicionId.Equals(requisicionId)).Select(e => e.Estatus.Descripcion).FirstOrDefault(),
                    estatusId = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(c.CandidatoId) && x.RequisicionId.Equals(requisicionId)).Select(e => e.EstatusId).FirstOrDefault(),
                    horario = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(c.CandidatoId) && x.RequisicionId.Equals(requisicionId)).Select(h => h.Horario.Nombre).FirstOrDefault(),
                    horarioId = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(c.CandidatoId) && x.RequisicionId.Equals(requisicionId)).Select(h => h.HorarioId).FirstOrDefault(),
                    tipoMediosId = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(c.CandidatoId) && x.RequisicionId.Equals(requisicionId)).Select(h => h.TipoMediosId).FirstOrDefault(),
                }).ToList().Where(xx => xx.estatusId.Equals(12)).ToList();

                return Ok(candidatos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }

        [HttpGet]
        [Route("getMisCandidatos")]
        [Authorize]
        public IHttpActionResult GetMisCandidatos(Guid reclutadorId)
        {
            try
            {
                var candidatos = db.ProcesoCampo.OrderBy(f => f.Candidato.ApellidoPaterno).Where(x => x.UsuarioId.Equals(reclutadorId)).Select(c => new
                {
                    candidatoId = c.CandidatoId,
                    foto = @"https://apisb.damsa.com.mx/utilerias/img/Candidatos/" + c.CandidatoId + "/foto.jpg",
                    //vacantes = db.HorariosRequis.Where(x => x.RequisicionId.Equals(c.RequisicionId)).Count() > 0 ? db.HorariosRequis.Where(x => x.RequisicionId.Equals(c.RequisicionId)).Sum(h => h.numeroVacantes) : 0,
                    curp = String.IsNullOrEmpty(c.Candidato.CURP) ? "Sin registro" : c.Candidato.CURP,
                    nombreCompleto = c.Candidato.Nombre + " " + c.Candidato.ApellidoPaterno + " " + c.Candidato.ApellidoMaterno,
                    fechaNac = c.Candidato.FechaNacimiento,
                    genero = c.Candidato.GeneroId == 1 ? "Hombre" : "Mujer",
                    generoId = c.Candidato.GeneroId,
                    lada = c.Candidato.telefonos.FirstOrDefault().ClaveLada.Trim(),
                    telefono = c.Candidato.telefonos.FirstOrDefault().telefono.Trim(),
                    email = c.Candidato.emails.FirstOrDefault().email.Trim(),
                    estadoNacimiento = c.Candidato.estadoNacimiento.estado,
                    estadoNacimientoId = c.Candidato.EstadoNacimientoId,
                    estatus = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(e => e.Estatus.Descripcion).FirstOrDefault(),
                    estatusId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(e => e.EstatusId).FirstOrDefault()
                }).ToList().Where(e => e.estatusId.Equals(27)).ToList();

                return Ok(candidatos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpGet]
        [Route("getCandidato")]
        [Authorize]
        public IHttpActionResult GetCandidato(Guid candidatoId)
        {
            try
            {
                var candidatos = db.Candidatos.Where(x => x.Id.Equals(candidatoId)).Select(c => new
                {
                    candidatoId = c.Id,
                    foto = @"https://apisb.damsa.com.mx/utilerias/img/Candidatos/" + c.Id + "/foto.jpg",
                    credencial = @"https://apisb.damsa.com.mx/utilerias/img/Candidatos/" + c.Id + "/credencial.jpg",
                    apellidoP = c.ApellidoPaterno,
                    apellidoM = c.ApellidoMaterno,
                    name = c.Nombre,
                    curp = String.IsNullOrEmpty(c.CURP) ? "Sin registro" : c.CURP,
                    nombre = c.Nombre + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno,
                    dateBirth = c.FechaNacimiento,
                    localidad = c.municipioNacimiento.municipio + " / " + c.estadoNacimiento.estado,
                    gender = c.GeneroId,
                    estado = c.estadoNacimiento.Clave,
                    lada = c.telefonos.FirstOrDefault().ClaveLada.Trim(),
                    telefono = c.telefonos.FirstOrDefault().telefono.Trim(),
                    email = c.emails.FirstOrDefault().email.Trim(),
                    registro = !string.IsNullOrEmpty(c.emails.FirstOrDefault().email.Trim()) ? 1 : 2,
                    horarioId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(c.Id) && x.EstatusId.Equals(12)).Select(h => h.HorarioId).FirstOrDefault(),
                    tipoMediosId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(c.Id) && x.EstatusId.Equals(12)).Select(h => h.TipoMediosId).FirstOrDefault(),
                });

                return Ok(candidatos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpPost]
        [Route("updateDtosCandidato")]
        [Authorize]
        public IHttpActionResult UpdateDtosCandidato(CandidatosGralDto datos)
        {
            try
            {
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

                if (!String.IsNullOrEmpty(datos.Email.Select(e => e.email).ToString()))
                {
                    candidato.emails = datos.Email;
                }
                if (!String.IsNullOrEmpty(datos.Telefono.Select(e => e.telefono).ToString()))
                {
                    candidato.telefonos = datos.Telefono;
                }

                db.SaveChanges();

                if(datos.Foto != "")
                {
                    this.GuardarImagen("foto", datos.Id.ToString(), datos.Foto, "jpeg");
                }
                if (datos.Credencial != "")
                {
                    this.GuardarImagen("foto", datos.Id.ToString(), datos.Credencial, "jpeg");
                }

                var horario = auxID;
                if (datos.horarioId == auxID)
                {
                    horario = db.HorariosRequis.Where(x => x.RequisicionId.Equals(datos.requisicionId)).Select(h => h.Id).FirstOrDefault();
                }
                else
                {
                    horario = datos.horarioId;
                }

                var idP = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(x => x.CandidatoId.Equals(datos.Id) && x.RequisicionId.Equals(datos.requisicionId) && x.EstatusId.Equals(12)).Select(id => id.Id).FirstOrDefault();
                if(idP != auxID)
                {
                    var proceso = db.ProcesoCandidatos.Find(idP);
                    db.Entry(proceso).Property(x => x.HorarioId).IsModified = true;
                    db.Entry(proceso).Property(x => x.TipoMediosId).IsModified = true;

                    proceso.HorarioId = horario;
                    proceso.TipoMediosId = datos.tipoMediosId;

                    db.SaveChanges();
                }

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [Route("updateContratadosCampo")]
        [Authorize]
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

        [HttpPost]
        [Route("apartarCandidato")]
        [Authorize]
        public IHttpActionResult ApartarCandidato(CandidatosGralDto datos)
        {
            try
            {
                var id = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(datos.Id)).OrderByDescending(f => f.Fch_Modificacion).Select(x => x.Id).FirstOrDefault();
                if (id != auxID)
                {
                    var c = db.ProcesoCandidatos.Find(id);

                    if (c.EstatusId == 27 || c.EstatusId == 10)
                    {
                        db.Entry(c).Property(x => x.EstatusId).IsModified = true;
                        db.Entry(c).Property(x => x.HorarioId).IsModified = true;
                        db.Entry(c).Property(x => x.Fch_Modificacion).IsModified = true;
                        db.Entry(c).Property(x => x.ReclutadorId).IsModified = true;
                        db.Entry(c).Property(x => x.RequisicionId).IsModified = true;
                        db.Entry(c).Property(x => x.Reclutador).IsModified = true;

                        var horario = auxID;
                        if (datos.horarioId == auxID)
                        {
                            horario = db.HorariosRequis.Where(x => x.RequisicionId.Equals(datos.requisicionId)).Select(h => h.Id).FirstOrDefault();
                        }
                        else
                        {
                            horario = datos.horarioId;
                        }

                        c.EstatusId = 12;
                        c.HorarioId = horario;
                        c.Fch_Modificacion = DateTime.Now;
                        c.ReclutadorId = datos.reclutadorId;
                        c.Reclutador = db.Usuarios.Where(x => x.Id.Equals(datos.reclutadorId)).Select(u => u.Nombre + " " + u.ApellidoPaterno + " " + u.ApellidoMaterno).FirstOrDefault();
                        c.RequisicionId = datos.requisicionId;

                        db.SaveChanges();

                        var idC = db.ProcesoCampo.Where(x => x.CandidatoId.Equals(datos.Id)
                        && x.UsuarioId.Equals(datos.reclutadorCampoId)
                        ).Select(r => r.Id).FirstOrDefault();
                        if (idC != auxID)
                        {
                            var pcc = db.ProcesoCampo.Find(idC);
                            db.Entry(pcc).Property(x => x.RequisicionId).IsModified = true;
                            db.Entry(pcc).Property(x => x.ReclutadorId).IsModified = true;

                            pcc.RequisicionId = datos.requisicionId;
                            pcc.ReclutadorId = datos.reclutadorId;

                            db.SaveChanges();
                        }

                        return Ok(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Ok(HttpStatusCode.Ambiguous);
                    }
                }
                else
                {
                    return Ok(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getInfoVacante")]
        [Authorize]
        public IHttpActionResult GetInfoVacante(Guid requisicionId)
        {
            try
            {
                var info = db.Requisiciones.Where(x => x.Id.Equals(requisicionId) && !x.Confidencial).Select(r => new
                {
                    vBtra = r.VBtra,
                    vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                    folio = r.Folio,
                    genero = r.Genero.genero,
                    edadMax = r.EdadMaxima,
                    edadMin = r.EdadMinima,
                    edoCivil = r.EstadoCivil.estadoCivil,
                    escolaridad = r.escolaridadesRequi.Select(e => e.Escolaridad.gradoEstudio),
                    aptitudes = r.aptitudesRequi.Select(a => a.Aptitud.aptitud),
                    areaExperiencia = r.Area.areaExperiencia,
                    experiencia = r.Experiencia,
                    horario = r.horariosRequi.Select(h => h.deDia.diaSemana + " a " + h.aDia.diaSemana + " de " + h.deHora.Hour + " a " + h.aHora.Hour),
                    actividades = r.actividadesRequi.Select(a => a.Actividades),
                    sueldoMin = r.SueldoMinimo,
                    sueldo = r.SueldoMinimo,
                    beneficios = r.beneficiosRequi.Select(b => b.TipoBeneficio.tipoBeneficio)
                }).ToList();

                return Ok(info);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("sendEmailRegistro")]
        [Authorize]
        public IHttpActionResult SendEmailRegistro(Guid candidato)
        {
            try
            {
                var datos = db.Candidatos.Where(x => x.Id.Equals(candidato)).Select(d => new {
                    nombre = d.Nombre + " " + d.ApellidoPaterno + " " + d.ApellidoMaterno,
                    email = d.emails.Select(e => e.email).FirstOrDefault(),
                    pass = db.AspNetUsers.Where(a => a.IdPersona.Equals(candidato)).Select(p => p.Pasword).FirstOrDefault(),
                    idBolsa = db.AspNetUsers.Where(a => a.IdPersona.Equals(candidato)).Select(p => p.Id).FirstOrDefault()
                }).ToList();

                var pass = db.Database.SqlQuery<String>("dbo.spDesencriptarPasword @id", new SqlParameter("id", datos[0].idBolsa)).FirstOrDefault();
                var body = "";
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "Bolsa de Trabajo DAMSA");
                m.Subject = "Datos de Registro a Bolsa de Trabajo DAMSA";
                m.To.Add(datos[0].email);

                m.Bcc.Add("bmorales@damsa.com.mx");
                m.Bcc.Add("mventura@damsa.com.mx");

                m.Subject = "Tu acceso a Bolsa de Trabajo DAMSA está listo!";
                body = "<html><body><table width=\"80%\" style=\"font-family:'calibri'\">";
                body = body + "<tr><th bgcolor=\"#044464\" style=\"color:white; text-align:left;\">Se creó una nueva cuenta para BOLSA TRABAJO DAMSA </th></ tr>";
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Usuario:</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td>{0}</td></tr>", datos[0].email);
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Nombre :</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} </td></tr>", datos[0].nombre);
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Correo :</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} </td></tr>", datos[0].email);
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Contraseña :</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} </td></tr>", pass);
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Registrado :</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#FDC613\"><td>{0}<br/>", DateTime.Now.ToShortDateString());
                body = body + "<p> Podrás acceder mediante la siguiente dirección: https://www.damsa.com.mx/bt <br/>";
                body = body + "Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx </p></td></tr></table>";
                body = body + "</body></html>";

                m.Body = body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                smtp.Send(m);

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("EnviarSMS")]
        [Authorize]
        public async Task<IHttpActionResult> EnviarSMS(Guid candidato)
        {
            Regex reg = new Regex("[^a-zA-Z0-9] ");
            var datos = db.Candidatos.Where(x => x.Id.Equals(candidato)).Select(d => new {
                nombre = d.Nombre + " " + d.ApellidoPaterno + " " + d.ApellidoMaterno,
                telefono = d.telefonos.Select(e => e.ClaveLada + e.telefono).FirstOrDefault(),
                pass = db.AspNetUsers.Where(a => a.IdPersona.Equals(candidato)).Select(p => p.Pasword).FirstOrDefault(),
                idBolsa = db.AspNetUsers.Where(a => a.IdPersona.Equals(candidato)).Select(p => p.Id).FirstOrDefault()
            }).ToList();

            var pass = db.Database.SqlQuery<String>("dbo.spDesencriptarPasword @id", new SqlParameter("id", datos[0].idBolsa)).FirstOrDefault();
            List<string> Destino = new List<string>(1) { datos[0].telefono };
            BasicAuthConfiguration BASIC_AUTH_CONFIGURATION = new BasicAuthConfiguration(ConfigurationManager.AppSettings["BaseUrl"], ConfigurationManager.AppSettings["UserInfobip"], ConfigurationManager.AppSettings["PassInfobip"]);

            SendSingleTextualSms smsClient = new SendSingleTextualSms(BASIC_AUTH_CONFIGURATION);

            string texto = "Datos de acceso a Bolsa Trabajo DAMSA: Usuario: " + datos[0].telefono + " Contraseña: " + pass + " Da click https://www.damsa.com.mx/bt para cambiar contraseña y actualizar tu perfil";
            texto = texto.Normalize(NormalizationForm.FormD);
            texto = reg.Replace(texto, " ");

            SMSTextualRequest request = new SMSTextualRequest
            {
                From = "Bolsa de Trabajo DAMSA",
                To = Destino,
                Text = ConfigurationManager.AppSettings["NameAppMsj"] + texto

            };

            SMSResponse smsResponse = await smsClient.ExecuteAsync(request); // Manda el mensaje con código.

            SMSResponseDetails sentMessageInfo = smsResponse.Messages[0];


            return Ok(HttpStatusCode.Created);
        }
    }
}
