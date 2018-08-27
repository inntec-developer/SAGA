﻿using System;
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
        public IHttpActionResult GetFiles(string entidadId)
        {
            var path = "~/utilerias/Files/users/" + entidadId;
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
        public HttpResponseMessage DownloadFiles(string file)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(file);
            byte[] pdf = System.IO.File.ReadAllBytes(path);
            string nom = Path.GetFileName(path);
            string ext = Path.GetExtension(path);
            string mimetype = MimeMapping.GetMimeMapping(path);


            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
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

            //}
            //catch( Exception ex)
            //{
            //    return Ok(HttpStatusCode.ExpectationFailed);
            //}


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

                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/Files/users/83569bac-0d68-e811-80e1-9e274155325e/" + fileName);

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
