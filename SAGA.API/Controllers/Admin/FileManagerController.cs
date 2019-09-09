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
using SAGA.API.Dtos;
using SAGA.API.Dtos.Admin;

namespace SAGA.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/admin")]
    public class FileManagerController : ApiController
    {


        [HttpGet]
        [Route("viewFile")]
        public HttpResponseMessage ViewFile(string ruta)
        {
            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/" + ruta);
         

                if (File.Exists(path))
                {
                    byte[] pdf = System.IO.File.ReadAllBytes(path);
                    string nom = Path.GetFileName(path);
                    string ext = Path.GetExtension(path);
                    string mimetype = MimeMapping.GetMimeMapping(path);

                    result.Content = new ByteArrayContent(pdf);
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                    result.Content.Headers.ContentDisposition.FileName = nom;
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue(mimetype);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.NoContent);

                }
            }
            catch(Exception ex)
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;

        }

        [HttpGet]
        [Route("getFiles")]
        public IHttpActionResult GetFiles(Guid entidadId)
        {
            var path = "~/utilerias/Files/users/" + entidadId.ToString();
            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(path);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                return Ok(0);
            }
            else
            {
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
                        }).OrderByDescending(o => o.fc);



                var egrp = extensions.Select(file => Path.GetExtension(file).TrimStart('.').ToLower())
                         .GroupBy(x => x, (ext, extCnt) => new
                         {
                             Extension = ext,
                             Count = extCnt.Count()
                         });
                return Ok(files);
            }
      
        }

        [HttpGet]
        [Route("getBGArte")]
        public IHttpActionResult GetBGArte()
        {
            var path = "~/utilerias/img/ArteRequi/BG";
            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(path);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                return Ok(0);
            }
            else
            {
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
                        }).OrderByDescending(o => o.fc);

                var egrp = extensions.Select(file => Path.GetExtension(file).TrimStart('.').ToLower())
                         .GroupBy(x => x, (ext, extCnt) => new
                         {
                             Extension = ext,
                             Count = extCnt.Count()
                         });
                return Ok(files);
            }

        }

        [HttpPost]
        [Route("guardarArte")]
        public IHttpActionResult GuardarArte(ArteDto Arte)
        {
            try
            {
                string x = Arte.arte.Replace("data:image/png;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(x);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/ArteRequi/Arte/" + Arte.requisicionId.ToString() + ".png");

                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                image.Save(fullPath);

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("downloadFiles")]
        public HttpResponseMessage DownloadFiles(string file)
        {
            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            try { 
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/" + file);
            byte[] pdf = System.IO.File.ReadAllBytes(path);
            string nom = Path.GetFileName(path);
            string ext = Path.GetExtension(path);
            string mimetype = MimeMapping.GetMimeMapping(path);


          
            result.Content = new ByteArrayContent(pdf);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            result.Content.Headers.ContentDisposition.FileName = nom;
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(mimetype);
            return result;


            //try
            //{
            //    string localFilePath = System.Web.Hosting.HostingEnvironment.MapPath(file);

            //    if (File.Exists(localFilePath))
            //    {
            //        string nom = Path.GetFileName(localFilePath);

            //        string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //        string DownloadsFolder = userProfileFolder + "\\Downloads\\";
            //        var ruta_descarga = Path.Combine(DownloadsFolder, nom);

            //        WebClient wc = new WebClient();
            //        wc.DownloadFile(localFilePath, ruta_descarga);

            //        return Ok(HttpStatusCode.OK);
            //    }
            //    else
            //    {
            //        return Ok(HttpStatusCode.BadRequest);
            //    }

            }
            catch (Exception ex)
            {
                return result;
            }


        }

        [HttpGet]
        [Route("deleteFiles")]
        public IHttpActionResult DeleteFiles(string file)
        {
            try
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath(file);
                string nom = Path.GetFileName(path);
                string ext = Path.GetExtension(path);
                string mimetype = MimeMapping.GetMimeMapping(path);

                if (File.Exists(path))
                    File.Delete(path);

                return Ok(HttpStatusCode.OK);
            }
            catch
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpPost]
        [Route("uploadFile")]
        public IHttpActionResult UploadFile()
        {
            string fileName = null;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files["file"];

                fileName = Path.GetFileName(postedFile.FileName);
                var idx = fileName.LastIndexOf('_') + 1;
                var lon = fileName.Length - idx;
                var id = fileName.Substring(idx, lon);

                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/Files/users/");
                if (!Directory.Exists(fullPath + '/' + id ))
                {
                    Directory.CreateDirectory(fullPath + '/' + id);
                }

                fileName = fileName.Substring(0, idx - 1);

                fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/Files/users/" + id + '/' + fileName);

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
                fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/" + ruta);
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
