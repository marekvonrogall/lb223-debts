using System;
using System.Collections.Generic;
using System.Net.Http;
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
            using HttpResponseMessage response = await SharedClient.GetAsync("read");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var responseDict = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonResponse);
                return responseDict.ContainsKey("debt") ? responseDict["debt"] : null;
            }
            return null;
        }
    }
}
