using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class SelectTest : ParseTestBase
	{
		[Fact]
		public void Select_1_from_table_with_nolock()
		{
			GivenText("select 1 from Ref with (nolock)");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlNumberExpression
					{
						Value = 1,
						ValueTypeFullname = "System.Int32"
					}
				},
				From = new SqlTableExpression()
				{
					Name = "Ref",
					WithOption = new SqlWithOptionExpression()
					{
						Nolock = true
					}
				}
			});
		}

		[Fact]
		public void FilterExpr_variable_eq_1()
		{
			GivenText("@name = 1");
			WhenParse(SqlParser.FilterExpr);
			ThenResultShouldBe(new SqlFilterExpression()
			{
				Left = new VariableExpression
				{
					Name = "@name",
				},
				Oper = "=",
				Right = new SqlNumberExpression()
				{
					Value = 1,
					ValueTypeFullname = typeof(int).FullName,
				}
			});
		}

		[Fact]
		public void Where_field_eq_1()
		{
			GivenText("where name = 1");
			WhenParse(SqlParser.WhereExpr);
			ThenResultShouldBe(new WhereExpression()
			{
				SqlFilter = new SqlFilterExpression
				{
					Left = new SqlTableFieldExpression
					{
						Name = "name",
						TextSpan = new TextSpan
						{
							Position = 0,
							Length = 0
						}
					},
					Oper = "=",
					Right = new SqlNumberExpression
					{
						Value = 1,
						ValueTypeFullname = "System.Int32",
						TextSpan = new TextSpan
						{
							Position = 0,
							Length = 0
						}
					},
					TextSpan = new TextSpan
					{
						File = "",
						Text = "name = 1",
						Position = 0,
						Length = 0
					}
				}
			});
		}

		[Fact]
		public void Database_dbo_name()
		{
			GivenText("db1.dbo.customer");
			WhenParse(SqlParser.DatabaseSchemaObjectName);
			ThenResultShouldBe(new ObjectNameExpression()
			{
				Name = "db1.dbo.customer"
			});
		}

		[Fact]
		public void Dbo_name()
		{
			GivenText("dbo.customer");
			WhenParse(SqlParser.DatabaseSchemaObjectName);
			ThenResultShouldBe(new ObjectNameExpression()
			{
				Name = "dbo.customer"
			});
		}

		[Fact]
		public void Tablename()
		{
			GivenText("customer");
			WhenParse(SqlParser.DatabaseSchemaObjectName);
			ThenResultShouldBe(new ObjectNameExpression()
			{
				Name = "customer"
			});
		}

		[Fact]
		public void Tablename_as_alias()
		{
			GivenText("customer as c1");
			WhenParse(SqlParser.TableExpr);
			ThenResultShouldBe(new SqlTableExpression()
			{
				Name = "customer",
				AliasName = "c1"
			});
		}

		[Fact]
		public void Tablename_nolock_as_alias()
		{
			GivenText("customer with(nolock) as c1");
			WhenParse(SqlParser.TableExpr);
			ThenResultShouldBe(new SqlTableExpression()
			{
				Name = "customer",
				AliasName = "c1",
				WithOption = new SqlWithOptionExpression()
				{
					Nolock = true
				}
			});
		}

		[Fact]
		public void Tablename_nolock()
		{
			GivenText("customer with(nolock)");
			WhenParse(SqlParser.TableExpr);
			ThenResultShouldBe(new SqlTableExpression()
			{
				Name = "customer",
				WithOption = new SqlWithOptionExpression()
				{
					Nolock = true
				}
			});
		}

		[Fact]
		public void Select_field_from_table()
		{
			GivenText("select name from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_field_add_1_from_table()
		{
			GivenText("select id+1 from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new ArithmeticOperatorExpression
					{
						Left = new SqlTableFieldExpression
						{
							Name = "id",
						},
						Oper = "+",
						Right = new SqlNumberExpression
						{
							Value = 1,
							ValueTypeFullname = typeof(int).FullName,
						},
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_field_from_table_where_field_eq_1()
		{
			GivenText("select name from customer where name = 1");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer"
				},
				Where = new WhereExpression()
				{
					SqlFilter = new SqlFilterExpression()
					{
						Left = new SqlTableFieldExpression()
						{
							Name = "name"
						},
						Oper = "=",
						Right = new SqlNumberExpression()
						{
							ValueTypeFullname = typeof(int).FullName,
							Value = 1
						}
					}
				}
			});
		}

		[Fact]
		public void Select_field_from_table_where_field_eq_variable()
		{
			GivenText("select name from customer where name = @name");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer"
				},
				Where = new WhereExpression()
				{
					SqlFilter = new SqlFilterExpression()
					{
						Left = new SqlTableFieldExpression()
						{
							Name = "name"
						},
						Oper = "=",
						Right = new VariableExpression()
						{
							Name = "@name"
						}
					}
				}
			});
		}

		[Fact]
		public void Select_variable_assign_field_from_table()
		{
			GivenText("select @name=name from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlVariableAssignFieldExpression
					{
						VariableName = new VariableExpression
						{
							Name = "@name",
						},
						From = new SqlTableFieldExpression
						{
							Name = "name",
						},
					},
				},
				From = new SqlTableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_field_from_table_nolock()
		{
			GivenText("select name from customer with(nolock)");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer",
					WithOption = new SqlWithOptionExpression()
					{
						Nolock = true
					}
				}
			});
		}

		[Fact]
		public void Select_tb_field_from_table_alias()
		{
			GivenText("select c.name from customer c");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
						From = "c"
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer",
					AliasName = "c"
				}
			});
		}

		[Fact]
		public void Select_tb_field_alias_from_table_alias()
		{
			GivenText("select c.name username from customer c");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
						From = "c",
						AliasName = "username"
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer",
					AliasName = "c"
				}
			});
		}

		[Fact]
		public void Select_Field_alias_from_table()
		{
			GivenText("select name n1 from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
						AliasName = "n1",
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_Field_as_alias_from_table()
		{
			GivenText("select name as n1 from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
						AliasName = "n1"
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_Field_as_alias_from_table_as_alias()
		{
			GivenText("select name as n1 from customer as c");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
						AliasName = "n1"
					}
				},
				From = new SqlTableExpression()
				{
					Name = "customer",
					AliasName = "c"
				}
			});
		}

		[Fact]
		public void Select_Field_as_alias_from_lparen_select_name_from_table_rparen_alias()
		{
			GivenText("select name as n1 from (select name1 from customer) c");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SqlSelectExpression()
			{
				Fields = new SqlExpression[]
				{
					new SqlTableFieldExpression()
					{
						Name = "name",
						AliasName = "n1"
					}
				},
				From = new SourceExpression()
				{
					Item = new SqlSelectExpression()
					{
						Fields = new SqlExpression[]
						{
							new SqlTableFieldExpression()
							{
								Name = "name1"
							}
						},
						From = new SqlTableExpression()
						{
							Name = "customer"
						}
					},
					AliasName = "c"
				}
			});
		}

		//select 1 from sys.databases where name = DB_NAME() and SUSER_SNAME(owner_sid) = 'sa'
		[Fact]
		public void Select_1_from_table_where_field_eq_func_and_func_eq_str1()
		{
			
		}

		[Fact]
		public void print_nstring()
		{
			GivenText("PRINT N'SQLCMD .';");
			WhenParse(SqlParser.PrintExpr);
			ThenResultShouldBe(new SqlPrintExpression()
			{
				TextSpan = new TextSpan
				{
					File = "",
					Text = "PRINT N'SQLCMD .';",
					Position = 0,
					Length = 18
				},
				Value = new SqlStringExpression()
				{
					TextSpan = new TextSpan
					{
						File = "",
						Text = "N'SQLCMD .'",
						Position = 6,
						Length = 11
					},
					IsUnicode = true,
					Text = "SQLCMD ."
				}
			});
		}
	}
}