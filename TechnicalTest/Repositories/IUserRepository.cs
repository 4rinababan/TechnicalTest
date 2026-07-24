using TechnicalTest.Models;

namespace TechnicalTest.Repositories
{
    public interface IUserRepository
    {
        /// <summary>Ambil kredensial user by username, untuk keperluan verifikasi login.</summary>
        UserCredentialModel GetCredentialByUsername(string username);
    }
}
