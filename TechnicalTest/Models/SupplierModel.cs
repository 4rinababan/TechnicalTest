using System;
using System.ComponentModel.DataAnnotations;

namespace TechnicalTest.Models
{
    public class SupplierModel
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Kode supplier wajib diisi")]
        [StringLength(20, ErrorMessage = "Kode supplier maksimal 20 karakter")]
        [Display(Name = "Kode Supplier")]
        public string SupplierCode { get; set; }

        [Required(ErrorMessage = "Nama supplier wajib diisi")]
        [StringLength(150, ErrorMessage = "Nama supplier maksimal 150 karakter")]
        [Display(Name = "Nama Supplier")]
        public string SupplierName { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(30)]
        [Display(Name = "No. Telepon")]
        [RegularExpression(@"^[0-9\-\+\s]*$", ErrorMessage = "Format nomor telepon tidak valid")]
        public string Phone { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string Email { get; set; }

        [Display(Name = "Status Aktif")]
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    /// <summary>Parameter untuk pencarian/filter/paging list supplier.</summary>
    public class SupplierQueryModel
    {
        public string Keyword { get; set; }
        public string City { get; set; }
        public bool? IsActive { get; set; }
        public string SortColumn { get; set; } = "SupplierName";
        public string SortDirection { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
