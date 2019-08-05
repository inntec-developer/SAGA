using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing.Printing;
using System.Drawing;
using System.Printing;
using System.IO;
using System.Web.Services;
using System.Diagnostics;
using System.Configuration;

namespace SAGA.API.Utilerias
{
    public class GenerarTicket
    {
        public string TicketNo { get; set; }
        public string Nombre { get; set; }

        public void print()
        {
            try
            {
                PrintDocument recordDoc = new PrintDocument();
                recordDoc.DocumentName = "Customer Receipt";

                recordDoc.PrintPage += new PrintPageEventHandler(this.PrintPage); //function below
                                                                                  // recordDoc.PrintController = new StandardPrintController(); //hides status dialog popup
                                                                                  //Comment if debugging 
                var imp = FindPrinter();

                LogMessageToFile(imp.FullName);
                LogMessageToFile(imp.IsNotAvailable.ToString());
                PrinterSettings ps = new PrinterSettings();

                ps.PrinterName = imp.FullName;
                recordDoc.PrinterSettings = ps;
                recordDoc.Print();

                recordDoc.Dispose();
            }
            catch (Exception ex)
            {
                LogMessageToFile(ex.Message);
            }


        }

        public PrintQueue FindPrinter()
        {
            var queue = new LocalPrintServer().GetPrintQueue("EPSON TM-T20II Receipt");
            var queueStatus = queue.QueueStatus;
            LogMessageToFile(queueStatus.ToString());
            //"EPSON TM-T20II Receipt"
            var printers = new PrintServer().GetPrintQueues();

            foreach (var printer in printers)
            {
                LogMessageToFile(printer.FullName);

                if (printer.FullName == "EPSON TM-T20II Receipt")
                {
                    return printer;
                }
            }

            return LocalPrintServer.GetDefaultPrintQueue();
        }

        void PrintPage(object sender, PrintPageEventArgs e)
        {
            float x = 10;
            float y = 5;
            float width = 270.0F; //max width I found through trial and error
            float height = 0F;
            Font drawFontArial24Bold = new Font("Arial", 24, FontStyle.Bold);
            Font drawFontArial12Regular = new Font("Arial", 12, FontStyle.Regular);
            Font drawFontArial10Regular = new Font("Arial", 10, FontStyle.Regular);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            //Set format of string.
            StringFormat drawFormatCenter = new StringFormat();
            drawFormatCenter.Alignment = StringAlignment.Center;
            StringFormat drawFormatLeft = new StringFormat();
            drawFormatLeft.Alignment = StringAlignment.Near;
            StringFormat drawFormatRight = new StringFormat();
            drawFormatRight.Alignment = StringAlignment.Far;
            //Draw string to screen.
            string text = "SISTEMA DE TURNOS";
            e.Graphics.DrawString(text, drawFontArial12Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += e.Graphics.MeasureString(text, drawFontArial12Regular).Height;
            text = "DAMSA";

            e.Graphics.DrawString(text, drawFontArial12Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += e.Graphics.MeasureString(text, drawFontArial12Regular).Height;

            text = DateTime.Now.ToString();
            e.Graphics.DrawString(text, drawFontArial12Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += e.Graphics.MeasureString(text, drawFontArial12Regular).Height + 10;

     
            if (this.Nombre != "")
            {
                text = "Bienvenido";
                e.Graphics.DrawString(text, drawFontArial10Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                y += e.Graphics.MeasureString(text, drawFontArial10Regular).Height;

                text = Nombre;
                e.Graphics.DrawString(text, drawFontArial10Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                y += e.Graphics.MeasureString(text, drawFontArial10Regular).Height + 10;

            }

            text = "Su turno es:";
            e.Graphics.DrawString(text, drawFontArial10Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += e.Graphics.MeasureString(text, drawFontArial10Regular).Height;

            text = this.TicketNo;
            e.Graphics.DrawString(text, drawFontArial24Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += e.Graphics.MeasureString(text, drawFontArial24Bold).Height + 10;

            text = "Por favor espere ser llamado";
            e.Graphics.DrawString(text, drawFontArial10Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += e.Graphics.MeasureString(text, drawFontArial10Regular).Height;

            text = "Visítenos en: bolsa.damsa.com.mx";
            e.Graphics.DrawString(text, drawFontArial10Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += e.Graphics.MeasureString(text, drawFontArial10Regular).Height;
            LogMessageToFile(this.TicketNo);
        }
        public string GetTempPath()
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP");
            if (!path.EndsWith("\\")) path += "\\";
            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("/Utilerias/");
            return fullPath;
        }

        public void LogMessageToFile(string msg)
        {
            //if (!File.Exists(GetTempPath() + "MelinaLog.txt"))
            //{
            //    System.IO.StreamWriter sw = System.IO.File.Create(GetTempPath() + "MelinaLog.txt");
            //}  StreamWriter stwriter = File.CreateText(path);

            //StreamWriter stwriter = File.CreateText(GetTempPath() + "MelinaLog.txt");
            System.IO.StreamWriter stwriter = System.IO.File.AppendText(
                GetTempPath() + "MelinaLog.txt");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                stwriter.WriteLine(logLine);    
            }
            finally
            {
                stwriter.Close();
            }
        }
    }
}