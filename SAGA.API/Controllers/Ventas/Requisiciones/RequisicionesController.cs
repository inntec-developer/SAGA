using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Requisiciones")]
    public class RequisicionesController : ApiController
    {
        private SAGADBContext db;
        Damfo290Dto DamfoDto;

        public RequisicionesController()
        {
            db = new SAGADBContext();
            DamfoDto = new Damfo290Dto();
        }

        [HttpGet]
        [Route("getAddress")]
        public IHttpActionResult GetAddress(Guid Id)
        {
            try
            {
                DamfoDto.Damfo290Address = (from damfo in db.DAMFO290
                                            join cliente in db.Clientes on damfo.ClienteId equals cliente.Id
                                            join persona in db.Personas on cliente.Id equals persona.Id
                                            join direccion in db.Direcciones on persona.Id equals direccion.PersonaId
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

        //api/Requisiciones/createRequi
        [HttpPost]
        [Route("createRequi")]
        public IHttpActionResult Clon(CreateRequiDto cr)
        {
            try
            {
                object[] _params = {
                    new SqlParameter("@Id", cr.IdDamfo),
                    new SqlParameter("@IdAddress", cr.IdAddress)
                };

                var requi = db.Database.SqlQuery<Requisicion>("exec createRequisicion @Id, @IdAddress", _params).SingleOrDefault();
                Guid RequisicionId = requi.Id;
                return Ok(RequisicionId);
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
                e.Id,e.VBtra, e.TipoReclutamiento, e.ClaseReclutamiento, 
                e.SueldoMinimo, e.SueldoMaximo, e.fch_Creacion, e.fch_Cumplimiento,
                e.Estatus,
                Prioridad = db.Prioridades.Where(p => p.Id == e.PrioridadId).Select( p => new {
                        p.Descripcion,
                        p.Id
                    }).FirstOrDefault(),
                Cliente = db.Clientes.Where(c => c.Id == e.ClienteId).Select(c => new
                    {
                        c.Nombrecomercial, c.GiroEmpresas,
                        c.ActividadEmpresas, c.RFC}).FirstOrDefault()
            }).ToList();
            return Ok(requisicion);
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
