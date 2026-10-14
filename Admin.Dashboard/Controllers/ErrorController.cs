using Microsoft.AspNetCore.Mvc;

namespace Admin.Dashboard.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/NotFound")]
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }

        [Route("Error/ServerInternalError")]
        public IActionResult ServerError()
        {
            return View("ServerError");
        }
    }
}