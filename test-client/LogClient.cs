/*
 * FILE: LogClient.cs
 * PROJECT: Logging System
 * PROGRAMMER: Aryankumar malaviya ,Smaran Adhikari
 * DATE: 2025-02-23
 * DESCRIPTION: This class is a simple TCP client that sends log messages 
 *              to a remote server. It converts log details into JSON, 
 *              sends them over the network, and handles any response 
 *              from the server.
 */

using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoggingClient
{
    public class LogClient
    {
        private readonly string serverHost; // The log server's hostname or IP
        private readonly int serverPort; // The port to connect to on the log server
        private readonly string clientId; // A unique ID for this client

        public LogClient(string host, int port, string clientId)
        {
            this.serverHost = host;
            this.serverPort = port;
            this.clientId = clientId;
        }

        public async Task<bool> SendLog(string level, string message, string formatType)
        {
            // Create the log data as an anonymous object
            var logData = new
            {
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), // Current timestamp
                level = level, // Log level (e.g., INFO, ERROR, DEBUG)
                message = message, // The actual log message
                client_id = clientId, // Identifies the client sending the log
                format_type = formatType // Log format type
            };

            using (TcpClient client = new TcpClient()) // Create a TCP client
            {
                try
                {
                    await client.ConnectAsync(serverHost, serverPort); // Connect to the log server
                    using (NetworkStream stream = client.GetStream()) // Get the network stream
                    {
                        string jsonData = JsonSerializer.Serialize(logData); // Convert log data to JSON
                        byte[] data = Encoding.UTF8.GetBytes(jsonData); // Convert JSON to bytes
                        await stream.WriteAsync(data, 0, data.Length); // Send log data to server

                        // Prepare buffer for response
                        byte[] responseBuffer = new byte[1024]; 
                        int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length); 
                        string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead); // Convert response to string
                        
                        // Deserialize response
                        var responseObj = JsonSerializer.Deserialize<JsonElement>(response);

                        // Check if the response contains an error
                        if (responseObj.TryGetProperty("error", out var error))
                        {
                            Console.WriteLine($"Error: {error}"); // Print the error
                            return false;
                        }

                        // Print the server's success message (if any)
                        if (responseObj.TryGetProperty("message", out var responseMessage))
                        {
                            Console.WriteLine(responseMessage);
                        }

                        return true; // Log sent successfully
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}"); // Handle connection errors
                    return false;
                }
            }
        }
    }
}
