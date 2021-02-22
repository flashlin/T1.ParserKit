namespace T1.ParserKit.Core.Parsers
{
	public class ThenLeftParser<T1, T2> : IParser<T1>
	{
		private readonly IParser<T1> _p1;
		private readonly IParser<T2> _p2;
		public ThenLeftParser(IParser<T1> p1, IParser<T2> p2)
		{
			Name = $"{p1.Name} .>> {p2.Name}";
			_p1 = p1;
			_p2 = p2;
		}

		public string Name { get; set; }
		public IParseResult<T1> TryParse(IInputReader inp)
		{
			var parsed1 = _p1.TryParse(inp);
			if (!parsed1.IsSuccess())
			{
				return parsed1;
			}
			var pos = inp.GetPosition();

			var parsed2 = _p2.TryParse(inp);
			if (!parsed2.IsSuccess())
			{
				return Parse.Error<T1>(parsed1.Error);
			}
			inp.Seek(pos);
			return parsed1;
		}
	}
}