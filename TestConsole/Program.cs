using System;
using System.IO;
using System.Linq;
using T1.ParserKit.Core;
using T1.ParserKit.SqlDom;

namespace TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var file = @"D:\VDisk\MyGitHub\SQL\TigerSoft\Consus.Account\AccountDB\bin\Release\AccountDB.publish.sql";
			var text = File.ReadAllText(file);
			var rc = SqlParser.StartExpr.TryParseAllText(text)
				.ToArray();
			Console.WriteLine(rc.Length);
		}
	}
}
