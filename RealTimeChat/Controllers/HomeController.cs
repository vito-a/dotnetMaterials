using Microsoft.AspNetCore.Mvc;

namespace RealTimeChat.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return File("~/index.html", "text/html");
    }
}
