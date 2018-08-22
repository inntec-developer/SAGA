using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/admin")]
    public class FileManagerController : ApiController
    {
        [HttpGet]
        [Route("getFiles")]
        public IHttpActionResult GetFiles()
        {
            var path = "~/utilerias/";
            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            DirectoryInfo folderInfo = new DirectoryInfo(fullPath);
            List<string> extensions = folderInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).Select(x => x.FullName).ToList();
            //var mocos = folderInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).Select(x => new {
            //    fullPath = x.FullName,
            //    nom = x.Name,
            //    ext = x.Extension,
            //    size = (long)x.Length / 1024,
            //    fc = x.LastWriteTime.ToShortDateString()
            //});

            var files = folderInfo.GetFiles(
                    "*.*",
                    SearchOption.AllDirectories).Select(x => new {
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

            //can read and compare for given extension

            //foreach (var extension in extensions)
            //{
            //    var files = folderInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).Where(x => x.Extension == extension).ToList();
            //    Console.WriteLine("Total Files with {0} Extension = {1}  ", extension, files.Count);
            //   
            //}

        
            return Ok(files);
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
