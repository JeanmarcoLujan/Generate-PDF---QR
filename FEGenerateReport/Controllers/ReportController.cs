using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using FEGenerateReport.Models.DTO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZXing.Rendering;


namespace FEGenerateReport.Controllers
{
    public class ReportController : ApiController
    {
        public string Get(int id, string report, string docentry)
        {

            return "value";
        }
        //public string Post([FromBody] string value)
        public IHttpActionResult Post([FromBody] RequestReportDTO parameters)
        {
            object obj;
            string descripcion = "";
            bool continuar = true;
            bool result = false;
            string archivo = "";

            try
            {

                ReportDocument reportDocument = new ReportDocument();

                switch (parameters.Report)
                {
                    case "1":
                        archivo = "report1";
                        break;
                    case "2":
                        archivo = "report2";
                        break;
                    default:
                        archivo = "";
                        break;

                }

                if (archivo!="")
                {
                    try
                    {
                       // string report = "C:\\fe\\cr\\" + archivo + ".rpt";
                        string report = parameters.Ruta+ "\\" + archivo + ".rpt";
                        reportDocument.Load(report); //C:\Users\alujan\Documents\Files\CR\new
                                                     // reportDocument.Load(@"C:\fe\cr\report1.rpt"); //C:\Users\alujan\Documents\Files\CR\new
                                                     //reportDocument.Load(emaling.DirectoryReport.ToString()); //C:\Users\alujan\Documents\Files\CR\new
                                                     //reportDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "\\FacturaResumenHanaV2.rpt");

                        //reportDocument.Load(emaling.DirectoryReport.ToString());
                        continuar = true;
                    }
                    catch (Exception ex)
                    {
                        descripcion = ex.Message + " | Cargar la platilla .rpt del reporte ";
                        continuar = false;
                        result = false;
                    }

                    if (continuar)
                    {
                        int num = checked(reportDocument.DataSourceConnections.Count - 1);
                        int index = 0;

                        while (index <= num)
                        {


                            //string strConnection = "DRIVER={HDBODBC};SERVERNODE=172.26.1.227:31015;DATABASE=SBODLA;PWD=Fshana18";


                            NameValuePairs2 logonProps2 = reportDocument.DataSourceConnections[index].LogonProperties;
                            logonProps2.Set("Provider", "B1CRHProxy");
                            logonProps2.Set("Server Type", "B1CRHProxy");
                            logonProps2.Set("Connection String", "DRIVER={B1CRHProxy};SERVERNODE=10.140.205.203:30015;DATABASE=TEST_AFOODS_20240913"); // ConfigurationManager.AppSettings["ConnectionStr"].ToString());
                                                                                                                                                   //logonProps2.Set("Locale Identifier", "1033");

                            reportDocument.DataSourceConnections[index].SetLogonProperties(logonProps2);

                            //reportDocument.DataSourceConnections[index].SetConnection(ConfigurationManager.AppSettings["sServerHana"].ToString(), ConfigurationManager.AppSettings["sCompanyName"].ToString(), ConfigurationManager.AppSettings["sUserBD"].ToString(), ConfigurationManager.AppSettings["sPassBD"].ToString());
                            reportDocument.DataSourceConnections[index].SetConnection("10.140.205.203:30015", "TEST_AFOODS_20240913", "B1ADMIN", "@rgentisB1");
                            checked { ++index; }
                        }


                        if (!string.IsNullOrEmpty(parameters.DocEntry))
                            reportDocument.SetParameterValue("DocKey@", parameters.DocEntry);
                        if (!string.IsNullOrEmpty(parameters.ObjectId) && archivo == "report1")
                            reportDocument.SetParameterValue("ObjectId@", parameters.ObjectId);

                        //var asdasd = saveCodeQr(parameters.UrlQr, "C:\\fe\\cr\\img\\qr.png");
                        var asdasd = saveCodeQr(parameters.UrlQr, parameters.Ruta +  "\\qr.png");


                        reportDocument.SetParameterValue("@ImageUrl", asdasd); //"C:\\fe\\cr\\img\\peru.png");

                        //reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, @"C:\fe\cr\files\" + "Factura_v"+parameters.Report+"_"+ parameters.DocEntry + ".pdf");
                        reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, parameters.Ruta +"\\files\\" + "Factura_v"+parameters.Report+"_"+ parameters.DocEntry + ".pdf");

                        reportDocument.Close();
                        reportDocument.Dispose();
                        reportDocument = null;
                        result = true;
                        descripcion = "";
                    }
                }


            }
            catch (Exception ex)
            {

                Exception exception = ex;
                descripcion = ex.Message + " | Al guardar el PDF";
                result = false;


            }


            //(return result;
            var responseData = new { ValorRecibido = result };
            return Json(responseData);
            //return Request.CreateResponse(HttpStatusCode.OK, "Valor recibido: " +  (result?"Ok":"Error"));
        }


        private string saveCodeQr(string text, string filename)
        {
            string str;
            try
            {
                using (Bitmap img = generateCodeQR(text, filename))
                {
                    img.Save(filename);
                }
                str = filename;
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                //Generate_PDF.Log.Debug(string.Format("Se encontró el siguiente ERROR : {0} ", ex.Message));
                str = filename;
            }
            return str;
        }

        private static Bitmap generateCodeQR(string text, string filename)
        {
            Bitmap bitmap;
            try
            {
                BarcodeWriter bw = new BarcodeWriter();
                EncodingOptions encOptions = new EncodingOptions()
                {
                    Width = 200,
                    Height = 200,
                    Margin = 0,
                    PureBarcode = false
                };
                encOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.Q);
                bw.Renderer = new BitmapRenderer();
                bw.Options = encOptions;
                bw.Format = BarcodeFormat.QR_CODE;
                bitmap = bw.Write(text);
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                //Generate_PDF.Log.Debug(string.Format("Se encontró el siguiente ERROR : {0} ", ex.Message));
                bitmap = null;
            }
            return bitmap;
        }
    }
}
