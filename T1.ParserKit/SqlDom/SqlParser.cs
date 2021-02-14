using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using T1.ParserKit.Core;
using T1.ParserKit.Helpers;
using T1.ParserKit.SqlDom.Expressions;
using T1.Standard.Common;
using T1.Standard.Extensions;

namespace T1.ParserKit.SqlDom
{
	public class SqlParser
	{
		public IParser LParen => Symbol("(");
		public IParser RParen => Symbol(")");

		public IParser WithOptionExpr
		{
			get
			{
				return Parse.Chain(
					Match("with"),
					LParen,
					Match("nolock"),
					RParen
				).MapResult(x => new WithOptionExpression()
				{
					Nolock = true
				});
			}
		}

		public IParser Variable
		{
			get
			{
				return Parse.Chain(
					Symbol("@"),
					Identifier()
				).Merge();
			}
		}

		public IParser VariableAssignFieldExpr(IParser fieldExpr)
		{
			var assignField = Parse.Chain(
				Variable,
				Symbol("="),
				fieldExpr)
				.MapAssign<VariableAssignFieldExpression>((x, expr) =>
				{
					var f = (FieldExpression)x[2];
					expr.VariableName = x[0].GetText();
					expr.Name = f.Name;
					expr.From = f.From;
					expr.AliasName = f.AliasName;
				});
			return Parse.Any(assignField, fieldExpr);
		}

		public IParser FieldExpr
		{
			get
			{
				var tableField =
					Parse.Chain(Identifier(), Symbol("."), Identifier())
						.MapAssign<FieldExpression>((x, expr) =>
						{
							expr.Name = x[2].GetText();
							expr.From = x[0].GetText();
						});

				var field = Identifier()
					.MapAssign<FieldExpression>((x, expr) =>
					{
						expr.Name = x[0].GetText();
					});

				var field1 = Parse.Any(tableField, field)
					.MapResult(x =>
					{
						return x[0];
					})
					.Named("FieldExpr1");

				var field2 = new[]
				{
					field1,
					Identifier()
				}.Chain().MapResult(x =>
				{
					var expr = (FieldExpression)x[0];
					expr.AliasName = x[1].GetText();
					return expr;
				}).Named("FieldExpr2");

				var field3 = new[]
				{
					field1,
					Match("as"),
					Identifier()
				}.Chain().MapResult(x =>
				{
					var expr = (FieldExpression)x[0];
					expr.AliasName = x[2].GetText();
					return expr;
				}).Named("FieldExpr3");

				return Parse.Any(field3, field2, field1).Named("FieldExpr");
			}
		}

		public IParser RecFieldExpr(IParser factor)
		{
			return VariableAssignFieldExpr(FieldExpr);
		}

		public IParser Comma =>
			Symbol(",");

		public IParser FieldsExpr
		{
			get
			{
				var fieldExpr = RecFieldExpr(FieldExpr);

				var fields = fieldExpr.ManyDelimitedBy(Comma)
					.MapResult(x => new FieldsExpression()
					{
						Items = x.TakeEvery(1).Cast<SqlExpression>().ToList()
					});

				var fields1 = fieldExpr
					.MapResult(x => new FieldsExpression()
					{
						Items = new List<SqlExpression>()
						{
							x[0] as FieldExpression
						}
					});

				return fields.Or(fields1).Named("FieldsExpr");
			}
		}

		public IParser Atom
		{
			get
			{
				return Parse.Any(FieldExpr, NumberExpr);
			}
		}

		public IParser ArithmeticOperatorExpr()
		{
			return Parse.RecGroupOperatorExpr(LParen, Atom, RParen, new[]
			{
				Symbol("*"),
				Symbol("/"),
				Symbol("+"),
				Symbol("-")
			}, x => new ArithmeticOperatorExpression
			{
				Left = (SqlExpression)x[0],
				Oper = x[1].GetText(),
				Right = (SqlExpression)x[2]
			});
		}

		public IParser FilterExpr(IParser atom)
		{
			var oper = ContainsSymbol(">=", "<=", "!=", ">", "<", "=");
			return Parse.Chain(
				atom,
				oper,
				atom
				).MapResult(x => new FilterExpression()
				{
					Left = (SqlExpression)x[0],
					Oper = x[1].GetText(),
					Right = (SqlExpression)x[2],
				});
		}

