using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using T1.ParserKit.Core;
using T1.ParserKit.Core.Parsers;
using T1.ParserKit.Helpers;
using T1.ParserKit.SqlDom.Expressions;
using T1.Standard.Common;
using T1.Standard.Extensions;

namespace T1.ParserKit.SqlDom
{
	public class SqlParser
	{
		public static IParser<TextSpan> SqlIdentifier = _SqlIdentifier();

		public static IParser<TextSpan> _SqlIdentifier()
		{
			var start = Parse.Equal("[");
			var body = Parse.NotEqual("]").Many1();
			var end = Parse.Equal("]");
			var identifier = Parse.Sequence(start, body, end).Merge();
			return identifier.Or(Parse.CStyleIdentifier)
					.Named("SqlIdentifier");
		}

		public static IParser<TextSpan> LParen = ParseToken.Symbol("(");
		public static IParser<TextSpan> RParen = ParseToken.Symbol(")");
		public static IParser<TextSpan> SemiColon = ParseToken.Symbol(";");
		public static IParser<TextSpan> Dot = ParseToken.Symbol(".");
		public static IParser<TextSpan> Comma =	ParseToken.Symbol(",");
		public static IParser<TextSpan> Minus =	ParseToken.Symbol("-");

		public static IParser<TextSpan> SqlDataType =
			ParseToken.Contains("DATETIME", "BIGINT");

		//public static IParser<SqlFunctionExpression> FuncGetdate =
		//	Parse.Sequence(ParseToken.Match("GETDATE"),
		//			LParen,
		//			RParen)
		//		.MapResult(x =>
		//		{
		//			var xs = x.CastArray();
		//			return new SqlFunctionExpression()
		//			{
		//				TextSpan = xs.GetTextSpan(),
		//				Name = "GETDATE",
		//				Parameters = new SqlExpression[0]
		//			};
		//		});

		public static IParser<SqlFunctionExpression> FuncGetdate =
			from getdate in ParseToken.Match("GETDATE")
			from lparen in LParen
			from rparen in RParen
			select new SqlFunctionExpression
			{
				TextSpan = new[] { getdate, lparen, rparen }.GetTextSpan(),
				Name = "GETDATE",
				Parameters = new SqlExpression[0]
			};

		////DATEADD(DD,-1,DATEDIFF(dd, 0, GETDATE()))
		//public IParser FuncDateadd(IParser factor)
		//{
		//	var datepart = ContainsText(DateaddDetepart)
		//		.MapResult(x => new SqlOptionNameExpression()
		//		{
		//			Value = x[0].GetText()
		//		});

		//	return Parse.Chain(
		//		Match("DATEADD"),
		//		LParen,
		//		datepart,
		//		Comma,
		//		factor,
		//		Comma,
		//		factor,
		//		RParen
		//	).MapResult(x => new SqlFunctionExpression()
		//	{
		//		Name = "DATEADD",
		//		Parameters = new[]
		//		{
		//			(SqlExpression)x[2],
		//			(SqlExpression)x[4],
		//			(SqlExpression)x[6],
		//		}
		//	});
		//}

		//static readonly string[] DatediffDatepartStr = new[]
		//{
		//	"year", "quarter", "month", "dayofyear", "day",
		//	"week", "hour", "minute", "second", "millisecond",
		//	"microsecond", "nanosecond"
		//};

		//static readonly string[] DatediffAbbreviationDatepartStr = new[]
		//{
		//	"yy", "yyyy", "qq", "q", "mm", "m",
		//	"dy", "y", "dd", "d", "wk", "ww",
		//	"hh", "mi", "n", "ss", "s", "ms",
		//	"mcs", "ns"
		//};

		//private static readonly string[] DatediffDatepart = DatediffDatepartStr
		//	.Concat(DatediffAbbreviationDatepartStr)
		//	.OrderByDescending(x => x.Length)
		//	.ToArray();

		//private static readonly string[] DateaddDatepartStr = new[]
		//{
		//	"year", "quarter", "month", "dayofyear", "day",
		//	"week", "weekday", "hour", "minute", "second", "millisecond", "microsecond",
		//	"nanosecond"
		//};

		//private static readonly string[] DateaddAbbreviationDatepartStr = new[]
		//{
		//	"yy", "yyyy", "qq", "q", "mm", "m", "dy", "y",
		//	"dd", "d", "wk", "ww", "dw", "w", "hh", "mi", "n",
		//	"ss", "s", "ms", "mcs", "ns"
		//};

