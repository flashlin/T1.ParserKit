using T1.ParserKit.Core;
using Xunit;

namespace T1.ParserKitTests
{
	public class ParseTokenTest : ParseTestBase
	{
		[Fact]
		public void Symbol()
		{
			GiveText(">=");
			WhenParse(ParseToken.Symbol(">="));
			ThenResultShouldBe(">=");
		}
	}
}