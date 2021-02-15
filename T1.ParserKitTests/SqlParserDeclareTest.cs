using System;
using System.Collections.Generic;
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

		[Fact]
		public void Set_nocount_on()
		{
			GiveText("SET NOCOUNT ON;");
			WhenParse();
			ThenResultShouldBe(new SetOptionExpression()
			{
				File = String.Empty,
				Content = _code,
				Position = 0,
				Length = _code.Length,
				OptionName = "NOCOUNT",
				IsToggle = true
			});
		}

		[Fact]
		public void If_variable_eq_1_begin_select_field_from_table_end()
		{
			GiveText("if @name=1 BEGIN select name from customer END");
			WhenParse();
			ThenResultShouldBe(new IfExpression()
			{
				File = String.Empty,
				Content = _code,
				Position = 0,
				Length = _code.Length,
				Condition = new FilterExpression
				{
					Left = new VariableExpression
					{
						Name = "@name",
						File = "",
						Length = 5,
						Position = 3,
						Content = _code
					},
					Oper = "=",
					Right = new NumberExpression
					{
						Value = 1,
						ValueTypeFullname = typeof(int).FullName,
						File = "",
						Length = 1,
						Position = 9,
						Content = _code
					},
					File = "",
					Length = 7,
					Position = 3,
					Content = _code
				},
				Body = new StatementsExpression
				{
					Items = new SqlExpression[] { new SelectExpression
					{
						Fields = new FieldsExpression
						{
							Items = new List<SqlExpression> {
								new FieldExpression
								{
									Name = "name",
									File = string.Empty,
									Length = 4,
									Position = 24,
									Content = _code
								}},
								File = string.Empty,
								Length = 4,
								Position = 24,
								Content = _code
							},
							From = new TableExpression
							{
								Name = "customer",
								File = string.Empty,
								Length = 8,
								Position = 34,
								Content = _code
							},
							File = string.Empty,
							Length = 25,
							Position = 17,
							Content = _code
						}},
					File = "",
					Length = 25,
					Position = 17,
					Content = "if @name=1 BEGIN select name from customer END"
				}
			});
		}

		[Fact]
		public void If_variable_eq_1_begin_nest_if_select_field_from_table_end()
		{
			GiveText("if @name=1 BEGIN if @id=2 BEGIN select name from customer END END");
			WhenParse();
			ThenResultShouldBe(new IfExpression()
			{
				File = String.Empty,
				Content = _code,
				Position = 0,
				Length = _code.Length,
				Condition = new FilterExpression
				{
					Left = new VariableExpression
					{
						Name = "@name",
						File = "",
						Length = 5,
						Position = 3,
						Content = _code
					},
					Oper = "=",
					Right = new NumberExpression
					{
						Value = 1,
						ValueTypeFullname = typeof(int).FullName,
						File = "",
						Length = 1,
						Position = 9,
						Content = _code
					},
					File = "",
					Length = 7,
					Position = 3,
					Content = _code
				},
				Body = new StatementsExpression
				{
					Items = new SqlExpression[] { new SelectExpression
					{
						Fields = new FieldsExpression
						{
							Items = new List<SqlExpression> {
								new FieldExpression
								{
									Name = "name",
									File = string.Empty,
									Length = 4,
									Position = 24,
									Content = _code
								}},
								File = string.Empty,
								Length = 4,
								Position = 24,
								Content = _code
							},
							From = new TableExpression
							{
								Name = "customer",
								File = string.Empty,
								Length = 8,
								Position = 34,
								Content = _code
							},
							File = string.Empty,
							Length = 25,
							Position = 17,
							Content = _code
						}},
					File = "",
					Length = 25,
					Position = 17,
					Content = "if @name=1 BEGIN select name from customer END"
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