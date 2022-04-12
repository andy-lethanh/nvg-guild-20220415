namespace SimpleApp.Me.Domain.FileHelper;

public interface IImageStore
{
	Task UploadImageAsync(ImageUploadInput input, CancellationToken cancellationToken = default);
}
