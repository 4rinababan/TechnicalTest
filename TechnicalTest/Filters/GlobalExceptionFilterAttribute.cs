using System;
using System.Web.Mvc;
using TechnicalTest.Helpers;
using TechnicalTest.Models;

namespace TechnicalTest.Filters
{
    /// <summary>
    /// Global exception handler untuk seluruh Controller/Action.
    /// Didaftarkan sebagai Global Filter di App_Start/FilterConfig.cs
    /// menggantikan HandleErrorAttribute bawaan.
    /// </summary>
    public class GlobalExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            var currentUser = SessionHelper.GetCurrentUser(filterContext.HttpContext);
            var controllerName = filterContext.RouteData.Values["controller"] as string;
            var actionName = filterContext.RouteData.Values["action"] as string;
            var requestUrl = filterContext.HttpContext.Request.Url != null
                ? filterContext.HttpContext.Request.Url.ToString()
                : null;

            DbLogger.LogException(
                filterContext.Exception,
                controllerName,
                actionName,
                currentUser != null ? currentUser.Username : null,
                requestUrl);

            var isAjaxRequest = filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjaxRequest)
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { success = false, message = "Terjadi kesalahan pada server." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                var model = new ErrorViewModel
                {
                    Message = "Terjadi kesalahan yang tidak terduga. Tim kami sudah mencatat kejadian ini.",
                    RequestId = Guid.NewGuid().ToString("N").Substring(0, 8)
                };

                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Error.cshtml",
                    ViewData = new ViewDataDictionary<ErrorViewModel>(model)
                };
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
