using System;
using System.Collections.Generic;
using System.Drawing;

namespace TrumpCL
{
    public class DataFieldCourier
    {
        public int CouriarId { get; set; }
        public string strC_Date { get; set; }
        public string Region { get; set; }
        public string NameOfCompany { get; set; }
        public string City { get; set; }
        public string Documents { get; set; }
        public string ReceiveTime { get; set; }
        public string DocketNo { get; set; }
        public string CouriarVendor { get; set; }
        public string Remark { get; set; }
        public string CurrentStatus { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<System.DateTime> C_Date { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DeskRemarks { get; set; }
        public Nullable<System.DateTime> DeskTransDate { get; set; }
        public string EmployeeRemark { get; set; }
        public Nullable<System.DateTime> EmployeeTransDate { get; set; }
        public string CouriarType { get; set; }
        public string Department { get; set; }
        public string Person { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string strTransactionDate { get; set; }
        public string strDeskTransDate { get; set; }
        public string strEmployeeTransDate { get; set; }

       

        public Nullable<System.DateTime> DocketDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
       
        public string FromCity { get; set; }
        public string PacketType { get; set; }
        public string SenderName { get; set; }
        public string SenderContactNo { get; set; }
        public string HazardousItemYN { get; set; }
        public string AdminRegion { get; set; }
        public string PODFile { get; set; }
        

    }


    //------------
}
