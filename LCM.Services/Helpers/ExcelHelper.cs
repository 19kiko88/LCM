using ClosedXML.Excel;
using CsvHelper;
using DocumentFormat.OpenXml.Spreadsheet;
using LCM.Services.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCM.Services.Helpers
{
    public static class ExcelHelper
    {
        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭不變動，可以與DB欄位直接對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public static Task<DataTable> ReadExcel(string filePath, int? lastCell, int headerRow = 1, int sheetIndex = 1)
        {
            return ReadExcel(filePath, null, lastCell, headerRow, sheetIndex);
        }

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭由Model重新取得，才能與DB欄位對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public static Task<DataTable> ReadExcel<T>(string filePath, int? lastCell, int headerRow = 1, int sheetIndex = 1)
        {
            var obj = Activator.CreateInstance<T>();
            return ReadExcel(filePath, obj, lastCell, headerRow, sheetIndex);
        }

        /// <summary>
        /// Ref：[Read Excel worksheet into DataTable using ClosedXML]
        /// https://stackoverflow.com/questions/48756449/read-excel-worksheet-into-datatable-using-closedxml
        /// 讀取Excel內容轉DataTable
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="obj">新header對應物件</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public async static Task<DataTable> ReadExcel(string filePath, object? obj, int? lastCell, int headerRow = 1, int sheetIndex = 1) 
        {
            //Create a new DataTable.
            DataTable dt = new DataTable();

            using (XLWorkbook workBook = new XLWorkbook(filePath)) 
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(sheetIndex);

                //Loop through the Worksheet rows.
                bool columnNameSetDone = false;

                var sn = 1;
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if (sn == headerRow)
                    {
                        if (obj == null)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString()?.Replace("\r\n", " "));
                            }
                        }
                        else
                        {
                            foreach (var item in obj.GetType().GetProperties())
                            {
                                var col = obj.GetType().GetProperty(item.Name);//get class property name
                                //var columnName = obj.GetType().GetProperty(item.Name).GetValue(obj);
                                //var type = obj.GetType().GetProperty(item.Name).PropertyType;
                                dt.Columns.Add(col.GetValue(obj).ToString(), col.PropertyType);
                            }
                        }

                        columnNameSetDone = true;
                    }
                    else if (columnNameSetDone)
                    {
                        int i = 0;

                        if (!row.IsEmpty())
                        {//Add rows to DataTable.
                            dt.Rows.Add();
                            var endCell = lastCell.HasValue ? lastCell.Value : row.LastCellUsed().Address.ColumnNumber;
                            foreach (IXLCell cell in row.Cells(1/*row.FirstCellUsed().Address.ColumnNumber*/, endCell))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value;//.ToString();
                                i++;
                            }
                        }
                    }

                    sn++;
                }
            }

            return dt;
        }


        /// <summary>
        /// Ref：[save excel file to csv format without opening in .net core 3.1] 
        /// https://stackoverflow.com/questions/70084112/save-excel-file-to-csv-format-without-opening-in-net-core-3-1
        /// xlsx轉csv
        /// </summary>
        /// <param name="xlsxFilePath"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async static Task ExcelToCsv(string xlsxFilePath, DataTable dt)
        {
            string newFile = $"{xlsxFilePath.Replace(".xlsx", "")}.csv";
            using (StreamWriter writer = new StreamWriter(newFile))
            {
                using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    //add headers 
                    foreach (DataColumn dc in dt.Columns)
                    {
                        csv.WriteField(dc.ColumnName);
                    }
                    csv.NextRecord();
                    foreach (DataRow dr in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            csv.WriteField(dr[i]);
                        }
                        csv.NextRecord();
                    }
                }
            }
        }

        /// <summary>
        /// 設定Excell Cell Style
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="dataCount"></param>
        public static void SettingCellStyle(IXLWorksheet ws, int dataCount)
        {
            var NumberOfLastRow = 3;// Worksheet.LastRowUsed().RowNumber();
            if (dataCount > 0)
            {
                /*Set Default Cell Style
                 *Ref：https://github.com/ClosedXML/ClosedXML/issues/290  
                 */
                //IXLCell defaultcell = Worksheet.Cell(3, 1);
                ws.Range($"A4:AB{NumberOfLastRow + dataCount}").Style = ws.Cell(6, 40).Style;//defaultcell.Style; //預設全部Cell Background Color
                ws.Range($"A4:F{NumberOfLastRow + dataCount}").Style = ws.Cell(8, 40).Style;//灰底有邊框
                ws.Range($"N4:T{NumberOfLastRow + dataCount }").Style = ws.Cell(8, 40).Style;//灰底有邊框
                ws.Range($"AA4:AC{NumberOfLastRow + dataCount }").Style = ws.Cell(8, 40).Style;//灰底有邊框
            }
            //Set cell Style
            foreach (var cell in ws.Cells().Where(c => c.Value.ToString().Contains("@")))
            {
                var array = cell.Value?.ToString()?.Split('@');
                var styleString = "";

                if (array != null)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        styleString = string.Concat("@", array[i]);

                        if (styleString.Contains(EXCEL_CELL_STYLE.Waring))
                        {//設定Excell Cell Style => 黃底.紅字.粗體，拿掉Style字串
                            cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                            cell.Style.Font.FontColor = XLColor.Red;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            cell.Value = cell.Value?.ToString()?.Replace(EXCEL_CELL_STYLE.Waring, "");
                        }

                        if (styleString.Contains(EXCEL_CELL_STYLE.NoBorder))
                        {//設定Excell Cell Style => 拿掉Cell內.外邊框，拿掉Style字串        
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.None;
                            cell.Style.Border.InsideBorder = XLBorderStyleValues.None;
                            cell.Value = cell.Value?.ToString()?.Replace(EXCEL_CELL_STYLE.NoBorder, "");
                        }

                        if (styleString.Contains(EXCEL_CELL_STYLE.FormatCnMD))
                        {//設定PO出貨日欄位格式為m月d日
                            cell.Style.NumberFormat.Format = "m\"月\"d\"日\"";
                            cell.Value = cell.Value?.ToString()?.Replace(EXCEL_CELL_STYLE.FormatCnMD, "");
                        }

                        if (styleString.Contains(EXCEL_CELL_STYLE.Merge))
                        {//合併儲存格 by 小18
                            var range = cell?.Value?.ToString()?.Split(new char[] { '▲', '▼' });
                            var start = Convert.ToInt32(range[1]);
                            var end = Convert.ToInt32(range[2]);

                            cell.Value = cell.Value?.ToString()?.Replace($"{EXCEL_CELL_STYLE.Merge}▲{start}▼{end}", "");

                            ws.Range($"AA{NumberOfLastRow + start}:AA{NumberOfLastRow + end}").Merge();//是否人工結案
                            ws.Range($"AB{NumberOfLastRow + start}:AB{NumberOfLastRow + end}").Merge();//廠商備註
                            ws.Range($"AC{NumberOfLastRow + start}:AC{NumberOfLastRow + end}").Merge();//人工結案備註
                        }

                        if (styleString.Contains(EXCEL_CELL_STYLE.DropDownList))
                        {//是否人工結案下拉選單
                            cell.Value = cell.Value?.ToString()?.Replace(EXCEL_CELL_STYLE.DropDownList, "");
                            cell.SetDataValidation().List("\"是,否\"", true);                        
                        }
                    }
                }

            }
        }

        public static int GetSheetCount(string fullFilePath)
        {
            var res = 1;
            using (var workBook = new XLWorkbook(fullFilePath))
            {
                //Read the first Sheet from Excel file.
                res = workBook.Worksheets.Count;
            }

            return res;
        }
    }
}
