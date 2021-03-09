using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T1.Standard.DynamicCode;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core.Parsers
{
	public static class CombineParser
	{
		public static IParser<T> CastParser<T>(this object parser)
		{
			var parserType = parser.GetType();

			var nameGetter = DynamicProperty.GetProperty(parserType, nameof(Parser<object>.Name));
			var tryParseMethod = parserType.GetMethod(nameof(Parser<object>.TryParse));
			var tryParse = DynamicMethod.GetMethod(tryParseMethod);
			
			var parseResult = tryParseMethod.ReturnType;
			var isSuccess = DynamicMethod.GetMethod(parseResult,
				nameof(IParseResult<object>.IsSuccess));
			var errorGetter = DynamicProperty.GetProperty(parseResult, nameof(IParseResult<object>.Error));
			var resultGetter = DynamicProperty.GetProperty(parseResult, nameof(IParseResult<object>.Result));
			var name = (string)nameGetter(parser);
			return new Parser<T>(name, inp =>
			{
				var parsed = tryParse(parser, new object[] { inp });
				if (!(bool)isSuccess(parsed, new object[0]))
				{
					var error = (ParseError)errorGetter(parsed);
					return Parse.Error<T>(error);
				}

				var result = resultGetter(parsed);
				return Parse.Success<T>(result);
			});
		}

		public static IParser<T> MapParser<T>(this IEnumerable<object> parsers, 
			Func<IParser<T>[], IParser<T>> map)
		{
			var parsersArr = parsers.CastArray();
			var parsersTArr = new IParser<T>[parsersArr.Length];
			foreach (var p in parsersArr.SelectWithIndex())
			{
				parsersTArr[p.index] = p.value.CastParser<T>();
			}
			return map(parsersTArr);
		}

		public static IParser<IEnumerable<T>> MapParser<T>(this IEnumerable<object> parsers,
			Func<IParser<T>[], IParser<IEnumerable<T>>> map)
		{
			var parsersArr = parsers.CastArray();
			var parsersTArr = new IParser<T>[parsersArr.Length];
			foreach (var p in parsersArr.SelectWithIndex())
			{
				parsersTArr[p.index] = p.value.CastParser<T>();
			}
			return map(parsersTArr);
		}

		public static IParser<IEnumerable<T>> MapParser<T>(this IEnumerable<object> parsers,
			Func<IParser<TextSpan>, IParser<T>> mapParser,
			Func<IParser<T>[], IParser<IEnumerable<T>>> mapParsers)
		{
			var parsersArr = parsers.CastArray();
			var parsersTArr = new IParser<T>[parsersArr.Length];
			foreach (var p in parsersArr.SelectWithIndex())
			{
				var parserType = p.value.GetType();

				if (parserType.IsGenericType && parserType.GetGenericArguments()[0] == typeof(TextSpan))
				{
					parsersTArr[p.index] = mapParser((IParser<TextSpan>)p.value);
				}
				else
				{
					parsersTArr[p.index] = p.value.CastParser<T>();
				}
			}
			return mapParsers(parsersTArr);
		}

		public static IParser<T> AnyCastParser<T>(this IEnumerable<object> parsers)
		{
			return parsers.MapParser<T>(x => x.Any());
		}

		public static IParser<TextSpan> Many(this IParser<TextSpan> parser)
		{
			TextSpan Seed() => TextSpan.Empty;
			TextSpan AccFunc(TextSpan builder, TextSpan c)
			{
				return builder + c;
			}
			TextSpan ResultSelector(TextSpan builder) => builder;
			return parser.Aggregate(Seed, AccFunc, ResultSelector);
		}

		public static IParser<TextSpan> Many1(this IParser<TextSpan> parser)
		{
			var name = $"{parser.Name}+";
			return (from x in parser
					  from xs in Many(parser)
					  select x + xs).Named(name);
		}

		public static IParser<IEnumerable<T>> Many<T>(this IParser<T> parser)
		{
			return parser.Aggregate(Enumerable.Empty<T>, Concat, x => x);
		}

		public static IParser<IEnumerable<T>> Many1<T>(this IParser<T> parser)
		{
			return from x in parser
					 from xs in Many(parser)
					 select new[] { x }.Concat(xs);
		}

		public static IParser<IEnumerable<TValue>> Repeat<TValue>(this IParser<TValue> parser, int repeatCount)
		{
			return Sequence(Enumerable.Repeat(parser, repeatCount));
		}

		private static IEnumerable<T> Concat<T>(IEnumerable<T> xs, T y)
		{
			return xs.Concat(Enumerable.Repeat(y, 1));
		}

		public static IParser<IEnumerable<T>> Sequence<T>(this IEnumerable<IParser<T>> parsers)
		{
			return new SequenceParser<T>(parsers);
		}

		public static IParser<T> Chain<T>(this IEnumerable<IParser<T>> parsers)
		{
			return new ChainParser<T>(parsers);
		}

		public static IParser<T> Any<T>(this IEnumerable<IParser<T>> parsers)
		{
			var parserArr = parsers.CastArray();
			var name = string.Join(" / ", parserArr.Select(x => x.Name));
			return new Parser<T>($"({name})", inp =>
			{
				var acc = new List<ParseError>();
				foreach (var parser in parserArr)
				{
					var pos = inp.GetPosition();
					var parsed = parser.TryParse(inp);
					if (parsed.IsSuccess())
					{
						return parsed;
					}
					inp.Seek(pos);
					acc.Add(parsed.Error);
				}
				var ch = inp.Substr(20);
				return Parse.Error<T>($"Expect ({name}), but got '{ch}'", acc, inp);
			});
		}

		private static IParser<T> Empty<T>(string errorMessage)
		{
			return new ErrorParser<T>(errorMessage);
		}

		public static IParser<T> Or<T>(this IParser<T> parserA, IParser<T> parserB)
		{
			return new OrParser<T>(parserA, parserB);
		}
	}
}