using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Controllers.Component;
using SAGA.DAL;
using SAGA.BOL;
using System.Data.Entity;
using System.Data.SqlClient;
using SAGA.API.Dtos;
using SAGA.API.Utilerias;
using SAGA.API.Dtos.SistTickets;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/SistTickets")]
    public class TicketsController : ApiController
    {
        public readonly HashSet<string> PalabrasInconvenientes = new HashSet<string>
        {
            "BACA", "BAKA", "BUEI", "BUEY",
            "CACA", "CACO", "CAGA", "CAGO", "CAKA", "CAKO", "COGE", "COGI", "COJA", "COJE", "COJI", "COJO", "COLA", "CULO",
            "FALO", "FETO",
            "GETA", "GUEI", "GUEY",
            "JETA", "JOTO",
            "KACA", "KACO", "KAGA", "KAGO", "KAKA", "KAKO", "KOGE", "KOGI", "KOJA", "KOJE", "KOJI", "KOJO", "KOLA", "KULO",
            "LILO", "LOCA", "LOCO", "LOKA", "LOKO",
            "MAME", "MAMO", "MEAR", "MEAS", "MEON", "MIAR", "MION", "MOCO", "MOKO", "MULA", "MULO",
            "NACA", "NACO",
            "PEDA", "PEDO", "PENE", "PIPI", "PITO", "POPO", "PUTA", "PUTO",
            "QULO",
            "RATA", "ROBA", "ROBE", "ROBO", "RUIN",
            "SENO",
            "TETA",
            "VACA", "VAGA", "VAGO", "VAKA", "VUEI", "VUEY",
            "WUEI", "WUEY"
        };


        private SAGADBContext db;

        Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
        public TicketsController()
        {
            db = new SAGADBContext();
        }

        /// <summary>
        ///     Calcula el codigo verificador en base a la pre CURP.
        /// </summary>
        /// <param name="preCURP"> La pre CURP.</param>
        /// <returns> El código verificador.</returns>
        /// <exception cref="ArgumentException"> Cuando alguno de los caracteres de la pre CURP no es válido.</exception>
        private static int CodigoVerificador(string preCURP)
        {
            var contador = 18;
            var sumatoria = 0;

            // Por cada caracter
            foreach (var caracter in preCURP)
            {
                int valor;

                switch (caracter)
                {
                    case '0':
                        valor = 0 * contador;
                        break;
                    case '1':
                        valor = 1 * contador;
                        break;
                    case '2':
                        valor = 2 * contador;
                        break;
                    case '3':
                        valor = 3 * contador;
                        break;
                    case '4':
                        valor = 4 * contador;
                        break;
                    case '5':
                        valor = 5 * contador;
                        break;
                    case '6':
                        valor = 6 * contador;
                        break;
                    case '7':
                        valor = 7 * contador;
                        break;
                    case '8':
                        valor = 8 * contador;
                        break;
                    case '9':
                        valor = 9 * contador;
                        break;
                    case 'A':
                        valor = 10 * contador;
                        break;
                    case 'B':
                        valor = 11 * contador;
                        break;
                    case 'C':
                        valor = 12 * contador;
                        break;
                    case 'D':
                        valor = 13 * contador;
                        break;
                    case 'E':
                        valor = 14 * contador;
                        break;
                    case 'F':
                        valor = 15 * contador;
                        break;
                    case 'G':
                        valor = 16 * contador;
                        break;
                    case 'H':
                        valor = 17 * contador;
                        break;
                    case 'I':
                        valor = 18 * contador;
                        break;
                    case 'J':
                        valor = 19 * contador;
                        break;
                    case 'K':
                        valor = 20 * contador;
                        break;
                    case 'L':
                        valor = 21 * contador;
                        break;
                    case 'M':
                        valor = 22 * contador;
                        break;
                    case 'N':
                        valor = 23 * contador;
                        break;
                    case 'Ñ':
                        valor = 24 * contador;
                        break;
                    case 'O':
                        valor = 25 * contador;
                        break;
                    case 'P':
                        valor = 26 * contador;
                        break;
                    case 'Q':
                        valor = 27 * contador;
                        break;
                    case 'R':
                        valor = 28 * contador;
                        break;
                    case 'S':
                        valor = 29 * contador;
                        break;
                    case 'T':
                        valor = 30 * contador;
                        break;
                    case 'U':
                        valor = 31 * contador;
                        break;
                    case 'V':
                        valor = 32 * contador;
                        break;
                    case 'W':
                        valor = 33 * contador;
                        break;
                    case 'X':
                        valor = 34 * contador;
                        break;
                    case 'Y':
                        valor = 35 * contador;
                        break;
                    case 'Z':
                        valor = 36 * contador;
                        break;
                    default:
                        throw new ArgumentException($"Caracter invalido en la compisicion de la pre CURP. [{caracter}]");
                }

                contador--;
                sumatoria = sumatoria + valor;
            }

            // 12.- 2do digito verificador
            var numVer = sumatoria % 10;
            numVer = 10 - numVer;
            numVer = numVer == 10 ? 0 : numVer;

            return numVer;
        }

        [HttpGet]
        [Route("getEstados")]
        public IHttpActionResult GetEstados()
        {
            try
            {
                var estados = db.Estados.Where(x => x.Id != 0).Select(E => new { id = E.Id, estado = E.estado, clave = E.Clave }).ToArray();

                return Ok(estados);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("InsertTicketRecl")]
        public IHttpActionResult InsertTicketRecl(Guid Ticket, Guid ReclutadorId, int ModuloId)
        {
            try
            {
                PostulateVacantController obj = new PostulateVacantController();
                ProcesoDto dto = new ProcesoDto();
                Ticket t = new Ticket();
                TicketReclutador tr = new TicketReclutador();

                tr.ReclutadorId = ReclutadorId;
                tr.TicketId = Ticket;
                tr.fch_Atencion = DateTime.Now;
                tr.fch_Final = DateTime.Now;

                db.TicketsReclutador.Add(tr);
                db.SaveChanges();

                dto.ReclutadorId = ReclutadorId;

                t = db.Tickets.Find(Ticket);

                db.Entry(t).State = EntityState.Modified;
                t.Estatus = 2;
                t.ModuloId = ModuloId;

                db.SaveChanges();

                dto.candidatoId = t.CandidatoId;

                var horario = db.HorariosRequis.Where(x => x.RequisicionId.Equals(t.RequisicionId)).Select(H => H.Id).FirstOrDefault();

                dto.horarioId = horario;
                dto.requisicionId = t.RequisicionId;
                dto.estatusId = 18; //entrevista reclutamiento

                obj.UpdateStatus(dto);
 
                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("ticketConCita")]
        public IHttpActionResult TicketConCita(int folio)
        {
            try
            {
                DateTime endDate = DateTime.Now.AddDays(1);

                var cita = (from items in db.CalendarioCandidato
                           where items.Folio.Equals(folio) 
                           select new { id= items.Id, activa = items.Estatus, fecha = items.Fecha, requisicionId = items.RequisicionId, candidatoId = items.CandidatoId }).ToList();

                if (cita.Count() > 0 && cita[0].fecha.Date == DateTime.Now.Date)
                {
                    if (cita[0].activa == 1)
                    {
                        if (cita[0].fecha.Date < endDate.Date && cita[0].fecha.Date >= DateTime.Now.Date && cita[0].fecha.Hour + 1 > DateTime.Now.Hour)
                        {

                            //if ((cita.fecha - DateTime.Now).TotalMinutes <= -15 || (cita.fecha - DateTime.Now).TotalMinutes >= 15)
                            //var cita = db.CalendarioCandidato.Where(x => x.Folio.Equals(folio)).Select(T => new
                            //{
                            //    requisicionId = T.RequisicionId,
                            //    candidatoId = T.CandidatoId
                            //}).FirstOrDefault();

                            //if (cita != null)
                            //{
                            Ticket ticket = new Ticket();
                            ticket.CandidatoId = cita[0].candidatoId;
                            ticket.RequisicionId = cita[0].requisicionId;
                            ticket.MovimientoId = 1;
                            ticket.ModuloId = 1;
                            ticket.Estatus = 1;

                            var num = db.Tickets.Where(x => x.RequisicionId.Equals(ticket.RequisicionId) && x.MovimientoId.Equals(1)).Count();

                            var f = db.Requisiciones.Where(x => x.Id.Equals(ticket.RequisicionId)).Select(ff => ff.Folio).FirstOrDefault().ToString();

                            ticket.Numero = "CC-" + f.Substring(f.Length - 4, 4) + '-' + num.ToString().PadLeft(3, '0');

                            db.Tickets.Add(ticket);

                            db.SaveChanges();

                            var folioCita = db.CalendarioCandidato.Find(cita[0].id);

                            db.Entry(folioCita).Property(x => x.Estatus).IsModified = true;

                            folioCita.Estatus = 0;

                            db.SaveChanges();

                            return Ok(ticket.Numero);
                        }
                        else
                        {
                            return Ok(HttpStatusCode.NoContent); //204 se perdío la cita por la hora
                        }
                    }
                    else
                    {
                        return Ok(HttpStatusCode.OK); //ya se imprimio ticket para esa cita
                    }
                }
                else
                {
                    return Ok(HttpStatusCode.NotFound); //404 no se encontró la cita
                 
                }
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("ticketSinCita")]
        public IHttpActionResult TicketSinCita(Guid requisicionId, Guid candidatoId)
        {
            try
            {
                Ticket ticket = new Ticket();
                ticket.CandidatoId = candidatoId;
                    //new Guid("1FD57341-F35D-E811-80E1-9E274155325E"); //pablo
                //ticket.CandidatoId = new Guid("F66DA23E-9D69-E811-80E1-9E274155325E"); //coni
                ticket.RequisicionId = requisicionId;
                ticket.MovimientoId = 2;
                ticket.ModuloId = 1;
                ticket.Estatus = 1;

                var num = db.Tickets.Where(x => x.RequisicionId.Equals(requisicionId) && x.MovimientoId.Equals(2)).Count();

                var folio = db.Requisiciones.Where(x => x.Id.Equals(requisicionId)).Select(f => f.Folio).FirstOrDefault().ToString();

                ticket.Numero = "SC-" + folio.Substring(folio.Length - 4, 4) + '-' + num.ToString().PadLeft(3, '0');
             
                db.Tickets.Add(ticket);

                db.SaveChanges();

                if(candidatoId != auxID)
                {
                    Postulacion obj = new Postulacion();
                    obj.CandidatoId = candidatoId;
                    obj.fch_Postulacion = DateTime.Now;
                    obj.RequisicionId = requisicionId;
                    obj.StatusId = 1;

                    db.Postulaciones.Add(obj);
                    db.SaveChanges();

                    InfoCandidatoController O = new InfoCandidatoController();
                    ProcesoCandidato obj2 = new ProcesoCandidato();
                    obj2.CandidatoId = candidatoId;
                    obj2.RequisicionId = requisicionId;

                    O.ApartarCandidato(obj2);
                    
                }
                return Ok(ticket.Numero);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        private static string Filtrar(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            // Nombres, paterno y materno en mayuscula
            str = str.ToUpper();

            // Eliminar acentos en vocales
            str = str.RemoveAccentMarks();

            // Eliminar dieresis en vocales
            str = str.RemoveVowelDieresis();

            // Criterios de excepcion
            var palabras = str.Split(' ')
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();

            // Preposición, conjunción, contraccion
            var arr_1 = new[] { "DA", "DAS", "DE", "DEL", "DER", "DI", "DIE", "DD", "EL", "LA", "LOS", "LAS", "LE", "LES", "MAC", "MC", "VAN", "VON", "Y", "J", "MA" };

            palabras = palabras.Where(i => !arr_1.Contains(i))
                .ToList();

            // Nombre compuesto
            var arr_2 = new[] { "MARIA", "MA.", "MA", "JOSE", "J", "J." };

            if (palabras.Count >= 2 && arr_2.Contains(palabras[0]))
            {
                palabras.RemoveAt(0);
            }

            // Caracteres especiales
            str = palabras[0]
                .Replace('/', 'X')
                .Replace('-', 'X')
                .Replace('.', 'X');

            return str;
        }

        public string CalcularCURP(CandidatosGralDto datos)
        {
            var nombres = Filtrar(datos.Nombre);
            var paterno = Filtrar(datos.ApellidoPaterno);
            var materno = Filtrar(datos.ApellidoMaterno);

            // Posicion 1-4
            var uno = paterno[0] == 'Ñ' ? 'X' : paterno[0];
            var dos = paterno.InternalVowel(1) ?? 'X';
            var tres = string.IsNullOrWhiteSpace(materno) ? 'X' : (materno[0] == 'Ñ' ? 'X' : materno[0]);
            var cuatro = nombres[0] == 'Ñ' ? 'X' : nombres[0];

            var fecha = $"{datos.FechaNac:yy}{datos.FechaNac.Month:D2}{datos.FechaNac.Day:D2}";
            var sexo = datos.GeneroId == 1 ? 'M' : 'H';
            var estado = db.Estados.Where(e => e.Id.Equals(datos.EstadoNacimientoId)).Select(E => E.Clave).FirstOrDefault().ToString();

            // Posicion 14-16
            var x = paterno.InternalConsonant(1);
            var y = materno?.InternalConsonant(1);
            var z = nombres.InternalConsonant(1);

            var catorce = x == null ? 'X' : (x == 'Ñ' ? 'X' : x);
            var quince = y == null ? 'X' : (y == 'Ñ' ? 'X' : y);
            var dieciseis = z == null ? 'X' : (z == 'Ñ' ? 'X' : z);

            // Pre CURP
            var preCURP = $"{uno}{dos}{tres}{cuatro}{fecha}{sexo}{estado}{catorce}{quince}{dieciseis}";

            // Reemplaza el 2do caracter por una X donde comience con alguna de las palabras de la lisa de "Palabras Inconvenientes"
            if (this.PalabrasInconvenientes.Contains(preCURP.Substring(0, 4)))
            {
                preCURP = preCURP[0] + "X" + preCURP.Substring(2);
            }

            // Digito diferenciador de homonimia y siglo
            var diferenciador = datos.FechaNac.Year < 2000 ? "0" : "A";

            // Digito verificador
            var codigoVerificador = CodigoVerificador(preCURP);

            return $"{preCURP}{diferenciador}{codigoVerificador}";
        }

        [HttpPost]
        [Route("registrarCandidato")]
        public IHttpActionResult RegistrarCandidato(CandidatosGralDto datos)
        {
            try
            {
                AspNetUsers usuario = new AspNetUsers();
                var username = "";
                var pass = datos.Nombre.Substring(0,1).ToLower() + datos.ApellidoPaterno.Trim().ToLower();
                if(datos.OpcionRegistro == 1)
                {
                    username = datos.Email[0].email.ToString();
                }
                else
                {
                    username = datos.Telefono[0].telefono.ToString();
                }
                usuario.Id = Guid.NewGuid().ToString();
                usuario.PhoneNumber = datos.Telefono[0].telefono.ToString();
                usuario.Clave = "00000";
                usuario.Pasword = pass;
                usuario.RegistroClave = DateTime.Now;
                usuario.PhoneNumberConfirmed = false;
                usuario.EmailConfirmed = true;
                usuario.LockoutEnabled = false;
                usuario.AccessFailedCount = 0;
                usuario.Email = datos.Email[0].email.ToString();
                usuario.UserName = username;
                usuario.Activo = 0;
               
                db.AspNetUsers.Add(usuario);
                db.SaveChanges();

                var add = db.Database.ExecuteSqlCommand("spEncriptarPasword @id", new SqlParameter("id", usuario.Id));

                var candidato = new Candidato();

                candidato.CURP = CalcularCURP(datos);
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

                PerfilCandidato PC = new PerfilCandidato();

                PC.CandidatoId = candidato.Id;
                PC.Estatus = 41;

                db.PerfilCandidato.Add(PC);

                db.SaveChanges();

                var t = db.AspNetUsers.Find(usuario.Id);
                db.Entry(t).Property(x => x.IdPersona).IsModified = true;

                t.IdPersona = candidato.Id;
                db.SaveChanges();

                LoginDto login = new LoginDto();
                login.Id = candidato.Id;
                login.username = username;
                login.pass = pass;

                return Ok(login);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }


        [HttpGet]
        [Route("loginBolsa")]
        public IHttpActionResult LoginBolsa(string usuario, string pass)
        {
            try
            {

                var p = db.AspNetUsers.Where(x => x.UserName.Equals(usuario)).Select(U => new { id = U.Id, personaId = U.IdPersona, password = U.Pasword,
                        usuario = db.Entidad.Where(x => x.Id.Equals(U.IdPersona)).Select(u => u.Nombre + " " + u.ApellidoPaterno + " " + u.ApellidoMaterno).FirstOrDefault()
                }).ToList();
                
                if(p.Count > 0)
                {
                    var pd = db.Database.SqlQuery<String>("dbo.spDesencriptarPasword @id", new SqlParameter("id", p[0].id)).FirstOrDefault();
                    if(pd.Equals(pass))
                    {
                        return Ok(p);
                    }
                    else
                    {
                        return Ok(HttpStatusCode.Ambiguous); //300 diferentes contraseñas
                    }
                }
                else
                {
                    return Ok(HttpStatusCode.NotFound); //404
                }
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("updateRequiTicket")]
        [Authorize]
        public IHttpActionResult UpdateRequiTicket(Guid ticketId, Guid requisicionId)
        {
            try
            {
                var t = db.Tickets.Find(ticketId);
                db.Entry(t).Property(x => x.RequisicionId).IsModified = true;

                t.RequisicionId = requisicionId;
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("updateCandidatoTicket")]
        public IHttpActionResult UpdateCandidatoTicket(Guid ticketId, Guid candidatoId)
        {
            try
            {
                var t = db.Tickets.Find(ticketId);
                db.Entry(t).Property(x => x.CandidatoId).IsModified = true;

                t.CandidatoId = candidatoId;
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("postularCandidato")]
        public IHttpActionResult PostularCandidato(Guid candidatoId, Guid requisicionId)
        {
            try
            {
                if (db.Postulaciones.Where(x => x.CandidatoId.Equals(candidatoId) && x.RequisicionId.Equals(requisicionId)).Count() == 0)
                {
                    Postulacion obj = new Postulacion();
                    obj.CandidatoId = candidatoId;
                    obj.fch_Postulacion = DateTime.Now;
                    obj.RequisicionId = requisicionId;
                    obj.StatusId = 1;

                    db.Postulaciones.Add(obj);
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
        [Route("updateStatus")]
        [Authorize]
        public IHttpActionResult UpdateStatus(Guid ticketId, int estatus, int moduloId)
        {
            try
            {
                var id = db.TicketsReclutador.Where(x => ticketId.Equals(ticketId)).Select(ID => ID.Id).FirstOrDefault();
                var TR = db.TicketsReclutador.Find(id);

                db.Entry(TR).Property(x => x.fch_Final).IsModified = true;

                TR.fch_Final = DateTime.Now;

                db.SaveChanges();

                var t = db.Tickets.Find(ticketId);

                db.Entry(t).State = EntityState.Modified;

                t.Estatus = estatus;
                t.ModuloId = moduloId;
                
                db.SaveChanges();

            
                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }


        [HttpGet]
        [Route("getFilaTickets")]
        [Authorize]
        public IHttpActionResult GetFilaTickets(int estatus, Guid reclutadorId)
        {
            try
            {
                if (estatus == 3 || estatus == 1)
                {

                    var fila = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(estatus)).Select(t => new
                    {
                        ticketId = t.Id,
                        ticket = t.Numero,
                        candidatoId = t.CandidatoId,
                        requisicionId = t.RequisicionId,
                        movimientoId = t.MovimientoId,
                        moduloId = t.ModuloId,
                        fch_Creacion = t.fch_Creacion,
                        fch_Estatus = db.TicketsReclutador.Where(x => x.TicketId.Equals(t.Id)).Select(F => F.fch_Final).FirstOrDefault(),
                        fch_cita = db.CalendarioCandidato.Where(x => x.CandidatoId.Equals(t.CandidatoId)).Count() > 0 ? db.CalendarioCandidato.Where(x => x.CandidatoId.Equals(t.CandidatoId)).Select(f => f.Fecha).FirstOrDefault() : DateTime.Now,
                        tiempo =  0
                    }).ToList();

                    var tickets = from items in fila
                                  select new
                                  {
                                      ticketId = items.ticketId,
                                      ticket = items.ticket,
                                      candidatoId = items.candidatoId,
                                      requisicionId = items.requisicionId,
                                      movimientoId = items.movimientoId,
                                      moduloId = items.moduloId,
                                      fch_Creacion = items.fch_Estatus,
                                      fch_cita = items.fch_cita,
                                      tiempo = Math.Round((DateTime.Now - items.fch_Creacion).TotalMinutes, 0) //(DateTime.Now - items.fch_Estatus).TotalMinutes > 60 ? Math.Round((DateTime.Now - items.fch_Creacion).TotalMinutes / 60, 0) : Math.Round((DateTime.Now - items.fch_Creacion).TotalMinutes, 0)
                                  };


                    return Ok(tickets);
                }
                else
                {
                    var requis = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(reclutadorId)).Select(r => r.RequisicionId).ToList();

                    var fila = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(estatus) && requis.Contains(x.Requisicion.Id)).Select(t => new
                    {
                        ticketId = t.Id,
                        ticket = t.Numero,
                        candidatoId = t.CandidatoId,
                        requisicionId = t.RequisicionId,
                        movimientoId = t.MovimientoId,
                        moduloId = t.ModuloId,
                        fch_Creacion = t.fch_Creacion,
                        fch_cita = db.CalendarioCandidato.Where(x => x.CandidatoId.Equals(t.CandidatoId)).Count() > 0 ? db.CalendarioCandidato.Where(x => x.CandidatoId.Equals(t.CandidatoId)).Select(f => f.Fecha).FirstOrDefault() : DateTime.Now,
                        tiempo = 0
                }).ToList();


                    //(DateTime.Now.Minute - t.fch_Creacion.Minute) > 0 ? (DateTime.Now.Minute - t.fch_Creacion.Minute) : 0
                    var tickets = from items in fila
                               select new
                               {
                                   ticketId = items.ticketId,
                                   ticket = items.ticket,
                                   candidatoId = items.candidatoId,
                                   requisicionId = items.requisicionId,
                                   movimientoId = items.movimientoId,
                                   moduloId = items.moduloId,
                                   fch_Creacion = items.fch_Creacion,
                                   fch_cita = items.fch_cita,
                                   tiempo = Math.Round((DateTime.Now - items.fch_Creacion).TotalMinutes, 0) //(DateTime.Now - items.fch_Creacion).TotalMinutes > 60 ? Math.Round((DateTime.Now - items.fch_Creacion).TotalMinutes / 60, 0).ToString() + " h.": Math.Round((DateTime.Now - items.fch_Creacion).TotalMinutes, 0).ToString() + " min."
                               };

                    return Ok(tickets);

                }
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getTicketEnAtencion")]
        public IHttpActionResult GetTicketEnAtencion()
        {
            try
            {
                var ticket = db.Tickets.OrderByDescending(f => f.fch_Creacion).Where(x => x.Estatus.Equals(2) || x.Estatus.Equals(5)).Select(t => new
                {
                    ticketId = t.Id,
                    ticket = t.Numero,
                    candidatoId = t.CandidatoId,
                    requisicionId = t.RequisicionId,
                    movimientoId = t.MovimientoId,
                    modulo = t.Modulo.Modulo,
                    moduloId = t.ModuloId,
                    estatus = t.Estatus
                }).ToList();

                return Ok(ticket);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getTicketPrioridad")]
        public IHttpActionResult GetTicketPrioridad(Guid reclutadorId, int ModuloId)
        {

            try
            {
               var requiReclutador = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(reclutadorId)).Select(r => r.RequisicionId).ToList();

                var concita = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(1) && requiReclutador.Contains(x.RequisicionId) && x.MovimientoId.Equals(1))
                .Select(t => new
                {
                    ticketId = t.Id,
                    ticket = t.Numero,
                    candidatoId = t.CandidatoId,
                    movimientoId = t.MovimientoId,
                    tiempoCita = t.fch_Creacion.Minute - DateTime.Now.Minute
                }).FirstOrDefault();

                if (concita == null)
                {
                    var sincita = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(1) && requiReclutador.Contains(x.RequisicionId) && x.MovimientoId.Equals(2)).Select(t => new
                    {
                        ticketId = t.Id,
                        ticket = t.Numero,
                        candidatoId = t.CandidatoId,
                        requisicionId = t.RequisicionId,
                        movimientoId = t.MovimientoId,
                        tiempoCita = t.fch_Creacion.Minute - DateTime.Now.Minute
                    }).FirstOrDefault();

                    if (sincita == null)
                    {
                        return Ok(HttpStatusCode.ExpectationFailed);
                    }
                    else
                    {
                        this.InsertTicketRecl(sincita.ticketId, reclutadorId, ModuloId);
                        return Ok(sincita.ticketId);
                    }
                }
                else
                {
                    this.InsertTicketRecl(concita.ticketId, reclutadorId, ModuloId);
                    return Ok(concita.ticketId);
                }


                //    var result = this.GetTicketsReclutador(ticket.ticketId, reclutadorId);



            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getCitas")]
        public IHttpActionResult GetCitas(Guid reclutadorId, int ModuloId)
        {
            try
            {
                var concita = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(1) && x.MovimientoId.Equals(1))
                   .Select(t => new
                   {
                       ticketId = t.Id,
                       ticket = t.Numero,
                       candidatoId = t.CandidatoId,
                       movimientoId = t.MovimientoId,
                       tiempoCita = t.fch_Creacion.Minute - DateTime.Now.Minute
                   }).FirstOrDefault();

                if(concita != null)
                {
                    this.InsertTicketRecl(concita.ticketId, reclutadorId, ModuloId);
                    return Ok(concita.ticketId);
                }
                else
                {
                    return Ok(HttpStatusCode.ExpectationFailed);
                }
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }


        [HttpGet]
        [Route("getTicketsReclutador")]
        public IHttpActionResult GetTicketsReclutador(Guid Ticket, Guid ReclutadorId)
        {
            try
            {
                int tiempo;

                if (db.TicketsReclutador.Count() > 0)
                {
                    tiempo = (int)(from TR in db.TicketsReclutador where TR.ReclutadorId.Equals(ReclutadorId)
                                   select new { TR.fch_Atencion, TR.fch_Final }).ToArray().Average(x => (x.fch_Final - x.fch_Atencion).TotalMinutes);
                }
                else
                {
                    tiempo = 0;
                }
                var ticket = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId) && x.TicketId.Equals(Ticket)).Select(T => new
                {
                    ticketId = T.TicketId,
                    requisicionId = T.Ticket.RequisicionId,
                    numero = T.Ticket.Numero,
                    estado = T.Ticket.Estatus,
                    atendidos = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId) && (x.Ticket.Estatus.Equals(3) || x.Ticket.Estatus.Equals(4))).Count(),
                    fechaAtencion = T.fch_Atencion,
                    fechaFinal = T.fch_Final,
                    tiempo = tiempo,
                    //  tiempo = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId)).Select(TR=>(TR.fch_Final -TR.fch_Atencion).TotalMinutes),
                    //tiempo = from TR in db.TicketsReclutador group TR by TR.fch_Atencion.Day into G select new { Day = G.Key, Minutes =  G.Average(TR => (TR.fch_Final - TR.fch_Atencion).TotalMinutes) > 0 ? G.Average(TR => (TR.fch_Final - TR.fch_Atencion).TotalMinutes) : 0 },
                    candidato = db.Candidatos.Where(x => x.Id.Equals(T.Ticket.CandidatoId)).Select(C => new
                    {
                        candidatoId = T.Ticket.CandidatoId,
                        curp = C.CURP,
                        nombre = C.Nombre + " " + C.ApellidoPaterno + " " + C.ApellidoMaterno,
                        dirNacimiento = C.municipioNacimiento.municipio + " " + C.estadoNacimiento.estado,
                        fechaNac = C.FechaNacimiento,
                        edad = DateTime.Now.Year - C.FechaNacimiento.Value.Year >= 0 ? DateTime.Now.Year - C.FechaNacimiento.Value.Year : 0,
                        email = C.emails.Select(m => m.email).FirstOrDefault(),
                        estatusId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Select(E => E.EstatusId).FirstOrDefault() : 27,
                        estatus = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Select(E => E.Estatus.Descripcion).FirstOrDefault() : "Disponible",
                        requisicionId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Select(r => r.RequisicionId).FirstOrDefault() : auxID
                    }).ToList()

                }).ToList();

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getTicketsExamen")]
        public IHttpActionResult GetTicketsExamen(Guid Ticket)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");

                var ticket = db.Tickets.Where(x => x.Id.Equals(Ticket)).Select(T => new
                {
                    ticketId = T.Id,
                    requisicionId = T.RequisicionId,
                    vBtra = T.Requisicion.VBtra,
                    folio = T.Requisicion.Folio,
                    numero = T.Numero,
                    estado = T.Estatus,
                    psicometria = db.PsicometriasDamsaRequis.Where(x => x.RequisicionId.Equals(T.RequisicionId) && x.PsicometriaId > 0).Count() > 0 ? true : false,
                    candidato = db.Candidatos.Where(x => x.Id.Equals(T.CandidatoId)).Select(C => new
                    {
                        candidatoId = T.CandidatoId,
                        curp = C.CURP,
                        nombre = C.Nombre + " " + C.ApellidoPaterno + " " + C.ApellidoPaterno,
                        dirNacimiento = C.municipioNacimiento.municipio + " " + C.estadoNacimiento.estado,
                        fechaNac = C.FechaNacimiento,
                        edad = DateTime.Now.Year - C.FechaNacimiento.Value.Year >= 0 ? DateTime.Now.Year - C.FechaNacimiento.Value.Year : 0,
                        email = C.emails.Select(m => m.email).FirstOrDefault(),
                        credenciales = db.AspNetUsers.Where(x => x.IdPersona.Equals(T.CandidatoId)).Select( U => new {
                            id = U.Id,
                            username = U.UserName,
                            pass = U.Pasword
                            }).FirstOrDefault(),
                        estatusId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Select(E => E.EstatusId).FirstOrDefault() : 27,
                        estatus = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(T.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Select(E => E.Estatus.Descripcion).FirstOrDefault() : "Disponible",
                        requisicionId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Select(r => r.RequisicionId).FirstOrDefault() : aux,
                        tecnicos = db.ExamenCandidato.Where(x => x.CandidatoId.Equals(T.CandidatoId) && x.RequisicionId.Equals(T.RequisicionId)).Count() > 0 ? db.ExamenCandidato.Where(x => x.CandidatoId.Equals(T.CandidatoId) && x.RequisicionId.Equals(T.RequisicionId)).Select(R => R.Resultado).FirstOrDefault() : 9999
                    }).FirstOrDefault()

                }).ToList();

                this.UpdateStatus(Ticket, 5, 1);

                var tt = from T in ticket select new
                {
                    ticketId = T.ticketId,
                    requisicionId = T.requisicionId,
                    vBtra = T.vBtra,
                    folio = T.folio,
                    numero = T.numero,
                    estado = T.estado,
                    psicometria = db.PsicometriasDamsaRequis.Where(x => x.RequisicionId.Equals(T.requisicionId) && x.PsicometriaId > 0).Count() > 0 ? true : false,
                    candidato = new
                    {
                        candidatoId = T.candidato.candidatoId,
                        curp = T.candidato.curp,
                        nombre = T.candidato.nombre,
                        dirNacimiento = T.candidato.dirNacimiento,
                        fechaNac = T.candidato.fechaNac,
                        edad = T.candidato.edad,
                        email = T.candidato.email,
                        credenciales = new
                        {
                            id = T.candidato.credenciales.id,
                            username = T.candidato.credenciales.username,
                            pass = db.Database.SqlQuery<String>("dbo.spDesencriptarPasword @id", new SqlParameter("id", T.candidato.credenciales.id)).FirstOrDefault()

                        },
                        estatusId = T.candidato.estatusId,
                        estatus = T.candidato.estatus,
                        requisicionId = T.candidato.requisicionId,
                        tecnicos = T.candidato.tecnicos
                    }
                };


                return Ok(tt);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getPostulaciones")]
        public IHttpActionResult GetPostulaciones(Guid candidatoId)
        {
            try
            {
             
                var requisicion = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Creacion)
                .Where(e => e.CandidatoId.Equals(candidatoId) && e.Requisicion.Activo.Equals(true))
                .Select(e => new
                {
                    Id = e.RequisicionId,
                    vBtra = e.Requisicion.VBtra,
                    //fch_Creacion = e.Requisicion.fch_Creacion,
                    //fch_Modificacion = e.Requisicion.fch_Modificacion,
                    //fch_Cumplimiento = e.Requisicion.fch_Cumplimiento,
                    Estatus = e.Requisicion.Estatus.Descripcion,
                    EstatusId = e.Requisicion.EstatusId,
                    //Prioridad = e.Requisicion.Prioridad.Descripcion,
                    //PrioridadId = e.Requisicion.PrioridadId,
                    Cliente = e.Requisicion.Cliente.Nombrecomercial,
                    //GiroEmpresa = e.Requisicion.Cliente.GiroEmpresas.giroEmpresa,
                    //ActividadEmpresa = e.Requisicion.Cliente.ActividadEmpresas.actividadEmpresa,
                    Vacantes = e.Requisicion.horariosRequi.Count() > 0 ? e.Requisicion.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                    Folio = e.Requisicion.Folio,
                    //DiasEnvio = e.Requisicion.DiasEnvio,
                    //Confidencial = e.Requisicion.Confidencial,
                    //Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Count(),
                    EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Count(),
                    //Propietario = db.Usuarios.Where(x => x.Id.Equals(e.Requisicion.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                    reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.RequisicionId)).Select(a => new
                    {
                        reclutadorId = a.GrpUsrId,
                        nombre = db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault()
                    }).Distinct().ToList(),
                    //ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList(),
                    examenId = db.RequiExamen.Where(x => x.RequisicionId.Equals(e.RequisicionId)).Count() > 0 ? db.RequiExamen.Where(x => x.RequisicionId.Equals(e.RequisicionId)).Select(ex => ex.ExamenId).FirstOrDefault() : 0
                }).ToArray();
                //&& !x.GrpUsrId.Equals(e.Requisicion.AprobadorId)
                return Ok(requisicion);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("liberarCandidato")]
        public IHttpActionResult LiberarCandidatos(Guid requisicionId, Guid candidatoId)
        {
            Guid aux = new Guid("00000000-0000-0000-0000-000000000000");
            try
            {
                var id = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(candidatoId) && x.RequisicionId.Equals(requisicionId)).Select(x => x.Id).FirstOrDefault();
                var c = db.ProcesoCandidatos.Find(id);

                db.Entry(c).Property(x => x.EstatusId).IsModified = true;
                c.EstatusId = 27;

                db.SaveChanges();

                var ids = db.Postulaciones.Where(x => x.RequisicionId.Equals(requisicionId) && x.CandidatoId.Equals(candidatoId)).Select(x => x.Id).FirstOrDefault();
                if (ids != aux)
                {
                    var cc = db.Postulaciones.Find(ids);

                    db.Entry(cc).Property(x => x.StatusId).IsModified = true;
                    cc.StatusId = 1;

                    db.SaveChanges();
                }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getVacantesReclutador")]
        public IHttpActionResult GetVacantesReclutador(Guid reclutadorId)
        {
            var asig = db.AsignacionRequis
                .OrderByDescending(e => e.Id)
                .Where(a => a.GrpUsrId.Equals(reclutadorId))
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
                    Cliente = e.Cliente.Nombrecomercial,
                    ClienteId = e.Cliente.Id,
                    Folio = e.Folio,
                    examenId = db.RequiExamen.Where(x => x.RequisicionId.Equals(e.Id)).Count() > 0 ? db.RequiExamen.Where(x => x.RequisicionId.Equals(e.Id)).Select(ex => ex.ExamenId).FirstOrDefault() : 0
                }).OrderBy(x => x.Folio).ToList();
            return Ok(vacantes);

        }
        [HttpGet]
        [Route("getVacantes")]
        public IHttpActionResult GetVacantes()
        {
            try
            {
                List<int> estatus = new List<int> { 34, 35, 36, 37 };

                Guid mocos = new Guid("1FF62A23-3664-E811-80E1-9E274155325E");
                var usuarios = db.Usuarios.Where(x => x.SucursalId.Equals(mocos)).Select(U => U.Id).ToList();
                var requis = db.AsignacionRequis.Where(x => usuarios.Contains(x.GrpUsrId)).Select(R => R.RequisicionId).ToArray();

                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => e.Activo && !e.Confidencial && !estatus.Contains(e.EstatusId) && requis.Contains(e.Id))
                    .Select(e => new
                    {
                        Id = e.Id,
                        estatus = e.Estatus.Descripcion,
                        Folio = e.Folio,
                        //Cliente = e.Cliente.Nombrecomercial,
                        //ClienteId = e.Cliente.Id,
                        //estado = e.Cliente.direcciones.Select(x => x.Municipio.municipio + " " + x.Estado.estado + " " + x.Estado.Pais.pais).FirstOrDefault(),
                        //domicilio_trabajo = e.Direccion.Calle + " " + e.Direccion.NumeroExterior + " " + e.Direccion.Colonia.colonia + " " + e.Direccion.Municipio.municipio + " " + e.Direccion.Estado.estado,
                        //Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        VBtra = e.VBtra,
                        requisitos = e.DAMFO290.escolardadesPerfil.Select(esc => esc.Escolaridad.gradoEstudio),
                        Actividades = e.DAMFO290.actividadesPerfil.Select(a => a.Actividades),
                        aptitudes = e.DAMFO290.aptitudesPerfil.Select(ap => ap.Aptitud.aptitud),
                        experiencia = e.Experiencia, 
                        categoria = e.Area.areaExperiencia,
                        icono = e.Area.Icono,
                        areaId = e.AreaId,
                        cubierta = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) - db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count() : 0
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
        [Route("getModulos")]
        public IHttpActionResult GetModulos()
        {
            try
            {
                var modulos = db.ModulosReclutamiento.Select(M => new {
                    M.Modulo,
                    M.Id, 
                    M.TipoModulo
                });

                return Ok(modulos);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpPost]
        [Route("setExamen")]
        public IHttpActionResult SetExamen(ExamenCandidato objeto)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");
                var e = db.ExamenCandidato.Where(x => x.CandidatoId.Equals(objeto.CandidatoId) && x.RequisicionId.Equals(objeto.RequisicionId)).Select(id => id.Id).FirstOrDefault();

                if(e == aux)
                {
                    objeto.fch_Creacion = DateTime.Now;
                    objeto.fch_Modificacion = DateTime.Now;

                    db.ExamenCandidato.Add(objeto);

                    db.SaveChanges();

                    var ind = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(objeto.CandidatoId) && x.RequisicionId.Equals(objeto.RequisicionId)).Select(c => c.Id).FirstOrDefault();
                    var C = db.ProcesoCandidatos.Find(ind);

                    db.Entry(C).Property(x => x.EstatusId).IsModified = true;
                    db.Entry(C).Property(x => x.Fch_Modificacion).IsModified = true;


                    if (db.InformeRequisiciones.Where(x => x.CandidatoId.Equals(objeto.CandidatoId) && x.RequisicionId.Equals(objeto.RequisicionId) && x.EstatusId.Equals(18)).Count() > 0)
                    {
                        C.EstatusId = 13;
                        C.Fch_Modificacion = DateTime.Now;

                        db.SaveChanges();
                    }
                    else
                    {
                        C.EstatusId = 18;
                        C.Fch_Modificacion = DateTime.Now;

                        db.SaveChanges();

                        C.EstatusId = 13;
                        C.Fch_Modificacion = DateTime.Now;

                        db.SaveChanges();

                    }
                }

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }
        [HttpGet]
        [Route("setEstatusCandidato")]
        public IHttpActionResult SetEstatus(Guid candidatoId, Guid requisicionId, int estatusId)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");

                var ind = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(candidatoId) && x.RequisicionId.Equals(requisicionId)).Select(c => c.Id).FirstOrDefault();
                var C = db.ProcesoCandidatos.Find(ind);

                db.Entry(C).Property(x => x.EstatusId).IsModified = true;
                db.Entry(C).Property(x => x.Fch_Modificacion).IsModified = true;

                if (db.InformeRequisiciones.Where(x => x.CandidatoId.Equals(candidatoId) && x.RequisicionId.Equals(requisicionId) && x.EstatusId.Equals(estatusId)).Count() == 0)
                {
                        C.EstatusId = 13;
                        C.Fch_Modificacion = DateTime.Now;

                        db.SaveChanges();
                }
                
            
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getConcurrencia")]
        public IHttpActionResult GetConcurrencia()
        {
            try
            {
                var conc = db.HistoricosTickets.GroupBy(T => T.Numero)
                    .Select(C => new
                    {
                        candidatoId = C.Where(x => x.Estatus > 1).Select(c => c.CandidatoId).FirstOrDefault(),
                        fecha = C.Where(x => x.Estatus.Equals(1)).Select( f=> f.fch_Modificacion).FirstOrDefault(),
                        usuario = db.Usuarios.Where( u => u.Id.Equals(C.Where(xx => !xx.ReclutadorId.Equals(auxID)).Select(x => x.ReclutadorId).FirstOrDefault())).Select(U => U.Nombre + " " + U.ApellidoPaterno + " " + U.ApellidoMaterno).FirstOrDefault(),
                        tiempo = C.Where(x => x.Estatus > 1).Sum(s => s.fch_Modificacion.Minute) > 60 ? C.Where(x => x.Estatus > 1).Sum(s => s.fch_Modificacion.Minute) / 60 : C.Where(x => x.Estatus > 1).Sum(s => s.fch_Modificacion.Minute),
                        hora = C.Where(x => x.Estatus == 2).Select(h => h.fch_Modificacion).FirstOrDefault(),
                        modulo = C.Select(m => m.Modulo.Modulo).FirstOrDefault(),
                        turno = C.Select(t => t.Numero.Substring(t.Numero.Length - 3, 3)).FirstOrDefault(),
                        resumen = C.Select(tt => new
                        {
                            Fecha = tt.fch_Modificacion,
                            EstatusId = tt.Estatus,
                            Estatus = tt.Estatus == 2 ? "En Atencion" : tt.Estatus == 3 ? "Examenes" : tt.Estatus == 4 ? "Finalizado" : tt.Estatus == 5 ? "En Atencion Examen" : "En Espera"
                            

                            //db.HistoricosTickets.Where(x => x.CandidatoId.Equals(C.CandidatoId) && x.RequisicionId.Equals(C.RequisicionId)).Select(T => T.fch_Modificacion.Minute).Sum() >= 60 ? db.HistoricosTickets.Where(x => x.CandidatoId.Equals(C.CandidatoId) && x.RequisicionId.Equals(C.RequisicionId)).Select(T => T.fch_Modificacion.Minute).Sum() / 60 : db.HistoricosTickets.Where(x => x.CandidatoId.Equals(C.CandidatoId) && x.RequisicionId.Equals(C.RequisicionId)).Select(T => T.fch_Modificacion.Minute).Sum()
                        }).ToList()
                    }).OrderByDescending(o => o.fecha).ToList();

                return Ok(conc);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getTicketsGenerados")]
        public IHttpActionResult GetTicketsGenerados()
        {
            try
            {
                //tickets con cita y sin cita
                var conc = db.HistoricosTickets
                    .Select(C => new
                    {

                        fecha = C.fch_Modificacion,
                        estatus = C.Estatus,
                        ticket = C.Numero,
                        tipo = C.MovimientoId
                        //total = C.Select(t => t.Numero).Count(),

                    
                    }).OrderByDescending(o => o.fecha).ToList();

                var result = from T in conc
                             group T by T.fecha.Date into g
                             select new { fecha = g.Key, total=g.Select(t => t.ticket).Distinct().Count(), atendidos = g.Where(x => x.estatus.Equals(2)).Select(x => x.ticket).Count(),
                             concita = g.Where(x => x.tipo.Equals(1) && x.estatus.Equals(1)).Count(), sincita = g.Where(x => x.tipo.Equals(2) && x.estatus.Equals(1)).Count()};
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getRportAtencion")]
        public IHttpActionResult GetRportAtencion()
        {
            try
            {
                //tickets con cita y sin cita
                var conc = db.HistoricosTickets
                    .Select(C => new
                    {

                        fecha = C.fch_Modificacion,
                        estatus = C.Estatus,
                        ticket = C.Numero,
                        tipo = C.MovimientoId,
                        reclutadorId = C.ReclutadorId
                        //total = C.Select(t => t.Numero).Count(),


                    }).OrderByDescending(o => o.fecha).ToList();

                var result = from T in conc
                             group T by T.fecha.Date into g
                             select new
                             {
                                 fecha = g.Key,
                                 datos = from R in g
                                         group R by R.reclutadorId into r
                                         select new
                                         {
                                             reclutador = String.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(R => R.Nombre + " " + R.ApellidoPaterno + " " + R.ApellidoMaterno).FirstOrDefault()) ? "SIN REGISTRO" : db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(R => R.Nombre + " " + R.ApellidoPaterno + " " + R.ApellidoMaterno).FirstOrDefault(),
                                             total = r.Select(x => x.ticket).Distinct().Count(),
                                             concita = r.Where(x => x.tipo.Equals(1) && x.estatus.Equals(2)).Count(),
                                             sincita = r.Where(x => x.tipo.Equals(2) && x.estatus.Equals(2)).Count(),
                                             //mocos = (from TR in db.HistoricosTickets
                                             //              where TR.ReclutadorId.Equals(r.Key)
                                             //              group TR by TR.Estatus into f

                                             //              select new {
                                             //                  rid = f.Select(rrr => rrr.ReclutadorId),
                                             //                  f1 = f.Where(x => x.Estatus.Equals(2)).Select(f1 => f1.fch_Modificacion),
                                             //                  f2 = f.Where(x => x.Estatus.Equals(3) || x.Estatus.Equals(4)).Select(f2 => f2.fch_Modificacion) }
                                             //              ),

                                       f1 = r.Where(x => x.estatus.Equals(2)).Select(f1 => f1.fecha).ToArray(),
                                       f2 = r.Where(x => x.estatus.Equals(3) || x.estatus.Equals(4)).Select(f2 => f2.fecha).ToArray(),
                                     
                                     
                                 }
                             };
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

    }
}
