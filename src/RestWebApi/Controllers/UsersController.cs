using System;
using System.Linq;
using System.Web.Http;
using JWT.Security.Models;
using JWT.Security.Security;
using JWT.Security.Services;
using Microsoft.IdentityModel.Tokens;
using RestWebApi.Services.Helpers;


namespace RestWebApi.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        //Global variable for Configuration
        Configuration config =CommonHelperService.GetJsonFileData();

        [Route("authenticate")]
        [HttpPost]
        public IHttpActionResult Authenticate(Credentials credentials)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // It goes without question that in the real world our passwords would be hashed
            var user = config.Users.FirstOrDefault(u => u.EmailAddress.Equals(credentials.EmailAddress, StringComparison.OrdinalIgnoreCase) && credentials.Password.Equals(u.Password));
            if(user == null)
                return Unauthorized();
            //Getting the lifetime of the token and generating the web service.
            var lifetimeInMinutes = SecurityConfiguration.Lifetime;

            SecurityTokenDescriptor securityTokenDescriptor;

            var token = GenerateTokenService.CreateToken(user.UserId.ToString(), user.FullName, lifetimeInMinutes,out securityTokenDescriptor);

            return Ok(new
            {
                Token = token,
                LifetimeInMinutes = lifetimeInMinutes,
                FullName = user.FullName,
                Id = user.UserId,
                StartTime = securityTokenDescriptor.IssuedAt,
                EndDate = securityTokenDescriptor.Expires
                // Any anything else that you want here...
            }); ; ;
        }

        [Authorize]
        [Route("{userId}")]
        public IHttpActionResult GetUser(long userId)
        {
            // Example of using the JWT claims to ensure a user can only access their own user information
            if (userId.ToString() != User.Identity.GetUserId())
                return Unauthorized();

            var user = config.Users.First(u => u.UserId == userId);
            return Ok(user);
        }

    }
}
