using System;
using System.Collections.Generic;
using System.Linq;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core
{
	public static class ParseCombinator
	{
		public static IParser Assertion(this IParser p, bool isWords)
		{
			return p.Then(Parse.Assertion(isWords));
		}

		public static IParser AtLeastOnce(this IParser parser)
		{
			return parser.Many(1);
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

		public static IParser Many(this IParser p, int min = 0, int max = int.MaxValue)
		{
			var name = $"{p.Name}({min},{max})";

			if (max == int.MaxValue)
			{
				name = $"{p.Name}({min})";
			}

			if (min == 0 && max == 1)
			{
				name = $"{p.Name}?";
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

		public static IParser ManyDelimitedBy(this IParser parser, IParser delimited)
		{
			var tail = delimited.Then(parser);
			//return parser.Then(tail.AtLeastOnce());
			return Parse.Any(parser.Then(tail.AtLeastOnce()), parser);
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

		public static IParser Named(this IParser p, string name)
		{
			p.Name = name;
			return p;
		}

		public static IParser Next(this IParser p1, Func<ITextSpan[], string> p2)
		{
			return new Parser(p1.Name, inp =>
			{
				var parsed = p1.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}
				var error = p2(parsed.Result);
				if (error == string.Empty)
				{
					return parsed;
				}
				return Parse.Error(error, inp);
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

		public static IParser Optional(this IParser p)
		{
			return p.Many(0, 1);
		}
		public static IParser Or(this IParser p1, IParser p2)
		{
			return new[] { p1, p2 }.Choice();
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

		public static IParser RemapResult(this IParser p, Func<IInputReader, IParseResult, IParseResult> remap)
		{
			return new Parser(p.Name, inp =>
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					return parsed;
				}
				return remap(inp, parsed);
			});
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
			return new[] { p1, p2 }.Chain();
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
		public static IParseResult TryParseAllText(this IParser p, string code)
		{
			IInputReader inp = new StringInputReader(code);

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

		public static IParseResult TryParseText(this IParser p, string code)
		{
			IInputReader inp = new StringInputReader(code);
			return p.TryParse(inp);
		}
	}
}