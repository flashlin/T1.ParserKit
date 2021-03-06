﻿using System;

namespace T1.ParserKit.Core.Parsers
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
			Name = $"{parser.Name}";
		}

		public string Name { get; set; }
		public IParseResult<TResult> TryParse(IInputReader inp)
		{
			var result = _parser.TryParse(inp);
			if (!result.IsSuccess())
			{
				return Parse.Error<TResult>(result.Error);
			}
			return Parse.Success(_mapFunc(result));
		}
	}
}