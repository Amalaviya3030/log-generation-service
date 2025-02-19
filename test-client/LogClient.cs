using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Configuration;
using System.Threading.Tasks;

public class LogClient
{
    private readonly string host;
    private readonly int port;
    private readonly string clientId;

    public LogClient()
    {
        host = ConfigurationManager.AppSettings["ServerHost"] ?? "127.0.0.1";
        port = int.Parse(ConfigurationManager.AppSettings["ServerPort"] ?? "5000");
        clientId = ConfigurationManager.AppSettings["ClientId"] ?? "TestClient1";
    }

    public async Task<bool> SendLog(string level, string message, string format = "default", 
        string sourceFile = "", int lineNumber = 0)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(host, port);
            
            var logMessage = new
            {
                client_id = clientId,
                level = level,
                message = message,
                format = format,
                source_file = sourceFile,
                line_number = lineNumber
            };

            var json = JsonSerializer.Serialize(logMessage);
            var data = Encoding.UTF8.GetBytes(json);

            using var stream = client.GetStream();
            await stream.WriteAsync(data);

            // Read response
            var buffer = new byte[1024];
            var bytesRead = await stream.ReadAsync(buffer);
            var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            var responseObj = JsonSerializer.Deserialize<JsonElement>(response);
            var status = responseObj.GetProperty("status").GetString();

            if (status != "success")
            {
                Console.WriteLine($"Error: {responseObj.GetProperty("message").GetString()}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending log: {ex.Message}");
            return false;
        }
    }
}