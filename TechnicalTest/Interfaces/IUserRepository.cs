using TechnicalTest.Models;

namespace TechnicalTest.Interfaces
{
    public interface IUserRepository
    {
        UserCredentialModel GetCredentialByUsername(string username);
    }
}
