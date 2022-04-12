using Adl.ModelBinding;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleApp.Me.Application.Dto;
using SimpleApp.Me.Application.Requests;

namespace SimpleApp.Me.WebApi.Controllers;

/// <summary>
/// Me service, all about current user
/// </summary>
[ApiController]
[Route("v{version:apiVersion}/me")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class MeController : ControllerBase
{
	/// <summary>
	/// Get current user's avatar
	/// </summary>
	/// <param name="mediator"></param>
	/// <param name="cancellationToken"></param>
	/// <returns>User Avatar</returns>
	/// <response code="200">Success</response>
	/// <response code="400">Client error</response>
	[HttpGet("avatar")]
	[ProducesResponseType(typeof(RestApiResponse.SuccessResponse<UploadedAvatarDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(RestApiResponse.ErrorResponse), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetAvatar([FromServices] IMediator mediator, CancellationToken cancellationToken)
	{
		var avatarDto = await mediator.Send(new GetAvatarRequest(), cancellationToken);
		return RestApiResponse.Ok(avatarDto).ObjectResult();
	}

	/// <summary>
	/// Upload current user's avatar
	/// </summary>
	/// <param name="request"></param>
	/// <param name="mediator"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <response code="200">Success</response>
	/// <response code="400">Client error</response>
	[HttpPut("avatar")]
	[RequestFormLimits(MultipartBodyLengthLimit = UploadAvatarRequest.AvatarImageLimitBodySize)]
	[ProducesResponseType(typeof(RestApiResponse.SuccessResponse<UploadedAvatarDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(RestApiResponse.ErrorResponse), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarRequest request,
		[FromServices] IMediator mediator,
		CancellationToken cancellationToken)
	{
		var avatarDto = await mediator.Send(request, cancellationToken);
		return RestApiResponse.Ok(avatarDto).ObjectResult();
	}
}
