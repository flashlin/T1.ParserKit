using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using T1.ParserKit.Core;
using T1.ParserKit.Helpers;
using T1.ParserKit.SqlDom.Expressions;
using T1.Standard.Common;
using T1.Standard.Extensions;

namespace T1.ParserKit.SqlDom
{
	public class SqlParser
	{
		public IParser SqlIdentifier
		{
			get
			{
				var start = Parse.Equal("[");
				var body = Parse.NotEqual("]").Many(1);
				var end = Parse.Equal("]");
				var identifier = Parse.Chain(start, body, end).Merge();
				return identifier.Or(Parse.CStyleIdentifier())
					.Named("SqlIdentifier");
			}
		}


		public IParser LParen => Symbol("(");
		public IParser RParen => Symbol(")");
		public IParser SemiColon => Symbol(";");

		public IParser SqlDataType
		{
			get
			{
				return ContainsText("DATETIME", "BIGINT");
			}
		}

		public IParser SetNocountExpr
		{
			get
			{
				var onOFf = ContainsText("ON", "OFF");
				return Parse.Chain(
					Match("SET"),
					Match("NOCOUNT"),
					onOFf,
					SemiColon.Optional()
					).MapResult(x => new SetOptionExpression()
					{
						OptionName = x[1].GetText(),
						IsToggle = string.Equals(x[2].GetText().ToUpper(), "ON", StringComparison.Ordinal)
					});
			}
		}

		public IParser DeclareVariableExpr
		{
			get
			{
				return Parse.Chain(
					Match("DECLARE"),
					Variable,
					SqlDataType)
					.MapResult(x => new DeclareExpression()
					{
						Name = x[1].GetText(),
						DataType = x[2].GetText()
					});
			}
		}

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
				).Merge()
				.MapResult(x => new VariableExpression()
				{
					Name = x[0].GetText()
				});
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
			return VariableAssignFieldExpr(factor);
		}

		public IParser Comma =>
			Symbol(",");

		public IParser FieldsExpr
		{
			get
			{
				var fieldExpr = RecFieldExpr(ArithmeticOperatorExpr());

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
							x[0] as SqlExpression
						}
					});

				return fields.Or(fields1).Named("FieldsExpr");
			}
		}

		public IParser Atom
		{
			get
			{
				return Parse.Any(FieldExpr, NumberExpr, Variable);
			}
		}

		public IParser ArithmeticOperatorExpr(IParser atom)
		{
			return Parse.RecGroupOperatorExpr(LParen, atom, RParen, new[]
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

		public IParser ArithmeticOperatorExpr()
		{
			return ArithmeticOperatorExpr(Atom);
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

		public IParser IfExpr(IParser factor)
		{
			var body = factor.AtLeastOnce()
				.MapResult(x => new StatementsExpression()
				{
					Items = x.Cast<SqlExpression>().ToArray()
				});

			var ifExpr = Parse.Chain(
				Match("IF"),
				FilterExpr(Atom),
				Match("BEGIN"),
				body,
				Match("END"))
				.MapResult(x => new IfExpression()
				{
					Condition = (FilterExpression)x[1],
					Body = (StatementsExpression)x[3]
				});

			return Parse.Any(ifExpr, factor);
		}

		public IParser RecStartExpr(IParser factor)
		{
			return IfExpr(StartExpr).Or(factor);
		}

		public IParser StartExpr
		{
			get
			{
				var startExpr = Parse.Any(RecSelectExpr(SelectExpr),
					DeclareVariableExpr,
					SetNocountExpr);
				return IfExpr(startExpr);
			}
		}

		public SqlExpression[] ParseText(string code)
		{
			return RecStartExpr(StartExpr).ParseText(code).Cast<SqlExpression>().ToArray();
		}

		public IParser Identifier()
		{
			return SkipBlanks(SqlIdentifier.Next(NotKeyword));
		}

		private static string NotKeyword(ITextSpan[] r)
		{
			return SqlTokenizer.Keywords.Contains(r[0].GetText().ToUpper()) ? "keyword" : "";
		}

		protected IParser Match(string text)
		{
			return SkipBlanks(
				Parse.Equal(text, true).Assertion(true)
			).Named($"{text}");
		}

		protected IParser Symbol(string text)
		{
			return SkipBlanks(
				Parse.Equal(text)
			);
		}

		protected IParser ContainsText(params string[] texts)
		{
			return SkipBlanks(
				Parse.Contains(texts, true).Assertion(true)
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
