using Amazon.S3.Model;

namespace SimpleApp.Avatar.Shared;

public record AvatarMetadata
{
	private static class Names
	{
		public const string SizeType = "size-type";
		public const string Width = "width";
		public const string Height = "height";
		public const string OriginalName = "original-name";
	}

	public string SizeType { get; init; } = string.Empty;
	public int Width { get; init; }
	public int Height { get; init; }
	public string OriginalName { get; init; } = string.Empty;

	public MetadataCollection MetadataCollection()
	{
		return new MetadataCollection
		{
			[Names.SizeType] = SizeType,
			[Names.Width] = Width.ToString(),
			[Names.Height] = Height.ToString(),
			[Names.OriginalName] = OriginalName
		};
	}

	public IDictionary<string, string> ToDictionary()
	{
		return new Dictionary<string, string>
		{
			[Names.SizeType] = SizeType,
			[Names.Width] = Width.ToString(),
			[Names.Height] = Height.ToString(),
			[Names.OriginalName] = OriginalName
		};
	}

	public void SetTo(MetadataCollection metadataCollection)
	{
		metadataCollection[Names.SizeType] = SizeType;
		metadataCollection[Names.Width] = Width.ToString();
		metadataCollection[Names.Height] = Height.ToString();
		metadataCollection[Names.OriginalName] = OriginalName;
	}

	public static AvatarMetadata FromMetadataCollection(MetadataCollection metadataCollection)
	{
		return new AvatarMetadata
		{
			SizeType = metadataCollection[Names.SizeType],
			Width = int.TryParse(metadataCollection[Names.Width] ?? string.Empty, out var width) ? width : 0,
			Height = int.TryParse(metadataCollection[Names.Height] ?? string.Empty, out var height) ? height : 0,
			OriginalName = metadataCollection[Names.OriginalName]
		};
	}
}
