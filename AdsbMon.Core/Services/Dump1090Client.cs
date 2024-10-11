using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AdsbMon.Core.Services;

public class Dump1090Client : IDisposable, IAsyncDisposable
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    public bool IsConnected { get; private set; } = false;

    public async Task Connect()
    {
        var ipAddress = Environment.GetEnvironmentVariable("DUMP1090_IP_ADDR");
        var port = Environment.GetEnvironmentVariable("DUMP1090_PORT");

        if (ipAddress == null || port == null)
        {
            throw new Exception("DUMP1090_IP_ADDR and DUMP1090_PORT environment variables are not set! Ensure you've set these variables to an open Dump1090 socket on your network.");
        }
        
        var ipEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.121"), 30002);

        _client = new();
        await _client.ConnectAsync(ipEndPoint);
        _stream = _client.GetStream();
        IsConnected = true;
    }
    public async Task<string> GetNextSocketMessage()
    {
        if (!IsConnected)
        {
            throw new Exception("Client must be connected before reading from socket");
        }

        // We only want the Mode S extended squitter messages which are 112 bits
        // rather than 56.
        // While 112 bits is 14 bytes, the message is hex encoded with a leading "*",
        // and a trailing ";\n". In addition, C# encodes characters as 1 byte so the total length
        // is 31 rather than 14.
        // Later in the process, the hex converter will fix this.
        string message;
        do
        {
            var buffer = new byte[31];
            int received = await _stream!.ReadAsync(buffer);
            message = Encoding.ASCII.GetString(buffer, 0, received);
        } while (message.Length < 31);
        return message;
    }

    public void Dispose()
    {
        _stream?.Dispose();
        _client?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_stream != null) await _stream.DisposeAsync();
        _client?.Dispose();
    }
}