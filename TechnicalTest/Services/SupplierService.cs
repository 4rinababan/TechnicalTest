using System;
using TechnicalTest.Interfaces;
using TechnicalTest.Models;
using TechnicalTest.Repositories;

namespace TechnicalTest.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService() : this(new SupplierRepository())
        {
        }

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public PagedResult<SupplierModel> GetList(SupplierQueryModel query, int userId, string role)
        {
            return _supplierRepository.GetList(query, userId, role);
        }

        public PagedResult<SupplierModel> Search(string keyword, int pageNumber, int pageSize, int userId, string role)
        {
            return _supplierRepository.Search(keyword, pageNumber, pageSize, userId, role);
        }

        public SupplierModel GetById(int supplierId, int userId, string role)
        {
            return _supplierRepository.GetById(supplierId, userId, role);
        }

        public OperationResult<int> Create(SupplierModel supplier, int createdBy)
        {
            if (_supplierRepository.IsDuplicateCode(supplier.SupplierCode, null))
            {
                return OperationResult<int>.Fail("DuplicateCode");
            }

            try
            {
                var newId = _supplierRepository.Insert(supplier, createdBy);
                return OperationResult<int>.Ok(newId);
            }
            catch (Exception ex)
            {
                // Bubble up a friendly message; logging will be handled by global filters.
                return OperationResult<int>.Fail(ex.Message);
            }
        }

        public OperationResult<bool> Update(SupplierModel supplier, int modifiedBy, int userId, string role)
        {
            if (_supplierRepository.IsDuplicateCode(supplier.SupplierCode, supplier.SupplierID))
            {
                return OperationResult<bool>.Fail("DuplicateCode");
            }

            try
            {
                var updated = _supplierRepository.Update(supplier, modifiedBy, userId, role);
                if (!updated)
                {
                    return OperationResult<bool>.Fail("ForbiddenOrNotFound");
                }
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }

        public OperationResult<bool> Delete(int supplierId, int modifiedBy, string role)
        {
            try
            {
                var deleted = _supplierRepository.Delete(supplierId, modifiedBy, role);
                if (!deleted)
                {
                    return OperationResult<bool>.Fail("ForbiddenOrNotFound");
                }
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message);
            }
        }

        public bool IsDuplicateCode(string supplierCode, int? supplierId)
        {
            return _supplierRepository.IsDuplicateCode(supplierCode, supplierId);
        }
    }
}
