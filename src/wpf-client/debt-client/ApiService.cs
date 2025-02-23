using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace debt_client
{
    public class ApiService
    {
        private readonly HttpClient SharedClient = new HttpClient()
        {
            BaseAddress = new Uri("https://lb223.vrmarek.me/")
        };

        public async Task<decimal?> GetDebt()
        {
            try
            {
                using HttpResponseMessage response = await SharedClient.GetAsync("read");
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var responseDict = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonResponse);
                return responseDict.ContainsKey("debt") ? responseDict["debt"] : null;
            }
            catch
            {
                return null;
            }
        }
        public async Task<string> AddDebt(decimal amount)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(amount), Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await SharedClient.PostAsync("add", content);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var responseDict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
                return responseDict.ContainsKey("message") ? responseDict["message"] : null;
            }
            catch
            {
                return "Could not reach API";
            }
        }

        public async Task<string> SubtractDebt(decimal amount)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(amount), Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await SharedClient.PostAsync("subtract", content);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var responseDict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
                return responseDict.ContainsKey("message") ? responseDict["message"] : null;
            }
            catch
            {
                return "Could not reach API";
            }
        }
    }
}
