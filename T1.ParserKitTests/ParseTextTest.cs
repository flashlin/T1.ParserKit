using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using T1.ParserKit.Core;
using T1.ParserKit.Core.Parsers;
using Xunit;

namespace T1.ParserKitTests
{
	public class ParseTextTest : ParseTestBase
	{
		[Fact]
		public void Letters()
		{
			GiveText("abc");
			WhenParse(Parse.Letters);
			ThenResultShouldBe("abc");
		}

		[Fact]
		public void Digits()
		{
			GiveText("123");
			WhenParse(Parse.Digits);
			ThenResultShouldBe("123");
		}

		[Fact]
		public void NotDigit_Letter()
		{
			GiveText("a");
			WhenParse(Parse.Digit.Not().ThenRight(Parse.Letter));
			ThenResultShouldBe("a");
		}

		[Fact]
		public void NotDigit()
		{
			GiveText("a");
			WhenParse(Parse.Digit.Not());
			ThenResultShouldBe(Unit.Instance);
		}

		[Fact]
		public void Any()
		{
			GiveText("b");
			WhenParse(Parse.Any(Parse.Equal("a"), Parse.Equal("b")));
			ThenResultShouldBe("b");
		}

		[Fact]
		public void Equal()
		{
			GiveText("a");
			WhenParse(Parse.Equal("a"));
			ThenResultShouldBe("a");
		}

		[Fact]
		public void Many()
		{
			GiveText("bb");
			WhenParse(Parse.Equal("b").Many());
			ThenResultShouldBe("bb");
		}

		[Fact]
		public void Many_empty()
		{
			GiveText("a");
			WhenParse(Parse.Equal("b").Many());
			ThenResultShouldBe(null);
		}

		[Fact]
		public void CStyleIdentifier()
		{
			GiveText("name");
			WhenParse(Parse.CStyleIdentifier);
			ThenResultShouldBe("name");
		}

		[Fact]
		public void Sequence()
		{
			GiveText("ab");
			WhenParse(Parse.Sequence(
				Parse.Match("a"),
				Parse.Match("b")
				).Merge()
			);
			ThenResultShouldBe("ab");
		}


	}
}
