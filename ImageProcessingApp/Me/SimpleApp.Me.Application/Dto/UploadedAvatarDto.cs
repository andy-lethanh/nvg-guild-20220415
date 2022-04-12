using System.Text.Json.Serialization;

namespace SimpleApp.Me.Application.Dto;

public record UploadedAvatarDto
{
	[JsonPropertyName("images")] public IEnumerable<AvatarDto> Images { get; set; } = Array.Empty<AvatarDto>();
}
