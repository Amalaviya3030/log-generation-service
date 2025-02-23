/*
 * FILE: TestSuite.cs
 * PROJECT: Logging System
 * PROGRAMMER: Aryankumar Malaviya, Smaran Adhikari
 * DATE: 2025-02-23
 */

using System;
using System.Threading.Tasks;

namespace LoggingClient
{
    public class TestSuite
    {
        private readonly LogClient client; // The logging client to test

        public TestSuite(LogClient client)
        {
            this.client = client;
        }

        public async Task RunAllTests()
        {
            Console.WriteLine("Starting automated tests...");

            await TestLogLevels(); // Check if different log levels are handled correctly
            await TestRateLimiting(); // Simulate sending logs quickly to test rate limits
            await TestMessageTypes(); // Test various message formats (long, special characters, etc.)
            await TestErrorCases(); // Check how errors are handled

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
                await Task.Delay(100); // Small delay to avoid overwhelming the server
            }
        }

        private async Task TestRateLimiting()
        {
            Console.WriteLine("\nTesting rate limiting...");

            for (int i = 0; i < 15; i++) // Send multiple logs quickly
            {
                bool success = await client.SendLog("INFO", $"Rate limit test message {i}", "text");
                Console.WriteLine($"Message {i}: {(success ? "Sent" : "Rate limited")}");
            }
        }

        private async Task TestMessageTypes()
        {
            Console.WriteLine("\nTesting message types...");

            // Test a very long message
            string longMessage = new string('X', 1000);
            bool longMsgSuccess = await client.SendLog("INFO", longMessage, "text");
            Console.WriteLine($"Long message test: {(longMsgSuccess ? "Passed" : "Failed")}");

            // Test special characters in a message
            bool specialCharSuccess = await client.SendLog("INFO", "Special chars: !@#$%^&*()_+", "text");
            Console.WriteLine($"Special characters test: {(specialCharSuccess ? "Passed" : "Failed")}");
        }

        private async Task TestErrorCases()
        {
            Console.WriteLine("\nTesting error cases...");

            // Test sending an empty message
            bool emptyMsgSuccess = await client.SendLog("INFO", "", "text");
            Console.WriteLine($"Empty message test: {(emptyMsgSuccess ? "Failed" : "Passed")}");

            // Test an invalid log level
            bool invalidLevelSuccess = await client.SendLog("INVALID_LEVEL", "Test message", "text");
            Console.WriteLine($"Invalid level test: {(invalidLevelSuccess ? "Failed" : "Passed")}");
        }
    }
}