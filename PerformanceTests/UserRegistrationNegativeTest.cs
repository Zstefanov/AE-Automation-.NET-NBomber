using AE_extensive_project.PerformanceTests.Models;
using NBomber.Contracts;
using NBomber.CSharp;
using System.Text.RegularExpressions;
using System.Net;

namespace AE_extensive_project.PerformanceTests.PerformanceTests
{
    public static class UserRegistrationNegativeTest
    {
        public static ScenarioProps CreateScenario(List<UserCredentials> registeredUsers)
        {
            var scenario = Scenario.Create("user_registration_negative_already_used_email", async context =>
            {
                // Pick a random registered email
                var user = registeredUsers[Random.Shared.Next(registeredUsers.Count)];
                var name = "NegativeTestUser";
                var email = user.Email;

                using var handler = new HttpClientHandler { UseCookies = true, CookieContainer = new CookieContainer() };
                using var client = new HttpClient(handler);

                // 1. GET signup page to get CSRF cookie and token
                var getSignupResp = await client.GetAsync("https://automationexercise.com/signup");
                var getSignupBody = await getSignupResp.Content.ReadAsStringAsync();

                // Extract csrfmiddlewaretoken from HTML
                var csrfTokenMatch = Regex.Match(getSignupBody, @"name=['""]csrfmiddlewaretoken['""] value=['""]([^'""]+)['""]");
                if (!csrfTokenMatch.Success)
                    return Response.Fail(message: "Failed to extract CSRF token from signup page.");

                var csrfToken = csrfTokenMatch.Groups[1].Value;

                // 2. Build form data as application/x-www-form-urlencoded
                var registrationData = new Dictionary<string, string>
                {
                    { "csrfmiddlewaretoken", csrfToken },
                    { "name", name },
                    { "email", email },
                    { "form_type", "signup" }
                };

                var registerContent = new FormUrlEncodedContent(registrationData);

                // Set Referer header for CSRF
                client.DefaultRequestHeaders.Referrer = new Uri("https://automationexercise.com/signup");

                Console.WriteLine($"Attempting registration with ALREADY USED email: {email}");

                // 3. POST registration with token/cookie
                var registerResponse = await client.PostAsync("https://automationexercise.com/signup", registerContent);
                var registerBody = await registerResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"REGISTER Status: {registerResponse.StatusCode}");

                // Negative test: Expect registration to fail for already-registered email
                if (!registerResponse.IsSuccessStatusCode)
                    return Response.Ok(); // Expected failure

                // Check response body for error message (i.e., "Email Address already exists!")
                if (registerBody.ToLower().Contains("already exists") || registerBody.ToLower().Contains("email address already"))
                    return Response.Ok(); // Expected failure

                return Response.Fail(message: "Registration unexpectedly succeeded for already-used email! Body: " + registerBody);
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(
                Simulation.Inject(
                    rate: 10,
                    interval: TimeSpan.FromSeconds(1),
                    during: TimeSpan.FromSeconds(5)
                )
            );

            return scenario;
        }
    }
}