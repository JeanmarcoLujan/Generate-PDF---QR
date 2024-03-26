using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FEGenerateReport.Models.DTO
{
    public class RequestReportDTO
    {
        public string DocEntry {  get; set; }
        public string ObjectId { get; set; }
        public string UrlQr { get; set; }
        public string Report { get; set; }
        public string Ruta { get; set; }
    }
}