using System;
using System.Collections.Generic;
using System.Linq;
using T1.ParserKit.Core.Parsers;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core
{
	public static class ParserExtension
	{
		//public static IParser<T2> CastParser<T1, T2>(this IParser<T1> p)
		//	where T1 : T2
		//{
		//	var name = p.Name;
		//	return new Parser<T2>(name, inp =>
		//	{
		//		var parsed = p.TryParse(inp);
		//		if (!parsed.IsSuccess())
		//		{
		//			return Parse.Error<T2>(parsed.Error);
		//		}

		//		return Parse.Success<T2>(parsed.Result);
		//	});
		//}

		public static IParseResult<T> ParseText<T>(this IParser<T> p, string code)
		{
			var parsed = p.TryParseText(code);
			if (!parsed.IsSuccess())
			{
				throw new ParseException(parsed.Error);
			}
			return parsed;
		}

		public static IParseResult<T>[] ParseAllText<T>(this IParser<T> p, string code)
		{
			var result = p.TryParseAllText(code).ToArray();
			var parsed = result.Last();
			if (!parsed.IsSuccess())
			{
				throw new ParseException(parsed.Error);
			}
			return result;
		}

		public static IParseResult<object> CastToParseResult<T>(this IParseResult<T> parsed)
		{
			return new ParseResult<object>()
			{
				Result = parsed.Result,
				Error = parsed.Error,
			};
		}

		//public static IParser<T2> RemapResult<T, T2>(this IParser<T> p,
		//	Func<IInputReader, IParseResult<T>, IParseResult<T2>> remap)
		//{
		//	return new Parser<T2>(p.Name, inp =>
		//	{
		//		var parsed = p.TryParse(inp);
		//		if (!parsed.IsSuccess())
		//		{
		//			return Parse.Error<T2>(parsed.Error, inp);
		//		}
		//		return remap(inp, parsed);
		//	});
		//}

		//public static IParser<T> Skip<T>(this IParser<T> p)
		//{
		//	return new Parser<T>($"~{p.Name}", inp =>
		//	{
		//		var parsed = p.TryParse(inp);
		//		if (parsed.IsSuccess())
		//		{
		//			return Parse.Success<T>(parsed.Rest);
		//		}
		//		return parsed;
		//	});
		//}

		//public static IParser<T> Then<T>(this IParser<T> p1, IParser<T> p2,
		//	Func<IEnumerable<IParseResult<T>>, T> apply)
		//{
		//	var name = $"{p1.Name}.>>{p2.Name}";
		//	return new Parser<T>(name, inp =>
		//	{
		//		var parsed1 = p1.TryParse(inp);
		//		if (!parsed1.IsSuccess())
		//		{
		//			return parsed1;
		//		}

		//		var parsed2 = p2.TryParse(inp);
		//		if (!parsed2.IsSuccess())
		//		{
		//			return parsed2;
		//		}

		//		return GetAccumResult()
		//		return apply(new[] { parsed1, parsed2 });
		//	});
		//}

		//public static IParser<T> ThenLeft<T>(this IParser<T> p1, IParser<T> p2)
		//{
		//	var name = $"{p1.Name}.>>{p2.Name}";
		//	return new Parser<T>(name, inp =>
		//	{
		//		var parsed1 = p1.TryParse(inp);
		//		if (!parsed1.IsSuccess())
		//		{
		//			return parsed1;
		//		}

		//		var parsed2 = p2.TryParse(inp);
		//		if (!parsed2.IsSuccess())
		//		{
		//			return parsed2;
		//		}
		//		return parsed1;
		//	});
		//}

		public static IEnumerable<IParseResult<T>> TryParseAllText<T>(this IParser<T> p, string code)
		{
			IInputReader inp = new CacheStringReader(new StringInputReader(code));
			do
			{
				var parsed = p.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					yield return parsed;
					break;
				}
			} while (!inp.Eof());
		}

		public static IParseResult<T> TryParseText<T>(this IParser<T> p, string code)
		{
			IInputReader inp = new CacheStringReader(new StringInputReader(code));
			return p.TryParse(inp);
		}
	}
}