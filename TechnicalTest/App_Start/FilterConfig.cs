using System.Web;
using System.Web.Mvc;
using TechnicalTest.Filters;

namespace TechnicalTest
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalExceptionFilterAttribute());
        }
    }
}
