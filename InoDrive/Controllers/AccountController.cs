using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

using AutoMapper;

using Domain;
using Domain.Abstract;
using Domain.Abstract.EntityRepositories;
using Domain.Entities;
using Domain.Models;
using Domain.Concrete.Repositories;

using WebUI.Infrastructure;

namespace WebUI.Controllers
{
    // api/Account/
    [ApplicationExceptionFilterAttribute]
    [RoutePrefix("api/Account")]
    public class AccountController : BaseApiController
    {
        public AccountController(IUnitOfWork unitOfWork) : base(unitOfWork) { }


        // POST api/Account/Register/
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserRegistrationModel userModel)
        {
            if (userModel == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User newUser = Mapper.Map<UserRegistrationModel, User>(userModel);
            newUser.UserProfile = Mapper.Map<UserRegistrationModel, UserProfile>(userModel);
            newUser.UserProfile.UserId = newUser.Id;

            var repo = _UOW.GetRepository<IUserRepository>();
            IdentityResult result = await repo.RegisterUserAsync(newUser, userModel.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);

            }

           
            return Json(new { message = Resources.Language.UserRegistered });
        }

        // POST api/Account/ChangePassword/
        [ApplicationAuthorizeAttribute]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repo = _UOW.GetRepository<IUserRepository>();
            IdentityResult result = await repo.AppUserManager.ChangePasswordAsync(GetUserId(), model.OldPassword, model.NewPassword);
            await repo.AppUserManager.UpdateSecurityStampAsync(GetUserId());
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            
            return Json(new { message = Resources.Language.PasswordChanged });
        }

        // GET api/Account/GetProfile
        [ApplicationAuthorizeAttribute]
        [Route("GetProfile")]
        public IHttpActionResult GetProfile()
        {
            var repo = _UOW.GetRepository<IUserRepository>();

            UserProfile profile = repo.GetProfile(GetUserId());
            

            if (profile != null) return Ok(Mapper.Map<UserProfile, UserProfileModel>(profile));
            return BadRequest(Resources.Language.UserProfileNotExist);
        }

        // GET api/Account/UpdateProfile
        [ApplicationAuthorizeAttribute]
        [Route("UpdateProfile")]
        public IHttpActionResult UpdateProfile([FromBody]UserProfileModel profile)
        {
            UserProfile profileToUpdate = Mapper.Map<UserProfileModel, UserProfile>(profile);
            profileToUpdate.UserId = GetUserId();
            var repo = _UOW.GetRepository<IUserRepository>();
            repo.UpdateProfile(profileToUpdate);
            _UOW.Commit();
           

            return Json(new { message = Resources.Language.UserProfileUpdated });
        }

        // POST api/Account/ChangeLogin
        [ApplicationAuthorizeAttribute]
        [Route("ChangeLogin")]
        public async Task<IHttpActionResult> ChangeLogin(ChangeLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var repo = _UOW.GetRepository<IUserRepository>();

            var user = await repo.AppUserManager.FindByEmailAsync(model.OldEmail);
            if (user == null || user.Id != GetUserId()) return BadRequest(Resources.Language.IncorrectEmail);

            var userWithNewMail = await repo.AppUserManager.FindByEmailAsync(model.NewEmail);
            if (userWithNewMail != null) return BadRequest(Resources.Language.EmailAlreadyInUse);

            var result = await repo.AppUserManager.UpdateSecurityStampAsync(user.Id);

            var provider = new DpapiDataProtectionProvider("Sample");
            repo.AppUserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(provider.Create("EmailConfirmation"));
            string code = repo.AppUserManager.GenerateEmailConfirmationToken(user.Id);
            code = System.Web.HttpUtility.UrlEncode(code);

            var callbackUrl = new Uri(Url.Link("ConfirmChangeLogin", new { userId = user.Id, email = model.NewEmail.ToLower(), code = code }));
            string message = string.Format("Пожалуйста, подтвердите ваш новый E-mail ( {0} ) перейдя по <a href=\"{1}\">ссылке</a>.", model.NewEmail, callbackUrl);
        
            await repo.AppUserManager.SendEmailAsync(user.Id, "Подтверждение смены E-mail.", message);
            
            return Json(new { message = "Письмо смены E-mail отправлено на " + model.OldEmail });


        }

        // GET api/Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmail", Name = "ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId, string code)
        {
            var repo = _UOW.GetRepository<IUserRepository>();
            var user = await repo.AppUserManager.FindByIdAsync(userId);
            if (user == null) return BadRequest(Resources.Language.UserNotExist);



            var provider = new DpapiDataProtectionProvider("InoDrive");
            repo.AppUserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(provider.Create("EmailConfirmation"));

            var result = await repo.AppUserManager.ConfirmEmailAsync(user.Id, code);
            if (!result.Succeeded) return BadRequest(Resources.Language.EmailConfirmError);
            

            return Json(new { message = Resources.Language.EmailConfirmSuccess });
        }

        // GET api/Account/ConfirmChangeLogin
        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmChangeLogin", Name = "ConfirmChangeLogin")]
        public async Task<IHttpActionResult> ConfirmChangeLogin(string userId, string code, string email)
        {
            code = System.Web.HttpUtility.UrlDecode(code);

            var repo = _UOW.GetRepository<IUserRepository>();
            var user = await repo.AppUserManager.FindByIdAsync(userId);
            if (user == null) return BadRequest(Resources.Language.UserNotExist);
          
            var provider = new DpapiDataProtectionProvider("Sample");
            repo.AppUserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(provider.Create("EmailConfirmation"));

            var result = await repo.AppUserManager.ConfirmEmailAsync(user.Id, code);
            if (!result.Succeeded) return BadRequest(Resources.Language.EmailConfirmError);

            user.Email = email;
            user.UserName = email;
            result = await repo.AppUserManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(Resources.Language.EmailConfirmError);

            await repo.AppUserManager.SendEmailAsync(user.Id, "Смена E-mail.", "E-mail успешно изменен на текущий.");

            await repo.AppUserManager.UpdateSecurityStampAsync(user.Id);
           
            return Json(new { message = Resources.Language.EmailConfirmSuccess });
        }



        // POST api/Account/restore
        [HttpPost]
        [AllowAnonymous]
        [Route("Restore")]
        public async Task<IHttpActionResult> RestorePasswordSendEmail(PasswordRestoreModel model)
        {
            var repo = _UOW.GetRepository<IUserRepository>();

            var user = await repo.AppUserManager.FindByEmailAsync(model.UserName);
            if (user == null ) return BadRequest(Resources.Language.EmailNotExist);

            var result = await repo.AppUserManager.UpdateSecurityStampAsync(user.Id);

            var provider = new DpapiDataProtectionProvider("Sample");
            repo.AppUserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(provider.Create("PasswordConfirmation"));
            string code = await repo.AppUserManager.GeneratePasswordResetTokenAsync(user.Id);
            code = System.Web.HttpUtility.UrlEncode(code);

            string password = System.Web.Security.Membership.GeneratePassword(10, 0);
            var regex = new System.Text.RegularExpressions.Regex(@"[^A-Za-z0-9]+");
            password = regex.Replace(password, "");

            var callbackUrl = new Uri(Url.Link("Restore", new { username = model.UserName, code = code, password = password }));
            string message = string.Format("Пожалуйста, подтвердите ваш новый пароль ( {0} ) перейдя по <a href=\"{1}\">ссылке</a>.", password, callbackUrl);


            await repo.AppUserManager.SendEmailAsync(user.Id, "Восстановление пароля от системы.", message);

            return Json(new { message = String.Format("Письмо с паролем отправлено на {0}.", model.UserName) });


        }

        // Get api/Account/Restore
        [HttpGet]
        [AllowAnonymous]
        [Route("Restore", Name = "Restore")]
        public async Task<IHttpActionResult> PasswordConfirm([FromUri] PasswordRestoreModel model)
        {
            model.Code = System.Web.HttpUtility.UrlDecode(model.Code);
            var repo = _UOW.GetRepository<IUserRepository>();

            var user = await repo.AppUserManager.FindByEmailAsync(model.UserName);
            if (user == null) return BadRequest(Resources.Language.EmailNotExist);

            var provider = new DpapiDataProtectionProvider("Sample");
            repo.AppUserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(provider.Create("PasswordConfirmation"));

            var result = await repo.AppUserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded) await repo.AppUserManager.UpdateSecurityStampAsync(user.Id);
           


            return Json(new { message = "Пароль изменен." });
        }
    }
}
