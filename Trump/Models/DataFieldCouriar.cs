using System;
using System.Collections.Generic;

namespace Trump.Models
{
    public class DataFieldCouriar
    {        
        //public string Approver { get; set; }
       

    }

  
    public class HistoryModel
    {
        public List<tblCouriarHistory> CouriarHistory { get; set; }
        public tblCouriar Couriar { get; set; }
        



    }
    public class DataFieldCourierCompany
    {
        public string CourierCompany { get; set; }

        public Int32 CourierCompanyId { get; set; }
        public string Status { get; set; }
        
    }
    public class DataFieldRegion
    {
        public string Region { get; set; }

        public Int32 RegionId { get; set; }
        

    }
    public class DataFieldWorkingLogin
    {
        public int W_ID { get; set; }
        public string UserID { get; set; }
        public string UPassword { get; set; }
        public string S_Location { get; set; }
        public string WStatus { get; set; }
        public string RoleType { get; set; }
        public string Email { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string Region { get; set; }


    }




}