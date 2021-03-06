using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class ExecTest : ParseTestBase
	{
		[Fact]
		public void Exec_name_str()
		{
			GivenText("EXEC sys.sp_changedbowner 'sa'");
			WhenParse(SqlParser.ExecExpr);
			ThenResultShouldBe(new SqlExecExpression()
			{
				Name = new ObjectNameExpression()
				{
					Name = "sys.sp_changedbowner"
				},
				Parameters = new SqlExpression[]
				{
					new SqlStringExpression()
					{
						Text = "sa"
					}
				}
			});
		}
	}
}