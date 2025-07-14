using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace TrumpService
{
    public class MailClass
    {
        TrumpEntities db = new TrumpEntities();
        MasterDatabaseEntities login = new MasterDatabaseEntities();
        public void AppointmentMail()
        {
            try
            {
                var data = db.tblAppoinments.Where(m => m.Flag == "False" && (m.ComanyID != null || m.VisitorID != null)).FirstOrDefault();
                if (data != null)
                {
                    var requestor = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.Requestor).FirstOrDefault();

                    var visitor = db.tblVisitors.Where(m => m.V_ID == data.VisitorID).FirstOrDefault();
                    var company = db.tblCompanies.Where(m => m.C_ID == data.ComanyID).FirstOrDefault();

                    var rejection = db.tblApprovals.Where(m => m.AppType == "OUTWORD" && m.AppStatus == "Active" && m.Region == data.V_Location).FirstOrDefault();

                    string mastercompany = login.MasterCompanies.Where(m => m.MC_ID == requestor.CompanyID).FirstOrDefault().CompanyName;

                    if (data.V_Status == "Pre")
                    {
                        //create_ePass(data.GPID);

                        var msgtemp = db.Msgtemplates.Where(m => m.Action == "Appointment" && m.Region == data.V_Location).FirstOrDefault();

                        string temp = "Dear " + visitor.V_Name + "<br/>" + msgtemp.Template.Replace("@p1", mastercompany).Replace("@p2", data.V_DateFrom.Value.ToString("dd/MMM/yyyy")).Replace("@p3", data.A_Time + "<br/><br/>");

                        StringBuilder sb = new StringBuilder();
                        sb.Append(temp);
                        sb.Append("<br/><a href='http://122.185.30.43:8018/Home/SG/" + data.GPID + "'>Safety Guidelines & e-Pass</a>");

                        StringBuilder rejectBody = new StringBuilder();
                        rejectBody.Append("Dear Sir,<br/><br/>");
                        rejectBody.Append("New appointment has been scheduled as per below details..");
                        rejectBody.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                        rejectBody.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                        rejectBody.Append("<td style = 'vertical-align : middle;text-align:center;'>VisitId</td>");
                        rejectBody.Append("<td style = 'vertical-align : middle;text-align:center;'>Company</td>");
                        rejectBody.Append("<td style = 'vertical-align : middle;text-align:center;'>Visitor Name</td>");
                        rejectBody.Append("<td style = 'vertical-align : middle;text-align:center;'>Appointment Type</td>");
                        rejectBody.Append("<td style = 'vertical-align : middle;text-align:center;'>Date</td>");
                        rejectBody.Append("<td style = 'vertical-align : middle;text-align:center;'>Time</td >");
                        rejectBody.Append("</tr>");
                        rejectBody.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.GPID + "</td><td> " + company.CName + "</td><td> " + visitor.V_Name + "</td><td> " + data.Visitor_Type + "</td><td> " + data.V_DateFrom.Value.ToString("dd/MMM/yyyy") + "</td><td> " + data.A_Time + "</td></tr>");
                        rejectBody.Append("</table><br/><br/>");
                        rejectBody.Append("If you want to reject the appointment ,please click the below link <br/><br/>");
                        rejectBody.Append("<a href='http://172.20.0.3:8018/Home/Reject_App/" + data.A_ID + "'>Reject</a>");
                        if (IsValidEmailAddress(visitor.V_Email))
                        {
                            //Send mail to visitor
                            try
                            {
                                SendMail(sb.ToString(), visitor.V_Email, null, "Appointment at " + mastercompany);
                                data.Flag = "True";
                                db.Entry(data).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch
                            {
                                data.Flag = "False";
                                db.Entry(data).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        //Send mail for rejection
                        SendMail(rejectBody.ToString(), rejection.Email, requestor.Email_ID, "Appointment at " + mastercompany);
                    }
                    else if (data.V_Status == "Arrived" && data.A_Type == "Pre")
                    {
                        var securityTemp = db.Msgtemplates.Where(m => m.Region == data.V_Location && m.Action == "Arrived").FirstOrDefault();
                        var aomteam = db.tblApprovals.Where(m => m.Region == data.V_Location && m.AppType == "VISITOR ARRIVED").FirstOrDefault();
                        string security = "Dear " + requestor.NAME + "<br/>" + securityTemp.Template;

                        if (IsValidEmailAddress(requestor.Email_ID))
                        {
                            StringBuilder securityMail = new StringBuilder();
                            securityMail.Append(security);
                            securityMail.Append("Please find below details.<br/><br/>");
                            securityMail.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                            securityMail.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                            securityMail.Append("<td style = 'vertical-align : middle;text-align:center;'>VisitId</td>");
                            securityMail.Append("<td style = 'vertical-align : middle;text-align:center;'>Visitor Name</td>");
                            securityMail.Append("<td style = 'vertical-align : middle;text-align:center;'>Appointment Type</td>");
                            securityMail.Append("<td style = 'vertical-align : middle;text-align:center;'>Date</td>");
                            securityMail.Append("<td style = 'vertical-align : middle;text-align:center;'>Time</td >");
                            securityMail.Append("</tr>");
                            securityMail.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.GPID + "</td><td> " + visitor.V_Name + "</td><td> " + data.Visitor_Type + "</td><td> " + data.V_DateFrom.Value.ToShortDateString() + "</td><td> " + data.A_Time + "</td></tr>");
                            securityMail.Append("</table>");

                            //SendMail(securityMail.ToString(), requestor.Email_ID, null, "Vistor Arrived");
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        StringBuilder aom_Mail = new StringBuilder();
                        aom_Mail.Append("Dear All<br/><br/>");
                        aom_Mail.Append("Visitor has arrived at security gate with respect to the appointment scheduled today.");
                        aom_Mail.Append("Please find below details.<br/><br/>");
                        aom_Mail.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                        aom_Mail.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>VisitId</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Requestor</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Visitor Name</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Appointment Type</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Date</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Time</td >");
                        aom_Mail.Append("</tr>");
                        aom_Mail.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.GPID + "</td><td>" + requestor.NAME + "</td><td> " + visitor.V_Name + "</td><td> " + data.Visitor_Type + "</td><td> " + data.V_DateFrom.Value.ToShortDateString() + "</td><td> " + data.A_Time + "</td></tr>");
                        aom_Mail.Append("</table>");

                        SendMail(aom_Mail.ToString(), aomteam.Email.ToString(), null, "Vistor Arrived");
                    }
                    else if (data.V_Status == "Arrived" && data.A_Type == "Request")
                    {
                        if (IsValidEmailAddress(visitor.V_Email))
                        {
                            StringBuilder securityAppointment = new StringBuilder();
                            securityAppointment.Append("Dear Sir/Ma'am,");
                            securityAppointment.Append("<br/>Welcome to one of the leading PPE manufacturing unit " + mastercompany + " Please follow our guidelines board and click the below link to ACCEPT Safety Guidelines and generate ePass.<br/>");
                            securityAppointment.Append("<a href='http://122.185.30.43:8018/Home/SG/" + data.GPID + "'>Generate ePass</a>");

                            SendMail(securityAppointment.ToString(), visitor.V_Email, null, "Visit at " + mastercompany);
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var aomteam = db.tblApprovals.Where(m => m.Region == data.V_Location && m.AppType == "VISITOR ARRIVED").FirstOrDefault();

                        StringBuilder aom_Mail = new StringBuilder();
                        aom_Mail.Append("Dear All<br/><br/>");
                        aom_Mail.Append("Visitor has arrived at security gate with respect to the appointment scheduled today.");
                        aom_Mail.Append("Please find below details.<br/><br/>");
                        aom_Mail.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                        aom_Mail.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>VisitId</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Requestor</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Visitor Name</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Appointment Type</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Date</td>");
                        aom_Mail.Append("<td style = 'vertical-align : middle;text-align:center;'>Time</td >");
                        aom_Mail.Append("</tr>");
                        aom_Mail.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.GPID + "</td><td>" + requestor.NAME + "</td><td> " + visitor.V_Name + "</td><td> " + data.Visitor_Type + "</td><td> " + data.V_DateFrom.Value.ToShortDateString() + "</td><td> " + data.A_Time + "</td></tr>");
                        aom_Mail.Append("</table>");
                        SendMail(aom_Mail.ToString(), aomteam.Email, null, "Vistor Arrived");
                    }
                    else if (data.V_Status == "Cancel")
                    {
                        string securityData = db.tblApprovals.Where(m => m.AppType == "SECURITY" && m.Region == data.V_Location).FirstOrDefault().Email;

                        var msg = db.Msgtemplates.Where(m => m.Action == "Cancel" & m.Region == data.V_Location).FirstOrDefault();
                        string temp = "Dear Security Team" + "<br/>" + msg.Template.Replace("@p1", requestor.NAME);

                        StringBuilder sb = new StringBuilder();
                        sb.Append(temp);
                        sb.Append("<br/><br/>");
                        sb.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                        sb.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>VisitId</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Company</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Visitor Name</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Appointment Type</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Date</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Time</td >");
                        sb.Append("</tr>");
                        sb.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.GPID + "</td><td> " + company.CName + "</td><td> " + visitor.V_Name + "</td><td> " + data.Visitor_Type + "</td><td> " + data.V_DateFrom.Value.ToString("dd/MMM/yyyy") + "</td><td> " + data.A_Time + "</td></tr>");
                        sb.Append("</table><br/><br/>");

                        var msgtemp = db.Msgtemplates.Where(m => m.Action == "CancelVisitor" && m.Region == data.V_Location).FirstOrDefault();

                        string msgBody = "Dear " + visitor.V_Name + "<br/>" + msgtemp.Template.Replace("@p1 ", mastercompany + " ").Replace("@p2", data.V_DateFrom.Value.ToString("dd/MMM/yyyy")).Replace("@p3", data.A_Time);

                        try
                        {
                            SendMail(sb.ToString(), securityData, requestor.Email_ID, "Appointment Cancel");
                            if (IsValidEmailAddress(visitor.V_Email))
                            {
                                SendMail(msgBody, visitor.V_Email, null, "Appointment Cancel");
                            }
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch
                        {
                            data.Flag = "False";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (data.V_Status == "Reject")
                    {
                        var rejectionAuthority = db.tblApprovals.Where(m => m.AppType == "OUTWORD" && m.Region == data.V_Location).FirstOrDefault();

                        string securityData = db.tblApprovals.Where(m => m.AppType == "SECURITY" && m.Region == data.V_Location).FirstOrDefault().Email;

                        var msg = db.Msgtemplates.Where(m => m.Action == "Cancel" && m.Region == data.V_Location).FirstOrDefault();
                        string temp = "Dear Security Team" + "<br/>" + msg.Template.Replace("@p1", rejectionAuthority.Name_);

                        var msgtemp = db.Msgtemplates.Where(m => m.Action == "CancelVisitor" && m.Region == data.V_Location).FirstOrDefault();

                        string msgBody = "Dear " + visitor.V_Name + "<br/>" + msgtemp.Template.Replace("@p1 ", mastercompany + " ").Replace("@p2", data.V_DateFrom.Value.ToString("dd/MMM/yyyy")).Replace("@p3", data.A_Time);

                        StringBuilder sb = new StringBuilder();
                        sb.Append(temp);
                        sb.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                        sb.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>VisitId</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Company</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Visitor Name</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Appointment Type</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Date</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Time</td >");
                        sb.Append("</tr>");
                        sb.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.GPID + "</td><td> " + company.CName + "</td><td> " + visitor.V_Name + "</td><td> " + data.Visitor_Type + "</td><td> " + data.V_DateFrom.Value.ToString("dd/MMM/yyyy") + "</td><td> " + data.A_Time + "</td></tr>");
                        sb.Append("</table><br/>");

                        try
                        {
                            SendMail(sb.ToString(), securityData, requestor.Email_ID, "Appointment Rejected");
                            if (IsValidEmailAddress(visitor.V_Email))
                            {
                                SendMail(msgBody, visitor.V_Email, null, "Appointment Cancel");
                            }
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch
                        {
                            data.Flag = "False";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (data.V_Status == "OUT")
                    {
                        data.Flag = "True";
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger exception = new ExceptionLogger();
                exception.ExceptionMessage = ex.Message;
                exception.ExceptionStackTrace = ex.StackTrace;
                exception.LogTime = DateTime.Now;
                exception.ControllerName = "Service Appointment";
                exception.IPAddress = "";
                db.ExceptionLoggers.Add(exception);
                db.SaveChanges();
            }
        }

        public void VisitorAcceptance()
        {
            try
            {
                var data = db.VisitorAcceptances.Where(m => m.Flag == "False").FirstOrDefault();
                if (data != null)
                {
                    var appintmentdata = db.tblAppoinments.Where(m => m.GPID == data.GatePassID).FirstOrDefault();

                    var req = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == appintmentdata.Requestor).FirstOrDefault();

                    var visitor = db.tblVisitors.Where(m => m.V_ID == data.VisitorID).FirstOrDefault();

                    string mastercompany = login.MasterCompanies.Where(m => m.MC_ID == req.CompanyID).FirstOrDefault().CompanyName;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("Dear " + visitor.V_Name);
                    sb.Append("<br/><br/>Please show the attatched e-Pass on security gate at the time of entry in " + mastercompany);

                    if (IsValidEmailAddress(visitor.V_Email))
                    {
                        try
                        {
                            SendMailAtt(sb.ToString(), visitor.V_Email, "e-Pass", appintmentdata.GPID + ".jpg");
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch
                        {
                            data.Flag = "False";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        data.Flag = "True";
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionLogger exception = new ExceptionLogger();
                exception.ExceptionMessage = ex.Message;
                exception.ExceptionStackTrace = ex.StackTrace;
                exception.LogTime = DateTime.Now;
                exception.ControllerName = "Service Visitor Acceptance";
                exception.IPAddress = "";
                db.ExceptionLoggers.Add(exception);
                db.SaveChanges();
            }
        }

        public void OutWord()
        {
            var data = db.tblIN_Out.Where(m => m.Flag == "False").FirstOrDefault();

            try
            {
                if (data != null)
                {
                    var requestor = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.Requestor).FirstOrDefault();
                    var HOD = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.ApprovingHOD).FirstOrDefault();
                    var reqDept = login.Departments.Where(m => m.D_ID == requestor.DepartmentID).FirstOrDefault().DepartmentName;

                    if (data.Delay == "True" && data.Flag == "False")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Dear " + HOD.NAME + ",<br/><br/>");
                        sb.Append("Below mentioned " + data.M_Type + " was delayed to reach the destination<br/>");
                        sb.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                        sb.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Challan Number</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Department</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Requestor</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Material Type</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Source</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Destination</td >");
                        sb.Append("</tr>");
                        sb.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.ChallanNmbr + "</td><td> " + reqDept + "</td><td> " + requestor.NAME + "</td><td> " + data.M_Type + "</td><td> " + data.OutFrom + "</td><td> " + data.M_Location + "</td></tr>");
                        sb.Append("</table><br/><br/>");

                        try
                        {
                            SendMail(sb.ToString(), HOD.Email_ID, requestor.Email_ID, "Material Delay");
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch
                        {
                            data.Flag = "False";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (data.Flag == "False" && data.Delay == "Verify")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Dear " + HOD.NAME + ",<br/><br/>");
                        sb.Append("Challan number " + data.ChallanNmbr + " which was delayed to reach the destination for the below reason<br/>");
                        sb.Append("Reason: ");
                        sb.Append(data.DelayReason);

                        try
                        {
                            SendMail(sb.ToString(), HOD.Email_ID, requestor.Email_ID, "Material Delay Reason");
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch
                        {
                            data.Flag = "False";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (data.Flag == "False" && data.M_Status == "Short Closed")
                    {
                        var SClosed = db.Item_Material.Where(m => m.M_Status == "Short Closed" && m.ID == data.M_ID).ToList();

                        StringBuilder sb = new StringBuilder();
                        sb.Append("Dear " + HOD.NAME + ",<br/><br/>");
                        sb.Append("Below Item is short closed by " + requestor.NAME + " against challan number " + data.ChallanNmbr + "<br/><br/>");
                        sb.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                        sb.Append("<tr style='background-color:Aqua' class='text - center'><td colspan='6' style='font-size:16px;font-weight:bold;color: black;text-align:center;'>Item Details</td></tr>");
                        sb.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Item</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Description</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>UOM</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Quantity</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Received Qty</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Reason</td></tr>");

                        foreach (var id in SClosed)
                        {
                            sb.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + id.Item + "</td><td> " + id.Descriptn + "</td><td> " + id.UOM + "</td><td> " + id.Qty + "</td><td> " + id.RecQty + "</td><td> " + id.Reason + "</td></tr>");
                        }
                        sb.Append("</table>");
                        try
                        {
                            SendMail(sb.ToString(), HOD.Email_ID, requestor.Email_ID, "Item Short Closed");
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch
                        {
                            data.Flag = "False";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger exception = new ExceptionLogger();
                exception.ExceptionMessage = ex.Message;
                exception.ExceptionStackTrace = ex.StackTrace;
                exception.LogTime = DateTime.Now;
                exception.ControllerName = "OutWard Method";
                exception.IPAddress = "";
                db.ExceptionLoggers.Add(exception);
                db.SaveChanges();
            }
        }

        public void SuppliearAdd()
        {
            var data = db.tblIN_Out.Where(m => m.Flag == "False1" && m.M_Status != "Cancel").FirstOrDefault();
            try
            {
                if (data != null)
                {
                    var requestor = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.Requestor).FirstOrDefault();
                    var HOD = login.Master_Requestor.Where(m => m.EMPLOYEE_ID == data.ApprovingHOD).FirstOrDefault();
                    var reqDept = login.Departments.Where(m => m.D_ID == requestor.DepartmentID).FirstOrDefault().DepartmentName;

                    if (data.Flag == "False1" && data.HODTime == null && data.SupplierID == null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Dear " + HOD.NAME + ",<br/><br/>");
                        sb.Append("Below mentioned " + data.M_Type + " was raised <br/>");
                        sb.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                        sb.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Challan Number</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Department</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Requestor</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Material Type</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Source</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.ChallanNmbr + "</td><td> " + reqDept + "</td><td> " + requestor.NAME + "</td><td> " + data.M_Type + "</td><td> " + data.OutFrom + "</td></tr>");
                        sb.Append("</table><br/><br/>");
                        sb.Append("If you want to reject the " + data.M_Type + " ,please click the below link <br/><br/>");
                        sb.Append("<a href='http://172.20.0.3:8018/Material/Reject/" + data.M_ID + "'>Reject</a>");
                        try
                        {
                            if (data.Flag == "False1" && data.M_Status == "Partial" && data.SupplierID == null)
                            {
                                StringBuilder sbPurchase = new StringBuilder();
                                sbPurchase.Append("Dear Team,<br/><br/>");
                                sbPurchase.Append("Below mentioned " + data.M_Type + " was raised <br/>");
                                sbPurchase.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                                sbPurchase.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                                sbPurchase.Append("<td style = 'vertical-align : middle;text-align:center;'>Challan Number</td>");
                                sbPurchase.Append("<td style = 'vertical-align : middle;text-align:center;'>Department</td>");
                                sbPurchase.Append("<td style = 'vertical-align : middle;text-align:center;'>Requestor</td>");
                                sbPurchase.Append("<td style = 'vertical-align : middle;text-align:center;'>Material Type</td>");
                                sbPurchase.Append("<td style = 'vertical-align : middle;text-align:center;'>Source</td>");
                                sbPurchase.Append("<td style = 'vertical-align : middle;text-align:center;'>MRS/PRS</td>");
                                sbPurchase.Append("</tr>");
                                sbPurchase.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.ChallanNmbr + "</td><td> " + reqDept + "</td><td> " + requestor.NAME + "</td><td> " + data.M_Type + "</td><td> " + data.OutFrom + "</td><td> " + data.MRS + "</td></tr>");
                                sbPurchase.Append("</table><br/><br/>");
                                SendMail(sbPurchase.ToString(), "purchase.pni@karam.in", null, "Outward raised");
                            }
                            SendMail(sb.ToString(), HOD.Email_ID, null, "Outward raised");
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch
                        {
                            data.Flag = "False1";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (data.Flag == "False1" && data.M_Status == "Open" && data.S_Type == "THIRD")
                    {
                        StringBuilder sb = new StringBuilder();
                        var supp = db.VendorDatas.Where(m => m.VNID == data.SupplierID).FirstOrDefault();

                        sb.Append("Dear " + requestor.NAME + ",<br/><br/>");
                        sb.Append("Supplier name is added by Purchase department of Challen Number " + data.ChallanNmbr + ".<br/>");
                        sb.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                        sb.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Supplier Name</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Supplier Address</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Material Type</td>");
                        sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Source</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + supp.PartyName + "</td><td> " + supp.PartyAddress + "</td><td> " + data.M_Type + "</td><td> " + data.OutFrom + "</td></tr>");
                        sb.Append("</table>");
                        try
                        {
                            if (IsValidEmailAddress(requestor.Email_ID))
                            {
                                SendMail(sb.ToString(), requestor.Email_ID, null, "Supplier added of Challen Number " + data.ChallanNmbr);
                            }
                            data.Flag = "True";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch
                        {
                            data.Flag = "False1";
                            db.Entry(data).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger exception = new ExceptionLogger();
                exception.ExceptionMessage = ex.Message;
                exception.ExceptionStackTrace = ex.StackTrace;
                exception.LogTime = DateTime.Now;
                exception.ControllerName = "Method Suppliear";
                exception.IPAddress = "";
                db.ExceptionLoggers.Add(exception);
                db.SaveChanges();
            }
        }

        public void PO()
        {
            try
            {
                var data = db.tblPOes.Where(m => m.Flag == "False").FirstOrDefault();

                if (data != null)
                {
                    var poreq = db.tblApprovals.Where(m => m.AppType == "PO" && m.Region == data.Region).FirstOrDefault();

                    var msgtemp = db.Msgtemplates.Where(m => m.Action == "PO" && m.Region == data.Region).FirstOrDefault();
                    string temp = "Dear Sir/Ma'am " + "<br/>" + msgtemp.Template;

                    StringBuilder sb = new StringBuilder();
                    sb.Append(temp);
                    sb.Append("<br/><br/>");
                    sb.Append("<table border=" + 1 + " cellpadding=" + 2 + " cellspacing=" + 1 + " width = " + 800 + ">");
                    sb.Append("<tr style='background-color:gray' class='text - center'><td colspan='7' style='font-size:16px;font-weight:bold;color: black;text-align:center;'>PO Detail</td></tr>");
                    sb.Append("<tr class='text - center' style='font-size: 13px;font-weight: bold;color: black;'>");
                    sb.Append("<td style = 'vertical-align : middle;text-align:center;'>PO Number</td>");
                    sb.Append("<td style = 'vertical-align : middle;text-lign:center;'>Vendor Name</td>");
                    sb.Append("<td style='vertical-align : middle;text-align:left;width: 10%;'>Item Code</td>");
                    sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Description</td>");
                    sb.Append("<td style = 'vertical-align : middle;text-align:center;'>UOM</td >");
                    sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Qty</td>");
                    sb.Append("<td style = 'vertical-align : middle;text-align:center;'>Rec Qty</td>");
                    sb.Append("</tr>");
                    var itemList = db.tblPOItems.Where(m => m.PONmbr == data.ID).ToList();

                    foreach (var id in itemList)
                    {
                        sb.Append("<tr><td  style='vertical - align : middle; text - align:center;font-weight:bold;'>" + data.PONum + "</td><td> " + data.SupplierName + "</td><td> " + id.Item + "</td><td> " + id.Descriptn + "</td><td> " + id.UOM + "</td><td> " + id.Qty + "</td><td> " + id.RecQty + "</td></tr>");
                    }
                    sb.Append("</table>");

                    try
                    {
                        SendMail(sb.ToString(), poreq.Email, null, "eXN");
                        data.Flag = "True";
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch
                    {
                        data.Flag = "False";
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger exception = new ExceptionLogger();
                exception.ExceptionMessage = ex.Message;
                exception.ExceptionStackTrace = ex.StackTrace;
                exception.LogTime = DateTime.Now;
                exception.ControllerName = "Service PO";
                exception.IPAddress = "";
                db.ExceptionLoggers.Add(exception);
                db.SaveChanges();
            }
        }

        public void SendMail(string textbox, string toEmail, string cc, string sub)
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
            if (cc != null)
            {
                if (IsValidEmailAddress(cc))
                {
                    mMailMessage.CC.Add(new MailAddress(cc));
                }
            }
            mMailMessage.Body = textbox;
            mMailMessage.IsBodyHtml = true;
            mMailMessage.Subject = sub;
            mMailMessage.Priority = MailPriority.Normal;

            //create the SmtpClient instance
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.office365.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            //smtp.Credentials = new NetworkCredential("karamsupport@karam.in", "Passw0rd");
            smtp.Credentials = new System.Net.NetworkCredential("karamsupport@karam.in", "Angel@123", "Karam.in");
            smtp.EnableSsl = true;

            if (IsValidEmailAddress(toEmail))
            {
                smtp.Send(mMailMessage);
            }
        }

        public void SendMailAtt(string textbox, string toEmail, string sub, string imgPath)
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
            string attachmentFilename = @"C:\inetpub\wwwroot\Trump\Content\images\StringImage\" + imgPath;
            Attachment attachment = new Attachment(attachmentFilename, MediaTypeNames.Application.Octet);

            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.FileName = Path.GetFileName(attachmentFilename);
            disposition.Size = new FileInfo(attachmentFilename).Length;
            disposition.DispositionType = DispositionTypeNames.Attachment;
            mMailMessage.Attachments.Add(attachment);

            mMailMessage.Body = textbox;
            mMailMessage.IsBodyHtml = true;
            mMailMessage.Subject = sub;
            mMailMessage.Priority = MailPriority.Normal;

            //create the SmtpClient instance
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.office365.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            //smtp.Credentials = new NetworkCredential("karamsupport@karam.in", "Passw0rd");
            smtp.Credentials = new System.Net.NetworkCredential("karamsupport@karam.in", "Angel@123", "Karam.in");
            smtp.EnableSsl = true;

            smtp.Send(mMailMessage);
        }

        public bool IsValidEmailAddress(string emailAddress)
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
                return true;
            else
                return false;
        }

        public string create_ePass(string ID)
        {
            string mthd = "";
            try
            {
                HttpWebRequest request = WebRequest.Create("http://172.20.0.3:8018/Home/Create_ePass/" + ID) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionLogger exception = new ExceptionLogger();
                exception.ExceptionMessage = ex.Message;
                exception.ExceptionStackTrace = ex.StackTrace;
                exception.LogTime = DateTime.Now;
                exception.ControllerName = "e-Pass";
                db.ExceptionLoggers.Add(exception);
                db.SaveChanges();
            }
            return mthd;
        }
    }
}
