using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserArithmeticTest : ParseTestBase
	{
		[Fact]
		public void Add_1_2()
		{
			GiveText("1 + 2");	
			WhenParse(SqlParser.ArithmeticOperatorAtomExpr);
			ThenResultShouldBe(new ArithmeticOperatorExpression()
			{
				Left = new NumberExpression()
				{
					Value = 1,
					ValueTypeFullname = typeof(int).FullName
				},
				Oper = "+",
				Right = new NumberExpression()
				{
					Value = 2,
					ValueTypeFullname = typeof(int).FullName
				}
			});
		}
	}
}