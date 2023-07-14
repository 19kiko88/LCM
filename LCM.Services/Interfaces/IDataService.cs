using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using LCM.Services.Models;
using DocumentFormat.OpenXml.Spreadsheet;

namespace LCM.Services.Interfaces
{
    public interface IDataService
    {
        public string UserName { get; set; }

        /// <summary>
        /// 小18 data insert to DB
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public Task<string> InsertS18(DataTable dt, string updateUser);
        /// <summary>
        /// 大18 data insert to DB
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public Task<string> InsertB18(DataTable dt, string updateUser);
        /// <summary>
        /// 取得PK報表EXCEL內容
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Task<List<List<PK_RESULT_REPORT>>> GetPkBs18Content(DataTable dt);
        /// <summary>
        /// 手動結案
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Task<int> ManualClose(DataTable dt, string modifyUser);

        /// <summary>
        /// 更新小18資料狀態 & 廠商備註欄位
        /// </summary>
        /// <param name="closeData">結案資料</param>
        /// <param name="noneCloseData">未結案資料</param>
        /// <returns></returns>
        public Task<int> UpdateS18(List<PK_RESULT_REPORT> closeData, List<PK_RESULT_REPORT> noneCloseData);
    }
}
