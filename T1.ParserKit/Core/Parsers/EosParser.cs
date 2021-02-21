namespace T1.ParserKit.Core.Parsers
{
	public class EosParser<T> : IParser<T>
	{
		public string Name { get; set; }

		public IParseResult<T> TryParse(IInputReader inp)
		{
			if (inp.Eof())
			{
				return Parse.Success<T>(inp);
			}

			var ch = inp.Substr(20);
			return Parse.Error<T>($"Expected EOS, but got '{ch}' at {inp}.", inp);
		}
	}
}