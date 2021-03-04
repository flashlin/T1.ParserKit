using System;
using System.Linq;
using System.Text.RegularExpressions;
using T1.Standard.IO;

namespace T1.ParserKit.Core
{
	public class ParseError
	{
		public static ParseError Empty = new ParseError()
		{
			Message = string.Empty,
			Position = -1,
			InnerErrors = new ParseError[0]
		};

		public ParseError[] InnerErrors { get; set; }

		public string Message { get; set; }

		public int Position { get; set; }

		public string GetErrorMessage(int tabs = 0)
		{
			var errorMessage = new IndentStringBuilder
			{
				Indent = tabs
			};

			errorMessage.WriteLine(ToDebugText(Message));
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

		private string ToDebugText(string message)
		{
			var text = message.Replace("\r", "\\r");
			text = text.Replace("\n", "\\n");
			text = text.Replace("\t", "\\t");
			return text;
		}

		public override string ToString()
		{
			if (Message == string.Empty)
			{
				return string.Empty;
			}
			return $"{Message} at {Position}\r\n" + GetErrorMessage();
		}
	}
}