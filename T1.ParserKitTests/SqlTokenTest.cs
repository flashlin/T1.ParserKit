using System;
using System.Linq;
using System.Text;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlTokenTest : ParseTestBase
	{
		[Fact]
		public void NonIdentifier()
		{
			GiveText("from");
			WhenParse(SqlParser.Identifier);
			ThenResultShouldFail();
		}

		[Fact]
		public void Identifier()
		{
			GiveText("name");
			WhenParse(SqlParser.Identifier);
			ThenResultShouldBe("name");
		}

		[Theory]
		[InlineData("where")]
		[InlineData("select")]
		public void MoreNonIdentifier(string keyword)
		{
			GiveText(keyword);
			WhenParse(SqlParser.Identifier);
			ThenResultShouldFail();
		}
	}
}
