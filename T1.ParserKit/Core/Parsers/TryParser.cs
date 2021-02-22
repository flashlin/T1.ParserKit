namespace T1.ParserKit.Core.Parsers
{
	public class TryParser<T> : IParser<T>
	{
		private readonly IParser<T> _parser;

		public TryParser(IParser<T> parser)
		{
			Name = $"{parser.Name}\\?";
			_parser = parser;
		}

		public string Name { get; set; }

		public IParseResult<T> TryParse(IInputReader inp)
		{
			var pos = inp.GetPosition();
			var parsed = _parser.TryParse(inp);
			if (!parsed.IsSuccess())
			{
				return Parse.Error<T>(parsed.Error);
			}
			inp.Seek(pos);
			return Parse.Success<T>();
		}
	}
}