using NBomber.Contracts;
using NBomber.CSharp;

namespace AE_extensive_project.PerformanceTests.PerformanceTests
{
    public static class ProductSearchLoadTest
    {
        // List of popular search terms for simulation
        private static readonly string[] SearchTerms =
        [
        "shirt", "jeans", "dress", "shoes", "jacket", "t-shirt", "pants", "skirt", "blouse", "coat"
        ];

        public static ScenarioProps CreateScenario()
        {
            var scenario = Scenario.Create("product_search_high_volume", async context =>
            {
                // Pick a random search term for this request
                var searchTerm = SearchTerms[Random.Shared.Next(SearchTerms.Length)];

                var searchData = new Dictionary<string, string>
            {
                { "search_product", searchTerm }
            };

                using var client = new HttpClient();
                var content = new FormUrlEncodedContent(searchData);

                Console.WriteLine($"Searching for product: {searchTerm}");

                var httpResponse = await client.PostAsync("https://automationexercise.com/api/searchProduct", content);
                var responseBody = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {httpResponse.StatusCode}, Body: {responseBody}");

                // Consider 200 status code as success
                return httpResponse.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            // Simulate 20 users searching simultaneously for 30 seconds
            .WithLoadSimulations(
                Simulation.KeepConstant(20, TimeSpan.FromSeconds(30)) 
            );

            return scenario;
        }
    }
}
