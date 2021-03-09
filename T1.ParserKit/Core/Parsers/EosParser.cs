namespace T1.ParserKit.Core.Parsers
{
	public class EosParser<T> : IParser<T>
	{
		public string Name { get; set; }

		public IParseResult<T> TryParse(IInputReader inp)
		{
			if (inp.Eof())
			{
				return Parse.Success<T>();
			}

			var ch = inp.Substr(20);
			string message = $"Expected EOS, but got '{ch}' at {inp}.";
			return Parse.Error<T>(() => message, inp.GetPosition());
		}
	}
}