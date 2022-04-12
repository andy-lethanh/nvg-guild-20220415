namespace SimpleApp.Avatar.Shared;

public record AccountAvatarEntity
{
	public string PK { get; init; } = string.Empty;
	public string SK { get; init; } = string.Empty;

	public string UserId { get; init; } = string.Empty;
	public string Key { get; init; } = string.Empty;
	public string OriginalName { get; init; } = string.Empty;
	public int? Width { get; init; }
	public int? Height { get; init; }

	public string SizeType { get; init; } = string.Empty;

	public static readonly string PKPrefix = "ACCOUNT#";
	public static readonly string SKPrefix = "ACCOUNT_AVATAR#";

	public static string CreatePK(string userId)
	{
		return PKPrefix + userId;
	}

	public static string CreateSK(string size)
	{
		return SKPrefix + size;
	}
}
