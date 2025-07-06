using AE_extensive_project.PerformanceTests.Models;
using NBomber.Contracts;
using NBomber.CSharp;
using System.Collections.Generic;

public static class LoginLoadTest
{
    // Remove internal loading of users; rely on passed-in users
    public static ScenarioProps CreateScenario(List<UserCredentials> users)
    {
        var scenario = Scenario.Create("login_load", async context =>
        {
            // Select a random user for each request
            var user = users[Random.Shared.Next(users.Count)];
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", user.Email),
                new KeyValuePair<string, string>("password", user.Password)
            });
            Console.WriteLine($"Trying to login with {user.Email}");
            var httpResponse = await client.PostAsync("https://automationexercise.com/api/verifyLogin", content);

            return httpResponse.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(10))
        // 10 simultaneous users for a 30 second duration
        .WithLoadSimulations(Simulation.KeepConstant(10, TimeSpan.FromSeconds(30)));

        return scenario;
    }
}