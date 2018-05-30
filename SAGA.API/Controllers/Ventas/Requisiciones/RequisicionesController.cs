using SAGA.DAL;
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
        Damfo290Dto DamfoDto;
        BusinessDay businessDay;

        public RequisicionesController()
        {
            db = new SAGADBContext();
            DamfoDto = new Damfo290Dto();
            businessDay = new BusinessDay();
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
                                            where damfo.Id == Id
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

        //api/Requisiciones/getRequisicion
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
        //api/Requisiciones/getRequisicion
        [HttpGet]
        [Route("getByFolio")]
        public IHttpActionResult GetRequisicionFolio(int folio)
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
                    r.Estatus
                }).FirstOrDefault();
                return Ok(requisicion);
            }
            else
            {
                return NotFound();
            }

        }


        //api/Requisiciones/createRequi
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
                int Folio = requi.Folio;
                

                return Ok(requi);
            }
            catch (Exception ex)
            {
                string messg = ex.Message;
                return Ok(messg);
            }

        }

        //api/Requisiciones/getRequisiciones
        [HttpGet]
        [Route("getRequisiciones")]
        public IHttpActionResult GetRequisiciones()
        {
            var requisicion = db.Requisiciones.Select(e => new
            {
                e.Id, e.VBtra,
                e.TipoReclutamiento,
                e.ClaseReclutamiento,
                e.SueldoMinimo,
                e.SueldoMaximo,
                e.fch_Creacion,
                e.fch_Cumplimiento,
                e.Estatus,
                Prioridad = db.Prioridades.Where(p => p.Id == e.PrioridadId).Select(p => new {
                    p.Descripcion,
                    p.Id
                }).FirstOrDefault(),
                Cliente = db.Clientes.Where(c => c.Id == e.ClienteId).Select(c => new
                {
                    c.Nombrecomercial, c.GiroEmpresas,
                    c.ActividadEmpresas, c.RFC }).FirstOrDefault(),
                HorarioRequi = db.HorariosRequis.Where(x => x.RequisicionId.Equals(e.Id)).Select( x => new {
                    x.Nombre, 
                    x.deDia,
                    x.aDia,
                    x.deHora,
                    x.aHora,
                    x.numeroVacantes,
                    x.Especificaciones
                }).ToList(),
                e.Folio
            }).ToList().OrderByDescending(x => x.Folio);
            return Ok(requisicion);
        }

        [HttpPost]
        [Route("updateRequisiciones")]
        public IHttpActionResult UpdateRequi(RequisicionDto requi)
        {
            try
            {
                var requisicion = db.Requisiciones.Find(requi.Id);
                db.Entry(requisicion).State = EntityState.Modified;
                requisicion.fch_Cumplimiento = requi.fch_Cumplimiento;
                requisicion.EstatusId = requi.EstatusId;
                if (requi.EstatusId == 6)
                {
                    requisicion.Aprobada = true;
                    requisicion.fch_Aprobacion = DateTime.Now;
                    requisicion.Aprobador = requi.Usuario;
                }
                else
                {
                    requisicion.Aprobada = false;
                    requisicion.fch_Aprobacion = null;
                    requisicion.Aprobador = "";
                }
                requisicion.PrioridadId = requi.PrioridadId;
                requisicion.Confidencial = requi.Confidencial;
                requisicion.fch_Modificacion = DateTime.Now;
                requisicion.UsuarioMod = requi.Usuario;
                
                db.SaveChanges();
                //string msj = string.Format("La requisición Folio: {0} se actualizo correctamente.", requi.Folio);
                return Ok(requi.Folio);
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
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
    }
}
