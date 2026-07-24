using System;
using System.Data;
using System.Data.SqlClient;
using TechnicalTest.Helpers;
using TechnicalTest.Models;

namespace TechnicalTest.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        public PagedResult<SupplierModel> GetList(SupplierQueryModel query, int userId, string role)
        {
            var table = DbHelper.ExecuteDataTable(
                "sp_Supplier_GetList",
                new SqlParameter("@UserID", userId),
                new SqlParameter("@Role", role),
                new SqlParameter("@City", (object)query.City ?? DBNull.Value),
                new SqlParameter("@IsActive", (object)query.IsActive ?? DBNull.Value),
                new SqlParameter("@SortColumn", query.SortColumn ?? "SupplierName"),
                new SqlParameter("@SortDirection", query.SortDirection ?? "ASC"),
                new SqlParameter("@PageNumber", query.PageNumber),
                new SqlParameter("@PageSize", query.PageSize));

            return MapToPagedResult(table, query.PageNumber, query.PageSize);
        }

        public PagedResult<SupplierModel> Search(string keyword, int pageNumber, int pageSize, int userId, string role)
        {
            var table = DbHelper.ExecuteDataTable(
                "sp_Supplier_Search",
                new SqlParameter("@Keyword", keyword ?? string.Empty),
                new SqlParameter("@UserID", userId),
                new SqlParameter("@Role", role),
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize", pageSize));

            return MapToPagedResult(table, pageNumber, pageSize);
        }

        public SupplierModel GetById(int supplierId, int userId, string role)
        {
            var row = DbHelper.ExecuteSingleRow(
                "sp_Supplier_GetById",
                new SqlParameter("@SupplierID", supplierId),
                new SqlParameter("@UserID", userId),
                new SqlParameter("@Role", role));

            return row == null ? null : MapRow(row);
        }

        public int Insert(SupplierModel supplier, int createdBy)
        {
            var outputParam = new SqlParameter("@NewSupplierID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            DbHelper.ExecuteNonQueryWithOutputParams("sp_Supplier_Insert", new[]
            {
                new SqlParameter("@SupplierCode", supplier.SupplierCode),
                new SqlParameter("@SupplierName", supplier.SupplierName),
                new SqlParameter("@Address", (object)supplier.Address ?? DBNull.Value),
                new SqlParameter("@City", (object)supplier.City ?? DBNull.Value),
                new SqlParameter("@Phone", (object)supplier.Phone ?? DBNull.Value),
                new SqlParameter("@Email", (object)supplier.Email ?? DBNull.Value),
                new SqlParameter("@CreatedBy", createdBy),
                outputParam
            });

            return (int)outputParam.Value;
        }

        public bool Update(SupplierModel supplier, int modifiedBy, int userId, string role)
        {
            var result = DbHelper.ExecuteScalar(
                "sp_Supplier_Update",
                new SqlParameter("@SupplierID", supplier.SupplierID),
                new SqlParameter("@SupplierCode", supplier.SupplierCode),
                new SqlParameter("@SupplierName", supplier.SupplierName),
                new SqlParameter("@Address", (object)supplier.Address ?? DBNull.Value),
                new SqlParameter("@City", (object)supplier.City ?? DBNull.Value),
                new SqlParameter("@Phone", (object)supplier.Phone ?? DBNull.Value),
                new SqlParameter("@Email", (object)supplier.Email ?? DBNull.Value),
                new SqlParameter("@IsActive", supplier.IsActive),
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@UserID", userId),
                new SqlParameter("@Role", role));

            return Convert.ToInt32(result) > 0;
        }

        public bool Delete(int supplierId, int modifiedBy, string role)
        {
            var result = DbHelper.ExecuteScalar(
                "sp_Supplier_Delete",
                new SqlParameter("@SupplierID", supplierId),
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Role", role));

            return Convert.ToInt32(result) > 0;
        }

        public bool IsDuplicateCode(string supplierCode, int? supplierId)
        {
            var result = DbHelper.ExecuteScalar(
                "sp_Supplier_CheckDuplicateCode",
                new SqlParameter("@SupplierCode", supplierCode),
                new SqlParameter("@SupplierID", (object)supplierId ?? DBNull.Value));

            return Convert.ToInt32(result) == 1;
        }

        private static PagedResult<SupplierModel> MapToPagedResult(DataTable table, int pageNumber, int pageSize)
        {
            var result = new PagedResult<SupplierModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = table.Rows.Count > 0 ? Convert.ToInt32(table.Rows[0]["TotalRecords"]) : 0
            };

            foreach (DataRow row in table.Rows)
            {
                result.Items.Add(MapRow(row));
            }

            return result;
        }

        private static SupplierModel MapRow(DataRow row)
        {
            return new SupplierModel
            {
                SupplierID = Convert.ToInt32(row["SupplierID"]),
                SupplierCode = row["SupplierCode"].ToString(),
                SupplierName = row["SupplierName"].ToString(),
                Address = row["Address"] == DBNull.Value ? null : row["Address"].ToString(),
                City = row["City"] == DBNull.Value ? null : row["City"].ToString(),
                Phone = row["Phone"] == DBNull.Value ? null : row["Phone"].ToString(),
                Email = row["Email"] == DBNull.Value ? null : row["Email"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                ModifiedDate = row["ModifiedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ModifiedDate"])
            };
        }
    }
}
