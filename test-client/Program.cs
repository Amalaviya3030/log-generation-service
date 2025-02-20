// Program.cs
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LoggingClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            Console.WriteLine("Logging Client");
            
            string defaultHost = config["LogServer:Host"];
            Console.Write($"Enter logging server IP address (default: {defaultHost}): ");
            string host = Console.ReadLine();
            host = string.IsNullOrEmpty(host) ? defaultHost : host;

            int defaultPort = int.Parse(config["LogServer:Port"]);
            Console.Write($"Enter server port (default: {defaultPort}): ");
            string portStr = Console.ReadLine();
            int port = string.IsNullOrEmpty(portStr) ? defaultPort : int.Parse(portStr);

            Console.Write("Enter client identifier: ");
            string clientId = Console.ReadLine();

            var client = new LogClient(host, port, clientId);
            var testSuite = new TestSuite(client);

            while (true)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. Send log message");
                Console.WriteLine("2. Run automated tests");
                Console.WriteLine("3. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await SendManualLog(client);
                        break;
                    case "2":
                        await testSuite.RunAllTests();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
        }

        static async Task SendManualLog(LogClient client)
        {
            Console.WriteLine("\nSelect log level:");
            Console.WriteLine("1. DEBUG");
            Console.WriteLine("2. INFO");
            Console.WriteLine("3. WARNING");
            Console.WriteLine("4. ERROR");
            Console.WriteLine("5. FATAL");

            string choice = Console.ReadLine();
            string level = choice switch
            {
                "1" => "DEBUG",
                "2" => "INFO",
                "3" => "WARNING",
                "4" => "ERROR",
                "5" => "FATAL",
                _ => "INFO"
            };

            Console.WriteLine("\nSelect log format:");
            Console.WriteLine("1. text");
            Console.WriteLine("2. json");
            Console.WriteLine("3. csv");
            Console.WriteLine("4. key-value");

            string formatChoice = Console.ReadLine();
            string format = formatChoice switch
            {
                "1" => "text",
                "2" => "json",
                "3" => "csv",
                "4" => "key-value",
                _ => "text"
            };

            Console.Write("Enter log message: ");
            string message = Console.ReadLine();

            bool success = await client.SendLog(level, message, format);
            Console.WriteLine(success ? "Log sent successfully" : "Failed to send log");
        }
    }
}