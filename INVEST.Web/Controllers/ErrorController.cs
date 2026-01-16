using Microsoft.AspNetCore.Mvc;

namespace INVEST.Web.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("/Error")]
        public IActionResult Index(string? traceId = null)
        {
            ViewBag.TraceId = traceId;
            return View("Error");
        }

        [HttpGet("/StatusCode/{code:int}")]
        public IActionResult StatusCodePage(int code)
        {
            // Mantém o status code na resposta
            Response.StatusCode = code;

            return code switch
            {
                404 => View("NotFound"),
                _ => View("Generic", code)
            };
        }
    }
}