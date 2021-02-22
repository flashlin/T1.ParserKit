﻿using T1.ParserKit.SqlDom;
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
	}
}