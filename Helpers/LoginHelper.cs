using AE_extensive_project.PerformanceTests.Models;


namespace AE_extensive_project.PerformanceTests.Helpers
{
    public class LoginHelper
    {
        private readonly string _loginUrl = "https://automationexercise.com/api/verifyLogin";

        public async Task<bool> LoginAsync(UserCredentials user, HttpClient client)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", user.Email),
                new KeyValuePair<string, string>("password", user.Password)
            });

            var response = await client.PostAsync(_loginUrl, content);
            return response.IsSuccessStatusCode;
        }
    }
}