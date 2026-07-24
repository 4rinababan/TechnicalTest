namespace TechnicalTest.Models
{
    public class SupplierListViewModel
    {
        public SupplierQueryModel Query { get; set; }
        public PagedResult<SupplierModel> Result { get; set; }
    }
}
