using System;

namespace T1.ParserKit.Core.Parsers
{
	public class PredicateParser<T> : IParser<T>
	{
		private readonly IParser<T> _parser;

		public PredicateParser(IParser<T> parser, Predicate<T> predicate)
		{
			_parser = parser;
			Predicate = predicate;
		}

		public Predicate<T> Predicate { get; set; }

		public string Name { get; set; }

		IParseResult<T> IParser<T>.TryParse(IInputReader inp)
		{
			var result = _parser.TryParse(inp);
			if (!result.IsSuccess())
				return result;

			T resultValue = result.Result;
			if (Predicate(resultValue))
				return result;

			var ch = inp.Substr(20);
			return Parse.Error<T>($"Expect {resultValue}, but got '{ch}' at {inp}.", inp);
		}
	}
}
