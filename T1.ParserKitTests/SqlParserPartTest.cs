using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserPartTest : ParseTestBase
	{
		[Fact]
		public void Tablename()
		{
			GiveText("name");
			WhenParse(SqlParser.TableFieldExpr);
			ThenResultShouldBe(new FieldExpression()
			{
				Name = "name",
			});
		}

		[Fact]
		public void Tablename1()
		{
			GiveText("name");
			WhenParse(SqlParser.TableFieldExpr1);
			ThenResultShouldBe(new FieldExpression()
			{
				Name = "name",
			});
		}

		[Fact]
		public void Table_name()
		{
			GiveText("customer.name");
			WhenParse(SqlParser.TableFieldExpr);
			ThenResultShouldBe(new FieldExpression()
			{
				Name = "name",
				From = "customer"
			});
		}

		[Fact]
		public void Table_name2()
		{
			GiveText("customer.name");
			WhenParse(SqlParser.TableFieldExpr2);
			ThenResultShouldBe(new FieldExpression()
			{
				Name = "name",
				From = "customer"
			});
		}

		[Fact]
		public void Database_table_name()
		{
			GiveText("db1.customer.name");
			WhenParse(SqlParser.TableFieldExpr);
			ThenResultShouldBe(new FieldExpression()
			{
				Name = "name",
				From = "db1.customer"
			});
		}

		[Fact]
		public void Database_table_name3()
		{
			GiveText("db1.customer.name");
			WhenParse(SqlParser.TableFieldExpr3);
			ThenResultShouldBe(new FieldExpression()
			{
				Name = "name",
				From = "db1.customer"
			});
		}

		[Fact]
		public void Integer()
		{
			GiveText("123");
			WhenParse(SqlParser.IntegerExpr);
			ThenResultShouldBe(new NumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = 123
			});
		}

		[Fact]
		public void NagativeInteger()
		{
			GiveText("-32");
			WhenParse(SqlParser.NegativeIntegerExpr);
			ThenResultShouldBe(new NumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = -32
			});
		}

		[Fact]
		public void NumberExpr_NagativeInteger()
		{
			GiveText("-32");
			WhenParse(SqlParser.NumberExpr);
			ThenResultShouldBe(new NumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = -32
			});
		}

		[Fact]
		public void NumberExpr_Integer()
		{
			GiveText("32");
			WhenParse(SqlParser.NumberExpr);
			ThenResultShouldBe(new NumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = 32
			});
		}

		[Fact]
		public void Atom_Integer()
		{
			GiveText("32");
			WhenParse(SqlParser.Atom);
			ThenResultShouldBe(new NumberExpression()
			{
				ValueTypeFullname = typeof(int).FullName,
				Value = 32
			});
		}

		[Fact]
		public void Atom_Variable()
		{
			GiveText("@name");
			WhenParse(SqlParser.Atom);
			ThenResultShouldBe(new VariableExpression()
			{
				Name = "@name"
			});
		}


	}
}