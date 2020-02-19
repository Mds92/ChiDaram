using Microsoft.AspNetCore.Mvc;

namespace ChiDaram.Api.Controllers
{
    public class TestController : BaseController
    {
        [HttpPost]
        public IActionResult TestPersianDateTime()
        {
            return Ok(true);
        }
    }
}
