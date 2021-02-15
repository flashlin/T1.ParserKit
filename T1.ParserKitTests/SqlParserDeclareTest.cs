using System;
using System.Linq;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserDeclareTest
	{
		private string _code;
		private ITextSpan[] _result;
		private SqlParser _parser;

		[Fact]
		public void Variable_datetime()
		{
			GiveText("declare @name datetime");
			WhenParse();
			ThenResultShouldBe(new DeclareExpression()
			{
				File = String.Empty,
				Content = _code,
				Position = 0,
				Length = _code.Length,
				Name = "@name",
				DataType = "datetime"
			});
		}

		private void WhenParse()
		{
			_parser = new SqlParser();
			_result = _parser.ParseText(_code).Cast<ITextSpan>().ToArray();
		}

		private void GiveText(string code)
		{
			_code = code;
		}

		private void ThenResultShouldBe(SqlExpression expression)
		{
			expression.ToExpectedObject()
				.ShouldMatch(_result[0]);
		}
	}
}