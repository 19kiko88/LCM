﻿using System.Data;
using EFCore.BulkExtensions;
using System.Text.RegularExpressions;

using LCM.Repositories;
using LCM.Repositories.Models;
using LCM.Services.Interfaces;
using LCM.Services.Models.DataTablelHederMapping;
using LCM.Services.Models;
using System.Transactions;
using LCM.Services.Enums;
using LCM.Services.Dtos;

namespace LCM.Services.Implements
{
    public class DataService : IDataService
    {
        private readonly CAEDB01Context _context;
        public string UserName { get; set; }

        public DataService(CAEDB01Context context) 
        {
            _context = context;
        }

        /// <summary>
        /// 小18 data insert to DB
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public async Task<InsertInfo> InsertS18(DataTable dt, string updateUser)
        {
            var res = new InsertInfo();
            var s18Header = new XxPort0001Resell();
            var listData = new List<XxPor0001_Resell>();
            var updateTime = DateTime.Now;

            foreach (var dr in dt.AsEnumerable())
            {
                Int32.TryParse(dr[s18Header.Quantity].ToString(), out var quantity) ;
                decimal.TryParse(dr[s18Header.NetPrice].ToString(), out var netPrice) ;
                DateTime.TryParse(dr[s18Header.ShipmentDate].ToString(), out var shipmentDate);
                DateTime.TryParse(dr[s18Header.Date].ToString(), out var date);

                listData.Add(new XxPor0001_Resell()
                {
                    PENo = dr[s18Header.PENo].ToString(),
                    OrderNo = CleanOrderNo(dr[s18Header.OrderNumber].ToString()),
                    SOLine = dr[s18Header.SOLine].ToString(),
                    PartNo = dr[s18Header.PN].ToString(),
                    Quantity = quantity,
                    NetPrice = netPrice,
                    ShipmentDate = DateOnly.FromDateTime(shipmentDate),
                    SystemUpdateDate = DateOnly.FromDateTime(date),
                    Updater = updateUser,
                    UpdateTime = updateTime,
                    StatusID = (int?)StatusId.NoneClose
                });
            }

            //跟DB資料比對，拿掉OrderNo. SOLine已經存在DB的重複資料
            var listWithoutDuplicateData = (from a in listData
                      join b in _context.XxPor0001_Resell.Select(c => new { c.OrderNo, c.SOLine })
                      on new { a.OrderNo, a.SOLine } equals new { b.OrderNo, b.SOLine }
                      into groupjoin
                      from g in groupjoin.DefaultIfEmpty()
                      where g == null
                      select a).ToList();

            //SqlBulkCopy Insert with Identity Column
            //Ref：https://stackoverflow.com/questions/6651809/sqlbulkcopy-insert-with-identity-column
            var config = new BulkConfig { SetOutputIdentity = false, BatchSize = 500000, UseTempDB = true };
            _context.BulkInsert(listWithoutDuplicateData, config);

            res.LastUploadDateTime = updateTime;
            res.EsbDtStart = Convert.ToDateTime(listData.Select(c => c.SystemUpdateDate).Min().ToShortDateString()) == DateTime.MinValue ? "" : listData.Select(c => c.SystemUpdateDate).Min().ToShortDateString();
            res.EsbDtEnd = Convert.ToDateTime(listData.Select(c => c.SystemUpdateDate).Max().ToShortDateString()) == DateTime.MinValue ? "" : listData.Select(c => c.SystemUpdateDate).Max().ToShortDateString();
            res.UploadSuccessCount = listWithoutDuplicateData.Count;
            res.UploadTotalCount = listData.Count;

            return res;
        }

