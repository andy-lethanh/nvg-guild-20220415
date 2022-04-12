using SimpleApp.Avatar.Shared;
using Xunit;

namespace SimpleApp.Me.Domain.Test.Entity;

public class AccountAvatarEntityTest
{
	[Fact]
	public void Assert_CreatePK_Correctly()
	{
		var pk = AccountAvatarEntity.CreatePK("andy");
		Assert.Equal("ACCOUNT#andy", pk);
	}

	[Fact]
	public void Assert_CreateSK_Correctly()
	{
		var sk = AccountAvatarEntity.CreateSK(ImageSizeTypes.Origin);
		Assert.Equal("ACCOUNT_AVATAR#Origin", sk);
	}
}
