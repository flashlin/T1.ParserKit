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
		public void Symbol()
		{
			GivenText(">=");
			WhenParse(ParseToken.Symbol(">="));
			ThenResultShouldBe(">=");
		}

		[Fact]
		public void NonIdentifier()
		{
			GivenText("from");
			WhenParse(SqlToken.Identifier);
			ThenResultShouldFail();
		}

		[Fact]
		public void Identifier()
		{
			GivenText("name");
			WhenParse(SqlToken.Identifier);
			ThenResultShouldBe("name");
		}

		[Theory]
		[InlineData("where")]
		[InlineData("select")]
		public void MoreNonIdentifier(string keyword)
		{
			GivenText(keyword);
			WhenParse(SqlToken.Identifier);
			ThenResultShouldFail();
		}

		[Fact]
		public void Comment1()
		{
			GivenText("--123");
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
			GivenText("--123\r\n");
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
			GivenText("/*123\r\n456*/");
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
			GivenText(",");
			WhenParse(SqlToken.Comma);
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

		[Fact]
		public void NString()
		{
			GivenText("N'123'");
			WhenParse(SqlToken.NString);
			ThenResultShouldBe(new SqlStringExpression()
			{
				TextSpan = new TextSpan()
				{
					File = string.Empty,
					Text = "N'123'",
					Length = 6
				},
				IsUnicode = true,
				Text = "123"
			});
		}
	}
}
