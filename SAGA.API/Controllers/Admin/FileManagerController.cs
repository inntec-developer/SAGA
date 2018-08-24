using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SAGA.API.Controllers
{

    [RoutePrefix("api/admin")]
    public class FileManagerController : ApiController
    {
       
        [HttpGet]
        [Route("getFiles")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetFiles()
        {
            var path = "~/utilerias/";
            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            DirectoryInfo folderInfo = new DirectoryInfo(fullPath);
            List<string> extensions = folderInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).Select(x => x.FullName).ToList();


            var files = folderInfo.GetFiles(
                    "*.*",
                    SearchOption.AllDirectories).Select(x => new
                    {
                        fullPath = x.FullName,
                        nom = x.Name,
                        ext = x.Extension,
                        size = (long)x.Length / 1024,
                        fc = x.LastWriteTime.ToShortDateString()
                    });



            var egrp = extensions.Select(file => Path.GetExtension(file).TrimStart('.').ToLower())
                     .GroupBy(x => x, (ext, extCnt) => new
                     {
                         Extension = ext,
                         Count = extCnt.Count()
                     });

            return Ok(files);
        }

        [HttpGet]
        [Route("downloadFiles")]
        public IHttpActionResult DownloadFiles(string file)
        {
            try
            {
                string localFilePath = System.Web.Hosting.HostingEnvironment.MapPath(file);

                if (File.Exists(localFilePath))
                {
                    string nom = Path.GetFileName(localFilePath);

                    string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string DownloadsFolder = userProfileFolder + "\\Downloads\\";
                    var ruta_descarga = Path.Combine(DownloadsFolder, nom);

                    WebClient wc = new WebClient();
                    wc.DownloadFile(localFilePath, ruta_descarga);

                    return Ok(HttpStatusCode.OK);
                }
                else
                {
                    return Ok(HttpStatusCode.BadRequest);
                }

            }
            catch( Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

          
        }

        [HttpPost]
        [Route("UploadFile")]
        public IHttpActionResult UploadImage()
        {
            string fileName = null;
            var path = "";
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files["file"];
                var id = Guid.Parse(Path.GetFileNameWithoutExtension(postedFile.FileName).ToString());
                //var id = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");

                var ext = Path.GetExtension(postedFile.FileName);
                fileName = Path.GetFileName(postedFile.FileName);

                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    path = "~/utilerias/img/user/" + fileName;
                }
                else
                {
                    path = "~/utilerias/pdf/" + fileName;
                }

                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(path);

                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                postedFile.SaveAs(fullPath);

                return Ok(HttpStatusCode.Created); //201

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.InternalServerError);
            }

        }

        [HttpGet]
        [Route("getImage")]
        public IHttpActionResult GetImage2(string ruta)
        {
            string fullPath;

            try
            {
                fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/user/" + ruta);
            }
            catch
            {
                fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/user/default.jpg");

            }

            FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            byte[] bimage = new byte[fs.Length];
            fs.Read(bimage, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            return Ok(Convert.ToBase64String(bimage));

        }

    }

}
