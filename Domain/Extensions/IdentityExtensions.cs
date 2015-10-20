using System;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

using Domain.Models;
using Domain.Entities;

namespace Domain
{
    /// <summary>
    /// This class allows users use userName or e-mail for auth
    /// </summary>
    public static class IdentityExtensions
    {
        public static async Task<User> FindByNameOrEmailAsync (this UserManager<User> userManager, string usernameOrEmail, string password)
        {
            var username = usernameOrEmail;
            if (usernameOrEmail.Contains("@"))
            {
                var userForEmail = await userManager.FindByEmailAsync(usernameOrEmail);
                if (userForEmail != null)
                {
                    username = userForEmail.UserName;
                }
            }
            return await userManager.FindAsync(username, password);

        }

        public static User FindByNameOrEmail (this UserManager<User> userManager, string usernameOrEmail, string password)
        {
            var username = usernameOrEmail;
            if (usernameOrEmail.Contains("@"))
            {
                var userForEmail = userManager.FindByEmail(usernameOrEmail);
                if (userForEmail != null)
                {
                    username = userForEmail.UserName;
                }
            }
            return userManager.Find(username, password);
        }
    }
}