using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;

namespace AE_extensive_project.PerformanceTests.PerformanceTests
{
    public class GetProductEndpointLoadTest
    {
        public static ScenarioProps CreateScenario()
        {
            var baseUrl = "https://automationexercise.com";

            var scenario = Scenario.Create("critical_products_list", async context =>
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{baseUrl}/products");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Fetched products list successfully.");
                    return Response.Ok();
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to fetch products. Status: {response.StatusCode}, Body: {body}");
                    return Response.Fail(message: $"Failed to fetch products. Status: {response.StatusCode}, Body: {body}");
                }
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(// 5 users, 20 seconds
                Simulation.KeepConstant(5, TimeSpan.FromSeconds(20))
            );
            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFolder(@"..\..\..\bomber_reports")
                .WithReportFormats(ReportFormat.Txt, ReportFormat.Html)
                .Run();

            return scenario;
        }
    }
}
