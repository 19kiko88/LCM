using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCM.Services.Models.DataTablelHederMapping
{
    public class PkResultWithError
    {
        public string VendorS18PeNo { get { return "PE No."; } }
        public string VendorS18OrderNumber { get { return "Order Number(SO NO)"; } }
        public string VendorS18SOLine { get { return "SO line"; } }
        public string VendorS18PN { get { return "P/N"; } }
        public string VendorS18Quantity { get { return "Quantity"; } }
        public string VendorS18UnitSellingPrice { get { return "Unit Selling Price"; } }
        public string DbS18PeNo { get { return "① PE No."; } }
        public string DbS18OrderNo { get { return "② Order Number(SO NO)"; } }
        public string DbS18SOLine { get { return "SO line_DbS18"; } }
        public string DbS18PN { get { return "③ P/N"; } }
        public string DbS18Quantity { get { return "④ Quantity"; } }
        public string DbS18UnitSellingPrice { get { return "⑤ Unit Selling Price"; } }
        public string DbS18Shipment_Date { get { return "Shipment Date"; } }

        public string VendorB18ShipmentPoDate { get { return "PO 出貨日"; } }
        public string VendorS18OrderNo2 { get { return "小18 SO"; } }
        public string VendorB18PoNo { get { return "po_number"; } }
        public string VendorB18PoLineNo { get { return "po_line_number"; } }
        public string VendorB18PN { get { return "part_number"; } }
        public string VendorB18Quantity { get { return "quantity"; } }
        public string VendorB18PoUnitPrice { get { return "po_unit_price"; } }
        public string DbB18PoNo { get { return "① po_number"; } }
        public string DbB18PoLineNo { get { return "② po_line_number"; } }
        public string DbB18PN { get { return "③ part_number"; } }
        public string DbB18Quantity { get { return "④quantity"; } }
        public string DbB18PoUnitPrice { get { return "⑤ po_unit_price"; } }
        public string DbB18TransactionDate { get { return "transaction_date"; } }
        public string VendorB18Project { get { return "Project"; } }
        public string VendorB18PoNote { get { return "廠商備註"; } }

        public string IsManualClose { get { return "是否人工結案"; } }
        public string ManualCloseNote { get { return "人工結案備註"; } }

        public string HiddenS18SoNo { get { return "HiddenS18SoNo"; } }
        public string HiddenS18SoLineNo { get { return "HiddenS18SoLineNo"; } }
        public string HiddenTransactionId { get { return "HiddenTransactionId"; } }
        public string HiddenS18StatusId { get { return "HiddenS18StatusId"; } }

    }
}
