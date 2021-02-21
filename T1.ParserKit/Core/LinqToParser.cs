using System;
using T1.ParserKit.Core.Parsers;

namespace T1.ParserKit.Core
{
	public static class LinqToParser
	{
		public static IParser<TResult> Select<T, TResult>(
			this IParser<T> parser, 
			Func<IParseResult<T>, TResult> mapFunc)
		{
			return new SelectParser<T, TResult>(parser, mapFunc);
		}

		public static IParser<TResult> SelectMany<T, TIntermediate, TResult>(
			this IParser<T> parser,
			Func<T, IParser<TIntermediate>> bindParser,
			Func<T, TIntermediate, TResult> combine)
		{
			return new SelectManyParser<T, TIntermediate, TResult>(parser, bindParser, combine);
		}

		public static IParser<TResult> Where<TResult>(this IParser<TResult> parser, Predicate<TResult> predicate)
		{
			return new PredicateParser<TResult>(parser, predicate);
		}

		public static IParser<TResult> Aggregate<T, TAccum, TResult>(this IParser<T> parser,
			Func<TAccum> seed,
			Func<TAccum, T, TAccum> accFunc,
			Func<TAccum, TResult> resultSelector)
		{
			return new AggregateParser<T, TAccum, TResult>(parser, seed, accFunc, resultSelector);
		}
		
		public static IParser<TAccum> Aggregate<T, TAccum>(
			this IParser<T> parser, 
			Func<TAccum> seed, 
			Func<TAccum, T, TAccum> func)
		{
			return Aggregate(parser, seed, func, x => x);
		}

		public static IParseResult<TResult> Select<T, TResult>(this IParseResult<T> parsed, 
			Func<T, IParseResult<TResult>> func)
		{
			if (parsed.IsSuccess())
			{
				return func(parsed.Result);
			}
			return Parse.Error<TResult>(parsed.Error, parsed.Rest);
		}

		public static IParseResult<TResult> Select<T, TResult>(
			this IParseResult<T> parsed, 
			Func<T, TResult> func)
		{
			return parsed.Select(x => Parse.Success(parsed.TextSpan, func(x), parsed.Rest));
		}

		public static IParseResult<TResult> SelectMany<T1, T2, TResult>(
			this IParseResult<T1> parsed, 
			Func<T1, IParseResult<T2>> func, 
			Func<T1, T2, TResult> select)
		{
			return parsed.Select(x => func(x).Select(
				y => select(x, y))
			);
		}

	}
}