		//private static readonly string[] DateaddDetepart = DateaddDatepartStr.Concat(DateaddAbbreviationDatepartStr)
		//	.OrderByDescending(x => x.Length)
		//	.ToArray();

		////DATEDIFF(dd, 0, GETDATE())
		//public IParser FuncDatediff(IParser factor)
		//{
		//	var datepart = ContainsText(DatediffDatepart)
		//		.MapResult(x => new SqlOptionNameExpression()
		//		{
		//			Value = x[0].GetText()
		//		});

		//	return Parse.Chain(
		//		Match("DATEDIFF"),
		//		LParen,
		//		datepart,
		//		Comma,
		//		NumberExpr,
		//		Comma,
		//		factor,
		//		RParen
		//		).MapResult(x => new SqlFunctionExpression()
		//		{
		//			Name = "DATEDIFF",
		//			Parameters = new[]
		//		{
		//			(SqlExpression)x[2],
		//			(SqlExpression)x[4],
		//			(SqlExpression)x[6]
		//		}
		//		});
		//}

		//public IParser SqlFunctions(IParser factor)
		//{
		//	return Parse.Any(
		//		FuncGetdate(),
		//		FuncDateadd(factor),
		//		FuncIsnull(factor),
		//		FuncDatediff(factor),
		//		factor);
		//}

		////ISNULL(@SblimitExpiredDate, xxx)
		//public IParser FuncIsnull(IParser factor)
		//{
		//	return Parse.Chain(
		//		Match("ISNULL"),
		//		LParen,
		//		factor,
		//		Comma,
		//		factor,
		//		RParen)
		//		.MapResult(x => new SqlFunctionExpression()
		//		{
		//			Name = "ISNULL",
		//			Parameters = new[] { (SqlExpression)x[2], (SqlExpression)x[4] }
		//		});
		//}

		//public IParser SetNocountExpr
		//{
		//	get
		//	{
		//		var onOFf = ContainsText("OFF", "ON");
		//		return Parse.Chain(
		//			Match("SET"),
		//			Match("NOCOUNT"),
		//			onOFf,
		//			SemiColon.Optional()
		//			).MapResult(x => new SetOptionExpression()
		//			{
		//				OptionName = x[1].GetText(),
		//				IsToggle = string.Equals(x[2].GetText().ToUpper(), "ON", StringComparison.Ordinal)
		//			});
		//	}
		//}

		//public IParser DeclareVariableExpr
		//{
		//	get
		//	{
		//		return Parse.Chain(
		//			Match("DECLARE"),
		//			Variable,
		//			SqlDataType)
		//			.MapResult(x => new DeclareExpression()
		//			{
		//				Name = x[1].GetText(),
		//				DataType = x[2].GetText()
		//			});
		//	}
		//}

		//public IParser WithOptionExpr
		//{
		//	get
		//	{
		//		return Parse.Chain(
		//			Match("with"),
		//			LParen,
		//			Match("nolock"),
		//			RParen
		//		).MapResult(x => new WithOptionExpression()
		//		{
		//			Nolock = true
		//		});
		//	}
		//}

		//public IParser Variable
		//{
		//	get
		//	{
		//		return Parse.Chain(
		//			Symbol("@"),
		//			Identifier()
		//		).Merge()
		//		.MapResult(x => new VariableExpression()
		//		{
		//			Name = x[0].GetText()
		//		});
		//	}
		//}

		//public IParser VariableAssignFieldExpr(IParser fieldExpr)
		//{
		//	var assignField = Parse.Chain(
		//		Variable,
		//		Symbol("="),
		//		fieldExpr)
		//		.MapAssign<VariableAssignFieldExpression>((x, expr) =>
		//		{
		//			var f = (FieldExpression)x[2];
		//			expr.VariableName = x[0].GetText();
		//			expr.Name = f.Name;
		//			expr.From = f.From;
		//			expr.AliasName = f.AliasName;
		//		});
		//	return Parse.Any(assignField, fieldExpr);
		//}
		
		private static readonly HashSet<string> Keywords = new HashSet<string>(
			SqlToken.Keywords.Concat(SqlToken.Keywords.Select(x => x.ToLower())));

