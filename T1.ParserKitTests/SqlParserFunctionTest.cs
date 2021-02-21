using System;
using System.Linq;
using System.Text.Json;
using ExpectedObjects;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class SqlParserFunctionTest
	{
		private string _code;
		private SqlParser _parser;
		private IParseResult<object> _result;

		[Fact]
		public void Getdate()
		{
			GiveText("GETDATE()");
			WhenParse(SqlParser.FuncGetdate);
			ThenResultShouldBe(new SqlFunctionExpression()
			{
				File = String.Empty,
				Content = _code,
				Position = 0,
				Length = _code.Length,
				Name = "GETDATE",
				Parameters = new SqlExpression[0]
			});
		}

		//[Fact]
		//public void Datediff_dd_0_getdate()
		//{
		//	GiveText("DATEDIFF(dd, 0, GETDATE())");
		//	WhenParse();
		//	ThenResultShouldBe(new SqlFunctionExpression()
		//	{
		//		File = String.Empty,
		//		Content = _code,
		//		Position = 0,
		//		Length = _code.Length,
		//		Name = "DATEDIFF",
		//		Parameters = new SqlExpression[]
		//		{
		//			new SqlOptionNameExpression
		//			{
		//				Value = "dd",
		//				File = string.Empty,
		//				Length = 2,
		//				Position = 9,
		//				Content = _code
		//			},
		//			new NumberExpression
		//			{
		//				Value = 0,
		//				ValueTypeFullname = typeof(int).FullName,
		//				File = "",
		//				Length = 1,
		//				Position = 13,
		//				Content = _code
		//			},
		//			new SqlFunctionExpression
		//			{
		//				Name = "GETDATE",
		//				Parameters = new SqlExpression[] { },
		//				File = "",
		//				Length = 9,
		//				Position = 16,
		//				Content = _code
		//			}
		//		}
		//	});
		//}



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

		private void WhenParse<T>(IParser<T> parser)
		{
			_result = parser.ParseText(_code).CastToParseResult();
		}

		private void GiveText(string code)
		{
			_code = code;
		}

		private void ThenResultShouldBe(SqlExpression expression)
		{
			expression.ToExpectedObject()
				.ShouldMatch(_result.Result);
		}
	}
}