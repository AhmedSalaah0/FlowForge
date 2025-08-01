using Microsoft.AspNetCore.Mvc;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("Error")]
    public class ErrorsController : Controller
    {
        public IActionResult StatusCode(int code = 500)
        {
            Response.StatusCode = code;
            return code switch
            {
                403 => View("Forbidden"),
                404 => View("Error404"),
                _ => View("Error"),
            };
        }
    }
}
