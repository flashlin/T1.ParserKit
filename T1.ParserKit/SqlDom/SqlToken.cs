using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T1.ParserKit.Core;

namespace T1.ParserKit.SqlDom
{
	public static class SqlToken
	{
		public static string[] Keywords = new[]
		{
			"ABSOLUTE", "ACTION", "ADA", "ADD", "ALL", "ALLOCATE", "ALTER", "AND", "ANY", "ARE",
			"AS", "ASC", "ASSERTION", "AT", "AUTHORIZATION", "AVG", "BEGIN", "BETWEEN", "BIT",
			"BIT_LENGTH", "BOTH", "BY", "CASCADE", "CASCADED", "CASE", "CAST", "CATALOG", "CHAR",
			"CHAR_LENGTH", "CHARACTER", "CHARACTER_LENGTH", "CHECK", "CLOSE", "COALESCE",
			"COLLATE", "COLLATION", "COLUMN", "COMMIT", "CONNECT", "CONNECTION", "CONSTRAINT",
			"CONSTRAINTS", "CONTINUE", "CONVERT", "CORRESPONDING", "COUNT", "CREATE", "CROSS",
			"CURRENT", "CURRENT_DATE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER",
			"CURSOR", "DATE", "DAY", "DEALLOCATE", "DEC", "DECIMAL", "DECLARE", "DEFAULT", "DEFERRABLE",
			"DEFERRED", "DELETE", "DESC", "DESCRIBE", "DESCRIPTOR", "DIAGNOSTICS", "DISCONNECT",
			"DISTINCT", "DOMAIN", "DOUBLE", "DROP", "ELSE", "END", "END-EXEC", "ESCAPE", "EXCEPT",
			"EXCEPTION", "EXEC", "EXECUTE", "EXISTS", "EXTERNAL", "EXTRACT", "FALSE", "FETCH",
			"FIRST", "FLOAT", "FOR", "FOREIGN", "FORTRAN", "FOUND", "FROM", "FULL", "GET", "GLOBAL",
			"GO", "GOTO", "GRANT", "GROUP", "HAVING", "HOUR", "IDENTITY", "IMMEDIATE", "IN",
			"INCLUDE", "INDEX", "INDICATOR", "INITIALLY", "INNER", "INPUT", "INSENSITIVE",
			"INSERT", "INT", "INTEGER", "INTERSECT", "INTERVAL", "INTO", "IS", "ISOLATION",
			"JOIN", "KEY", "LANGUAGE", "LAST", "LEADING", "LEFT", "LEVEL", "LIKE", "LOCAL",
			"LOWER", "MATCH", "MAX", "MIN", "MINUTE", "MODULE", "MONTH", "NAMES", "NATIONAL",
			"NATURAL", "NCHAR", "NEXT", "NO", "NONE", "NOT", "NULL", "NULLIF", "NUMERIC",
			"OCTET_LENGTH", "OF", "ON", "ONLY", "OPEN", "OPTION", "OR", "ORDER", "OUTER", "OUTPUT",
			"OVERLAPS", "PAD", "PARTIAL", "PASCAL", "POSITION", "PRECISION", "PREPARE", "PRESERVE",
			"PRIMARY", "PRIOR", "PRIVILEGES", "PROCEDURE", "PUBLIC", "READ", "REAL", "REFERENCES",
			"RELATIVE", "RESTRICT", "REVOKE", "RIGHT", "ROLLBACK", "ROWS", "SCHEMA", "SCROLL", "SECOND",
			"SECTION", "SELECT", "SESSION", "SESSION_USER", "SET", "SIZE", "SMALLINT", "SOME", "SPACE",
			"SQL", "SQLCA", "SQLCODE", "SQLERROR", "SQLSTATE", "SQLWARNING", "SUBSTRING", "SUM",
			"SYSTEM_USER", "TABLE", "TEMPORARY", "THEN", "TIME", "TIMESTAMP", "TIMEZONE_HOUR",
			"TIMEZONE_MINUTE", "TO", "TRAILING", "TRANSACTION", "TRANSLATE", "TRANSLATION", "TRIM",
			"TRUE", "UNION", "UNIQUE", "UNKNOWN", "UPDATE", "UPPER", "USAGE", "USER", "USING", "VALUE",
			"VALUES", "VARCHAR", "VARYING", "VIEW", "WHEN", "WHENEVER", "WHERE", "WITH", "WORK", "WRITE",
			"YEAR", "ZONE"
		};

		private static readonly string[] DateaddDatepartStr = new[]
		{
			"year", "quarter", "month", "dayofyear", "day",
			"week", "weekday", "hour", "minute", "second", "millisecond", "microsecond",
			"nanosecond"
		};

		private static readonly string[] DateaddAbbreviationDatepartStr = new[]
		{
			"yy", "yyyy", "qq", "q", "mm", "m", "dy", "y",
			"dd", "d", "wk", "ww", "dw", "w", "hh", "mi", "n",
			"ss", "s", "ms", "mcs", "ns"
		};

		public static readonly string[] DateaddDetepart = DateaddDatepartStr.Concat(DateaddAbbreviationDatepartStr)
			.ToArray();

		static readonly string[] DatediffDatepartStr = new[]
		{
			"year", "quarter", "month", "dayofyear", "day",
			"week", "hour", "minute", "second", "millisecond",
			"microsecond", "nanosecond"
		};

		static readonly string[] DatediffAbbreviationDatepartStr = new[]
		{
			"yy", "yyyy", "qq", "q", "mm", "m",
			"dy", "y", "dd", "d", "wk", "ww",
			"hh", "mi", "n", "ss", "s", "ms",
			"mcs", "ns"
		};

		public static readonly string[] DatediffDatepart = DatediffDatepartStr
			.Concat(DatediffAbbreviationDatepartStr)
			.ToArray();

		public static IParser<TextSpan> Word(string text)
		{
			return ParseToken.Lexeme(ParseToken.Match(text));
		}

		public static IParser<TextSpan> ContainsWord(string[] texts)
		{
			var sortedTexts = texts
				.OrderByDescending(x => x.Length)
				.ToArray();
			return ParseToken.Lexeme(ParseToken.Matchs(sortedTexts));
		}
	}
}
