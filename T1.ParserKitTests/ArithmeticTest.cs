using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class ArithmeticTest : ParseTestBase
	{
		[Fact]
		public void Arithmetic_1_add_2()
		{
			GivenText("1 + 2");	
			WhenParse(SqlParser.ArithmeticOperatorAtomExpr);
			ThenResultShouldBe(new SqlArithmeticOperatorExpression()
			{
				Left = new SqlNumberExpression()
				{
					Value = 1,
					ValueTypeFullname = typeof(int).FullName
				},
				Oper = "+",
				Right = new SqlNumberExpression()
				{
					Value = 2,
					ValueTypeFullname = typeof(int).FullName
				}
			});
		}

		[Fact]
		public void Arithmetic_1_add_2_mul_3()
		{
			GivenText("1 + 2 * 3");	
			WhenParse(SqlParser.ArithmeticOperatorAtomExpr);
			ThenResultShouldBe(new SqlArithmeticOperatorExpression()
			{
				Left = new SqlNumberExpression()
				{
					Value = 1,
					ValueTypeFullname = typeof(int).FullName
				},
				Oper = "+",
				Right = new SqlArithmeticOperatorExpression()
				{
					Left = new SqlNumberExpression()
					{
						Value = 2,
						ValueTypeFullname = typeof(int).FullName
					},
					Oper = "*",
					Right = new SqlNumberExpression()
					{
						Value = 3,
						ValueTypeFullname = typeof(int).FullName
					}
				}
			});
		}

		[Fact]
		public void Arithmetic_1_add_2_mul_3_add_4()
		{
			GivenText("1 + 2 * 3 + 4");
			WhenParse(SqlParser.ArithmeticOperatorAtomExpr);
			ThenResultShouldBe(new SqlArithmeticOperatorExpression()
			{
				Oper = "+",
				Left = new SqlArithmeticOperatorExpression
				{
					Left = new SqlNumberExpression
					{
						Value = 1,
						ValueTypeFullname = typeof(int).FullName,
					},
					Oper = "+",
					Right = new SqlArithmeticOperatorExpression
					{
						Left = new SqlNumberExpression
						{
							Value = 2,
							ValueTypeFullname = typeof(int).FullName,
						},
						Oper = "*",
						Right = new SqlNumberExpression
						{
							Value = 3,
							ValueTypeFullname = typeof(int).FullName,
						},
					},
				},
				Right = new SqlNumberExpression
				{
					Value = 4,
					ValueTypeFullname = typeof(int).FullName,
				}
			});
		}

		[Fact]
		public void Arithmetic_Group_LParen_1_add_2_RParen_mul_3()
		{
			GivenText("(1 + 2) * 3");
			WhenParse(SqlParser.ArithmeticOperatorAtomExpr);
			ThenResultShouldBe(new SqlArithmeticOperatorExpression()
			{
				Left = new SqlArithmeticOperatorExpression
				{
					Left = new SqlNumberExpression
					{
						Value = 1,
						ValueTypeFullname = typeof(int).FullName,
					},
					Oper = "+",
					Right = new SqlNumberExpression
					{
						Value = 2,
						ValueTypeFullname = typeof(int).FullName,
					},
				},
				Oper = "*",
				Right = new SqlNumberExpression
				{
					Value = 3,
					ValueTypeFullname = typeof(int).FullName,
				}
			});
		}
	}
}