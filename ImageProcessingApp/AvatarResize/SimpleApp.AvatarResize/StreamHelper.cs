using Microsoft.IO;

namespace SimpleApp.AvatarResize;

public static class StreamHelper
{
	public static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();
}
