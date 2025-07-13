using AE_extensive_project.PerformanceTests.Helpers;
using AE_extensive_project.PerformanceTests.Models;
using NBomber.Contracts;
using NBomber.CSharp;

namespace AE_extensive_project.PerformanceTests.PerformanceTests
{
    public static class LoginNegativeTest
    {
        public static ScenarioProps CreateScenario()
        {
            var loginClient = new LoginHelper();

            // Example: list of deliberately invalid credentials
            var invalidUsers = new List<UserCredentials>
            {
                new UserCredentials { Email = "nonexistent_user@example.com", Password = "wrongpassword" },
                new UserCredentials { Email = "test@example.com", Password = "incorrect" },
                new UserCredentials { Email = "invalidemail", Password = "123456" },
                new UserCredentials { Email = "", Password = "" }, // empty email
                new UserCredentials { Email = "user@example.com", Password = "" }, // missing password
            };

            var scenario = Scenario.Create("login_negative", async context =>
            {
                // Pick a random invalid user for each request
                var user = invalidUsers[Random.Shared.Next(invalidUsers.Count)];
                using var client = new HttpClient();
                Console.WriteLine($"Trying to login with INVALID credentials: {user.Email}");

                var success = await loginClient.LoginAsync(user, client);

                // This is a negative test: we expect login to FAIL!
                return !success ? Response.Ok() : Response.Fail(message: "Login unexpectedly succeeded with invalid credentials");
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.KeepConstant(5, TimeSpan.FromSeconds(15)));

            return scenario;
        }
    }
}