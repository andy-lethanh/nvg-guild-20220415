using MediatR;
using SimpleApp.Me.Application.Dto;

namespace SimpleApp.Me.Application.Requests;

public record GetAvatarRequest : IRequest<UploadedAvatarDto>
{
}
