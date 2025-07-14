using System;

namespace Trump.Models
{
    public class POMailList
    {
        public Nullable<long> ID { get; set; }
        public string Item { get; set; }
        public string Descriptn { get; set; }
        public string UOM { get; set; }
        public int Qty { get; set; }
        public int RecQty { get; set; }
        public string SupplierName { get; set; }

    }

    public class Inward
    {
        public string ChallenNmbr { get; set; }
        public DateTime RaiseDate { get; set; }
        public string Supp_Type { get; set; }
        public string Suppliear { get; set; }
        public string Item { get; set; }
        public string Desc { get; set; }
        public string Status { get; set; }
        public string M_Type { get; set; }
        public int ID { get; set; }
        public bool Delay { get; set; }
    }
}