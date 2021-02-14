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
				Result = new[] {textSpan},
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
			return Error(message, new[] {innerError}, rest);
		}

		public static IParser Choice(this IEnumerable<IParser> parsers)
		{
			var parsersArr = parsers.CastArray();
			var name = 
				"(" +
				string.Join(" / ", parsersArr.Select(x => x.Name)) +
				")";

			return new Parser(name, inp =>
			{
				var errList = new List<ParseError>();
				foreach (var parser in parsersArr)
				{
					var rc = parser.TryParse(inp);
					if (rc.IsSuccess())
					{
						return rc;
					}

					errList.Add(rc.Error);
				}

				return Parse.Error($"Expect {name} at {inp}.", errList, inp);
			});
		}

		public static IParser Any(params IParser[] parsers)
		{
			return Choice(parsers);
		}

		public static IParser Optional(this IParser p)
		{
			return p.Many(0,1);
		}

		public static IParser Many(this IParser p, int min = 0, int max = int.MaxValue)
		{
			var name = $"{p.Name}({min},{max})";

			if (max == int.MaxValue)
			{
				name = $"{p.Name}({min})";
			}

			if (min == 0 && max == int.MaxValue)
			{
				name = $"{p.Name}*";
			}
			
			if (min == 1 && max == int.MaxValue)
			{
				name = $"{p.Name}+";
			}

			return new Parser(name, (inp) =>
			{
				var acc = new List<ITextSpan>();
				var curr = inp;
				var count = 0;
				var ch = inp.Substr(20);
				IParseResult parsed = null;
				while (!curr.Eof())
				{
					parsed = p.TryParse(curr);
					if (!parsed.IsSuccess())
					{
						if (min <= count && count <= max)
						{
							return Parse.Success(acc, parsed.Rest);
						}

						return Parse.Error($"Expect {name}, but got {ch} at {inp}", inp);
					}

					acc.AddRange(parsed.Result);
					curr = parsed.Rest;
					count++;
					if (count == max && count >= min)
					{
						return Parse.Success(acc, parsed.Rest);
					}
				}

				if (min <= count && count <= max)
				{
					return Parse.Success(acc, parsed != null ? parsed.Rest : inp);
				}

				return Parse.Error($"Expect {name}, but got EOF at {inp}", inp);
			});
		}

		public static IParser Named(this IParser p, string name)
		{
			p.Name = name;
			return p;
		}

		public static IParser Skip(this IParser p)
		{
			return new Parser($"~{p.Name}", inp =>
			{
				var parsed = p.TryParse(inp);
				if (parsed.IsSuccess())
				{
					return Parse.Success(new ITextSpan[0], parsed.Rest);
				}

				return parsed;
			});
		}

		public static IParser Then(this IParser p1, IParser p2)
		{
			return Chain(new[] { p1, p2 });
		}

		public static IParser Or(this IParser p1, IParser p2)
		{
			return Any(new[] { p1, p2 });
		}

		public static IParser Chain(params IParser[] parsers)
		{
			return parsers.Chain();
		}

		public static IParser Chain(this IEnumerable<IParser> parsers)
		{
			var parsersArr = parsers.CastArray();
			var name = string.Join(" ", parsersArr.Select(x => x.Name));

			return new Parser(name, inp =>
			{
				var curr = inp;
				var acc = new List<ITextSpan>();
				IParseResult parsed = null;
				foreach (var parser in parsersArr)
				{
					parsed = parser.TryParse(curr);
					if (!parsed.IsSuccess())
					{
						var ch = curr.Substr(20);
						return Parse.Error($"Expect {name}, but got '{ch}' at {curr}.", 
							parsed.Error, parsed.Rest);
					}

					acc.AddRange(parsed.Result);
					curr = parsed.Rest;
				}

				return Parse.Success(acc, parsed.Rest);
			});
		}

		public static IParser Assertion(this IParser p, bool isWords)
		{
			return p.Then(Assertion(isWords));
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

		public static IParser Not(this IParser p)
		{
			var name = $"!{p.Name}";
			return new Parser(name, inp =>
			{
				var parsed = p.TryParse(inp);
				if (parsed.IsSuccess())
				{
					var ch = inp.Substr(20);
					return Parse.Error($"Expect {name}, but got '{ch}' at {inp}.", inp);
				}

				return Parse.Success(inp);
			});
		}

		public static IParser ThenRight(this IParser p1, IParser p2)
		{
			var name = $"{p1.Name}>>.{p2.Name}";
			return new Parser(name, inp =>
			{
				var parsed = p1.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}
				return p2.TryParse(inp);
			});
		}

		public static IParser ThenLeft(this IParser p1, IParser p2)
		{
			var name = $"{p1.Name}.>>{p2.Name}";
			return new Parser(name, inp =>
			{
				var parsed1 = p1.TryParse(inp);
				if (!parsed1.IsSuccess())
				{
					return parsed1;
				}

				var parsed2 = p2.TryParse(inp);
				if (!parsed2.IsSuccess())
				{
					return parsed2;
				}
				return parsed1;
			});
		}

		public static IParser Merge(this IParser p)
		{
			return new Parser(p.Name, inp =>
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}

				if (parsed.Result.Length == 0)
				{
					return parsed;
				}

				var head = parsed.Result.First();
				var tail = parsed.Result.Last();

				var span = new TextSpan()
				{
					File = head.File,
					Content = head.Content,
					Position = head.Position,
					Length = tail.Position + tail.Length - head.Position
				};

				return Parse.Success(span, parsed.Rest);
			});
		}

		public static IParser AtLeastOnce(this IParser parser)
		{
			return parser.Many(1);
		}

		public static IParser ManyDelimitedBy(this IParser parser, IParser delimited)
		{
			var tail = delimited.Then(parser);
			return parser.Then(tail.AtLeastOnce());
		}

		public static IParser LeftRecursive(this IParser factor,
			params Func<IParser, IParser>[] exprs)
		{
			var f = factor;
			foreach (var expr in exprs)
			{
				f = expr(f);
			}
			return f;
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

		public static IParseResult TryParseText(this IParser p, string code)
		{
			ITextSpan inp = new TextSpan()
			{
				File = string.Empty,
				Content = code,
				Length = code.Length,
				Position = 0
			};
			return p.TryParse(inp);
		}

		public static IParseResult TryParseAllText(this IParser p, string code)
		{
			ITextSpan inp = new TextSpan()
			{
				File = string.Empty,
				Content = code,
				Length = code.Length,
				Position = 0
			};

			var acc = new List<ITextSpan>();
			var curr = inp;
			IParseResult parsed = default;
			do
			{
				parsed = p.TryParse(curr);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}

				acc.AddRange(parsed.Result);
				curr = parsed.Rest;
			} while (!curr.Eof());

			return Parse.Success(acc, parsed.Rest);
		}

		public static IParser MapResult(this IParser p, Func<ITextSpan[], ITextSpan> f)
		{
			return new Parser(p.Name, inp =>
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}

				var newResult = f(parsed.Result);
				newResult.CopyTextSpan(parsed.Result);
				return Parse.Success(newResult, parsed.Rest);
			});
		}

		public static IParser MapAssign<T>(this IParser p, Action<ITextSpan[], T> f)
			where T : ITextSpan, new()
		{
			return new Parser(p.Name, inp =>
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}

				var newResult = new T();
				newResult.CopyTextSpan(parsed.Result);
				f(parsed.Result, newResult);
				return Parse.Success(newResult, parsed.Rest);
			});
		}

		public static ITextSpan[] ParseText(this IParser p, string code)
		{
			var parsed = p.TryParseAllText(code);
			if (!parsed.IsSuccess())
			{
				throw new ParseException(parsed.Error);
			}
			return parsed.Result;
		}

		public static ITextSpan GetTextSpan(this string text)
		{
			return new TextSpan()
			{
				File = string.Empty,
				Content = text,
				Position = 0,
				Length = text.Length
			};
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

		public static IParser Contains(string[] texts, bool ignoreCase = false)
		{
			var sortedTexts = texts.OrderByDescending(x=>x.Length).ToArray();
			var maxLen = sortedTexts[0].Length;

			var strTexts = string.Join(",", texts);
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
			var chs = new[] {" ", "\t", "\r", "\n"};
			return Parse.Contains(chs).Named("blank");
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
			var underscore = Parse.Equal("_");
			var body = Parse.Any(underscore, Letters(), Digits()).Many();

			var identifierCharacters1 =
				underscore.Then(body);

			var identifierCharacters2 = Letters().Then(body);

			return identifierCharacters1.Or(identifierCharacters2).Merge().Named("CStyleIdentifier");
		}
	}
}