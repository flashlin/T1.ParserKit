namespace T1.ParserKit.Core.Parsers
{
	public class OrParser<T> : IParser<T>
	{
		private readonly IParser<T> _parserA;
		private readonly IParser<T> _parserB;

		public OrParser(IParser<T> parserA, IParser<T> parserB)
		{
			_parserA = parserA;
			_parserB = parserB;
			Name = $"({_parserA} / {_parserB})";
		}

		public string Name { get; set; }

		public IParseResult<T> TryParse(IInputReader inp)
		{
			var parsed1 = _parserA.TryParse(inp);
			if (parsed1.IsSuccess())
			{
				return parsed1;
			}

			var parsed2 = _parserB.TryParse(inp);
			if (parsed2.IsSuccess())
			{
				return parsed2;
			}

			var ch = inp.Substr(20);
			return Parse.Error<T>($"Expect {Name}, but got '{ch}' at {inp}.", 
				new []{ parsed1.Error, parsed2.Error }, 
				inp.GetPosition());
		}
	}
}