using SimpleApp.Avatar.Shared;
using SixLabors.ImageSharp;

namespace SimpleApp.AvatarResize;

public record ResizeImageInfo(Size Size, string SizeType)
{
	public static readonly ResizeImageInfo X256 = new(new Size(256, 256), ImageSizeTypes.X256);
	public static readonly ResizeImageInfo X512 = new(new Size(512, 512), ImageSizeTypes.X512);
	public static readonly ResizeImageInfo X1024 = new(new Size(1024, 1024), ImageSizeTypes.X1024);

	public static readonly ResizeImageInfo[] DefaultResizeImageInfos = {X256, X512, X1024};
}
