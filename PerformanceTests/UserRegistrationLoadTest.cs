using NBomber.Contracts;
using NBomber.CSharp;

namespace AE_extensive_project.PerformanceTests.PerformanceTests
{
    public static class UserRegistrationLoadTest
    {
        public static ScenarioProps CreateScenario()
        {
            var scenario = Scenario.Create("user_registration_spike", async context =>
            {
                // Generate unique user details for each registration
                var uniqueId = Guid.NewGuid().ToString("N")[..8];
                var email = $"testuser_{uniqueId}@example.com";

                var registrationData = new Dictionary<string, string>
            {
                {"name", $"Test{uniqueId}"},
                {"email", email},
                {"password", "Password123!"},
                {"title", "Mr"},
                {"birth_date", "1"},
                {"birth_month", "1"},
                {"birth_year", "1990"},
                {"firstname", $"First{uniqueId}"},
                {"lastname", $"Last{uniqueId}"},
                {"company", "FakeCompany"},
                {"address1", "123 Main St"},
                {"address2", "Suite 100"},
                {"country", "United States"},
                {"zipcode", "12345"},
                {"state", "CA"},
                {"city", "Los Angeles"},
                {"mobile_number", "5551112233"}
            };

                using var client = new HttpClient();
                var content = new FormUrlEncodedContent(registrationData);

                Console.WriteLine($"Registering user: {email}");

                var httpResponse = await client.PostAsync("https://automationexercise.com/api/createAccount", content);
                var responseBody = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {httpResponse.StatusCode}, Body: {responseBody}");

                return httpResponse.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            // Spike: 100 users immediately, then hold for 5 seconds
            .WithLoadSimulations(
                Simulation.Inject(
                    rate: 100,
                    interval: TimeSpan.FromSeconds(1),
                    during: TimeSpan.FromSeconds(5)
                )
            );

            return scenario;
        }
    }
}
