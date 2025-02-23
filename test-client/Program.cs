/*
 * FILE: Program.cs
 * PROJECT: Logging System
 * PROGRAMMER: Aryankumar Malaviya, Smaran Adhikari
 * DATE: 2025-02-23
 */

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LoggingClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load configuration from appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            Console.WriteLine("Logging Client");

            // Get default host from config and allow user input
            string defaultHost = config["LogServer:Host"];
            Console.Write($"Enter logging server IP address (default: {defaultHost}): ");
            string host = Console.ReadLine();
            host = string.IsNullOrEmpty(host) ? defaultHost : host;

            // Get default port from config and allow user input
            int defaultPort = int.Parse(config["LogServer:Port"]);
            Console.Write($"Enter server port (default: {defaultPort}): ");
            string portStr = Console.ReadLine();
            int port = string.IsNullOrEmpty(portStr) ? defaultPort : int.Parse(portStr);

            // Ask for client identifier
            Console.Write("Enter client identifier: ");
            string clientId = Console.ReadLine();

            // Initialize log client and test suite
            var client = new LogClient(host, port, clientId);
            var testSuite = new TestSuite(client);

            while (true)
            {
                // Display menu options
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. Send log message");
                Console.WriteLine("2. Run automated tests");
                Console.WriteLine("3. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await SendManualLog(client); // Manually send a log message
                        break;
                    case "2":
                        await testSuite.RunAllTests(); // Run all test cases
                        break;
                    case "3":
                        return; // Exit program
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
        }

        static async Task SendManualLog(LogClient client)
        {
            // Select log level
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
                _ => "INFO" // Default to INFO if input is invalid
            };

            // Select log format
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
                _ => "text" // Default to text if input is invalid
            };

            // Get log message from user
            Console.Write("Enter log message: ");
            string message = Console.ReadLine();

            // Send the log and display the result
            bool success = await client.SendLog(level, message, format);
            Console.WriteLine(success ? "Log sent successfully" : "Failed to send log");
        }
    }
}
