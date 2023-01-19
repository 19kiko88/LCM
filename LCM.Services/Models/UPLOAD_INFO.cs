using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCM.Services.Models
{
    public class UPLOAD_INFO
    {
        public DateTime uploadDT { get; set; }
        public DateTime esbDTStart { get; set; }
        public DateTime esbDTSEnd { get; set; }
        public DateTime dataMarketDTStart { get; set; }
        public DateTime dataMarketDTEnd { get; set; }
        public int totalDataCount { get; set; }
        public int uploadDataCount { get; set; }
        public string message { get; set; }
    }
}
