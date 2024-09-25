using System.Diagnostics;
using System.Text.Json;
using AdsbMon.Core.Services;
using Microsoft.AspNetCore.Mvc;
using AdsbMon.Web.Models;
using Microsoft.Net.Http.Headers;

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
        var model = GetAircraftModel();
        return View(model);
    }

    public async Task GetAircraftUpdate(CancellationToken cancellationToken)
    {
        Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            
            var model = GetAircraftModel();
            var dataString = $"data: {JsonSerializer.Serialize(model)}\n\n";
            await Response.WriteAsync(dataString, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        await Response.CompleteAsync();
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

    private List<AircraftViewModel> GetAircraftModel()
    {
        var aircraft = _aircraftService.GetAllAircraft();
        var model = new List<AircraftViewModel>();
        foreach (var ac in aircraft)
        {
            model.Add(new AircraftViewModel()
            {
                Icao = ac.Icao,
                Latitude = ac.Latitude,
                Longitude = ac.Longitude,
                GroundSpeed= ac.GroundSpeed,
                GroundTrackAngle = ac.GroundTrackAngle
            });
        }

        return model;
    }
}