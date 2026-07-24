using System.Net;
using System.Web.Mvc;
using TechnicalTest.Filters;
using TechnicalTest.Helpers;
using TechnicalTest.Helpers.Account;
using TechnicalTest.Models;
using TechnicalTest.Repositories;
using TechnicalTest.Services;

namespace TechnicalTest.Controllers
{
    [AuthorizeUser]
    public class SupplierController : Controller
    {
        private readonly Interfaces.ISupplierService _supplierService;

        public SupplierController() : this(new Services.SupplierService())
        {
        }

        public SupplierController(Interfaces.ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        private UserModel CurrentUser
        {
            get { return SessionHelper.GetCurrentUser(HttpContext); }
        }

        // GET: /Supplier
        public ActionResult Index(SupplierQueryModel query)
        {
            if (query == null)
            {
                query = new SupplierQueryModel();
            }
            if (query.PageNumber <= 0) query.PageNumber = 1;
            if (query.PageSize <= 0) query.PageSize = 10;

            PagedResult<SupplierModel> result;

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                result = _supplierService.Search(
                    query.Keyword, query.PageNumber, query.PageSize,
                    CurrentUser.UserID, CurrentUser.Role);
            }
            else
            {
                result = _supplierService.GetList(query, CurrentUser.UserID, CurrentUser.Role);
            }

            var viewModel = new SupplierListViewModel
            {
                Query = query,
                Result = result
            };

            return View(viewModel);
        }

        // GET: /Supplier/Details/5
        public ActionResult Details(int id)
        {
            var supplier = _supplierService.GetById(id, CurrentUser.UserID, CurrentUser.Role);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: /Supplier/Create
        [AuthorizeUser(Roles = RoleConstants.Admin)]
        public ActionResult Create()
        {
            return View(new SupplierModel { IsActive = true });
        }

        // POST: /Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Roles = RoleConstants.Admin)]
        public ActionResult Create(SupplierModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _supplierService.Create(model, CurrentUser.UserID);
            if (!result.Success)
            {
                if (result.Message == "DuplicateCode")
                {
                    ModelState.AddModelError("SupplierCode", "Kode supplier sudah digunakan, gunakan kode lain.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Gagal membuat supplier: " + result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = "Supplier berhasil ditambahkan.";
            return RedirectToAction("Details", new { id = result.Data });
        }

        // GET: /Supplier/Edit/5
        public ActionResult Edit(int id)
        {
            var supplier = _supplierService.GetById(id, CurrentUser.UserID, CurrentUser.Role);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: /Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SupplierModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _supplierService.Update(
                model, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.Role);

            if (!result.Success)
            {
                if (result.Message == "DuplicateCode")
                {
                    ModelState.AddModelError("SupplierCode", "Kode supplier sudah digunakan, gunakan kode lain.");
                    return View(model);
                }

                return new HttpStatusCodeResult(
                    HttpStatusCode.Forbidden, "Anda tidak memiliki akses untuk mengubah data ini.");
            }

            TempData["SuccessMessage"] = "Supplier berhasil diperbarui.";
            return RedirectToAction("Details", new { id = model.SupplierID });
        }

        // GET: /Supplier/Delete/5
        // Hanya Admin yang boleh menghapus (soft delete)
        [AuthorizeUser(Roles = RoleConstants.Admin)]
        public ActionResult Delete(int id)
        {
            var supplier = _supplierService.GetById(id, CurrentUser.UserID, CurrentUser.Role);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: /Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Roles = RoleConstants.Admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            var result = _supplierService.Delete(id, CurrentUser.UserID, CurrentUser.Role);
            if (!result.Success)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Anda tidak memiliki akses untuk menghapus data ini.");
            }

            TempData["SuccessMessage"] = "Supplier berhasil dinonaktifkan.";
            return RedirectToAction("Index");
        }
    }
}
