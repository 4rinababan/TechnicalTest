using TechnicalTest.Models;
using TechnicalTest.Services;

namespace TechnicalTest.Interfaces
{
    public interface ISupplierService
    {
        PagedResult<SupplierModel> GetList(SupplierQueryModel query, int userId, string role);
        PagedResult<SupplierModel> Search(string keyword, int pageNumber, int pageSize, int userId, string role);
        SupplierModel GetById(int supplierId, int userId, string role);
        OperationResult<int> Create(SupplierModel supplier, int createdBy);
        OperationResult<bool> Update(SupplierModel supplier, int modifiedBy, int userId, string role);
        OperationResult<bool> Delete(int supplierId, int modifiedBy, string role);
        bool IsDuplicateCode(string supplierCode, int? supplierId);
    }
}
