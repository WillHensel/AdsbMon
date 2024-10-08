using AdsbMon.Core.Services;
using AdsbMon.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdsbMon.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AircraftController : ControllerBase
{
    private readonly AircraftService _aircraftService;

    public AircraftController(AircraftService aircraftService)
    {
        _aircraftService = aircraftService;
    }

    public List<AircraftViewModel> Index()
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
                GroundTrackAngle = ac.GroundTrackAngle,
                Altitude = ac.Altitude
            });
        }

        return model;
    }
}