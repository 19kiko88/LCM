using LCM.Repositories;
using Moq;
using LCM.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using LCM.Services.Models.DataTablelHederMapping;
using LCM.Services.Models;

namespace LCM.Tests.DataService
{
    [TestClass()]
    public class DataServiceTests
    {
        private Services.Interfaces.IDataService _dataService;
        private Mock<CAEDB01Context> _mockContext = new Mock<CAEDB01Context>();
        private DataTable DtS18 = new DataTable();
        private DataTable DtB18 = new DataTable();
        private DataTable DtVendor = new DataTable();
        private DateTime shipmentDate = new DateTime(2023, 7, 1);
        private DateOnly limitTransactionDate = new DateOnly(2023, 7, 1).AddDays(181);//出貨日隔日+181天
        private DateOnly tsDate = new DateOnly(2023, 8, 1);


        public DataServiceTests()
        {
            #region 製作小18 Context假資料            
            var FakeDbData_S18 = new List<XxPor0001_Resell> {
                new XxPor0001_Resell{ID = 1, PENo = "PE-2300667", OrderNo = "111323051013639", SOLine = "2.1", PartNo = "18010-11622200", Quantity = 2500, NetPrice = 23M},
                new XxPor0001_Resell{ID = 1, PENo = "PE-2300667", OrderNo = "111323051013639", SOLine = "2.1", PartNo = "18010-11622200", Quantity = 860, NetPrice = 23M},

                new XxPor0001_Resell{ID = 2, PENo = "PE-2200433", OrderNo = "111322041014381", SOLine = "5.1", PartNo = "18200-16000200", Quantity = 1962, NetPrice = 193M},

                new XxPor0001_Resell{ID = 2, PENo = "PE-3345678", OrderNo = "111322043345678", SOLine = "9.9", PartNo = "18200-99999999", Quantity = 9999, NetPrice = 999M, StatusID = 555},
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
                new Xx_Po_Receipt{TransactionID = "53432220", PONo = "111223000110308", POLineNo = 1, PartNo = "18100-11640500", Quantity=1500, POUnitPrice = 37.27M, TransactionDate = tsDate},
                new Xx_Po_Receipt{TransactionID = "53432222", PONo = "111223000110308", POLineNo = 2, PartNo = "18100-11640500", Quantity=667, POUnitPrice = 37.27M, TransactionDate = tsDate},
                new Xx_Po_Receipt{TransactionID = "53535206", PONo = "111223000110308", POLineNo = 3, PartNo = "18100-11640500", Quantity=133, POUnitPrice = 37.27M, TransactionDate = tsDate},
                new Xx_Po_Receipt{TransactionID = "53535206", PONo = "111223000110309", POLineNo = 1, PartNo = "18100-11640500", Quantity=200, POUnitPrice = 37.27M, TransactionDate = tsDate},
                new Xx_Po_Receipt{TransactionID = "53535206", PONo = "111223000110309", POLineNo = 2, PartNo = "18100-11640500", Quantity=860, POUnitPrice = 37.27M, TransactionDate = tsDate},

                new Xx_Po_Receipt{TransactionID = "47400902", PONo = "111222000122645", POLineNo = 2, PartNo = "18210-16000000", Quantity=130, POUnitPrice = 235.66M, TransactionDate = limitTransactionDate},                
                new Xx_Po_Receipt{TransactionID = "47400904", PONo = "111222000122645", POLineNo = 1, PartNo = "18210-16000000", Quantity=932, POUnitPrice = 235.55M, TransactionDate = tsDate},
                new Xx_Po_Receipt{TransactionID = "47400904999", PONo = "111222000122645", POLineNo = 1, PartNo = "18210-16000000", Quantity=932, POUnitPrice = 235.55M, TransactionDate = tsDate.AddDays(-15)},
                new Xx_Po_Receipt{TransactionID = "99999999", PONo = "111222000122645", POLineNo = 5, PartNo = "18210-16000000", Quantity=367, POUnitPrice = 235.55M, TransactionDate = tsDate},


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

            #region 製作PoStatusDesc假資料            
            var FakeDbData_PoStatusDesc = new List<PoStatusDesc> {
                new PoStatusDesc{ID = 1, Description = "未處理"},
                new PoStatusDesc{ID = 555, Description = "人工結案"},
                new PoStatusDesc{ID = 666, Description = "自動結案"}
            }.AsQueryable();

            //Mock PoStatusDesc DB假資料
            var mockPoStatusDesc = new Mock<DbSet<PoStatusDesc>>();
            mockPoStatusDesc.As<IQueryable<PoStatusDesc>>().Setup(m => m.Provider).Returns(FakeDbData_PoStatusDesc.Provider);
            mockPoStatusDesc.As<IQueryable<PoStatusDesc>>().Setup(m => m.Expression).Returns(FakeDbData_PoStatusDesc.Expression);
            mockPoStatusDesc.As<IQueryable<PoStatusDesc>>().Setup(m => m.ElementType).Returns(FakeDbData_PoStatusDesc.ElementType);
            mockPoStatusDesc.As<IQueryable<PoStatusDesc>>().Setup(m => m.GetEnumerator()).Returns(FakeDbData_PoStatusDesc.GetEnumerator());

            //setup _mockContext
            _mockContext.Setup(c => c.PoStatusDesc).Returns(mockPoStatusDesc.Object);
            #endregion

            #region 設置小18 DataTable Header
            var S18Header = new XxPort0001Resell();
            DtS18.Columns.Add(S18Header.PENo);
            DtS18.Columns.Add(S18Header.OrderNumber);
            DtS18.Columns.Add(S18Header.SOLine);
            DtS18.Columns.Add(S18Header.PN);
            DtS18.Columns.Add(S18Header.Quantity);
            DtS18.Columns.Add(S18Header.NetPrice);
            DtS18.Columns.Add(S18Header.ShipmentDate);
            DtS18.Columns.Add(S18Header.Date);
            #endregion

            #region 設置大18 DataTable Header
            var B18Header = new XxPoReceipt();
            DtB18.Columns.Add(B18Header.TransactionId);
            DtB18.Columns.Add(B18Header.PoNumber);
            DtB18.Columns.Add(B18Header.PoLineNum);
            DtB18.Columns.Add(B18Header.PoUnitPrice);
            DtB18.Columns.Add(B18Header.PartNo);
            DtB18.Columns.Add(B18Header.TransactionDate);
            DtB18.Columns.Add(B18Header.SystemUpdateDate);
            DtB18.Columns.Add(B18Header.Quantity);
            #endregion

            #region 設置廠商提供報表 DataTable Header
            var vendorReportHeader = new VendorReport();
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
        public void S18InsertDuplicateCheck()
        {
            #region Arrange
            var UpdateUser = "Homer_Chen";
            DtS18.Rows.Add(new object[] { "PE-2300667", "111323051013639", "2.1", "18010-11622200", 2500, 23, DateTime.Now, DateTime.Now });//重複資料
            #endregion

            #region Act
            //Mock BulkInsert
            _mockContext.Setup(c => c.BulkInsert(It.IsAny<List<XxPor0001_Resell>>(), null, null, null));
            //注入Service
            _dataService = new Services.Implements.DataService(_mockContext.Object);
            var Res = _dataService.InsertS18(DtS18, UpdateUser).Result;
            #endregion

            #region Assert
            try
            {
                Assert.AreEqual(Res.UploadSuccessCount, 0, "排除小18重複資料異常");
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
        public void B18InsertDuplicateCheck()
        {
            #region Assert
            var UpdateUser = "Homer_Chen";
            DtB18.Rows.Add(new object[] { "53432220", "999999999999999", "1", 9999, "08001-18221100", DateTime.Now, DateTime.Now, 999 });//重複資料
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
                Assert.AreEqual(Res.UploadSuccessCount, 0, "排除大18重複資料異常");
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
        public void PkResultContentCheck()
        {
            #region Arrange
            DtVendor.Rows.Add(new object[] { "2023/5/23", "PE-2300667", "111323051013639", "2.1", "18010-11622200", "2500", "23", "111323051013639", "111223000110308", "1", "18100-11640500", "1500", shipmentDate.ToString("yyyy/MM/dd"), "37.27", "BR1100 INX", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111323051013639", "111223000110308", "2", "18100-11640500", "667", shipmentDate.ToString("yyyy/MM/dd"), "37.27", "BR1100 INX", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111323051013639", "111223000110308", "3", "18100-11640500", "133", shipmentDate.ToString("yyyy/MM/dd"), "37.27", "BR1100 INX", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111323051013639", "111223000110309", "1", "18100-11640500", "200", shipmentDate.ToString("yyyy/MM/dd"), "37.27", "BR1100 INX", "" });
            DtVendor.Rows.Add(new object[] { "2023/5/23", "PE-2300667", "111323051013639", "2.1", "18010-11622200", "860", "23", "111323051013639", "111223000110309", "2", "18100-11640500", "860", shipmentDate.ToString("yyyy/MM/dd"), "37.27", "BR1100 INX", "" });



            DtVendor.Rows.Add(new object[] { "2022/4/18", "PE-2100419", "111322041014381", "5.1", "18200-16000200", "1962", "193", "111322041014381", "111222000122645", "1", " 18210-16000000", "932", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111322041014381", "111222000122645", "2", " 18210-16000000", "130", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111322041014381", "111222000122645", "3", " 18210-16000000", "82", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111322041014381", "111222000122645", "4", " 18210-16000000", "176", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111322041014381", "111222000122645", "5", " 18210-16000000", "30", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111322041014381", "111222000122645", "5", " 18210-16000000", "337", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111322041014381", "111222000122645", "6", " 18210-16000000", "26", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111322041014381", "111222000122645", "7", " 18210-16000000", "166", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });
            DtVendor.Rows.Add(new object[] { "", "", "", "", "", "", "", "111322041014381", "111222000122645", "8", " 18210-16000000", "70", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });

            DtVendor.Rows.Add(new object[] { "2022/4/18", "PE-3345678", "111322043345678", "9.9", "18200-99999999", "9999", "199", "111322043345678", "111222000122645", "1", " 18210-16000000", "932", shipmentDate.ToString("yyyy/MM/dd"), "235.66", "UX7602", "" });
            #endregion

            try
            {
                #region Act
                _dataService = new Services.Implements.DataService(_mockContext.Object);
                var Res = _dataService.GetPkBs18Content(DtVendor).Result;
                #endregion


                #region Assert
                var closeData = Res[0];
                var noneCloseData = Res[1];

                var clseS18Group =
                    closeData.GroupBy(g => new { g.V_S18_PE_No, g.V_S18_SO_No, g.V_S18_SO_Line })
                    .Where(c => !string.IsNullOrEmpty(c.Key.V_S18_PE_No) && !string.IsNullOrEmpty(c.Key.V_S18_SO_No) && !string.IsNullOrEmpty(c.Key.V_S18_SO_Line))
                    .AsEnumerable();

                var noneClseS18Group =
                    noneCloseData.GroupBy(g => new { g.V_S18_PE_No, g.V_S18_SO_No, g.V_S18_SO_Line })
                    .Where(c => !string.IsNullOrEmpty(c.Key.V_S18_PE_No) && !string.IsNullOrEmpty(c.Key.V_S18_SO_No) && !string.IsNullOrEmpty(c.Key.V_S18_SO_Line))
                    .AsEnumerable();


                //相同的PE_NO, SO_NO, SO_Line_No的小18資料要合併為一筆資料
                Assert.AreEqual(clseS18Group.Where(c => c.Key.V_S18_PE_No == "PE-2300667" && c.Key.V_S18_SO_No == "111323051013639" && c.Key.V_S18_SO_Line == "2.1").ToList().Count, 1, "相同PE_NO, SO_NO, SO_Line_No的小18資料要合併為一筆資料");

                //結案狀態為555(人工結案)要顯示於結案List，不管小18Qty 是否等於 DB的大18Qty加總
                Assert.AreEqual(closeData.Where(c => c.V_S18_PE_No == "PE-3345678" && c.DB_S18_OrderNo == "111322043345678" && c.DB_S18_SOLine == "9.9").Any(), true);
                Assert.AreEqual(noneCloseData.Where(c => c.HiddenS18StatusId == 555).Any(), false, "手動結案之比對資料要顯示於結案分頁");

                //結案狀態為666(自動結案)的資料廠商提供小18Qty = DB的大18Qty加總
                Assert.AreEqual(closeData.Where(c => c.HiddenS18StatusId == 666).Select(c => Convert.ToInt32(c.V_S18_Qty ?? "0")).Sum(), closeData.Where(c => c.HiddenS18StatusId == 666).Select(c => Convert.ToInt32(c.DB_B18_Quantity ?? "0")).Sum());

                //結案 & 未結案是否分類正確
                Assert.AreEqual(clseS18Group.Where(c => c.Key.V_S18_PE_No == "PE-2300667" && c.Key.V_S18_SO_No == "111323051013639" && c.Key.V_S18_SO_Line == "2.1").Any(), true);
                Assert.AreEqual(noneClseS18Group.Where(c => c.Key.V_S18_PE_No == "PE-2100419" && c.Key.V_S18_SO_No == "111322041014381" && c.Key.V_S18_SO_Line == "5.1").Any(), true);

                //transaction_date大於Shipment Date + 180天的話，Shipment Date要顯示異常
                var listWaringShipmentDate = new List<string>();
                foreach (var item in noneCloseData)
                {
                    DateTime dtTransactionDate = DateTime.MinValue;
                    DateTime dtShipmentDate = DateTime.MinValue;

                    var checkTransactionDate =
                        !string.IsNullOrEmpty(item.DB_B18_TransactionDate) &&
                        DateTime.TryParse(item.DB_B18_TransactionDate.IndexOf('@') > 0 ? item.DB_B18_TransactionDate.Split('@')[0] : item.DB_B18_TransactionDate, out dtTransactionDate) &&
                        dtTransactionDate != DateTime.MinValue;

                    var checkShipmentDate =
                        !string.IsNullOrEmpty(item.V_B18_PO_Shipment_Date) &&
                        DateTime.TryParse(item.V_B18_PO_Shipment_Date.IndexOf('@') > 0 ? item.V_B18_PO_Shipment_Date.Split('@')[0] : item.V_B18_PO_Shipment_Date, out dtShipmentDate) &&
                        dtShipmentDate != DateTime.MinValue;


                    if (checkTransactionDate && checkShipmentDate && dtTransactionDate > Convert.ToDateTime(dtShipmentDate).AddDays(180))
                    {
                        listWaringShipmentDate.Add(item.V_B18_PO_Shipment_Date);
                    }
                }
                Assert.AreEqual(listWaringShipmentDate.Where(c => !c.Contains(ExcelCellStyle.Waring)).Any(), false, "Shipment Date檢核異常");

                //DB的大18資料有重複(PO_NO, PO_Line_No相同)時，取transaction date離出貨日最近的那筆資料。
                Assert.AreEqual(noneCloseData.Where(c => c.HiddenTransactionId == "47400904999").Any(), true, "重複大18資料取得異常");

                #endregion
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}