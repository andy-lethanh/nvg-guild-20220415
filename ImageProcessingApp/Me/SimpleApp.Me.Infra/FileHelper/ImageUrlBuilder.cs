using SimpleApp.Me.Domain.Configuration;
using SimpleApp.Me.Domain.FileHelper;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace SimpleApp.Me.Infra.FileHelper;

public class ImageUrlBuilder : IImageUrlBuilder
{
	private const int MaxDaysForValidUrl = 5;
	private readonly IAmazonS3 _s3;
	private readonly ImageConfiguration _imageConfiguration;

	public ImageUrlBuilder(IAmazonS3 s3, IOptions<ImageConfiguration> imageOptions)
	{
		_s3 = s3 ?? throw new ArgumentNullException(nameof(s3));
		_imageConfiguration = imageOptions.Value;
	}

	public string BuildUrl(string path)
	{
		var preSignedUrlRequest = new GetPreSignedUrlRequest
		{
			BucketName = _imageConfiguration.S3BucketName,
			Key = path,
			Expires = DateTimeOffset.UtcNow.DateTime.AddDays(MaxDaysForValidUrl)
		};
		return _s3.GetPreSignedURL(preSignedUrlRequest);
	}
}
