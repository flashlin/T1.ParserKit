using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T1.ParserKit.Core;
using T1.ParserKit.Core.Parsers;
using T1.ParserKit.Helpers;
using T1.ParserKit.SqlDom.Expressions;
using T1.Standard.Extensions;

namespace T1.ParserKit.SqlDom
{
	public static class SqlToken
	{
		public static readonly string[] UpperKeywords = new[]
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

		public static readonly IParser<SqlExpression> Digits =
			Parse.Digits.MapResult(x => new SqlExpression()
			{
				TextSpan = x
			});

		public static readonly IParser<SqlCommentExpression> Comment1 =
			from start1 in Parse.Equal("--")
			from body in Parse.NewLine.Not().ThenRight(Parse.AnyChars(1)).Many()
			from end1 in Parse.Any(Parse.NewLine, Parse.Eos<TextSpan>())
			select new SqlCommentExpression()
			{
				IsMultipleLines = false,
				Content = body.Text
			};

		public static readonly IParser<SqlCommentExpression> Comment2 =
			Parse.CStyleComment2
				.MapResult(x => new SqlCommentExpression()
				{
					IsMultipleLines = true,
					Content = x.Text
				});

		public static readonly IParser<SqlCommentExpression> Comment =
			Parse.Any(Comment2, Comment1)
				.Named("Comment");

		private static readonly IParser<SqlExpression> Blanks1 =
			from blanks1 in Parse.Blanks
			select new SqlExpression()
			{
				TextSpan = blanks1
			};

		public static readonly IParser<IEnumerable<SqlExpression>> Blanks =
			Parse.RepeatAny(
				Blanks1,
				Comment.CastParser<SqlExpression>()
			).Named(nameof(Blanks));

		private static IParser<SqlExpression> Surrounded(IParser<TextSpan> mark)
		{
			var doubleQuotation = Parse.Seq(mark, mark).Merge();
			var notMark = mark.Not().ThenRight(Parse.AnyChars(1));

			return from start1 in mark
					 from body1 in Parse.Any(doubleQuotation, notMark).Many()
					 from end1 in mark
					 select new SqlExpression()
					 {
						 TextSpan = body1.Length == 0 ?
							 new[] { start1, end1 }.GetTextSpan() :
							 new[] { start1, body1, end1 }.GetTextSpan()
					 };
		}

		public static readonly IParser<SqlExpression> String2 =
			Surrounded(Parse.Equal("\""));

		public static readonly IParser<SqlStringExpression> String1 =
			Surrounded(Parse.Equal("'"))
				.MapResult(x => new SqlStringExpression()
				{
					TextSpan = x.TextSpan,
					Text = x.GetText().GetCStyleStringText()
				});

		public static readonly IParser<SqlStringExpression> NString =
			from n1 in Parse.Equal("N")
			from s1 in String1
			select new SqlStringExpression()
			{
				TextSpan = new[] { n1, s1.TextSpan }.GetTextSpan(),
				IsUnicode = true,
				Text = s1.GetText().GetCStyleStringText()
			};

		public static readonly IParser<SqlExpression> LexemeString =
			Lexeme(Parse.AnyCast<SqlExpression>(NString, String1));

		public static readonly IParser<SqlNullExpression> Null =
			from null1 in Word("NULL")
			select new SqlNullExpression()
			{
				TextSpan = null1.TextSpan,
			};

		public static IParser<T> Lexeme<T>(IParser<T> parser)
		{
			return (
				from blanks1 in Blanks
				from p1 in parser
				select p1).Named($"\\s*{parser.Name}");
		}

		public static IParser<SqlExpression> Word(string text)
		{
			return Lexeme(ParseToken.Match(text))
				.MapResult(x => new SqlExpression()
				{
					TextSpan = x
				});
		}

		public static IParser<SqlExpression> Symbol(string text)
		{
			return ParseToken.Symbol(text)
				.MapResult(x => new SqlExpression()
				{
					TextSpan = x
				});
		}

		public static IParser<SqlExpression> Contains(params string[] texts)
		{
			return Lexeme(ParseToken.Matchs(texts))
				.MapResult(x => new SqlExpression()
				{
					TextSpan = x
				});
		}

