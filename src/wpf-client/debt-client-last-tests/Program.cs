using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace debt_client_last_tests
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var random = new Random();

            // Read Scenario
            var readScenario = Scenario.Create("read_scenario", async context =>
            {
                var request = Http.CreateRequest("GET", "https://lb223.vrmarek.me/read")
                    .WithHeader("Accept", "application/json");

                var response = await Http.Send(httpClient, request);

                return response;
            })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

            // Add Scenario
            var addScenario = Scenario.Create("add_scenario", async context =>
            {
                int amount = random.Next(1, 1000);
                var jsonContent = new StringContent(JsonSerializer.Serialize(amount), Encoding.UTF8, "application/json");

                var addRequest = Http.CreateRequest("POST", "https://lb223.vrmarek.me/add")
                    .WithHeader("Accept", "application/json")
                    .WithBody(jsonContent);

                var response = await Http.Send(httpClient, addRequest);

                return response;
            })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

            // Subtract Scenario
            var subtractScenario = Scenario.Create("subtract_scenario", async context =>
            {
                int amount = random.Next(1, 1000);
                var jsonContent = new StringContent(JsonSerializer.Serialize(amount), Encoding.UTF8, "application/json");

                var subtractRequest = Http.CreateRequest("POST", "https://lb223.vrmarek.me/subtract")
                    .WithHeader("Accept", "application/json")
                    .WithBody(jsonContent);

                var response = await Http.Send(httpClient, subtractRequest);

                return response;
            })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

            // Run all scenarios
            NBomberRunner
                .RegisterScenarios(readScenario, addScenario, subtractScenario)
                .WithReportFileName("read_add_subtract_report")
                .WithReportFolder("read_add_subtract_reports")
                .WithReportFormats(ReportFormat.Html)
                .Run();

            Console.WriteLine("Press any key");
        }
    }
}