		public static IParser<TextSpan> SqlIdentifierExcludeKeyword =
			SqlIdentifier.TransferToNext(rc =>
			{
				var ch = rc.Text;
				if (Keywords.Contains($"{ch}"))
				{
					return $"Expect not keyword, but got '{ch}'";
				}
				return "";
			});
		
		public static IParser<TextSpan> Identifier =
			ParseToken.Lexeme(SqlIdentifierExcludeKeyword);

		public static IParser<FieldExpression> TableFieldExpr1 =
			Identifier.MapResult(x => new FieldExpression()
				{
					Name = x.Text
				});

		public static IParser<FieldExpression> TableFieldExpr2 =
			Parse.Sequence(Identifier, Dot, Identifier)
				.MapResult(x => new FieldExpression()
				{
					Name = x[2].Text,
					From = x[0].Text
				});

		public static IParser<FieldExpression> TableFieldExpr3 =
			Parse.Sequence(Identifier, Dot, Identifier, Dot, Identifier)
				.MapResult(x => new FieldExpression()
				{
					Name = x[4].Text,
					From = $"{x[0].Text}.{x[2].Text}"
				});

		public static IParser<FieldExpression> TableFieldExpr =
			Parse.Any(TableFieldExpr3, TableFieldExpr2, TableFieldExpr1)
				.Named(nameof(TableFieldExpr));

		//public IParser FieldExpr
		//{
		//	get
		//	{
		//		return Parse.Any(field3, field2, field1).Named("FieldExpr");
		//	}
		//}

		//public IParser RecFieldExpr(IParser factor)
		//{
		//	return VariableAssignFieldExpr(factor);
		//}

		//public IParser FieldsExpr
		//{
		//	get
		//	{
		//		var fieldExpr = RecFieldExpr(ArithmeticOperatorExpr());

		//		var fields = fieldExpr.ManyDelimitedBy(Comma)
		//			.MapResult(x => new FieldsExpression()
		//			{
		//				Items = x.TakeEvery(1).Cast<SqlExpression>().ToList()
		//			});

		//		var fields1 = fieldExpr
		//			.MapResult(x => new FieldsExpression()
		//			{
		//				Items = new List<SqlExpression>()
		//				{
		//					x[0] as SqlExpression
		//				}
		//			});

		//		return fields.Or(fields1).Named("FieldsExpr");
		//	}
		//}

		//public IParser Atom
		//{
		//	get
		//	{
		//		return Parse.Any(
		//			FuncGetdate(),
		//			FieldExpr,
		//			NumberExpr,
		//			Variable);
		//	}
		//}

		//public IParser ArithmeticOperatorExpr(IParser atom)
		//{
		//	return Parse.RecGroupOperatorExpr(LParen, atom, RParen, new[]
		//	{
		//		Symbol("*"),
		//		Symbol("/"),
		//		Symbol("+"),
		//		Symbol("-")
		//	}, x => new ArithmeticOperatorExpression
		//	{
		//		Left = (SqlExpression)x[0],
		//		Oper = x[1].GetText(),
		//		Right = (SqlExpression)x[2]
		//	});
		//}

		//public IParser ArithmeticOperatorExpr()
		//{
		//	return ArithmeticOperatorExpr(Atom);
		//}

		//public IParser FilterExpr(IParser atom)
		//{
		//	var oper = ContainsSymbol(">=", "<=", "!=", ">", "<", "=");
		//	return Parse.Chain(
		//		atom,
		//		oper,
		//		atom
		//		).MapResult(x => new FilterExpression()
		//		{
		//			Left = (SqlExpression)x[0],
		//			Oper = x[1].GetText(),
		//			Right = (SqlExpression)x[2],
		//		});
		//}

		public static IParser<NumberExpression> IntegerExpr =
			ParseToken.Lexeme(Parse.Digits)
				.MapResult(x => new NumberExpression()
				{
					ValueTypeFullname = typeof(int).FullName,
					Value = int.Parse(x.Text)
				});

		public static IParser<NumberExpression> NegativeIntegerExpr =
			ParseToken.Lexeme(Minus, Parse.Digits)
				.MapResult(x => new NumberExpression()
				{
					ValueTypeFullname = typeof(int).FullName,
					Value = int.Parse($"{x[0].Text}{x[1].Text}")
				});

