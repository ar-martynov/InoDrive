using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Domain;
using Domain.Abstract.EntityRepositories;
using Domain.Entities;
using Domain.Models;
using Domain.Helpers;
using Domain.Concrete;
using Domain.Abstract;

using WebUI.Infrastructure;

namespace WebUI.Controllers
{
    [ApplicationExceptionFilterAttribute]
    [RoutePrefix("api/RefreshTokens")]
    public class RefreshTokensController : BaseApiController
    {
        public RefreshTokensController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /*
        //[Authorize(Users = "Admin")]
        [AllowAnonymous]
        public IHttpActionResult Get()
        {
            return Ok(_repo.GetAllRefreshTokens());
        }

        //[Authorize(Users = "Admin")]
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> Delete(string tokenId)
        {
            string hash = CryptHelper.GetHash(tokenId);
            var result = await _repo.RemoveRefreshToken(hash);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Token Id does not exist");

        }
         * */
    }
}
