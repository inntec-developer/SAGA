using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook;
using System.Configuration;
using System.IO;
using System.Dynamic;


namespace SAGA.API.Utilerias
{
    public class FacebookTools
    {
        public Facebook.FacebookOAuthResult resultadoOAuth;


        public FacebookTools() { }

        public object HttpUtil { get; private set; }

       
        public string ObtenerUrlAutorizacion()
        {
            string permisos = "publish_actions,publish_stream,manage_pages";
            dynamic parametros = new ExpandoObject();
            string AppId = "2290965441015344";
            string APP_SECRET = "015711a76cfc24645d97278f6673c382";
            string Picture_Path = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/ArteRequi/BG/DamsaVacantes_PP17.jpg");

            var fb = new FacebookClient();
            dynamic result = fb.Get("oauth/access_token", new
            {
                client_id = AppId,
                client_secret = APP_SECRET,
                grant_type = "client_credentials",
                scope = permisos
            });

            fb.AccessToken = result["access_token"];
            dynamic result2 = fb.Get("me");
            var artestream = File.OpenRead(Picture_Path);

            var result3 = fb.Post("/me/feed", "mocos mocos mocos");

            //dynamic res = fb.Post("me/photos", new
            //{
            //    message = "Pruebas",
            //    file = new FacebookMediaStream
            //    {
            //        ContentType = MimeMapping.GetMimeMapping(Picture_Path),
            //        FileName = Path.GetFileName(Picture_Path)
            //    }.SetValue(artestream)
            //});
            //if (string.IsNullOrEmpty(AppId))
            //    throw new Exception("No ha ha definido smpFaceBookApi.AppId");

            //string UrlCallback = "https://www.facebook.com/connect/login_success.html";

            //parametros.client_id = AppId;
            //parametros.redirect_uri = UrlCallback;
            //parametros.response_type = "token";
            //parametros.display = "popup";

            //if (!string.IsNullOrEmpty(permisos))
            //    parametros.scope = permisos;

            //Facebook.FacebookClient clienteFB = new Facebook.FacebookClient();
            //var mocos = clienteFB.GetLoginUrl(parametros).ToString();
            //var longtoken = HttpUtility.ParseQueryString(HttpUtil.GetHtmlPage("https://graph.facebook.com/oauth/access_token?client_id=" + AppId + "&client_secret=" + APP_SECRET + "&grant_type=fb_exchange_token&fb_exchange_token=" + SHORT_TOKEN))
            //var mocos2 = clienteFB.TryParseOAuthCallbackUrl(mocos, out resultadoOAuth);

            return "mocos";
        }
        public bool PostArte()
        {
            // new FacebookClient instance (AT: user access token or page access token)
            try
            {
                var fb = new FacebookClient("EAAHNGh7U3qYBAKMZBwOZBjKoo7dj3lyooiIUysdYAPvcaDNWnWHCPNyr1mhAeLWZBCy4p38YvhINmpG85LQgDHJsNQtuIuGRZAtaSHcJedJA1sVoKL8R47I8clEn56L4JOvPuUEVL7mZA30wtYA65FVhLdei288UZD");
                string Picture_Path = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/ArteRequi/BG/DamsaVacantes_PP17.jpg");
                FacebookMediaObject mediaObject = new FacebookMediaObject
                {
                    ContentType = "image/jpg",
                    FileName = Path.GetFileName(Picture_Path)
                };

                mediaObject.SetValue(File.ReadAllBytes(Picture_Path));

                IDictionary<string, object> upload = new Dictionary<string, object>();
                upload.Add("name", "mocos");
                upload.Add(Picture_Path, mediaObject);

                dynamic res = fb.Post("/me/photos", upload) as JsonObject;

                return true;
            }
            catch (Exception ex)
            {
                return false;
               
            }
            

            //dynamic parameters = new ExpandoObject();
            //parameters.message = Picture_Caption;
            //parameters.source = new FacebookMediaObject
            //{
            //    ContentType = "image/jpeg",
            //    FileName = Path.GetFileName(Picture_Path)
            //}.SetValue(File.ReadAllBytes(Picture_Path));

            //try
            //{
            //    if (Is_User_Wall)
            //    {
            //        // Post the image/picture to User wall
            //        fb.Post("me/photos", parameters);
            //    }
            //    else
            //    {
            //        // Post the image/picture to the Page's Wall Photo album
            //        fb.Post("/" + AlbumID + "/photos", parameters);
            //    }

            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            //try
            //{
            //    FacebookClient fb = new FacebookClient();
            //    fb.AppSecret = "3a68f82f08f80706b4498f6501fc3533";
            //    fb.AppId = ConfigurationManager.AppSettings.Get("FacebookId");
            //    fb.AccessToken = ConfigurationManager.AppSettings.Get("FacebookToken");


            //    var artestream = File.OpenRead(artePath);

            //    dynamic res = fb.Post("me/photos", new
            //    {
            //        message = "Pruebas",
            //        file = new FacebookMediaStream
            //        {
            //            ContentType = MimeMapping.GetMimeMapping(artePath),
            //            FileName = Path.GetFileName(artePath)
            //        }.SetValue(artestream)
            //    });

            //    return true;
            //}
            //catch (Exception ex)
            //{

            //    return false;
            //}
        }

        public void PostToWall(string message)
        {
            string scope = "publish_stream,manage_pages";
            var fb = new FacebookClient(ConfigurationManager.AppSettings.Get("FacebookToken"));
            string url = string.Format("{0}/{1}", ConfigurationManager.AppSettings.Get("FacebookId") , "feed");
            var argList = new Dictionary<string, object>();
            argList["message"] = message;
            fb.Post(url, argList);
        }
    }
}