using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

using Domain.Contexts;
using Domain.Infrastructure;
using Domain.Models;
using Domain.Entities;
using Domain.Abstract;
using Domain.Abstract.EntityRepositories;

namespace Domain.Concrete.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository, IDisposable
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager AppUserManager
        {
            get
            {
                return this._userManager;
            }
        }

        public UserRepository(InoDriveContext context)
            : base(context)
        {
            _userManager = new ApplicationUserManager(new UserStore<User>((IdentityDbContext<User>)_context));
           
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public IdentityResult RegisterUser(User user, string password)
        {
            return _userManager.Create(user, password);
        }

        /// <summary>
        /// Registers the user asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<IdentityResult> RegisterUserAsync(User user, string password)
        {

            return await _userManager.CreateAsync(user, password);
        }

        /// <summary>
        /// Changes the password asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="oldPwd">The old password.</param>
        /// <param name="newPwd">The new password.</param>
        /// <returns></returns>
        public async Task<IdentityResult> ChangePasswordAsync(string userId, string oldPwd, string newPwd)
        {
            return await _userManager.ChangePasswordAsync(userId, oldPwd, newPwd);
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public UserProfile GetProfile(string id)
        {
            var userProfile = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
            return userProfile;
        }

        /// <summary>
        /// Updates the profile.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <returns></returns>
        public bool UpdateProfile(UserProfile profile)
        {
            var oldProfile = _context.UserProfiles.SingleOrDefault(x => x.UserId == profile.UserId);
            if (oldProfile == null) return false;

            _context.Entry(oldProfile).CurrentValues.SetValues(profile);
            return true;
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public User FindUser(string userName, string password)
        {
            //_userManager.Find(userName, password); //user can login by userName only
            User user = _userManager.FindByNameOrEmail(userName, password);
            return user;
        }

        /// <summary>
        /// Finds the user asynchronous.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<User> FindUserAsync(string userName, string password)
        {
            //return await _userManager.FindAsync(userName, password); //user can login by userName only
            // **Extension for UserManager class** user can login by userName or E-mail
            User user = await _userManager.FindByNameOrEmailAsync(userName, password);
            return user;
        }

        /// <summary>
        /// Checks the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public User CheckUser(string id)
        {
            User user = _userManager.FindById(id);
            return user;
        }

        /// <summary>
        /// Gets the user rating.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int GetUserRating(User user)
        { 
            //user.Ratings.Count()
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            _userManager.Dispose();
        }

    }
}
