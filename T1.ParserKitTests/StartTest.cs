using System.Collections.Generic;
using System.IO;
using System.Linq;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using Xunit;

namespace T1.ParserKitTests
{
	public class StartTest : ParseTestBase
	{
		[Fact]
		public void Start_declare_name_int()
		{
			GiveText("declare @name int");
			WhenParse(SqlParser.StartExpr);
			ThenResultShouldBe(new DeclareExpression()
			{
				Name = new VariableExpression()
				{
					Name = "@name"
				},
				DataType = "int"
			});
		}

		//[Fact]
		[Fact(Skip = "Test Samples")]
		public void Test()
		{
			var folder = @"D:\VDisk\MyGitHub\SQL";
			var samples = GetSqlFiles(folder);
			foreach (var sample in samples)
			{
				GiveTextFile(sample);
				WhenParse(SqlParser.StartExpr);
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