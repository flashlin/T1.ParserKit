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
			GiveText("GETDATE()");
			WhenParse(SqlParser.FuncGetdate);
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
			GiveText("GETDATE()");
			WhenParse(SqlParser.SqlFunctions(SqlParser.Atom));
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
			GiveText("DATEDIFF(dd, 0, GETDATE())");
			WhenParse(SqlParser.FuncDatediff(SqlParser.Atom));
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				Name = "DATEDIFF",
				Parameters = new SqlExpression[]
				{
					new SqlOptionNameExpression
					{
						Value = "dd",
					},
					new NumberExpression
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
			GiveText("isnull(@name, 50)");
			WhenParse(SqlParser.SqlFunctions(SqlParser.Atom));
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				Name = "ISNULL",
				Parameters = new SqlExpression[]
				{
					new VariableExpression
					{
						Name = "@name",
						TextSpan = new TextSpan
						{
							Position = 0,
							Length = 0
						}
					},
					new NumberExpression
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