using System.Web.Mvc;
using TechnicalTest.Models;

namespace TechnicalTest.Controllers
{
    public class ErrorController : Controller
    {
        // Fallback untuk error yang tidak tertangkap MVC exception filter
        // (mis. error level IIS/routing), dipanggil lewat <customErrors> di Web.config.
        public ActionResult Index()
        {
            var model = new ErrorViewModel
            {
                Message = "Terjadi kesalahan yang tidak terduga."
            };
            return View("~/Views/Shared/Error.cshtml", model);
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}
