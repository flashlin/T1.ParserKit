using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using T1.ParserKit.Helpers;
using T1.Standard.Common;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core
{
	public static class Parse
	{
		public static IParseResult Success(ITextSpan rest)
		{
			return new ParseResult()
			{
				Result = new ITextSpan[0],
				Rest = rest,
				Error = ParseError.Empty,
			};
		}

		public static IParseResult Success(ITextSpan textSpan, ITextSpan rest)
		{
			return new ParseResult()
			{
				Result = new[] { textSpan },
				Rest = rest,
				Error = ParseError.Empty,
			};
		}

		public static IParseResult Success(IEnumerable<ITextSpan> textSpanList, ITextSpan rest)
		{
			return new ParseResult()
			{
				Result = textSpanList.CastArray(),
				Rest = rest,
				Error = ParseError.Empty,
			};
		}

		public static IParseResult Error(string message, ITextSpan rest)
		{
			return new ParseResult()
			{
				Result = new ITextSpan[0],
				Rest = rest,
				Error = new ParseError()
				{
					Message = message,
					Inp = rest,
					InnerErrors = new ParseError[0]
				}
			};
		}

		public static IParseResult Error(string message, IEnumerable<ParseError> innerErrors, ITextSpan rest)
		{
			return new ParseResult()
			{
				Result = new ITextSpan[0],
				Rest = rest,
				Error = new ParseError()
				{
					Message = message,
					Inp = rest,
					InnerErrors = innerErrors.CastArray()
				}
			};
		}

		public static IParseResult Error(string message, ParseError innerError, ITextSpan rest)
		{
			return Error(message, new[] { innerError }, rest);
		}

		public static IParser Any(params IParser[] parsers)
		{
			return parsers.Choice();
		}

		public static IParser Chain(params IParser[] parsers)
		{
			return parsers.Chain();
		}

		public static IParser Assertion(bool isWords = true)
		{
			return new Parser("assertion", inp =>
			{
				if (inp.Eof())
				{
					return Parse.Success(inp);
				}

				var blank = Blank().TryParse(inp);
				if (blank.IsSuccess())
				{
					return Parse.Success(inp);
				}

				var ch = inp.Substr(1);
				var error = Parse.Error($"Expect assertion, but got '{ch}' at {inp}.", inp);

				var letter = Letter().TryParse(inp);
				if (isWords)
				{
					if (letter.IsSuccess())
					{
						return error;
					}
					return Parse.Success(inp);
				}

				if (letter.IsSuccess())
				{
					return Parse.Success(inp);
				}
				return error;
			});
		}	

		public static IParser Eos()
		{
			return new Parser("eos", inp =>
			{
				if (inp.Eof())
				{
					return Parse.Success(new ITextSpan[0], inp);
				}

				var ch = inp.Substr(20);
				return Parse.Error("Expect EOS, but got '{ch}' at {inp}.", inp);
			});
		}

		public static IParser Equal(string text, bool ignoreCase = false)
		{
			return new Parser($"{text}", (inp) =>
			{
				var ch = inp.Substr(text.Length);
				var ch1 = ignoreCase ? ch.ToLower() : ch;
				var text1 = ignoreCase ? text.ToLower() : text;
				if (text1 == ch1)
				{
					var textSpan = inp.Consume(text.Length);
					return Parse.Success(textSpan, inp.AdvanceBy(text.Length));
				}
				return Parse.Error($"Expect {text}, but got '{ch}' at {inp}.", inp);
			});
		}

		public static IParser NotEqual(string text, bool ignoreCase = false)
		{
			return new Parser($"{text}", (inp) =>
			{
				var ch = inp.Substr(text.Length);
				var ch1 = ignoreCase ? ch.ToLower() : ch;
				var text1 = ignoreCase ? text.ToLower() : text;
				if (text1 != ch1)
				{
					var textSpan = inp.Consume(text.Length);
					return Parse.Success(textSpan, inp.AdvanceBy(text.Length));
				}
				return Parse.Error($"Expect {text}, but got '{ch}' at {inp}.", inp);
			});
		}

		private static string[] SortTexts(IEnumerable<string> texts)
		{
			var textsArr = texts.CastArray();
			var sortedTexts = textsArr.OrderByDescending(x => x.Length).ToArray();
			return sortedTexts;
		}

		public static IParser Contains(IEnumerable<string> texts, bool ignoreCase = false)
		{
			var sortedTexts = SortTexts(texts);
			var maxLen = sortedTexts[0].Length;

			var strTexts = string.Join(",", sortedTexts);
			if (ignoreCase)
			{
				strTexts = $"'{strTexts}'";
			}
			var name = $"[{strTexts}]";

			return new Parser(name, (inp) =>
			{
				var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
				var maxText = inp.Substr(maxLen);
				var matched = false;
				var consumed = 0;
				for (var i = 0; i < sortedTexts.Length && !matched; i++)
				{
					matched = maxText.StartsWith(sortedTexts[i], comparison);
					if (matched)
					{
						consumed = sortedTexts[i].Length;
					}
				}

				if (!matched)
				{
					return Parse.Error($"Expect {name}, but got '{maxText}' at {inp}.", inp);
				}

				var span = inp.Consume(consumed);
				return Parse.Success(span, inp.AdvanceBy(consumed));
			});
		}

		public static IParser Blank()
		{
			var chs = new[] { " ", "\t", "\r", "\n" };
			return Contains(chs).Named("blank");
		}

		public static IParser Blanks()
		{
			return Blank().Many(1).Named("blanks");
		}

		public static IParser Digit()
		{
			return new Parser("digit", inp =>
			{
				var ch = inp.Substr(1);
				if (char.IsDigit(ch[0]))
				{
					return Parse.Success(inp.Consume(1), inp.AdvanceBy(1));
				}

				return Parse.Error($"Expect digit, but got '{ch}' at {inp}.", inp);
			});
		}

		public static IParser Letter()
		{
			return new Parser("letter", inp =>
			{
				var ch = inp.Substr(1);
				if (char.IsLetter(ch[0]))
				{
					return Parse.Success(inp.Consume(1), inp.AdvanceBy(1));
				}

				return Parse.Error($"Expect letter, but got '{ch}' at {inp}.", inp);
			});
		}

		public static IParser Digits()
		{
			return Digit().Many(1).Merge().Named("digits");
		}

		public static IParser Letters()
		{
			return Letter().Many(1).Merge().Named("letters");
		}

		public static IParser CStyleIdentifier()
		{
			var underscore = Equal("_");
			var body = Any(underscore, Letters(), Digits()).Many();

			var identifierCharacters1 =
				underscore.Then(body);

			var identifierCharacters2 = Letters().Then(body);

			return identifierCharacters1.Or(identifierCharacters2).Merge().Named("CStyleIdentifier");
		}

		public static IParser GroupExpr(IParser lparen, IParser atom, IParser rparen)
		{
			var groupExpr = Chain(lparen, atom, rparen)
				.MapResult(x => x[1]);
			return groupExpr.Or(atom);
		}

		public static IParser RecOperatorExpr(IParser item, IParser[] operators, Func<ITextSpan[], ITextSpan> mapResult)
		{
			var expr = item;
			foreach (var oper in operators)
			{
				IParser RecExpr(IParser atom)
				{
					var operandExpr = Chain(atom, oper, atom)
						.MapResult(mapResult);
					return operandExpr.Or(atom);
				}
				expr = RecExpr(expr);
			}
			return expr.Or(item);
		}

		public static IParser RecGroupOperatorExpr(IParser lparen, IParser atom, IParser rparen,
			IParser[] operators, Func<ITextSpan[], ITextSpan> mapResult)
		{
			var expr = RecOperatorExpr(atom, operators, mapResult);
			var groupExpr = GroupExpr(lparen, expr, rparen);
			return RecOperatorExpr(groupExpr, operators, mapResult);
		}

		private static IParser ChainLeft1(IParser p,
			IParser @operator, IParser operand,
			Func<ITextSpan[], ITextSpan[], ITextSpan> apply)
		{
			return new Parser("ChainLeft1", inp =>
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}

				var result = parsed;
				var remainder = parsed.Rest;

				var operatorResult = @operator.TryParse(remainder);
				while (operatorResult.IsSuccess())
				{
					remainder = operatorResult.Rest;

					var operandResult = operand.TryParse(remainder);
					if (!operandResult.IsSuccess())
					{
						return operandResult;
					}

					var newResult = apply(result.Result, operandResult.Result);

					result = Parse.Success(newResult, operatorResult.Rest);

					remainder = operandResult.Rest;
					operatorResult = @operator.TryParse(remainder);
				}

				return result;
			});
		}

		public static IParser ChainLeft(IParser @operator, IParser operand,
			Func<ITextSpan[], ITextSpan[], ITextSpan> apply)
		{
			return ChainLeft1(operand, @operator, operand, apply);
		}

		//public static IParser Ref(IParser reference)
		//{
		//	IParser parser = null;

		//	return new Parser("Ref", inp =>
		//	{
		//		if (parser == null)
		//			parser = reference();

		//		return parser.TryParse(inp);
		//	});
		//}
	}
}