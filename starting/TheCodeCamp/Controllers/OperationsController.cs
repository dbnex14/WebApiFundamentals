using System;
using System.Configuration;
using System.Web.Http;

namespace TheCodeCamp.Controllers
{
    public class OperationsController : ApiController
    {
        // We need to specify verb still in Functional API but because it is operational, we dont want
        // to use traditional REST verbs like GET, PUT, POST, DELETE.  We use OPTIONS which is verb to specify 
        // options on server and this way we make it less discoverable.
        [HttpOptions]
        [Route("api/refreshconfig")]
        public IHttpActionResult RefreshAppSettings()
        {
            try
            {
                // refresh part of webconfig for us.  Note this is Functional API
                ConfigurationManager.RefreshSection("AppSettings");
                return Ok();
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
