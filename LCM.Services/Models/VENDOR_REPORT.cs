namespace LCM.Services.Models
{
    public class VENDOR_REPORT
    {
        /// <summary>
        /// Promised Date
        /// </summary>
        public DateOnly S18_Promised_Date { get; set; }
        /// <summary>
        /// PE No
        /// </summary>
        public string S18_PE_No { get; set; }
        /// <summary>
        /// 小18 SO Number
        /// </summary>
        public string S18_SO_No { get; set; }
        /// <summary>
        /// SO Line
        /// </summary>
        public string S18_SO_Line { get; set; }
        /// <summary>
        /// ASUS 小18 P/N
        /// </summary>
        public string S18_PN { get; set; }
        /// <summary>
        /// Qty
        /// </summary>
        public int S18_Qty { get; set; }
        /// <summary>
        /// Unit Price
        /// </summary>
        public decimal S18_Unit_Price { get; set; }
        /// <summary>
        /// 小18 SO Number(與小18join用)
        /// </summary>
        public string B18_SO_No { get; set; }
        /// <summary>
        /// 小18 SO Line(與小18join用)
        /// </summary>
        public string B18_SO_Line { get; set; }
        /// <summary>
        /// 大18 PO Number
        /// </summary>
        public string B18_PO_No { get; set; }
        /// <summary>
        /// 大18 PO Line number
        /// </summary>
        public int B18_PO_Line_No { get; set; }
        /// <summary>
        /// 大18 P/N
        /// </summary>
        public string B18_PN { get; set; }
        /// <summary>
        /// Qty
        /// </summary>
        public int B18_Qty { get; set; }
        /// <summary>
        /// 大18 PO出貨日
        /// </summary>
        public DateOnly B18_PO_Shipment_Date { get; set; }
        /// <summary>
        /// 大18 PO Unit Price
        /// </summary>
        public decimal B18_PO_Unit_Price { get; set; }
        /// <summary>
        /// Project
        /// </summary>
        public string B18_Project { get; set; }
        /// <summary>
        /// Note
        /// </summary>
        public string B18_Note { get; set; }
    }
}
