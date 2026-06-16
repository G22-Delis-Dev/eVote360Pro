using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using eVote360Pro.Web.Models;

namespace eVote360Pro.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Acción inicial que redirige al dashboard administrativo
    public IActionResult Index()
    {
        // Por ahora redirigimos al HomeAdmin para que el 404 desaparezca.
        // Después, aquí puedes añadir lógica para saber a qué carpeta enviar al usuario.
        return RedirectToAction("Index", "HomeAdmin");
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