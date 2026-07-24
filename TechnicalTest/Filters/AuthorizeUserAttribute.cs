using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TechnicalTest.Helpers;
using TechnicalTest.Helpers.Account;

namespace TechnicalTest.Filters
{
    public class AuthorizeUserAttribute : FilterAttribute, IAuthorizationFilter
    {
        public string Roles { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var currentUser = SessionHelper.GetCurrentUser(filterContext.HttpContext);

            if (currentUser == null)
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                var returnUrl = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectResult(
                    urlHelper.Action("Login", "Account", new { returnUrl = returnUrl }));
                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                var allowedRoles = Roles.Split(',').Select(r => r.Trim()).ToArray();
                if (!allowedRoles.Contains(currentUser.Role, StringComparer.OrdinalIgnoreCase))
                {
                    filterContext.Result = new HttpStatusCodeResult(
                        HttpStatusCode.Forbidden, "Anda tidak memiliki akses ke halaman ini.");
                }
            }
        }
    }
}
