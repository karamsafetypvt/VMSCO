using System;
using System.Collections.Generic;
using System.Drawing;

namespace TrumpCL
{
    public class DataField
    {
        public int GP_ID { get; set; }
        public string GP { get; set; }
        public string Req { get; set; }
        public string Dept { get; set; }
        public DateTime VisDate { get; set; }
        public string strVisDate { get; set; }
        public string comName { get; set; }
        public string visName { get; set; }
        public string VisTime { get; set; }
        public string VisDuration { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string TimeIN { get; set; }
        public string TimeOUT { get; set; }
        public string V_Type { get; set; }

        public string RejectRemark { get; set; }
        public string Region { get; set; }
        public string ReqName { get; set; }
        public string Vehicleno { get; set; }
    }

    public class DataFieldAdmin
    {
        public int A_ID { get; set; }
        public string GPID { get; set; }
        public string V_Name { get; set; }
        public string CName { get; set; }
        public string V_Phone { get; set; }
        public string NAME { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public Nullable<System.DateTime> V_DateFrom { get; set; }
        public string A_Time { get; set; }
        public string Duration { get; set; }
        public string V_Status { get; set; }
        public string Visitor_Type { get; set; }
        public string Location { get; set; }
        public string Valid_at { get; set; }
        public Nullable<int> regionid { get; set; }
        public string Region { get; set; }
    }
    //public class DataFieldCourier
    //{
    //    public int CouriarId { get; set; }
    //    public string Region { get; set; }
    //    public string NameOfCompany { get; set; }
    //    public string City { get; set; }
    //    public string Documents { get; set; }
    //    public string ReceiveTime { get; set; }
    //    public string DocketNo { get; set; }
    //    public string CouriarVendor { get; set; }
    //    public string Remark { get; set; }
    //    public string CurrentStatus { get; set; }
    //    public Nullable<System.DateTime> TransactionDate { get; set; }
    //    public Nullable<System.DateTime> C_Date { get; set; }
    //    public string EmployeeId { get; set; }
    //    public string EmployeeName { get; set; }
    //    public string DeskRemarks { get; set; }
    //    public Nullable<System.DateTime> DeskTransDate { get; set; }
    //    public string EmployeeRemark { get; set; }
    //    public Nullable<System.DateTime> EmployeeTransDate { get; set; }
    //    public string CouriarType { get; set; }
    //    public string Department { get; set; }
    //    public string Person { get; set; }
    //    public string ContactNo { get; set; }
    //    public string Address { get; set; }
    //}



    public class Company
    {
        public string CName { get; set; }
        public string Addres { get; set; }
        public string Phone { get; set; }
        public string GST { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string PIN { get; set; }
    }

    public class Appointment
    {
        public int A_ID { get; set; }
        public int ComanyID { get; set; }
        public int VisitorID { get; set; }
        public int V_Material { get; set; }
        public int P_Accompanyng { get; set; }
        public DateTime V_DateFrom { get; set; }
        public string A_Time { get; set; }
        public DateTime V_DateTo { get; set; }
        public DateTime RaiseDate { get; set; }
        public DateTime V_InDate { get; set; }
        public DateTime V_OutDate { get; set; }
        public string Requestor { get; set; }
        public string Deptment { get; set; }
        public string Visitor_Type { get; set; }
        public string Duration { get; set; }
        public string V_Location { get; set; }
        public string Location { get; set; }
        public string Remark { get; set; }
        public string V_Allowed { get; set; }
        public string A_Type { get; set; }
        public string V_Status { get; set; }
        public string EntryType { get; set; }
        public string VehicleNo { get; set; }
    }

    public class Visitor
    {
        public int V_ID { get; set; }
        public string V_Name { get; set; }
        public string V_Phone { get; set; }
        public string V_Email { get; set; }
        public string Visitor_ID { get; set; }
        public string V_IDNumber { get; set; }
        public string V_Status { get; set; }
        public Image V_Pic { get; set; }
        public int C_ID { get; set; }
        public string V_Type { get; set; }
    }

    public class CommonViewModel
    {
        public  List<DataField> Appointment { get; set; }
        public List<Material> material { get; set; }
        public List<Couriar> CouriarList { get; set; }
       
    }

    public class Material
    {
        public string GPID { get; set; }
        public string Type { get; set; }
        public string Supplier { get; set; }
        public string Location { get; set; }
        public string OutFrom { get; set; }
        public DateTime RaiseDate { get; set; }
        public string Dept { get; set; }
        public string Req { get; set; }
        public string Status { get; set; }
        public string Delay { get; set; }
    }

    public class MaterialReport
    {
        public int M_ID { get; set; }
        public string ChallanNmbr { get; set; }
        public string Requestor { get; set; }
        public string Dept { get; set; }
        public DateTime RaiseDate { get; set; }
        public string M_Type { get; set; }
        public string S_Type { get; set; }
        public string M_Location { get; set; }
        public string OutFrom { get; set; }
        public string SecApproveTime { get; set; }
        public string HODTime { get; set; }
        public string SupplierName { get; set; }
        public string M_Status { get; set; }
        public string Description { get; set; }
        public string ModeOfTrans { get; set; }
    }
    //------------
    public class Couriar : CommonViewModel
    {
        public int CouriarId { get; set; }
        //public string C_Date { get; set; }
        public Nullable<System.DateTime> C_Date { get; set; }
        public string Region { get; set; }
        public string NameOfCompany { get; set; }
        public string City { get; set; }
        public string Documents { get; set; }
        public string ReceiveTime { get; set; }
        public string DocketNo { get; set; }
        public string CouriarVendor { get; set; }
        public string Remark { get; set; }
        public string CurrentStatus { get; set; }
        //public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Person { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public int EmployeeId { get; set; }
        public string SenderName { get; set; }
        public string EmployeeRemark { get; set; }
        public string DeskRemarks { get; set; }

        public string AdminRegion { get; set; }
        public string PodFile { get; set; }
        public string EmployeeName { get; set; }

    }

    //------------
}
