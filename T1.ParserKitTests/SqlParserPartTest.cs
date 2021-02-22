using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserPartTest : ParseTestBase
	{
		[Fact]
		public void Table_name()
		{
			GiveText("customer.name");
			WhenParse(SqlParser.TableFieldExpr2);
			ThenResultShouldBe(new FieldExpression()
			{
				Name = "name",
				From = "customer"
			});
		}
	}
}