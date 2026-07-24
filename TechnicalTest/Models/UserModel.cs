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

    public class UserCredentialModel : UserModel
    {
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
