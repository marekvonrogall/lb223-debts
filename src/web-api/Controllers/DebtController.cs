using Microsoft.AspNetCore.Mvc;

namespace web_api.Controllers;

[ApiController]
[Route("[controller]")]
public class DebtController : ControllerBase
{
    [HttpPost(Name = "update")]
    public async Task<IActionResult> ModifyDebt()
    {
        //Modify debt in database
        return Ok(new
        {
            message = "Debt modified"
        });
    }

    [HttpGet(Name = "read")]
    public async Task<IActionResult> ReadDebt()
    {
        // Read debt from database
        return Ok(new
        {
            debt = "placeholder"
        });
    }
}
