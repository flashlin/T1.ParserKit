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
	public class SqlParserPartialTest
	{
		private string _text;
		private readonly SqlParser _parser;
		private IParseResult _parsed;

		public SqlParserPartialTest()
		{
			_parser = new SqlParser();
		}

		[Fact]
		public void Keyword()
		{
			var code = "from";
			var rc = SqlTokenizer.KeywordsParser.ParseText(code);
			Assert.Equal("from", rc[0].GetText());
		}

		[Fact]
		public void SelectKeyword()
		{
			var code = "select";
			var parsed = SqlTokenizer.KeywordsParser.TryParseAllText(code);
			Assert.True(parsed.IsSuccess());
		}

		[Fact]
		public void NotSqlIdentifier()
		{
			GiveText("from");
			Assert.Throws<ParseException>(() => _parser.Identifier().ParseText(_text));
		}

		[Fact]
		public void SqlIdentifier()
		{
			GiveText("name");
			WhenTryParse(_parser.Identifier());
			ThenResultShouldBe("name", 4);
		}

		[Fact]
		public void SqlIdentifier_from()
		{
			GiveText("name from");
			WhenTryParse(_parser.Identifier());
			Assert.Equal("name", _parsed.Result[0].GetText());
			ThenResultShouldBe("name", 4);
		}

		[Fact]
		public void Field()
		{
			GiveText("name from");
			WhenTryParse(_parser.FieldExpr);
			ThenResultShouldBe(new FieldExpression()
			{
				Name = "name",
				File = "",
				Content = _text,
				Length = 4
			});
		}

		[Fact]
		public void Field_aliasName()
		{
			GiveText("name n1");
			WhenTryParse(_parser.FieldExpr);
			ThenResultShouldBe(new FieldExpression()
			{
				Name = "name",
				AliasName = "n1",
				File = "",
				Content = _text,
				Length = 7
			});
		}

		[Fact]
		public void WithNolock()
		{
			GiveText("with(nolock)");
			WhenTryParse(_parser.WithOptionExpr);
			ThenResultShouldBe(new WithOptionExpression()
			{
				File = "",
				Content = _text,
				Length = _text.Length,
				Nolock = true
			});
		}



		private void ThenResultShouldBe(SqlExpression expression)
		{
			expression.ToExpectedObject()
				.ShouldMatch(_parsed.Result[0]);
		}

		private void ThenResultShouldBe(string expectedText, int consumed)
		{
			Assert.Equal(expectedText, _parsed.Result[0].GetText());
			Assert.Equal(consumed, _parsed.Rest.Position);
		}

		private void WhenTryParse(IParser parser)
		{
			_parsed = parser.TryParseText(_text);
		}

		private void GiveText(string code)
		{
			_text = code;
		}
	}
}
