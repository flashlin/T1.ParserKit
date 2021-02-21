using System.Linq;

namespace T1.ParserKit.Core
{
	public struct ParseResult<T> : IParseResult<T>
	{
		public ITextSpan TextSpan { get; set; }
		public T Result { get; set; }
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
				return TextSpan.GetText();
			}
			return Error.Message;
		}
	}
}