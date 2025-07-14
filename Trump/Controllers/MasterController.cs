using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Trump.Models;
using TrumpCL;
using System.Data;
using System.Text;
//using Trump.Models;
using System.Collections.Generic;
using static Trump.FilterConfig;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web.Helpers;
using System.Data.SqlClient;
using System.Configuration;
using Org.BouncyCastle.Ocsp;

namespace Trump.Controllers
{
    [ExceptionHandler]
    public class MasterController : Controller
    {
        TrumpEntities db = new TrumpEntities();
        MasterDatabaseEntities login = new MasterDatabaseEntities();
        BusinessLayer layer = new BusinessLayer();
        TrumpService service = new TrumpService();

        string KeyMasterDB = ConfigurationManager.AppSettings["KeyMasterDB"].ToString();
        //SAVIOREntities hc = new SAVIOREntities();
        public ActionResult VisitorCompany()
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}

            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }



            VisitorCount();
            var data = db.tblCompanies.ToList().OrderByDescending(m =>m.C_ID);
            return View(data);
        }

        public ActionResult DeliveryTimeline()
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }

          
            VisitorCount();
            var data = db.tblDeliveryTimelines.Where(z=>z.Status=="Active").ToList();
            return View(data);
        }


        public ActionResult Approver()
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }
            VisitorCount();
            var data = db.tblApprovals.ToList();
            return View(data);
        }
        public ActionResult EditCompany(int EditID)
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}

            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }
            VisitorCount();
            var data = db.tblCompanies.Where(z => z.C_ID == EditID).ToList();
            return View(data);
        }

        //------------
        public ActionResult EditDeliveryTimeline(int EditID)
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }
            VisitorCount();
            var data = db.tblDeliveryTimelines.Where(z => z.Id == EditID).ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult EditDeliveryTimeline(int hidd, FormCollection fc)
        {
            var data = db.tblDeliveryTimelines.Where(z => z.Id == hidd).FirstOrDefault();
            tblDeliveryTimeline tb = new tblDeliveryTimeline();
            data.Source = fc["E_Cname"];
            data.Destination = fc["E_Caddress"];
            data.PackageType = fc["E_PackageType"];
            data.Days = fc["E_Days"];
            data.Status = fc["E_CStatus"];
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
            TempData["CEdit"] = "CEdit";
            //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In").Select(z => z.paycode).Count();
            ViewData["hcw"] = 0;
            ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In").Select(z => z.PNI_code).Count();
            ViewData["hcv"] = db.sp_HCVALL().Select(z => z.Visitor).Count();
            // return Redirect(Request.UrlReferrer.ToString());
            //return View("DeliveryTimeline");
            return RedirectToAction("DeliveryTimeline");

        }

        public ActionResult NewDeliveryTimeline()
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }
            return View();
        
        }
     [HttpPost]
        public ActionResult NewDeliveryTimeline(FormCollection fc)
        {
            //var data = db.tblDeliveryTimelines.Where(z => z.Id == hidd).FirstOrDefault();
            tblDeliveryTimeline tb = new tblDeliveryTimeline();
            tb.Source = fc["E_Cname"];
            tb.Destination = fc["E_Caddress"];
            tb.PackageType = fc["E_PackageType"];
            tb.Days = fc["E_Days"];
            tb.Status = fc["E_CStatus"];
            //db.Entry(data).State = EntityState.Modified;
            //db.SaveChanges();
            db.tblDeliveryTimelines.Add(tb);
            db.SaveChanges();

            TempData["CAdd"] = "CAdd";
            //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In").Select(z => z.paycode).Count();
            ViewData["hcw"] = 0;
            ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In").Select(z => z.PNI_code).Count();
            ViewData["hcv"] = db.sp_HCVALL().Select(z => z.Visitor).Count();
            // return View("DeliveryTimeline");
            return RedirectToAction("DeliveryTimeline");
            //return Redirect(Request.UrlReferrer.ToString());
        }


        //-----------

        
 public ActionResult NewSite()
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}


            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }
            return View();

        }
        [HttpPost]
        public ActionResult NewSite(FormCollection fc)
        {

            if (Session["NAME"] != null)
            {


            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
            //var data = db.tblDeliveryTimelines.Where(z => z.Id == hidd).FirstOrDefault();
            tblSiteDistance tb = new tblSiteDistance();
            tb.Distance = fc["Distance"];
            tb.FromSite = fc["FromSite"];
            tb.ToSite = fc["ToSite"];
           
           
            db.tblSiteDistances.Add(tb);
            db.SaveChanges();

            TempData["CAdd"] = "CAdd";
            
            VisitorCount();
            return RedirectToAction("Site");
            //return Redirect(Request.UrlReferrer.ToString());
        }


        //--------------



        public ActionResult DeleteDeliveryTimeline(string hidden)
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }
            if (hidden == "")
            {
                TempData["error"] = "Please Select CheckBox";
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                string[] KEY = hidden.Split(';');
                foreach (var key in KEY)
                {
                    if (key != "")
                    {
                        int k = Convert.ToInt32(key);

                        tblDeliveryTimeline tb = db.tblDeliveryTimelines.Where(z => z.Id == k).FirstOrDefault();
                        tb.Status = "Inactive";
                        db.Entry(tb).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            TempData["Cdel"] = "Cdel";
            
            VisitorCount();
            return Redirect(Request.UrlReferrer.ToString());
        }



        //----jan17----------



        [HttpPost]
        public ActionResult EditCompany(int hidd, FormCollection fc)
        {
            if (Session["NAME"] != null)
            {


            }
            else
            {
                return RedirectToAction("../Home/Login");
            }

            string phone = fc["E_Cnum"] == null ? "NA" : fc["E_Cnum"].ToUpper();
            string address= fc["E_Caddress"] == null ? "NA" : fc["E_Caddress"].ToUpper(); //fc["E_Caddress"];
            string company= fc["E_Cname"];
            string status = fc["E_CStatus"];
            if (phone == "")
            {
                phone = "NA";
            }



           int cnt= layer.UpdateCompany(hidd, company, address, phone,status);
            //var data = db.tblCompanies.Where(z => z.C_ID == hidd).FirstOrDefault();
            //tblCompany tb = new tblCompany();
            //data.CName = fc["E_Cname"];
            ////data.Addres = fc["E_Caddress"];
            ////data.Phone = ///fc["E_Cnum"];

            //data.Addres = fc["E_Caddress"] == null ? "NA" : fc["E_Caddress"].ToUpper(); //fc["E_Caddress"];
            //data.Phone = phone; ///fc["E_Cnum"];
            //data.RegionId = 0;
            //data.C_Status = fc["E_CStatus"];
            //db.Entry(data).State = EntityState.Modified;
            //db.SaveChanges();
            if (cnt == 1) {
            TempData["CEdit"] = "CEdit";
            }
            VisitorCount();
            //return Redirect(Request.UrlReferrer.ToString());
            return Redirect("VisitorCompany");
            


        }


        public ActionResult AddCompany()
        {
          


            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("Login");
            }
            return View();

        }
        [HttpPost]
        public ActionResult SaveCompany(FormCollection fc)
        {
            if (Session["NAME"] != null)
            {


            }
            else
            {
                return RedirectToAction("../Home/Login");
            }

            string phone = fc["E_Cnum"] == null ? "NA" : fc["E_Cnum"].ToUpper();
            string address = fc["E_Caddress"] == null ? "NA" : fc["E_Caddress"].ToUpper(); //fc["E_Caddress"];
            string company = fc["E_Cname"];
            string status = fc["E_CStatus"];

            if (phone == "")
            {
                phone = "NA";
            }



            string cnt = layer.SaveCompanyforAdmin(company, address, phone, status);
            //var data = db.tblCompanies.Where(z => z.C_ID == hidd).FirstOrDefault();
            //tblCompany tb = new tblCompany();
            //data.CName = fc["E_Cname"];
            ////data.Addres = fc["E_Caddress"];
            ////data.Phone = ///fc["E_Cnum"];

            //data.Addres = fc["E_Caddress"] == null ? "NA" : fc["E_Caddress"].ToUpper(); //fc["E_Caddress"];
            //data.Phone = phone; ///fc["E_Cnum"];
            //data.RegionId = 0;
            //data.C_Status = fc["E_CStatus"];
            //db.Entry(data).State = EntityState.Modified;
            //db.SaveChanges();
           
                TempData["CEdit"] = "CEdit";
           
            VisitorCount();
            //return Redirect(Request.UrlReferrer.ToString());
            return Redirect("VisitorCompany");



        }

        
        public ActionResult DeleteCompany(string hidden)
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
            if (hidden == "")
            {
                TempData["error"] = "Please Select CheckBox";
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                string[] KEY = hidden.Split(';');
                foreach (var key in KEY)
                {
                    if (key != "")
                    {
                        int k = Convert.ToInt32(key);
                        tblCompany tb = db.tblCompanies.Where(z => z.C_ID == k).FirstOrDefault();
                        tb.C_Status = "Inactive";
                        db.Entry(tb).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            TempData["Cdel"] = "Cdel";
           
            VisitorCount();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Visitors()
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            var data = (from v in db.tblVisitors
                        join c in db.tblCompanies
                        on v.C_ID equals c.C_ID
                        //orderby v.C_ID descending


                        select new VisitorViewModel
                        {
                            tbv = v,
                            tbc = c,
                        }
             ).ToList().OrderByDescending(m=>m.tbv.V_ID);
            ViewBag.a = data;
           
            VisitorCount();
            return View(data);
        }
        public ActionResult EditVisitor(int EditID)
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }


            var data = (from v in db.tblVisitors
                        join c in db.tblCompanies
                        on v.C_ID equals c.C_ID
                        where v.V_ID == EditID
                        select new VisitorViewModel
                        {
                            tbv = v,
                            tbc = c,
                        }
            ).FirstOrDefault();
           
            VisitorCount();
            return View(data);
        }
        [HttpPost]
        public ActionResult EditVisitor(int hidd, FormCollection fc)
        {
            var data = db.tblVisitors.Where(z => z.V_ID == hidd).FirstOrDefault();
            data.V_Name = fc["E_Vname"];
            data.V_Phone = fc["E_VPhone"];
            data.V_Email = fc["E_Vemail"];
           //data.Visitor_ID = fc["E_VID"];
            data.V_IDNumber = fc["E_VIDNum"];
            //data.V_Status = fc["E_VStatus"];
            data.C_ID = Convert.ToInt32(fc["E_VComanyID"]);

            data.Visitor_ID = fc["txtidtype"];
            data.V_Status = fc["txtstatus"];

            
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
            TempData["VEdit"] = "VEdit";
           
            VisitorCount();
            return Redirect("Visitors");
        }
        public ActionResult DeleteVisitor(string hidden)
        {
            if (hidden == "")
            {
                TempData["error"] = "Please Select CheckBox";
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                string[] KEY = hidden.Split(';');

                foreach (var key in KEY)
                {
                    if (key != "")
                    {
                        int k = Convert.ToInt32(key);
                        tblVisitor tb = db.tblVisitors.Where(z => z.V_ID == k).FirstOrDefault();
                        tb.V_Status = "Inactive";
                        db.Entry(tb).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            TempData["Cdel"] = "Cdel";
           
            VisitorCount();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult EditApprover(int EditID)
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
            var data = db.tblApprovals.Where(m => m.App_ID == EditID).FirstOrDefault();
            
            VisitorCount();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit_Approver(int App_ID)
        {
            var data = db.tblApprovals.Where(m => m.App_ID == App_ID).FirstOrDefault();
            data.Name_ = Request.Form["Name_"].ToString();
            data.Email = Request.Form["Email"].ToString();
            data.AppStatus = Request.Form["AppStatus"].ToString();//Added
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
           
            VisitorCount();
            return RedirectToAction("Approver");
        }

        public ActionResult VisitType()
        {
            try
            {
                //if (Session["NAME"] != null)
                //{


                //}
                //else
                //{
                //    return RedirectToAction("../Home/Login");
                //}
                bool allowYN = PageRoletype("A");
                if (allowYN == false)
                {
                    return RedirectToAction("../Home/Login");
                }
                var data = db.VisitTypes.ToList();
                ViewBag.a = data;
                
                VisitorCount();
                return View(data);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ActionResult EditVisitType(int EditID)
        {
            try
            {
                //if (Session["NAME"] != null)
                //{


                //}
                //else
                //{
                //    return RedirectToAction("../Home/Login");
                //}
                bool allowYN = PageRoletype("A");
                if (allowYN == false)
                {
                    return RedirectToAction("../Home/Login");
                }
                var data = db.VisitTypes.Where(z => z.V_TY == EditID).FirstOrDefault();
               
                VisitorCount();
                return View(data);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpPost]
        public ActionResult EditVisitType(int hidd, FormCollection fc)
        {
            try
            {
                var data = db.VisitTypes.Where(z => z.V_TY == hidd).FirstOrDefault();
                data.VstTpe = fc["E_VT"];
                data.VStatus = fc["E_VTStatus"];
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                TempData["VTEdit"] = "VTEdit";
               
                VisitorCount();
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public ActionResult DeleteVisitType(string hidden)
        {
            if (hidden == "")
            {
                TempData["error"] = "Please Select CheckBox";
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                string[] KEY = hidden.Split(';');

                foreach (var key in KEY)
                {
                    if (key != "")
                    {
                        try
                        {
                            int k = Convert.ToInt32(key);
                            VisitType tb = db.VisitTypes.Where(z => z.V_TY == k).FirstOrDefault();
                            tb.VStatus = "Inactive";
                            db.Entry(tb).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
            TempData["VTdel"] = "VTdel";
          
            VisitorCount();
            return Redirect(Request.UrlReferrer.ToString());
        }
        [HttpPost]
        public ActionResult AddNewVisitType(string txtVisitType)
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
            VisitType vt = new VisitType();
            vt.VstTpe = txtVisitType;
            vt.VStatus = "Active";
            db.VisitTypes.Add(vt);
            db.SaveChanges();
            TempData["VTnew"] = "VTnew";
           
            VisitorCount();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Site()
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
            var data = db.tblSiteDistances.ToList();
           
            VisitorCount();

            return View(data);
        }
        public ActionResult EditSite(int EditID)
        {
            //  if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
            var data = db.tblSiteDistances.Where(m => m.SD_ID == EditID).FirstOrDefault();
          

            VisitorCount();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit_Site(int Sid)
        {
            var data = db.tblSiteDistances.Where(m => m.SD_ID == Sid).FirstOrDefault();
            data.Distance = Request.Form["Distance"];
            data.FromSite = Request.Form["FromSite"];
            data.ToSite = Request.Form["ToSite"];
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
           

            VisitorCount();
            return RedirectToAction("Site");
        }

        public ActionResult Template()
        {
            if (Session["SITE"] != null)
            {
                bool allowYN = PageRoletype("A");
                if (allowYN == false)
                {
                    return RedirectToAction("../Home/Login");
                }
                string region = Session["SITE"].ToString();
            
                VisitorCount();
                var data = db.Msgtemplates.Where(m => m.Region == region).ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult EditTemplate(int ID)
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            VisitorCount();


            var data = db.Msgtemplates.Where(m => m.M_ID == ID).FirstOrDefault();
            return View(data);
        }

        public ActionResult Edit_Template(int M_ID)
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
            var data = db.Msgtemplates.Where(m => m.M_ID == M_ID).FirstOrDefault();
            string formTemplate = Request.Form["formTemplate"].ToString();
            int para = Regex.Matches(data.Template, "@").Count;
            int para2 = Regex.Matches(formTemplate, "@").Count;
            if (para == para2)
            {
                data.Template = formTemplate;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Template");
            }
            else
            {
                TempData["paranot"] = "paranot";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public ActionResult AppTodayAll()
        {
            //if (Session["NAME"] != null)
            //{


            //}
            //else
            //{
            //    return RedirectToAction("../Home/Login");
            //}
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            VisitorCount();
           var data = db.sp_AppToday().ToList();//2july2023
         
         

            return View(data);
        }

        public ActionResult AppTodayAllforAdmin()
        {
           
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            VisitorCount();
            // var data = db.sp_AppToday().ToList();//2july2023
            var data = layer.AppTodayForAdmin("Admin");


            return View(data);
        }

        public ActionResult AppUpcoming()
        {
            
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            VisitorCount();
            var data = db.sp_AppUpcoming().ToList();
            return View(data);
        }
        public ActionResult AppUpcomingforAdmin()
        {

            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            VisitorCount();
            /// var data = db.sp_AppUpcoming().ToList();
            var data = layer.AppUpcomingForAdmin("Admin");

            return View(data);
        }

        //---------------
        private bool PageRoletype(string Roletype)
        {
            bool allowYN = false;
            if (Session["RoleType"] != null)
            {
                if (Session["RoleType"].ToString() == Roletype)
                {

                    allowYN = true;
                }


            }
            return allowYN;

        }
        //-----------------

        private void VisitorCount()
        {
            //ViewData["hcw"] = hc.sp_HCW().Where(z => z.Current_Status == "In").Select(z => z.paycode).Count();

            //  ViewData["hce"] = login.sp_HCE().Where(z => z.status == "In").Select(z => z.PNI_code).Count();
            ViewData["hcw"] = 0;
            ViewData["hce"] = 0;
            ViewData["hcv"] = db.sp_HCVALL().Select(z => z.Visitor).Count();

        }

        public ActionResult CourierCompany()
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
            List<DataFieldCourierCompany> ItemList = new List<DataFieldCourierCompany>();
            string kee = "select CourierCompanyId,CourierCompany, case when status='Y' then 'Active' else 'Inactive' end as status from tblCourierCompany";
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERY(kee);
            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                DataFieldCourierCompany obj = new DataFieldCourierCompany();
                obj.CourierCompanyId = Convert.ToInt32(ktd.Rows[i]["CourierCompanyId"].ToString());
                obj.CourierCompany = ktd.Rows[i]["CourierCompany"].ToString();
                obj.Status = ktd.Rows[i]["status"].ToString();
                ItemList.Add(obj);
            }
            return View(ItemList.ToList());
           // return Json(ItemList.ToList(), JsonRequestBehavior.AllowGet);




        }


        [HttpPost]
        public ActionResult AddNewCourierCompany(FormCollection fc)
        {
           

            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
            VisitorCount();
            string CourierCompany = Request.Form["CourierCompany"];
            string Status = Request.Form["txtstatus"];
            layer.InsertCourierCompany(fc);
            VisitorCount();
            return RedirectToAction("CourierCompany");


            //return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult EditCourierCompany(int EditID)
        {

            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
            VisitorCount();


            List<DataFieldCourierCompany> ItemList = new List<DataFieldCourierCompany>();
            string kee = "select CourierCompanyId,CourierCompany,status from tblCourierCompany  where CourierCompanyId=" + EditID + " ";
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERY(kee);
            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                DataFieldCourierCompany obj = new DataFieldCourierCompany();
                obj.CourierCompanyId = Convert.ToInt32(ktd.Rows[i]["CourierCompanyId"].ToString());
                obj.CourierCompany = ktd.Rows[i]["CourierCompany"].ToString();
                obj.Status = ktd.Rows[i]["status"].ToString();
                ItemList.Add(obj);
            }
            var data = ItemList.FirstOrDefault();
            return View(data);


        }

        [HttpPost]
        public ActionResult UpdateCourierCompany(int CourierCompanyId, FormCollection fc)
        {
           
            string    CourierCompany= Request.Form["CourierCompany"]; 
            string Status =Request.Form["txtstatus"];
            layer.UpdateCourierCompany(fc, CourierCompanyId);
           
            return RedirectToAction("CourierCompany");
        }

        
             public ActionResult DeleteCourierComany(string hidden)
        {
            if (hidden == "")
            {
                TempData["error"] = "Please Select CheckBox";
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                string[] KEY = hidden.Split(';');

                foreach (var key in KEY)
                {
                    if (key != "")
                    {
                        try
                        {
                            //int k = Convert.ToInt32(key);
                            //VisitType tb = db.VisitTypes.Where(z => z.V_TY == k).FirstOrDefault();
                            //tb.VStatus = "Inactive";
                            //db.Entry(tb).State = EntityState.Modified;
                            //db.SaveChanges();

                            layer.DeleteCourierCompany(Convert.ToInt32(key));

                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
            TempData["CourierComanyDelete"] = "CourierComanyDelete";

            //VisitorCount();
            //return Redirect(Request.UrlReferrer.ToString());
            return RedirectToAction("CourierCompany");
        }
        //**************************************24feb************************
      


      


     

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

        public ActionResult GetDepartmenByRegiontListNew(string Region, string Region2)
        {


            var data = (from v in login.Departments

                        where (v.Region == Region || v.Region== Region2)
                        select new
                        {

                            DepartmentId = v.D_ID,
                            DEPARTMENT = v.DepartmentName
                        }
                     ).ToList().Distinct();



            //Select(m => m.CategoryName).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
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


        public ActionResult GetRegionList(string Region)
        {


            List<DataFieldRegion> ItemList = new List<DataFieldRegion>();
            string kee = "";
           
                kee = "SELECT  RegionId, Region   FROM Region where Status='Y'";
       
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERYMaster(kee);

            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                DataFieldRegion obj = new DataFieldRegion();
                obj.RegionId = Convert.ToInt32(ktd.Rows[i]["RegionId"].ToString());
                obj.Region = ktd.Rows[i]["Region"].ToString();
                ItemList.Add(obj);
            }

            return Json(ItemList.ToList(), JsonRequestBehavior.AllowGet);

            //var data = (from v in login


            //            select new
            //            {

            //                CompanyId = v.MC_ID,
            //                Company = v.CompanyName
            //            }
            //         ).ToList().Distinct();




            //return Json(data, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult MasterEmployeeList()
        //{
        //    Session.LCID = 1033;
        //    if (Session["NAME"] != null)
        //    {
        //        var data = layer.MasterRequesterListNew;
        //        return View(data);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login");
        //    }





        //}

        [HttpPost]
        public ActionResult CancelEntry(string delHidden)
        {
            if (Session["NAME"] != null)
            {

                string[] KEY = delHidden.Split(';');
                foreach (var key in KEY)
                {
                    if (key != "")
                    {
                        int chlnKey = Convert.ToInt32(key);
                        var data = login.Master_Requestor.Where(m => m.Sr_No == chlnKey).FirstOrDefault();//db.tblAppoinments.Where(m => m.A_ID == chlnKey && m.V_Status == "Pre").FirstOrDefault();
                        if (data != null)
                        {
                            var requestor = login.Master_Requestor.Where(m => m.Sr_No == chlnKey).FirstOrDefault();
                            requestor.Status = "Cancel";
                            db.Entry(requestor).State = EntityState.Modified;
                            db.SaveChanges();
                            //----------------

                            //var requestorN = db.Master_RequestorN.Where(m => m.EMPLOYEE_ID == requestor.EMPLOYEE_ID).FirstOrDefault();
                            //requestorN.Status = "Cancel";
                            //db.Entry(requestorN).State = EntityState.Modified;
                            //db.SaveChanges();

                            //var KrRequestor_New = kr.Requestor_New.Where(m => m.PNI_Code == requestor.EMPLOYEE_ID).FirstOrDefault();
                            //KrRequestor_New.Status = "Cancel";
                            //kr.Entry(KrRequestor_New).State = EntityState.Modified;
                            //kr.SaveChanges();
                            //----------------------






                        }
                    }
                }
                return RedirectToAction("MasterRequesterList");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        //++++++++++++++++++++12april------------
        public ActionResult Master_EmployeeEntry()
        {
            if (Session["NAME"] != null)
            {
                //ViewBag.Message = "Your contact page.";

                bool allowYN = PageRoletype("A");
                if (allowYN == false)
                {
                    return RedirectToAction("../Home/Login");
                }


                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }


        }


        public ActionResult Master_EmployeeEdit(int EditID)
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {

                MasterModel data = new MasterModel();


                data.A = login.Master_Requestor.Where(m => m.Sr_No == EditID).FirstOrDefault();
                bool aa = Convert.ToBoolean(data.A.InvestmentDeclarationStatus);
                ViewBag.INdStatus = Convert.ToString(aa);
                ViewBag.Dept = "Customer Relations & Certifications";
                try 
                {

                    string query = "select isnull(ActualRegionId,0) as ActualRegionId from Master_Requestor   where [Sr#No]= " + EditID + "";
                    DataTable dt = layer.Datatable_QUERYMaster(query);
                    int actualregionid = Convert.ToInt32(dt.Rows[0]["ActualRegionId"].ToString());
                    string query2 = "select isnull(Region,'') as Region from Region where  Regionid= " + actualregionid + "";
                    DataTable dt2 = layer.Datatable_QUERYMaster(query2);
                    if (dt2 != null)
                    {
                        if (dt2.Rows.Count > 0)
                        {
                            string actualregion = dt2.Rows[0]["Region"].ToString();
                            ViewBag.ActualRegionId = actualregionid;
                            ViewBag.ActualRegion = actualregion;

                        }
                        else
                        {
                            ViewBag.ActualRegionId = 0;
                            ViewBag.ActualRegion = "--Please Select--";
                        }
                       
                    }
                    else
                    {
                        ViewBag.ActualRegionId = 0;
                        ViewBag.ActualRegion = "--Please Select--";
                    }

                }

                catch(Exception ex)
                {
                    ex.Message.ToString();
                }
               
                return View(data);

            }
            else
            {
                return RedirectToAction("Login");
            }


        }



        [HttpPost]

        public ActionResult SaveRequestorDetail(FormCollection fc)
        {
            try
            {
                Session.LCID = 1033;
                Master_Requestor Obj = new Master_Requestor();

                if (Session["NAME"] != null)
                {

                    string a = layer.SaveRequestor(fc);
                    //string operationType = fc["OperationType"].ToString();

                    //string companyid = fc["ddlCompany"].ToString();
                    //string strtime = DateTime.Now.ToString("hh:mm tt");

                    //Obj.EMPLOYEE_ID = fc["txtEMPLOYEE_ID"].ToString();
                    //Obj.NAME = fc["txtNAME"].ToString();
                    //Obj.KSS_Department = checknullReplaceZero(fc["txtKSSDepartment"].ToString());
                    //Obj.requestor_key = checknullReplaceZero(fc["txtrequestor_key"].ToString());
                    //Obj.DESIGNATION = fc["txtDESIGNATION"].ToString();
                    //Obj.DEPARTMENT = fc["txtDEPARTMENT"].ToString();
                    //Obj.DepartmentID = Convert.ToInt32(fc["txtDepartmentID"].ToString());
                    //Obj.Immediate_Supervisor = fc["txtImmediate_Supervisor"].ToString();
                    //Obj.Immediate_Supervisor_EMP_ID = fc["txtImmediate_Supervisor_EMP_ID"].ToString();
                    //Obj.HOD = fc["txtHOD"].ToString();
                    //Obj.HOD_EMP_ID = fc["txtHOD_EMP_ID"].ToString();
                    ////Obj.MOBILE_NUMBER = Convert.ToDouble(fc["txtMOBILE"].ToString());

                    ////if(IsNullOrEmpty(fc["txtMOBILE"].ToString())
                    //// Obj.MOBILE_NUMBER = Convert.ToDouble(String.IsNullOrEmpty(fc["txtMOBILE"].ToString()) ? "0" : fc["txtMOBILE"].ToString());
                    //Obj.MOBILE_NUMBER = Convert.ToDouble(checknullReplaceZero(fc["txtMOBILE"].ToString()));


                    ////string value = String.IsNullOrEmpty(fc["txtMOBILE"].ToString()) ? "0" : fc["txtMOBILE"].ToString();
                    //Obj.DATE_OF_BIRTH = Convert.ToDateTime(fc["V_Dob"].ToString());
                    //Obj.PAYROLL = checknullReplaceZero(fc["txtPAYROLL"].ToString());
                    //Obj.DATE_OF_JOINING = Convert.ToDateTime(fc["V_Doj"].ToString());
                    //Obj.Email_ID = fc["txtEmail"].ToString();
                    //Obj.VPF = Convert.ToDouble(checknullReplaceZero(fc["txtVPF"].ToString()));
                    //Obj.EPF = Convert.ToDouble(checknullReplaceZero(fc["txtEPF"].ToString()));
                    //Obj.PAN_NO_ = checknullReplaceZero(fc["txtPANNO"].ToString());
                    //Obj.Total_deductions_ = Convert.ToDouble(checknullReplaceZero(fc["txtDeduction"].ToString()));
                    //Obj.Yearly_VPF = Convert.ToDouble(checknullReplaceZero(fc["txtYVPF"].ToString()));
                    //Obj.Yearly_EPF = Convert.ToDouble(checknullReplaceZero(fc["txtYEPF"].ToString()));
                    //Obj.Password = fc["txtPassword"].ToString();
                    ////Obj.Status = fc["txtStatus"].ToString();

                    //Obj.Status = fc["E_VStatus"].ToString();
                    //Obj.UserType = checknullReplaceZero(fc["ddlUserType"].ToString());


                    //if (operationType == "Routing")
                    //{
                    //    Obj.InvestmentDeclarationStatus = true; //fc["txtInvestmentDeclarationStatus"].ToString();
                    //}
                    //else
                    //{
                    //    Obj.InvestmentDeclarationStatus = false;
                    //}



                    //Obj.Investment_Declaration_Password = fc["txtInvestment_Declaration_Password"].ToString();
                    //Obj.HRMSPassword = fc["txtHRMSPassword"].ToString();
                    //Obj.TrumpPassword = fc["txtTrumpPassword"].ToString();
                    ////Obj.CompanyID = Convert.ToInt32(fc["txtCompanyID"].ToString());
                    //Obj.CompanyID = Convert.ToInt32(fc["ddlCompany"].ToString());
                    //Obj.Region = fc["ddlRegion"].ToString();




                    ////----------------------------
                    //login.Master_Requestor.Add(Obj);
                    //login.SaveChanges();


                    // //-------------
                    TempData["Empcreate"] = "Empcreate";

                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                //  layer.SendExcepToDB(ex);
            }

            //TempData["Feedback"] = "Feedback";
            return Redirect(Request.UrlReferrer.ToString());
        }

        private string checknullReplaceZero(string fcval)
        {

            string value = String.IsNullOrEmpty(fcval) ? "0" : fcval;
            return value;
        }
       // ---------------
        //UpdateRequestorDetail
        [HttpPost]

        public ActionResult UpdateRequestorDetail(FormCollection fc)
        {
            try
            {

                Session.LCID = 1033;

                if (Session["NAME"] != null)

                {



                

                    string strtime = DateTime.Now.ToString("hh:mm tt");

                    //int srno = Convert.ToInt32(fc["hiddenSr_No"].ToString());
                    string stremployeeid = fc["txtEMPLOYEE_ID"].ToString();
                   // var Obj = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == stremployeeid).FirstOrDefault();

                   string a = layer.UpdateRequestor(stremployeeid,fc);

                    //Obj.EMPLOYEE_ID = stremployeeid; //fc["txtEMPLOYEE_ID"].ToString();
                    //Obj.NAME = fc["txtNAME"].ToString();
                    //Obj.KSS_Department = checknullReplaceZero(fc["txtKSSDepartment"].ToString());
                    //Obj.requestor_key = checknullReplaceZero(fc["txtrequestor_key"].ToString());
                    //Obj.DESIGNATION = fc["txtDESIGNATION"].ToString();
                    //Obj.DEPARTMENT = fc["txtDEPARTMENT"].ToString();
                    //Obj.DepartmentID = Convert.ToInt32(fc["txtDepartmentID"].ToString());
                    //Obj.Immediate_Supervisor = fc["txtImmediate_Supervisor"].ToString();
                    //Obj.Immediate_Supervisor_EMP_ID = checknullReplaceZero(fc["txtImmediate_Supervisor_EMP_ID"].ToString());
                    //Obj.HOD = checknullReplaceZero(fc["txtHOD"].ToString());
                    //Obj.HOD_EMP_ID = checknullReplaceZero(fc["txtHOD_EMP_ID"].ToString());
                    //Obj.MOBILE_NUMBER = Convert.ToDouble(checknullReplaceZero(fc["txtMOBILE"].ToString()));
                    //Obj.DATE_OF_BIRTH = Convert.ToDateTime(fc["V_Dob"].ToString());
                    //Obj.PAYROLL = checknullReplaceZero(fc["txtPAYROLL"].ToString());
                    //Obj.DATE_OF_JOINING = Convert.ToDateTime(fc["V_Doj"].ToString());
                    //Obj.Email_ID = fc["txtEmail"].ToString();
                    //Obj.VPF = Convert.ToDouble(checknullReplaceZero(fc["txtVPF"].ToString()));
                    //Obj.EPF = Convert.ToDouble(checknullReplaceZero(fc["txtEPF"].ToString()));
                    //Obj.PAN_NO_ = checknullReplaceZero(fc["txtPANNO"].ToString());
                    //Obj.Total_deductions_ = Convert.ToDouble(checknullReplaceZero(fc["txtDeduction"].ToString()));
                    //Obj.Yearly_VPF = Convert.ToDouble(checknullReplaceZero(fc["txtYVPF"].ToString()));
                    //Obj.Yearly_EPF = Convert.ToDouble(checknullReplaceZero(fc["txtYEPF"].ToString()));
                    //Obj.Password = fc["txtPassword"].ToString();
                    //Obj.Status = fc["ddlStatus"].ToString();
                    //Obj.UserType = fc["ddlUserType"].ToString();
                    ////Obj.InvestmentDeclarationStatus = true; //fc["txtInvestmentDeclarationStatus"].ToString();
                    //if (operationType == "Routing")
                    //{
                    //    Obj.InvestmentDeclarationStatus = true; //fc["txtInvestmentDeclarationStatus"].ToString();
                    //}
                    //else
                    //{
                    //    Obj.InvestmentDeclarationStatus = false;
                    //}


                    //Obj.Investment_Declaration_Password = fc["txtInvestment_Declaration_Password"].ToString();
                    //Obj.HRMSPassword = fc["txtHRMSPassword"].ToString();
                    //Obj.TrumpPassword = fc["txtTrumpPassword"].ToString();
                    //Obj.CompanyID = Convert.ToInt32(fc["ddlCompany"].ToString());
                    //Obj.Region = fc["ddlRegion"].ToString();

                    //login.Entry(Obj).State = EntityState.Modified;


                    //login.SaveChanges();




                    TempData["EmpUpdate"] = "EmpUpdate";

                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            catch (Exception ex)
            {
                //ex.Message.ToString();
                //  layer.SendExcepToDB(ex);
            }

            //TempData["Feedback"] = "Feedback";
            // return Redirect(Request.UrlReferrer.ToString());

            return RedirectToAction("MasterEmployeeList");
        }

        public ActionResult MasterEmployeeList()
        {
            Session.LCID = 1033;
            if (Session["NAME"] != null)
            {
                bool allowYN = PageRoletype("A");
                if (allowYN == false)
                {
                    return RedirectToAction("../Home/Login");
                }
                var data = layer.MasterRequesterListNew;
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }





        }

        [HttpPost]
        public ActionResult EmployeeCancelEntry(string delHidden)
        {
            if (Session["NAME"] != null)
            {

                string[] KEY = delHidden.Split(';');
                foreach (var key in KEY)
                {
                    if (key != "")
                    {
                        int chlnKey = Convert.ToInt32(key);
                        var data = login.Master_Requestor.Where(m => m.Sr_No == chlnKey).FirstOrDefault();//db.tblAppoinments.Where(m => m.A_ID == chlnKey && m.V_Status == "Pre").FirstOrDefault();
                        if (data != null)
                        {
                            var requestor = login.Master_Requestor.Where(m => m.Sr_No == chlnKey).FirstOrDefault();
                            //requestor.Status = "Exit";
                            requestor.Status = "InActive";
                            login.Entry(requestor).State = EntityState.Modified;
                            login.SaveChanges();







                        }
                    }
                }
                return RedirectToAction("MasterEmployeeList");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        public JsonResult EmployeeIdExists(string Employeeid)
        {

            if (login.Master_Requestor.Any(m => m.EMPLOYEE_ID == Employeeid) == true)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EmployeeEmailExists(string Email,string EmployeeId)
        {
            string query = " select  Email from WorkingLogin where Email= '" + Email + "'";
            int cnt = layer.CheckExists(query);
            int cntWorking;
            if (EmployeeId == "0")
            {
                if (login.Master_Requestor.Any(m => m.Email_ID == Email) == true)
                {
                    //return Json(1, JsonRequestBehavior.AllowGet);
                    cntWorking = 1;
                }
                else
                {
                    cntWorking = 0;
                    //return Json(0, JsonRequestBehavior.AllowGet);
                }


            }

            else 
            {

                if (login.Master_Requestor.Any(m => m.Email_ID == Email && m.EMPLOYEE_ID!= EmployeeId) == true)
                {
                    cntWorking = 1;
                    //return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    cntWorking = 0;
                    //return Json(0, JsonRequestBehavior.AllowGet);
                }




            }
            if (cnt == 1 || cntWorking == 1)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }



        }

        public JsonResult EmployeeMobileExists(double Mobile, string EmployeeId)
        {
            if (EmployeeId == "0")
            {
                if (login.Master_Requestor.Any(m => m.MOBILE_NUMBER == Mobile) == true)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }


            }

            else
            {

                if (login.Master_Requestor.Any(m => m.MOBILE_NUMBER == Mobile && m.EMPLOYEE_ID != EmployeeId) == true)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }




            }

        }

        //private string checknullReplaceZero(string fcval)
        //{

        //    string value = String.IsNullOrEmpty(fcval) ? "0" : fcval;
        //    return value;
        //}

        //-----------------------12april

        //***********************
        //==========

        //-----------26 June 2023 -----------
        //--------------WORK----Start withSQL of Code-Region----------------
        public ActionResult RegionDepartment()
        {

            List<ReportFieldRegion> ItemList = new List<ReportFieldRegion>();
            string kee = "SELECT        R.RegionId, R.Region, R.Status, D.DepartmentName, D.D_ID FROM    dbo.Region AS R INNER JOIN  dbo.Department AS D ON R.Region = D.Region";
            var ktd = new DataTable();
            //ktd = layer.Datatable_QUERY(kee);

            //------------
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            //---------


            ktd = ktd = layer.Datatable_QUERYMaster(kee);
            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                ReportFieldRegion obj = new ReportFieldRegion();
                obj.RegionId = Convert.ToInt32(ktd.Rows[i]["D_ID"].ToString());
                obj.Region = ktd.Rows[i]["Region"].ToString();
                obj.Status = ktd.Rows[i]["DepartmentName"].ToString();

                ItemList.Add(obj);
            }
            return View(ItemList.ToList());
            // return Json(ItemList.ToList(), JsonRequestBehavior.AllowGet);
        }

        
         public ActionResult NewRegionDepartment()
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            return View();
        }



        [HttpPost]
        public ActionResult RegionDepartment(FormCollection fc)
        {


          string  check=  layer.InsertRegionDepartment(fc);
            if (check =="1")
            {
                TempData["RegionDepartmentInsert"] = "RegionDepartmentInsert";
                return RedirectToAction("RegionDepartment");
            }
            else {
                TempData["NotRegionDepartmentInsert"] = "NotRegionDepartmentInsert";
                return Redirect(Request.UrlReferrer.ToString());

            }
            //return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult EditRegionDepartment(int EditID)
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
           
            List<ReportFieldRegion> ItemList = new List<ReportFieldRegion>();
            string kee = "SELECT        R.RegionId, R.Region, R.Status, D.DepartmentName, D.D_ID FROM    dbo.Region AS R Left JOIN  dbo.Department AS D ON R.Region = D.Region where D_ID=" + EditID + " ";
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERYMaster(kee);
            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                ReportFieldRegion obj = new ReportFieldRegion();
                obj.RegionId = Convert.ToInt32(ktd.Rows[i]["D_ID"].ToString());
                obj.Region = ktd.Rows[i]["Region"].ToString();
                obj.Status = ktd.Rows[i]["DepartmentName"].ToString();

                ItemList.Add(obj);
            }
            //var data = ItemList.FirstOrDefault();
            var data = ItemList.ToList();
            return View(data); ;
        }

        [HttpPost]
        public ActionResult UpdateRegionDepartment(int txtRegionId, FormCollection fc)
        {


            string check = layer.UpdateRegionDepartment(fc, txtRegionId);
            if (check == "1")
            {
                TempData["RegionDepartmentUpdate"] = "RegionDepartmentUpdate";
                return RedirectToAction("RegionDepartment");
            }
            else
            {
                TempData["NotRegionDepartmentUpdate"] = "NotRegionDepartmentUpdate";
                return Redirect(Request.UrlReferrer.ToString());

            }
            //return Redirect(Request.UrlReferrer.ToString());
        }


        //UpdateRegionDepartment
        public ActionResult Region()
        {

            List<ReportFieldRegion> ItemList = new List<ReportFieldRegion>();
            string kee = "select * from Region";
            var ktd = new DataTable();
            //ktd = layer.Datatable_QUERY(kee);

            //------------
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }
        
            //---------


            ktd = ktd = layer.Datatable_QUERYMaster(kee);
            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                ReportFieldRegion obj = new ReportFieldRegion();
                obj.RegionId = Convert.ToInt32(ktd.Rows[i]["RegionId"].ToString());
                obj.Region = ktd.Rows[i]["Region"].ToString();
                obj.Status = ktd.Rows[i]["Status"].ToString();

                ItemList.Add(obj);
            }
            return View(ItemList.ToList());
            // return Json(ItemList.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult RegionEntry()
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult SaveRegion(FormCollection fc)
        {


            layer.InsertRegion(fc);
            TempData["RegionInsert"] = "RegionInsert";
            return RedirectToAction("Region");
            //return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult EditRegion(int EditID)
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            List<ReportFieldRegion> ItemList = new List<ReportFieldRegion>();
            string kee = "select * from Region  where RegionId=" + EditID + " ";
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERYMaster(kee);
            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                ReportFieldRegion obj = new ReportFieldRegion();

                obj.RegionId = Convert.ToInt32(ktd.Rows[i]["RegionId"].ToString());
                obj.Region = ktd.Rows[i]["Region"].ToString();
                obj.Status = ktd.Rows[i]["Status"].ToString();

                ItemList.Add(obj);
            }
            //var data = ItemList.FirstOrDefault();
            var data = ItemList.ToList();
            return View(data);


        }

        [HttpPost]
        public ActionResult UpdateRegion(int RegionId, FormCollection fc)
        {

            layer.UpdateRegion(fc, RegionId);
            TempData["RegionUpdate"] = "RegionUpdate";
            return RedirectToAction("Region");
        }

       
        public ActionResult DeleteRegion(string hidden)
        {
            if (hidden == "")
            {
                TempData["error"] = "Please Select CheckBox";
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                string[] KEY = hidden.Split(';');

                foreach (var key in KEY)
                {
                    if (key != "")
                    {
                        try
                        {


                            layer.DeleteRegion(Convert.ToInt32(key));

                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
            TempData["RegionDelete"] = "RegionDelete";
            return RedirectToAction("Region");
        }



        //---------------WORK----Start withSQL of Code-WorkingLogin----------------

        public ActionResult WorkingLogin()
        {
            // int RegionId = Convert.ToInt32(Session["RegionId"]);
            //List<WorkingLogin> ItemList = new List<WorkingLogin>();
            List<DataFieldWorkingLogin> ItemList = new List<DataFieldWorkingLogin>();
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }



            // string kee = "select * from WorkingLogin where RegionId="+ RegionId + "";
            string kee = "select *,(Select Region from  " + KeyMasterDB + ".[dbo].Region where RegionId=a.RegionId) as Region, Case When RoleType='S' Then 'Security'  When RoleType='D' Then 'Desk' When RoleType='B' Then 'Both' else '' end as Role   from WorkingLogin a where a.RoleType!='A' ";
            var ktd = new DataTable();
           
            ktd = layer.Datatable_QUERY(kee);
            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                //WorkingLogin obj = new WorkingLogin();
                DataFieldWorkingLogin obj = new DataFieldWorkingLogin();
                
                obj.W_ID = Convert.ToInt32(ktd.Rows[i]["W_ID"].ToString());
                obj.UserID = ktd.Rows[i]["UserID"].ToString();
                obj.UPassword = ktd.Rows[i]["UPassword"].ToString();
                obj.S_Location = ktd.Rows[i]["S_Location"].ToString();
                obj.WStatus = ktd.Rows[i]["WStatus"].ToString();
                //  obj.RoleType = ktd.Rows[i]["RoleType"].ToString();
                obj.RoleType = ktd.Rows[i]["Role"].ToString();
                
                obj.Email = ktd.Rows[i]["Email"].ToString();
                obj.RegionId = Convert.ToInt32(ktd.Rows[i]["RegionId"].ToString());
                obj.Region= ktd.Rows[i]["Region"].ToString();
                ItemList.Add(obj);
            }
            return View(ItemList.ToList());
            // return Json(ItemList.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewWorkingLogin()
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult SaveWorkingLogin(FormCollection fc)
        {
            string strEmail = checknullReplaceZero(fc["txtEmail"].ToString());
            //int cnt = layer.CheckEmployeeEmailExists(strEmail);
            //if (cnt == 0)
            //{
               string check=  layer.InsertWorkingLogin(fc);

                if (check == "0")
                {
                    TempData["WorkingLoginNotInsert"] = "WorkingLoginNotInsert";
                    
                }
                else 
                {
                    TempData["WorkingLoginInsert"] = "WorkingLoginInsert";
                }
                return RedirectToAction("NewWorkingLogin");
            //}
            //else 
            //{

            //    TempData["EmployeeEmailExists"] = "EmployeeEmailExists";
               
            //   return Redirect(Request.UrlReferrer.ToString());
             
                
            //}
            
           // TempData["WorkingLoginInsert"] = "WorkingLoginInsert";
           // return RedirectToAction("NewWorkingLogin");
            //return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult EditWorkingLogin(int EditID)
        {
            bool allowYN = PageRoletype("A");
            if (allowYN == false)
            {
                return RedirectToAction("../Home/Login");
            }


            List<WorkingLogin> ItemList = new List<WorkingLogin>();
            string kee = "select * from WorkingLogin  where W_ID=" + EditID + " ";
            var ktd = new DataTable();
            ktd = layer.Datatable_QUERY(kee);
            for (int i = 0; i < ktd.Rows.Count; i++)
            {
                WorkingLogin obj = new WorkingLogin();

                obj.W_ID = Convert.ToInt32(ktd.Rows[i]["W_ID"].ToString());
                obj.UserID = ktd.Rows[i]["UserID"].ToString();
                obj.UPassword = ktd.Rows[i]["UPassword"].ToString();
                obj.S_Location = ktd.Rows[i]["S_Location"].ToString();
                obj.WStatus = ktd.Rows[i]["WStatus"].ToString();
                obj.RoleType = ktd.Rows[i]["RoleType"].ToString();
                obj.Email = ktd.Rows[i]["Email"].ToString();
                obj.RegionId = Convert.ToInt32(ktd.Rows[i]["RegionId"].ToString());

                ItemList.Add(obj);
            }
            //var data = ItemList.FirstOrDefault();
            var data = ItemList.ToList();
            return View(data);


        }

        [HttpPost]
        public ActionResult UpdateWorkingLogin(int txtW_ID, FormCollection fc)
        {

         int check= layer.UpdateWorkingLogin(fc, txtW_ID);

            if (check == 0)
            {
                TempData["WorkingLoginNotUpdate"] = "WorkingLoginNotUpdate";
               
            }
            else
            {
                TempData["WorkingLoginUpdate"] = "WorkingLoginUpdate";
            }
            TempData["WorkingLoginUpdate"] = "WorkingLoginUpdate";
            return RedirectToAction("WorkingLogin");
        }


        //public ActionResult DeleteWorkingLogin(string hidden)
        //{
        //    if (hidden == "")
        //    {
        //        TempData["error"] = "Please Select CheckBox";
        //        return Redirect(Request.UrlReferrer.ToString());
        //    }
        //    else
        //    {
        //        string[] KEY = hidden.Split(';');

        //        foreach (var key in KEY)
        //        {
        //            if (key != "")
        //            {
        //                try
        //                {


        //                    SqlLayer.DeleteWorkingLogin(Convert.ToInt32(key));

        //                }
        //                catch (Exception e)
        //                {
        //                    throw e;
        //                }
        //            }
        //        }
        //    }
        //    TempData["WorkingLoginDelete"] = "WorkingLoginDelete";
        //    return RedirectToAction("WorkingLogin");
        //}
        //----------------End sql---------------------





        //----------------End sql---------------------

        //-----------------26 june 2023
        //------------
        public JsonResult CheckWorkingLoginEmailExists(string Email,int WId)
        {

            string query = "";
            if (WId == 0)
            {
                query = " select  Email from WorkingLogin where Email= '" + Email + "'";
            }
            else 
            {
                query = " select  Email from WorkingLogin where Email= '" + Email + "' and W_ID!=" + WId + "";
            }

            int cnt = layer.CheckExists(query);
            //----------
            int cntWorking;
            if (login.Master_Requestor.Any(m => m.Email_ID == Email) == true)
            {
                cntWorking = 1;
            }
            else
            {
                cntWorking = 0;
            }

            //-------------
            if (cnt == 1 || cntWorking==1)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
           
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CheckWorkingLoginUserIdExists(string UserId, int WId)
        {
            string query = "";
            if (WId == 0)

            {
                query = " select  UserID from WorkingLogin where UserID= '" + UserId + "'";
            }
            else
            {
                query = " select  UserID from WorkingLogin where UserID= '" + UserId + "' and W_ID!=" + WId + "";
            }
      
                 
            int cnt = layer.CheckExists(query);
            if (cnt == 1)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetRegionIdByEmpId(string EmpId)
        {


            string query = "  select RegionId from Master_Requestor   where EMPLOYEE_ID= '" + EmpId + "'";
           DataTable dt = layer.Datatable_QUERYMaster(query);
            int RegionId = 0;
            if (dt != null)
            {
                RegionId = Convert.ToInt32(dt.Rows[0]["RegionId"].ToString());
                return Json(RegionId, JsonRequestBehavior.AllowGet);
            }
            

            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        // CheckWorkingLoginUserIdExists
        //--------------

        //---------
        public JsonResult DepartmentExists(string Dept, string Region, int Did)
        {
            string query;
            if (Did == 0)
            {
                 query = "select D_ID from Department where DepartmentName ='" + Dept + "' and  Region ='" + Region + "'";

            }
            else
            { 

             query = "select D_ID from Department where DepartmentName ='" + Dept + "' and  Region ='" + Region + "' and  D_ID != " + Did + "";
            }
            int cnt = layer.CheckExistsmaster(query);
           
           
            if (cnt == 1 )
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }



        }
        //--------------
       


    }
}