﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using T1.ParserKit.Core.Parsers;
using T1.ParserKit.Helpers;
using T1.ParserKit.SqlDom;
using T1.ParserKit.SqlDom.Expressions;
using T1.Standard.Common;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core
{
	public static class Parse
	{
		public static IParseResult<T> Success<T>()
		{
			return new ParseResult<T>()
			{
				Result = default,
				Error = ParseError.Empty,
			};
		}

		public static IParseResult<T> Success<T>(T result)
		{
			return new ParseResult<T>()
			{
				Result = result,
				Error = ParseError.Empty,
			};
		}

		public static IParseResult<T> Success<T>(object result)
		{
			return new ParseResult<T>()
			{
				Result = (T)result,
				Error = ParseError.Empty,
			};
		}

		public static IParseResult<T> Error<T>(string message, IInputReader inp)
		{
			return new ParseResult<T>()
			{
				Result = default,
				Error = new ParseError()
				{
					Message = message,
					TextOffset = inp.GetTextOffset(),
					InnerErrors = new ParseError[0]
				}
			};
		}

		public static IParseResult<T> Error<T>(ParseError error)
		{
			return new ParseResult<T>()
			{
				Result = default,
				Error = error
			};
		}

		public static IParseResult<T> Error<T>(string message,
			IEnumerable<ParseError> innerErrors, IInputReader inp)
		{
			return new ParseResult<T>()
			{
				Result = default,
				Error = new ParseError()
				{
					Message = message,
					TextOffset = inp.GetTextOffset(),
					InnerErrors = innerErrors.CastArray()
				}
			};
		}

		public static IParser<T> AnyCast<T>(params object[] parsers)
		{
			return parsers.AnyCastParser<T>();
		}

		public static IParser<IEnumerable<T>> SeqCast<T>(params object[] parsers)
		{
			return parsers.MapParser<T>(x => Parse.Seq(x));
		}

		public static IParser<IEnumerable<T>> SeqCast<T>(
			Func<IParser<TextSpan>, IParser<T>> map,
			params object[] parsers)
		{
			return parsers.MapParser<T>(map, x => Parse.Seq(x));
		}

		public static TResult Reduce<TResult>(this IEnumerable<TextSpan> rc, Func<ITextSpan, TResult> fn)
		{
			return rc.Cast<ITextSpan>().Reduce(fn);
		}

		public static IParser<TResult> MapResultList<T, TResult>(this IParser<IEnumerable<T>> p,
			Func<T[], TResult> map)
		{
			return new Parser<TResult>(p.Name, inp =>
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return Parse.Error<TResult>(parsed.Error);
				}

				var resultArr = parsed.Result.CastArray();
				var result = map(resultArr);
				return Parse.Success<TResult>(result);
			});
		}

		public static IParser<TResult> MapResult<T, TResult>(this IParser<T> p,
			Func<T, TResult> map)
		{
			return new Parser<TResult>(p.Name, inp =>
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return Parse.Error<TResult>(parsed.Error);
				}

				var result = map(parsed.Result);
				return Parse.Success<TResult>(result);
			});
		}

		public static TResult Reduce<TResult>(this IEnumerable<ITextSpan> rc, Func<ITextSpan, TResult> fn)
		{
			var rcArr = rc.CastArray();
			var textSpan = TextSpan.From(rcArr);
			return fn(textSpan);
		}

		public static IParser<Unit> Not<T>(this IParser<T> parser)
		{
			return new NotParser<T>(parser);
		}

		private static string GetManyText(int min, int max)
		{
			var text = $"({min},{max})";

			if (max == int.MaxValue)
			{
				text = $"({min})";
			}

			if (min == 0 && max == 1)
			{
				text = $"?";
			}

			if (min == 0 && max == int.MaxValue)
			{
				text = $"*";
			}

			if (min == 1 && max == int.MaxValue)
			{
				text = $"+";
			}

			return text;
		}

		public static IParser<TextSpan> AnyChars(int count)
		{
			var name = GetManyText(count, count);
			return new Parser<TextSpan>($".{name}", inp =>
			{
				var ch = inp.Substr(count);
				if (ch.Length == count)
				{
					var textSpan = inp.Consume(count);
					return Parse.Success<TextSpan>(textSpan);
				}

				return Parse.Error<TextSpan>($"Expect anyChar {count}, but got '{ch}'", inp);
			});
		}

		public static IParser<TextSpan> Equal(string text)
		{
			return new Parser<TextSpan>($"\"{text}\"", (inp) =>
			{
				var ch = inp.Substr(text.Length);
				if (text == ch)
				{
					var textSpan = inp.Consume(text.Length);
					return Parse.Success<TextSpan>(textSpan);
				}

				return Parse.Error<TextSpan>($"Expect {text}, but got '{ch}'", inp);
			});
		}

		public static IParser<TextSpan> NotEqual(string text)
		{
			return new Parser<TextSpan>($"^\"{text}\"", (inp) =>
			{
				var ch = inp.Substr(text.Length);
				if (text != ch)
				{
					var textSpan = inp.Consume(text.Length);
					return Parse.Success<TextSpan>(textSpan);
				}

				return Parse.Error<TextSpan>($"Expect {text}, but got '{ch}'", inp);
			});
		}

		public static IParser<TextSpan> Match(string text)
		{
			return new Parser<TextSpan>($"'{text}'", (inp) =>
			{
				var ch = inp.Substr(text.Length);
				var ch1 = ch.ToLower();
				var text1 = text.ToLower();
				if (text1 == ch1)
				{
					var textSpan = inp.Consume(text.Length);
					return Parse.Success<TextSpan>(textSpan);
				}

				return Parse.Error<TextSpan>($"Expect {text}, but got '{ch}'", inp);
			});
		}

		public static IParser<TextSpan> NotMatch(string text)
		{
			return new Parser<TextSpan>($"^'{text}'", (inp) =>
			{
				var ch = inp.Substr(text.Length);
				var ch1 = ch.ToLower();
				var text1 = text.ToLower();
				if (text1 != ch1)
				{
					var textSpan = inp.Consume(text.Length);
					return Parse.Success<TextSpan>(textSpan);
				}

				return Parse.Error<TextSpan>($"Expect {text}, but got '{ch}'", inp);
			});
		}

		//public static IParser<T> Chain<T>(this IEnumerable<IParser<T>> parsers,
		//	Func<IEnumerable<IParseResult<T>>, IParseResult<T>> apply)
		//{
		//	var parsersArr = parsers.CastArray();
		//	var name = string.Join(" ", parsersArr.Select(x => x.Name));

		//	return new Parser<T>(name, inp =>
		//	{
		//		var curr = inp;
		//		var acc = new List<IParseResult<T>>();
		//		IParseResult<T> parsed = null;
		//		foreach (var parser in parsersArr)
		//		{
		//			parsed = parser.TryParse(curr);
		//			if (!parsed.IsSuccess())
		//			{
		//				var ch = curr.Substr(20);
		//				return Parse.Error<T>($"Expect {name}, but got '{ch}' at {curr}.",
		//					parsed.Error, parsed.Rest);
		//			}

		//			acc.Add(parsed);
		//			curr = parsed.Rest;
		//		}

		//		return apply(acc);
		//	});
		//}

		public static IParser<T> Any<T>(params IParser<T>[] parsers)
		{
			return parsers.Any();
		}

		public static IParser<T> Chain<T>(params IParser<T>[] parsers)
		{
			return parsers.Chain();
		}

		public static IParser<IEnumerable<T>> Seq<T>(IEnumerable<IParser<T>> parsers)
		{
			return new SequenceParser<T>(parsers);
		}

		public static IParser<IEnumerable<T>> Seq<T>(params IParser<T>[] parsers)
		{
			return new SequenceParser<T>(parsers);
		}

		public static IParser<T> Optional<T>(this IParser<T> p)
		{
			return new OptionalParser<T>(p);
		}

		public static TryParser<T> Try<T>(this IParser<T> p)
		{
			return new TryParser<T>(p);
		}

		public static IParser<T> TransferToNext<T>(this IParser<T> p, Func<T, string> mapParseResult)
		{
			return new Parser<T>($"{p.Name}->", inp =>
			{
				var pos = inp.GetPosition();
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}

				var errorMessage = mapParseResult(parsed.Result);
				if (!string.IsNullOrEmpty(errorMessage))
				{
					inp.Seek(pos);
					return Parse.Error<T>($"{errorMessage}", inp);
				}

				return parsed;
			});
		}

		//public static IParser<T2> Many<T1, T2>(this IParser<T1> p,
		//	Func<T1[], T2> apply,
		//	int min = 0, int max = int.MaxValue)
		//{
		//	return new Parser<T2>(name, (inp) =>
		//	{
		//		var acc = new List<T1>();
		//		var count = 0;
		//		var ch = inp.Substr(20);
		//		while (!inp.Eof())
		//		{
		//			var parsed = p.TryParse(inp);
		//			if (!parsed.IsSuccess())
		//			{
		//				if (min <= count && count <= max)
		//				{
		//					return MapAccumSuccess(acc, apply);
		//				}

		//				return Parse.Error<T2>($"Expect {name}, but got {ch} at {inp}", inp.GetPosition());
		//			}

		//			acc.Add(parsed.Result);
		//			count++;
		//			if (count == max && count >= min)
		//			{
		//				return MapAccumSuccess(acc, apply);
		//			}
		//		}

		//		if (min <= count && count <= max)
		//		{
		//			return MapAccumSuccess(acc, apply);
		//		}

		//		return Parse.Error<T2>($"Expect {name}, but got EOF at {inp}", inp.GetPosition());
		//	});
		//}

		//public static IParser<TextSpan> Many(this IParser<TextSpan> p, int min = 0, int max = int.MaxValue)
		//{
		//	return p.Many(accum => accum.GetTextSpan(), min, max);
		//}

		//public static IParser<IEnumerable<T>> ManyDelimitedBy<T>(this IParser<T> parser, IParser<T> delimited)
		//{
		//	var tail = delimited.Then(parser, (a, b) => { return new T[] { a, b }; });
		//	var expr2 = parser.Then(tail.Many(), (a, b) =>
		//	{
		//		if (b == null)
		//		{
		//			return Enumerable.Repeat(a, 1);
		//		}

		//		var list = Enumerable.Repeat(a, 1).Concat(b.SelectMany(x => x));
		//		return list;
		//	});
		//	var expr1 = parser.MapResult(x => Enumerable.Repeat(x, 1));
		//	return Parse.Any(expr2, expr1);
		//}

		public static IParser<IEnumerable<T>> ManyDelimitedBy<T>(this IParser<T> parser, IParser<T> delimited)
		{
			var name = $"{parser.Name} {delimited.Name}...";
			return new Parser<IEnumerable<T>>(name, inp =>
			{
				var acc = new List<T>();
				var lastPos = inp.GetPosition();
				do
				{
					var pos1 = inp.GetPosition();
					var parsed1 = parser.TryParse(inp);
					if (!parsed1.IsSuccess())
					{
						inp.Seek(pos1);
						break;
					}
					acc.Add(parsed1.Result);

					var pos2 = inp.GetPosition();
					lastPos = pos2;
					var parsed2 = delimited.TryParse(inp);
					if (!parsed2.IsSuccess())
					{
						inp.Seek(pos2);
						break;
					}
					acc.Add(parsed2.Result);
				} while (!inp.Eof());

				if (acc.Count == 0)
				{
					var ch = inp.Substr(20);
					return Parse.Error<IEnumerable<T>>($"Expect {name}, but got '{ch}'", inp);
				}

				if (acc.Count % 2 == 0)
				{
					inp.Seek(lastPos);
					var ch = inp.Substr(20);
					var outOfRangeIndex = acc.Count / 2 + 1;
					return Parse.Error<IEnumerable<T>>(
						$"Expect {name}, but got '{ch}' outOfRangeIndex:{outOfRangeIndex}", inp);
				}

				return Parse.Success<IEnumerable<T>>(acc);
			});
		}

		public static IParser<IEnumerable<T1>> SeparatedBy<T1, T2>(this IParser<T1> p, IParser<T2> delimited)
			where T1 : T2
		{
			return p.CastParser<T2>().ManyDelimitedBy(delimited)
				.MapResultList(x => x.TakeEvery(1).Cast<T1>());
		}

		public static IParser<T> Named<T>(this IParser<T> p, string name)
		{
			p.Name = name;
			return p;
		}

		public static IParser<T3> Then<T1, T2, T3>(this IParser<T1> p1,
			IParser<T2> p2,
			Func<T1, T2, T3> combine)
		{
			var name = $"{p1.Name} {p2.Name}";
			return new Parser<T3>(name, inp =>
			{
				var pos1 = inp.GetPosition();
				var parsed1 = p1.TryParse(inp);
				if (!parsed1.IsSuccess())
				{
					inp.Seek(pos1);
					return Parse.Error<T3>(parsed1.Error);
				}

				var pos2 = inp.GetPosition();
				var parsed2 = p2.TryParse(inp);
				if (!parsed2.IsSuccess())
				{
					inp.Seek(pos2);
					return Parse.Error<T3>(parsed2.Error);
				}

				var result = combine(parsed1.Result, parsed2.Result);
				return Parse.Success<T3>(result);
			});
		}

		public static IParser<T2> ThenRight<T1, T2>(this IParser<T1> p1, IParser<T2> p2)
		{
			return new ThenRightParser<T1, T2>(p1, p2);
		}

		public static IParser<T1> ThenLeft<T1, T2>(this IParser<T1> p1, IParser<T2> p2)
		{
			return new ThenLeftParser<T1, T2>(p1, p2);
		}

		public static IParser<T> Eos<T>()
		{
			return new EosParser<T>();
		}

		public static IParser<TextSpan> Contains(IEnumerable<string> sortedTexts, bool ignoreCase = false)
		{
			var textsArr = sortedTexts.CastArray();
			var maxLen = textsArr[0].Length;

			var strTexts = string.Join(",", textsArr);

			strTexts = strTexts.Replace("\t", "\\t");
			strTexts = strTexts.Replace("\r", "\\r");
			strTexts = strTexts.Replace("\n", "\\n");
			strTexts = strTexts.Replace(" ", "\\s");

			if (ignoreCase)
			{
				strTexts = $"'{strTexts}'";
			}

			var name = $"[{strTexts}]";

			return new Parser<TextSpan>(name, (inp) =>
			{
				var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
				var maxText = inp.Substr(maxLen);
				var matched = false;
				var consumed = 0;
				for (var i = 0; i < textsArr.Length && !matched; i++)
				{
					matched = maxText.StartsWith(textsArr[i], comparison);
					if (matched)
					{
						consumed = textsArr[i].Length;
					}
				}

				if (!matched)
				{
					return Parse.Error<TextSpan>($"Expect {name}, but got '{maxText}'", inp);
				}

				var span = inp.Consume(consumed);
				return Parse.Success<TextSpan>(span);
			});
		}

		public static IParser<TextSpan> Merge(this IParser<IEnumerable<TextSpan>> parser)
		{
			return parser.Select(x =>
			{
				var resultArr = x.CastArray();
				return new TextSpan()
				{
					File = resultArr[0].File,
					Text = string.Join("", resultArr.Select(x1 => x1.Text)),
					Position = resultArr[0].Position,
					Length = resultArr.Sum(x2 => x2.Length)
				};
			});
		}

		public static readonly IParser<TextSpan> Blank =
			Contains(new[] { " ", "\t", "\r", "\n" }).Named("blank");

		public static readonly IParser<TextSpan> Blanks =
			Blank.Many1().Named("blanks");

		public static readonly IParser<TextSpan> Digit =
			new Parser<TextSpan>("digit", inp =>
			{
				var ch = inp.Substr(1);
				if (ch != string.Empty && char.IsDigit(ch[0]))
				{
					return Parse.Success<TextSpan>(inp.Consume(1));
				}

				return Parse.Error<TextSpan>($"Expect digit, but got '{ch}'", inp);
			});

		public static readonly IParser<TextSpan> Letter =
			new Parser<TextSpan>("letter", inp =>
			{
				var ch = inp.Substr(1);
				if (ch != string.Empty && char.IsLetter(ch[0]))
				{
					return Parse.Success<TextSpan>(inp.Consume(1));
				}

				return Parse.Error<TextSpan>($"Expect letter, but got '{ch}'", inp);
			});

		public static readonly IParser<TextSpan> Digits =
			Digit.Many1().Named("digits");

		public static readonly IParser<TextSpan> Letters =
			Letter.Many1().Named("letters");

		public static readonly IParser<TextSpan> CStyleIdentifier = CStyleIdentifierF().Named(nameof(CStyleIdentifier));

		public static readonly IParser<TextSpan> CStyleString =
			from start1 in Parse.Equal("\"")
			from body1 in Parse.Any(Parse.Equal("\\\""), Parse.Equal("\"").Not().ThenRight(Parse.AnyChars(1))).Many()
			from end1 in Parse.Equal("\"")
			select new[] { start1, body1, end1 }.GetTextSpan();

		public static readonly IParser<TextSpan> NewLine =
			Parse.Any(
				Parse.Equal("\r\n"),
				Parse.Equal("\n")
			);

		private static IParser<TextSpan> CStyleIdentifierF()
		{
			var underscore = Equal("_");
			var body = Any(underscore, Letters, Digits).Many();
			var identifierCharacters1 = Seq(underscore, body);
			var identifierCharacters2 = Seq(Letters, body);
			return identifierCharacters1.Or(identifierCharacters2).Merge();
		}

		public static IParser<T> GroupExpr<T>(IParser<T> lparen, IParser<T> atom, IParser<T> rparen)
		{
			var groupExpr = Parse.Seq(lparen, atom, rparen)
				.MapResultList(x => x[1]);
			return groupExpr.Or(atom);
		}

		public static IParser<T> RecOperatorExpr<T>(IParser<T> item,
			IParser<T>[] operators,
			Func<T[], T> mapResult)
		{
			var expr = item;
			foreach (var oper in operators)
			{
				IParser<T> RecExpr(IParser<T> atom)
				{
					var operandExpr = Seq(atom, oper, atom)
						.MapResultList(mapResult);
					return operandExpr.Or(atom);
				}

				expr = RecExpr(expr);
			}

			return expr.Or(item);
		}

		public static IParser<T> RecGroupOperatorExpr<T>(IParser<T> lparen, IParser<T> atom, IParser<T> rparen,
			IParser<T>[] operators, Func<T[], T> mapBinaryExprResult)
		{
			var expr = RecOperatorExpr(atom, operators, mapBinaryExprResult);
			var groupExpr = GroupExpr(lparen, expr, rparen);
			return RecOperatorExpr(groupExpr, operators, mapBinaryExprResult);
		}

		public static IParser<T> LeftRecursive<T>(this IParser<T> factor,
			params Func<IParser<T>, IParser<T>>[] parsers)
		{
			var curr = (IParser<T>)null;
			foreach (var parser in parsers)
			{
				curr = parser(curr ?? factor);
			}

			return curr;
		}

		private static IParser<T> ChainLeft1<T>(IParser<T> p,
			IParser<T> @operator, IParser<T> operand,
			Func<T, T, T> apply)
		{
			return new Parser<T>("ChainLeft1", inp =>
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}

				var startPos = inp.GetPosition();
				var result = parsed.Result;

				var operatorResult = @operator.TryParse(inp);
				while (operatorResult.IsSuccess())
				{
					var operandResult = operand.TryParse(inp);
					if (!operandResult.IsSuccess())
					{
						inp.Seek(startPos);
						return operandResult;
					}

					result = apply(result, operandResult.Result);

					startPos = inp.GetPosition();
					operatorResult = @operator.TryParse(inp);
				}

				return Parse.Success(result);
			});
		}

		public static IParser<T> ChainLeft<T>(IParser<T> @operator, IParser<T> operand,
			Func<T, T, T> apply)
		{
			return ChainLeft1(operand, @operator, operand, apply);
		}

		public static IEnumerable<TResult> GetAccumResults<TResult>(this IEnumerable<IParseResult<TResult>> accum)
		{
			return accum.Select(x => x.Result);
		}

		public static T Assign<T>(this T result, Action<T> fn)
		{
			fn(result);
			return result;
		}

		public static readonly IParser<TextSpan> CStyleComment2 =
			from start1 in Parse.Equal("/*")
			from body1 in Parse.Equal("*/").Not().ThenRight(Parse.AnyChars(1)).Many()
			from end1 in Parse.Equal("*/")
			select body1;

		public static IParser<IEnumerable<T>> RepeatAny<T>(params IParser<T>[] parsers)
		{
			var name = string.Join(" / ", parsers.Select(x => x.Name));
			return new Parser<IEnumerable<T>>($"({name})*", inp =>
			{
				var isParsedSuccess = false;
				var results = new List<T>();
				do
				{
					var parsedParser = parsers.FirstOrDefault(x =>
					{
						var parsed = x.TryParse(inp);
						if (parsed.IsSuccess())
						{
							results.Add(parsed.Result);
						}
						return parsed.IsSuccess();
					});
					isParsedSuccess = parsedParser != null;
				} while (isParsedSuccess);

				return Parse.Success<IEnumerable<T>>(results);
			});
		}
	}
}