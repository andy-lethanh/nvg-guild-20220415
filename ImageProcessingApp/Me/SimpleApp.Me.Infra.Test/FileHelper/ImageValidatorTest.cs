using System.IO;
using SimpleApp.Me.Domain.FileHelper;
using SimpleApp.Me.Domain.Test.Setup;
using SimpleApp.Me.Infra.FileHelper;
using Xunit;

namespace SimpleApp.Me.Domain.Test.FileHelper;

public class ImageValidatorTest
{
	[Fact]
	public void Assert_ImageValidate_NoError()
	{
		var fileStream = File.OpenRead("./Resources/image-512x512.png");
		var imageValidator = new ImageValidator(Instances.ImageTypeVerifier);
		var input = new IImageValidator.Input(fileStream, "image/png", "image-512x512.png");
		var error = imageValidator.Validate(input, out _);
		Assert.Null(error);
	}
}
