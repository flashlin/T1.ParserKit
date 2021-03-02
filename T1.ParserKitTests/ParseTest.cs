using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using T1.ParserKit.Core;
using T1.ParserKit.Core.Parsers;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class ParseTest : ParseTestBase
	{
		[Fact]
		public void Letters()
		{
			GivenText("abc");
			WhenParse(Parse.Letters);
			ThenResultShouldBe("abc");
		}

		[Fact]
		public void Digits()
		{
			GivenText("123");
			WhenParse(Parse.Digits);
			ThenResultShouldBe("123");
		}

		[Fact]
		public void NotDigit_Letter()
		{
			GivenText("a");
			WhenParse(Parse.Digit.Not().ThenRight(Parse.Letter));
			ThenResultShouldBe("a");
		}

		[Fact]
		public void NotDigit()
		{
			GivenText("a");
			WhenParse(Parse.Digit.Not());
			ThenResultShouldBe(Unit.Instance);
		}

		[Fact]
		public void Any()
		{
			GivenText("b");
			WhenParse(Parse.Any(Parse.Equal("a"), Parse.Equal("b")));
			ThenResultShouldBe("b");
		}

		[Fact]
		public void Equal()
		{
			GivenText("a");
			WhenParse(Parse.Equal("a"));
			ThenResultShouldBe("a");
		}

		[Fact]
		public void Many()
		{
			GivenText("bb");
			WhenParse(Parse.Equal("b").Many());
			ThenResultShouldBe("bb");
		}

		[Fact]

		public void Many_empty()
		{
			GivenText("a");
			WhenParse(Parse.Equal("b").Many());
			ThenResultShouldBe("");
		}

		[Fact]
		public void CStyleIdentifier()
		{
			GivenText("name");
			WhenParse(Parse.CStyleIdentifier);
			ThenResultShouldBe("name");
		}

		[Theory]
		[InlineData("/**/")]
		[InlineData("/*1*/")]
		[InlineData("/*12*/")]
		[InlineData("/*123*/")]
		[InlineData("/*12**/")]
		public void Comment2(string text)
		{
			GivenText(text);
			WhenParse(Parse.CStyleComment2);
			ThenResultShouldBe(new TextSpan()
			{
				File = string.Empty,
				Text = text.Substring(2, text.Length - 4),
				Position = (text.Length - 4) > 0 ? 2 : 0,
				Length = text.Length - 4
			});
		}

		[Fact]
		public void Sequence()
		{
			GivenText("ab");
			WhenParse(Parse.Seq(
				Parse.Match("a"),
				Parse.Match("b")
				).Merge()
			);
			ThenResultShouldBe("ab");
		}

		[Fact]
		public void Not_then_anyChar1()
		{
			GivenText("a");
			WhenParse(Parse.Equal("b").Not().ThenRight(Parse.AnyChars(1)));
			ThenResultShouldBe("a");
		}

		[Fact]
		public void CStyleString()
		{
			GivenText("\"123\"");
			WhenParse(Parse.CStyleString);
			ThenResultShouldBe("\"123\"");
		}

		[Fact]
		public void CStyleString_with_escape_char()
		{
			GivenText("\"12\\\"3\"");
			WhenParse(Parse.CStyleString);
			ThenResultShouldBe("\"12\\\"3\"");
		}

		
	}
}
