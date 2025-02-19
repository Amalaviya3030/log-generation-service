using System;
using System.Net.Sockets;

namespace LoggingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter server IP address: ");
            string serverIp = Console.ReadLine();
            
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    Console.WriteLine("Attempting to connect to server...");
                    client.Connect(serverIp, 5000);
                    Console.WriteLine("Connected successfully!");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}