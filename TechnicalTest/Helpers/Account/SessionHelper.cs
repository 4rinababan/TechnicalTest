using System.Web;
using TechnicalTest.Models;

namespace TechnicalTest.Helpers.Account
{
    public static class SessionHelper
    {
        private const string CurrentUserKey = "CurrentUser";

        public static void SetCurrentUser(HttpSessionStateBase session, UserModel user)
        {
            session[CurrentUserKey] = user;
        }

        public static UserModel GetCurrentUser(HttpContextBase httpContext)
        {
            if (httpContext == null || httpContext.Session == null)
            {
                return null;
            }
            return httpContext.Session[CurrentUserKey] as UserModel;
        }

        public static void ClearSession(HttpSessionStateBase session)
        {
            session.Clear();
            session.Abandon();
        }
    }
}
