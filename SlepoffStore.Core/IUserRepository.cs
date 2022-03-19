using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Core
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username);
    }
}
