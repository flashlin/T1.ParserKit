namespace T1.ParserKit.Core.Parsers
{
	public class OptionalParser<T> : IParser<T>
	{
		private readonly IParser<T> _parser;

		public OptionalParser(IParser<T> parser)
		{
			Name = $"~{parser.Name}";
			_parser = parser;
		}

		public string Name { get; set; }

		public IParseResult<T> TryParse(IInputReader inp)
		{
			var parsed = _parser.TryParse(inp);
			if (!parsed.IsSuccess())
			{
				return Parse.Success<T>(inp);
			}

			return parsed;
		}
	}
}