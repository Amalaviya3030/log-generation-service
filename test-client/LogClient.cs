// LogClient.cs
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoggingClient
{
    public class LogClient
    {
        private readonly string serverHost;
        private readonly int serverPort;
        private readonly string clientId;

        public LogClient(string host, int port, string clientId)
        {
            this.serverHost = host;
            this.serverPort = port;
            this.clientId = clientId;
        }

        public async Task<bool> SendLog(string level, string message, string formatType)
        {
            var logData = new
            {
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                level = level,
                message = message,
                client_id = clientId,
                format_type = formatType
            };

            using (TcpClient client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(serverHost, serverPort);
                    using (NetworkStream stream = client.GetStream())
                    {
                        string jsonData = JsonSerializer.Serialize(logData);
                        byte[] data = Encoding.UTF8.GetBytes(jsonData);
                        await stream.WriteAsync(data, 0, data.Length);

                        // Read response
                        byte[] responseBuffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                        string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                        
                        var responseObj = JsonSerializer.Deserialize<JsonElement>(response);
                        if (responseObj.TryGetProperty("error", out var error))
                        {
                            Console.WriteLine($"Error: {error}");
                            return false;
                        }
                        if (responseObj.TryGetProperty("message", out var responseMessage))
                        {
                            Console.WriteLine(responseMessage);
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }
    }
}