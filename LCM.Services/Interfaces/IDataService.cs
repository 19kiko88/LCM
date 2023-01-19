using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using LCM.Services.Models;

namespace LCM.Services.Interfaces
{
    public interface IDataService
    {
        /// <summary>
        /// 小18 data insert to DB
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public Task<UPLOAD_INFO> InsertS18(DataTable dt, string updateUser);
        /// <summary>
        /// 大18 data insert to DB
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public Task<UPLOAD_INFO> InsertB18(DataTable dt, string updateUser);
        /// <summary>
        /// 取得PK報表EXCEL內容
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Task<List<List<PK_RESULT_REPORT>>> GetPkBs18Content(DataTable dt);        
    }
}
