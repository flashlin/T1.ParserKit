using System.Linq;
using T1.Standard.IO;

namespace T1.ParserKit.Core
{
	public struct ParseError
	{
		public static ParseError Empty = new ParseError()
		{
			Message = string.Empty,
			Inp = default,
			InnerErrors = new ParseError[0]
		};

		public ParseError[] InnerErrors { get; set; }

		public string Message { get; set; }

		public IInputReader Inp { get; set; }

		public string GetErrorMessage(int tabs = 0)
		{
			var errorMessage = new IndentStringBuilder
			{
				Indent = tabs
			};

			errorMessage.WriteLine(Message);
			if (InnerErrors.Length > 0)
			{
				errorMessage.Indent++;
				foreach (var innerError in InnerErrors)
				{
					errorMessage.WriteLine(innerError.GetErrorMessage(tabs + 2));
				}
				errorMessage.Indent--;
			}
			return errorMessage.ToString();
		}

		public override string ToString()
		{
			return $"{Message} at {Inp}\r\n" + GetErrorMessage();
		}

		public static bool operator !=(ParseError c1, ParseError c2)
		{
			return !c1.Equals(c2);
		}

		public static bool operator ==(ParseError c1, ParseError c2)
		{
			return c1.Equals(c2);
		}

		public bool Equals(ParseError other)
		{
			return Message == other.Message && Equals(Inp, other.Inp) && Equals(InnerErrors, other.InnerErrors);
		}

		public override bool Equals(object obj)
		{
			return obj is ParseError other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Message != null ? Message.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Inp != null ? Inp.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (InnerErrors != null ? InnerErrors.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}