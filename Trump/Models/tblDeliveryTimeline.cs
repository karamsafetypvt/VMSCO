//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Trump.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblDeliveryTimeline
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string PackageType { get; set; }
        public string Days { get; set; }
        public string Status { get; set; }
    }
}
