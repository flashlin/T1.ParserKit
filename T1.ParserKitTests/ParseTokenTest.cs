using T1.ParserKit.Core;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class ParseTokenTest : ParseTestBase
	{
		[Fact]
		public void Symbol()
		{
			GivenText(">=");
			WhenParse(ParseToken.Symbol(">="));
			ThenResultShouldBe(">=");
		}
	}
}