		//public IParser NumberExpr
		//{
		//	get
		//	{
		//		var number = SkipBlanks(Parse.Digits().Assertion(true))
		//			.Named("NumberExpr")
		//			.MapResult(x => new NumberExpression()
		//			{
		//				ValueTypeFullname = typeof(int).FullName,
		//				Value = int.Parse(x[0].GetText())
		//			});

		//		var negativeNumber = Parse.Chain(
		//				Symbol("-"),
		//				number)
		//			.MapResult(x =>
		//			{
		//				var numExpr = (NumberExpression)x[1];
		//				numExpr.Value = -(int)numExpr.Value;
		//				return numExpr;
		//			});

		//		return Parse.Any(negativeNumber, number);
		//	}
		//}

		//public IParser WhereExpr
		//{
		//	get
		//	{
		//		return Parse.Chain(
		//			Match("WHERE"),
		//			FilterExpr(Atom)
		//		)
		//		.Named("WhereExpr")
		//		.MapResult(x => new WhereExpression()
		//		{
		//			Filter = (FilterExpression)x[1]
		//		});
		//	}
		//}

		//public IParser DatabaseDboSchemaName =>
		//	Parse.Chain(Identifier(),
		//		Symbol("."),
		//		Identifier(),
		//		Symbol("."),
		//		Identifier()
		//	).Merge()
		//	.MapResult(x => new ObjectNameExpression()
		//	{
		//		Name = x[0].GetText()
		//	});

		//private IParser DboSchemaName =>
		//	Parse.Chain(Identifier(),
		//		Symbol("."),
		//		Identifier()
		//	).Merge()
		//	.MapResult(x => new ObjectNameExpression()
		//	{
		//		Name = x[0].GetText()
		//	});

		//private IParser SchemaName =>
		//	Identifier()
		//	.MapResult(x => new ObjectNameExpression()
		//	{
		//		Name = x[0].GetText()
		//	});

		//private IParser DatabaseSchemaObjectName =>
		//	Parse.Any(DatabaseDboSchemaName, DboSchemaName, SchemaName);


		//private IParser SetFieldEqualExpr(IParser factor)
		//{
		//	return Parse.Chain(
		//		Match("SET"),
		//		SchemaName,
		//		Symbol("="),
		//		factor)
		//		.MapResult(x => new UpdateSetFieldExpression()
		//		{
		//			FieldName = x[1].GetText(),
		//			AssignExpr = (SqlExpression)x[3]
		//		});
		//}

		//private IParser SetFieldEqualExprs(IParser factor)
		//{
		//	return SetFieldEqualExpr(factor).ManyDelimitedBy(Comma)
		//		.MapResult(x => new ManySqlExpression()
		//		{
		//			Items = x.TakeEvery(1).ToArray()
		//		});
		//}

		//public IParser UpdateExpr(IParser factor)
		//{
		//	var updateExpr = Parse.Chain(
		//		Match("UPDATE"),
		//		DatabaseSchemaObjectName,
		//		SetFieldEqualExprs(factor),
		//		WhereExpr.Optional()
		//	).MapResult(x => new UpdateExpression()
		//	{
		//		SetFields = ((ManySqlExpression)x[2]).Items.Cast<UpdateSetFieldExpression>().ToArray(),
		//		WhereExpr = x.FirstCast<WhereExpression>()
		//	});
		//	return Parse.Any(updateExpr, factor);
		//}

		//public IParser SelectExpr =>
		//	Parse.Chain(
		//		Match("SELECT"),
		//		FieldsExpr,
		//		Match("FROM"),
		//		TableExpr,
		//		WhereExpr.Optional()
		//	).MapResult(x => new SelectExpression()
		//	{
		//		Fields = x[1] as FieldsExpression,
		//		From = x[3] as TableExpression,
		//		Where = x.FirstCast<WhereExpression>()
		//	}).Named("SelectExpr");

		//public IParser Group(IParser p)
		//{
		//	return Parse.Chain(
		//		Symbol("("),
		//		p,
		//		Symbol(")"));
		//}

		//public IParser RecSelectExpr(IParser factor)
		//{
		//	var subTableExpr =
		//		Parse.Chain(
		//			Group(factor),
		//			Identifier())
		//			.MapResult(x => new SourceExpression()
		//			{
		//				Item = x[1] as SqlExpression,
		//				AliasName = x[3].GetText()
		//			}).Named("SubTableExpr");

