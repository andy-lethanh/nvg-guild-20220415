using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using SimpleApp.Me.Application.Dto;

namespace SimpleApp.Me.Application.Requests;

public record UploadAvatarRequest : IRequest<UploadedAvatarDto>
{
	public const long AvatarImageLimitBodySize = 2_097_152;

	[Required] public IFormFile? File { get; set; }
}
