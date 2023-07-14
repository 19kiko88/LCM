using LCM.Repositories;
using Moq;
using LCM.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using LCM.Services.Models.DataTablelHederMapping;
using EFCore.BulkExtensions;
using LCM.Services.Implements;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LCM.Tests.DataService
{
    [TestClass()]
    public class DataServiceTests
    {
        private LCM.Services.Interfaces.IDataService _dataService;
        private Mock<CAEDB01Context> _mockContext = new Mock<CAEDB01Context>();
        private DataTable DtS18 = new DataTable();
        private DataTable DtB18 = new DataTable();
        private DataTable DtVendor = new DataTable();

        public DataServiceTests() 
        {
            #region 製作小18 Context假資料            
            var FakeDbData_S18 = new List<XxPor0001_Resell> {
                new XxPor0001_Resell{ID = 1, PENo = "PE-2100419", OrderNo = "111322041000318", SOLine = "1.1", PartNo = "13PD02V0AM0411", Quantity = 10, NetPrice = 26.9M},
                new XxPor0001_Resell{ID = 2, PENo = "PE-2200005", OrderNo = "111322041000319", SOLine = "1.1", PartNo = "13PD02V0AM0411", Quantity = 1000, NetPrice = 9999},
                //new XxPor0001_Resell{ID = 3, PENo = "PE-2000338", OrderNo = "111322041000320", SOLine = "1.1", PartNo = "13PD02V0AM0411", Quantity = 1, NetPrice = 9999},
            }.AsQueryable();

            //Mock小18 DB假資料
            var mockS18 = new Mock<DbSet<XxPor0001_Resell>>();
            mockS18.As<IQueryable<XxPor0001_Resell>>().Setup(m => m.Provider).Returns(FakeDbData_S18.Provider);
            mockS18.As<IQueryable<XxPor0001_Resell>>().Setup(m => m.Expression).Returns(FakeDbData_S18.Expression);
            mockS18.As<IQueryable<XxPor0001_Resell>>().Setup(m => m.ElementType).Returns(FakeDbData_S18.ElementType);
            mockS18.As<IQueryable<XxPor0001_Resell>>().Setup(m => m.GetEnumerator()).Returns(FakeDbData_S18.GetEnumerator());

            //setup _mockContext
            _mockContext.Setup(c => c.XxPor0001_Resell).Returns(mockS18.Object);
            #endregion

            #region 製作大18 Context假資料
            var FakeDbData_B18 = new List<Xx_Po_Receipt> {
                new Xx_Po_Receipt{TransactionID = "46830290", PONo = "112222000063953", POLineNo = 1, PartNo = "13PD02V0AM0411", Quantity=10, POUnitPrice = 99.9M, TransactionDate = DateOnly.FromDateTime(Convert.ToDateTime("2022/09/19"))},
                new Xx_Po_Receipt{TransactionID = "46830291", PONo = "111222000120735", POLineNo = 2, PartNo = "QQQQ1", Quantity=999, POUnitPrice = 99.9M, TransactionDate = DateOnly.FromDateTime(DateTime.Now)},
                new Xx_Po_Receipt{TransactionID = "46830292", PONo = "111222000120735", POLineNo = 4, PartNo = "QQQQ2", Quantity=999, POUnitPrice = 99.9M, TransactionDate = DateOnly.FromDateTime(DateTime.Now)},
                new Xx_Po_Receipt{TransactionID = "46830293", PONo = "111222000120735", POLineNo = 1, PartNo = "QQQQ3", Quantity=999, POUnitPrice = 99.9M, TransactionDate = DateOnly.FromDateTime(DateTime.Now)}

            }.AsQueryable();

            //Mock大18 DB假資料
            var mockB18 = new Mock<DbSet<Xx_Po_Receipt>>();
            mockB18.As<IQueryable<Xx_Po_Receipt>>().Setup(m => m.Provider).Returns(FakeDbData_B18.Provider);
            mockB18.As<IQueryable<Xx_Po_Receipt>>().Setup(m => m.Expression).Returns(FakeDbData_B18.Expression);
            mockB18.As<IQueryable<Xx_Po_Receipt>>().Setup(m => m.ElementType).Returns(FakeDbData_B18.ElementType);
            mockB18.As<IQueryable<Xx_Po_Receipt>>().Setup(m => m.GetEnumerator()).Returns(FakeDbData_B18.GetEnumerator());

            //setup _mockContext
            _mockContext.Setup(c => c.Xx_Po_Receipt).Returns(mockB18.Object);
            #endregion

            #region 設置小18 DataTable Header
            var S18Header = new XX_PORT0001_RESELL();
            DtS18.Columns.Add(S18Header.PE_No);
            DtS18.Columns.Add(S18Header.Order_Number);
            DtS18.Columns.Add(S18Header.SO_Line);
            DtS18.Columns.Add(S18Header.PN);
            DtS18.Columns.Add(S18Header.Quantity);
            DtS18.Columns.Add(S18Header.Net_Price);
            DtS18.Columns.Add(S18Header.Shipment_Date);
            DtS18.Columns.Add(S18Header.Date);
            #endregion

            #region 設置小18 DataTable Header
            var B18Header = new XX_PO_RECEIPT();
            DtB18.Columns.Add(B18Header.Transaction_Id);
            DtB18.Columns.Add(B18Header.Po_Number);
            DtB18.Columns.Add(B18Header.Po_Line_Num);
            DtB18.Columns.Add(B18Header.Po_Unit_Price);
            DtB18.Columns.Add(B18Header.Item);
            DtB18.Columns.Add(B18Header.Transaction_Date);
            DtB18.Columns.Add(B18Header.Last_Update_Date);
            DtB18.Columns.Add(B18Header.Quantity);
            #endregion

            #region 設置小18 DataTable Header
            var vendorReportHeader = new VENDOR_REPORT();
            DtVendor.Columns.Add(vendorReportHeader.S18_Promised_Date);
            DtVendor.Columns.Add(vendorReportHeader.S18_PE_No);
            DtVendor.Columns.Add(vendorReportHeader.S18_SO_No);
            DtVendor.Columns.Add(vendorReportHeader.S18_SO_Line);
            DtVendor.Columns.Add(vendorReportHeader.S18_PN);
            DtVendor.Columns.Add(vendorReportHeader.S18_Qty);
            DtVendor.Columns.Add(vendorReportHeader.S18_Unit_Price);
            DtVendor.Columns.Add(vendorReportHeader.B18_SO_No);
            DtVendor.Columns.Add(vendorReportHeader.B18_PO_No);
            DtVendor.Columns.Add(vendorReportHeader.B18_PO_Line_No);
            DtVendor.Columns.Add(vendorReportHeader.B18_PN);
            DtVendor.Columns.Add(vendorReportHeader.B18_Qty);
            DtVendor.Columns.Add(vendorReportHeader.B18_PO_Shipment_Date);
            DtVendor.Columns.Add(vendorReportHeader.B18_PO_Unit_Price);
            DtVendor.Columns.Add(vendorReportHeader.B18_Project);
            DtVendor.Columns.Add(vendorReportHeader.B18_Note);
            #endregion
        }

        /// <summary>
        /// 小18匯入
        /// 
        /// 驗證項目：
        /// 1.是否排除DB已經存在之重複資料(OrderNo + SOLine)
        /// </summary>
        [TestMethod()]
        public void InsertS18_DataRepeatCheck_Ok()
        {
            #region Arrange
            var UpdateUser = "Homer_Chen";
            DtS18.Rows.Add(new object[] { "PE-2100419", "111322041000318", "1.1", "13PD02V0AM0411", 1, 9999, DateTime.Now, DateTime.Now });//重複資料
            //DtS18.Rows.Add(new object[] { "PE-2100419", "111322041000319", "1.1", "13PD02V0AM0411", 1, 9999, DateTime.Now, DateTime.Now });//重複資料
            //DtS18.Rows.Add(new object[] { "PE-2100419", "111322041000320", "1.1", "13PD02V0AM0411", 1, 9999, DateTime.Now, DateTime.Now});
            //DtS18.Rows.Add(new object[] { "PE-2100419", "111322041000999", "9.9", "13PD02V0AM0411", 1, 9999, DateTime.Now, DateTime.Now });
            #endregion

            #region Act
            //Mock BulkInsert
            _mockContext.Setup(c => c.BulkInsert(It.IsAny<List<XxPor0001_Resell>>(), null, null, null));
            //注入Service
            _dataService = new LCM.Services.Implements.DataService(_mockContext.Object);

            var Res = _dataService.InsertS18(DtS18, UpdateUser).Result;
            #endregion

            #region Assert
            try
            {
                Assert.AreEqual(Res, 0, "排除小18重複資料異常");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// 大18匯入
        /// 
        /// 驗證項目：
        /// 1.是否排除DB已經存在之重複資料(TransactionID)
        /// </summary>
        [TestMethod()]
        public void InsertB18_DataRepeatCheck_Ok()
        {
            #region Assert
            var UpdateUser = "Homer_Chen";
            DtB18.Rows.Add(new object[] { "46830290", "999999999999999", "1", 9999, "08001-18221100", DateTime.Now, DateTime.Now, 999 });//重複資料
            //DtB18.Rows.Add(new object[] { "46830291", "999999999999999", "1", 9999, "08001-18221100", DateTime.Now, DateTime.Now, 999 });
            //DtB18.Rows.Add(new object[] { "46830292", "999999999999999", "1", 9999, "08001-18221100", DateTime.Now, DateTime.Now, 999 });
            //DtB18.Rows.Add(new object[] { "46830293", "999999999999999", "1", 9999, "08001-18221100", DateTime.Now, DateTime.Now, 999 });
            //DtB18.Rows.Add(new object[] { "46830299", "999999999999999", "1", 9999, "08001-18221100", DateTime.Now, DateTime.Now, 999 });
            #endregion

            #region Act
            //Mock BulkInsert
            _mockContext.Setup(c => c.BulkInsert(It.IsAny<List<Xx_Po_Receipt>>(), null, null, null));//Mock BulkInsert
            //注入Service
            _dataService = new LCM.Services.Implements.DataService(_mockContext.Object);

            var Res = _dataService.InsertB18(DtB18, UpdateUser).Result;
            #endregion

            #region Assert
            try
            {
                Assert.AreEqual(Res, 0, "排除大18重複資料異常");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// 取得PK報表EXCEL內容，並轉換成PK_RESULT_REPORT List
        /// 
        /// 驗證項目：
        /// 1.驗證是產出兩筆List分頁資料(結案 & 未結案)
        /// 2.驗證結案分頁小18 QTY加總 = 大18 QTY加總
        /// 3.驗證結案分頁PK結果只會有[結案]或[結案，但比對資料異常]資料
        /// 4.驗證未結案分頁PK結果不會有[結案]資料
        /// </summary>
        [TestMethod()]
        public void GetPkBs18Content_DataContentToList_Ok()
        {
            #region Arrange
            DtVendor.Rows.Add(new object[] { "2022/5/11", "PE-2100419", "111322041000318", "1.1", "13PD02V0AM0411", "10", "26.9", "111322041000318", "112222000063953", "1", "13PD02V0AM0411", "10", "2022/5/28", "99.9", "CR1100AUO", "" });
            DtVendor.Rows.Add(new object[] { "2022/5/11", "PE-2100419", "111322041000319", "1.1", "13PD02V0AM0411", "500", "9999", "111322041000319", "111222000120735", "2", "13PD02V0AM0411", "4000", "2022/5/28", "99.9", "CR1100AUO", "持續交料" });
            #endregion

            try
            {
                #region Act
                _dataService = new LCM.Services.Implements.DataService(_mockContext.Object);
                var Res = _dataService.GetPkBs18Content(DtVendor).Result;
                #endregion

                var ClosedString = new string[] { "結案", "結案，但比對資料異常" };
                var SumS18Qty = 0;
                Res[0].ForEach(c => { Int32.TryParse(c.V_S18_Qty, out var qty); SumS18Qty += qty; });
                var SumB18Qty = 0;
                Res[0].ForEach(c => { Int32.TryParse(c.DB_B18_Quantity, out var qty); SumB18Qty += qty; });

                #region Assert
                Assert.AreEqual(Res.Count, 2, "分頁資料數量異常");//驗證是產出兩筆List分頁資料(結案 & 未結案)
                Assert.AreEqual(SumS18Qty, SumB18Qty, "結案Qty加總數異常");//驗證結案分頁小18 QTY加總 = 大18 QTY加總
                Assert.AreEqual(Res[0].Count > 0 && Res[0].Where(c => ClosedString.Contains(c.DB_B18_Quantity?.Split("@")?[0])).Any(), true, "結案List查無結案資料");//結案分頁PK結果只會為[結案]或[結案，但比對資料異常]
                Assert.AreEqual(Res[1].Count > 0 && Res[1].Where(c => ClosedString.Contains(c.DB_B18_Quantity?.Split("@")?[0])).Any(), false, "未結案List出現結案資料");//未結案分頁PK結果不會有[結案]相關
                #endregion
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}