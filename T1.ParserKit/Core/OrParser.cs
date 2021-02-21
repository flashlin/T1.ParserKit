namespace T1.ParserKit.Core
{
	public class OrParser<T> : IParser<T>
	{
		private readonly IParser<T> _parserA;
		private readonly IParser<T> _parserB;

		public OrParser(IParser<T> parserA, IParser<T> parserB)
		{
			_parserA = parserA;
			_parserB = parserB;
		}

		public string Name { get; set; }

		public IParseResult<T> TryParse(IInputReader inp)
		{
			var parsed = _parserA.TryParse(inp);
			if (parsed.IsSuccess())
			{
				return parsed;
			}

			return _parserB.TryParse(inp);

		}
	}
}