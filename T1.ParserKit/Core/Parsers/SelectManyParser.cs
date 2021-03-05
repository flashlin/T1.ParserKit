using System;

namespace T1.ParserKit.Core.Parsers
{
	public class SelectManyParser<TSourceA, TSourceB, TResult> : IParser<TResult>
	{
		private readonly IParser<TSourceA> _parser;
		private readonly Func<TSourceA, IParser<TSourceB>> _bindParser;
		private readonly Func<TSourceA, TSourceB, TResult> _combineResult;

		public SelectManyParser(IParser<TSourceA> parser, 
			Func<TSourceA, IParser<TSourceB>> bindParser, 
			Func<TSourceA, TSourceB, TResult> combineResult)
		{
			_parser = parser;
			_bindParser = bindParser;
			_combineResult = combineResult;
			Name = $"{parser.Name}*";
		}

		public string Name { get; set; }

		public IParseResult<TResult> TryParse(IInputReader inp)
		{
			return from resultA in _parser.TryParse(inp)
				from resultB in _bindParser(resultA).TryParse(inp)
				select _combineResult(resultA, resultB);
		}
	}
}
