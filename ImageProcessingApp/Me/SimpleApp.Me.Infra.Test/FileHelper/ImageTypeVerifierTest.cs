using System.IO;
using SimpleApp.Me.Domain.FileHelper;
using SimpleApp.Me.Domain.Test.Setup;
using Xunit;

namespace SimpleApp.Me.Domain.Test.FileHelper;

public class ImageTypeVerifierTest
{
	[Theory]
	[InlineData("./Resources/test-image-file-type-png.png")]
	public void Assert_VerifyLoadedPngImage_Success(string imagePath)
	{
		using var fileStream = File.OpenRead(imagePath);
		var verifiedResult = Instances.ImageTypeVerifier.VerifyImageType(new FileVerifiedInput(fileStream, ".png"));
		Assert.True(verifiedResult.Success);
	}

	[Theory]
	[InlineData("./Resources/test-image-fake-type-png.png")]
	public void Assert_VerifyFakePngImage_Fail(string imagePath)
	{
		using var fileStream = File.OpenRead(imagePath);
		var verifiedResult = Instances.ImageTypeVerifier.VerifyImageType(new FileVerifiedInput(fileStream, ".png"));
		Assert.False(verifiedResult.Success);
	}
}
