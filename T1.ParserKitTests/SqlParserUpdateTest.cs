using System;
using System.Linq;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserUpdateTest
	{
		private string _code;
		private ITextSpan[] _result;
		private SqlParser _parser;

		[Fact]
		public void Update_table_set_field1_eq_1()
		{
			GiveText("UPDATE customer set id=1 where custId=@customerId");
			WhenParse();
			ThenResultShouldBe(new UpdateExpression()
			{
				File = string.Empty,
				Position = 0,
				Length = _code.Length,
				Content = _code,
				SetFields = new []
				{
					new UpdateSetFieldExpression()
					{
						File = string.Empty,
						Position = 16,
						Length = 8,
						Content = _code,
						FieldName = "id",
						AssignExpr = new NumberExpression
						{
							Value = 1,
							ValueTypeFullname = typeof(int).FullName,
							File = string.Empty,
							Length = 1,
							Position = 23,
							Content = _code
						},
					}
				},
				WhereExpr = new WhereExpression
				{
					Filter = new FilterExpression
					{
						Left = new FieldExpression
						{
							Name = "custId",
							File = "",
							Length = 6,
							Position = 31,
							Content = _code
						},
						Oper = "=",
						Right = new VariableExpression
						{
							Name = "@customerId",
							File = "",
							Length = 11,
							Position = 38,
							Content = _code
						},
						File = "",
						Length = 18,
						Position = 31,
						Content = _code
					},
					File = "",
					Length = 24,
					Position = 25,
					Content = _code
				}
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