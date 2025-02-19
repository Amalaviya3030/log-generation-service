using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TestSuite
{
    private readonly LogClient client;

    public TestSuite(LogClient client)
    {
        this.client = client;
    }

    public async Task RunAllTests()
    {
        Console.WriteLine("Running all tests...");

        // Test different log levels
        await TestLogLevels();

        // Test different formats
        await TestFormats();

        // Test invalid inputs
        await TestInvalidInputs();

        Console.WriteLine("All tests completed.");
    }

    private async Task TestLogLevels()
    {
        var levels = new[] { "DEBUG", "INFO", "WARNING", "ERROR", "FATAL" };
        foreach (var level in levels)
        {
            Console.WriteLine($"Testing {level} level...");
            await client.SendLog(level, $"Test message for {level} level");
        }
    }

    public async Task TestFormats()
    {
        var formats = new[] { "w3c", "syslog", "json", "key_value", "csv" };
        foreach (var format in formats)
        {
            Console.WriteLine($"Testing {format} format...");
            await client.SendLog("INFO", $"Test message in {format} format", format, "test.cs", 42);
        }
    }

    private async Task TestInvalidInputs()
    {
        Console.WriteLine("Testing invalid inputs...");
        await client.SendLog("INVALID_LEVEL", "Test message");
        await client.SendLog("INFO", new string('X', 10000));  // Very long message
        await client.SendLog("INFO", "Test message", "invalid_format");
    }

    public async Task TestRateLimiting()
    {
        Console.WriteLine("Testing rate limiting...");
        var tasks = new List<Task>();
        
        for (int i = 0; i < 150; i++)
        {
            tasks.Add(client.SendLog("INFO", $"Rate limit test message {i}"));
            await Task.Delay(10);
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("Rate limit test completed.");
    }
}