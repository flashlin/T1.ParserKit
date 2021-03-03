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

		public static IParser<SqlExpression> SqlDataType0 =
			SqlToken.Contains(
				"bit", "smallint", "smallmoney", "int", "tinyint",
				"money", "real", "date", "smalldatetime", "datetime",
				"image", "text", "ntext"
			);

		public static IParser<SqlExpression> SqlDataType1 =
			SqlToken.Contains(
				"bigint", "bit", "float", "datetime2", "time",
				"char", "varchar", "binary", "varbinary", "nchar",
				"nvarchar", "datetimeoffset"
			);

		public static IParser<SqlExpression> SqlDataType2 =
			SqlToken.Contains("decimal");

		public static IParser<SqlExpression> SqlDataType =
			Parse.Any(SqlDataType0, SqlDataType1, SqlDataType2);
		//SqlToken.ContainsWord(
		//	"bigint", "numeric", "bit", "smallint", "decimal",
		//	"smallmoney", "int", "tinyint", "money", "float",
		//	"real", "date", "datetimeoffset", "datetime2", "smalldatetime",
		//	"datetime", "time", "char", "varchar", "text",
		//	"nchar", "nvarchar", "ntext", "binary", "varbinary",
		//	"image"
		//);

		public static TextSpan GetTextSpan(this IEnumerable<SqlExpression> exprs)
		{
			return exprs.Select(x => x.TextSpan).GetTextSpan();
		}

		public static IParser<SqlExpression> Merge(this IParser<IEnumerable<SqlExpression>> parsers)
		{
			return parsers.Select(x => new SqlExpression()
			{
				TextSpan = x.Select(item => item.TextSpan).GetTextSpan()
			});
		}

		public static IParser<SqlFunctionExpression> FuncGetdate =
			from getdate in SqlToken.Word("GETDATE")
			from lparen in SqlToken.LParen
			from rparen in SqlToken.RParen
			select new SqlFunctionExpression
			{
				TextSpan = new[] { getdate, lparen, rparen }.GetTextSpan(),
				Name = "GETDATE",
				Parameters = new SqlExpression[0]
			};

		//DATEADD(DD,-1,DATEDIFF(dd, 0, GETDATE()))
		public static IParser<SqlFunctionExpression> FuncDateadd(IParser<SqlExpression> factor)
		{
			var datepart = SqlToken.Contains(SqlToken.DateaddDetepart)
				.MapResult(x => new SqlOptionNameExpression()
				{
					Value = x.TextSpan.Text
				});

			return from dateadd in SqlToken.Word("DATEADD")
					 from lparen in SqlToken.LParen
					 from tDatepart in datepart
					 from comma1 in SqlToken.Comma
					 from tFactor1 in factor
					 from comma2 in SqlToken.Comma
					 from tFactor2 in factor
					 from rparen in SqlToken.RParen
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
			var datepart = SqlToken.Contains(SqlToken.DatediffDatepart)
				.MapResult(x => new SqlOptionNameExpression()
				{
					Value = x.TextSpan.Text
				});

			return from datediff1 in SqlToken.Word("DATEDIFF")
					 from lparen in SqlToken.LParen
					 from datepart1 in datepart
					 from comma1 in SqlToken.Comma
					 from numberExpr1 in NumberExpr
					 from comma2 in SqlToken.Comma
					 from factor1 in factor
					 from rparen in SqlToken.RParen
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
				SqlToken.Word("ISNULL"), SqlToken.LParen,
				factor, SqlToken.Comma,
				factor, SqlToken.RParen)
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

		public static IParser<SqlExpression> OptionName =
			SqlToken.Contains("NOCOUNT", "NOEXEC",
				"ANSI_NULLS", "ANSI_PADDING", "ANSI_WARNINGS", "ARITHABORT", "CONCAT_NULL_YIELDS_NULL",
				"QUOTED_IDENTIFIER",
				"NUMERIC_ROUNDABORT"
			);

		public static IParser<bool> OnOffExpr =
			from onOff1 in SqlToken.Contains("OFF", "ON")
			select string.Equals(onOff1.GetText().ToUpper(), "ON", StringComparison.Ordinal);

		public static IParser<SetOptionExpression> SetOptionOnOffExpr =
			(from set1 in SqlToken.Word("SET")
			 from optionName1 in OptionName
			 from onOff1 in OnOffExpr
			 from semiColon1 in SqlToken.SemiColon.Optional()
			 select new SetOptionExpression()
			 {
				 OptionName = optionName1.GetText(),
				 IsToggle = onOff1
			 }).Named(nameof(SetOptionOnOffExpr));

		public static IParser<SetManyOptionExpression> SetManyOptionOnOffExpr =
			(from set1 in SqlToken.Word("SET")
			 from optionNames1 in OptionName.SeparatedBy(SqlToken.Comma)
			 from onOff1 in OnOffExpr
			 from semiColon1 in SqlToken.SemiColon.Optional()
			 select new SetManyOptionExpression()
			 {
				 Items = optionNames1.Select(x => new SetOptionExpression()
				 {
					 OptionName = x.GetText(),
					 IsToggle = onOff1
				 }).ToArray()
			 }).Named(nameof(SetManyOptionOnOffExpr));

		public static IParser<SqlExpression> GoExpr =
			from go1 in SqlToken.Word("GO")
			select go1;

		public static IParser<WithOptionExpression> WithOptionExpr =
			Parse.Seq(
				SqlToken.Word("with"), SqlToken.LParen,
				SqlToken.Word("nolock"), SqlToken.RParen
			).MapResult(x => new WithOptionExpression()
			{
				Nolock = true
			});

		public static IParser<SqlExpression> VariableAssignFieldExpr(IParser<SqlExpression> fieldExpr)
		{
			var assignExpr =
				from variable1 in Variable
				from assign1 in SqlToken.Assign
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
			Parse.Seq(SqlToken.At,
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
			Parse.Seq(Identifier, SqlToken.Dot, Identifier)
				.MapResultList(x => new FieldExpression()
				{
					Name = x[2].GetText(),
					From = x[0].GetText()
				});

		private static readonly IParser<FieldExpression> TableFieldExpr3 =
			Parse.Seq(Identifier, SqlToken.Dot, Identifier, SqlToken.Dot, Identifier)
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
			return Parse.RecGroupOperatorExpr(SqlToken.LParen, atom, SqlToken.RParen, new[]
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
			ParseToken.Lexeme(SqlToken.Minus, SqlToken.Digits)
				.MapResultList(x => new NumberExpression()
				{
					ValueTypeFullname = typeof(int).FullName,
					Value = int.Parse($"{x[0].GetText()}{x[1].GetText()}")
				});

		public static IParser<NumberExpression> NumberExpr =
			Parse.Any(NegativeIntegerExpr, IntegerExpr);

		public static IParser<SqlExpression> Atom =
			Parse.AnyCast<SqlExpression>(
				SqlToken.NString,
				FuncGetdate,
				TableFieldAliasExpr,
				NumberExpr,
				Variable);

		public static IParser<SqlExpression> ArithmeticOperatorAtomExpr =
			ArithmeticOperatorExpr(Atom);

		public static IParser<FieldsExpression> FieldsExpr =
			RecFieldExpr(ArithmeticOperatorAtomExpr.MapSqlExpr())
				.ManyDelimitedBy(SqlToken.Comma)
				.MapResultList(x => new FieldsExpression()
				{
					Items = x.TakeEvery(1).ToList()
				});

		public static IParser<FilterExpression> FilterExpr(IParser<SqlExpression> atom)
		{
			var oper = SqlToken.Symbols(">=", "<=", "!=", ">", "<", "=");
			var oper2 = Parse.Any(
				SqlToken.Word("LIKE"),
				Parse.Seq(SqlToken.Word("NOT"), SqlToken.Word("LIKE")).Merge()
				);
			return Parse.Seq(
				atom,
				oper.Or(oper2),
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

		public static IParser<SourceExpression> ToTableExpr(this IParser<SelectExpression> subSelect)
		{
			return from subQuery1 in subSelect.Group()
					 from alias1 in AliasExpr.Optional()
					 select new SourceExpression()
					 {
						 Item = subQuery1,
						 AliasName = alias1?.Name
					 };
		}

		public static IParser<SelectExpression> SelectExpr =
			from select1 in SqlToken.Word("SELECT")
			from fields1 in FieldsExpr
			from from1 in SqlToken.Word("FROM")
			from table1 in Parse.AnyCast<SqlExpression>(TableExpr, SelectExpr.ToTableExpr())
			from where1 in WhereExpr.Optional()
			select new SelectExpression()
			{
				Fields = fields1,
				From = table1,
				Where = where1
			};

		private static readonly IParser<ObjectNameExpression> DatabaseDboSchemaName3 =
			Parse.Seq(Identifier, SqlToken.Dot,
				Identifier, SqlToken.Dot,
				Identifier
			).Merge()
			.MapResult(x => new ObjectNameExpression()
			{
				Name = x.GetText()
			});

		private static readonly IParser<ObjectNameExpression> DatabaseDboSchemaName2 =
			Parse.Seq(Identifier, SqlToken.Dot,
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

		private static IParser<UpdateSetFieldExpression> SetFieldEqualExpr(IParser<SqlExpression> factor)
		{
			return from field1 in DatabaseDboSchemaName1
					 from _ in SqlToken.Assign
					 from expr1 in factor
					 select new UpdateSetFieldExpression()
					 {
						 FieldName = field1.Name,
						 AssignExpr = expr1
					 };
		}

		private static IParser<UpdateSetFieldExpression[]> SetFieldEqualExprs(IParser<SqlExpression> factor)
		{
			return SetFieldEqualExpr(factor)
				.CastParser<SqlExpression>()
				.ManyDelimitedBy(SqlToken.Comma)
				.MapResult(x => x.TakeEvery(1).Cast<UpdateSetFieldExpression>().ToArray());
		}

		public static IParser<SqlExpression> UpdateExpr(IParser<SqlExpression> factor)
		{
			var updateExpr =
				from update1 in SqlToken.Word("UPDATE")
				from table1 in DatabaseSchemaObjectName
				from set1 in SqlToken.Word("SET")
				from updateFields1 in SetFieldEqualExprs(factor)
				from where1 in WhereExpr.Optional()
				select new UpdateExpression()
				{
					SetFields = updateFields1,
					WhereExpr = where1
				};
			return Parse.Any(updateExpr.MapSqlExpr(), factor);
		}

		public static IParser<T> Group<T>(this IParser<T> p)
		{
			return from lparen1 in SqlToken.LParen
					 from p1 in p
					 from rparen1 in SqlToken.RParen
					 select p1;
		}

		public static IParser<T> GroupOptional<T>(this IParser<T> p)
		{
			return Parse.Any(p.Group(), p);
		}

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

		public static IParser<SqlExpression> IfExpr(IParser<SqlExpression> factor)
		{
			var body = factor.Many1()
				.MapResult(x => new StatementsExpression()
				{
					Items = x.ToArray()
				});

			var groupFilterExpr = FilterExpr(Atom).Group();

			var conditionExpr = Parse.Any(groupFilterExpr, FilterExpr(Atom));

			var ifExpr =
				from if1 in SqlToken.Word("IF")
				from conditionExpr1 in conditionExpr
				from begin1 in SqlToken.Word("BEGIN")
				from body1 in body
				from end1 in SqlToken.Word("END")
				select new IfExpression()
				{
					Condition = conditionExpr1,
					Body = body1
				};

			return Parse.Any(ifExpr.MapSqlExpr(), factor);
		}

		public static IParser<IfExpression> IfExprs2 =
			(from if1 in SqlToken.Word("IF")
			from conditionExpr1 in FilterExpr(Atom).GroupOptional()
			from begin1 in SqlToken.Word("BEGIN")
			from body1 in StartExpr.Many1()
				.MapResult(x => new StatementsExpression()
				{
					Items = x.ToArray()
				})
			from end1 in SqlToken.Word("END")
			select new IfExpression()
			{
				Condition = conditionExpr1,
				Body = body1
			}).Named(nameof(IfExprs2));

		public static IParser<SqlDataTypeExpression> SqlDataType0Expr =
			Parse.Any(SqlDataType0, SqlDataType1)
				.MapResult(x => new SqlDataTypeExpression()
				{
					DataType = x.GetText(),
				});

		public static IParser<SqlDataTypeExpression> SqlDataType1Expr =
			from dataType1 in Parse.Any(SqlDataType2, SqlDataType1)
			from lparen1 in SqlToken.LParen
			from size1 in IntegerExpr
			from rparen1 in SqlToken.RParen
			select new SqlDataTypeExpression()
			{
				DataType = dataType1.GetText(),
				Size = (int)size1.Value
			};

		public static IParser<SqlDataTypeExpression> SqlDataType2Expr =
			from dataType1 in SqlDataType2
			from lparen1 in SqlToken.LParen
			from size1 in IntegerExpr
			from comma1 in SqlToken.Comma
			from scale1 in IntegerExpr
			from rparen1 in SqlToken.RParen
			select new SqlDataTypeExpression()
			{
				DataType = dataType1.GetText(),
				Size = (int)size1.Value,
				Scale = (int)scale1.Value
			};

		public static IParser<SqlDataTypeExpression> SqlDataTypeExpr =
			Parse.Any(SqlDataType2Expr, SqlDataType1Expr, SqlDataType0Expr);

		public static IParser<SqlParameterExpression> SqlParameterExpr =
			from variableName1 in Variable
			from dataType1 in SqlDataTypeExpr
			select new SqlParameterExpression()
			{
				Name = variableName1,
				DataType = dataType1
			};

		public static IParser<IEnumerable<SqlParameterExpression>> SqlParameterListExpr =
			SqlParameterExpr.SeparatedBy(SqlToken.Comma);

		//CREATE PROCEDURE [dbo].[AccountAPI_AddSportsCashUsed_19.05]
		public static IParser<SqlCreateStoredProcedureExpression> CreateStoredProcedureExpr =
			from create1 in SqlToken.Word("CREATE")
			from proc1 in SqlToken.Word("PROCEDURE")
			from procName1 in Parse.Any(DatabaseDboSchemaName2, DatabaseDboSchemaName1)
			from procParams1 in SqlParameterListExpr
			from as1 in SqlToken.Word("AS")
			from begin1 in SqlToken.Word("BEGIN")
			from body1 in StartExpr.Many()
			from end1 in SqlToken.Word("END")
			select new SqlCreateStoredProcedureExpression()
			{
				Name = procName1,
				Parameters = procParams1.ToArray(),
				Body = body1.ToArray()
			};

		//:setvar DatabaseName "AccountDB"
		public static IParser<SqlSetVarExpression> SetVarExpr =
			from setVar1 in SqlToken.Word(":setVar")
			from name1 in SqlToken.Lexeme(Parse.CStyleIdentifier)
			from value1 in SqlToken.Lexeme(SqlToken.String2)
			select new SqlSetVarExpression
			{
				Name = name1.Text,
				Value = value1.GetText().GetCStyleStringText()
			};

		//:on error exit
		public static IParser<SqlOnErrorExitExpression> OnErrorExitExpr =
			from on1 in SqlToken.Word(":ON")
			from error1 in SqlToken.Word("ERROR")
			from exit1 in SqlToken.Word("EXIT")
			select new SqlOnErrorExitExpression();

		//PRINT N'xxx';
		public static IParser<SqlPrintExpression> PrintExpr =
			from print1 in SqlToken.Word("PRINT")
			from str1 in SqlToken.LexemeString
			from end1 in SqlToken.SemiColon.Optional()
			select new SqlPrintExpression()
			{
				TextSpan = new[] { print1, str1, end1 }.GetTextSpan(),
				Value = str1
			};

		public static IParser<SqlExpression> BatchExpr =
			Parse.AnyCast<SqlExpression>(
				SetVarExpr,
				OnErrorExitExpr,
				DeclareVariableExpr
				);

		public static IParser<SqlExpression> StartExpr =
			Parse.AnyCast<SqlExpression>(
				BatchExpr,
				PrintExpr,
				SetOptionOnOffExpr,
				SetManyOptionOnOffExpr,
				GoExpr,
				SelectExpr.MapSqlExpr().LeftRecursive(
				IfExpr,
				SqlFunctions),
				IfExprs2
			);

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
