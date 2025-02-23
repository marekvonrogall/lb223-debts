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

    [HttpPost("add")]
    public async Task<IActionResult> AddDebt([FromBody] decimal amount)
    {
        if (amount <= 0)
        {
            return BadRequest(new { message = "Amount must be greater than zero" });
        }

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    string query = "UPDATE Debts SET debt = debt + @Amount";
        
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            return Ok(new { message = "Debt increased" });
                        }
                        transaction.Rollback();
                        return NotFound(new { message = "Debt not found" });
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return StatusCode(500, new { message = "Internal server error" });
                }
            }
        }
    }

    [HttpPost("subtract")]
    public async Task<IActionResult> SubtractDebt([FromBody] decimal amount)
    {
        if (amount <= 0)
        {
            return BadRequest(new { message = "Amount must be greater than zero" });
        }

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    string checkQuery = "SELECT TOP 1 debt FROM Debts";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                    {
                        object result = await checkCmd.ExecuteScalarAsync();
                        if (result == null)
                        {
                            transaction.Rollback();
                            return NotFound(new { message = "Debt not found" });
                        }

                        decimal currentDebt = Convert.ToDecimal(result);
                        if (amount > currentDebt)
                        {
                            transaction.Rollback();
                            return BadRequest(new { message = "Deposit exceeds available debt" });
                        }
                    }

                    string query = "UPDATE Debts SET debt = debt - @Amount";
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            return Ok(new { message = "Debt decreased" });
                        }
                        transaction.Rollback();
                        return NotFound(new { message = "Debt not found" });
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return StatusCode(500, new { message = "Internal server error" });
                }
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
