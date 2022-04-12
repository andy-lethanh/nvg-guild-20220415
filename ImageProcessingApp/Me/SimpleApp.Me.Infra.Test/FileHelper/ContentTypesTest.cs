using System;
using SimpleApp.Me.Domain.FileHelper;
using Xunit;

namespace SimpleApp.Me.Domain.Test.FileHelper;

public class ContentTypesTest
{
	[Theory]
	[InlineData("image/jpeg")]
	[InlineData("image/png")]
	[InlineData("image/webp")]
	public void Assert_ImageContentTypes_Support_4SpecificImageType(string imageType)
	{
		Assert.Contains(ContentTypes.SupportedImageContentTypes,
			expectedImageType => string.Equals(expectedImageType, imageType, StringComparison.OrdinalIgnoreCase));
	}
}
