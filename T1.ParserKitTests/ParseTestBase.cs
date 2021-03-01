using System;
using System.IO;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public abstract class ParseTestBase
	{
		private string _text;
		private IParseResult<object> _parsed;
		private string _file;

		protected void ThenResultShouldBe(string expected)
		{
			if (!_parsed.IsSuccess())
			{
				throw new ParseException(_parsed.Error);
			}

			if (_parsed.Result is TextSpan textSpan)
			{
				expected.ToExpectedObject()
					.ShouldMatch(textSpan.Text);
				return;
			}

			var actualResult = (SqlExpression)_parsed.Result;
			expected.ToExpectedObject()
				.ShouldMatch(actualResult.TextSpan.Text);
		}

		protected void ThenResultShouldFail()
		{
			Assert.False(_parsed.IsSuccess());
		}

		protected void ThenResultShouldBe<T>(T expected)
		{
			if (!_parsed.IsSuccess())
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

		protected void ThenResultShouldSuccess()
		{
			if (!_parsed.IsSuccess())
			{
				var parseEx = new ParseException(_parsed.Error);
				throw new Exception(_file, parseEx);
			}
		}

		protected void GiveText(string text)
		{
			_file = String.Empty;
			_text = text;
		}

		protected void GiveTextFile(string file)
		{
			_file = file;
			_text = File.ReadAllText(file);
		}
	}
}