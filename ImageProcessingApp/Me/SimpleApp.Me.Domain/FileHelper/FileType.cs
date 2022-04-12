namespace SimpleApp.Me.Domain.FileHelper;

public class FileType
{
	public FileType(string name, IEnumerable<string> extensions, IEnumerable<byte[]> signatures)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentNullException(nameof(name));
		}

		Name = name;
		Extensions = extensions ?? throw new ArgumentNullException(nameof(extensions));
		Signatures = signatures ?? throw new ArgumentNullException(nameof(signatures));
	}

	public string Name { get; }
	public IEnumerable<string> Extensions { get; }
	public IEnumerable<byte[]> Signatures { get; }

	public int MaxSignatureLength => Signatures.Max(m => m.Length);
}
