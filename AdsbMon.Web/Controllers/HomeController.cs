using System.Diagnostics;
using AdsbMon.Core.Services;
using Microsoft.AspNetCore.Mvc;
using AdsbMon.Web.Models;

namespace AdsbMon.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AircraftService _aircraftService;

    public HomeController(ILogger<HomeController> logger, AircraftService aircraftService)
    {
        _logger = logger;
        _aircraftService = aircraftService;
    }

    public IActionResult Index()
    {
        var aircraft = _aircraftService.GetAllAircraft();
        var model = new List<AircraftViewModel>();
        foreach (var ac in aircraft)
        {
            model.Add(new AircraftViewModel() { Icao = ac.Icao });
        }
        return View(model);
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