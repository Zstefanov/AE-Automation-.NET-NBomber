using AE_extensive_project.PerformanceTests.Models;
using NBomber.Contracts;
using NBomber.CSharp;
using System.Net;

namespace AE_extensive_project.PerformanceTests.PerformanceTests
{
    public static class CartOperationsLoadTest
    {
        public static ScenarioProps CreateScenario(List<UserCredentials> users)
        {
            var baseUrl = "https://automationexercise.com";
            var loginHelper = new Helpers.LoginHelper();

            var scenario = Scenario.Create("cart_operations", async context =>
            { 
                // Retrieve a random user for each request
                var user = users[Random.Shared.Next(users.Count)];
                var handler = new HttpClientHandler() { CookieContainer = new CookieContainer(), UseCookies = true };
                using var client = new HttpClient(handler);

                var baseUrl = "https://automationexercise.com";

                // 1. LOGIN via Login Helper
                var loginSuccess = await loginHelper.LoginAsync(user, client);

                if (!loginSuccess)
                {
                    Console.WriteLine($"LOGIN FAILED: {user.Email}");
                    return Response.Fail(message: $"Login failed for user {user.Email}");
                }
                Console.WriteLine($"LOGIN SUCCESS: {user.Email}");

                // 2. ADD 2-3 RANDOM PRODUCTS TO CART
                var addedProducts = new List<int>();
                int numToAdd = Random.Shared.Next(2, 4); // 2 or 3 products
                for (int i = 0; i < numToAdd; i++)
                {
                    int productId = Random.Shared.Next(1, 5);
                    var addResp = await client.GetAsync($"{baseUrl}/add_to_cart/{productId}");
                    if (addResp.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Added product {productId} to cart.");
                        addedProducts.Add(productId);
                    }
                    else
                    {
                        // Logging for debugging
                        var body = await addResp.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to add product {productId} to cart. Status: {addResp.StatusCode}, Body: {body}");
                        return Response.Fail(message: $"Failed to add product {productId} to cart. Status: {addResp.StatusCode}, Body: {body}");
                        
                    }
                }

                // 3. VIEW CART
                var viewCartResp = await client.GetAsync($"{baseUrl}/view_cart");
                if (!viewCartResp.IsSuccessStatusCode)
                {
                    // Logging for debugging
                    var body = await viewCartResp.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to view cart after adding products. Status: {viewCartResp.StatusCode}, Body: {body}");
                    return Response.Fail(message: $"Failed to view cart after adding products. Status: {viewCartResp.StatusCode}, Body: {body}");
                    
                }
                Console.WriteLine("Viewed cart after adding products.");

                // 4. REMOVE 1 PRODUCT FROM CART
                if (addedProducts.Count > 0)
                {
                    int removeIndex = Random.Shared.Next(addedProducts.Count);
                    int productIdToRemove = addedProducts[removeIndex];
                    var removeResp = await client.GetAsync($"{baseUrl}/delete_cart/{productIdToRemove}");
                    if (removeResp.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Removed product {productIdToRemove} from cart.");
                    }
                    else
                    {
                        // Logging for debugging
                        var body = await removeResp.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to remove product {productIdToRemove} from cart. Status: {removeResp.StatusCode}, Body: {body}");
                        return Response.Fail(message: $"Failed to remove product {productIdToRemove} from cart. Status: {removeResp.StatusCode}, Body: {body}");
                        
                    }
                }

                // 5. VIEW CART AGAIN
                var viewCartAgainResp = await client.GetAsync($"{baseUrl}/view_cart");
                if (!viewCartAgainResp.IsSuccessStatusCode)
                {
                    // Logging for debugging
                    var body = await viewCartAgainResp.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to view cart after removing product. Status: {viewCartAgainResp.StatusCode}, Body: {body}");
                    return Response.Fail(message: $"Failed to view cart after removing product. Status: {viewCartAgainResp.StatusCode}, Body: {body}");
                    
                }
                Console.WriteLine("Viewed cart after removing a product.");

                return Response.Ok();
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(
                Simulation.KeepConstant(4, TimeSpan.FromSeconds(20))
            );

            // Extracted to the program.cs class to generate a report for all scenarios
            //NBomberRunner
            //    .RegisterScenarios(scenario)
            //    .WithReportFolder(@"..\..\..\bomber_reports")
            //    .WithReportFormats(ReportFormat.Txt, ReportFormat.Html)
            //    .WithReportFileName("cart_operations_report")
            //    .Run();

            return scenario;
        }
    }
}