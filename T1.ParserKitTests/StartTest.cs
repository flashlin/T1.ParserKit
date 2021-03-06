using System.Collections.Generic;
using System.IO;
using System.Linq;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.ParserKitTests.Helpers;
using Xunit;

namespace T1.ParserKitTests
{
	public class StartTest : ParseTestBase
	{
		[Fact]
		public void Start_declare_name_int()
		{
			GivenText("declare @name int");
			WhenParse(SqlParser.StartExpr);
			ThenResultShouldBe(new SqlDeclareExpression()
			{
				Name = new SqlVariableExpression()
				{
					Name = "@name"
				},
				DataType = "int"
			});
		}

		[Fact]
		public void Start_set_ANSI_NULLS_on()
		{
			GivenText("SET ANSI_NULLS ON;");
			WhenParse(SqlParser.StartExpr);
			ThenResultShouldBe(new SetOptionExpression()
			{
				OptionName = "ANSI_NULLS",
				IsToggle = true
			});
		}

		[Fact]
		public void Start_set_ANSI_NULLS_ANSI_PADDING_on()
		{
			GivenText("SET ANSI_NULLS, ANSI_PADDING ON;");
			WhenParse(SqlParser.StartExpr);
			ThenResultShouldBe(new SetManyOptionExpression()
			{
				Items = new []
				{
					new SetOptionExpression()
					{
						OptionName = "ANSI_NULLS",
						IsToggle = true
					},
					new SetOptionExpression()
					{
						OptionName = "ANSI_PADDING",
						IsToggle = true
					}
				}
			});
		}

		[Fact]
		public void Start_set_ANSI_NULLS__ANSI_PADDING_on()
		{
			GivenText("SET ANSI_NULLS , ANSI_PADDING ON;");
			WhenParse(SqlParser.StartExpr);
			ThenResultShouldBe(new SetManyOptionExpression()
			{
				Items = new []
				{
					new SetOptionExpression()
					{
						OptionName = "ANSI_NULLS",
						IsToggle = true
					},
					new SetOptionExpression()
					{
						OptionName = "ANSI_PADDING",
						IsToggle = true
					}
				}
			});
		}

		[Fact]
		//[Fact(Skip = "Test Samples")]
		public void Test()
		{
			var folder = @"D:\VDisk\MyGitHub\SQL";
			var samples = GetSqlFiles(folder);
			foreach (var sample in samples)
			{
				GivenTextFile(sample);
				WhenParseAll(SqlParser.StartExpr);
				ThenResultShouldSuccess();
			}
		}

		private IEnumerable<string> GetSqlFiles(string folder)
		{
			var files = Directory.EnumerateFiles(folder, "*.sql");
			var subFiles = Directory.EnumerateDirectories(folder)
				.Select(GetSqlFiles)
				.SelectMany(x => x);
			return files.Concat(subFiles);
		}
	}
}