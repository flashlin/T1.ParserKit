using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class CreateStoredProcedureTest : ParseTestBase
	{
		[Fact]
		public void Create_proc_name()
		{
			GivenText(@"
CREATE PROCEDURE [dbo].[AddCustomer]
	@customerId int,
	@name nvarchar(10),
	@amount decimal(19,6)
AS
BEGIN
	SET NOCOUNT ON;
END
");
			WhenParse(SqlParser.CreateStoredProcedureExpr);
			ThenResultShouldBe(new SqlCreateStoredProcedureExpression()
			{
				Name = new ObjectNameExpression()
				{
					Name = "[dbo].[AddCustomer]"
				},
				Parameters = new[]
				{
					new SqlParameterExpression()
					{
						Name = new VariableExpression()
						{
							Name = "@customerId"
						},
						DataType = new SqlDataTypeExpression()
						{
							DataType = "int",
						}
					},
					new SqlParameterExpression()
					{
						Name = new VariableExpression()
						{
							Name = "@name",
						},
						DataType = new SqlDataTypeExpression()
						{
							DataType = "nvarchar",
							Size = 10
						}
					},
					new SqlParameterExpression()
					{
						Name = new VariableExpression()
						{
							Name = "@amount"
						},
						DataType = new SqlDataTypeExpression()
						{
							DataType = "decimal",
							Size = 19,
							Scale = 6
						}
					},
				},
				Body = new SqlExpression[]
				{
					new SetOptionExpression()
					{
						OptionName = "NOCOUNT",
						IsToggle = true
					}
				}
			});
		}
	}
}