        /// <summary>
        /// 大18 data insert to DB
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public async Task<InsertInfo> InsertB18(DataTable dt, string updateUser)
        {
            var res = new InsertInfo();
            var b18Header = new XxPoReceipt();
            var listData = new List<Xx_Po_Receipt>();
            var updateTime = DateTime.Now;
            var lastUpdateTime = DateTime.MinValue;

            foreach (var dr in dt.AsEnumerable())
            {
                Int32.TryParse(dr[b18Header.PoLineNum].ToString(), out var poLineNumber);
                Int32.TryParse(dr[b18Header.Quantity].ToString(), out var quantity);
                decimal.TryParse(dr[b18Header.PoUnitPrice].ToString(), out var poUnitPrice);
                DateTime.TryParse(dr[b18Header.TransactionDate].ToString(), out var transactionDate);

                if (!DateTime.TryParse(dr[b18Header.SystemUpdateDate].ToString(), out lastUpdateTime))
                {//大18的lastUpdateTime格式有可能為數值或是時間格式，數值的話要在轉時間格式
                    lastUpdateTime = DateTime.FromOADate(Convert.ToDouble(dr[b18Header.SystemUpdateDate]));
                }                

                listData.Add(new Xx_Po_Receipt()
                {
                    TransactionID = dr[b18Header.TransactionId].ToString(),
                    PONo = dr[b18Header.PoNumber].ToString(),
                    POLineNo = poLineNumber,
                    POUnitPrice = poUnitPrice,
                    PartNo = dr[b18Header.PartNo].ToString(),
                    TransactionDate = DateOnly.FromDateTime(transactionDate),                    
                    SystemUpdateDate = DateOnly.FromDateTime(lastUpdateTime),
                    Updater = updateUser,
                    Quantity = quantity,
                    UpdateTime = updateTime                    
                });
            }

            //跟DB資料比對，拿掉TransactionID已經存在DB的重複資料
            var listWithoutDuplicateData = (from a in listData                                            
                                            join b in _context.Xx_Po_Receipt.Select(c => c.TransactionID)
                                            on a.TransactionID equals b
                                            into groupjoin
                                            from g in groupjoin.DefaultIfEmpty()
                                            where g == null
                                            select a).ToList();
            //SqlBulkCopy Insert with Identity Column
            //Ref：https://stackoverflow.com/questions/6651809/sqlbulkcopy-insert-with-identity-column
            var config = new BulkConfig { SetOutputIdentity = false, BatchSize = 500000, UseTempDB = true };
            _context.BulkInsert(listWithoutDuplicateData, config);

            var dataMarketDTStart = Convert.ToDateTime(listData.Select(c => c.SystemUpdateDate).Min().ToShortDateString()) == DateTime.MinValue ? "" : listData.Select(c => c.SystemUpdateDate).Min().ToShortDateString();
            var dataMarketDTEnd = Convert.ToDateTime(listData.Select(c => c.SystemUpdateDate).Max().ToShortDateString()) == DateTime.MinValue ? "" : listData.Select(c => c.SystemUpdateDate).Max().ToShortDateString();

            res.LastUploadDateTime = updateTime;
            res.EsbDtStart = Convert.ToDateTime(listData.Select(c => c.SystemUpdateDate).Min().ToShortDateString()) == DateTime.MinValue ? "" : listData.Select(c => c.SystemUpdateDate).Min().ToShortDateString();
            res.EsbDtEnd = Convert.ToDateTime(listData.Select(c => c.SystemUpdateDate).Max().ToShortDateString()) == DateTime.MinValue ? "" : listData.Select(c => c.SystemUpdateDate).Max().ToShortDateString();
            res.UploadSuccessCount = listWithoutDuplicateData.Count;
            res.UploadTotalCount = listData.Count;

            return res;
        }

