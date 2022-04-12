using System.Text;
using SimpleApp.Me.Domain.FileHelper;

namespace SimpleApp.Me.Infra.FileHelper;

public class ImageTypeVerifier : IImageTypeVerifier
{
	private readonly IEnumerable<FileType> _fileTypes;

	public ImageTypeVerifier(IEnumerable<FileType> fileTypes)
	{
		_fileTypes = fileTypes ?? throw new ArgumentNullException(nameof(fileTypes));
	}

	public FileTypeVerifiedResult VerifyImageType(FileVerifiedInput input)
	{
		var (stream, fileExtension) = input;
		var fileType = GetFileTypeByFileExtension(fileExtension);
		if (fileType is null)
		{
			return new FileTypeVerifiedResult(false, null);
		}

		using var reader = new BinaryReader(stream, Encoding.UTF8, true);
		var headerBytes = reader.ReadBytes(fileType.MaxSignatureLength);
		stream.Seek(0, SeekOrigin.Begin);
		var success = fileType.Signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
		return new FileTypeVerifiedResult(success, success ? fileType : null);
	}

	private FileType? GetFileTypeByFileExtension(string extension)
	{
		return _fileTypes.FirstOrDefault(f =>
			f.Extensions.Any(ex => string.Equals(ex, extension, StringComparison.OrdinalIgnoreCase)));
	}

	public static ImageTypeVerifier CreateDefault()
	{
		var fileTypes = new[] {ImageFileTypes.Jpeg, ImageFileTypes.Png, ImageFileTypes.Webp};
		return new ImageTypeVerifier(fileTypes);
	}
}
