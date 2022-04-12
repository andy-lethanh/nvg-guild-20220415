using SimpleApp.Me.Domain.FileHelper;
using SixLabors.ImageSharp;
using Xunit;

namespace SimpleApp.Me.Domain.Test.FileHelper;

public class ImageHelperTest
{
	[Theory]
	[MemberData(nameof(GetImageSizeInRangeData))]
	public void Assert_CheckImageSizeInRange_Success(Size imageSize, Size fromSize, Size toSize)
	{
		var success = ImageHelper.ImageSizeInRange(imageSize, fromSize, toSize);
		Assert.True(success);
	}

	[Theory]
	[MemberData(nameof(GetSquareImageSizeData))]
	public void Assert_CheckSquareImageSize_Success(Size squareImageSize)
	{
		var success = ImageHelper.ImageSizeIsSquare(squareImageSize);
		Assert.True(success);
	}

	private static object[][] GetImageSizeInRangeData()
	{
		return new[]
		{
			new object[] {new Size(768, 524), ImageHelper.SquareImageMinSize, ImageHelper.SquareImageMaxSize}
		};
	}

	private static object[][] GetSquareImageSizeData()
	{
		return new[]
		{
			new object[] {new Size(768, 768)}, new object[] {new Size(125, 125)},
			new object[] {new Size(1024, 1024)}, new object[] {new Size(258, 258)}
		};
	}
}
