using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCM.Services.Dtos
{
    public class InsertInfo
    {
        public DateTime LastUploadDateTime { get; set; }
        public string EsbDtStart { get; set; }
        public string EsbDtEnd { get; set; }
        public int UploadTotalCount { get; set; }
        public int UploadSuccessCount { get; set; }
    }
}
