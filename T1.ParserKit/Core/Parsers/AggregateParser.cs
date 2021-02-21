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
		}

		public string Name { get; set; }

		public IParseResult<TResult> TryParse(IInputReader inp)
		{
			TAccum acc = _seed();

			var position = inp.GetPosition();
			IParseResult<T> lastParsed = null;
			IParseResult<T> parsed = null;
			var curr = inp;

			do
			{
				parsed = _parser.TryParse(curr);
				if (!parsed.IsSuccess())
				{
					break;
				}
				lastParsed = parsed;
				acc = _accFunc(acc, parsed.Result);
				curr = parsed.Rest;
			} while (true);

			if (lastParsed == null)
			{
				return Parse.Success(TextSpan.Empty, _resultSelector(acc), parsed.Rest);
			}

			var textSpan = GetParsedTextSpan(inp, lastParsed);
			return Parse.Success(textSpan, _resultSelector(acc), parsed.Rest);
		}

		private static TextSpan GetParsedTextSpan(IInputReader inp, IParseResult<T> parsed)
		{
			var consumed = parsed.TextSpan.Position + parsed.TextSpan.Length - inp.GetPosition();

			var textSpan = new TextSpan
			{
				File = parsed.TextSpan.File,
				Content = inp.Substr(consumed),
				Position = inp.GetPosition(),
				Length = consumed
			};
			return textSpan;
		}
	}
}
	