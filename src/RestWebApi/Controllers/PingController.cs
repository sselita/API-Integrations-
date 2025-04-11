using System.Web.Http;

namespace RestWebApi.Controllers
{
    /// <summary>
    /// Controller for checking the availability of web service.
    /// </summary>
    [RoutePrefix("api/ping")]
    public class PingController : ApiController
    {
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                Message = "You are authenticated!"
            });
        }

        [Authorize]
        [Route("authenticated")]
        public IHttpActionResult GetAuthenticated()
        {
            // Will return 401 Unauthorized if not authenticated
            return  Get();
        }
    }
}
