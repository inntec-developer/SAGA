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
            var path = "~/utilerias/img/user/";
            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(path);

            DirectoryInfo folderInfo = new DirectoryInfo(fullPath);
            List<string> extensions = new List<string>(); // folderInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).Select(x => x.Extension).Distinct().ToList();
            var files = folderInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                extensions.Add(file.Name);
            }
                //can read and compare for given extension

                //foreach (var extension in extensions)
                //{
                //    var files = folderInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).Where(x => x.Extension == extension).ToList();
                //    Console.WriteLine("Total Files with {0} Extension = {1}  ", extension, files.Count);
                //   
                //}

                return Ok(files);
        }
}
    
}
