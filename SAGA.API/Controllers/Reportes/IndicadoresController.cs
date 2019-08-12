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
            var ListaUsuario = new List<Guid>();
            ListaUsuario.Add(id);
            var arbol = db.Subordinados.Where(e => e.LiderId == id).ToList();
            if (arbol.Count > 0)
            {
                ListaUsuario.AddRange(arbol.Select(e => e.UsuarioId));
                foreach (var item in arbol)
                {
                    var hijos = db.Subordinados.Where(e => e.LiderId == item.UsuarioId).ToList();
                    if (hijos.Count > 0)
                    {
                        ListaUsuario.AddRange(hijos.Select(e => e.UsuarioId));
                    }
                }
            }

         //   DateTime fecha = DateTime.Now.AddMonths(-3);
            var asigna = db.ProcesoCandidatos.Where(e => ListaUsuario.Contains(e.ReclutadorId)).Select(e => e.RequisicionId).ToList();
            var datos2 = db.Requisiciones.Where(e => asigna.Contains(e.Id) && e.Activo == true).ToList();
            
            var datos = db.ProcesoCandidatos.Where(e => asigna.Contains(e.RequisicionId) ).ToList();

            int[] rango = new[] { 0, 1, 2, 3, 4, 5,6};
           
            int mes = 0;
            DateTime fechaInicio = DateTime.Now;
            DateTime fechaFinal = DateTime.Now;
            List<Dtos.Reporte.IndicadorDto> DatosLista = new List<Dtos.Reporte.IndicadorDto>();
            int cubiertaPos = 0;
            int mediosPos = 0;
            int parcialPos = 0;
            int clientePos = 0;
            int operacionPos = 0;
            int promocionPos = 0;

            foreach (var item in rango)
            {
               
                var lista = new Dtos.Reporte.IndicadorDto();
                lista.nombre = DateTime.Now.AddMonths(mes).ToString("MMMM");
                fechaInicio = new DateTime(fechaInicio.Year, DateTime.Now.AddMonths(mes).Month, 1);
                fechaFinal = new DateTime(fechaInicio.Year, fechaInicio.Month, fechaInicio.AddMonths(1).AddDays(-1).Day);
                asigna = datos.Where(e => e.Fch_Modificacion >= fechaInicio && e.Fch_Modificacion <= fechaFinal).Select(e => e.RequisicionId).ToList();
                lista.id = mes;

                var cubierID = datos2.Where(e => e.EstatusId == 34 && asigna.Contains(e.Id)).Select(e=>e.Id).ToList();
                lista.cubierta = 0;
                if (cubierID.Count > 0)
                {
                     cubiertaPos = db.HorariosRequis.Where(x => cubierID.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    lista.cubierta = db.ProcesoCandidatos.Where(e => e.EstatusId == 24 && cubierID.Contains(e.RequisicionId)).ToList().Count();
                }

                var mediosID = datos2.Where(e => e.EstatusId == 36 && asigna.Contains(e.Id)).Select(e => e.Id).ToList();
                lista.medios = 0;
                if (mediosID.Count > 0)
                {
                     mediosPos = db.HorariosRequis.Where(x => mediosID.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    lista.medios = db.ProcesoCandidatos.Where(e => e.EstatusId == 24 && mediosID.Contains(e.RequisicionId)).ToList().Count();
                }

                var parcialID = datos2.Where(e => e.EstatusId == 35 && asigna.Contains(e.Id)).Select(e => e.Id).ToList();
                lista.parcial = 0;
                if (parcialID.Count > 0)
                {
                     parcialPos = db.HorariosRequis.Where(x => parcialID.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    lista.parcial = db.ProcesoCandidatos.Where(e => e.EstatusId == 24 && parcialID.Contains(e.RequisicionId)).ToList().Count();
                }
                
                var clienteID = datos2.Where(e => e.EstatusId == 37 && asigna.Contains(e.Id)).Select(e => e.Id).ToList();
                lista.cliente = 0;
                if (clienteID.Count > 0)
                {
                     clientePos = db.HorariosRequis.Where(x => clienteID.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    lista.cliente = db.ProcesoCandidatos.Where(e => e.EstatusId == 24 && clienteID.Contains(e.RequisicionId)).ToList().Count();
                }

                var operacionID = datos2.Where(e => e.EstatusId == 48 && asigna.Contains(e.Id)).Select(e => e.Id).ToList();
                lista.operacion = 0;
                if (operacionID.Count > 0)
                {
                     operacionPos = db.HorariosRequis.Where(x => operacionID.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    lista.operacion = db.ProcesoCandidatos.Where(e => e.EstatusId == 24 && operacionID.Contains(e.RequisicionId)).ToList().Count();
                }

                var promocionID = datos2.Where(e => e.EstatusId == 47 && asigna.Contains(e.Id)).Select(e => e.Id).ToList();
                lista.promo = 0;
                if (promocionID.Count > 0)
                {
                    promocionPos = db.HorariosRequis.Where(x => promocionID.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    lista.promo = db.ProcesoCandidatos.Where(e => e.EstatusId == 24 && promocionID.Contains(e.RequisicionId)).ToList().Count();
                }

                lista.possicion = cubiertaPos + mediosPos + parcialPos + clientePos + operacionPos + promocionPos;
                lista.Totalcubierta = lista.cubierta + lista.medios + lista.parcial + lista.cliente + lista.operacion + lista.promo;
                DatosLista.Add(lista);
                mes--;
                 cubiertaPos = 0;
                 mediosPos = 0;
                 parcialPos = 0;
                 clientePos = 0;
                 operacionPos = 0;
                 promocionPos = 0;
            }

            DatosLista = DatosLista.OrderBy(e => e.id).ToList();


            return Ok(DatosLista);
        }


        [HttpGet]
        [Route("vactiva")]
        
        public IHttpActionResult vactiva(string usuario)
        {
            Guid id = new Guid(usuario);

            var ListaUsuario = new List<Guid>();
            ListaUsuario.Add(id);
            var arbol = db.Subordinados.Where(e => e.LiderId == id).ToList();
            if (arbol.Count > 0)
            {
                ListaUsuario.AddRange(arbol.Select(e => e.UsuarioId));
                foreach (var item in arbol)
                {
                    var hijos = db.Subordinados.Where(e => e.LiderId == item.UsuarioId).ToList();
                    if (hijos.Count > 0)
                    {
                        ListaUsuario.AddRange(hijos.Select(e => e.UsuarioId));
                    }
                }
            }

            DateTime fecha = DateTime.Now.AddMonths(-1);
            var asigna = db.AsignacionRequis.Where(e => ListaUsuario.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
            int[] EstatusList = new[] { 4,6,7,29,30,31,32,33,38,39 };
            var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || ListaUsuario.Contains(e.AprobadorId)).ToList();
            
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
            var Activalista = datos.Where(e => EstatusList.Contains(e.EstatusId)).Select(e=>e.Id).ToList();
            int numeropos = db.HorariosRequis.Where(x => Activalista.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);

            var requien = datos.Where(e=> EstatusList.Contains(e.EstatusId)).Select(e => e.Id).ToList();
            var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
            int cubierto = proseso.Where(e => e.EstatusId == 24).Distinct().ToList().Count;
            int faltante = 0;
            faltante = numeropos - cubierto;
            
            var apartado = proseso.Where(e => e.EstatusId == 12).Distinct().ToList();
            var cita = proseso.Where(e => e.EstatusId == 17 && e.EstatusId == 16).Distinct().ToList();
            var entrevista = proseso.Where(e => e.EstatusId == 18).Distinct().ToList();
            int[] evaID = new[] { 13, 14, 15 };
            var evaluacion = proseso.Where(e => evaID.Contains(e.EstatusId)).Distinct().ToList();
            var finalista = proseso.Where(e => e.EstatusId == 21).Distinct().ToList();
            var entrevistacliente = proseso.Where(e => e.EstatusId == 22).Distinct().ToList();
            var finalistacliente = proseso.Where(e => e.EstatusId == 23).Distinct().ToList();

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
                numeropos = numeropos,
                cubierto = cubierto,
                faltante = faltante,
                apartado = apartado.Count,
                cita = cita.Count,
                entrevista = entrevista.Count,
                evaluacion = evaluacion.Count,
                finalista = finalista.Count,
                entrecliente = entrevistacliente.Count,
                fincliente = finalistacliente.Count
            };
            return Ok(obj);
        }

        [HttpGet]
        [Route("vporvencer")]
        
        public IHttpActionResult vporvencer(string usuario)
        {
            Guid id = new Guid(usuario);

            var ListaUsuario = new List<Guid>();
            ListaUsuario.Add(id);
            var arbol = db.Subordinados.Where(e => e.LiderId == id).ToList();
            if (arbol.Count > 0)
            {
                ListaUsuario.AddRange(arbol.Select(e => e.UsuarioId));
                foreach (var item in arbol)
                {
                    var hijos = db.Subordinados.Where(e => e.LiderId == item.UsuarioId).ToList();
                    if (hijos.Count > 0)
                    {
                        ListaUsuario.AddRange(hijos.Select(e => e.UsuarioId));
                    }
                }
            }

            DateTime hoy = DateTime.Now;
            DateTime fecha = DateTime.Now.AddMonths(-1);
            DateTime vencida = DateTime.Now.AddDays(3);
            var asigna = db.AsignacionRequis.Where(e => ListaUsuario.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
            var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || ListaUsuario.Contains(e.PropietarioId)).ToList();
            int[] EstatusList = new[] { 7,24, 29, 30, 31,32,33,38,39 };
            var info = db.InformeRequisiciones.Where(e => asigna.Contains(e.RequisicionId)).Where(e => EstatusList.Contains(e.EstatusId)).Select(e=>e.RequisicionId).ToList();
            //var porVencer = datos.Where(e => e.fch_Cumplimiento <= vencida && e.fch_Cumplimiento > hoy).ToList().Count;
            //var vencidas = datos.Where(e => e.fch_Cumplimiento < hoy).ToList().Count;

            //int Nuevo = datos.Where(e => e.EstatusId == 4).ToList().Count;
            //int Aprobada = datos.Where(e => e.EstatusId == 6).ToList().Count;
            datos = datos.Where(e => !info.Contains(e.Id)).ToList();
            int Publicada = datos.Where(e => e.EstatusId == 7).ToList().Count;
            int BusCandidatos = datos.Where(e => e.EstatusId == 29).ToList().Count;
            int EnvCliente = datos.Where(e => e.EstatusId == 30).ToList().Count;
            int NuBusqueda = datos.Where(e => e.EstatusId == 31).ToList().Count;
            int Socioeconomicos = datos.Where(e => e.EstatusId == 32).ToList().Count;
            int Espera = datos.Where(e => e.EstatusId == 33).ToList().Count;
            int Pausada = datos.Where(e => e.EstatusId == 39).ToList().Count;
            int Garantia = datos.Where(e => e.EstatusId == 38).ToList().Count;
            int total = Publicada + BusCandidatos + EnvCliente + NuBusqueda + Socioeconomicos + Espera + Pausada + Garantia;

       //     var cubiertass = db.ProcesoCandidatos.Where(x => x.RequisicionId == e.Id && x.EstatusId == 24).Select(a => a.CandidatoId).Distinct().ToList().Count;

            var obj = new
            {
                //vencidas = vencidas,
                //porVencer = porVencer,
                total = total,
                //Nuevo = Nuevo,
                //Aprobada = Aprobada,
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
            var asigna = vacantes.Select(e => e.RequisicionId).ToList();

            int entrevTotal = db.HorariosRequis.Where(e => asigna.Contains(e.RequisicionId)).Sum(e =>e.numeroVacantes);
            decimal entrevi = db.InformeRequisiciones.Where(e => asigna.Contains(e.RequisicionId) && e.EstatusId == 18).Select(a=>a.CandidatoId).Distinct().ToList().Count;

            int finalistaTotal = entrevTotal;
            decimal finalista = db.InformeRequisiciones.Where(e => asigna.Contains(e.RequisicionId) && e.EstatusId == 21).Select(a => a.CandidatoId).Distinct().ToList().Count;

            int enviadoTotal = entrevTotal;
            decimal enviado = db.InformeRequisiciones.Where(e => asigna.Contains(e.RequisicionId) && e.EstatusId == 22).Select(a => a.CandidatoId).Distinct().ToList().Count;

            int aceptadoTotal = entrevTotal;
            decimal aceptado = db.InformeRequisiciones.Where(e => asigna.Contains(e.RequisicionId) && e.EstatusId == 23).Select(a => a.CandidatoId).Distinct().ToList().Count;

            int contraTotal = entrevTotal;
            decimal contrata = db.InformeRequisiciones.Where(e => asigna.Contains(e.RequisicionId) && e.EstatusId == 24).Select(a => a.CandidatoId).Distinct().ToList().Count;

            int rechazadoTotal = entrevTotal;
            decimal rechazado = db.InformeRequisiciones.Where(e => asigna.Contains(e.RequisicionId) && e.EstatusId == 40).Select(a => a.CandidatoId).Distinct().ToList().Count;

            //entrevTotal = 200;
            int totales = entrevTotal;
            entrevTotal = calculo(entrevi, entrevTotal);
            finalistaTotal = calculo(finalista, finalistaTotal);
            enviadoTotal = calculo(enviado, enviadoTotal);
            rechazadoTotal = calculo(rechazado, rechazadoTotal);
            aceptadoTotal = calculo(aceptado, aceptadoTotal);
            contraTotal = calculo(contrata, contraTotal);

            string total = pintado(entrevTotal);
            string total2 = pintado(enviadoTotal); 
            string total3 = pintado(contraTotal); 
            string total4 = pintado(finalistaTotal); 
            string total5 = pintado(aceptadoTotal); 
            string total6 = pintado(rechazadoTotal);
            var obj = new
            {
                entrevi = entrevi,
                entrevTotal = total,
                finalista = finalista,
                finaTotal = total4,
                enviadoTotal = total2,
                enviado = enviado,
                aceptado = aceptado,
                acepTotal = total5,
                recha = rechazado,
                rechaTotal = total6,
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
                if (b < 10)
                {
                    b = 10;
                }
                return b;
            }
            else
            {
                return 0;
            }
        }

        public string pintado(int a)
        {
            if (a == 100)
            {
                return "100";
            }
            else
            {
                return a.ToString().Substring(0, 1) + "0";
            }
        }

        [HttpGet]
        [Route("captadocontra")]
        public IHttpActionResult captadoContra(string usuario)
        {
            Guid id = new Guid(usuario);
           
            var ListaUsuario = new List<Guid>();
            ListaUsuario.Add(id);
            var arbol = db.Subordinados.Where(e => e.LiderId == id).ToList();
            if (arbol.Count > 0)
            {
                ListaUsuario.AddRange(arbol.Select(e => e.UsuarioId));
                foreach (var item in arbol)
                {
                    var hijos = db.Subordinados.Where(e => e.LiderId == item.UsuarioId).ToList();
                    if (hijos.Count > 0)
                    {
                        ListaUsuario.AddRange(hijos.Select(e => e.UsuarioId));
                    }
                }
            }

            var datos = db.ProcesoCandidatos.Where(e=> ListaUsuario.Contains(e.ReclutadorId)).ToList();
            var capta = datos.Select(e => e.CandidatoId).Distinct().ToList();
            var contra = datos.Where(e => e.EstatusId == 24).Distinct().ToList();
            int[] rango = new[] { 1, 2, 3, 4, 5, 6,7 };
            int dia = 0;
        //    int[] enviados = new[] { 1, 2, 3, 4, 5, 6 };
            int mes = 0;
            DateTime fechaInicio = DateTime.Now;
            DateTime fechaFinal = DateTime.Now;
            List<ReportesController.proactividad> Captados = new List<ReportesController.proactividad>();
            foreach (var item in rango)
            {
                var obj = new ReportesController.proactividad();
                obj.porcentaje = mes;
                obj.nombre = DateTime.Now.AddMonths(mes).ToString("MMMM");
                fechaInicio = new DateTime(fechaInicio.Year, DateTime.Now.AddMonths(mes).Month, 1);
                dia = fechaInicio.AddMonths(1).AddDays(-1).Day;
                fechaFinal = new DateTime(fechaInicio.Year, fechaInicio.Month,dia);
                var MesLista = datos.Where(e => e.Fch_Modificacion >= fechaInicio && e.Fch_Modificacion <= fechaFinal).ToList();
                obj.numeropos = MesLista.Select(e => e.CandidatoId).Distinct().Count();
                obj.puntaje = MesLista.Where(e => e.EstatusId == 22 || e.EstatusId == 23 || e.EstatusId == 24).Select(e => e.CandidatoId).Distinct().Count();
                obj.cubiertas = MesLista.Where(e => e.EstatusId == 24).Select(e => e.CandidatoId).Distinct().Count();
                Captados.Add(obj);
                mes--;
            }

            Captados = Captados.OrderBy(e=>e.porcentaje).ToList();


            return Ok(Captados);
        }

        [HttpGet]
        [Route("productividad")]
        public IHttpActionResult productividad(string usuario)
        {
            
            int[] Status = new[] { 34, 35, 36, 37, 47, 48 };
            var listaRequi = db.EstatusRequisiciones.Where(e => Status.Contains(e.EstatusId)).Select(e => e.RequisicionId).ToList().Distinct();

            var candidatos = db.AsignacionRequis.Where(e => listaRequi.Contains(e.RequisicionId)).ToList();
            var recluta = candidatos.Select(e => new { e.GrpUsrId }).Distinct().ToList();
            var vacantes = db.Requisiciones.Where(e => listaRequi.Contains(e.Id) && Status.Contains(e.EstatusId)).ToList();
            var aprovador = vacantes.Select(e => e.AprobadorId).ToList();
            recluta = recluta.Where(e => !aprovador.Contains(e.GrpUsrId)).ToList();
            var recluta2 = recluta.Select(e => e.GrpUsrId).ToList();
            var datos = db.Usuarios.Where(e => recluta2.Contains(e.Id)).ToList();
            try
            {

                List<ReportesController.proactividad> ProActi = new List<ReportesController.proactividad>();

                foreach (var item in datos)
                {
                    var obj = new ReportesController.proactividad();
                    var listaRequien = vacantes.Select(e => e.Id).ToList();
                    var ListaCubierta = db.ProcesoCandidatos.Where(e => e.ReclutadorId == item.Id && e.EstatusId == 24 && listaRequien.Contains(e.RequisicionId)).ToList();
                    var listaPosicion = db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && e.GrpUsrId == item.Id).Select(e => e.RequisicionId).ToList();

                    obj.vacantes = db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && e.GrpUsrId == item.Id).ToList().Count();
                    try
                    {
                        obj.numeropos = db.HorariosRequis.Where(x => listaPosicion.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    }
                    catch (Exception)
                    {
                        obj.numeropos = 0;
                    }

                    try
                    {
                        obj.cubiertas = ListaCubierta.Count();
                        obj.puntaje = new ReportesController.Calculo().PuntajeCalculo(item.Id, ListaCubierta);

                    }
                    catch (Exception)
                    {
                        obj.cubiertas = 0;
                    }
                    obj.nombre = item.Nombre + " " + item.ApellidoPaterno + " " + item.ApellidoMaterno;
                    obj.id = item.Id;

                    ProActi.Add(obj);
                }
                var datal = ProActi.ToList();
                return Ok(datal);
            }
            catch (Exception eror)
            {
                var algo = eror;
            }
            return Ok("Por el momento el servidor no responde");
        }

        [HttpGet]
        [Route("Coordinacion")]
        public IHttpActionResult Coordinacion(string usuario)
        {
            Guid id = new Guid(usuario);

            var ListaUsuario = new List<Guid>();
            ListaUsuario.Add(id);
            var arbol = db.Subordinados.Where(e => e.LiderId == id).ToList();
            if (arbol.Count > 0)
            {
                ListaUsuario.AddRange(arbol.Select(e => e.UsuarioId));
                foreach (var item in arbol)
                {
                    var hijos = db.Subordinados.Where(e => e.LiderId == item.UsuarioId).ToList();
                    if (hijos.Count > 0)
                    {
                        ListaUsuario.AddRange(hijos.Select(e => e.UsuarioId));
                    }
                }
            }
            var asigna = db.AsignacionRequis.Where(e => ListaUsuario.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
            int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39 };
            var requi = db.Requisiciones.Where(e=> asigna.Contains(e.Id) && EstatusList.Contains(e.EstatusId)).ToList();
            var masivo = requi.Where(e => e.ClaseReclutamientoId == 3 && e.Activo == true).Select(e => new {
                e.Id,
                e.EstatusId
            }).ToList();

            var operativo = requi.Where(e => e.ClaseReclutamientoId == 2 && e.Activo == true).Select(e => new {
                e.Id,
                e.EstatusId
            }).ToList();

            var ezpeciali = requi.Where(e => e.ClaseReclutamientoId == 1 && e.Activo == true).Select(e => new {
                e.Id,
                e.EstatusId
            }).ToList();
            var masivolist = masivo.Select(x => x.Id).ToList();
            var operativolist = operativo.Select(x => x.Id).ToList();
            var especiallist = ezpeciali.Select(x => x.Id).ToList();
            int masivopos = db.HorariosRequis.Where(e => masivolist.Contains(e.RequisicionId)).Sum(e => e.numeroVacantes);
            int operativopos = db.HorariosRequis.Where(e => operativolist.Contains(e.RequisicionId)).Sum(e => e.numeroVacantes);
            int especialpos = db.HorariosRequis.Where(e => especiallist.Contains(e.RequisicionId)).Sum(e => e.numeroVacantes);

            var datos = new {
                    masivo = masivo.Count,
                    masivopos = masivopos,
                    operativo = operativo.Count,
                    operativopos = operativopos,
                    ezpecial = ezpeciali.Count,
                    especialpos = especialpos,
                };
                return Ok(datos);
        }

        [HttpGet]
        [Route("resumen")]
        public IHttpActionResult resumen(string usuario)
        {
            Guid id = new Guid(usuario);
            DateTime hoy = DateTime.Now;
            DateTime fechaMes = DateTime.Now.AddMonths(-1);
            DateTime fechaMes2 = DateTime.Now.AddMonths(-2);
            DateTime fechaMes3 = DateTime.Now.AddMonths(-3);

            var vacante1 = db.ProcesoCandidatos.Where(e => e.ReclutadorId == id && e.Fch_Creacion > fechaMes).Select(e=>e.RequisicionId).ToList();
            var vacante2 = db.ProcesoCandidatos.Where(e => e.ReclutadorId == id && e.Fch_Creacion > fechaMes2 && e.Fch_Creacion <= fechaMes).Select(e => e.RequisicionId).ToList();
            var vacante3 = db.ProcesoCandidatos.Where(e => e.ReclutadorId == id && e.Fch_Creacion > fechaMes3 && e.Fch_Creacion <= fechaMes2).Select(e => e.RequisicionId).ToList();
            var mes1 = db.InformeRequisiciones.Where(e => vacante1.Contains(e.RequisicionId)).ToList();
            var mes2 = db.InformeRequisiciones.Where(e => vacante2.Contains(e.RequisicionId)).ToList();
            var mes3 = db.InformeRequisiciones.Where(e => vacante3.Contains(e.RequisicionId)).ToList();

            int dia5Entrevista = mes1.Where(e => e.EstatusId == 18).Select(a => a.CandidatoId).Distinct().ToList().Count;
            int dia5Enviado = mes1.Where(e => e.EstatusId == 22).Select(a => a.CandidatoId).Distinct().ToList().Count;
            int dia5Contratado = mes1.Where(e => e.EstatusId == 24).Select(a => a.CandidatoId).Distinct().ToList().Count;

            int dia10Entrevista = mes2.Where(e => e.EstatusId == 18).Select(a => a.CandidatoId).Distinct().ToList().Count;
            int dia10Enviado = mes2.Where(e => e.EstatusId == 22).Select(a => a.CandidatoId).Distinct().ToList().Count;
            int dia10Contratado = mes2.Where(e => e.EstatusId == 24).Select(a => a.CandidatoId).Distinct().ToList().Count;

            int dia15Entrevista = mes3.Where(e => e.EstatusId == 18).Select(a => a.CandidatoId).Distinct().ToList().Count;
            int dia15Enviado = mes3.Where(e => e.EstatusId == 22).Select(a => a.CandidatoId).Distinct().ToList().Count;
            int dia15Contratado = mes3.Where(e => e.EstatusId == 24).Select(a => a.CandidatoId).Distinct().ToList().Count;


            var obj = new
            {
                mes1Entrevista = dia5Entrevista,
                mes1Enviado = dia5Enviado,
                mes1Contratado = dia5Contratado,

                mes2Entrevista = dia10Entrevista,
                mes2Enviado = dia10Enviado,
                mes2Contratado = dia10Contratado,

                mes3Entrevista = dia15Entrevista,
                mes3Enviado = dia15Enviado,
                mes3Contratado = dia15Contratado,
                
            };
            return Ok(obj);
        }

    }
}


        
