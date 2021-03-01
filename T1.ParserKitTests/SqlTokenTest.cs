using System;
using System.Linq;
using System.Text;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
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

		[Fact]
		public void Comment1()
		{
			GiveText("--123");
			WhenParse(SqlToken.Comment);
			ThenResultShouldBe(new SqlCommentExpression()
			{
				IsMultipleLines = false,
				Content = "123"
			});
		}

		[Fact]
		public void Comment1_crlf()
		{
			GiveText("--123\r\n");
			WhenParse(SqlToken.Comment);
			ThenResultShouldBe(new SqlCommentExpression()
			{
				IsMultipleLines = false,
				Content = "123"
			});
		}

		[Fact]
		public void Comment2_crlf()
		{
			GiveText("/*123\r\n456*/");
			WhenParse(SqlToken.Comment);
			ThenResultShouldBe(new SqlCommentExpression()
			{
				IsMultipleLines = true,
				Content = "123\r\n456"
			});
		}

		[Fact]
		public void Comma()
		{
			GiveText(",");
			WhenParse(SqlParser.Comma);
			ThenResultShouldBe(new SqlExpression()
			{
				TextSpan = new TextSpan()
				{
					File = string.Empty,
					Text = ",",
					Length = 1
				}
			});
		}


	}
}
