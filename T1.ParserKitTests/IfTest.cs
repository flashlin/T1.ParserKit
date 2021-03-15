using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class IfTest : ParseTestBase
	{
		[Fact]
		public void If_exists_select_1_from_table_begin_select_1_end()
		{
			GivenText(@"
if exists (select 1 from customer where id is null)
Begin
select 1
END");
			WhenParse(SqlParser.IfExprs);
			ThenResultShouldBe(new SqlIfExpression()
			{
				Condition = new SqlFilterExpression()
				{
					Oper = "bool",
					Right = new SqlFuncExistsExpression()
					{
						Name = "EXISTS",
						Parameters = new SqlExpression[]
						{
							new SqlSelectExpression()
							{
								Fields = new SqlExpression[]
								{
									new SqlNumberExpression()
									{
										Value = 1,
										ValueTypeFullname = typeof(int).FullName
									}
								},
								From = new SqlTableExpression()
								{
									Name = "customer"
								},
								Where = new SqlWhereExpression()
								{
									Filter = new SqlFilterExpression()
									{
										Left = new SqlTableFieldExpression()
										{
											Name = "id"
										},
										Oper = "IS",
										Right = new SqlNullExpression()
									}
								}
							}
						}
					}
				},
				Body = new StatementsExpression()
				{
					Items = new SqlExpression[]
					{
						new SqlSimpleExpression()
						{
							Value = new SqlNumberExpression()
							{
								Value = 1,
								ValueTypeFullname = typeof(int).FullName
							}
						}
					}
				}
			});
		}
	}
}