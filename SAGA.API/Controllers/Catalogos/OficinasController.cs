using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.BOL;
using SAGA.DAL;
using System.Text.RegularExpressions;

namespace SAGA.API.Controllers.Catalogos
{
    [RoutePrefix("api/Oficina")]
    public class OficinasController : ApiController
    {
        private SAGADBContext db = new SAGADBContext();

        [HttpGet]
        [Route("oficina")]
        public IHttpActionResult oficina(string filtro)
        {
            try
            {
                string cadena = "1,2,3,4,5";
                var array = cadena.Split(',');
                List<int> tipos = new List<int>();
                for (int i = 0; i < array.Length; i++)
                {
                    tipos.Add(Int32.Parse(array[i]));
                }

                var datos = db.OficinasReclutamiento.Where(e => tipos.Contains(e.TipoOficinaId)).Select(
                     caja => new
                     {
                        caja.Id,
                        nombre = db.Entidad.Where(e => e.Id == caja.Id).Select(e => e.Nombre).FirstOrDefault(),
                        caja.Latitud,
                        caja.Longitud,
                        caja.TipoOficina.tipoOficina,
                        caja.Activo,
                        cp = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().CodigoPostal,
                        Estado = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Estado.estado,
                        Estadoid = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Estado.Id,
                         Municipio = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Municipio.municipio,
                         Municipioid = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Municipio.Id,
                         Colonia = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Colonia.colonia,
                         coloniaid = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Colonia.Id,
                         Calle = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Calle,
                        NumeroExt = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().NumeroExterior,
                        Telefono = db.Telefonos.Where(e => e.EntidadId == caja.Id && e.esPrincipal == true).FirstOrDefault().telefono,
                        Extension = db.Telefonos.Where(e => e.EntidadId == caja.Id && e.esPrincipal == true).FirstOrDefault().Extension,
                        Correo = db.Emails.Where(e => e.EntidadId == caja.Id && e.esPrincipal == true).FirstOrDefault().email,
                     }).OrderBy(e => e.nombre).ToList();

                if (filtro != "" && filtro != null )
                {
                    filtro = filtro.ToUpper();
                    datos = datos.Where(a => Regex.IsMatch(a.nombre.ToUpper(), ".*"+filtro+".*")).ToList();
                //    datos = datos.Where(e => filtro.Contains(e.nombre)).ToList();
                }
              
                return Ok(datos);
            }
            catch (Exception ex)
            {
                var mensaje = ex;
            }

            return Ok("");
        }

        [HttpGet]
        [Route("add")]
        public IHttpActionResult agregar(string nombre, string estado, string municipio, string colonia, string cp,string calle, 
            string numero,string telefono, string email,string latitud, string longitud,string tipoOfi)
        {
            try
            {

                var ofi = new OficinaReclutamiento();
                ofi.Nombre = nombre;
                ofi.ApellidoPaterno = "";
                ofi.ApellidoMaterno = "";
                ofi.Activo = true;
                ofi.fch_Creacion = DateTime.Now;
                ofi.TipoOficinaId = Convert.ToInt32(tipoOfi);
                ofi.UnidadNegocioId = 1;
                ofi.Orden = 11;
                ofi.Latitud = latitud;
                ofi.Longitud = longitud;
                ofi.TipoEntidadId = 5;
                ofi.Foto = "";
                db.OficinasReclutamiento.Add(ofi);
                db.SaveChanges();

                var enti = db.Entidad.Where(e => e.Nombre == nombre && e.TipoEntidadId == 5).FirstOrDefault();

                var dire = new Direccion();
                dire.EntidadId = enti.Id;
                dire.Activo = true;
                dire.esMoral = true;
                dire.Calle = calle;
                dire.CodigoPostal = cp;
                dire.ColoniaId = Convert.ToInt32(colonia);
                dire.esPrincipal = true;
                dire.EstadoId = Convert.ToInt32(estado);
                dire.MunicipioId = Convert.ToInt32(municipio);
                dire.NumeroExterior = numero;
                dire.NumeroInterior = "S/N";
                dire.PaisId = 42;
                dire.TipoDireccionId = 3;
                dire.UsuarioAlta = "INNTEC";
                db.Direcciones.Add(dire);
                db.SaveChanges();

               

                var ema = new Email();
                ema.esPrincipal = true;
                ema.EntidadId = enti.Id;
                ema.fch_Creacion = DateTime.Now;
                ema.email = email;
                ema.UsuarioAlta = "INNTEC";
                db.Emails.Add(ema);
                db.SaveChanges();

                var tel = new Telefono();
                tel.esPrincipal = true;
                tel.EntidadId = enti.Id;
                tel.fch_Creacion = DateTime.Now;
                tel.Activo = true;
                tel.UsuarioAlta = "INNTEC";
                tel.ClaveLada = "33";
                tel.ClavePais = "52";
                tel.telefono = telefono;
                tel.TipoTelefonoId = 4;
                db.Telefonos.Add(tel);


               db.SaveChanges();
              

                return Ok("Éxito al agregar la oficina");
            }
            catch (Exception ex)
            {
                var mensaje = ex;
            }
            return Ok("Algo paso, intentelo mas tarde");
        }

