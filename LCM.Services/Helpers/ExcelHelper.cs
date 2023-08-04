using ClosedXML.Excel;
using CsvHelper;
using DocumentFormat.OpenXml.Spreadsheet;
using LCM.Services.Models;
using LCM.Services.Models.DataTablelHederMapping;
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
        /// 讀取Excel內容轉DataTable(DT表頭不變動與實體excel檔表頭一樣)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public static Task<DataTable> ReadExcel(string filePath, int? lastCell, int headerRow = 1, int sheetIndex = 1, bool onlyHeader = false)
        {
            return ReadExcel(filePath, null, lastCell, headerRow, sheetIndex, onlyHeader);
        }

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭由Model重新取得，實體excel檔表頭不限制內容)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public static Task<DataTable> ReadExcel<T>(string filePath, int? lastCell, int headerRow = 1, int sheetIndex = 1, bool onlyHeader = false)
        {
            var obj = Activator.CreateInstance<T>();
            return ReadExcel(filePath, obj, lastCell, headerRow, sheetIndex, onlyHeader);
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
        public async static Task<DataTable> ReadExcel(string filePath, object? obj, int? lastCell, int headerRow = 1, int sheetIndex = 1, bool onlyHeader = false)
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
                            var columnSn = 1;
                            foreach (IXLCell cell in row.Cells(false))
                            {
                                if (cell.IsEmpty())
                                {
                                    dt.Columns.Add($"Column_{columnSn}");
                                }
                                else
                                {
                                    dt.Columns.Add(cell.Value.ToString()?.Replace("\r\n", " ").TrimEnd());
                                }                                
                                columnSn++;
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

                        if (onlyHeader == true)
                        {
                            break;
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
            var NumberOfLastCell = 30;// Worksheet.LastRowUsed().RowNumber();
            var sampleStyleCell = 41;

            if (dataCount > 0)
            {
                /*Set Default Cell Style
                 *Ref：https://github.com/ClosedXML/ClosedXML/issues/290  
                 */
                //IXLCell defaultcell = Worksheet.Cell(3, 1);
                ws.Range($"A4:AD{NumberOfLastRow + dataCount}").Style = ws.Cell(6, sampleStyleCell).Style;//defaultcell.Style; //預設全部Cell Background Color
                ws.Range($"A4:F{NumberOfLastRow + dataCount}").Style = ws.Cell(8, sampleStyleCell).Style;//灰底有邊框
                ws.Range($"N4:T{NumberOfLastRow + dataCount }").Style = ws.Cell(8, sampleStyleCell).Style;//灰底有邊框
                ws.Range($"AA4:AD{NumberOfLastRow + dataCount }").Style = ws.Cell(8, sampleStyleCell).Style;//灰底有邊框
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

                        if (styleString.Contains(ExcelCellStyle.Waring))
                        {//設定Excell Cell Style => 黃底.紅字.粗體，拿掉Style字串
                            cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                            cell.Style.Font.FontColor = XLColor.Red;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            cell.Value = cell.Value?.ToString()?.Replace(ExcelCellStyle.Waring, "");
                        }

                        if (styleString.Contains(ExcelCellStyle.NoBorder))
                        {//設定Excell Cell Style => 拿掉Cell內.外邊框，拿掉Style字串        
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.None;
                            cell.Style.Border.InsideBorder = XLBorderStyleValues.None;
                            cell.Value = cell.Value?.ToString()?.Replace(ExcelCellStyle.NoBorder, "");
                        }

                        if (styleString.Contains(ExcelCellStyle.FormatCnMD))
                        {//設定PO出貨日欄位格式為m月d日
                            cell.Style.NumberFormat.Format = "m\"月\"d\"日\"";
                            cell.Value = cell.Value?.ToString()?.Replace(ExcelCellStyle.FormatCnMD, "");
                        }

                        if (styleString.Contains(ExcelCellStyle.MergeNote))
                        {//合併儲存格 by 小18(廠商備註, 否人工結案, 人工結案備註)
                            var range = cell?.Value?.ToString()?.Split(new char[] { '▲', '▼' });
                            var start = Convert.ToInt32(range[1]);
                            var end = Convert.ToInt32(range[2]);
                            
                            cell.Value = cell.Value?.ToString()?.Replace($"{ExcelCellStyle.MergeNote}▲{start}▼{end}", "");                            

                            string[] columnArray = new string[] { "AB", "AC", "AD" };
                            for (int ii = 0; ii < columnArray.Length; ii++)
                            {
                                ws.Range($"{columnArray[ii]}{NumberOfLastRow + start}:{columnArray[ii]}{NumberOfLastRow + end}").Merge();
                            }
                        }


                        if (styleString.Contains(ExcelCellStyle.MergeDown))
                        {//向下合併儲存格 
                            var range = cell?.Value?.ToString()?.Split(new char[] {'▼' });
                            var mergeCount = Convert.ToInt32(range[1]);

                            //cell.Value = cell.Value?.ToString();?.Replace($"{ExcelCellStyle.MergeDown}▼{mergeCount}", "");
                            ws.Cell(cell.Address).SetValue<string>($"{cell.Value?.ToString()?.Split('@')[0]}");

                            ws.Range($"{cell.Address}:{cell.Address.ColumnLetter}{cell.Address.RowNumber + mergeCount}").Merge();
                        }

                        if (styleString.Contains(ExcelCellStyle.DropDownList))
                        {//是否人工結案下拉選單
                            cell.Value = cell.Value?.ToString()?.Replace(ExcelCellStyle.DropDownList, "");
                            cell.SetDataValidation().List("\"是,否\"", true);                        
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 取得Excel分頁數量
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
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

        /// <summary>
        /// excel表頭檢查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="includeColumns">指定檢核欄位</param>
        /// <param name="exceptColumns">排除檢核欄位</param>
        /// <returns></returns>
        public static bool ExcelHeaderColumnCheck<T>(DataTable dt, string[] includeColumns = null, string[] exceptColumns = null)
        {
            var obj = Activator.CreateInstance<T>();
            var validateHeaderColumns = obj.GetType().GetProperties().Select(c => obj.GetType().GetProperty(c.Name).GetValue(obj).ToString()).ToArray();
            var excelHeaderColumns = new string[] {};
            if (dt != null && dt.Columns.Count > 0)
            {
                excelHeaderColumns = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            }

            if (includeColumns != null && includeColumns.Length > 0)
            {//指定檢核欄位
                validateHeaderColumns = validateHeaderColumns.Where(c => includeColumns.Contains(c)).ToArray();
                excelHeaderColumns = excelHeaderColumns.Where(c => includeColumns.Contains(c)).ToArray();
            }

            if (exceptColumns != null && exceptColumns.Length > 0)
            {//排除檢核欄位
                validateHeaderColumns = validateHeaderColumns.Where(c => !exceptColumns.Contains(c)).ToArray();
                excelHeaderColumns = excelHeaderColumns.Where(c => !exceptColumns.Contains(c)).ToArray();
            }

            var passCheckCount = excelHeaderColumns.Where(c => !string.IsNullOrEmpty(c)).Select(c => validateHeaderColumns.Contains(c)).Count();
            var res = passCheckCount != validateHeaderColumns.Length ? false : true;

            return res;
        }
    }
}