        /// <summary>
        /// 取得PK報表EXCEL內容，並轉換成List
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async Task<List<List<PK_RESULT_REPORT>>> GetPkBs18Content(DataTable dt)
        {
            var poStatus = _context.PoStatusDesc.ToList();
            var vendorReportHeader = new VendorReport();
            var listData = new List<LCM.Services.Models.VENDOR_REPORT>();
            var currentPE_No = "";
            var currentSO = "";
            var currentSO_Line = "";
            var res = new List<List<PK_RESULT_REPORT>>();
            var listPkResult_Closed = new List<PK_RESULT_REPORT>();
            var listPkResult_NoneClose = new List<PK_RESULT_REPORT>();

            if (dt != null && dt.Rows.Count > 0)
            {
                #region 廠商提供報表轉物件 VENDER_REPORT
                var serial = 0;
                foreach (var dr in dt.AsEnumerable())
                {
                    if (
                        !string.IsNullOrEmpty(dr[vendorReportHeader.S18_PE_No].ToString()) &&
                        !string.IsNullOrEmpty(dr[vendorReportHeader.S18_SO_No].ToString()) &&
                        !string.IsNullOrEmpty(dr[vendorReportHeader.S18_SO_Line].ToString()) &&
                        (currentPE_No != dr[vendorReportHeader.S18_PE_No].ToString() || currentSO != dr[vendorReportHeader.S18_SO_No].ToString() || currentSO_Line != dr[vendorReportHeader.S18_SO_Line].ToString())
                        )
                    {
                        //set PK
                        currentPE_No = dr[vendorReportHeader.S18_PE_No].ToString();
                        currentSO = dr[vendorReportHeader.S18_SO_No].ToString();
                        currentSO_Line = dr[vendorReportHeader.S18_SO_Line].ToString();
                    }

                    DateTime.TryParse(dr[vendorReportHeader.S18_Promised_Date].ToString(), out var s18_Promised_Date);
                    Int32.TryParse(dr[vendorReportHeader.S18_Qty].ToString(), out var s18_Qty);
                    decimal.TryParse(dr[vendorReportHeader.S18_Unit_Price].ToString(), out var s18oUnitPrice);
                    Int32.TryParse(dr[vendorReportHeader.B18_PO_Line_No].ToString(), out var b18_PoLineNo);
                    var checkB18_Qty = Int32.TryParse(dr[vendorReportHeader.B18_Qty].ToString(), out var b18_Qty);
                    var checkB18_PO_Shipment_Date = DateTime.TryParse(dr[vendorReportHeader.B18_PO_Shipment_Date].ToString(), out var b18_PoShipmentDate);
                    decimal.TryParse(dr[vendorReportHeader.B18_PO_Unit_Price].ToString(), out var b18_PoUnitPrice);


                    if (
                        !string.IsNullOrEmpty(dr[vendorReportHeader.S18_PE_No].ToString()?.Trim()) &&
                        !string.IsNullOrEmpty(dr[vendorReportHeader.S18_SO_No].ToString()?.Trim()) &&
                        !string.IsNullOrEmpty(dr[vendorReportHeader.S18_SO_Line].ToString()?.Trim())
                        )
                    {
                        serial++;
                    }

                    listData.Add(new VENDOR_REPORT()
                    {                       
                        S18_Promised_Date = DateOnly.FromDateTime(s18_Promised_Date),
                        S18_PE_No = dr[vendorReportHeader.S18_PE_No].ToString()?.Trim(),
                        S18_SO_No = dr[vendorReportHeader.S18_SO_No].ToString()?.Trim(),
                        S18_SO_Line = dr[vendorReportHeader.S18_SO_Line].ToString()?.Trim(),
                        S18_PN = dr[vendorReportHeader.S18_PN].ToString().Trim(),
                        S18_Qty = s18_Qty,
                        S18_Unit_Price = s18oUnitPrice,
                        B18_SO_No = dr[vendorReportHeader.B18_SO_No].ToString()?.Trim(),
                        B18_SO_Line = !string.IsNullOrEmpty(dr[vendorReportHeader.B18_PO_No].ToString()?.Trim()) && !string.IsNullOrEmpty(dr[vendorReportHeader.B18_PO_Line_No].ToString()?.Trim()) ? currentSO_Line : null,//dr[vendorReportHeader.S18_SO_Line].ToString(),
                        B18_PO_No = dr[vendorReportHeader.B18_PO_No].ToString()?.Trim(),
                        B18_PO_Line_No = b18_PoLineNo,
                        B18_PN = dr[vendorReportHeader.B18_PN].ToString()?.Trim(),
                        B18_Qty = b18_Qty,
                        B18_Qty_String = checkB18_Qty ? "" : dr[vendorReportHeader.B18_Qty].ToString(),
                        B18_PO_Shipment_Date = DateOnly.FromDateTime(b18_PoShipmentDate),
                        B18_PO_Shipment_Date_String = checkB18_PO_Shipment_Date ? b18_PoShipmentDate.ToString("yyyy/MM/dd") : dr[vendorReportHeader.B18_PO_Shipment_Date].ToString(),                            
                        B18_PO_Unit_Price = b18_PoUnitPrice,
                        B18_Project = dr[vendorReportHeader.B18_Project].ToString()?.Trim(),
                        B18_Note = dr[vendorReportHeader.B18_Note].ToString()?.Trim(),
                        SN = serial
                    });
                }
                #endregion

                #region 轉換成要輸出的格式
                var s18VendorReportData =
                    listData
                    .Select((c, serialNo) => new
                    {
                        SN = serialNo,
                        c.S18_Promised_Date,
                        c.S18_PE_No,
                        c.S18_SO_No,
                        c.S18_SO_Line,
                        c.S18_PN,
                        c.S18_Qty,
                        c.S18_Unit_Price
                    })
                    .Where(c => !string.IsNullOrEmpty(c.S18_SO_No) && !string.IsNullOrEmpty(c.S18_SO_Line)).OrderBy(c => c.SN)
                    .GroupBy(g => new { g.S18_PE_No, g.S18_SO_No, g.S18_SO_Line })
                    .Select(c =>
                    new
                    {
                        SN = c.Select(s => s.SN).FirstOrDefault(),
                        S18_Promised_Date = c.Select(s => s.S18_Promised_Date).FirstOrDefault(),
                        c.Key.S18_PE_No,
                        c.Key.S18_SO_No,
                        c.Key.S18_SO_Line,
                        S18_PN = c.Select(s => s.S18_PN).FirstOrDefault(),
                        S18_Qty = c.Sum(s => s.S18_Qty),
                        S18_Unit_Price = c.Select(s => s.S18_Unit_Price).FirstOrDefault()
                    })
                    ;

                var b18VendorReportData = listData.Select((c, serialNo) => new
                {
                    c.B18_SO_No,
                    c.B18_SO_Line,
                    c.B18_PO_No,
                    c.B18_PO_Line_No,
                    c.B18_PN,
                    c.B18_Qty,
                    c.B18_Qty_String,
                    c.B18_PO_Shipment_Date,
                    c.B18_PO_Shipment_Date_String,
                    c.B18_PO_Unit_Price,
                    c.B18_Note,
                    c.B18_Project
                });

                var dbS18 = new XxPor0001_Resell();
                var dbB18 = new Xx_Po_Receipt();

                foreach (var s18_V_Item in s18VendorReportData)
                {//小18資料處理
                    var listPkResult = new List<PK_RESULT_REPORT>();
                    var data = new PK_RESULT_REPORT();
                    var dbB18QtySum = 0;
                    var errorCount = 0;

                    //廠商提供小18
                    data.V_S18_PE_No = s18_V_Item.S18_PE_No;
                    data.V_S18_SO_No = s18_V_Item.S18_SO_No;
                    data.V_S18_SO_Line = s18_V_Item.S18_SO_Line;
                    data.V_S18_PN = s18_V_Item.S18_PN;
                    data.V_S18_Qty = s18_V_Item.S18_Qty.ToString();
                    data.V_S18_Unit_Price = s18_V_Item.S18_Unit_Price.ToString();

                    //DB小18
                    dbS18 = _context.XxPor0001_Resell.Where(c => c.PENo == s18_V_Item.S18_PE_No && c.OrderNo == s18_V_Item.S18_SO_No && c.SOLine == s18_V_Item.S18_SO_Line).FirstOrDefault();
                    data.DB_S18_PENo = StringPK(s18_V_Item.S18_PE_No, dbS18?.PENo?.Trim(), ref errorCount);
                    data.DB_S18_OrderNo = StringPK(s18_V_Item.S18_SO_No, dbS18?.OrderNo?.Trim(), ref errorCount);
                    data.DB_S18_SOLine = StringPK(s18_V_Item.S18_SO_Line, dbS18?.SOLine?.Trim(), ref errorCount);
                    data.DB_S18_PartNo = StringPK(s18_V_Item.S18_PN, dbS18?.PartNo?.Trim(), ref errorCount);
                    data.DB_S18_Quantity = StringPK(s18_V_Item.S18_Qty.ToString(), dbS18?.Quantity.ToString()?.Trim(), ref errorCount);
                    data.DB_S18_NetPrice = StringPK(s18_V_Item.S18_Unit_Price.ToString(), dbS18?.NetPrice.ToString()?.Trim(), ref errorCount);
                    data.DB_S18_ShipmentDate = string.IsNullOrEmpty(dbS18?.ShipmentDate.ToString("yyyy/MM/dd")) ? string.Concat("No Data", ExcelCellStyle.Waring) : dbS18?.ShipmentDate.ToString("yyyy/MM/dd");
                    data.MANUAL_CLOSED_NOTE = dbS18?.ProcureRemark??string.Empty;
                    data.HiddenS18StatusId = dbS18 != null && dbS18.StatusID.HasValue ? dbS18.StatusID.Value : (int)StatusId.NoneClose;//  (int)StatusId.NoneClose ;

                    var b18_V_Items = b18VendorReportData
                        .Where(c => c.B18_SO_No == s18_V_Item.S18_SO_No && c.B18_SO_Line == s18_V_Item.S18_SO_Line)
                        .OrderBy(o => o.B18_PO_Shipment_Date).ThenBy(o => o.B18_PO_No).ThenBy(o => o.B18_PO_Line_No)
                        .ToList();

                    if (b18_V_Items.Count > 0)
                    {//大18資料處理

                        var sn = 0;
                        var needMerge = false;
                        var mergeString = "";
                        var b18Group = b18_V_Items.GroupBy(g => new { g.B18_PO_Shipment_Date, g.B18_PO_No, g.B18_PO_Line_No }).Select(c => new
                        {
                            c.Key.B18_PO_Shipment_Date,
                            c.Key.B18_PO_No,
                            c.Key.B18_PO_Line_No,
                            SumQty = c.Sum(s => s.B18_Qty),
                            CNT = c.Count()
                        }).Where(c => c.CNT > 1).ToList();


                        foreach (var b18_V_Item in b18_V_Items)
                        {
                            sn++;

                            /*
                             * DB資料有一筆以上時，大18資料取TransactionDate大於廠商提供之交貨日，且TransactionDate離交貨日最近之資料。
                             * by bruenor 20230711
                             */
                            dbB18 = _context.Xx_Po_Receipt.Where(c => 
                            c.PONo == b18_V_Item.B18_PO_No && 
                            c.POLineNo == b18_V_Item.B18_PO_Line_No &&
                            c.TransactionDate > b18_V_Item.B18_PO_Shipment_Date
                            )
                            .OrderBy(o => o.TransactionDate)
                            .FirstOrDefault();

                            if (sn > 1)
                            {//第二筆資料起不須列出小18欄位資料
                                data = new PK_RESULT_REPORT();
                            }

                            #region 大18DB資料合併儲存格
                            var mergeCount =
                                b18Group
                                .Where(c => c.B18_PO_Shipment_Date == b18_V_Item.B18_PO_Shipment_Date && c.B18_PO_No == b18_V_Item.B18_PO_No && c.B18_PO_Line_No == b18_V_Item.B18_PO_Line_No)
                                .FirstOrDefault()?.CNT;

                            if (mergeCount > 1 && string.IsNullOrEmpty(mergeString))
                            {
                                needMerge = true;
                                mergeString = $"{ExcelCellStyle.MergeDown}▼{(mergeCount) - 1}";

                                dbB18QtySum += b18Group.FirstOrDefault()?.SumQty ?? 0;
                            }
                            else
                            {
                                needMerge = false;
                                mergeString = "";
                            }
                            #endregion

                            //廠商提供大18 PO出貨日 & Transaction_Date比對
                            if (!DateTime.TryParse(b18_V_Item.B18_PO_Shipment_Date_String, out var shipDate))
                            {//大18 PO出貨日不為日期格式
                                data.V_B18_PO_Shipment_Date = string.Concat(b18_V_Item.B18_PO_Shipment_Date_String, ExcelCellStyle.Waring, ExcelCellStyle.FormatCnMD);
                            }
                            else
                            {
                                if (dbB18?.TransactionDate < b18_V_Item.B18_PO_Shipment_Date.AddDays(180))
                                {//Transaction_Date在PO出貨日加180天內 => 比對正常
                                    data.V_B18_PO_Shipment_Date = string.Concat(b18_V_Item.B18_PO_Shipment_Date.ToString("yyyy/MM/dd"), ExcelCellStyle.FormatCnMD);
                                }
                                else
                                {//Transaction_Date超出PO出貨日加180天 => 比對異常
                                    data.V_B18_PO_Shipment_Date = string.Concat(b18_V_Item.B18_PO_Shipment_Date.ToString("yyyy/MM/dd"), ExcelCellStyle.Waring, ExcelCellStyle.FormatCnMD);
                                    errorCount++;
                                }
                            }

                            data.V_B18_SO_No = b18_V_Item.B18_SO_No;
                            data.V_B18_PO_No = b18_V_Item.B18_PO_No;
                            data.V_B18_PO_Line_No = b18_V_Item.B18_PO_Line_No > 0 ? b18_V_Item.B18_PO_Line_No.ToString() : "";
                            data.V_B18_PN = b18_V_Item.B18_PN;
                            data.V_B18_Qty = string.IsNullOrEmpty(b18_V_Item.B18_Qty_String) ? b18_V_Item.B18_Qty.ToString() : b18_V_Item.B18_Qty_String;
                            data.V_B18_PO_Unit_Price = b18_V_Item.B18_PO_Unit_Price.ToString();

                            //DB大18
                            data.DB_B18_PONo = StringPK(b18_V_Item.B18_PO_No, dbB18?.PONo.Trim(), ref errorCount, mergeString);
                            data.DB_B18_POLineNo = StringPK(b18_V_Item.B18_PO_Line_No.ToString(), dbB18?.POLineNo.ToString()?.Trim(), ref errorCount, mergeString);
                            data.DB_B18_PartNo = StringPK(b18_V_Item.B18_PN, dbB18?.PartNo?.Trim()!, ref errorCount, mergeString);

                            //DB_B18_Quantity
                            if (needMerge == false)
                            {
                                data.DB_B18_Quantity = StringPK(b18_V_Item.B18_Qty.ToString(), dbB18?.Quantity.ToString()?.Trim(), ref errorCount, mergeString);
                                dbB18QtySum += dbB18?.Quantity ?? 0;
                            }
                            else
                            {
                                var dbSum = ""; 
                                if (dbB18 != null)
                                {
                                    dbSum = dbB18?.Quantity.ToString();
                                }

                                data.DB_B18_Quantity = StringPK(b18Group.FirstOrDefault()?.SumQty.ToString() ?? 0.ToString(), dbSum, ref errorCount, mergeString);
                            }

                            data.DB_B18_POUnitPrice = StringPK(b18_V_Item.B18_PO_Unit_Price.ToString(), dbB18?.POUnitPrice.ToString()!, ref errorCount, mergeString);
                            data.DB_B18_TransactionDate = string.IsNullOrEmpty(dbB18?.TransactionDate.ToString("yyyy/MM/dd")) ? string.Concat("No Data", ExcelCellStyle.Waring, mergeString) : string.Concat(dbB18?.TransactionDate.ToString("yyyy/MM/dd"), mergeString);
                            data.V_B18_Project = b18_V_Item.B18_Project;
                            data.V_B18_Note = b18_V_Item.B18_Note;

                            //隱藏欄位
                            data.HiddenSONo = s18_V_Item.S18_SO_No;
                            data.HiddenSOLineNo = s18_V_Item.S18_SO_Line;
                            data.HiddenTransactionId = dbB18?.TransactionID;

                            listPkResult.Add(data);
                        }
                    }                    
                    else
                    {
                        //隱藏欄位
                        data.HiddenSONo = s18_V_Item.S18_SO_No;
                        data.HiddenSOLineNo = s18_V_Item.S18_SO_Line;

                        listPkResult.Add(data);
                    }

                    if (
                        (dbS18 != null && dbS18.StatusID == (int?)StatusId.ManualClose) ||
                        (b18_V_Items.Count > 0 && dbB18QtySum == s18_V_Item.S18_Qty)
                    )
                    {//DB大18 Qty加總等於廠商提供小18 Qty => 增加結案列
                        listPkResult.Add(new PK_RESULT_REPORT { PK_RESULT = string.Concat(dbB18QtySum.ToString(), ExcelCellStyle.Waring) });

                        var status = poStatus.Where(c => c.ID == (int)StatusId.AutoClose).FirstOrDefault()?.Description;//自動結案;

                        if (errorCount > 0)
                        {//自動結案，但比對資料異常
                            status = $"{status}，但比對資料異常";
                        }

                        if (dbS18 != null && dbS18.StatusID == (int)StatusId.ManualClose)
                        {//手動結案
                            status = poStatus.Where(c => c.ID == (int)StatusId.ManualClose).FirstOrDefault()?.Description;
                        }

                        listPkResult.Add(new PK_RESULT_REPORT { PK_RESULT = string.Concat(status, ExcelCellStyle.Waring) });
                        errorCount = 0;
                    }
                    else
                    {
                        errorCount++;
                    }

                    listPkResult.Add(new PK_RESULT_REPORT { });//空白列

                    if (errorCount <= 0)
                    {
                        listPkResult_Closed.AddRange(listPkResult);
                    }
                    else
                    {//比對異常須在第一筆大18的資料列加上是否手動結案下拉選單
                        var idx = 0;
                        if (listPkResult_NoneClose.Count > 0)
                        {
                            idx = listPkResult_NoneClose.Count;
                        }
                        listPkResult_NoneClose.AddRange(listPkResult);

                        var s18DbData = _context.XxPor0001_Resell.Where(c => c.OrderNo == listPkResult_NoneClose[idx].V_S18_SO_No && c.SOLine == listPkResult_NoneClose[idx].V_S18_SO_Line).FirstOrDefault();
                        listPkResult_NoneClose[idx].MANUAL_CLOSED_NOTE = s18DbData?.ProcureRemark;
                        listPkResult_NoneClose[idx].PK_RESULT = s18DbData != null ? string.Concat("否", ExcelCellStyle.DropDownList) : "資料庫查無小18資料，無法手動結案";

                        if (listPkResult_NoneClose[idx].HiddenS18StatusId != (int)StatusId.NoneClose)
                        {//已經於其他廠商提供報表轉結案
                            listPkResult_NoneClose[idx].PK_RESULT = $"此SO NO + SO line已於先前{poStatus.Where(c => c.ID == listPkResult_NoneClose[idx].HiddenS18StatusId).FirstOrDefault()?.Description}，但此次比對資料異常";
                        }
                    }
                }

                //合併NOTE欄位處理
                AddMergeString(ref listPkResult_Closed);
                AddMergeString(ref listPkResult_NoneClose);
                #endregion
            }

            res.Add(listPkResult_Closed);
            res.Add(listPkResult_NoneClose);

            return res;
        }

