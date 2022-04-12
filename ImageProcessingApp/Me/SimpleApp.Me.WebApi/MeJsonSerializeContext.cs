using System.Text.Json.Serialization;
using Adl.ModelBinding;
using SimpleApp.Me.Application.Dto;

namespace SimpleApp.Me.WebApi;

/// <summary>
/// Serialize context
/// </summary>
[JsonSourceGenerationOptions]
[JsonSerializable(typeof(RestApiResponse.ErrorResponse))]
[JsonSerializable(typeof(AvatarDto))]
[JsonSerializable(typeof(UploadedAvatarDto))]
[JsonSerializable(typeof(RestApiResponse.SuccessResponse<UploadedAvatarDto>))]
public partial class MeJsonSerializeContext : JsonSerializerContext
{
}
