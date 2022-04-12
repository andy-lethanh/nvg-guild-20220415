using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SimpleApp.AvatarResize;

public static class ResizeImageHelper
{
	public static async Task<ResizeImageResponse> ResizeImageAsync(ResizeImageRequest request)
	{
		var (inputStream, info) = request;
		var (image, format) = await Image.LoadWithFormatAsync(inputStream);
		try
		{
			image.Mutate(m => m.Resize(info.Size));
			var outputStream = StreamHelper.MemoryStreamManager.GetStream();
			await image.SaveAsync(outputStream, format);
			outputStream.Seek(0, SeekOrigin.Begin);
			return new ResizeImageResponse(outputStream, info);
		}
		finally
		{
			image.Dispose();
		}
	}

	public static async Task<ResizeImageResponse> ResizeImageAsync(Stream stream, ResizeImageInfo info)
	{
		var resizedImageRequest = new ResizeImageRequest(stream, info);
		var resizedImageResponse = await ResizeImageAsync(resizedImageRequest);
		return resizedImageResponse;
	}
}
