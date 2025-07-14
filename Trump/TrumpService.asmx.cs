using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace Trump
{
    /// <summary>
    /// Summary description for Trump
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TrumpService : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> GetVendorMBL(string Prefix)
        {
            List<string> empResult = new List<string>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrumpConn"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                    cmd.CommandText = "select distinct top 10 V_Phone from tblVisitor where V_Status='Active' and  V_Phone  LIKE ''+@SearchEmpName+'%'";// and RegionId=" + RegionId + "
                    //cmd.CommandText = "select distinct top 10 V_Phone,V_ID from tblVisitor where V_Phone LIKE ''+@SearchEmpName+'%' order by V_ID desc";
                    //cmd.CommandText = "Select distinct top 10 V_Phone from (select distinct top 10 V_Phone,V_ID from tblVisitor where V_Phone LIKE ''+@SearchEmpName+'%' order by V_ID desc)a";

                    cmd.Connection = con;
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchEmpName", Prefix);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        empResult.Add(string.Format("{0}", dr["V_Phone"]));
                    }
                    con.Close();
                    return empResult;
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> GetExecutive(string Prefix)
        {
            List<string> empResult = new List<string>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Master"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                    // cmd.CommandText = "select distinct top 10 R.NAME,D.DepartmentName,R.EMPLOYEE_ID,R.[Email ID],R.DESIGNATION,R.[MOBILE NUMBER], from Master_Requestor R,Department D where R.DEPARTMENTID = D.D_ID and R.NAME LIKE ''+@SearchEmpName+'%'";
                    cmd.CommandText = "select distinct top 10 R.NAME,D.DepartmentName,R.EMPLOYEE_ID,R.[Email ID],R.DESIGNATION,R.[MOBILE NUMBER],R.Region from Master_Requestor R,Department D where R.DEPARTMENTID = D.D_ID and R.NAME LIKE ''+@SearchEmpName+'%'  and R.RegionId=" + RegionId + " and Status='Active'";
                    cmd.Connection = con;
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchEmpName", Prefix);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        //empResult.Add(string.Format("{0}/{1}/{2}/{3}/{4}/{5}", dr["NAME"],dr["DepartmentName"],dr["EMPLOYEE_ID"],dr["Email ID"],dr["DESIGNATION"],dr["MOBILE NUMBER"]));
                        empResult.Add(string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", dr["NAME"], dr["DepartmentName"], dr["EMPLOYEE_ID"], dr["Email ID"], dr["DESIGNATION"], dr["MOBILE NUMBER"], dr["Region"]));
                    }
                    con.Close();
                    return empResult;
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> OutwordContact(string Prefix)
        {
            List<string> empResult = new List<string>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Master"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "select distinct top 10 R.requestor_key,R.[KSS Department],R.EMPLOYEE_ID,R.[MOBILE NUMBER] from Master_Requestor R,Department D where R.DEPARTMENTID = D.D_ID and R.NAME LIKE ''+@SearchEmpName+'%'";
                    cmd.Connection = con;
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchEmpName", Prefix);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        empResult.Add(string.Format("{0}/{1}/{2}", dr["requestor_key"], dr["KSS Department"], dr["MOBILE NUMBER"]));
                    }
                    con.Close();
                    return empResult;
                }
            }
        }

        [WebMethod()]
        public static bool SaveCapturedImage(string data,string VID)
        {
            string fileName = VID;

            //Convert Base64 Encoded string to Byte Array.
            byte[] imageBytes = Convert.FromBase64String(data.Split(',')[1]);

            //------------Addedjan 3----
            //string CurrentFileName = HttpContext.Current.Server.MapPath("~/Content/Profilpic/" + VID + ".jpg");
            //FileInfo fileinfo = new FileInfo(CurrentFileName);
            //if (fileinfo.Exists)
            //{
            //    fileinfo.Delete();
            //}

            //------------------


            //Save the Byte Array as Image File.
            string filePath = HttpContext.Current.Server.MapPath(string.Format("~/Content/Profilpic/{0}.jpg", fileName));
            File.WriteAllBytes(filePath, imageBytes);
            return true;
        }
        
    }
}
