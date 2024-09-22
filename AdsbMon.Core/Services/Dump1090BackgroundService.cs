using AdsbMon.Core.Models;
using AdsbMon.Core.Models.Messages;
using Microsoft.Extensions.Hosting;

namespace AdsbMon.Core.Services;

/// <summary>
/// Takes Mode S extended squitter messages from a socket client,
/// transforms them, and updates the aircraft state
/// </summary>
public class Dump1090BackgroundService : BackgroundService, IDisposable, IAsyncDisposable
{
    private readonly AircraftService _aircraftService;
    private readonly Dump1090Client _dump1090Client;

    public Dump1090BackgroundService(AircraftService aircraftService)
    {
        _aircraftService = aircraftService;
        _dump1090Client = new();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_dump1090Client.IsConnected)
        {
            await _dump1090Client.Connect();
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWork();
            await Task.Delay(TimeSpan.FromMilliseconds(10), stoppingToken);
        }
    }

    private async Task DoWork()
    {
        try
        {
            var next = await _dump1090Client.GetNextSocketMessage();
            next = next.TrimStart('*').TrimEnd('\n').TrimEnd(';');
            var message = Message.FromHex(next);
            _aircraftService.UpdateAircraft(message);
        }
        catch
        {
            // Ignore
        }
    }

    public override void Dispose()
    {
        _dump1090Client.Dispose();
        base.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _dump1090Client.DisposeAsync();
        base.Dispose();
    }
}