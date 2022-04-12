namespace SimpleApp.Me.Domain.FileHelper;

public static class ImageFileTypes
{
	private static readonly Lazy<FileType> LazyPng = new(() => new FileType("png",
		new[] {".png"},
		new[] {new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}}));

	private static readonly Lazy<FileType> LazyJpeg = new(() => new FileType("jpeg",
		new[] {".jpeg"},
		new[]
		{
			new byte[] {0xFF, 0xD8, 0xFF, 0xE3}, new byte[] {0xFF, 0xD8, 0xFF, 0xE2},
			new byte[] {0xFF, 0xD8, 0xFF, 0xE0}, new byte[] {0xFF, 0xD8, 0xFF, 0xE8},
			new byte[] {0xFF, 0xD8, 0xFF, 0xE1}
		}));

	private static readonly Lazy<FileType> LazyWebp = new(() => new FileType("webp",
		new[] {".webp"},
		new[] {new byte[] {0x52, 0x49, 0x46, 0x46}, new byte[] {0x57, 0x45, 0x42, 0x50}}));

	public static FileType Png => LazyPng.Value;

	public static readonly FileType Jpeg = LazyJpeg.Value;

	public static readonly FileType Webp = LazyWebp.Value;
}
