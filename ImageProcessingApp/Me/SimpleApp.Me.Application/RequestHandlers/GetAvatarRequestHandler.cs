using System.Runtime.CompilerServices;
using SimpleApp.Avatar.Shared;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MediatR;
using Microsoft.Extensions.Options;
using SimpleApp.Me.Application.Dto;
using SimpleApp.Me.Application.Requests;
using SimpleApp.Me.Application.Session;
using SimpleApp.Me.Domain.Configuration;
using SimpleApp.Me.Domain.FileHelper;

[assembly: InternalsVisibleTo("SimpleApp.Me.Applicationtion.Test")]

namespace SimpleApp.Me.Application.RequestHandlers;

public class GetAvatarRequestHandler : IRequestHandler<GetAvatarRequest, UploadedAvatarDto>
{
	private readonly IDynamoDBContext _dbContext;
	private readonly ISession _session;
	private readonly ImageConfiguration _imageConfiguration;
	private readonly IImageUrlBuilder _imageUrlBuilder;

	public GetAvatarRequestHandler(IDynamoDBContext dbContext, ISession session,
		IOptions<ImageConfiguration> imageOptions, IImageUrlBuilder imageUrlBuilder)
	{
		_dbContext = dbContext;
		_session = session;
		_imageConfiguration = imageOptions.Value;
		_imageUrlBuilder = imageUrlBuilder;
	}

	public async Task<UploadedAvatarDto> Handle(GetAvatarRequest request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var userId = _session.GetUserId();
		var queryOperationConfig = CreateQueryOperationConfig(userId);
		var dynamoDbOperationConfig =
			new DynamoDBOperationConfig {OverrideTableName = _imageConfiguration.ImageTableName};
		var avatars = await _dbContext
			.FromQueryAsync<AccountAvatarEntity>(queryOperationConfig, dynamoDbOperationConfig)
			.GetRemainingAsync(cancellationToken);

		return new UploadedAvatarDto {Images = avatars.Select(CreateAvatarDto)};
	}

	private AvatarDto CreateAvatarDto(AccountAvatarEntity avatar)
	{
		return new AvatarDto
		{
			Url = _imageUrlBuilder.BuildUrl(avatar.Key),
			Width = avatar.Width,
			Height = avatar.Height,
			SizeType = avatar.SizeType
		};
	}

	internal static QueryOperationConfig CreateQueryOperationConfig(string userId)
	{
		return new QueryOperationConfig
		{
			KeyExpression = new Expression
			{
				ExpressionStatement = "PK = :pk and begins_with(SK, :sk)",
				ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
				{
					{":pk", AccountAvatarEntity.CreatePK(userId)}, {":sk", AccountAvatarEntity.SKPrefix}
				}
			}
		};
	}
}
