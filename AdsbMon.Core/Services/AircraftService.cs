using AdsbMon.Core.Models;

namespace AdsbMon.Core.Services;

/// <summary>
/// Singleton service responsible for tracking aircraft state
/// </summary>
public class AircraftService
{

    private Dictionary<string, Aircraft> _state = new();

    /// <summary>
    /// Updates an aircraft in the state or creates a new
    /// entry if it doesn't exist
    /// </summary>
    /// <param name="aircraft">The aircraft to update or add</param>
    public void UpdateAircraft(Aircraft aircraft)
    {
        
    }
}