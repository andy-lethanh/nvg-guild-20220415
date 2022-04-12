using Adl.ModelBinding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace SimpleApp.Me.Domain.FileHelper;

public interface IImageValidator
{
	public record Input(Stream ImageStream, string ContentType, string TrustedImageName);

	public record ImageValidatedData(IImageInfo ImageInfo, IImageFormat ImageFormat, string TrustedImageName,
		string ImageExtension);

	Error? Validate(Input input, out ImageValidatedData? imageValidatedData);
}
