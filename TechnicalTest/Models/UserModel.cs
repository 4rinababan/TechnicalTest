using System;

namespace TechnicalTest.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // "Admin" atau "Supplier"
    }

    /// <summary>
    /// Dipakai KHUSUS di dalam proses login untuk verifikasi hash.
    /// Tidak boleh dipakai/ditampilkan di luar AccountController.
    /// </summary>
    public class UserCredentialModel : UserModel
    {
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
