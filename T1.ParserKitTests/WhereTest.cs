using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class WhereTest : ParseTestBase
	{
		[Fact]
		public void Where_name_eq_1()
		{
			GivenText("where name = 1");
			WhenParse(SqlParser.WhereExpr);
			ThenResultShouldBe(new WhereExpression()
			{
				Filter = new FilterExpression()
				{
					Left = new FieldExpression()
					{
						Name = "name"
					},
					Oper = "=",
					Right = new NumberExpression()
					{
						ValueTypeFullname = typeof(int).FullName,
						Value = 1
					}
				}
			});
		}

		[Fact]
		public void NString_not_like_NString()
		{
			GivenText("Where N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'");
			WhenParse(SqlParser.WhereExpr);
			ThenResultShouldBe(new WhereExpression()
			{
				Filter = new FilterExpression
				{
					Left = new SqlExpression
					{
						TextSpan = new TextSpan
						{
							File = "",
							Text = "N'$(__IsSqlCmdEnabled)'",
							Position = 6,
							Length = 23
						}
					},
					Oper = "NOT LIKE",
					Right = new SqlExpression
					{
						TextSpan = new TextSpan
						{
							File = "",
							Text = "N'True'",
							Position = 39,
							Length = 7
						}
					},
				}
			});
		}
	}
}