		//	var recSelectExpr = Parse.Chain(
		//		Match("SELECT"),
		//		FieldsExpr,
		//		Match("FROM"),
		//		subTableExpr
		//	)
		//	.MapResult(x => new SelectExpression()
		//	{
		//		Fields = x[1] as FieldsExpression,
		//		From = x[3] as SqlExpression
		//	});

		//	return recSelectExpr.Or(factor);
		//}

		//public IParser TableExpr
		//{
		//	get
		//	{
		//		var withOption = WithOptionExpr.Many(0, 1);

		//		var table1 =
		//			Parse.Chain(Identifier(),
		//					withOption)
		//			.MapResult(x => new TableExpression()
		//			{
		//				Name = x[0].GetText(),
		//				WithOption = x.FirstCast<WithOptionExpression>()
		//			});

		//		var table2 =
		//			Parse.Chain(
		//				Identifier(),
		//				Identifier()
		//			).MapResult(x => new TableExpression()
		//			{
		//				Name = x[0].GetText(),
		//				AliasName = x[1].GetText()
		//			});

		//		var table3 =
		//			Parse.Chain(
		//				Identifier(),
		//				Match("as"),
		//				Identifier()
		//			).MapResult(x => new TableExpression()
		//			{
		//				Name = x[0].GetText(),
		//				AliasName = x[2].GetText()
		//			});

		//		return Parse.Any(table3, table2, table1);
		//	}
		//}

		//public IParser IfExpr(IParser factor)
		//{
		//	var body = factor.AtLeastOnce()
		//		.MapResult(x => new StatementsExpression()
		//		{
		//			Items = x.Cast<SqlExpression>().ToArray()
		//		});

		//	var groupFilterExpr =
		//		Parse.Chain(LParen, FilterExpr(Atom), RParen)
		//			.MapResult(x => x[1]);

		//	var conditionExpr = Parse.Any(groupFilterExpr, FilterExpr(Atom));

		//	var ifExpr = Parse.Chain(
		//		Match("IF"),
		//		conditionExpr,
		//		Match("BEGIN"),
		//		body,
		//		Match("END"))
		//		.MapResult(x => new IfExpression()
		//		{
		//			Condition = (FilterExpression)x[1],
		//			Body = (StatementsExpression)x[3]
		//		});

		//	return Parse.Any(ifExpr, factor);
		//}

		//public IParser Recursive(IParser factor, IEnumerable<Func<IParser, IParser>> parsers)
		//{
		//	var curr = (IParser)null;
		//	foreach (var parser in parsers)
		//	{
		//		curr = parser(curr ?? factor);
		//	}
		//	return curr;
		//}

		//public IParser StartExpr()
		//{
		//	var statementExpr = Parse.Any(
		//		RecSelectExpr(SelectExpr),
		//		DeclareVariableExpr,
		//		SetNocountExpr,
		//		SqlFunctions(Atom));

		//	var ifExpr = IfExpr(statementExpr);
		//	var updateExpr = UpdateExpr(ifExpr);

		//	var lastExpr =
		//		Recursive(updateExpr, new Func<IParser, IParser>[]
		//		{
		//			IfExpr,
		//			SqlFunctions
		//		});

		//	return lastExpr;
		//}

		//public SqlExpression[] ParseText(string code)
		//{
		//	return StartExpr().ParseText(code).Cast<SqlExpression>().ToArray();
		//}

		//public IParser<TextSpan> Identifier =
		//	ParseToken.Lexeme(SqlIdentifier);
		//{
		//	return SkipBlanks(SqlIdentifier.Next(NotKeyword));
		//}

		//protected static IParser Symbol(string text)
		//{
		//	return SkipBlanks(
		//		Parse.Equal(text)
		//	);
		//}

		//protected IParser ContainsText(params string[] texts)
		//{
		//	return SkipBlanks(
		//		Parse.Contains(texts, true).Assertion(true)
		//	);
		//}

		//protected IParser ContainsSymbol(params string[] symbols)
		//{
		//	return SkipBlanks(
		//		Parse.Contains(symbols)
		//	);
		//}

		//protected IParser SkipBlanks(IParser p)
		//{
		//	return new[] {
		//		Parse.Blanks().Many().Skip(),
		//		p
		//	}.Chain(TODO).Named($">>{p.Name}");
		//}
	}
}
