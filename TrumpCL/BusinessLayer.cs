using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Collections;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Text;
using static QRCoder.PayloadGenerator;
using iTextSharp.xmp.impl;
using System.CodeDom;

namespace TrumpCL
{
    public class BusinessLayer
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["TrumpConn"].ConnectionString);
        SqlConnection Mastercon = new SqlConnection(ConfigurationManager.ConnectionStrings["Master"].ConnectionString);
        //MasterDB
        string KeyMasterDB = ConfigurationManager.AppSettings["KeyMasterDB"].ToString();

        public string UpdatePassword(string password, string PNI)
        {
            string i = "";
            try
            {
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                SqlCommand cmd = new SqlCommand("update Master_Requestor set TrumpPassword ='" + password + "' where EMPLOYEE_ID='" + PNI + "'", Mastercon);
                i = cmd.ExecuteNonQuery().ToString();
            }
            catch
            {

            }
            finally
            {
                if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
            }
            return i;
        }

        public void SendExcepToDB(Exception ex)
        {
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }

                SqlCommand cmd = new SqlCommand("sp_ExceptionLogger", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ExceptionMessage", ex.Message);
                cmd.Parameters.AddWithValue("@ExceptionStackTrace", ex.StackTrace);
                cmd.Parameters.AddWithValue("@ControllerName", HttpContext.Current.Request.Url.AbsoluteUri);
                cmd.Parameters.AddWithValue("@IPAddress", HttpContext.Current.Request.UserHostAddress);
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
        }

        public string SaveCompany(Company com)
        {
            string CID = "";
            try
            {
                int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                if (con.State == ConnectionState.Closed) { con.Open(); }
                string address = com.Addres == null ? "NA" : com.Addres.ToUpper();

                SqlCommand cmd = new SqlCommand("sp_Company", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", com.CName.ToUpper());
                cmd.Parameters.AddWithValue("@Addres", address);
                cmd.Parameters.AddWithValue("@phone", com.Phone);
                cmd.Parameters.AddWithValue("@gst", com.GST);
                cmd.Parameters.AddWithValue("@city", com.city);
                cmd.Parameters.AddWithValue("@state", com.state);
                cmd.Parameters.AddWithValue("@pin", com.PIN);
                cmd.Parameters.AddWithValue("@RegionId", RegionId);
                SqlParameter sqlparam = new SqlParameter();
                sqlparam.ParameterName = "@ID";
                sqlparam.SqlDbType = SqlDbType.Int;
                sqlparam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlparam);
                int i = cmd.ExecuteNonQuery();
                CID = sqlparam.Value.ToString();
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return CID;
        }

        //Requestor recent visitor
        public List<DataField> RecentVisitor
        {
            get
            {
                List<DataField> Visitor = new List<DataField>();
                try
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    string empCode = HttpContext.Current.Session["PNICODE"].ToString();
                    int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);

                    SqlCommand cmd = new SqlCommand("select top(15)A.A_ID, A.V_DateFrom, V.V_Phone,C.CName,V.V_Name,A.V_Status,A.Visitor_Type from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.Requestor = '" + empCode + "' and A.V_Status not in ('Cancel','Reject') and A.RegionId=" + RegionId + " order by A.A_ID desc", con);

                  


                   

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            DataField lst = new DataField();
                            lst.Phone = rdr["V_Phone"].ToString();
                            lst.comName = rdr["CName"].ToString();
                            lst.VisDate = Convert.ToDateTime(rdr["V_DateFrom"]);
                            lst.visName = rdr["V_Name"].ToString();
                            lst.Status = rdr["V_Status"].ToString();
                            lst.Type = rdr["Visitor_Type"].ToString();
                            
                          
                            Visitor.Add(lst);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendExcepToDB(ex);
                }
                finally
                {
                    if (con.State == ConnectionState.Open) { con.Close(); }
                }
                return Visitor;
            }
        }

        public IEnumerable<DataField> RecentMaterial
        {
            get
            {
                List<DataField> Visitor = new List<DataField>();
                try
                {
                    string dept = HttpContext.Current.Session["PNICODE"].ToString();
                    SqlCommand cmd = new SqlCommand("select top(5)A.A_ID, A.V_DateFrom, V.V_Phone,C.CName,V.V_Name from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.Requestor = '" + dept + "' and A.V_Status !='Close' order by A.A_ID desc", con);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            DataField ecn = new DataField();
                            ecn.VisDate = Convert.ToDateTime(rdr["V_DateFrom"]);
                            ecn.Phone = rdr["V_Phone"].ToString();
                            ecn.comName = rdr["CName"].ToString();
                            ecn.visName = rdr["V_Name"].ToString();
                            Visitor.Add(ecn);
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    con.Close();
                }
                return Visitor;
            }
        }

        public List<DataField> RecentVisitorSecurity
        {
            get
            {
                List<DataField> Visitor = new List<DataField>();
                try
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                    //string location = HttpContext.Current.Session["LOCATION"].ToString();
                    //SqlCommand cmd = new SqlCommand("select top(15)A.A_ID, A.V_DateFrom,A.Requestor, V.V_Phone,C.CName,V.V_Name,A.V_Status,A.Visitor_Type from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and (A.V_Status != 'Cancel' and A.V_Status != 'Reject') and A.Valid_at like '%" + location + "%' and A.RegionId=" + RegionId + " order by A.A_ID desc", con);
                  
                    string location = HttpContext.Current.Session["LOCATION"].ToString();
                    SqlCommand cmd = new SqlCommand("select top(15)A.A_ID, A.V_DateFrom,A.Requestor, V.V_Phone,C.CName,V.V_Name,A.V_Status,A.Visitor_Type,isnull(M.NAME,'')  as ReqName from tblAppoinment A left join " + KeyMasterDB + ".[dbo].[Master_Requestor] M on A.Requestor=M.EMPLOYEE_ID , tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and (A.V_Status != 'Cancel' and A.V_Status != 'Reject') and A.Valid_at like '%" + location + "%' and A.RegionId=" + RegionId + " order by A.A_ID desc", con);

                   

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            DataField lst = new DataField();
                            lst.VisDate = Convert.ToDateTime(rdr["V_DateFrom"]);
                            lst.Phone = rdr["V_Phone"].ToString();
                            lst.comName = rdr["CName"].ToString();
                            lst.visName = rdr["V_Name"].ToString();
                            lst.Status = rdr["V_Status"].ToString();
                            lst.Type = rdr["Visitor_Type"].ToString();
                            lst.Req = rdr["Requestor"].ToString();
                       
                            lst.ReqName = rdr["ReqName"].ToString();
                            Visitor.Add(lst);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendExcepToDB(ex);
                }
                finally
                {
                    if (con.State == ConnectionState.Open) { con.Close(); }
                }
                return Visitor;
            }
        }

        public List<DataField> RecentVisitorAdmin
        {
            get
            {
                List<DataField> Visitor = new List<DataField>();
                try
                {
                    int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    //comment 9 oct 2023
                    // regionwise  SqlCommand cmd = new SqlCommand("select top(5)A.A_ID, A.V_DateFrom,A.Requestor, V.V_Phone,C.CName,V.V_Name,A.V_Status,A.Visitor_Type from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.V_Status != 'Cancel' and A.RegionId=" + RegionId + " order by A.A_ID desc", con);

                    SqlCommand cmd = new SqlCommand("select top(5)A.A_ID, A.V_DateFrom,A.Requestor, V.V_Phone,C.CName,V.V_Name,A.V_Status,A.Visitor_Type,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.V_Status != 'Cancel'  order by A.A_ID desc", con);



                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            DataField ecn = new DataField();
                            ecn.VisDate = Convert.ToDateTime(rdr["V_DateFrom"]);
                            ecn.Phone = rdr["V_Phone"].ToString();
                            ecn.comName = rdr["CName"].ToString();
                            ecn.visName = rdr["V_Name"].ToString();
                            ecn.Status = rdr["V_Status"].ToString();
                            ecn.Type = rdr["Visitor_Type"].ToString();
                            ecn.Req = rdr["Requestor"].ToString();
                            ecn.Region = rdr["Region"].ToString();
                            Visitor.Add(ecn);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendExcepToDB(ex);
                }
                finally
                {
                    if (con.State == ConnectionState.Open) { con.Close(); }
                }
                return Visitor;
            }
        }


        public IEnumerable<DataField> AppoinmentList
        {
            get
            {
                List<DataField> Visitor = new List<DataField>();
                try
                {
                    int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    string Req = HttpContext.Current.Session["PNICODE"].ToString();
                    SqlCommand cmd = new SqlCommand("select A.A_ID,A.GPID, A.V_DateFrom,A.A_Time,A.Duration,C.CName,V.V_Name,A.Visitor_Type, A.V_Status,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,Isnull(A.RejectRemark,'') as RejectRemark  from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.Requestor = '" + Req + "' and A.V_Status not in('Out','Cancel') and A.RegionId=" + RegionId + " ORDER BY  A.A_ID DESC", con);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            DataField dataField = new DataField();
                            dataField.GP_ID = Convert.ToInt32(rdr["A_ID"]);
                            dataField.GP = rdr["GPID"].ToString();
                            dataField.VisDate = Convert.ToDateTime(rdr["V_DateFrom"]);
                            dataField.VisTime = rdr["A_Time"].ToString();
                            dataField.VisDuration = rdr["Duration"].ToString();
                            dataField.comName = rdr["CName"].ToString();
                            dataField.visName = rdr["V_Name"].ToString();
                            dataField.Type = rdr["Visitor_Type"].ToString();
                            dataField.Status = rdr["V_Status"].ToString();
                            dataField.V_Type = rdr["V_Type"].ToString();
                            dataField.RejectRemark = rdr["RejectRemark"].ToString();
                            Visitor.Add(dataField);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendExcepToDB(ex);
                }
                finally
                {
                    if (con.State == ConnectionState.Open) { con.Close(); }
                }
                return Visitor;
            }
        }


        //-----------All AppointMent-----

        public IEnumerable<DataField> AllAppoinmentList( string userId)
        {
            //get
            //{
                List<DataField> Visitor = new List<DataField>();
                try
                {
                int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                if (con.State == ConnectionState.Closed) { con.Open(); }
                    //string Req = HttpContext.Current.Session["PNICODE"].ToString();
                SqlCommand cmd;
                if (userId == "Admin")//Added 30 june
                {
                    //,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type
                    //Isnull(V.V_Type,'') as V_Type

                    cmd = new SqlCommand("select A.A_ID,A.GPID, A.V_DateFrom,A.A_Time,A.Duration,C.CName,Trim(V.V_Name) as V_Name,A.Visitor_Type, CASE WHEN A.V_Status='Arrived' THEN 'Check-in' WHEN A.V_Status='OUT' THEN 'Check-out' WHEN A.V_Status='Reject' THEN 'Reject' WHEN A.V_Status='Cancel' THEN 'Cancel'  WHEN A.V_Status='Pre' THEN 'Appointment' end as V_Status ,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type ,Isnull(A.RejectRemark,'') as RejectRemark,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID     ORDER BY  A.A_ID DESC", con);

                }

               else if (userId == "")
                {
                    //cmd = new SqlCommand("select A.A_ID,A.GPID, A.V_DateFrom,A.A_Time,A.Duration,C.CName,V.V_Name,A.Visitor_Type, A.V_Status,Isnull(V.V_Type,'') as V_Type ,Isnull(A.RejectRemark,'') as RejectRemark from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID   ORDER BY  A.A_ID DESC", con);//and A.V_Status not in('Out','Cancel')
                    //feb28
                    cmd = new SqlCommand("select A.A_ID,A.GPID, A.V_DateFrom,A.A_Time,A.Duration,C.CName,Trim(V.V_Name) as V_Name,A.Visitor_Type, CASE WHEN A.V_Status='Arrived' THEN 'Check-in' WHEN A.V_Status='OUT' THEN 'Check-out' WHEN A.V_Status='Reject' THEN 'Reject' WHEN A.V_Status='Cancel' THEN 'Cancel'  WHEN A.V_Status='Pre' THEN 'Appointment' end as V_Status ,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type ,Isnull(A.RejectRemark,'') as RejectRemark,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.RegionId=" + RegionId + "   ORDER BY  A.A_ID DESC", con);//and A.V_Status not in('Out','Cancel')

                }
                else {
                    //cmd = new SqlCommand("select A.A_ID,A.GPID, A.V_DateFrom,A.A_Time,A.Duration,C.CName,V.V_Name,A.Visitor_Type, A.V_Status,Isnull(V.V_Type,'') as V_Type ,Isnull(A.RejectRemark,'') as RejectRemark from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.Requestor = '" + userId + "'  ORDER BY  A.A_ID DESC", con);//and A.V_Status not in('Out','Cancel')
                    //feb28
                    cmd = new SqlCommand("select A.A_ID,A.GPID, A.V_DateFrom,A.A_Time,A.Duration,C.CName,Trim(V.V_Name) as V_Name,A.Visitor_Type,  CASE WHEN A.V_Status='Arrived' THEN 'Check-in' WHEN A.V_Status='OUT' THEN 'Check-out' WHEN A.V_Status='Reject' THEN 'Reject' WHEN A.V_Status='Cancel' THEN 'Cancel'  WHEN A.V_Status='Pre' THEN 'Appointment' end as V_Status,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type ,Isnull(A.RejectRemark,'') as RejectRemark,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.Requestor = '" + userId + "' and A.RegionId=" + RegionId + "  ORDER BY  A.A_ID DESC", con);//and A.V_Status not in('Out','Cancel')

                }

                using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            DataField dataField = new DataField();
                            dataField.GP_ID = Convert.ToInt32(rdr["A_ID"]);
                            dataField.GP = rdr["GPID"].ToString();
                            dataField.VisDate = Convert.ToDateTime(rdr["V_DateFrom"]);
                            dataField.VisTime = rdr["A_Time"].ToString();
                            dataField.VisDuration = rdr["Duration"].ToString();
                            dataField.comName = rdr["CName"].ToString();
                            dataField.visName = rdr["V_Name"].ToString();
                            dataField.Type = rdr["Visitor_Type"].ToString();
                            dataField.Status = rdr["V_Status"].ToString();
                            dataField.V_Type = rdr["V_Type"].ToString();
                        dataField.RejectRemark = rdr["RejectRemark"].ToString();
                        dataField.Region = rdr["Region"].ToString(); 

                            Visitor.Add(dataField);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendExcepToDB(ex);
                }
                finally
                {
                    if (con.State == ConnectionState.Open) { con.Close(); }
                }
                return Visitor;
            //}
        }

        public IEnumerable<DataFieldAdmin> AppTodayForAdmin(string userId)
        {
            //get
            //{
            List<DataFieldAdmin> Visitor = new List<DataFieldAdmin>();
            try
            {
                int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                if (con.State == ConnectionState.Closed) { con.Open(); }
                //string Req = HttpContext.Current.Session["PNICODE"].ToString();
                SqlCommand cmd;
                if (userId == "Admin")//Added 30 june
                {
                    cmd = new SqlCommand("select distinct t.GPID, v.V_Name, c.CName, v.V_Phone, a.NAME, a.EMPLOYEE_ID, t.V_DateFrom,t.A_Time,t.Duration,t.V_Status,t.Visitor_Type,t.Location,t.Valid_at,t.Regionid ,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region from tblAppoinment t join tblVisitor v on t.VisitorID=v.V_ID join tblCompany c on t.ComanyID=c.C_ID join " + KeyMasterDB + ".[dbo].[Master_Requestor] a on t.Requestor=a.EMPLOYEE_ID where (t.V_DateFrom) = FORMAT (getdate(),'yyyy-MM-dd') or ((t.V_DateFrom) <= FORMAT (getdate(),'yyyy-MM-dd') and t.V_DateTo >= FORMAT(getdate(),'yyyy-MM-dd'))", con);
                    //cmd = new SqlCommand("select distinct t.GPID, v.V_Name, c.CName, v.V_Phone, a.NAME, a.EMPLOYEE_ID, t.V_DateFrom,t.A_Time,t.Duration,t.V_Status,t.Visitor_Type,t.Location,t.Valid_at,t.Regionid ,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region from tblAppoinment t join tblVisitor v on t.VisitorID=v.V_ID join tblCompany c on t.ComanyID=c.C_ID join " + KeyMasterDB + ".[dbo].[Master_Requestor] a on t.Requestor=a.EMPLOYEE_ID where (t.V_DateFrom) = FORMAT (getdate(),'yyyy-MM-dd') or ((t.V_DateFrom) <= FORMAT (getdate(),'yyyy-MM-dd') and t.V_DateTo >= FORMAT(getdate(),'yyyy-MM-dd'))) aa", con);



                }
                else
                {
                    cmd = new SqlCommand("", con);
                }

               
              

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataFieldAdmin dataField = new DataFieldAdmin();
                        //dataField.A_ID = Convert.ToInt32(rdr["A_ID"]);
                        dataField.GPID = rdr["GPID"].ToString();
                        dataField.V_Name = rdr["V_Name"].ToString();// Convert.ToDateTime(rdr["V_DateFrom"]);
                        dataField.CName = rdr["CName"].ToString();
                        dataField.V_Phone = rdr["V_Phone"].ToString();
                        dataField.NAME = rdr["NAME"].ToString();
                        dataField.EMPLOYEE_ID = rdr["EMPLOYEE_ID"].ToString();
                        dataField.V_DateFrom = Convert.ToDateTime(rdr["V_DateFrom"].ToString());
                        dataField.A_Time = rdr["A_Time"].ToString();
                        dataField.Location = rdr["Location"].ToString();
                        dataField.Valid_at = rdr["Valid_at"].ToString();
                        dataField.regionid = Convert.ToInt32(rdr["Regionid"]);
                        dataField.Region = rdr["Region"].ToString();
                       dataField.Duration = rdr["Duration"].ToString();
                        dataField.Visitor_Type = rdr["Visitor_Type"].ToString();
                        dataField.V_Status = rdr["V_Status"].ToString();
                        dataField.Region = rdr["Region"].ToString();

                        Visitor.Add(dataField);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Visitor;
            //}
        }

        public IEnumerable<DataFieldAdmin> AppUpcomingForAdmin(string userId)
        {
            //get
            //{
            List<DataFieldAdmin> Visitor = new List<DataFieldAdmin>();
            try
            {
                //int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                if (con.State == ConnectionState.Closed) { con.Open(); }
                
                SqlCommand cmd;
                if (userId == "Admin")
                {
                    cmd = new SqlCommand("select distinct t.GPID, v.V_Name, c.CName, v.V_Phone, a.NAME, a.EMPLOYEE_ID, t.V_DateFrom,t.A_Time,t.Duration,t.V_Status,t.Visitor_Type,t.Location,t.Valid_at,t.Regionid, isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region from tblAppoinment t join tblVisitor v on t.VisitorID=v.V_ID join tblCompany c on t.ComanyID=c.C_ID join "+ KeyMasterDB + ".[dbo].[Master_Requestor] a on t.Requestor=a.EMPLOYEE_ID where  (t.V_DateFrom) > FORMAT (getdate(),'yyyy-MM-dd') and t.V_Status!='Close'", con);

                }
                else
                {
                    cmd = new SqlCommand("", con);
                }




                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataFieldAdmin dataField = new DataFieldAdmin();
                        //dataField.A_ID = Convert.ToInt32(rdr["A_ID"]);
                        dataField.GPID = rdr["GPID"].ToString();
                        dataField.V_Name = rdr["V_Name"].ToString();// Convert.ToDateTime(rdr["V_DateFrom"]);
                        dataField.CName = rdr["CName"].ToString();
                        dataField.V_Phone = rdr["V_Phone"].ToString();
                        dataField.NAME = rdr["NAME"].ToString();
                        dataField.EMPLOYEE_ID = rdr["EMPLOYEE_ID"].ToString();
                        dataField.V_DateFrom = Convert.ToDateTime(rdr["V_DateFrom"].ToString());
                        dataField.A_Time = rdr["A_Time"].ToString();
                        dataField.Location = rdr["Location"].ToString();
                        dataField.Valid_at = rdr["Valid_at"].ToString();
                        dataField.regionid = Convert.ToInt32(rdr["Regionid"]);
                        dataField.Region = rdr["Region"].ToString();
                        dataField.Duration = rdr["Duration"].ToString();
                        dataField.Visitor_Type = rdr["Visitor_Type"].ToString();
                        dataField.V_Status = rdr["V_Status"].ToString();
                        dataField.Region = rdr["Region"].ToString();

                        Visitor.Add(dataField);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Visitor;
            //}
        }
        //---------------


        //-------------
        //public IEnumerable<Couriar> CouriarList
        //{
        //    get
        //    {
        //        List<Couriar> Visitor = new List<Couriar>();
        //        try
        //        {
        //            if (con.State == ConnectionState.Closed) { con.Open(); }
        //            //string Req = HttpContext.Current.Session["PNICODE"].ToString();
        //            SqlCommand cmd = new SqlCommand("select CouriarId, Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, Remark, CurrentStatus from tblCouriar", con);
        //            using (SqlDataReader rdr = cmd.ExecuteReader())
        //            {
        //                while (rdr.Read())
        //                {
        //                    Couriar dataField = new Couriar();
        //                    dataField.CouriarId = Convert.ToInt32(rdr["CouriarId"]);
        //                    dataField.Date = rdr["Date"].ToString();
        //                    dataField.NameOfCompany = rdr["NameOfCompany"].ToString();
        //                    dataField.Region = rdr["Region"].ToString();
        //                    dataField.City = rdr["City"].ToString();
        //                    dataField.Documents = rdr["Documents"].ToString();
        //                    dataField.ReceiveTime = rdr["ReceiveTime"].ToString();
        //                    dataField.DocketNo = rdr["DocketNo"].ToString();
        //                    dataField.CouriarVendor = rdr["CouriarVendor"].ToString();
        //                    dataField.CurrentStatus = rdr["CurrentStatus"].ToString();

        //                    Visitor.Add(dataField);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            SendExcepToDB(ex);
        //        }
        //        finally
        //        {
        //            if (con.State == ConnectionState.Open) { con.Close(); }
        //        }
        //        return Visitor;
        //    }
        //}

        public List<Couriar> CouriarList(string status1, string strUserId)
        {
           

            List<Couriar> Visitor = new List<Couriar>();
            try
            {
                int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                if (con.State == ConnectionState.Closed) { con.Open(); }
                //string Req = HttpContext.Current.Session["PNICODE"].ToString();
                //FORMAT (Date, 'dd/MM/yyyy') as
                SqlCommand cmd;
                if (strUserId != "" && status1 != "All")// Foremployee MANAGE
                {

                        
                        //isnull(EmployeeName, '') as EmployeeName
                    cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, CurrentStatus,[dbo].[udf_GetEmployeeName](ISNULL(TRY_CAST(EmployeeId AS INT), 0)) as EmployeeName,isnull(Department,'') as Department,isnull(EmployeeId,0) as EmployeeId,isnull(SenderName,'') as SenderName,isnull(Remark,'') as Remark, isnull(DeskRemarks,'') as DeskRemarks, isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId ),'') as AdminRegion    from tblCouriar where CurrentStatus='Intrasit' and CouriarType='Inward' and EmployeeId='" + strUserId + "' and RegionId=" + RegionId + " order by CouriarId desc", con);
                }
                else if (status1 == "All")//TOTAL
                {

                    if (strUserId != "")// Foremployee
                    {
                        cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, CurrentStatus,[dbo].[udf_GetEmployeeName](ISNULL(TRY_CAST(EmployeeId AS INT), 0)) as EmployeeName,isnull(Department,'') as Department,isnull(EmployeeId,0) as EmployeeId,isnull(SenderName,'') as SenderName,isnull(Remark,'') as Remark, isnull(DeskRemarks,'') as DeskRemarks, isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId ),'') as AdminRegion    from tblCouriar where CouriarType='Inward' and EmployeeId='" + strUserId + "' and RegionId=" + RegionId + " order by CouriarId desc", con);
                    }
                    else
                    {//deSKKLL
                        cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, CurrentStatus,[dbo].[udf_GetEmployeeName](ISNULL(TRY_CAST(EmployeeId AS INT), 0)) as EmployeeName,isnull(Department,'') as Department,isnull(EmployeeId,0) as EmployeeId,isnull(SenderName,'') as SenderName,isnull(Remark,'') as Remark, isnull(DeskRemarks,'') as DeskRemarks, isnull(EmployeeRemark,'') as EmployeeRemark ,isnull([dbo].[udf_GetRegion](RegionId ),'') as AdminRegion   from tblCouriar where CouriarType='Inward' and RegionId=" + RegionId + " order by CouriarId desc", con);

                    }



                }
                else if (status1 == "Admin")
                {
                    cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, CurrentStatus,[dbo].[udf_GetEmployeeName](ISNULL(TRY_CAST(EmployeeId AS INT), 0)) as EmployeeName,isnull(Department,'') as Department,isnull(EmployeeId,0) as EmployeeId,isnull(SenderName,'') as SenderName,isnull(Remark,'') as Remark, isnull(DeskRemarks,'') as DeskRemarks, isnull(EmployeeRemark,'') as EmployeeRemark ,isnull([dbo].[udf_GetRegion](RegionId ),'') as AdminRegion   from tblCouriar where CouriarType='Inward'  order by CouriarId desc", con);

                }

                else//MANAGE DESK
                {
                    cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, CurrentStatus,[dbo].[udf_GetEmployeeName](ISNULL(TRY_CAST(EmployeeId AS INT), 0)) as EmployeeName,isnull(Department,'') as Department,isnull(EmployeeId,0) as EmployeeId,isnull(SenderName,'') as SenderName,isnull(Remark,'') as Remark, isnull(DeskRemarks,'') as DeskRemarks, isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId ),'') as AdminRegion    from tblCouriar where CurrentStatus='" + status1 + "' and CouriarType='Inward' and RegionId=" + RegionId + " order by CouriarId desc", con);
                }
               
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Couriar dataField = new Couriar();
                        dataField.CouriarId = Convert.ToInt32(rdr["CouriarId"]);
                        dataField.C_Date = Convert.ToDateTime(rdr["C_Date"].ToString());//rdr["C_Date"].ToString(); //
                        dataField.NameOfCompany = rdr["NameOfCompany"].ToString();
                        dataField.Region = rdr["Region"].ToString();
                        dataField.City = rdr["City"].ToString();
                        dataField.Documents = rdr["Documents"].ToString();
                        dataField.ReceiveTime = rdr["ReceiveTime"].ToString();
                        dataField.DocketNo = rdr["DocketNo"].ToString();
                        dataField.CouriarVendor = rdr["CouriarVendor"].ToString();
                        dataField.CurrentStatus = rdr["CurrentStatus"].ToString();
                        
                        dataField.Department = rdr["Department"].ToString();
                        //dataField.EmployeeId = Convert.ToInt32(rdr["EmployeeId"].ToString());
                        dataField.EmployeeId = Convert.ToInt32(string.IsNullOrEmpty(rdr["EmployeeId"]?.ToString()) ? "0" : rdr["EmployeeId"].ToString());
                        dataField.SenderName = rdr["SenderName"].ToString();

                        dataField.DeskRemarks = rdr["DeskRemarks"].ToString();
                        dataField.EmployeeRemark = rdr["EmployeeRemark"].ToString();

                        dataField.Remark = rdr["Remark"].ToString();
                        dataField.AdminRegion = rdr["AdminRegion"].ToString();
                        dataField.EmployeeName = rdr["EmployeeName"].ToString();
                        //
                        Visitor.Add(dataField);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Visitor;
            //return Itemlist;
        }
        public List<Couriar> DispatchList(string status1, string strUserId)
        {


            List<Couriar> Visitor = new List<Couriar>();
            try
            {

                int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                if (con.State == ConnectionState.Closed) { con.Open(); }
                //string Req = HttpContext.Current.Session["PNICODE"].ToString();
                //FORMAT (Date, 'dd/MM/yyyy') as
                SqlCommand cmd;
                //if (strUserId != "")// Foremployee
                //{
                //    cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, Remark, CurrentStatus,isnull(EmployeeName,'') as EmployeeName from tblCouriar where CurrentStatus='Intrasit' and CouriarType='Outward' and EmployeeId='" + strUserId + "'", con);
                //}
                //else
                //{
                if (status1 == "All")
                {
                    //totaloutward Admin/employee
                    if (strUserId != "")//employee
                    {
                        cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,  CurrentStatus,isnull(EmployeeName,'') as EmployeeName,isnull(Person,'') as Person,isnull(ContactNo,'') as ContactNo ,isnull(Address,'') as Address, isnull(Remark,'') as  Remark,isnull(DeskRemarks,'') as DeskRemarks,isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile  from tblCouriar where  EmployeeId='" + strUserId + "' and CouriarType='Outward' and RegionId=" + RegionId + "  order by CouriarId desc", con);
                    }
                    else {//sec/desk total
                        cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,  CurrentStatus,isnull(EmployeeName,'') as EmployeeName,isnull(Person,'') as Person,isnull(ContactNo,'') as ContactNo ,isnull(Address,'') as Address, isnull(Remark,'') as  Remark,isnull(DeskRemarks,'') as DeskRemarks,isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile  from tblCouriar where  CouriarType='Outward' and RegionId=" + RegionId + "  order by CouriarId desc", con);
                    }
                   

                }

               else if (status1 == "Admin")//Admin
                {
                    cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,  CurrentStatus,isnull(EmployeeName,'') as EmployeeName,isnull(Person,'') as Person,isnull(ContactNo,'') as ContactNo ,isnull(Address,'') as Address, isnull(Remark,'') as  Remark,isnull(DeskRemarks,'') as DeskRemarks,isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile  from tblCouriar where  CouriarType='Outward'   order by CouriarId desc", con);

                }

                //else if (status1 == "Open"  && strUserId != "")//26-09-24
                    
                    else if ((status1 == "Open" || status1 == "Close") && strUserId != "")
                {
                    if (strUserId == "Desk")//deskmanage
                    {
                        cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,  CurrentStatus,isnull(EmployeeName,'') as EmployeeName,isnull(Person,'') as Person,isnull(ContactNo,'') as ContactNo ,isnull(Address,'') as Address, isnull(Remark,'') as  Remark,isnull(DeskRemarks,'') as DeskRemarks,isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile from tblCouriar where  CouriarType='Outward'  and RegionId=" + RegionId + " and CurrentStatus='" + status1 + "' order by CouriarId desc", con);

                    }
                    else 
                    {///employeemanage cancel
                        cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,  CurrentStatus,isnull(EmployeeName,'') as EmployeeName,isnull(Person,'') as Person,isnull(ContactNo,'') as ContactNo ,isnull(Address,'') as Address, isnull(Remark,'') as  Remark,isnull(DeskRemarks,'') as DeskRemarks,isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile from tblCouriar where  CouriarType='Outward' and CurrentStatus='" + status1 + "'  and RegionId=" + RegionId + " and EmployeeId='" + strUserId + "'  order by CouriarId desc", con);
                    }


                }


                //--26sep2023------------

                

                else if (status1 == "DispatchListForSecurityCancel")//desk
                {
                    cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,  CurrentStatus,isnull(EmployeeName,'') as EmployeeName,isnull(Person,'') as Person,isnull(ContactNo,'') as ContactNo ,isnull(Address,'') as Address, isnull(Remark,'') as  Remark,isnull(DeskRemarks,'') as DeskRemarks,isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile from tblCouriar where  CouriarType='Outward'  and RegionId=" + RegionId + " and CurrentStatus in('Open','Intransit') order by CouriarId desc", con);

                }

                //--------------
                //=26 sep




                else
                {

                    cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,  CurrentStatus,isnull(EmployeeName,'') as EmployeeName,isnull(Person,'') as Person,isnull(ContactNo,'') as ContactNo ,isnull(Address,'') as Address, isnull(Remark,'') as  Remark,isnull(DeskRemarks,'') as DeskRemarks,isnull(EmployeeRemark,'') as EmployeeRemark,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile from tblCouriar where CurrentStatus='" + status1 + "' and CouriarType='Outward' and RegionId=" + RegionId + "  order by CouriarId desc", con);

                }


                //}

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Couriar dataField = new Couriar();
                        dataField.CouriarId = Convert.ToInt32(rdr["CouriarId"]);
                        dataField.C_Date = Convert.ToDateTime(rdr["C_Date"].ToString());//rdr["C_Date"].ToString(); //
                        dataField.NameOfCompany = rdr["NameOfCompany"].ToString();
                        dataField.Region = rdr["Region"].ToString();
                        dataField.City = rdr["City"].ToString();
                        dataField.Documents = rdr["Documents"].ToString();
                        dataField.ReceiveTime = rdr["ReceiveTime"].ToString();
                        dataField.DocketNo = rdr["DocketNo"].ToString();
                        dataField.CouriarVendor = rdr["CouriarVendor"].ToString();
                        dataField.CurrentStatus = rdr["CurrentStatus"].ToString();
                        //dataField.EmployeeName= rdr["EmployeeName"].ToString(); 
                        dataField.Person= rdr["Person"].ToString();
                        dataField.ContactNo= rdr["ContactNo"].ToString();
                        dataField.Address = rdr["Address"].ToString();

                        dataField.Remark = rdr["Remark"].ToString();
                        dataField.DeskRemarks = rdr["DeskRemarks"].ToString();
                        dataField.EmployeeRemark = rdr["EmployeeRemark"].ToString();
                        //, isnull(Remark,'') as  Remark,isnull(DeskRemarks,'') as DeskRemarks,isnull(EmployeeRemark,'') as EmployeeRemark
                        //,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion
                        dataField.AdminRegion = rdr["AdminRegion"].ToString();
                        dataField.PodFile= rdr["PodFile"].ToString(); 
                        Visitor.Add(dataField);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Visitor;
            //return Itemlist;
        }


        public List<Couriar> CourierReport(string status1)
        {


            List<Couriar> Visitor = new List<Couriar>();
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                //string Req = HttpContext.Current.Session["PNICODE"].ToString();
                //FORMAT (Date, 'dd/MM/yyyy') as
                int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);

                SqlCommand cmd;

                // cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, Remark, CurrentStatus,isnull(EmployeeName,'') as EmployeeName,isnull(Department,'') as Department from tblCouriar where  CouriarType='" + status1 + "'", con);
                cmd = new SqlCommand("select CouriarId, C_Date, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, Remark, CurrentStatus,isnull(EmployeeName,'') as EmployeeName,isnull(Department,'') as Department,isnull(Person,'') as Person,isnull(ContactNo,'') as ContactNo ,isnull(Address,'') as Address from tblCouriar where  CouriarType='" + status1 + "' and RegionId="+ RegionId + " order by CouriarId desc", con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Couriar dataField = new Couriar();
                        dataField.CouriarId = Convert.ToInt32(rdr["CouriarId"]);
                        dataField.C_Date = Convert.ToDateTime(rdr["C_Date"].ToString());//rdr["C_Date"].ToString(); //
                        dataField.NameOfCompany = rdr["NameOfCompany"].ToString();
                        dataField.Region = rdr["Region"].ToString();
                        dataField.City = rdr["City"].ToString();
                        dataField.Documents = rdr["Documents"].ToString();
                        dataField.ReceiveTime = rdr["ReceiveTime"].ToString();
                        dataField.DocketNo = rdr["DocketNo"].ToString();
                        dataField.CouriarVendor = rdr["CouriarVendor"].ToString();
                        dataField.CurrentStatus = rdr["CurrentStatus"].ToString();
                        //dataField.EmployeeName= rdr["EmployeeName"].ToString(); 
                        //---30oct------
                        dataField.Department = rdr["Department"].ToString();
                        dataField.Person = rdr["Person"].ToString();
                        dataField.ContactNo = rdr["ContactNo"].ToString();
                      

                        Visitor.Add(dataField);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Visitor;
            //return Itemlist;
        }

        //-------------
        public string SaveVisitor(Visitor V)
        {
            string VID = "";
            try
            {
               // int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int company = V.C_ID == 0 ? 4963 : V.C_ID;
                string email = V.V_Email == null ? "NA" : V.V_Email;
                string visitor_ID = V.Visitor_ID == "? object:null ?" ? null : V.Visitor_ID;

                SqlCommand cmd = new SqlCommand("sp_visitor", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@V_Name", V.V_Name.ToUpper());
                cmd.Parameters.AddWithValue("@V_Phone", V.V_Phone);
                cmd.Parameters.AddWithValue("@V_Email", email);
                cmd.Parameters.AddWithValue("@Visitor_ID", visitor_ID);
                cmd.Parameters.AddWithValue("@V_IDNumber", V.V_IDNumber);
                cmd.Parameters.AddWithValue("@V_Pic", V.V_Pic);
                cmd.Parameters.AddWithValue("@C_ID", company);
                cmd.Parameters.AddWithValue("@V_Type",V.V_Type );// 0ct 14

                //cmd.Parameters.AddWithValue("@RegionId", RegionId);

                SqlParameter sqlparam = new SqlParameter();
                sqlparam.ParameterName = "@ID";
                sqlparam.SqlDbType = SqlDbType.Int;
                sqlparam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlparam);
                int i = cmd.ExecuteNonQuery();
                VID = sqlparam.Value.ToString();
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return VID;
        }

        public void SaveVendor(Company C)
        {
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand("sp_PurchaseVendor", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PartyName", C.CName);
                cmd.Parameters.AddWithValue("@PartyAddress", C.Addres.ToUpper());
                cmd.Parameters.AddWithValue("@GST", C.GST.ToUpper());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
        }

        public static void SendMail(string textbox, string toEmail, string sub)
        {
            MailMessage mMailMessage = new MailMessage();
            mMailMessage.From = new MailAddress("karamsupport@karam.in");
            if (toEmail.Contains(";"))
            {
                string[] toMult = toEmail.Split(';');

                foreach (string Tomail1 in toMult)
                {
                    mMailMessage.To.Add(new MailAddress(Tomail1));
                }
            }
            else
            {
                mMailMessage.To.Add(new MailAddress(toEmail));
            }

            mMailMessage.Body = textbox;
            mMailMessage.IsBodyHtml = true;
            mMailMessage.Subject = sub;
            mMailMessage.Priority = MailPriority.Normal;

            //create the SmtpClient instance
            ////SmtpClient smtp = new SmtpClient();
            ////smtp.Host = "smtp.office365.com";
            ////smtp.Port = 587;
            ////smtp.UseDefaultCredentials = false;
            //////smtp.Credentials = new NetworkCredential("karamsupport@karam.in", "Passw0rd");
            ////smtp.Credentials = new System.Net.NetworkCredential("karamsupport@karam.in", "Angel@123", "Karam.in");
            ////smtp.EnableSsl = true;
            ///
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
            //smtp.Send(mail);


            if (IsValidEmailAddress(toEmail))
            {
                try {

                    smtp.Send(mMailMessage);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                
                }
             
            }
        }

        public static bool IsValidEmailAddress(string emailAddress)
        {
            // An empty or null string is not valid
            if (String.IsNullOrEmpty(emailAddress))
            {
                return (false);
            }

            // Regular expression to match valid email address
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            // Match the email address using a regular expression
            Regex re = new Regex(emailRegex);
            if (re.IsMatch(emailAddress))
                return (true);
            else
                return (false);
        }
        //CodeComment 28-06-2024 
        //public string SendSMS(string tosms, string body,string tempID,string senderID)
        //{
        //    try
        //    {
        //        string isparsh, iPassword, KARAM, acuse, soap1, entityID;
        //        string[] res;
        //        //isparsh = "SFTPRO";
        //        //iPassword = "dd09cd21eeXX";
        //        KARAM = Convert.ToString("KRMITI");
        //        //acuse = Convert.ToString('1');
        //        //entityID = "1701159170548618728";
        //        //-------------

        //        //isparsh = "KARAMSMS";
        //        //iPassword = "dd09cd21eeXX";
        //        ////KARAM = Convert.ToString("KRMITI");
        //        //acuse = Convert.ToString('1');
        //        //entityID = "1701159170548618728"; //1701159170548618728
        //        //----------------------------------------
        //        isparsh = "KARAMSMS";
        //        iPassword = "8806ced559XX";
        //        //KARAM = Convert.ToString("KRMITI");
        //        acuse = Convert.ToString('1');
        //        entityID = "1701159170548618728"; //1701159170548618728


        //        string contact = "http://sms.bulkssms.com/submitsms.jsp?user=" + isparsh + "&key=" + iPassword + "&mobile=" + tosms.ToString() + "&message=" + body + "&senderid=" + senderID + "&accusage=" + acuse + "&entityid=" + entityID + "&tempid=" + tempID;

        //        HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(contact);
        //        using (WebResponse response = webRequest1.GetResponse())
        //        using (StreamReader rd = new StreamReader(response.GetResponseStream()))
        //        {
        //            soap1 = rd.ReadToEnd();
        //            res = soap1.ToString().Split(',');
        //        }
        //        return soap1;
        //    }
        //    catch (Exception ex)
        //    { ex.Message.ToString(); return "3"; }

        //}
      


        public string SendSMS(string tosms, string body, string tempID, string senderID)
        {
            try
            {
                string isparsh, iPassword, KARAM, acuse, soap1, entityID;
                string[] res;

                //KARAM = Convert.ToString("KRMITI");


                //isparsh = "KARAMSMS";
                //iPassword = "8806ced559XX";

                //acuse = Convert.ToString('1');
                //entityID = "1701159170548618728";


                string contact = "https://trans.smsfresh.co/api/sendmsg.php?user=Jyotikaramsms&pass=123456&sender=" + senderID + "&phone=" + tosms.ToString() + "&text=" + body + " KARAM&priority=ndnd&stype=normal";

                HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(contact);
                using (WebResponse response = webRequest1.GetResponse())
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soap1 = rd.ReadToEnd();
                    res = soap1.ToString().Split(',');
                }
                return soap1;
            }
            catch (Exception ex)
            { ex.Message.ToString(); return "3"; }

        }
        //Added 27-02-25 whats msg
        public string SendWhatsMessage(string tosms, string str1, string str2, string str3, string str4, string str5, string str6)
        {
            try
            {
                string soap1;
                string[] res;
                string senderID = "BUZWAP";
                string pass = "7500092111";
                //string contact = "http://trans.smsfresh.co/api/sendmsg.php?user=karamsms&pass=" + pass + "&sender=" + senderID + "&phone=" + tosms.ToString() + "&text=vmsfinal&priority=wa&stype=normal&Params= " + str1 + ", " + str2 + ", " + str3 + ", " + str4 + ", " + str5 + ", " + str6 + "";

                string contact = "http://trans.smsfresh.co/api/sendmsg.php?user=karamsms&pass=" + pass + "&sender=" + senderID + "&phone=" + tosms.ToString() + "&text=vmsfinal&priority=wa&stype=normal&Params=" +
     str1 + "," + str2 + "," + str3 + "," + str4 + "," + str5 + "," + str6;


                HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(contact);
                using (WebResponse response = webRequest1.GetResponse())
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soap1 = rd.ReadToEnd();
                    res = soap1.ToString().Split(',');
                }
                return soap1;
            }
            catch (Exception ex)
            { ex.Message.ToString(); return "3"; }

        }

        //Added 08-07-2025 employee whats
        //--------------------------
        //Added 04-07-25
        public string SendWhatsMessageEmployee(string tosms, string str1, string str2, string str3)
        {
            try
            {
                string soap1;
                string[] res;
                string senderID = "BUZWAP";
                string pass = "7500092111";
                str1 = "Dear " + str1;
                string contact = "http://trans.smsfresh.co/api/sendmsg.php?user=karamsms&pass=" + pass + "&sender=" + senderID + "&phone=" + tosms.ToString() + "&text=visitor_arrived&priority=wa&stype=normal&Params=" +
                     str1 + "," + str2 + "," + str3;
                HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(contact);
                using (WebResponse response = webRequest1.GetResponse())
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soap1 = rd.ReadToEnd();
                    res = soap1.ToString().Split(',');
                }
                return soap1;
            }
            catch (Exception ex)
            { ex.Message.ToString(); return "3"; }

        }




        //---------------
        //-------------ArrivalStatus--test------

        public string ArrivalSendSMS(string tosms, string body, string tempID, string senderID)
        {
            try
            {
                string isparsh, iPassword, KARAM, acuse, soap1, entityID;
                string[] res;
                //isparsh = "SFTPRO";
                //iPassword = "dd09cd21eeXX";
                KARAM = Convert.ToString("KRMITI");
                //acuse = Convert.ToString('1');
                //entityID = "1701159170548618728";
                //-------------

                //isparsh = "KARAMSMS";
                //iPassword = "dd09cd21eeXX";
                ////KARAM = Convert.ToString("KRMITI");
                //acuse = Convert.ToString('1');
                //entityID = "1701159170548618728"; //1701159170548618728
                //----------------------------------------
                isparsh = "KARAMSMS";
                iPassword = "8806ced559XX";
                //KARAM = Convert.ToString("KRMITI");
                acuse = Convert.ToString('1');
                entityID = "1007161949720682356"; //1701159170548618728


                string contact = "http://sms.bulkssms.com/submitsms.jsp?user=" + isparsh + "&key=" + iPassword + "&mobile=" + tosms.ToString() + "&message=" + body + "&senderid=" + senderID + "&accusage=" + acuse + "&entityid=" + entityID + "&tempid=" + tempID;

                HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(contact);
                using (WebResponse response = webRequest1.GetResponse())
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soap1 = rd.ReadToEnd();
                    res = soap1.ToString().Split(',');
                }
                return soap1;
            }
            catch (Exception ex)
            { ex.Message.ToString(); return "3"; }

        }



        //-------------31oct------
        //----------

        public DataTable VendorData(string mbl)
        {
            DataTable dt = new DataTable();
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlDataAdapter da = new SqlDataAdapter("select V.V_Name,V.V_Phone,V.V_Email,C.CName from tblVisitor V, tblCompany C where V.C_ID = C.C_ID and V_Phone = '" + mbl + "'", con);
                da.Fill(dt);
            }
            catch
            {

            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return dt;
        }

        public List<DataField> ReportVisitor(params string[] paramtr)
        {
            List<DataField> Report = new List<DataField>();
            try
            {
                string Req = paramtr[5] == "Select" ? HttpContext.Current.Session["PNICODE"].ToString() : paramtr[5];

                string[] arrFrom = paramtr[3] != "" ? paramtr[3].ToString().Split('/') : null;
                string[] arrTo = paramtr[4] != "" ? paramtr[4].ToString().Split('/') : null;
                int fromM = arrFrom != null ? Convert.ToInt32(arrFrom[0]) : 0;
                int toM = arrTo != null ? Convert.ToInt32(arrTo[0]) : 0;
                string f = arrFrom != null ? arrFrom[2] + "-" + arrFrom[0] + "-" + arrFrom[1] : "";
                string t = arrTo != null ? arrTo[2] + "-" + arrTo[0] + "-" + arrTo[1] : "";

                if (con.State == ConnectionState.Closed) { con.Open(); }
                string query = "";

                //If From and To parameter is provided
                if (paramtr[0] == "All" && paramtr[1] == "All" && (paramtr[2] == "All" || paramtr[2] == "") && paramtr[3] != "" && paramtr[4] != "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "'";
                }
                // If All parameter is null or Only Host is provided
                else if (paramtr[0] == "All" && paramtr[1] == "All" && (paramtr[2] == "All" || paramtr[2] == "") && paramtr[3] == "" && paramtr[4] == "" && (paramtr[5] == "Select" || Req != "Select") && paramtr[6] == "Select")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and V_DateFrom >= '" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-1' AND V_DateFrom <= '" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "'";
                }
                // If only Company provided
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] == "" && paramtr[4] == "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If only Status provided
                else if (paramtr[0] == "All" && paramtr[1] == "All" && paramtr[2] == "All" && paramtr[3] == "" && paramtr[4] == "" && paramtr[6] != "Select")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.V_Status='" + paramtr[6] + "'";
                }
                //If only From date is provided
                else if (paramtr[0] == "All" && paramtr[1] == "All" && (paramtr[2] == "All" || paramtr[2] == "") && paramtr[3] != "" && paramtr[4] == "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and V_DateFrom >= '" + f + "'";
                }
                //If only To date is provided
                else if (paramtr[0] == "All" && paramtr[1] == "All" && (paramtr[2] == "All" || paramtr[2] == "") && paramtr[3] == "" && paramtr[4] != "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and  A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and A.V_DateFrom <= '" + t + "'";
                }
                // If only Visit type is provided  
                else if (paramtr[0] != "All" && paramtr[1] == "All" && (paramtr[2] == "All" || paramtr[2] == "") && paramtr[3] == "" && paramtr[4] == "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and A.Visitor_Type = '" + paramtr[0] + "'";
                }
                //If only company and visitor provided.
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] == "" && paramtr[4] == "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and A.ComanyID='" + paramtr[1] + "' and A.VisitorID='" + paramtr[2] + "'";
                }
                //If only Visit type and company provided.
                else if (paramtr[0] != "All" && paramtr[1] != "All" && (paramtr[2] == "All" || paramtr[2] == "") && paramtr[3] == "" && paramtr[4] == "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Visit Type,Company and Visitor provided
                else if (paramtr[0] != "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] == "" && paramtr[4] == "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.ComanyID='" + paramtr[1] + "' and A.VisitorID='" + paramtr[2] + "'";
                }
                //If Company and from date provided
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] == "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and A.ComanyID='" + paramtr[1] + "' and A.V_DateFrom >='" + f + "'";
                }
                //If Comapany and To date provided
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] == "" && paramtr[4] != "")
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and A.ComanyID='" + paramtr[1] + "' and A.V_DateFrom <='" + t + "'";
                }
                //If all parameter is provided
                else
                {
                    query = "select A.GPID,Req.NAME,A.Deptment,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and A.VisitorID = V.V_ID and A.Requestor='" + Req + "' and A.ComanyID='" + paramtr[1] + "' and A.VisitorID='" + paramtr[2] + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.V_DateFrom >= '" + f + "' AND A.V_DateFrom <= '" + t + "'";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataField dataField = new DataField();
                        dataField.GP = rdr["GPID"].ToString();
                        dataField.Req = rdr["NAME"].ToString();
                        dataField.Dept = rdr["Deptment"].ToString();
                        dataField.comName = rdr["CName"].ToString();
                        dataField.visName = rdr["V_Name"].ToString();
                        dataField.Type = rdr["Visitor_Type"].ToString();
                        dataField.VisDate = Convert.ToDateTime(rdr["V_DateFrom"]);
                        dataField.VisTime = rdr["A_Time"].ToString();
                        dataField.VisDuration = rdr["Duration"].ToString();
                        dataField.Status = rdr["V_Status"].ToString();
                        dataField.Location = rdr["Location"].ToString();
                        dataField.Remark = rdr["Remark"].ToString();
                        Report.Add(dataField);
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Report;
        }

        public enum Hours
        {
            A = 1, B = 2, C = 3, D = 4, E = 5, F = 6, G = 7, H = 8, Z = 9, J = 10, K = 11, L = 11, M = 12, N = 13, O = 14, P = 15, Q = 16, R = 17, S = 18, T = 19, U = 20, V = 21, W = 22, X = 23, Y = 24, I = 25
        }

        public List<MaterialReport> ReportMaterialPDF(params string[] paramtr)
        {
            List<MaterialReport> Report = new List<MaterialReport>();
            try
            {
                string stats = paramtr[6] == "OUT" ? "'OUT','OUT TW','Partialy Closed','IN'" : paramtr[6];
                string[] arrFrom = paramtr[4] != "" ? paramtr[4].ToString().Split('/') : null;
                string[] arrTo = paramtr[5] != "" ? paramtr[5].ToString().Split('/') : null;
                int fromM = arrFrom != null ? Convert.ToInt32(arrFrom[0]) : 0;
                int toM = arrTo != null ? Convert.ToInt32(arrTo[0]) : 0;
                string f = arrFrom != null ? arrFrom[2] + "-" + arrFrom[1] + "-" + arrFrom[0] : DateTime.Now.Year + "-" + DateTime.Now.AddMonths(-1).Month.ToString() + "-" + DateTime.Now.Day;

                string t = arrTo != null ? arrTo[2] + "-" + arrTo[1] + "-" + arrTo[0] : DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                string query = "";

                //If Requestor+ From + To only
                if (paramtr[0] != "Select One" && paramtr[1] == "Select One" && paramtr[2] == "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] == "All")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and Requestor = '" + paramtr[0] + "'";
                }
                //If Requestor+ From + To+ Status only
                else if (paramtr[0] != "Select One" && paramtr[1] == "Select One" && paramtr[2] == "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and Requestor = '" + paramtr[0] + "' and M_Status in ('" + stats + "')";
                }
                //If Requestor+ Material Type+ Movement Type,From and To+ Status only
                else if (paramtr[0] != "Select One" && paramtr[1] == "Select One" && paramtr[2] != "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type = '" + paramtr[2] + "' and M_Type = '" + paramtr[3] + "' and M_Status in (" + stats + ") and Requestor = '" + paramtr[0] + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type = '" + paramtr[2] + "'and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "' and Requestor = '" + paramtr[0] + "'";
                    }
                }
                //If Requestor+ From + To + Material Type+ Movement Type only
                else if (paramtr[0] != "Select One" && paramtr[1] == "Select One" && paramtr[2] != "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] == "All")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and Requestor = '" + paramtr[0] + "'and M_Type = '" + paramtr[3] + "' and S_Type = '" + paramtr[2] + "'";
                }
                //If Requestor+ From + To + Material Type+ only
                else if (paramtr[0] != "Select One" && paramtr[1] == "Select One" && paramtr[2] == "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] == "All")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and Requestor = '" + paramtr[0] + "'and M_Type = '" + paramtr[3] + "'";
                }
                //If Requestor+ From + To + Material Type+ Status only
                else if (paramtr[0] != "Select One" && paramtr[1] == "Select One" && paramtr[2] == "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and M_Type = '" + paramtr[3] + "' and M_Status in (" + stats + ") and Requestor = '" + paramtr[0] + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "' and Requestor = '" + paramtr[0] + "'";
                    }
                }
                //If Requestor+ From +  Movement Type +To only
                else if (paramtr[0] != "Select One" && paramtr[1] == "Select One" && paramtr[2] != "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] == "All")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and Requestor = '" + paramtr[0] + "' and S_Type = '" + paramtr[2] + "'";
                }
                //If Requestor+ From +  Movement Type +To+ Status  only
                else if (paramtr[0] != "Select One" && paramtr[1] == "Select One" && paramtr[2] != "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type = '" + paramtr[2] + "' and  M_Status in (" + stats + ") and Requestor = '" + paramtr[0] + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type = '" + paramtr[2] + "' and M_Status = '" + stats + "' and Requestor = '" + paramtr[0] + "'";
                    }
                }
                //If only Status+ From+To provided
                else if (paramtr[0] == "Select One" && paramtr[1] == "Select One" && paramtr[2] == "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and M_Status in (" + stats + ")";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and M_Status = '" + stats + "'";
                    }
                }
                //If only Movement type + From+ To is provided
                else if (paramtr[0] != "All" && paramtr[1] == "Select One" && paramtr[2] != "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] == "All")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type ='" + paramtr[2] + "'";
                }
                //If only Material type+From +To is provided
                else if (paramtr[0] == "Select One" && paramtr[1] == "Select One" && paramtr[2] == "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] == "All")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and  M_Type ='" + paramtr[3] + "'";
                }
                //If only From and To provided
                else if (paramtr[0] == "Select One" && paramtr[1] == "Select One" && paramtr[2] == "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'  ";
                }
                //If Material Type +Movement Type+ From +To is provided
                else if ((paramtr[0] == "Select One" || paramtr[0] != "All") && paramtr[1] == "Select One" && paramtr[2] == "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] == "")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' and RaiseDate <= '" + t + "'  and S_Type = '" + paramtr[2] + "'  and M_Type ='" + paramtr[3] + "'";
                }
                //If Movement Type,From and To+Status provided
                else if (paramtr[0] == "Select One" && paramtr[1] == "Select One" && paramtr[2] != "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept  where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type = '" + paramtr[2] + "' and M_Status in (" + stats + ")";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type = '" + paramtr[2] + "' and M_Status = '" + stats + "'";
                    }

                }
                //If Material Type,From and To+ Status provided
                else if (paramtr[0] == "Select One" && paramtr[1] == "Select One" && paramtr[2] == "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and M_Type = '" + paramtr[3] + "' and M_Status in (" + stats + ")";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "'";
                    }
                }
                //If Material Type+ Movement Type,From and To+ Status provided
                else if (paramtr[0] == "Select One" && paramtr[1] == "Select One" && paramtr[2] != "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type = '" + paramtr[2] + "' and M_Type = '" + paramtr[3] + "' and M_Status in (" + stats + ")";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type = '" + paramtr[2] + "'and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "'";
                    }
                }
                //Department, From and To provided
                else if (paramtr[0] == "Select One" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Status in (" + stats + ")";
                    }
                    else if (paramtr[6] == "All")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Status = '" + stats + "'";
                    }
                }
                //Department, From and To+ Requestor provided
                else if (paramtr[0] != "Select One" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'and Requestor = '" + paramtr[0] + "'";
                }
                //Department, From and To+  Material Type+ Movement Typeprovided
                else if (paramtr[0] == "Select One" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "")
                {
                    if (paramtr[1] == "Select One")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and S_Type = '" + paramtr[2] + "' and M_Type = '" + paramtr[3] + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'  and S_Type = '" + paramtr[2] + "' and M_Type = '" + paramtr[3] + "'";
                    }
                }
                //Department, From and To+  Material Type+ Movement Type + Requestor provided
                else if (paramtr[0] != "Select One" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'  and S_Type = '" + paramtr[2] + "' and M_Type = '" + paramtr[3] + "'and Requestor = '" + paramtr[0] + "'";
                }
                //Department, From and To+  Material Type 
                else if (paramtr[0] == "Select One" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Type = '" + paramtr[3] + "'";
                }
                //Department, From and To+  Material Type+ requestor 
                else if (paramtr[0] != "Select One" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Type = '" + paramtr[3] + "'and Requestor = '" + paramtr[0] + "'";
                }
                //Department, From and To+   Movement Type provided
                else if (paramtr[0] == "Select One" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'  and S_Type = '" + paramtr[2] + "'";
                }
                //Department, From and To+   Movement Type provided + requestor
                else if (paramtr[0] != "Select One" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "")
                {
                    query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'  and S_Type = '" + paramtr[2] + "'and Requestor = '" + paramtr[0] + "'";
                }

                //Department, From and To+   Material Type+Status provided
                else if (paramtr[0] == "Select One" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "'";
                    }

                }

                //Department, From and To+   Material Type+Status+ Requestor provided
                else if (paramtr[0] != "Select One" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "'and Requestor = '" + paramtr[0] + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "'and Requestor = '" + paramtr[0] + "'";
                    }

                }

                //Department, From and To+   Movement Type+Status provided
                else if (paramtr[0] == "Select One" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT  t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'  and S_Type = '" + paramtr[2] + "' and M_Status = '" + stats + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'  and S_Type = '" + paramtr[2] + "' and M_Status = '" + stats + "'";
                    }

                }

                //Department, From and To+   Movement Type+Status+ Requestor provided
                else if (paramtr[0] != "Select One" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] == "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'  and S_Type = '" + paramtr[2] + "' and M_Status = '" + stats + "'and Requestor = '" + paramtr[0] + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and  RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "'  and S_Type = '" + paramtr[2] + "' and M_Status = '" + stats + "'and Requestor = '" + paramtr[0] + "'";
                    }

                }

                //All Selected
                else if (paramtr[0] != "Select One" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "All" && paramtr[4] != "" && paramtr[5] != "" && paramtr[6] != "All")
                {
                    if (paramtr[6] == "OUT")
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "'and Requestor = '" + paramtr[0] + "' and S_Type = '" + paramtr[2] + "'";
                    }
                    else
                    {
                        query = "SELECT t.ChallanNmbr as[Challan Number],Req.NAME as[Requestor],Dept.DepartmentName as Department,RaiseDate as [Raise Date],t.M_Type as[Material Type],t.S_Type as [Supplier Type],t.OutFrom as[From Location],t.M_Location as [To Location],t.ModeOfTrans as [Mode Of Transport], t.SecApproveTime as [Security Approve Time],t.Descriptn as[Description], t.M_Status as[Status] FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where Req.EMPLOYEE_ID = t.Requestor and Req.DepartmentID = Dept.D_ID and RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "'and Dept ='" + paramtr[1] + "' and M_Type = '" + paramtr[3] + "' and M_Status = '" + stats + "'and Requestor = '" + paramtr[0] + "' and S_Type = '" + paramtr[2] + "'";
                    }
                }

                else
                {
                    query = "SELECT * FROM tblIN_Out t,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where RaiseDate >= '" + f + "' AND RaiseDate <= '" + t + "' and M_Type = '" + paramtr[3] + "' and S_Type = '" + paramtr[2] + "' ";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        MaterialReport mr = new MaterialReport();
                        mr.ChallanNmbr = rdr["Challan Number"].ToString();
                        mr.Requestor = rdr["Requestor"].ToString();
                        mr.Dept = rdr["Department"].ToString();
                        mr.RaiseDate = Convert.ToDateTime(rdr["Raise Date"]);
                        mr.M_Type = rdr["Material Type"].ToString();
                        mr.S_Type = rdr["Supplier Type"].ToString();
                        if (rdr["To Location"].ToString() == "")
                        {
                            mr.M_Location = "Third Party";
                        }
                        else
                        {
                            mr.M_Location = rdr["To Location"].ToString();
                        }
                        mr.OutFrom = rdr["From Location"].ToString();
                        mr.ModeOfTrans = rdr["Mode Of Transport"].ToString();
                        mr.Description = rdr["Description"].ToString();
                        mr.SecApproveTime = rdr["Security Approve Time"].ToString() != null ? rdr["Security Approve Time"].ToString() : "Pending";

                        if (rdr["Status"].ToString() == "IN" || rdr["Status"].ToString() == "OUT TW" || rdr["Status"].ToString() == "OUT")
                        {
                            mr.M_Status = "Open";
                        }
                        else
                        {
                            mr.M_Status = rdr["Status"].ToString();
                        }
                        Report.Add(mr);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Report;
        }

    

        public List<DataField> ReportVisitorPDF(params string[] paramtr)
        {
            int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);





            List<DataField> Report = new List<DataField>();
            try
            {
                string[] arrFrom = paramtr[3] != "" ? paramtr[3].ToString().Split('/') : null;
                string[] arrTo = paramtr[4] != "" ? paramtr[4].ToString().Split('/') : null;
                int fromM = arrFrom != null ? Convert.ToInt32(arrFrom[0]) : 0;
                int toM = arrTo != null ? Convert.ToInt32(arrTo[0]) : 0;

                string f = arrFrom != null ? arrFrom[2] + "-" + arrFrom[1] + "-" + arrFrom[0] : DateTime.Now.Year + "-" + DateTime.Now.AddMonths(-1).Month.ToString() + "-" + DateTime.Now.Day;
                string t = arrTo != null ? arrTo[2] + "-" + arrTo[1] + "-" + arrTo[0] : DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;

                if (con.State == ConnectionState.Closed) { con.Open(); }
                string query = "";

                //If From and To parameter is provided DONE
                if (paramtr[0] == "All" && paramtr[1] == "All" && (paramtr[2] == "All" || paramtr[2] == "") && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] == "All")
                {
                    //query = "select A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate from tblAppoinment A,tblCompany C, tblVisitor V,"+ KeyMasterDB + ".[dbo].[Master_Requestor] Req,"+ KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "'";
                    query = "select  distinct A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' ";
                }

                //If From and To parameter AND Host name is provided DONE
                else if (paramtr[0] == "All" && paramtr[1] == "All" && (paramtr[2] == "All" || paramtr[2] == "") && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All")
                {
                    query = "select distinct  A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "'";
                }

                //If Host + From + To + Visit Type  name is provided DONE
                else if (paramtr[0] != "All" && paramtr[1] == "All" && (paramtr[2] == "All" || paramtr[2] == "") && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] == "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.Visitor_Type = '" + paramtr[0] + "'";
                }

                //If Host + From + To + Visit Type+Company is provided DONE
                else if (paramtr[0] != "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] == "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.ComanyID='" + paramtr[1] + "'";
                }

                //If Host + From + To + Company+Visitor is provided DONE
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] == "All")
                {
                    query = "select distinct     A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.VisitorID='" + paramtr[2] + "' and A.ComanyID='" + paramtr[1] + "'";
                }

                //If Host + From + To + Company+Visitor+Status is provided DONE
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] != "All")
                {
                    query = "select distinct      A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.VisitorID='" + paramtr[2] + "' and A.ComanyID='" + paramtr[1] + "' and A.V_Status='" + paramtr[6] + "'";
                }
                //If Host + From + To + Visitor is provided DONE
                else if (paramtr[0] == "All" && paramtr[1] == "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] == "All")
                {
                    query = "select distinct           A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.VisitorID='" + paramtr[2] + "'";
                }

                //If Host + From + To + Visitor+Status is provided DONE
                else if (paramtr[0] == "All" && paramtr[1] == "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] != "All")
                {
                    query = "select distinct         A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.VisitorID='" + paramtr[2] + "' and A.V_Status='" + paramtr[6] + "'";
                }

                //If Host + From + To + Company + Status is provided DONE
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] != "All")
                {
                    query = "select distinct        A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "'  and A.V_Status='" + paramtr[6] + "' and A.ComanyID='" + paramtr[1] + "'";
                }

                //IF Host,from,to and company is provided
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] == "All")
                {
                    query = "select distinct          A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Host,to and company is provided
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] == "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] == "All")
                {
                    query = "select distinct       A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.ComanyID='" + paramtr[1] + "'";
                }

                //If Host + From + To  + Status is provided DONE
                else if (paramtr[0] == "All" && paramtr[1] == "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] != "All")
                {
                    query = "select distinct          A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "'  and A.V_Status='" + paramtr[6] + "'";
                }
                //If Host + From + To + Visit Type + Company + Visitor is provided DONE
                else if (paramtr[0] != "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] == "All")
                {
                    query = "select distinct        A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.VisitorID='" + paramtr[2] + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If All Parameters Done
                else if (paramtr[0] != "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] != "All")
                {
                    query = "select distinct      A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Requestor='" + paramtr[5] + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.VisitorID='" + paramtr[2] + "' and A.V_Status='" + paramtr[6] + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Visit Type+ From+ To provided
                else if (paramtr[0] != "All" && paramtr[1] == "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] == "All")
                {
                    query = "select distinct     A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Visitor_Type = '" + paramtr[0] + "'";
                }
                //If Visit Type+ From+ To + Company provided
                else if (paramtr[0] != "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] == "All")
                {
                    query = "select distinct     A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Visitor_Type = '" + paramtr[0] + "'  and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Visit Type+ From+ To + Visitor provided
                else if (paramtr[0] != "All" && paramtr[1] == "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] == "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.VisitorID='" + paramtr[2] + "'";
                }
                //If Visit Type+ From+ To + Status provided
                else if (paramtr[0] != "All" && paramtr[1] == "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] != "All")
                {
                    query = "select distinct      A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "'and A.Visitor_Type = '" + paramtr[0] + "' and A.V_Status='" + paramtr[6] + "'";
                }
                //Visit type,Requestor,From,To and status
                else if (paramtr[0] != "All" && paramtr[1] == "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] != "All" && paramtr[6] != "All")
                {
                    query = "select distinct     A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.V_Status='" + paramtr[6] + "' and A.Requestor='" + paramtr[5] + "' and A.Visitor_Type='" + paramtr[0] + "'";
                }
                //If Visit Type+ From+ To +Company +Visitor provided
                else if (paramtr[0] != "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] == "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.VisitorID='" + paramtr[2] + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Visit Type+ From+ To +Company +Visitor+ Status provided
                else if (paramtr[0] != "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] != "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.VisitorID='" + paramtr[2] + "' and A.V_Status='" + paramtr[6] + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Visit Type+ From+ To +Company+ Status provided
                else if (paramtr[0] != "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] != "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.V_Status='" + paramtr[6] + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Visit Type+ From+ To +Visitor + Status provided
                else if (paramtr[0] != "All" && paramtr[1] == "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] != "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.Visitor_Type = '" + paramtr[0] + "' and A.VisitorID='" + paramtr[2] + "' and A.V_Status='" + paramtr[6] + "'";
                }
                //If Company + From + To provided
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] == "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Company + From + To + Visitor provided
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] == "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.VisitorID='" + paramtr[2] + "'and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Company + From + To + Status provided
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] != "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.V_Status='" + paramtr[6] + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Company + From + To + Visitor + Status provided
                else if (paramtr[0] == "All" && paramtr[1] != "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] != "All")
                {
                    query = "select distinct    A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.VisitorID='" + paramtr[2] + "' and A.V_Status='" + paramtr[6] + "' and A.ComanyID='" + paramtr[1] + "'";
                }
                //If Visitor + From + To  provided
                else if (paramtr[0] == "All" && paramtr[1] == "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] == "All")
                {
                    query = "select distinct   A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.VisitorID='" + paramtr[2] + "'";
                }
                //If Visitor + From + To + Status  provided
                else if (paramtr[0] == "All" && paramtr[1] == "All" && paramtr[2] != "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] != "All")
                {
                    query = "select distinct   A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.VisitorID='" + paramtr[2] + "' and A.V_Status='" + paramtr[6] + "'";
                }
                //If  From + To + Status  provided
                else if (paramtr[0] == "All" && paramtr[1] == "All" && paramtr[2] == "All" && paramtr[3] != "" && paramtr[4] != "" && paramtr[5] == "All" && paramtr[6] != "All")
                {
                    query = "select distinct   A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "' and A.V_Status='" + paramtr[6] + "'";
                }
                //If all parameter is provided
                else
                {
                    query = "select distinct   A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate,isnull(A.RejectRemark,'') as RejectRemark,case when V.regionId=1 then 'Visitor' when  V.regionId=2 then 'CoVisitor' else '' end as V_Type,A.RaiseDate,isnull([dbo].[udf_GetRegion](A.RegionId),'') as Region,ISNULL(Vehicleno,'') as Vehicleno from tblAppoinment A,tblCompany C, tblVisitor V," + KeyMasterDB + ".[dbo].[Master_Requestor] Req," + KeyMasterDB + ".[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and A.Visitor_Type = '" + paramtr[0] + "' and A.ComanyID='" + paramtr[1] + "' and A.VisitorID='" + paramtr[2] + "' and A.Requestor = '" + paramtr[5] + "' and V_DateFrom >= '" + f + "' and A.V_DateFrom <= '" + t + "' and A.V_Status='" + paramtr[6] + "'";
                }
                // query = query + " order by A.V_InDate desc";

                if (HttpContext.Current.Session["RoleType"] != null)//Added 30june
                {

                    if (HttpContext.Current.Session["RoleType"].ToString() == "A")//Added 30june
                    {


                        query = query + " order by A.RaiseDate desc";

                    }
                    else if (HttpContext.Current.Session["RoleType"].ToString() == "Employee")
                    {

                        string strReq = HttpContext.Current.Session["PNICODE"].ToString();
                        string qry2 = "and A.RegionId=" + RegionId + "   and  A.Requestor='" + strReq + "'";//Session["PNICODE"]
                        query = query + qry2 + " order by A.RaiseDate desc";

                    }
                    //Session["RoleType"] 




                    else
                    {

                        query = query + " and A.RegionId=" + RegionId + " order by A.RaiseDate desc";
                    }
                }




                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataField dataField = new DataField();
                        dataField.GP = rdr["GPID"].ToString();
                        dataField.Req = rdr["NAME"].ToString();
                        dataField.Dept = rdr["DepartmentName"].ToString();
                        dataField.comName = rdr["CName"].ToString();
                        dataField.visName = rdr["V_Name"].ToString();
                        dataField.Type = rdr["Visitor_Type"].ToString();
                        dataField.VisDate = Convert.ToDateTime(rdr["V_DateFrom"]);
                        dataField.VisTime = rdr["A_Time"].ToString();
                        dataField.VisDuration = rdr["Duration"].ToString();
                        dataField.Region = rdr["Region"].ToString();
                        dataField.Vehicleno = rdr["Vehicleno"].ToString();

                        //dataField.Status = rdr["V_Status"].ToString();
                        if (rdr["V_Status"].ToString() == "Pre")
                        {

                            //dataField.Status = "Prebooked";
                            dataField.Status = "Appointment";
                        }

                        else if (rdr["V_Status"].ToString() == "Arrived")
                        {

                            dataField.Status = "Check-in";
                        }

                        else if (rdr["V_Status"].ToString() == "OUT")
                        {

                            dataField.Status = "Check-out";
                        }
                        else if (rdr["V_Status"].ToString() == "Reject")
                        {

                            dataField.Status = "Reject";
                        }
                        else if (rdr["V_Status"].ToString() == "Cancel")
                        {

                            dataField.Status = "Cancel";
                        }




                        //--------------
                        dataField.Location = rdr["Location"].ToString();
                        dataField.Remark = rdr["Remark"].ToString();
                        //dataField.TimeIN = rdr["V_InDate"].ToString();
                        //dataField.TimeOUT = rdr["V_OutDate"].ToString();
                        string str11 = rdr["V_InDate"].ToString();
                        if (rdr["V_InDate"].ToString() != "")
                        {
                            dataField.TimeIN = Convert.ToDateTime(rdr["V_InDate"].ToString()).ToString("dd/MMM/yyyy hh:mm tt");
                        }
                        else
                        {
                            dataField.TimeIN = "NA";
                        }
                        if (rdr["V_OutDate"].ToString() != "")
                        {
                            dataField.TimeOUT = Convert.ToDateTime(rdr["V_OutDate"].ToString()).ToString("dd/MMM/yyyy hh:mm tt");
                        }
                        else
                        {
                            dataField.TimeOUT = "NA";
                        }


                        dataField.strVisDate = Convert.ToDateTime(rdr["V_DateFrom"].ToString()).ToString("dd/MMM/yyyy");

                        dataField.RejectRemark = rdr["RejectRemark"].ToString();
                        dataField.V_Type = rdr["V_Type"].ToString();
                        Report.Add(dataField);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Report;
        }
        public List<DataFieldCourier> ReportCourierPDF(params string[] paramtr)
        {
            int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
            List<DataFieldCourier> Report = new List<DataFieldCourier>();
            try
            {
                string[] arrFrom = paramtr[3] != "" ? paramtr[3].ToString().Split('/') : null;
                string[] arrTo = paramtr[4] != "" ? paramtr[4].ToString().Split('/') : null;
                int fromM = arrFrom != null ? Convert.ToInt32(arrFrom[0]) : 0;
                int toM = arrTo != null ? Convert.ToInt32(arrTo[0]) : 0;

                string f = arrFrom != null ? arrFrom[2] + "-" + arrFrom[1] + "-" + arrFrom[0] : DateTime.Now.Year + "-" + DateTime.Now.AddMonths(-1).Month.ToString() + "-" + DateTime.Now.Day;
                string t = arrTo != null ? arrTo[2] + "-" + arrTo[1] + "-" + arrTo[0] : DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                t = t + "  23:59:02.473";
                if (con.State == ConnectionState.Closed) { con.Open(); }
                string query = "";
                string employeeuserId = paramtr[7];
                //If From and To parameter is provided DONE

                if ( paramtr[3] != "" && paramtr[4] != "" && paramtr[6] == "All")
                {
                    // query = "select A.GPID,Req.NAME,Dept.DepartmentName,C.CName,V.V_Name,A.Visitor_Type,A.V_DateFrom,A.A_Time,A.Duration,A.V_Status,A.Location,A.Remark,A.V_InDate,A.V_OutDate from tblAppoinment A,tblCompany C, tblVisitor V,[MasterDatabaseCO].[dbo].[Master_Requestor] Req,[MasterDatabaseCO].[dbo].[Department] Dept where A.ComanyID = C.C_ID and Req.EMPLOYEE_ID = A.Requestor and req.DepartmentID = Dept.D_ID and A.VisitorID = V.V_ID  and V_DateFrom >= '" + f + "' AND V_DateFrom <= '" + t + "'";
                    //[dbo].[udf_GetEmployeeName](ISNULL(TRY_CAST(EmployeeId AS INT), 0)) as EmployeeName
                    query = "Select CouriarId, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, Remark, CurrentStatus, TransactionDate, C_Date, EmployeeId, [dbo].[udf_GetEmployeeName](ISNULL(TRY_CAST(EmployeeId AS INT), 0)) as EmployeeName, DeskRemarks, isnull(DeskTransDate,'1900-01-01') as DeskTransDate, EmployeeRemark, isnull(EmployeeTransDate,'1900-01-01') as EmployeeTransDate, CouriarType, Department, Person, ContactNo, Address,isnull(PacketType,'') as PacketType ,isnull(SenderName,'') as SenderName,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion  from tblCouriar where CouriarType='Inward' and TransactionDate >= '" + f + "' AND TransactionDate <= '" + t + "'";
                }
                else if (paramtr[3] != "" && paramtr[4] != "" && paramtr[6] != "")
                {
               
                    query = "Select CouriarId, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, Remark, CurrentStatus, TransactionDate, C_Date, EmployeeId, [dbo].[udf_GetEmployeeName](ISNULL(TRY_CAST(EmployeeId AS INT), 0)) as EmployeeName, DeskRemarks, isnull(DeskTransDate,'1900-01-01') as DeskTransDate, EmployeeRemark, isnull(EmployeeTransDate,'1900-01-01') as EmployeeTransDate, CouriarType, Department, Person, ContactNo, Address,isnull(PacketType,'') as PacketType ,isnull(SenderName,'') as SenderName,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion  from tblCouriar where CouriarType='Inward' and TransactionDate >= '" + f + "' AND TransactionDate <= '" + t + "'  and CurrentStatus='" + paramtr[6] + "'";
                }

               
                //If all parameter is provided
                else
                {
                    

                    query = "Select CouriarId, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, Remark, CurrentStatus, TransactionDate, C_Date, EmployeeId, [dbo].[udf_GetEmployeeName](ISNULL(TRY_CAST(EmployeeId AS INT), 0)) as EmployeeName, DeskRemarks, isnull(DeskTransDate,'1900-01-01') as DeskTransDate, EmployeeRemark, isnull(EmployeeTransDate,'1900-01-01') as EmployeeTransDate, CouriarType, Department, Person, ContactNo, Address,isnull(PacketType,'') as PacketType ,isnull(SenderName,'') as SenderName,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion  from tblCouriar where CouriarType='Inward' and TransactionDate >= '" + f + "' AND TransactionDate <= '" + t + "'  and CurrentStatus='" + paramtr[6] + "' ";
                }

                if (paramtr[7] != "") {

                    if (paramtr[7] == "Admin")//Added30 june
                    {

                    }
                    else
                    {
                        query = query + " and EmployeeId=" + paramtr[7] + " ";
                    }


                    //query = query + "  and EmployeeId='" + paramtr[7] + "' ";
                }

                if (paramtr[7] == "Admin")//Added30 june
                {

                }
                else
                {

                    query = query + " and RegionId=" + RegionId + " order by CouriarId desc ";
                }


                //query = query + " and RegionId=" + RegionId + " order by CouriarId desc ";
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataFieldCourier dataField = new DataFieldCourier();
                        dataField.CouriarId =Convert.ToInt32( rdr["CouriarId"].ToString());
                        dataField.strC_Date = Convert.ToDateTime(rdr["C_Date"].ToString()).ToString("dd/MMM/yyyy");
                        dataField.Region = rdr["Region"].ToString();
                        dataField.NameOfCompany = rdr["NameOfCompany"].ToString();
                        dataField.City = rdr["City"].ToString();
                        dataField.Documents = rdr["Documents"].ToString();

                        dataField.ReceiveTime = rdr["ReceiveTime"].ToString();
                        dataField.DocketNo = rdr["DocketNo"].ToString();
                        dataField.CouriarVendor = rdr["CouriarVendor"].ToString();
                        dataField.CurrentStatus = rdr["CurrentStatus"].ToString();
                        dataField.TransactionDate =Convert.ToDateTime (rdr["TransactionDate"].ToString());


                        dataField.Department = rdr["Department"].ToString();
                        //dataField.Department =Convert.ToDateTime(rdr["TransactionDate"].ToString()).ToString("dd/MMM/yyyy hh:mm tt"); 

                       dataField.C_Date = Convert.ToDateTime(rdr["C_Date"].ToString());


                        dataField.DeskTransDate = Convert.ToDateTime(rdr["DeskTransDate"].ToString());
                   
                        dataField.EmployeeTransDate = Convert.ToDateTime(rdr["EmployeeTransDate"].ToString());
                        dataField.DeskRemarks= rdr["DeskRemarks"].ToString();
                        dataField.EmployeeRemark = rdr["EmployeeRemark"].ToString();
                     

                        dataField.strTransactionDate = Convert.ToDateTime(rdr["TransactionDate"].ToString()).ToString("dd/MMM/yyyy hh:mm tt");
                        dataField.strDeskTransDate = Convert.ToDateTime(rdr["DeskTransDate"].ToString()).ToString("dd/MMM/yyyy hh:mm tt").Replace("01/Jan/1900 12:00 AM", "NA");
                        dataField.strEmployeeTransDate = Convert.ToDateTime(rdr["EmployeeTransDate"].ToString()).ToString("dd/MMM/yyyy hh:mm tt").Replace("01/Jan/1900 12:00 AM", "NA");

                        dataField.PacketType= rdr["PacketType"].ToString();

                        dataField.SenderName = rdr["SenderName"].ToString();


                        dataField.Remark = rdr["Remark"].ToString();


                        dataField.AdminRegion = rdr["AdminRegion"].ToString();
                        dataField.EmployeeName = rdr["EmployeeName"].ToString();

                        Report.Add(dataField);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Report;
        }

        public List<DataFieldCourier> ReportDispatchPDF(params string[] paramtr)
        {
            List<DataFieldCourier> Report = new List<DataFieldCourier>();
            try
            {
                int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                string[] arrFrom = paramtr[3] != "" ? paramtr[3].ToString().Split('/') : null;
                string[] arrTo = paramtr[4] != "" ? paramtr[4].ToString().Split('/') : null;
                int fromM = arrFrom != null ? Convert.ToInt32(arrFrom[0]) : 0;
                int toM = arrTo != null ? Convert.ToInt32(arrTo[0]) : 0;
                string employeeuserId = paramtr[7];
                string f = arrFrom != null ? arrFrom[2] + "-" + arrFrom[1] + "-" + arrFrom[0] : DateTime.Now.Year + "-" + DateTime.Now.AddMonths(-1).Month.ToString() + "-" + DateTime.Now.Day;
                string t = arrTo != null ? arrTo[2] + "-" + arrTo[1] + "-" + arrTo[0] : DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                t = t + "  23:59:02.473";
                if (con.State == ConnectionState.Closed) { con.Open(); }
                string query = "";

                //If From and To parameter is provided DONE

                if (paramtr[3] != "" && paramtr[4] != "" && paramtr[6] == "All")
                {
                   

                    query = "Select CouriarId, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,isnull(Remark,'') as  Remark, CurrentStatus, TransactionDate, C_Date, EmployeeId, EmployeeName,isnull(DeskRemarks,'') as DeskRemarks, isnull(DeskTransDate,'1900-01-01') as DeskTransDate, isnull(EmployeeRemark,'') as EmployeeRemark, isnull(EmployeeTransDate,'1900-01-01') as EmployeeTransDate, CouriarType, Department, Person, ContactNo, Address,TransactionDate,isnull(DocketDate,'1900-01-01') as DocketDate, isnull(DeliveryDate,'1900-01-01') as DeliveryDate,isnull(FromCity,'') as FromCity,isnull(PacketType,'') as PacketType,isnull(SenderName,'') as SenderName,isnull(Region,'') as Region, isnull(Department,'') as Department,isnull(SenderContactNo,'') as SenderContactNo,isnull(HazardousItemYN,'') as HazardousItemYN,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile   from tblCouriar where CouriarType='Outward' and TransactionDate >= '" + f + "' AND TransactionDate <= '" + t + "'";
                }
                else if (paramtr[3] != "" && paramtr[4] != "" && paramtr[6] != "")
                {

                    //query = "Select CouriarId, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, Remark, CurrentStatus, TransactionDate, C_Date, EmployeeId, EmployeeName, isnull(DeskRemarks,'') as DeskRemarks, isnull(DeskTransDate,'1900-01-01') as DeskTransDate, EmployeeRemark, isnull(EmployeeTransDate,'1900-01-01') as EmployeeTransDate, CouriarType, Department, Person, ContactNo, Address,isnull(DocketDate,'1900-01-01') as DocketDate, isnull(DeliveryDate,'1900-01-01') as DeliveryDate,isnull(FromCity,'') as FromCity,isnull(PacketType,'') as PacketType,isnull(SenderName,'') as SenderName,isnull(Region,'') as Region, isnull(Department,'') as Department,isnull(SenderContactNo,'') as SenderContactNo   from tblCouriar where CouriarType='Outward' and TransactionDate >= '" + f + "' AND TransactionDate <= '" + t + "'  and CurrentStatus='" + paramtr[6] + "' ";
                    query = "Select CouriarId, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,isnull(Remark,'') as  Remark, CurrentStatus, TransactionDate, C_Date, EmployeeId, EmployeeName,isnull(DeskRemarks,'') as DeskRemarks, isnull(DeskTransDate,'1900-01-01') as DeskTransDate, isnull(EmployeeRemark,'') as EmployeeRemark, isnull(EmployeeTransDate,'1900-01-01') as EmployeeTransDate, CouriarType, Department, Person, ContactNo, Address,TransactionDate,isnull(DocketDate,'1900-01-01') as DocketDate, isnull(DeliveryDate,'1900-01-01') as DeliveryDate,isnull(FromCity,'') as FromCity,isnull(PacketType,'') as PacketType,isnull(SenderName,'') as SenderName,isnull(Region,'') as Region, isnull(Department,'') as Department,isnull(SenderContactNo,'') as SenderContactNo,isnull(HazardousItemYN,'') as HazardousItemYN,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile   from tblCouriar where CouriarType='Outward' and TransactionDate >= '" + f + "' AND TransactionDate <= '" + t + "'  and CurrentStatus='" + paramtr[6] + "' ";

                }


                //If all parameter is provided
                else
                {


                    // query = "Select CouriarId, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor, Remark, CurrentStatus, TransactionDate, C_Date, EmployeeId, EmployeeName, DeskRemarks, isnull(DeskTransDate,'1900-01-01') as DeskTransDate, EmployeeRemark, isnull(EmployeeTransDate,'1900-01-01') as EmployeeTransDate, CouriarType, Department, Person, ContactNo, Address,isnull(DocketDate,'1900-01-01') as DocketDate, isnull(DeliveryDate,'1900-01-01') as DeliveryDate,isnull(FromCity,'') as FromCity,isnull(PacketType,'') as PacketType,isnull(SenderName,'') as SenderName  from tblCouriar where CouriarType='Outward' and TransactionDate >= '" + f + "' AND TransactionDate <= '" + t + "'  and CurrentStatus='" + paramtr[6] + "' ";
                    query = "Select CouriarId, Region, NameOfCompany, City, Documents, ReceiveTime, DocketNo, CouriarVendor,isnull(Remark,'') as  Remark, CurrentStatus, TransactionDate, C_Date, EmployeeId, EmployeeName,isnull(DeskRemarks,'') as DeskRemarks, isnull(DeskTransDate,'1900-01-01') as DeskTransDate, isnull(EmployeeRemark,'') as EmployeeRemark, isnull(EmployeeTransDate,'1900-01-01') as EmployeeTransDate, CouriarType, Department, Person, ContactNo, Address,TransactionDate,isnull(DocketDate,'1900-01-01') as DocketDate, isnull(DeliveryDate,'1900-01-01') as DeliveryDate,isnull(FromCity,'') as FromCity,isnull(PacketType,'') as PacketType,isnull(SenderName,'') as SenderName,isnull(Region,'') as Region, isnull(Department,'') as Department,isnull(SenderContactNo,'') as SenderContactNo,isnull(HazardousItemYN,'') as HazardousItemYN,isnull([dbo].[udf_GetRegion](RegionId),'') as AdminRegion,isnull(PODFile,'') as PODFile  from tblCouriar where CouriarType='Outward' and TransactionDate >= '" + f + "' AND TransactionDate <= '" + t + "'  and CurrentStatus='" + paramtr[6] + "' ";
                }
                if (paramtr[7] != "")
                {
                    if (paramtr[7] == "Admin")//Added30 june
                    {
                        
                    }
                    else
                    {
                        query = query + " and EmployeeId=" + paramtr[7] + " ";
                    }

                    //query = query + " and EmployeeId=" + paramtr[7] + " ";
                }
                if (paramtr[7] == "Admin")//Added30 june
                {

                }
                else
                {

                    query = query + " and RegionId=" + RegionId + " order by CouriarId desc ";
                }
                //   query = query + " and RegionId=" + RegionId + " order by CouriarId desc ";
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataFieldCourier dataField = new DataFieldCourier();
                        dataField.CouriarId = Convert.ToInt32(rdr["CouriarId"].ToString());
                        dataField.Region = rdr["Region"].ToString();
                        dataField.NameOfCompany = rdr["NameOfCompany"].ToString();
                        dataField.City = rdr["City"].ToString();
                        dataField.Documents = rdr["Documents"].ToString();

                        dataField.ReceiveTime = rdr["ReceiveTime"].ToString();
                        dataField.DocketNo = rdr["DocketNo"].ToString();
                        dataField.CouriarVendor = rdr["CouriarVendor"].ToString();
                        dataField.CurrentStatus = rdr["CurrentStatus"].ToString();
                        dataField.TransactionDate = Convert.ToDateTime(rdr["TransactionDate"].ToString());
                        dataField.Department = rdr["Department"].ToString();
                        dataField.C_Date = Convert.ToDateTime(rdr["C_Date"].ToString());
                        dataField.Person=rdr["Person"].ToString();
                        dataField.ContactNo = rdr["ContactNo"].ToString();

                        
                        dataField.EmployeeTransDate = Convert.ToDateTime(rdr["EmployeeTransDate"].ToString());
                       
                        dataField.EmployeeRemark = rdr["EmployeeRemark"].ToString();
                        //---------------
                        dataField.strC_Date = Convert.ToDateTime(rdr["C_Date"].ToString()).ToString("dd/MMM/yyyy");
                        dataField.strTransactionDate = Convert.ToDateTime(rdr["TransactionDate"].ToString()).ToString("dd/MMM/yyyy hh:mm tt");
                        dataField.strDeskTransDate = Convert.ToDateTime(rdr["DeskTransDate"].ToString()).ToString("dd/MMM/yyyy hh:mm tt").Replace("01/Jan/1900 12:00 AM", "NA");
                        dataField.strEmployeeTransDate = Convert.ToDateTime(rdr["EmployeeTransDate"].ToString()).ToString("dd/MMM/yyyy hh:mm tt").Replace("01/Jan/1900 12:00 AM", "NA");
                        dataField.DocketDate = Convert.ToDateTime(rdr["DocketDate"].ToString());
                        dataField.DeliveryDate = Convert.ToDateTime(rdr["DeliveryDate"].ToString());
                        dataField.FromCity = rdr["FromCity"].ToString();
                        dataField.PacketType = rdr["PacketType"].ToString();
                        dataField.SenderName = rdr["SenderName"].ToString();
                        dataField.Department = rdr["Department"].ToString();
                        dataField.Region = rdr["Region"].ToString();
                        dataField.SenderContactNo = rdr["SenderContactNo"].ToString();

                        dataField.DeskTransDate = Convert.ToDateTime(rdr["DeskTransDate"].ToString());
                        dataField.Remark= rdr["Remark"].ToString();
                        dataField.DeskRemarks = rdr["DeskRemarks"].ToString();
                        dataField.EmployeeRemark = rdr["EmployeeRemark"].ToString();
                        dataField.HazardousItemYN= rdr["HazardousItemYN"].ToString();
                        dataField.AdminRegion = rdr["AdminRegion"].ToString();
                        dataField.Address = rdr["Address"].ToString();
                        dataField.PODFile= rdr["PODFile"].ToString();
                        //-------------------


                        Report.Add(dataField);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Report;
        }

        public DataTable Datatable_QUERY(string query)
        {
            SqlDataAdapter Adpt = new SqlDataAdapter(query, con);
           DataTable dt = new DataTable();
            try
            {
                Adpt.Fill(dt);
            }
            catch (SqlException ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con != null)
                    if (con.State == ConnectionState.Open) con.Close();
                Adpt.Dispose();
            }
            return dt;
        }

        public DataTable Datatable_QUERYMaster(string query)
        {
            SqlDataAdapter Adpt = new SqlDataAdapter(query, Mastercon);
            DataTable dt = new DataTable();
            try
            {
                Adpt.Fill(dt);
            }
            catch (SqlException ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (Mastercon != null)
                    if (Mastercon.State == ConnectionState.Open) Mastercon.Close();
                Adpt.Dispose();
            }
            return dt;
        }

        //----------------


        public List<MaterialReport> ReportPO(params string[] paramtr)
        {
            List<MaterialReport> Report = new List<MaterialReport>();
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                string[] arrFrom = paramtr[0] != "" ? paramtr[0].ToString().Split('/') : null;
                string[] arrTo = paramtr[1] != "" ? paramtr[1].ToString().Split('/') : null;
                int fromM = arrFrom != null ? Convert.ToInt32(arrFrom[0]) : 0;
                int toM = arrTo != null ? Convert.ToInt32(arrTo[0]) : 0;

                string f = arrFrom != null ? arrFrom[2] + "-" + arrFrom[1] + "-" + arrFrom[0] : DateTime.Now.Year + "-" + DateTime.Now.AddMonths(-1).Month.ToString() + "-" + DateTime.Now.Day;
                string t = arrTo != null ? arrTo[2] + "-" + arrTo[1] + "-" + arrTo[0] : DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;

                string query = "select distinct P.ID, P.PODate,P.PONum,P.SupplierName,P.SupplierAddress,P.GST_No,i.Item,i.UOM,i.Qty,I.RecQty,I.CloseDate from tblPO P, tblPOItem I WHERE P.ID = I.PONmbr and P.PODate >= '" + f + "' and P.PODate <= '" + t + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        MaterialReport mr = new MaterialReport();
                        mr.ChallanNmbr = rdr["PONum"].ToString();
                        mr.Requestor = rdr["SupplierName"].ToString();
                        mr.Dept = rdr["SupplierAddress"].ToString();
                        mr.RaiseDate = Convert.ToDateTime(rdr["PODate"]);
                        mr.M_Type = rdr["GST_No"].ToString();
                        mr.S_Type = rdr["Item"].ToString();
                        mr.M_Location = rdr["UOM"].ToString();
                        mr.OutFrom = rdr["Qty"].ToString();
                        mr.ModeOfTrans = rdr["RecQty"].ToString();
                        mr.SecApproveTime = rdr["CloseDate"].ToString() != null ? rdr["CloseDate"].ToString() : "Pending";
                        Report.Add(mr);
                    }
                }
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return Report;
        }
        public void WhatsApp(string nmbr, string templatename, string company, string visitDate, string VisitTime, string url)
        {
            var client = new RestClient("https://api.in.freshchat.com/v2/outbound-messages/whatsapp");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJsY1pWMF82VTNza2RKU19GOV9qdGNMNXotNlZPc05EaGNuZ1lLNHFHaVJvIn0.eyJqdGkiOiJlZWNkMzQwYi1mNjNlLTQzYTktYmM4OC01YzZjYjEyMjIwZWMiLCJleHAiOjE5MzAwMjYyNDgsIm5iZiI6MCwiaWF0IjoxNjE0NjY2MjQ4LCJpc3MiOiJodHRwOi8vaW50ZXJuYWwtZmMtYXBzMS0wMC1hbGIta2V5Y2xvYWstMjAzODM0MDkxMS5hcC1zb3V0aC0xLmVsYi5hbWF6b25hd3MuY29tL2F1dGgvcmVhbG1zL3Byb2R1Y3Rpb24iLCJhdWQiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJzdWIiOiI3N2EzYzljNi0yMjdiLTQ0NGEtYmY1Yy05YWJlMTA2NDYwYTEiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJhdXRoX3RpbWUiOjAsInNlc3Npb25fc3RhdGUiOiIwYTU3MGU4MS04NWMwLTQ0N2EtYjdmNy03MzU5YzIwYjVmZjciLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im1lc3NhZ2U6Z2V0IGFnZW50OnVwZGF0ZSBkYXNoYm9hcmQ6cmVhZCBhZ2VudDpyZWFkIG1lc3NhZ2U6Y3JlYXRlIGFnZW50OmRlbGV0ZSByZXBvcnRzOnJlYWQgY29udmVyc2F0aW9uOnVwZGF0ZSBpbWFnZTp1cGxvYWQgcmVwb3J0czpleHRyYWN0IHVzZXI6dXBkYXRlIGFnZW50OmNyZWF0ZSByZXBvcnRzOmV4dHJhY3Q6cmVhZCB1c2VyOnJlYWQgZmlsdGVyaW5ib3g6cmVhZCBvdXRib3VuZG1lc3NhZ2U6Z2V0IHJvbGU6cmVhZCByZXBvcnRzOmZldGNoIGZpbHRlcmluYm94OmNvdW50OnJlYWQgY29udmVyc2F0aW9uOnJlYWQgdXNlcjpkZWxldGUgY29udmVyc2F0aW9uOmNyZWF0ZSBvdXRib3VuZG1lc3NhZ2U6c2VuZCBiaWxsaW5nOnVwZGF0ZSB1c2VyOmNyZWF0ZSIsImNsaWVudElkIjoiYTE0ZWY0NDUtNzZkYy00NzEzLWIzZmItNGVlMWZjNDY3YzQ2IiwiY2xpZW50SG9zdCI6IjEwLjY4LjkuMTA3IiwiY2xpZW50QWRkcmVzcyI6IjEwLjY4LjkuMTA3In0.O1zXrx2Fa4YEMCpQaW6Qp9dM0nd8y8eI5GwEUaXDYtwgl3nxRvReXzV1I6vvAIyFUQVFmqXGAlp8eRPiDsapJaX6qNj0SzN2rwfqEw7bz5eAEcIM1dEOP8siud7VAwagvrjC8lp1XO4s1K9r4yxyFNjyQLRhBTNdPNHQl9YtNTXi-wNFSrZj3ToCafDsNjbWMUDcMa1AkDo1HSE4fN5B5b7rZODozAUVQxSSS7OoxQMSYE8rzgW-1PgG8HkmwBmY2gfCtEtTiNy1gGNuUlzzlyChkNlZhEPNhAV394HloirX92h_wGVykNKcR9SBU5x-AiZJpdBN9-H6KmazrPjxow");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\"from\": {\t\"phone_number\": \"+919044842222\"\t},\t\"provider\":\t\"whatsapp\", \t\"to\": \t[\t{\t\"phone_number\": \"+91" + nmbr + "\"}\n],\"data\": {\"message_template\": {\"storage\": \"none\",\"namespace\": \"75df6b91_7be4_4cc2_909d_e754e228870f\",\"template_name\": \"" + templatename + "\",\"language\": {\"policy\": \"deterministic\",\"code\": \"en\"},\"template_data\":[\n{\"data\":\"" + company + "\"},{\"data\": \"" + visitDate + "\"},{\"data\": \"" + VisitTime + "\"},{\"data\": \"" + url + "\"}\n]\n}\n}\n}", ParameterType.RequestBody);
            //request.AddParameter("application/json", "{\n\"from\": {\t\"phone_number\": \"+919044842222\"\t},\t\"provider\":\t\"whatsapp\", \t\"to\": \t[\t{\t\"phone_number\": \"+91" + nmbr + "\"}\n],\"data\": {\"message_template\": {\"storage\": \"none\",\"namespace\": \"75df6b91_7be4_4cc2_909d_e754e228870f\",\"template_name\": \"" + templatename + "\",\"language\": {\"policy\": \"deterministic\",\"code\": \"en\"},\"template_data\":[\n{\"data\":\"" + company + "\"},{\"data\": \"" + visitDate + "\"},{\"data\": \"" + VisitTime + "\"},{\"data\": \"" + url + "\"}\n]\n}", ParameterType.RequestBody);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
        public void WhatsApp(string nmbr, string templatename, string company)
        {
            var client = new RestClient("https://api.in.freshchat.com/v2/outbound-messages/whatsapp");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJsY1pWMF82VTNza2RKU19GOV9qdGNMNXotNlZPc05EaGNuZ1lLNHFHaVJvIn0.eyJqdGkiOiJlZWNkMzQwYi1mNjNlLTQzYTktYmM4OC01YzZjYjEyMjIwZWMiLCJleHAiOjE5MzAwMjYyNDgsIm5iZiI6MCwiaWF0IjoxNjE0NjY2MjQ4LCJpc3MiOiJodHRwOi8vaW50ZXJuYWwtZmMtYXBzMS0wMC1hbGIta2V5Y2xvYWstMjAzODM0MDkxMS5hcC1zb3V0aC0xLmVsYi5hbWF6b25hd3MuY29tL2F1dGgvcmVhbG1zL3Byb2R1Y3Rpb24iLCJhdWQiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJzdWIiOiI3N2EzYzljNi0yMjdiLTQ0NGEtYmY1Yy05YWJlMTA2NDYwYTEiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJhdXRoX3RpbWUiOjAsInNlc3Npb25fc3RhdGUiOiIwYTU3MGU4MS04NWMwLTQ0N2EtYjdmNy03MzU5YzIwYjVmZjciLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im1lc3NhZ2U6Z2V0IGFnZW50OnVwZGF0ZSBkYXNoYm9hcmQ6cmVhZCBhZ2VudDpyZWFkIG1lc3NhZ2U6Y3JlYXRlIGFnZW50OmRlbGV0ZSByZXBvcnRzOnJlYWQgY29udmVyc2F0aW9uOnVwZGF0ZSBpbWFnZTp1cGxvYWQgcmVwb3J0czpleHRyYWN0IHVzZXI6dXBkYXRlIGFnZW50OmNyZWF0ZSByZXBvcnRzOmV4dHJhY3Q6cmVhZCB1c2VyOnJlYWQgZmlsdGVyaW5ib3g6cmVhZCBvdXRib3VuZG1lc3NhZ2U6Z2V0IHJvbGU6cmVhZCByZXBvcnRzOmZldGNoIGZpbHRlcmluYm94OmNvdW50OnJlYWQgY29udmVyc2F0aW9uOnJlYWQgdXNlcjpkZWxldGUgY29udmVyc2F0aW9uOmNyZWF0ZSBvdXRib3VuZG1lc3NhZ2U6c2VuZCBiaWxsaW5nOnVwZGF0ZSB1c2VyOmNyZWF0ZSIsImNsaWVudElkIjoiYTE0ZWY0NDUtNzZkYy00NzEzLWIzZmItNGVlMWZjNDY3YzQ2IiwiY2xpZW50SG9zdCI6IjEwLjY4LjkuMTA3IiwiY2xpZW50QWRkcmVzcyI6IjEwLjY4LjkuMTA3In0.O1zXrx2Fa4YEMCpQaW6Qp9dM0nd8y8eI5GwEUaXDYtwgl3nxRvReXzV1I6vvAIyFUQVFmqXGAlp8eRPiDsapJaX6qNj0SzN2rwfqEw7bz5eAEcIM1dEOP8siud7VAwagvrjC8lp1XO4s1K9r4yxyFNjyQLRhBTNdPNHQl9YtNTXi-wNFSrZj3ToCafDsNjbWMUDcMa1AkDo1HSE4fN5B5b7rZODozAUVQxSSS7OoxQMSYE8rzgW-1PgG8HkmwBmY2gfCtEtTiNy1gGNuUlzzlyChkNlZhEPNhAV394HloirX92h_wGVykNKcR9SBU5x-AiZJpdBN9-H6KmazrPjxow");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\"from\": {\t\"phone_number\": \"+919044842222\"\t},\t\"provider\":\t\"whatsapp\", \t\"to\": \t[\t{\t\"phone_number\": \"+91" + nmbr + "\"}\n],\"data\": {\"message_template\": {\"storage\": \"none\",\"namespace\": \"75df6b91_7be4_4cc2_909d_e754e228870f\",\"template_name\": \"" + templatename + "\",\"language\": {\"policy\": \"deterministic\",\"code\": \"en\"},\"template_data\":[\n{\"data\":\"" + company + "\"}\n]\n}\n}\n}", ParameterType.RequestBody);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
        public void WhatsApp(string nmbr, string templatename, string company, string visitDate, string VisitTime)
        {
            var client = new RestClient("https://api.in.freshchat.com/v2/outbound-messages/whatsapp");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJsY1pWMF82VTNza2RKU19GOV9qdGNMNXotNlZPc05EaGNuZ1lLNHFHaVJvIn0.eyJqdGkiOiJlZWNkMzQwYi1mNjNlLTQzYTktYmM4OC01YzZjYjEyMjIwZWMiLCJleHAiOjE5MzAwMjYyNDgsIm5iZiI6MCwiaWF0IjoxNjE0NjY2MjQ4LCJpc3MiOiJodHRwOi8vaW50ZXJuYWwtZmMtYXBzMS0wMC1hbGIta2V5Y2xvYWstMjAzODM0MDkxMS5hcC1zb3V0aC0xLmVsYi5hbWF6b25hd3MuY29tL2F1dGgvcmVhbG1zL3Byb2R1Y3Rpb24iLCJhdWQiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJzdWIiOiI3N2EzYzljNi0yMjdiLTQ0NGEtYmY1Yy05YWJlMTA2NDYwYTEiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJhdXRoX3RpbWUiOjAsInNlc3Npb25fc3RhdGUiOiIwYTU3MGU4MS04NWMwLTQ0N2EtYjdmNy03MzU5YzIwYjVmZjciLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im1lc3NhZ2U6Z2V0IGFnZW50OnVwZGF0ZSBkYXNoYm9hcmQ6cmVhZCBhZ2VudDpyZWFkIG1lc3NhZ2U6Y3JlYXRlIGFnZW50OmRlbGV0ZSByZXBvcnRzOnJlYWQgY29udmVyc2F0aW9uOnVwZGF0ZSBpbWFnZTp1cGxvYWQgcmVwb3J0czpleHRyYWN0IHVzZXI6dXBkYXRlIGFnZW50OmNyZWF0ZSByZXBvcnRzOmV4dHJhY3Q6cmVhZCB1c2VyOnJlYWQgZmlsdGVyaW5ib3g6cmVhZCBvdXRib3VuZG1lc3NhZ2U6Z2V0IHJvbGU6cmVhZCByZXBvcnRzOmZldGNoIGZpbHRlcmluYm94OmNvdW50OnJlYWQgY29udmVyc2F0aW9uOnJlYWQgdXNlcjpkZWxldGUgY29udmVyc2F0aW9uOmNyZWF0ZSBvdXRib3VuZG1lc3NhZ2U6c2VuZCBiaWxsaW5nOnVwZGF0ZSB1c2VyOmNyZWF0ZSIsImNsaWVudElkIjoiYTE0ZWY0NDUtNzZkYy00NzEzLWIzZmItNGVlMWZjNDY3YzQ2IiwiY2xpZW50SG9zdCI6IjEwLjY4LjkuMTA3IiwiY2xpZW50QWRkcmVzcyI6IjEwLjY4LjkuMTA3In0.O1zXrx2Fa4YEMCpQaW6Qp9dM0nd8y8eI5GwEUaXDYtwgl3nxRvReXzV1I6vvAIyFUQVFmqXGAlp8eRPiDsapJaX6qNj0SzN2rwfqEw7bz5eAEcIM1dEOP8siud7VAwagvrjC8lp1XO4s1K9r4yxyFNjyQLRhBTNdPNHQl9YtNTXi-wNFSrZj3ToCafDsNjbWMUDcMa1AkDo1HSE4fN5B5b7rZODozAUVQxSSS7OoxQMSYE8rzgW-1PgG8HkmwBmY2gfCtEtTiNy1gGNuUlzzlyChkNlZhEPNhAV394HloirX92h_wGVykNKcR9SBU5x-AiZJpdBN9-H6KmazrPjxow");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\"from\": {\t\"phone_number\": \"+919044842222\"\t},\t\"provider\":\t\"whatsapp\", \t\"to\": \t[\t{\t\"phone_number\": \"+91" + nmbr + "\"}\n],\"data\": {\"message_template\": {\"storage\": \"none\",\"namespace\": \"75df6b91_7be4_4cc2_909d_e754e228870f\",\"template_name\": \"" + templatename + "\",\"language\": {\"policy\": \"deterministic\",\"code\": \"en\"},\"template_data\":[\n{\"data\":\"" + company + "\"},{\"data\": \"" + visitDate + "\"},{\"data\": \"" + VisitTime + "\"}\n]\n}\n}\n}", ParameterType.RequestBody);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
        public void WhatsApp(string nmbr, string templatename, string Type, string ChallanNmbr)
        {
            var client = new RestClient("https://api.in.freshchat.com/v2/outbound-messages/whatsapp");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJsY1pWMF82VTNza2RKU19GOV9qdGNMNXotNlZPc05EaGNuZ1lLNHFHaVJvIn0.eyJqdGkiOiJlZWNkMzQwYi1mNjNlLTQzYTktYmM4OC01YzZjYjEyMjIwZWMiLCJleHAiOjE5MzAwMjYyNDgsIm5iZiI6MCwiaWF0IjoxNjE0NjY2MjQ4LCJpc3MiOiJodHRwOi8vaW50ZXJuYWwtZmMtYXBzMS0wMC1hbGIta2V5Y2xvYWstMjAzODM0MDkxMS5hcC1zb3V0aC0xLmVsYi5hbWF6b25hd3MuY29tL2F1dGgvcmVhbG1zL3Byb2R1Y3Rpb24iLCJhdWQiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJzdWIiOiI3N2EzYzljNi0yMjdiLTQ0NGEtYmY1Yy05YWJlMTA2NDYwYTEiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJhdXRoX3RpbWUiOjAsInNlc3Npb25fc3RhdGUiOiIwYTU3MGU4MS04NWMwLTQ0N2EtYjdmNy03MzU5YzIwYjVmZjciLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im1lc3NhZ2U6Z2V0IGFnZW50OnVwZGF0ZSBkYXNoYm9hcmQ6cmVhZCBhZ2VudDpyZWFkIG1lc3NhZ2U6Y3JlYXRlIGFnZW50OmRlbGV0ZSByZXBvcnRzOnJlYWQgY29udmVyc2F0aW9uOnVwZGF0ZSBpbWFnZTp1cGxvYWQgcmVwb3J0czpleHRyYWN0IHVzZXI6dXBkYXRlIGFnZW50OmNyZWF0ZSByZXBvcnRzOmV4dHJhY3Q6cmVhZCB1c2VyOnJlYWQgZmlsdGVyaW5ib3g6cmVhZCBvdXRib3VuZG1lc3NhZ2U6Z2V0IHJvbGU6cmVhZCByZXBvcnRzOmZldGNoIGZpbHRlcmluYm94OmNvdW50OnJlYWQgY29udmVyc2F0aW9uOnJlYWQgdXNlcjpkZWxldGUgY29udmVyc2F0aW9uOmNyZWF0ZSBvdXRib3VuZG1lc3NhZ2U6c2VuZCBiaWxsaW5nOnVwZGF0ZSB1c2VyOmNyZWF0ZSIsImNsaWVudElkIjoiYTE0ZWY0NDUtNzZkYy00NzEzLWIzZmItNGVlMWZjNDY3YzQ2IiwiY2xpZW50SG9zdCI6IjEwLjY4LjkuMTA3IiwiY2xpZW50QWRkcmVzcyI6IjEwLjY4LjkuMTA3In0.O1zXrx2Fa4YEMCpQaW6Qp9dM0nd8y8eI5GwEUaXDYtwgl3nxRvReXzV1I6vvAIyFUQVFmqXGAlp8eRPiDsapJaX6qNj0SzN2rwfqEw7bz5eAEcIM1dEOP8siud7VAwagvrjC8lp1XO4s1K9r4yxyFNjyQLRhBTNdPNHQl9YtNTXi-wNFSrZj3ToCafDsNjbWMUDcMa1AkDo1HSE4fN5B5b7rZODozAUVQxSSS7OoxQMSYE8rzgW-1PgG8HkmwBmY2gfCtEtTiNy1gGNuUlzzlyChkNlZhEPNhAV394HloirX92h_wGVykNKcR9SBU5x-AiZJpdBN9-H6KmazrPjxow");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\"from\": {\t\"phone_number\": \"+919044842222\"\t},\t\"provider\":\t\"whatsapp\", \t\"to\": \t[\t{\t\"phone_number\": \"+91" + nmbr + "\"}\n],\"data\": {\"message_template\": {\"storage\": \"none\",\"namespace\": \"75df6b91_7be4_4cc2_909d_e754e228870f\",\"template_name\": \"" + templatename + "\",\"language\": {\"policy\": \"deterministic\",\"code\": \"en\"},\"template_data\":[\n{\"data\":\"" + Type + "\"},{\"data\": \"" + ChallanNmbr + "\"}\n]\n}\n}\n}", ParameterType.RequestBody);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
        public void WhatsApp_With(string nmbr, string templatename)
        {
            var client = new RestClient("https://api.in.freshchat.com/v2/outbound-messages/whatsapp");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJsY1pWMF82VTNza2RKU19GOV9qdGNMNXotNlZPc05EaGNuZ1lLNHFHaVJvIn0.eyJqdGkiOiJlZWNkMzQwYi1mNjNlLTQzYTktYmM4OC01YzZjYjEyMjIwZWMiLCJleHAiOjE5MzAwMjYyNDgsIm5iZiI6MCwiaWF0IjoxNjE0NjY2MjQ4LCJpc3MiOiJodHRwOi8vaW50ZXJuYWwtZmMtYXBzMS0wMC1hbGIta2V5Y2xvYWstMjAzODM0MDkxMS5hcC1zb3V0aC0xLmVsYi5hbWF6b25hd3MuY29tL2F1dGgvcmVhbG1zL3Byb2R1Y3Rpb24iLCJhdWQiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJzdWIiOiI3N2EzYzljNi0yMjdiLTQ0NGEtYmY1Yy05YWJlMTA2NDYwYTEiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJhdXRoX3RpbWUiOjAsInNlc3Npb25fc3RhdGUiOiIwYTU3MGU4MS04NWMwLTQ0N2EtYjdmNy03MzU5YzIwYjVmZjciLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im1lc3NhZ2U6Z2V0IGFnZW50OnVwZGF0ZSBkYXNoYm9hcmQ6cmVhZCBhZ2VudDpyZWFkIG1lc3NhZ2U6Y3JlYXRlIGFnZW50OmRlbGV0ZSByZXBvcnRzOnJlYWQgY29udmVyc2F0aW9uOnVwZGF0ZSBpbWFnZTp1cGxvYWQgcmVwb3J0czpleHRyYWN0IHVzZXI6dXBkYXRlIGFnZW50OmNyZWF0ZSByZXBvcnRzOmV4dHJhY3Q6cmVhZCB1c2VyOnJlYWQgZmlsdGVyaW5ib3g6cmVhZCBvdXRib3VuZG1lc3NhZ2U6Z2V0IHJvbGU6cmVhZCByZXBvcnRzOmZldGNoIGZpbHRlcmluYm94OmNvdW50OnJlYWQgY29udmVyc2F0aW9uOnJlYWQgdXNlcjpkZWxldGUgY29udmVyc2F0aW9uOmNyZWF0ZSBvdXRib3VuZG1lc3NhZ2U6c2VuZCBiaWxsaW5nOnVwZGF0ZSB1c2VyOmNyZWF0ZSIsImNsaWVudElkIjoiYTE0ZWY0NDUtNzZkYy00NzEzLWIzZmItNGVlMWZjNDY3YzQ2IiwiY2xpZW50SG9zdCI6IjEwLjY4LjkuMTA3IiwiY2xpZW50QWRkcmVzcyI6IjEwLjY4LjkuMTA3In0.O1zXrx2Fa4YEMCpQaW6Qp9dM0nd8y8eI5GwEUaXDYtwgl3nxRvReXzV1I6vvAIyFUQVFmqXGAlp8eRPiDsapJaX6qNj0SzN2rwfqEw7bz5eAEcIM1dEOP8siud7VAwagvrjC8lp1XO4s1K9r4yxyFNjyQLRhBTNdPNHQl9YtNTXi-wNFSrZj3ToCafDsNjbWMUDcMa1AkDo1HSE4fN5B5b7rZODozAUVQxSSS7OoxQMSYE8rzgW-1PgG8HkmwBmY2gfCtEtTiNy1gGNuUlzzlyChkNlZhEPNhAV394HloirX92h_wGVykNKcR9SBU5x-AiZJpdBN9-H6KmazrPjxow");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\"from\": {\t\"phone_number\": \"+919044842222\"\t},\t\"provider\":\t\"whatsapp\", \t\"to\": \t[\t{\t\"phone_number\": \"+91" + nmbr + "\"}\n],\"data\": {\"message_template\": {\"storage\": \"none\",\"namespace\": \"75df6b91_7be4_4cc2_909d_e754e228870f\",\"template_name\": \"" + templatename + "\",\"language\": {\"policy\": \"deterministic\",\"code\": \"en\"}\n}\n}\n}", ParameterType.RequestBody);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
        public void WhatsAppAttachment(string nmbr,string location,string company, string URl)
        {
            var client = new RestClient("https://api.in.freshchat.com/v2/outbound-messages/whatsapp");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJsY1pWMF82VTNza2RKU19GOV9qdGNMNXotNlZPc05EaGNuZ1lLNHFHaVJvIn0.eyJqdGkiOiJlZWNkMzQwYi1mNjNlLTQzYTktYmM4OC01YzZjYjEyMjIwZWMiLCJleHAiOjE5MzAwMjYyNDgsIm5iZiI6MCwiaWF0IjoxNjE0NjY2MjQ4LCJpc3MiOiJodHRwOi8vaW50ZXJuYWwtZmMtYXBzMS0wMC1hbGIta2V5Y2xvYWstMjAzODM0MDkxMS5hcC1zb3V0aC0xLmVsYi5hbWF6b25hd3MuY29tL2F1dGgvcmVhbG1zL3Byb2R1Y3Rpb24iLCJhdWQiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJzdWIiOiI3N2EzYzljNi0yMjdiLTQ0NGEtYmY1Yy05YWJlMTA2NDYwYTEiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJhMTRlZjQ0NS03NmRjLTQ3MTMtYjNmYi00ZWUxZmM0NjdjNDYiLCJhdXRoX3RpbWUiOjAsInNlc3Npb25fc3RhdGUiOiIwYTU3MGU4MS04NWMwLTQ0N2EtYjdmNy03MzU5YzIwYjVmZjciLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im1lc3NhZ2U6Z2V0IGFnZW50OnVwZGF0ZSBkYXNoYm9hcmQ6cmVhZCBhZ2VudDpyZWFkIG1lc3NhZ2U6Y3JlYXRlIGFnZW50OmRlbGV0ZSByZXBvcnRzOnJlYWQgY29udmVyc2F0aW9uOnVwZGF0ZSBpbWFnZTp1cGxvYWQgcmVwb3J0czpleHRyYWN0IHVzZXI6dXBkYXRlIGFnZW50OmNyZWF0ZSByZXBvcnRzOmV4dHJhY3Q6cmVhZCB1c2VyOnJlYWQgZmlsdGVyaW5ib3g6cmVhZCBvdXRib3VuZG1lc3NhZ2U6Z2V0IHJvbGU6cmVhZCByZXBvcnRzOmZldGNoIGZpbHRlcmluYm94OmNvdW50OnJlYWQgY29udmVyc2F0aW9uOnJlYWQgdXNlcjpkZWxldGUgY29udmVyc2F0aW9uOmNyZWF0ZSBvdXRib3VuZG1lc3NhZ2U6c2VuZCBiaWxsaW5nOnVwZGF0ZSB1c2VyOmNyZWF0ZSIsImNsaWVudElkIjoiYTE0ZWY0NDUtNzZkYy00NzEzLWIzZmItNGVlMWZjNDY3YzQ2IiwiY2xpZW50SG9zdCI6IjEwLjY4LjkuMTA3IiwiY2xpZW50QWRkcmVzcyI6IjEwLjY4LjkuMTA3In0.O1zXrx2Fa4YEMCpQaW6Qp9dM0nd8y8eI5GwEUaXDYtwgl3nxRvReXzV1I6vvAIyFUQVFmqXGAlp8eRPiDsapJaX6qNj0SzN2rwfqEw7bz5eAEcIM1dEOP8siud7VAwagvrjC8lp1XO4s1K9r4yxyFNjyQLRhBTNdPNHQl9YtNTXi-wNFSrZj3ToCafDsNjbWMUDcMa1AkDo1HSE4fN5B5b7rZODozAUVQxSSS7OoxQMSYE8rzgW-1PgG8HkmwBmY2gfCtEtTiNy1gGNuUlzzlyChkNlZhEPNhAV394HloirX92h_wGVykNKcR9SBU5x-AiZJpdBN9-H6KmazrPjxow");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\"from\": {\t\"phone_number\": \"+919044842222\"\t},\t\"provider\":\t\"whatsapp\", \t\"to\": \t[\t{\t\"phone_number\": \"+91" + nmbr + "\"}\n],\"data\": {\"message_template\": {\"storage\": \"none\",\"namespace\": \"75df6b91_7be4_4cc2_909d_e754e228870f\",\"template_name\": \"enx_epass_generate_confirmation\",\"language\": {\"policy\": \"deterministic\",\"code\": \"en\"},\"rich_template_data\":{\"header\":{\n\"type\": \"image\",\"media_url\":\"" + URl + "\"},\n\"body\":{\"params\":[{\"data\":\"" + location + "\"},{\"data\":\"" + company + "\"}]} }\n}\n}\n}", ParameterType.RequestBody);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        //----------
        public IEnumerable<DataField3> MasterRequesterList
        {
            get
            {
                List<DataField3> Visitor = new List<DataField3>();
                try
                {

                    

                    if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                    // string Req = HttpContext.Current.Session["PNICODE"].ToString();
                    //SqlCommand cmd = new SqlCommand("select A.A_ID,A.GPID, A.V_DateFrom,A.A_Time,A.Duration,C.CName,V.V_Name,A.Visitor_Type, A.V_Status,Isnull(V.V_Type,'') as V_Type  from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.Requestor = '" + Req + "' and A.V_Status not in('Out','Cancel') ORDER BY  A.A_ID DESC", con);
                    SqlCommand cmd = new SqlCommand("select Top 1 *  from Master_Requestor", Mastercon);



                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            DataField3 dataField = new DataField3();
                            dataField.Email_ID = rdr["Email ID"].ToString();
                            dataField.Sr_No = Convert.ToInt64(rdr["Sr#No"].ToString());
                            //dataField.NAME = Convert.ToDateTime(rdr["NAME"]);
                            //dataField.VisTime = rdr["A_Time"].ToString();
                            dataField.NAME = rdr["NAME"].ToString();
                            //dataField.comName = rdr["CName"].ToString();
                            //dataField.visName = rdr["V_Name"].ToString();
                            //dataField.Type = rdr["Visitor_Type"].ToString();
                            //dataField.Status = rdr["V_Status"].ToString();
                            //dataField.V_Type = rdr["V_Type"].ToString();

                            Visitor.Add(dataField);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendExcepToDB(ex);
                }
                finally
                {
                    if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
                }
                return Visitor;
            }
        }



        public IEnumerable<DataFieldEmployee> MasterRequesterListNew
        {
            get
            {
                List<DataFieldEmployee> Visitor = new List<DataFieldEmployee>();
                try
                {
                    if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }

                    int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                    // string Req = HttpContext.Current.Session["PNICODE"].ToString();
                    //SqlCommand cmd = new SqlCommand("select A.A_ID,A.GPID, A.V_DateFrom,A.A_Time,A.Duration,C.CName,V.V_Name,A.Visitor_Type, A.V_Status,Isnull(V.V_Type,'') as V_Type  from tblAppoinment A, tblCompany C, tblVisitor V where A.ComanyID = C.C_ID and A.VisitorID = V.V_ID and A.Requestor = '" + Req + "' and A.V_Status not in('Out','Cancel') ORDER BY  A.A_ID DESC", con);
                    //SqlCommand cmd = new SqlCommand("select Sr#No,Isnull(Name,'') as Name,Isnull([Email ID],'') as [Email ID],Isnull(DEPARTMENT,'') as DEPARTMENT,Isnull(DESIGNATION,'') as DESIGNATION,Isnull([KSS Department],'') as [KSS Department],Isnull([MOBILE NUMBER],'') as [MOBILE NUMBER],isnull([DATE OF BIRTH],'1900-01-01') as [DATE OF BIRTH],isnull([DATE OF JOINING],'1900-01-01') as [DATE OF JOINING],Isnull(Status,'') as Status,Isnull(EMPLOYEE_ID,'') as EMPLOYEE_ID  from Master_Requestor where regionId="+ RegionId + " and   status!='Cancel' order by  Sr#No desc", Mastercon);



                    SqlCommand cmd = new SqlCommand("select Sr#No,Isnull(Name,'') as Name,Isnull([Email ID],'') as [Email ID],Isnull(DEPARTMENT,'') as DEPARTMENT,Isnull(DESIGNATION,'') as DESIGNATION,Isnull([KSS Department],'') as [KSS Department],Isnull([MOBILE NUMBER],'') as [MOBILE NUMBER],isnull([DATE OF BIRTH],'1900-01-01') as [DATE OF BIRTH],isnull([DATE OF JOINING],'1900-01-01') as [DATE OF JOINING],Isnull(Status,'') as Status,Isnull(EMPLOYEE_ID,'') as EMPLOYEE_ID,isnull(Investment_Declaration_Password,'') as Investment_Declaration_Password,Region, isnull(HOD,'') as HOD,isnull( Immediate_Supervisor,'') as Immediate_Supervisor , isnull((select Region from Region where  Regionid=Mr.ActualRegionId),'')  as ActualRegion from Master_Requestor Mr where    status!='Cancel'  order by  Sr#No desc", Mastercon);


                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {

                            DataFieldEmployee dataField = new DataFieldEmployee();
                            dataField.Email_ID = rdr["Email ID"].ToString();
                            dataField.Sr_No = Convert.ToInt64(rdr["Sr#No"].ToString());
                            dataField.DEPARTMENT = rdr["DEPARTMENT"].ToString();
                            dataField.DESIGNATION = rdr["DESIGNATION"].ToString();
                            dataField.KSS_Department = rdr["KSS Department"].ToString();
                            dataField.MOBILE_NUMBER = Convert.ToDouble(rdr["MOBILE NUMBER"].ToString());
                            dataField.NAME = rdr["NAME"].ToString();
                            dataField.DATE_OF_BIRTH = Convert.ToDateTime(rdr["DATE OF BIRTH"].ToString());
                            dataField.DATE_OF_JOINING = Convert.ToDateTime(rdr["DATE OF JOINING"].ToString());
                            dataField.Status = rdr["Status"].ToString();
                            dataField.EMPLOYEE_ID = rdr["EMPLOYEE_ID"].ToString();
                            dataField.Investment_Declaration_Password = rdr["Investment_Declaration_Password"].ToString();
                            //dataField.visName = rdr["V_Name"].ToString();
                            //dataField.Type = rdr["Visitor_Type"].ToString();
                            //dataField.Status = rdr["V_Status"].ToString();
                            //dataField.V_Type = rdr["V_Type"].ToString();
                            dataField.Immediate_Supervisor= rdr["Immediate_Supervisor"].ToString();
                            dataField.HOD = rdr["HOD"].ToString();

                            dataField.Region= rdr["Region"].ToString();
                            dataField.ActualRegion = rdr["ActualRegion"].ToString();
                            
                            Visitor.Add(dataField);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendExcepToDB(ex);
                }
                finally
                {
                    if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
                }
                return Visitor;
            }
        }

        //-------------------

        //--------comany-------
        public string InsertCourierCompany(FormCollection fc)
        {
            string CID = "";
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }


                string CourierCompany = fc["CourierCompany"].ToString();
               
                string sql = "INSERT INTO tblCourierCompany ([CourierCompany],[Status]) VALUES('" + CourierCompany + "','Y') ";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;

                //connection.Open();
                cmd.ExecuteNonQuery();
                //connection.Close();



            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return CID;
        }

        public string UpdateCourierCompany(FormCollection fc, int CourierCompanyId)
        {
            string CID = "";
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }

               
              string CourierCompany=  fc["CourierCompany"].ToString();
                string status = fc["txtstatus"].ToString();
               
                string sql = "UPDATE tblCourierCompany  SET CourierCompany = '" + CourierCompany + "',Status = '" + status + "'  WHERE CourierCompanyId=" + CourierCompanyId + "";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;

               
                cmd.ExecuteNonQuery();
             



            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return CID;
        }

        public string DeleteCourierCompany(int CourierCompanyId)
        {
            string CID = "";
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                string sql = "UPDATE tblCourierCompany  SET Status = 'N'  WHERE CourierCompanyId=" + CourierCompanyId + "";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return CID;
        }
        //--------------
        //Feb24********************

        //***************Feb24
        //-----17april-------------
        public int InsertSqlCourierEntry(FormCollection fc)
        {
                try
                {

                string strtime = DateTime.Now.ToString("hh:mm tt");
                string Vdate = fc["V_DateFrom"].ToString();
                int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                //-----------Feb22-----
                string dept = fc["Deptment"].ToString();
                //if (dept.Length >= 50)
                //{
                //    dept = dept.Substring(0, 49);
                //}


                int Check = 0;
                    string strUDId = "";
                    SqlCommand cmd = new SqlCommand("", con);
                    cmd.Parameters.Clear();
                    cmd.Connection = con;
                    cmd.CommandText = "Proc_tblCouriar_Insert";
                    cmd.CommandType = CommandType.StoredProcedure;

                    //cmd.Parameters.AddWithValue("@DId", did);
                    cmd.Parameters.AddWithValue("@Remark", fc["txtremark"].ToString());
                    cmd.Parameters.AddWithValue("@CouriarVendor", fc["ddlCourierCompany"].ToString());
                    cmd.Parameters.AddWithValue("@DocketNo", fc["txtdocket"].ToString());
                cmd.Parameters.AddWithValue("@ReceiveTime", strtime);
                cmd.Parameters.AddWithValue("@Documents", fc["txtdoc"].ToString());
                cmd.Parameters.AddWithValue("@City", fc["ddlCity"].ToString());
                cmd.Parameters.AddWithValue("@C_Date", Convert.ToDateTime(Vdate));
                cmd.Parameters.AddWithValue("@CurrentStatus", "Open");
                cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@CouriarType", "Inward");
                cmd.Parameters.AddWithValue("@Region", fc["txtRegionNew"].ToString());
                cmd.Parameters.AddWithValue("@Department", dept);
                cmd.Parameters.AddWithValue("@NameOfCompany", fc["host"].ToString());
                cmd.Parameters.AddWithValue("@EmployeeName", fc["host"].ToString());
                cmd.Parameters.AddWithValue("@EmployeeId", fc["Requestor"].ToString());
                cmd.Parameters.AddWithValue("@SenderName", fc["txtSendername"].ToString());
                cmd.Parameters.AddWithValue("@PacketType", fc["ddlPacketType"].ToString());
                cmd.Parameters.AddWithValue("@RegionId", RegionId);
                

                cmd.Parameters.AddWithValue("@CommandType", "INSERT");
               
                    SqlParameter p1 = new SqlParameter("CouriarId", SqlDbType.Int);
                    cmd.Parameters.Add(p1);
                    p1.Direction = ParameterDirection.InputOutput;
                    SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                    cmd.Parameters.Add(p2);
                    p2.Direction = ParameterDirection.InputOutput;
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    int cnt = cmd.ExecuteNonQuery();

                    strUDId = cmd.Parameters["CouriarId"].Value.ToString();
                    Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
                //return strUDId;
                // return Check;//26
                return Convert.ToInt32(strUDId);
            }
                catch (Exception ex)
                {
                    return 0;
                }
                finally
                {
                    if (con.State == ConnectionState.Open) { con.Close(); }
                }

         




        }

        public int Updatehazardousitem(int couriarId, string Ishazardousitem)
        {

            int CID = 0;
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                string sql = "UPDATE tblCouriar  SET HazardousItemYN = '"+ Ishazardousitem + "'  WHERE CouriarId=" + couriarId + "";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return CID;


        }

        public int InsertSqlDispatchEntry(FormCollection fc)
        {
            try
            {


                int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                string Ishazardousitem = fc["operationType"].ToString();
                if (Ishazardousitem == "Routing")
                {
                    Ishazardousitem = "Yes";

                }
                else
                {
                    Ishazardousitem = "No";
                }

                string strtime = DateTime.Now.ToString("hh:mm tt");

                string Vdate = fc["V_DateFrom"].ToString();
                //objcCouriar.C_Date = Convert.ToDateTime(Vdate);


                int Check = 0;
                string strUDId = "";
                SqlCommand cmd = new SqlCommand("", con);
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "Proc_tblCouriarDIsptch_Insert";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Remark", fc["txtremark"].ToString());
                //cmd.Parameters.AddWithValue("@CouriarVendor", fc["ddlCourierCompany"].ToString());
                //cmd.Parameters.AddWithValue("@DocketNo", fc["txtdocket"].ToString());
                cmd.Parameters.AddWithValue("@ReceiveTime", strtime);
                cmd.Parameters.AddWithValue("@Documents", fc["txtdoc"].ToString());
                cmd.Parameters.AddWithValue("@City", fc["txtcity"].ToString());
                cmd.Parameters.AddWithValue("@C_Date", Convert.ToDateTime(Vdate));
                cmd.Parameters.AddWithValue("@CurrentStatus", "Open");
                cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@CouriarType", "Outward");
                cmd.Parameters.AddWithValue("@Region", fc["txtRegion"].ToString());
                cmd.Parameters.AddWithValue("@Department", fc["txtDepartment"].ToString());
                cmd.Parameters.AddWithValue("@NameOfCompany", fc["txtcompany"].ToString());
                //cmd.Parameters.AddWithValue("@EmployeeName", fc["host"].ToString());
                cmd.Parameters.AddWithValue("@EmployeeId", fc["txtEmpId"].ToString());
                cmd.Parameters.AddWithValue("@SenderName", fc["txtSendername"].ToString());
                cmd.Parameters.AddWithValue("@PacketType", fc["ddlPacketType"].ToString());


                //-------------
                cmd.Parameters.AddWithValue("@Person", fc["txtPerson"].ToString());
                cmd.Parameters.AddWithValue("@ContactNo", fc["txtContactNo"].ToString());
                cmd.Parameters.AddWithValue("@Address", fc["txtAddress"].ToString());
                cmd.Parameters.AddWithValue("@FromCity", fc["txtfromcity"].ToString());
                cmd.Parameters.AddWithValue("@SenderContactNo", fc["txtSenderContactno"].ToString());
                cmd.Parameters.AddWithValue("@HazardousItemYN", Ishazardousitem);
                cmd.Parameters.AddWithValue("@DeliveryDate", Convert.ToDateTime(fc["txtdeliverydate"].ToString()));

                cmd.Parameters.AddWithValue("@RegionId", RegionId);


                cmd.Parameters.AddWithValue("@CommandType", "INSERT");

                SqlParameter p1 = new SqlParameter("CouriarId", SqlDbType.Int);
                cmd.Parameters.Add(p1);
                p1.Direction = ParameterDirection.InputOutput;
                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int cnt = cmd.ExecuteNonQuery();

                strUDId = cmd.Parameters["CouriarId"].Value.ToString();
                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
                //return strUDId;
                //return Check;
                return Convert.ToInt32(strUDId);
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }






        }

        //----------------prevent duplicate insert---------

        public int preventduplicateInsert(string str)
        {

            int CID = 0;
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                string sql = "Insert into PreventDuplicate(Timestamp) values('"+ str + "') ";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return CID;


        }

        public int checkduplicatecount(string str)
        {

            int cnt = 0;
                try
                {
               
                    SqlCommand cmd = new SqlCommand(" select Timestamp from PreventDuplicate where Timestamp= '" + str + "'", con);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                        cnt = 1;
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    con.Close();
                }
                return cnt;
           
        }


        public int ChangeEmpMobileNo(string Empid,string Mobileno )
        {

            int Check = 0;
            try
            {
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                string sql = "  Update Master_Requestor set [MOBILE NUMBER]='" + Mobileno + "'  where [EMPLOYEE_ID]='" + Empid + "' ";
                SqlCommand cmd = new SqlCommand(sql, Mastercon);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                Check = 1;
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
            }
            return Check;


        }
        //------------------------------
        //------17 april------

        //----------26 june----------
        //**************Work**Insert Functionality in Region*****in Buisines class****************

        //using System.Web.Mvc;
        public string InsertRegion(FormCollection fc)
        {
            try
            {
                int Check = 0;
                string strRegionId = "";
                SqlCommand cmd = new SqlCommand("", Mastercon);
                cmd.Parameters.Clear();
                cmd.Connection = Mastercon;
                cmd.CommandText = "Proc_Region_Insert";
                cmd.CommandType = CommandType.StoredProcedure;
               
                cmd.Parameters.AddWithValue("@Region", checknullReplaceZero(fc["txtRegion"].ToString()));
                cmd.Parameters.AddWithValue("@Status", "Y");

                cmd.Parameters.AddWithValue("@CommandType", "INSERT");
                //SqlParameter p1 = new SqlParameter("RegionId", SqlDbType.VarChar, 6);
                SqlParameter p1 = new SqlParameter("RegionId", SqlDbType.Int);
                cmd.Parameters.Add(p1);
                p1.Direction = ParameterDirection.InputOutput;
                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                int cnt = cmd.ExecuteNonQuery();
                //int strRegionId = Convert.ToInt32(cmd.Parameters["RegionId"].Value.ToString());
                strRegionId = cmd.Parameters["RegionId"].Value.ToString();
                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
                return strRegionId;
            }
            catch (Exception ex)
            {
                return "0";
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }

        }

        public string InsertRegionDepartment(FormCollection fc)
        {
            int CID = 0;
            try
            {


                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }


                string DepartmentName = checknullReplaceZero(fc["txtDepartment"].ToString());
                //string Region = checknullReplaceZero(fc["txtRegion"].ToString());
                int ddlRegionId =Convert.ToInt32(checknullReplaceZero(fc["ddlRegion"].ToString()));
                var dt2 = new DataTable();
                string sqlregion = " Select Region from Region where RegionId="+ ddlRegionId + "";
                dt2 = Datatable_QUERYMaster(sqlregion);
                string Region = dt2.Rows[0]["Region"].ToString();
                var dt = new DataTable();
                string sqldept = " select isnull(max(D_ID),0)+1  as DepartmentId from Department";
                dt = Datatable_QUERYMaster(sqldept);
                int DepartId =Convert.ToInt32(dt.Rows[0]["DepartmentId"].ToString());
                string sql = " Insert into Department(DepartmentName, Region,D_ID) values('" + DepartmentName + "', '" + Region + "'," + DepartId + ") ";
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                SqlCommand cmd = new SqlCommand(sql, Mastercon);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                CID = 1;
            }
            catch (Exception ex)
            {
                CID = 0;
                SendExcepToDB(ex);
            }
            finally
            {
                if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
            }
            return Convert.ToString(CID);


            //try
            //{
            //    int Check = 0;
            //    string strRegionId = "";
            //    SqlCommand cmd = new SqlCommand("", Mastercon);
            //    cmd.Parameters.Clear();
            //    cmd.Connection = Mastercon;
            //    cmd.CommandText = "Proc_Region_Insert";
            //    cmd.CommandType = CommandType.StoredProcedure;

            //    cmd.Parameters.AddWithValue("@Region", checknullReplaceZero(fc["txtRegion"].ToString()));
            //    cmd.Parameters.AddWithValue("@Status", "Y");

            //    cmd.Parameters.AddWithValue("@CommandType", "INSERT");
            //    //SqlParameter p1 = new SqlParameter("RegionId", SqlDbType.VarChar, 6);
            //    SqlParameter p1 = new SqlParameter("RegionId", SqlDbType.Int);
            //    cmd.Parameters.Add(p1);
            //    p1.Direction = ParameterDirection.InputOutput;
            //    SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
            //    cmd.Parameters.Add(p2);
            //    p2.Direction = ParameterDirection.InputOutput;
            //    if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
            //    int cnt = cmd.ExecuteNonQuery();
            //    //int strRegionId = Convert.ToInt32(cmd.Parameters["RegionId"].Value.ToString());
            //    strRegionId = cmd.Parameters["RegionId"].Value.ToString();
            //    Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
            //    return strRegionId;
            //}
            //catch (Exception ex)
            //{
            //    return "0";
            //}
            //finally
            //{
            //    if (con.State == ConnectionState.Open) { con.Close(); }
            //}

        }

        public string UpdateRegionDepartment(FormCollection fc,int D_ID)
        {
            int CID = 0;
            try
            {


                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }


                string DepartmentName = checknullReplaceZero(fc["txtDepartment"].ToString());
                string Region = checknullReplaceZero(fc["txtRegion"].ToString());
                string ddlRegion = checknullReplaceZero(fc["ddlRegion"].ToString());
              
                string sql = " Update  Department set DepartmentName='" + DepartmentName + "',Region= '" + Region + "' where D_ID=" + D_ID + " ";

                SqlCommand cmd = new SqlCommand(sql, Mastercon);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                CID = 1;
            }
            catch (Exception ex)
            {
                CID = 0;
                SendExcepToDB(ex);
            }
            finally
            {
                if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
            }
            return Convert.ToString(CID);


            

        }

        //****************Update Functionality in Region*********************
        public int UpdateRegion(FormCollection fc, int RegionId)
        {
            int Check = 0;
            try
            {

                string strRegionId = "";
                SqlCommand cmd = new SqlCommand("", Mastercon);
                cmd.Parameters.Clear();
                cmd.Connection = Mastercon;
                cmd.CommandText = "Proc_Region_Insert";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RegionId", RegionId);
                cmd.Parameters.AddWithValue("@Region", checknullReplaceZero(fc["txtRegion"].ToString()));
                cmd.Parameters.AddWithValue("@Status", checknullReplaceZero(fc["txtStatus"].ToString()));

                cmd.Parameters.AddWithValue("@CommandType", "UPDATE");


                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                int cnt = cmd.ExecuteNonQuery();
                //int strRegionId = Convert.ToInt32(cmd.Parameters["RegionId"].Value.ToString());

                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
                return Check;
            }
            catch (Exception ex)
            {
                return Check;
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }

        }
        //****************Delete Functionality in Region*********************
        //public int DeleteRegion(FormCollection fc, int RegionId)
        public int DeleteRegion(int RegionId)
        {
            int Check = 0;
            try
            {

                string strRegionId = "";
                SqlCommand cmd = new SqlCommand("", Mastercon);
                cmd.Parameters.Clear();
                cmd.Connection = Mastercon;
                cmd.CommandText = "Proc_Region_Insert";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RegionId", RegionId);
                cmd.Parameters.AddWithValue("@CommandType", "Delete");


                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                int cnt = cmd.ExecuteNonQuery();
                //int strRegionId = Convert.ToInt32(cmd.Parameters["RegionId"].Value.ToString());

                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
                return Check;
            }
            catch (Exception ex)
            {
                return Check;
            }
            finally
            {
                if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
            }

        }



        //----------------

        private string checknullReplaceZero(string fcval)
        {

            string value = String.IsNullOrEmpty(fcval) ? "0" : fcval;
            return value;
        }




        //**************Work**Insert Functionality in WorkingLogin*****in Buisines class****************

        //using System.Web.Mvc;
        public string InsertWorkingLogin(FormCollection fc)
        {

            try
            {
                // int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                int regid = Convert.ToInt32(checknullReplaceZero(fc["txtRegionId"].ToString()));

                int Check = 0;
                string strW_ID = "";
                SqlCommand cmd = new SqlCommand("", con);
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "Proc_WorkingLogin_Insert";
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@W_ID", Convert.ToInt32(checknullReplaceZero(fc["txtW_ID"].ToString())));
                cmd.Parameters.AddWithValue("@UserID", checknullReplaceZero(fc["txtUserID"].ToString()));
                cmd.Parameters.AddWithValue("@UPassword", checknullReplaceZero(fc["txtUPassword"].ToString()));
                cmd.Parameters.AddWithValue("@S_Location", checknullReplaceZero(fc["txtS_Location"].ToString()));
                cmd.Parameters.AddWithValue("@WStatus", checknullReplaceZero(fc["txtWStatus"].ToString()));
                cmd.Parameters.AddWithValue("@RoleType", checknullReplaceZero(fc["txtRoleType"].ToString()));
                cmd.Parameters.AddWithValue("@Email", checknullReplaceZero(fc["txtEmail"].ToString()));
                cmd.Parameters.AddWithValue("@RegionId", regid); 

                cmd.Parameters.AddWithValue("@CommandType", "INSERT");
                //SqlParameter p1 = new SqlParameter("W_ID", SqlDbType.VarChar, 6);
                SqlParameter p1 = new SqlParameter("W_ID", SqlDbType.Int);
                cmd.Parameters.Add(p1);
                p1.Direction = ParameterDirection.InputOutput;
                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int cnt = cmd.ExecuteNonQuery();
                //int strW_ID = Convert.ToInt32(cmd.Parameters["W_ID"].Value.ToString());
                strW_ID = cmd.Parameters["W_ID"].Value.ToString();
                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
                return strW_ID;
            }
            catch (Exception ex)
            {
                return "0";
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }

        }

        //****************Update Functionality in WorkingLogin*********************
        public int UpdateWorkingLogin(FormCollection fc, int W_ID)
        {
            int Check = 0;
            try
            {

                int regid= Convert.ToInt32( checknullReplaceZero(fc["txtRegionId"].ToString()));
                //int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                string strW_ID = "";
                SqlCommand cmd = new SqlCommand("", con);
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "Proc_WorkingLogin_Insert";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@W_ID", Convert.ToInt32(checknullReplaceZero(fc["txtW_ID"].ToString())));
                cmd.Parameters.AddWithValue("@UserID", checknullReplaceZero(fc["txtUserID"].ToString()));
                cmd.Parameters.AddWithValue("@UPassword", checknullReplaceZero(fc["txtUPassword"].ToString()));
                cmd.Parameters.AddWithValue("@S_Location", checknullReplaceZero(fc["txtS_Location"].ToString()));
                cmd.Parameters.AddWithValue("@WStatus", checknullReplaceZero(fc["txtWStatus"].ToString()));
                cmd.Parameters.AddWithValue("@RoleType", checknullReplaceZero(fc["txtRoleType"].ToString()));
                cmd.Parameters.AddWithValue("@Email", checknullReplaceZero(fc["txtEmail"].ToString()));
                cmd.Parameters.AddWithValue("@RegionId", regid);
                //cmd.Parameters.AddWithValue("@RegionId", RegionId);
                cmd.Parameters.AddWithValue("@CommandType", "UPDATE");


                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int cnt = cmd.ExecuteNonQuery();
                //int strW_ID = Convert.ToInt32(cmd.Parameters["W_ID"].Value.ToString());

                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
                return Check;
            }
            catch (Exception ex)
            {
                return Check;
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }

        }
        //****************Delete Functionality in WorkingLogin*********************
        //public int DeleteWorkingLogin(FormCollection fc, int W_ID)
        public int DeleteWorkingLogin(int W_ID)
        {
            int Check = 0;
            try
            {

                string strW_ID = "";
                SqlCommand cmd = new SqlCommand("", con);
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "Proc_WorkingLogin_Insert";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@W_ID", W_ID);
                cmd.Parameters.AddWithValue("@CommandType", "Delete");


                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int cnt = cmd.ExecuteNonQuery();
                //int strW_ID = Convert.ToInt32(cmd.Parameters["W_ID"].Value.ToString());

                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
                return Check;
            }
            catch (Exception ex)
            {
                return Check;
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }

        }



        //----------------




        //*****************End With Sql***Buisiness********************

        //*****************End With Sql***Buisiness********************

        //--------26june
        //----------- july 5--------
        public string SaveRequestor(FormCollection fc)
        {
            string CID = "";
            try
            {

                //string operationType = fc["OperationType"].ToString();
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                //string address = com.Addres == null ? "NA" : com.Addres.ToUpper();
                DateTime datetdefault = DateTime.Now; 
                SqlCommand cmd = new SqlCommand("Proc_Master_Requestor_Insert", Mastercon);
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.AddWithValue("@EMPLOYEE_ID", checknullReplaceZero(fc["txtEMPLOYEE_ID"].ToString()));
                cmd.Parameters.AddWithValue("@NAME", checknullReplaceZero(fc["txtNAME"].ToString()));
                cmd.Parameters.AddWithValue("@KSS_Department", "NA");
                cmd.Parameters.AddWithValue("@requestor_key", "NA");
                cmd.Parameters.AddWithValue("@DESIGNATION", checknullReplaceZero(fc["txtDESIGNATION"].ToString()));
                cmd.Parameters.AddWithValue("@DEPARTMENT", checknullReplaceZero(fc["txtDEPARTMENT"].ToString()));
                cmd.Parameters.AddWithValue("@DepartmentID", Convert.ToInt32(checknullReplaceZero(fc["txtDepartmentID"].ToString())));
                cmd.Parameters.AddWithValue("@Immediate_Supervisor", checknullReplaceZero(fc["txtImmediate_Supervisor"].ToString()));//"NA"
                cmd.Parameters.AddWithValue("@Immediate_Supervisor_EMP_ID", "NA");
                cmd.Parameters.AddWithValue("@HOD", checknullReplaceZero(fc["txtHOD"].ToString()));
                cmd.Parameters.AddWithValue("@HOD_EMP_ID", "NA");
                cmd.Parameters.AddWithValue("@MOBILE_NUMBER", Convert.ToDouble(checknullReplaceZero(fc["txtMOBILE"].ToString())));
                cmd.Parameters.AddWithValue("@DATE_OF_BIRTH", datetdefault); //datetdefault);//checknullReplaceZero(fc["V_Dob"].ToString()))
                cmd.Parameters.AddWithValue("@PAYROLL", "PNI");
                cmd.Parameters.AddWithValue("@DATE_OF_JOINING", datetdefault);//Convert.ToDateTime(checknullReplaceZero(fc["V_Doj"].ToString()))
                cmd.Parameters.AddWithValue("@Email_ID", checknullReplaceZero(fc["txtEmail"].ToString()));
                cmd.Parameters.AddWithValue("@VPF", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@EPF", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@PAN_NO#", "NA");
                cmd.Parameters.AddWithValue("@Total_deductions", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@Yearly_VPF", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@Yearly_EPF", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@TrumpPassword", checknullReplaceZero(fc["txtTrumpPassword"].ToString()));
                cmd.Parameters.AddWithValue("@Status", checknullReplaceZero(fc["E_VStatus"].ToString()));
                cmd.Parameters.AddWithValue("@UserType", checknullReplaceZero(fc["ddlUserType"].ToString()));

               
                cmd.Parameters.AddWithValue("@InvestmentDeclarationStatus", false);
                cmd.Parameters.AddWithValue("@Investment_Declaration_Password", checknullReplaceZero(fc["txtInvestment_Declaration_Password"].ToString()));//Extno
                cmd.Parameters.AddWithValue("@HRMSPassword", "NA");
                //cmd.Parameters.AddWithValue("@TrumpPassword", checknullReplaceZero(fc["txtTrumpPassword"].ToString()));
                cmd.Parameters.AddWithValue("@CompanyID", 1);//Convert.ToInt32(checknullReplaceZero(fc["ddlCompany"].ToString()))
                cmd.Parameters.AddWithValue("@Region", checknullReplaceZero(fc["txtRegion"].ToString()));
                cmd.Parameters.AddWithValue("@RegionId", Convert.ToInt32(checknullReplaceZero(fc["txtRegionId"].ToString())));

                cmd.Parameters.AddWithValue("@ActualRegionId", Convert.ToInt32(checknullReplaceZero(fc["txtactualRegionId"].ToString())));
                
                cmd.Parameters.AddWithValue("@CommandType", "INSERT");
                SqlParameter sqlparam = new SqlParameter();
                sqlparam.ParameterName = "@Sr#No";
                sqlparam.SqlDbType = SqlDbType.Int;
                sqlparam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlparam);

                SqlParameter sqlparam2 = new SqlParameter();
                sqlparam2.ParameterName = "@IsSuccess";
                sqlparam2.SqlDbType = SqlDbType.Int;
                sqlparam2.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlparam2);



                int i = cmd.ExecuteNonQuery();
                CID = sqlparam.Value.ToString();
                string chk = sqlparam2.Value.ToString();


            }


            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
            }
            return CID;
        }


        public string UpdateRequestor( string stremployeeid,FormCollection fc)
        {
            string CID = "";
            try
            {
                DateTime datetdefault = DateTime.Now;
                //string operationType = fc["OperationType"].ToString();
                string name = fc["txtNAME"].ToString();
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                //string address = com.Addres == null ? "NA" : com.Addres.ToUpper();

                SqlCommand cmd = new SqlCommand("Proc_Master_Requestor_Insert", Mastercon);
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
               

                
                string gc = stremployeeid;

                cmd.Parameters.AddWithValue("@NAME", checknullReplaceZero(fc["txtNAME"].ToString()));
                cmd.Parameters.AddWithValue("@KSS_Department", "NA");
                cmd.Parameters.AddWithValue("@requestor_key", "NA");
                cmd.Parameters.AddWithValue("@DESIGNATION", checknullReplaceZero(fc["txtDESIGNATION"].ToString()));
                cmd.Parameters.AddWithValue("@DEPARTMENT", checknullReplaceZero(fc["txtDEPARTMENT"].ToString()));
                cmd.Parameters.AddWithValue("@DepartmentID", Convert.ToInt32(checknullReplaceZero(fc["txtDepartmentID"].ToString())));
                cmd.Parameters.AddWithValue("@Immediate_Supervisor", checknullReplaceZero(fc["txtImmediate_Supervisor"].ToString()));
                cmd.Parameters.AddWithValue("@Immediate_Supervisor_EMP_ID", "NA");
                cmd.Parameters.AddWithValue("@HOD", checknullReplaceZero(fc["txtHOD"].ToString()));
                cmd.Parameters.AddWithValue("@HOD_EMP_ID", "NA");
                cmd.Parameters.AddWithValue("@MOBILE_NUMBER", Convert.ToDouble(checknullReplaceZero(fc["txtMOBILE"].ToString())));
                // cmd.Parameters.AddWithValue("@DATE_OF_BIRTH", Convert.ToDateTime(checknullReplaceZero(fc["V_Dob"].ToString())));//datetdefault
                cmd.Parameters.AddWithValue("@DATE_OF_BIRTH", datetdefault);//

                cmd.Parameters.AddWithValue("@PAYROLL", "PNI");
                // cmd.Parameters.AddWithValue("@DATE_OF_JOINING", Convert.ToDateTime(checknullReplaceZero(fc["V_Doj"].ToString())));//datetdefault
                cmd.Parameters.AddWithValue("@DATE_OF_JOINING", datetdefault);//datetdefault
                cmd.Parameters.AddWithValue("@Email_ID", checknullReplaceZero(fc["txtEmail"].ToString()));
                cmd.Parameters.AddWithValue("@VPF", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@EPF", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@PAN_NO#", "NA");
                cmd.Parameters.AddWithValue("@Total_deductions", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@Yearly_VPF", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@Yearly_EPF", Convert.ToDouble(0));
                cmd.Parameters.AddWithValue("@TrumpPassword", checknullReplaceZero(fc["txtTrumpPassword"].ToString()));
                cmd.Parameters.AddWithValue("@Status", checknullReplaceZero(fc["ddlStatus"].ToString()));
                cmd.Parameters.AddWithValue("@UserType", checknullReplaceZero(fc["ddlUserType"].ToString()));
                cmd.Parameters.AddWithValue("@EMPLOYEE_ID", checknullReplaceZero(fc["txtEMPLOYEE_ID"].ToString()));

                
                cmd.Parameters.AddWithValue("@InvestmentDeclarationStatus", false);
                cmd.Parameters.AddWithValue("@Investment_Declaration_Password", checknullReplaceZero(fc["txtInvestment_Declaration_Password"].ToString()));
                cmd.Parameters.AddWithValue("@HRMSPassword", "NA");
;
                cmd.Parameters.AddWithValue("@CompanyID", 1);
                cmd.Parameters.AddWithValue("@Region", checknullReplaceZero(fc["txtRegion"].ToString()));
                cmd.Parameters.AddWithValue("@RegionId", Convert.ToInt32(checknullReplaceZero(fc["txtRegionId"].ToString())));
                cmd.Parameters.AddWithValue("@ActualRegionId", Convert.ToInt32(checknullReplaceZero(fc["txtactualRegionId"].ToString())));
                cmd.Parameters.AddWithValue("@CommandType", "UPDATE");
           

                SqlParameter sqlparam2 = new SqlParameter();
                sqlparam2.ParameterName = "@IsSuccess";
                sqlparam2.SqlDbType = SqlDbType.Int;
                sqlparam2.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlparam2);



                int i = cmd.ExecuteNonQuery();
              
                string chk = sqlparam2.Value.ToString();


            }


            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
            }
            return CID;
        }


        //-------

        public int CheckExists(string query)
        {

            int cnt = 0;
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                SqlCommand cmd = new SqlCommand(query, con);
              
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        cnt = 1;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return cnt;

        }

        public int CheckExistsmaster(string query)
        {
            
            int cnt = 0;
            try
            {
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                SqlCommand cmd = new SqlCommand(query, Mastercon);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        cnt = 1;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Mastercon.Close();
            }
            return cnt;

        }


        public IEnumerable<DataFieldEmployee> EmployeeList
        {
            get
            {
                List<DataFieldEmployee> Visitor = new List<DataFieldEmployee>();
                try
                {
                    if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }

                    int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
               



                    SqlCommand cmd = new SqlCommand("select Sr#No,Isnull(Name,'') as Name,Isnull([Email ID],'') as [Email ID],Isnull(DEPARTMENT,'') as DEPARTMENT,Isnull(DESIGNATION,'') as DESIGNATION,Isnull([KSS Department],'') as [KSS Department],Isnull([MOBILE NUMBER],'') as [MOBILE NUMBER],isnull([DATE OF BIRTH],'1900-01-01') as [DATE OF BIRTH],isnull([DATE OF JOINING],'1900-01-01') as [DATE OF JOINING],Isnull(Status,'') as Status,Isnull(EMPLOYEE_ID,'') as EMPLOYEE_ID,isnull(Investment_Declaration_Password,'') as Investment_Declaration_Password,Region, isnull(HOD,'') as HOD,isnull( Immediate_Supervisor,'') as Immediate_Supervisor  from Master_Requestor  where    status!='Cancel' and  status!='InActive' and  RegionId=" + RegionId + " order by  Sr#No desc", Mastercon);


                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {

                            DataFieldEmployee dataField = new DataFieldEmployee();
                            dataField.Email_ID = rdr["Email ID"].ToString();
                            dataField.Sr_No = Convert.ToInt64(rdr["Sr#No"].ToString());
                            dataField.DEPARTMENT = rdr["DEPARTMENT"].ToString();
                            dataField.DESIGNATION = rdr["DESIGNATION"].ToString();
                            dataField.KSS_Department = rdr["KSS Department"].ToString();
                            dataField.MOBILE_NUMBER = Convert.ToDouble(rdr["MOBILE NUMBER"].ToString());
                            dataField.NAME = rdr["NAME"].ToString();
                            dataField.DATE_OF_BIRTH = Convert.ToDateTime(rdr["DATE OF BIRTH"].ToString());
                            dataField.DATE_OF_JOINING = Convert.ToDateTime(rdr["DATE OF JOINING"].ToString());
                            dataField.Status = rdr["Status"].ToString();
                            dataField.EMPLOYEE_ID = rdr["EMPLOYEE_ID"].ToString();
                            dataField.Investment_Declaration_Password = rdr["Investment_Declaration_Password"].ToString();
                          
                            dataField.Immediate_Supervisor = rdr["Immediate_Supervisor"].ToString();
                            dataField.HOD = rdr["HOD"].ToString();

                            dataField.Region = rdr["Region"].ToString();
                            Visitor.Add(dataField);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendExcepToDB(ex);
                }
                finally
                {
                    if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
                }
                return Visitor;
            }
        }




        //-----encrypt--------
        public string Encrypt(string clearText)
        {
            //string encryptionKey = "MAKV2SPBNI99212";
            //byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            //using (Aes encryptor = Aes.Create())
            //{
            //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            //    encryptor.Key = pdb.GetBytes(32);
            //    encryptor.IV = pdb.GetBytes(16);
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            //        {
            //            cs.Write(clearBytes, 0, clearBytes.Length);
            //            cs.Close();
            //        }
            //        clearText = Convert.ToBase64String(ms.ToArray());
            //    }
            //}

            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            //string encryptionKey = "MAKV2SPBNI99212";
            //byte[] cipherBytes = Convert.FromBase64String(cipherText);
            //using (Aes encryptor = Aes.Create())
            //{
            //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            //    encryptor.Key = pdb.GetBytes(32);
            //    encryptor.IV = pdb.GetBytes(16);
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            //        {
            //            cs.Write(cipherBytes, 0, cipherBytes.Length);
            //            cs.Close();
            //        }
            //        cipherText = Encoding.Unicode.GetString(ms.ToArray());
            //    }
            //}

            return cipherText;
        }

        //------------------enc
        public int ChangePasswordbyEmail(string Email, string password)
        {

            int Check = 0;
            try
            {
                if (Mastercon.State == ConnectionState.Closed) { Mastercon.Open(); }
                string sql = "  Update Master_Requestor set TrumpPassword='" + password + "'  where [Email ID]='" + Email + "' ";
                SqlCommand cmd = new SqlCommand(sql, Mastercon);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                Check = 1;
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (Mastercon.State == ConnectionState.Open) { Mastercon.Close(); }
            }
            return Check;


        }



        public int UpdateCompany(int companyid, string company, string address, string phone, string status )
        {

            int CID = 0;
            try
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
               // string sql = "Insert into PreventDuplicate(Timestamp) values('" + str + "') ";
                string sql = "update tblCompany set CName= '" + company + "',Addres= '" + address + "',Phone= '" + phone + "',C_Status='" + status + "' where C_ID="+ companyid + "";
                //update tblCompany set CName
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                CID = 1;
            }
            catch (Exception ex)
            {
                SendExcepToDB(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }
            return CID;


        }


        public string SaveCompanyforAdmin(string company, string address, string phone,string status)
        {
            string CID = "";
            try
            {
               // int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                if (con.State == ConnectionState.Closed) { con.Open(); }
 

    SqlCommand cmd = new SqlCommand("sp_Company", con);
    cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", company.ToUpper());
                cmd.Parameters.AddWithValue("@Addres", address);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@gst", "");
                cmd.Parameters.AddWithValue("@city", "");
                cmd.Parameters.AddWithValue("@state", "");
                cmd.Parameters.AddWithValue("@pin", "");
                cmd.Parameters.AddWithValue("@RegionId", 0);
                SqlParameter sqlparam = new SqlParameter();
    sqlparam.ParameterName = "@ID";
                sqlparam.SqlDbType = SqlDbType.Int;
                sqlparam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlparam);
                int i = cmd.ExecuteNonQuery();
    CID = sqlparam.Value.ToString();
            }
            catch (Exception ex)
            {
    SendExcepToDB(ex);
}
            finally
            {
    if (con.State == ConnectionState.Open) { con.Close(); }
}
return CID;
        }


        //-------------sep27

        public string Encode(string sData)
        {
            try
            {
                byte[] encData_byte = new byte[sData.Length];

                encData_byte = System.Text.Encoding.UTF8.GetBytes(sData);

                string encodedData = Convert.ToBase64String(encData_byte);

                return encodedData;

            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public string Decode(string sData)
        {

            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();

            System.Text.Decoder utf8Decode = encoder.GetDecoder();

            byte[] todecode_byte = Convert.FromBase64String(sData);

            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);

            char[] decoded_char = new char[charCount];

            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);

            string result = new String(decoded_char);

            return result;

        }

        //-------------
        //***************** functionality added Update Inward Detail*************
        public int UpdateCourierDetail(FormCollection fc)
        {
            try
            {

                //string strtime = DateTime.Now.ToString("hh:mm tt");
                //string Vdate = fc["V_DateFrom"].ToString();
                //int RegionId = Convert.ToInt32(HttpContext.Current.Session["RegionId"]);
                int CouriarId = Convert.ToInt32(fc["txtCouriarId"].ToString());
                //-----------Feb22-----
                string dept = fc["Deptment"].ToString();
                int Check = 0;
                SqlCommand cmd = new SqlCommand("", con);
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "Proc_tblCouriar_Update";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CouriarId", CouriarId);
                cmd.Parameters.AddWithValue("@Remark", fc["txtremark"].ToString());
                cmd.Parameters.AddWithValue("@CouriarVendor", fc["ddlCourierCompany"].ToString());
                cmd.Parameters.AddWithValue("@DocketNo", fc["txtdocket"].ToString());
                //cmd.Parameters.AddWithValue("@ReceiveTime", strtime);
                cmd.Parameters.AddWithValue("@Documents", fc["txtdoc"].ToString());
                cmd.Parameters.AddWithValue("@City", fc["ddlCity"].ToString());
                //cmd.Parameters.AddWithValue("@C_Date", Convert.ToDateTime(Vdate));
                //cmd.Parameters.AddWithValue("@CurrentStatus", "Open");
                //cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@CouriarType", "Inward");
                //cmd.Parameters.AddWithValue("@Region", fc["txtRegionNew"].ToString());
                cmd.Parameters.AddWithValue("@Department", dept);
                cmd.Parameters.AddWithValue("@NameOfCompany", fc["host"].ToString());
                cmd.Parameters.AddWithValue("@EmployeeName", fc["host"].ToString());
                cmd.Parameters.AddWithValue("@EmployeeId", fc["Requestor"].ToString());
                cmd.Parameters.AddWithValue("@SenderName", fc["txtSendername"].ToString());
                cmd.Parameters.AddWithValue("@PacketType", fc["ddlPacketType"].ToString());
                //cmd.Parameters.AddWithValue("@RegionId", RegionId);
                cmd.Parameters.AddWithValue("@CommandType", "INWARDUPDATE");
                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int cnt = cmd.ExecuteNonQuery();
                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());
                return Check;
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }


        }
        public int UpdateDispatchDetail(FormCollection fc)
        {
            try
            {


                
                string Ishazardousitem = fc["operationType"].ToString();
                if (Ishazardousitem == "Routing")
                {
                    Ishazardousitem = "Yes";

                }
                else
                {
                    Ishazardousitem = "No";
                }

                string strtime = DateTime.Now.ToString("hh:mm tt");

                string Vdate = fc["V_DateFrom"].ToString();
                //objcCouriar.C_Date = Convert.ToDateTime(Vdate);
                int CouriarId = Convert.ToInt32(fc["txtCouriarId"].ToString());

                int Check = 0;
                string strUDId = "";
                SqlCommand cmd = new SqlCommand("", con);
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "Proc_tblCouriar_Update";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Remark", fc["txtremark"].ToString());
                
                cmd.Parameters.AddWithValue("@ReceiveTime", strtime);
                cmd.Parameters.AddWithValue("@Documents", fc["txtdoc"].ToString());
                cmd.Parameters.AddWithValue("@City", fc["txtcity"].ToString());
                //cmd.Parameters.AddWithValue("@C_Date", Convert.ToDateTime(Vdate));
                //cmd.Parameters.AddWithValue("@CurrentStatus", "Open");
                //cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                //cmd.Parameters.AddWithValue("@CouriarType", "Outward");
                //cmd.Parameters.AddWithValue("@Region", fc["txtRegion"].ToString());
                //cmd.Parameters.AddWithValue("@Department", fc["txtDepartment"].ToString());
                cmd.Parameters.AddWithValue("@NameOfCompany", fc["txtcompany"].ToString());
              
                //cmd.Parameters.AddWithValue("@EmployeeId", fc["txtEmpId"].ToString());
                cmd.Parameters.AddWithValue("@SenderName", fc["txtSendername"].ToString());
                cmd.Parameters.AddWithValue("@PacketType", fc["ddlPacketType"].ToString());


                //-------------
                cmd.Parameters.AddWithValue("@Person", fc["txtPerson"].ToString());
                cmd.Parameters.AddWithValue("@ContactNo", fc["txtContactNo"].ToString());
                cmd.Parameters.AddWithValue("@Address", fc["txtAddress"].ToString());
                cmd.Parameters.AddWithValue("@FromCity", fc["txtfromcity"].ToString());
                cmd.Parameters.AddWithValue("@SenderContactNo", fc["txtSenderContactno"].ToString());
                cmd.Parameters.AddWithValue("@HazardousItemYN", Ishazardousitem);
                cmd.Parameters.AddWithValue("@DeliveryDate", Convert.ToDateTime(fc["txtdeliverydate"].ToString()));

             


                cmd.Parameters.AddWithValue("@CommandType", "OUTWARDUPDATE");
                cmd.Parameters.AddWithValue("@CouriarId", CouriarId);
                //SqlParameter p1 = new SqlParameter("CouriarId", SqlDbType.Int);
                //cmd.Parameters.Add(p1);
                //p1.Direction = ParameterDirection.InputOutput;
                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int cnt = cmd.ExecuteNonQuery();

                //strUDId = cmd.Parameters["CouriarId"].Value.ToString();
                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());



                return Check;
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }


        }
        public int UpdatePodDetail(string podfilename, int CouriarId)
        {
            try
            {
                int Check = 0;
                SqlCommand cmd = new SqlCommand("", con);
                cmd.Parameters.Clear();
                cmd.Connection = con;
                cmd.CommandText = "Proc_tblCouriar_Update";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CommandType", "UploadPOD");
                cmd.Parameters.AddWithValue("@CouriarId", CouriarId);
                cmd.Parameters.AddWithValue("@Remark", podfilename);
                SqlParameter p2 = new SqlParameter("IsSuccess", SqlDbType.Int);
                cmd.Parameters.Add(p2);
                p2.Direction = ParameterDirection.InputOutput;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int cnt = cmd.ExecuteNonQuery();
                Check = Convert.ToInt32(cmd.Parameters["IsSuccess"].Value.ToString());

                return Check;
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
            }


        }
        //*********************

        //-----------------------


    }
}
