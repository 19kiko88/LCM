using System.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

using LCM.Services.Helpers;
using LCM.Services.Models;
using LCM.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace LCM.Services.Implements
{
    public class ExcelService: IExcelService
    {
        private readonly AppSettings.PathSettings _optPathSettings;

        public ExcelService(IOptions<AppSettings.PathSettings> optPathSettings)
        {
            _optPathSettings = optPathSettings.Value;
        }

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭不變動，可以與DB欄位直接對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public async Task<DataTable> ReadExcel(string filePath, int? lastCell, int headerRow = 1, int sheetIndex = 1)
        {
            var dt = new DataTable();
            if (!Directory.Exists(filePath))
            {
                dt = await ExcelHelper.ReadExcel(filePath, lastCell, headerRow, sheetIndex);
            }
            return dt;
        }

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭由Model重新取得，才能與DB欄位對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public async Task<DataTable> ReadExcel<T>(string filePath, int? lastCell, int headerRow = 1, int sheetIndex = 1)
        {
            var dt = new DataTable();
            if (!Directory.Exists(filePath))
            {
                dt = await ExcelHelper.ReadExcel<T>(filePath, lastCell, headerRow, sheetIndex);
            }
            return dt;
        }

        /// <summary>
        /// PK報表。Excel寫入 & 匯出
        /// </summary>
        /// <param name="data">報表內容</param>
        /// <param name="filePath">廠商提供報表的路徑，要把PK結果加到Sheet</param>
        /// <returns></returns>
        public async Task<FileStreamResult> ExportExcel(List<List<PK_RESULT_REPORT>> data, string filePath) 
        {
            XLWorkbook wb = new XLWorkbook(Path.Combine(_optPathSettings.TemplateFilePath, "PK_Report_Template_v2.0.xlsx"));
            IXLWorksheet wsClosed = wb.Worksheet("Closed");
            IXLWorksheet wsNoneClose = wb.Worksheet("NoneClose");
            int NumberOfLastRow = 3;// Worksheet.LastRowUsed().RowNumber();
            var listPkResult_Closed = data[0];
            var listPkResult_NoneClose = data[1];

            //Append List_Closed Data
            IXLCell CellForNewData_Closed = wsClosed.Cell(NumberOfLastRow + 1, 1);
            CellForNewData_Closed.InsertData(listPkResult_Closed);
            //Set Style
            ExcelHelper.SettingCellStyle(wsClosed, listPkResult_Closed.Count);

            //Append List_NoneClose Data
            IXLCell CellForNewData_NoneClose = wsNoneClose.Cell(NumberOfLastRow + 1, 1);
            CellForNewData_NoneClose.InsertData(listPkResult_NoneClose);
            //Set Style
            ExcelHelper.SettingCellStyle(wsNoneClose, listPkResult_NoneClose.Count);

            //把系統產出的報表appen到廠商提供excel的分頁後面
            XLWorkbook wbVendorReport = new XLWorkbook(filePath);
            wbVendorReport.AddWorksheet(wb.Worksheet(1));
            wbVendorReport.AddWorksheet(wb.Worksheet(2));

            //輸出Excel報表
            var ms = new MemoryStream();
            wbVendorReport.SaveAs(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
