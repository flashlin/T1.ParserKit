using System;
using System.Collections.Generic;
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
			TextOffset = TextOffset.Empty,
			InnerErrors = new ParseError[0]
		};

		public ParseError[] InnerErrors { get; set; }

		public string Message { get; set; }

		public int Position => TextOffset.Offset;

		public TextOffset TextOffset { get; set; }

		private IEnumerable<ParseError> GetAllErrors()
		{
			return Enumerable.Repeat(this, 1)
				.Concat(InnerErrors.SelectMany(x => x.GetAllErrors()));
		}

		public ParseError GetLastError()
		{
			var allErrors = GetAllErrors().ToArray();

			var lastError = allErrors
				.Aggregate((a, b) => a.Position > b.Position ? a : b);

			var innerErrors = new List<ParseError>();
			innerErrors.Add(lastError);

			foreach (var error in allErrors)
			{
				if (error == lastError)
				{
					continue;
				}

				if (error.Position == lastError.Position)
				{
					innerErrors.Add(error);
				}
			}

			return new ParseError
			{
				Message = nameof(GetLastError),
				TextOffset = TextOffset.Empty,
				InnerErrors = innerErrors.ToArray()
			};
		}

		public string GetErrorMessage(int tabs = 0)
		{
			var errorMessage = new IndentStringBuilder
			{
				Indent = tabs
			};

			var debugText = GetDebugErrorMessage();
			errorMessage.WriteLine(debugText);

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

		private string GetDebugErrorMessage()
		{
			var message = GetErrorPositionMessage();
			var debugText = ToDebugText(message);
			return debugText;
		}

		private string GetErrorPositionMessage()
		{
			var errorPos = TextOffset.GetPosition();
			var rest = TextOffset.Substr(40);
			var message = $"{Message} at {errorPos} rest='{rest}'.";
			return message;
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