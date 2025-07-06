using AE_extensive_project.PerformanceTests.Models;
using AE_extensive_project.PerformanceTests.PerformanceTests;
using NBomber.CSharp;
using System.Collections.Generic; // For List<T>
using System.IO; // Make sure to include for File/Path
using System.Text.Json;

namespace AE_extensive_project.PerformanceTests
{
    public class Program
    {
        public static void Main()
        {
            var users = LoadUsers();

            // Print file content BEFORE deserialization
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "users.json");
            var json = File.ReadAllText(jsonPath);
            Console.WriteLine($"File content: {json}");

            // Print deserialized objects
            foreach (var user in users)
                Console.WriteLine($"- Email: '{user.Email ?? "null"}', Password: '{user.Password ?? "null"}'");

            Console.WriteLine($"Loaded {users.Count} users:");
            

            // Or, run all scenarios in the future:
            var scenarios = new[] {
                //disable homepageload tes for now - it works
                //HomePageLoadTest.CreateScenario(),
                //LoginLoadTest.CreateScenario(users),
                UserRegistrationLoadTest.CreateScenario()
            };

            NBomberRunner
                .RegisterScenarios(scenarios)
                .Run();
        }

        // Load users from JSON file in Data folder once here, to be used in all tests
        private static List<UserCredentials> LoadUsers()
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "users.json");
            var json = File.ReadAllText(jsonPath);

            JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };
            var options = jsonSerializerOptions;

            return JsonSerializer.Deserialize<List<UserCredentials>>(json, options) ?? []; //new List<UserCredentials>();
        }
    }
}