		public IParser NumberExpr =>
			SkipBlanks(Parse.Digits().Assertion(true)).Named("NumberExpr")
				.MapResult(x => new NumberExpression()
				{
					ValueTypeFullname = typeof(int).FullName,
					Value = int.Parse(x[0].GetText())
				});

		public IParser WhereExpr
		{
			get
			{
				return Parse.Chain(
					Match("WHERE"),
					FilterExpr(Atom)
				)
				.Named("WhereExpr")
				.MapResult(x => new WhereExpression()
				{
					Filter = (FilterExpression)x[1]
				});
			}
		}

		public IParser SelectExpr =>
			Parse.Chain(
				Match("SELECT"),
				FieldsExpr,
				Match("FROM"),
				TableExpr,
				WhereExpr.Optional()
			).MapResult(x => new SelectExpression()
			{
				Fields = x[1] as FieldsExpression,
				From = x[3] as TableExpression,
				Where = x.FirstCast<WhereExpression>()
			}).Named("SelectExpr");

		public IParser Group(IParser p)
		{
			return Parse.Chain(
				Symbol("("),
				p,
				Symbol(")"));
		}

		public IParser RecSelectExpr(IParser factor)
		{
			var subTableExpr =
				Parse.Chain(
					Group(factor),
					Identifier())
					.MapResult(x => new SourceExpression()
					{
						Item = x[1] as SqlExpression,
						AliasName = x[3].GetText()
					}).Named("SubTableExpr");

			var recSelectExpr = Parse.Chain(
				Match("SELECT"),
				FieldsExpr,
				Match("FROM"),
				subTableExpr
			)
			.MapResult(x => new SelectExpression()
			{
				Fields = x[1] as FieldsExpression,
				From = x[3] as SqlExpression
			});

			return recSelectExpr.Or(factor);
		}

		public IParser TableExpr
		{
			get
			{
				var withOption = WithOptionExpr.Many(0, 1);

				var table1 =
					Parse.Chain(Identifier(),
							withOption)
					.MapResult(x => new TableExpression()
					{
						Name = x[0].GetText(),
						WithOption = x.FirstCast<WithOptionExpression>()
					});

				var table2 =
					Parse.Chain(
						Identifier(),
						Identifier()
					).MapResult(x => new TableExpression()
					{
						Name = x[0].GetText(),
						AliasName = x[1].GetText()
					});

				var table3 =
					Parse.Chain(
						Identifier(),
						Match("as"),
						Identifier()
					).MapResult(x => new TableExpression()
					{
						Name = x[0].GetText(),
						AliasName = x[2].GetText()
					});

				return Parse.Any(table3, table2, table1);
			}
		}

		public SqlExpression[] ParseText(string code)
		{
			var startExpr = SelectExpr;

			startExpr = RecSelectExpr(SelectExpr);

			return startExpr.ParseText(code).Cast<SqlExpression>().ToArray();
		}

		public IParser Identifier()
		{
			return SkipBlanks(SqlToken.Identifier.ThenLeft(NotKeyword()))
				.Named("SqlIdentifier");
		}

		protected IParser NotKeyword()
		{
			return SqlTokenizer.KeywordsParser.Not().Named("!SqlKeyword");
		}

		protected bool IsKeyword(string text)
		{
			var parsed = SqlTokenizer.KeywordsParser.TryParseAllText(text);
			return parsed.IsSuccess();
		}

		protected IParser Match(string text)
		{
			if (IsKeyword(text))
			{
				return SkipBlanks(Parse.Equal(text, true)).Named($"{text}");
			}

			return SkipBlanks(
				NotKeyword().ThenRight(Parse.Equal(text, true))
				).Named($"{text}");
		}

		protected IParser Symbol(string text)
		{
			return SkipBlanks(
				Parse.Equal(text)
			);
		}

		protected IParser ContainsSymbol(params string[] symbols)
		{
			return SkipBlanks(
				Parse.Contains(symbols)
			);
		}

		protected IParser SkipBlanks(IParser p)
		{
			return new[] {
				Parse.Blanks().Many().Skip(),
				p
			}.Chain().Named($">>{p.Name}");
		}
	}
}
