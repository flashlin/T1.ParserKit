using System;
using System.Linq;
using System.Text.Json;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class FunctionTest : ParseTestBase
	{
		[Fact]
		public void FuncGetdate()
		{
			GivenText("GETDATE()");
			WhenParse(SqlParser.FuncGetdateExpr);
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				TextSpan = new TextSpan()
				{
					File = String.Empty,
					Text = _text,
					Position = 0,
					Length = _text.Length,
				},
				Name = "GETDATE",
				Parameters = new SqlExpression[0]
			});
		}

		[Fact]
		public void SqlFunctions_Getdate()
		{
			GivenText("GETDATE()");
			WhenParse(SqlParser.SqlFunctionsExpr);
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				TextSpan = new TextSpan()
				{
					File = String.Empty,
					Text = _text,
					Position = 0,
					Length = _text.Length,
				},
				Name = "GETDATE",
				Parameters = new SqlExpression[0]
			});
		}

		[Fact]
		public void Datediff_dd_0_getdate()
		{
			GivenText("DATEDIFF(dd, 0, GETDATE())");
			WhenParse(SqlParser.FuncDatediffExpr);
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				Name = "DATEDIFF",
				Parameters = new SqlExpression[]
				{
					new SqlOptionNameExpression
					{
						Value = "dd",
					},
					new SqlNumberExpression
					{
						Value = 0,
						ValueTypeFullname = typeof(int).FullName,
					},
					new SqlFunctionExpression
					{
						Name = "GETDATE",
						Parameters = new SqlExpression[] { },
						TextSpan = new TextSpan
						{
							File = string.Empty,
							Text = "GETDATE()",
							Position = 16,
							Length = 9
						}
					}
				},
				TextSpan = new TextSpan
				{
					Position = 0,
					Length = 0
				}
			});
		}

		[Fact]
		public void SqlFunctions_isnull()
		{
			GivenText("isnull(@name, 50)");
			WhenParse(SqlParser.SqlFunctionsExpr);
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				Name = "ISNULL",
				Parameters = new SqlExpression[]
				{
					new SqlVariableExpression
					{
						Name = "@name",
						TextSpan = new TextSpan
						{
							Position = 0,
							Length = 0
						}
					},
					new SqlNumberExpression
					{
						Value = 50,
						ValueTypeFullname = typeof(int).FullName,
					}
				},
				TextSpan = new TextSpan
				{
					Position = 0,
					Length = 0
				},
			});
		}

		[Fact]
		public void Exists()
		{
			GivenText("exists(1)");	
			WhenParse(SqlParser.SqlFunctionsExpr);
			ThenResultShouldBe(new SqlFuncExistsExpression()
			{
				Name = "EXISTS",
				Parameters = new SqlExpression[]
				{
					new SqlNumberExpression()
					{
						ValueTypeFullname = typeof(int).FullName,
						Value = 1
					}
				}
			});
		}

		[Fact]
		public void Exists_select_1_from_table_with_nolock_where_field_is_null()
		{
			GivenText("exists (select 1 from customer with (nolock) where id is null)");	
			WhenParse(SqlParser.SqlFunctionsExpr);
			ThenResultShouldBe(new SqlFuncExistsExpression()
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
							Name = "customer",
							WithOption = new SqlWithOptionExpression()
							{
								Nolock = true
							}
						},
						Where = new SqlWhereExpression()
						{
							Filter = new SqlFilterExpression()
							{
								Left = new SqlTableFieldExpression()
								{
									Name = "id"
								},
								Oper = "IS",
								Right = new SqlNullExpression()
							}
						}
					}
				}
			});
		}

		[Fact]
		public void Cast_hex_as_datetime()
		{
			GivenText("Cast(0x0000A5E5006236FB as DateTime)");	
			WhenParse(SqlParser.FuncCastExpr);
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				Name = "CAST",
				Parameters = new SqlExpression[]
				{
					new SqlHexExpression()
					{
						HexStr = "0000A5E5006236FB"
					},
					new SqlDataTypeExpression()
					{
						DataType = "DateTime"
					}
				}
			});
		}

		[Fact]
		public void Cast_float_as_decimal()
		{
			GivenText("CAST(0.0075 AS Decimal(5, 4))");	
			WhenParse(SqlParser.FuncCastExpr);
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				Name = "CAST",
				Parameters = new SqlExpression[]
				{
					new SqlNumberExpression()
					{
						Value = 0.0075m,
						ValueTypeFullname = typeof(decimal).FullName
					},
					new SqlDataTypeExpression()
					{
						DataType = "Decimal",
						Size = 5,
						Scale = 4
					}
				}
			});
		}

		[Fact]
		public void Cast_negative_float_as_decimal()
		{
			GivenText("CAST(-12.00 AS Numeric(4, 2))");	
			WhenParse(SqlParser.FuncCastExpr);
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				Name = "CAST",
				Parameters = new SqlExpression[]
				{
					new SqlNumberExpression()
					{
						Value = -12.0m,
						ValueTypeFullname = typeof(decimal).FullName
					},
					new SqlDataTypeExpression()
					{
						DataType = "Numeric",
						Size = 4,
						Scale = 2
					}
				}
			});
		}



		[Fact]
		public void SUSER_SNAME()
		{
			GivenText("SUSER_SNAME(server_user_sid)");
			WhenParse(SqlParser.SqlFunctionsExpr);
			ThenResultShouldBe(new SqlFuncSuserSnameExpression()
			{
				Name = "SUSER_SNAME",
				Parameters = new SqlExpression[]
				{
					new SqlIdentifierExpression()
					{
						Name = "server_user_sid"
					}
				}
			});
		}

		//[Fact]
		//public void Dateadd_d_1_datediff_getdate()
		//{
		//	GiveText("DATEADD(DD,-1,DATEDIFF(dd, 0, GETDATE()))");
		//	WhenParse();
		//	ThenResultShouldBe(new SqlFunctionExpression()
		//	{
		//		File = String.Empty,
		//		Content = _code,
		//		Position = 0,
		//		Length = _code.Length,
		//		Name = "DATEADD",
		//		Parameters = new SqlExpression[]
		//		{
		//			new SqlOptionNameExpression
		//			{
		//				Value = "DD",
		//				File = "",
		//				Length = 2,
		//				Position = 8,
		//				Content = _code
		//			},
		//			new NumberExpression
		//			{
		//				Value = -1,
		//				ValueTypeFullname = "System.Int32",
		//				File = "",
		//				Length = 1,
		//				Position = 11,
		//				Content = _code
		//			},
		//			new SqlFunctionExpression
		//			{
		//				Name = "DATEDIFF",
		//				Parameters = new SqlExpression[]
		//				{
		//					new SqlOptionNameExpression
		//					{
		//						Value = "dd",
		//						File = "",
		//						Length = 2,
		//						Position = 23,
		//						Content = _code
		//					},new NumberExpression
		//					{
		//						Value = 0,
		//						ValueTypeFullname = "System.Int32",
		//						File = "",
		//						Length = 1,
		//						Position = 27,
		//						Content = _code
		//					},new SqlFunctionExpression
		//					{
		//						Name = "GETDATE",
		//						Parameters = new SqlExpression[] { },
		//						File = "",
		//						Length = 9,
		//						Position = 30,
		//						Content = _code
		//					}
		//				},
		//				File = "",
		//				Length = 26,
		//				Position = 14,
		//				Content = _code
		//			}
		//		}
		//	});
		//}
	}
}