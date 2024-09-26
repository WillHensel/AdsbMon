using AdsbMon.Core.Models;
using AdsbMon.Core.Models.Messages;

namespace AdsbMon.Core.Services;

/// <summary>
/// Service for tracking aircraft state
/// </summary>
public class AircraftService
{
    private readonly Dictionary<string, Aircraft> _state = new();

    /// <summary>
    /// Updates an aircraft in the state or creates a new
    /// entry if it doesn't exist
    /// </summary>
    /// <param name="message">The message to update the aircraft with</param>
    public void UpdateAircraft(Message message)
    {
        var aircraftToUpdate = _state.GetValueOrDefault(message.Icao);
        if (aircraftToUpdate == null)
        {
            var aircraft = Aircraft.FromMessage(message);
            _state[message.Icao] = aircraft;
        }
        else
        {
            aircraftToUpdate.UpdateWithMessage(message);
        }
        
        RemoveOld();
    }

    /// <summary>
    /// Retrieves all the aircraft in the state
    /// </summary>
    /// <returns>List of all aircraft in state</returns>
    public List<Aircraft> GetAllAircraft()
    {
        return _state.Values.ToList();
    }

    /// <summary>
    /// Retrieves a singular aircraft from the state by ICAO
    /// </summary>
    /// <param name="icao">The ICAO string of the aircraft to retrieve</param>
    /// <returns>An Aircraft object if one exists with the ICAO, otherwise returns null</returns>
    public Aircraft? GetAircraft(string icao)
    {
        return _state.GetValueOrDefault(icao);
    }

    private void RemoveOld()
    {
        foreach (var item in _state)
        {
            if (DateTime.Now.Subtract(item.Value.LastSeen).Seconds > 15)
            {
                _state.Remove(item.Key);
            }
        }
    }
}