namespace T1.ParserKit.Core.Parsers
{
	public class ErrorParser<T> : IParser<T>
	{
		private readonly string _errorMessage;

		public ErrorParser(string errorMessage)
		{
			_errorMessage = errorMessage;
		}

		public string Name { get; set; }

		public IParseResult<T> TryParse(IInputReader inp)
		{
			return Parse.Error<T>(_errorMessage, inp);
		}
	}
}