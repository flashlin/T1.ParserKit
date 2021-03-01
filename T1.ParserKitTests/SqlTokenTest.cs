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

		[Theory]
		[InlineData("/**/")]
		[InlineData("/*1*/")]
		[InlineData("/*12*/")]
		[InlineData("/*123*/")]
		[InlineData("/*12**/")]
		public void Comment2(string text)
		{
			GiveText(text);
			WhenParse(Parse.CStyleComment2);
			ThenResultShouldBe(new TextSpan()
			{
				File = string.Empty,
				Text = text.Substring(2, text.Length - 4),
				Position = (text.Length - 4) > 0 ? 2 : 0,
				Length = text.Length - 4
			});
		}

	}
}
