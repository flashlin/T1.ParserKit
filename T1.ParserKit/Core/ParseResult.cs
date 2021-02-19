using System.Linq;

namespace T1.ParserKit.Core
{
	public struct ParseResult : IParseResult
	{
		public ITextSpan[] Result { get; set; }
		public ParseError Error { get; set; }
		public IInputReader Rest { get; set; }

		public bool IsSuccess()
		{
			return Error == ParseError.Empty;
		}

		public override string ToString()
		{
			if (IsSuccess())
			{
				return string.Join(" ",Result.Select(x => x.GetText()));
			}
			return Error.Message;
		}
	}
}