using SixLabors.ImageSharp;

namespace SimpleApp.Me.Domain.FileHelper;

public static class ImageHelper
{
	public static readonly Size SquareImageMinSize = new(512, 512);
	public static readonly Size SquareImageMaxSize = new(1024, 1024);

	public static bool ImageSizeIsSquare(in Size size)
	{
		return !size.IsEmpty && size.Width == size.Height;
	}

	public static bool ImageSizeInRange(in Size imageSize, in Size from, in Size to)
	{
		var (width, height) = imageSize;
		var isWidthInRage = from.Width <= width && width <= to.Width;
		if (!isWidthInRage)
		{
			return false;
		}

		var isHeightInRange = from.Height <= height && height <= to.Height;
		return isHeightInRange;
	}

	public static bool ImageSizeIsAvatar(in Size imageSize, in Size minAvatarSize, in Size maxAvatarSize)
	{
		if (!ImageSizeIsSquare(minAvatarSize))
		{
			throw new InvalidOperationException($"{nameof(minAvatarSize)} should has same width and height");
		}

		if (!ImageSizeIsSquare(maxAvatarSize))
		{
			throw new InvalidOperationException($"{nameof(maxAvatarSize)} should has same width and height");
		}

		return ImageSizeIsSquare(imageSize) && ImageSizeInRange(imageSize, minAvatarSize, maxAvatarSize);
	}

	public static bool ContainsImageContentType(IEnumerable<string> imageContentTypes, string contentType)
	{
		return imageContentTypes.Any(imageContentType =>
			string.Equals(imageContentType, contentType, StringComparison.OrdinalIgnoreCase));
	}
}
