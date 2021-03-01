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
			GiveText("where name = 1");
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
	}
}