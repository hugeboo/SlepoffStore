using SlepoffStore.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username);
    }
}
