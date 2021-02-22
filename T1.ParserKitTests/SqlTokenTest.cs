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
	public class SqlTokenTest : ParseTestBase
	{
		[Fact]
		public void Keyword()
		{
			GiveText("from");
			WhenParse(SqlParser.Identifier);
			ThenResultShouldFail();
		}

		//[Fact]
		//public void SelectKeyword()
		//{
		//	var code = "select";
		//	var parsed = SqlTokenizer.KeywordsParser.TryParseAllText(code);
		//	Assert.True(parsed.IsSuccess());
		//}

		//[Fact]
		//public void NotSqlIdentifier()
		//{
		//	GiveText("from");
		//	Assert.Throws<ParseException>(() => _parser.Identifier().ParseText(_text));
		//}

		//[Fact]
		//public void SqlIdentifier()
		//{
		//	GiveText("name");
		//	WhenTryParse(_parser.Identifier());
		//	ThenResultShouldBe("name", 4);
		//}

		//[Theory]
		//[InlineData("where")]
		//public void Identifier_Parse_Keywords(string keyword)
		//{
		//	GiveText(keyword);
		//	WhenTryParse(_parser.Identifier());
		//	ThenResultShouldFail();
		//}


		//[Fact]
		//public void SqlIdentifier_from()
		//{
		//	GiveText("name from");
		//	WhenTryParse(_parser.Identifier());
		//	Assert.Equal("name", _parsed.Result.GetText());
		//	ThenResultShouldBe("name", 4);
		//}

		//[Fact]
		//public void Field()
		//{
		//	GiveText("name from");
		//	WhenTryParse(_parser.FieldExpr);
		//	ThenResultShouldBe(new FieldExpression()
		//	{
		//		Name = "name",
		//		File = "",
		//		Content = _text,
		//		Length = 4
		//	});
		//}

		//[Fact]
		//public void Field_aliasName()
		//{
		//	GiveText("name n1");
		//	WhenTryParse(_parser.FieldExpr);
		//	ThenResultShouldBe(new FieldExpression()
		//	{
		//		Name = "name",
		//		AliasName = "n1",
		//		File = "",
		//		Content = _text,
		//		Length = 7
		//	});
		//}

		//[Fact]
		//public void WithNolock()
		//{
		//	GiveText("with(nolock)");
		//	WhenTryParse(_parser.WithOptionExpr);
		//	ThenResultShouldBe(new WithOptionExpression()
		//	{
		//		File = "",
		//		Content = _text,
		//		Length = _text.Length,
		//		Nolock = true
		//	});
		//}

		//[Fact]
		//public void Table_where()
		//{
		//	GiveText("customer where");
		//	WhenTryParse(_parser.TableExpr);
		//	ThenResultShouldBe(new TableExpression()
		//	{
		//		File = "",
		//		Content = _text,
		//		Length = 8,
		//		Name = "customer"
		//	});
		//}



		//[Fact]
		//public void Where_name_eq_1()
		//{
		//	GiveText("where name = 1");
		//	WhenTryParse(_parser.WhereExpr);
		//	ThenResultShouldBe(new WhereExpression()
		//	{
		//		File = "",
		//		Content = _text,
		//		Length = _text.Length,
		//		Position = 0,
		//		Filter = new FilterExpression()
		//		{
		//			File = "",
		//			Content = _text,
		//			Length = 8,
		//			Position = 6,
		//			Left = new FieldExpression()
		//			{
		//				File = "",
		//				Content = _text,
		//				Length = 4,
		//				Position = 6,
		//				Name = "name"
		//			},
		//			Oper = "=",
		//			Right = new NumberExpression()
		//			{
		//				File = "",
		//				Content = _text,
		//				Length = 1,
		//				Position = 13,
		//				ValueTypeFullname = typeof(int).FullName,
		//				Value = 1
		//			}
		//		}
		//	});
		//}

		//[Fact]
		//public void Arithmetic_1_add_2_mul_3()
		//{
		//	GiveText("1 + 2 * 3");
		//	WhenTryParse(_parser.ArithmeticOperatorExpr());
		//	ThenResultShouldBe(new ArithmeticOperatorExpression()
		//	{
		//		File = "",
		//		Content = _text,
		//		Length = _text.Length,
		//		Position = 0,
		//		Left = new NumberExpression()
		//		{
		//			Value = 1,
		//			ValueTypeFullname = typeof(int).FullName,
		//			File = "",
		//			Length = 1,
		//			Position = 0,
		//			Content = _text
		//		},
		//		Oper = "+",
		//		Right = new ArithmeticOperatorExpression
		//		{
		//			Left = new NumberExpression
		//			{
		//				Value = 2,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 4,
		//				Content = _text
		//			},
		//			Oper = "*",
		//			Right = new NumberExpression
		//			{
		//				Value = 3,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 8,
		//				Content = _text
		//			},
		//			File = "",
		//			Length = 5,
		//			Position = 4,
		//			Content = _text
		//		}
		//	});
		//}

		//[Fact]
		//public void Arithmetic_1_mul_2_add_3()
		//{
		//	GiveText("1 * 2 + 3");
		//	WhenTryParse(_parser.ArithmeticOperatorExpr());
		//	ThenResultShouldBe(new ArithmeticOperatorExpression()
		//	{
		//		File = "",
		//		Content = _text,
		//		Length = _text.Length,
		//		Position = 0,
		//		Left = new ArithmeticOperatorExpression
		//		{
		//			File = "",
		//			Length = 5,
		//			Position = 0,
		//			Content = _text,
		//			Left = new NumberExpression
		//			{
		//				Value = 1,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 0,
		//				Content = _text
		//			},
		//			Oper = "*",
		//			Right = new NumberExpression
		//			{
		//				Value = 2,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 4,
		//				Content = _text
		//			},
		//		},
		//		Oper = "+",
		//		Right = new NumberExpression()
		//		{
		//			Value = 3,
		//			ValueTypeFullname = typeof(int).FullName,
		//			File = "",
		//			Length = 1,
		//			Position = 8,
		//			Content = _text
		//		},
		//	});
		//}

		//[Fact]
		//public void Arithmetic_1_add_2_mul_3_add_4()
		//{
		//	GiveText("1 + 2 * 3 + 4");
		//	WhenTryParse(_parser.ArithmeticOperatorExpr());
		//	ThenResultShouldBe(new ArithmeticOperatorExpression()
		//	{
		//		File = "",
		//		Content = _text,
		//		Length = _text.Length,
		//		Position = 0,
		//		Oper = "+",
		//		Left = new ArithmeticOperatorExpression
		//		{
		//			Left = new NumberExpression
		//			{
		//				Value = 1,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 0,
		//				Content = _text
		//			},
		//			Oper = "+",
		//			Right = new ArithmeticOperatorExpression
		//			{
		//				Left = new NumberExpression
		//				{
		//					Value = 2,
		//					ValueTypeFullname = typeof(int).FullName,
		//					File = "",
		//					Length = 1,
		//					Position = 4,
		//					Content = _text
		//				},
		//				Oper = "*",
		//				Right = new NumberExpression
		//				{
		//					Value = 3,
		//					ValueTypeFullname = typeof(int).FullName,
		//					File = "",
		//					Length = 1,
		//					Position = 8,
		//					Content = _text
		//				},
		//				File = "",
		//				Length = 5,
		//				Position = 4,
		//				Content = _text
		//			},
		//			File = "",
		//			Length = 9,
		//			Position = 0,
		//			Content = _text
		//		},
		//		Right = new NumberExpression
		//		{
		//			Value = 4,
		//			ValueTypeFullname = typeof(int).FullName,
		//			File = "",
		//			Length = 1,
		//			Position = 12,
		//			Content = _text
		//		}
		//	});
		//}

		//[Fact]
		//public void Arithmetic_Group_LParen_1_add_2_RParen_mul_3()
		//{
		//	GiveText("(1 + 2) * 3");
		//	WhenTryParse(_parser.ArithmeticOperatorExpr());
		//	ThenResultShouldBe(new ArithmeticOperatorExpression()
		//	{
		//		File = "",
		//		Content = _text,
		//		Length = _text.Length,
		//		Position = 0,
		//		Left = new ArithmeticOperatorExpression
		//		{
		//			File = "",
		//			Length = 7,
		//			Position = 0,
		//			Content = _text,
		//			Left = new NumberExpression
		//			{
		//				Value = 1,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 1,
		//				Content = _text
		//			},
		//			Oper = "+",
		//			Right = new NumberExpression
		//			{
		//				Value = 2,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 5,
		//				Content = _text
		//			},
		//		},
		//		Oper = "*",
		//		Right = new NumberExpression
		//		{
		//			Value = 3,
		//			ValueTypeFullname = typeof(int).FullName,
		//			File = "",
		//			Length = 1,
		//			Position = 10,
		//			Content = _text
		//		}
		//	});
		//}

		//[Fact]
		//public void Declare_variable_datatype()
		//{
		//	GiveText("DECLARE @start datetime");
		//	WhenTryParse(_parser.DeclareVariableExpr);
		//	ThenResultShouldBe(new DeclareExpression()
		//	{
		//		File = "",
		//		Content = _text,
		//		Length = _text.Length,
		//		Position = 0,
		//		Name = "@start",
		//		DataType = "datetime"
		//	});
		//}



		//private void ThenResultShouldBe(SqlExpression expression)
		//{
		//	if (!_parsed.IsSuccess())
		//	{
		//		throw new ParseException(_parsed.Error);
		//	}

		//	expression.ToExpectedObject()
		//		.ShouldMatch(_parsed.Result);
		//}

		//private void ThenResultShouldSuccess()
		//{
		//	Assert.True(_parsed.IsSuccess());
		//}

		//private void ThenResultShouldFail()
		//{
		//	Assert.False(_parsed.IsSuccess());
		//}

		//private void ThenResultShouldBe(string expectedText, int consumed)
		//{
		//	Assert.Equal(expectedText, _parsed.Result.GetText());
		//	//Assert.Equal(consumed, _parsed.Rest.Position);
		//}

		//private void WhenTryParse(IParser parser)
		//{
		//	_parsed = parser.TryParseText(_text);
		//}

		//private void GiveText(string code)
		//{
		//	_text = code;
		//}
	}
}
