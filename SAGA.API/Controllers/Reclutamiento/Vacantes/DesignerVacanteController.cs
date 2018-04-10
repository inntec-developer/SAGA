using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.Web.Script.Serialization;


namespace SAGA.API.Controllers
{
    [RoutePrefix("api/dvacante")]
    public class DesignerVacanteController : ApiController
    {
        private SAGADBContext db;
        public DesignerVacanteController()
        {
            db = new SAGADBContext();
        }
        [HttpGet]
        [Route("get")]
        public IHttpActionResult Action()
        {
            var datos = db.Requisiciones.Select(e=> new { e.Id , e.VBtra, e.Experiencia }).ToList();
            
            return Ok(datos);
        }

        [HttpGet]
        [Route("setResumen")]
        public IHttpActionResult ActualizarResumen(Guid Requi, int Idcampo, bool resumen)
        {
            string mensaje;
            bool bandera;
            try
            {
                var datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
                if (datos.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.IdTipoEstructura == 7).ToList();
                    foreach (var item in estructura)
                    {
                        ConfiguracionRequi caja = new ConfiguracionRequi();
                        caja.Campo = item.Nombre;
                        caja.IdRequi = Requi;
                        caja.Detalle = false;
                        caja.Resumen = false;
                        caja.R_D = 0;
                        caja.IdEstructura = item.Id;
                        db.ConfiguracionRequis.Add(caja);
                        db.SaveChanges();
                    }
                }
                var lista = datos.Where(e => e.IdEstructura == Idcampo).FirstOrDefault();
                lista.Resumen = resumen;
                lista.R_D = Bandera(Requi, Idcampo);
                db.SaveChanges();
                mensaje = "Exito al guardar tu configuracion";
                bandera = true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                bandera = false;
            }
            var obj = new { Mensaje = mensaje, Bandera = bandera };
            return Ok(obj);
        }

        [HttpGet]
        [Route("setDetalle")]
        public IHttpActionResult ActualizarDetalle(Guid Requi, int Idcampo, bool detalle)
        {
            string mensaje;
            bool bandera;
            try
            {
                var datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
                if (datos.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.IdTipoEstructura == 7).ToList();
                    foreach (var item in estructura)
                    {
                        ConfiguracionRequi caja = new ConfiguracionRequi();
                        caja.Campo = item.Nombre;
                        caja.IdRequi = Requi;
                        caja.Detalle = false;
                        caja.Resumen = false;
                        caja.R_D = 0;
                        caja.IdEstructura = item.Id;
                        db.ConfiguracionRequis.Add(caja);
                        db.SaveChanges();
                    }
                }
                var lista = datos.Where(e => e.IdEstructura == Idcampo).FirstOrDefault();
                lista.Detalle = detalle;
                lista.R_D = Bandera(Requi, Idcampo);
                db.SaveChanges();
                mensaje = "Exito al guardar tu configuracion";
                bandera = true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                bandera = false;
            }
            var obj = new { Mensaje = mensaje, Bandera = bandera };
            return Ok(obj);
        }

        [HttpGet]
        [Route("getGenerales")]
        public IHttpActionResult General(Guid Requi)
        {
            var datos = Listado(1, Requi);
            return Ok(datos);
        }


        [HttpGet]
        [Route("getContrato")]
        public IHttpActionResult Contrato(Guid Requi)
        {
            var datos = Listado(2, Requi);
            return Ok(datos);
        }


        [HttpGet]
        [Route("getPuestoReclutar")]
        public IHttpActionResult PuestoReclutar(Guid Requi)
        {
            var datos = Listado(3, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getHorario")]
        public IHttpActionResult Horario(Guid Requi)
        {
            var datos = Listado(4, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getsueldo")]
        public IHttpActionResult sueldo(Guid Requi)
        {
            var datos = Listado(5, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getOtros")]
        public IHttpActionResult Otros(Guid Requi)
        {
            var datos = Listado(6, Requi);
            return Ok(datos);
        }

        private int Bandera(Guid Requi, int Idcampo)
        {
            var datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi && e.IdEstructura == Idcampo).FirstOrDefault();
            if (datos.Resumen == true && datos.Detalle == true)
            {
                return 3;
            }

            if (datos.Resumen == false && datos.Detalle == true)
            {
                return 2;
            }

            if (datos.Resumen == true && datos.Detalle == false)
            {
                return 1;
            }

            return 0;
        }

        private List<listadoEstru> Listado(int Orden, Guid Requi)
        {

            List<listadoEstru> lista = new List<listadoEstru>();
            var datos = db.Estructuras.Where(a => a.Orden == Orden).ToList();
            var configuracion = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
            foreach (var item in datos)
            {
                listadoEstru pieza = new listadoEstru();
                pieza.Id = item.Id;
                pieza.Nombre = item.Nombre;
                pieza.Descripcion = item.Descripcion;
                pieza.Activo = item.Activo;
                pieza.Confidencial = item.Confidencial;
                pieza.DescripcionInclusivo = item.DescripcionInclusivo;
                pieza.Inclusivo = item.Inclusivo;
                pieza.Icono = item.Icono;
                pieza.Resumen = false;
                pieza.Detalle = false;
                if (configuracion.Count > 0)
                {
                    pieza.Resumen = configuracion.Where(e => e.IdEstructura == item.Id).Select(e => e.Resumen).FirstOrDefault();
                    pieza.Detalle = configuracion.Where(e => e.IdEstructura == item.Id).Select(e => e.Detalle).FirstOrDefault();
                }

                lista.Add(pieza);
            }
            return lista;
        }
        public class listadoEstru
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public bool Activo { get; set; }
            public bool Confidencial { get; set; }
            public string DescripcionInclusivo { get; set; }
            public bool Inclusivo { get; set; }
            public string Icono { get; set; }
            public bool? Resumen { get; set; }
            public bool? Detalle { get; set; }
        }
    }
} 
