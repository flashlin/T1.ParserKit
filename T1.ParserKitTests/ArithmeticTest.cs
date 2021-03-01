using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class ArithmeticTest : ParseTestBase
	{
		[Fact]
		public void Arithmetic_1_add_2()
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

		[Fact]
		public void Arithmetic_1_add_2_mul_3()
		{
			GiveText("1 + 2 * 3");	
			WhenParse(SqlParser.ArithmeticOperatorAtomExpr);
			ThenResultShouldBe(new ArithmeticOperatorExpression()
			{
				Left = new NumberExpression()
				{
					Value = 1,
					ValueTypeFullname = typeof(int).FullName
				},
				Oper = "+",
				Right = new ArithmeticOperatorExpression()
				{
					Left = new NumberExpression()
					{
						Value = 2,
						ValueTypeFullname = typeof(int).FullName
					},
					Oper = "*",
					Right = new NumberExpression()
					{
						Value = 3,
						ValueTypeFullname = typeof(int).FullName
					}
				}
			});
		}
	}
}