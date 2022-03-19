using SlepoffStore.Core;
using System.Security.Cryptography;
using System.Text;

namespace SlepoffStore.WebApi.Services
{
    public interface IUserService
    {
        Task<bool> CheckCredentials(string username, string password);
    }

    internal sealed class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CheckCredentials(string username, string password)
        {
            var user = await _repository.GetUser(username);
            if (user == null) return false;
            var pwd = ComputeSha256Hash(password);
            return user.Password == pwd;
        }

        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
