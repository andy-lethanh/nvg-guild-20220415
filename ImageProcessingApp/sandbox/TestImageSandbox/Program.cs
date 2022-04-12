// See https://aka.ms/new-console-template for more information

using SimpleApp.Avatar.Shared;

for (var i = 0; i < 100; i++)
{
	Console.WriteLine(AvatarHelper.CreateUniqueImageKey("user-avatar/", ".png"));
}

Console.ReadLine();
