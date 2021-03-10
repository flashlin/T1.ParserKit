using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class DeleteTest : ParseTestBase
	{
		[Fact]
		public void Delete_from_table_where_field_in_select_field_from_table()
		{
			GivenText("delete from customer where id in (select custid from oldCustomer)");	
			WhenParse(SqlParser.DeleteExpr);
			ThenResultShouldBe(new SqlDeleteExpression()
			{
				From = new ObjectNameExpression()
				{
					Name = "customer"
				},
				Where = new SqlWhereExpression()
				{
					Filter = new SqlFilterExpression()
					{
						Left = new ObjectNameExpression()
						{
							Name = "id"
						},
						Oper = "IN",
						Right = new SqlSelectExpression()
						{
							Fields = new SqlExpression[]
							{
								new SqlTableFieldExpression()
								{
									Name = "custid"
								}
							},
							From = new SqlTableExpression()
							{
								Name = "oldCustomer"
							}
						}
					}
				}
			});
		}
	}
}