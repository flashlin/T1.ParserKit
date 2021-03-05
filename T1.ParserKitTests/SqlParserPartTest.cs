using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserPartTest : ParseTestBase
	{
		[Fact]
		public void Tablename()
		{
			GivenText("name");
			WhenParse(SqlParser.TableFieldExpr);
			ThenResultShouldBe(new SqlTableFieldExpression()
			{
				Name = "name",
			});
		}

		[Fact]
		public void Tablename1()
		{
			GivenText("name");
			WhenParse(SqlParser.TableFieldExpr);
			ThenResultShouldBe(new SqlTableFieldExpression()
			{
				Name = "name",
			});
		}

		[Fact]
		public void Table_name()
		{
			GivenText("customer.name");
			WhenParse(SqlParser.TableFieldExpr);
			ThenResultShouldBe(new SqlTableFieldExpression()
			{
				Name = "name",
				From = "customer"
			});
		}

		[Fact]
		public void Table_name2()
		{
			GivenText("customer.name");
			WhenParse(SqlParser.TableFieldExpr);
			ThenResultShouldBe(new SqlTableFieldExpression()
			{
				Name = "name",
				From = "customer"
			});
		}

		[Fact]
		public void Database_table_name()
		{
			GivenText("db1.customer.name");
			WhenParse(SqlParser.TableFieldExpr);
			ThenResultShouldBe(new SqlTableFieldExpression()
			{
				Name = "name",
				From = "db1.customer"
			});
		}

		[Fact]
		public void Database_table_name3()
		{
			GivenText("db1.customer.name");
			WhenParse(SqlParser.TableFieldExpr);
			ThenResultShouldBe(new SqlTableFieldExpression()
			{
				Name = "name",
				From = "db1.customer"
			});
		}

		[Fact]
		public void Integer()
		{
			GivenText("123");
			WhenParse(SqlParser.IntegerExpr);
			ThenResultShouldBe(new SqlNumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = 123
			});
		}

		[Fact]
		public void NagativeInteger()
		{
			GivenText("-32");
			WhenParse(SqlParser.NegativeIntegerExpr);
			ThenResultShouldBe(new SqlNumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = -32
			});
		}

		[Fact]
		public void NumberExpr_NagativeInteger()
		{
			GivenText("-32");
			WhenParse(SqlParser.NumberExpr);
			ThenResultShouldBe(new SqlNumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = -32
			});
		}

		[Fact]
		public void NumberExpr_Integer()
		{
			GivenText("32");
			WhenParse(SqlParser.NumberExpr);
			ThenResultShouldBe(new SqlNumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = 32
			});
		}

		[Fact]
		public void Atom_Integer()
		{
			GivenText("32");
			WhenParse(SqlParser.Atom);
			ThenResultShouldBe(new SqlNumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = 32
			});
		}

		[Fact]
		public void Atom_Variable()
		{
			GivenText("@name");
			WhenParse(SqlParser.Atom);
			ThenResultShouldBe(new VariableExpression()
			{
				Name = "@name"
			});
		}


	}
}