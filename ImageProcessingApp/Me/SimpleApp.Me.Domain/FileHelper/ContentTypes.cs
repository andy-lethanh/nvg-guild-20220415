namespace SimpleApp.Me.Domain.FileHelper;

public static class ContentTypes
{
	private static readonly Lazy<string[]> LazySupportedImageContentTypes =
		new(() => new[] {"image/jpeg", "image/png", "image/webp"});

	public static string[] SupportedImageContentTypes => LazySupportedImageContentTypes.Value;
}
