using NBomber.Contracts;
using NBomber.CSharp;
using System.Net;
using System.Text.Json;

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
                var password = "Password123!";

                // Hardcode repeatable data
                var registrationData = new Dictionary<string, string>
                {
                    {"name", $"Test{uniqueId}"},
                    {"email", email},
                    {"password", password},
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

                using var handler = new HttpClientHandler { CookieContainer = new CookieContainer(), UseCookies = true };
                using var client = new HttpClient(handler);

                Console.WriteLine($"Registering user: {email}");

                // 1. Register the user
                var registerContent = new FormUrlEncodedContent(registrationData);
                var registerResponse = await client.PostAsync("https://automationexercise.com/api/createAccount", registerContent);
                var registerBody = await registerResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"REGISTER Status: {registerResponse.StatusCode}, Body: {registerBody}");

                if (!registerResponse.IsSuccessStatusCode)
                    return Response.Fail(message: "Registration failed");

                // 2. Delete the user account (API: DELETE, email+password)
                var deleteForm = new Dictionary<string, string>
                {
                    { "email", email },
                    { "password", password }
                };
                var deleteContent = new FormUrlEncodedContent(deleteForm);

                var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "https://automationexercise.com/api/deleteAccount")
                {
                    Content = deleteContent
                };
                var deleteResponse = await client.SendAsync(deleteRequest);
                var deleteBody = await deleteResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"DELETE Status: {deleteResponse.StatusCode}, Body: {deleteBody}");

                // Check for 200 code and expected message
                try
                {
                    using var doc = JsonDocument.Parse(deleteBody);
                    var root = doc.RootElement;
                    var responseCode = root.GetProperty("responseCode").GetInt32();
                    var message = root.GetProperty("message").GetString();

                    if (deleteResponse.StatusCode == HttpStatusCode.OK &&
                        responseCode == 200 &&
                        !string.IsNullOrWhiteSpace(message) &&
                        message.ToLower().Contains("account deleted"))
                    {
                        return Response.Ok();
                    }
                    else
                    {
                        return Response.Fail(message: $"User deleted API call failed. Message: {message}");
                    }
                }
                catch
                {
                    return Response.Fail(message: "User deleted API call failed. Bad response JSON.");
                }
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            // Spike: 20 users immediately, then hold for 5 seconds
            .WithLoadSimulations(
                Simulation.Inject(
                    rate: 20,
                    interval: TimeSpan.FromSeconds(1),
                    during: TimeSpan.FromSeconds(5)
                )
            );

            return scenario;
        }
    }
}