using DAL.Models;
using System;

namespace Services.IdentityServices
{
    public class UserService : DualPracticeServiceAbstract<User>, IUserService
    {
        public UserService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