        /// <summary>
        /// 手動結案
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async Task<int> ManualClose(DataTable dt, string modifyUser)
        {
            var errorData = new PkResultWithError();
            var currentSoNo = "";
            var currentSoLineNo = "";
            var currentIsManualClose = "";
            var currentManualCloseNote = "";

            var firstData = true;
            var listTransactionId = new List<string>();
            var res = 0;

            //for last update
            var row = dt.NewRow();
            row[errorData.HiddenS18SoNo] = "NewSoNo";
            row[errorData.HiddenS18SoLineNo] = "NewSoLineNo";
            dt.Rows.Add(row);

            using (var ts = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in dt.AsEnumerable())
                    {
                        if (
                                string.IsNullOrEmpty(item[errorData.HiddenS18SoNo].ToString()) &&
                                string.IsNullOrEmpty(item[errorData.HiddenS18SoLineNo].ToString()) &&
                                string.IsNullOrEmpty(item[errorData.HiddenTransactionId].ToString())
                            )
                        {
                            continue;
                        }

                        if (currentSoNo != item[errorData.HiddenS18SoNo].ToString() || currentSoLineNo != item[errorData.HiddenS18SoLineNo].ToString())
                        {
                            if (firstData == false)
                            {
                                var needUpdate = false;
                                var s18DbData = _context.XxPor0001_Resell.Where(c => c.OrderNo == currentSoNo && c.SOLine == currentSoLineNo).FirstOrDefault();
                                int? id = null;
                                int? statusId = null;

                                if (s18DbData != null)
                                {
                                    if (s18DbData.StatusID == (int)StatusId.NoneClose && currentIsManualClose == "是")
                                    {//未結案 => 人工結案
                                        needUpdate = true;
                                        statusId = (int)StatusId.ManualClose;//人工結案
                                        id = s18DbData.ID;
                                    }
                                    //else if (s18DbData.StatusID == (int)StatusId.AutoClose && currentIsManualClose == "否")
                                    //{//人工結案 => 未結案
                                    //    needUpdate = true;
                                    //    statusId = (int)StatusId.NoneClose;//尚未結案
                                    //    id = null;
                                    //}

                                    if (needUpdate == true)
                                    {
                                        s18DbData.StatusID = statusId;
                                        s18DbData.ProcureRemark = currentManualCloseNote;//null; //人工結案備註
                                        s18DbData.UpdateTime = DateTime.Now;
                                        s18DbData.Updater = modifyUser;
                                        res += _context.SaveChanges();

                                        var b18DbData = _context.Xx_Po_Receipt.Where(c => listTransactionId.Contains(c.TransactionID));
                                        foreach (var itemB18 in b18DbData)
                                        {
                                            itemB18.XxPor0001_Resell_ID = id;
                                        }
                                        res += _context.SaveChanges();
                                    }

                                    listTransactionId = new List<string>();
                                }
                            }

                            currentSoNo = item[errorData.HiddenS18SoNo].ToString();
                            currentSoLineNo = item[errorData.HiddenS18SoLineNo].ToString();
                            currentIsManualClose = item[errorData.IsManualClose].ToString();
                            currentManualCloseNote = item[errorData.ManualCloseNote].ToString();

                            firstData = false;
                        }


                        listTransactionId.Add(item[errorData.HiddenTransactionId].ToString());
                    }

                    ts.Commit();
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                }
            }

