namespace SimpleApp.Me.Domain.Configuration;

public class ImageConfiguration
{
	public string AvatarImagePathPrefix { get; set; } = string.Empty;
	public string S3CustomEndpoint { get; set; } = string.Empty;
	public string S3BucketName { get; set; } = string.Empty;
	public string ImageTableName { get; set; } = string.Empty;
}
