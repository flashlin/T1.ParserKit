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
			GiveText("UPDATE customer set id=1 where custId=@customerId");
			WhenParse(SqlParser.UpdateExpr(SqlParser.Atom));
			ThenResultShouldBe(new UpdateExpression()
			{
				SetFields = new[]
				{
					new UpdateSetFieldExpression()
					{
						FieldName = "id",
						AssignExpr = new NumberExpression
						{
							Value = 1,
							ValueTypeFullname = typeof(int).FullName,
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
						},
						Oper = "=",
						Right = new VariableExpression
						{
							Name = "@customerId",
						},
					},
				}
			});
		}
	}
}