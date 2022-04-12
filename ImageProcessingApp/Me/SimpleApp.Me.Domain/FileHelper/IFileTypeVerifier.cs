namespace SimpleApp.Me.Domain.FileHelper;

public interface IFileTypeVerifier
{
	FileTypeVerifiedResult VerifyImageType(FileVerifiedInput input);
}
