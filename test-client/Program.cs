global using System.IO;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string clientId = args.Length > 0 ? args[0] : "TestClient1";
        Console.WriteLine($"Running as client: {clientId}");

        var logClient = new LogClient();
        var testSuite = new TestSuite(logClient);

        while (true)
        {
            Console.WriteLine($"\nLogging Service Test Client ({clientId})");
            Console.WriteLine("1. Send Manual Log Message");
            Console.WriteLine("2. Run Automated Tests");
            Console.WriteLine("3. Test Rate Limiting");
            Console.WriteLine("4. Test All Log Formats");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await SendManualLog(logClient);
                    break;
                case "2":
                    await testSuite.RunAllTests();
                    break;
                case "3":
                    await testSuite.TestRateLimiting();
                    break;
                case "4":
                    await testSuite.TestFormats();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    static async Task SendManualLog(LogClient client)
    {
        Console.Write("Enter log level (DEBUG/INFO/WARNING/ERROR/FATAL): ");
        var level = Console.ReadLine()?.ToUpper() ?? "INFO";

        Console.Write("Enter message: ");
        var message = Console.ReadLine() ?? "Test message";

        Console.Write("Enter format (text/json/key_value/csv): ");
        var format = Console.ReadLine()?.ToLower() ?? "text";

        await client.SendLog(level, message, format);
    }
}