using System;
using System.Data;
using System.Data.SqlClient;
using TechnicalTest.Helpers;
using TechnicalTest.Models;

namespace TechnicalTest.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserCredentialModel GetCredentialByUsername(string username)
        {
            DataRow row = DbHelper.ExecuteSingleRow(
                "sp_Login",
                new SqlParameter("@Username", username));

            if (row == null)
            {
                return null;
            }

            return new UserCredentialModel
            {
                UserID = Convert.ToInt32(row["UserID"]),
                Username = row["Username"].ToString(),
                FullName = row["FullName"].ToString(),
                Role = row["Role"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                PasswordSalt = row["PasswordSalt"].ToString()
            };
        }
    }
}
