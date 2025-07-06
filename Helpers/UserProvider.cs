using AE_extensive_project.PerformanceTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AE_extensive_project.PerformanceTests.Helpers
{
    public static class UserProvider
    {
        // This class provides a static list of user credentials loaded from a JSON file - loaded once at startup in Program.cs
        private static readonly List<UserCredentials> users = LoadUsers();

        public static IReadOnlyList<UserCredentials> Users => users;

        private static List<UserCredentials> LoadUsers()
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "users.json");
            var json = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<List<UserCredentials>>(json);
        }
    }
}
