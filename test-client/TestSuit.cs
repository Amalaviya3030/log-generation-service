// TestSuite.cs
using System;
using System.Threading.Tasks;

namespace LoggingClient
{
    public class TestSuite
    {
        private readonly LogClient client;

        public TestSuite(LogClient client)
        {
            this.client = client;
        }

        public async Task RunAllTests()
        {
            Console.WriteLine("Starting automated tests...");
            
            await TestLogLevels();
            await TestRateLimiting();
            await TestMessageTypes();
            await TestErrorCases();

            Console.WriteLine("All tests completed.");
        }

        private async Task TestLogLevels()
        {
            Console.WriteLine("\nTesting log levels...");
            string[] levels = { "DEBUG", "INFO", "WARNING", "ERROR", "FATAL" };
            foreach (var level in levels)
            {
                bool success = await client.SendLog(level, $"Test message for {level} level", "text");
                Console.WriteLine($"{level} level test: {(success ? "Passed" : "Failed")}");
                await Task.Delay(100);
            }
        }

        private async Task TestRateLimiting()
        {
            Console.WriteLine("\nTesting rate limiting...");
            for (int i = 0; i < 15; i++)
            {
                bool success = await client.SendLog("INFO", $"Rate limit test message {i}", "text");
                Console.WriteLine($"Message {i}: {(success ? "Sent" : "Rate limited")}");
            }
        }

        private async Task TestMessageTypes()
        {
            Console.WriteLine("\nTesting message types...");
            
            // Test long message
            string longMessage = new string('X', 1000);
            bool longMsgSuccess = await client.SendLog("INFO", longMessage, "text");
            Console.WriteLine($"Long message test: {(longMsgSuccess ? "Passed" : "Failed")}");

            // Test special characters
            bool specialCharSuccess = await client.SendLog("INFO", "Special chars: !@#$%^&*()_+", "text");
            Console.WriteLine($"Special characters test: {(specialCharSuccess ? "Passed" : "Failed")}");
        }

        private async Task TestErrorCases()
        {
            Console.WriteLine("\nTesting error cases...");
            
            // Test empty message
            bool emptyMsgSuccess = await client.SendLog("INFO", "", "text");
            Console.WriteLine($"Empty message test: {(emptyMsgSuccess ? "Failed" : "Passed")}");

            // Test invalid log level
            bool invalidLevelSuccess = await client.SendLog("INVALID_LEVEL", "Test message", "text");
            Console.WriteLine($"Invalid level test: {(invalidLevelSuccess ? "Failed" : "Passed")}");
        }
    }
}