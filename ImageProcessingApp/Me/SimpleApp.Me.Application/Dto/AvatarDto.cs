using System.Text.Json.Serialization;

namespace SimpleApp.Me.Application.Dto;

public record AvatarDto
{
	[JsonPropertyName("url")] public string? Url { get; set; }

	[JsonPropertyName("sizeType")] public string? SizeType { get; set; }

	[JsonPropertyName("width")] public int? Width { get; set; }

	[JsonPropertyName("height")] public int? Height { get; set; }
}
