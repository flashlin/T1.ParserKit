﻿//using System;
//using System.Collections.Generic;
//using System.Text;
//using T1.ParserKit.Core;

//namespace T1.ParserKit.SqlDom
//{
//	public class SqlTokenizer
//	{
//		public string[] Symbols = new[]
//		{
//			"[", "]", ",", ".", ";", "@", "::", ":", "\\", "$",
//			">=", "<>", "<=", "<", ">", "=", "!=", "+", "-", "*", "/",
//			"&", "|", "~", "^", "%", "(", ")"
//		};

//		public static string[] Keywords = new[]
//		{
//			"ABSOLUTE", "ACTION", "ADA", "ADD", "ALL", "ALLOCATE", "ALTER", "AND", "ANY", "ARE",
//			"AS", "ASC", "ASSERTION", "AT", "AUTHORIZATION", "AVG", "BEGIN", "BETWEEN", "BIT",
//			"BIT_LENGTH", "BOTH", "BY", "CASCADE", "CASCADED", "CASE", "CAST", "CATALOG", "CHAR",
//			"CHAR_LENGTH", "CHARACTER", "CHARACTER_LENGTH", "CHECK", "CLOSE", "COALESCE",
//			"COLLATE", "COLLATION", "COLUMN", "COMMIT", "CONNECT", "CONNECTION", "CONSTRAINT",
//			"CONSTRAINTS", "CONTINUE", "CONVERT", "CORRESPONDING", "COUNT", "CREATE", "CROSS",
//			"CURRENT", "CURRENT_DATE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER",
//			"CURSOR", "DATE", "DAY", "DEALLOCATE", "DEC", "DECIMAL", "DECLARE", "DEFAULT", "DEFERRABLE",
//			"DEFERRED", "DELETE", "DESC", "DESCRIBE", "DESCRIPTOR", "DIAGNOSTICS", "DISCONNECT",
//			"DISTINCT", "DOMAIN", "DOUBLE", "DROP", "ELSE", "END", "END-EXEC", "ESCAPE", "EXCEPT",
//			"EXCEPTION", "EXEC", "EXECUTE", "EXISTS", "EXTERNAL", "EXTRACT", "FALSE", "FETCH",
//			"FIRST", "FLOAT", "FOR", "FOREIGN", "FORTRAN", "FOUND", "FROM", "FULL", "GET", "GLOBAL",
//			"GO", "GOTO", "GRANT", "GROUP", "HAVING", "HOUR", "IDENTITY", "IMMEDIATE", "IN",
//			"INCLUDE", "INDEX", "INDICATOR", "INITIALLY", "INNER", "INPUT", "INSENSITIVE",
//			"INSERT", "INT", "INTEGER", "INTERSECT", "INTERVAL", "INTO", "IS", "ISOLATION",
//			"JOIN", "KEY", "LANGUAGE", "LAST", "LEADING", "LEFT", "LEVEL", "LIKE", "LOCAL",
//			"LOWER", "MATCH", "MAX", "MIN", "MINUTE", "MODULE", "MONTH", "NAMES", "NATIONAL",
//			"NATURAL", "NCHAR", "NEXT", "NO", "NONE", "NOT", "NULL", "NULLIF", "NUMERIC",
//			"OCTET_LENGTH", "OF", "ON", "ONLY", "OPEN", "OPTION", "OR", "ORDER", "OUTER", "OUTPUT",
//			"OVERLAPS", "PAD", "PARTIAL", "PASCAL", "POSITION", "PRECISION", "PREPARE", "PRESERVE",
//			"PRIMARY", "PRIOR", "PRIVILEGES", "PROCEDURE", "PUBLIC", "READ", "REAL", "REFERENCES",
//			"RELATIVE", "RESTRICT", "REVOKE", "RIGHT", "ROLLBACK", "ROWS", "SCHEMA", "SCROLL", "SECOND",
//			"SECTION", "SELECT", "SESSION", "SESSION_USER", "SET", "SIZE", "SMALLINT", "SOME", "SPACE",
//			"SQL", "SQLCA", "SQLCODE", "SQLERROR", "SQLSTATE", "SQLWARNING", "SUBSTRING", "SUM",
//			"SYSTEM_USER", "TABLE", "TEMPORARY", "THEN", "TIME", "TIMESTAMP", "TIMEZONE_HOUR",
//			"TIMEZONE_MINUTE", "TO", "TRAILING", "TRANSACTION", "TRANSLATE", "TRANSLATION", "TRIM",
//			"TRUE", "UNION", "UNIQUE", "UNKNOWN", "UPDATE", "UPPER", "USAGE", "USER", "USING", "VALUE",
//			"VALUES", "VARCHAR", "VARYING", "VIEW", "WHEN", "WHENEVER", "WHERE", "WITH", "WORK", "WRITE",
//			"YEAR", "ZONE"
//		};

//		public IParser KeywordsParser =>
//			Parse.Contains(Keywords, true).Assertion(true);

//		public IParser SymbolsParser =>
//			Parse.Contains(Symbols).Assertion(false);

//		public IParser SqlIdentifier
//		{
//			get
//			{
//				var start = Parse.Equal("[");
//				var body = Parse.NotEqual("]").Many(1);
//				var end = Parse.Equal("]");
//				var identifier = Parse.Chain(start, body, end).Merge();
//				return identifier.Or(Parse.CStyleIdentifier())
//					.Named("SqlIdentifier");
//			}
//		}


//		public IParser TmpVariable => Parse.Equal("@").Then(SqlIdentifier)
//			.Named("variable");

//		public ITextSpan Tokenize(string code)
//		{
//			var p = Parse.Any(
//				Parse.Blanks().Skip(),
//				KeywordsParser,
//				Parse.Digits(),
//				SqlIdentifier,
//				TmpVariable,
//				SymbolsParser
//			).Many(1, 1).Then(Parse.Eos());
//			return p.ParseText(code);
//		}
//	}
//}
