using System.Linq;

namespace T1.ParserKit.Core
{
	public class ParseResult<T> : IParseResult<T>
	{
		public T Result { get; set; }
		public ParseError Error { get; set; }

		public bool IsSuccess()
		{
			return Error == ParseError.Empty;
		}

		public override string ToString()
		{
			if (IsSuccess())
			{
				return $"{Result}";
			}
			return Error.Message();
		}
	}
}