using System;
using System.Collections.Generic;
using System.Text;

namespace SlepoffStore.Core
{
    public interface IUserRepository
    {
        User GetUser(string username);
    }
}
