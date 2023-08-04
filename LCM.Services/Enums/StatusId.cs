using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCM.Services.Enums
{
    public enum StatusId
    {
        /// <summary>
        /// 尚未結案
        /// </summary>
        NoneClose = 1,
        /// <summary>
        /// 人工結案
        /// </summary>
        ManualClose = 555,
        /// <summary>
        /// 自動結案
        /// </summary>
        AutoClose = 666,
    }
}
