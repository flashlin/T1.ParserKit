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
// ReSharper disable InconsistentNaming

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

		public static readonly IParser<SqlVariableExpression> Variable =
			from _ in SqlToken.Blanks.Optional()
			from at1 in SqlToken.At
			from identifier in SqlToken.SqlIdentifier
			select new SqlVariableExpression()
			{
				TextSpan = new[] { at1, identifier }.GetTextSpan(),
				Name = at1.GetText() + identifier.GetText()
			};

		public static IParser<SqlExpression> Merge(this IParser<IEnumerable<SqlExpression>> parsers)
		{
			return parsers.Select(x => new SqlExpression()
			{
				TextSpan = x.Select(item => item.TextSpan).GetTextSpan()
			});
		}

		public static readonly IParser<SqlFunctionExpression> FuncGetdateExpr =
			(from getdate in SqlToken.Word("GETDATE")
			 from lparen in SqlToken.LParen
			 from rparen in SqlToken.RParen
			 select new SqlFunctionExpression
			 {
				 TextSpan = new[] { getdate, lparen, rparen }.GetTextSpan(),
				 Name = "GETDATE",
				 Parameters = new SqlExpression[0]
			 }
			).Named(nameof(FuncGetdateExpr));

		//DATEADD(DD,-1,DATEDIFF(dd, 0, GETDATE()))
		public static IParser<SqlFunctionExpression> FuncDateaddExpr =
			(from dateadd in SqlToken.Word("DATEADD")
			 from lparen in SqlToken.LParen
			 from tDatepart in SqlToken.Contains(SqlToken.DateaddDetepart)
			 from comma1 in SqlToken.Comma
			 from tFactor1 in FunctionFactor
			 from comma2 in SqlToken.Comma
			 from tFactor2 in FunctionFactor
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
			 }).Named(nameof(FuncDatediffExpr));

		//DATEDIFF(dd, 0, GETDATE())
		public static readonly IParser<SqlFunctionExpression> FuncDatediffExpr =
			(from datediff1 in SqlToken.Word("DATEDIFF")
			 from lparen in SqlToken.LParen
			 from datepart1 in SqlToken.Contains(SqlToken.DatediffDatepart)
			 from comma1 in SqlToken.Comma
			 from numberExpr1 in NumberExpr
			 from comma2 in SqlToken.Comma
			 from factor1 in FunctionFactor
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
			 }).Named(nameof(FuncDatediffExpr));


		//ISNULL(@SblimitExpiredDate, xxx)
		public static readonly IParser<SqlFunctionExpression> FuncIsnullExpr =
			(from isNull1 in SqlToken.Word("ISNULL")
			 from lparen in SqlToken.LParen
			 from checkExpr1 in FunctionFactor
			 from comma1 in SqlToken.Comma
			 from replacementValue in FunctionFactor
			 from rparen1 in SqlToken.RParen
			 select new SqlFunctionExpression()
			 {
				 Name = "ISNULL",
				 Parameters = new[]
				 {
						checkExpr1,
						replacementValue
					}
			 }).Named(nameof(FuncIsnullExpr));


		public static readonly IParser<SqlFuncExistsExpression> FuncExistsExpr =
			(from exists in SqlToken.Word("EXISTS")
			 from start in SqlToken.LParen
			 from subquery in FunctionFactor
			 from end in SqlToken.RParen
			 select new SqlFuncExistsExpression()
			 {
				 TextSpan = new[] { start, subquery, end }.GetTextSpan(),
				 Name = "EXISTS",
				 Parameters = new[] { subquery },
			 }).Named(nameof(FuncExistsExpr));

		public static readonly IParser<SqlFuncSuserSnameExpression> FuncSuserSnameExpr =
			(from suser_sname in SqlToken.Word("SUSER_SNAME")
			 from lparen in SqlToken.LParen
			 from server_user_sid in SqlToken.SqlIdentifier
			 from rparen in SqlToken.RParen
			 select new SqlFuncSuserSnameExpression()
			 {
				 TextSpan = new[] { suser_sname, lparen, server_user_sid, rparen }.GetTextSpan(),
				 Name = "SUSER_SNAME",
				 Parameters = new SqlExpression[]
				 {
					 server_user_sid
				 }
			 }).Named(nameof(FuncSuserSnameExpr));

		public static readonly IParser<SqlFuncDbNameExpression> FuncDbNameExpr =
			(from db_name in SqlToken.Word("DB_NAME")
			 from lparen in SqlToken.LParen
			 from rparen in SqlToken.RParen
			 select new SqlFuncDbNameExpression()
			 {
				 TextSpan = new[] { db_name, lparen, rparen }.GetTextSpan(),
				 Name = "DB_NAME"
			 }).Named(nameof(FuncDbNameExpr));

		public static readonly IParser<SqlFunctionExpression> SqlFunctionsExpr =
			Parse.AnyCast<SqlFunctionExpression>(
				FuncGetdateExpr,
				FuncIsnullExpr,
				FuncDateaddExpr,
				FuncDatediffExpr,
				FuncSuserSnameExpr,
				FuncDbNameExpr,
				FuncExistsExpr);

		public static readonly IParser<SqlExpression> OptionName =
			SqlToken.Contains("NOCOUNT", "NOEXEC",
				"ANSI_NULLS", "ANSI_PADDING", "ANSI_WARNINGS", "ARITHABORT", "CONCAT_NULL_YIELDS_NULL",
				"QUOTED_IDENTIFIER",
				"NUMERIC_ROUNDABORT"
			);

		public static readonly IParser<bool> OnOffExpr =
			from onOff1 in SqlToken.Contains("OFF", "ON")
			select string.Equals(onOff1.GetText().ToUpper(), "ON", StringComparison.Ordinal);

		public static readonly IParser<SetOptionExpression> SetOptionOnOffExpr =
			(from set1 in SqlToken.Word("SET")
			 from optionName1 in OptionName
			 from onOff1 in OnOffExpr
			 from semiColon1 in SqlToken.SemiColon.Optional()
			 select new SetOptionExpression()
			 {
				 OptionName = optionName1.GetText(),
				 IsToggle = onOff1
			 }).Named(nameof(SetOptionOnOffExpr));

		public static readonly IParser<SetManyOptionExpression> SetManyOptionOnOffExpr =
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

		public static readonly IParser<SqlExpression> GoExpr =
			(from go1 in SqlToken.Word("GO")
			 select go1
			).Named(nameof(GoExpr));

		public static readonly IParser<SqlWithOptionExpression> WithOptionExpr =
			Parse.Seq(
				SqlToken.Word("with"), SqlToken.LParen,
				SqlToken.Word("nolock"), SqlToken.RParen
			).MapResult(x => new SqlWithOptionExpression()
			{
				Nolock = true
			});

		public static readonly IParser<SqlVariableAssignFieldExpression> VariableAssignFieldExpr1 =
				from variable1 in Variable
				from assign1 in SqlToken.Assign
				from expr1 in TableFieldExpr
				select new SqlVariableAssignFieldExpression()
				{
					VariableName = variable1,
					From = expr1,
				};

		public static IParser<SqlVariableAssignExpression> VariableAssignExpr(IParser<SqlExpression> factor)
		{
			return from variable1 in Variable
					 from assign1 in SqlToken.Assign
					 from expr1 in factor
					 select new SqlVariableAssignExpression()
					 {
						 VariableName = variable1,
						 AssignFrom = expr1,
					 };
		}

		public static readonly IParser<SqlBatchVariableExpression> BatchVariableExpr =
			from dollarSign in SqlToken.DollarSign
			from lparen in SqlToken.LParen
			from name in SqlToken.Lexeme(Parse.CStyleIdentifier).ToExpr()
			from rparen in SqlToken.RParen
			select new SqlBatchVariableExpression()
			{
				TextSpan = new[] { dollarSign, lparen, name, rparen }.GetTextSpan(),
				Name = name.GetText()
			};

		public static readonly IParser<SqlUseDatabaseExpression> UseDatabaseExpr =
			(from use in SqlToken.Word("USE")
			 from dbname in SqlToken.Identifier
			 from end in SqlToken.SemiColon.Optional()
			 select new SqlUseDatabaseExpression()
			 {
				 DatabaseName = dbname.GetText()
			 }
			).Named(nameof(UseDatabaseExpr));

		public static readonly IParser<SqlDeclareExpression> DeclareVariableExpr =
			(from declare1 in SqlToken.Word("DECLARE")
			 from variable1 in Variable
			 from sqlDataType1 in SqlToken.SqlDataType
			 select new SqlDeclareExpression()
			 {
				 Name = variable1,
				 DataType = sqlDataType1.GetText()
			 }
			).Named(nameof(DeclareVariableExpr));

		private static readonly IParser<SqlTableFieldExpression> TableFieldExpr1 = SqlToken.Identifier.MapResult(x => new SqlTableFieldExpression()
		{
			Name = x.GetText()
		});

		private static readonly IParser<SqlTableFieldExpression> TableFieldExpr2 =
			Parse.SeqCast<SqlExpression>(SqlToken.Identifier, SqlToken.Dot, SqlToken.Identifier)
				.MapResultList(x => new SqlTableFieldExpression()
				{
					Name = x[2].GetText(),
					From = x[0].GetText()
				});

		private static readonly IParser<SqlTableFieldExpression> TableFieldExpr3 =
			Parse.SeqCast<SqlExpression>(SqlToken.Identifier, SqlToken.Dot, SqlToken.Identifier, SqlToken.Dot, SqlToken.Identifier)
				.MapResultList(x => new SqlTableFieldExpression()
				{
					Name = x[4].GetText(),
					From = $"{x[0].GetText()}.{x[2].GetText()}"
				});

		public static readonly IParser<SqlBaseFieldExpression> TableFieldExpr =
			Parse.AnyCast<SqlBaseFieldExpression>(VariableAssignFieldExpr1,
					TableFieldExpr3,
					TableFieldExpr2,
					TableFieldExpr1)
				.Named(nameof(TableFieldExpr));


		public static readonly IParser<SqlBaseFieldExpression> TableFieldAliasExpr =
			from tableField1 in TableFieldExpr
			from alias1 in AliasExpr.Optional()
			select tableField1.Assign(x => { x.AliasName = alias1?.Name; });


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

		public static readonly IParser<SqlNumberExpression> IntegerExpr =
			ParseToken.Lexeme(Parse.Digits)
				.MapResult(x => new SqlNumberExpression()
				{
					TextSpan = x,
					ValueTypeFullname = typeof(int).FullName,
					Value = int.Parse(x.Text)
				});

		public static readonly IParser<SqlNumberExpression> NegativeIntegerExpr =
			ParseToken.Lexeme(SqlToken.Minus, SqlToken.Digits)
				.MapResultList(x => new SqlNumberExpression()
				{
					ValueTypeFullname = typeof(int).FullName,
					Value = int.Parse($"{x[0].GetText()}{x[1].GetText()}")
				});

		public static readonly IParser<SqlNumberExpression> NumberExpr =
			Parse.Any(NegativeIntegerExpr, IntegerExpr);

		public static readonly IParser<SqlExpression> Atom =
			Parse.AnyCast<SqlExpression>(
				SqlToken.NString,
				SqlToken.LexemeString,
				FuncGetdateExpr,
				TableFieldAliasExpr,
				NumberExpr,
				Variable);

		public static readonly IParser<SqlExpression> ArithmeticOperatorAtomExpr =
			ArithmeticOperatorExpr(Atom);

		public static readonly IParser<SqlExpression[]> FieldsExpr =
			from fields in ArithmeticOperatorAtomExpr.SeparatedBy(SqlToken.Comma)
			select fields.ToArray();

		//RecFieldExpr(ArithmeticOperatorAtomExpr.MapSqlExpr())
		//	.ManyDelimitedBy(SqlToken.Comma)
		//	.MapResultList(x => new SqlFieldsExpression()
		//	{
		//		Items = x.TakeEvery(1).ToList()
		//	});

		public static readonly IParser<SqlFilterExpression> FilterExpr1 =
			from not1 in SqlToken.Word("NOT")
			from subquery in StartExpr.GroupOptional()
			select new SqlFilterExpression()
			{
				TextSpan = new[] { not1, subquery }.GetTextSpan(),
				Oper = "NOT",
				Right = subquery
			};


		public static readonly IParser<SqlFilterExpression> FilterExpr2 =
			from _ in SqlToken.Blanks.Optional()
			from left in StartExpr.Or(Atom)
			from oper in Oper1.Or(Oper2)
			from right in StartExpr.Or(Atom)
			select new SqlFilterExpression()
			{
				TextSpan = new[] { left, oper, right }.GetTextSpan(),
				Left = left,
				Oper = oper.GetText(),
				Right = right
			};

		public static readonly IParser<SqlFilterExpression> FilterExpr =
			(from filterExpr in FilterExpr1.Or(FilterExpr2)
			 select filterExpr).Named(nameof(FilterExpr));


		public static readonly IParser<SqlExpression> FilterAndExpr =
			AndExpr(FilterExpr);

		public static readonly IParser<SqlExpression> FilterOrExpr =
			OrExpr(FilterAndExpr);

		public static readonly IParser<SqlExpression> FilterChainExpr =
			FilterOrExpr;

		public static IParser<SqlExpression> AndExpr<T>(IParser<T> factor)
			where T : SqlExpression
		{
			var andExpr =
				from left in factor
				from oper1 in SqlToken.Word("AND")
				from right in factor
				select new SqlAndOperExpression()
				{
					Left = left,
					Oper = "AND",
					Right = right
				};
			return andExpr.CastParser<SqlExpression>().Or(factor.CastParser<SqlExpression>());
		}

		public static IParser<SqlExpression> OrExpr<T>(IParser<T> factor)
			where T : SqlExpression
		{
			var orExpr =
				from left in factor
				from oper1 in SqlToken.Word("OR")
				from right in factor
				select new SqlAndOperExpression()
				{
					Left = left,
					Oper = "OR",
					Right = right
				};
			return orExpr.CastParser<SqlExpression>().Or(factor.CastParser<SqlExpression>());
		}

		public static readonly IParser<SqlWhereExpression> WhereExpr =
			from _ in SqlToken.Word("WHERE")
			from filter1 in FilterChainExpr
			select new SqlWhereExpression()
			{
				Filter = filter1
			};

		public static IParser<SourceExpression> ToTableExpr(this IParser<SqlSelectExpression> subSelect)
		{
			return from subQuery1 in subSelect.Group()
					 from alias1 in AliasExpr.Optional()
					 select new SourceExpression()
					 {
						 Item = subQuery1,
						 AliasName = alias1?.Name
					 };
		}

		public static readonly IParser<SqlSimpleExpression> SimpleSelectExpr =
			from select1 in SqlToken.Word("SELECT")
			from atom in Atom
			select new SqlSimpleExpression()
			{
				Value = atom
			};

		public static readonly IParser<SqlSelectExpression> ComplexSelectExpr =
			from select1 in SqlToken.Word("SELECT")
			from fields1 in FieldsExpr
			from from1 in SqlToken.Word("FROM")
			from table1 in Parse.AnyCast<SqlExpression>(TableExpr, ComplexSelectExpr.ToTableExpr())
			from where1 in WhereExpr.Optional()
			select new SqlSelectExpression()
			{
				Fields = fields1,
				From = table1,
				Where = where1
			};

		public static readonly IParser<SqlExpression> SelectExpr =
			Parse.AnyCast<SqlExpression>(ComplexSelectExpr, SimpleSelectExpr);

		private static readonly IParser<ObjectNameExpression> DatabaseDboSchemaName3 =
			Parse.SeqCast<SqlExpression>(SqlToken.Identifier, SqlToken.Dot, SqlToken.Identifier, SqlToken.Dot, SqlToken.Identifier
				).Merge()
				.MapResult(x => new ObjectNameExpression()
				{
					Name = x.GetText()
				});

		private static readonly IParser<ObjectNameExpression> DatabaseDboSchemaName2 =
			Parse.SeqCast<SqlExpression>(SqlToken.Identifier, SqlToken.Dot, SqlToken.Identifier
				).Merge()
				.MapResult(x => new ObjectNameExpression()
				{
					Name = x.GetText()
				});

		private static readonly IParser<ObjectNameExpression> DatabaseDboSchemaName1 = SqlToken.Identifier
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
			return (from lparen1 in SqlToken.LParen
					  from p1 in p
					  from rparen1 in SqlToken.RParen
					  select p1).Named($"\\( {p.Name} \\)");
		}

		public static IParser<T> GroupOptional<T>(this IParser<T> p)
		{
			return Parse.Any(p.Group(), p);
		}

		private static readonly IParser<AliasExpression> AliasExpr =
			from as1 in SqlToken.Word("AS").Optional()
			from identifier in SqlToken.Identifier
			select new AliasExpression()
			{
				Name = identifier.GetText()
			};

		public static readonly IParser<SqlTableExpression> TableExpr =
			(from databaseTable1 in DatabaseSchemaObjectName
			 from withOption1 in WithOptionExpr.Optional()
			 from alias1 in AliasExpr.Optional()
			 select new SqlTableExpression()
			 {
				 Name = databaseTable1.Name,
				 AliasName = alias1?.Name,
				 WithOption = withOption1
			 }).Named(nameof(TableExpr));

		public static readonly IParser<SqlIfExpression> IfExprs =
			(from if1 in SqlToken.Word("IF")
			 from conditionExpr1 in FilterExpr.GroupOptional()
			 from begin1 in SqlToken.Word("BEGIN")
			 from body1 in StartExpr.Many1()
				 .MapResult(x => new StatementsExpression()
				 {
					 Items = x.ToArray()
				 })
			 from end1 in SqlToken.Word("END")
			 select new SqlIfExpression()
			 {
				 Condition = conditionExpr1,
				 Body = body1
			 }).Named(nameof(IfExprs));

		public static readonly IParser<SqlDataTypeExpression> SqlDataType0Expr =
			Parse.Any(SqlToken.SqlDataType0, SqlToken.SqlDataType1)
				.MapResult(x => new SqlDataTypeExpression()
				{
					DataType = x.GetText(),
				});

		public static readonly IParser<SqlDataTypeExpression> SqlDataType1Expr =
			from dataType1 in Parse.Any(SqlToken.SqlDataType2, SqlToken.SqlDataType1)
			from lparen1 in SqlToken.LParen
			from size1 in IntegerExpr
			from rparen1 in SqlToken.RParen
			select new SqlDataTypeExpression()
			{
				DataType = dataType1.GetText(),
				Size = (int)size1.Value
			};

		public static readonly IParser<SqlDataTypeExpression> SqlDataType2Expr =
			from dataType1 in SqlToken.SqlDataType2
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

		public static readonly IParser<SqlDataTypeExpression> SqlDataTypeExpr =
			Parse.Any(SqlDataType2Expr, SqlDataType1Expr, SqlDataType0Expr);

		public static readonly IParser<SqlParameterExpression> SqlParameterExpr =
			from variableName1 in Variable
			from dataType1 in SqlDataTypeExpr
			select new SqlParameterExpression()
			{
				Name = variableName1,
				DataType = dataType1
			};

		public static readonly IParser<IEnumerable<SqlParameterExpression>> SqlParameterListExpr =
			SqlParameterExpr.SeparatedBy(SqlToken.Comma);

		//CREATE PROCEDURE [dbo].[AccountAPI_AddSportsCashUsed_19.05]
		public static readonly IParser<SqlCreateStoredProcedureExpression> CreateStoredProcedureExpr =
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
		public static readonly IParser<SqlSetVarExpression> SetVarExpr =
			(from setVar1 in SqlToken.Word(":setVar")
			 from name1 in SqlToken.Lexeme(Parse.CStyleIdentifier)
			 from value1 in SqlToken.Lexeme(SqlToken.String2)
			 select new SqlSetVarExpression
			 {
				 Name = name1.Text,
				 Value = value1.GetText().GetCStyleStringText()
			 }
			).Named(nameof(SetVarExpr));

		//:on error exit
		public static readonly IParser<SqlOnErrorExitExpression> OnErrorExitExpr =
			(from on1 in SqlToken.Word(":ON")
			 from error1 in SqlToken.Word("ERROR")
			 from exit1 in SqlToken.Word("EXIT")
			 select new SqlOnErrorExitExpression()
			).Named(nameof(OnErrorExitExpr));

		//PRINT N'xxx';
		public static readonly IParser<SqlPrintExpression> PrintExpr =
			(from print1 in SqlToken.Word("PRINT")
			 from str1 in SqlToken.LexemeString
			 from end1 in SqlToken.SemiColon.Optional()
			 select new SqlPrintExpression()
			 {
				 TextSpan = new[] { print1, str1, end1 }.GetTextSpan(),
				 Value = str1
			 }
			).Named(nameof(PrintExpr));

		public static readonly IParser<SqlExecExpression> ExecExpr =
			from exec1 in SqlToken.Word("EXEC")
			from name1 in DatabaseSchemaObjectName
			from parameters1 in Parse.AnyCast<SqlExpression>(
				 VariableAssignExpr(Atom), 
				 Atom)
				.SeparatedBy(SqlToken.Comma)
			from semicolon in SqlToken.SemiColon.Optional()
			select new SqlExecExpression()
			{
				TextSpan = new[] { exec1, name1 }.Concat(parameters1).GetTextSpan(),
				Name = name1,
				Parameters = parameters1.ToArray()
			};

		public static readonly IParser<SqlExpression> BatchExpr =
			Parse.AnyCast<SqlExpression>(
				SetVarExpr,
				OnErrorExitExpr,
				DeclareVariableExpr,
				UseDatabaseExpr,
				ExecExpr
			);

		public static readonly IParser<SqlExpression> StartExpr =
			Parse.AnyCast<SqlExpression>(
				BatchExpr,
				PrintExpr,
				SetOptionOnOffExpr,
				SetManyOptionOnOffExpr,
				GoExpr,
				SelectExpr,
				IfExprs,
				SqlFunctionsExpr
			).Named(nameof(StartExpr));

		private static readonly IParser<SqlExpression> Oper1 =
			SqlToken.Symbols(">=", "<=", "!=", ">", "<", "=")
				.Named("cp-oper");

		private static readonly IParser<SqlExpression> Oper2 = Parse.Any(
			SqlToken.Word("LIKE"),
			Parse.Seq(SqlToken.Word("NOT"), SqlToken.Word("LIKE")).Merge()
		);

		private static readonly IParser<SqlExpression> FunctionFactor = StartExpr.Or(Atom);

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