using SimpleApp.Me.Domain.Configuration;
using SimpleApp.Me.Domain.FileHelper;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;

namespace SimpleApp.Me.Infra.FileHelper;

public class ImageStore : IImageStore
{
	private readonly ITransferUtility _transferUtility;
	private readonly ImageConfiguration _imageConfiguration;

	public ImageStore(ITransferUtility transferUtility, IOptions<ImageConfiguration> imageOptions)
	{
		_transferUtility = transferUtility ?? throw new ArgumentNullException(nameof(transferUtility));
		_imageConfiguration = imageOptions.Value ?? throw new ArgumentNullException(nameof(imageOptions.Value));
	}

	public async Task UploadImageAsync(ImageUploadInput input, CancellationToken cancellationToken = default)
	{
		var (imageStream, imageKey, metadata, tagging) = input;
		var uploadRequest = new TransferUtilityUploadRequest
		{
			BucketName = _imageConfiguration.S3BucketName,
			Key = imageKey,
			InputStream = imageStream,
			AutoCloseStream = false,
			TagSet = tagging is not null && tagging.Count > 0 ? CreateTagSet(tagging) : null
		};
		if (metadata is not null && metadata.Count > 0)
		{
			SetMetadata(uploadRequest.Metadata, metadata);
		}

		await _transferUtility.UploadAsync(uploadRequest, cancellationToken);
	}

	private static void SetMetadata(MetadataCollection metadataCollection, IDictionary<string, string> metadata)
	{
		foreach (var (key, value) in metadata)
		{
			metadataCollection[key] = value;
		}
	}

	private static List<Tag> CreateTagSet(IDictionary<string, string> tagging)
	{
		var tags = new List<Tag>(tagging.Count);
		foreach (var (key, value) in tagging)
		{
			tags.Add(new Tag {Key = key, Value = value});
		}

		return tags;
	}
}
