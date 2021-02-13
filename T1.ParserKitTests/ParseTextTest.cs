using System;
using System.Linq;
using ExpectedObjects;
using T1.ParserKit.Core;
using Xunit;

namespace T1.ParserKitTests
{
	public class ParseTextTest
	{
		private string _text;
		private IParseResult _result;

		[Fact]
		public void Letters()
		{
			GiveText("abc");
			WhenParse(Parse.Letters());
			ThenResultShouldBe(new []{ "abc" }, 3);
		}

		[Fact]
		public void Digits()
		{
			GiveText("123");
			WhenParse(Parse.Digits());
			ThenResultShouldBe(new []{ "123" }, 3);
		}

		[Fact]
		public void NotDigit_Letter()
		{
			GiveText("a");
			WhenParse(Parse.Digit().Not().ThenRight(Parse.Letter()));
			ThenResultShouldBe(new []{ "a" }, 1);
		}

		[Fact]
		public void NotDigit()
		{
			GiveText("a");
			WhenParse(Parse.Digit().Not());
			ThenResultShouldBe(new string[0], 0);
		}

		[Fact]
		public void CStyleIdentifier()
		{
			GiveText("name");
			WhenParse(Parse.CStyleIdentifier());
			ThenResultShouldBe(new []{ "name" }, 4);
		}




		private void ThenResultShouldBe(string[] expecteds, int consumed)
		{
			var strings = _result.Result.Select(x => x.GetText()).ToArray();
			strings.ToExpectedObject()
				.ShouldMatch(expecteds);
		}

		private void WhenParse(IParser parser)
		{
			var inp = _text.GetTextSpan();
			_result = parser.TryParse(inp);
		}

		private void GiveText(string text)
		{
			_text = text;
		}
	}
}
