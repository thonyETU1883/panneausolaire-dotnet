using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using panneau_project.Models;

namespace panneau_project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        DateTime date = DateTime.Parse("2023-12-01");
        Departement departement = new Departement("departement1","departementA");
        List<Luminiosite> liste = departement.getLuminiosite_departement_panneau(null,date);
        List<Luminiosite> listebat = departement.coupurewithconsommationteste(null,3000,liste,7000);
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult versmanger()
    {
        return View("manger");
    }

        

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
