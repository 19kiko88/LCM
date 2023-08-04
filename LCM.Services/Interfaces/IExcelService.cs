using LCM.Services.Models;
using Microsoft.AspNetCore.Mvc;
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
        /// 讀取Excel內容轉DataTable(DT表頭不變動與實體excel檔表頭一樣)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <param name="sheetIndex">要讀取的分頁index</param>
        /// <returns></returns>
        public Task<DataTable> ReadExcel(string filePath, int? lastCell, int headerRow = 1, int sheetIndex = 1, bool onlyHeader = false);

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭由Model重新取得，實體excel檔表頭不限制內容)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <param name="sheetIndex">要讀取的分頁index</param>
        /// <returns></returns>
        public Task<DataTable> ReadExcel<T>(string filePath, int? lastCell, int headerRow = 1, int sheetIndex = 1, bool onlyHeader = false);

        /// <summary>
        /// 匯出PK報表
        /// </summary>
        /// <param name="data">報表內容</param>
        /// <param name="filePath">廠商提供報表的路徑，要把PK結果加到Sheet</param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportExcel(List<List<PK_RESULT_REPORT>> data, string filePath);

        public Task<FileStreamResult> ExportExcel(string filePath);

        /// <summary>
        /// excel表頭檢查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="includeColumns">指定檢核欄位</param>
        /// <param name="exceptColumns">排除檢核欄位</param>
        /// <returns></returns>
        public Task<bool> ExcelHeaderColumnCheck<T>(DataTable dt, string[] includeColumns = null, string[] exceptColumns = null);
    }
}
