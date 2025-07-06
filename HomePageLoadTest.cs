using NBomber.Contracts.Stats;
using NBomber.CSharp;


namespace AE_extensive_project.PerformanceTests
{
    public class HomePageLoadTest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("NBomber test is starting...");

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
            //10 seconds warm-up period for stability and cache warming
            .WithWarmUpDuration(TimeSpan.FromSeconds(10))
            //10 simultaneous users for a 30 second duration
            .WithLoadSimulations(Simulation.KeepConstant(10, TimeSpan.FromSeconds(30)));

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFolder(@"..\..\..\bomber_reports")
                .WithReportFormats(ReportFormat.Txt, ReportFormat.Csv, ReportFormat.Html)
                .Run();

            Console.WriteLine("NBomber test finished.");
        }
    }
}