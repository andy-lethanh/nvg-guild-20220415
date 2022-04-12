using SimpleApp.Me.Domain.Errors;
using SimpleApp.Me.Domain.FileHelper;
using Adl.ModelBinding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace SimpleApp.Me.Infra.FileHelper;

public class ImageValidator : IImageValidator
{
	private readonly IImageTypeVerifier _imageTypeVerifier;

	public ImageValidator(IImageTypeVerifier imageTypeVerifier)
	{
		_imageTypeVerifier = imageTypeVerifier;
	}

	public Error? Validate(IImageValidator.Input input, out IImageValidator.ImageValidatedData? imageValidatedData)
	{
		var (imageStream, contentType, trustedImageName) = input;
		if (!ImageHelper.ContainsImageContentType(ContentTypes.SupportedImageContentTypes, contentType))
		{
			imageValidatedData = null;
			return new Error {Code = AccountErrorCodes.AvatarImageContentTypeNotValid};
		}

		IImageInfo imageInfo;
		IImageFormat imageFormat;
		string fileExtension;
		try
		{
			fileExtension = Path.GetExtension(trustedImageName);
			var verifyInput = new FileVerifiedInput(imageStream, fileExtension);
			var validatedResult = _imageTypeVerifier.VerifyImageType(verifyInput);
			if (!validatedResult.Success)
			{
				imageValidatedData = null;
				return new Error {Code = AccountErrorCodes.AvatarImageTypeNotValid};
			}

			imageInfo = Image.Identify(imageStream, out imageFormat);
			if (imageInfo is null)
			{
				imageValidatedData = null;
				return new Error {Code = AccountErrorCodes.AvatarImageInvalid};
			}
		}
		catch
		{
			imageValidatedData = null;
			return new Error {Code = AccountErrorCodes.AvatarImageInvalid};
		}

		if (!ImageHelper.ImageSizeIsAvatar(imageInfo.Size(),
			    ImageHelper.SquareImageMinSize,
			    ImageHelper.SquareImageMaxSize))
		{
			imageValidatedData = null;
			return new Error {Code = AccountErrorCodes.AvatarImageSizeNotValid};
		}

		imageValidatedData = new IImageValidator.ImageValidatedData(imageInfo,
			imageFormat,
			trustedImageName,
			fileExtension);
		return null;
	}
}
