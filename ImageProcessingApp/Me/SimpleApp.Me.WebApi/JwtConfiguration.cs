namespace SimpleApp.Me.WebApi;

public class JwtConfiguration
{
	public string Issuer { get; set; } = null!;
	public string KeySetUrl { get; set; } = null!;
}
