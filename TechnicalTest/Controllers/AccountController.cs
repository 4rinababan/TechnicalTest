using System;
using System.Web.Mvc;
using TechnicalTest.Helpers;
using TechnicalTest.Models;
using TechnicalTest.Interfaces;
using TechnicalTest.Repositories;
using TechnicalTest.Helpers.Account;

namespace TechnicalTest.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AccountController() : this(new UserRepository())
        {
        }

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (SessionHelper.GetCurrentUser(HttpContext) != null)
            {
                return RedirectToAction("Index", "Supplier");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var credential = _userRepository.GetCredentialByUsername(model.Username);

            if (credential == null)
            {
                ModelState.AddModelError(string.Empty, "Username atau password salah.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var computedHash = PasswordHelper.ComputeHash(model.Password, credential.PasswordSalt);

            if (!string.Equals(computedHash, credential.PasswordHash, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Username atau password salah.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = new UserModel
            {
                UserID = credential.UserID,
                Username = credential.Username,
                FullName = credential.FullName,
                Role = credential.Role
            };

            SessionHelper.SetCurrentUser(Session, user);

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Supplier");
        }

        public ActionResult Logout()
        {
            SessionHelper.ClearSession(Session);
            return RedirectToAction("Login");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
