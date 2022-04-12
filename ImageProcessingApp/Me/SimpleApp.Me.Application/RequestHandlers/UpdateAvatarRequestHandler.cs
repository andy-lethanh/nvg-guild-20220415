using System.Net;
using SimpleApp.Avatar.Shared;
using Adl.ModelBinding;
using Amazon.DynamoDBv2.DataModel;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleApp.Me.Application.Dto;
using SimpleApp.Me.Application.Requests;
using SimpleApp.Me.Application.Session;
using SimpleApp.Me.Domain.Configuration;
using SimpleApp.Me.Domain.FileHelper;

namespace SimpleApp.Me.Application.RequestHandlers;

public class UpdateAvatarRequestHandler : IRequestHandler<UploadAvatarRequest, UploadedAvatarDto>
{
	private readonly IServiceProvider _serviceProvider;

	public UpdateAvatarRequestHandler(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
	}

	public async Task<UploadedAvatarDto> Handle(UploadAvatarRequest request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var file = request.File;
		await using var imageStream = file!.OpenReadStream();
		var imageValidateInput = new IImageValidator.Input(imageStream,
			file.ContentType,
			WebUtility.HtmlEncode(file.FileName));
		var error = GetImageValidator().Validate(imageValidateInput, out var imageData);
		if (error is not null)
		{
			throw new RestApiException(error);
		}

		var imageConfiguration = GetImageConfiguration();
		var key = AvatarHelper.CreateUniqueImageKey(imageConfiguration.AvatarImagePathPrefix,
			imageData!.ImageExtension);
		var userId = GetSession().GetUserId();
		var metadata = new AvatarMetadata
		{
			OriginalName = imageData.TrustedImageName,
			Height = imageData.ImageInfo.Height,
			Width = imageData.ImageInfo.Width,
			SizeType = ImageSizeTypes.Origin
		};
		var tagging = new AvatarTagSet {UserId = userId}.ToList().ToDictionary(t => t.Key, t => t.Value);
		var uploadImageInput = new ImageUploadInput(imageStream, key, metadata.ToDictionary(), tagging);

		await GetImageStore().UploadImageAsync(uploadImageInput, cancellationToken);

		var avatarEntity = new AccountAvatarEntity
		{
			PK = AccountAvatarEntity.CreatePK(userId),
			SK = AccountAvatarEntity.CreateSK(metadata.SizeType),
			UserId = userId,
			Width = metadata.Width,
			Height = metadata.Height,
			Key = key,
			OriginalName = metadata.OriginalName,
			SizeType = metadata.SizeType
		};

		var savingConfig = new DynamoDBOperationConfig {OverrideTableName = imageConfiguration.ImageTableName};
		await GetDynamoDbContext().SaveAsync(avatarEntity, savingConfig, cancellationToken);

		return new UploadedAvatarDto
		{
			Images = new[]
			{
				new AvatarDto
				{
					Url = GetImageUrlBuilder().BuildUrl(key),
					Width = avatarEntity.Width,
					Height = avatarEntity.Height,
					SizeType = avatarEntity.SizeType
				}
			}
		};
	}

	private IImageValidator GetImageValidator() => _serviceProvider.GetRequiredService<IImageValidator>();

	private ImageConfiguration GetImageConfiguration() =>
		_serviceProvider.GetRequiredService<IOptions<ImageConfiguration>>().Value;

	private ISession GetSession() => _serviceProvider.GetRequiredService<ISession>();

	private IImageStore GetImageStore() => _serviceProvider.GetRequiredService<IImageStore>();

	private IDynamoDBContext GetDynamoDbContext() => _serviceProvider.GetRequiredService<IDynamoDBContext>();

	private IImageUrlBuilder GetImageUrlBuilder() => _serviceProvider.GetRequiredService<IImageUrlBuilder>();
}
