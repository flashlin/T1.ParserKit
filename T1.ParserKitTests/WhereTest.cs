using System;
using T1.ParserKit.Core;
using T1.ParserKit.Core.Parsers;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class WhereTest : ParseTestBase
	{
		[Fact]
		public void Where_name_eq_1()
		{
			GivenText("where name = 1");
			WhenParse(SqlParser.WhereExpr);
			ThenResultShouldBe(new SqlWhereExpression()
			{
				Filter = new SqlFilterExpression()
				{
					Left = new SqlTableFieldExpression()
					{
						Name = "name"
					},
					Oper = "=",
					Right = new SqlNumberExpression()
					{
						ValueTypeFullname = typeof(int).FullName,
						Value = 1,
						TextSpan = new TextSpan()
						{
							File = "",
							Text = "1",
							Position = 13,
							Length = 1
						},
					},
				}
			});
		}

		[Fact]
		public void Filter_1_eq_1()
		{
			GivenText("1 = 1");
			WhenParse(SqlParser.FilterExpr);
			ThenResultShouldBe(new SqlFilterExpression()
			{
				TextSpan = new TextSpan()
				{
					File = String.Empty,
					Text = "1 = 1",
					Length = 5,
				},
				Left = new SqlNumberExpression
				{
					Value = 1,
					ValueTypeFullname = "System.Int32",
					TextSpan = new TextSpan
					{
						File = string.Empty,
						Text = "1",
						Position = 0,
						Length = 1
					}
				},
				Oper = "=",
				Right = new SqlNumberExpression
				{
					Value = 1,
					ValueTypeFullname = "System.Int32",
					TextSpan = new TextSpan
					{
						File = string.Empty,
						Text = "1",
						Position = 4,
						Length = 1
					}
				},
			});
		}

		[Fact]
		public void Filter_not_exists()
		{
			GivenText("not exists(1)");
			WhenParse(SqlParser.FilterExpr);
			ThenResultShouldBe(new SqlFilterExpression()
			{
				Left = null,
				Oper = "NOT",
				Right = new SqlFuncExistsExpression()
				{
					Name = "EXISTS",
					Parameters = new SqlExpression[]
					{
						new SqlNumberExpression
						{
							Value = 1,
							ValueTypeFullname = "System.Int32",
						}
					}
				},
			});
		}

		// where name = DB_NAME()
		// and SUSER_SNAME(owner_sid) = 'sa'
		[Fact]
		public void Filter_func_eq_str()
		{
			GivenText("SUSER_SNAME(owner_sid) = 'sa'");
			WhenParse(SqlParser.FilterExpr);
			ThenResultShouldBe(new SqlFilterExpression()
			{
				Left = new SqlFuncSuserSnameExpression()
				{
					Name = "SUSER_SNAME",
					Parameters = new SqlExpression[]
					{
						new SqlIdentifierExpression()
						{
							Name = "owner_sid"
						}
					}
				},
				Oper = "=",
				Right = new SqlStringExpression()
				{
					Text = "sa"
				}
			});
		}

		[Fact]
		public void FilterAndExpr_1_eq_2_AND_3_eq_4()
		{
			GivenText("1 = 2 and 3 = 4");
			WhenParse(SqlParser.FilterAndExpr);
			ThenResultShouldBe(new SqlAndOperExpression()
			{
				Left = new SqlFilterExpression()
				{
					Left = new SqlNumberExpression()
					{
						Value = 1,
						ValueTypeFullname = typeof(int).FullName
					},
					Oper = "=",
					Right = new SqlNumberExpression()
					{
						Value = 2,
						ValueTypeFullname = typeof(int).FullName
					}
				},
				Oper = "AND",
				Right = new SqlFilterExpression()
				{
					Left = new SqlNumberExpression()
					{
						Value = 3,
						ValueTypeFullname = typeof(int).FullName,
					},
					Oper = "=",
					Right = new SqlNumberExpression()
					{
						Value = 4,
						ValueTypeFullname = typeof(int).FullName
					}
				}
			});
		}

		[Fact]
		public void FilterOrExpr_1_eq_2_AND_3_eq_4()
		{
			GivenText("1 = 2 and 3 = 4");
			WhenParse(SqlParser.FilterOrExpr);
			ThenResultShouldBe(new SqlAndOperExpression()
			{
				Left = new SqlFilterExpression()
				{
					Left = new SqlNumberExpression()
					{
						Value = 1,
						ValueTypeFullname = typeof(int).FullName
					},
					Oper = "=",
					Right = new SqlNumberExpression()
					{
						Value = 2,
						ValueTypeFullname = typeof(int).FullName
					}
				},
				Oper = "AND",
				Right = new SqlFilterExpression()
				{
					Left = new SqlNumberExpression()
					{
						Value = 3,
						ValueTypeFullname = typeof(int).FullName,
					},
					Oper = "=",
					Right = new SqlNumberExpression()
					{
						Value = 4,
						ValueTypeFullname = typeof(int).FullName
					}
				}
			});
		}

		[Fact]
		public void FilterChainExpr_1_eq_2_AND_3_eq_4()
		{
			GivenText("1 = 2 and 3 = 4");
			WhenParse(SqlParser.FilterChainExpr);
			ThenResultShouldBe(new SqlAndOperExpression()
			{
				Left = new SqlFilterExpression()
				{
					Left = new SqlNumberExpression()
					{
						Value = 1,
						ValueTypeFullname = typeof(int).FullName
					},
					Oper = "=",
					Right = new SqlNumberExpression()
					{
						Value = 2,
						ValueTypeFullname = typeof(int).FullName
					}
				},
				Oper = "AND",
				Right = new SqlFilterExpression()
				{
					Left = new SqlNumberExpression()
					{
						Value = 3,
						ValueTypeFullname = typeof(int).FullName,
					},
					Oper = "=",
					Right = new SqlNumberExpression()
					{
						Value = 4,
						ValueTypeFullname = typeof(int).FullName
					}
				}
			});
		}

		[Fact]
		public void FilterChainExpr_1_eq_2()
		{
			GivenText("1 = 2");
			WhenParse(SqlParser.FilterChainExpr);
			ThenResultShouldBe(new SqlFilterExpression()
			{
				Left = new SqlNumberExpression()
				{
					Value = 1,
					ValueTypeFullname = typeof(int).FullName
				},
				Oper = "=",
				Right = new SqlNumberExpression()
				{
					Value = 2,
					ValueTypeFullname = typeof(int).FullName
				}
			});
		}

		[Fact]
		public void FilterChainExpr_field_eq_func_AND_func_eq_str1()
		{
			GivenText("name = DB_NAME() and SUSER_SNAME(owner_sid) = 'sa'");
			WhenParse(SqlParser.FilterChainExpr);
			ThenResultShouldBe(new SqlAndOperExpression()
			{
				Left = new SqlFilterExpression()
				{
					Left = new SqlTableFieldExpression()
					{
						Name = "name"
					},
					Oper = "=",
					Right = new SqlFuncDbNameExpression()
					{
						Name = "DB_NAME"
					}
				},
				Oper = "AND",
				Right = new SqlFilterExpression()
				{
					Left = new SqlFuncSuserSnameExpression()
					{
						Name = "SUSER_SNAME",
						Parameters = new SqlExpression[]
						{
							new SqlIdentifierExpression()
							{
								Name = "owner_sid"
							}
						}
					},
					Oper = "=",
					Right = new SqlStringExpression()
					{
						Text = "sa"
					}
				}
			});
		}

		[Fact]
		public void NString_not_like_NString()
		{
			GivenText("Where N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'");
			WhenParse(SqlParser.WhereExpr);
			ThenResultShouldBe(new SqlWhereExpression()
			{
				Filter = new SqlFilterExpression
				{
					Left = new SqlStringExpression
					{
						TextSpan = new TextSpan
						{
							File = "",
							Text = "N'$(__IsSqlCmdEnabled)'",
							Position = 6,
							Length = 23
						},
						IsUnicode = true,
						Text = "$(__IsSqlCmdEnabled)"
					},
					Oper = "NOT LIKE",
					Right = new SqlStringExpression()
					{
						TextSpan = new TextSpan
						{
							File = "",
							Text = "N'True'",
							Position = 39,
							Length = 7
						},
						IsUnicode = true,
						Text = "True"
					},
				}
			});
		}

		[Fact]
		public void If_nstring_eq_nstring_begin_print_string_end()
		{
			GivenText("IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True' BEGIN PRINT N'123'; SET NOEXEC ON; END");
			WhenParse(SqlParser.IfExprs);
			ThenResultShouldBe(new SqlIfExpression()
			{
				Condition = new SqlFilterExpression
				{
					Left = new SqlStringExpression
					{
						IsUnicode = true,
						Text = "$(__IsSqlCmdEnabled)",
						TextSpan = new TextSpan
						{
							File = "",
							Text = "N'$(__IsSqlCmdEnabled)'",
							Position = 3,
							Length = 23
						}
					},
					Oper = "NOT LIKE",
					Right = new SqlStringExpression
					{
						IsUnicode = true,
						Text = "True",
						TextSpan = new TextSpan
						{
							File = "",
							Text = "N'True'",
							Position = 36,
							Length = 7
						}
					},
					TextSpan = new TextSpan
					{
						Position = 0,
						Length = 0
					}
				},
				Body = new StatementsExpression
				{
					Items = new SqlExpression[]
					{
						new SqlPrintExpression
						{
							Value = new SqlStringExpression
							{
								IsUnicode = true,
								Text = "123",
								TextSpan = new TextSpan
								{
									File = "",
									Text = "N'123'",
									Position = 56,
									Length = 6
								}
							},
							TextSpan = new TextSpan
							{
								File = "",
								Text = "PRINT N'123';",
								Position = 50,
								Length = 13
							}
						},
						new SetOptionExpression
						{
							OptionName = "NOEXEC",
							IsToggle = true,
							TextSpan = new TextSpan
							{
								Position = 0,
								Length = 0
							}
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
		public void If_not_exists_select_1_begin_select_1()
		{
			GivenText(@"if not exists (select 1 from Ref with (nolock)) Begin
select 1 END");
			WhenParse(SqlParser.IfExprs);
			ThenResultShouldBe(new SqlIfExpression()
			{
				Condition = new SqlFilterExpression()
				{
					Oper = "NOT",
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
									Name = "Ref",
									WithOption = new SqlWithOptionExpression()
									{
										Nolock = true
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

		[Fact]
		public void Field_in_select_field_from_table()
		{
			GivenText("id in (select custid from oldCustomer)");	
			WhenParse(SqlParser.FilterExpr);
			ThenResultShouldBe(new SqlFilterExpression()
			{
				Left = new SqlObjectNameExpression()
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
			});
		}

		[Fact]
		public void Filter_name_is_null()
		{
			GivenText("name is null");
			WhenParse(SqlParser.FilterExpr);
			ThenResultShouldBe(new SqlFilterExpression()
			{
				Left = new SqlObjectNameExpression()
				{
					Name = "name"
				},
				Oper = "IS",
				Right = new SqlNullExpression()
			});
		}
	}
}