		public static IParser<SqlExpression> Symbols(params string[] texts)
		{
			return Lexeme(ParseToken.Symbols(texts))
				.MapResult(x => new SqlExpression()
				{
					TextSpan = x
				});
		}

		public static IParser<SqlExpression> ToExpr(this IParser<TextSpan> p)
		{
			return from p1 in p
					 select new SqlExpression()
					 {
						 TextSpan = p1
					 };
		}

		public static readonly IParser<SqlExpression> LParen = SqlToken.Symbol("(");
		public static readonly IParser<SqlExpression> RParen = SqlToken.Symbol(")");
		public static readonly IParser<SqlExpression> SemiColon = SqlToken.Symbol(";");
		public static readonly IParser<SqlExpression> Dot = SqlToken.Symbol(".");
		public static readonly IParser<SqlExpression> Comma = SqlToken.Symbol(",");
		public static readonly IParser<SqlExpression> Minus = SqlToken.Symbol("-");
		public static readonly IParser<SqlExpression> At = SqlToken.Symbol("@");
		public static readonly IParser<SqlExpression> Assign = SqlToken.Symbol("=");
		public static readonly IParser<SqlExpression> DollarSign = SqlToken.Symbol("$");

		public static readonly IParser<SqlIdentifierExpression> SqlIdentifier =
			Parse.Named<SqlIdentifierExpression>(_SqlIdentifier(), nameof(SqlIdentifier));

		private static readonly HashSet<string> Keywords = new HashSet<string>(
			SqlToken.UpperKeywords.Concat(SqlToken.UpperKeywords.Select(x => x.ToLower())));

		public static readonly IParser<SqlIdentifierExpression> SqlIdentifierExcludeKeyword = SqlIdentifier.TransferToNext(rc =>
			{
				var ch = rc.TextSpan.Text;
				if (Keywords.Contains($"{ch}"))
				{
					return $"Expect not keyword, but got '{ch}'";
				}

				return "";
			});

		public static readonly IParser<SqlIdentifierExpression> Identifier =
			ParseToken.Lexeme(SqlIdentifierExcludeKeyword);

		private static IParser<SqlIdentifierExpression> _SqlIdentifier()
		{
			var cstyleIdentifier =
				from ident in Parse.CStyleIdentifier
				select new SqlIdentifierExpression()
				{
					TextSpan = ident,
					Name = ident.Text
				};

			var sqlIdentifier =
				from start in Parse.Equal("[")
				from body in Parse.NotEqual("]").Many1()
				from end in Parse.Equal("]")
				select new SqlIdentifierExpression()
				{
					TextSpan = new[] { start, body, end }.GetTextSpan(),
					Name = start.Text + body.Text + end.Text
				};

			return sqlIdentifier.Or(cstyleIdentifier);
		}

		private static readonly string[] SqlDataType0Words =
			new[]
			{
				"bit", "smallint", "smallmoney", "int", "tinyint",
				"money", "real", "date", "smalldatetime", "datetime",
				"image", "text", "ntext"
			}.OrderByDescending(x => x.Length).ToArray();

		public static readonly IParser<SqlExpression> SqlDataType0 =
			SqlToken.Contains(SqlDataType0Words);

		private static readonly string[] SqlDataType1Words =
			new[]
			{
				"bigint", "bit", "float", "datetime2", "time",
				"char", "varchar", "binary", "varbinary", "nchar",
				"nvarchar", "datetimeoffset"
			}.OrderByDescending(x => x.Length).ToArray();

		public static readonly IParser<SqlExpression> SqlDataType01 =
			SqlToken.Contains(SqlDataType0Words.Concat(SqlDataType1Words)
				.OrderByDescending(x => x.Length).ToArray());

		public static readonly IParser<SqlExpression> SqlDataType1 =
			SqlToken.Contains(SqlDataType1Words);

		public static readonly IParser<SqlExpression> SqlDataType2 =
			SqlToken.Contains("decimal", "numeric");

		public static readonly IParser<SqlExpression> SqlDataType =
			Parse.Any(SqlDataType0, SqlDataType1, SqlDataType2);
	}
}
