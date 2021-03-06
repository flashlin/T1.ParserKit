using System;

namespace T1.ParserKit.Core.Parsers
{
	public class AggregateParser<T, TAccum, TResult> : IParser<TResult>
	{
		private readonly IParser<T> _parser;
		private readonly Func<TAccum> _seed;
		private readonly Func<TAccum, T, TAccum> _accFunc;
		private readonly Func<TAccum, TResult> _resultSelector;

		public AggregateParser(IParser<T> parser,
			Func<TAccum> seed,
			Func<TAccum, T, TAccum> accFunc,
			Func<TAccum, TResult> resultSelector)
		{
			_parser = parser;
			_seed = seed;
			_accFunc = accFunc;
			_resultSelector = resultSelector;
			Name = $"{_parser.Name}";
		}

		public string Name { get; set; }

		public IParseResult<TResult> TryParse(IInputReader inp)
		{
			TAccum acc = _seed();
			do
			{
				var parsed = _parser.TryParse(inp);
				if (!parsed.IsSuccess())
				{
					break;
				}
				acc = _accFunc(acc, parsed.Result);
			} while (true);

			return Parse.Success(_resultSelector(acc));
		}
	}
}
	