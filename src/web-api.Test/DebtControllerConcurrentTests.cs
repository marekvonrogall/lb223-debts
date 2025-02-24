using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;
using web_api.Controllers;

namespace web_api.Test
{
    public class DebtControllerConcurrentTests
    {
        private readonly ITestOutputHelper _output;

        public DebtControllerConcurrentTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // Funktion von ChatGPT, die meine Code länge erheblich reduziert, da jeder Testfall aus der Datenbank lesen muss
        // Helper method to read and assert the debt value.
        private async Task<decimal> ReadDebtAsync(DebtController controller)
        {
            // ACT: Call ReadDebt.
            IActionResult result = await controller.ReadDebt();

            // ASSERT: Validate the result type and extract the 'debt' property.
            var okResult = Assert.IsType<OkObjectResult>(result);
            var debtProp = okResult.Value?.GetType().GetProperty("debt");
            Assert.NotNull(debtProp);
            object? debtValue = debtProp.GetValue(okResult.Value);
            Assert.NotNull(debtValue);

            return Convert.ToDecimal(debtValue);
        }

        [Fact]
        public async Task ReadDebtTest()
        {
            // ARRANGE
            const int TASK_COUNT = 50;
            var controller = new DebtController();
            Task[] tasks = new Task[TASK_COUNT];

            // ACT
            for (int i = 0; i < TASK_COUNT; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    decimal debt = await ReadDebtAsync(controller);
                });
            }
            await Task.WhenAll(tasks);
        }

        [Fact]
        public async Task AddDebtTest()
        {
            // ARRANGE
            const int TASK_COUNT = 50;
            decimal addAmount = 1;
            var controller = new DebtController();
            decimal startingDebt = await ReadDebtAsync(controller);
            Task[] tasks = new Task[TASK_COUNT];

            // ACT
            for (int i = 0; i < TASK_COUNT; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    IActionResult result = await controller.AddDebt(addAmount);

                    // ASSERT
                    var okResult = Assert.IsType<OkObjectResult>(result);
                    var messageProp = okResult.Value?.GetType().GetProperty("message");
                    Assert.NotNull(messageProp);
                    string message = messageProp.GetValue(okResult.Value)?.ToString()!;
                    Assert.Equal("Debt increased", message);
                });
            }
            await Task.WhenAll(tasks);

            // ASSERT
            decimal finalDebt = await ReadDebtAsync(controller);
            Assert.Equal(startingDebt + TASK_COUNT * addAmount, finalDebt);
        }

        [Fact]
        public async Task SubtractDebtTest()
        {
            // ARRANGE
            const int TASK_COUNT = 50;
            decimal subtractAmount = 1;
            var controller = new DebtController();
            decimal currentDebt = await ReadDebtAsync(controller);

            if (currentDebt < TASK_COUNT * subtractAmount)
            {
                decimal needed = TASK_COUNT * subtractAmount - currentDebt;
                var addResult = await controller.AddDebt(needed);
                Assert.IsType<OkObjectResult>(addResult);
                currentDebt = await ReadDebtAsync(controller);
            }

            Task[] tasks = new Task[TASK_COUNT];

            // ACT
            for (int i = 0; i < TASK_COUNT; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    IActionResult result = await controller.SubtractDebt(subtractAmount);

                    // ASSERT
                    var okResult = Assert.IsType<OkObjectResult>(result);
                    var messageProp = okResult.Value?.GetType().GetProperty("message");
                    Assert.NotNull(messageProp);
                    string message = messageProp.GetValue(okResult.Value)?.ToString()!;
                    Assert.Equal("Debt decreased", message);
                });
            }
            await Task.WhenAll(tasks);

            // ASSERT
            decimal finalDebt = await ReadDebtAsync(controller);
            Assert.Equal(currentDebt - TASK_COUNT * subtractAmount, finalDebt);
        }
    }
}
