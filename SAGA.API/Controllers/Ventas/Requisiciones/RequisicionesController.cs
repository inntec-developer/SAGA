﻿using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using SAGA.API.Utilerias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Requisiciones")]
    public class RequisicionesController : ApiController
    {
        private SAGADBContext db;
        private Damfo290Dto DamfoDto;
        private BusinessDay businessDay;
        private Rastreabilidad rastreabilidad;
        private SendEmails SendEmail;

        public RequisicionesController()
        {
            db = new SAGADBContext();
            DamfoDto = new Damfo290Dto();
            businessDay = new BusinessDay();
            rastreabilidad = new Rastreabilidad();
            SendEmail = new SendEmails();
        }

        [HttpGet]
        [Route("getAddress")]
        public IHttpActionResult GetAddress(Guid Id)
        {
            try
            {
                DamfoDto.Damfo290Address = (from damfo in db.DAMFO290
                                            join cliente in db.Clientes on damfo.ClienteId equals cliente.Id
                                            join persona in db.Entidad on cliente.Id equals persona.Id
                                            join direccion in db.Direcciones on persona.Id equals direccion.EntidadId
                                            join tipoDireccion in db.TiposDirecciones on direccion.TipoDireccionId equals tipoDireccion.Id
                                            join pais in db.Paises on direccion.PaisId equals pais.Id
                                            join estado in db.Estados on direccion.EstadoId equals estado.Id
                                            join municipio in db.Municipios on direccion.MunicipioId equals municipio.Id
                                            join colonia in db.Colonias on direccion.ColoniaId equals colonia.Id
                                            where damfo.Id == Id && direccion.Activo == true
                                            select new Damfo290AddressDto
                                            {
                                                Id = direccion.Id,
                                                TipoDireccion = tipoDireccion.tipoDireccion,
                                                Pais = pais.pais,
                                                Estado = estado.estado,
                                                Municipio = municipio.municipio,
                                                Colonia = colonia.colonia,
                                                Calle = direccion.Calle + " " + direccion.NumeroExterior + " C.P: " + direccion.CodigoPostal + " Col: " + colonia.colonia,
                                                CodigoPostal = direccion.CodigoPostal,
                                                NumeroExterior = direccion.NumeroExterior,
                                                NumeroInterior = direccion.NumeroInterior

                                            }).ToList();
                return Ok(DamfoDto.Damfo290Address);

            }
            catch (Exception ex)
            {
                string messg = ex.Message;
                return Ok(messg);
            }
        }

        [HttpGet]
        [Route("getById")]
        public IHttpActionResult GetRequisicion(Guid Id)
        {
            if (Id != null)
            {
                var requisicion = db.Requisiciones.FirstOrDefault(x => x.Id.Equals(Id));
                return Ok(requisicion);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet]
        [Route("getByFolio")]
        public IHttpActionResult GetRequisicionFolio(Int64 folio)
        {
            if (folio != 0)
            {

                var requisicion = db.Requisiciones.Where(x => x.Folio.Equals(folio)).Select(r => new {
                    r.Id,
                    r.Folio,
                    r.fch_Cumplimiento,
                    r.fch_Creacion,
                    r.fch_Limite,
                    r.Prioridad,
                    r.Confidencial,
                    r.Estatus,
                    asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id)).Select(x => x.GrpUsrId).ToList(),
                    asignadosN = r.AsignacionRequi.Where(x => x.RequisicionId.Equals(r.Id)).Select(x => new {
                        x.GrpUsr.Nombre,
                        x.GrpUsr.ApellidoMaterno,
                        x.GrpUsr.ApellidoPaterno
                    }),
                    vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                    r.VBtra
                }).FirstOrDefault();
                return Ok(requisicion);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost]
        [Route("createRequi")]
        public IHttpActionResult Clon(CreateRequiDto cr)
        {
            try
            {
                object[] _params = {
                    new SqlParameter("@Id", cr.IdDamfo),
                    new SqlParameter("@IdAddress", cr.IdAddress),
                    new SqlParameter("@UserAlta", cr.Usuario)
                };

                var requi = db.Database.SqlQuery<Requisicion>("exec createRequisicion @Id, @IdAddress, @UserAlta", _params).SingleOrDefault();

                Guid RequisicionId = requi.Id;
                Int64 Folio = requi.Folio;


                return Ok(requi);
            }
            catch (Exception ex)
            {
                string messg = ex.Message;
                return Ok(messg);
            }

        }

        [HttpGet]
        [Route("getRequisiciones")]
        public IHttpActionResult GetRequisiciones(string propietario)
        {
            var requisicion = db.Requisiciones
                .Where(e => e.Activo.Equals(true) && e.Propietario.Equals(propietario))
                .Select(e => new
                {
                    Id = e.Id,
                    VBtra = e.VBtra,
                    TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento,
                    ClaseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento,
                    SueldoMinimo = e.SueldoMinimo,
                    SueldoMaximo = e.SueldoMaximo,
                    fch_Creacion = e.fch_Creacion,
                    fch_Cumplimiento = e.fch_Cumplimiento,
                    Estatus = e.Estatus.Descripcion,
                    EstatusId = e.EstatusId,
                    Prioridad = e.Prioridad.Descripcion,
                    PrioridadId = e.PrioridadId,
                    Cliente = e.Cliente.Nombrecomercial,
                    GiroEmpresa = e.Cliente.GiroEmpresas.giroEmpresa,
                    ActividadEmpresa = e.Cliente.ActividadEmpresas.actividadEmpresa,
                    Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                    Folio = e.Folio,
                    DiasEnvio = e.DiasEnvio,
                    Confidencial = e.Confidencial,
                    Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id)).Count(),
                    PostuladosN = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id)).Select(p => new
                    {
                       p.Candidato.Nombre,
                       p.Candidato.ApellidoPaterno,
                       p.Candidato.ApellidoMaterno,
                       p.Candidato.CURP
                    }),
                    EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id)).Count()
                }).ToList().OrderByDescending(x => x.Folio);
            return Ok(requisicion);
        } 

        [HttpGet]
        [Route("getRequiReclutador")]
        public IHttpActionResult GtRequiReclutador(Guid IdUsuario)
        {
            try
            {
                List<Guid> grp = new List<Guid>();

                var Grupos = db.GruposUsuarios
                    .Where(g => g.EntidadId.Equals(IdUsuario) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();


                foreach (var grps in Grupos)
                {
                    grp = GetGrupo(grps, grp);
                }

                grp.Add(IdUsuario);

                var asig = db.AsignacionRequis
                    .OrderByDescending(e => e.Id)
                    .Where(a => grp.Distinct().Contains(a.GrpUsrId))
                    .Select(a => a.RequisicionId)
                    .Distinct()
                    .ToList();


                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => asig.Contains(e.Id))
                    .Where(e => e.Activo.Equals(true))
                    .Select(e => new
                    {
                        Id = e.Id,
                        VBtra = e.VBtra,
                        TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento,
                        tipoReclutamientoId = e.TipoReclutamientoId,
                        ClaseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento,
                        ClaseReclutamientoId = e.ClaseReclutamientoId,
                        SueldoMinimo = e.SueldoMinimo,
                        SueldoMaximo = e.SueldoMaximo,
                        fch_Creacion = e.fch_Creacion,
                        fch_Cumplimiento = e.fch_Cumplimiento,
                        Estatus = e.Estatus.Descripcion,
                        EstatusId = e.EstatusId,
                        Prioridad = e.Prioridad.Descripcion,
                        PrioridadId = e.PrioridadId,
                        Cliente = e.Cliente.Nombrecomercial,
                        ClienteId = e.Cliente.Id,
                        Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        Folio = e.Folio,
                        DiasEnvio = e.DiasEnvio,
                        Confidencial = e.Confidencial,
                        //asignados = e.AsignacionRequi.Select(a => a.GrpUsrId).ToList(),
                        Asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id)).Select(x => x.GrpUsrId).ToList(),
                        Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id)).Count(),
                        EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id)).Count(),
                        Solicita = db.Usuarios.Where(x => x.Usuario.Equals(e.Propietario)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault(),
                        AreaExperiencia = e.Area.areaExperiencia
                    }).ToList();
                return Ok(vacantes);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getDireccionRequisicon")]
        public IHttpActionResult GetDireccionRequisicon(Guid Id)
        {
            try
            {
                var direccion = db.Direcciones.Where(x => x.Id.Equals(Id)).ToList();
                return Ok(direccion);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("upadateVacantes")]
        public IHttpActionResult UpdateVacantes(HorariosRequi horario)
        {
            try
            {
                var hr = db.HorariosRequis.Find(horario.Id);
                db.Entry(hr).State = EntityState.Modified;
                hr.numeroVacantes = horario.numeroVacantes;
                hr.UsuarioMod = horario.Usuario;
                hr.fch_Modificacion = DateTime.Now;
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getHorariosRequisicion")]
        public IHttpActionResult GetHorariosRequisicion(Guid Id)
        {
            try
            {
                var direccion = db.HorariosRequis.Where(x => x.RequisicionId.Equals(Id)).ToList();
                return Ok(direccion);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("updateRequisiciones")]
        public IHttpActionResult UpdateRequi(RequisicionDto requi)
        {
            db.Database.Log = Console.Write;
            using (DbContextTransaction beginTran = db.Database.BeginTransaction())
            {
                try
                {
                    var requisicion = db.Requisiciones.Find(requi.Id);
                    db.Entry(requisicion).State = EntityState.Modified;
                    requisicion.fch_Cumplimiento = requi.fch_Cumplimiento;
                    requisicion.PrioridadId = requi.PrioridadId;
                    requisicion.Confidencial = requi.Confidencial;
                    if (requi.Confidencial && requisicion.EstatusId == 7)
                        requisicion.EstatusId = 6;
                    else
                        requisicion.EstatusId = requi.EstatusId;
                    requisicion.fch_Modificacion = DateTime.Now;
                    requisicion.UsuarioMod = requi.Usuario;
                    AlterAsignacionRequi(requi.AsignacionRequi, requi.Id, requi.Folio, requi.Usuario, requisicion.VBtra);
                    db.SaveChanges();

                    Int64 Folio = requisicion.Folio;
                    //Creacion de Trazabalidad par ala requisición.
                    Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                    //Isertar el registro de la rastreabilidad. 
                    rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.Usuario, 3);

                    beginTran.Commit();

                    return Ok(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    beginTran.Rollback();
                    Console.Write(ex.Message);
                    return NotFound();
                }
            }


        }

        [HttpPost]
        [Route("deleteRequisiciones")]
        public IHttpActionResult DeleteRequi(RequisicionDeleteDto requi)
        {
            try
            {
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi.Id)).ToList();

                var requisicion = db.Requisiciones.Find(requi.Id);
                db.Entry(requisicion).State = EntityState.Modified;
                requisicion.Activo = false;
                requisicion.UsuarioMod = requi.UsuarioMod;
                requisicion.fch_Modificacion = DateTime.Now;
                requisicion.EstatusId = 9;

                db.AsignacionRequis.RemoveRange(asignados);



                Int64 Folio = requisicion.Folio;
                string VBra = requisicion.VBtra;
                Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                //Isertar el registro de la rastreabilidad. 
                rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.UsuarioMod, 4);

                SendEmail.ConstructEmail(asignados, null, "RD", Folio, string.Empty, VBra);

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch
            {
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        [Route("cancelRequisiciones")]
        public IHttpActionResult CancelRequi(RequisicionDeleteDto requi)
        {
            try
            {
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi.Id)).ToList();

                var requisicion = db.Requisiciones.Find(requi.Id);
                db.Entry(requisicion).State = EntityState.Modified;
                requisicion.EstatusId = 8;
                requisicion.Aprobada = false;
                requisicion.Aprobador = string.Empty;
                requisicion.UsuarioMod = requi.UsuarioMod;
                requisicion.fch_Modificacion = DateTime.Now;

                db.AsignacionRequis.RemoveRange(asignados);



                Int64 Folio = requisicion.Folio;
                string VBra = requisicion.VBtra;

                Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                //Isertar el registro de la rastreabilidad. 
                rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.UsuarioMod, 6);

                SendEmail.ConstructEmail(asignados, null, "RU", Folio, string.Empty, VBra);

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        [Route("reActivarRequisiciones")]
        public IHttpActionResult ReActivar(RequisicionDeleteDto requi)
        {
            try
            {
                var requisicion = db.Requisiciones.Find(requi.Id);
                db.Entry(requisicion).State = EntityState.Modified;
                requisicion.EstatusId = 5;
                requisicion.UsuarioMod = requi.UsuarioMod;
                requisicion.fch_Modificacion = DateTime.Now;


                Int64 Folio = requisicion.Folio;
                Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                //Isertar el registro de la rastreabilidad. 
                rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.UsuarioMod, 3);

                db.SaveChanges();


                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        [Route("asignacionRequisiciones")]
        public IHttpActionResult AsginarRequi(AsignarVacanteReclutador requi)
        {
            db.Database.Log = Console.Write;
            using (DbContextTransaction beginTran = db.Database.BeginTransaction())
            {
                try
                {
                    var requisicion = db.Requisiciones.Find(requi.Id);
                    db.Entry(requisicion).State = EntityState.Modified;
                    requisicion.fch_Cumplimiento = requi.fch_Cumplimiento;
                    if (requisicion.EstatusId != 7)
                    {
                        requisicion.EstatusId = 6;
                        requisicion.Aprobador = requi.Usuario;
                        requisicion.Aprobada = true;
                        requisicion.fch_Aprobacion = DateTime.Now;
                    }
                    requisicion.DiasEnvio = requi.DiasEnvio;
                    requisicion.fch_Modificacion = DateTime.Now;
                    requisicion.UsuarioMod = requi.Usuario;
                    db.SaveChanges();
                    AlterAsignacionRequi(requi.AsignacionRequi, requi.Id, requisicion.Folio, requi.Usuario, requisicion.VBtra);
                    db.SaveChanges();
                    Int64 Folio = requisicion.Folio;
                    //Creacion de Trazabalidad par ala requisición.
                    Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                    //Isertar el registro de la rastreabilidad. 
                    rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.Usuario, 5);


                    beginTran.Commit();

                    return Ok(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    beginTran.Rollback();
                    Console.Write(ex.Message);
                    return NotFound();
                }
            }


        }

        private void Save()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    // Update the values of the entity that failed to save from the store 
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }

        public void AlterAsignacionRequi(List<AsignacionRequi> asignaciones, Guid RequiId, Int64 Folio, string Usuario, string VBra)
        {
            var user = db.Usuarios.Where(x => x.Usuario.Equals(Usuario)).Select(x =>
               x.Nombre + " " + x.ApellidoPaterno + " " + x.ApellidoMaterno
            ).FirstOrDefault();

            List<AsignacionRequi> NotChange = new List<AsignacionRequi>();
            List<AsignacionRequi> CheckExcept = new List<AsignacionRequi>();
            List<AsignacionRequi> AddElmt = new List<AsignacionRequi>();
            List<AsignacionRequi> Delete = new List<AsignacionRequi>();
            var asg = db.AsignacionRequis
                .Where(x => x.RequisicionId.Equals(RequiId))
                .ToList();
            if (asg.Count() > 0)
            {
                for (int i = 0; i < asg.Count(); i++)
                {
                    if (asignaciones.Count() > 0)
                    {
                        for (int x = 0; x < asignaciones.Count(); x++)
                        {
                            if (asignaciones[x].GrpUsrId.Equals(asg[i].GrpUsrId))
                            {
                                NotChange.Add(asignaciones[x]);
                                CheckExcept.Add(asg[i]);

                            }
                            if (asignaciones[x].GrpUsrId != asg[i].GrpUsrId)
                            {
                                AddElmt.Add(asignaciones[x]);

                            }
                        }
                    }
                }

                var filterAdd = AddElmt.Except(NotChange).ToList();
                var delet = asg.Except(CheckExcept).ToList();

                if (delet.Count() > 0)
                {
                    db.AsignacionRequis.RemoveRange(delet);
                    SendEmail.ConstructEmail(delet, NotChange, "D", Folio, user, VBra);
                }
                if (filterAdd.Count() > 0)
                {
                    db.AsignacionRequis.AddRange(filterAdd);
                    SendEmail.ConstructEmail(filterAdd, NotChange, "C", Folio, user, VBra);
                }
            }
            else
            {
                db.AsignacionRequis.AddRange(asignaciones);
                SendEmail.ConstructEmail(asignaciones, NotChange, "C", Folio, user, VBra);
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
    }
}
