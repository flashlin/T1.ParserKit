using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserSelectTest : ParseTestBase
	{
		[Fact]
		public void Where_field_eq_1()
		{
			GiveText("where name = 1");
			WhenParse(SqlParser.WhereExpr);
			ThenResultShouldBe(new WhereExpression()
			{
				Filter = new FilterExpression
				{
					Left = new FieldExpression
					{
						Name = "name",
						TextSpan = new TextSpan
						{
							Position = 0,
							Length = 0
						}
					},
					Oper = "=",
					Right = new NumberExpression
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
						Position = 0,
						Length = 0
					}
				}
			});
		}

		[Fact]
		public void Database_dbo_name()
		{
			GiveText("db1.dbo.customer");
			WhenParse(SqlParser.DatabaseSchemaObjectName);
			ThenResultShouldBe(new ObjectNameExpression()
			{
				Name = "db1.dbo.customer"
			});
		}

		[Fact]
		public void Dbo_name()
		{
			GiveText("dbo.customer");
			WhenParse(SqlParser.DatabaseSchemaObjectName);
			ThenResultShouldBe(new ObjectNameExpression()
			{
				Name = "dbo.customer"
			});
		}

		[Fact]
		public void Tablename()
		{
			GiveText("customer");
			WhenParse(SqlParser.DatabaseSchemaObjectName);
			ThenResultShouldBe(new ObjectNameExpression()
			{
				Name = "customer"
			});
		}

		[Fact]
		public void Tablename_as_alias()
		{
			GiveText("customer as c1");
			WhenParse(SqlParser.TableExpr);
			ThenResultShouldBe(new TableExpression()
			{
				Name = "customer",
				AliasName = "c1"
			});
		}

		[Fact]
		public void Tablename_nolock_as_alias()
		{
			GiveText("customer with(nolock) as c1");
			WhenParse(SqlParser.TableExpr);
			ThenResultShouldBe(new TableExpression()
			{
				Name = "customer",
				AliasName = "c1",
				WithOption = new WithOptionExpression()
				{
					Nolock = true
				}
			});
		}

		[Fact]
		public void Tablename_nolock()
		{
			GiveText("customer with(nolock)");
			WhenParse(SqlParser.TableExpr);
			ThenResultShouldBe(new TableExpression()
			{
				Name = "customer",
				WithOption = new WithOptionExpression()
				{
					Nolock = true
				}
			});
		}

		[Fact]
		public void Select_field_from_table()
		{
			GiveText("select name from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_field_add_1_from_table()
		{
			GiveText("select id+1 from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new ArithmeticOperatorExpression
						{
							Left = new FieldExpression
							{
								Name = "id",
							},
							Oper = "+",
							Right = new NumberExpression
							{
								Value = 1,
								ValueTypeFullname = typeof(int).FullName,
							},
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_field_from_table_where_field_eq_1()
		{
			GiveText("select name from customer where name = 1");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer"
				},
				Where = new WhereExpression()
				{
					Filter = new FilterExpression()
					{
						Left = new FieldExpression()
						{
							Name = "name"
						},
						Oper = "=",
						Right = new NumberExpression()
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
			GiveText("select name from customer where name = @name");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer"
				},
				Where = new WhereExpression()
				{
					Filter = new FilterExpression()
					{
						Left = new FieldExpression()
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
			GiveText("select @name=name from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new VariableAssignFieldExpression
						{
							VariableName = new VariableExpression
							{
								Name = "@name",
							},
							From = new FieldExpression
							{
								Name = "name",
							},
						},
					}
				},
				From = new TableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_field_from_table_nolock()
		{
			GiveText("select name from customer with(nolock)");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer",
					WithOption = new WithOptionExpression()
					{
						Nolock = true
					}
				}
			});
		}

		[Fact]
		public void Select_tb_field_from_table_alias()
		{
			GiveText("select c.name from customer c");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
							From = "c"
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer",
					AliasName = "c"
				}
			});
		}

		[Fact]
		public void Select_tb_field_alias_from_table_alias()
		{
			GiveText("select c.name username from customer c");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
							From = "c",
							AliasName = "username"
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer",
					AliasName = "c"
				}
			});
		}

		[Fact]
		public void Select_Field_alias_from_table()
		{
			GiveText("select name n1 from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
							AliasName = "n1",
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_Field_as_alias_from_table()
		{
			GiveText("select name as n1 from customer");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
							AliasName = "n1"
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer"
				}
			});
		}

		[Fact]
		public void Select_Field_as_alias_from_table_as_alias()
		{
			GiveText("select name as n1 from customer as c");
			WhenParse(SqlParser.SelectExpr);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
							AliasName = "n1"
						}
					}
				},
				From = new TableExpression()
				{
					Name = "customer",
					AliasName = "c"
				}
			});
		}

		[Fact]
		public void Select_Field_as_alias_from_lparen_select_name_from_table_rparen_alias()
		{
			GiveText("select name as n1 from (select name1 from customer) c");
			WhenParse(SqlParser.SelectExpr2);
			ThenResultShouldBe(new SelectExpression()
			{
				Fields = new FieldsExpression()
				{
					Items = new List<SqlExpression>()
					{
						new FieldExpression()
						{
							Name = "name",
							AliasName = "n1"
						}
					}
				},
				From = new SourceExpression()
				{
					Item = new SelectExpression()
					{
						Fields = new FieldsExpression()
						{
							Items = new List<SqlExpression>()
							{
								new FieldExpression()
								{
									Name = "name1"
								}
							}
						},
						From = new TableExpression()
						{
							Name = "customer"
						}
					},
					AliasName = "c"
				}
			});
		}
	}
}
