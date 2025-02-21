using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace web_api.Controllers;

[ApiController]
[Route("[controller]")]
public class DebtController : ControllerBase
{
    private string ConnectionString => $"Server=lb223.vrmarek.me,1433; Database=lb223; User Id=sa; Password={Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")};";
    
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        return Ok(new
        {
            message = "OK"
        });
    }

    [HttpPut("update")]
    public async Task<IActionResult> ModifyDebt([FromBody] decimal newDebt)
    {
        if (newDebt < 0)
        {
            return BadRequest(new { message = "Invalid input" });
        }

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
            string query = "UPDATE Debts SET debt = @NewDebt WHERE (SELECT MIN(debt) FROM Debts) IS NOT NULL";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@NewDebt", newDebt);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Debt modified" });
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
