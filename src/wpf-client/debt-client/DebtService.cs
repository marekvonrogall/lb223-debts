using System.Threading.Tasks;

namespace debt_client
{
    public class DebtService
    {
        private readonly ApiService apiService = new ApiService();

        public async Task<decimal?> GetDebt()
        {
            return await apiService.GetDebt();
        }

        public async Task<string> AddDebt(string input)
        {
            if (!decimal.TryParse(input, out decimal amount) || amount <= 0)
                return "Invalid input. Please enter a number greater than zero.";

            return await apiService.AddDebt(amount);
        }

        public async Task<string> SubtractDebt(string input)
        {
            if (!decimal.TryParse(input, out decimal amount) || amount <= 0)
                return "Invalid input. Please enter a number greater than zero.";

            return await apiService.SubtractDebt(amount);
        }
    }
}
