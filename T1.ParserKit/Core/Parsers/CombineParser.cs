using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T1.ParserKit.Core.Parsers
{
	public static class CombineParser
	{
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
			return from x in parser
				from xs in Many(parser)
				select x + xs;
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
			return parsers.Aggregate(Empty<T>("Empty choose sequence"), 
				(acc, p) => acc.Or(p));
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