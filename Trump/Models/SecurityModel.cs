using System.Collections.Generic;

namespace Trump.Models
{
    public class SecurityModel
    {
        public tblAppoinment A { get; set; }
        public tblVisitor V { get; set; }
        public tblCompany C { get; set; }
        public List<tblMaterial> M { get; set; }
    }

    public class SecurityMaterial
    {   
        public List<tblIN_Out> RPG { get; set; }

        public List<tblIN_Out> RPG_Third { get; set; }

        public List<tblIN_Out> NRGP { get; set; }
        public List<tblIN_Out> NRPG_Third { get; set; }
    }

    public class MaterialView
    {
        public List<Item_Material> Item { get; set; }
        public tblIN_Out RGP { get; set; }
        public tblIN_Out NRGP { get; set; }
        public string Type { get; set; }
        public Master_Requestor requestor { get; set; }
    }

    public class APP_Material
    {
        public List<tblIN_Out> tblIN_Outs { get; set; }
        public List<tblAppoinment> appoinments { get; set; }
    }

    public class VisitorViewModel
    {
        public tblVisitor tbv { get; set; }
        public tblCompany tbc { get; set; }
    }
}