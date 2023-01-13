using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCM.Services.Models
{
    public class PK_RESULT_REPORT
    {
        /*廠商提供小18資料*/
        public string V_S18_PE_No { get; set; }
        public string V_S18_SO_No { get; set; }
        public string V_S18_SO_Line { get; set; }
        public string V_S18_PN { get; set; }
        public string V_S18_Qty { get; set; }
        public string V_S18_Unit_Price { get; set; }

        /*DB小18資料*/
        public string DB_S18_PENo { get; set; }
        public string DB_S18_OrderNo { get; set; }
        public string DB_S18_SOLine { get; set; }
        public string DB_S18_PartNo { get; set; }
        public string DB_S18_Quantity { get; set; }
        public string DB_S18_NetPrice { get; set; }
        public string DB_S18_ShipmentDate { get; set; }


        /*廠商提供大18資料*/
        public string V_B18_PO_Shipment_Date { get; set; }
        public string V_B18_SO_No { get; set; }
        public string V_B18_PO_No { get; set; }
        public string V_B18_PO_Line_No { get; set; }
        public string V_B18_PN { get; set; }
        public string V_B18_Qty { get; set; }
        public string V_B18_PO_Unit_Price { get; set; }

        /*DB大18資料*/
        public string DB_B18_PONo { get; set; }
        public string DB_B18_POLineNo { get; set; }
        public string DB_B18_PartNo { get; set; }
        public string DB_B18_Quantity { get; set; }
        public string DB_B18_POUnitPrice { get; set; }
        public string DB_B18_TransactionDate { get; set; }


        public string V_B18_Note { get; set; }

        public string HiddenSOLineNo { get; set; }
    }
}
