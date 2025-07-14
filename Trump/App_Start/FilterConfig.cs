using System;
using System.Data.Entity.Validation;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Trump.Models;

namespace Trump
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionHandlerAttribute());
        }
        //public class SkipImportantTaskAttribute : Attribute { }
        public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
        {
            public void OnException(ExceptionContext filterContext)
            {
                using (TrumpEntities ctx = new TrumpEntities())
                {
                    if (!filterContext.ExceptionHandled)
                    {
                        ExceptionLogger logger = new ExceptionLogger();
                        logger.ExceptionMessage = filterContext.Exception.Message;
                        logger.ExceptionStackTrace = filterContext.Exception.StackTrace;
                        logger.ControllerName = filterContext.RouteData.Values["controller"].ToString() + "/" + filterContext.RouteData.Values["Action"].ToString();
                        logger.LogTime = DateTime.Now;
                        logger.IPAddress = HttpContext.Current.Request.UserHostAddress;
                        try
                        {
                            ctx.ExceptionLoggers.Add(logger);
                            ctx.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                }
                            }
                        }
                        filterContext.ExceptionHandled = true;
                    }
                }
            }
        }
    }
}
