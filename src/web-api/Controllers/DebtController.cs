using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace web_api.Controllers;

[ApiController]
[Route("[controller]")]
public class DebtController : ControllerBase
{
    private string ConnectionString => $"Server=lb223.vrmarek.me,1433; Database=lb223; User Id=sa; Password={Environment.GetEnvironmentVariable("SA_PASSWORD")};";
    
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        return Ok(new
        {
            message = "OK"
        });
    }

    [HttpPut("add")]
    public async Task<IActionResult> AddDebt([FromBody] decimal amount)
    {
        if (amount <= 0)
        {
            return BadRequest(new { message = "Amount must be greater than zero" });
        }

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
        
            string query = "UPDATE Debts SET debt = debt + @Amount WHERE (SELECT MIN(debt) FROM Debts) IS NOT NULL";
        
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Amount", amount);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Debt increased" });
                }
                return NotFound(new { message = "Debt not found" });
            }
        }
    }

    [HttpPut("subtract")]
    public async Task<IActionResult> SubtractDebt([FromBody] decimal amount)
    {
        if (amount <= 0)
        {
            return BadRequest(new { message = "Amount must be greater than zero" });
        }

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            string query = @"
                UPDATE Debts 
                SET debt = CASE 
                    WHEN debt - @Amount < 0 THEN 0 
                    ELSE debt - @Amount 
                END
                WHERE (SELECT MIN(debt) FROM Debts) IS NOT NULL";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Amount", amount);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Debt decreased" });
                }
                return NotFound(new { message = "Debt not found" });
            }
        }
    }

    [HttpGet("read")]
    public async Task<IActionResult> ReadDebt()
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
            string query = "SELECT TOP 1 debt FROM Debts";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                object result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    return Ok(new { debt = result });
                }
                return NotFound(new { message = "Debt not found" });
            }
        }
    }
}
