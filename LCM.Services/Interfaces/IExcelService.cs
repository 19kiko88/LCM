using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCM.Services.Interfaces
{
    public interface IExcelService
    {
        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭不變動，可以與DB欄位直接對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public Task<DataTable> ReadExcel(string filePath, int? lastCell, int headerRow = 1);

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭由Model重新取得，才能與DB欄位對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public Task<DataTable> ReadExcel<T>(string filePath, int? lastCell, int headerRow = 1);
    }
}
