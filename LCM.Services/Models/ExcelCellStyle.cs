using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCM.Services.Models
{
    public static class ExcelCellStyle
    {
        public static string Waring { get { return "@Waring"; } }
        public static string NoBorder { get { return "@NoBorder"; } }
        public static string MergeNote { get { return "@MergeNote"; } }
        public static string MergeDown { get { return "@MergeDown"; } }

        public static string FormatCnMD { get { return "@FormatCnMD"; } }
        public static string DropDownList { get { return "@DropDownList"; } }
    }
}
