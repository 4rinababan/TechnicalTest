using System.Net;
using System.Web.Mvc;
using TechnicalTest.Filters;
using TechnicalTest.Helpers;
using TechnicalTest.Models;
using TechnicalTest.Repositories;

namespace TechnicalTest.Controllers
{
    [AuthorizeUser] // wajib login untuk semua action di controller ini
    public class SupplierController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierController() : this(new SupplierRepository())
        {
        }

        public SupplierController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        private UserModel CurrentUser
        {
            get { return SessionHelper.GetCurrentUser(HttpContext); }
        }

        // GET: /Supplier
        // RBAC: Admin lihat semua, Supplier hanya lihat data miliknya (difilter di dalam SP)
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
                result = _supplierRepository.Search(
                    query.Keyword, query.PageNumber, query.PageSize,
                    CurrentUser.UserID, CurrentUser.Role);
            }
            else
            {
                result = _supplierRepository.GetList(query, CurrentUser.UserID, CurrentUser.Role);
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
            var supplier = _supplierRepository.GetById(id, CurrentUser.UserID, CurrentUser.Role);
            if (supplier == null)
            {
                // null berarti data tidak ada, ATAU ada tapi bukan milik user ini (RBAC di SP)
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: /Supplier/Create
        // Hanya Admin yang boleh membuat supplier baru (asumsi bisnis, lihat README/asumsi)
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
            if (_supplierRepository.IsDuplicateCode(model.SupplierCode, null))
            {
                ModelState.AddModelError("SupplierCode", "Kode supplier sudah digunakan, gunakan kode lain.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newSupplierId = _supplierRepository.Insert(model, CurrentUser.UserID);

            TempData["SuccessMessage"] = "Supplier berhasil ditambahkan.";
            return RedirectToAction("Details", new { id = newSupplierId });
        }

        // GET: /Supplier/Edit/5
        // Admin bisa edit semua, Supplier hanya bisa edit miliknya (dicek di SP lewat GetById & Update)
        public ActionResult Edit(int id)
        {
            var supplier = _supplierRepository.GetById(id, CurrentUser.UserID, CurrentUser.Role);
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
            if (_supplierRepository.IsDuplicateCode(model.SupplierCode, model.SupplierID))
            {
                ModelState.AddModelError("SupplierCode", "Kode supplier sudah digunakan, gunakan kode lain.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = _supplierRepository.Update(
                model, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.Role);

            if (!success)
            {
                // Update gagal karena RBAC (bukan Admin & bukan pemilik data) atau ID tidak ditemukan
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
            var supplier = _supplierRepository.GetById(id, CurrentUser.UserID, CurrentUser.Role);
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
            _supplierRepository.Delete(id, CurrentUser.UserID, CurrentUser.Role);
            TempData["SuccessMessage"] = "Supplier berhasil dinonaktifkan.";
            return RedirectToAction("Index");
        }
    }
}
