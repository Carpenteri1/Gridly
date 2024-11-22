using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Gridly.Controllers;

public class MainController : Controller
{
    private readonly IWebHostEnvironment _env;

    public MainController(IWebHostEnvironment env)
    {
        _env = env;
    }

    // This action returns the Angular index.html
    public IActionResult Index()
    {
        var indexPath = Path.Combine(_env.WebRootPath, "index.html");

        // Ensure the file exists
        if (System.IO.File.Exists(indexPath))
        {
            return PhysicalFile(indexPath, "text/html");
        }

        return NotFound();
    }
}
