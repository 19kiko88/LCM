using LCM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using LCM.Services.service;
using ClosedXML.Excel;

namespace LCM.Services.Implements
{
    public class ExcelService: IExcelService
    {
        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭不變動，可以與DB欄位直接對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public async Task<DataTable> ReadExcel(string filePath, int? lastCell, int headerRow = 1)
        {
            var dt = new DataTable();
            if (!Directory.Exists(filePath))
            {
                dt = await ExcelHelper.ReadExcel(filePath, lastCell, headerRow);
            }
            return dt;
        }

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭由Model重新取得，才能與DB欄位對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public async Task<DataTable> ReadExcel<T>(string filePath, int? lastCell, int headerRow = 1)
        {
            var dt = new DataTable();
            if (!Directory.Exists(filePath))
            {
                dt = await ExcelHelper.ReadExcel<T>(filePath, lastCell, headerRow);
            }
            return dt;
        }
    }
}
