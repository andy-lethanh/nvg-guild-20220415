namespace SimpleApp.Me.Domain.FileHelper;

public record ImageUploadInput(Stream ImageStream, string ImageKey, 
	IDictionary<string, string>? Metadata = null,
	IDictionary<string, string>? Tagging = null);
