using TechnicalTest.Models;

namespace TechnicalTest.Interfaces
{
    public interface ISupplierRepository
    {
        PagedResult<SupplierModel> GetList(SupplierQueryModel query, int userId, string role);
        PagedResult<SupplierModel> Search(string keyword, int pageNumber, int pageSize, int userId, string role);
        SupplierModel GetById(int supplierId, int userId, string role);
        int Insert(SupplierModel supplier, int createdBy);
        bool Update(SupplierModel supplier, int modifiedBy, int userId, string role);
        bool Delete(int supplierId, int modifiedBy, string role);
        bool IsDuplicateCode(string supplierCode, int? supplierId);
    }
}
