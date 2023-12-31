using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using panneau_project.Models;
using Npgsql;

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
        Departement departement = new Departement();
        Connexion connexion = new Connexion();
        NpgsqlConnection liaisonbase = connexion.createLiaisonBase();
        List<Departement> listedepartement = departement.getalldepartement(liaisonbase);
        ViewBag.listedepartement = listedepartement;
        return View("previsionformulaire");
    }

    public IActionResult verscoupure(String id_departement,String date)
    {   
        DateTime datetime = DateTime.Parse(date);
        Departement departement = new Departement(id_departement);
        Connexion connexion = new Connexion();
        NpgsqlConnection liaisonbase = connexion.createLiaisonBase(); 

        List<Luminiosite> liste_luminiosite = new List<Luminiosite>();
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-21 08:00:00"),8));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-21 09:00:00"),7));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-21 10:00:00"),9));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-21 11:00:00"),9));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-21 14:00:00"),8));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-21 15:00:00"),7));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-21 16:00:00"),6));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-21 17:00:00"),4));

        DateTime datecoupure = departement.datecoupureprevision(liaisonbase,datetime,liste_luminiosite);
        ViewBag.date = date;
        ViewBag.datecoupure = datecoupure.ToString("HH:mm:ss");
        ViewBag.departement = departement;
        return View("coupure");
    }

    public IActionResult versdetailcoupure(String id_departement,String date)
    {
        Console.WriteLine(date);
        DateTime datetime = DateTime.Parse(date);
        Departement departement = new Departement(id_departement);
        Connexion connexion = new Connexion();
        NpgsqlConnection liaisonbase = connexion.createLiaisonBase(); 

        List<Luminiosite> liste_luminiosite = new List<Luminiosite>();
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-20 08:00:00"),9));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-20 09:00:00"),9));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-20 10:00:00"),9));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-20 11:00:00"),9));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-20 14:00:00"),9));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-20 15:00:00"),9));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-20 16:00:00"),9));
        liste_luminiosite.Add(new Luminiosite(DateTime.Parse("2023-12-20 17:00:00"),9));


        List<Luminiosite> listedetail = departement.todoprevision(liaisonbase,datetime,liste_luminiosite);
        double consommation= departement.moyenne_consommation(liaisonbase);
        double nombre_eleve = departement.getnumberelevebynamedate(liaisonbase,datetime); 
        ViewBag.liste = listedetail;
        ViewBag.date = date;
        ViewBag.consommation =consommation; 
        ViewBag.nombre_eleve = nombre_eleve;
        ViewBag.total_consommation = consommation*nombre_eleve;
        ViewBag.departement = departement;
        ViewBag.capacitebatterie = departement.getcapacitebatterie(liaisonbase);


        return View("detail_coupure");
    }

    public IActionResult teste(){
        DateTime datetime = DateTime.Parse("2023-12-13");
        Departement departement = new Departement("departement1","A");
        double a = departement.getConsommationDepartement(null,datetime);
        //Console.WriteLine("ok : "+a.ToString());
        //List<Luminiosite> liste = departement.getLuminiosite_departement_panneau(null,datetime);
         //DateTime a = departement.coupurewithconsommationteste(null,17299.999999999999,liste);
     Console.WriteLine("ok : "+a);
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
