using SAGA.API.Dtos.Reclutamiento.Ingresos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.SistGestionPersonal
{
    [RoutePrefix("api/gestionpersonal")]
    public class ChecadorController : ApiController
    {
        private SAGADBContext db;
        public ChecadorController()
        {
            db = new SAGADBContext();
        }
        [HttpPost]
        [Route("addJornadaManual")]
        public IHttpActionResult AddJornadaManual(List<Jornada> registros)
        {
            try
            {
                db.Jornada.AddRange(registros);
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("getAsignadosByCliente")]
        public IHttpActionResult GetAsignadosByCliente(Guid ClienteId)
        {
            try
            {
                   var datos = db.EmpleadoHorario.Where(x => x.Activo && x.HorariosIngresos.ClienteId.Equals(ClienteId)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        foto = "img/Candidatos/" + d.empleadoId + "/foto.jpg",
                        nombre = d.Empleado.Nombre,
                        nombreCompleto = d.Empleado.Nombre + " " + d.Empleado.ApellidoPaterno + " " + d.Empleado.ApellidoMaterno,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        curp = d.Empleado.CURP,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000", 
                        horario = "l,m,m,j,v 08:00 - 18:00"
                   }).ToList();

                    return Ok(datos);
              
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
    }
}
