using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;

namespace AE_extensive_project.PerformanceTests.PerformanceTests
{
    public static class HomePageLoadTest
    {
        public static ScenarioProps CreateScenario()
        {
            var scenario = Scenario.Create("home_page_load", async context =>
            {
                var response = await Step.Run("fetch_homepage", context, async () =>
                {
                    using var client = new HttpClient();
                    var httpResponse = await client.GetAsync("https://automationexercise.com");
                    Console.WriteLine($"HTTP status: {httpResponse.StatusCode}");
                    return httpResponse.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
                });

                return Response.Ok();
            })
            // 10 seconds warm-up period for stability and cache warming
            .WithWarmUpDuration(TimeSpan.FromSeconds(10))
            // 10 simultaneous users for a 30 second duration
            .WithLoadSimulations(Simulation.KeepConstant(10, TimeSpan.FromSeconds(30)));

            return scenario;
        }
    }
}