        [HttpGet]
        [Route("editar")]
        public IHttpActionResult editar(string nombre, string estado, string municipio, string colonia, string cp, string calle,
              string numero, string telefono, string email, string latitud, string longitud, string tipoOfi, string activo, string id)
        {
            try
            {
                Guid ide = new Guid(id);
                var ofi = db.OficinasReclutamiento.Where(e=>e.Id == ide).FirstOrDefault();
                ofi.Nombre = nombre;
                ofi.Activo = Convert.ToBoolean(activo);
                ofi.TipoOficinaId = Convert.ToInt32(tipoOfi);
                ofi.Latitud = latitud;
                ofi.Longitud = longitud;

                var dire = db.Direcciones.Where(e=>e.EntidadId == ide).FirstOrDefault();
               
                dire.Calle = calle;
                dire.CodigoPostal = cp;
                dire.ColoniaId = Convert.ToInt32(colonia);
                dire.EstadoId = Convert.ToInt32(estado);
                dire.MunicipioId = Convert.ToInt32(municipio);
                dire.NumeroExterior = numero;

                var ema = db.Emails.Where(e => e.EntidadId == ide).FirstOrDefault();
                ema.email = email;

                var tel = db.Telefonos.Where(e => e.EntidadId == ide).FirstOrDefault();
                tel.telefono = telefono;
                db.SaveChanges();


                return Ok("Éxito al actualizar la oficina");
            }
            catch (Exception ex)
            {
                var mensaje = ex;
            }
            return Ok("Algo paso, intentelo mas tarde");
        }

        [HttpGet]
        [Route("delete")]
        public IHttpActionResult eliminar(string id)
        {
            try
            {
                Guid iden = new Guid(id);

                var dire = db.Direcciones.Where(e => e.EntidadId == iden).FirstOrDefault();
                db.Direcciones.Remove(dire);
                var ofi = db.OficinasReclutamiento.Where(e => e.Id == iden).FirstOrDefault();
                db.OficinasReclutamiento.Remove(ofi);
                var ema = db.Emails.Where(e => e.EntidadId == iden).FirstOrDefault();
                db.Emails.Remove(ema);
                var tel = db.Telefonos.Where(e => e.EntidadId == iden).FirstOrDefault();
                db.Telefonos.Remove(tel);
                var datos = db.OficinasReclutamiento.Where(e => e.Id == iden).FirstOrDefault();
                db.Entidad.Remove(datos);
                db.SaveChanges();
                return Ok("éxito al eliminar la oficina");
            }
            catch (Exception ex)
            {
                var mensaje = ex;
            }
            return Ok("Algo paso intentelo mas tarde");
        }

        [HttpGet]
        [Route("estado")]
        public IHttpActionResult estado(string id)
        {
                int iden = Convert.ToInt32(id);
                var datos = db.Estados.Where(e => e.PaisId == 42 ).Select(e=> new {e.estado, e.Id }).ToList();
            datos.Insert(0, new { estado = "Seleccione un estado", Id = 0 });
            if (iden != 0)
            {
                datos = datos.OrderByDescending(e => e.Id == iden).ThenBy(e => e.Id != iden).ToList();
            }
           
            return Ok(datos);
        }

        [HttpGet]
        [Route("municipio")]
        public IHttpActionResult municipio(string estado, string municipio)
        {
            int estadoid = Convert.ToInt32(estado);
            int id = Convert.ToInt32(municipio);
            var datos = db.Municipios.Where(e => e.EstadoId == estadoid).Select(e => new { e.municipio, e.Id }).ToList();
            datos.Insert(0, new { municipio = "Seleccione un municipio", Id = 0 });
            if (id != 0)
            {
                datos = datos.OrderByDescending(e => e.Id == id).ThenBy(e => e.Id != id).ToList();
            }
           
            return Ok(datos);
        }

        [HttpGet]
        [Route("colonia")]
        public IHttpActionResult colonia(string municipio, string colonia)
        {
            int municipioid = Convert.ToInt32(municipio);
            int id = Convert.ToInt32(colonia);
            var datos = db.Colonias.Where(e => e.MunicipioId == municipioid).Select(e => new { e.colonia, e.Id }).ToList();
            datos.Insert(0, new { colonia = "Seleccione una colonia", Id = 0 });
            if (id != 0)
            {
                datos = datos.OrderByDescending(e => e.Id == id).ThenBy(e => e.Id != id).ToList();
            }
            return Ok(datos);
        }


        //[HttpGet]
        //[Route("tipo")]
        //public IHttpActionResult tipo(string tipo)
        //{
           
        //    int id = Convert.ToInt32(tipo);
        //    var datos = db.of.Where(e => e.MunicipioId == municipioid).Select(e => new { e.colonia, e.Id }).ToList();
        //    if (id != 0)
        //    {
        //        datos = datos.Where(e => e.Id == id).ToList();
        //    }
        //    return Ok(datos);
        //}
      


    }
}
