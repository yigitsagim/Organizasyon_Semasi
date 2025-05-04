using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using veri_yapilari.Models;

namespace veri_yapilari.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
{
    return View();
}

public IActionResult insan_k()
{
    return View();
}

public IActionResult satis_d()
{
    return View();
}

public IActionResult pazarlama_d()
{
    return View();
}

public IActionResult bilgi_t()
{
    return View();
}

public IActionResult finans()
{
    return View();
}

    public IActionResult Privacy()
    
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
