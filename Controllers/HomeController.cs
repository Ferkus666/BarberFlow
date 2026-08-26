using BarberFlow.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Erro(int statusCode = 500, string? traceId = null)
    {
        Response.StatusCode = statusCode;
        return View(new ErroViewModel
        {
            StatusCode = statusCode,
            TraceId = string.IsNullOrWhiteSpace(traceId) ? HttpContext.TraceIdentifier : traceId
        });
    }
}
