﻿
namespace T1.ParserKit.Core.Parsers
{
	public class NotParser<T> : IParser<Unit>
	{
		private readonly IParser<T> _parser;

		public NotParser(IParser<T> parser)
		{
			_parser = parser;
		}

		public string Name { get; set; }

		public IParseResult<Unit> TryParse(IInputReader inp)
		{
			var result = _parser.TryParse(inp);
			if (result.IsSuccess())
				return Parse.Error<Unit>($"Expect {_parser.Name}, but got '{result.Result}' at {inp}.", inp);

			return Parse.Success(inp.Consume(0), Unit.Instance, inp);
		}
	}
}