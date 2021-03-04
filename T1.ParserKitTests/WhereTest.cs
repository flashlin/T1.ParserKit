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
			ThenResultShouldBe(new WhereExpression()
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
		public void Filter()
		{
			GivenText("1 = 1");
			WhenParse(SqlParser.FilterExpr);
			ThenResultShouldBe(new FilterExpression()
			{
				TextSpan = new TextSpan()
				{
					File = String.Empty,
					Text = "1 = 1",
					Length = 5,
				},
				Left = new NumberExpression
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
				Right = new NumberExpression
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
		public void NString_not_like_NString()
		{
			GivenText("Where N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'");
			WhenParse(SqlParser.WhereExpr);
			ThenResultShouldBe(new WhereExpression()
			{
				Filter = new FilterExpression
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
			WhenParse(SqlParser.IfExprs2);
			ThenResultShouldBe(new IfExpression()
			{
				Condition = new FilterExpression
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
	}
}