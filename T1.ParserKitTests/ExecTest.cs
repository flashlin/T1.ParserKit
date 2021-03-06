using T1.ParserKit.Core.Parsers;
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

		[Fact]
		public void Exec_name_var1_eq_nstr_var2_eq_str()
		{
			GivenText("exec sys.sp_addextendedproperty @name = N'MS_Description', @value = 'test'");
			WhenParse(SqlParser.ExecExpr);
			ThenResultShouldBe(new SqlExecExpression()
			{
				Name = new ObjectNameExpression()
				{
					Name = "sys.sp_addextendedproperty"
				},
				Parameters = new SqlExpression[]
				{
					new SqlVariableAssignExpression()
					{
						VariableName = new SqlVariableExpression()
						{
							Name = "@name"
						},
						AssignFrom = new SqlStringExpression()
						{
							IsUnicode = true,
							Text = "MS_Description"
						}
					},
					new SqlVariableAssignExpression()
					{
						VariableName = new SqlVariableExpression()
						{
							Name = "@value"
						},
						AssignFrom = new SqlStringExpression()
						{
							Text = "test"
						}
					}
				}
			});
		}
	}
}