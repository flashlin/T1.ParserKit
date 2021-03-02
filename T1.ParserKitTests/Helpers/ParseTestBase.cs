using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests.Helpers
{
	public abstract class ParseTestBase
	{
		protected string _text;
		private string _file;
		private IParseResult<object>[] _parsedList;
		protected IParseResult<object> _parsed => _parsedList.Last();

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

			var actualName = actualResult.GetType().FullName;
			var expectedName = typeof(T).FullName;
			if (actualName != expectedName)
			{
				throw new Exception($"Expect {expectedName} result, but got {actualName} result.");
			}

			expected.ToExpectedObject()
				.ShouldMatch(actualResult);
		}

		protected void WhenParse<T>(IParser<T> parser)
		{
			var inp = new StringInputReader(_text);
			_parsedList = new[]
			{
				parser.TryParse(inp).CastToParseResult()
			};
		}

		protected void WhenParseAll<T>(IParser<T> parser)
		{
			var inp = new StringInputReader(_text);
			_parsedList = parser.TryParseAllText(_text)
				.Select(x => x.CastToParseResult())
				.ToArray();
		}

		protected void ThenResultShouldSuccess()
		{
			if (!_parsed.IsSuccess())
			{
				var parseEx = new ParseException(_parsed.Error);
				throw new Exception(_file, parseEx);
			}
		}

		protected void GivenText(string text)
		{
			_file = String.Empty;
			_text = text;
		}

		protected void GivenTextFile(string file)
		{
			_file = file;
			_text = File.ReadAllText(file);
		}
	}
}