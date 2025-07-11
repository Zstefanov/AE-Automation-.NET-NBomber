using AE_extensive_project.PerformanceTests.Helpers;
using AE_extensive_project.PerformanceTests.Models;
using NBomber.Contracts;
using NBomber.CSharp;

namespace AE_extensive_project.PerformanceTests.PerformanceTests
{
    public static class LoginLoadTest
    {
        public static ScenarioProps CreateScenario(List<UserCredentials> users)
        {
            // Login helper class instance and use
            var loginClient = new LoginHelper();

            var scenario = Scenario.Create("login_load", async context =>
            {
                // Select a random user for each request
                var user = users[Random.Shared.Next(users.Count)];
                using var client = new HttpClient();
                Console.WriteLine($"Trying to login with {user.Email}");

                var success = await loginClient.LoginAsync(user, client);

                return success ? Response.Ok() : Response.Fail();
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(10))
            // 10 simultaneous users for a 30 second duration
            .WithLoadSimulations(Simulation.KeepConstant(5, TimeSpan.FromSeconds(20)));

            return scenario;
        }
    }
}