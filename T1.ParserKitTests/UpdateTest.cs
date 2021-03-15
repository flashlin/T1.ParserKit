using System;
using System.Linq;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class UpdateTest : ParseTestBase
	{
		[Fact]
		public void Update_table_set_field1_eq_1()
		{
			GivenText("UPDATE customer set id=1 where custId=@customerId");
			WhenParse(SqlParser.UpdateExpr);
			ThenResultShouldBe(new UpdateExpression()
			{
				Table = new SqlObjectNameExpression()
				{
					Name = "customer"
				},
				SetFields = new[]
				{
					new UpdateSetFieldExpression()
					{
						FieldName = "id",
						AssignExpr = new SqlNumberExpression
						{
							Value = 1,
							ValueTypeFullname = typeof(int).FullName,
						},
					}
				},
				WhereExpr = new SqlWhereExpression
				{
					Filter = new SqlFilterExpression
					{
						Left = new SqlTableFieldExpression
						{
							Name = "custId",
						},
						Oper = "=",
						Right = new SqlVariableExpression
						{
							Name = "@customerId",
						},
					},
				}
			});
		}

		[Fact]
		public void Update_table_set_field1_eq_field2_add_1()
		{
			GivenText("Update exchange set ForecastRate = actualrate + 1");
			WhenParse(SqlParser.UpdateExpr);
			ThenResultShouldBe(new UpdateExpression()
			{
				Table = new SqlObjectNameExpression()
				{
					Name = "exchange"
				},
				SetFields = new[]
				{
					new UpdateSetFieldExpression()
					{
						FieldName = "ForecastRate",
						AssignExpr = new ArithmeticOperatorExpression()
						{
							Left = new SqlTableFieldExpression()
							{
								Name = "actualrate"
							},
							Oper = "+",
							Right = new SqlNumberExpression()
							{
								Value = 1,
								ValueTypeFullname = typeof(int).FullName
							}
						}
					}
				},
			});
		}
	}
}