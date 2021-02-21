using ExpectedObjects;
using T1.ParserKit.Core;

namespace T1.ParserKitTests
{
	public abstract class ParseTestBase
	{
		private string _text;
		private IParseResult<object> _parsed;

		protected void ThenResultShouldBe<T>(T expected)
		{
			if (!_parsed.IsSuccess() )
			{
				throw new ParseException(_parsed.Error);
			}

			var actualResult = _parsed.Result;
			expected.ToExpectedObject()
				.ShouldMatch(actualResult);
		}

		protected void WhenParse<T>(IParser<T> parser)
		{
			var inp = new StringInputReader(_text);
			_parsed = parser.TryParse(inp).CastToParseResult();
		}

		protected void GiveText(string text)
		{
			_text = text;
		}
	}
}