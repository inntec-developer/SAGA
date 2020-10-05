using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Configuration;
using System.Threading.Tasks;
namespace SAGA.API.Utilerias
{
    public class AppDownload
    {
        public string DownloadFiles(string file)
        {
            string ruta_descarga = "";
            try
            {

                string localFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Utilerias/CampoApp/campoApp.apk");

                string nom = Path.GetFileName(localFilePath);

                string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string DownloadsFolder = "/data/media/0/"; // userProfileFolder + "\\Downloads\\";
                ruta_descarga = Path.Combine(DownloadsFolder, nom);

                WebClient wc = new WebClient();
                wc.DownloadFile(localFilePath, ruta_descarga);

                //        return Ok(HttpStatusCode.OK);
                //  

                //string DownloadsFolder = "storage/emulated/0/Download";
                //ruta_descarga = Path.Combine(DownloadsFolder, "campoApp.apk");

                //WebClient wc = new WebClient();
                //    wc.DownloadFile(@"https://apisb.damsa.com.mx/utilerias/CampoApp/campoApp.apk", ruta_descarga);

                    return "descargó";
         
            }

            catch (Exception ex)
            {
                return ex.Message + ex.InnerException + ruta_descarga;
            }
        }

    }
}