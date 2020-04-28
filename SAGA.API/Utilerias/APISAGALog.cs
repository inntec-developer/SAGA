using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SAGA.API.Utilerias
{
    public class APISAGALog
    {
        public void WriteError(string error)
        {
            string path = "APISAGALog_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            string tempPath = GetTempPath();
            if (!File.Exists(tempPath + path))
            {
                File.Create(tempPath + path);
            }

           StreamWriter stwriter = System.IO.File.AppendText(
               tempPath + path);
            try
            {
                string logLine = String.Format(
                    "{0:G}: {1}.", DateTime.Now, error);
                stwriter.WriteLine(logLine);
            }
            finally
            {
                stwriter.Close();
            }

        }
        public string GetTempPath()
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP");
            if (!path.EndsWith("\\")) path += "\\";
            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Utilerias/");
            return fullPath;
        }
    }
}