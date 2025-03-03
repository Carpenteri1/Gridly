using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

public class MainController : Controller
{
    private readonly IWebHostEnvironment _env;

    public MainController(IWebHostEnvironment env)
    {
        _env = env;
    }

    public IActionResult Index()
    {
        var indexPath = Path.Combine(_env.WebRootPath, "index.html");

        if (System.IO.File.Exists(indexPath))
        {
            return PhysicalFile(indexPath, "text/html");
        }

        return NotFound();
    }
}
