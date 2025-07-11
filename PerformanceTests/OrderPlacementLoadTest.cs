﻿using AE_extensive_project.PerformanceTests.Helpers;
using AE_extensive_project.PerformanceTests.Models;
using NBomber.Contracts;
using NBomber.CSharp;
using System.Net;
using System.Text.RegularExpressions;

namespace AE_extensive_project.PerformanceTests.PerformanceTests
{
    public static class OrderPlacementLoadTest
    {
        public static ScenarioProps CreateScenario(List<UserCredentials> users)
        {
            // Initialize the login helper
            var loginHelper = new LoginHelper();

            var scenario = Scenario.Create("checkout_order_placement", async context =>
            {
                // Pick a random user for this order simulation
                var user = users[Random.Shared.Next(users.Count)];

                // Maintain cookies for session continuity
                var handler = new HttpClientHandler() { CookieContainer = new CookieContainer(), UseCookies = true };
                using var client = new HttpClient(handler);

                var baseUrl = "https://automationexercise.com";

                // 1. NAVIGATE TO HOME (GET)
                await client.GetAsync(baseUrl);

                // 2. LOGIN via LoginHelper (POST)
                var loginSuccess = await loginHelper.LoginAsync(user, client);
                if (loginSuccess)
                {
                    Console.WriteLine($"Login Success: {user.Email}");
                }
                else
                {
                    Console.WriteLine($"Login Failed: {user.Email}");
                    return Response.Fail(message: "Login failed.");
                }

                // 3. ADD PRODUCT TO CART (GET)
                var productId = Random.Shared.Next(1, 5); // Hardcoded product IDs from 1 to 5

                var addToCartResponse = await client.GetAsync($"{baseUrl}/add_to_cart/{productId}");
                if (addToCartResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Successfully added product to cart.");
                }
                else
                {
                    return Response.Fail(message: "Add to cart failed.");
                }

                // 4. VIEW CART (GET)
                var viewCartResponse = await client.GetAsync($"{baseUrl}/view_cart");
                if (!viewCartResponse.IsSuccessStatusCode)
                    return Response.Fail(message: "View cart failed.");

                // 5. CHECKOUT (GET)
                var checkoutResponse = await client.GetAsync($"{baseUrl}/checkout");
                if (!checkoutResponse.IsSuccessStatusCode)
                    return Response.Fail(message: "Checkout failed.");

                // 6. PAYMENT PAGE (GET, to fetch CSRF token)
                var paymentGetResponse = await client.GetAsync($"{baseUrl}/payment");
                if (!paymentGetResponse.IsSuccessStatusCode)
                    return Response.Fail(message: "Payment page load failed.");

                var paymentHtml = await paymentGetResponse.Content.ReadAsStringAsync();
                var csrfMatch = Regex.Match(paymentHtml, @"name=['""]csrfmiddlewaretoken['""] value=['""]([^'""]+)");
                if (!csrfMatch.Success)
                    return Response.Fail(message: "CSRF token not found.");


                var csrfToken = csrfMatch.Groups[1].Value;

                // 7. PLACE ORDER (POST /payment)
                var paymentData = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"csrfmiddlewaretoken", csrfToken},
                    {"name_on_card", "Test User"},
                    {"card_number", "1234 1234 1234 1234"},
                    {"cvc", "123"},
                    {"expiry_month", "12"},
                    {"expiry_year", "2030"}
                });

                var paymentRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/payment")
                {
                    Content = paymentData
                };
                paymentRequest.Headers.Referrer = new Uri($"{baseUrl}/payment");

                var paymentPostResponse = await client.SendAsync(paymentRequest);
                var paymentPostBody = await paymentPostResponse.Content.ReadAsStringAsync();

                bool orderConfirmed = paymentPostBody.Contains("Congratulations! Your order has been confirmed!");

                return paymentPostResponse.IsSuccessStatusCode && orderConfirmed
                    ? Response.Ok()
                    : Response.Fail(message: "Order not confirmed or HTTP error");
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(
                Simulation.KeepConstant(3, TimeSpan.FromSeconds(10))
            );
            return scenario;
        }
    }
}