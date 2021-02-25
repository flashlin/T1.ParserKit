using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserStartTest : ParseTestBase
	{
		[Fact]
		public void Start_declare_name_int()
		{
			GiveText("declare @name int");	
			WhenParse(SqlParser.StartExpr);
			ThenResultShouldBe(new DeclareExpression()
			{
				Name = new VariableExpression()
				{
					Name = "@name"
				},
				DataType = "int"
			});
		}
	}
}