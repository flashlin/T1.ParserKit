using System;
using System.Linq;

namespace T1.ParserKit.Core
{
	public class ParseException : Exception
	{
		public ParseException(ParseError parsedError)
			: base(parsedError.GetErrorMessage())
		{
		}

		public ParseException() : base()
		{
		}

		public ParseException(string message) : base(message)
		{
		}

		public ParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}