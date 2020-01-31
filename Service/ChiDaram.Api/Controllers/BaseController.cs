using ChiDaram.Api.Classes.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChiDaram.Api.Controllers
{
    [ApiController]
    [CustomExceptionFilter]
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [Route("api/[controller]/[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class BaseController : ControllerBase
    {

    }
}
