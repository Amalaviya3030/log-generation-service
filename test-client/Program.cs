using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var logClient = new LogClient();
        var testSuite = new TestSuite(logClient);

        while (true)
        {
            Console.WriteLine("\nLogging Service Test Client");
            Console.WriteLine("1. Send Manual Log Message");
            Console.WriteLine("2. Run Automated Tests");
            Console.WriteLine("3. Test Rate Limiting");
            Console.WriteLine("4. Exit");
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
                    return;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    static async Task SendManualLog(LogClient client)
    {
        Console.Write("Enter log level (INFO/WARNING/ERROR): ");
        var level = Console.ReadLine()?.ToUpper() ?? "INFO";

        Console.Write("Enter message: ");
        var message = Console.ReadLine() ?? "Test message";

        Console.Write("Enter format (default/detailed/minimal): ");
        var format = Console.ReadLine()?.ToLower() ?? "default";

        await client.SendLog(level, message, format);
    }
}