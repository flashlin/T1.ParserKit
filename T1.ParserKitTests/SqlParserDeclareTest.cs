using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserDeclareTest : ParseTestBase
	{
		[Fact]
		public void Declare_variable_datetime()
		{
			GiveText("declare @name datetime");
			WhenParse(SqlParser.DeclareVariableExpr);
			ThenResultShouldBe(new DeclareExpression()
			{
				Name = new VariableExpression()
				{
					Name = "@name"
				},
				DataType = "datetime"
			});
		}

		[Fact]
		public void Set_nocount_on()
		{
			GiveText("SET NOCOUNT ON;");
			WhenParse(SqlParser.SetNocountExpr);
			ThenResultShouldBe(new SetOptionExpression()
			{
				OptionName = "NOCOUNT",
				IsToggle = true
			});
		}

		[Fact]
		public void Set_nocount_on2()
		{
			GiveText("SET NOCOUNT ON");
			WhenParse(SqlParser.SetNocountExpr);
			ThenResultShouldBe(new SetOptionExpression()
			{
				OptionName = "NOCOUNT",
				IsToggle = true
			});
		}

		//[Fact]
		//public void If_variable_eq_1_begin_select_field_from_table_end()
		//{
		//	GiveText("if @name=1 BEGIN select name from customer END");
		//	WhenParse();
		//	ThenResultShouldBe(new IfExpression()
		//	{
		//		File = String.Empty,
		//		Content = _code,
		//		Position = 0,
		//		Length = _code.Length,
		//		Condition = new FilterExpression
		//		{
		//			Left = new VariableExpression
		//			{
		//				Name = "@name",
		//				File = "",
		//				Length = 5,
		//				Position = 3,
		//				Content = _code
		//			},
		//			Oper = "=",
		//			Right = new NumberExpression
		//			{
		//				Value = 1,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 9,
		//				Content = _code
		//			},
		//			File = "",
		//			Length = 7,
		//			Position = 3,
		//			Content = _code
		//		},
		//		Body = new StatementsExpression
		//		{
		//			Items = new SqlExpression[] { new SelectExpression
		//			{
		//				Fields = new FieldsExpression
		//				{
		//					Items = new List<SqlExpression> {
		//						new FieldExpression
		//						{
		//							Name = "name",
		//							File = string.Empty,
		//							Length = 4,
		//							Position = 24,
		//							Content = _code
		//						}},
		//						File = string.Empty,
		//						Length = 4,
		//						Position = 24,
		//						Content = _code
		//					},
		//					From = new TableExpression
		//					{
		//						Name = "customer",
		//						File = string.Empty,
		//						Length = 8,
		//						Position = 34,
		//						Content = _code
		//					},
		//					File = string.Empty,
		//					Length = 25,
		//					Position = 17,
		//					Content = _code
		//				}},
		//			File = "",
		//			Length = 25,
		//			Position = 17,
		//			Content = "if @name=1 BEGIN select name from customer END"
		//		}
		//	});
		//}

		//[Fact]
		//public void If_L_variable_eq_1_R_begin_select_field_from_table_end()
		//{
		//	GiveText("if (@name=1) BEGIN select name from customer END");
		//	WhenParse();
		//	ThenResultShouldBe(new IfExpression()
		//	{
		//		File = String.Empty,
		//		Content = _code,
		//		Position = 0,
		//		Length = _code.Length,
		//		Condition = new FilterExpression
		//		{
		//			Left = new VariableExpression
		//			{
		//				Name = "@name",
		//				File = "",
		//				Length = 5,
		//				Position = 4,
		//				Content = _code
		//			},
		//			Oper = "=",
		//			Right = new NumberExpression
		//			{
		//				Value = 1,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 10,
		//				Content = _code
		//			},
		//			File = "",
		//			Length = 9,
		//			Position = 3,
		//			Content = _code
		//		},
		//		Body = new StatementsExpression
		//		{
		//			Items = new SqlExpression[]
		//			{
		//				new SelectExpression
		//				{
		//					Fields = new FieldsExpression
		//					{
		//						Items = new List<SqlExpression>
		//						{
		//							new FieldExpression
		//							{
		//								Name = "name",
		//								File = string.Empty,
		//								Length = 4,
		//								Position = 26,
		//								Content = _code
		//							}
		//						},
		//						File = string.Empty,
		//						Length = 4,
		//						Position = 26,
		//						Content = _code
		//					},
		//					From = new TableExpression
		//					{
		//						Name = "customer",
		//						File = string.Empty,
		//						Length = 8,
		//						Position = 36,
		//						Content = _code
		//					},
		//					File = string.Empty,
		//					Length = 25,
		//					Position = 19,
		//					Content = _code
		//				}
		//			},
		//			File = "",
		//			Length = 25,
		//			Position = 19,
		//			Content = _code
		//		}
		//	});
		//}

		//[Fact]
		//public void If_variable_eq_1_begin_nest_if_select_field_from_table_end()
		//{
		//	GiveText("if @name=1 BEGIN if @id=2 BEGIN select name from customer END END");
		//	WhenParse();
		//	var actual = (IfExpression)_result[0];

		//	new FilterExpression
		//	{
		//		Left = new VariableExpression
		//		{
		//			Name = "@name",
		//			File = "",
		//			Length = 5,
		//			Position = 3,
		//			Content = _code
		//		},
		//		Oper = "=",
		//		Right = new NumberExpression
		//		{
		//			Value = 1,
		//			ValueTypeFullname = "System.Int32",
		//			File = "",
		//			Length = 1,
		//			Position = 9,
		//			Content = _code
		//		},
		//		File = "",
		//		Length = 7,
		//		Position = 3,
		//		Content = _code
		//	}.ToExpectedObject()
		//		.ShouldMatch(actual.Condition);

		//	new IfExpression()
		//	{
		//		File = "",
		//		Length = 44,
		//		Position = 17,
		//		Content = _code,
		//		Condition = new FilterExpression
		//		{
		//			Left = new VariableExpression
		//			{
		//				Name = "@id",
		//				File = "",
		//				Length = 3,
		//				Position = 20,
		//				Content = _code
		//			},
		//			Oper = "=",
		//			Right = new NumberExpression
		//			{
		//				Value = 2,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 24,
		//				Content = _code
		//			},
		//			File = "",
		//			Length = 5,
		//			Position = 20,
		//			Content = _code
		//		},
		//		Body = new StatementsExpression()
		//		{
		//			File = "",
		//			Length = 25,
		//			Position = 32,
		//			Content = _code,
		//			Items = new SqlExpression[]
		//			{
		//				new SelectExpression
		//				{
		//					Fields = new FieldsExpression
		//					{
		//						 Items = new List<SqlExpression>
		//						 {
		//							 new FieldExpression
		//							 {
		//								  Name = "name",
		//								  File = "",
		//								  Length = 4,
		//								  Position = 39,
		//								  Content = _code
		//							 }
		//						 },
		//						 File = "",
		//						 Length = 4,
		//						 Position = 39,
		//						 Content = _code
		//					},
		//					From = new TableExpression
		//					{
		//						 Name = "customer",
		//						 File = "",
		//						 Length = 8,
		//						 Position = 49,
		//						 Content = _code
		//					},
		//					File = "",
		//					Length = 25,
		//					Position = 32,
		//					Content = _code
		//				}
		//			}
		//		}
		//	}.ToExpectedObject()
		//		.ShouldMatch(actual.Body.Items[0]);
		//}
	}
}