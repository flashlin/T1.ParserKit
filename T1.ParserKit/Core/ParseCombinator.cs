using System.Collections.Generic;
using System.Linq;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core
{
	public static class ParseCombinator
	{
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

		public static IParser Optional(this IParser p)
		{
			return p.Many(0, 1);
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
	}
}