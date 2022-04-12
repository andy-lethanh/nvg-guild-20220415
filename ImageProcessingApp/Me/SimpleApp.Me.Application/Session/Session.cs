using Microsoft.AspNetCore.Http;

namespace SimpleApp.Me.Application.Session;

public class Session : ISession
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public Session(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public string GetUserId()
	{
		var userIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim =>
			string.Equals(claim.Type, "username", StringComparison.OrdinalIgnoreCase));
		return userIdClaim!.Value;
	}
}
