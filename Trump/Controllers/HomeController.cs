using ClosedXML.Excel;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trump.Models;
using TrumpCL;
using ZXing;
using ZXing.Common;
using static Trump.FilterConfig;
using System.Net;
//using System.Net.Mail;
using System.Configuration;
using crypto;
using System.Web;
using System.Data.SqlClient;
using Org.BouncyCastle.Crypto.Tls;
using DocumentFormat.OpenXml.Wordprocessing;
//using DocumentFormat.OpenXml.Office2010.Excel;

namespace Trump.Controllers
{
    [ExceptionHandler]
    public class HomeController : Controller
    {
        TrumpEntities db = new TrumpEntities();
        MasterDatabaseEntities login = new MasterDatabaseEntities();
        BusinessLayer layer = new BusinessLayer();
        TrumpService service = new TrumpService();
        //SAVIOREntities hc = new SAVIOREntities();
        public ActionResult Index()
        {
            //string str = "654321";

            //string aa = layer.Encode(str);
            //string bb = aa;


            //string zz = layer.Decode(bb);



            bool allowYN = PageRoletype("Employee");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }

            if (Session["NAME"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                // && m.Regionid == RegionId


                string s = Session["PNICODE"].ToString();
                ViewBag.Today = db.sp_AppToday().Where(m => m.EMPLOYEE_ID == s && m.regionid == RegionId).Count();
                ViewBag.Upcoming = db.sp_AppUpcoming().Where(m => m.EMPLOYEE_ID == s && m.Regionid == RegionId).ToList().Count;
             

                int totalApp = db.tblAppoinments.Where(m => m.Requestor == s && m.RegionId == RegionId).ToList().Count();
                ViewBag.TotalApp = totalApp;
               //  string UserId = Session["UserID"].ToString();
                // ViewBag.CouriarCount = db.tblCouriars.Where(m => m.EmployeeId == s && m.CurrentStatus == "Intrasit" && m.CouriarType == "Inward").Count();//change 29jan
                ViewBag.CouriarCount = db.tblCouriars.Where(m => m.EmployeeId == s && m.CouriarType == "Inward" && m.RegionId == RegionId).Count();
          
                ViewBag.OutwardCouriarCount = db.tblCouriars.Where(m => m.EmployeeId == s && m.CouriarType == "Outward" && m.RegionId == RegionId).Count();

                CommonViewModel model = new CommonViewModel();
                model.Appointment = layer.RecentVisitor;
              
                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult CouriarListCancel()
        {
            if (Session["NAME"] != null)
            {
                Session.LCID = 1033;

                RollPermission();
                Couriar data = new Couriar();
              
                if (Session["RoleTypeBothYN"] != null)
                {
                    if (Session["RoleTypeBothYN"].ToString() == "Yes")
                    {

                        data.CouriarList = layer.CouriarList("Intrasit", "");

                    }


                }


                //data.CouriarList = layer.CouriarList("Open", "");


                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }

        public ActionResult CouriarList()
        {
            if (Session["NAME"] != null)
            {
                Session.LCID = 1033;

                RollPermission();
                Couriar data = new Couriar();
                data.CouriarList = layer.CouriarList("Open", "");
                


                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }
        [HttpPost]
        public ActionResult ChangeCourierStatusDesk(FormCollection fc, int Courier_ID, string ddldept, string ddlEmployee, string ddlCouriarstatus)
        {


            string employeename = fc["apphidEmployee"].ToString();
            string remarks = fc["CouriarRemark"].ToString();
            if (Session["NAME"] != null)
            {
                string name = Session["NAME"].ToString();
                var data = db.tblCouriars.Where(m => m.CouriarId == Courier_ID).FirstOrDefault();
                if (data != null)
                {
                    data.CurrentStatus = ddlCouriarstatus;
                    data.EmployeeId = ddlEmployee;
                    data.EmployeeName = employeename;
                    data.DeskRemarks = remarks;
                    data.DeskTransDate = DateTime.Now;
                    //data.Region = ddldept;
                    data.Department = ddldept;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }


               SendMailEmployeeCourierconfirm(Courier_ID);

            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        //**********

        //public ActionResult CouriarListEmployee()
        //{
        //    if (Session["NAME"] != null)
        //    {

        //        string site = Session["SITE"].ToString();
        //        string location = Session["LOCATION"].ToString();
        //        var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();
        //        ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == dataloc.DeviceNameWrkr).Select(z => z.paycode).Count();
        //        ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();
        //        ViewData["hcv"] = db.sp_HCV(location).Select(z => z.Visitor).Count();
        //        Couriar data = new Couriar();
        //        string UserId = Session["UserID"].ToString();
        //        data.CouriarList = layer.CouriarList("Intrasit", UserId);
        //        //return View(data);

        //        return View(data.CouriarList);

        //    }
        //    else
        //    {
        //        return RedirectToAction("Login");
        //    }




        //}
        public ActionResult CouriarListEmployee()
        {
            if (Session["NAME"] != null)
            {

                bool allowYN = PageRoletype("Employee");
                if (allowYN == false)
                {
                    return RedirectToAction("Login");
                }
                RollPermission();

                Couriar data = new Couriar();
                string UserId = Session["UserID"].ToString();
                data.CouriarList = layer.CouriarList("Intrasit", UserId);
                //return View(data);
                //ViewBag.CouriarCount = data.CouriarList.Count.ToString();


                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }
        [HttpPost]
        public ActionResult ChangeCourierStatusEmployee(FormCollection fc, int Courier_ID)
        {

            if (Session["NAME"] != null)
            {
                string remarks = fc["CouriarRemark"].ToString();
                string name = Session["NAME"].ToString();
                var data = db.tblCouriars.Where(m => m.CouriarId == Courier_ID).FirstOrDefault();
                if (data != null)
                {
                    data.CurrentStatus = "Close";

                    data.EmployeeRemark = remarks;
                    data.EmployeeTransDate = DateTime.Now;

                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }

               

            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult CouriarListAllEmployee()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {


                RollPermission();
                Couriar data = new Couriar();
               // string UserId = Session["UserID"].ToString();
                data.CouriarList = layer.CouriarList("Intrasit", "");
                //return View(data);

                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }

        public ActionResult CouriarListInwardAll()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {


                RollPermission();
                Couriar data = new Couriar();
                if (Session["RoleType"].ToString() == "Employee")
                {
                    string UserId = Session["UserID"].ToString();
                    data.CouriarList = layer.CouriarList("All", UserId);
                }
               else if (Session["RoleType"].ToString() == "A")//Add 3ojune
                {
                 
                    data.CouriarList = layer.CouriarList("Admin", "");
                }
                else
                {
                    data.CouriarList = layer.CouriarList("All", "");
                }
                //return View(data);

                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }







        public ActionResult CouriarListReceiver()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {

                RollPermission();
                Couriar data = new Couriar();
                data.CouriarList = layer.CouriarList("Close", "");
                //return View(data);

                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }

        private void RollPermission()
        {
            if (Session["RoleType"] != null)
            {
                string strRoleType = Session["RoleType"].ToString();
                string strSecurity = Session["SECURITY"].ToString();
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                if (strSecurity == "NO")
                {
                    string s = Session["PNICODE"].ToString();
                    ViewBag.Today = db.sp_AppToday().Where(m => m.EMPLOYEE_ID == s && m.regionid== RegionId).Count();
                    ViewBag.Upcoming = db.sp_AppUpcoming().Where(m => m.EMPLOYEE_ID == s && m.Regionid == RegionId).ToList().Count;
                   

                }
                //else if (strRoleType == "S")
                else if (strSecurity == "YES")
                {
                    string site = Session["SITE"].ToString();
                    string location = Session["LOCATION"].ToString();
                    var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();
                    //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == dataloc.DeviceNameWrkr).Select(z => z.paycode).Count();
                    ViewData["hcw"] = 0;
                    // ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();//18jan
                    ViewData["hce"] = 0;
                    ViewData["hcv"] = db.sp_HCV(location).Where(m=> m.RegionId == RegionId).Select(z => z.Visitor).Count();


                }


            }


        }
        //[HttpPost]
        //public ActionResult ChangeCourierStatusReceiver(FormCollection fc, int Courier_ID, string ddlCouriarstatus)
        //{

        //    if (Session["NAME"] != null)
        //    {
        //        string remarks = fc["CouriarRemark"].ToString();
        //        string name = Session["NAME"].ToString();
        //        var data = db.tblCouriars.Where(m => m.CouriarId == Courier_ID).FirstOrDefault();
        //        if (data != null)
        //        {
        //            data.CurrentStatus = ddlCouriarstatus;

        //            data.ReceiverRemark = remarks;
        //            data.ReceiverTransDate = DateTime.Now;

        //            db.Entry(data).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }

        //        string userId = Session["UserID"].ToString();
        //        CourierHistry(Courier_ID, Session["NAME"].ToString(), "Status  InTransit", "Status Changed by Desk", userId);


        //    }
        //    return Redirect(Request.UrlReferrer.ToString());
        //}




        //**************

        //-------------------
        private bool PageRoletype(string Roletype)
        {
            bool allowYN = false;
            if (Session["RoleType"] != null)
            {
                if (Session["RoleType"].ToString() == Roletype)
                {

                    allowYN = true;
                }
                //----------24-02-25--------------
                if (Session["RoleTypeBothYN"].ToString() == "Yes")
                {
                    allowYN = true;
                }

            }
            return allowYN;

        }

        //----------------
        public ActionResult CouriarEntry()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                bool allowYN = PageRoletype("S");
                if (allowYN == false)
                {
                    return RedirectToAction("Login");
                }
             
                try
                {



                    ViewBag.Cdate = Convert.ToString(DateTime.Now.ToString("dd/MMM/yyyy"));
                    RollPermission();



                }
                catch (Exception ex)
                {

                    layer.SendExcepToDB(ex);

                }



                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }




        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCouriarDetail(FormCollection fc)
        {
            try
            {
                tblCouriar objcCouriar = new tblCouriar();
      
                if (Session["NAME"] != null)
                {

                    //-------insert with sql-17april------
                    int CId = 0;
                    string strgenerate = fc["txtgenno"].ToString();
                    int checkduplicate = layer.checkduplicatecount(strgenerate);
                    if (checkduplicate == 0)
                    {
                         CId= layer.InsertSqlCourierEntry(fc);
                        //layer.preventduplicateInsert(strgenerate);
                    }
                    if (Session["RoleTypeBothYN"].ToString() == "Yes")
                    {
                        string employeename = fc["host"].ToString();
                        string remarks = "Out for Delivery";
                        if (Session["NAME"] != null)
                        {
                            string name = Session["NAME"].ToString();
                            var data = db.tblCouriars.Where(m => m.CouriarId == CId).FirstOrDefault();
                            if (data != null)
                            {
                                data.CurrentStatus = "Intrasit";
                                data.EmployeeId = fc["Requestor"].ToString();
                                data.EmployeeName = employeename;
                                data.DeskRemarks = remarks;
                                data.DeskTransDate = DateTime.Now;
                                //data.Region = ddldept;
                                data.Department = fc["Deptment"].ToString();
                                db.Entry(data).State = EntityState.Modified;
                                db.SaveChanges();
                            }


                         SendMailEmployeeCourierconfirm(CId);

                        }




                    }


                    //}
                    //--------commented in 17 april-remove Entity insert-----------
                    //string strtime = DateTime.Now.ToString("hh:mm tt");

                    //objcCouriar.Remark = fc["txtremark"].ToString();
                    //objcCouriar.CouriarVendor = fc["ddlCourierCompany"].ToString();
                    //objcCouriar.DocketNo = fc["txtdocket"].ToString();

                    //objcCouriar.ReceiveTime = strtime;

                    //objcCouriar.Documents = fc["txtdoc"].ToString();
                    //objcCouriar.City = fc["ddlCity"].ToString();//txtcity


                    //string Vdate = fc["V_DateFrom"].ToString();//+ " 12:00:00 AM"; //10 / 18 / 2022 12:00:00 AM}
                    //objcCouriar.C_Date = Convert.ToDateTime(Vdate);
                    //objcCouriar.CurrentStatus = "Open";
                    //objcCouriar.TransactionDate = DateTime.Now;
                    //objcCouriar.CouriarType = "Inward";

                    ////-----------Feb22-----
                    ////objcCouriar.Region = fc["txtRegion"].ToString();
                    ////objcCouriar.Department = fc["txtdept"].ToString();
                    ////objcCouriar.NameOfCompany = fc["txtcompany"].ToString();
                    //objcCouriar.Region = fc["txtRegionNew"].ToString();

                    //string dept=fc["Deptment"].ToString();
                    //if (dept.Length >= 50)
                    //{
                    //    dept = dept.Substring(0, 49);
                    //}


                    //objcCouriar.Department = dept;
                    //objcCouriar.NameOfCompany = fc["host"].ToString();
                    // objcCouriar.EmployeeName= fc["host"].ToString();
                    //objcCouriar.EmployeeId = fc["Requestor"].ToString();
                    ////----------------------------

                    //objcCouriar.SenderName = fc["txtSendername"].ToString();
                    //objcCouriar.PacketType = fc["ddlPacketType"].ToString();

                    ////------------------
                    //db.tblCouriars.Add(objcCouriar);
                    //db.SaveChanges();
                    //-------------coment 17april

                    if (CId > 0) 
                    { 
                    TempData["CouriarEntry"] = "CouriarEntry";
                    }

                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            catch (Exception ex)
            {
                //ex.Message.ToString();
                layer.SendExcepToDB(ex);
            }

            
            return Redirect(Request.UrlReferrer.ToString());
        }

    
        public ActionResult DispatchEntry()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                ///********Acess permission control
                if (Session["RoleType"] != null)
                {
                    if (Session["RoleType"].ToString() == "Employee")//D
                    {



                        string employeeId = Session["UserID"].ToString();
                        string Department = Session["DEPT"].ToString();
                        var data = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == employeeId).FirstOrDefault();
                        ViewBag.Region = data.Region;
                        ViewBag.Name = data.NAME;
                        ViewBag.Department = Department;
                        ViewBag.senderContactNo = data.MOBILE_NUMBER;
                        ViewBag.Employeeid = employeeId;

                       int RegionId =Convert.ToInt32(data.RegionId);

                     DataTable   dt = layer.Datatable_QUERY("SELECT  RoleType FROM WorkingLogin where RegionId="+ RegionId + "");
                        if (dt != null )
                        {
                            if (dt.Rows.Count > 0)
                            {
                                @ViewBag.RegionRoleType = dt.Rows[0][0].ToString();
                            }
                            else { @ViewBag.RegionRoleType = ""; }
                        }
                        else { @ViewBag.RegionRoleType = ""; }

                    }
                    else
                    {
                        return RedirectToAction("Login");

                    }

                }
                ///********Acess permission control 


                try
                {

                 
                    //RollPermissionDisPatch();
                    tblCouriar objcCouriar = new tblCouriar();
                   
                    ViewBag.Cdate = Convert.ToString(DateTime.Now.ToString("dd/MMM/yyyy"));


                    RollPermission();

                }
                catch (Exception ex)
                {
                    //ex.Message.ToString();
                    layer.SendExcepToDB(ex);

                }



                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }




        }


        [HttpPost]

        public ActionResult SaveDispatchDetail(FormCollection fc)
        {
            try
            {
                tblCouriar objcCouriar = new tblCouriar();
                
                if (Session["NAME"] != null)
                {

                   
                    int CId = layer.InsertSqlDispatchEntry(fc);


                    //-----
                    string RegionRoletype = fc["txtRegionRoletype"].ToString();
                  if (RegionRoletype == "B")//Both
                    {
                        string remarks = "Send to Security";

                        //string name = Session["NAME"].ToString();
                        var data = db.tblCouriars.Where(m => m.CouriarId == CId).FirstOrDefault();
                        if (data != null)
                        {
                            data.CurrentStatus = "Intransit";

                            data.DeskRemarks = remarks;
                            data.DeskTransDate = DateTime.Now; //Convert.ToDateTime(docketdate);
                            int days = 0;
                            if (data.PacketType == "Courier")
                            {
                                days = 7;
                            }
                            else
                            {
                                days = 9;
                            }

                            data.DeliveryDate = DateTime.Now.AddDays(days);

                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();

                        }


                    }
                       

                    //----------
                    if (CId > 1)
                    {

                        TempData["DispatchEntry"] = "DispatchEntry";
                    }

                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            catch (Exception ex)
            {
               
                layer.SendExcepToDB(ex);
            }

            //TempData["Feedback"] = "Feedback";
            return Redirect(Request.UrlReferrer.ToString());
        }


        //DispatchListSecurity
        public ActionResult DispatchListSecurity()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {

                ///********Acess permission control

                bool allowYN = PageRoletype("S");
                if (allowYN == false)
                {
                    return RedirectToAction("Login");
                }
                //***********************




                RollPermission();

                Couriar data = new Couriar();
                string UserId = Session["UserID"].ToString();
                data.CouriarList = layer.DispatchList("Intransit", UserId);

                //ViewBag.CouriarCount = data.CouriarList.Count.ToString();


                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }


        public ActionResult DispatchListAllSecurity()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {

                ///********Acess permission control


                //***********************




                RollPermission();

                Couriar data = new Couriar();

                if (Session["RoleType"].ToString() == "Employee")
                {
                    string UserId = Session["UserID"].ToString();
                    data.CouriarList = layer.DispatchList("All", UserId);
                }
               else if (Session["RoleType"].ToString() == "A")
                {

                    data.CouriarList = layer.DispatchList("Admin", "");
                }
                else
                {
                    data.CouriarList = layer.DispatchList("All", "");

                }


                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }

        public ActionResult DispatchListForDesk()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                ///********Acess permission control

                bool allowYN = PageRoletype("D");
                if (allowYN == false)
                {
                    return RedirectToAction("Login");
                }

                RollPermission();

                Couriar data = new Couriar();
                string UserId = Session["UserID"].ToString();
                //data.CouriarList = layer.DispatchList("Open", UserId);
                data.CouriarList = layer.DispatchList("Open", "Desk");
                //ViewBag.CouriarCount = data.CouriarList.Count.ToString();


                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }


        public ActionResult DispatchListForSecurityCancel()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                ///********Acess permission control

                bool allowYN = PageRoletype("Employee");
                if (allowYN == false)
                {
                    return RedirectToAction("Login");
                }

                RollPermission();

                Couriar data = new Couriar();
                string UserId = Session["UserID"].ToString();
               
                data.CouriarList = layer.DispatchList("DispatchListForSecurityCancel", "Desk");
                


                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }

        public ActionResult DispatchListForEmployee()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                ///********Acess permission control

                bool allowYN = PageRoletype("Employee");
                if (allowYN == false)
                {
                    return RedirectToAction("Login");
                }

                RollPermission();

                Couriar data = new Couriar();
                string UserId = Session["UserID"].ToString();
                data.CouriarList = layer.DispatchList("Open", UserId);

                //ViewBag.CouriarCount = data.CouriarList.Count.ToString();


                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }

        
        public ActionResult DispatchReport()
        {
            if (Session["NAME"] != null)
            {


                RollPermission();

                Couriar data = new Couriar();
                string UserId = Session["UserID"].ToString();
                data.CouriarList = layer.CourierReport("Outward");




                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }
        public ActionResult CourierReport()
        {
            if (Session["NAME"] != null)
            {


                RollPermission();

                Couriar data = new Couriar();
                string UserId = Session["UserID"].ToString();
                data.CouriarList = layer.CourierReport("Inward");




                return View(data.CouriarList);

            }
            else
            {
                return RedirectToAction("Login");
            }




        }

        public ActionResult ChangeDispatchStatusSecurity(FormCollection fc, int Courier_ID)
        {

            if (Session["NAME"] != null)
            {
                string remarks = fc["CouriarRemark"].ToString();
                string ddlCourierCompany = fc["ddlCourierCompany"].ToString();
                string docketdate = fc["txtdocketDate"].ToString();
                string name = Session["NAME"].ToString();
                var data = db.tblCouriars.Where(m => m.CouriarId == Courier_ID).FirstOrDefault();
                if (data != null)
                {
                    data.CurrentStatus = "Close";
                   
                    data.CouriarVendor = fc["txtvendor"].ToString();
                    data.DocketNo = fc["txtdocket"].ToString();
                    //--------------------
                    data.EmployeeRemark = remarks;
                    data.EmployeeTransDate = DateTime.Now;
                    data.DocketDate = Convert.ToDateTime(docketdate);
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }

               

            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult ChangeDispatchStatusDesk(FormCollection fc, int Courier_ID)
        {

            if (Session["NAME"] != null)
            {
                string remarks = fc["CouriarRemark"].ToString();

                string name = Session["NAME"].ToString();
                var data = db.tblCouriars.Where(m => m.CouriarId == Courier_ID).FirstOrDefault();
                if (data != null)
                {
                    data.CurrentStatus = "Intransit";

                    data.DeskRemarks = remarks;
                    data.DeskTransDate = DateTime.Now; //Convert.ToDateTime(docketdate);
                    int days = 0;
                    if (data.PacketType == "Courier")
                    {
                        days = 7;
                    }
                    else
                    {
                        days = 9;
                    }

                    data.DeliveryDate = DateTime.Now.AddDays(days);

                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }

              

            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        //-------------cancel ddispanch-------------
        public ActionResult ChangeDispatchStatusDeskCancel(FormCollection fc, int Courier_ID_cancel)
        {

            if (Session["NAME"] != null)
            {
               

                string name = Session["NAME"].ToString();
                var data = db.tblCouriars.Where(m => m.CouriarId == Courier_ID_cancel).FirstOrDefault();
                if (data != null)
                {
                    data.CurrentStatus = "Cancel";

                    data.DeskRemarks = "Cancel";
                    data.DeskTransDate = DateTime.Now;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }

              
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult ChangeDispatchStatusEmployeeCancel(FormCollection fc, int Courier_ID_cancel)
        {

            if (Session["NAME"] != null)
            {
               

                string name = Session["NAME"].ToString();
                var data = db.tblCouriars.Where(m => m.CouriarId == Courier_ID_cancel).FirstOrDefault();
                if (data != null)
                {
                    data.CurrentStatus = "Cancel";

                    //data.EmployeeRemark = "Cancel";
                    //data.EmployeeTransDate = DateTime.Now;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }

                

            }
            return Redirect(Request.UrlReferrer.ToString());
        }


        //----------------------
        private void RollPermissionDisPatch()
        {
            int RegionId = Convert.ToInt32(Session["RegionId"]);
            if (Session["RoleType"] != null)
            {
                string strRoleType = Session["RoleType"].ToString();
                if (strRoleType == "D")
                {
                    string s = Session["PNICODE"].ToString();
                    ViewBag.Today = db.sp_AppToday().Where(m => m.EMPLOYEE_ID == s  && m.regionid== RegionId).Count();
                    ViewBag.Upcoming = db.sp_AppUpcoming().Where(m => m.EMPLOYEE_ID == s && m.Regionid == RegionId).ToList().Count;
                   


                }
                else if (strRoleType == "S")

                {
                    string site = Session["SITE"].ToString();
                    string location = Session["LOCATION"].ToString();
                    var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();
      
                    ViewData["hcw"] = 0;
                    ViewData["hce"] = 0; //login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();
                    ViewData["hcv"] = db.sp_HCV(location).Where(m=>m.RegionId== RegionId).Select(z => z.Visitor).Count();


                }


            }


        }


       
        public ActionResult GetDepartmentList(string Region)
        {
           


            var data = (from v in login.Master_Requestor

                        where v.Region == Region
                        select new
                        {
                            //NAME = v.NAME,
                            //EMPLOYEE_ID = v.EMPLOYEE_ID,
                            DEPARTMENT = v.DEPARTMENT
                        }
                     ).ToList().Distinct();



            //Select(m => m.CategoryName).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // public ActionResult GetEmployeeList()
        public ActionResult GetEmployeeList(string Dept)
        {
          int regid= Session["RegionId"] != null ? Convert.ToInt32(Session["RegionId"]) : 0;
            //var vData = (from v in login.Master_Requestor
            //             where v.DEPARTMENT == Dept

            var vData = (from v in login.Master_Requestor
                         where v.RegionId == regid && v.DEPARTMENT == Dept
                         select new
                         {
                             NAME = v.NAME,
                             EMPLOYEE_ID = v.EMPLOYEE_ID,
                             DEPARTMENT = v.NAME
                         }
                     ).ToList().Distinct();
            return Json(vData.ToList(), JsonRequestBehavior.AllowGet);



        }
        //---------------region------------

        public ActionResult GetRegionList(string Dept)
        {

         

            var vData = (from v in login.Master_Requestor


                         select new
                         {
                             NAME = v.Region,
                             EMPLOYEE_ID = v.Region,
                             DEPARTMENT = v.Region
                         }
                     ).ToList().Distinct();
            return Json(vData.ToList(), JsonRequestBehavior.AllowGet);



        }


        //---------------------
        public ActionResult GetRegionListForCourier(string Dept)
        {



            var vData = (from v in db.tblCRegions


                         select new
                         {
                             NAME = v.Region,
                             EMPLOYEE_ID = v.Region,
                             DEPARTMENT = v.Region

                         }
                     ).ToList().Distinct();
            return Json(vData.ToList(), JsonRequestBehavior.AllowGet);



        }
        public ActionResult GetDepartmentListForCourier(string Region)
        {
            var data = (from v in db.tblCDepartments

                        where v.Region == Region
                        select new
                        {

                            Id = v.Id,
                            DEPARTMENT = v.Department
                        }
                     ).ToList().Distinct();



            
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCityCourier(string Region)
        {



            var data = (from v in db.tblCCities


                        select new
                        {

                            Id = v.Id,
                            DEPARTMENT = v.City
                        }
                     ).ToList().Distinct();



            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //-----------Region Courier----------

        //-----------------
        public ActionResult DynamicallyRenderWithAllRevision(string RequestId, string page)
        {
            if (Session["NAME"] != null)
            {
                try
                {

                    var rid = Convert.ToInt32(RequestId);

                    List<HistoryModel> itemlist = new List<HistoryModel>();
                    HistoryModel history = new HistoryModel();
                    history.Couriar = db.tblCouriars.Where(m => m.CouriarId == rid).FirstOrDefault();
                    var alldata = db.tblCouriarHistories.Where(m => m.CouriarId == rid).ToList();

                    history.CouriarHistory = alldata;

                    return PartialView(page, history);
                    //--------
                }
                catch (Exception ex)
                {

                    layer.SendExcepToDB(ex);
                    return RedirectToAction("Login");

                }

            }
            else
            {
                return RedirectToAction("Login");
            }

            //-----------
        }





        //---------------

        public JsonResult PopulateDept()
        {
            string region = Session["SITE"].ToString();
            var data = login.Departments.Where(m => m.Region == region).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Unit()
        {
            string region = Session["SITE"].ToString();
            var data = db.tblSites.Where(m => m.Region == region).Select(m => m.L_name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Login()
        {
           
            if (Session["NAME"] != null)
            {
                string s = Session["NAME"].ToString();
                string URL = "";
                switch (s)
                {
                    case "SECURITY":
                        URL = "Security";
                        break;
                    case "ADMIN":
                        URL = "ADMIN";
                        break;
                    default:
                        URL = "Index";
                        break;
                }
                return RedirectToAction(URL);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(string ID, string Password)
        {
            //int RegionId = Convert.ToInt32(Session["RegionId"]);

            if (db.WorkingLogins.Any(m => m.Email == ID && m.UPassword == Password ) == true)//&& m.RegionId== RegionId
            {
                var data = db.WorkingLogins.Where(m => m.Email == ID && m.UPassword == Password).FirstOrDefault();
                Session["UserID"] = data.UserID;// Added jitendra
                Session["SECURITY"] = "YES";
                Session["DEPT"] = "SECURITY";
                Session["SITE"] = data.WStatus;
                Session["LOCATION"] = data.S_Location;
                Session["HOD"] = "NO";

                Session["RegionId"] = data.RegionId;

                ViewData["hcw"] = 0;
                ViewData["hce"] = 0;
                ViewData["hcv"] = 0;
                //------25sep2023
               // Session["RoleType"] = data.RoleType;
                if (data.RoleType == "B")
                {
                    Session["RoleType"] = "S"; 
                    Session["RoleTypeBothYN"] = "Yes";
                }
                else 
                {
                    Session["RoleTypeBothYN"] = "No";
                    Session["RoleType"] = data.RoleType;
                }
                //----
               
                string result = "";
                string[] nn = data.S_Location.Split();

                for (int i = 0; i < nn.Length; i++)
                {
                    if (i < 2)
                    {
                        if (nn[i] != "")
                        {
                            result += nn[i].Substring(0, 1);
                        }
                    }
                }

                Session["PROFILE"] = result;
                // if (ID == "ADMIN")
                //if (data.UserID == "ADMIN")//changed dec5
                if (Session["RoleType"].ToString() == "A")

                {
                    Session["NAME"] = "ADMIN";
                    return RedirectToAction("ADMIN");
                }
                else
                {
                    Session["NAME"] = "SECURITY";
                    return RedirectToAction("Security");
                }
            }
            //else if (login.Master_Requestor.Any(m => m.EMPLOYEE_ID == ID && m.TrumpPassword == Password) == true)
            else if (login.Master_Requestor.Any(m => m.Email_ID == ID && m.TrumpPassword == Password && m.Status=="Active") == true)//changed dec23
            {
                //var data = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == ID && m.TrumpPassword == Password).FirstOrDefault();
                var data = login.Master_Requestor.Where(m => m.Email_ID == ID && m.TrumpPassword == Password).FirstOrDefault();

                Session["COMPANY"] = login.MasterCompanies.Where(m => m.MC_ID == data.CompanyID).FirstOrDefault().CompanyName;
                Session["SECURITY"] = "NO";
                Session["SITE"] = data.Region;
                Session["PNICODE"] = data.EMPLOYEE_ID;
                Session["NAME"] = data.NAME;
                Session["DEPTID"] = data.DepartmentID;
                Session["DEPT"] = login.Departments.Where(m => m.D_ID == data.DepartmentID).FirstOrDefault().DepartmentName;
                Session["MBL"] = data.MOBILE_NUMBER;
                Session["DES"] = data.DESIGNATION;
                Session["EMAIL"] = data.Email_ID;
                Session["KSSDEPT"] = data.KSS_Department;
                
                Session["AP_HOD"] = data.HOD_EMP_ID;
                Session["UserID"] = data.EMPLOYEE_ID;
                Session["RoleType"] = "Employee";
                Session["RegionId"] = data.RegionId;

                //---------Added 26sep-2023 --------

                int RegionId = Convert.ToInt32(data.RegionId);
            
                DataTable dt = layer.Datatable_QUERY("SELECT  RoleType FROM WorkingLogin where RegionId=" + RegionId + "");
                Session["RoleTypeBothYN"] = "No";
                if (dt != null)
                {
                    if (dt.Rows.Count > 0) 
                    {

                    if (dt.Rows[0][0].ToString() == "B")
                    {
                        Session["RoleTypeBothYN"] = "Yes";
                    }
                   
                    }


                }
                    //----------------------

                    if (db.WorkingLogins.Any(m => m.Email == ID) == true)
                {
                    
                    var dataRole = db.WorkingLogins.Where(m => m.Email == ID).FirstOrDefault();
                    Session["RoleType"] = dataRole.RoleType;

                }
                //-------------

                string result = "";
                string[] nn = data.NAME.Split();

             
                if (login.Master_Requestor.Any(m => m.HOD_EMP_ID == data.EMPLOYEE_ID) == true)
                {
                    Session["HOD"] = "YES";
                }
                else
                {
                    Session["HOD"] = "NO";
                }
                for (int i = 0; i < nn.Length; i++)
                {
                    if (i < 2)
                    {
                        if (nn[i] != "")
                        {
                            result += nn[i].Substring(0, 1);
                        }
                    }
                }
                Session["PROFILE"] = result;
                if (data.TrumpPassword == "123456" || data.TrumpPassword == "karam@123")
                {
                    return RedirectToAction("ChangePassword");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["LoginFail"] = "LoginFail";

                return View();
            }
        }

        public ActionResult HCVAll()
        {
            int RegionId = Convert.ToInt32(Session["RegionId"]);
            //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In").Select(z => z.paycode).Count();
            ViewData["hcw"] = 0;
           // ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In").Select(z => z.PNI_code).Count();
            ViewData["hce"] = 0;
            ViewData["hcv"] = db.sp_HCVALL().Where(m=>m.Regionid== RegionId).Select(z => z.Visitor).Count();
            var data = db.sp_HCVALL().Where(m => m.Regionid == RegionId).ToList();
            return View(data);
        }

        //public ActionResult ADMIN()//Regionwise
        //{
        //    int RegionId = Convert.ToInt32(Session["RegionId"]);
        //    bool allowYN = PageRoletype("A");
        //    if (allowYN == false)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    ViewBag.Today = db.tblAppoinments.Where(m => m.V_DateFrom == DateTime.Today && (m.V_Status != "Cancel" && m.V_Status != "Reject" && m.RegionId == RegionId)).Count();
        //    ViewBag.Upcoming = db.sp_AppUpcoming().Where(m => m.Regionid == RegionId).ToList().Count();
        //    // ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In").Select(z => z.paycode).Count();
        //    ViewData["hcw"] = 0;
        //    //ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In").Select(z => z.PNI_code).Count();change 19 jan
        //    //ViewData["hcv"] = db.sp_HCVALL().Select(z => z.Visitor).Count();//change 19 jan
        //    ViewData["hce"] = 0;//change 19 jan
        //    ViewData["hcv"] = 0;
        //    //--------------

        //    ViewBag.InwardCourierCount = db.tblCouriars.Where(m => m.CouriarType == "Inward" && m.RegionId == RegionId).ToList().Count;
        //    ViewBag.OutwardCourierCount = db.tblCouriars.Where(m => m.CouriarType == "Outward" && m.RegionId == RegionId).ToList().Count;

        //    int totalApp = db.tblAppoinments.Where(m=> m.RegionId == RegionId).ToList().Count();
        //    ViewBag.TotalApp = totalApp;
        //    //-----------------


        //    CommonViewModel model = new CommonViewModel();
        //    model.Appointment = layer.RecentVisitorAdmin;

        //    ViewBag.Req = login.Master_Requestor.ToList();
        //    return View(model);
        //}

        public ActionResult ADMIN()
        {
            int RegionId = Convert.ToInt32(Session["RegionId"]);
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }

            //ViewBag.Today = db.tblAppoinments.Where(m => m.V_DateFrom == DateTime.Today && (m.V_Status != "Cancel" && m.V_Status != "Reject" && m.RegionId == RegionId)).Count();
            //ViewBag.Upcoming = db.sp_AppUpcoming().Where(m => m.Regionid == RegionId).ToList().Count();


            ViewBag.Today = db.tblAppoinments.Where(m => m.V_DateFrom == DateTime.Today && (m.V_Status != "Cancel" && m.V_Status != "Reject" )).Count();
            ViewBag.Upcoming = db.sp_AppUpcoming().ToList().Count();


            // ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In").Select(z => z.paycode).Count();
            ViewData["hcw"] = 0;
            //ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In").Select(z => z.PNI_code).Count();change 19 jan
            //ViewData["hcv"] = db.sp_HCVALL().Select(z => z.Visitor).Count();//change 19 jan
            ViewData["hce"] = 0;//change 19 jan
            ViewData["hcv"] = 0;
            //--------------

            ViewBag.InwardCourierCount = db.tblCouriars.Where(m => m.CouriarType == "Inward" ).ToList().Count;
            ViewBag.OutwardCourierCount = db.tblCouriars.Where(m => m.CouriarType == "Outward").ToList().Count;

            int totalApp = db.tblAppoinments.ToList().Count();
            ViewBag.TotalApp = totalApp;
            //-----------------


            CommonViewModel model = new CommonViewModel();
            model.Appointment = layer.RecentVisitorAdmin;

            ViewBag.Req = login.Master_Requestor.ToList();
            return View(model);
        }
        public ActionResult EditPage(int EditID)
        {
            if (Session["NAME"] != null)
            {
                Session.LCID = 1033;
                SecurityModel data = new SecurityModel();
                data.A = db.tblAppoinments.Where(m => m.A_ID == EditID).FirstOrDefault();
                data.M = db.tblMaterials.Where(m => m.App_ID == data.A.A_ID).ToList();
                var vData = (from v in db.tblVisitors
                             join c in db.tblCompanies
                             on v.C_ID equals c.C_ID
                             where v.V_ID == data.A.VisitorID
                             select new
                             {
                                 Name = v.V_Name,
                                 Mobl = v.V_Phone,
                                 ID = v.V_ID,
                                 Pic = v.V_Pic,
                                 v_ID = v.Visitor_ID,
                                 v_IDNumber = v.V_IDNumber,
                                 ComName = c.CName
                             }
                      ).FirstOrDefault();
                ViewBag.Company = vData.ComName;
                ViewBag.MBL = vData.Mobl;
                ViewBag.VName = vData.Name;
                ViewBag.VIDType = vData.v_ID;
                ViewBag.VID_nmbr = vData.v_IDNumber;
                ViewBag.Pic = vData.Pic;
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        //public ActionResult EditPage(tblAppoinment ap, int hiddenGPID)
        public ActionResult EditPage(tblAppoinment ap, int hiddenGPID, FormCollection fc)
        {
            if (Session["NAME"] != null)
            {
                RemoveCache();
                var tb = db.tblAppoinments.Where(z => z.A_ID == hiddenGPID).FirstOrDefault();
                var req = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == tb.Requestor).FirstOrDefault();
                string CompanyMaster = login.MasterCompanies.Where(m => m.MC_ID == req.CompanyID).FirstOrDefault().CompanyName;

                tb.Visitor_Type = ap.Visitor_Type;
                tb.V_DateFrom = ap.V_DateFrom;
                tb.V_DateTo = tb.EntryType == "1" ? ap.V_DateFrom : ap.V_DateTo;
                tb.A_Time = ap.A_Time;
                tb.Duration = ap.Duration;
                tb.Location = ap.Location;
                tb.Flag = "False";
                db.Entry(tb).State = EntityState.Modified;
                db.SaveChanges();
                ///-----Added adhar c--------
                var vDataUp = db.tblVisitors.Where(m => m.V_ID == tb.VisitorID).FirstOrDefault();
                vDataUp.Visitor_ID = fc["IDProofType"].ToString();
                vDataUp.V_IDNumber = fc["Vidnumber"].ToString();

                db.Entry(vDataUp).State = EntityState.Modified;
                db.SaveChanges();
                //-----------

                string path = Server.MapPath("~/Content/images/QR/" + tb.GPID + ".png");
                var report = new Rotativa.ActionAsImage("Contact", new { id = tb.GPID, path = path })
                {
                    PageWidth = 370,
                    FileName = "MyImage.jpeg",
                };

                string fullPath = Server.MapPath("../Content/images/StringImage/" + tb.GPID + ".jpg");

                var binary = report.BuildFile(this.ControllerContext);
                var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                fileStream.Write(binary, 0, binary.Length);
                fileStream.Close();

                // var msgtemp = db.Msgtemplates.Where(m => m.Action == "AppointmentSMS" && m.Region == tb.V_Location).FirstOrDefault();
                var msgtemp = db.Msgtemplates.Where(m => m.Action == "AppointmentSMS" && m.Region == "PNI").FirstOrDefault();
                var vData = db.tblVisitors.Where(m => m.V_ID == tb.VisitorID).FirstOrDefault();
                //---------------feb17-----------------
                //string AppointmntMsg = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", tb.V_DateFrom.Value.ToString("dd/MMM/yyyy")).Replace("@p3", tb.A_Time + ".").Replace("@p4", tb.GPID).Replace("@p5", tb.GPID);

                //layer.SendSMS(vData.V_Phone, AppointmntMsg, "1007161968597981063", "KRMINF");


                string link = "https://vms.karam.in";
                string Appurl = ConfigurationManager.AppSettings["Appurl"].ToString();
                // string AppointmntMsg = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", tb.V_DateFrom.Value.ToString("dd/MMM/yyyy")).Replace("@p3", tb.A_Time + ".").Replace("@p4", tb.GPID).Replace("@p5", tb.GPID).Replace("@p6", link);//working
                //string AppointmntMsg = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", tb.V_DateFrom.Value.ToString("dd/MMM/yyyy") + "-").Replace("@p3", tb.A_Time + ".").Replace("@p4", tb.GPID).Replace("@p5", tb.GPID).Replace("@p6", link);//with dash

                string AppointmntMsg = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", tb.V_DateFrom.Value.ToString("dd/MMM/yyyy") + "-").Replace("@p3", tb.A_Time + "").Replace("@p4", tb.GPID).Replace("@p5", tb.GPID).Replace("@p6", Appurl);//with dash



                layer.SendSMS(vData.V_Phone, AppointmntMsg, "1007167628437266921", "KRMINF");// 

                //Added whats msg 27-02-25
                try
                {
                    string str1 = CompanyMaster;
                    string str2 = tb.V_DateFrom.Value.ToString("dd/MMM/yyyy");
                    string str3 = tb.A_Time;
                    string str4 = Appurl;// "https://vms.karam.in";
                    string str5 = tb.GPID;
                    string str6 = tb.GPID;
                    layer.SendWhatsMessage(vData.V_Phone, str1, str2, str3, str4, str5, str6);// 
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                //-----------------------------

                //------------------

                string url = @"https://vms.karam.in/Home/SG/" + tb.GPID;
                layer.WhatsApp(vData.V_Phone, "enx_schedule_appointment", CompanyMaster, tb.V_DateFrom.Value.ToString("dd MMM"), tb.A_Time, url);

                return RedirectToAction("Appointment");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public JsonResult GetVendorMbl(string Prefix)
        {
            var empResult = service.GetVendorMBL(Prefix).ToList();
            return Json(empResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Contact(string id, string path)
        {
            ViewBag.a = path;
            var data = db.sp_DownloadePass(id).ToList();
            return View(data);
        }
        public JsonResult GetVendorMbl(int ID)
        {
            var data = db.tblVisitors.Where(m => m.V_ID == ID).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVendorData(string Prefix)
        {

            RemoveCache();
            var vData = (from v in db.tblVisitors
                         join c in db.tblCompanies
                         on v.C_ID equals c.C_ID
                         where v.V_Phone == Prefix
                         orderby v.V_ID descending
                         select new
                         {
                             Name = v.V_Name,
                             ID = v.V_ID,
                             Pic = v.V_Pic,
                             CName = c.CName,
                             IDType = v.Visitor_ID,
                             IDNumber = v.V_IDNumber,
                             CID = v.C_ID
                         }
                      ).FirstOrDefault();
            return Json(vData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetExecutiveData(string Prefix)
        {
            var data = service.GetExecutive(Prefix);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OutwordContact(string Prefix)
        {
            var data = service.OutwordContact(Prefix);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSiteData()
        {
            //int RegionId = Convert.ToInt32(Session["RegionId"]);

            var vendordata = db.tblCompanies.Where(m => m.C_Status == "Active" ).OrderBy(m => m.CName).ToList();//&& m.RegionId== RegionId
            //var result = new { SiteData = sitedata, VendorData = vendordata };
            return Json(vendordata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVisitType()
        {
            var data = db.VisitTypes.Where(m => m.VStatus == "Active").OrderBy(m => m.VstTpe).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetVisitorData(int ID)
        {
            //int RegionId = Convert.ToInt32(Session["RegionId"]);


            var visitordata = db.tblVisitors.Where(m => m.C_ID == ID).ToList();//&& m.RegionId== RegionId
            return Json(visitordata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Register(Company c)
        {
            var cc = "";
            string res = string.Empty;
            try
            {
                cc = layer.SaveCompany(c);
                return Json(cc, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                res = "Failed";
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult RegisterVisitor(Visitor c)
        {
            var cc = "";
            string res = string.Empty;
            try
            {
                cc = layer.SaveVisitor(c);
                res = "Successfully Inserted...!";
                return Json(cc, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                res = "Failed";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Appointment1()
        {
            if (Session["NAME"] != null)
            {
                var data = layer.AppoinmentList;
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Create()
        {
            if (Session["NAME"] != null)
            {
                bool allowYN = PageRoletype("Employee");
             
                if (allowYN == false)
                {
                    return RedirectToAction("Login");
                }

                Session.LCID = 1033;
                RemoveCache();

                ViewBag.Cdate = Convert.ToString(DateTime.Now.ToString("dd/MMM/yyyy"));

                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public PartialViewResult _ClickedImage()
        {
            var img = Session["CapturedImage"];
            if (img == null)
            {
                Session["CapturedImage"] = "";
            }
            else
            {
                Session["CapturedImage"].ToString();
            }

            return PartialView();
        }

        public ActionResult SG(string ID)
        {
            ViewBag.GatePassID = ID;
            return View();
        }

        [HttpPost]
        public ActionResult AcceptGuidelines(string GatePassID)
        {
            var dd = db.VisitorAcceptances.Any(z => z.GatePassID == GatePassID);
            if (dd == true)
            {
                ViewFile(GatePassID);
                string path = Server.MapPath("~/QRCode/" + GatePassID + ".png");

                string filename = "ePass.jpg";

                Response.Clear();
                Response.ContentType = "image/png";
                Response.AppendHeader("content-disposition", "filename=" + filename);
                Response.TransmitFile(Server.MapPath("~/StringImage/" + GatePassID + ".jpg"));
                Response.Flush();
                Response.End();

                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                var data = db.tblAppoinments.Where(m => m.GPID == GatePassID).FirstOrDefault();

                VisitorAcceptance accept = new VisitorAcceptance();
                accept.AccptGate = DateTime.Now;
                accept.GatePassID = GatePassID;
                accept.VisitorID = data.VisitorID;
                db.VisitorAcceptances.Add(accept);
                db.SaveChanges();

                string fullPath = Server.MapPath("../StringImage/" + GatePassID + ".jpg");
                string filename = "ePass.jpg";

                string attachmentFilename = fullPath;

                MailMessage mm = new MailMessage();
                mm.From = new MailAddress("karamsupport@karam.in");
                mm.To.Add("ashahar.zaved@karam.in");
                mm.Subject = "e-Pass";
                StringBuilder sb = new StringBuilder();
                sb.Append("Dear Sir/Ma'am,");
                sb.Append("<br/><br/>Please show the attatched e-Pass on security gate at the time of entry in PN Internation Pvt Ltd.");

                AlternateView aw = AlternateView.CreateAlternateViewFromString("<img src=cid:imagepath width='100' height='80' />", null, "text/html");

                if (attachmentFilename != null)
                {
                    Attachment attachment = new Attachment(attachmentFilename, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = attachment.ContentDisposition;
                    disposition.FileName = Path.GetFileName(attachmentFilename);
                    disposition.Size = new FileInfo(attachmentFilename).Length;
                    disposition.DispositionType = DispositionTypeNames.Attachment;
                    mm.Attachments.Add(attachment);
                }

                mm.Body = sb.ToString();
                mm.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new NetworkCredential("karamsupport@karam.in", "Passw0rd");
                smtp.Credentials = new System.Net.NetworkCredential("karamsupport@karam.in", "Angel@123", "Karam.in");
                smtp.EnableSsl = true;

                //smtp.Send(mm);

                Response.Clear();
                Response.ContentType = "image/png";
                Response.AppendHeader("content-disposition", "filename=" + filename);
                Response.TransmitFile(Server.MapPath("~/StringImage/" + GatePassID + ".jpg"));
                Response.Flush();
                Response.End();
                return RedirectToAction("SG", "Home", new { ID = GatePassID });
            }
        }
        [HttpPost]
        public FileResult ImageDownload(string GatePassID)
        {
            var dd = db.VisitorAcceptances.Any(z => z.GatePassID == GatePassID);
            var data = db.tblAppoinments.Where(m => m.GPID == GatePassID).FirstOrDefault();

            //string epassfile = Server.MapPath("~/StringImage/" + data.GPID + ".jpg");

            //FileInfo file = new FileInfo(epassfile);
            //if(!file.Exists)
            //{
            //    var write = new BarcodeWriter();
            //    write.Format = BarcodeFormat.QR_CODE;
            //    var result = write.Write(GatePassID);
            //    Bitmap bitmap = new Bitmap(result);
            //    write.Options = new EncodingOptions()
            //    {
            //        Height = 80,
            //        Width = 100
            //    };
            //    bitmap.SetResolution(300, 300);

            //    string path = Server.MapPath("~/QRCode/" + GatePassID + ".png");
            //    using (MemoryStream memory = new MemoryStream())
            //    {
            //        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            //        {
            //            bitmap.Save(memory, ImageFormat.Png);
            //            byte[] bytes = memory.ToArray();
            //            fs.Write(bytes, 0, bytes.Length);
            //        }
            //    }

            //    var report = new ActionAsImage("Contact", new { id = GatePassID, path = path })
            //    {
            //        PageWidth = 370,
            //        FileName = "MyImage.jpeg",
            //    };
            //    string fullPath = Server.MapPath("../StringImage/" + GatePassID + ".jpg");

            //    var binary = report.BuildFile(this.ControllerContext);
            //    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            //    fileStream.Write(binary, 0, binary.Length);
            //    fileStream.Close();
            //}

            string imgPath = "";
            if (dd == true)
            {
                imgPath = Server.MapPath("../Content/images/StringImage/" + GatePassID + ".jpg");
            }
            else
            {
                imgPath = Server.MapPath("../Content/images/StringImage/" + GatePassID + ".jpg");

                var vData = db.tblVisitors.Where(m => m.V_ID == data.VisitorID).FirstOrDefault();
                var req = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.Requestor).FirstOrDefault();
                string mastercompany = login.MasterCompanies.Where(m => m.MC_ID == req.CompanyID).FirstOrDefault().CompanyName;

                VisitorAcceptance accept = new VisitorAcceptance();
                accept.AccptGate = DateTime.Now;
                accept.GatePassID = GatePassID;
                accept.VisitorID = data.VisitorID;
                accept.Flag = "False";
                db.VisitorAcceptances.Add(accept);
                db.SaveChanges();

                // var msgdata = db.Msgtemplates.Where(m => m.Region == data.V_Location && m.Action == "Accept").FirstOrDefault();//nov18
                var msgdata = db.Msgtemplates.Where(m => m.Region == "PNI" && m.Action == "Accept").FirstOrDefault();
                string ePass_Save = msgdata.Template.Replace("@p1", "(" + data.Valid_at + ")").Replace("@p2", mastercompany);

                layer.SendSMS(vData.V_Phone, ePass_Save, "1007161949713007374", "KRMITI");
                //layer.WhatsAppAttachment(vData.V_Phone.ToString(), data.Valid_at, mastercompany, "https://vms.karam.in/Content/images/StringImage/" + GatePassID + ".jpg");
            }
            return File(imgPath, "image/jpeg", "e-Pass.jpg");
        }
        public ActionResult ViewFile(string GatePassID)
        {
            string contentType = "Image/jpeg";
            string fullPath = Server.MapPath("../Content/images/StringImage/" + GatePassID + ".jpg");

            if (fullPath == null)
            {
                return this.Content("No picture for this program.");
            }

            return File(fullPath, contentType, GatePassID + ".jpg");
        }

        //Create Appointment 
        [HttpPost]
        // public ActionResult Create(tblAppoinment ap, List<string> Valid_at)//comment jitendra 14oct
        public ActionResult Create(tblAppoinment ap, List<string> Valid_at, string[] cvName, string[] cvIdType, string[] cvIdNumber, string[] cvMobile, string txtfile)
        {
            if (Session["NAME"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                string validatnew = Session["SITE"].ToString();
                //-------------
                //------erpdetail------------
                ////------------15may----------------
                string strgenerate = Request.Form["txtgenno"].ToString();
                int checkduplicate = layer.checkduplicatecount(strgenerate);
                if (checkduplicate == 0)
                {
                    layer.preventduplicateInsert(strgenerate);

                    //    //-----15may------------

                    int VisitorId = Convert.ToInt32(ap.VisitorID);
                    functionCreateVisitor(ap, Valid_at, VisitorId, "Visitor", txtfile, RegionId, validatnew);

                    string VisitorType = "";
                    var visitordata = db.tblVisitors.Where(m => m.V_ID == ap.VisitorID).FirstOrDefault();

                    //--change Aug 16 2023-- for visitor type identification-

                    visitordata.RegionId = 1;
                    db.Entry(visitordata).State = EntityState.Modified;
                    db.SaveChanges();
                    //----------

                    //---------
                    for (int i = 0; i < cvName.Length; i++)
                    {


                        string strName = cvName[i];
                        string bb = cvIdType[i];
                        string cc = cvIdNumber[i];
                        string dd = cvMobile[i];
                        VisitorType = "CoVisitor";
                        if (strName.Trim() != "")
                        {
                            tblVisitor obj = new tblVisitor();
                            obj.V_Name = cvName[i];
                            obj.V_Phone = cvMobile[i];
                            obj.V_Email = "";
                            obj.Visitor_ID = cvIdType[i];
                            obj.V_IDNumber = cvIdNumber[i];
                            obj.V_Pic = "";
                            obj.V_Status = visitordata.V_Status;
                            obj.V_Type = VisitorType;
                            obj.C_ID = visitordata.C_ID;
                            obj.RegionId = 2;
                            db.tblVisitors.Add(obj);

                            db.SaveChanges();
                            int V_ID = obj.V_ID;
                            functionCreateVisitor(ap, Valid_at, V_ID, "", "", RegionId, validatnew);


                        }


                    }
                    //-----------
                    //functiontest(ap, Valid_at);
                    //------------

                   

                }
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //private void functionCreateVisitor(tblAppoinment ap, List<string> Valid_at, int V_ID)
        //{
        //    string[] dept = Session["PNICODE"].ToString().Split('_');
        //    string deptID = Session["DEPTID"].ToString();
        //    string deptIDw = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
        //    string[] date = deptIDw.Split('-');
        //    string[] yr = date[2].Split(' '); string[] tm = yr[1].Split(':');
        //    string r = date[0].ToString().TrimStart('0');
        //    string h = tm[0].ToString().TrimStart('0');

        //    string region = Session["SITE"].ToString();

        //    var req = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == ap.Requestor).FirstOrDefault();
        //    string CompanyMaster = login.MasterCompanies.Where(m => m.MC_ID == req.CompanyID).FirstOrDefault().CompanyName;

        //    //Generate ID
        //    string value1 = "V" + date[1] + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(r)) + yr[0].Substring(2) + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(h));

        //    //string vis_AllowLocation = ap.Location == null ? "Green" : ap.Location;//31oct
        //    string vis_AllowLocation = "Green";

        //    string duration = ap.Duration == null ? "0-2(Hrs)" : ap.Duration;
        //    string visitorType = ap.Visitor_Type == null ? "Official" : ap.Visitor_Type;

        //    if (ap.ComanyID == null || ap.VisitorID == null)
        //    {
        //        TempData["V_C_notselect"] = "V_C_notselect";
        //    }
        //    else
        //    {

        //        string ddd = Convert.ToDateTime(DateTime.Now).ToShortDateString();
        //        string frdate = Convert.ToDateTime(ap.V_DateFrom).ToShortDateString();
        //        string valid = String.Join(",", Valid_at);
        //        tblAppoinment appoinment = new tblAppoinment();
        //        appoinment.A_Time = ap.A_Time;
        //        appoinment.A_Type = ap.A_Type;
        //        appoinment.ComanyID = ap.ComanyID;
        //        appoinment.Deptment = Convert.ToInt32(deptID);
        //        appoinment.Duration = duration;
        //        appoinment.Valid_at = valid;
        //        appoinment.EntryType = ap.EntryType;
        //        appoinment.Location = vis_AllowLocation;
        //        appoinment.RaiseDate = DateTime.Now;
        //        appoinment.V_DateTo = Convert.ToDateTime(frdate);//Convert.ToDateTime(ddd);
        //        appoinment.Remark = ap.Remark;
        //        appoinment.Requestor = ap.Requestor;
        //        //appoinment.VisitorID = ap.VisitorID;
        //        appoinment.VisitorID = V_ID;

        //        appoinment.Visitor_Type = visitorType;
        //        appoinment.V_Allowed = ap.V_Allowed;
        //        //appoinment.V_DateFrom = ap.V_DateFrom;
        //        appoinment.V_DateFrom = Convert.ToDateTime(frdate); //Convert.ToDateTime(ddd);
        //        appoinment.Flag = "False";
        //        if (ap.EntryType == "2")
        //        {
        //            var a = Request.Form["V_DateFromM"]; var b = Request.Form["V_DateToM"];
        //            var c = Request.Form["A_TimeM"];
        //            //appoinment.V_DateFrom = Convert.ToDateTime(a);
        //            //appoinment.V_DateTo = Convert.ToDateTime(ddd);
        //            appoinment.V_DateFrom = Convert.ToDateTime(ap.V_DateFrom);
        //            appoinment.V_DateTo = Convert.ToDateTime(ap.V_DateFrom);



        //            appoinment.A_Time = c.ToString();
        //        }
        //        else
        //        {
        //            appoinment.V_DateTo = Convert.ToDateTime(frdate);
        //        }
        //        appoinment.V_Location = ap.V_Location;
        //        appoinment.V_Status = ap.A_Type;
        //        //-------31 oct------
        //        appoinment.VehicleNo = ap.VehicleNo;
        //        //-----------
        //        db.tblAppoinments.Add(appoinment);
        //        db.SaveChanges();

        //        string ch = "";
        //        switch (appoinment.A_ID)
        //        {
        //            case int n when n <= 9:
        //                ch = ch + "000" + appoinment.A_ID;
        //                break;
        //            case int n when n >= 10 && n <= 99:
        //                ch = ch + "00" + appoinment.A_ID;
        //                break;
        //            case int n when n > 100 && n <= 999:
        //                ch = "0" + appoinment.A_ID;
        //                break;
        //            default:
        //                ch = ch + appoinment.A_ID;
        //                break;
        //        }

        //        var lastData = db.tblAppoinments.Where(m => m.A_ID == appoinment.A_ID).FirstOrDefault();
        //        lastData.GPID = value1 + ch;
        //        db.Entry(lastData).State = EntityState.Modified;
        //        db.SaveChanges();

        //        var com = db.tblCompanies.Where(m => m.C_ID == lastData.ComanyID).FirstOrDefault();

        //        string chkT = Request.Form["chkselectedmaterial_T"];
        //        string chkL = Request.Form["chkselectedmaterial_L"];
        //        string chkMD = Request.Form["chkselectedmaterial_MD"];
        //        string chkSD = Request.Form["chkselectedmaterial_SD"];

        //        if (chkL == "on")
        //        {
        //            tblMaterial material = new tblMaterial();
        //            string sL = Request.Form["Serial_No_L"].ToString().ToUpper();
        //            material.App_ID = Convert.ToInt32(lastData.A_ID);
        //            material.M_Name = "Laptop";
        //            material.Serial_No = sL;
        //            db.tblMaterials.Add(material);
        //            db.SaveChanges();
        //        }
        //        if (chkT == "on")
        //        {
        //            tblMaterial material = new tblMaterial();
        //            string sL = Request.Form["Serial_No_T"].ToString().ToUpper();
        //            material.App_ID = Convert.ToInt32(lastData.A_ID);
        //            material.M_Name = "Tool Box";
        //            material.Serial_No = sL;
        //            db.tblMaterials.Add(material);
        //            db.SaveChanges();
        //        }
        //        if (chkMD == "on")
        //        {
        //            tblMaterial material = new tblMaterial();
        //            string sL = Request.Form["Serial_No_MD"].ToString().ToUpper();
        //            material.App_ID = Convert.ToInt32(lastData.A_ID);
        //            material.M_Name = "Media Device";
        //            material.Serial_No = sL;
        //            db.tblMaterials.Add(material);
        //            db.SaveChanges();
        //        }
        //        if (chkSD == "on")
        //        {
        //            tblMaterial material = new tblMaterial();
        //            string sL = Request.Form["Serial_No_SD"].ToString().ToUpper();
        //            material.App_ID = Convert.ToInt32(lastData.A_ID);
        //            material.M_Name = "Storage Device";
        //            material.Serial_No = sL;
        //            db.tblMaterials.Add(material);
        //            db.SaveChanges();
        //        }

        //        var vData = db.tblVisitors.Where(m => m.V_ID == lastData.VisitorID).FirstOrDefault();
        //        var msgtemp = db.Msgtemplates.Where(m => m.Action == "AppointmentSMS" && m.Region == "PNI").FirstOrDefault();

        //        string AppointmntTemp = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy")).Replace("@p3", lastData.A_Time + ".").Replace("@p4", lastData.GPID).Replace("@p5", lastData.GPID);


        //      layer.SendSMS(vData.V_Phone, AppointmntTemp, "1007161968597981063", "KRMINF");//     comment 17octn  comment Oct25




        //        string urls = "https://vms.karam.in/Home/SG/" + lastData.GPID;


        //        string urlimage = "https://vms.karam.in/Content/images/StringImage/" + lastData.GPID + ".jpg";


        //        TempData["create"] = "create";

        //        var write = new BarcodeWriter();
        //        write.Format = BarcodeFormat.QR_CODE;
        //        var result = write.Write(lastData.GPID);
        //        Bitmap bitmap = new Bitmap(result);
        //        write.Options = new EncodingOptions()
        //        {
        //            Height = 80,
        //            Width = 100
        //        };
        //        bitmap.SetResolution(300, 300);

        //        string path = Server.MapPath("~/Content/images/QR/" + lastData.GPID + ".png");

        //        using (MemoryStream memory = new MemoryStream())
        //        {
        //            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
        //            {
        //                bitmap.Save(memory, ImageFormat.Png);
        //                byte[] bytes = memory.ToArray();
        //                fs.Write(bytes, 0, bytes.Length);
        //            }
        //        }

        //        var report = new ActionAsImage("Contact", new { id = lastData.GPID, path = path })
        //        {
        //            PageWidth = 370,
        //            FileName = "MyImage.jpeg",
        //        };

        //        string fullPath = Server.MapPath("../Content/images/StringImage/" + lastData.GPID + ".jpg");

        //        var binary = report.BuildFile(this.ControllerContext);
        //        var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        //        fileStream.Write(binary, 0, binary.Length);
        //        fileStream.Close();

        //       layer.WhatsAppAttachment(vData.V_Phone.ToString(), lastData.V_DateFrom.Value.ToString("dd MMM"), CompanyMaster, urlimage);//urlimage fullPath

        //    }
        //}
        //  private void functionCreateVisitor(tblAppoinment ap, List<string> Valid_at, int V_ID)
        private void functionCreateVisitor(tblAppoinment ap, List<string> Valid_at, int V_ID, string Vistortype, string txtfile,int RegionId, string validatnew)
        {
            Session.LCID = 1033;

            string[] dept = Session["PNICODE"].ToString().Split('_');
            string deptID = Session["DEPTID"].ToString();
            // string deptIDw = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            //string[] date = deptIDw.Split('-');
            //string[] yr = date[2].Split(' '); string[] tm = yr[1].Split(':');
            //string r = date[0].ToString().TrimStart('0');
            //string h = tm[0].ToString().TrimStart('0');
            string[] date = DateTime.Now.ToString().Split('/');
            string[] yr = date[2].Split(' '); string[] tm = yr[1].Split(':');
            string r = date[0].ToString().TrimStart('0');
            string h = tm[0].ToString().TrimStart('0');

            //---------jan 31---
            if (Vistortype == "Visitor" && txtfile == "FileExists")
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files[0];

                string str = Convert.ToString(files[0]);
                string saveFileN = Convert.ToString(V_ID) + ".jpg";
                if (Request.Files.Count > 0)
                {

                    string folderPath = Server.MapPath("~/Content/ProfilPic/");

                    var ServerSavePath = Path.Combine(folderPath + saveFileN);
                    // Get the complete folder path and store the file inside it.  
                    file.SaveAs(ServerSavePath);

                    //============
                    var VistorData = db.tblVisitors.Where(m => m.V_ID == V_ID).FirstOrDefault();

                    VistorData.V_Pic = saveFileN;
                    // string strapVisitorid = Convert.ToString(ap.VisitorID);
                    //TrumpService.SaveCapturedImage(imgHidden, strapVisitorid);
                    //VistorData.V_Pic = strapVisitorid + ".jpg";
                    db.Entry(VistorData).State = EntityState.Modified;
                    db.SaveChanges();



                    //================



                }


            }

            //----------------

            string region = Session["SITE"].ToString();

            var req = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == ap.Requestor).FirstOrDefault();
            string CompanyMaster = login.MasterCompanies.Where(m => m.MC_ID == req.CompanyID).FirstOrDefault().CompanyName;

            //Generate ID
            string value1 = "V" + date[1] + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(r)) + yr[0].Substring(2) + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(h));

            //string vis_AllowLocation = ap.Location == null ? "Green" : ap.Location;//31oct
            string vis_AllowLocation = "Green";

            string duration = ap.Duration == null ? "0-2(Hrs)" : ap.Duration;
            string visitorType = ap.Visitor_Type == null ? "Official" : ap.Visitor_Type;

            if (ap.ComanyID == null || ap.VisitorID == null)
            {
                TempData["V_C_notselect"] = "V_C_notselect";
            }
            else
            {

                string ddd = Convert.ToDateTime(DateTime.Now).ToShortDateString();
                string frdate = Convert.ToDateTime(ap.V_DateFrom).ToShortDateString();
                string valid = String.Join(",", Valid_at);
                tblAppoinment appoinment = new tblAppoinment();
                appoinment.A_Time = ap.A_Time;
                appoinment.A_Type = ap.A_Type;
                appoinment.ComanyID = ap.ComanyID;
                appoinment.Deptment = Convert.ToInt32(deptID);
                appoinment.Duration = duration;
                appoinment.Valid_at = validatnew;//valid;
                appoinment.EntryType = ap.EntryType;
                appoinment.Location = vis_AllowLocation;
                appoinment.RaiseDate = DateTime.Now;
                appoinment.V_DateTo = Convert.ToDateTime(frdate);//Convert.ToDateTime(ddd);
                // appoinment.Remark = ap.Remark;
                string remark = ap.Remark == null ? "" : ap.Remark;
                appoinment.Remark = remark;

                appoinment.Requestor = ap.Requestor;
                //appoinment.VisitorID = ap.VisitorID;
                appoinment.VisitorID = V_ID;

                appoinment.Visitor_Type = visitorType;
                appoinment.V_Allowed = ap.V_Allowed;
                //appoinment.V_DateFrom = ap.V_DateFrom;
                appoinment.V_DateFrom = Convert.ToDateTime(frdate); //Convert.ToDateTime(ddd);
                appoinment.Flag = "False";
                if (ap.EntryType == "2")
                {
                    var a = Request.Form["V_DateFromM"]; var b = Request.Form["V_DateToM"];
                    var c = Request.Form["A_TimeM"];
                    //appoinment.V_DateFrom = Convert.ToDateTime(a);
                    //appoinment.V_DateTo = Convert.ToDateTime(ddd);
                    appoinment.V_DateFrom = Convert.ToDateTime(ap.V_DateFrom);
                    appoinment.V_DateTo = Convert.ToDateTime(ap.V_DateFrom);



                    appoinment.A_Time = c.ToString();
                }
                else
                {
                    appoinment.V_DateTo = Convert.ToDateTime(frdate);
                }
                appoinment.V_Location = ap.V_Location;
                appoinment.V_Status = ap.A_Type;
                //-------31 oct------
                // appoinment.VehicleNo = ap.VehicleNo;
                appoinment.VehicleNo = ap.VehicleNo == null ? "NA" : ap.VehicleNo;
                appoinment.RegionId = RegionId;
                //-----------
                db.tblAppoinments.Add(appoinment);
                db.SaveChanges();

                string ch = "";
                switch (appoinment.A_ID)
                {
                    case int n when n <= 9:
                        ch = ch + "000" + appoinment.A_ID;
                        break;
                    case int n when n >= 10 && n <= 99:
                        ch = ch + "00" + appoinment.A_ID;
                        break;
                    case int n when n > 100 && n <= 999:
                        ch = "0" + appoinment.A_ID;
                        break;
                    default:
                        ch = ch + appoinment.A_ID;
                        break;
                }

                var lastData = db.tblAppoinments.Where(m => m.A_ID == appoinment.A_ID).FirstOrDefault();
                lastData.GPID = value1 + ch;
                db.Entry(lastData).State = EntityState.Modified;
                db.SaveChanges();

                var com = db.tblCompanies.Where(m => m.C_ID == lastData.ComanyID).FirstOrDefault();

                string chkT = Request.Form["chkselectedmaterial_T"];
                string chkL = Request.Form["chkselectedmaterial_L"];
                string chkMD = Request.Form["chkselectedmaterial_MD"];
                string chkSD = Request.Form["chkselectedmaterial_SD"];

                if (chkL == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_L"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(lastData.A_ID);
                    material.M_Name = "Laptop";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }
                if (chkT == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_T"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(lastData.A_ID);
                    material.M_Name = "Tool Box";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }
                if (chkMD == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_MD"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(lastData.A_ID);
                    material.M_Name = "Media Device";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }
                if (chkSD == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_SD"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(lastData.A_ID);
                    material.M_Name = "Storage Device";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }

                var vData = db.tblVisitors.Where(m => m.V_ID == lastData.VisitorID).FirstOrDefault();
                var msgtemp = db.Msgtemplates.Where(m => m.Action == "AppointmentSMS" && m.Region == "PNI").FirstOrDefault();
                //-------------feb17-----------------
                //string AppointmntTemp = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy")).Replace("@p3", lastData.A_Time + ".").Replace("@p4", lastData.GPID).Replace("@p5", lastData.GPID);


                //layer.SendSMS(vData.V_Phone, AppointmntTemp, "1007161968597981063", "KRMINF");//     comment 17octn  comment Oct25

                string link = "https://vms.karam.in";
                string Appurl = ConfigurationManager.AppSettings["Appurl"].ToString();
                // string AppointmntTemp = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy")).Replace("@p3", lastData.A_Time + ".").Replace("@p4", lastData.GPID).Replace("@p5", lastData.GPID).Replace("@p6", link); working
                //string AppointmntTemp = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy") + "-").Replace("@p3", lastData.A_Time + ".").Replace("@p4", lastData.GPID).Replace("@p5", lastData.GPID).Replace("@p6", link);//with dash
                string AppointmntTemp = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy") + "-").Replace("@p3", lastData.A_Time + "").Replace("@p4", lastData.GPID).Replace("@p5", lastData.GPID).Replace("@p6", Appurl);//with dash
                layer.SendSMS(vData.V_Phone, AppointmntTemp, "1007167628437266921", "KRMINF");
                //Added whats msg 27-02-25
                try
                {
                    string str1 = CompanyMaster;
                    string str2 = lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy");
                    string str3 = lastData.A_Time;
                    string str4 = Appurl;// "https://vms.karam.in";
                    string str5 = lastData.GPID;
                    string str6 = lastData.GPID;
                    layer.SendWhatsMessage(vData.V_Phone, str1, str2, str3, str4, str5, str6);// 
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                //--------------

                string urls = "https://vms.karam.in/Home/SG/" + lastData.GPID;


                string urlimage = "https://vms.karam.in/Content/images/StringImage/" + lastData.GPID + ".jpg";


                TempData["create"] = "create";

                var write = new BarcodeWriter();
                write.Format = BarcodeFormat.QR_CODE;
                var result = write.Write(lastData.GPID);
                Bitmap bitmap = new Bitmap(result);
                write.Options = new EncodingOptions()
                {
                    Height = 80,
                    Width = 100
                };
                bitmap.SetResolution(300, 300);

                string path = Server.MapPath("~/Content/images/QR/" + lastData.GPID + ".png");

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        bitmap.Save(memory, ImageFormat.Png);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }

                var report = new ActionAsImage("Contact", new { id = lastData.GPID, path = path })
                {
                    PageWidth = 370,
                    FileName = "MyImage.jpeg",
                };

                string fullPath = Server.MapPath("../Content/images/StringImage/" + lastData.GPID + ".jpg");

                var binary = report.BuildFile(this.ControllerContext);
                var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                fileStream.Write(binary, 0, binary.Length);
                fileStream.Close();

                layer.WhatsAppAttachment(vData.V_Phone.ToString(), lastData.V_DateFrom.Value.ToString("dd MMM"), CompanyMaster, urlimage);//urlimage fullPath

            }
        }
        public void Create_ePass(string ID)
        {
            var write = new BarcodeWriter();
            write.Format = BarcodeFormat.QR_CODE;
            var result = write.Write(ID);
            Bitmap bitmap = new Bitmap(result);
            write.Options = new EncodingOptions()
            {
                Height = 80,
                Width = 100
            };
            bitmap.SetResolution(300, 300);

            string path = Server.MapPath("~/QRCode/" + ID + ".png");

            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }

            var report = new ActionAsImage("Contact", new { id = ID, path = path })
            {
                PageWidth = 370,
                FileName = "MyImage.jpeg",
            };

            string fullPath = Server.MapPath("~/StringImage/" + ID + ".jpg");

            var binary = report.BuildFile(this.ControllerContext);
            var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            fileStream.Write(binary, 0, binary.Length);
            fileStream.Close();
        }

        [HttpPost]
        public ActionResult CancelEntry(string delHidden)
        {
            if (Session["NAME"] != null)
            {
                var req = Session["NAME"]; string region = Session["SITE"].ToString();
                string[] KEY = delHidden.Split(';');
                foreach (var key in KEY)
                {
                    if (key != "")
                    {
                        int chlnKey = Convert.ToInt32(key);
                        var data = db.tblAppoinments.Where(m => m.A_ID == chlnKey && m.V_Status == "Pre").FirstOrDefault();
                        if (data != null)
                        {
                            var requestor = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.Requestor).FirstOrDefault();
                            var vData = db.tblVisitors.Where(m => m.V_ID == data.VisitorID).FirstOrDefault();
                            var com = db.tblCompanies.Where(m => m.C_ID == data.ComanyID).FirstOrDefault();
                            string mastercompany = login.MasterCompanies.Where(m => m.MC_ID == requestor.CompanyID).FirstOrDefault().CompanyName;

                            //var msg = db.Msgtemplates.Where(m => m.Action == "Cancel" & m.Region == region).FirstOrDefault();
                            var msg = db.Msgtemplates.Where(m => m.Action == "Cancel" & m.Region == "PNI").FirstOrDefault();
                            data.V_Status = "Cancel";
                            //--------------
                            string CancelkRemark = Request.Form["CancelkRemark"].ToString();
                            data.RejectRemark = CancelkRemark;
                            //------------------
                            data.Flag = "False";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();

                            //var CancelMsg = db.Msgtemplates.Where(m => m.Region == region && m.Action == "CancelVisitor").FirstOrDefault();
                            var CancelMsg = db.Msgtemplates.Where(m => m.Region == "PNI" && m.Action == "CancelVisitor").FirstOrDefault();
                            string Cancel_msgBody = CancelMsg.Template.Replace("@p1 ", mastercompany + " ").Replace("@p2", data.V_DateFrom.Value.ToString("dd/MMM/yyyy")).Replace("@p3", data.A_Time);

                            layer.SendSMS(vData.V_Phone, Cancel_msgBody, "1007161949733216155", "KRMITI");
                            layer.WhatsApp(vData.V_Phone, "enx_cancel_appointment", mastercompany, data.V_DateFrom.Value.ToString("dd MMM"), data.A_Time);
                        }
                    }
                }
                return RedirectToAction("Appointment");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult SingleDayPass()
        {
            if (Session["NAME"] != null)
            {
                bool allowYN = PageRoletype("S");
                if (allowYN == false)
                {
                    return RedirectToAction("../Home/Login");
                }


                RemoveCache();
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                string location = Session["LOCATION"].ToString();
                string site = Session["SITE"].ToString();
                var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();

                //ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();

                ViewData["hce"] = 0;
                //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == dataloc.DeviceNameWrkr).Select(z => z.paycode).Count();
                ViewData["hcw"] = 0;
                ViewData["hcv"] = db.sp_HCV(location).Where(m=>m.RegionId==RegionId).Select(z => z.Visitor).Count();
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
      
        public ActionResult ScanView(string scanText)
        {
           
            RemoveCache();


            int RegionId = Convert.ToInt32(Session["RegionId"]);
            if (Session["NAME"] != null)
            {
                Session.LCID = 1033;
                string location = Session["LOCATION"].ToString();
                string site = Session["SITE"].ToString();
                var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();

               // ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();
                //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == dataloc.DeviceNameWrkr).Select(z => z.paycode).Count();
                ViewData["hcw"] = 0;

                ViewData["hce"] = 0;

                ViewData["hcv"] = db.sp_HCV(location).Where(m => m.RegionId == RegionId).Select(z => z.Visitor).Count();
             

                var stext = scanText.ToUpper().Trim();
                if (stext.StartsWith("R") || stext.StartsWith("N"))
                {

                    //if (db.tblIN_Out.Any(m => m.OutFrom == location && m.M_Status == "Open" && m.ChallanNmbr == stext) == true)
                    //{
                    //    return RedirectToAction("OutWardView", "Material", new { challan = stext });
                    //}
                    //else
                    //{
                    //    return RedirectToAction("OutWardIN", "Material", new { challan = stext });
                    //}
                    return RedirectToAction("OutWardIN", "Material", new { challan = stext });
                }
                else if (stext.StartsWith("W"))
                {
                    return RedirectToAction("SelfAppointment");
                }
                else
                {
                    SecurityModel security = new SecurityModel();

                    var datacheck = db.tblAppoinments.Where(m => m.GPID == stext && m.RegionId == RegionId).FirstOrDefault();


                    if (datacheck == null)
                    {
                        TempData["GPNotFount"] = "GPNotFount";
                        return RedirectToAction("Security");
                    }
                    if (db.tblAppoinments.Any(m => m.GPID == stext) == true)
                      //if (db.tblAppoinments.Any(m => m.GPID == stext) == true)
                    {
                        security.A = db.tblAppoinments.Where(m => m.GPID == stext).FirstOrDefault();
                        var de = login.Departments.Where(m => m.D_ID == security.A.Deptment).FirstOrDefault().DepartmentName;
                        ViewBag.Dept = de;
                        security.V = db.tblVisitors.Where(m => m.V_ID == security.A.VisitorID).FirstOrDefault();
                        security.C = db.tblCompanies.Where(m => m.C_ID == security.A.ComanyID).FirstOrDefault();
                        security.M = db.tblMaterials.Where(m => m.App_ID == security.A.A_ID).ToList();
                        //----------
                        var datapass = db.tblPasses.Where(m => m.GatePassNumber == stext && m.RegionId==RegionId).FirstOrDefault();
                        if (datapass != null)
                        {
                            @ViewBag.pass = datapass.PassNo;
                        }
                        else
                        {

                            @ViewBag.pass = "";
                        }
                       

                        if (db.tblVisitorINOUTs.Any(m => m.GPID == security.A.A_ID.ToString() && m.VL_Status == "IN" && m.Location != location) == true)
                        {
                            ViewBag.VisitorAtOtherLocation = "True";
                        }
                        if (security.A.A_Type == "Pre")
                        {
                            ViewBag.Accecpt = db.VisitorAcceptances.Any(m => m.GatePassID == security.A.GPID) == true ? "Accecpted" : "NotAccept";
                        }
                        else
                        {
                            ViewBag.Accecpt = "NA";
                        }
                        ViewBag.Req = login.Master_Requestor.Where(m=>m.RegionId== RegionId).ToList();
                        return View(security);
                    }
                    else
                    {
                        TempData["GPNotFount"] = "GPNotFount";
                        return RedirectToAction("Security");
                    }
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult ViewAppointment(string GPID)
        {
            if (Session["NAME"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);

                string location = Session["LOCATION"].ToString();
                string site = Session["SITE"].ToString();

                var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();

               // ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();
                //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == dataloc.DeviceNameWrkr).Select(z => z.paycode).Count();
                ViewData["hce"] = 0;
                ViewData["hcw"] = 0;
                ViewData["hcv"] = db.sp_HCV(location).Where(m=>m.RegionId==RegionId).Select(z => z.Visitor).Count();
                var data = db.sp_ViewApp(GPID).FirstOrDefault();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Security()
        {

            //testmail();

            //try { 

          
            


            if (Session["NAME"] != null)
            {
                //bool allowYN = PageRoletype("S");
                ////bool allowYNAdmin = PageRoletype("A");
                //if (allowYN == false)
                //{
                //    return RedirectToAction("Login");
                //}

                int RegionId = Convert.ToInt32(Session["RegionId"]);
                string site = Session["SITE"].ToString();
                string location = Session["LOCATION"].ToString();

                ViewBag.OpenCourierCount = db.tblCouriars.Where(m => m.CurrentStatus == "Open" && m.CouriarType == "Inward" && m.RegionId==RegionId).ToList().Count;
                ViewBag.IntrasitCourierCount = db.tblCouriars.Where(m => m.CurrentStatus == "Intrasit" && m.CouriarType == "Inward" && m.RegionId == RegionId).ToList().Count;

                ViewBag.InwardCourierCount = db.tblCouriars.Where(m => m.CouriarType == "Inward" && m.RegionId == RegionId).ToList().Count;
                ViewBag.OutwardCourierCount = db.tblCouriars.Where(m => m.CouriarType == "Outward" && m.RegionId == RegionId).ToList().Count;
                 var data = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();
                ViewBag.Upcoming = db.sp_AppUpcoming().Where(m => m.Valid_at.Contains(location) && m.Regionid == RegionId).ToList().Count();
                try {
                    ViewBag.Today = db.sp_SecurityToday().Where(m => m.Valid_at.Contains(location)).ToList().Count();
                    //ViewBag.Today = db.Proc_SecurityToday().ToList().Count;//.Where(m => m.Valid_at.Contains(location)).ToList().Count() //db.sp_SecurityToday().Where(m => m.Valid_at.Contains(location)).ToList().Count();
                }
                catch(Exception ex) { ex.Message.ToString(); }
                
                //ViewData["hcv"] = db.sp_HCV(location).Select(z => z.Visitor).Count();//5june 2023

                ViewData["hce"] = 0;
                ViewData["hcw"] = 0;
                // ViewData["hcv"] = 0;

              
              //  var today = db.sp_SecurityToday().Where(m => m.Valid_at.Contains(location) && m.Regionid == RegionId).ToList().Count();
                
            


                int totalApp = db.tblAppoinments.Where(m=>m.RegionId== RegionId).ToList().Count();
                ViewBag.TotalApp = totalApp;//Convert.ToInt32(ViewBag.Today) + Convert.ToInt32(ViewBag.Upcoming);

             
                CommonViewModel model = new CommonViewModel();
                model.Appointment = layer.RecentVisitorSecurity;
               
                ViewBag.Req = login.Master_Requestor.Where(m=>m.RegionId== RegionId).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
            //}
            // catch (Exception ex)
            // {
            //     ex.Message.ToString();
            //    // return RedirectToAction("Login");
            // }

        }
        [HttpPost]

        public ActionResult Question(int ID, float Q1, string Q2, string Q3, string Q4)
        {

            //---------Addedfeb09--------
            string id = "";
            if (Session["PROFILE"] != null)
            {
                 id = Session["PROFILE"].ToString();
            }
            DateTime dd = DateTime.Now;
            string ss = id + "" + dd.Day + "" + dd.Month + "" + dd.Year + "" + dd.Hour + "" + dd.Minute + "" + dd.Second;
            //-----------------
            string res = "";
            tblQuestion question = new tblQuestion();
            question.GP_ID = ID;
            question.Q1 = Q1.ToString();
            question.Q2 = Q2;
            question.Q3 = Q3;
            question.Q4 = Q4;
            //----Added feb9----
            question.SessionTime = ss;
            Session["SecirutyINTime"] = ss;
            //-------
            if (Q1 < 99 && Q2 == "No" && Q3 == "Yes" && Q4 == "No")
            {
                res = "Success";
            }
            else
            {
                res = "Exception";
            }
            db.tblQuestions.Add(question);
            db.SaveChanges();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckMobileNum(string num)
        {
            int RegionId = Convert.ToInt32(Session["RegionId"]);
            var data = db.tblVisitors.Any(z => z.V_Phone == num && z.RegionId== RegionId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SecurityAction(int GPID, string imgHidden, string[] visitorItemName, string[] visitorItem)
        {
            Session.LCID = 1033;

            if (Session["NAME"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);


                var data = db.tblAppoinments.Where(m => m.A_ID == GPID).FirstOrDefault();

                    //if (db.tblQuestions.Any(m => m.GP_ID == GPID) == true)//April12  enable when covid
                    //{
                    string location = Session["LOCATION"].ToString();
                    string region = Session["SITE"].ToString();

                    int Person = Convert.ToInt32(Request.Form["PersonAccompanyingS"]);
                    string VT = Request.Form["VisitTypeIDS"].ToString();
                    string ML = Request.Form["MeetingLocationIDS"].ToString();
                    int VID = Convert.ToInt32(Request.Form["VistID_S"]);
                    string vID = Request.Form["Visitor_ID"].ToString().ToUpper();
                    string IDN = Request.Form["V_IDNumber"].ToString().ToUpper();
                    string pass = Request.Form["txtPass"].ToString().ToUpper();
                    string Exceptionremark = Request.Form["ExceptionRemarkRemark"].ToString();//added dec2
                    data.V_Status = "Arrived";
                    data.Visitor_Type = VT;
                    data.Location = ML;
                    data.V_InDate = DateTime.Now;
                    data.Flag = VT == "Canteen" ? "True" : "False";
                    data.RejectRemark = Exceptionremark;//added dec2

                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();

                    tblVisitorINOUT tbl = new tblVisitorINOUT();
                    tbl.GPID = GPID.ToString();
                    tbl.FDate = data.V_InDate;
                    tbl.VL_Status = "IN";
                    tbl.Location = location;
                    db.tblVisitorINOUTs.Add(tbl);
                    db.SaveChanges();




                    var VistorData = db.tblVisitors.Where(m => m.V_ID == data.VisitorID).FirstOrDefault();

                    if (imgHidden != "")
                    {
                        TrumpService.SaveCapturedImage(imgHidden, data.VisitorID.ToString());
                        VistorData.V_Pic = data.VisitorID + ".jpg";
                    }

                    VistorData.Visitor_ID = vID;
                    VistorData.V_IDNumber = IDN;
                    db.Entry(VistorData).State = EntityState.Modified;
                    db.SaveChanges();
                    var host = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.Requestor).FirstOrDefault();
                    //var temp = db.Msgtemplates.Where(m => m.Region == region && m.Action == "Arrived").FirstOrDefault();

                    string chkT = Request.Form["chkTB"];
                    string chkL = Request.Form["chkLP"];
                    string chkMD = Request.Form["chkMD"];
                    string chkSD = Request.Form["chkSD"];

                    if (chkL == "on")
                    {
                        tblMaterial material = new tblMaterial();
                        string sL = Request.Form["txtLP"].ToString().ToUpper();
                        material.App_ID = Convert.ToInt32(data.A_ID);
                        material.M_Name = "Laptop";
                        material.Serial_No = sL;
                        db.tblMaterials.Add(material);
                        db.SaveChanges();
                    }
                    if (chkT == "on")
                    {
                        tblMaterial material = new tblMaterial();
                        string sL = Request.Form["txtTB"].ToString().ToUpper();
                        material.App_ID = Convert.ToInt32(data.A_ID);
                        material.M_Name = "Tool Box";
                        material.Serial_No = sL;
                        db.tblMaterials.Add(material);
                        db.SaveChanges();
                    }
                    if (chkMD == "on")
                    {
                        tblMaterial material = new tblMaterial();
                        string sL = Request.Form["txtMD"].ToString().ToUpper();
                        material.App_ID = Convert.ToInt32(data.A_ID);
                        material.M_Name = "Media Device";
                        material.Serial_No = sL;
                        db.tblMaterials.Add(material);
                        db.SaveChanges();
                    }
                    if (chkSD == "on")
                    {
                        tblMaterial material = new tblMaterial();
                        string sL = Request.Form["txtSD"].ToString().ToUpper();
                        material.App_ID = Convert.ToInt32(data.A_ID);
                        material.M_Name = "Storage Device";
                        material.Serial_No = sL;
                        db.tblMaterials.Add(material);
                        db.SaveChanges();
                    }

                    if (visitorItemName != null)
                    {
                        for (int i = 0; i < visitorItemName.Length; i++)
                        {
                            var material = db.tblMaterials.Where(m => m.App_ID == data.A_ID).ToList();
                            if (material != null)
                            {
                                string itrm = visitorItemName[i];
                                var itm = db.tblMaterials.Where(m => m.M_Name == itrm && m.App_ID == data.A_ID).FirstOrDefault();
                                itm.Serial_No = visitorItem[i];
                                db.Entry(itm).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }

                    if (VT != "Canteen")
                    {
                        //var Arivl = db.Msgtemplates.Where(m => m.Region == region && m.Action == "Arrived").FirstOrDefault();
                        var Arivl = db.Msgtemplates.Where(m => m.Region == "PNI" && m.Action == "Arrived").FirstOrDefault();

                        string ArrivalSMS = Arivl.Template.Replace("@p1", host.NAME + " ").Replace("@p2", " " + VistorData.V_Name).Replace("@p3", location);
                        ///----------tesing---
                        ///layer.SendSMS(vData.V_Phone, AppointmntTemp, "1007161968597981063", "KRMINF")
                        /// layer.ArrivalSendSMS("8354048225", ArrivalSMS, "1007161949720682356", "KRMITI"); 

                        /// 
                        ///-------testing-------

                        layer.SendSMS(host.MOBILE_NUMBER.ToString(), ArrivalSMS, "1007161949720682356", "KRMITI"); //comment Oct25
                    //---------
                    //Added whats msg 04-07-25
                    try
                    {
                        string str1 = host.NAME;
                        string str2 = VistorData.V_Name;
                        string str3 = location;
                        layer.SendWhatsMessageEmployee(host.MOBILE_NUMBER.ToString(), str1, str2, str3);// 
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    //-----------------------------
                    //--------

                    string CompanyMaster = login.MasterCompanies.Where(m => m.MC_ID == host.CompanyID).FirstOrDefault().CompanyName;
                        layer.WhatsApp_With(host.MOBILE_NUMBER.ToString(), "enx_visitor_in"); //comment Oct25
                    }


                    try
                    {
                        string gatepass = data.GPID;
                        tblPass tblp2 = new tblPass();
                        tblp2.PassNo = pass;
                        tblp2.Status = "IN";
                        tblp2.GatePassNumber = gatepass;
                        tblp2.Transdate = DateTime.Now;
                        tblp2.RegionId = RegionId;
                        db.tblPasses.Add(tblp2);
                        db.SaveChanges();
                    }
                    catch (Exception ex) { ex.Message.ToString(); }

                    //---feb9---------
                    try
                    {
                        //    string ss = Session["SecirutyINTime"].ToString();
                        //tblQuestion qu = db.tblQuestions.Where(z => z.SessionTime == ss).FirstOrDefault();

                        //qu.GP_ID = GPID;
                        //db.Entry(qu).State = EntityState.Modified;
                        //db.SaveChanges();
                        //Session.Remove("SecirutyINTime");

                        VisitorAcceptance vs = new VisitorAcceptance();
                        vs.VisitorID = data.VisitorID;
                        vs.GatePassID = data.GPID;
                        vs.AccptGate = DateTime.Now;
                        db.VisitorAcceptances.Add(vs);
                        db.SaveChanges();
                    }
                    catch (Exception ex) { ex.Message.ToString(); }
                    //-------------


                    



                return RedirectToAction("Security");
                    //}
                    //else
                    //{
                    //    TempData["notAns"] = "NOTANSWER";
                    //    return RedirectToAction("ScanView", new { scanText = data.GPID });
                    //}
               
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
      
        public ActionResult Security(Appointment ap, string imgHidden, List<string> Valid_at, string[] cvName, string[] cvIdType, string[] cvIdNumber, string[] cvMobile)

        {
            if (Session["NAME"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                string s = Session["NAME"].ToString();
                string validatnew = Session["LOCATION"].ToString();
                //string validatnew=

                //****18n0v**************
                //------------15may----------------
                string strgenerate = Request.Form["txtgenno"].ToString();
                int checkduplicate = layer.checkduplicatecount(strgenerate);
                if (checkduplicate == 0)
                {
                 
                //-----15may------------

                int VisitorId = Convert.ToInt32(ap.VisitorID);
                functionCreateVisitorforSecurity(ap, Valid_at, VisitorId, imgHidden, RegionId, validatnew);

                string VisitorType = "";
                var visitordata = db.tblVisitors.Where(m => m.V_ID == ap.VisitorID).FirstOrDefault();

                    //--change Aug 16 2023-- for visitor type identification-

                    visitordata.RegionId = 1;
                    db.Entry(visitordata).State = EntityState.Modified;
                    db.SaveChanges();
                    //----------


                    //---------
                    for (int i = 0; i < cvName.Length; i++)
                {


                    string strName = cvName[i];
                    string bb = cvIdType[i];
                    string cc = cvIdNumber[i];
                    string dd = cvMobile[i];
                    VisitorType = "CoVisitor";
                    if (strName.Trim() != "")
                    {
                        tblVisitor obj = new tblVisitor();
                        obj.V_Name = cvName[i];
                        obj.V_Phone = cvMobile[i];
                        obj.V_Email = "";
                        obj.Visitor_ID = cvIdType[i];
                        obj.V_IDNumber = cvIdNumber[i];
                        obj.V_Pic = "";
                        obj.V_Status = visitordata.V_Status;
                        obj.V_Type = VisitorType;
                        obj.C_ID = visitordata.C_ID;
                            obj.RegionId = 2;
                            db.tblVisitors.Add(obj);
                        db.SaveChanges();
                        int V_ID = obj.V_ID;
                        functionCreateVisitorforSecurity(ap, Valid_at, V_ID, "", RegionId, validatnew);


                    }


                }




                    layer.preventduplicateInsert(strgenerate);
                }


                //*******end 18nov*****************
                //-----------------Previous code---------------
                //string location = Session["LOCATION"].ToString();
                //string dept = Session["SITE"].ToString();
                //string[] date = DateTime.Now.ToString().Split('/');
                //string[] yr = date[2].Split(' '); string[] tm = yr[1].Split(':');

                //string name = Request.Form["host"];
                //string r = date[0].ToString().TrimStart('0');
                //string h = tm[0].ToString().TrimStart('0');

                //string value1 = "V" + date[1] + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(r)) + yr[0].Substring(2) + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(h));

                //string vis_AllowLocation = ap.Location == null ? "Green" : ap.Location;
                //string duration = ap.Duration == null ? "0-2(Hrs)" : ap.Duration;
                //string visitorType = ap.Visitor_Type == null ? "Official" : ap.Visitor_Type;

                //if (ap.VisitorID == 0)
                //{
                //    TempData["V_notselect_security"] = "V_notselect_security";
                //}
                //else
                //{
                //    var dd = db.tblVisitors.Where(z => z.V_ID == ap.VisitorID).FirstOrDefault();

                //    string valid = String.Join(",", Valid_at);
                //    tblAppoinment tblAppoinment = new tblAppoinment();
                //    tblAppoinment.Requestor = ap.Requestor;
                //    tblAppoinment.Deptment = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == ap.Requestor).FirstOrDefault().DepartmentID;
                //    tblAppoinment.ComanyID = dd.C_ID;
                //    tblAppoinment.VisitorID = ap.VisitorID;
                //    tblAppoinment.Visitor_Type = visitorType;
                //    tblAppoinment.V_DateFrom = DateTime.Today;
                //    tblAppoinment.V_DateTo = DateTime.Today;
                //    tblAppoinment.A_Time = ap.A_Time;
                //    tblAppoinment.Valid_at = valid;
                //    tblAppoinment.Duration = duration;
                //    tblAppoinment.V_Location = ap.V_Location;
                //    tblAppoinment.Remark = ap.Remark;
                //    tblAppoinment.V_Allowed = "false";
                //    tblAppoinment.A_Type = "Request";
                //    tblAppoinment.RaiseDate = DateTime.Now;
                //    tblAppoinment.Location = vis_AllowLocation;
                //    tblAppoinment.V_Status = "Arrived";
                //    tblAppoinment.EntryType = "1";
                //    tblAppoinment.V_InDate = DateTime.Now;
                //    tblAppoinment.Flag = visitorType == "Canteen" ? "True" : "False";
                //    db.tblAppoinments.Add(tblAppoinment);
                //    db.SaveChanges();

                //    dd.Visitor_ID = Request.Form["Visitor_ID"].ToString().ToUpper();
                //    dd.V_IDNumber = Request.Form["V_IDNumber"].ToString().ToUpper();
                //    if (imgHidden != "")
                //    {
                //        TrumpService.SaveCapturedImage(imgHidden, ap.VisitorID.ToString());
                //        dd.V_Pic = ap.VisitorID + ".jpg";
                //    }
                //    db.Entry(dd).State = EntityState.Modified;
                //    db.SaveChanges();

                //    string ch = "";
                //    switch (tblAppoinment.A_ID)
                //    {
                //        case int n when n <= 9:
                //            ch = ch + "000" + tblAppoinment.A_ID;
                //            break;
                //        case int n when n >= 10 && n <= 99:
                //            ch = ch + "00" + tblAppoinment.A_ID;
                //            break;
                //        case int n when n > 100 && n <= 999:
                //            ch = "0" + tblAppoinment.A_ID;
                //            break;
                //        default:
                //            ch = ch + tblAppoinment.A_ID;
                //            break;
                //    }
                //    var lastData = db.tblAppoinments.Where(m => m.A_ID == tblAppoinment.A_ID).FirstOrDefault();
                //    lastData.GPID = value1 + ch;
                //    db.Entry(lastData).State = EntityState.Modified;
                //    db.SaveChanges();

                //    string ss = Session["SecirutyINTime"].ToString();
                //    tblQuestion qu = db.tblQuestions.Where(z => z.SessionTime == ss).FirstOrDefault();
                //    qu.GP_ID = tblAppoinment.A_ID;
                //    db.Entry(qu).State = EntityState.Modified;
                //    db.SaveChanges();
                //    Session.Remove("SecirutyINTime");

                //    VisitorAcceptance vs = new VisitorAcceptance();
                //    vs.VisitorID = lastData.VisitorID;
                //    vs.GatePassID = lastData.GPID;
                //    vs.AccptGate = DateTime.Now;
                //    db.VisitorAcceptances.Add(vs);
                //    db.SaveChanges();

                //    tblVisitorINOUT visitorINOUT = new tblVisitorINOUT();
                //    visitorINOUT.Location = location;
                //    visitorINOUT.FDate = DateTime.Now;
                //    visitorINOUT.VL_Status = "IN";
                //    visitorINOUT.GPID = lastData.A_ID.ToString();
                //    db.tblVisitorINOUTs.Add(visitorINOUT);
                //    db.SaveChanges();

                //    string chkT = Request.Form["chkselectedmaterial_T"];
                //    string chkL = Request.Form["chkselectedmaterial_L"];
                //    string chkMD = Request.Form["chkselectedmaterial_MD"];
                //    string chkSD = Request.Form["chkselectedmaterial_SD"];
                //    if (chkL == "on")
                //    {
                //        tblMaterial material = new tblMaterial();
                //        string sL = Request.Form["Serial_No_L"].ToString().ToUpper();
                //        material.App_ID = Convert.ToInt32(tblAppoinment.A_ID);
                //        material.M_Name = "Laptop";
                //        material.Serial_No = sL;
                //        db.tblMaterials.Add(material);
                //        db.SaveChanges();
                //    }
                //    if (chkT == "on")
                //    {
                //        tblMaterial material = new tblMaterial();
                //        string sL = Request.Form["Serial_No_T"].ToString().ToUpper();
                //        material.App_ID = Convert.ToInt32(tblAppoinment.A_ID);
                //        material.M_Name = "Tool Box";
                //        material.Serial_No = sL;
                //        db.tblMaterials.Add(material);
                //        db.SaveChanges();
                //    }
                //    if (chkMD == "on")
                //    {
                //        tblMaterial material = new tblMaterial();
                //        string sL = Request.Form["Serial_No_MD"].ToString().ToUpper();
                //        material.App_ID = Convert.ToInt32(tblAppoinment.A_ID);
                //        material.M_Name = "Media Device";
                //        material.Serial_No = sL;
                //        db.tblMaterials.Add(material);
                //        db.SaveChanges();
                //    }
                //    if (chkSD == "on")
                //    {
                //        tblMaterial material = new tblMaterial();
                //        string sL = Request.Form["Serial_No_SD"].ToString().ToUpper();
                //        material.App_ID = Convert.ToInt32(tblAppoinment.A_ID);
                //        material.M_Name = "Storage Device";
                //        material.Serial_No = sL;
                //        db.tblMaterials.Add(material);
                //        db.SaveChanges();
                //    }

                //    var req = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == ap.Requestor).FirstOrDefault();

                //    string mastercompany = login.MasterCompanies.Where(m => m.MC_ID == req.CompanyID).FirstOrDefault().CompanyName;

                //    string SecurityAppointmnt = "Dear " + dd.V_Name + " Welcome to one of the leading PPE manufacturing unit " + mastercompany + " Please follow our guidelines board and click the below link to ACCEPT Safety Guidelines and generate ePass " + "https://vms.karam.in/Home/SG/" + lastData.GPID + " KARAM";

                //    if (visitorType != "Canteen")
                //    {
                //        string nmbr = dd.V_Phone;
                //        layer.SendSMS(nmbr, SecurityAppointmnt, "1007161968544985713", "KRMINF");
                //        //layer.WhatsApp(nmbr, d);
                //    }

                //    var write = new BarcodeWriter();
                //    write.Format = BarcodeFormat.QR_CODE;
                //    var result = write.Write(lastData.GPID);
                //    Bitmap bitmap = new Bitmap(result);
                //    write.Options = new EncodingOptions()
                //    {
                //        Height = 80,
                //        Width = 100
                //    };
                //    bitmap.SetResolution(300, 300);

                //    string path = Server.MapPath("~/Content/images/QR/" + lastData.GPID + ".png");
                //    using (MemoryStream memory = new MemoryStream())
                //    {
                //        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                //        {
                //            bitmap.Save(memory, ImageFormat.Png);
                //            byte[] bytes = memory.ToArray();
                //            fs.Write(bytes, 0, bytes.Length);
                //        }
                //    }

                //    var report = new Rotativa.ActionAsImage("Contact", new { id = lastData.GPID, path = path })
                //    {
                //        PageWidth = 370,
                //        FileName = "MyImage.jpeg",
                //    };
                //    string fullPath = Server.MapPath("../Content/images/StringImage/" + lastData.GPID + ".jpg");

                //    var binary = report.BuildFile(this.ControllerContext);
                //    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                //    fileStream.Write(binary, 0, binary.Length);
                //    fileStream.Close();

                //    TempData["single"] = "single";
                //}
                //---------------------------------------------

                if (s == "ADMIN")
                {
                    TempData["createSingleDayPass"] = "createSingleDayPassAdmin";
                    return RedirectToAction("ADMIN");

                }
                else
                {

                    TempData["createSingleDayPass"] = "createSingleDayPass";
                    return Redirect(Request.UrlReferrer.ToString());
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        private void functionCreateVisitorforSecurity(Appointment ap, List<string> Valid_at, int V_ID, string imgHidden,int RegionId, string validatnew)
        {
            //string[] dept = Session["PNICODE"].ToString().Split('_');
            //string deptID = Session["DEPTID"].ToString();
            string deptIDw = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            string[] date = deptIDw.Split('-');
            string[] yr = date[2].Split(' '); string[] tm = yr[1].Split(':');
            string r = date[0].ToString().TrimStart('0');
            string h = tm[0].ToString().TrimStart('0');

            string region = Session["SITE"].ToString();

            var req = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == ap.Requestor).FirstOrDefault();
            string CompanyMaster = login.MasterCompanies.Where(m => m.MC_ID == req.CompanyID).FirstOrDefault().CompanyName;

            //Generate ID
            string value1 = "V" + date[1] + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(r)) + yr[0].Substring(2) + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(h));

            //string vis_AllowLocation = ap.Location == null ? "Green" : ap.Location;//31oct
            string vis_AllowLocation = "Green";

            string duration = ap.Duration == null ? "0-2(Hrs)" : ap.Duration;
            string visitorType = ap.Visitor_Type == null ? "Official" : ap.Visitor_Type;

            if (ap.ComanyID == 0 || ap.VisitorID == 0)
            {
                TempData["V_C_notselect"] = "V_C_notselect";
            }
            else
            {

                //---------jan31 changed---------

              
                var VistorData = db.tblVisitors.Where(m => m.V_ID == ap.VisitorID).FirstOrDefault();

                if (imgHidden != "")
                {
                    string strapVisitorid = Convert.ToString(ap.VisitorID);
                    TrumpService.SaveCapturedImage(imgHidden, strapVisitorid);
                    VistorData.V_Pic = strapVisitorid + ".jpg";
                    db.Entry(VistorData).State = EntityState.Modified;
                    db.SaveChanges();

                }


                //---------jan 31---------------------


                string ddd = Convert.ToDateTime(DateTime.Now).ToShortDateString();
                string valid = String.Join(",", Valid_at);
                tblAppoinment appoinment = new tblAppoinment();
                appoinment.A_Time = ap.A_Time;

                appoinment.ComanyID = ap.ComanyID;
                appoinment.Deptment = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == ap.Requestor).FirstOrDefault().DepartmentID;//Convert.ToInt32(deptID);
                appoinment.Duration = duration;
                appoinment.Valid_at = validatnew;//valid;

                appoinment.EntryType = "1";
                appoinment.A_Type = "Pre";




                appoinment.Location = vis_AllowLocation;
                appoinment.RaiseDate = DateTime.Now;
                appoinment.V_DateTo = Convert.ToDateTime(ddd);
                //appoinment.Remark = ap.Remark;
                string remark = ap.Remark == null ? "" : ap.Remark;
                appoinment.Remark = remark;
                appoinment.Requestor = ap.Requestor;
                //appoinment.VisitorID = ap.VisitorID;
                appoinment.VisitorID = V_ID;

                appoinment.Visitor_Type = visitorType;
                appoinment.V_Allowed = ap.V_Allowed;
                //appoinment.V_DateFrom = ap.V_DateFrom;
                appoinment.V_DateFrom = Convert.ToDateTime(ddd);
                appoinment.Flag = "False";
                if (ap.EntryType == "2")
                {
                    var a = Request.Form["V_DateFromM"]; var b = Request.Form["V_DateToM"];
                    var c = Request.Form["A_TimeM"];
                    appoinment.V_DateFrom = Convert.ToDateTime(a);
                    appoinment.V_DateTo = Convert.ToDateTime(ddd);
                    appoinment.A_Time = c.ToString();
                }
                else
                {
                    appoinment.V_DateTo = Convert.ToDateTime(ddd);
                }
                appoinment.V_Location = ap.V_Location;
                //appoinment.V_Status = ap.A_Type;
                appoinment.V_Status = "Pre";



                appoinment.VehicleNo = ap.VehicleNo == null ? "NA" : ap.VehicleNo; //ap.VehicleNo;14 Aug
                appoinment.RegionId = RegionId;
                db.tblAppoinments.Add(appoinment);
                db.SaveChanges();

                string ch = "";
                switch (appoinment.A_ID)
                {
                    case int n when n <= 9:
                        ch = ch + "000" + appoinment.A_ID;
                        break;
                    case int n when n >= 10 && n <= 99:
                        ch = ch + "00" + appoinment.A_ID;
                        break;
                    case int n when n > 100 && n <= 999:
                        ch = "0" + appoinment.A_ID;
                        break;
                    default:
                        ch = ch + appoinment.A_ID;
                        break;
                }

                var lastData = db.tblAppoinments.Where(m => m.A_ID == appoinment.A_ID).FirstOrDefault();
                lastData.GPID = value1 + ch;
                db.Entry(lastData).State = EntityState.Modified;
                db.SaveChanges();

               


                var com = db.tblCompanies.Where(m => m.C_ID == lastData.ComanyID).FirstOrDefault();

                string chkT = Request.Form["chkselectedmaterial_T"];
                string chkL = Request.Form["chkselectedmaterial_L"];
                string chkMD = Request.Form["chkselectedmaterial_MD"];
                string chkSD = Request.Form["chkselectedmaterial_SD"];

                if (chkL == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_L"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(lastData.A_ID);
                    material.M_Name = "Laptop";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }
                if (chkT == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_T"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(lastData.A_ID);
                    material.M_Name = "Tool Box";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }
                if (chkMD == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_MD"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(lastData.A_ID);
                    material.M_Name = "Media Device";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }
                if (chkSD == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_SD"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(lastData.A_ID);
                    material.M_Name = "Storage Device";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }

                var vData = db.tblVisitors.Where(m => m.V_ID == lastData.VisitorID).FirstOrDefault();


                var msgtemp = db.Msgtemplates.Where(m => m.Action == "AppointmentSMS" && m.Region == "PNI").FirstOrDefault();
                //-----------17feb--------------
                //string AppointmntTemp = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy")).Replace("@p3", lastData.A_Time + ".").Replace("@p4", lastData.GPID).Replace("@p5", lastData.GPID);


                //layer.SendSMS(vData.V_Phone, AppointmntTemp, "1007161968597981063", "KRMINF");//     comment 17octn  comment Oct25
                string link = "https://vms.karam.in";
                string Appurl = ConfigurationManager.AppSettings["Appurl"].ToString();
                // string AppointmntTemp = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy")).Replace("@p3", lastData.A_Time + ".").Replace("@p4", lastData.GPID).Replace("@p5", lastData.GPID).Replace("@p6", link);//working
                //string AppointmntTemp = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy") + "-").Replace("@p3", lastData.A_Time + ".").Replace("@p4", lastData.GPID).Replace("@p5", lastData.GPID).Replace("@p6", link);//with dash

                string AppointmntTemp = msgtemp.Template.Replace("@p1", CompanyMaster).Replace("@p2", lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy") + "-").Replace("@p3", lastData.A_Time + "").Replace("@p4", lastData.GPID).Replace("@p5", lastData.GPID).Replace("@p6", Appurl);//with dash


                layer.SendSMS(vData.V_Phone, AppointmntTemp, "1007167628437266921", "KRMINF");// 

                //Added whats msg 27-02-25
                try
                {
                    string str1 = CompanyMaster;
                    string str2 = lastData.V_DateFrom.Value.ToString("dd-MMM-yyyy");
                    string str3 = lastData.A_Time;
                    string str4 = Appurl;// "https://vms.karam.in";
                    string str5 = lastData.GPID;
                    string str6 = lastData.GPID;
                    layer.SendWhatsMessage(vData.V_Phone, str1, str2, str3, str4, str5, str6);// 
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

                //-----------------------------



                string urls = "https://vms.karam.in/Home/SG/" + lastData.GPID;


                string urlimage = "https://vms.karam.in/Content/images/StringImage/" + lastData.GPID + ".jpg";


             

                var write = new BarcodeWriter();
                write.Format = BarcodeFormat.QR_CODE;
                var result = write.Write(lastData.GPID);
                Bitmap bitmap = new Bitmap(result);
                write.Options = new EncodingOptions()
                {
                    Height = 80,
                    Width = 100
                };
                bitmap.SetResolution(300, 300);

                string path = Server.MapPath("~/Content/images/QR/" + lastData.GPID + ".png");

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        bitmap.Save(memory, ImageFormat.Png);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }

                var report = new ActionAsImage("Contact", new { id = lastData.GPID, path = path })
                {
                    PageWidth = 370,
                    FileName = "MyImage.jpeg",
                };

                string fullPath = Server.MapPath("../Content/images/StringImage/" + lastData.GPID + ".jpg");

                var binary = report.BuildFile(this.ControllerContext);
                var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                fileStream.Write(binary, 0, binary.Length);
                fileStream.Close();

                layer.WhatsAppAttachment(vData.V_Phone.ToString(), lastData.V_DateFrom.Value.ToString("dd MMM"), CompanyMaster, urlimage);//urlimage fullPath

            }
        }
        //
        public ActionResult Reject_App(int ID)
        {
            var data = db.tblAppoinments.Where(m => m.A_ID == ID && m.V_Status == "Pre").FirstOrDefault();
            if (data != null)
            {
                data.V_Status = "Reject";
                data.Flag = "False";
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                var req = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.Requestor).FirstOrDefault();

                var masterCompany = login.MasterCompanies.Where(m => m.MC_ID == req.CompanyID).FirstOrDefault();

                var vData = db.tblVisitors.Where(m => m.V_ID == data.VisitorID).FirstOrDefault();
                var com = db.tblCompanies.Where(m => m.C_ID == data.ComanyID).FirstOrDefault();

                var rejectionAuthority = db.tblApprovals.Where(m => m.AppType == "OUTWORD" && m.Region == data.V_Location).FirstOrDefault();

                var msg = db.Msgtemplates.Where(m => m.Action == "Cancel").FirstOrDefault();
                var msgtemp = db.Msgtemplates.Where(m => m.Action == "CancelVisitor").FirstOrDefault();

                string CancelMsg = msgtemp.Template.Replace("@p1 ", masterCompany.CompanyName + " ").Replace("@p2", data.V_DateFrom.Value.ToString("dd/MMM/yyyy")).Replace("@p3", data.A_Time);

                string securityData = db.tblApprovals.Where(m => m.AppType == "SECURITY" && m.Region == data.V_Location).FirstOrDefault().Email;

                layer.SendSMS(vData.V_Phone, CancelMsg, "1007161949733216155", "KRMITI");
                layer.WhatsApp(vData.V_Phone, "enx_cancel_appointment", masterCompany.CompanyName, data.V_DateFrom.Value.ToString("dd MMM"), data.A_Time);

                TempData["reject"] = "reject";

                return RedirectToAction("Login");
            }
            else
            {
                TempData["allreject"] = "allreject";
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public JsonResult OutGPID(int GPID, string pass)
        {
            int RegionId = Convert.ToInt32(Session["RegionId"]);
            var data = db.tblAppoinments.Where(z => z.A_ID == GPID).FirstOrDefault();
            DateTime date = Convert.ToDateTime(data.V_InDate);
            //var tbl = db.tblVisitorINOUTs.Where(z => z.FDate.Value.Day == date.Day && z.FDate.Value.Hour == date.Hour && z.GPID == data.A_ID.ToString()).FirstOrDefault();
            var tbl = db.tblVisitorINOUTs.Where(z => z.GPID == data.A_ID.ToString()).ToList().LastOrDefault();
            if (tbl != null)
            {
                if (tbl.VL_Status == "IN")
                {
                    tbl.TDate = DateTime.Now;
                    tbl.VL_Status = "OUT";
                    db.Entry(tbl).State = EntityState.Modified;
                    db.SaveChanges();

                    //---------

                }
            }
            data.V_Status = "OUT";
            data.V_OutDate = DateTime.Now;
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
            //--------------
            //----Added 26 oct----------
            tblPass tblp = new tblPass();
           

            var datapass = db.tblPasses.Where(z => z.PassNo == pass && z.Status == "IN" && z.RegionId== RegionId).FirstOrDefault();//oct29



            if (datapass != null)
            {

                datapass.Status = "OUT";

                db.Entry(datapass).State = EntityState.Modified;

                db.SaveChanges();
            }

            //----------------------


            var host = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.Requestor).FirstOrDefault();
            layer.WhatsApp_With(host.MOBILE_NUMBER.ToString(), "enx_visitor_out");



            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RejectSecurity(int GPID)
        {
            var data = db.tblAppoinments.Where(m => m.A_ID == GPID && m.V_Status == "Pre").FirstOrDefault();
            if (data != null)
            {
                data.V_Status = "Reject";
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult RejectSecurityWithRemark(int GPID, string Remark)
        {
            var data = db.tblAppoinments.Where(m => m.A_ID == GPID && m.V_Status == "Pre").FirstOrDefault();
            if (data != null)
            {
                data.V_Status = "Reject";
                data.RejectRemark = Remark;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                SendMailReject(GPID);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Inward()
        //{
        //    if (Session["NAME"] != null)
        //    {
        //        string location = Session["LOCATION"].ToString();
        //        string site = Session["SITE"].ToString();
        //        var data = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();

        //        //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == data.DeviceNameWrkr).Select(z => z.paycode).Count();
        //        ViewData["hcw"] = 0;
        //        ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == data.DeviceNameExe).Select(z => z.PNI_code).Count();
        //        ViewData["hcv"] = db.sp_HCV(location).Select(z => z.Visitor).Count();
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login");
        //    }
        //}
       
        public ActionResult AppTodayReq()
        {
            if (Session["PNICODE"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                string pni = Session["PNICODE"].ToString();
                var data = db.sp_AppToday().Where(m => m.EMPLOYEE_ID == pni && m.regionid == RegionId).ToList().OrderByDescending(m => m.A_ID); 
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult AppUpcomingReq()
        {
            if (Session["PNICODE"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                string pni = Session["PNICODE"].ToString();
                var data = db.sp_AppUpcoming().Where(m => m.EMPLOYEE_ID == pni && m.Regionid == RegionId).ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult AppToday()
        {
            if (Session["NAME"] != null)
            {
                Session.LCID = 1033;
                if (Session["RoleType"] != null)
                {


                    if (Session["RoleType"].ToString() == "S")
                    {
                        int RegionId = Convert.ToInt32(Session["RegionId"]);

                        string location = Session["LOCATION"].ToString();
                        string site = Session["SITE"].ToString();
                        var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();

                        //ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();
                        // ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == dataloc.DeviceNameWrkr).Select(z => z.paycode).Count();
                        ViewData["hce"] = 0;
                        ViewData["hcw"] = 0;
                        ViewData["hcv"] = db.sp_HCV(location).Where(z => z.RegionId == RegionId).Select(z => z.Visitor).Count();
                        var data = db.sp_SecurityToday().Where(m => m.Valid_at.Contains(location) && m.Regionid== RegionId).ToList().OrderByDescending(m => m.A_ID);
                        return View(data);

                    }
                    else
                    {
                        return RedirectToAction("Login");
                    }



                }
                else
                {
                    return RedirectToAction("Login");
                }


            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult AppUpcoming()
        {
            if (Session["NAME"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                string location = Session["LOCATION"].ToString();
                string site = Session["SITE"].ToString();
                var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();

                // ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();
                //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == dataloc.DeviceNameWrkr).Select(z => z.paycode).Count();
                ViewData["hce"] = 0;
                ViewData["hcw"] = 0;
                ViewData["hcv"] = db.sp_HCV(location).Where(m => m.RegionId == RegionId).Select(z => z.Visitor).Count();

                var data = db.sp_AppUpcoming().Where(m => m.Valid_at.Contains(location) && m.Regionid == RegionId).ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult VisitorReport()
        {
            Session.LCID = 1033;
          
            if (Session["NAME"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                string name = Session["NAME"].ToString();
                switch (name)
                {
                    case "SECURITY":
                        string site = Session["SITE"].ToString();
                        string location = Session["LOCATION"].ToString();
                        var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();
            
                        ViewData["hcw"] = 0;
                        ViewData["hce"] = 0;

                        
                        ViewData["hcv"] = db.sp_HCV(location).Where(m => m.RegionId == RegionId).Select(z => z.Visitor).Count();
                        break;
                    case "ADMIN":
                     
                        ViewData["hcw"] = 0;
                    

                        ViewData["hce"] = 0;
                        ViewData["hcv"] = db.sp_HCVALL().Select(z => z.Visitor).Count();
                        break;
                }
                ViewBag.Fromdate = Convert.ToString(DateTime.Now.AddMonths(-1).ToString("dd/MMM/yyyy"));
                ViewBag.Todate = Convert.ToString(DateTime.Now.ToString("dd/MMM/yyyy"));
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult CourierNewReport()
        {
            Session.LCID = 1033;
            //Session.LCID = 2057;


            if (Session["NAME"] != null)
            {

                ViewBag.Fromdate = Convert.ToString(DateTime.Now.AddMonths(-1).ToString("dd/MMM/yyyy"));
                ViewBag.Todate = Convert.ToString(DateTime.Now.ToString("dd/MMM/yyyy"));
                //ViewBag.Fromdate = Convert.ToString(DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"));
                

                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult P_Courier(string visitype, string company, string visitor, string FDate, string TDate, string Req, string status)
        {
            Session.LCID = 1033;
            // Session.LCID = 2057;
            // Session.LCID = 1081;
            if (Req == "Select")
            {
                Req = "All";
            }
            if (status == "Select")
            {
                status = "All";
            }

            string UserId = "";
            if (Session["RoleType"] != null && Session["UserID"] != null)
            {
                if (Session["RoleType"].ToString() == "Employee")
                {

                    UserId = Session["UserID"].ToString();
                }

                if (Session["RoleType"].ToString() == "A")//Added 30june
                {

                    UserId ="Admin";
                }



            }
            else
            {
                return RedirectToAction("Login");
            }

            var pp = layer.ReportCourierPDF(visitype, company, visitor, FDate, TDate, Req, status, UserId);

            return PartialView(pp);
        }

        public ActionResult DispatchNewReport()
        {
            Session.LCID = 1033;

            if (Session["NAME"] != null)
            {
                ViewBag.Fromdate = Convert.ToString(DateTime.Now.AddMonths(-1).ToString("dd/MMM/yyyy"));
                ViewBag.Todate = Convert.ToString(DateTime.Now.ToString("dd/MMM/yyyy"));
             
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult P_Dispatch(string visitype, string company, string visitor, string FDate, string TDate, string Req, string status)
        {
            Session.LCID = 1033;
            if (Req == "Select")
            {
                Req = "All";
            }
            if (status == "Select")
            {
                status = "All";
            }

            string UserId = "";
            if (Session["RoleType"] != null && Session["UserID"] != null)
            {
                if (Session["RoleType"].ToString() == "Employee")
                {

                    UserId = Session["UserID"].ToString();
                }

                if (Session["RoleType"].ToString() == "A")//Added 30june
                {

                    UserId = "Admin";
                }

            }
            else
            {
                return RedirectToAction("Login");
            }



            var pp = layer.ReportDispatchPDF(visitype, company, visitor, FDate, TDate, Req, status, UserId);

            return PartialView(pp);
        }

        public ActionResult P_Visitor(string visitype, string company, string visitor, string FDate, string TDate, string Req, string status)
        {
            Session.LCID = 1033;
            if (Req == "Select")
            {
                Req = "All";
            }
            if (status == "Select")
            {
                status = "All";
            }
            var pp = layer.ReportVisitorPDF(visitype, company, visitor, FDate, TDate, Req, status);

            return PartialView(pp);
        }

        public ActionResult P_POReport(string FDate, string TDate)
        {
            var data = layer.ReportPO(FDate, TDate);
            return PartialView(data);
        }

        public JsonResult PopulateHost()
        {
            string region = Session["SITE"].ToString();
            var data = login.Master_Requestor.Where(m => m.Region == region).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PopulateUOM()
        {
            var data = db.tblUOMs.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public void ExportVis(string nameStatus, string nameCompanyID, string VisitorId, string FromDateRpt, string tDate, string HostName, string V_Status)
        {
            Session.LCID = 1033;
            if (HostName == "Select")
            {
                HostName = "All";
            }
            if (V_Status == "Select")
            {
                V_Status = "All";
            }
            string fname = "VisitorReport-" + DateTime.Now;
            List<DataField> data = new List<DataField>();
            data = layer.ReportVisitorPDF(nameStatus, nameCompanyID, VisitorId, FromDateRpt, tDate, HostName, V_Status);
            DataTable dt = ToDataTable<DataField>(data);
            dt.Columns.Remove("GP_ID");
            dt.Columns.Remove("Phone");
            dt.Columns.Remove("VisDuration");
            dt.Columns.Remove("V_Type");
            dt.Columns.Remove("VisDate");

            dt.Columns["Req"].ColumnName = "Host";
            //dt.Columns["VisDate"].ColumnName = "Visit Date";

            dt.Columns["strVisDate"].ColumnName = "Visit Date";
            dt.Columns["comName"].ColumnName = "Company";
            dt.Columns["visName"].ColumnName = "Visitor";
            //dt.Columns["VisTime"].ColumnName = "Time";
            dt.Columns.Remove("VisTime");

            dt.Columns["Type"].ColumnName = "Purpose";

            //-----------23 jan------

            dt.Columns["TimeIN"].ColumnName = "Check-in (Date & Time)";
            dt.Columns["TimeOUT"].ColumnName = "Check-out (Date & Time)";
            dt.Columns["RejectRemark"].ColumnName = "Remarks";
            dt.Columns["GP"].ColumnName = "Visitor Id";


            //------------------

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= " + fname + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
        //--------02nov  exelcourial------------
        public void ExportCourier(string Reportname, string nameStatus, string nameCompanyID, string VisitorId, string FromDateRpt, string tDate, string HostName, string V_Status)
        {
            Session.LCID = 1033;
            if (HostName == "Select")
            {
                HostName = "All";
            }
            if (V_Status == "Select")
            {
                V_Status = "All";
            }
            string fname = "CourierReport-" + DateTime.Now;
            List<DataFieldCourier> data = new List<DataFieldCourier>();
            //---for employee----
            string UserId = "";
            if (Session["RoleType"] != null && Session["UserID"] != null)
            {
                if (Session["RoleType"].ToString() == "Employee")
                {

                    UserId = Session["UserID"].ToString();
                }
                if (Session["RoleType"].ToString() == "A")//Added 30june
                {

                    UserId = "Admin";
                }

            }

            //----------


            if (Reportname == "Courier")
            {
                fname = "InwardPacketReport-" + DateTime.Now;



                data = layer.ReportCourierPDF(nameStatus, nameCompanyID, VisitorId, FromDateRpt, tDate, HostName, V_Status, UserId);
            }
            else
            {
                fname = "OutwardPacketReport-" + DateTime.Now;
                data = layer.ReportDispatchPDF(nameStatus, nameCompanyID, VisitorId, FromDateRpt, tDate, HostName, V_Status, UserId);
            }

            //List<DataField> data = new List<DataField>();
            //data = layer.ReportVisitorPDF(nameStatus, nameCompanyID, VisitorId, FromDateRpt, tDate, HostName, V_Status);

            DataTable dt = ToDataTable<DataFieldCourier>(data);
            dt.Columns["strC_Date"].ColumnName = "Date";
           
            dt.Columns.Remove("EmployeeId");
            dt.Columns.Remove("EmployeeName");
            

            dt.Columns.Remove("CouriarType");
           
            dt.Columns.Remove("EmployeeTransDate");
            dt.Columns.Remove("DeskTransDate");
            dt.Columns.Remove("C_Date");
            dt.Columns.Remove("TransactionDate");

            //dt.Columns.Remove("Address");

           

          
            dt.Columns["strTransactionDate"].ColumnName = "Entry Date";

           
        
            dt.Columns["CouriarId"].ColumnName = "Packet ID";
            if (Reportname == "Courier")
            {
                dt.Columns.Remove("Person");
                dt.Columns.Remove("ContactNo");
                dt.Columns["NameOfCompany"].ColumnName = "Name";
                dt.Columns["strEmployeeTransDate"].ColumnName = "Recipient Check-in (Date & Time)";

                //dt.Columns["strDeskTransDate"].ColumnName = "Support Desk Check-in(Date & Time)";

                dt.Columns["CouriarVendor"].ColumnName = "Delivery Provider";
               // dt.Columns["Entry Date"].ColumnName = "Security Check-in (Date & Time)";
                dt.Columns["Name"].ColumnName = "RECIPIENT NAME";

                dt.Columns.Remove("DocketDate");
                dt.Columns.Remove("FromCity");
                dt.Columns.Remove("DeliveryDate");
                dt.Columns.Remove("SenderContactNo");

                // dt.Columns["Remark"].ColumnName = "SECURITY REMARK";
                
        dt.Columns["EmployeeRemark"].ColumnName = "RECIPIENT REMARKS";// Added 10 oct
                /// 
                //---------------
                if (Session["RoleTypeBothYN"] != null)
                {
                    if (Session["RoleTypeBothYN"].ToString() == "No")
                    {
                        dt.Columns["strDeskTransDate"].ColumnName = "Support Desk Check-in(Date & Time)";
                        dt.Columns["Entry Date"].ColumnName = "Security Check-in (Date & Time)";
                        dt.Columns["Remark"].ColumnName = "SECURITY Remark";

                    }
                    else 
                    {
                        dt.Columns.Remove("DeskRemarks");
                        dt.Columns["Entry Date"].ColumnName = "Support Desk Check-in (Date & Time)";
                        dt.Columns["Remark"].ColumnName = "Desk Remark";
                        dt.Columns.Remove("strDeskTransDate");

                    }
                }

                        //--------------------

                    }
            else
            {
                //dt.Columns["strDeskTransDate"].ColumnName = "Desk check-out (date & time)";
                
                dt.Columns.Remove("Department");
                dt.Columns["NameOfCompany"].ColumnName = "Company";
                //dt.Columns.Remove("DeskTransDate");

               // dt.Columns["strEmployeeTransDate"].ColumnName = "Security Transaction Date";


                dt.Columns["CouriarVendor"].ColumnName = "Delivery Provider";
                dt.Columns["Entry Date"].ColumnName = "Sender check-out (date & time)";


 

               // dt.Columns["Security Transaction Date"].ColumnName = "Security check-out (date & time)";

   
                dt.Columns["DeliveryDate"].ColumnName = "Expected Delivery Date"; // //jan23


                dt.Columns["Remark"].ColumnName = "SENDER REMARK";

                //dt.Columns["EmployeeRemark"].ColumnName = "SECURITY REMARK";
                if (Session["RoleTypeBothYN"] != null)
                {
                    if (Session["RoleTypeBothYN"].ToString() == "No")
                    {
                        
                        dt.Columns["EmployeeRemark"].ColumnName = "SECURITY REMARK";
                        dt.Columns["strEmployeeTransDate"].ColumnName = "Security check-out (date & time)";
              
                        dt.Columns["strDeskTransDate"].ColumnName = "Desk check-out (date & time)";
                    }
                    else 
                    {
                       
                        dt.Columns["EmployeeRemark"].ColumnName = "Desk REMARK";

                        dt.Columns.Remove("DeskRemarks");
                        dt.Columns.Remove("strDeskTransDate");
                          dt.Columns["strEmployeeTransDate"].ColumnName = "Desk check-out (date & time)"; 

                    }
                }



                    }

            
                dt.Columns.Remove("PODFile");
            //--------------


            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= " + fname + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        //-------------
        public void ExportPO(string FromDatePO, string ToDatePO)
        {
            string fname = "POReport-" + DateTime.Now;
            List<MaterialReport> data = new List<MaterialReport>();
            data = layer.ReportPO(FromDatePO, ToDatePO);
            DataTable dt = ToDataTable<MaterialReport>(data);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            dt.Columns.Remove("M_ID"); dt.Columns.Remove("HODTime"); dt.Columns.Remove("SupplierName");
            dt.Columns.Remove("M_Status"); dt.Columns.Remove("Description");

            dt.Columns["ChallanNmbr"].ColumnName = "PO Number";
            dt.Columns["Requestor"].ColumnName = "Supplier Name";
            dt.Columns["Dept"].ColumnName = "Supplier Address";
            dt.Columns["ModeOfTrans"].ColumnName = "Rec Qty";
            dt.Columns["RaiseDate"].ColumnName = "PO Date";
            dt.Columns["M_Type"].ColumnName = "GST";
            dt.Columns["S_Type"].ColumnName = "Item";
            dt.Columns["M_Location"].ColumnName = "UOM";
            dt.Columns["OutFrom"].ColumnName = "Quantity";
            dt.Columns["SecApproveTime"].ColumnName = "Rec date";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= " + fname + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                    //values[i] = Convert.ChangeType(i[""], "dd/MMM/yyyy");
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

     

        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(string pniCodeF)
        {
            var data = login.Master_Requestor.Where(z => z.EMPLOYEE_ID == pniCodeF).FirstOrDefault();
            if (data != null)
            {
                var ran = RandomString(7);
                data.TrumpPassword = ran;
                login.Entry(data).State = EntityState.Modified;
                login.SaveChanges();

                StringBuilder sb = new StringBuilder();
                sb.Append("Dear " + data.NAME + " <br/> Your new password for eNX is '" + ran + "' <br/> ");
                layer.SendSMS(data.MOBILE_NUMBER.ToString(), "Dear " + data.NAME + " Your new password for eNX is " + ran + " KARAM", "1007161949739472289", "KRMITI");
                try { 
                if (BusinessLayer.IsValidEmailAddress(data.Email_ID))
                {
                    BusinessLayer.SendMail(sb.ToString(), data.Email_ID, "Forget Password for eXN");
                }
                }
                catch (Exception ex) { }
            }
            else
            {
                TempData["wrongPNI"] = "wrongPNI";
                return Redirect(Request.UrlReferrer.ToString());
            }
            TempData["pwdChange"] = "pwdChange";
            return Redirect(Request.UrlReferrer.ToString());
        }

        //--------------
        public ActionResult ForgetPasswordNew()
        {
            return View();
        }
        // [HttpPost]
        //public ActionResult ForgetPasswordNew(string pniCodeF)
        //{
        //    var data = login.Master_Requestor.Where(z => z.EMPLOYEE_ID == pniCodeF).FirstOrDefault();
        //    if (data != null)
        //    {
        //        var ran = RandomString(7);
        //        data.TrumpPassword = ran;
        //        login.Entry(data).State = EntityState.Modified;
        //        login.SaveChanges();

        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("Dear " + data.NAME + " <br/> Your new password for eNX is '" + ran + "' <br/> ");
        //       // layer.SendSMS(data.MOBILE_NUMBER.ToString(), "Dear " + data.NAME + " Your new password for eNX is " + ran + " KARAM", "1007161949739472289", "KRMITI");
        //        if (BusinessLayer.IsValidEmailAddress(data.Email_ID))
        //        {
        //            BusinessLayer.SendMail(sb.ToString(), data.Email_ID, "Forget Password for eXN");
        //        }
        //    }
        //    else
        //    {
        //        TempData["wrongPNI"] = "wrongPNI";
        //        return Redirect(Request.UrlReferrer.ToString());
        //    }
        //    TempData["pwdChange"] = "pwdChange";
        //    return Redirect(Request.UrlReferrer.ToString());
        //}

        public JsonResult SendForgetOtp(string UserId)
        {

            var ran = "0";
            var data = login.Master_Requestor.Where(z => z.Email_ID == UserId).FirstOrDefault();
            var workdata = db.WorkingLogins.Where(z => z.Email == UserId).FirstOrDefault();
            if (data != null)
            {
                ran = Randomnumber(6);


                StringBuilder sb = new StringBuilder();
                sb.Append("Dear " + data.NAME + ", <br/> Your OTP for new password request is " + ran + " <br/> ");
                // layer.SendSMS(data.MOBILE_NUMBER.ToString(), "Dear " + data.NAME + " Your new password for eNX is " + ran + " KARAM", "1007161949739472289", "KRMITI");
                if (BusinessLayer.IsValidEmailAddress(data.Email_ID))
                {
                    BusinessLayer.SendMail(sb.ToString(), data.Email_ID, "OTP for new password for Visitors Management System");
                }


            }
            else if (workdata != null)
            {
                ran = Randomnumber(6);


                StringBuilder sb = new StringBuilder();
                sb.Append("Dear " + workdata.UserID + ", <br/> Your OTP for new password request is " + ran + " <br/> ");

                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("Thanks");
                sb.Append("<br />");
                sb.Append("KARAM Team");


                // layer.SendSMS(data.MOBILE_NUMBER.ToString(), "Dear " + data.NAME + " Your new password for eNX is " + ran + " KARAM", "1007161949739472289", "KRMITI");
                if (BusinessLayer.IsValidEmailAddress(workdata.Email))
                {
                    BusinessLayer.SendMail(sb.ToString(), workdata.Email, "OTP for new password request for Visitors Management System ");
                }


            }
            else
            {
                ran = "0";

            }

            //ran = "123";
            return Json(ran, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendForgetPasswordNew(string UserId)
        {
            string msg = "0";
            var data = login.Master_Requestor.Where(z => z.Email_ID == UserId).FirstOrDefault();
            var workdata = db.WorkingLogins.Where(z => z.Email == UserId).FirstOrDefault();

            if (data != null)
            {
                var ran = RandomString(7);
                //data.TrumpPassword = ran;
                //login.Entry(data).State = EntityState.Modified;
                //login.SaveChanges();

                int check =layer.ChangePasswordbyEmail(UserId, ran);



                StringBuilder sb = new StringBuilder();
                sb.Append("Dear " + data.NAME + ", <br/> Your new password for Visitors Management System is " + ran + " <br/> ");
                //layer.SendSMS(data.MOBILE_NUMBER.ToString(), "Dear " + data.NAME + " Your new password for eNX is " + ran + " KARAM", "1007161949739472289", "KRMITI");
                if (BusinessLayer.IsValidEmailAddress(data.Email_ID))
                {
                    BusinessLayer.SendMail(sb.ToString(), data.Email_ID, "New  password for Visitors Management System");
                }
                msg = "ok";
            }

            else if (workdata != null)
            {

                var ran = RandomString(7);
                data.TrumpPassword = ran;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                StringBuilder sb = new StringBuilder();
                sb.Append("Dear " + workdata.UserID + ", <br/> Your new password for Visitors Management System is " + ran + " <br/> ");

                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("Thanks");
                sb.Append("<br />");
                sb.Append("KARAM Team");


                if (BusinessLayer.IsValidEmailAddress(data.Email_ID))
                {
                    BusinessLayer.SendMail(sb.ToString(), workdata.Email, "New  password for Visitors Management System");
                }
                msg = "ok";


            }
            else
            {
                msg = "0";
            }

            //msg = "ok";
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        //----------------



        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string Randomnumber(int length)
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string OldPwd, string NewPwd)
        {
            if (Session["PNICODE"] != null)
            {
                var pniCode = Session["PNICODE"].ToString();
                layer.UpdatePassword(NewPwd, pniCode);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public JsonResult GetOldPassword(string OldPassword)
        {
            var pniCode = Session["PNICODE"].ToString();
            var dd = login.Master_Requestor.Where(z => z.EMPLOYEE_ID == pniCode).FirstOrDefault();
            if (OldPassword != dd.TrumpPassword)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SingleDayQuestion(float q1, string q2, string q3, string q4)
        {
            string id = "";
            if(Session["PROFILE"]!=null)
                    { 
             id = Session["PROFILE"].ToString();
            }
            DateTime dd = DateTime.Now;
            string ss = id + "" + dd.Day + "" + dd.Month + "" + dd.Year + "" + dd.Hour + "" + dd.Minute + "" + dd.Second;
           // Session["SecirutyINTime"] = ss;
            tblQuestion q = new tblQuestion();
            q.Q1 = q1.ToString();
            q.Q2 = q2;
            q.Q3 = q3;
            q.Q4 = q4;
            q.SessionTime = ss;
            db.tblQuestions.Add(q);
            db.SaveChanges();
            string res = "";
            if (q1 < 99 && q2 == "No" && q3 == "Yes" && q4 == "No")
            {
                res = "Success";
            }
            else
            {
                res = "Exception";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PDFpage(string visitype)
        {
            string[] aa = visitype.Split('-');
            string VType = aa[0]; string company = aa[1];
            string visitor = aa[2] == "null" ? "All" : aa[2];
            string Month = aa[3]; string yr = aa[4];
            string Req = aa[5] == "Select" ? "All" : aa[5];
            string status = aa[6] == "Select" ? "All" : aa[6];

            var data = layer.ReportVisitorPDF(VType, company, visitor, Month, yr, Req, status);
            return View(data);
        }

        public ActionResult ConvertToPDF(string visitype)
        {
            var printpdf = new ActionAsPdf("PDFpage", new { visitype = visitype })
            {
                //PageWidth = 370,
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                FileName = "MyImage.jpeg",
            };
            string fullPath = Server.MapPath("../StringImage/VisitorReport.pdf");
            string filename = "VisitorReport-" + DateTime.Now + ".pdf";

            var binary = printpdf.BuildFile(this.ControllerContext);
            var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            fileStream.Write(binary, 0, binary.Length);
            fileStream.Close();
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("content-disposition", "filename=" + filename);
            Response.TransmitFile(Server.MapPath("~/StringImage/VisitorReport.pdf"));
            //Response.WriteFile(filePath);
            Response.Flush();
            Response.End();
            return RedirectToAction("VisitorReport");
        }

        public ActionResult Appointment()
        {
            if (Session["NAME"] != null)
            {
                Session.LCID = 1033;
                var data = layer.AppoinmentList;
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult HCV()
        {
            if (Session["LOCATION"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);

                string location = Session["LOCATION"].ToString();
                string site = Session["SITE"].ToString();
                var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();

               // ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();
                //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == dataloc.DeviceNameWrkr).Select(z => z.paycode).Count();
                ViewData["hcw"] = 0;
                ViewData["hce"] = 0;
                ViewData["hcv"] = db.sp_HCV(location).Where(m=>m.RegionId==RegionId).Select(z => z.Visitor).Count();
                var data = db.sp_HCV(location).Where(m => m.RegionId == RegionId).ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //Test
        public ActionResult TestImagePartialViewWithModel(string id)
        {
            var data = db.sp_DownloadePass(id).ToList();
            return new PartialViewAsImage("TestPartialViewWithModel", data);
        }
        public ActionResult TestImageViewWithModel(string id)
        {
            var model = db.sp_DownloadePass(id).ToList();
            return new ViewAsImage("TestViewWithModel", model);
        }

        [HttpGet]
        public ActionResult Print(string visitype)
        {
            return new ActionAsPdf("PDFpage", new { visitype = visitype })
            { FileName = "VisitorReport(" + DateTime.Now + ").pdf" };
        }

        public ActionResult SaveVID(string id, string path)
        {
            return new ActionAsImage("Contact", new { id = id, path = path })
            { FileName = id + ".jpg" };
        }

        [HttpGet]
        public ActionResult SelfAppointment()
        {
            if (Session["NAME"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);
                string location = Session["LOCATION"].ToString();
                string site = Session["SITE"].ToString();
                var dataloc = db.tblSites.Where(m => m.L_name == location && m.Region == site).FirstOrDefault();

                //ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In" && z.DeviceShortName == dataloc.DeviceNameExe).Select(z => z.PNI_code).Count();
                //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In" && z.UNIT == dataloc.DeviceNameWrkr).Select(z => z.paycode).Count();
                ViewData["hce"] = 0;
                ViewData["hcw"] = 0;
                ViewData["hcv"] = db.sp_HCV(location).Where(m => m.RegionId == RegionId).Select(z => z.Visitor).Count();

                var data = db.tblApprovals.Where(m => m.AppType == "SELF" && m.Region == site).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public ActionResult SelfAppointment(Appointment ap)
        {
            if (Session["NAME"] != null)
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);

                string location = Session["LOCATION"].ToString();
                string dept = Session["SITE"].ToString();
                string[] date = DateTime.Now.ToString().Split('/');
                string[] yr = date[2].Split(' '); string[] tm = yr[1].Split(':');

                string name = Request.Form["host"];
                string r = date[0].ToString().TrimStart('0');
                string h = tm[0].ToString().TrimStart('0');

                string value1 = "V" + date[1] + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(r)) + yr[0].Substring(2) + Enum.GetName(typeof(BusinessLayer.Hours), Convert.ToInt32(h));

                tblAppoinment tblAppoinment = new tblAppoinment();
                tblAppoinment.Requestor = ap.Requestor;
                tblAppoinment.Deptment = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == ap.Requestor).FirstOrDefault().DepartmentID;
                tblAppoinment.ComanyID = ap.ComanyID;
                tblAppoinment.VisitorID = ap.VisitorID;
                tblAppoinment.Visitor_Type = "Official";
                tblAppoinment.V_DateFrom = DateTime.Today;
                tblAppoinment.V_DateTo = DateTime.Today;
                tblAppoinment.A_Time = ap.A_Time;
                tblAppoinment.Valid_at = location;
                tblAppoinment.Duration = ap.Duration;
                tblAppoinment.V_Location = ap.V_Location;
                tblAppoinment.Remark = ap.Remark;
                tblAppoinment.V_Allowed = "false";
                tblAppoinment.A_Type = "Request";
                tblAppoinment.RaiseDate = DateTime.Now;
                tblAppoinment.Location = ap.Location;
                tblAppoinment.V_Status = "Arrived";
                tblAppoinment.EntryType = "1";
                tblAppoinment.V_InDate = DateTime.Now;
                tblAppoinment.Flag = "True";

                tblAppoinment.RegionId = RegionId;

                db.tblAppoinments.Add(tblAppoinment);
                db.SaveChanges();

                string ch = "";
                switch (tblAppoinment.A_ID)
                {
                    case int n when n <= 9:
                        ch = ch + "000" + tblAppoinment.A_ID;
                        break;
                    case int n when n >= 10 && n <= 99:
                        ch = ch + "00" + tblAppoinment.A_ID;
                        break;
                    case int n when n > 100 && n <= 999:
                        ch = "0" + tblAppoinment.A_ID;
                        break;
                    default:
                        ch = ch + tblAppoinment.A_ID;
                        break;
                }
                var lastData = db.tblAppoinments.Where(m => m.A_ID == tblAppoinment.A_ID).FirstOrDefault();
                lastData.GPID = value1 + ch;
                db.Entry(lastData).State = EntityState.Modified;
                db.SaveChanges();

                string ss = Session["SecirutyINTime"].ToString();
                tblQuestion qu = db.tblQuestions.Where(z => z.SessionTime == ss).FirstOrDefault();
                qu.GP_ID = tblAppoinment.A_ID;
                db.Entry(qu).State = EntityState.Modified;
                db.SaveChanges();
                Session.Remove("SecirutyINTime");

                VisitorAcceptance vs = new VisitorAcceptance();
                vs.VisitorID = lastData.VisitorID;
                vs.GatePassID = lastData.GPID;
                vs.AccptGate = DateTime.Now;
                db.VisitorAcceptances.Add(vs);
                db.SaveChanges();

                tblVisitorINOUT visitorINOUT = new tblVisitorINOUT();
                visitorINOUT.Location = location;
                visitorINOUT.FDate = DateTime.Now;
                visitorINOUT.VL_Status = "IN";
                visitorINOUT.GPID = lastData.A_ID.ToString();
                db.tblVisitorINOUTs.Add(visitorINOUT);
                db.SaveChanges();

                string chkT = Request.Form["chkselectedmaterial_T"];
                string chkL = Request.Form["chkselectedmaterial_L"];
                string chkMD = Request.Form["chkselectedmaterial_MD"];
                string chkSD = Request.Form["chkselectedmaterial_SD"];
                if (chkL == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_L"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(tblAppoinment.A_ID);
                    material.M_Name = "Laptop";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }
                if (chkT == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_T"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(tblAppoinment.A_ID);
                    material.M_Name = "Tool Box";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }
                if (chkMD == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_MD"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(tblAppoinment.A_ID);
                    material.M_Name = "Media Device";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }
                if (chkSD == "on")
                {
                    tblMaterial material = new tblMaterial();
                    string sL = Request.Form["Serial_No_SD"].ToString().ToUpper();
                    material.App_ID = Convert.ToInt32(tblAppoinment.A_ID);
                    material.M_Name = "Storage Device";
                    material.Serial_No = sL;
                    db.tblMaterials.Add(material);
                    db.SaveChanges();
                }

                TempData["single"] = "single";
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

   

        public ActionResult GuestRequestor()
        {
            string site = Session["SITE"].ToString();
            var vData = (from v in db.tblApprovals
                         where v.AppType == "SELF" && v.Region == site
                         select new
                         {
                             Name = v.Name_,
                             ID = v.AppName
                         }
                      ).ToList();
            return Json(vData.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuestRequestorData(string pni)
        {
            var data = (from D in login.Departments
                        join R in login.Master_Requestor
                        on D.D_ID equals R.DepartmentID
                        where R.EMPLOYEE_ID == pni
                        select new
                        {
                            Name_ = R.NAME,
                            ID = R.EMPLOYEE_ID,
                            Email = R.Email_ID,
                            Dept = D.DepartmentName,
                            Designtn = R.DESIGNATION,
                            MBL = R.MOBILE_NUMBER
                        }
                      ).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //--
        [HttpGet]
        public JsonResult PassExists(int GPID, string Pass)
        {
            int RegionId = Convert.ToInt32(Session["RegionId"]);
            var data = db.tblPasses.Where(z => z.PassNo == Pass && z.Status == "IN"  &&  z.RegionId== RegionId).FirstOrDefault();

            //var data = db.tblPasses.Where(z => z.PassNo == Pass && z.Status == "IN" && z.RegionId == RegionId).FirstOrDefault();

            if (data != null)
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("0", JsonRequestBehavior.AllowGet);
            }



        }

       
        public ActionResult Log()
        {
            if (Session["NAME"] != null)
            {


                var data = db.ExceptionLoggers.OrderByDescending(m => m.EXID).ToList();

                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

       

        public ActionResult SendCourierMail()
        {

            try
            {
                int RegionId = Convert.ToInt32(Session["RegionId"]);

                var alldata = db.tblCouriars.Where(m => m.CurrentStatus == "Intrasit"  && m.CouriarType == "Inward" && m.RegionId== RegionId).ToList();


                foreach (var a in alldata)
                {
                    //=========comment 7 feb===================
                    //  string empid = a.EmployeeId;
                    //  string Region = a.Region;
                    //  string dept = a.Department;
                    //  string docket = a.DocketNo;
                    //  string vendor = a.CouriarVendor;



                    //  var data = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == empid).FirstOrDefault();

                    //  string email = data.Email_ID;
                    //  string strname = data.NAME;
                    //// email = "jeetu80@gmail.com";

                    //SendMail(email, strname, Region, dept, docket, vendor);
                    //=================

                    //---------------new function for Acknowlege mail ------------------
                    int Courier_ID = a.CouriarId;
                    SendMailEmployeeCourierconfirm(Courier_ID);
                    //----------------------------

                }
            }
            catch (Exception ex)
            {
                ///objException.SendExcepToDB(ex);
                ex.Message.ToString();
            }

            return View();

        }


        //public string SendMail(string to, string strname, string Region, string dept, string docket, string vendor)
        //{
        //    string literal = null;
        //    DataTable dt = new DataTable();
        //    try
        //    {

        //        StringBuilder stringBuilder = new StringBuilder();
        //        string strdate = DateTime.Now.ToString();


        //        //  stringBuilder.Append("Dear " + strname + ", <br/><br/>Your courier is pending to acknowledge. Kindly acknowledge it. <br/>Click link to acknowledge:<a href='https://172.20.1.106:8016/'>Click</a><br/>");
        //        stringBuilder.Append("Dear " + strname + ", <br/><br/>Your courier is pending to acknowledge. Courier information is given below: <br/>");


        //        //------------

        //        stringBuilder.Append("<table border='0' id='datatable' style='5px solid #0e0e0e'>");




        //        stringBuilder.Append("<tr><td><span style='color:black;font:bold 12px arial'>Courier From Region  :</td>");
        //        stringBuilder.Append("<td>" + Region + "</td></tr>");

        //        stringBuilder.Append("<tr><td><span style='color:black;font:bold 12px arial'>Department   :</td>");
        //        stringBuilder.Append("<td>" + dept + "</td></tr>");

        //        stringBuilder.Append("<tr><td><span style='color:black;font:bold 12px arial'>Docket Number  :</td>");
        //        stringBuilder.Append("<td>" + docket + "</td></tr>");

        //        stringBuilder.Append("<tr><td><span style='color:black;font:bold 12px arial'>Docket Vendor  :</td>");
        //        stringBuilder.Append("<td>" + vendor + "</td></tr>");



        //        stringBuilder.Append("</table>");

        //        //stringBuilder.Append("Click link to acknowledge:< a href = 'https://172.20.1.106:8016/' > Click </ a >< br />");
        //        stringBuilder.Append(" <br/><a href='https://172.20.1.106:8016/'>Click</a><br/>");

        //        stringBuilder.Append("<br />");
        //        stringBuilder.Append("<br />");
        //        stringBuilder.Append("Thanks");
        //        stringBuilder.Append("<br />");
        //        stringBuilder.Append("IT-KARAM");

        //        //----------------

        //        literal = stringBuilder.ToString();
        //        MailMessage mail = new MailMessage();
        //        mail.To.Add(to);
        //        mail.From = new MailAddress("karamsupport@karam.in");
        //        mail.Subject = "Reminder to Acknowledge Your Courier";
        //        mail.Body = literal;
        //        mail.IsBodyHtml = true;
        //        string KeyHost = ConfigurationManager.AppSettings["KeyHost"].ToString();
        //        string KeyPort = ConfigurationManager.AppSettings["KeyPort"].ToString();
        //        string KeyUId = ConfigurationManager.AppSettings["KeyUId"].ToString();
        //        string KeyPass = ConfigurationManager.AppSettings["KeyPass"].ToString();

        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = KeyHost;
        //        smtp.Port = Convert.ToInt32(KeyPort);
        //        smtp.UseDefaultCredentials = false;
        //        smtp.Credentials = new NetworkCredential(KeyUId, KeyPass);
        //        smtp.EnableSsl = true;
        //        smtp.Send(mail);


        //    }
        //    catch (Exception ex)
        //    {
        //        ///objException.SendExcepToDB(ex);
        //        ex.Message.ToString();
        //    }
        //    return literal;
        //}

        //------new--------
        public ActionResult Master_RequestorEntry()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]

        public ActionResult SaveRequestorDetail(FormCollection fc)
        {
            try
            {

                Master_Requestor Obj = new Master_Requestor();
                
                //if (Session["NAME"] != null)
                //{
                string strtime = DateTime.Now.ToString("hh:mm tt");

                Obj.EMPLOYEE_ID = fc["txtEMPLOYEE_ID"].ToString();
                Obj.NAME = fc["txtNAME"].ToString();
                Obj.KSS_Department = fc["txtKSSDepartment"].ToString();
                Obj.requestor_key = fc["txtrequestor_key"].ToString();
                Obj.DESIGNATION = fc["txtDESIGNATION"].ToString();
                Obj.DEPARTMENT = fc["txtDEPARTMENT"].ToString();
                Obj.DepartmentID = Convert.ToInt32(fc["txtDepartmentID"].ToString());
                Obj.Immediate_Supervisor = fc["txtImmediate_Supervisor"].ToString();
                Obj.Immediate_Supervisor_EMP_ID = fc["txtImmediate_Supervisor_EMP_ID"].ToString();
                Obj.HOD = fc["txtHOD"].ToString();
                Obj.HOD_EMP_ID = fc["txtHOD_EMP_ID"].ToString();
                Obj.MOBILE_NUMBER = Convert.ToDouble(fc["txtMOBILE"].ToString());
                Obj.DATE_OF_BIRTH = Convert.ToDateTime(fc["V_Dob"].ToString());
                Obj.PAYROLL = fc["txtPAYROLL"].ToString();
                Obj.DATE_OF_JOINING = Convert.ToDateTime(fc["V_Doj"].ToString());
                Obj.Email_ID = fc["txtEmail"].ToString();
                Obj.VPF = Convert.ToDouble(fc["txtVPF"].ToString());
                Obj.EPF = Convert.ToDouble(fc["txtEPF"].ToString());
                Obj.PAN_NO_ = fc["txtPANNO"].ToString();
                Obj.Total_deductions_ = Convert.ToDouble(fc["txtDeduction"].ToString());
                Obj.Yearly_VPF = Convert.ToDouble(fc["txtYVPF"].ToString());
                Obj.Yearly_EPF = Convert.ToDouble(fc["txtYEPF"].ToString());
                Obj.Password = fc["txtPassword"].ToString();
                Obj.Status = fc["txtStatus"].ToString();
                Obj.UserType = fc["txtUserType"].ToString();
                Obj.InvestmentDeclarationStatus = true; //fc["txtInvestmentDeclarationStatus"].ToString();
                Obj.Investment_Declaration_Password = fc["txtInvestment_Declaration_Password"].ToString();
                Obj.HRMSPassword = fc["txtHRMSPassword"].ToString();
                Obj.TrumpPassword = fc["txtTrumpPassword"].ToString();
                Obj.CompanyID = Convert.ToInt32(fc["txtCompanyID"].ToString());
                Obj.Region = fc["ddlRegion"].ToString();


                //objcCouriar.Documents = fc["txtdoc"].ToString();
                //objcCouriar.City = fc["txtcity"].ToString();
                //objcCouriar.NameOfCompany = fc["txtcompany"].ToString();

                //string Vdate = fc["V_DateFrom"].ToString();//+ " 12:00:00 AM"; //10 / 18 / 2022 12:00:00 AM}


                //----------------------------
                login.Master_Requestor.Add(Obj);
                login.SaveChanges();

                TempData["create"] = "create";

                //}
                //else
                //{
                //    return RedirectToAction("Login");
                //}

            }
            catch (Exception ex)
            {
                //ex.Message.ToString();
                //  layer.SendExcepToDB(ex);
            }

            //TempData["Feedback"] = "Feedback";
            return Redirect(Request.UrlReferrer.ToString());
        }

        //--------
        public ActionResult GetDepartmenByRegiontList(string Region)
        {


            var data = (from v in login.Departments

                        where v.Region == Region
                        select new
                        {

                            DepartmentId = v.D_ID,
                            DEPARTMENT = v.DepartmentName
                        }
                     ).ToList().Distinct();



            //Select(m => m.CategoryName).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult MasterRequesterList()
        {
            
            var data = layer.MasterRequesterListNew;
            return View(data);




        }

        //-new-----------
        public ActionResult GetCourierCompanyList(string Dept)
        {
            List<DataFieldCourierCompany> ItemList = new List<DataFieldCourierCompany>();
            string kee = "select CourierCompanyId,CourierCompany from tblCourierCompany where status='Y' ";
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERY(kee);

            

            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                DataFieldCourierCompany obj = new DataFieldCourierCompany();
                obj.CourierCompanyId = Convert.ToInt32(ktd.Rows[i]["CourierCompanyId"].ToString());
                obj.CourierCompany = ktd.Rows[i]["CourierCompany"].ToString();
                ItemList.Add(obj);
            }

            return Json(ItemList.ToList(), JsonRequestBehavior.AllowGet);




        }


     

        public ActionResult SendRejectMail(int GPid)
        {

            try
            {


                //var alldata = db.tblCouriars.Where(m => m.CurrentStatus == "Intrasit" && m.CouriarType == "Inward").ToList();


                //foreach (var a in alldata)
                //{

                //    string empid = a.EmployeeId;
                //    string Region = a.Region;
                //    string dept = a.Department;
                //    string docket = a.DocketNo;
                //    string vendor = a.CouriarVendor;



                //    var data = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == empid).FirstOrDefault();

                //    string email = data.Email_ID;
                //    string strname = data.NAME;
                //    // email = "jeetu80@gmail.com";


                //    SendMailReject(email, strname, Region, dept, docket, vendor);



                //}
            }
            catch (Exception ex)
            {
                ///objException.SendExcepToDB(ex);
                ex.Message.ToString();
            }

            return View();

        }


        public string SendMailReject(int GPID)
        {
            string literal = null;
            DataTable dt = new DataTable();
            try
            {
                //------------
                string to = "";
                string strname = "";
                string strremarks = "";
                string strVisitorname = "";
              

                var data = db.tblAppoinments.Where(m => m.A_ID == GPID).FirstOrDefault();
                if (data != null)
                {


                    string RId = data.Requestor;
                    var masterdata = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == RId).FirstOrDefault();

                    if (masterdata != null)
                    {
                        strname = masterdata.NAME;
                        strremarks = data.RejectRemark;
                        to = masterdata.Email_ID;
                       
                    }


                    int Vid = Convert.ToInt32(data.VisitorID);
                    var Visitordata = db.tblVisitors.Where(m => m.V_ID == Vid).FirstOrDefault();


                    if (Visitordata != null)
                    {
                        strVisitorname = Visitordata.V_Name;

                    }

                }
                //--------------



                // 
                Session.LCID = 1033;
                StringBuilder stringBuilder = new StringBuilder();
                string strdate = DateTime.Now.ToString();
                 //string appdate22 = data.V_DateFrom.Value.ToString("dd/MMM/yyyy");
                string appdate = data.V_DateFrom.Value.ToString("dd-MM-yyyy");

               

                stringBuilder.Append("Dear " + strname + ", <br/><br/>Your appointement dated " + appdate + " with " + strVisitorname + "  is Rejected. <br/>");
                stringBuilder.Append("<br/>");
                stringBuilder.Append("Reason:" + strremarks + " <br/>");
                //------------

               

                stringBuilder.Append("<br />");
                stringBuilder.Append("<br />");
                stringBuilder.Append("Thanks");
                stringBuilder.Append("<br />");
                stringBuilder.Append("KARAM Team");




                //----------------
                if (to != "")
                {
                    literal = stringBuilder.ToString();
                    MailMessage mail = new MailMessage();
                    mail.To.Add(to);
                    mail.From = new MailAddress("karamsupport@karam.in");
                
                    mail.Subject = " Your appointment rejected from Visitors Management System";

                    mail.Body = literal;
                    mail.IsBodyHtml = true;
                    string KeyHost = ConfigurationManager.AppSettings["KeyHost"].ToString();
                    string KeyPort = ConfigurationManager.AppSettings["KeyPort"].ToString();
                    string KeyUId = ConfigurationManager.AppSettings["KeyUId"].ToString();
                    string KeyPass = ConfigurationManager.AppSettings["KeyPass"].ToString();

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = KeyHost;
                    smtp.Port = Convert.ToInt32(KeyPort);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(KeyUId, KeyPass);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                }

            }
            catch (Exception ex)
            {
                ///objException.SendExcepToDB(ex);
                ex.Message.ToString();
            }
            return literal;
        }

        
        public ActionResult GetFromCityList(string Dept)
        {
            List<DataFieldCourierCompany> ItemList = new List<DataFieldCourierCompany>();
            string kee = "select distinct Source as Source From tblDeliveryTimeline where status='Active'";//select CourierCompanyId,CourierCompany from tblCourierCompany where status='Y' 
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERY(kee);

            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                DataFieldCourierCompany obj = new DataFieldCourierCompany();
               
                obj.CourierCompany = ktd.Rows[i]["Source"].ToString();
                ItemList.Add(obj);
            }

            return Json(ItemList.ToList(), JsonRequestBehavior.AllowGet);




        }
        public ActionResult GetToCityList(string Dept)
        {
            List<DataFieldCourierCompany> ItemList = new List<DataFieldCourierCompany>();
            string kee = "select distinct Destination as Destination From tblDeliveryTimeline where status='Active'";//select CourierCompanyId,CourierCompany from tblCourierCompany where status='Y' 
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERY(kee);

            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                DataFieldCourierCompany obj = new DataFieldCourierCompany();
               
                obj.CourierCompany = ktd.Rows[i]["Destination"].ToString();
                ItemList.Add(obj);
            }

            return Json(ItemList.ToList(), JsonRequestBehavior.AllowGet);




        }

        public ActionResult GetDeliverhyDays(string FromCity, string ToCity)
        {

            Session.LCID = 1033;
            List<DataFieldCourierCompany> ItemList = new List<DataFieldCourierCompany>();
            string kee = "select Days from tblDeliveryTimeline where source='" + FromCity + "' and Destination='" + ToCity + "'";//select CourierCompanyId,CourierCompany from tblCourierCompany where status='Y' 
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERY(kee);

            double days1 = Convert.ToDouble(ktd.Rows[0]["Days"].ToString());
            string strdate = Convert.ToString(DateTime.Now.AddDays(days1).ToString("dd/MMM/yyyy"));


            return Json(strdate, JsonRequestBehavior.AllowGet);




        }

        public ActionResult GetDeliveryDaysbypacketType(string PacketType)
        {

            Session.LCID = 1033;
            List<DataFieldCourierCompany> ItemList = new List<DataFieldCourierCompany>();
            string kee = "select top 1 Days as Days from tblDeliveryTimeline where PackageType='" + PacketType + "' ";//select CourierCompanyId,CourierCompany from tblCourierCompany where status='Y' 
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERY(kee);

            double days1 = Convert.ToDouble(ktd.Rows[0]["Days"].ToString());
            string strdate = Convert.ToString(DateTime.Now.AddDays(days1).ToString("dd/MMM/yyyy"));


            return Json(strdate, JsonRequestBehavior.AllowGet);




        }

        public ActionResult GetPacketTypeList(string Dept)
        {
            List<DataFieldCourierCompany> ItemList = new List<DataFieldCourierCompany>();
            string kee = "";
            if (Dept.TrimEnd().TrimStart() == "Procurement & Stores")
            {
                kee = "select  Distinct[PackageType] From tblDeliveryTimeline where status='Active'";
            }
            else if (Dept == "Courier")///for inward both show
            {
                kee = "select  Distinct[PackageType] From tblDeliveryTimeline where status='Active'";
            }

            else

            {

                kee = "select  Distinct[PackageType] From tblDeliveryTimeline where status='Active' and PackageType='Courier'";
            }
            // string kee = "select  Distinct[PackageType] From tblDeliveryTimeline where status='Active'";//select CourierCompanyId,CourierCompany from tblCourierCompany where status='Y' 
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERY(kee);

            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                DataFieldCourierCompany obj = new DataFieldCourierCompany();
                //obj.CourierCompanyId = Convert.ToInt32(ktd.Rows[i]["Source"].ToString());
                obj.CourierCompany = ktd.Rows[i]["PackageType"].ToString();
                ItemList.Add(obj);
            }

            return Json(ItemList.ToList(), JsonRequestBehavior.AllowGet);




        }
        //------SEND mail Employee couriar confirm----------------

        public string SendMailEmployeeCourierconfirm(int CouriarId)
        {
            string literal = null;
            DataTable dt = new DataTable();
            try
            {

                string to = "";
                string strname = "";
                string dispatchid = "";
                var data = db.tblCouriars.Where(m => m.CouriarId == CouriarId).FirstOrDefault();
                if (data != null)
                {

                    dispatchid = data.DocketNo.ToString();
                    string RId = data.EmployeeId;
                    var masterdata = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == RId).FirstOrDefault();

                    if (masterdata != null)
                    {
                        strname = masterdata.NAME;


                        to = masterdata.Email_ID;

                    }


                }

               // to = "jitender@samparksoftwares.com";
                StringBuilder stringBuilder = new StringBuilder();
                string strdate = DateTime.Now.ToString();
                string deskremark = data.DeskRemarks;
                stringBuilder.Append("Dear " + strname + ", <br/><br/>Please confirm your courier acceptance for <b> Docket Number:</b> " + dispatchid + ".<br/><b> Desk Remarks:</b> " + deskremark + "<br/>");

                //------------
                //  string CouriarIdnew =layer.Encrypt(Convert.ToString(CouriarId));
                string CouriarIdnew = layer.Encode(Convert.ToString(CouriarId));

                //CouriarId
                string UrlPort = ConfigurationManager.AppSettings["UrlPort"].ToString();

                //stringBuilder.Append(" <br/><a href='https://172.20.1.106:8016/Home/EmployeeAcknowledgeCourier?EditID=" + CouriarIdnew + "' class='btn btn-primary'><button style='background-color: #008CBA; border: none; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px;border-radius:6px'>Acknowledge</button></a><br/>");


                //stringBuilder.Append(" <br/><a href='https://172.20.1.106:" + UrlPort + "/Home/EmployeeAcknowledgeCourier?EditID=" + CouriarIdnew + "' class='btn btn-primary'><button style='background-color: #008CBA; border: none; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px;border-radius:6px'>Acknowledge</button></a><br/>");



                //stringBuilder.Append(" <br/><a href='https://vms.karam.in/Home/EmployeeAcknowledgeCourier?EditID=" + CouriarIdnew + "' class='btn btn-primary'><button style='background-color: #008CBA; border: none; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px;border-radius:6px'>Acknowledge</button></a><br/>");
                string Appurl = ConfigurationManager.AppSettings["Appurl"].ToString();
                stringBuilder.Append(" <br/><a href='" + Appurl + "/Home/EmployeeAcknowledgeCourier?EditID=" + CouriarIdnew + "' class='btn btn-primary'><button style='background-color: #008CBA; border: none; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px;border-radius:6px'>Acknowledge</button></a><br/>");


                stringBuilder.Append("<br />");
                stringBuilder.Append("<br />");
                stringBuilder.Append("Thanks");
                stringBuilder.Append("<br />");
                stringBuilder.Append("KARAM Team");

                if (to != "")
                {
                    //to = "jitender@samparksoftwares.com";
                    literal = stringBuilder.ToString();
                    MailMessage mail = new MailMessage();
                    mail.To.Add(to);
                    mail.From = new MailAddress("karamsupport@karam.in");
                    mail.Subject = "Courier confirmation on Visitors Management System";
                    mail.Body = literal;
                    mail.IsBodyHtml = true;
                    string KeyHost = ConfigurationManager.AppSettings["KeyHost"].ToString();
                    string KeyPort = ConfigurationManager.AppSettings["KeyPort"].ToString();
                    string KeyUId = ConfigurationManager.AppSettings["KeyUId"].ToString();
                    string KeyPass = ConfigurationManager.AppSettings["KeyPass"].ToString();

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = KeyHost;
                    smtp.Port = Convert.ToInt32(KeyPort);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(KeyUId, KeyPass);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                }

            }
            catch (Exception ex)
            {
                
                ex.Message.ToString();
            }
            return literal;
        }

        //public ActionResult EmployeeAcknowledgeCourier(int EditID)//16aug
        //{
        //    //int RegionId = Convert.ToInt32(Session["RegionId"]);
        //    //var data = db.tblCouriars.Where(m => m.CouriarId == EditID && m.RegionId== RegionId).FirstOrDefault();
        //    var data = db.tblCouriars.Where(m => m.CouriarId == EditID).FirstOrDefault();
        //    ViewBag.CourierId = EditID;

        //    //----button  enable and disable-----
        //    ViewBag.Status = data.CurrentStatus;
        //    ViewBag.DocketNo = data.DocketNo;
        //    ViewBag.Sender = data.SenderName;
        //    ViewBag.SenderCity = data.City;

        //    return View(data);
        //}
        public ActionResult EmployeeAcknowledgeCourier(String EditID)
        {
            //string aa = layer.Decrypt(EditID);
          //  int CouriarId = Convert.ToInt32(layer.Decrypt(EditID));
            int CouriarId = Convert.ToInt32(layer.Decode(EditID));
            
            //int CouriarId = Convert.ToInt32(layer.Decrypt(aa));
            var data = db.tblCouriars.Where(m => m.CouriarId == CouriarId).FirstOrDefault();
            ViewBag.CourierId = EditID;

            //----button  enable and disable-----
            ViewBag.Status = data.CurrentStatus;
            ViewBag.DocketNo = data.DocketNo;
            ViewBag.Sender = data.SenderName;
            ViewBag.SenderCity = data.City;

            return View(data);
        }

        [HttpPost]
        public ActionResult EmployeeAcknowledgeCourier(FormCollection fc)
        {

            //int Courier_ID = Convert.ToInt32(fc["txtcourierId"]);aug16

            //string input =layer.Decrypt(fc["txtcourierId"]);
            string input = layer.Decode(fc["txtcourierId"]);
            int Courier_ID = Convert.ToInt32(input);



            string remarks = fc["CouriarRemark"].ToString();

            var data = db.tblCouriars.Where(m => m.CouriarId == Courier_ID).FirstOrDefault();
            if (data != null)
            {
                data.CurrentStatus = "Close";

                data.EmployeeRemark = remarks;
                data.EmployeeTransDate = DateTime.Now;

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }

            TempData["EmployeeAcknowledgeDone"] = "EmployeeAcknowledgeDone";
          
            return Redirect(Request.UrlReferrer.ToString());


        }




        public ActionResult UpdateEmployeeDetail()
        {



            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                bool allowYN = PageRoletype("Employee");
                if (allowYN == false)
                {
                    return RedirectToAction("Login");
                }
                Master_Requestor data = new Master_Requestor();
                string strEMPLOYEE_ID = Session["UserID"].ToString();
                data = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == strEMPLOYEE_ID).FirstOrDefault();
                bool aa = Convert.ToBoolean(data.InvestmentDeclarationStatus);
                ViewBag.INdStatus = Convert.ToString(aa);
                ViewBag.Dept = "Customer Relations & Certifications";

                return View(data);

            }
            else
            {
                return RedirectToAction("Login");
            }



        }
        [HttpPost]
        public ActionResult UpdateEmployeeDetail(FormCollection fc)
        {

            int Check = 0;
            string strmobile = fc["txtMOBILE"].ToString();
            //string stremail = fc["txtEmail"].ToString();//email update not requred
            string strEMPLOYEE_ID = Session["UserID"].ToString();
            var data = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == strEMPLOYEE_ID).FirstOrDefault();

            if (data != null)
            {
                data.MOBILE_NUMBER = Convert.ToDouble(strmobile);
                try {
                    //data.Email_ID = stremail;

                    //login.Entry(data).State = EntityState.Modified;
                    //login.SaveChanges();
                     Check = layer.ChangeEmpMobileNo(strEMPLOYEE_ID, strmobile);

                    Session["MBL"] = strmobile;

                }
                catch (Exception ex) { ex.Message.ToString(); }

            }

            if (Check == 1) 
            
            {
                TempData["UpdateEmployeeDetail"] = "UpdateEmployeeDetail";
            }
           

            return Redirect(Request.UrlReferrer.ToString());

        }

        public ActionResult GetCompanyList(string Region)
        {


            var data = (from v in login.MasterCompanies


                        select new
                        {

                            CompanyId = v.MC_ID,
                            Company = v.CompanyName
                        }
                     ).ToList().Distinct();




            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AppointmentAll()
        {
            if (Session["NAME"] != null)
            {
                string UserId;
                if (Session["RoleType"].ToString() == "Employee")
                {
                    UserId = Session["UserID"].ToString();
                }
              else  if (Session["RoleType"].ToString() == "A")//Added 30 june
                {
                    UserId = "Admin";
                }

                else
                {
                    UserId = "";

                }

                var data = layer.AllAppoinmentList(UserId);
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        private void RemoveCache()

        {
            Response.Expires = 0;
            Response.CacheControl = "no-cache";
            Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            Response.Cache.SetNoStore();
            Response.Buffer = true;
            Response.ExpiresAbsolute = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
            Response.AppendHeader("Pragma", "no-cache");
            Response.AppendHeader("", "");
            Response.AppendHeader("Cache-Control", "no-cache"); //HTTP 1.1
            Response.AppendHeader("Cache-Control", "private"); // HTTP 1.1
            Response.AppendHeader("Cache-Control", "no-store"); // HTTP 1.1
            Response.AppendHeader("Cache-Control", "must-revalidate"); // HTTP 1.1
            Response.AppendHeader("Cache-Control", "max-stale=0"); // HTTP 1.1
            Response.AppendHeader("Cache-Control", "post-check=0"); // HTTP 1.1
            Response.AppendHeader("Cache-Control", "pre-check=0"); // HTTP 1.1
            Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.1
                                                         //Response.AppendHeader("Keep-Alive", "timeout=3, max=993"); // HTTP 1.


            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();


        }

        private void testmail()
        {
            //try
            //{
            //    // literal = stringBuilder.ToString();
            //    MailMessage mail = new MailMessage();
            //    //mail.To.Add("jeetu80@gmail.com,keshav.kashyap@karam.in");
            //    mail.To.Add("jeetu80@gmail.com");

            //    mail.From = new MailAddress("karamsupport@karam.in");

            //    mail.Subject = " Test mail from Visitors Management System";

            //    mail.Body = "test";
            //    mail.IsBodyHtml = true;
            //    string KeyHost = ConfigurationManager.AppSettings["KeyHost"].ToString();
            //    string KeyPort = ConfigurationManager.AppSettings["KeyPort"].ToString();
            //    string KeyUId = ConfigurationManager.AppSettings["KeyUId"].ToString();
            //    string KeyPass = ConfigurationManager.AppSettings["KeyPass"].ToString();

            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = KeyHost;
            //    smtp.Port = Convert.ToInt32(KeyPort);
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = new NetworkCredential(KeyUId, KeyPass);
            //    smtp.EnableSsl = true;
            //    smtp.Send(mail);
            //}
            //catch (Exception ex)
            //{
            //    ex.Message.ToString();
            //}


        }
        //---------
        public ActionResult ChangeInwardStatusCancel(FormCollection fc, int Courier_ID_cancel)
        {

            if (Session["NAME"] != null)
            {


                string name = Session["NAME"].ToString();
                var data = db.tblCouriars.Where(m => m.CouriarId == Courier_ID_cancel).FirstOrDefault();
                if (data != null)
                {
                    data.CurrentStatus = "Cancel";

                    //data.EmployeeRemark = "Cancel";
                    //data.EmployeeTransDate = DateTime.Now;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }



            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult EmployeeList()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                bool allowYNS = PageRoletype("S");
                bool allowYND = PageRoletype("D");

                if (allowYNS == false && allowYND == false)

                {
                    return RedirectToAction("../Home/Login");
                }
                var data = layer.EmployeeList;
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }





        }


        public ActionResult UpdateCouriar(int Id)
        {
            string name = Session["NAME"].ToString();
          int  CId =Convert.ToInt32(Id);
            var data = db.tblCouriars.Where(m => m.CouriarId == CId).FirstOrDefault();
            if (data != null)
            {
              string a=  data.CurrentStatus;
                ViewBag.EmpId = data.EmployeeId;
                ViewBag.Empname = data.EmployeeName;
                ViewBag.Deskremark = data.DeskRemarks;
                //string e = data.DeskTransDate;
                //data.Region = ddldept;
                ViewBag.Department = data.Department;
                ViewBag.City = data.City;
                ViewBag.Cdate = Convert.ToDateTime(data.C_Date.ToString()).ToString("dd/MMM/yyyy");
                ViewBag.Region = data.Region;
                ViewBag.NameOfCompany = data.NameOfCompany;
                ViewBag.Documents = data.Documents;
                ViewBag.ReceiveTime = data.ReceiveTime;
                ViewBag.DocketNo = data.DocketNo;
                ViewBag.CouriarVendor = data.CouriarVendor;
                ViewBag.SenderName = data.SenderName;
                string time = data.ReceiveTime;
                DateTime parsedTime = DateTime.ParseExact(time, "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                // Get hours and minutes separately
                ViewBag.hour = parsedTime.Hour;
                ViewBag.minute = parsedTime.Minute;
                ViewBag.AmPm = parsedTime.ToString("tt");
                ViewBag.PacketType = data.PacketType;
                ViewBag.Remark = data.Remark;
                @ViewBag.CouriarId = Id;
                
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCouriarDetail(FormCollection fc)
        {
            try
            {
                tblCouriar objcCouriar = new tblCouriar();

                if (Session["NAME"] != null)
                {
                    int CId = 0;
                    CId = layer.UpdateCourierDetail(fc);
                    if (CId > 0)
                    {
                        TempData["CouriarUpdate"] = "CouriarUpdate";
                    }
                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            catch (Exception ex)
            {
                //ex.Message.ToString();
                layer.SendExcepToDB(ex);
            }


            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult UpdateDispatch(int Id)
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                ///********Acess permission control
                if (Session["RoleType"] != null)
                {
                    //if (Session["RoleType"].ToString() == "D")//D
                    if (Session["RoleType"].ToString() == "D" || Session["RoleTypeBothYN"].ToString() == "Yes")
                    {

                        var disdata = db.tblCouriars.Where(m => m.CouriarId == Id).FirstOrDefault();

                        string employeeId = disdata.EmployeeId; //Session["UserID"].ToString();
                        string Department = disdata.Department;//Session["DEPT"].ToString();
                        ViewBag.Department = disdata.Department;
                        ViewBag.City = disdata.City;
                        ViewBag.Cdate = Convert.ToDateTime(disdata.C_Date.ToString()).ToString("dd/MMM/yyyy");
                        ViewBag.Region = disdata.Region;
                        ViewBag.NameOfCompany = disdata.NameOfCompany;
                        ViewBag.Documents = disdata.Documents;
                        ViewBag.ReceiveTime = disdata.ReceiveTime;
                        //ViewBag.DocketNo = disdata.DocketNo;
                        //ViewBag.CouriarVendor = disdata.CouriarVendor;
                        ViewBag.SenderName = disdata.SenderName;
                        ViewBag.HazardousItemYN = disdata.HazardousItemYN;
                        ViewBag.Person = disdata.Person;
                        string time = disdata.ReceiveTime;

                        DateTime parsedTime = DateTime.ParseExact(time, "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                        // Get hours and minutes separately
                        ViewBag.hour = parsedTime.Hour;
                        ViewBag.minute = parsedTime.Minute;
                        ViewBag.AmPm = parsedTime.ToString("tt");
                        ViewBag.PacketType = disdata.PacketType;
                        ViewBag.Remark = disdata.Remark;
                        @ViewBag.CouriarId = Id;
                        ViewBag.FromCity = disdata.FromCity;
                        ViewBag.Address = disdata.Address;
                        ViewBag.ContactNo = disdata.ContactNo;
                        ViewBag.DeliveryDate = Convert.ToDateTime(disdata.DeliveryDate.ToString()).ToString("dd/MMM/yyyy"); 


                        var data = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == employeeId).FirstOrDefault();
                        ViewBag.Region = data.Region;
                        ViewBag.Name = data.NAME;
                        ViewBag.Department = Department;
                        ViewBag.senderContactNo = data.MOBILE_NUMBER;
                        ViewBag.Employeeid = employeeId;
                        ViewBag.Id = Id;
                       
                        int RegionId = Convert.ToInt32(data.RegionId);

                        DataTable dt = layer.Datatable_QUERY("SELECT  RoleType FROM WorkingLogin where RegionId=" + RegionId + "");
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                @ViewBag.RegionRoleType = dt.Rows[0][0].ToString();
                            }
                            else { @ViewBag.RegionRoleType = ""; }
                        }
                        else { @ViewBag.RegionRoleType = ""; }

                    }
                    else
                    {
                        return RedirectToAction("Login");

                    }

                }
                ///********Acess permission control 


                try
                {


                    //RollPermissionDisPatch();
                    tblCouriar objcCouriar = new tblCouriar();

                    ViewBag.Cdate = Convert.ToString(DateTime.Now.ToString("dd/MMM/yyyy"));


                    RollPermission();

                }
                catch (Exception ex)
                {
                   
                    layer.SendExcepToDB(ex);

                }



                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
            //return View();
        }
        [HttpPost]
        public ActionResult UpdateDispatchDetail(FormCollection fc)
        {
            
            try
            {
                tblCouriar objcCouriar = new tblCouriar();

                if (Session["NAME"] != null)
                {
                    int CId = 0;
                    CId = layer.UpdateDispatchDetail(fc);
                    if (CId > 0)
                    {
                        TempData["DispatchUpdate"] = "DispatchUpdate";
                    }
                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            catch (Exception ex)
            {
               
                layer.SendExcepToDB(ex);
            }


            return Redirect(Request.UrlReferrer.ToString());

           
        }
        public ActionResult DispatchListForDeskPod()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                ///********Acess permission control
                try
                {
                    bool allowYN = PageRoletype("D");
                    if (allowYN == false)
                    {
                        return RedirectToAction("Login");
                    }

                    RollPermission();
                    Couriar data = new Couriar();
                    string UserId = Session["UserID"].ToString();
                    data.CouriarList = layer.DispatchList("Close", "Desk");
                    return View("DispatchListForDeskPod", data.CouriarList);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Login");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase filepod,string Courier_ID)
        {
            if (filepod != null && filepod.ContentLength > 0)
            {
                try
                {
                    int CID = Convert.ToInt32(Courier_ID);
                    // Define the path where the file will be saved
                    string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string filefullname = timeStamp + Courier_ID + ".pdf";

                    //-------
                    string folderPath = Server.MapPath("~/UploadPod");
                    // Ensure the folder exists, create it if not
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    //----------

                    string path = Path.Combine(Server.MapPath("~/UploadPod"),
                             Path.GetFileName(filefullname));//Courier_ID+filepod.FileName
                    var disdata = db.tblCouriars.Where(m => m.CouriarId == CID).FirstOrDefault();
                    // Save the file to the server
                    filepod.SaveAs(path);
                    int CId = 0;
                    //string filename = Courier_ID + filepod.FileName;
                    CId =  layer.UpdatePodDetail(filefullname, CID);
                    if (CId > 0)
                    {
                        TempData["UploadPod"] = "UploadPod";
                    }


                    TempData["UploadPod"] = "UploadPod";
                    //ViewBag.Message = "File uploaded successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "No file selected for upload.";
            }
            return RedirectToAction("DispatchListforDeskPod");
     
        }
        public ActionResult DownloadPdf(int id)
        {
            try
            {
                DataTable dt = layer.Datatable_QUERY("SELECT isnull( PODFile,'') as PODFile FROM tblCouriar where CouriarId=" + id + "");
                string PODFile = dt.Rows[0]["PODFile"].ToString();
                string filePath = Path.Combine(Server.MapPath("~/UploadPod/" + PODFile));
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string fileName = "POD_" + id + ".pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }

        }
        public ActionResult SendEmailInwardTansitPacket()
        {
            try
            {
                SendEmailInwardPacket();
            }
            catch (Exception ex) { }
            return View();

        }
        private void  SendEmailInwardPacket()
        {
            //----------------
            try 
            { 
            string KeyMasterDB = ConfigurationManager.AppSettings["KeyMasterDB"].ToString();
            string sql = "SELECT c.DocketNo, c.CouriarId, mr.[Email ID] as Email, mr.[Name] AS Name FROM tblCouriar c LEFT JOIN " + KeyMasterDB + ".dbo.Master_Requestor mr ON c.EmployeeId = mr.EMPLOYEE_ID WHERE c.CouriarType = 'Inward' AND c.CurrentStatus = 'Intrasit' and DATEDIFF(DAY, DeskTransDate,GETDATE())<=3 ORDER BY c.CouriarId DESC;";
            DataTable dt = new DataTable();
            dt = layer.Datatable_QUERY(sql);
            int CouriarId=0;
            string Name = "";
            string Email = "";
                string docketno = "";
               for (int i = 0; i < dt.Rows.Count; i++)
            {
                CouriarId =Convert.ToInt32(dt.Rows[i]["CouriarId"].ToString());
                Name = dt.Rows[i]["Name"]?.ToString().Trim() ?? "User";
                Email = dt.Rows[i]["Email"]?.ToString().Trim() ?? "";
                    docketno = dt.Rows[i]["DocketNo"]?.ToString().Trim() ?? "";
                    try 
                    { 
                    SendEmailClosedInwardPacket(CouriarId, Name, Email, docketno);
                    }
                    catch (Exception ex) { ex.Message.ToString(); }
                }
            }
            catch (Exception ex) { ex.Message.ToString(); }
        }
        private void SendEmailClosedInwardPacket(int CouriarId,string Name, string Email,string docketno) 
        {
            StringBuilder stringBuilder = new StringBuilder();
            string literal = null;
            stringBuilder.Append("Dear " + Name + ",<br/><br/>");
            stringBuilder.Append("Please close the following Packet Id: " + CouriarId + " for Docket No: " + docketno + ".<br/><br/>");
            stringBuilder.Append("Thank you.<br/><br/>");
            stringBuilder.Append("Best Regards,<br/>");
            stringBuilder.Append("KARAM Team");
            if (Email != "")
            {
                //Email = "jitender@samparksoftwares.com";

                literal = stringBuilder.ToString();
                MailMessage mail = new MailMessage();
                mail.To.Add(Email);
                mail.From = new MailAddress("karamsupport@karam.in");
                //mail.Subject = "Courier confirmation on Visitors Management System";
                mail.Subject = $"Close VMS Inward Packet Id - {CouriarId}";
                mail.Body = literal;
                mail.IsBodyHtml = true;
                string KeyHost = ConfigurationManager.AppSettings["KeyHost"].ToString();
                string KeyPort = ConfigurationManager.AppSettings["KeyPort"].ToString();
                string KeyUId = ConfigurationManager.AppSettings["KeyUId"].ToString();
                string KeyPass = ConfigurationManager.AppSettings["KeyPass"].ToString();

                SmtpClient smtp = new SmtpClient();
                smtp.Host = KeyHost;
                smtp.Port = Convert.ToInt32(KeyPort);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(KeyUId, KeyPass);
                smtp.EnableSsl = true;
                smtp.Send(mail);

            }

        }
        //private void closedInwardpacket()
        //{
        //    //select Couriar,DATEDIFF(DAY, DeskTransDate,GETDATE()) as days    from tblCouriar c where CouriarType='Inward' and CurrentStatus='Intrasit' and RegionId=14 order by CouriarId
        //    //
        //    string sql = "select CouriarId,DATEDIFF(DAY, DeskTransDate,GETDATE()) as days    from tblCouriar  where CouriarType='Inward' and CurrentStatus='Intrasit'  order by CouriarId";
        //    DataTable dt = new DataTable();
        //    dt = layer.Datatable_QUERY(sql);
        //    int CouriarId;
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        CouriarId = Convert.ToInt32(dt.Rows[i]["CouriarId"].ToString());
        //        int days= Convert.ToInt32(dt.Rows[i]["days"].ToString());
        //        if(days>=7)
        //        {
                
        //        }

        //    }

        //}


        //-----------

    }
}
