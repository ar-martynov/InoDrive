using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using Domain.Entities;
using Domain.Contexts;
using Domain.Models;
using Domain.Infrastructure;

namespace Domain.Abstract.EntityRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        ApplicationUserManager AppUserManager { get; }
        IdentityResult RegisterUser(User user, string password);
        Task<IdentityResult> RegisterUserAsync(User user, string password);
        Task<IdentityResult> ChangePasswordAsync(string userId, string oldPwd, string newPwd);
        User FindUser(string userName, string password);
        Task<User> FindUserAsync(string userName, string password);
        UserProfile GetProfile(string id);
        bool UpdateProfile(UserProfile profile);
        User CheckUser(string id);
        int GetUserRating(User user);

    }
}
