using System.Collections.Generic;
using T1.Standard.Extensions;

namespace T1.ParserKit.Core.Parsers
{
	public class SequenceParser<T> : IParser<IEnumerable<T>>
	{
		private readonly IParser<T>[] _parsers;

		public SequenceParser(IEnumerable<IParser<T>> parsers)
		{
			_parsers = parsers.CastArray();
		}

		public string Name { get; set; }

		public IParseResult<IEnumerable<T>> TryParse(IInputReader inp)
		{
			var acc = new List<IParseResult<T>>(_parsers.Length);
			var curr = inp;

			IParseResult<T> parsed = null;
			foreach (var parser in _parsers)
			{
				parsed = parser.TryParse(curr);
				if (!parsed.IsSuccess())
				{
					return Parse.Error<IEnumerable<T>>(parsed.Error, curr);
				}
				acc.Add(parsed);
				curr = parsed.Rest;
			}

			var textSpan = acc.GetAccumTextSpan();
			return Parse.Success(textSpan, acc.GetAccumResults(), parsed.Rest);
		}
	}
}