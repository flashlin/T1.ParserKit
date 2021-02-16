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
		private readonly Parse _parse = new Parse();

		[Fact]
		public void Letters()
		{
			GiveText("abc");
			WhenParse(_parse.Letters());
			ThenResultShouldBe(new []{ "abc" }, 3);
		}

		[Fact]
		public void Digits()
		{
			GiveText("123");
			WhenParse(_parse.Digits());
			ThenResultShouldBe(new []{ "123" }, 3);
		}

		[Fact]
		public void NotDigit_Letter()
		{
			GiveText("a");
			WhenParse(_parse.Digit().Not().ThenRight(_parse.Letter()));
			ThenResultShouldBe(new []{ "a" }, 1);
		}

		[Fact]
		public void NotDigit()
		{
			GiveText("a");
			WhenParse(_parse.Digit().Not());
			ThenResultShouldBe(new string[0], 0);
		}

		[Fact]
		public void CStyleIdentifier()
		{
			GiveText("name");
			WhenParse(_parse.CStyleIdentifier());
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
