namespace SimpleApp.Avatar.Shared;

public static class AvatarHelper
{
	private const string GenerateRandomValue = "0123456789";

	public static string CreateUniqueImageKey(string prefix, string imageExtension)
	{
		var random16 = Nanoid.Nanoid.Generate(GenerateRandomValue, 16);
		return $"{prefix}{random16}/{Guid.NewGuid():N}{imageExtension}";
	}
}
