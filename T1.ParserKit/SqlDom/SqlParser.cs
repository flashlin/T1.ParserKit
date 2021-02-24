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
using T1.Standard.DynamicCode;
using T1.Standard.Extensions;

namespace T1.ParserKit.SqlDom
{
	public static class SqlParser
	{
		public static IParser<SqlExpression> MapSqlExpr(this IParser<TextSpan> p)
		{
			return new Parser<SqlExpression>(p.Name, inp =>
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return Parse.Error<SqlExpression>(parsed.Error);
				}
				return Parse.Success(new SqlExpression()
				{
					TextSpan = parsed.Result
				});
			});
		}

		public static IParser<SqlExpression> MapSqlExpr<T>(this IParser<T> p)
		{
			return p.CastParser<SqlExpression>();
		}

		public static IParser<SqlExpression> SqlIdentifier = _SqlIdentifier();

		public static IParser<SqlExpression> _SqlIdentifier()
		{
			var start = Parse.Equal("[");
			var body = Parse.NotEqual("]").Many1();
			var end = Parse.Equal("]");
			var identifier = Parse.Seq(start, body, end).Merge();
			return identifier.Or(Parse.CStyleIdentifier).Named("SqlIdentifier")
				.MapResult(x => new SqlExpression()
				{
					TextSpan = x
				});
		}

		public static IParser<SqlExpression> LParen = SqlToken.Symbol("(");
		public static IParser<SqlExpression> RParen = SqlToken.Symbol(")");
		public static IParser<SqlExpression> SemiColon = SqlToken.Symbol(";");
		public static IParser<SqlExpression> Dot = SqlToken.Symbol(".");
		public static IParser<SqlExpression> Comma = SqlToken.Symbol(",");
		public static IParser<SqlExpression> Minus = SqlToken.Symbol("-");
		public static IParser<SqlExpression> At = SqlToken.Symbol("@");
		public static IParser<SqlExpression> Assign = SqlToken.Symbol("=");

		public static IParser<SqlExpression> SqlDataType =
			SqlToken.ContainsWord("DATETIME", "BIGINT");

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

		public static TextSpan GetTextSpan(this IEnumerable<SqlExpression> exprs)
		{
			return exprs.Select(x => x.TextSpan).GetTextSpan();
		}

		public static IParser<SqlExpression> Merge(this IParser<IEnumerable<SqlExpression>> parsers)
		{
			return parsers.Select(x => new SqlExpression()
			{
				TextSpan = x.Result.GetTextSpan()
			});
		}

		public static IParser<SqlFunctionExpression> FuncGetdate =
			from getdate in SqlToken.Word("GETDATE")
			from lparen in LParen
			from rparen in RParen
			select new SqlFunctionExpression
			{
				TextSpan = new[] { getdate, lparen, rparen }.GetTextSpan(),
				Name = "GETDATE",
				Parameters = new SqlExpression[0]
			};

		//DATEADD(DD,-1,DATEDIFF(dd, 0, GETDATE()))
		public static IParser<SqlFunctionExpression> FuncDateadd(IParser<SqlExpression> factor)
		{
			var datepart = SqlToken.ContainsWord(SqlToken.DateaddDetepart)
				.MapResult(x => new SqlOptionNameExpression()
				{
					Value = x.TextSpan.Text
				});

			return from dateadd in SqlToken.Word("DATEADD")
					 from lparen in LParen
					 from tDatepart in datepart
					 from comma1 in Comma
					 from tFactor1 in factor
					 from comma2 in Comma
					 from tFactor2 in factor
					 from rparen in RParen
					 select new SqlFunctionExpression()
					 {
						 Name = "DATEADD",
						 Parameters = new SqlExpression[]
						 {
						tDatepart,
						tFactor1,
						tFactor2
						 }
					 };
		}

		//DATEDIFF(dd, 0, GETDATE())
		public static IParser<SqlFunctionExpression> FuncDatediff(IParser<SqlExpression> factor)
		{
			var datepart = SqlToken.ContainsWord(SqlToken.DatediffDatepart)
				.MapResult(x => new SqlOptionNameExpression()
				{
					Value = x.TextSpan.Text
				});

			return from datediff1 in SqlToken.Word("DATEDIFF")
					 from lparen in LParen
					 from datepart1 in datepart
					 from comma1 in Comma
					 from numberExpr1 in NumberExpr
					 from comma2 in Comma
					 from factor1 in factor
					 from rparen in RParen
					 select new SqlFunctionExpression()
					 {
						 Name = "DATEDIFF",
						 Parameters = new SqlExpression[]
						 {
							datepart1,
							numberExpr1,
							factor1
						}
					 };
		}

		//ISNULL(@SblimitExpiredDate, xxx)
		public static IParser<SqlFunctionExpression> FuncIsnull(IParser<SqlExpression> factor)
		{
			return Parse.Seq(
				SqlToken.Word("ISNULL"),
				LParen,
				factor,
				Comma,
				factor,
				RParen)
				.MapResultList(x => new SqlFunctionExpression()
				{
					Name = "ISNULL",
					Parameters = new[]
					{
						x[2], x[4]
					}
				});
		}


		public static IParser<SqlExpression> SqlFunctions(IParser<SqlExpression> factor)
		{
			return Parse.AnyCast<SqlExpression>(
				FuncGetdate,
				FuncDateadd(factor),
				FuncIsnull(factor),
				FuncDatediff(factor),
				factor);
		}

		public static IParser<SetOptionExpression> SetNocountExpr =
				Parse.Seq(
					SqlToken.Word("SET"),
					SqlToken.Word("NOCOUNT"),
					SqlToken.ContainsWord("OFF", "ON"),
					SemiColon.Optional()
					).MapResultList(x => new SetOptionExpression()
					{
						OptionName = x[1].TextSpan.Text,
						IsToggle = string.Equals(x[2].TextSpan.Text.ToUpper(), "ON", StringComparison.Ordinal)
					});

		public static IParser<WithOptionExpression> WithOptionExpr =
			Parse.Seq(
				SqlToken.Word("with"),
				LParen,
				SqlToken.Word("nolock"),
				RParen
			).MapResult(x => new WithOptionExpression()
			{
				Nolock = true
			});

		public static IParser<SqlExpression> VariableAssignFieldExpr(IParser<SqlExpression> fieldExpr)
		{
			var assignExpr =
				from variable1 in Variable
				from assign1 in Assign
				from expr1 in fieldExpr
				select new VariableAssignFieldExpression()
				{
					VariableName = variable1,
					From = expr1,
				};
			return Parse.AnyCast<SqlExpression>(assignExpr, fieldExpr);
		}

		private static readonly HashSet<string> Keywords = new HashSet<string>(
			SqlToken.Keywords.Concat(SqlToken.Keywords.Select(x => x.ToLower())));

		public static IParser<SqlExpression> SqlIdentifierExcludeKeyword =
			SqlIdentifier.TransferToNext(rc =>
			{
				var ch = rc.TextSpan.Text;
				if (Keywords.Contains($"{ch}"))
				{
					return $"Expect not keyword, but got '{ch}'";
				}
				return "";
			});

		public static IParser<SqlExpression> Identifier =
			ParseToken.Lexeme(SqlIdentifierExcludeKeyword);

		public static IParser<VariableExpression> Variable =
			Parse.Seq(
			At,
				Identifier
			).Merge()
			.MapResult(x => new VariableExpression()
			{
				Name = x.TextSpan.Text
			});

		public static IParser<DeclareExpression> DeclareVariableExpr =
			from declare1 in SqlToken.Word("DECLARE")
			from variable1 in Variable
			from sqlDataType1 in SqlDataType
			select new DeclareExpression()
			{
				Name = variable1,
				DataType = sqlDataType1.GetText()
			};

		private static readonly IParser<FieldExpression> TableFieldExpr1 =
			Identifier.MapResult(x => new FieldExpression()
			{
				Name = x.GetText()
			});

		private static readonly IParser<FieldExpression> TableFieldExpr2 =
			Parse.Seq(Identifier, Dot, Identifier)
				.MapResultList(x => new FieldExpression()
				{
					Name = x[2].GetText(),
					From = x[0].GetText()
				});

		private static readonly IParser<FieldExpression> TableFieldExpr3 =
			Parse.Seq(Identifier, Dot, Identifier, Dot, Identifier)
				.MapResultList(x => new FieldExpression()
				{
					Name = x[4].GetText(),
					From = $"{x[0].GetText()}.{x[2].GetText()}"
				});

		public static IParser<FieldExpression> TableFieldExpr =
			Parse.Any(TableFieldExpr3, TableFieldExpr2, TableFieldExpr1)
				.Named(nameof(TableFieldExpr));


		public static readonly IParser<FieldExpression> TableFieldAliasExpr =
			from tableField1 in TableFieldExpr
			from alias1 in AliasExpr.Optional()
			select tableField1.Assign(x =>
			{
				x.AliasName = alias1?.Name;
			});


		public static IParser<SqlExpression> RecFieldExpr(IParser<SqlExpression> factor)
		{
			return VariableAssignFieldExpr(factor);
		}

		public static IParser<SqlExpression> ArithmeticOperatorExpr(IParser<SqlExpression> atom)
		{
			return Parse.RecGroupOperatorExpr(LParen, atom, RParen, new[]
			{
				SqlToken.Symbol("*"),
				SqlToken.Symbol("/"),
				SqlToken.Symbol("+"),
				SqlToken.Symbol("-")
			}, x => new ArithmeticOperatorExpression
			{
				Left = x[0],
				Oper = x[1].GetText(),
				Right = x[2]
			});
		}

		public static IParser<NumberExpression> IntegerExpr =
			ParseToken.Lexeme(Parse.Digits)
				.MapResult(x => new NumberExpression()
				{
					ValueTypeFullname = typeof(int).FullName,
					Value = int.Parse(x.Text)
				});

		public static IParser<NumberExpression> NegativeIntegerExpr =
			ParseToken.Lexeme(Minus, SqlToken.Digits)
				.MapResultList(x => new NumberExpression()
				{
					ValueTypeFullname = typeof(int).FullName,
					Value = int.Parse($"{x[0].GetText()}{x[1].GetText()}")
				});

		public static IParser<NumberExpression> NumberExpr =
			Parse.Any(NegativeIntegerExpr, IntegerExpr);

		public static IParser<SqlExpression> Atom =
			Parse.AnyCast<SqlExpression>(
					FuncGetdate,
					TableFieldAliasExpr,
					NumberExpr,
					Variable);

		public static IParser<SqlExpression> ArithmeticOperatorAtomExpr =
			ArithmeticOperatorExpr(Atom);

		public static IParser<FieldsExpression> FieldsExpr =
			RecFieldExpr(ArithmeticOperatorAtomExpr.MapSqlExpr())
				.ManyDelimitedBy(Comma)
				.MapResultList(x => new FieldsExpression()
				{
					Items = x.TakeEvery(1).ToList()
				});

		public static IParser<FilterExpression> FilterExpr(IParser<SqlExpression> atom)
		{
			var oper = SqlToken.Symbols(">=", "<=", "!=", ">", "<", "=");
			return Parse.Seq(
				atom,
				oper,
				atom
				).MapResultList(x => new FilterExpression()
				{
					Left = x[0],
					Oper = x[1].GetText(),
					Right = x[2],
				});
		}

		public static IParser<WhereExpression> WhereExpr =
			from _ in SqlToken.Word("WHERE")
			from filter1 in FilterExpr(Atom)
			select new WhereExpression()
			{
				Filter = filter1
			};

		public static IParser<SelectExpression> SelectExpr =
			from select1 in SqlToken.Word("SELECT")
			from fields1 in FieldsExpr
			from from1 in SqlToken.Word("FROM")
			from table1 in TableExpr
			from where1 in WhereExpr.Optional()
			select new SelectExpression()
			{
				Fields = fields1,
				From = table1,
				Where = where1
			};

		private static readonly IParser<ObjectNameExpression> DatabaseDboSchemaName3 =
			Parse.Seq(Identifier,
				Dot,
				Identifier,
				Dot,
				Identifier
			).Merge()
			.MapResult(x => new ObjectNameExpression()
			{
				Name = x.GetText()
			});

		private static readonly IParser<ObjectNameExpression> DatabaseDboSchemaName2 =
			Parse.Seq(Identifier,
				Dot,
				Identifier
			).Merge()
			.MapResult(x => new ObjectNameExpression()
			{
				Name = x.GetText()
			});

		private static readonly IParser<ObjectNameExpression> DatabaseDboSchemaName1 =
			Identifier
			.MapResult(x => new ObjectNameExpression()
			{
				Name = x.GetText()
			});

		public static readonly IParser<ObjectNameExpression> DatabaseSchemaObjectName =
			Parse.Any(DatabaseDboSchemaName3, 
				DatabaseDboSchemaName2, 
				DatabaseDboSchemaName1);

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

		private static readonly IParser<AliasExpression> AliasExpr =
			from as1 in SqlToken.Word("AS").Optional()
			from identifier in Identifier
			select new AliasExpression()
			{
				Name = identifier.GetText()
			};

		public static IParser<TableExpression> TableExpr =
			from databaseTable1 in DatabaseSchemaObjectName
			from withOption1 in WithOptionExpr.Optional()
			from alias1 in AliasExpr.Optional()
			select new TableExpression()
			{
				Name = databaseTable1.Name,
				AliasName = alias1?.Name,
				WithOption = withOption1
			};

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
	}
}