            return res;
        }

        /// <summary>
        /// 更新小18資料狀態 & 廠商備註欄位
        /// </summary>
        /// <param name="closeData">結案資料</param>
        /// <param name="noneCloseData">未結案資料</param>
        /// <returns></returns>

        public async Task<int> UpdateS18(List<PK_RESULT_REPORT> closeData, List<PK_RESULT_REPORT> noneCloseData)
        {
            var res = 0;
            var filterCloseData = closeData.Where(c => c.HiddenS18StatusId == (int)StatusId.NoneClose).ToList();
            var filterNoneCloseData = noneCloseData.Where(c => c.HiddenS18StatusId == (int)StatusId.NoneClose).ToList();

            using (var ts = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in filterCloseData)
                    {
                        var s18DbData = _context.XxPor0001_Resell.Where(c => c.OrderNo == item.V_S18_SO_No && c.SOLine == item.V_S18_SO_Line).FirstOrDefault();
                        if (s18DbData != null)
                        {
                            s18DbData.StatusID = (int?)StatusId.AutoClose;//自動結案
                            if (item.V_B18_Note?.IndexOf(',') > 0)
                            {//刪除合併字串，更新廠商備註欄位
                                s18DbData.VendorRemark = item.V_B18_Note.Split(',')[0];
                            }
                            s18DbData.Updater = UserName;
                            s18DbData.UpdateTime = DateTime.Now;

                            var b18Data = filterCloseData.Where(c => c.HiddenSONo == item.V_S18_SO_No && c.HiddenSOLineNo == item.V_S18_SO_Line).Select(c => c.HiddenTransactionId).ToArray();
                            var b18DbData = _context.Xx_Po_Receipt.Where(c => b18Data.Contains(c.TransactionID));
                            foreach (var itemDbB18 in b18DbData)
                            {
                                itemDbB18.XxPor0001_Resell_ID = s18DbData.ID;
                                itemDbB18.Updater = UserName;
                                itemDbB18.UpdateTime = DateTime.Now;
                            }
                        }
                    }

                    foreach (var item in filterNoneCloseData)
                    {
                        var s18DbData = _context.XxPor0001_Resell.Where(c => c.OrderNo == item.V_S18_SO_No && c.SOLine == item.V_S18_SO_Line).FirstOrDefault();
                        if (s18DbData != null)
                        {
                            s18DbData.StatusID = (int?)StatusId.NoneClose;//尚未結案
                            if (item.V_B18_Note?.IndexOf(',') > 0)
                            {//刪除合併字串，更新廠商備註欄位
                                s18DbData.VendorRemark = item.V_B18_Note.Split(',')[0];
                            }
                            s18DbData.Updater = UserName;
                            s18DbData.UpdateTime = DateTime.Now;
                        }
                    }

                    res = _context.SaveChanges();
                    ts.Commit();
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                }
            }

            return res;
        }

        /// <summary>
        /// 資料PK(廠商提供 vs DB資料)
        /// </summary>
        /// <param name="strOriginal"></param>
        /// <param name="strPK"></param>
        /// <param name="errorCount"></param>
        /// <returns></returns>
        private string StringPK(string strOriginal, string strPK, ref int errorCount, string mergeString = "")
        {
            if (strOriginal != strPK)
            {                
                errorCount++;
                strPK = string.IsNullOrEmpty(strPK) ? "No Data" : strPK;
                return string.Concat(strPK, ExcelCellStyle.Waring, mergeString);
            }

            return string.Concat(strOriginal, mergeString);
        }

        /// <summary>
        /// 幫NOTE欄位加上欄位合併字串
        /// </summary>
        /// <param name="data"></param>
        private void AddMergeString(ref List<PK_RESULT_REPORT> data)
        {
            //Group by 找出小18底下有多少大18
            var b18CNT = data
                .GroupBy(c => new { c.V_B18_SO_No, c.HiddenSOLineNo })
                .Select(c => new
                {
                    SONo = c.Key.V_B18_SO_No,
                    SOLineNo = c.Key.HiddenSOLineNo,
                    Note = string.Join($"\r\n=====\r\n", c.Where(s => !string.IsNullOrEmpty(s.V_B18_Note)).Select(s => s.V_B18_Note)),
                    Count = c.Count()
                });

            //幫大18的NOTE欄位加上欄位合併標籤(@Merge_{合併起始列},{合併結束列})
            var replaceNoteContent =
                data
                .Select((c, serialNo) => new
                {
                    SN = serialNo + 1,
                    Data = c
                })
                //.Where(c => !string.IsNullOrEmpty(c.Data.V_B18_Note))
                .Join(
                b18CNT,
                a => new { SONo = a.Data.V_S18_SO_No, SOLineNo = a.Data.HiddenSOLineNo },
                b => new { b.SONo, b.SOLineNo },
                (a, b) => new { 
                    DATA = b, 
                    NEW_NOTE = string.Concat(b.Note, $"{ExcelCellStyle.MergeNote}▲{a.SN}▼{(a.SN + b.Count) - 1}") 
                });


            //把原本的NOTE欄位替換成有合併字串的NOTE欄位，並把NEW_NOTE放到第一筆資料供合併
            foreach (var item in replaceNoteContent)
            {
                var b18 = data.Where(c =>
                    !string.IsNullOrEmpty(c.V_S18_SO_No) &&
                    c.V_S18_SO_No == item.DATA.SONo &&
                    c.V_S18_SO_Line == item.DATA.SOLineNo
                ).FirstOrDefault();

                if (b18 != null)
                {
                    b18.V_B18_Note = item.NEW_NOTE;
                }
            }
        }

        /// <summary>
        /// 拿掉隱藏特殊字元，非數字字元就取代為空白
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        private string CleanOrderNo(string orderNo)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(orderNo, "");
        }
    }
}
