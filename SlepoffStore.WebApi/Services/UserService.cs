using SlepoffStore.Core;
using SlepoffStore.Repository;
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
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository repository, ILogger<UserService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> CheckCredentials(string username, string password)
        {
            var user = await _repository.ReadUser(username);
            if (user == null)
            {
                _logger.LogTrace("User '{0}' is not found in db", username);
                return false;
            }
            var pwd = ComputeSha256Hash(password);
            var ok = user.Password == pwd;
            if (!ok) _logger.LogTrace("User '{0}': password incorrect", username);
            return ok;
        }

        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using SHA256 sha256Hash = SHA256.Create();
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
