using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Http;
using SAGA.DAL;
using System.Net;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Infobip.Api.Config;
using Infobip.Api.Client;
using System.Text;
using Infobip.Api.Model.Sms.Mt.Send.Textual;
using Infobip.Api.Model.Sms.Mt.Send;
using System.Threading.Tasks;

namespace SAGA.API.Controllers.BTDamsa
{
    [RoutePrefix("api/btadmin")]
    public class AdminController : ApiController
    {
        private SAGADBContext db;
        public AdminController()
        {
            db = new SAGADBContext();
        }

        public string ValidarCorreo(string correo)
        {
            try
            {
                // Generar código
                Random rnd = new Random();
                string code = rnd.Next(100000, 999999).ToString();

                var xml = "";
                var asunto = string.Format("Bolsa de Trabajo Damsa - Registro");
                var copia = "mventura@damsa.com.mx";
                //Cuerpo del Mensaje
                var body = "&lt;!DOCTYPE html>";
                body = body + "&lt;html lang=&quot;es&quot;>";
                body = body + "&lt;meta http-equiv=&quot;Content - Type&quot; content=&quot;text / html; charset = utf - 8&quot;/>";
                body = body + "&lt;link href=&quot;{ { URL::Content('css/bootstrap.min.css') } }&quot; rel=&quot;stylesheet&quot; type=&quot;text / css&quot; media=&quot;all&quot; />";
                body = body + "&lt;body> &lt;div style=&quot;text-align: center; font-family:'calibri';&quot;>";
                body = body + "&lt;p>Se ha recibido una petición para activar registro en bolsa de trabajo DAMSA.&lt;/p>";
                body = body + "&lt;p>Ingresa el siguiente código para activar registro: &lt;strong>" + code + "&lt;/strong>&lt;/p>";
                body = body + "&lt;br> Esta notificación es por seguridad y sirve para que nadie use tu cuenta de correo electrónico sin autorización. &lt;br>&lt;/ p > ";
                body = body + "&lt;p>&lt;small>Por favor no respondas a este correo.&lt;br>";
                body = body + "Gracias.&lt;br>El equipo &lt;strong> INNTEC.&lt;/strong>&lt;/small>&lt;/p>&lt;hr>";
                body = body + "&lt;/div>&lt;/body>&lt;/html>";


                xml = string.Format("<Parametros><Parametro Id_Sistema=\"SISTEMA_DEMO\" De=\"noreply@damsa.com.mx\" "
                                   + "Para=\"{0}\" Copia=\"{1}\"  CopiaOculta=\"\" Asunto=\"{2}\" Msg=\"{3}\"/> "
                                   + "</Parametros>", correo, copia, asunto, body);

                SqlParameter[] Parameters = { new SqlParameter("@ParametrosXML", xml) };
                db.Database.ExecuteSqlCommand("sp_emailFirmas @ParametrosXML", Parameters);
                return code;
            }
            catch(Exception ex)
            {
                return "400";
            }
        }

        public async Task<string> EnviarSMS(string telefono)
        {
            try
            {
                Random rnd = new Random();
                string code = rnd.Next(100000, 999999).ToString();
                Regex reg = new Regex("[^a-zA-Z0-9] ");
                List<string> Destino = new List<string>(1) { telefono };
                BasicAuthConfiguration BASIC_AUTH_CONFIGURATION = new BasicAuthConfiguration(ConfigurationManager.AppSettings["BaseUrl"], ConfigurationManager.AppSettings["UserInfobip"], ConfigurationManager.AppSettings["PassInfobip"]);

                SendSingleTextualSms smsClient = new SendSingleTextualSms(BASIC_AUTH_CONFIGURATION);

                string texto = "Bolsa Trabajo DAMSA codigo para validacion de registro " + code;
                texto = texto.Normalize(NormalizationForm.FormD);
                texto = reg.Replace(texto, " ");

                SMSTextualRequest request = new SMSTextualRequest
                {
                    From = "DAMSA",
                    To = Destino,
                    Text = ConfigurationManager.AppSettings["NameAppMsj"] + texto

                };

                SMSResponse smsResponse = await smsClient.ExecuteAsync(request); // Manda el mensaje con código.

                SMSResponseDetails sentMessageInfo = smsResponse.Messages[0];

                return code;
            }
            catch (Exception ex)
            {
                return "400";
            }

        }
        }
}