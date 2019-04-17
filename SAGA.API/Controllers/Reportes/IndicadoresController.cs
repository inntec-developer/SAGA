using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SAGA.API.Controllers.Reportes
{
    [RoutePrefix("api/indicador")]
    public class IndicadoresController : ApiController
    {
        private SAGADBContext db = new SAGADBContext();

        [HttpGet]
        [Route("vcubierta")]
        public IHttpActionResult vcubierta(string usuario)
        {
            Guid id = new Guid(usuario);
            DateTime fecha = DateTime.Now.AddMonths(-3);
            var asigna = db.AsignacionRequis.Where(e => e.GrpUsrId == id).Select(e => e.RequisicionId).ToList();
            var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) && e.fch_Creacion > fecha).ToList();
            var cubierta = datos.Where(e => e.EstatusId == 34).ToList();
            var cubiertaParcialmente = datos.Where(e => e.EstatusId == 35).ToList();
            var cubiertaMedios = datos.Where(e => e.EstatusId == 36).ToList();
            var obj = new {
                cubiertas = cubierta.Count,
                Parcialmente = cubiertaParcialmente.Count,
                medios = cubiertaMedios.Count,
                total = 0 + cubierta.Count + cubiertaParcialmente.Count + cubiertaMedios.Count
            };
            return Ok(obj);
        }


        [HttpGet]
        [Route("vactiva")]
        public IHttpActionResult vactiva(string usuario)
        {
            Guid id = new Guid(usuario);
            DateTime fecha = DateTime.Now.AddMonths(-1);
            var asigna = db.AsignacionRequis.Where(e => e.GrpUsrId == id).Select(e => e.RequisicionId).ToList();
            var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) && e.fch_Creacion > fecha).ToList();
            int Nuevo = datos.Where(e => e.EstatusId == 4).ToList().Count;
            int Aprobada = datos.Where(e => e.EstatusId == 6).ToList().Count;
            int Publicada = datos.Where(e => e.EstatusId == 7).ToList().Count;
            int BusCandidatos = datos.Where(e => e.EstatusId == 29).ToList().Count;
            int EnvCliente = datos.Where(e => e.EstatusId == 30).ToList().Count;
            int NuBusqueda = datos.Where(e => e.EstatusId == 31).ToList().Count;
            int Socioeconomicos = datos.Where(e => e.EstatusId == 32).ToList().Count;
            int Espera = datos.Where(e => e.EstatusId == 33).ToList().Count;
            int Pausada = datos.Where(e => e.EstatusId == 39).ToList().Count;
            int Garantia = datos.Where(e => e.EstatusId == 38).ToList().Count;
            var obj = new {
                total = Nuevo + Aprobada + Publicada + BusCandidatos + EnvCliente + NuBusqueda + Socioeconomicos + Espera + Pausada + Garantia,
                Nuevo = Nuevo,
                Aprobada = Aprobada,
                Publicada = Publicada,
                BusCandidatos = BusCandidatos,
                EnvCliente = EnvCliente,
                NuBusqueda = NuBusqueda,
                Socioeconomicos = Socioeconomicos,
                Espera = Espera,
                Pausada = Pausada,
                Garantia = Garantia,
            };
            return Ok(obj);
        }

        [HttpGet]
        [Route("vporvencer")]
        public IHttpActionResult vporvencer(string usuario)
        {
            Guid id = new Guid(usuario);
            DateTime hoy = DateTime.Now;
            DateTime fecha = DateTime.Now.AddMonths(-1);
            DateTime vencida = DateTime.Now.AddDays(3);
            var asigna = db.AsignacionRequis.Where(e => e.GrpUsrId == id).Select(e => e.RequisicionId).ToList();
            var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) && e.fch_Creacion > fecha).ToList();
            var porVencer = datos.Where(e => e.fch_Cumplimiento <= vencida && e.fch_Cumplimiento > hoy).ToList().Count;
            var vencidas = datos.Where(e => e.fch_Cumplimiento < hoy).ToList().Count;
            var total = datos.Where(e => e.fch_Cumplimiento > vencida).ToList().Count;
            int Nuevo = datos.Where(e => e.EstatusId == 4).ToList().Count;
            int Aprobada = datos.Where(e => e.EstatusId == 6).ToList().Count;
            int Publicada = datos.Where(e => e.EstatusId == 7).ToList().Count;
            int BusCandidatos = datos.Where(e => e.EstatusId == 29).ToList().Count;
            int EnvCliente = datos.Where(e => e.EstatusId == 30).ToList().Count;
            int NuBusqueda = datos.Where(e => e.EstatusId == 31).ToList().Count;
            int Socioeconomicos = datos.Where(e => e.EstatusId == 32).ToList().Count;
            int Espera = datos.Where(e => e.EstatusId == 33).ToList().Count;
            int Pausada = datos.Where(e => e.EstatusId == 39).ToList().Count;
            int Garantia = datos.Where(e => e.EstatusId == 38).ToList().Count;
            var obj = new
            {
                vencidas = vencidas,
                porVencer = porVencer,
                total = total,
                Nuevo = Nuevo,
                Aprobada = Aprobada,
                Publicada = Publicada,
                BusCandidatos = BusCandidatos,
                EnvCliente = EnvCliente,
                NuBusqueda = NuBusqueda,
                Socioeconomicos = Socioeconomicos,
                Espera = Espera,
                Pausada = Pausada,
                Garantia = Garantia,
            };
            return Ok(obj);
        }


        [HttpGet]
        [Route("vvencida")]
        public IHttpActionResult vvencida(string usuario)
        {
            Guid id = new Guid(usuario);
            DateTime hoy = DateTime.Now;
            DateTime fecha = DateTime.Now.AddMonths(-1);
            DateTime vencida = DateTime.Now.AddDays(3);
            var asigna = db.AsignacionRequis.Where(e => e.GrpUsrId == id).Select(e => e.RequisicionId).ToList();
            var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) && e.fch_Creacion > fecha).ToList();
            var vencidas = datos.Where(e => e.fch_Cumplimiento < hoy).ToList().Count;
            var total = datos.Count;
            var obj = new
            {
                vencidas = vencidas,
                total = total
            };
            return Ok(obj);
        }

        [HttpGet]
        [Route("radial")]
        public IHttpActionResult radial(string usuario)
        {
            Guid id = new Guid(usuario);
            DateTime hoy = DateTime.Now;
            DateTime fecha = DateTime.Now.AddMonths(-1);

            var vacantes = db.ProcesoCandidatos.Where(e => e.ReclutadorId == id && e.Fch_Creacion > fecha).ToList();

            int entrevTotal = vacantes.Select(e => e.HorarioId).Distinct().ToList().Count;
            decimal entrevi = vacantes.Where(e => e.EstatusId == 18).ToList().Count;

            int finalistaTotal = entrevTotal;
            decimal finalista = vacantes.Where(e => e.EstatusId == 21).ToList().Count;

            int enviadoTotal = entrevTotal;
            decimal enviado = vacantes.Where(e => e.EstatusId == 22).ToList().Count;

            int aceptadoTotal = entrevTotal;
            decimal aceptado = vacantes.Where(e => e.EstatusId == 23).ToList().Count;

            int contraTotal = entrevTotal;
            decimal contrata = vacantes.Where(e => e.EstatusId == 24).ToList().Count;

            int rechazadoTotal = entrevTotal;
            decimal rechazado = vacantes.Where(e => e.EstatusId == 40).ToList().Count;
            
            //entrevTotal = 200;
            int totales = entrevTotal;
            entrevTotal = calculo(entrevi, entrevTotal);
            finalistaTotal = calculo(finalista, finalistaTotal);
            enviadoTotal = calculo(enviado, enviadoTotal);
            rechazadoTotal = calculo(rechazado, rechazadoTotal);
            aceptadoTotal = calculo(aceptado, aceptadoTotal);
            contraTotal = calculo(contrata, contraTotal);

            string total = entrevTotal.ToString().Substring(0, 1) + "0";
            string total2 = enviadoTotal.ToString().Substring(0, 1) + "0";
            string total3 = contraTotal.ToString().Substring(0, 1) + "0";
            var obj = new
            {
                entrevi = entrevi,
                entrevTotal = total,
                finalista = finalista,
                finaTotal = finalistaTotal,
                enviadoTotal = total2,
                enviado = enviado,
                aceptado = aceptado,
                acepTotal = aceptadoTotal,
                recha = rechazado,
                rechaTotal = rechazadoTotal,
                contraTotal = total3,
                contrata = contrata,
                total = totales,
            };
            return Ok(obj);
        }

        public int calculo(decimal a, int b)
        {
            if (a > 0)
            {
                decimal operacion = (a / b) * (100m);
                b = Convert.ToInt32(operacion);
                return b;
            }
            else
            {
                return 0;
            }
          
        }

        [HttpGet]
        [Route("resumen")]
        public IHttpActionResult resumen(string usuario)
        {
            Guid id = new Guid(usuario);
            DateTime hoy = DateTime.Now;
            DateTime fechaMes = DateTime.Now.AddMonths(-1);
            DateTime fecha5 = DateTime.Now.AddDays(-5);
            DateTime fecha10 = fecha5.AddDays(-5);
            DateTime fecha15 = fecha10.AddDays(-5);
            DateTime fecha20 = fecha15.AddDays(-5);
            DateTime fecha25 = fecha20.AddDays(-5);
            DateTime fecha30 = fecha25.AddDays(-5);
            var asigna = db.AsignacionRequis.Where(e => e.GrpUsrId == id && e.fch_Creacion > fechaMes).Select(e => e.RequisicionId).ToList();
           
            var proceso5 = db.ProcesoCandidatos.Where(e => asigna.Contains(e.RequisicionId) && e.Fch_Modificacion > fecha5 && e.Fch_Modificacion < fecha10).ToList();
            var proceso10 = db.ProcesoCandidatos.Where(e => asigna.Contains(e.RequisicionId) && e.Fch_Modificacion > fecha10 && e.Fch_Modificacion < fecha5).ToList();
            var proceso15 = db.ProcesoCandidatos.Where(e => asigna.Contains(e.RequisicionId) && e.Fch_Modificacion > fecha15 && e.Fch_Modificacion < fecha10).ToList();
            var proceso20 = db.ProcesoCandidatos.Where(e => asigna.Contains(e.RequisicionId) && e.Fch_Modificacion > fecha20 && e.Fch_Modificacion < fecha15).ToList();
            var proceso25 = db.ProcesoCandidatos.Where(e => asigna.Contains(e.RequisicionId) && e.Fch_Modificacion > fecha25 && e.Fch_Modificacion < fecha20).ToList();
            var proceso30 = db.ProcesoCandidatos.Where(e => asigna.Contains(e.RequisicionId) && e.Fch_Modificacion > fecha30 && e.Fch_Modificacion < fecha25).ToList();


            int dia5Entrevista = proceso5.Where(e => e.EstatusId == 18).ToList().Count;
            int dia5Enviado = proceso5.Where(e => e.EstatusId == 22).ToList().Count;
            int dia5Contratado = proceso5.Where(e => e.EstatusId == 24).ToList().Count;

            int dia10Entrevista = proceso5.Where(e => e.EstatusId == 18).ToList().Count;
            int dia10Enviado = proceso5.Where(e => e.EstatusId == 22).ToList().Count;
            int dia10Contratado = proceso5.Where(e => e.EstatusId == 24).ToList().Count;

            int dia15Entrevista = proceso5.Where(e => e.EstatusId == 18).ToList().Count;
            int dia15Enviado = proceso5.Where(e => e.EstatusId == 22).ToList().Count;
            int dia15Contratado = proceso5.Where(e => e.EstatusId == 24).ToList().Count;

            int dia20Entrevista = proceso5.Where(e => e.EstatusId == 18).ToList().Count;
            int dia20Enviado = proceso5.Where(e => e.EstatusId == 22).ToList().Count;
            int dia20Contratado = proceso5.Where(e => e.EstatusId == 24).ToList().Count;

            int dia25Entrevista = proceso5.Where(e => e.EstatusId == 18).ToList().Count;
            int dia25Enviado = proceso5.Where(e => e.EstatusId == 22).ToList().Count;
            int dia25Contratado = proceso5.Where(e => e.EstatusId == 24).ToList().Count;

            int dia30Entrevista = proceso5.Where(e => e.EstatusId == 18).ToList().Count;
            int dia30Enviado = proceso5.Where(e => e.EstatusId == 22).ToList().Count;
            int dia30Contratado = proceso5.Where(e => e.EstatusId == 24).ToList().Count;

            var obj = new
            {
                dia5Entrevista = dia5Entrevista,
                dia5Enviado = dia5Enviado,
                dia5Contratado = dia5Contratado,

                dia10Entrevista = dia10Entrevista,
                dia10Enviado = dia10Enviado,
                dia10Contratado = dia10Contratado,

                dia15Entrevista = dia15Entrevista,
                dia15Enviado = dia15Enviado,
                dia15Contratado = dia15Contratado,

                dia20Entrevista = dia20Entrevista,
                dia20Enviado = dia20Enviado,
                dia20Contratado = dia20Contratado,

                dia25Entrevista = dia25Entrevista,
                dia25Enviado = dia25Enviado,
                dia25Contratado = dia25Contratado,

                dia30Entrevista = dia30Entrevista,
                dia30Enviado = dia30Enviado,
                dia30Contratado = dia30Contratado
              
            };
            return Ok(obj);
        }

    }
}


        
