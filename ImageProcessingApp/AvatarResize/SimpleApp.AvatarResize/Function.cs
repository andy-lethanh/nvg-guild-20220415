using SimpleApp.Avatar.Shared;
using SimpleApp.AvatarResize;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<AvatarResizeJsonContext>))]

namespace SimpleApp.AvatarResize;

public class Function
{
	private static readonly string AvatarTableName = Environment.GetEnvironmentVariable("AVATAR_TABLE_NAME")!;
	private static readonly string ObjectPrefix = Environment.GetEnvironmentVariable("OBJECT_PREFIX")!;

	private readonly IAmazonS3 _s3;
	private readonly ITransferUtility _transferUtility;
	private readonly IDynamoDBContext _context;

	public Function()
	{
		_s3 = new AmazonS3Client();
		_transferUtility = new TransferUtility(_s3);
		_context = new DynamoDBContext(new AmazonDynamoDBClient());
	}

	public async Task<SQSBatchResponse> ResizeImageFromS3(SQSEvent sqsEvent, ILambdaContext context)
	{
		var batchItemFailures = new List<SQSBatchResponse.BatchItemFailure>(sqsEvent.Records.Count);
		foreach (var message in sqsEvent.Records)
		{
			try
			{
				context.Logger.LogInformation(message.Body);
				await ProcessImageFromS3EventsAsync(message);
			}
			catch (Exception e)
			{
				var itemFailure = new SQSBatchResponse.BatchItemFailure {ItemIdentifier = message.MessageId};
				batchItemFailures.Add(itemFailure);
				context.Logger.LogError(e.ToString());
			}
		}

		return new SQSBatchResponse(batchItemFailures);
	}

	private async Task ProcessImageFromS3EventsAsync(SQSEvent.SQSMessage message)
	{
		var s3Events = S3EventNotification.ParseJson(message.Body);
		foreach (var s3Event in s3Events.Records)
		{
			var bucketName = s3Event.S3.Bucket.Name;
			var key = s3Event.S3.Object.Key;
			var taggingResponse = await _s3.GetObjectTaggingAsync(new GetObjectTaggingRequest
			{
				Key = key, BucketName = bucketName
			});
			var avatarTagSet = AvatarTagSet.FromTagList(taggingResponse.Tagging);
			await ProcessImageAsync(avatarTagSet.UserId, bucketName, key);
		}
	}

	private async Task ProcessImageAsync(string userId, string bucketName, string key)
	{
		using var objectResponse = await _s3.GetObjectAsync(bucketName, key);
		var objectMetadata = AvatarMetadata.FromMetadataCollection(objectResponse.Metadata);
		await using var stream = objectResponse.ResponseStream;
		await using var memoryStream = StreamHelper.MemoryStreamManager.GetStream();

		await stream.CopyToAsync(memoryStream);
		memoryStream.Seek(0, SeekOrigin.Begin);

		var uploadedImageResponses = new List<UploadImageResponse>(ResizeImageInfo.DefaultResizeImageInfos.Length);

		foreach (var info in ResizeImageInfo.DefaultResizeImageInfos)
		{
			var resizeImageResponse = await ResizeImageHelper.ResizeImageAsync(memoryStream, info);
			var uploadedImageResponse = await UploadResizedImage(userId,
				objectResponse, objectMetadata, resizeImageResponse);
			uploadedImageResponses.Add(uploadedImageResponse);
			memoryStream.Seek(0, SeekOrigin.Begin);
		}

		var copyRequest = new CopyObjectRequest
		{
			SourceBucket = bucketName,
			DestinationBucket = bucketName,
			SourceKey = objectResponse.Key,
			DestinationKey = AvatarHelper.CreateUniqueImageKey(ObjectPrefix, Path.GetExtension(objectResponse.Key))
		};
		await _s3.CopyObjectAsync(copyRequest);

		uploadedImageResponses.Add(new UploadImageResponse(copyRequest.DestinationKey, objectMetadata));
		await PutAvatarMetadataToDb(userId, uploadedImageResponses);
	}

	private async Task<UploadImageResponse> UploadResizedImage(string userId,
		GetObjectResponse objectResponse,
		AvatarMetadata objectMetadata,
		ResizeImageResponse resizeImageResponse)
	{
		var imageExtension = Path.GetExtension(objectResponse.Key);
		var key = AvatarHelper.CreateUniqueImageKey(ObjectPrefix, imageExtension);
		var (resizedImage, ((width, height), sizeType)) = resizeImageResponse;
		var metadata = new AvatarMetadata
		{
			OriginalName = objectMetadata.OriginalName, Width = width, Height = height, SizeType = sizeType
		};
		try
		{
			var request = new TransferUtilityUploadRequest
			{
				BucketName = objectResponse.BucketName,
				Key = key,
				InputStream = resizedImage,
				AutoCloseStream = true,
				TagSet = new AvatarTagSet {UserId = userId}.ToList()
			};
			metadata.SetTo(request.Metadata);
			await _transferUtility.UploadAsync(request);
		}
		finally
		{
			await resizedImage.DisposeAsync();
		}

		return new UploadImageResponse(key, metadata);
	}

	private async Task PutAvatarMetadataToDb(string userId, IEnumerable<UploadImageResponse> uploadedImageResponses)
	{
		var config = new DynamoDBOperationConfig {OverrideTableName = AvatarTableName};
		var batchWriter = _context.CreateBatchWrite<AccountAvatarEntity>(config);
		var avatarEntities = uploadedImageResponses.Select(response => CreateAvatar(userId, response));
		batchWriter.AddPutItems(avatarEntities);
		await batchWriter.ExecuteAsync();

		static AccountAvatarEntity CreateAvatar(string userId, UploadImageResponse response) =>
			new()
			{
				PK = AccountAvatarEntity.CreatePK(userId),
				SK = AccountAvatarEntity.CreateSK(response.Metadata.SizeType),
				Width = response.Metadata.Width,
				Height = response.Metadata.Height,
				Key = response.Key,
				SizeType = response.Metadata.SizeType,
				OriginalName = response.Metadata.OriginalName,
				UserId = userId
			};
	}
}
