using System;

namespace T1.ParserKit.Core
{
	public class SelectParser<TSource, TResult> : IParser<TResult>
	{
		private readonly IParser<TSource> _parser;
		private readonly Func<IParseResult<TSource>, TResult> _mapFunc;

		public SelectParser(IParser<TSource> parser,
			Func<IParseResult<TSource>, TResult> mapFunc)
		{
			_parser = parser;
			_mapFunc = mapFunc;
		}

		public string Name { get; set; }
		public IParseResult<TResult> TryParse(IInputReader inp)
		{
			var result = _parser.TryParse(inp);
			if (!result.IsSuccess())
				return Parse.Error<TResult>(result.Error, inp);

			return Parse.Success(result.TextSpan, _mapFunc(result), result.Rest);

		}
	}
}