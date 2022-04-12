using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SimpleApp.Me.WebApi;

public static class JwtConfigurations
{
	public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
	{
		var jwtConfiguration = new JwtConfiguration();
		configuration.GetSection("Jwt").Bind(jwtConfiguration);
		var httpClient = new HttpClient();
		var content = httpClient.GetStringAsync(jwtConfiguration.KeySetUrl)
			.ConfigureAwait(false)
			.GetAwaiter()
			.GetResult();
		var keySet = JsonWebKeySet.Create(content);
		return new TokenValidationParameters
		{
			IssuerSigningKeys = keySet.Keys,
			RequireExpirationTime = true,
			SaveSigninToken = true,
			TryAllIssuerSigningKeys = true,
			ValidIssuer = jwtConfiguration.Issuer,
			ValidateIssuer = true,
			ValidateIssuerSigningKey = true,
			ValidateLifetime = true,
			ValidateAudience = false,
			RequireAudience = false
		};
	}
}
