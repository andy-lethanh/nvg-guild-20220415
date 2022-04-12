using Amazon.S3.Model;

namespace SimpleApp.Avatar.Shared;

public record AvatarTagSet
{
	public string UserId { get; init; } = string.Empty;
	public string ObjectType { get; init; } = "UserAvatar";

	private static class Names
	{
		public const string UserId = "UserId";
		public const string ObjectType = "ObjectType";
	}

	public List<Tag> ToList()
	{
		return new List<Tag>
		{
			new() {Key = Names.UserId, Value = UserId}, new() {Key = Names.ObjectType, Value = ObjectType}
		};
	}

	public static AvatarTagSet FromTagList(List<Tag> tags)
	{
		return new AvatarTagSet
		{
			UserId = tags.FirstOrDefault(t => string.Equals(Names.UserId, t.Key))?.Value ?? string.Empty,
			ObjectType = tags.FirstOrDefault(t => string.Equals(Names.ObjectType, t.Key))?.Value ?? string.Empty
		};
	}
}
