using Microsoft.AspNetCore.Mvc;

namespace ForTestingWeb.Controllers;

[ApiController]
[Route("v1.0/me")]
public class MeController : ControllerBase
{
	[HttpGet("avatar")]
	public Task<IActionResult> Avatar()
	{
		return Task.FromResult(Ok("Haha") as IActionResult);
	}
}
